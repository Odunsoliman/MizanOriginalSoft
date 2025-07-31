using MizanOriginalSoft.MainClasses;
using Signee.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.MainForms
{
    public partial class frm_Journal : Form
    {
        public frm_Journal(int Bill_No, int InvTypeID)
        {
            InitializeComponent();
            billid = Bill_No;
            typeId = InvTypeID;
            this.Shown += frm_Journal_Shown;

        }
        int  billid = 0;
        int typeId = 0;

        private void frm_Journal_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt = DBServiecs.DailyFee_GetJournalEntry(billid, typeId);
            DGV.DataSource = dt;
            DGVStyl();
            DGV.ClearSelection();
        }
        private void DGVStyl()
        {
            // تعطيل التعديل والإضافة والحذف
            DGV.ReadOnly = true;
            DGV.AllowUserToAddRows = false;
            DGV.AllowUserToDeleteRows = false;
            DGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGV.MultiSelect = false;

            // إخفاء الأعمدة غير المرغوبة[]
            if (DGV.Columns.Contains("ID")) DGV.Columns["ID"].Visible = false;
            if (DGV.Columns.Contains("Acc_ID")) DGV.Columns["Acc_ID"].Visible = false;
            if (DGV.Columns.Contains("InvTypeID")) DGV.Columns["InvTypeID"].Visible = false;
            if (DGV.Columns.Contains("Us_Daily")) DGV.Columns["Us_Daily"].Visible = false;
            if (DGV.Columns.Contains("Bill_No")) DGV.Columns["Bill_No"].Visible = false;
            //if (DGV.Columns.Contains("")) DGV.Columns["Bill_No"].Visible = false;

            // إخفاء رؤوس الصفوف
            DGV.RowHeadersVisible = false;

            // تلوين الصفوف بناءً على محتوى Madeen أو Daeen
            foreach (DataGridViewRow row in DGV.Rows)
            {
                if (row.IsNewRow) continue;

                decimal madeen = 0;
                decimal daeen = 0;

                decimal.TryParse(Convert.ToString(row.Cells["Madeen"].Value), out madeen);
                decimal.TryParse(Convert.ToString(row.Cells["Daeen"].Value), out daeen);

                if (madeen > 0)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(230, 255, 240); // أخضر فاتح جدًا
                }
                else if (daeen > 0)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 255); // أزرق فاتح جدًا
                }

                // إخفاء الأصفار بتغيير لون الخط إلى لون الخلفية
                if (madeen == 0)
                {
                    row.Cells["Madeen"].Style.ForeColor = row.DefaultCellStyle.BackColor;
                }

                if (daeen == 0)
                {
                    row.Cells["Daeen"].Style.ForeColor = row.DefaultCellStyle.BackColor;
                }
            }
            // تعيين الخط العام (Times New Roman + Bold) لكل الخلايا
            Font generalFont = new Font("Times New Roman", 14, FontStyle.Bold);
            DGV.DefaultCellStyle.Font = generalFont;

            // تعيين نفس الخط + اللون الأزرق لرؤوس الأعمدة
            DGV.ColumnHeadersDefaultCellStyle.Font = generalFont;
            DGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            // زيادة ارتفاع رأس العمود
            DGV.ColumnHeadersHeight = 60; // يمكنك تعديل الرقم حسب الحاجة

            // تلوين خلفية الرأس (اختياري)
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            DGV.EnableHeadersVisualStyles = false;

            // إعدادات توسيط العناوين وتوزيع العرض
            foreach (DataGridViewColumn col in DGV.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                switch (col.Name)
                {
                    case "Madeen":
                        col.HeaderText = "مدين";
                        col.FillWeight = 15; // الوزن 1
                        break;

                    case "Daeen":
                        col.HeaderText = "دائن";
                        col.FillWeight = 15; // الوزن 1
                        break;

                    case "AccName":
                        col.HeaderText = "اسم الحساب";
                        col.FillWeight = 20; // الوزن 2
                        break;

                    case "Bill_Code":
                        col.HeaderText = "كود السند";
                        col.FillWeight = 20; // الوزن 2
                        break;

                    case "dateRDaily":
                        col.HeaderText = "تاريخ القيد";
                        col.FillWeight = 20; // الوزن 2
                        break;

                    case "noteDaily":
                        col.HeaderText = "ملاحظات";
                        col.FillWeight = 30; // الوزن 3
                        break;
                }
            }

            // إلغاء تحديد أي صف عند التحميل
            DGV.ClearSelection();

            // (افتراضيًا يمكن تحديد الصفوف بالماوس طالما ReadOnly=true و MultiSelect=false)
        }

        private void frm_Journal_Shown(object sender, EventArgs e)
        {
            DGV.ClearSelection();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
