using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MizanOriginalSoft.MainClasses.OriginalClasses;

using MizanOriginalSoft.MainClasses.Enums;
using MizanOriginalSoft.MainClasses; // هنا يوجد enum InvoiceType

namespace MizanOriginalSoft.Views.Forms.Movments
{


    public partial class frm_DynamicInvoice : Form
    {
        #region Fields
        private InvoiceType currentInvoiceType; // نوع الفاتورة الحالية
        #endregion

        #region Form Initialization
        public frm_DynamicInvoice()
        {
            InitializeComponent();
        }

        public void InitializeInvoice(InvoiceType type)
        {
            currentInvoiceType = type;

            // ضبط عنوان الفورم
            this.Text = type switch
            {
                InvoiceType.Sale => "فاتورة بيع",
                InvoiceType.SaleReturn => "فاتورة بيع مرتد",
                InvoiceType.Purchase => "فاتورة شراء",
                InvoiceType.PurchaseReturn => "فاتورة شراء مرتد",
                InvoiceType.Inventory => "إذن تسوية مخزن",
                InvoiceType.DeductStock => "إذن خصم مخزن",
                InvoiceType.AddStock => "إذن إضافة مخزن",
                _ => "فاتورة"
            };

            // تعبئة الحقول
            FillDefaultAccount();              // txtAccName + lblAccID
            ConfigureAutoCompleteForAccount(); // AutoComplete
            FillSellerComboBox();              // cbxSellerID
            SetupFormByInvoiceType();          // إعدادات أخرى
        }
        #endregion

        #region Default Account
        private void FillDefaultAccount()
        {
            string invoiceTypeKey = InvoiceTypeHelper.ToAccountTypeString(currentInvoiceType);

            if (string.IsNullOrEmpty(invoiceTypeKey))
                return;

            DataTable dt = DBServiecs.NewInvoice_GetAcc(invoiceTypeKey);

            // 🔥 تحديد الحساب الافتراضي حسب نوع الفاتورة
            int defaultAccID = currentInvoiceType switch
            {
                InvoiceType.Sale or InvoiceType.SaleReturn => 55, // عميل نقدي
                InvoiceType.Purchase or InvoiceType.PurchaseReturn => 56, // مورد نقدي
                _ => -1
            };

            if (defaultAccID != -1)
            {
                // 🔍 البحث عن الحساب في الجدول
                DataRow[] rows = dt.Select($"AccID = {defaultAccID}");
                if (rows.Length > 0)
                {
                    lblAccID.Text = rows[0]["AccID"].ToString();
                    txtAccName.Text = rows[0]["AccName"].ToString();
                    return;
                }
            }

            // 📌 لو الحساب الافتراضي غير موجود نرجع لأول صف
            if (dt.Rows.Count > 0)
            {
                lblAccID.Text = dt.Rows[0]["AccID"].ToString();
                txtAccName.Text = dt.Rows[0]["AccName"].ToString();
            }
            else
            {
                lblAccID.Text = "0";
                txtAccName.Text = string.Empty;
            }
        }
        #endregion

        #region AutoComplete Configuration
        private void ConfigureAutoCompleteForAccount()
        {
            txtAccName.AutoCompleteCustomSource.Clear();
            string invoiceTypeKey = InvoiceTypeHelper.ToAccountTypeString(currentInvoiceType);

            if (!string.IsNullOrEmpty(invoiceTypeKey))
            {
                DataTable dt = DBServiecs.NewInvoice_GetAcc(invoiceTypeKey);
                var names = dt.AsEnumerable()
                              .Select(r => r.Field<string?>("AccName") ?? string.Empty)
                              .ToArray();

                txtAccName.AutoCompleteCustomSource.AddRange(names);
            }

            txtAccName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtAccName.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }
        #endregion

