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
using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.Views.Forms.Accounts; // هنا يوجد enum InvoiceType

namespace MizanOriginalSoft.Views.Forms.Movments
{


    public partial class frm_DynamicInvoice : Form
    {
        #region Fields
        private InvoiceType currentInvoiceType; // نوع الفاتورة الحالية

        // 🔹 متغير يحدد إذا كان مسموح البيع بدون رصيد (على المكشوف)
        private bool allowNegativeStock;

        // 🔹 متغير يحدد إذا كان المرتجع يشترط إدخال رقم فاتورة البيع
        private bool reSaleByInvoiceSale;
        #endregion

        #region Form Initialization
        public frm_DynamicInvoice()
        {
            InitializeComponent();
        }

        public void InitializeInvoice(InvoiceType type)
        {
            currentInvoiceType = type;

            // 🔹 استخدم switch لتحديد العنوان والرقم مع النص العربي
            (string arabicTitle, string typeId) = type switch
            {
                InvoiceType.Sale => ("فاتورة بيع", "1"),
                InvoiceType.SaleReturn => ("فاتورة بيع مرتد", "2"),
                InvoiceType.Purchase => ("فاتورة شراء", "3"),
                InvoiceType.PurchaseReturn => ("فاتورة شراء مرتد", "4"),
                InvoiceType.Inventory => ("إذن تسوية مخزن", "5"),
                InvoiceType.DeductStock => ("إذن خصم مخزن", "6"),
                InvoiceType.AddStock => ("إذن إضافة مخزن", "7"),
                _ => ("فاتورة", "0")
            };

            // 🔹 عيّن القيم
            this.Text = arabicTitle;
            lblTypeInv.Text = arabicTitle;   // 🔥 الآن يعرض النص العربي
            lblTypeInvID.Text = typeId;

            // تعبئة الحقول
            FillDefaultAccount();
            ConfigureAutoCompleteForAccount();
            FillSellerComboBox();
            SetupFormByInvoiceType();
        }

        private void frm_DynamicInvoice_Load(object sender, EventArgs e)
        {
            // ✅ تحميل ملف الإعدادات
            if (!AppSettingsIsLoaded())
            {
                string settingsPath = Path.Combine(Application.StartupPath, "AppSettings.txt");
                AppSettings.Load(settingsPath);
            }

            // ✅ قراءة الإعدادات
            LoadSettings();

            // ✅ تحويل النص لرقم أولاً
            if (int.TryParse(lblTypeInvID.Text, out int typeInvID))
            {
                if (typeInvID == 1)
                    UpdateLabelsForSale();
                else if (typeInvID == 2)
                    UpdateLabelsForResale();
            }


        }
        #endregion

  
        // ✅ التحقق إذا كان AppSettings متحمل
        private bool AppSettingsIsLoaded()
        {
            try
            {
                // لو حاولنا قراءة أي قيمة من غير تحميل هيعمل Exception
                AppSettings.GetAllSettings();
                return true;
            }
            catch
            {
                return false;
            }
        }


        // 🔹 تحديث النصوص لو اخترت "بيع"
        private void UpdateLabelsForSale()
        {
            if (allowNegativeStock)
                lblInvStat.Text = "البيع على مكشوف";
            else
                lblInvStat.Text = "البيع حسب الرصيد";

            lblCodeTitel.Text = "ادخل كود الصنف";
        }

        // 🔹 تحديث النصوص لو اخترت "مرتجع"
        private void UpdateLabelsForResale()
        {
            lblInvStat.Text = ""; // ممكن تكتب "مرتجع" لو تحب

            if (reSaleByInvoiceSale)
            {
                lblCodeTitel.Text = " رقم فاتورة البيع";
                lblInvStat.Text = "البيع المرتد يكون عن طريق رقم فاتورة البيع الاصلية";
            }

            else
                lblCodeTitel.Text = "ادخل كود الصنف";
        }

        // ✅ تحميل القيم من ملف الإعدادات
        private void LoadSettings()
        {
            
            allowNegativeStock = AppSettings.GetBool("NegativeStockSale");
            reSaleByInvoiceSale = AppSettings.GetBool("ReSaleByInvoiceSale");
        }

