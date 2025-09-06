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

            // ✅ تحميل باقي الحسابات
            DataTable dt = DBServiecs.MainAcc_GetHierarchy();
            DGV.DataSource = dt;
            DGVStyl();
        }
        /*
         لدى شاشة رئيسية اسمها frmMainAccounts وبها زر باسم btnStripChangeCat وبها جريد باسم DGV
        عند النقر على btnStripChangeCat يتم فتح هذه الشاشة والمطلوب تمرير رقم واسم الحساب المحمل اساسة فى الجريد وتم تحديده
              private void btnStripChangeCat_Click(object sender, EventArgs e)
        {
            // جمع كل الحسابات المحددة في الـ DGV
            List<int> selectedAccIDs = new List<int>();
            foreach (DataGridViewRow row in DGV.SelectedRows)
            {
                if (!row.IsNewRow)
                {
                    // افترض أن العمود يحتوي على AccID
                    int accID = Convert.ToInt32(row.Cells["AccID"].Value);
                    selectedAccIDs.Add(accID);
                }
            }

            // تمريرها إلى الفورم الجديد
            frmModifyParentAccID frm = new frmModifyParentAccID();
            frm.SelectedAccIDs = selectedAccIDs;
            frm.ShowDialog();
        }

        وبعد التمرير يجب العرض فى DGVSelectedAcc رقم حساب واسم

        فقط فما الحل الجذرى



         
         
         
         */

















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
            /*
            
            AccName,
            اريد اخفاء كل الاعمدة الواردة واظهار حقل واحد فقط ولكن لا يظهر
            فما السبب
             */
            ApplyGridFormatting();
            foreach (DataGridViewColumn col in DGV.Columns)
                col.Visible = false;
            void Show(string name, string header, float weight)
            {
                if (!DGV.Columns.Contains(name)) return;
                var c = DGV.Columns[name];
                c.Visible = true;
                c.HeaderText = header;
                c.FillWeight = weight;
            }

         //   Show("AccID", "كود", 1f);
            Show("AccName", "اسم الحساب الرئيسى", 3f);
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
