using MizanOriginalSoft.MainClasses;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.MainForms
{
    public partial class frm_Journal : Form
    {
        private readonly int billId;
        private readonly int typeId;

        public frm_Journal(int billNo, int invTypeId)
        {
            InitializeComponent();
            billId = billNo;
            typeId = invTypeId;

            this.Shown += frm_Journal_Shown;
        }

        private void frm_Journal_Load(object sender, EventArgs e)
        {
            DataTable journalData = DBServiecs.DailyFee_GetJournalEntry(billId, typeId);
            DGV.DataSource = journalData;
            DGVStyl();
        }

        private void DGVStyl()
        {
            // إعدادات عامة
            DGV.ReadOnly = true;
            DGV.AllowUserToAddRows = false;
            DGV.AllowUserToDeleteRows = false;
            DGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGV.MultiSelect = false;
            DGV.RowHeadersVisible = false;

            // إخفاء الأعمدة غير الضرورية إن وجدت
            string[] hiddenColumns = { "ID", "Acc_ID", "InvTypeID", "Us_Daily", "Bill_No" };
            foreach (string colName in hiddenColumns)
            {
                if (DGV.Columns.Contains(colName))
                    DGV.Columns[colName].Visible = false;
            }

            // تنسيق الصفوف بناءً على القيمة
            foreach (DataGridViewRow row in DGV.Rows)
            {
                if (row.IsNewRow) continue;

                decimal.TryParse(Convert.ToString(row.Cells["Madeen"].Value), out decimal madeen);
                decimal.TryParse(Convert.ToString(row.Cells["Daeen"].Value), out decimal daeen);

                if (madeen > 0)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(230, 255, 240); // أخضر فاتح
                }
                else if (daeen > 0)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 255); // أزرق فاتح
                }

                // إخفاء القيم = 0 بلون الخلفية
                if (madeen == 0)
                    row.Cells["Madeen"].Style.ForeColor = row.DefaultCellStyle.BackColor;

                if (daeen == 0)
                    row.Cells["Daeen"].Style.ForeColor = row.DefaultCellStyle.BackColor;
            }

            // تعيين خط عام
            Font generalFont = new Font("Times New Roman", 14, FontStyle.Bold);
            DGV.DefaultCellStyle.Font = generalFont;

            // تنسيق رؤوس الأعمدة
            DGV.ColumnHeadersDefaultCellStyle.Font = generalFont;
            DGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            DGV.ColumnHeadersHeight = 60;
            DGV.EnableHeadersVisualStyles = false;

            // تنسيق الأعمدة وتحديد أسماء المستخدم
            foreach (DataGridViewColumn col in DGV.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                switch (col.Name)
                {
                    case "Madeen":
                        col.HeaderText = "مدين";
                        col.FillWeight = 15;
                        break;
                    case "Daeen":
                        col.HeaderText = "دائن";
                        col.FillWeight = 15;
                        break;
                    case "AccName":
                        col.HeaderText = "اسم الحساب";
                        col.FillWeight = 20;
                        break;
                    case "Bill_Code":
                        col.HeaderText = "كود السند";
                        col.FillWeight = 20;
                        break;
                    case "dateRDaily":
                        col.HeaderText = "تاريخ القيد";
                        col.FillWeight = 20;
                        break;
                    case "noteDaily":
                        col.HeaderText = "ملاحظات";
                        col.FillWeight = 30;
                        break;
                }
            }

            DGV.ClearSelection(); // إلغاء التحديد الافتراضي
        }

        private void frm_Journal_Shown(object? sender, EventArgs e)
        {
            DGV.ClearSelection(); // للتأكيد عند الظهور
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
