using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.MainForms
{
    public partial class frmReportsManager : Form
    {
        private int selectedReportID = 0; // لمعرفة هل تعديل أو إضافة جديدة
        private KeyboardLanguageManager _languageManager;

        public class ComboItem
        {
            public string? Text { get; set; }
            public int Value { get; set; }
        }

        public frmReportsManager()
        {
            InitializeComponent();
            _languageManager = new KeyboardLanguageManager(this);

        }

        private void frmReportsManager_Load(object sender, EventArgs e)
        {
            FillTopAccCombo();
        }

        private void FillTopAccCombo()
        {
            var items = new List<ComboItem>
    {
        new ComboItem { Text = "التقارير الختامية العامة", Value = 0 },
        new ComboItem { Text = "تقارير الصندوق", Value = 3 },
        new ComboItem { Text = "تقارير المدينون", Value = 6 },
        new ComboItem { Text = "تقارير العملاء", Value = 7 },
        new ComboItem { Text = "تقارير الأصول الثابتة", Value = 9 },
        new ComboItem { Text = "تقارير الدائنون", Value = 13 },
        new ComboItem { Text = "تقارير الموردين", Value = 14 },
        new ComboItem { Text = "تقارير المصروفات", Value = 19 },
        new ComboItem { Text = "تقارير العاملون", Value = 22 },
        new ComboItem { Text = "تقارير جاري الشركاء", Value = 39 },
        new ComboItem { Text = "التقارير للأصناف", Value = 200 }
    };

            cbxID_TopAcc.DataSource = items;
            cbxID_TopAcc.DisplayMember = "Text";
            cbxID_TopAcc.ValueMember = "Value";
            cbxID_TopAcc.SelectedIndex = -1;
            cbxID_TopAcc.DropDownStyle = ComboBoxStyle.DropDownList; // للقراءة فقط
        }


        private void LoadReports(int topAcc)
        {
            DataTable dt = DBServiecs.Reports_GetByTopAcc(topAcc);
            DGV.DataSource = dt;

            // لا يظهر رأس السطر
            DGV.RowHeadersVisible = false;

            // غير قابلة للكتابة
            DGV.ReadOnly = true;

            // تلوين الأسطر المتعاقبة
            DGV.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);

            // إخفاء الأعمدة غير المطلوبة
            foreach (DataGridViewColumn col in DGV.Columns)
            {
                if (col.Name != "ReportDisplayName" &&
                    col.Name != "ReportCodeName" &&
                    col.Name != "Notes")
                {
                    col.Visible = false;
                }
            }

            // عدم تحديد أي سطر عند التحميل
            DGV.ClearSelection();

            // التحكم في الخط
            DGV.DefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Regular);
            DGV.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Bold);
            DGV.DefaultCellStyle.ForeColor = Color.Black;

            // تسمية الأعمدة بالعربي
            if (DGV.Columns.Contains("ReportDisplayName"))
                DGV.Columns["ReportDisplayName"].HeaderText = "اسم التقرير";

            if (DGV.Columns.Contains("ReportCodeName"))
                DGV.Columns["ReportCodeName"].HeaderText = "اسم الكود";

            if (DGV.Columns.Contains("Notes"))
                DGV.Columns["Notes"].HeaderText = "ملاحظات";

            // توسيط رؤوس الأعمدة
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // محاذاة النصوص داخل الأعمدة
            if (DGV.Columns.Contains("ReportDisplayName"))
                DGV.Columns["ReportDisplayName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            if (DGV.Columns.Contains("Notes"))
                DGV.Columns["Notes"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            if (DGV.Columns.Contains("ReportCodeName"))
                DGV.Columns["ReportCodeName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // ضبط عرض الأعمدة بالنسبة 1:1:2
            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            int totalWidth = DGV.ClientSize.Width;
            int unitWidth = totalWidth / 4; // لأن 1+1+2 = 4

            if (DGV.Columns.Contains("ReportDisplayName"))
                DGV.Columns["ReportDisplayName"].Width = unitWidth;

            if (DGV.Columns.Contains("ReportCodeName"))
                DGV.Columns["ReportCodeName"].Width = unitWidth;

            if (DGV.Columns.Contains("Notes"))
                DGV.Columns["Notes"].Width = unitWidth * 2;
        }



        private void cbxID_TopAcc_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (cbxID_TopAcc.SelectedValue is int topAcc)
            {
                LoadReports(topAcc);
            }
        }

        private void DGV_SelectionChanged(object sender, EventArgs e)
        {
            if (DGV.CurrentRow != null && DGV.CurrentRow.Index >= 0)
            {
                selectedReportID = Convert.ToInt32(DGV.CurrentRow.Cells["ReportID"].Value ?? 0);
                txtReportDisplayName.Text = DGV.CurrentRow.Cells["ReportDisplayName"].Value?.ToString() ?? "";
                txtReportCodeName.Text = DGV.CurrentRow.Cells["ReportCodeName"].Value?.ToString() ?? "";

                bool isGrouped = Convert.ToBoolean(DGV.CurrentRow.Cells["IsGrouped"].Value ?? false);
                chkIsGrouped.Checked = isGrouped;
                rdoIsGrouped.Checked = isGrouped;
                rdoIndividual.Checked = !isGrouped;

                txtNotes.Text = DGV.CurrentRow.Cells["Notes"].Value?.ToString() ?? "";
                chkIsActivRep.Checked = Convert.ToBoolean(DGV.CurrentRow.Cells["IsActivRep"].Value ?? false);
            }
        }

        // عند اختيار الراديو Individual → الـ chkIsGrouped = false
        private void rdoIndividual_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoIndividual.Checked)
            {
                chkIsGrouped.Checked = false;
            }
        }

        // عند اختيار الراديو IsGrouped → الـ chkIsGrouped = true
        private void rdoIsGrouped_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoIsGrouped.Checked)
            {
                chkIsGrouped.Checked = true;
            }
        }

        // عند تغيير الـ chkIsGrouped يدور الراديو المناسب
        private void chkIsGrouped_CheckedChanged(object sender, EventArgs e)
        {
            if (chkIsGrouped.Checked)
            {
                rdoIsGrouped.Checked = true;
            }
            else
            {
                rdoIndividual.Checked = true;
            }
        }

        private void btnNew_Click(object? sender, EventArgs? e)
        {
            selectedReportID = 0;
            txtReportDisplayName.Clear();
            txtReportCodeName.Clear();
            chkIsGrouped.Checked = false;
            txtNotes.Clear();
            chkIsActivRep.Checked = true;
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            // تحقق أن هناك تقرير محدد
            if (selectedReportID <= 0)
            {
                MessageBox.Show("الرجاء اختيار تقرير لحذفه.", "تنبيه",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // رسالة تأكيد قبل الحذف
            DialogResult confirm = MessageBox.Show(
                "هل أنت متأكد أنك تريد حذف هذا التقرير؟",
                "تأكيد الحذف",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                // تنفيذ الحذف واستقبال رسالة النتيجة
                string msg = DBServiecs.ReportsMaster_Delete(selectedReportID);

                // عرض النتيجة للمستخدم
                MessageBox.Show(msg, "نتيجة العملية",
                                MessageBoxButtons.OK,
                                msg.Contains("نجاح") ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

                // في حالة نجاح الحذف، إعادة تحميل التقارير ومسح البيانات
                if (msg.Contains("نجاح"))
                {
                    if (cbxID_TopAcc.SelectedValue != null && int.TryParse(cbxID_TopAcc.SelectedValue.ToString(), out int idTopAcc))
                    {
                        LoadReports(idTopAcc);
                    }
                    btnNew_Click(null, null); // مسح الحقول
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء الحذف: {ex.Message}", "خطأ",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            // تحقق أن القيمة المختارة موجودة
            if (cbxID_TopAcc.SelectedValue == null)
            {
                MessageBox.Show("الرجاء اختيار الحساب الرئيسي أولاً.", "تنبيه",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbxID_TopAcc.Focus();
                return;
            }

            // تحويل القيمة المختارة إلى رقم بأمان
            int idTopAcc;
            if (!int.TryParse(cbxID_TopAcc.SelectedValue.ToString(), out idTopAcc))
            {
                MessageBox.Show("قيمة الحساب الرئيسي غير صحيحة.", "خطأ",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string msg;
            bool success = DBServiecs.ReportsMaster_Save(
                selectedReportID,
                txtReportDisplayName.Text.Trim(),
                idTopAcc, // القيمة المحولة بأمان
                txtReportCodeName.Text.Trim(),
                chkIsGrouped.Checked,
                0, // ParentID
                txtNotes.Text.Trim(),
                chkIsActivRep.Checked,
                out msg
            );

            MessageBox.Show(msg, "نتيجة العملية", MessageBoxButtons.OK,
                            success ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (success)
            {
                LoadReports(idTopAcc);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtReportDisplayName_Enter(object sender, EventArgs e)
        {
            _languageManager.SetArabicLanguage(); // البحث غالباً عربى
        }

        private void txtReportCodeName_Enter(object sender, EventArgs e)
        {
            _languageManager.SetEnglishLanguage(); // البحث غالباً إنجليزي
        }

        private void txtNotes_Enter(object sender, EventArgs e)
        {
            _languageManager.SetArabicLanguage(); // البحث غالباً عربى
        }


    }
}



/*
                رموز التقارير الرئيسية:
        0 = التقارير الختامية العامة
        تقارير الصندوق = 3
        تقارير المدينون = 6
        تقارير العملاء = 7
        تقارير الأصول الثابتة = 9
        تقارير الدائنون = 13
        تقارير الموردين = 14
        تقارير المصروفات = 19
        تقارير العاملون = 22
        تقارير جاري الشركاء = 39
        200 = التقارير للاصناف

*/