using MizanOriginalSoft.MainClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.Accounts
{
    public partial class frm_AddAccount : Form
    {
        public string CreatedAccountName { get; private set; }
        public int CreatedAccountID { get; private set; }

        public frm_AddAccount(string NameAcc ,int typ)
        {
            InitializeComponent();
            TypeAcc =typ;
            txtAccName.Text  = NameAcc;
        }
        int TypeAcc;
        private void frm_AddAccount_Load(object sender, EventArgs e)
        {
            // توليد رقم جديد إذا كان lblAccID فارغًا
            if (string.IsNullOrWhiteSpace(lblAccID.Text))
            {
                DataTable tblNewID = DBServiecs.MainAcc_GetNewID();
                if (tblNewID != null && tblNewID.Rows.Count > 0)
                {
                    lblAccID.Text = Convert.ToInt32(tblNewID.Rows[0][0]).ToString();
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string accName = txtAccName.Text.Trim();

            if (string.IsNullOrEmpty(accName))
            {
                MessageBox.Show("يرجى إدخال اسم الحساب أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAccName.Focus();
                return;
            }

            // توليد رقم الحساب إذا لم يكن موجودًا
            int accID;
            if (!int.TryParse(lblAccID.Text, out accID))
            {
                DataTable tblNewID = DBServiecs.MainAcc_GetNewID();
                if (tblNewID != null && tblNewID.Rows.Count > 0)
                {
                    accID = Convert.ToInt32(tblNewID.Rows[0][0]);
                    lblAccID.Text = accID.ToString();
                }
                else
                {
                    MessageBox.Show("تعذر توليد رقم الحساب الجديد.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // جمع البيانات
            string firstPhon = txtFirstPhon.Text;
            string antherPhon = txtAntherPhon.Text;
            string accNote = txtAccNote.Text;
            string clientEmail = txtClientEmail.Text;
            string clientAddress = txtClientAddress.Text;

            // تحديد القيمة النهائية لـ TypeAcc حسب القيم الأصلية
            int ParentAccID;
            if (TypeAcc == 1 || TypeAcc == 2)
                ParentAccID = 7; // عملاء
            else if (TypeAcc == 3 || TypeAcc == 4)
                ParentAccID = 14; // موردين
            else
                ParentAccID = TypeAcc; // قيم أخرى غير معروفة

            // تنفيذ الإدخال أو التحديث
            bool result = DBServiecs.MainAcc_UpdateOrInsert(
                accID,
                ParentAccID, // هنا التمرير يكون حسب القيمة المحوّلة
                accName,
                false,
                0,
                0,
                0,
                0,
                0,
                false,
                null,
                firstPhon,
                antherPhon,
                accNote,
                clientEmail,
                clientAddress,
                out string resultMessage
            );


            if (result)
            {
                MessageBox.Show(resultMessage, "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // تمرير البيانات إلى النموذج الرئيسي
                CreatedAccountID = accID;
                CreatedAccountName = accName;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(resultMessage, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
