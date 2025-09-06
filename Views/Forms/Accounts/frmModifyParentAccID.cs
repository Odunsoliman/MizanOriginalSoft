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
            lblCountSelected.Text = "عدد الحسابات المنقولة:   " + DGVSelectedAcc.RowCount.ToString();
            // ✅ تحميل باقي الحسابات
            DataTable dt = DBServiecs.MainAcc_GetHierarchy();
            DGV.DataSource = dt;
            DGVStyl();
            lblTitel.Text = "يتم النقل الى الحساب الرئيسى :  ";
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
            foreach (DataGridViewColumn col in DGVSelectedAcc.Columns)
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
            if (e.RowIndex < 0) return; // تجاهل النقر على الهيدر

            // 🔹 1. الحصول على AccID للحساب الذي نقر عليه المستخدم
            int newP = Convert.ToInt32(DGV.Rows[e.RowIndex].Cells["AccID"].Value);

            // 🔹 2. جمع كل الحسابات الموجودة في DGVSelectedAcc (المحددة مسبقاً)
            List<int> accountIds = new List<int>();
            foreach (DataGridViewRow row in DGVSelectedAcc.Rows)
            {
                if (!row.IsNewRow)
                {
                    int id = Convert.ToInt32(row.Cells["AccID"].Value);
                    accountIds.Add(id);
                }
            }

            // 🔹 3. تحويل القائمة إلى نص مفصول بفواصل لتمريره إلى SQL
            string accIDs = string.Join(",", accountIds);

            // 🔹 4. استدعاء الإجراء المخزن لتغيير الأب
            string resultMessage;
            bool success = DBServiecs.MainAcc_ChangAccCat(newP, accIDs, out resultMessage);

            // 🔹 5. عرض النتيجة للمستخدم
            MessageBox.Show(resultMessage, success ? "نجاح" : "خطأ",
                            MessageBoxButtons.OK,
                            success ? MessageBoxIcon.Information : MessageBoxIcon.Error);
        }


        private void DGV_SelectionChanged(object sender, EventArgs e)
        {
            if (DGV.CurrentRow != null && !DGV.CurrentRow.IsNewRow)
            {
                var hierarchyName = DGV.CurrentRow.Cells["HierarchyName"].Value?.ToString() ?? "غير معروف";
                lblTitel.Text = "يتم النقل الى الحساب الرئيسى : " + hierarchyName;
            }
            else
            {
                lblTitel.Text = "يتم النقل الى الحساب الرئيسى : (لا يوجد حساب محدد)";
            }
        }

    }




}
