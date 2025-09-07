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

using MizanOriginalSoft.MainClasses.Enums; // هنا يوجد enum InvoiceType

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

            // ضبط عنوان الفورم حسب نوع الفاتورة
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

            // تعبئة الحساب الافتراضي بناء على نوع الفاتورة
            FillDefaultAccount();

            // ضبط تكست بكس العميل/المورد
            ConfigureAutoCompleteForAccount();

            // إعدادات إضافية للفورم (مثل تمكين/تعطيل حقول، جداول، ...)
            SetupFormByInvoiceType();
        }
        #endregion

        #region Default Account
        private void FillDefaultAccount()
        {
            int defaultAccID = currentInvoiceType switch
            {
                InvoiceType.Sale or InvoiceType.SaleReturn => 55,     // عميل نقدى
                InvoiceType.Purchase or InvoiceType.PurchaseReturn => 56, // مورد نقدى
                _ => 0 // لا يوجد
            };

            lblAccID.Text = defaultAccID.ToString();

            if (defaultAccID != 0)
            {
                // يمكنك استدعاء دالة لجلب الاسم من قاعدة البيانات
                txtAccName.Text = GetAccountNameByID(defaultAccID);
            }
        }

        private string GetAccountNameByID(int accID)
        {
            // هنا ضع كود جلب الاسم من قاعدة البيانات
            return accID switch
            {
                55 => "عميل نقدى",
                56 => "مورد نقدى",
                _ => string.Empty
            };
        }
        #endregion

        #region AutoComplete Configuration
        private void ConfigureAutoCompleteForAccount()
        {
            // مسح المحتويات القديمة
            txtAccName.AutoCompleteCustomSource.Clear();

            if (currentInvoiceType == InvoiceType.Sale || currentInvoiceType == InvoiceType.SaleReturn)
            {
                // إضافة العملاء
                txtAccName.AutoCompleteCustomSource.AddRange(GetCustomerNames());
            }
            else if (currentInvoiceType == InvoiceType.Purchase || currentInvoiceType == InvoiceType.PurchaseReturn)
            {
                // إضافة الموردين
                txtAccName.AutoCompleteCustomSource.AddRange(GetSupplierNames());
            }

            txtAccName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtAccName.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }

        private string[] GetCustomerNames()
        {
            // جلب أسماء العملاء من قاعدة البيانات
            return new string[] { "عميل 1", "عميل 2", "عميل 3" };
        }

        private string[] GetSupplierNames()
        {
            // جلب أسماء الموردين من قاعدة البيانات
            return new string[] { "مورد 1", "مورد 2", "مورد 3" };
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