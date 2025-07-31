using MizanOriginalSoft.MainClasses;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.Accounts
{
    public partial class frm_AddAccount : Form
    {
        // اسم الحساب الذي تم إنشاؤه (يعاد إلى النموذج الرئيسي)
        public string CreatedAccountName { get; private set; } = string.Empty;

        // رقم الحساب الذي تم إنشاؤه
        public int CreatedAccountID { get; private set; }

        private readonly int TypeAcc;

        public frm_AddAccount(string nameAcc, int type)
        {
            InitializeComponent();
            TypeAcc = type;
            txtAccName.Text = nameAcc;
        }

        private void frm_AddAccount_Load(object sender, EventArgs e)
        {
            // توليد رقم جديد إذا لم يتم تعيين رقم للحساب مسبقًا
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

            // محاولة قراءة رقم الحساب، أو توليد رقم جديد إن لم يكن موجودًا
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

            // جمع البيانات من الحقول
            string firstPhon = txtFirstPhon.Text;
            string antherPhon = txtAntherPhon.Text;
            string accNote = txtAccNote.Text;
            string clientEmail = txtClientEmail.Text;
            string clientAddress = txtClientAddress.Text;

            // تحديد الحساب الأب بناءً على نوع الحساب
            int parentAccID = TypeAcc switch
            {
                1 or 2 => 7,   // عملاء
                3 or 4 => 14,  // موردين
                _ => TypeAcc   // أنواع أخرى
            };

            // تنفيذ الحفظ
            bool result = DBServiecs.MainAcc_UpdateOrInsert(
                accID,
                parentAccID,
                accName,
                false,
                0, 0, 0, 0, 0,
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

                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show(resultMessage, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
