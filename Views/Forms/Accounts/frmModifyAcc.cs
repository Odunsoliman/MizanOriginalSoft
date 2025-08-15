using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses;
namespace MizanOriginalSoft.Views.Forms.Accounts
{
    public partial class frmModifyAcc : Form
    {
        private int _accID; // يتم تمريره من الشاشة السابقة
        private bool _isAssests;
        private bool _isHasDetails;

        public frmModifyAcc(int accID, bool IsAssests, bool isHasDetails)
        {
            InitializeComponent();
            _accID = accID;
            _isAssests = IsAssests;
            _isHasDetails = isHasDetails;

        }
        private void frmModifyAcc_Load(object sender, EventArgs e)
        {
            LoadAccountData(_accID);

            ConnectPhonEvents();
            #region ربط الأحداث للحقول الرقمية والإهلاك
            txtFixedAssetsValue.KeyPress += InputValidationHelper.AllowOnlyNumbersAndDecimal;
            txtDepreciationRateAnnually.KeyPress += InputValidationHelper.AllowOnlyNumbersAndDecimal;
            txtFixedAssetsAge.KeyPress += InputValidationHelper.AllowOnlyNumbers;

            //txtFixedAssetsValue.KeyDown += TxtMoveNext_KeyDown;
            //txtDepreciationRateAnnually.KeyDown += TxtMoveNext_KeyDown;
            txtFixedAssetsAge.KeyDown += TxtCalculate_KeyDown;

            //txtFixedAssetsValue.TextChanged += TxtCalculate_TextChanged;
            //txtDepreciationRateAnnually.TextChanged += TxtCalculate_TextChanged;
            //txtFixedAssetsAge.TextChanged += TxtCalculate_TextChanged;
            #endregion

        }

        #region ============ Calculate Asset ===============

