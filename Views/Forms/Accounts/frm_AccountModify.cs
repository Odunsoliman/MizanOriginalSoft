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
            // 🔹 جلب البيانات من الإجراء المخزن
            dtAccData = DBServiecs.Acc_GetDataForModify(_accID);
            if (dtAccData.Rows.Count == 0)
                return;
            lblTitetl_Item.Text = "تعديل الحساب رقم : " + _accID;

            DataRow row = dtAccData.Rows[0];

            // 🔹 عرض اسم الحساب
            txtAccName.Text = row["AccName"].ToString();
            chkIsHidden.Checked = row["IsHidden"] != DBNull.Value && Convert.ToBoolean(row["IsHidden"]);
            if (chkIsHidden.Checked)
            {
                chkIsHidden.Text = "الحساب غير فعال";
            }
            else
            {
                chkIsHidden.Text = "الحساب  فعال";
            }
            lblIsEnerAcc .Text = row["IsEnerAcc"].ToString();     // هل الرقم داخلى ام تشغيلى

            // 🔹 عرض خصائص التعديل (قيم منطقية فقط)
            chkIsForManger.Checked = row["IsForManger"] != DBNull.Value && Convert.ToBoolean(row["IsForManger"]);
            chkIsHasDetails.Checked = row["IsHasDetails"] != DBNull.Value && Convert.ToBoolean(row["IsHasDetails"]);
        
                // 🔹 عرض المعلومات النصية الجاهزة من SQL
                lblTreeAccCode.Text = row["TreeAccCode"].ToString();     // الترقيم الشجري
            lblAccTypeID.Text = row["Acc_TypeName"].ToString();    // النوع المحاسبي
            lblParentTree.Text = row["ParentTree"].ToString();      // اسم الأب
            lblCreatedByUser.Text = row["UserName"].ToString();        // أنشئ بواسطة
            lblBalanceAndState.Text = row["Balance"].ToString();         // الرصيد الآن: xxx دائن
            lblDateOfJoin.Text = row["DateOfJoin"].ToString();      // تاريخ الإنشاء: yyyy-mm-dd

            // 🔹 لا حاجة لتحويل القيم الداخلية بعد الآن، لأنها أصبحت نصوصًا جاهزة
            // ولكن يمكنك تخزين قيم مهمة داخليًا إن أردت (اختياري)
            _AccTypeID = row["AccTypeID"] != DBNull.Value ? Convert.ToInt32(row["AccTypeID"]) : (int?)null;
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
