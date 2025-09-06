using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses;
using MizanOriginalSoft.Views.Forms.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinTextBox = System.Windows.Forms.TextBox;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Security.Cryptography.Xml;
using MizanOriginalSoft.Views.Reports;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MizanOriginalSoft.Views.Forms.Accounts
{
    public partial class frmMainAccounts : Form
    {
        // ================== المتغيرات العامة ==================
        #region Variables
        private KeyboardLanguageManager _languageManager;
        private int AccTopID;
        private int TopID, SubTopID;
        private ToolStripMenuItem? tsmiCategoryReports;
        private ToolStripMenuItem tsmiGroupedReports;
        private DataRow? tblRow;
        private DataTable? tblAccTopSub;
        private DataTable? tblAccTop;

        private string previousAccID = string.Empty;
        public bool newR = false;
        private bool hasDetails;
        private bool hasFixedAssets;
        #endregion
        // ===== (2) مُنشئ الفورم =====
        public frmMainAccounts(int TopID)
        {
            InitializeComponent();

            DBServiecs.A_UpdateAllDataBase();

            tsmiGroupedReports = new ToolStripMenuItem();
            _languageManager = new KeyboardLanguageManager(this);
            this.AccTopID = TopID;

            DGV.ClearSelection();

            // (جيد للأداء) إعدادات قبل التحميل
            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            DGV.EnableHeadersVisualStyles = false;
            DGV.RowHeadersVisible = false;

            #region ربط الأحداث للحقول الرقمية والإهلاك
            txtFixedAssetsValue.KeyPress += InputValidationHelper.AllowOnlyNumbersAndDecimal;
            txtDepreciationRateAnnually.KeyPress += InputValidationHelper.AllowOnlyNumbersAndDecimal;
            txtFixedAssetsAge.KeyPress += InputValidationHelper.AllowOnlyNumbers;
            #endregion

        }


        private void frmMainAccounts_Load(object sender, EventArgs e)
        {
            this.Visible = false;
            TypAcc();
            AccTop_LoadFollowers();
            AccountDGV(AccTopID);
            DGVStyl();
            FillcbxChangeCat();
            SetupMenuStrip();
            LoadReports(AccTopID);
            tabControlAccount.DrawMode = TabDrawMode.OwnerDrawFixed;
            connectRDO();
            ConnectEvents();
            txtSearch.Focus();
            txtSearch.SelectAll();
            ConnectKeyDown();
            this.Visible = true;
        }
        private void DGVStyl()
        {

            if (DGV.DataSource == null) return;

            // مهم: لا تعيد إنشاء DataTable جديد — استخدم الـ DefaultView للحفاظ على الربط
            if (DGV.DataSource is DataTable dt)
            {
                dt.DefaultView.Sort = "SortTree ASC"; // أو حسب ما تريده
                DGV.DataSource = dt.DefaultView;
            }

            // باقي تنسيق الأعمدة…
            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            string[] visibleColumns = { "AccName", "ParentAccName", "Balance", "BalanceState" };
            foreach (DataGridViewColumn col in DGV.Columns)
                col.Visible = visibleColumns.Contains(col.Name);

            DGV.Columns["AccName"].FillWeight = 3;
            DGV.Columns["ParentAccName"].FillWeight = 2;
            DGV.Columns["Balance"].FillWeight = 2;
            DGV.Columns["BalanceState"].FillWeight = 1;

            DGV.Columns["AccName"].HeaderText = "الاسم";
            DGV.Columns["ParentAccName"].HeaderText = "تصنيف";
            DGV.Columns["Balance"].HeaderText = "الرصيد";
            DGV.Columns["BalanceState"].HeaderText = "---";

            DGV.DefaultCellStyle.Font = new Font("Arial", 12);
            DGV.DefaultCellStyle.ForeColor = Color.Black;
            DGV.DefaultCellStyle.BackColor = Color.White;

            DGV.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);
            DGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DGV.Columns["BalanceState"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewRow row in DGV.Rows)
            {
                if (row.Cells["AccID"].Value != null &&
                    int.TryParse(row.Cells["AccID"].Value.ToString(), out int accId) &&
                    accId < 70)
                {
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                }
            }
        }






































        #region ================== التنقل بالإنتر ==================
        // ================== دوال المساعدة ==================
        private void FocusAndSelect(Control ctl)
        {
            if (ctl == null) return;
            ctl.Focus();
            this.BeginInvoke(new Action(() =>
            {
                if (this.ActiveControl is System.Windows.Forms.TextBox tb)
                    tb.SelectAll();
            }));
        }

        private void ConnectKeyDown()
        {
            // فك أي اشتراك قديم لمنع التكرار
            txtFixedAssetsValue.KeyDown -= GlobalTextBox_KeyDown;
            txtDepreciationRateAnnually.KeyDown -= GlobalTextBox_KeyDown;
            txtFixedAssetsAge.KeyDown -= GlobalTextBox_KeyDown;

            // ربط الحقول التي تريد التنقل بينها
            Control[] moveControls =
            {
        txtAccName, txtFirstPhon, txtAntherPhon, txtAccNote,
        txtClientEmail, txtClientAddress,
        txtFixedAssetsValue, txtDepreciationRateAnnually, txtFixedAssetsAge
    };

            foreach (var c in moveControls)
                c.KeyDown += GlobalTextBox_KeyDown;
        }


        private void GlobalTextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            e.Handled = true;
            e.SuppressKeyPress = true;

            if (!(sender is Control current)) return;

            // دورة مغلقة لحقول الأصول الثابتة
            if (current == txtFixedAssetsValue) { FocusAndSelect(txtDepreciationRateAnnually); return; }
            if (current == txtDepreciationRateAnnually) { FocusAndSelect(txtFixedAssetsAge); return; }
            if (current == txtFixedAssetsAge) { FocusAndSelect(txtFixedAssetsValue); return; }

            // بقية الحقول حسب ترتيبك
            if (current == txtAccName)
            {
                if (tlpPhon.Visible) { FocusAndSelect(txtFirstPhon); return; }
                else if (tlpFixedAssets.Visible) { FocusAndSelect(txtFixedAssetsValue); return; }
                else { btnSave.Focus(); return; }
            }

            if (current == txtFirstPhon) { FocusAndSelect(txtAntherPhon); return; }
            if (current == txtAntherPhon) { FocusAndSelect(txtAccNote); return; }
            if (current == txtAccNote) { FocusAndSelect(txtClientEmail); return; }
            if (current == txtClientEmail) { FocusAndSelect(txtClientAddress); return; }
            if (current == txtClientAddress) { FocusAndSelect(txtAccName); return; }

            // افتراضي: انتقل للحقل التالي بالتاب
            this.SelectNextControl(current, true, true, true, true);
            if (this.ActiveControl is System.Windows.Forms.TextBox tb)
                this.BeginInvoke(new Action(() => tb.SelectAll()));

        }

        #endregion

        #region ********************* languageManager ****************************

        public void SwitchToArabic()
        {
            InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new System.Globalization.CultureInfo("ar-EG"));
        }

        public void SwitchToEnglish()
        {
            InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new System.Globalization.CultureInfo("en-US"));
        }



        private void txtSearch_Enter(object sender, EventArgs e)
        {
            _languageManager.SetArabicLanguage(); // البحث غالباً إنجليزي
        }

        private void txtAccName_Enter(object sender, EventArgs e)
        {
            _languageManager.SetArabicLanguage(); // الأسماء بالعربي
        }

        private void txtAccNote_Enter(object sender, EventArgs e)
        {
            _languageManager.SetArabicLanguage(); // الملاحظات بالعربي
        }

        private void txtClientEmail_Enter(object sender, EventArgs e)
        {
            _languageManager.SetEnglishLanguage(); // الإيميل بالإنجليزي
        }

        private void txtClientAddress_Enter(object sender, EventArgs e)
        {
            _languageManager.SetArabicLanguage(); // العناوين بالعربي
        }
        #endregion

        #region *************** Calculate Asset  *** حساب الإهلاك **************
        bool isUpdating = false;

        private void CalculateDepreciation() => UpdateDepreciation();

        private void txtFixedAssetsValue_TextChanged(object sender, EventArgs e) => CalculateDepreciation();

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

        #region ************ Phone handlers ***  مع حماية من null **********


        private void ConnectEvents()
        {
            // أحداث الهاتف
            txtFirstPhon.KeyPress += PhoneTextBox_KeyPress;
            txtAntherPhon.KeyPress += PhoneTextBox_KeyPress;
            txtFirstPhon.TextChanged += PhoneTextBox_TextChanged;
            txtAntherPhon.TextChanged += PhoneTextBox_TextChanged;
            txtFirstPhon.Leave += PhoneTextBox_Leave;
            txtAntherPhon.Leave += PhoneTextBox_Leave;

        }

        private void PhoneTextBox_KeyPress(object? sender, KeyPressEventArgs e)
        {
            var txt = sender as System.Windows.Forms.TextBox;
            if (txt == null) return;

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

            // حساب عدد الأرقام فقط (من النص الحالي)
            string digitsOnly = new string(txt.Text.Where(char.IsDigit).ToArray());

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
            var txt = sender as System.Windows.Forms.TextBox;
            if (txt == null) return;

            // حذف أي أرقام زيادة بعد اللصق
            string digitsOnly = new string(txt.Text.Where(char.IsDigit).ToArray());

            if (digitsOnly.Length > 15)
            {
                int count = 0;
                var newText = new System.Text.StringBuilder();

                foreach (char c in txt.Text)
                {
                    if (char.IsDigit(c))
                    {
                        if (count < 15)
                        {
                            newText.Append(c);
                            count++;
                        }
                    }
                    else
                    {
                        newText.Append(c);
                    }
                }

                txt.Text = newText.ToString();
                // إعادة وضع المؤشر في نهاية النص
                txt.SelectionStart = txt.Text.Length;

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

        #region ******** btns Events  ازرار الشاشة *************

        // الغاء تحديدات التصنيفات
        private void btnClearLstSubAccTop_Click(object sender, EventArgs e)
        {
            AccountDGV(AccTopID);
            lstSubAccTop.SelectedIndex = -1;
            lblSubAccTop.Text = "0";
            DisplayNewRow();
        }

        //حذف حساب
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(lblAccID.Text, out int accID))
            {
                MessageBox.Show("رقم الحساب غير صالح.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int currentRowIndex = -1;
            foreach (DataGridViewRow row in DGV.Rows)
            {
                if (row.Cells["AccID"].Value != null && Convert.ToInt32(row.Cells["AccID"].Value) == accID)
                {
                    currentRowIndex = row.Index;
                    break;
                }
            }

            bool saveResult = DBServiecs.MainAcc_DeleteCatogryOrAcc(accID, out string resultMessage);

            if (saveResult)
                MessageBox.Show(resultMessage, "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                MessageBox.Show(resultMessage, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AccountDGV(AccTopID);
            DisplayNewRow();

            int newRowIndex = currentRowIndex;
            if (newRowIndex >= DGV.Rows.Count)
                newRowIndex = DGV.Rows.Count - 1;

            if (newRowIndex >= 0 && DGV.Rows.Count > 0)
            {
                DGV.ClearSelection();
                DGV.Rows[newRowIndex].Selected = true;
                DGV.FirstDisplayedScrollingRowIndex = newRowIndex;
            }
        }

        // اضافة تصنيف
        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            DialogResult result = CustomMessageBox.ShowStringInputBox(out string accName, "يرجى إدخال اسم التصنيف الفرعي:", "إضافة تصنيف جديد");

            if (result != DialogResult.OK || string.IsNullOrWhiteSpace(accName))
            {
                MessageBox.Show("يرجى إدخال اسم الحساب أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataTable tbl = DBServiecs.MainAcc_GetNewIDForCategory();
            int accID = Convert.ToInt32(tbl.Rows[0][0]);
            int? parentAccID = int.TryParse(lblAccTopID.Text, out int tempParentID) ? tempParentID : (int?)null;

            bool saveResult = DBServiecs.MainAcc_InsertSubCat(accID, parentAccID, accName, out string resultMessage);

            if (saveResult)
                MessageBox.Show(resultMessage, "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                MessageBox.Show(resultMessage, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AccTop_LoadFollowers();

            for (int i = 0; i < lstSubAccTop.Items.Count; i++)
            {
                if (lstSubAccTop.Items[i] is DataRowView row && Convert.ToInt32(row["AccID"]) == accID)
                {
                    lstSubAccTop.SelectedIndex = i;
                    break;
                }
            }
        }

        // تعديل تصنيف
        private void btnModifyCategory_Click(object sender, EventArgs e)
        {
            if (lstSubAccTop.SelectedItem is not DataRowView selectedRow || selectedRow["AccID"] == DBNull.Value)
            {
                MessageBox.Show("يرجى تحديد التصنيف الذي تريد تعديله أولاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int accID = Convert.ToInt32(selectedRow["AccID"]);

            DialogResult result = CustomMessageBox.ShowStringInputBox(out string accName, "يرجى إدخال اسم التصنيف الفرعي المعدل:", "تعديل تصنيف");

            if (result != DialogResult.OK || string.IsNullOrWhiteSpace(accName))
            {
                MessageBox.Show("يرجى إدخال اسم الحساب أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool saveResult = DBServiecs.MainAcc_UpdateSubCat(accID, accName, out string resultMessage);

            if (saveResult)
                MessageBox.Show(resultMessage, "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                MessageBox.Show(resultMessage, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AccTop_LoadFollowers();

            for (int i = 0; i < lstSubAccTop.Items.Count; i++)
            {
                if (lstSubAccTop.Items[i] is DataRowView row && Convert.ToInt32(row["AccID"]) == accID)
                {
                    lstSubAccTop.SelectedIndex = i;
                    break;
                }
            }
        }

        // حذف تصنيف
        private void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            if (lstSubAccTop.SelectedItem is not DataRowView selectedRow || selectedRow["AccID"] == DBNull.Value)
            {
                MessageBox.Show("يرجى تحديد التصنيف أولاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int accID = Convert.ToInt32(selectedRow["AccID"]);

            bool saveResult = DBServiecs.MainAcc_DeleteCatogryOrAcc(accID, out string resultMessage);

            if (saveResult)
                MessageBox.Show(resultMessage, "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(resultMessage, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);

            AccTop_LoadFollowers();
        }

        // حساب جديد
        private void btnNew_Click(object? sender, EventArgs e)
        {
            // أولاً: تحديث البيانات من القاعدة أو الفلترة (إذا لزم الأمر)
            FilterAccounts();

            // التحقق من أن جدول الحسابات محمّل
            if (tblAccDGV != null)
            {
                // إنشاء صف جديد في الجدول (بدون إضافته فعليًا بعد)
                tblRow = tblAccDGV.NewRow();

                // عرض بيانات الصف الجديد (تكون فارغة)
                DisplayNewRow();

                // تحديث حالة الإدخال الجديد
                newR = true;

                // إزالة التحديد من DataGridView
                DGV.ClearSelection();

                // تركيز المؤشر في أول حقل للإدخال
                txtAccName.Focus();
            }
            else
            {
                MessageBox.Show("حدث خطأ: جدول الحسابات غير محمّل.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            txtAccName.Focus();
        }

        //حفظ الاضافة
        private void btnSave_Click(object sender, EventArgs e)
        {
            string accName = txtAccName.Text.Trim();

            if (string.IsNullOrEmpty(accName))
            {
                MessageBox.Show("يرجى إدخال اسم الحساب أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAccName.Focus();
                return;
            }

            int accID;
            if (!int.TryParse(lblAccID.Text, out accID))
            {
                DataTable tblNewID = DBServiecs.MainAcc_GetNewID();
                lblAccID.Text = Convert.ToInt32(tblNewID.Rows[0][0]).ToString();
                accID = Convert.ToInt32(tblNewID.Rows[0][0]);
            }

            int? parentAccID;

            // محاولة التحويل إلى رقم من lblSubAccTop أولاً
            if (int.TryParse(lblSubAccTop.Text, out int subAccTopValue) && subAccTopValue > 0)
            {
                parentAccID = subAccTopValue;
            }
            else
            {
                // وإلا نأخذ القيمة الافتراضية من lblAccTopID
                parentAccID = int.TryParse(lblAccTopID.Text, out int accTopIDValue) ? accTopIDValue : (int?)null;
            }

            //     bool isHidden = chkHiddenAcc.Checked;
            float? fixedAssetsValue = float.TryParse(txtFixedAssetsValue.Text, out float f1) ? f1 : (float?)null;
            float? depreciationRateAnnually = float.TryParse(txtDepreciationRateAnnually.Text, out float f2) ? f2 : (float?)null;
            int? fixedAssetsAge = int.TryParse(txtFixedAssetsAge.Text, out int i1) ? i1 : (int?)null;
            float? annuallyInstallment = float.TryParse(lblAnnuallyInstallment.Text, out float f3) ? f3 : (float?)null;
            float? monthlyInstallment = float.TryParse(lblMonthlyInstallment.Text, out float f4) ? f4 : (float?)null;
            bool? isEndedFixedAssets = chkIsEndedFixedAssets.Checked;
            DateTime? fixedAssetsEndDate = DateTime.TryParse(lblFixedAssetsEndDate.Text, out DateTime tempEndDate) ? tempEndDate : (DateTime?)null;

            string firstPhon = txtFirstPhon.Text;
            string antherPhon = txtAntherPhon.Text;
            string accNote = txtAccNote.Text;
            string clientEmail = txtClientEmail.Text;
            string clientAddress = txtClientAddress.Text;

            bool result = DBServiecs.MainAcc_UpdateOrInsert(
     accID,
     parentAccID,
     accName,
     false,
     fixedAssetsValue,
     depreciationRateAnnually,
     fixedAssetsAge,
     annuallyInstallment,
     monthlyInstallment,
     isEndedFixedAssets,
     fixedAssetsEndDate,
     firstPhon,
     antherPhon,
     accNote,
     clientEmail,
     clientAddress,
     CurrentSession.UserID, // ⬅️ 
     out string resultMessage
 );


            if (result)
            {
                MessageBox.Show(resultMessage, "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisplayNewRow();

            }
            else
            {
                MessageBox.Show(resultMessage, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AccountDGV(AccTopID);

            foreach (DataGridViewRow row in DGV.Rows)
            {
                if (row.Cells["AccID"].Value != null && (int)row.Cells["AccID"].Value == accID)
                {
                    DGV.ClearSelection();
                    row.Selected = true;
                    DGV.FirstDisplayedScrollingRowIndex = row.Index;

                    // تعيين أول خلية مرئية كـ CurrentCell
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Visible)
                        {
                            DGV.CurrentCell = cell;
                            break;
                        }
                    }

                    break;
                }
            }



            //         chkNew.Checked = false;

            // تحميل بيانات الحساب المحدد
            LoadAccountDetails(accID.ToString());
        }

        // تعديل حساب
        private void btnModify_Click(object sender, EventArgs e)
        {
            if (DGV.CurrentRow == null)
            {
                MessageBox.Show("يرجى تحديد صف لتعديله");
                return;
            }

            // حفظ القيم قبل فتح شاشة التعديل
            int accID = Convert.ToInt32(DGV.CurrentRow.Cells["AccID"].Value);
            bool IsAssests = hasFixedAssets;
            bool isHasDetails = hasDetails;

            using (frmModifyAcc frm = new frmModifyAcc(accID, IsAssests, isHasDetails))
            {
                if (frm.ShowDialog() == DialogResult.OK)//لا يدخل الشرط هنا ويتخطى دالة التحديث
                {
                    // تحديث البيانات من قاعدة البيانات وإعادة التحديد
                    RefreshAccountData(accID);
                }
            }
        }



        #endregion

        #region الإعداد والتحميل

        /*
        رموز التقارير الرئيسية:
        0 = التقارير الختامية العامة
        الصندوق = 3
        المدينون = 6
        العملاء = 7
        الأصول الثابتة = 9
        الدائنون = 13
        الموردين = 14
        المصروفات = 19
        العاملون = 22
        جاري الشركاء = 39

        200 = التقارير للاصناف
        */

        // تحميل بيانات الحساب الأعلى وتحديد خصائص العرض المرتبطة به

        private void TypAcc()
        {
            tblAccTop = DBServiecs.MainAcc_LoadTopByID(AccTopID);

            if (tblAccTop != null && tblAccTop.Rows.Count > 0)
            {
                DataRow row = tblAccTop.Rows[0];

                lblAccTopID.Text = row["AccID"].ToString();
                lblTitel.Text = row["AccName"].ToString();

                // قراءة نوع الحساب من الحقول
                hasDetails = row["Is_HasDetails"] != DBNull.Value && Convert.ToBoolean(row["Is_HasDetails"]);
                hasFixedAssets = row["Is_FixedAssets"] != DBNull.Value && Convert.ToBoolean(row["Is_FixedAssets"]);

                if (hasFixedAssets)
                {
                    // 🟦 الحالة 1: أصول ثابتة فقط
                    SetViewState(
                        showPhon: false,
                        showMainData: false,
                        showFixedAssets: true,
                        showAssetsData: true,
                        row1Height: 0F,
                        row2Height: 100F
                    );
                }
                else if (hasDetails)
                {
                    // 🟦 الحالة 2: حساب عادي بتفاصيل
                    SetViewState(
                        showPhon: true,
                        showMainData: true,
                        showFixedAssets: false,
                        showAssetsData: false,
                        row1Height: 100F,
                        row2Height: 0F
                    );
                }
                else
                {
                    // 🟦 الحالة 3: حساب عادي بدون تفاصيل
                    SetViewState(
                        showPhon: false,
                        showMainData: false,
                        showFixedAssets: false,
                        showAssetsData: false,
                        row1Height: 100F,
                        row2Height: 0F
                    );
                }
            }
        }

        /// دالة موحدة لضبط العناصر والارتفاعات حسب نوع الحساب
        private void SetViewState(
            bool showPhon,
            bool showMainData,
            bool showFixedAssets,
            bool showAssetsData,
            float row1Height,
            float row2Height)
        {
            // إظهار/إخفاء العناصر
            tlpPhon.Visible = showPhon;
            tlpMainData.Visible = showMainData;
            tlpFixedAssets.Visible = showFixedAssets;
            tlpAssetsData.Visible = showAssetsData;

            // إجبار Dock لملء المكان
            tlpFixedAssets.Dock = DockStyle.Top;
            tlpAssetsData.Dock = DockStyle.Top;

            // BringToFront في حالة العرض
            if (showFixedAssets) tlpFixedAssets.BringToFront();
            if (showAssetsData) tlpAssetsData.BringToFront();

            // ضبط ارتفاعات tlpData
            tlpData.RowStyles[0].Height = row1Height;
            tlpData.RowStyles[1].Height = row2Height;
            tlpData.RowStyles[0].SizeType = SizeType.Percent;
            tlpData.RowStyles[1].SizeType = SizeType.Percent;
            tlpData.PerformLayout();

            // ضبط ارتفاعات tlpDetailsData
            tlpDetailsData.RowStyles[0].Height = row1Height;
            tlpDetailsData.RowStyles[1].Height = row2Height;
            tlpDetailsData.RowStyles[0].SizeType = SizeType.Percent;
            tlpDetailsData.RowStyles[1].SizeType = SizeType.Percent;
            tlpDetailsData.PerformLayout();

            // تعيين ارتفاع فقط للعناصر الظاهرة
            int sectionHeight = 230;
            if (showFixedAssets) tlpFixedAssets.Height = sectionHeight;
            if (showAssetsData) tlpAssetsData.Height = sectionHeight;
            if (showPhon) tlpPhon.Height = sectionHeight;
            if (showMainData) tlpMainData.Height = sectionHeight;
        }








        private void FillcbxChangeCat() // تعبئة الكمبوبوكس بالحسابات الرئيسية والفرعية
        {
            DataTable dt = DBServiecs.MainAcc_LoadFollowersAndParent(AccTopID);
            cbxChangeCat.DataSource = dt;
            cbxChangeCat.DisplayMember = "AccName";
            cbxChangeCat.ValueMember = "AccID";
            cbxChangeCat.SelectedIndex = -1;
            cbxChangeCat.DropDownWidth = 100;
        }

        #endregion

        #region تنسيق DataGridView


        private void DGV_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e) // إعادة تلوين الصف حسب رقم الحساب
        {
            try
            {
                if (DGV.Columns.Contains("AccID"))
                {
                    var accIdValue = DGV.Rows[e.RowIndex].Cells["AccID"].Value;
                    if (accIdValue != null && int.TryParse(accIdValue.ToString(), out int accId))
                    {
                        if (accId < 70)
                        {
                            DGV.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                        }
                    }
                }
            }
            catch
            {
                // تجاهل الأخطاء بالرسم
            }
        }

        #endregion

        #region بيانات السطر الجديد

        public void DisplayNewRow() // تجهيز السطر الجديد للإضافة
        {
            DataTable tblNewID = DBServiecs.MainAcc_GetNewID();
            lblAccID.Text = Convert.ToInt32(tblNewID.Rows[0][0]).ToString();
            txtAccName.Text = "";
            txtFirstPhon.Text = "";
            txtAntherPhon.Text = "";
            txtAccNote.Text = "";
            txtClientEmail.Text = "";
            txtClientAddress.Text = "";

            txtFixedAssetsValue.Text = "0";
            txtDepreciationRateAnnually.Text = "0";
            txtFixedAssetsAge.Text = "0";
            lblAnnuallyInstallment.Text = "0";
            lblMonthlyInstallment.Text = "0";
            lblSubAccTopName.Text = "0";
            lblFirstLine.Text = "-";
            lblNameLine.Text = "-";
            lblPhonAndAnther.Text = "-";
            lblClientEmail.Text = "-";
            lblClientAddress.Text = "-";
            lblAccNote.Text = "-";
            lblAssetsValue_DepreciationRateAnnually.Text = "-";
            lblAnnuallyInstallment.Text = "-";
            lblAnnuallyInstallment_MonthlyInstallment.Text = "-";
            lblFixedAssetsAge.Text = "-";


            rdoIsEndedFixedAssets_Yes.Checked = false;
            rdoIsEndedFixedAssets_No.Checked = false;

            DGV.ClearSelection();
        }

        #endregion

        #region ===== أدوات التقارير (Report Tools) =====

        // تعريف كلاس ليمثل بيانات التقرير بشكل منظم
        public class ReportInfo
        {
            public int ReportID { get; set; }
            public string ReportCodeName { get; set; } = "";
            public string ReportDisplayName { get; set; } = "";
            public bool IsGrouped { get; set; }
        }

        // إنشاء شريط القوائم
        private void SetupMenuStrip()
        {
            MenuStrip mainMenu = new MenuStrip
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightSteelBlue,
                Font = new Font("Times New Roman", 14, FontStyle.Regular)
            };

            // القوائم الرئيسية للتقارير
            tsmiCategoryReports = new ToolStripMenuItem("تقارير الحساب المحدد ▼");
            tsmiGroupedReports = new ToolStripMenuItem("تقارير مجمعة للحسابات المحددة ▼");

            mainMenu.Items.Add(tsmiCategoryReports);
            mainMenu.Items.Add(tsmiGroupedReports);

            pnlMenuContainer.Controls.Add(mainMenu);
            mainMenu.Location = new Point(10, 5);

            DGV.Dock = DockStyle.Fill;
        }

        // تحميل التقارير من ReportsMaster
        private void LoadReports(int topAcc)
        {
            try
            {
                // تأكد أن القوائم تم إنشاؤها
                if (tsmiCategoryReports == null || tsmiGroupedReports == null)
                {
                    SetupMenuStrip(); // إنشاء القوائم إذا لم تكن موجودة
                }

                // جلب البيانات من قاعدة البيانات
                DataTable dt = DBServiecs.Reports_GetByTopAcc(topAcc, false);

                // تقسيم البيانات (فردية / مجمعة)
                DataRow[] individualReports = dt.Select("IsGrouped = 0");
                DataRow[] groupedReports = dt.Select("IsGrouped = 1");

                if (tsmiCategoryReports != null)
                    LoadMenuItemsFromDataRows(tsmiCategoryReports, individualReports);

                if (tsmiGroupedReports != null)
                    LoadMenuItemsFromDataRows(tsmiGroupedReports, groupedReports);
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء تحميل التقارير: " + ex.Message);
            }
        }

        // تعبئة عناصر القائمة من DataRow[]
        private void LoadMenuItemsFromDataRows(ToolStripMenuItem parentMenu, DataRow[] rows)
        {
            parentMenu.DropDownItems.Clear();

            if (rows.Length == 0)
            {
                // لا توجد تقارير
                ToolStripMenuItem emptyItem = new ToolStripMenuItem("لا توجد تقارير متاحة")
                {
                    Enabled = false
                };
                parentMenu.DropDownItems.Add(emptyItem);
                return;
            }

            foreach (DataRow row in rows)
            {
                // تكوين عنصر القائمة
                ToolStripMenuItem menuItem = new ToolStripMenuItem(row["ReportDisplayName"].ToString())
                {
                    // تخزين بيانات التقرير داخل ReportInfo في خاصية Tag
                    Tag = new ReportInfo
                    {
                        ReportID = Convert.ToInt32(row["ReportID"]),
                        ReportCodeName = row["ReportCodeName"].ToString() ?? "",
                        ReportDisplayName = row["ReportDisplayName"].ToString() ?? "",
                        IsGrouped = Convert.ToBoolean(row["IsGrouped"])
                    }
                };

                // ربط حدث الضغط
                menuItem.Click += MenuItem_Click;
                parentMenu.DropDownItems.Add(menuItem);
            }
        }

        // حدث النقر على عنصر تقرير
        private void MenuItem_Click(object? sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem clickedItem) return;

            // الآن Tag يحتوي على ReportInfo وليس DataRow
            if (clickedItem.Tag is ReportInfo report)
            {
                try
                {
                    // تجهيز الديكشنري (parameters) لتمريرها إلى شاشة التقرير
                    Dictionary<string, object> reportParams = new Dictionary<string, object>
                    {
                        ["ReportCodeName"] = report.ReportCodeName,
                        ["ReportDisplayName"] = report.ReportDisplayName,
                        ["ReportID"] = report.ReportID
                    };

                    // حسب نوع التقرير (فردي أو مجمع)
                    if (report.IsGrouped == false)
                    {
                        // تقرير حساب فردي → يمرر الـ AccountID الحالي
                        reportParams["AccountID"] = GetCurrentEntityID() ?? DBNull.Value;
                    }
                    else
                    {
                        // تقرير مجمع → يمرر قائمة الحسابات المختارة
                        reportParams["AccountsList"] = GetFilteredData() ?? new DataTable();
                    }

                    // فتح شاشة إعدادات التقرير
                    using frmSettingReports previewForm = new frmSettingReports(reportParams);
                    previewForm.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"حدث خطأ أثناء تحضير التقرير: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // إرجاع معرف الحساب الحالي من الواجهة
        private object? GetCurrentEntityID()
        {
            return string.IsNullOrEmpty(lblAccID.Text) ? null : (object)Convert.ToInt32(lblAccID.Text);
        }

        // تجهيز البيانات المصفاة للحسابات (تستخدم مع التقارير المجمعة)
        private DataTable? GetFilteredData()
        {
            DataTable result = new DataTable();
            result.Columns.Add("ID", typeof(int));
            result.Columns.Add("Name", typeof(string));

            DataGridView? sourceGrid = DGV;
            string idColumn = "AccID";
            string nameColumn = "AccName";

            if (sourceGrid != null)
            {
                if (sourceGrid.SelectedRows.Count > 1)
                {
                    // لو المستخدم اختار أكثر من صف
                    foreach (DataGridViewRow row in sourceGrid.SelectedRows)
                    {
                        if (!row.IsNewRow && row.Cells[idColumn].Value != null)
                        {
                            result.Rows.Add(
                                Convert.ToInt32(row.Cells[idColumn].Value),
                                row.Cells[nameColumn].Value?.ToString() ?? ""
                            );
                        }
                    }
                }
                else
                {
                    // لو مفيش اختيار → نأخذ كل الصفوف
                    foreach (DataGridViewRow row in sourceGrid.Rows)
                    {
                        if (!row.IsNewRow && row.Cells[idColumn].Value != null)
                        {
                            result.Rows.Add(
                                Convert.ToInt32(row.Cells[idColumn].Value),
                                row.Cells[nameColumn].Value?.ToString() ?? ""
                            );
                        }
                    }
                }
            }

            return result;
        }

        #endregion



        /*
        #region ===== أدوات التقارير (Report Tools) =====

        // إنشاء شريط القوائم
        private void SetupMenuStrip()
        {
            MenuStrip mainMenu = new MenuStrip
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightSteelBlue,
                Font = new Font("Times New Roman", 14, FontStyle.Regular)
            };

            tsmiCategoryReports = new ToolStripMenuItem("تقارير الحساب المحدد ▼");
            tsmiGroupedReports = new ToolStripMenuItem("تقارير مجمعة للحسابات المحددة ▼");

            mainMenu.Items.Add(tsmiCategoryReports);
            mainMenu.Items.Add(tsmiGroupedReports);

            pnlMenuContainer.Controls.Add(mainMenu);
            mainMenu.Location = new Point(10, 5);

            DGV.Dock = DockStyle.Fill;
        }

        // تحميل التقارير من ReportsMaster
        private void LoadReports(int topAcc)
        {
            try
            {
                // تأكد أن القوائم تم إنشاؤها
                if (tsmiCategoryReports == null || tsmiGroupedReports == null)
                {
                    SetupMenuStrip(); // إنشاء القوائم إذا لم تكن موجودة
                }

                // جلب البيانات (المفعلة فقط false أو الجميع true حسب ما تحتاج)
                DataTable dt = DBServiecs.Reports_GetByTopAcc(topAcc, false);

                // تقسيم البيانات
                DataRow[] individualReports = dt.Select("IsGrouped = 0");
                DataRow[] groupedReports = dt.Select("IsGrouped = 1");

                if (tsmiCategoryReports != null)
                    LoadMenuItemsFromDataRows(tsmiCategoryReports, individualReports);

                if (tsmiGroupedReports != null)
                    LoadMenuItemsFromDataRows(tsmiGroupedReports, groupedReports);
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء تحميل التقارير: " + ex.Message);
            }
        }

        // تعبئة عناصر القائمة من DataRow[]
        private void LoadMenuItemsFromDataRows(ToolStripMenuItem parentMenu, DataRow[] rows)
        {
            parentMenu.DropDownItems.Clear();

            if (rows.Length == 0)
            {
                ToolStripMenuItem emptyItem = new ToolStripMenuItem("لا توجد تقارير متاحة")
                {
                    Enabled = false
                };
                parentMenu.DropDownItems.Add(emptyItem);
                return;
            }

            foreach (DataRow row in rows)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem(row["ReportDisplayName"].ToString())
                {
                    Tag = row["ReportCodeName"] // تمرير كود التقرير
                };
                menuItem.Click += MenuItem_Click;
                parentMenu.DropDownItems.Add(menuItem);
            }
        }

        // حدث النقر على التقرير
        private void MenuItem_Click(object? sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem clickedItem) return;

            if (clickedItem.Tag is DataRow reportRow)//لماذا يخرج من الشرط
            {
                try
                {
                    // تجهيز الديكشنري
                    Dictionary<string, object> reportParams = new Dictionary<string, object>();

                    // بيانات أساسية للتقرير
                    reportParams["ReportCodeName"] = reportRow["ReportCodeName"].ToString() ?? "";
                    reportParams["ReportDisplayName"] = reportRow["ReportDisplayName"].ToString() ?? "";
                    reportParams["ReportID"] = Convert.ToInt32(reportRow["ReportID"]);

                    // بيانات الحساب أو الحسابات
                    if (Convert.ToBoolean(reportRow["IsGrouped"]) == false)
                    {
                        // تقرير حساب فردي
                        reportParams["AccountID"] = GetCurrentEntityID() ?? DBNull.Value;
                    }
                    else
                    {
                        // تقرير مجمع
                        reportParams["AccountsList"] = GetFilteredData() ?? new DataTable();
                    }

                    using frmSettingReports previewForm = new frmSettingReports(reportParams);
                    previewForm.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"حدث خطأ أثناء تحضير التقرير: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private object? GetCurrentEntityID()
        {
            return string.IsNullOrEmpty(lblAccID.Text) ? null : (object)Convert.ToInt32(lblAccID.Text);
        }

        // تجهيز البيانات المصفاة
        private DataTable? GetFilteredData()
        {
            DataTable result = new DataTable();
            result.Columns.Add("ID", typeof(int));
            result.Columns.Add("Name", typeof(string));

            DataGridView? sourceGrid = DGV;
            string idColumn = "AccID";
            string nameColumn = "AccName";

            if (sourceGrid != null)
            {
                if (sourceGrid.SelectedRows.Count > 1)
                {
                    foreach (DataGridViewRow row in sourceGrid.SelectedRows)
                    {
                        if (!row.IsNewRow && row.Cells[idColumn].Value != null)
                        {
                            result.Rows.Add(
                                Convert.ToInt32(row.Cells[idColumn].Value),
                                row.Cells[nameColumn].Value?.ToString() ?? ""
                            );
                        }
                    }
                }
                else
                {
                    foreach (DataGridViewRow row in sourceGrid.Rows)
                    {
                        if (!row.IsNewRow && row.Cells[idColumn].Value != null)
                        {
                            result.Rows.Add(
                                Convert.ToInt32(row.Cells[idColumn].Value),
                                row.Cells[nameColumn].Value?.ToString() ?? ""
                            );
                        }
                    }
                }
            }

            return result;
        }

        #endregion
        */

        #region ======= عرض بيانات الحساب =======

        // ✅ دالة مركزية لملء الحقول من DataRow
        private void FillAccountFields(DataRow row)
        {
            // 🟡 السطر الأول: رقم الحساب - تاريخ الانضمام - الحالة (فعال / معطل)
            string accID = row["AccID"]?.ToString() ?? "غير متوفر";
            string dateOfJoin = row["DateOfJoin"] != DBNull.Value
                ? Convert.ToDateTime(row["DateOfJoin"]).ToString("yyyy/MM/dd")
                : "غير متوفر";
            bool isHidden = row["IsHidden"] != DBNull.Value && Convert.ToBoolean(row["IsHidden"]);
            string status = isHidden ? "معطل" : "فعال";
            // اجلب المعرف القديم
            string sorceIDAcc = row["SorceIDAcc"]?.ToString() ?? "غير متوفر";

            // عرض السطر في اللابل
            lblFirstLine.Text = $"رقم الحساب: {accID}    معرف قديم: {sorceIDAcc}   ";
            lblLastLine.Text = $"تاريخ الانضمام: {dateOfJoin}    الحالة: {status}";

            // 🟢 السطر الثاني: اسم الحساب - الرصيد - حالة الرصيد
            string accName = row["AccName"]?.ToString() ?? "غير معروف";
            string balance = row["Balance"] != DBNull.Value
                ? string.Format("{0:N2}", Convert.ToDecimal(row["Balance"]))
                : "0.00";
            string balanceState = row["BalanceState"]?.ToString() ?? "غير محددة";

            lblNameLine.Text = $"الاسم: {accName}    الرصيد: {balance}    الحالة: {balanceState}";

            // 🔵 معلومات الهاتف إن وجدت
            string? firstPhon = row["FirstPhon"]?.ToString();
            string? antherPhon = row["AntherPhon"]?.ToString();
            string phoneInfo = "";
            if (!string.IsNullOrWhiteSpace(firstPhon))
                phoneInfo += $"الهاتف: {firstPhon}";
            if (!string.IsNullOrWhiteSpace(antherPhon))
                phoneInfo += $"   هاتف آخر: {antherPhon}";

            lblPhonAndAnther.Text = phoneInfo;

            // 🔴 البريد الإلكتروني والعنوان إن وجدا
            string? email = row["ClientEmail"]?.ToString();
            lblClientEmail.Text = !string.IsNullOrWhiteSpace(email) ? $"البريد الإلكتروني: {email}" : "";

            string? address = row["ClientAddress"]?.ToString();
            lblClientAddress.Text = !string.IsNullOrWhiteSpace(address) ? $"العنوان: {address}" : "";

            // 🟠 ملاحظات الحساب
            string? note = row["AccNote"]?.ToString();
            lblAccNote.Text = !string.IsNullOrWhiteSpace(note) ? $"ملاحظات: {note}" : "";

            // 🟣 معلومات الأصل الثابت
            string fixedAssetsValue = row["FixedAssetsValue"] != DBNull.Value
                ? string.Format("{0:N2}", Convert.ToDecimal(row["FixedAssetsValue"]))
                : "0.00";
            string depreciationRate = row["DepreciationRateAnnually"] != DBNull.Value
                ? row["DepreciationRateAnnually"].ToString() + "%"
                : "0%";

            lblAssetsValue_DepreciationRateAnnually.Text = $"قيمة الأصل: {fixedAssetsValue}    معدل الإهلاك السنوي: {depreciationRate}";

            string? fixedAssetsAge = row["FixedAssetsAge"]?.ToString();

            if (int.TryParse(fixedAssetsAge, out int months))
            {
                int years = months / 12;
                int remainingMonths = months % 12;

                string formattedAge = "";

                if (years > 0)
                    formattedAge += $"{years} {(years == 1 ? "سنة" : (years <= 2 ? "سنتين" : "سنوات"))}";

                if (remainingMonths > 0)
                {
                    if (!string.IsNullOrEmpty(formattedAge))
                        formattedAge += " و ";

                    formattedAge += $"{remainingMonths} {(remainingMonths == 1 ? "شهر" : (remainingMonths == 2 ? "شهرين" : "شهور"))}";
                }

                lblFixedAssetsAge.Text = $"العمر الافتراضي: {months} شهر" + (string.IsNullOrEmpty(formattedAge) ? "" : $" ({formattedAge})");
            }
            else
            {
                lblFixedAssetsAge.Text = "العمر الافتراضي: غير معروف";
            }

            string monthlyInstallment = row["MonthlyInstallment"] != DBNull.Value
                ? string.Format("{0:N2}", Convert.ToDecimal(row["MonthlyInstallment"]))
                : "0.00";
            string annuallyInstallment = row["AnnuallyInstallment"] != DBNull.Value
                ? string.Format("{0:N2}", Convert.ToDecimal(row["AnnuallyInstallment"]))
                : "0.00";

            lblAnnuallyInstallment_MonthlyInstallment.Text = $"القسط الشهري: {monthlyInstallment}    القسط السنوي: {annuallyInstallment}";

            // ⚪ هل انتهى الأصل الثابت وتاريخ انتهائه إن وجد
            bool? isEndedFixedAssets = null;
            if (row["IsEndedFixedAssets"] != DBNull.Value)
                isEndedFixedAssets = Convert.ToBoolean(row["IsEndedFixedAssets"]);

            string fixedAssetsEndDate = row["FixedAssetsEndDate"] != DBNull.Value
                ? Convert.ToDateTime(row["FixedAssetsEndDate"]).ToString("yyyy/MM/dd")
                : "";

            string endStatus = isEndedFixedAssets == true
                ? $"تم إنهاء الأصل في: {fixedAssetsEndDate}"
                : "الأصل لم ينتهِ بعد";

            lblIsEndedFixedAssets_FixedAssetsEndDate.Text = endStatus;

            // تحديث الراديو بوتون حسب حالة انتهاء الأصل
            if (isEndedFixedAssets.HasValue)
            {
                rdoIsEndedFixedAssets_Yes.Checked = isEndedFixedAssets.Value;
                rdoIsEndedFixedAssets_No.Checked = !isEndedFixedAssets.Value;
            }
            else
            {
                rdoIsEndedFixedAssets_Yes.Checked = false;
                rdoIsEndedFixedAssets_No.Checked = false;
            }
        }


        #endregion

        #region ======= DGV Events =======

        private void DGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || tblAccDGV == null) return;

            object? cellValue = DGV.Rows[e.RowIndex].Cells["AccID"].Value;
            if (cellValue is null || cellValue == DBNull.Value) return;

            string selectedAccID = cellValue.ToString() ?? "";
            DataRow[] matchedRows = tblAccDGV.Select($"AccID = '{selectedAccID}'");

            if (matchedRows.Length > 0)
            {
                FillAccountFields(matchedRows[0]);
            }
        }

        private void DGV_SelectionChanged(object sender, EventArgs e)
        {
            if (tblAccDGV == null || !DGV.Focused || DGV.SelectedRows.Count == 0)
                return;

            object? cellValue = DGV.SelectedRows[0].Cells["AccID"].Value;
            if (cellValue is null || cellValue == DBNull.Value)
                return;

            string selectedAccID = cellValue.ToString() ?? "";
            DataRow[] matchedRows = tblAccDGV.Select($"AccID = '{selectedAccID}'");

            if (matchedRows.Length > 0)
            {
                FillAccountFields(matchedRows[0]);
            }
        }

        #endregion

        #region ======= LoadAccountDetails باستخدام accID =======

        private void LoadAccountDetails(string accID)
        {
            if (tblAccDGV == null)
            {
                MessageBox.Show("جدول الحسابات غير محمّل.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataRow[] matchedRows = tblAccDGV.Select($"AccID = '{accID}'");

            if (matchedRows.Length > 0)
            {
                FillAccountFields(matchedRows[0]);
            }
        }

        #endregion


        private void lstSubAccTop_SelectedIndexChanged(object sender, EventArgs e)
        {
            AccountDGV(AccTopID);
            if (lstSubAccTop.SelectedValue == null || lstSubAccTop.SelectedValue is DataRowView)
                return;

            if (int.TryParse(lstSubAccTop.SelectedValue.ToString(), out int selectedAccID))
            {
                if (tblAccDGV == null)
                {
                    MessageBox.Show("جدول الحسابات غير محمّل.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                lblSubAccTop.Text = selectedAccID.ToString();
                DataView dv = tblAccDGV.DefaultView;
                dv.RowFilter = $"ParentAccID = {selectedAccID}";
                DGV.DataSource = dv;
            }

            cbxChangeCat.SelectedIndex = -1;
            DisplayNewRow();
        }

        private void rdoIsEndedFixedAssets_No_CheckedChanged(object sender, EventArgs e)
        {
            //isEndedFixedAssets = true;
            lblFixedAssetsEndDate.Text = null;
            chkIsEndedFixedAssets.Checked = false;
        }

        private void rdoIsEndedFixedAssets_Yes_CheckedChanged(object sender, EventArgs e)
        {
            //isEndedFixedAssets = false;
            lblFixedAssetsEndDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
            chkIsEndedFixedAssets.Checked = true;
        }

        private void cbxChangeCat_SelectedValueChanged(object sender, EventArgs e)
        {
            //if (cbxChangeCat.SelectedValue != null)
            //    lblSubAccTop.Text = cbxChangeCat.SelectedValue.ToString();
            //else
            //    lblSubAccTop.Text = "0";

            //lstSubAccTop.ClearSelected();
        }
        private void btnChangeCat_Click(object sender, EventArgs e)
        {
            if (cbxChangeCat.SelectedValue == null)
            {
                MessageBox.Show("يرجى تحديد التصنيف الذي تريد نقل الأصناف إليه", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int newP = Convert.ToInt32(cbxChangeCat.SelectedValue);

            // جمع الأكواد المحددة من الـ DGV
            List<string> selectedAccIDs = new List<string>();
            foreach (DataGridViewRow row in DGV.SelectedRows)
            {
                object? accIdVal = row.Cells["AccID"].Value;
                if (accIdVal != null && !string.IsNullOrWhiteSpace(accIdVal.ToString()))
                {
                    selectedAccIDs.Add(accIdVal.ToString()!);
                }
            }

            if (selectedAccIDs.Count == 0)
            {
                MessageBox.Show("يرجى تحديد الحسابات التي تريد نقلها", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // تحويل القائمة إلى نص مفصول بفواصل
            string accIDs = string.Join(",", selectedAccIDs);

            // استدعاء الإجراء
            string resultMessage;
            bool success = DBServiecs.MainAcc_ChangAccCat(newP, accIDs, out resultMessage);


            // عرض النتيجة
            if (success)
            {
                MessageBox.Show(resultMessage, "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefrechCat(newP); // 👈 كده هيتنفذ بس لو success = true
            }
            else
            {
                MessageBox.Show(resultMessage, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        #region تبويب مخصص لتنسيق الـ Tabs
        private void tabControlAccount_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tabControl = (TabControl)sender;
            TabPage page = tabControl.TabPages[e.Index];

            Font font = new Font("Times New Roman", 14F, FontStyle.Bold);
            Brush textBrush = Brushes.White;

            Color backColor = (e.State & DrawItemState.Selected) == DrawItemState.Selected
                ? Color.DarkBlue
                : Color.LightSteelBlue;

            Rectangle tabBounds = tabControl.RightToLeft == RightToLeft.Yes
                ? new Rectangle(tabControl.Width - e.Bounds.Right, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height)
                : e.Bounds;

            using (SolidBrush backgroundBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backgroundBrush, tabBounds);
            }

            StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            e.Graphics.DrawString(page.Text, font, textBrush, tabBounds, sf);
        }
        #endregion



        #region ***************   البحث و تحديث  و تحميل البيانات  ********************         

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

            string searchText = txtSearch.Text.ToLower();

            // الحصول على DataView الحالي أو إنشاء واحد جديد إذا كان DataSource من نوع DataTable
            DataView dv;

            if (DGV.DataSource is DataView existingDataView)
            {
                dv = existingDataView;
            }
            else if (DGV.DataSource is DataTable dataTable)
            {
                dv = new DataView(dataTable);
            }
            else
            {
                // في حال عدم القدرة على تحديد نوع DataSource بشكل صحيح
                return;
            }

            if (string.IsNullOrWhiteSpace(searchText))
            {
                // إذا كان مربع البحث فارغًا، قم بإعادة ضبط الفلترة لعرض جميع العناصر
                dv.RowFilter = string.Empty;
            }
            else
            {
                // تصفية البيانات بناءً على نص البحث
                dv.RowFilter = $"AccName LIKE '%{searchText}%'";
            }

            // تحديث مصدر البيانات لـ DataGridView
            DGV.DataSource = dv;

        }

        private void RefreshAccountData(int accIDToSelect)
        {
            string balanceType = rdoMadeen.Checked ? "POS" :
                                 rdoDaeen.Checked ? "NEG" :
                                 rdoEqual.Checked ? "ZERO" : "All";

            if (!int.TryParse(lblAccTopID.Text, out int topID))
                return;

            if (int.TryParse(lblSubAccTop.Text, out int subTopID) && subTopID > 0)
                topID = subTopID;

            // تحميل البيانات الجديدة من SQL
            DataTable freshData = DBServiecs.MainAcc_LoadFinalAccounts(topID, balanceType);

            // تحديث النسخة الرئيسية
            tblAccDGV = freshData;
            DGV.DataSource = tblAccDGV;
            DGVStyl();

            // إعادة التحديد على الحساب المعدل
            foreach (DataGridViewRow row in DGV.Rows)
            {
                if (row.Cells["AccID"].Value != null &&
                    Convert.ToInt32(row.Cells["AccID"].Value) == accIDToSelect)
                {
                    // إيجاد أول عمود ظاهر
                    DataGridViewColumn? firstVisibleColumn = DGV.Columns
                        .Cast<DataGridViewColumn>()
                        .FirstOrDefault(c => c.Visible);

                    if (firstVisibleColumn != null)
                    {
                        DGV.CurrentCell = row.Cells[firstVisibleColumn.Index];
                        LoadSelectedAccountDetails();
                    }

                    row.Selected = true;
                    break;
                }
            }
            UpdateBalanceSummary();
        }

        private void UpdateBalanceSummary()
        {
            decimal totalMadeen = 0;  // الموجب
            decimal totalDaeen = 0;   // السالب
            int recordCount = 0;

            // التحقق من أن مصدر البيانات الحالي هو DataView
            if (DGV.DataSource is DataView dv)
            {
                foreach (DataRowView rowView in dv)
                {
                    if (rowView["Balance"] != DBNull.Value)
                    {
                        decimal balance = Convert.ToDecimal(rowView["Balance"]);
                        if (balance > 0)
                            totalMadeen += balance;
                        else if (balance < 0)
                            totalDaeen += Math.Abs(balance); // ناخذ القيمة المطلقة
                    }
                    recordCount++;
                }
            }
            else if (DGV.DataSource is DataTable dt)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["Balance"] != DBNull.Value)
                    {
                        decimal balance = Convert.ToDecimal(row["Balance"]);
                        if (balance > 0)
                            totalMadeen += balance;
                        else if (balance < 0)
                            totalDaeen += Math.Abs(balance);
                    }
                    recordCount++;
                }
            }

            // عرض النتيجة بالصيغة المطلوبة
            lblCountAndTotals.Text =
                $"عدد: {recordCount:N0}   اجمالى مدين: {totalMadeen:N2}   اجمالى دائن: {totalDaeen:N2}";
        }


        private void LoadSelectedAccountDetails()
        {
            if (tblAccDGV == null || DGV.SelectedRows.Count == 0)
                return;

            object? cellValue = DGV.SelectedRows[0].Cells["AccID"].Value;
            if (cellValue == null || cellValue == DBNull.Value)
                return;

            string selectedAccID = cellValue.ToString() ?? "";
            DataRow[] matchedRows = tblAccDGV.Select($"AccID = '{selectedAccID}'");

            if (matchedRows.Length > 0)
            {
                FillAccountFields(matchedRows[0]);
            }
        }

        private void connectRDO()
        {
            rdoAll.Tag = "All";
            rdoMadeen.Tag = "POS";
            rdoDaeen.Tag = "NEG";
            rdoEqual.Tag = "ZERO";

            rdoAll.CheckedChanged += rdo_CheckedChanged;
            rdoMadeen.CheckedChanged += rdo_CheckedChanged;
            rdoDaeen.CheckedChanged += rdo_CheckedChanged;
            rdoEqual.CheckedChanged += rdo_CheckedChanged;
        }
        private void LoadAccountsByBalanceType(int topID, string balanceType)
        {
            txtSearch.Text = "";
            tblAccDGV = DBServiecs.MainAcc_LoadFinalAccounts(topID, balanceType);
            DGV.DataSource = tblAccDGV;
            DGVStyl();
            DGV.ClearSelection();
        }

        // تحميل الحسابات بحسب النوع المحدد في الراديو
        private void rdo_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is RadioButton rdo && rdo.Checked)
            {
                string balanceType = rdo.Tag?.ToString() ?? "All";

                // تأكد من أن TopID محدث أولاً (إذا لزم الأمر استدعاء دالة لتحديثه)
                if (!int.TryParse(lblAccTopID.Text, out int topID))
                {
                    MessageBox.Show("قيمة lblAccTopID غير صحيحة. يرجى التحقق.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (int.TryParse(lblSubAccTop.Text, out int subTopID) && subTopID > 0)
                {
                    topID = subTopID;
                }

                LoadAccountsByBalanceType(topID, balanceType);
                UpdateBalanceSummary();
            }
        }

        // تحميل كل الحسابات عند بدء الشاشة
        public void AccountDGV(int topID)
        {
            rdoAll.Checked = true;
            txtSearch.Text = "";
            tblAccDGV = DBServiecs.MainAcc_LoadFinalAccounts(topID, "All");
            DGV.DataSource = tblAccDGV;
            DGVStyl();
            UpdateBalanceSummary();
        }

        // التصفية على الحسابات
        public void FilterAccounts()
        {
            if (!int.TryParse(lblAccTopID.Text, out TopID))
            {
                MessageBox.Show("قيمة lblAccTopID غير صحيحة. يرجى التحقق.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(lblSubAccTop.Text, out SubTopID))
            {
                MessageBox.Show("قيمة lblSubAccTop غير صحيحة. يرجى التحقق.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // استخدام SubTopID إذا كان أكبر من صفر
            if (SubTopID > 0)
            {
                TopID = SubTopID;
            }

            // تحديد نوع الرصيد بناءً على الراديو المختار
            string balanceType = "All"; // افتراضي
            if (rdoMadeen.Checked)
                balanceType = "POS";
            else if (rdoDaeen.Checked)
                balanceType = "NEG";
            else if (rdoEqual.Checked)
                balanceType = "ZERO";

            // تحميل البيانات
            LoadAccountsByBalanceType(TopID, balanceType);


            // تحديث DataGridView (يمكن حذفه إذا كانت LoadAccountsByBalanceType تقوم بذلك)
            DataView dv = new DataView(tblAccDGV);
            DGV.DataSource = dv;

            // تحديث ملخص الرصيد
            UpdateBalanceSummary();
        }


        //  ******  التحكم فى 
        private void tabControlAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlAccount.SelectedTab == tabPage1)
            {
                this.BeginInvoke((Action)(() =>
                {
                    txtSearch.Focus();
                    txtSearch.SelectAll();
                }));
            }
            else if (tabControlAccount.SelectedTab == tabPage2)
            {
                this.BeginInvoke((Action)(() =>
                {
                    txtAccName.Focus();
                    txtAccName.SelectAll();
                }));
            }
        }

        #endregion

        /*اين الخلل فى الصعود والنزول فى اليست والجريد هذه صورة كاملة عن التصفية والبحث يمكن يكون هناك تعارض*/

        #region ****  ترتيب الحسابات الفرعية والنهائية
        private DataTable? tblAccDGV;
        private void RefrechCat(int parentId)
        {
            AccountDGV(AccTopID);
            lblSubAccTop.Text = parentId.ToString();

            if (tblAccDGV != null)
            {
                DataView dv = tblAccDGV.DefaultView;
                dv.RowFilter = $"ParentAccID = {parentId}";
                DGV.DataSource = dv;
            }
            else
            {
                DGV.DataSource = null;
            }

            if (lstSubAccTop.Items.Count > 0)
                lstSubAccTop.SelectedValue = parentId;
        }

        private const string AccIdColumnName = "AccID";

        //هذه الدالة المسؤولة عن تحميل اللست
        private void AccTop_LoadFollowers()
        {
            int? selectedAccId = lstSubAccTop.SelectedValue as int?;

            tblAccTopSub = DBServiecs.MainAcc_LoadFollowers(AccTopID);
            lstSubAccTop.DataSource = tblAccTopSub;
            lstSubAccTop.DisplayMember = "AccName";
            lstSubAccTop.ValueMember = "AccID";

            if (tblAccTopSub != null && tblAccTopSub.Rows.Count > 0)
            {
                tlpTopLst.Visible = true;

                if (selectedAccId.HasValue)
                    lstSubAccTop.SelectedValue = selectedAccId.Value;
                else
                    lstSubAccTop.SelectedIndex = 0;
            }
            else
            {
                tlpTopLst.Visible = false;
            }
        }
        private void SelectRowByAccId(int accId)
        {
            var row = DGV.Rows
                         .Cast<DataGridViewRow>()
                         .FirstOrDefault(r => r.Cells[AccIdColumnName].Value != null &&
                                              Convert.ToInt32(r.Cells[AccIdColumnName].Value) == accId);

            if (row != null)
            {
                DGV.ClearSelection();
                row.Selected = true;

                // نختار أول خلية مرئية بدل العمود [0]
                var firstVisibleCell = row.Cells.Cast<DataGridViewCell>()
                                               .FirstOrDefault(c => c.Visible);

                if (firstVisibleCell != null)
                    DGV.CurrentCell = firstVisibleCell;
            }
        }


        /*
        يوجد تعارض ما فى ترتيب اللست وترتيب الجريد فما هو السبب 
        الللست بها التصنيفات ابناء الحساب الرئيسى وليست فينل اكونت
        اما الجريد فهم ابناء الحساب التصنيف واحفاد الحساب الرئيسى 
        ويوجد زرين للترتيب فى اللست للترتيب
        وزرين للترتيب للجريد
        وعند كتابة زرين الجريد توقفت كل الازرار لماذا
         */

        private void ReloadGrid(int? accIdToSelect = null)
        {
            // 1) تحديد الـ TopID
            if (!int.TryParse(lblAccTopID.Text, out int topID))
                return;

            if (int.TryParse(lblSubAccTop.Text, out int subTopID) && subTopID > 0)
                topID = subTopID;

            // 2) تحديد نوع الرصيد
            string balanceType = rdoMadeen.Checked ? "POS" :
                                 rdoDaeen.Checked ? "NEG" :
                                 rdoEqual.Checked ? "ZERO" : "All";

            // 3) تحميل البيانات
            tblAccDGV = DBServiecs.MainAcc_LoadFinalAccounts(topID, balanceType);

            // 4) تحويل إلى DataView علشان نستفيد من البحث والتصفية
            DataView dv = new DataView(tblAccDGV);

            // 5) تطبيق البحث
            string searchText = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
                dv.RowFilter = $"AccName LIKE '%{searchText}%'";

            // 6) ربط بالـ DGV
            DGV.DataSource = dv;
            DGVStyl();
            UpdateBalanceSummary();

            // 7) إعادة التحديد لو طلب
            if (accIdToSelect.HasValue)
                SelectRowByAccId(accIdToSelect.Value);
        }

        private void btnList_UP_Click(object sender, EventArgs e)
        {
            if (lstSubAccTop.SelectedItem is DataRowView rowView && lstSubAccTop.SelectedIndex > 0)
            {
                int currentAccID = Convert.ToInt32(rowView.Row["AccID"]);
                DBServiecs.MainAcc_MoveSortTree(currentAccID, "UP");

                // إعادة تحميل
                AccTop_LoadFollowers();

                lstSubAccTop.SelectedValue = currentAccID;
            }
        }

        private void btnList_DOWN_Click(object sender, EventArgs e)
        {
            if (lstSubAccTop.SelectedItem is DataRowView rowView)
            {
                int currentAccID = Convert.ToInt32(rowView.Row["AccID"]);
                DBServiecs.MainAcc_MoveSortTree(currentAccID, "DOWN");

                // إعادة تحميل
                AccTop_LoadFollowers();

                lstSubAccTop.SelectedValue = currentAccID;
            }
        }
        private void btnDGV_UP_Click(object sender, EventArgs e)
        {
            if (DGV.CurrentRow != null && DGV.CurrentRow.Index > 0)
            {
                int currentAccID = Convert.ToInt32(DGV.CurrentRow.Cells["AccID"].Value);

                DBServiecs.MainAcc_MoveSortTree(currentAccID, "UP");

                // إعادة تحميل الجريد مع تحديد الحساب
                ReloadGrid(currentAccID);
            }
        }

        private void btnDGV_DOWN_Click(object sender, EventArgs e)
        {
            if (DGV.CurrentRow != null)
            {
                int currentAccID = Convert.ToInt32(DGV.CurrentRow.Cells["AccID"].Value);

                DBServiecs.MainAcc_MoveSortTree(currentAccID, "DOWN");
                AccountDGV(AccTopID);
                // إعادة تحميل الجريد مع تحديد الحساب
                ReloadGrid(currentAccID);
            }
        }
        // 

        #endregion


        private void btnStripChangeCat_Click(object sender, EventArgs e)
        {
            // 🔹 تجهيز DataTable للحسابات المحددة
            DataTable selectedTable = new DataTable();
            selectedTable.Columns.Add("AccID", typeof(int));
            selectedTable.Columns.Add("AccName", typeof(string));

            foreach (DataGridViewRow row in DGV.SelectedRows)
            {
                if (!row.IsNewRow)
                {
                    int accID = row.Cells["AccID"].Value != null
                        ? Convert.ToInt32(row.Cells["AccID"].Value)
                        : 0; // أو تجاهل السطر لو null

                    string accName = row.Cells["AccName"].Value?.ToString() ?? "غير متوفر";

                    selectedTable.Rows.Add(accID, accName);
                }
            }

            // 🔹 تمرير الجدول للفورم الجديد
            frmModifyParentAccID frm = new frmModifyParentAccID();
            frm.SelectedAccounts = selectedTable;
            frm.ShowDialog();
        }




    }
}
