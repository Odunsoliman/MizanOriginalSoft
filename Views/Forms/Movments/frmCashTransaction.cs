using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.Views.Forms.MainForms;
using MizanOriginalSoft.MainClasses.OriginalClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlTypes;

namespace MizanOriginalSoft .Views.Forms.Movments
{
    public partial class frmCashTransaction : Form
    {
        private  int Type_ID; private  int BoxID;
        private int Payment_MethodID;
        public DataTable? tblTree;

        DataTable? tblAccBox;bool isLode=false ;  
        public enum TransactionType
        {
            Receipt = 9,   // تحصيل
            Payment = 8,   // صرف
            Dept = 13, // تسوية مدينة
            Credet = 14 // تسوية دائنة

        }

        public frmCashTransaction(int typMov)
        {
            InitializeComponent();
            Type_ID = typMov;
            TransactionType transactionType = (TransactionType)typMov;

            switch (transactionType)
            {
                case TransactionType.Receipt:
                    this.Text = "سند تحصيل نقدية";
                    lblOperationType.Text = "سند تحصيل نقدية رقم:";
                    lblBox.Text = "الى:";
                    break;

                case TransactionType.Payment:
                    this.Text = "صرف نقدية";
                    lblOperationType.Text = "سند صرف نقدية رقم:";
                    lblBox.Text = "من:";
                    break;

                case TransactionType.Dept:
                    this.Text = "سند قيد تسوية";
                    lblOperationType.Text = "سند قيد تسوية مدينة رقم:";
                    break;

                case TransactionType.Credet:
                    this.Text = "سند قيد تسوية";
                    lblOperationType.Text = "سند قيد تسوية دائنة رقم:";
                    break;
                default:
                    lblOperationType.Text = "عملية غير معروفة";
                    break;
            }
            createdByUsID = CurrentSession .UserID ;
            lblCreatedByUsID.Text = createdByUsID.ToString();
        }


        private void frmCashTransaction_Load(object sender, EventArgs e)
        {
            DBServiecs.A_UpdateAllDataBase ();

            LoadTreeAndSelectSpecificNode();

            FillPaymentMethods();   // يجب أن تسبق
            GetBills();             // لأن ShowBill يستخدم cbxPaymentMethod
            isLode = true ;
            AttachAutoSaveEvents(); tblAccBox = DBServiecs.MainAcc_LoadFinalAccounts(3, "All"); // "ZERO"E   "NEG"M   "POS"P
            cbxBox.DataSource = tblAccBox;
            cbxBox.ValueMember = "AccID";
            cbxBox.DisplayMember = "AccName";
            cbxBox.SelectedIndex = -1;
            cbxBox.DropDownWidth = 150; 
            isLode = false;
            txtSearchVoucher.Focus ();
        }

        #region @@@ Tree @@@@@@@@@
        //ونفس الدالة فى شاشة السندات لملئ شجرة الحسابات 
        //تحميل شجرة التصنيفات  ###
        private void LoadTreeAndSelectSpecificNode(int selectedID = 0)
        {
            // جلب جميع البراند
            tblTree = DBServiecs.MainAcc_GetTopAccountTree();

            treeViewTopAccounts.Nodes.Clear();

            DataTable? dt = tblTree;

            if (dt == null || dt.Rows.Count == 0)
                return; // لا يوجد بيانات

            foreach (DataRow row in dt.Rows)
            {
                if (row["ParentAccID"] == DBNull.Value || Convert.ToInt32(row["ParentAccID"]) == 0)
                {
                    TreeNode parentNode = new TreeNode(row["AccName"].ToString());
                    parentNode.Tag = Convert.ToInt32(row["AccID"]);
                    treeViewTopAccounts.Nodes.Add(parentNode);
                    AddChildNodes(dt, parentNode);
                }
            }

            if (selectedID > 0)
                SelectNodeById(treeViewTopAccounts.Nodes, selectedID);
        }

        //تحميل الفروع داخل الشجرة  ###
        private void AddChildNodes(DataTable dt, TreeNode parentNode)
        {
            int parentId = Convert.ToInt32(parentNode.Tag);
            foreach (DataRow row in dt.Select($"ParentAccID = {parentId}"))
            {
                TreeNode childNode = new TreeNode(row["AccName"].ToString());
                childNode.Tag = Convert.ToInt32(row["AccID"]);
                parentNode.Nodes.Add(childNode);
                AddChildNodes(dt, childNode);
            }
        }

