using MizanOriginalSoft.MainClasses;
using System;
using System.Data;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.Accounts
{
    public partial class frm_AccountDetailAdd : Form
    {
        private readonly int _accID;
        private readonly int? _detailID; // لتحديد إذا كانت تعديل أو إضافة

        public frm_AccountDetailAdd(int accID, int? detailID = null)
        {
            InitializeComponent();
            _accID = accID;
            _detailID = detailID;
        }

        private void frm_AccountDetailAdd_Load(object sender, EventArgs e)
        {
            try
            {
                if (_detailID == null)
                {
                    lblTitel.Text = $"➕ إضافة تفاصيل إلى الحساب: {_accID}";
                }
                else
                {
                    lblTitel.Text = $"✏️ تعديل تفاصيل الحساب: {_accID}";
                    LoadDetailData(_detailID.Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ أثناء تحميل النموذج: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// تحميل البيانات في حالة التعديل
        /// </summary>
        /// <summary>
        /// تحميل البيانات في حالة التعديل
        /// </summary>
        private void LoadDetailData(int detailID)
        {
            DataTable dt = DBServiecs.Acc_GetDetails(_accID); // استدعاء كل التفاصيل للحساب

            // فلترة الصف بناءً على DetailID القادم
            DataRow[] rows = dt.Select($"DetailID = {detailID}");
            if (rows.Length == 0)
            {
                MessageBox.Show("⚠️ لم يتم العثور على بيانات التفاصيل.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRow row = rows[0];

            txtContactName.Text = row["ContactName"]?.ToString() ?? string.Empty;
            txtPhone.Text = row["Phone"]?.ToString() ?? string.Empty;
            txtMobile.Text = row["Mobile"]?.ToString() ?? string.Empty;
            txtEmail.Text = row["Email"]?.ToString() ?? string.Empty;
            txtAddress.Text = row["Address"]?.ToString() ?? string.Empty;
            txtNotes.Text = row["Notes"]?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// حفظ (إضافة أو تعديل)
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string resultMsg = DBServiecs.Acc_SaveDetails(
                    _detailID, // null = إضافة، رقم = تعديل
                    _accID,
                    txtContactName.Text.Trim(),
                    txtPhone.Text.Trim(),
                    txtMobile.Text.Trim(),
                    txtEmail.Text.Trim(),
                    txtAddress.Text.Trim(),
                    txtNotes.Text.Trim()
                );

                if (string.IsNullOrWhiteSpace(resultMsg))
                {
                    MessageBox.Show("⚠️ لم يتم إرجاع رسالة من قاعدة البيانات.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!resultMsg.StartsWith("❌"))
                {
                    MessageBox.Show(resultMsg, "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show(resultMsg, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ حدث خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// إلغاء العملية
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
