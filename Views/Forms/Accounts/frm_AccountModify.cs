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
        private readonly int _accID;
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
            LoadData();            // تحميل بيانات الحساب الحالي
        }


        private void LoadData()
        {
            // 🔹 جلب البيانات من الإجراء المخزن
            dtAccData = DBServiecs.Acc_GetDataForModify(_accID);
            if (dtAccData.Rows.Count == 0)
                return;

            lblTitetl_Item.Text = "تعديل الحساب رقم : " + _accID;
            bool? isEnerAcc = false;     // هل الرقم داخلى ام تشغيلى

            DataRow row = dtAccData.Rows[0];
            isEnerAcc = row["IsEnerAcc"] != DBNull.Value && Convert.ToBoolean(row["IsEnerAcc"]);
            _isEnerAcc = isEnerAcc; // تخزين القيمة في متغير الفورم

            if (isEnerAcc == false)
            {
                cbxParentTree.Enabled = true;
                chkIsHidden.Enabled = true;
                lblCBX.Visible = true;
            }
            else
            {
                cbxParentTree.Enabled = false;
                chkIsHidden.Enabled = false;
                lblCBX.Visible = false;
            }

            // 🔹 عرض اسم الحساب
            txtAccName.Text = row["AccName"].ToString();
            chkIsHidden.Checked = row["IsHidden"] != DBNull.Value && Convert.ToBoolean(row["IsHidden"]);

            if (chkIsHidden.Checked)
            {
                chkIsHidden.Text = "الحساب غير فعال";
            }
            else
            {
                chkIsHidden.Text = "الحساب فعال";
            }

            lblIsEnerAcc.Text = row["IsEnerAccType"].ToString();     // هل الرقم داخلى ام تشغيلى
            chkIsForManger.Checked = row["IsForManger"] != DBNull.Value && Convert.ToBoolean(row["IsForManger"]);

            // 🔹 عرض خصائص التعديل (قيم منطقية فقط)
            chkIsHasDetails.Checked = row["IsHasDetails"] != DBNull.Value && Convert.ToBoolean(row["IsHasDetails"]);
            lblTreeAccCode.Text = row["TreeAccCode"].ToString();     // الترقيم الشجري

            lblTreeAccCodeAndText.Text = row["TreeAccCodeAndText"].ToString();     // الترقيم الشجري
            lblAccTypeID.Text = row["Acc_TypeName"].ToString();    // النوع المحاسبي
            lblParentTree.Text = row["ParentTreeName"].ToString();      // اسم الأب
            _parentTree = row["ParentTree"] != DBNull.Value ? Convert.ToInt32(row["ParentTree"]) : (int?)null;
            lblCreateByUserName.Text = "أنشئ بواسطة   " + row["UserName"].ToString();        // أنشئ بواسطة
            lblBalanceAndState.Text = row["Balance"].ToString();         // الرصيد الآن: xxx دائن
            lblDateOfJoin.Text = row["DateOfJoin"].ToString();      // تاريخ الإنشاء: yyyy-mm-dd

            // 🔹 تخزين القيم المهمة داخليًا
            _AccTypeID = row["AccTypeID"] != DBNull.Value ? Convert.ToInt32(row["AccTypeID"]) : (int?)null;

            LoadParentAccounts();  // تحميل قائمة الحسابات الأب في ComboBox
            LodeAccTypeID();
        }

        private void LodeAccTypeID()
        {
            DataTable dt = DBServiecs.Acc_GetAccTypeID();

            // 🔹 إعداد مصدر البيانات
            cbxAccTypeID.DataSource = dt;
            cbxAccTypeID.DisplayMember = "AccTypeName";
            cbxAccTypeID.ValueMember = "AccTypeID";
            cbxAccTypeID.DropDownStyle = ComboBoxStyle.DropDownList; // 🔒 منع الكتابة اليدوية

            // 🔹 تحديد القيمة الحالية في ال ComboBox
            if (_AccTypeID.HasValue)
            {
                cbxAccTypeID.SelectedValue = _AccTypeID.Value;
            }

            // 🔹 التحكم في إمكانية التعديل بناءً على نوع الحساب
            if (_isEnerAcc == false)
            {
                // الحساب العادي - متاح للتعديل
                cbxAccTypeID.Enabled = true;
            }
            else
            {
                // الحساب الداخلي - غير متاح للتعديل
                cbxAccTypeID.Enabled = false;
            }
        }



        private void LoadParentAccounts()
        {
            // 🔹 جلب قائمة الحسابات الأب من قاعدة البيانات
            DataTable dt = DBServiecs.Acc_GetChart();

            // 🔹 إنشاء عمود عرض جديد يحتوي على "اسم الحساب - النوع المحاسبي"
            if (!dt.Columns.Contains("DisplayText"))
                dt.Columns.Add("DisplayText", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                string accName = row["AccName"]?.ToString() ?? "";
                string accType = row["AccTypeName"]?.ToString() ?? "";
                row["DisplayText"] = $"{accName} - {accType}";
            }

            // 🔹 إعداد مصدر البيانات
            cbxParentTree.DataSource = dt;
            cbxParentTree.DisplayMember = "DisplayText";
            cbxParentTree.ValueMember = "TreeAccCode"; // يعتمد على ParentTree في الجدول
            cbxParentTree.DropDownStyle = ComboBoxStyle.DropDownList; // 🔒 منع الكتابة اليدوية

            // 🔹 تحديد الحساب الأب الحالي بناءً على الرقم الشجري
            if (_parentTree.HasValue)
            {
                cbxParentTree.SelectedValue = _parentTree.Value;
            }
            else
            {
                cbxParentTree.SelectedIndex = -1; // لا يوجد أب
            }
        }

        public int UpdatedAccID { get; private set; }

        private int parentTree;
        private bool isForManager;
        private bool isHasDetails;
        private bool isHidden;
        private int accTypeID;
        private int parentchildren;


        int AccTypeID;
        // 🟢 دالة لجمع البيانات من الواجهة
        private void GetDataForModify()
        {
            parentTree = Convert.ToInt32(cbxParentTree?.SelectedValue ?? 0);
            isForManager = chkIsForManger.Checked;
            isHasDetails = chkIsHasDetails.Checked;
            isHidden = chkIsHidden.Checked;
            accTypeID = Convert.ToInt32(cbxAccTypeID?.SelectedValue ?? 0);
            parentchildren = Convert.ToInt32(lblTreeAccCode.Text);
        }

        // 🟢 عند الضغط على زر الحفظ
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                GetDataForModify();

                // تنفيذ التحديث الرئيسي
                string resultMsg = DBServiecs.Acc_UpdateAccount(
                    _accID,
                    txtAccName.Text.Trim(),
                    parentTree,
                    isForManager,
                    isHasDetails,
                    isHidden,
                    accTypeID
                );

                // التحقق من النتيجة
                if (resultMsg.StartsWith("❌") || resultMsg.Contains("خطأ"))
                {
                    MessageBox.Show(resultMsg, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ✅ إذا تم الحفظ بنجاح
                UpdatedAccID = _accID;

                string fullMessage = resultMsg;

                // هل نطبق نفس الخاصية على الأبناء؟
                if (chkImplementOnChildren.Checked)
                {
                    string childMsg = DBServiecs.Acc_UpdateImplementChild_ForManger(parentchildren, isForManager);

                    // ضم الرسالتين في نافذة واحدة
                    fullMessage += Environment.NewLine + childMsg;
                }

                MessageBox.Show(fullMessage, "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ غير متوقع: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private void btnSave_Click_(object sender, EventArgs e)
        //{
        //    GetDataForModify();
        //    // استدعاء الدالة
        //    string resultMsg = DBServiecs.Acc_UpdateAccount(
        //        _accID,
        //        txtAccName.Text,
        //        parentTree,
        //        isForManager,
        //        isHasDetails,
        //        isHidden, AccTypeID
        //    );

        //    // التحقق من النتيجة
        //    if (resultMsg.StartsWith("❌"))
        //    {
        //        MessageBox.Show(resultMsg, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    else
        //    {
        //        MessageBox.Show(resultMsg, "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

        //        UpdatedAccID = _accID;

        //        this.DialogResult = DialogResult.OK;
        //        if (chkImplementOnChildren.Checked) 
        //        {
        //            ImplementOnChildren();
        //        }
                
        //        this.Close();
        //    }
        //}
        //private void ImplementOnChildren()
        //{
        //    // استدعاء الدالة
        //    string resultMsg = DBServiecs.Acc_UpdateImplementChild_ForManger(
        //        parentTree,
        //        isForManager
        //    );

        //    // التحقق من النتيجة
        //    if (resultMsg.StartsWith("❌"))
        //    {
        //        MessageBox.Show(resultMsg, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    else
        //    {
        //        MessageBox.Show(resultMsg, "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    }
        //}
        private void btnClose_Click(object sender, EventArgs e)
        {
            this .Close();
        }
    }
}