        // وظيفة اختيار عقدة بواسطة رقم المعرف
        private void SelectNodeById(TreeNodeCollection nodes, int id)
        {
            foreach (TreeNode node in nodes)
            {
                if (Convert.ToInt32(node.Tag) == id)
                {
                    treeViewTopAccounts.SelectedNode = node;
                    //treeViewCategories.Focus();
                    return;
                }

                if (node.Nodes.Count > 0)
                    SelectNodeById(node.Nodes, id);
            }
        }
        int selectedTopID;
        //private void treeViewTopAccounts_AfterSelect(object sender, TreeViewEventArgs e)
        //{
        //    if (e.Node != null && e.Node.Tag != null)
        //    {
        //        selectedTopID = Convert.ToInt32(e.Node.Tag);
        //        AccountDGV(selectedTopID);

        //    }
        //    DGVStyl();
        //}
        private void treeViewTopAccounts_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null && e.Node.Tag != null)
            {
                selectedTopID = Convert.ToInt32(e.Node.Tag);

                if (e.Node.Text == ".")
                    AccountDGV(0);          // عرض كل الحسابات
                else
                    AccountDGV(selectedTopID);  // عرض الحسابات التابعة للحساب المحدد
            }
        }


        private void txtSearchTree_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearchTree.Text.Trim();

            // أعد كل الألوان للوضع الطبيعي
            ResetTreeNodeColors(treeViewTopAccounts.Nodes);

            if (string.IsNullOrWhiteSpace(searchText))
                return;

            // البحث وتمييز النتائج
            HighlightMatchingNodes(treeViewTopAccounts.Nodes, searchText);
        }

        private void HighlightMatchingNodes(TreeNodeCollection nodes, string searchText)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text.Contains(searchText))
                {
                    node.BackColor = Color.Yellow;
                    node.ForeColor = Color.Black;
                    ExpandParentNodes(node); // لعرض العقدة حتى لو كانت داخلية
                }

                if (node.Nodes.Count > 0)
                {
                    HighlightMatchingNodes(node.Nodes, searchText);
                }
            }
        }

        private void ResetTreeNodeColors(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                node.BackColor = treeViewTopAccounts.BackColor;
                node.ForeColor = treeViewTopAccounts.ForeColor;

                if (node.Nodes.Count > 0)
                {
                    ResetTreeNodeColors(node.Nodes);
                }
            }
        }

        private void ExpandParentNodes(TreeNode node)
        {
            TreeNode current = node;
            while (current.Parent != null)
            {
                current.Parent.Expand();
                current = current.Parent;
            }
        }

        private void EmptyLabelValue()
        {

            lblAccountID.Text = "";
            lblAccName.Text = "";
            lblBalance.Text = "";
            lblBalanceState.Text = "";
            lblFirstPhon.Text = "";
            lblAntherPhon.Text = "";
            lblAccNote.Text = "";
            lblClientEmail.Text = "";
            lblClientAddress.Text = "";
            lblBalanceNow.Text = string.Empty;
        }

        #endregion


        #region @@@@ Main Data @@@@@
        private void cbxBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(isLode)return ;
            if (cbxBox.SelectedValue != null && int.TryParse(cbxBox.SelectedValue.ToString(), out int id))
            {
                BoxID = id;lbl_Box .Text = cbxBox .Text ;
            }
        }

        // قائمة طرق الدفع
        private List<PaymentMethod> allMethods = new List<PaymentMethod>();
        //تعبئة الكمبوبكس لانواع الدفع
        private void FillPaymentMethods()
        {
            if (Type_ID == 8 || Type_ID == 9)
            {
                allMethods = new List<PaymentMethod>
        {
            new PaymentMethod { ID = 1, NamePaymentMethod = "نقدًا" },
            new PaymentMethod { ID = 2, NamePaymentMethod = "فيزا" },
            new PaymentMethod { ID = 3, NamePaymentMethod = "تحويل بنكي" },
            new PaymentMethod { ID = 5, NamePaymentMethod = "محفظة إلكترونية" },
        };
                tplPaymentMethod.Visible = true; // تأكد أن التحكم ظاهر إذا أردت
            }
            else if (Type_ID == 13 || Type_ID == 14)
            {
                allMethods = new List<PaymentMethod>
        {
            new PaymentMethod { ID = 9, NamePaymentMethod = "تسوية دفترية" }
        };
                tplPaymentMethod.Visible = false;
            }
            else
            {
                // حالة افتراضية: اجعل القائمة فارغة أو قم بتعبئتها بقيم أخرى إذا أردت
                allMethods = new List<PaymentMethod>();
                tplPaymentMethod.Visible = true; // أو false حسب ما تريد
            }

            cbxPaymentMethod.DataSource = allMethods;
            cbxPaymentMethod.DisplayMember = "NamePaymentMethod";
            cbxPaymentMethod.ValueMember = "ID";
            cbxPaymentMethod.DropDownWidth = 150;

            cbxPaymentMethod.SelectedIndex = -1;

            if (allMethods.Any())
            {
                Payment_MethodID = allMethods.First().ID;
            }
            else
            {
                Payment_MethodID = 0; // أو أي قيمة مناسبة عند عدم وجود طرق دفع
            }
        }

        //عند الاختيار النص وتحديد القيمة
        private void cbxPaymentMethod_Enter(object sender, EventArgs e)
        {
  //          cbxPaymentMethod.DroppedDown = true;
        }

        //جلب السندات حسب النوع
        DataTable? tblBills;
        int currentIndex = 0;

        //جلب السندات بترتيب تصاعدى
        private void GetBills(int? focusTransactionID = null)
        {
            tblBills = DBServiecs.CashTransactions_GetBillByType(Type_ID);

            if (tblBills == null || tblBills.Rows.Count == 0)
            {
                currentIndex = 0;
                ShowBill(currentIndex); // عرض سند جديد فارغ
                return;
            }

            tblBills.DefaultView.Sort = "TransactionID ASC";
            tblBills = tblBills.DefaultView.ToTable();

            currentIndex = tblBills.Rows.Count; // يقف على سند جديد افتراضياً

            // البحث عن السند المطلوب إن وجد
            if (focusTransactionID.HasValue)
            {
                for (int i = 0; i < tblBills.Rows.Count; i++)
                {
                    if (Convert.ToInt32(tblBills.Rows[i]["TransactionID"]) == focusTransactionID.Value)
                    {
                        currentIndex = i;
                        break;
                    }
                }
            }

            ShowBill(currentIndex);
        }

        private void ShowBill(int index)
        {
            if (tblBills == null || index < 0 || index > tblBills.Rows.Count)
                return;

            if (tblBills.Rows.Count == 0)
            {
                ClearBillFields(); // لا توجد أي بيانات، تهيئة الحقول
                return;
            }

            if (index == tblBills.Rows.Count)
            {
                ClearBillFields(); // سند جديد
                return;
            }

            DataRow row = tblBills.Rows[index];

            // باقي الكود...
           // بيانات السند
            lblTransactionID.Text = GetSafeValue(row, "TransactionID");
            lblVoucherNumber.Text = GetSafeValue(row, "VoucherNumber");
            dtpTransactionDate.Value = Convert.ToDateTime(row["TransactionDate"]);
            lblOperationType_ID.Text = GetSafeValue(row, "OperationType_ID");
            lblAccountID.Text = GetSafeValue(row, "AccountID");
            txtAmount.Text = Convert.ToDecimal(row["Amount"]).ToString("N2");

            // طريقة الدفع
            if (tblBills.Columns.Contains("PaymentMethodID"))
                cbxPaymentMethod.SelectedValue = Convert.ToInt32(row["PaymentMethodID"]);
            else
                cbxPaymentMethod.SelectedIndex = -1;

            txtDescriptionNote.Text = GetSafeValue(row, "DescriptionNote");
            lblCreatedByUsID.Text = GetSafeValue(row, "CreatedByUsID");
            lblSaveTransaction.Text = GetSafeValue(row, "SaveTransaction");

            // بيانات الحساب الرئيسي (العميل أو المورد)
            lblAccName.Text = GetSafeValue(row, "AccountName");
            lblBalance.Text = GetSafeValue(row, "Balance");
            lblBalanceState.Text = GetSafeValue(row, "BalanceState");
            lblFirstPhon.Text = GetSafeValue(row, "FirstPhon");
            lblAntherPhon.Text = GetSafeValue(row, "AntherPhon");
            lblClientEmail.Text = GetSafeValue(row, "ClientEmail");
            lblClientAddress.Text = GetSafeValue(row, "ClientAddress");

            // بيانات حساب الصندوق
            lbl_Box.Text = GetSafeValue(row, "BoxAccountName");

            if (tblBills.Columns.Contains("BoxID") && row["BoxID"] != DBNull.Value)
                cbxBox.SelectedValue = Convert.ToInt32(row["BoxID"]);
            else
                cbxBox.SelectedIndex = -1;


            AttachAutoSaveEvents(); // مراقبة التعديلات
        }


        ////private void ShowBill(int index)
        ////{
        ////    if (tblBills == null || tblBills.Rows.Count == 0 || index < 0 || index > tblBills.Rows.Count)
        ////        return;//هنا فى بداية البرنامج تكون الجدول فارغة فتخرج من الدالة والمطلوب فى هذه الحالة تفعيل 
        ////               //الدالة ClearBillFields(); فكيف يكون ذلك

        ////    if (index == tblBills.Rows.Count)
        ////    {
        ////        ClearBillFields(); // سند جديد
        ////        return;
        ////    }

        ////    DataRow row = tblBills.Rows[index];

        ////    // بيانات السند
        ////    lblTransactionID.Text = GetSafeValue(row, "TransactionID");
        ////    lblVoucherNumber.Text = GetSafeValue(row, "VoucherNumber");
        ////    dtpTransactionDate.Value = Convert.ToDateTime(row["TransactionDate"]);
        ////    lblOperationType_ID.Text = GetSafeValue(row, "OperationType_ID");
        ////    lblAccountID.Text = GetSafeValue(row, "AccountID");
        ////    txtAmount.Text = Convert.ToDecimal(row["Amount"]).ToString("N2");

        ////    // طريقة الدفع
        ////    if (tblBills.Columns.Contains("PaymentMethodID"))
        ////        cbxPaymentMethod.SelectedValue = Convert.ToInt32(row["PaymentMethodID"]);
        ////    else
        ////        cbxPaymentMethod.SelectedIndex = -1;

        ////    txtDescriptionNote.Text = GetSafeValue(row, "DescriptionNote");
        ////    lblCreatedByUsID.Text = GetSafeValue(row, "CreatedByUsID");
        ////    lblSaveTransaction.Text = GetSafeValue(row, "SaveTransaction");

        ////    // بيانات الحساب الرئيسي (العميل أو المورد)
        ////    lblAccName.Text = GetSafeValue(row, "AccountName");
        ////    lblBalance.Text = GetSafeValue(row, "Balance");
        ////    lblBalanceState.Text = GetSafeValue(row, "BalanceState");
        ////    lblFirstPhon.Text = GetSafeValue(row, "FirstPhon");
        ////    lblAntherPhon.Text = GetSafeValue(row, "AntherPhon");
        ////    lblClientEmail.Text = GetSafeValue(row, "ClientEmail");
        ////    lblClientAddress.Text = GetSafeValue(row, "ClientAddress");

        ////    // بيانات حساب الصندوق
        ////    lbl_Box.Text = GetSafeValue(row, "BoxAccountName");

        ////    if (tblBills.Columns.Contains("BoxID") && row["BoxID"] != DBNull.Value)
        ////        cbxBox.SelectedValue = Convert.ToInt32(row["BoxID"]);
        ////    else
        ////        cbxBox.SelectedIndex = -1;


        ////    AttachAutoSaveEvents(); // مراقبة التعديلات
        ////}

        private string? GetSafeValue(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName) ? row[columnName].ToString() : string.Empty;//Possible null reference return.
        }

        private void ClearBillFields()
        {
            int newID = DBServiecs.CashTransactions_GetNextTransactionID();
            lblTransactionID.Text = newID.ToString();

            string newVoucher = DBServiecs.CashTransactions_GetNewVoucherNumber(Type_ID);
            lblVoucherNumber.Text = newVoucher;

            dtpTransactionDate.Value = DateTime.Now;
            lblOperationType_ID.Text = Type_ID.ToString();

            lblAccountID.Text = string.Empty;
            txtAmount.Text = "0.00";
            cbxPaymentMethod.SelectedIndex = -1;
            lblMethod.Text = string.Empty;

            cbxBox.SelectedIndex = -1;
            lbl_Box.Text = string.Empty;

            txtDescriptionNote.Text = string.Empty;
            lblCreatedByUsID.Text = createdByUsID.ToString();
            lblSaveTransaction.Text = string.Empty;

            lblAccName.Text = string.Empty;
            lblBalance.Text = string.Empty;
            lblBalanceState.Text = string.Empty;
            lblFirstPhon.Text = string.Empty;
            lblAntherPhon.Text = string.Empty;
            lblClientEmail.Text = string.Empty;
            lblClientAddress.Text = string.Empty;
        }

        #endregion

        #region @@@ Row @@@@
        private void btnFirst_Click(object sender, EventArgs e)
        {
            if (tblBills == null || tblBills.Rows.Count == 0) return;
            currentIndex = 0;
            ShowBill(currentIndex);
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (tblBills == null || tblBills.Rows.Count == 0) return;
            if (currentIndex > 0)
            {
                currentIndex--;
                ShowBill(currentIndex);
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (tblBills == null || tblBills.Rows.Count == 0) return;
            if (currentIndex < tblBills.Rows.Count - 1)
            {
                currentIndex++;
                ShowBill(currentIndex);
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            if (tblBills == null || tblBills.Rows.Count == 0) return;
            currentIndex = tblBills.Rows.Count - 1;
            ShowBill(currentIndex);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            currentIndex = tblBills?.Rows.Count ?? 0;
            ShowBill(currentIndex); // هذا سيستدعي ClearBillFields تلقائيًا
            AttachAutoSaveEvents();
        }

        #endregion

        #region @@@ Search Bill @@@@
        private string? BuildFullVoucherCode(string shortSerial)
        {
            string? year = DateTime.Now.Year.ToString();
            string? operationCode = GetOperationCodeByTypeID(Type_ID); // يجب أن تنفذ هذه الدالة
            int serial;

            if (!int.TryParse(shortSerial, out serial))
                return null;

            return $"{year}-{operationCode}-{serial.ToString("D6")}";
        }
        private string GetOperationCodeByTypeID(int typeId)
        {
            // عدل حسب نوع الكودات لديك
            switch (typeId)
            {
                case 8: return "PY"; // مثلاً إيصال قبض
                case 9: return "RC"; // مثلاً إيصال صرف
                case 10: return "AD"; // تسوية دفترية
                default: return "X";
            }
        }

        //طريقة البحث المتتطور
        private void txtSearchVoucher_Leave(object sender, EventArgs e)
        {
            string input = txtSearchVoucher.Text.Trim();

            if (string.IsNullOrWhiteSpace(input)) return;

            string? fullVoucher = BuildFullVoucherCode(input);

            if (string.IsNullOrEmpty(fullVoucher))
            {
                MessageBox.Show("الرقم غير صالح.");
                return;
            }

            SearchAndShowVoucher(fullVoucher);
        }

        private void txtSearchVoucher_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // لمنع صدور صوت الـ Beep
                txtSearchVoucher_Leave(sender, e); // استدعاء الحدث يدويًا
                txtSearchVoucher .SelectAll ();

            }
            
        }
        private void txtSearchVoucher_KeyPress(object sender, KeyPressEventArgs e)
        {
            // السماح فقط بالأرقام (0-9) ومفتاح Backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // تجاهل الحرف
            }
        }

        private void SearchAndShowVoucher(string voucher)
        {
            if (tblBills == null) return;

            for (int i = 0; i < tblBills.Rows.Count; i++)
            {
                if (tblBills.Rows[i]["VoucherNumber"].ToString() == voucher)
                {
                    currentIndex = i;
                    ShowBill(currentIndex);
                    return;
                }
            }

            MessageBox.Show("السند غير موجود.");
        }

        #endregion 

  
        #region @@@@@@@@ DGV @@@@@@@@@

        DataTable? tblAcc;
        public void AccountDGV(int TopID)
        {
            txtSearch.Text = "";

            // تحميل كل الحسابات من المصدر
            tblAcc = DBServiecs.MainAcc_LoadFinalAccounts(TopID, "All");

            // فلترة الحسابات التي AccID >= 200
            DataRow[] filteredRows = tblAcc.Select("AccID >= 200");

            if (filteredRows.Length > 0)
                DGV.DataSource = filteredRows.CopyToDataTable();
            else
                DGV.DataSource = null;

            // تمرير TopID للتنسيق
            DGVStyl(TopID);
        }

        private void DGVStyl(int TopID)
        {
            if (DGV.DataSource == null)
                return;

            if (DGV.DataSource is DataTable dt)
            {
                dt.DefaultView.Sort = "AccID ASC";
                DGV.DataSource = dt.DefaultView.ToTable();
            }

            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            string[] visibleColumns;

            if (TopID == 0)
            {
                visibleColumns = new string[] { "AccName", "ParentAccName" };
            }
            else
            {
                visibleColumns = new string[] { "AccName" };
            }

            foreach (DataGridViewColumn col in DGV.Columns)
            {
                col.Visible = visibleColumns.Contains(col.Name);
            }

            if (DGV.Columns.Contains("AccName"))
            {
                DGV.Columns["AccName"].FillWeight = 1;
                DGV.Columns["AccName"].HeaderText = "الاسم";
                DGV.Columns["AccName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }

            if (TopID == 0 && DGV.Columns.Contains("ParentAccName"))
            {
                DGV.Columns["ParentAccName"].FillWeight = 1;
                DGV.Columns["ParentAccName"].HeaderText = "التصنيف";
                DGV.Columns["ParentAccName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }

            // تحديد حجم الخط حسب TopID
            int fontSize = (TopID == 0) ? 14 : 16;

            Font rowFont = new Font("Times New Roman", fontSize, FontStyle.Bold);

            DGV.DefaultCellStyle.Font = rowFont;
            DGV.DefaultCellStyle.ForeColor = Color.Black;
            DGV.DefaultCellStyle.BackColor = Color.White;

            DGV.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", fontSize, FontStyle.Bold);
            DGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            for (int i = 0; i < DGV.Rows.Count; i++)
            {
                var row = DGV.Rows[i];

                if (i % 2 == 0)
                    row.DefaultCellStyle.BackColor = Color.White;
                else
                    row.DefaultCellStyle.BackColor = Color.LightYellow;

                row.DefaultCellStyle.ForeColor = Color.Black;

                if (row.Cells["AccID"].Value != null &&
                    int.TryParse(row.Cells["AccID"].Value.ToString(), out int accId) &&
                    accId < 70)
                {
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }

            DGV.RowTemplate.Height = rowFont.Height + 11;
            DGV.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            DGV.AllowUserToResizeRows = false;
            DGV.ClearSelection();
            EmptyLabelValue();
        }

        private void DGV_SelectionChanged(object sender, EventArgs e)
        {
            if (DGV.CurrentRow == null || DGV.CurrentRow.Index < 0)
                return;

            // تأكد من أن الصف ليس صفًا جديدًا
            if (DGV.CurrentRow.IsNewRow)
                return;

            // قراءة القيم وتعيينها للـ Labels
            SetLabelValue(lblAccountID, "AccID");
            SetLabelValue(lblAccName, "AccName");
            SetLabelValue(lblBalance, "Balance");
            SetLabelValue(lblBalanceState, "BalanceState");
            SetLabelValue(lblFirstPhon, "FirstPhon", "Phon1:  ");
            SetLabelValue(lblAntherPhon, "AntherPhon", "Phon2:  ");
            SetLabelValue(lblAccNote, "AccNote");
            SetLabelValue(lblClientEmail, "ClientEmail");
            SetLabelValue(lblClientAddress, "ClientAddress");
            lblBalanceNow.Text = "الرصيد الحالي ";
        }

        private void SetLabelValue(Label lbl, string columnName, string prefix = "")
        {
            object value = DGV.CurrentRow.Cells[columnName].Value;

            if (value != DBNull.Value && !string.IsNullOrWhiteSpace(value?.ToString()))
                lbl.Text = prefix + value.ToString();
            else
                lbl.Text = "";
        }

        private void DGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (DGV.CurrentRow == null || DGV.CurrentRow.Index < 0)
                return;

            // تأكد من أن الصف ليس صفًا جديدًا
            if (DGV.CurrentRow.IsNewRow)
                return;

            // قراءة القيم وتعيينها للـ Labels
            SetLabelValue(lblAccountID, "AccID");
            SetLabelValue(lblAccName, "AccName");
            SetLabelValue(lblBalance, "Balance");
            SetLabelValue(lblBalanceState, "BalanceState");
            SetLabelValue(lblFirstPhon, "FirstPhon", "Phon1:  ");
            SetLabelValue(lblAntherPhon, "AntherPhon", "Phon2:  ");
            SetLabelValue(lblAccNote, "AccNote");
            SetLabelValue(lblClientEmail, "ClientEmail");
            SetLabelValue(lblClientAddress, "ClientAddress");
        }

        ////دالة البحث بجزء من الاسم او بدايات ارقام الهواتف
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (tblAcc == null || tblAcc.Rows.Count == 0)
                return;

            string searchText = txtSearch.Text.Trim().Replace("'", "''"); // حماية من مشاكل الاقتباسات

            // شرط AccID > 200 مع شروط البحث في الاسم وأرقام الهواتف
            string filter = $"AccID > 200 AND (AccName LIKE '%{searchText}%' " +
                            $"OR FirstPhon LIKE '{searchText}%' " +
                            $"OR AntherPhon LIKE '{searchText}%')";

            DataView dv = tblAcc.DefaultView;
            dv.RowFilter = filter;

            DGV.DataSource = dv.ToTable();

            DGVStyl(selectedTopID);
        }


 
        #endregion


        #region @@@@ Save @@@

        // =====================
        // المتغيرات الخاصة بالحفظ
        // =====================
        private int transactionID;
        private string voucherNumber = string.Empty;
        private DateTime transactionDate;
        private int accountID;
        private float amount;
        private int paymentMethodID;
        private string descriptionNote = string.Empty;
        private int createdByUsID;
        private string saveTransaction = string.Empty;

        // حفظ نهائي أم مسودة
        private bool isFinalSave;

        // قيمة أصلية للمقارنة قبل وبعد التعديل
        private string originalValue = string.Empty;

        // =====================
        // زر الحفظ
        // =====================
        private void btnSave_Click(object sender, EventArgs e)
        {
            // تحقق إضافي إذا كان النوع 8 أو 9
            if (!ValidateBoxAndPaymentForSpecialTypes())
                return;

            // تعبئة المتغيرات من الواجهة
            isFinalSave = true;
            ItemSave();

            // تحقق من المدخلات الأساسية
            if (!ValidateBasicInputs())
                return;

            // حفظ المسودة/الفاتورة
            SaveDraftInvoice();
            isFinalSave = false;

            // إعادة تحميل البيانات
            AccountDGV(selectedTopID);
            tblBills = DBServiecs.CashTransactions_GetBillByType(Type_ID);

            if (tblBills != null)
            {
                tblBills.DefaultView.Sort = "TransactionID ASC";
                tblBills = tblBills.DefaultView.ToTable();
            }
            else
            {
                // مثلاً عرض رسالة أو تسجيل ملاحظة أن الجدول فارغ
                MessageBox.Show("لم يتم العثور على بيانات الفواتير.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // عرض السند المحفوظ الحالي
            ShowBillByTransactionID(transactionID);
        }

        // =====================
        // التحقق من الصندوق وطريقة الدفع إذا كان النوع 8 أو 9
        // =====================
        private bool ValidateBoxAndPaymentForSpecialTypes()
        {
            if (Type_ID != 8 && Type_ID != 9)
                return true;

            bool isValid = true;

            // تحقق من الصندوق
            if (cbxBox.SelectedValue == null || Convert.ToInt32(cbxBox.SelectedValue) <= 0)
            {
                lbl_Box.Text = "❓";
                lbl_Box.ForeColor = Color.Red;
                isValid = false;
            }
            else
            {
                lbl_Box.Text = cbxBox.Text;
            }

            // تحقق من طريقة الدفع
            if (cbxPaymentMethod.SelectedValue == null || Convert.ToInt32(cbxPaymentMethod.SelectedValue) <= 0)
            {
                lblMethod.Text = "❓";
                lblMethod.ForeColor = Color.Red;
                isValid = false;
            }
            else
            {
                lblMethod.Text = cbxPaymentMethod.Text;
            }

            if (!isValid)
            {
                MessageBox.Show("يرجى استكمال الحقول المطلوبة قبل الحفظ.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return isValid;
        }

        // =====================
        // التحقق من المدخلات الأساسية
        // =====================
        private bool ValidateBasicInputs()
        {
            if (transactionID <= 0)
            {
                MessageBox.Show("رقم السند غير صالح.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (accountID <= 0)
            {
                MessageBox.Show("يرجى اختيار حساب صالح.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (amount <= 0)
            {
                MessageBox.Show("يرجى إدخال مبلغ صحيح أكبر من صفر.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (paymentMethodID <= 0)
            {
                MessageBox.Show("يرجى اختيار طريقة دفع.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        // =====================
        // تعبئة المتغيرات من الواجهة
        // =====================
        private void ItemSave()
        {
            int.TryParse(lblTransactionID.Text, out transactionID);
            voucherNumber = lblVoucherNumber.Text.Trim();
            transactionDate = dtpTransactionDate.Value;
            int.TryParse(lblAccountID.Text, out accountID);
            float.TryParse(txtAmount.Text, out amount);

            if (cbxPaymentMethod.SelectedValue != null)
                int.TryParse(cbxPaymentMethod.SelectedValue.ToString(), out paymentMethodID);
            else
                paymentMethodID = 0;

            descriptionNote = txtDescriptionNote.Text.Trim();

            saveTransaction = isFinalSave ? "تم الحفظ" : lblSaveTransaction.Text.Trim();
        }

        // =====================
        // حفظ المسودة/الفاتورة
        // =====================
        public void SaveDraftInvoice()
        {
            if (!string.IsNullOrWhiteSpace(lblSaveTransaction.Text))
            {
                MessageBox.Show("الفاتورة محفوظة نهائيًا، لا يمكن التعديل.");
                return;
            }

            ItemSave();

            string resultMessage;
            bool isSaved = DBServiecs.CashTransactions_InsertOrUpdate(
                transactionID,
                voucherNumber,
                transactionDate,
                Type_ID,
                accountID,
                BoxID, // ✅ الصندوق
                amount,
                paymentMethodID,
                descriptionNote,
                createdByUsID,
                saveTransaction,
                out resultMessage
            );

            if (isFinalSave && isSaved && !string.IsNullOrWhiteSpace(resultMessage))
            {
                MessageBox.Show(resultMessage, "تم الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            GetBills(transactionID);
        }

        // =====================
        // عرض السند المحفوظ بناءً على TransactionID
        // =====================

        private void ShowBillByTransactionID(int focusTransactionID)
        {
            if (tblBills == null || tblBills.Rows.Count == 0)
                return;

            for (int i = 0; i < tblBills.Rows.Count; i++)
            {
                if (Convert.ToInt32(tblBills.Rows[i]["TransactionID"]) == focusTransactionID)
                {
                    currentIndex = i;
                    ShowBill(currentIndex);
                    break;
                }
            }
        }

 
        // =====================
        // تفعيل/تعطيل الحقول حسب حالة الحفظ
        // =====================
        private void AttachAutoSaveEvents()
        {
            bool isSaved = !string.IsNullOrWhiteSpace(lblSaveTransaction.Text);

            cbxPaymentMethod.Enabled = !isSaved;
            txtAmount.Enabled = !isSaved;
            txtDescriptionNote.Enabled = !isSaved;
            cbxBox.Enabled = !isSaved;
            DGV.Enabled = !isSaved;
            treeViewTopAccounts.Enabled = !isSaved;

            if (!isSaved)
            {
                // إزالة الربط السابق لمنع التكرار
                cbxPaymentMethod.Enter -= ValidateAccountBeforeEdit;
                txtAmount.Enter -= ValidateAccountBeforeEdit;
                txtDescriptionNote.Enter -= ValidateAccountBeforeEdit;

                cbxPaymentMethod.Leave -= Control_Leave;
                txtAmount.Leave -= Control_Leave;
                txtDescriptionNote.Leave -= Control_Leave;

                // إعادة ربط الأحداث
                cbxPaymentMethod.Enter += ValidateAccountBeforeEdit;
                txtAmount.Enter += ValidateAccountBeforeEdit;
                txtDescriptionNote.Enter += ValidateAccountBeforeEdit;

                cbxPaymentMethod.Leave += Control_Leave;
                txtAmount.Leave += Control_Leave;
                txtDescriptionNote.Leave += Control_Leave;
            }
        }

        // =====================
        // التحقق من اختيار الحساب قبل التعديل
        // =====================
        private void ValidateAccountBeforeEdit(object? sender, EventArgs e)
        {
            bool hasAccount = int.TryParse(lblAccountID.Text, out int accId) && accId > 0;

            if (!hasAccount)
            {
                MessageBox.Show("يرجى اختيار حساب أولاً قبل إدخال البيانات.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lblAccountID.Focus();
            }
            else
            {
                Control_Enter(sender, e);
            }
        }

        // =====================
        // عند الدخول إلى الحقل
        // =====================
        private void Control_Enter(object? sender, EventArgs e)
        {
            if (sender is TextBox txt)
                originalValue = txt.Text;
            else if (sender is ComboBox cbx)
                originalValue = cbx.SelectedValue?.ToString() ?? string.Empty;
        }

        // =====================
        // عند الخروج من الحقل
        // =====================
        private void Control_Leave(object? sender, EventArgs e)
        {
            string newValue = sender switch
            {
                TextBox txt => txt.Text,
                ComboBox cbx => cbx.SelectedValue?.ToString() ?? string.Empty,
                _ => string.Empty
            };

            if (originalValue != newValue)
            {
                SaveDraftInvoice();
            }
        }

        #endregion

        private void btnPrent_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(lblSaveTransaction.Text))
            {
                MessageBox.Show("جاري إعداد التقرير...", "طباعة", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // إعداد التقرير هنا
                // PrepareReport(transactionID);
            }
            else
            {
                MessageBox.Show("يجب حفظ السند أولاً قبل الطباعة.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnJournal_Click(object sender, EventArgs e)
        {
            // تحقق من أن السند محفوظ (أي أن lblSave تحتوي على نص)
            if (!string.IsNullOrWhiteSpace(lblSaveTransaction.Text))
            {
                int billNo;

                // تأكد من أن رقم السند ونوع الفاتورة متوفران بشكل صحيح
                if (int.TryParse(lblTransactionID .Text, out billNo))
                {
                    frm_Journal journalForm = new frm_Journal(billNo, Type_ID);
                    journalForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("تأكد من رقم السند ونوع العملية", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("يجب حفظ السند أولًا قبل عرض القيد المحاسبي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cbxPaymentMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMethod .Text = cbxPaymentMethod.Text;    
        }
  
    
    
    }
}
