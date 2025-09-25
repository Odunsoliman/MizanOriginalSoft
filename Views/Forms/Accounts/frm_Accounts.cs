using MizanOriginalSoft.MainClasses;
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

namespace MizanOriginalSoft.Views.Forms.Accounts
{
    public partial class frm_Accounts : Form
    {
        public frm_Accounts()
        {
            InitializeComponent();
        }

        private void frm_Accounts_Load(object sender, EventArgs e)
        {
            LoadAccountsTree();
        }
        #region !!!!!!! بناء الشجرة  !!!!!!!
        //اريد نسخة منقحة
        private void LoadAccountsTree()
        {
            treeViewAccounts.Nodes.Clear();

            DataTable dt = DBServiecs.Acc_GetChart() ?? new DataTable();
            if (dt.Rows.Count == 0) return;

            foreach (DataRow row in dt.Rows)
            {
                // تجاهل الصفوف التي تحتوي على قيم غير صالحة
                if (row["FullPath"] == DBNull.Value || row["AccName"] == DBNull.Value || row["AccID"] == DBNull.Value)
                    continue;

                string fullPath = row["FullPath"] as string ?? string.Empty;
                string accName = row["AccName"] as string ?? string.Empty;
                string parentAccID = row["ParentAccID"] as string ?? string.Empty;
                string balance = row["Balance"] as string ?? string.Empty;
                string balanceState = row["BalanceState"] as string ?? string.Empty;
                string isHidden = row["IsHidden"] as string ?? string.Empty;
                string dateOfJoin = row["DateOfJoin"] as string ?? string.Empty;

                if (string.IsNullOrWhiteSpace(fullPath) || string.IsNullOrWhiteSpace(accName))
                    continue;

                int level = GetLevelFromFullPath(fullPath);
                TreeNode node = new TreeNode(accName)
                {
                    Tag = row // حفظ كل البيانات لاستخدامها لاحقاً
                };

                if (level == 0)
                {
                    treeViewAccounts.Nodes.Add(node);
                }
                else
                {
                    TreeNode? parentNode = FindParentNode(treeViewAccounts.Nodes, fullPath, level - 1);
                    if (parentNode is not null) // التحقق باستخدام is not null
                    {
                        parentNode.Nodes.Add(node);
                    }
                    else
                    {
                        treeViewAccounts.Nodes.Add(node); // fallback
                    }
                }

            }

            //  treeViewAccounts.ExpandAll();
            // بدلاً من ExpandAll، نغلق كل العقد
            treeViewAccounts.CollapseAll();
        }

        // دالة مساعدة لإيجاد الأب حسب FullPath
        private TreeNode? FindParentNode(TreeNodeCollection nodes, string fullPath, int targetLevel)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag is DataRow row)
                {
                    // تحقق من أن القيمة صالحة
                    if (row["FullPath"] == DBNull.Value)
                        continue;

                    string nodePath = row["FullPath"] as string ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(nodePath))
                        continue;

                    int nodeLevel = GetLevelFromFullPath(nodePath);

                    if (nodeLevel == targetLevel && fullPath.StartsWith(nodePath))
                        return node;

