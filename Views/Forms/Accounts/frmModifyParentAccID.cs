using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.SearchClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.Accounts
{
    public partial class frmModifyParentAccID : Form
    {
        // 🟢 نخزن الجدول مباشرة
        public DataTable? SelectedAccounts { get; set; }

        public frmModifyParentAccID()
        {
            InitializeComponent();
        }

        private void frmModifyParentAccID_Load(object sender, EventArgs e)
        {
            // ✅ ربط البيانات مباشرة
            DGVSelectedAcc.DataSource = SelectedAccounts;
            DGVStylSelected();
            DGVSelectedAcc.ClearSelection();
            // ✅ تحميل باقي الحسابات
            DataTable dt = DBServiecs.MainAcc_GetHierarchy();
            DGV.DataSource = dt;
            DGVStyl();
        }
        public void ApplyGridFormatting()
        {
            // 1️⃣ التنسيق الموحد أولاً
            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DGV.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            DGV.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            DGV.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        // 🔹 الدالة الحالية تبقى لحسابات فقط
        public void DGVStyl_()
        {
         //   Show("AccID", "كود", 1f);
        }
        public void DGVStyl()
        {
            ApplyGridFormatting();

            // إخفاء كل الأعمدة
            foreach (DataGridViewColumn col in DGV.Columns)
                col.Visible = false;

            // إظهار العمود HierarchyName
            void Show(string name, string header, float weight)
            {
                var c = DGV.Columns.Cast<DataGridViewColumn>()
                          .FirstOrDefault(x => x.DataPropertyName == name || x.Name == name);
                if (c == null) return;
                c.Visible = true;
                c.HeaderText = header;
                c.FillWeight = weight;
                c.Width = 300; // اجعلها مناسبة
            }

            Show("HierarchyName", "اسم الحساب الرئيسى", 3f);
        }
        public void DGVStylSelected()
        {
            DGVSelectedAcc.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DGVSelectedAcc.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            DGVSelectedAcc.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            DGVSelectedAcc.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            DGVSelectedAcc.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            // إخفاء كل الأعمدة
            foreach (DataGridViewColumn col in DGVSelectedAcc .Columns)
                col.Visible = false;

            // إظهار العمود HierarchyName
            void Show(string name, string header, float weight)
            {
                var c = DGVSelectedAcc.Columns.Cast<DataGridViewColumn>()
                          .FirstOrDefault(x => x.DataPropertyName == name || x.Name == name);
                if (c == null) return;
                c.Visible = true;
                c.HeaderText = header;
                c.FillWeight = weight;
                c.Width = 300; // اجعلها مناسبة
            }

            Show("AccID", "كود الحساب", 3f);
            Show("AccName", "اسم الحساب الفرعى", 3f);
            DGVSelectedAcc.ClearSelection();
        }


        private void DGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (cbxChangeCat.SelectedValue == null)
            //{
            //    MessageBox.Show("يرجى تحديد التصنيف الذي تريد نقل الأصناف إليه", "تنبيه",
            //        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

          //  int newP = Convert.ToInt32(DGV .SelectedValue);

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
     //       string resultMessage;
       //     bool success = DBServiecs.MainAcc_ChangAccCat(newP, accIDs, out resultMessage);


            
        }

    }




}