        // تنفيذ الحساب عند الضغط على Enter في الحقل الأخير
        private void TxtCalculate_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                CalculateDepreciation();
            }
        }

        #region حساب الإهلاك

        bool isUpdating = false;

        private void CalculateDepreciation()
        {
            UpdateDepreciation();
        }

        private void txtFixedAssetsValue_TextChanged(object sender, EventArgs e)
        {
            CalculateDepreciation();
        }

        private void txtDepreciationRateAnnually_TextChanged(object sender, EventArgs e)
        {
            if (isUpdating) return;

            if (txtDepreciationRateAnnually.Focused)
            {
                isUpdating = true;
                CalculateMonthsFromRate();
                UpdateDepreciation();
                isUpdating = false;
            }
        }

        private void txtFixedAssetsAge_TextChanged(object sender, EventArgs e)
        {
            if (isUpdating) return;

            if (txtFixedAssetsAge.Focused)
            {
                isUpdating = true;
                CalculateRateFromMonths();
                UpdateDepreciation();
                isUpdating = false;
            }
        }

        private void CalculateMonthsFromRate()
        {
            if (decimal.TryParse(txtDepreciationRateAnnually.Text, out decimal rate) && rate > 0)
            {
                decimal years = 100 / rate;
                decimal months = years * 12;
                txtFixedAssetsAge.Text = Math.Round(months, 0).ToString();
            }
        }

        private void CalculateRateFromMonths()
        {
            if (decimal.TryParse(txtFixedAssetsAge.Text, out decimal months) && months > 0)
            {
                decimal years = months / 12;
                decimal rate = 100 / years;
                txtDepreciationRateAnnually.Text = Math.Round(rate, 2).ToString();
            }
        }
        private void UpdateDepreciation()
        {
            if (decimal.TryParse(txtFixedAssetsValue.Text, out decimal value) &&
                decimal.TryParse(txtDepreciationRateAnnually.Text, out decimal rate))
            {
                decimal annual = value * (rate / 100);
                decimal monthly = annual / 12;

                lblAnnuallyInstallment.Text = annual.ToString("N2");
                lblMonthlyInstallment.Text = monthly.ToString("N2");
            }
            else
            {
                lblAnnuallyInstallment.Text = "0.00";
                lblMonthlyInstallment.Text = "0.00";
            }
        }


        #endregion

        #endregion


        #region حساب الإهلاك

        //bool isUpdating = false;

        //private void CalculateDepreciation()
        //{
        //    UpdateDepreciation();
        //}

        //private void txtFixedAssetsValue_TextChanged(object sender, EventArgs e)
        //{
        //    CalculateDepreciation();
        //}

        //private void txtDepreciationRateAnnually_TextChanged(object sender, EventArgs e)
        //{
        //    if (isUpdating) return;

        //    if (txtDepreciationRateAnnually.Focused)
        //    {
        //        isUpdating = true;
        //        CalculateMonthsFromRate();
        //        UpdateDepreciation();
        //        isUpdating = false;
        //    }
        //}

        //private void txtFixedAssetsAge_TextChanged(object sender, EventArgs e)
        //{
        //    if (isUpdating) return;

        //    if (txtFixedAssetsAge.Focused)
        //    {
        //        isUpdating = true;
        //        CalculateRateFromMonths();
        //        UpdateDepreciation();
        //        isUpdating = false;
        //    }
        //}

        //private void CalculateMonthsFromRate()
        //{
        //    if (decimal.TryParse(txtDepreciationRateAnnually.Text, out decimal rate) && rate > 0)
        //    {
        //        decimal years = 100 / rate;
        //        decimal months = years * 12;
        //        txtFixedAssetsAge.Text = Math.Round(months, 0).ToString();
        //    }
        //}

        //private void CalculateRateFromMonths()
        //{
        //    if (decimal.TryParse(txtFixedAssetsAge.Text, out decimal months) && months > 0)
        //    {
        //        decimal years = months / 12;
        //        decimal rate = 100 / years;
        //        txtDepreciationRateAnnually.Text = Math.Round(rate, 2).ToString();
        //    }
        //}
        //private void UpdateDepreciation()
        //{
        //    if (decimal.TryParse(txtFixedAssetsValue.Text, out decimal value) &&
        //        decimal.TryParse(txtDepreciationRateAnnually.Text, out decimal rate))
        //    {
        //        decimal annual = value * (rate / 100);
        //        decimal monthly = annual / 12;

        //        lblAnnuallyInstallment.Text = annual.ToString("N2");
        //        lblMonthlyInstallment.Text = monthly.ToString("N2");
        //    }
        //    else
        //    {
        //        lblAnnuallyInstallment.Text = "0.00";
        //        lblMonthlyInstallment.Text = "0.00";
        //    }
        //}


        #endregion



        private void LoadAccountData(int accID)
        {
            DataTable dt = DBServiecs.MainAcc_GetAccountsByID(accID);
            if (dt.Rows.Count == 0) return;

            DataRow row = dt.Rows[0];

            // تنسيق النص في الليبل العلوي
            lblTitetl_Item.Text = $"حساب: {row["AccName"]}    كود رقم: {accID}    الرصيد: {row["Balance"]} {row["BalanceState"]}";

            txtAccName.Text = row["AccName"]?.ToString();
            chkIsHidden.Checked = row["IsHidden"] != DBNull.Value && Convert.ToBoolean(row["IsHidden"]);
            txtAccNote.Text = row["AccNote"]?.ToString();

            lblDateOfJoin.Text = row["DateOfJoin"] != DBNull.Value
                ? Convert.ToDateTime(row["DateOfJoin"]).ToShortDateString()
                : "";

            bool isFixedAssets = _isAssests;
            bool isHasDetails = _isHasDetails;

            // إخفاء كلا الأقسام
            tableLayoutPanelHasDetait.Visible = false;
            tableLayoutPanelIsAssets.Visible = false;

            // تهيئة RowStyles إذا لم تكن مهيأة مسبقًا
            if (tlpData.RowStyles.Count < 2)
            {
                tlpData.RowStyles.Clear();
                tlpData.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
                tlpData.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
                tableLayoutPanelIsAssets.Height = 260;

            }

            if (isFixedAssets)
            {
                tableLayoutPanelIsAssets.Visible = true;

                // تعديل ارتفاع الصفوف
                tlpData.RowStyles[0].Height = 1F;
                tlpData.RowStyles[1].Height = 99F;

                txtFixedAssetsValue.Text = row["FixedAssetsValue"] != DBNull.Value ? row["FixedAssetsValue"].ToString() : "";
                txtDepreciationRateAnnually.Text = row["DepreciationRateAnnually"] != DBNull.Value ? row["DepreciationRateAnnually"].ToString() : "";
                txtFixedAssetsAge.Text = row["FixedAssetsAge"] != DBNull.Value ? row["FixedAssetsAge"].ToString() : "";

                lblAnnuallyInstallment.Text = row["AnnuallyInstallment"] != DBNull.Value
                    ? Convert.ToDecimal(row["AnnuallyInstallment"]).ToString("N2")
                    : "0.00";

                lblMonthlyInstallment.Text = row["MonthlyInstallment"] != DBNull.Value
                    ? Convert.ToDecimal(row["MonthlyInstallment"]).ToString("N2")
                    : "0.00";

                chkIsEndedFixedAssets.Checked = row["IsEndedFixedAssets"] != DBNull.Value && Convert.ToBoolean(row["IsEndedFixedAssets"]);

                lblFixedAssetsEndDate.Text = row["FixedAssetsEndDate"] != DBNull.Value
                    ? Convert.ToDateTime(row["FixedAssetsEndDate"]).ToShortDateString()
                    : "";
            }
            else if (isHasDetails)
            {
                tableLayoutPanelHasDetait.Visible = true;

                // تعديل ارتفاع الصفوف
                tlpData.RowStyles[0].Height = 99F;
                tlpData.RowStyles[1].Height = 1F;
                tableLayoutPanelHasDetait.Height = 225;//كيف اتحكم فى ارتفاع طول التيبل بانل

                txtFirstPhon.Text = row["FirstPhon"]?.ToString();
                txtAntherPhon.Text = row["AntherPhon"]?.ToString();
                txtClientEmail.Text = row["ClientEmail"]?.ToString();
                txtClientAddress.Text = row["ClientAddress"]?.ToString();
            }
            else
            {
                // في حال عدم وجود بيانات أصول أو تفاصيل، توزيع متساوي
                tlpData.RowStyles[0].Height = 50F;
                tlpData.RowStyles[1].Height = 50F;
            }

            // يمكنك الاستغناء عن هذه الدالة إذا تم تعديل الارتفاعات هنا مباشرة
            AdjustRowHeights(isFixedAssets, isHasDetails);
        }


        private void AdjustRowHeights(bool isFixedAssets, bool isHasDetails)
        {
            if (tlpData.RowStyles.Count < 2)
            {
                tlpData.RowStyles.Clear();
                tlpData.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
                tlpData.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            }

            if (isFixedAssets)
            {
                tlpData.RowStyles[0].Height = 1F;
                tlpData.RowStyles[0].SizeType = SizeType.Percent;
                tlpData.RowStyles[1].Height = 99F;
                tlpData.RowStyles[1].SizeType = SizeType.Percent;
                tableLayoutPanelIsAssets.Visible = true;
                tableLayoutPanelHasDetait.Visible = false;
            }
            else if (isHasDetails)
            {
                tlpData.RowStyles[0].Height = 99F;
                tlpData.RowStyles[0].SizeType = SizeType.Percent;
                tlpData.RowStyles[1].Height = 1F;
                tlpData.RowStyles[1].SizeType = SizeType.Percent;
                tableLayoutPanelHasDetait.Visible = true;
                tableLayoutPanelIsAssets.Visible = false;
            }
            else
            {
                tlpData.RowStyles[0].Height = 50F;
                tlpData.RowStyles[1].Height = 50F;
                tlpData.RowStyles[0].SizeType = SizeType.Percent;
                tlpData.RowStyles[1].SizeType = SizeType.Percent;
                tableLayoutPanelHasDetait.Visible = false;
                tableLayoutPanelIsAssets.Visible = false;
            }
        }





        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAccName.Text))
            {
                MessageBox.Show("يرجى إدخال اسم الحساب الجديد.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAccName.Focus();
                return;
            }

            string resultMessage;

            try
            {
                int accID = _accID;
                string accName = txtAccName.Text.Trim();
                bool isHidden = chkIsHidden.Checked;

                float? fixedAssetsValue = string.IsNullOrWhiteSpace(txtFixedAssetsValue.Text)
                    ? (float?)null
                    : float.Parse(txtFixedAssetsValue.Text);

                float? depreciationRateAnnually = string.IsNullOrWhiteSpace(txtDepreciationRateAnnually.Text)
                    ? (float?)null
                    : float.Parse(txtDepreciationRateAnnually.Text);

                int? fixedAssetsAge = string.IsNullOrWhiteSpace(txtFixedAssetsAge.Text)
                    ? (int?)null
                    : int.Parse(txtFixedAssetsAge.Text);

                float? annuallyInstallment = string.IsNullOrWhiteSpace(lblAnnuallyInstallment.Text)
                    ? (float?)null
                    : float.Parse(lblAnnuallyInstallment.Text);

                float? monthlyInstallment = string.IsNullOrWhiteSpace(lblMonthlyInstallment.Text)
                    ? (float?)null
                    : float.Parse(lblMonthlyInstallment.Text);

                bool? isEndedFixedAssets = chkIsEndedFixedAssets.Checked;

                DateTime? fixedAssetsEndDate = chkIsEndedFixedAssets.Checked
                    ? DateTime.Parse(lblFixedAssetsEndDate.Text)
                    : (DateTime?)null;
                /*اريد التحقق ان النص لا تزيد  15 ديجيت */
                string firstPhon = txtFirstPhon.Text.Trim();
                string antherPhon = txtAntherPhon.Text.Trim();
                string accNote = txtAccNote.Text.Trim();
                string clientEmail = txtClientEmail.Text.Trim();
                string clientAddress = txtClientAddress.Text.Trim();

                int userId = CurrentSession.UserID;

                // ✅ استدعاء الدالة (تأكد أن دالة MainAcc_UpdateAccount تحتوي على 19 معامل)
                bool result = DBServiecs.MainAcc_UpdateAccount(
                   accID,
                   accName,
                   isHidden,
                   fixedAssetsValue,
                   depreciationRateAnnually,
                   fixedAssetsAge,
                   isEndedFixedAssets,
                   firstPhon,
                   antherPhon,
                   accNote,
                   clientEmail,
                   clientAddress,
                   userId,
                   out resultMessage
               );

                if (result)
                {
                    MessageBox.Show(resultMessage, "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK; // ✅ تحديد النتيجة قبل الإغلاق
                    this.Close();
                }
                else
                {
                    MessageBox.Show(resultMessage, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء الحفظ:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #region *********** PhonEvents ***********************
        private void ConnectPhonEvents()
        {
            txtFirstPhon.KeyPress += PhoneTextBox_KeyPress;
            txtAntherPhon.KeyPress += PhoneTextBox_KeyPress;

            txtFirstPhon.TextChanged += PhoneTextBox_TextChanged;
            txtAntherPhon.TextChanged += PhoneTextBox_TextChanged;

            txtFirstPhon.Leave += PhoneTextBox_Leave;
            txtAntherPhon.Leave += PhoneTextBox_Leave;

        }
        private void PhoneTextBox_KeyPress(object? sender, KeyPressEventArgs e)
        {
            TextBox? txt = sender as TextBox;

            // السماح بالأرقام وبعض الرموز الشائعة
            if (!char.IsControl(e.KeyChar) &&
                !char.IsDigit(e.KeyChar) &&
                e.KeyChar != '+' &&
                e.KeyChar != '-' &&
                e.KeyChar != ' ' &&
                e.KeyChar != '(' &&
                e.KeyChar != ')')
            {
                e.Handled = true;
                return;
            }

            // حساب عدد الأرقام فقط
            string digitsOnly = new string(txt?.Text.Where(char.IsDigit).ToArray());

            // إذا ضغط رقم وتجاوز عدد الأرقام 15 → منع الإدخال
            if (char.IsDigit(e.KeyChar) && digitsOnly.Length >= 15)
            {
                e.Handled = true;

                if (txt == txtFirstPhon)
                    lblErrorPhon.Text = "الرقم لا يزيد عن 15 رقم";
                else if (txt == txtAntherPhon)
                    lblErrorAntherPhon.Text = "الرقم لا يزيد عن 15 رقم";
            }
        }

        private void PhoneTextBox_TextChanged(object? sender, EventArgs e)
        {
            TextBox? txt = sender as TextBox;

            // حذف أي أرقام زيادة بعد اللصق
            string digitsOnly = new string(txt?.Text.Where(char.IsDigit).ToArray());

            if (txt != null && digitsOnly.Length > 15)
            {
                // خذ أول 15 رقم وأعد تركيب النص بنفس الرموز الموجودة
                int count = 0;
                string newText = "";

                foreach (char c in txt.Text)
                {
                    if (char.IsDigit(c))
                    {
                        if (count < 15)
                        {
                            newText += c;
                            count++;
                        }
                    }
                    else
                    {
                        newText += c; // أضف الرموز كما هي
                    }
                }

                txt.Text = newText;
                txt.SelectionStart = txt.Text.Length; // وضع المؤشر في آخر النص

                if (txt == txtFirstPhon)
                    lblErrorPhon.Text = "الرقم لا يزيد عن 15 رقم";
                else if (txt == txtAntherPhon)
                    lblErrorAntherPhon.Text = "الرقم لا يزيد عن 15 رقم";
            }
        }

        private void PhoneTextBox_Leave(object? sender, EventArgs e)
        {
            if (sender == txtFirstPhon)
                lblErrorPhon.Text = "";
            else if (sender == txtAntherPhon)
                lblErrorAntherPhon.Text = "";
        }

        #endregion 

        #region ******** KeyDown *****************
        private void txtAccName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (tableLayoutPanelHasDetait.Visible) // لو التفاصيل ظاهرة
                {
                    txtFirstPhon.Focus();
                    txtFirstPhon.SelectAll();
                }
                else if (tableLayoutPanelIsAssets.Visible) // لو الأصول الثابتة ظاهرة
                {
                    txtFixedAssetsValue.Focus();
                    txtFixedAssetsValue.SelectAll();
                }
                else // لو كلاهما مخفي
                {
                    btnSave.Focus();
                }

                e.Handled = true; // منع الانتقال الافتراضي
                e.SuppressKeyPress = true; // منع إصدار صوت "ding"
            }
        }


        private void txtFirstPhon_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtAntherPhon.Focus();
                txtAntherPhon.SelectAll();
                e.Handled = true; // منع الانتقال الافتراضي
                e.SuppressKeyPress = true; // منع إصدار صوت "ding"
            }
        }

        private void txtAntherPhon_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtAccNote.Focus();
                txtAccNote.SelectAll();
                e.Handled = true; // منع الانتقال الافتراضي
                e.SuppressKeyPress = true; // منع إصدار صوت "ding"
            }
        }

        private void txtAccNote_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtClientEmail.Focus();
                txtClientEmail.SelectAll();
                e.Handled = true; // منع الانتقال الافتراضي
                e.SuppressKeyPress = true; // منع إصدار صوت "ding"
            }
        }

        private void txtClientEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtClientAddress.Focus();
                txtClientAddress.SelectAll();
                e.Handled = true; // منع الانتقال الافتراضي
                e.SuppressKeyPress = true; // منع إصدار صوت "ding"
            }
        }

        private void txtClientAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtAccName.Focus();
                txtAccName.SelectAll();
                e.Handled = true; // منع الانتقال الافتراضي
                e.SuppressKeyPress = true; // منع إصدار صوت "ding"
            }
        }

        private void txtFixedAssetsValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtDepreciationRateAnnually.Focus();
                txtDepreciationRateAnnually.SelectAll();
                e.Handled = true; // منع الانتقال الافتراضي
                e.SuppressKeyPress = true; // منع إصدار صوت "ding"
            }
        }

        private void txtDepreciationRateAnnually_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtFixedAssetsAge.Focus();
                txtFixedAssetsAge.SelectAll();
                e.Handled = true; // منع الانتقال الافتراضي
                e.SuppressKeyPress = true; // منع إصدار صوت "ding"
            }
        }

        private void txtFixedAssetsAge_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtFixedAssetsValue.Focus();
                txtFixedAssetsValue.SelectAll();
                e.Handled = true; // منع الانتقال الافتراضي
                e.SuppressKeyPress = true; // منع إصدار صوت "ding"
            }
        }

        #endregion 
    }
}
