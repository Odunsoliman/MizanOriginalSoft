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
        private int returnSaleMode;

        // 🟦 متغيرات إعدادات
        private decimal MaxRateDiscount = 0m;
        private bool AllowChangeTax = true;

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
                InvoiceType.Sale => ("فاتورة بيع رقم: ", "1"),
                InvoiceType.SaleReturn => ("فاتورة بيع مرتد رقم: ", "2"),
                InvoiceType.Purchase => ("فاتورة شراء رقم: ", "3"),
                InvoiceType.PurchaseReturn => ("فاتورة شراء مرتد رقم: ", "4"),
                InvoiceType.Inventory => ("إذن تسوية مخزن رقم: ", "5"),
                InvoiceType.DeductStock => ("إذن خصم مخزن رقم: ", "6"),
                InvoiceType.AddStock => ("إذن إضافة مخزن رقم: ", "7"),
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
            LoadFooterSettings();
            CalculateInvoiceFooter();

        }

        #endregion

        #region Header   وظائف الجزء الاعلى من الفاتورة
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

            // نفّذ حسب القيمة
            switch (returnSaleMode)
            {
                case 1: // InvoiceOnly
                    lblCodeTitel.Text = "رقم فاتورة البيع";
                    lblInvStat.Text = "البيع المرتد يكون عن طريق رقم فاتورة البيع الأصلية";
                    tlpReturnMod.Visible = false;
                    rdoInvoice.Checked = true;
                    break;

                case 2: // FreeMode
                    lblCodeTitel.Text = "رقم كود الصنف";
                    lblInvStat.Text = "إرجاع بالكود";
                    tlpReturnMod.Visible = false;
                    rdoFree.Checked = true;
                    break;

                case 3: // MixedMode
                    lblCodeTitel.Text = "رقم كود الصنف";
                    lblInvStat.Text = "إرجاع بالكود";
                    tlpReturnMod.Visible = true;
                    rdoFree.Checked = true;
                    break;

                default:
                    // fallback لو فيه خطأ بالملف
                    lblCodeTitel.Text = "رقم كود الصنف";
                    lblInvStat.Text = "إرجاع حر";
                    tlpReturnMod.Visible = false;
                    rdoFree.Checked = true;
                    break;
            }
        }

        // ✅ تحميل القيم من ملف الإعدادات
        private void LoadSettings()
        {

            allowNegativeStock = AppSettings.GetBool("IsSaleByNegativeStock");
            returnSaleMode = AppSettings.GetInt("ReturnSaleMode");
        }

        private void rdoFree_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoFree.Checked) // تأكد أن الراديو مفعّل
            {
                lblCodeTitel.Text = "رقم كود الصنف";
                lblInvStat.Text = "إرجاع بالكود";
            }
        }

        private void rdoInvoice_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoInvoice.Checked)
            {
                lblCodeTitel.Text = "رقم فاتورة البيع";
                lblInvStat.Text = "البيع المرتد يكون عن طريق رقم فاتورة البيع الأصلية";
            }
        }



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

        #region Foter وظائف اجماليات الفاتورة


        // 🔹 تحميل الإعدادات
        private void LoadFooterSettings()
        {
            try
            {
                // 🟦 قراءة القيم من ملف الإعدادات
                decimal defaultTax = AppSettings.GetDecimal("SalesTax", 0m);
                AllowChangeTax = AppSettings.GetBool("IsEnablToChangTax", true);
                MaxRateDiscount = AppSettings.GetDecimal("MaxRateDiscount", 0.10m); // 10% افتراضياً

                // 🟦 تعيين نسبة الضريبة في الليبل
                lblTaxRate.Text = defaultTax > 0 ? (defaultTax * 100m).ToString("N2") + "%" : "0%";

                // 🟦 حساب قيمة الضريبة حسب الإجمالي إن وجد
                decimal total = 0m;
                decimal.TryParse(lblTotalInv.Text, out total); // يحاول قراءة الإجمالي

                decimal taxValue = total > 0 ? total * defaultTax : 0m;
                txtTaxVal.Text = taxValue.ToString("N2");

                // 🟦 السماح/منع تعديل قيمة الضريبة
                txtTaxVal.ReadOnly = !AllowChangeTax;
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowWarning($"خطأ أثناء تحميل إعدادات الفاتورة:\n{ex.Message}", "خطأ");
            }
        }

        // 🔹 الحد من الخصم
        private void txtDiscount_Leave(object? sender, EventArgs e)
        {
            if (!decimal.TryParse(txtDiscount.Text, out var discount)) discount = 0m;
            if (!decimal.TryParse(lblTotalInv.Text, out var total)) total = 0m;

            var maxDiscount = total * MaxRateDiscount;
            if (discount > maxDiscount)
            {
                discount = maxDiscount;
                txtDiscount.Text = discount.ToString("N2");
                CustomMessageBox.ShowWarning(
                    $"⚠️ الخصم لا يمكن أن يتجاوز {MaxRateDiscount:P0} من إجمالي الفاتورة.",
                    "تنبيه");
            }

            lblDiscountRate.Text = total > 0
            ? ((discount / total) * 100m).ToString("N2") + "%"
            : "0%";

            CalculateInvoiceFooter();
        }

        // 🔹 تحديث نسبة الضريبة
        private void txtTaxVal_Leave(object? sender, EventArgs e)
        {
            if (!decimal.TryParse(txtTaxVal.Text, out var tax)) tax = 0m;
            if (!decimal.TryParse(lblTotalInv.Text, out var total)) total = 0m;

            lblTaxRate.Text = total > 0 ? ((tax / total) * 100m).ToString("N2") : "0.00";
            CalculateInvoiceFooter();
        }

        // 🔹 تحديث نسبة الإضافة
        private void txtValueAdded_Leave(object? sender, EventArgs e)
        {
            if (!decimal.TryParse(txtValueAdded.Text, out var added)) added = 0m;
            if (!decimal.TryParse(lblTotalInv.Text, out var total)) total = 0m;

            lblAdditionalRate.Text = total > 0
            ? ((added / total) * 100m).ToString("N2") + "%"
            : "0%";
            CalculateInvoiceFooter();
        }

        // 🔹 تحديث المدفوعات
        private void txtPayment_Cash_Leave(object? sender, EventArgs e) => CalculateRemainingOnAccount();
        private void txtPayment_Electronic_Leave(object? sender, EventArgs e) => CalculateRemainingOnAccount();

        // 🔹 دعم النقر المزدوج للمدفوعات
        private void txtPayment_Cash_DoubleClick(object? sender, EventArgs e)
        {
            txtPayment_Cash.Text = lblNetTotal.Text;
            CalculateRemainingOnAccount();
        }
        private void txtPayment_Electronic_DoubleClick(object? sender, EventArgs e)
        {
            txtPayment_Electronic.Text = lblNetTotal.Text;
            CalculateRemainingOnAccount();
        }

        // 🔹 حساب الرصيد المتبقي
        private void CalculateRemainingOnAccount()
        {
            decimal.TryParse(lblNetTotal.Text, out decimal net);
            decimal.TryParse(txtPayment_Cash.Text, out decimal cash);
            decimal.TryParse(txtPayment_Electronic.Text, out decimal visa);

            var remaining = net - (cash + visa);
            lblRemainingOnAcc.Text = remaining.ToString("N2");

            if (remaining > 0)
            {
                lblStateRemaining.Text = "باقي عليه";
                lblStateRemaining.ForeColor = Color.Red;
                lblRemainingOnAcc.ForeColor = Color.Red;
            }
            else if (remaining < 0)
            {
                lblStateRemaining.Text = "باقي له";
                lblStateRemaining.ForeColor = Color.Green;
                lblRemainingOnAcc.ForeColor = Color.Green;
            }
            else
            {
                lblStateRemaining.Text = "تم السداد";
                lblStateRemaining.ForeColor = Color.Blue;
                lblRemainingOnAcc.ForeColor = Color.Blue;
            }
        }

        // 🔹 حساب جميع القيم
        private void CalculateInvoiceFooter()
        {
            // 🔹 مجموع الفاتورة من الجريد (مؤقتاً ثابت للتجربة)
            decimal total = 1000m;
            // if (DGV.DataSource is not DataTable dt) return;
            // total = dt.AsEnumerable()
            //          .Where(r => r["NetRow"] != DBNull.Value)
            //          .Sum(r => Convert.ToDecimal(r["NetRow"]));
            // lblTotalInv.Text = total.ToString("N2");

            // 🔹 قراءة القيم
            decimal.TryParse(txtTaxVal.Text, out var tax);
            decimal.TryParse(txtDiscount.Text, out var discount);
            decimal.TryParse(txtValueAdded.Text, out var added);

            // 🔹 حساب الإجمالي بعد الضريبة فقط
            var totalAfterTax = total + tax;
            lblTotalValueAfterTax.Text = totalAfterTax.ToString("N2");

            // 🔹 حساب الصافي النهائي
            var net = total + tax - discount + added;
            lblNetTotal.Text = net.ToString("N2");

            // 🔹 تحديث المتبقي
            CalculateRemainingOnAccount();
        }

        /*المنطق المتبع فى تذييل الفاتورة
         
        lblTotalInv; مجموع اجمالى قيم الاسطر فى الجريد DGV  فى الحقل NetRow
        txtTaxVal; قيمة موجودة فى ملف الاعداد يمكن قرائتها باسم SalesTax مع وجود خاصية بالملف باسم IsEnablToChangTax تسمح للمستخدم من تغيير القيمة او تجعل التكست غير مسموح التغيير فيه

        lblTotalValueAfterTax;=lblTotalInv + lblTotalInv * txtTaxVal
        txtDiscount;قيمة يكتبها المستخدم تكون متاحة فقط فى حال وجود قيمة قى lblTotalValueAfterTax
        ولها خاصية فى ملف الاعداد باسم MaxRateDiscount لا يسمح بكتابة قيمة تتعدى هذه النسبة من قيمة lblTotalInv
        وفى حال تعداها المستخدم تعود القيمة الى اعلى قيمة مسموح بها تلقائيا
        lblDiscountRate;= نسبة الرقم المخصوم من اجمالى الفاتورة lblTotalInv
        txtValueAdded;= القيمة المضافة على الفاتورة 
        lblAdditionalRate;=نسبة القيمة المضافة منسوبة الى lblTotalInv
        lblNetTotal;=lblTotalValueAfterTax - txtDiscount + txtValueAdded

        txtPayment_Cash;= القيمة المدفوعة كاش من صافى الفاتورة تكتب يدويا او بالنقر المزدوج يتم ترحيل كل المبلغ اليها من lblNetTotal
        txtPayment_Electronic;= القيمة المدفوعة بفيزا من صافى الفاتورة تكتب يدويا او بالنقر المزدوج يتم ترحيل كل المبلغ اليها من lblNetTotal
        lblRemainingOnAcc;= lblNetTotal -(txtPayment_Cash+txtPayment_Electronic)

        فماذا ينقصنا فى الكود التالى ام ما الذى يجب تعديله فيه 
        اريد كود متكامل داخل الريجون
         */


        #endregion
        /**/
        #region Save invoice الحفظ النهائى

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