        #region Seller ComboBox
        private void FillSellerComboBox()
        {
            string sellerKey = InvoiceTypeHelper.ToAccountTypeString(currentInvoiceType, forSeller: true);

            if (string.IsNullOrEmpty(sellerKey))
            {
                cbxSellerID.DataSource = null;
                return;
            }

            DataTable dt = DBServiecs.NewInvoice_GetAcc(sellerKey);

            cbxSellerID.DataSource = dt;
            cbxSellerID.DisplayMember = "AccName";
            cbxSellerID.ValueMember = "AccID";

            // 🔥 حدد الحساب الافتراضي حسب نوع الفاتورة
            int defaultAccID = currentInvoiceType switch
            {
                InvoiceType.Sale or InvoiceType.SaleReturn => 57, // ادارة البائعين
                InvoiceType.Purchase or InvoiceType.PurchaseReturn => 61, // ادارة المشتريات
                _ => -1 // لا يوجد حساب افتراضي
            };

            // 🔍 البحث عن الصف الذي يحتوي على الحساب الافتراضي
            if (defaultAccID != -1)
            {
                DataRow[] rows = dt.Select($"AccID = {defaultAccID}");
                if (rows.Length > 0)
                {
                    cbxSellerID.SelectedValue = defaultAccID;
                }
                else if (dt.Rows.Count > 0)
                {
                    cbxSellerID.SelectedIndex = 0; // fallback لو الحساب الافتراضي غير موجود
                }
            }
            else if (dt.Rows.Count > 0)
            {
                cbxSellerID.SelectedIndex = 0;
            }
        }
        #endregion


        #region Form Setup by Invoice Type
        private void SetupFormByInvoiceType()
        {
            switch (currentInvoiceType)
            {
                case InvoiceType.Inventory:
                case InvoiceType.DeductStock:
                case InvoiceType.AddStock:
                    // تعطيل أو تمكين بعض الحقول الخاصة بالمبيعات/المشتريات
                    txtAccName.Enabled = false;
                    break;

                default:
                    txtAccName.Enabled = true;
                    break;
            }

            // إعداد DataGridView أو أي عناصر أخرى
        }
        #endregion

        /*
         هذا اجراء جلب البيانات التى من المفترض تعبئ txtAccName و lblAccID , cbxSellerID 

        ALTER PROCEDURE [dbo].[NewInvoice_GetAcc]
    @InvoiceType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IDs NVARCHAR(MAX);

    -- جدول التحويل
    DECLARE @Mapping TABLE (TypeName NVARCHAR(50), IDs NVARCHAR(MAX));
    INSERT INTO @Mapping VALUES
        ('SalesMen', '60'),      --البائعون
        ('PurchaseMen', '70'),   -- مسؤولى الشراء
        ('Sale', '3,8,40'),      -- عملاء البيع والبيع المرتد
        ('Purchase', '4,40'),    -- موردين الشراء والشراء المرتد
        ('Inventory', '30');     -- حسابات ضبط المخزون

    SELECT @IDs = IDs FROM @Mapping WHERE TypeName = @InvoiceType;

    IF @IDs IS NULL OR @IDs = N''
    BEGIN
        SELECT TOP 0 * FROM dbo.MainAccounts;
        RETURN;
    END

    ;WITH StartAccs AS (
        SELECT TRY_CAST(value AS INT) AS AccID
        FROM STRING_SPLIT(@IDs, ',')
        WHERE TRY_CAST(value AS INT) IS NOT NULL
    ),
    RecursiveAccs AS (
        SELECT MA.AccID
        FROM dbo.MainAccounts MA
        INNER JOIN StartAccs SA ON MA.ParentAccID = SA.AccID
        UNION ALL
        SELECT MA.AccID
        FROM dbo.MainAccounts MA
        INNER JOIN RecursiveAccs R ON MA.ParentAccID = R.AccID
    )
    SELECT 
        MA.AccID,
        MA.AccName,
        (MA.AccName + ' / ' + ISNULL(ParentAcc.AccName, 'بدون')) AS FullAccName,
        MA.Balance,
        MA.BalanceState,
        MA.FirstPhon,
        MA.AntherPhon,
        MA.AccNote,
        MA.ClientEmail,
        MA.ClientAddress
    FROM dbo.MainAccounts MA
    LEFT JOIN dbo.MainAccounts ParentAcc ON MA.ParentAccID = ParentAcc.AccID
    WHERE MA.IsFinalAccount = 1
      AND MA.IsHidden = 0
      AND (MA.AccID IN (SELECT AccID FROM RecursiveAccs)
           OR MA.ParentAccID IN (SELECT AccID FROM RecursiveAccs));
END

/*
خاص بتعبئة الكمبو بكس cbxSellerID --------------------
فى حالة فاتورة البيع او مردوداته يتم تعبئة الكمبوبكس بهذة الحسابات مع مراعات الافتراضى cbxSellerID
EXEC dbo.NewInvoice_GetAcc @InvoiceType = N'SalesMen';     -- الحساب الافتراضى للفاتورة الجديدة = 57 ادارة البائعين

فى حالة فاتورة الشراء او مردوداته يتم تعبئة الكمبوبكس بهذة الحسابات مع مراعات الافتراضى cbxSellerID
EXEC dbo.NewInvoice_GetAcc @InvoiceType = N'PurchaseMen';  -- الحساب الافتراضى للفاتورة الجديدة = 228 الادارة العليا
===================================================================
===================================================================
حاص بتعبة الحساب المرتبط بالفاتورة txtAccName و lblAccID-----------------------
فى حالة فاتورة البيع او مردوداته يتم تعبئة التكست بكس  والليبل بهذة الحسابات مع مراعات الافتراضى txtAccName و lblAccID
EXEC dbo.NewInvoice_GetAcc @InvoiceType = N'Sale';       -- الحساب الافتراضى للفاتورة الجديدة = 55 عميل نقدى

فى حالة فاتورة الشراء او مردوداته يتم تعبئة التكست بكس  والليبل بهذة الحسابات مع مراعات الافتراضى txtAccName و lblAccID
EXEC dbo.NewInvoice_GetAcc @InvoiceType = N'Purchase';   -- الحساب الافتراضى للفاتورة الجديدة = 56 مورد عام نقدى

فى حالة فاتورة ضبط المخزون يتم تعبئة التكست بكس  والليبل بهذة الحسابات مع مراعات الافتراضى txtAccName و lblAccID
EXEC dbo.NewInvoice_GetAcc @InvoiceType = N'Inventory';  -- الحساب الافتراضى للفاتورة الجديدة = 72 حساب تسوية رصيد

*/
       




    }
}

















