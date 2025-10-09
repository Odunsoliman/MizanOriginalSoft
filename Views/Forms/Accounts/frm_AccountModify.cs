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
        int? _AccTypeID;
        decimal? _balance;
        string? _balanceState;
        DateTime? _dateOfJoin;
        int? _createByUserID;
        bool? _isEnerAcc;
        bool? _isHasDetails;
        bool? _isForManger;
        bool? _isHasChildren;
        bool? _isHidden;
        public frm_AccountModify(int AccID)
        {
            InitializeComponent();
            _accID = AccID;
        }

        private void frm_AccountModify_Load(object sender, EventArgs e)
        {
            LoadParentAccounts();  // تحميل قائمة الحسابات الأب في ComboBox
            LoadData();            // تحميل بيانات الحساب الحالي
        }

        private void LoadParentAccounts()
        {

        }
        private void LoadData()
        {
            dtAccData = DBServiecs.Acc_GetData(_accID);
            if (dtAccData.Rows.Count == 0)
                return;

            DataRow row = dtAccData.Rows[0];

            // 🔹 تخزين البيانات الداخلية
            _parentTree = row["ParentTree"] != DBNull.Value ? Convert.ToInt32(row["ParentTree"]) : (int?)null;
            _AccTypeID = row["AccTypeID"] != DBNull.Value ? Convert.ToInt32(row["AccTypeID"]) : (int?)null;
            _balance = row["Balance"] != DBNull.Value ? Convert.ToDecimal(row["Balance"]) : (decimal?)null;
            _balanceState = row["BalanceState"]?.ToString();
            _dateOfJoin = row["DateOfJoin"] != DBNull.Value ? Convert.ToDateTime(row["DateOfJoin"]) : (DateTime?)null;
            _createByUserID = row["CreateByUserID"] != DBNull.Value ? Convert.ToInt32(row["CreateByUserID"]) : (int?)null;
            _isHasDetails = row["IsHasDetails"] != DBNull.Value ? Convert.ToBoolean(row["IsHasDetails"]) : (bool?)null;
            _isForManger = row["IsForManger"] != DBNull.Value ? Convert.ToBoolean(row["IsForManger"]) : (bool?)null;
            _isHidden = row["IsHidden"] != DBNull.Value ? Convert.ToBoolean(row["IsHidden"]) : (bool?)null;

            // 🔹 عرض اسم الحساب في TextBox
            txtAccName.Text = row["AccName"].ToString();

            // 🔹 عرض خصائص التعديل
            chkIsForManger.Checked = _isForManger ?? false;
            chkIsHasDetails.Checked = _isHasDetails ?? false;
            chkIsHidden.Checked = _isHidden ?? false;

            // 🔹 عرض الأب في ComboBox
            if (_parentTree.HasValue)
                cbxParentTree.SelectedValue = _parentTree.Value;

            // 🔹 عرض الرصيد
            lblBalanceAndState.Text = $"الرصيد: {_balance:N2} {_balanceState}";

            // 🔹 معلومات إضافية للعرض فقط
            lblTreeAccCode.Text = row["TreeAccCode"].ToString();
            lblAccTypeID.Text = row["AccTypeID"].ToString();
            lblCreateByUserID.Text = row["CreateByUserID"].ToString();
            lblDateOfJoin.Text = _dateOfJoin?.ToShortDateString();
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this .Close();
        }
    }
}
/*

SELECT [AccID]              عرض
,[IsEnerAcc]          عرض          
,[TreeAccCode]        عرض
,[AccName]            للتعديل
,[ParentTree]         لتغير الاب من خلال كمبوبكس
,[IsForManger]        للتعديل
,[IsHasDetails]       للتعديل
,[IsHasChildren]      عرض
,[AccTypeID]          عرض
,[CreateByUserID]     عرض
,[Balance]            عرض
,[BalanceState]       عرض
,[IsHidden]           للتعديل
,[DateOfJoin]         عرض
FROM [dbo].[Accounts]
الدالة DBServiecs.Acc_GetData(_accID); تجلب كل الحقول السابقة
فكيف السيناريو لعرض كل كل هذه الحقول لتعديلها من من خلال الشاشة

 */