        #region Header   وظائف الجزء الاعلى من الفاتورة

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
        private void cbxSellerID_Enter(object sender, EventArgs e)
        {
            cbxSellerID.DroppedDown = true;
        }

        private void cbxSellerID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                // Enter فقط → التالي
                txtSeaarchProd.Focus();
                e.Handled = true;
            }
            else if ((e.KeyCode == Keys.Enter && e.Shift) || e.KeyCode == Keys.Up)
            {
                // Shift+Enter أو سهم ↑ → السابق
                this.SelectNextControl((Control)sender, false, true, true, true);
                e.Handled = true;
            }
        }

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
                InvoiceType.Sale or InvoiceType.SaleReturn => 57, // حساب ادارة البائعين
                InvoiceType.Purchase or InvoiceType.PurchaseReturn => 61, // حساب  ادارة المشتريات
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


            // 🔒 منع الكتابة في الكومبوبوكس
            cbxSellerID.DropDownStyle = ComboBoxStyle.DropDownList;
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

        #region Account Data Display
        private void txtAccName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                // Enter فقط → التالي
                cbxSellerID.Focus();
                e.Handled = true;
            }
            else if ((e.KeyCode == Keys.Enter && e.Shift) || e.KeyCode == Keys.Up)
            {
                // Shift+Enter أو سهم ↑ → السابق
                this.SelectNextControl((Control)sender, false, true, true, true);
                e.Handled = true;
            }

        }
        private void txtAccName_Leave(object sender, EventArgs e)
        {
            string input = txtAccName.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                ClearAccountDetails();
                return;
            }

            string invoiceTypeKey = InvoiceTypeHelper.ToAccountTypeString(currentInvoiceType);
            if (string.IsNullOrEmpty(invoiceTypeKey))
            {
                ClearAccountDetails();
                return;
            }

            DataTable dt = DBServiecs.NewInvoice_GetAcc(invoiceTypeKey);

            DataRow? selectedAccount = null;

            // 🔹 تحقق: هل المدخل رقم هاتف؟
            bool isPhoneNumber = input.All(c => char.IsDigit(c) || c == '+' || c == '-');

            if (isPhoneNumber)
            {
                // 🔹 البحث بالهاتف
                selectedAccount = dt.AsEnumerable()
                    .FirstOrDefault(row =>
                        string.Equals(row.Field<string?>("FirstPhon"), input, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(row.Field<string?>("AntherPhon"), input, StringComparison.OrdinalIgnoreCase));

                if (selectedAccount == null)
                {
                    // ❗ الرقم غير مسجل
                    DialogResult result = MessageBox.Show(
                        "⚠️ هذا الرقم غير مسجل، هل تريد إضافته إلى البيانات؟",
                        "إضافة حساب جديد",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        int type = (int)currentInvoiceType;  // 🔥 يحوّل Enum لرقم حقيقي
                        frm_AddAccount frm = new frm_AddAccount(input, type);
                        frm.ShowDialog();
                    }
                    else
                    {
                        LoadDefaultAccount();
                    }

                    return;
                }

                txtAccName.Text = selectedAccount["AccName"]?.ToString() ?? string.Empty;
            }
            else
            {
                // 🔹 البحث بالاسم
                selectedAccount = dt.AsEnumerable()
                    .FirstOrDefault(row =>
                        string.Equals(row.Field<string?>("AccName"), input, StringComparison.OrdinalIgnoreCase));

                if (selectedAccount == null)
                {
                    // ❗ الاسم غير مسجل
                    DialogResult result = MessageBox.Show(
                        "⚠️ هذا الاسم غير مسجل، هل تريد إضافته إلى البيانات؟",
                        "إضافة حساب جديد",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        int type = (currentInvoiceType == InvoiceType.Sale || currentInvoiceType == InvoiceType.SaleReturn) ? 1 : 2;
                        frm_AddAccount frm = new frm_AddAccount(input, type);
                        frm.ShowDialog();
                    }
                    else
                    {
                        LoadDefaultAccount();
                    }

                    return;
                }
            }

            if (selectedAccount != null)
            {
                lblAccID.Text = selectedAccount["AccID"]?.ToString() ?? "0";
                DisplayAccountDetails(selectedAccount);
            }
            else
            {
                ClearAccountDetails();
            }
        }

        // 🔹 دالة تحميل الحساب الافتراضي
        private void LoadDefaultAccount()
        {
            string invoiceTypeKey = InvoiceTypeHelper.ToAccountTypeString(currentInvoiceType);
            if (string.IsNullOrEmpty(invoiceTypeKey)) return;

            DataTable dt = DBServiecs.NewInvoice_GetAcc(invoiceTypeKey);

            if (dt.Rows.Count > 0)
            {
                int defaultId = (currentInvoiceType == InvoiceType.Sale || currentInvoiceType == InvoiceType.SaleReturn) ? 55 : 56;
                DataRow? defaultAccount = dt.AsEnumerable()
                    .FirstOrDefault(row => row.Field<int>("AccID") == defaultId);

                if (defaultAccount != null)
                {
                    lblAccID.Text = defaultAccount["AccID"].ToString();
                    txtAccName.Text = defaultAccount["AccName"].ToString();
                    DisplayAccountDetails(defaultAccount);
                }
            }
        }

        private void DisplayAccountDetails(DataRow accountRow)
        {
            // 🔹 الهاتفين
            string? firstPhone = accountRow.Field<string?>("FirstPhon");
            string? anotherPhone = accountRow.Field<string?>("AntherPhon");

            if (!string.IsNullOrWhiteSpace(firstPhone) && !string.IsNullOrWhiteSpace(anotherPhone))
            {
                lblFirstPhon.Text = $"هواتف: {firstPhone} - {anotherPhone}";
            }
            else if (!string.IsNullOrWhiteSpace(firstPhone))
            {
                lblFirstPhon.Text = $"هاتف: {firstPhone}";
            }
            else if (!string.IsNullOrWhiteSpace(anotherPhone))
            {
                lblFirstPhon.Text = $"هاتف: {anotherPhone}";
            }
            else
            {
                lblFirstPhon.Text = string.Empty;
            }

            // 🔹 البريد الإلكتروني
            string? email = accountRow.Field<string?>("ClientEmail");
            lblClientEmail.Text = !string.IsNullOrWhiteSpace(email)
                ? $"Email: {email}"
                : string.Empty;

            // 🔹 العنوان
            string? address = accountRow.Field<string?>("ClientAddress");
            lblClientAddress.Text = !string.IsNullOrWhiteSpace(address)
                ? $"العنوان: {address}"
                : string.Empty;

            // 🔹 الرصيد (حل مشكلة التحويل)
            decimal balance = 0;
            if (accountRow["Balance"] != DBNull.Value)
            {
                balance = Convert.ToDecimal(accountRow["Balance"]);
            }

            if (balance == 0)
            {
                lblB_Status.Text = "الرصيد: متوازن";
            }
            else if (balance > 0)
            {
                lblB_Status.Text = $"الرصيد: دائن بـ {balance:N2}";
            }
            else
            {
                lblB_Status.Text = $"الرصيد: مدين بـ {Math.Abs(balance):N2}";
            }
        }

        private void ClearAccountDetails()
        {
            lblAccID.Text = "0";
            lblFirstPhon.Text = string.Empty;
            lblClientEmail.Text = string.Empty;
            lblClientAddress.Text = string.Empty;
        }
        #endregion

        #endregion End Heder


        #region Body  وظائف الجزء الخاص بالاصناف 
        private void txtSeaarchProd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                // Enter فقط → التالي
                //cbxSellerID.Focus();
                //e.Handled = true;
            }
            else if ((e.KeyCode == Keys.Enter && e.Shift) || e.KeyCode == Keys.Up)
            {
                // Shift+Enter أو سهم ↑ → السابق
                cbxSellerID.Focus();
                e.Handled = true;
            }

        }


        #endregion

        #region Foter وظائف اجماليات الفاتورة والحفظ النهائى



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