/*
 لدى الان هذه الفاتورة واريد ان تكون اسمارت افتحها بازرار مختلفة مثل 
 //btnSales_Click او btnBackSales او btnPrococh او btnBackPrococh
حسب نوع الاداء الذى يريده المستخدم
ويوجد زر اخر خاص بالجرد باسم سانشء له شاشة اخرى مناسبة له 

فاكتب لى كود يستخدم الكلاسين 
using System;

namespace MizanOriginalSoft.MainClasses.Enums
{
    /// <summary>
    /// أنواع الفواتير المستخدمة في النظام
    /// </summary>
    public enum InvoiceType
    {
        Sale = 1,            // فاتورة بيع
        SaleReturn = 2,      // فاتورة بيع مرتد
        Purchase = 3,        // فاتورة شراء
        PurchaseReturn = 4,  // فاتورة شراء مرتد
        Inventory = 5,       // إذن تسوية مخزن
        DeductStock = 6,     // إذن خصم مخزن
        AddStock = 7         // إذن إضافة مخزن
    }
}
والكلاس
using System;
using MizanOriginalSoft.MainClasses.Enums; // 👈 أضف هذا السطر

namespace MizanOriginalSoft.MainClasses.OriginalClasses
{
    public static class InvoiceTypeHelper
    {
        public static string ToAccountTypeString(InvoiceType type)
        {
            return type switch
            {
                InvoiceType.Sale or InvoiceType.SaleReturn => "SalesMen",
                InvoiceType.Purchase or InvoiceType.PurchaseReturn => "PurchaseMen",
                InvoiceType.Inventory or InvoiceType.DeductStock or InvoiceType.AddStock => "Inventory",
                _ => string.Empty
            };
        }
    }
}


واستطيع فيما بعد تعبءة الحساب الافتراضى لكل فاتورة عميل نقدى رقمه 55 او مورد نقدى رقمه 56  فى ليبل lblAccID
وبناء عليه يتم تعبئة التكست بكس txtAccName باسم هذا الحساب

مع العلم ان txtAccName به تعبئة تلقائية بالعملاء او الموردين حسب نوع الفاتورة  التى ستفتح

فما هو السينارية العام لضبط هذه الفاتورة الجديدة والاستغناء عن القديمة بتنسيق وترتيب محترف داخل ريجونز لسهولة المراجعة



 */