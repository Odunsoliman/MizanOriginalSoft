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
using System.Xml.Linq;

namespace MizanOriginalSoft.Views.Forms.Accounts
{
    public partial class frm_AccountModify : Form
    {
        int _accID;
        DataTable dtAccData = new DataTable();

        // 🟢 متغيرات داخلية (لا تظهر على الشاشة)
        int? _parentTree;
        decimal? _balance;
        string? _balanceState;
        DateTime? _dateOfJoin;
        int? _createByUserID;
        bool? _isHasDetails;

        public frm_AccountModify(int AccID)
        {
            InitializeComponent();
            _accID = AccID;
        }


        private void frm_AccountModify_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            dtAccData = DBServiecs.Acc_GetData(_accID);

            if (dtAccData.Rows.Count == 0)
                return;

            DataRow row = dtAccData.Rows[0];

            // 🟢 تخزين الأعمدة في متغيرات
            _parentTree = row["ParentTree"] != DBNull.Value ? Convert.ToInt32(row["ParentTree"]) : (int?)null;
            _balance = row["Balance"] != DBNull.Value ? Convert.ToDecimal(row["Balance"]) : (decimal?)null;
            _balanceState = row["BalanceState"] != DBNull.Value ? row["BalanceState"].ToString() : null;
            _dateOfJoin = row["DateOfJoin"] != DBNull.Value ? Convert.ToDateTime(row["DateOfJoin"]) : (DateTime?)null;
            _isHasDetails = row["IsHasDetails"] != DBNull.Value ? Convert.ToBoolean(row["IsHasDetails"]) : (Boolean?)null;

            // ✅ عرض الرصيد وحالته في Label واحد
            Control[] balanceLbls = this.Controls.Find("lblBalanceAndState", true);
            if (balanceLbls.Length > 0 && balanceLbls[0] is Label lblBal)
            {
                string balText = _balance.HasValue ? _balance.Value.ToString("N2") : "0.00";
                string stateText = !string.IsNullOrEmpty(_balanceState) ? _balanceState : "";
                lblBal.Text = $"الرصيد: {balText}  {stateText}";
            }

            // ✅ إظهار الأعمدة المطلوبة فقط
            foreach (DataColumn col in dtAccData.Columns)
            {
                string colName = col.ColumnName;
                object value = row[colName];

                // Label
                Control[] lbls = this.Controls.Find("lbl" + colName, true);
                if (lbls.Length > 0 && lbls[0] is Label lbl)
                    lbl.Text = value?.ToString();

                // TextBox
                Control[] txts = this.Controls.Find("txt" + colName, true);
                if (txts.Length > 0 && txts[0] is TextBox txt)
                    txt.Text = value?.ToString();

                // CheckBox
                Control[] chks = this.Controls.Find("chk" + colName, true);
                if (chks.Length > 0 && chks[0] is CheckBox chk)
                    chk.Checked = value != DBNull.Value && Convert.ToBoolean(value);
            }

            // خاصية IsEnerAcc → حساب داخلي أو فارغ
            if (dtAccData.Columns.Contains("IsEnerAcc"))
            {
                object isEnerAcc = row["IsEnerAcc"];
                Control[] lbls = this.Controls.Find("lblIsEnerAcc", true);
                if (lbls.Length > 0 && lbls[0] is Label lblEner)
                {
                    if (isEnerAcc != DBNull.Value && Convert.ToBoolean(isEnerAcc))
                        lblEner.Text = "حساب داخلي";
                    else
                        lblEner.Text = "";
                }
            }
        }

        public int UpdatedAccID { get; private set; }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // استدعاء الإجراء مع القيم
            string resultMsg = DBServiecs.Acc_UpdateAccount(
                _accID,
                txtAccName.Text,                  // اسم الحساب
                chkIsHidden.Checked               // هل الحساب مخفي
            );

            // التحقق من النتيجة
            if (resultMsg.StartsWith("❌")) // ❌ خطأ
            {
                MessageBox.Show(resultMsg, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // لا نغلق الشاشة
            }
            else // ✅ نجاح
            {
                MessageBox.Show(resultMsg, "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // نحتفظ بالـID المعدل عشان نقدر نرجعله في الرئيسية
                UpdatedAccID = _accID;

                // نغلق الشاشة ونرجع OK
                this.DialogResult = DialogResult.OK;
                this.Close();
            }

        }

    }
}