                    TreeNode? found = FindParentNode(node.Nodes, fullPath, targetLevel);
                    if (found != null)
                        return found;
                }
            }
            return null;
        }




        #endregion 

        #region !!!!!! بحث فى الشجرة  !!!!!!!!

        private List<TreeNode> matchedNodes = new List<TreeNode>();
        private int currentMatchIndex = -1;

        private void txtSearchTree_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearchTree.Text.Trim().ToLower();

            matchedNodes.Clear();
            currentMatchIndex = -1;

            // إعادة تعيين الألوان وإغلاق كل الفروع
            ResetNodeColorsAndCollapse(treeViewAccounts.Nodes);

            if (string.IsNullOrEmpty(searchText))
                return;

            // البحث وتلوين النتائج وفتح الفروع التي تحتوي نتائج
            SearchAndHighlightNodes(treeViewAccounts.Nodes, searchText);

            // اختيار أول نتيجة
            if (matchedNodes.Count > 0)
            {
                currentMatchIndex = 0;
                var node = matchedNodes[0];
                treeViewAccounts.SelectedNode = node;
                node.EnsureVisible();
            }
        }
        private void ResetNodeColorsAndCollapse(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                node.BackColor = treeViewAccounts.BackColor;
                node.ForeColor = treeViewAccounts.ForeColor;
                node.Collapse(); // إغلاق الفروع

                if (node.Nodes.Count > 0)
                    ResetNodeColorsAndCollapse(node.Nodes);
            }
        }

        private void SearchAndHighlightNodes(TreeNodeCollection nodes, string searchText)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text.ToLower().Contains(searchText))
                {
                    node.BackColor = Color.Yellow;
                    node.ForeColor = Color.Black;
                    matchedNodes.Add(node);

                    // فتح العقدة الأصلية
                    ExpandParentNodes(node);
                }

                if (node.Nodes.Count > 0)
                {
                    SearchAndHighlightNodes(node.Nodes, searchText);
                }
            }
        }

        private void ExpandParentNodes(TreeNode node)
        {
            TreeNode? parent = node.Parent;
            while (parent != null)
            {
                parent.Expand();
                parent = parent.Parent;
            }
        }

        #endregion
        // دالة لحساب المستوى من FullPath
        private int GetLevelFromFullPath(string fullPath)
        {
            return fullPath.Split(new string[] { "→" }, StringSplitOptions.None).Length - 1;
        }
        // دالة لبناء المسار الكامل من شجرة العقد
        private string GetFullPathFromNode(TreeNode node)
        {
            if (node == null)
                return string.Empty;

            List<string> parts = new List<string>();
            TreeNode? current = node;

            while (current != null)
            {
                parts.Insert(0, current.Text); // نضيف من البداية لتكون من الأصل إلى الفرع
                current = current.Parent;
            }

            return string.Join(" → ", parts); // يمكنك تغيير السهم أو الفاصل حسب رغبتك
        }


        private int parentAccID = 0;
        private bool isHasChildren = false;
        private bool isHasDetails = false;
        // حقل على مستوى الفورم لتخزين الحساب المحدد
        private DataRow? selectedRow = null;

        // حدث اختيار العقدة
        private void treeViewAccounts_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewAccounts.SelectedNode != null)
            {
                TreeNode node = treeViewAccounts.SelectedNode;

                if (node.Tag is DataRow row)
                {
                    selectedRow = row; // ✅ خزناها هنا

                    string? accID = row["AccID"].ToString();
                    string? accName = row["AccName"].ToString();
                    int? parentAccID = row["ParentAccID"] == DBNull.Value
                        ? (int?)null
                        : Convert.ToInt32(row["ParentAccID"]);

                    string balance = row["Balance"].ToString() ?? string.Empty;
                    string balanceState = row["BalanceState"].ToString() ?? string.Empty;
                    bool isHidden = Convert.ToBoolean(row["IsHidden"]);
                    isHasChildren = Convert.ToBoolean(row["IsHasChildren"]);
                    isHasDetails = row.Field<bool?>("IsHasDetails") ?? false;

                    string dateOfJoin = row["DateOfJoin"].ToString() ?? string.Empty;

                    // عرض رقم الحساب واسم الحساب
                    lblSelectedTreeNod.Text = accID + " - " + accName;

                    // عرض المسار الكامل بالأسماء
                    lblPathNode.Text = GetFullPathFromNode(node);

                    txtAccName.Enabled = isHasChildren;

                    if (!isHasChildren) // لو مش مسموح إضافة حسابات فرعية
                    {
                        txtAccName.Clear();
                        lblParentAccName.Text = "لا يمكن اضافة حسابات فرعية هنا فهذا حساب نهائى";
                        lblParentAccName.ForeColor = Color.Red;

                        chkIsHasChildren.Enabled = false;
                        tlpData.Visible = false;
                        btnNew.Visible = false;
                        btnSave.Visible = false;
                    }
                    else // مسموح
                    {
                        lblParentAccName.Text = accName;
                        lblParentAccName.ForeColor = Color.Gray;
                        chkIsHasChildren.Enabled = true;
                        btnNew.Visible = true;
                        btnSave.Visible = true;
                    }

                    // 🔹 تحقق إذا أي من الآباء (الجدود) هو 12
                    bool hasFixedAssetParent = false;
                    TreeNode? current = node;
                    while (current != null)
                    {
                        if (current.Tag is DataRow parentRow)
                        {
                            if (Convert.ToInt32(parentRow["AccID"]) == 12)
                            {
                                hasFixedAssetParent = true;
                                break;
                            }
                        }
                        current = current.Parent;
                    }

                    if (isHasDetails)
                    {
                        tlpData.Visible = true;
                        btnDetails.Text = hasFixedAssetParent ? "بيانات الأصل الثابت" : "بيانات شخصية";

                        // إعادة ضبط نسب الصفوف
                        tlpData.RowStyles[0].SizeType = SizeType.Percent;
                        tlpData.RowStyles[0].Height = 10; // الصف الأول ثابت 10%

                        if (btnDetails.Text == "بيانات شخصية")
                        {
                            tlpData.RowStyles[1].SizeType = SizeType.Percent;
                            tlpData.RowStyles[1].Height = 90;

                            tlpData.RowStyles[2].SizeType = SizeType.Percent;
                            tlpData.RowStyles[2].Height = 0;
                        }
                        else // "بيانات الأصل الثابت"
                        {
                            tlpData.RowStyles[1].SizeType = SizeType.Percent;
                            tlpData.RowStyles[1].Height = 0;

                            tlpData.RowStyles[2].SizeType = SizeType.Percent;
                            tlpData.RowStyles[2].Height = 90;
                        }
                    }
                    else
                    {
                        tlpData.Visible = false;
                    }
                }
            }
        }

        // زر الحفظ
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (selectedRow == null)
            {
                MessageBox.Show("من فضلك اختر حساب من الشجرة أولاً");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtAccName.Text))
            {
                MessageBox.Show("من فضلك أدخل اسم الحساب الجديد");
                return;
            }

            string AccName = txtAccName.Text.Trim();
            bool IsHasChildren = chkIsHasChildren.Checked;
            int ParentAccID = Convert.ToInt32(selectedRow["AccID"]);
            int CreateByUserID = CurrentSession.UserID;

            // 🟢 استدعاء الإجراء
            string result = DBServiecs.Acc_AddAccount(AccName, ParentAccID, CreateByUserID, IsHasChildren);

            if (result.StartsWith("تم")) // يعني نجحت العملية
            {
                MessageBox.Show("تم حفظ الحساب بنجاح ✅");

                // 🟢 حفظ الـ ID بتاع العقدة المحددة
                int currentNodeId = ParentAccID;

                // 🟢 إعادة تحميل الشجرة
                LoadAccountsTree();

                // 🟢 البحث عن العقدة بنفس الـ ID
                TreeNode? node = FindNodeByAccID(treeViewAccounts.Nodes, currentNodeId);

                if (node != null)
                {
                    treeViewAccounts.SelectedNode = node;
                    node.EnsureVisible(); // يخليها تبان حتى لو داخل فرع مغلق
                }

                // 🟢 فتح وتحديد العقدة الأب
                HighlightAndExpandNode(currentNodeId);
            }
            else
            {
                MessageBox.Show("فشل في الحفظ ❌\n" + result);
            }
        }
        //📌 دالة البحث عن العقدة بالـ AccID
        private TreeNode? FindNodeByAccID(TreeNodeCollection nodes, int accID)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag is DataRow row && Convert.ToInt32(row["AccID"]) == accID)
                    return node;

                TreeNode? found = FindNodeByAccID(node.Nodes, accID);
                if (found != null)
                    return found;
            }
            return null;
        }

        //📌 مثال على الاستخدام بعد البحث
        private void HighlightAndExpandNode(int accID)
        {
            TreeNode? node = FindNodeByAccID(treeViewAccounts.Nodes, accID);

            if (node != null)
            {
                // تحديد العقدة
                treeViewAccounts.SelectedNode = node;

                // تغيير لون الخلفية (لتوضيح)
                node.BackColor = Color.LightBlue;
                node.ForeColor = Color.DarkRed;

                // فتح العقدة لرؤية الإضافات
                node.Expand();

                // 📌 إذا أردت فتح كل الآباء حتى تصل للعقدة
                node.EnsureVisible();
            }
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            // إذا كانت مخفية يظهرها، وإذا كانت ظاهرة يخفيها
            tlpPhon.Visible = !tlpPhon.Visible;

        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            txtAccName .Clear();
            chkIsHasChildren .Checked = false ;
        }


    }
}
