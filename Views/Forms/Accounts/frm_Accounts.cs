using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses;
using MizanOriginalSoft.Views.Reports;
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
            SetupMenuStrip();

            treeViewAccounts.ItemHeight = treeViewAccounts.Font.Height + 12;
            treeViewAccounts.DrawMode = TreeViewDrawMode.OwnerDrawText;
            treeViewAccounts.DrawNode += treeViewAccounts_DrawNode;
        }

        #region Build Tree

        private void LoadAccountsTree()
        {
            treeViewAccounts.Nodes.Clear();
            DataTable dt = DBServiecs.Acc_GetChart() ?? new DataTable();
            if (dt.Rows.Count == 0) return;

            // Dictionary لتخزين العقد أثناء البناء
            Dictionary<string, TreeNode> nodeDict = new Dictionary<string, TreeNode>();

            // عرض الحسابات التي لها فروع فقط
            var parentRows = dt.AsEnumerable()
                               .Where(r => r.Field<bool>("IsHasChildren"))
                               .ToList();

            foreach (DataRow row in parentRows)
            {
                string accName = row["AccName"] as string ?? string.Empty;
                if (string.IsNullOrWhiteSpace(accName)) continue;

                string treeCode = row["TreeAccCode"].ToString();
                string parentCode = row["ParentAccID"] != DBNull.Value ? row["ParentAccID"].ToString() : null;

                TreeNode node = new TreeNode(accName) { Tag = row };
                nodeDict[treeCode] = node;

                if (string.IsNullOrEmpty(parentCode))
                    treeViewAccounts.Nodes.Add(node); // الجذر
                else if (nodeDict.TryGetValue(parentCode, out TreeNode parentNode))
                    parentNode.Nodes.Add(node);
                else
                    treeViewAccounts.Nodes.Add(node); // fallback
            }

            SortTreeNodes(treeViewAccounts.Nodes);
            treeViewAccounts.CollapseAll();
        }

        private void SortTreeNodes(TreeNodeCollection nodes)
        {
            List<TreeNode> nodeList = nodes.Cast<TreeNode>()
                                           .OrderBy(n =>
                                           {
                                               if (n.Tag is DataRow row)
                                                   return row["TreeAccCode"].ToString();
                                               return string.Empty;
                                           })
                                           .ToList();

            nodes.Clear();
            foreach (TreeNode node in nodeList)
            {
                nodes.Add(node);
                SortTreeNodes(node.Nodes); // ترتيب الأبناء
            }
        }

        #endregion

        #region TreeView Events

        private void treeViewAccounts_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewAccounts.SelectedNode == null) return;
            TreeNode selectedNode = treeViewAccounts.SelectedNode;
            if (selectedNode.Tag is not DataRow row) return;

            // تمييز العقدة النشطة
            if (activeNode != null)
            {
                activeNode.NodeFont = new Font("Times New Roman", 12, FontStyle.Bold);
                activeNode.ForeColor = Color.Black;
            }

            activeNode = selectedNode;
            activeNode.NodeFont = new Font(treeViewAccounts.Font.FontFamily,
                                           treeViewAccounts.Font.Size + 1,
                                           FontStyle.Bold);
            activeNode.ForeColor = Color.Red;
            treeViewAccounts.Refresh();

            // بيانات الحساب
            int treeAccCode = row.Field<int>("TreeAccCode");
            int accID = row.Field<int>("AccID");
            string accName = row["AccName"]?.ToString() ?? string.Empty;
            bool hasChildren = row.Field<bool?>("IsHasChildren") ?? false;
            bool hasDetails = row.Field<bool?>("IsHasDetails") ?? false;
            bool isEnerAcc = row.Field<bool?>("IsEnerAcc") ?? false;

            // تحديث DGV
            if (hasChildren)
                LoadChildrenInDGV(treeAccCode); // بناء على TreeAccCode
            else
                DGV.DataSource = null;

            selectedRow = row;

            // تحديث التسميات
            lblSelectedTreeNod.Text = $"{treeAccCode} - {accName}";
            lblPathNode.Text = GetFullPathFromNode(selectedNode);
            lblNameNod.Text = accName;

            // التحقق من إمكانية إضافة حساب فرعي
            bool canAddChild = !(isEnerAcc && !hasChildren);
            txtAccName.Enabled = canAddChild;
            if (!canAddChild)
            {
                txtAccName.Clear();
                lblParentAccName.Text = "لا يمكن اضافة حسابات فرعية هنا فهذا حساب نهائى";
                lblParentAccName.ForeColor = Color.Red;
                chkIsHasChildren.Enabled = false;
            }
            else
            {
                lblParentAccName.Text = accName;
                lblParentAccName.ForeColor = Color.Gray;
                chkIsHasChildren.Enabled = true;
            }

            lblIsHasChildren.Text = hasChildren ? "" : "هذا الحساب مازال ليس له فروع";

            // التحقق من الأصول الثابتة
            if (!hasDetails)
            {
                lblAccDataDetails.Text = "";
                tlpBtnExec.Enabled = false;
            }
            else
            {
                bool hasFixedAssetParent = false;
                TreeNode? currentNode = selectedNode;
                while (currentNode != null)
                {
                    if (currentNode.Tag is DataRow parentRow &&
                        Convert.ToInt32(parentRow["TreeAccCode"]) == 12)
                    {
                        hasFixedAssetParent = true;
                        break;
                    }
                    currentNode = currentNode.Parent;
                }
                lblAccDataDetails.Text = hasFixedAssetParent ? "بيانات الأصل الثابت" : "بيانات شخصية";
                tlpBtnExec.Enabled = true;
            }

            LoadReportsForSelectedAccount();
        }

        // ==========================
        // وظيفة: التحكم في سلوك توسيع العقد في شجرة الحسابات
        // الهدف: عند محاولة توسيع عقدة جذرية أساسية (TreeAccCode = 1 إلى 5)،
        //        يتم إغلاق جميع الجذور الأخرى تلقائيًا.
        // سبب ذلك: لتجنب فتح أكثر من جذر رئيسي في نفس الوقت، مما يحافظ
        //        على ترتيب ووضوح الشجرة.
        // ملاحظات:
        // 1) إذا كان المستخدم يقوم بالبحث (isSearchActive = true)،
        //    لا يتم غلق أي عقد أخرى حتى لا يتداخل البحث مع التوسيع.
        // 2) تتحقق الدالة أولًا من أن العقدة المراد توسيعها ليست null.
        // 3) ثم تتأكد أن العقدة تحتوي على بيانات من نوع DataRow.
        // 4) تستخدم TreeAccCode لتحديد ما إذا كانت العقدة جذرية أساسية (1–5).
        // 5) إذا تحقق الشرط، يتم المرور على جميع الجذور في الشجرة
        //    وإغلاقها باستثناء العقدة التي تم توسيعها.
        // ==========================
        private void treeViewAccounts_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (isSearchActive) return; // أثناء البحث، لا نقوم بأي غلق

            if (e.Node?.Tag is DataRow row)
            {
                int treeCode = row.Field<int>("TreeAccCode"); // رقم الحساب الشجري
                if (row["ParentAccID"] == DBNull.Value && treeCode >= 1 && treeCode <= 5)
                {
                    // أغلق كل الجذور الأخرى
                    foreach (TreeNode rootNode in treeViewAccounts.Nodes)
                    {
                        if (rootNode != e.Node)
                            rootNode.Collapse();
                    }
                }
            }
        }



        private void treeViewAccounts_DrawNode(object? sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = false;
            Font nodeFont = e.Node == activeNode
                ? new Font("Times New Roman", 13, FontStyle.Bold)
                : new Font("Times New Roman", 12, FontStyle.Bold);
            Color foreColor = e.Node == activeNode ? Color.Red : Color.Black;
            TextRenderer.DrawText(e.Graphics, e.Node!.Text, nodeFont, e.Bounds, foreColor);
        }

        #endregion

        #region Search

        private List<TreeNode> matchedNodes = new List<TreeNode>();
        private int currentMatchIndex = -1;

        private void txtSearchTree_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearchTree.Text.Trim().ToLower();
            matchedNodes.Clear();
            currentMatchIndex = -1;
            ResetNodeColorsAndCollapse(treeViewAccounts.Nodes);

            if (string.IsNullOrEmpty(searchText)) return;

            SearchAndHighlightNodes(treeViewAccounts.Nodes, searchText);
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
                node.Collapse();
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
                    ExpandParentNodes(node);
                }

                if (node.Nodes.Count > 0)
                    SearchAndHighlightNodes(node.Nodes, searchText);
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

        private void txtSearchTree_Enter(object sender, EventArgs e) => isSearchActive = true;
        private void txtSearchTree_Leave(object sender, EventArgs e) => isSearchActive = false;

        #endregion

        #region DGV & Utilities

        private void LoadChildrenInDGV(int parentTreeAccCode)
        {
            DataTable dt = DBServiecs.Acc_GetLeafChildren(parentTreeAccCode);
            DGV.DataSource = dt.DefaultView;
            DGVStyl();
        }

        private void DGVStyl()
        {
            DGV.ReadOnly = true;
            DGV.AllowUserToAddRows = false;
            DGV.AllowUserToDeleteRows = false;
            DGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGV.MultiSelect = false;
            DGV.RowHeadersVisible = false;

            foreach (DataGridViewColumn col in DGV.Columns)
            {
                col.Visible = col.Name == "AccName" || col.Name == "Balance" || col.Name == "BalanceState";
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                switch (col.Name)
                {
                    case "Balance":
                        col.HeaderText = "الرصيد";
                        col.FillWeight = 15;
                        break;
                    case "BalanceState":
                        col.HeaderText = "--";
                        col.FillWeight = 15;
                        break;
                    case "AccName":
                        col.HeaderText = "اسم الحساب";
                        col.FillWeight = 20;
                        break;
                }
            }

            Font generalFont = new Font("Times New Roman", 14, FontStyle.Bold);
            DGV.DefaultCellStyle.Font = generalFont;
            DGV.ColumnHeadersDefaultCellStyle.Font = generalFont;
            DGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            DGV.ColumnHeadersHeight = 60;
            DGV.EnableHeadersVisualStyles = false;

            DGV.ClearSelection();
        }

        private int GetLevelFromFullPath(string fullPath)
            => fullPath.Split(new string[] { "→" }, StringSplitOptions.None).Length - 1;

        private string GetFullPathFromNode(TreeNode node)
        {
            if (node == null) return string.Empty;
            List<string> parts = new List<string>();
            TreeNode? current = node;
            while (current != null)
            {
                parts.Insert(0, current.Text);
                current = current.Parent;
            }
            return string.Join(" → ", parts);
        }

        #endregion

































        #region !!!!!!! بناء الشجرة  !!!!!!!
        private void LoadAccountsTree_()
        {
            treeViewAccounts.Nodes.Clear();
            DataTable dt = DBServiecs.Acc_GetChart() ?? new DataTable();
            if (dt.Rows.Count == 0) return;

            // إنشاء قاموس لتخزين العقد
            Dictionary<string, TreeNode> nodeDict = new Dictionary<string, TreeNode>();

            // فلترة الحسابات التي لها أبناء فقط للشجرة
            var parentRows = dt.AsEnumerable()
                               .Where(r => r.Field<bool>("IsHasChildren"))
                               .ToList();

            foreach (DataRow row in parentRows)
            {
                string accName = row["AccName"] as string ?? string.Empty;
                if (string.IsNullOrWhiteSpace(accName)) continue;

                string treeCode = row["TreeAccCode"].ToString();
                string parentCode = row["ParentAccID"] != DBNull.Value ? row["ParentAccID"].ToString() : null;

                TreeNode node = new TreeNode(accName) { Tag = row };
                nodeDict[treeCode] = node;

                if (string.IsNullOrEmpty(parentCode))
                {
                    treeViewAccounts.Nodes.Add(node); // الجذر
                }
                else if (nodeDict.TryGetValue(parentCode, out TreeNode parentNode))
                {
                    parentNode.Nodes.Add(node);
                }
                else
                {
                    treeViewAccounts.Nodes.Add(node); // fallback
                }
            }

            SortTreeNodes(treeViewAccounts.Nodes);
            treeViewAccounts.CollapseAll();
        }
        private void SortTreeNodes_(TreeNodeCollection nodes)
        {
            List<TreeNode> nodeList = nodes.Cast<TreeNode>()
                                           .OrderBy(n =>
                                           {
                                               if (n.Tag is DataRow row)
                                                   return row["TreeAccCode"].ToString();
                                               return string.Empty;
                                           })
                                           .ToList();

            nodes.Clear();
            foreach (TreeNode node in nodeList)
            {
                nodes.Add(node);
                SortTreeNodes(node.Nodes); // ترتيب الأبناء
            }
        }

 
        // دالة مساعدة لإيجاد الأب
        private TreeNode? FindParentNode(TreeNodeCollection nodes, string fullPath, int targetLevel)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag is DataRow row)
                {
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


        // اين يتم استدعاء الدالة LoadChildrenInDGV
        private void LoadChildrenInDGV_(int parentAccID)
        {
            DataTable dt = DBServiecs.Acc_GetLeafChildren(parentAccID); // هذه الدالة ترجع كل الأبناء
            DGV.DataSource = dt.DefaultView;
            DGVStyl();

        }

        private void DGVStyl_()
        {
            // إعدادات عامة
            DGV.ReadOnly = true;
            DGV.AllowUserToAddRows = false;
            DGV.AllowUserToDeleteRows = false;
            DGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGV.MultiSelect = false;
            DGV.RowHeadersVisible = false;

            foreach (DataGridViewColumn col in DGV.Columns)
            {
                if (col.Name == "AccName" || col.Name == "Balance" || col.Name == "BalanceState")
                    col.Visible = true;
                else
                    col.Visible = false;
            }

            // تعيين خط عام
            Font generalFont = new Font("Times New Roman", 14, FontStyle.Bold);
            DGV.DefaultCellStyle.Font = generalFont;

            // تنسيق رؤوس الأعمدة
            DGV.ColumnHeadersDefaultCellStyle.Font = generalFont;
            DGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            DGV.ColumnHeadersHeight = 60;
            DGV.EnableHeadersVisualStyles = false;

            // تنسيق الأعمدة وتحديد أسماء المستخدم
            foreach (DataGridViewColumn col in DGV.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                switch (col.Name)
                {
                    case "Balance":
                        col.HeaderText = "الرصيد";
                        col.FillWeight = 15;
                        break;
                    case "BalanceState":
                        col.HeaderText = "--";
                        col.FillWeight = 15;
                        break;
                    case "AccName":
                        col.HeaderText = "اسم الحساب";
                        col.FillWeight = 20;
                        break;
                }
            }

            DGV.ClearSelection(); // إلغاء التحديد الافتراضي
        }

        #endregion

        #region !!!!!! بحث فى الشجرة  !!!!!!!!


        private void txtSearchTree_TextChanged_(object sender, EventArgs e)
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
        private void ResetNodeColorsAndCollapse_(TreeNodeCollection nodes)
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

        private void SearchAndHighlightNodes_(TreeNodeCollection nodes, string searchText)
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

        private void ExpandParentNodes_(TreeNode node)
        {
            TreeNode? parent = node.Parent;
            while (parent != null)
            {
                parent.Expand();
                parent = parent.Parent;
            }
        }

        #endregion

        #region !!!!!!  عرض الحسابات  !!!!!!!!
        // دالة لحساب المستوى من FullPath
        private int GetLevelFromFullPath_(string fullPath)
        {
            return fullPath.Split(new string[] { "→" }, StringSplitOptions.None).Length - 1;
        }
        // دالة لبناء المسار الكامل من شجرة العقد
        private string GetFullPathFromNode_(TreeNode node)
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
        private TreeNode? activeNode; // العقدة النشطة (المميزة بالأحمر)

        private void treeViewAccounts_AfterSelect_(object sender, TreeViewEventArgs e)
        {
            // إذا لم يتم تحديد أي عقدة، اخرج من الدالة
            if (treeViewAccounts.SelectedNode == null) return;

            TreeNode selectedNode = treeViewAccounts.SelectedNode;

            // التأكد أن الـ Tag يحتوي على DataRow
            if (selectedNode.Tag is not DataRow row) return;

            // ==========================
            // 1) تمييز العقدة بالأحمر + خط أكبر
            // ==========================
            if (activeNode != null)
            {
                activeNode.NodeFont = new Font("Times New Roman", 12, FontStyle.Bold);
                activeNode.ForeColor = Color.Black;
            }

            activeNode = selectedNode;

            activeNode.NodeFont = new Font(treeViewAccounts.Font.FontFamily,
                                           treeViewAccounts.Font.Size + 1,
                                           FontStyle.Bold);
            activeNode.ForeColor = Color.Red;
            treeViewAccounts.Refresh();

            // ==========================
            // 2) استخراج بيانات الحساب
            // ==========================
            int currentTreeAccCode = row.Field<int>("TreeAccCode");   // الترقيم الشجري الجديد
            int currentAccID = row.Field<int>("AccID");               // المفتاح الأصلي (للحركات)
            string accName = row["AccName"]?.ToString() ?? string.Empty;
            int? parentAccID = row["ParentAccID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ParentAccID"]);
            bool hasChildren = row.Field<bool?>("IsHasChildren") ?? false;
            bool hasDetails = row.Field<bool?>("IsHasDetails") ?? false;
            bool isEnerAcc = row.Field<bool?>("IsEnerAcc") ?? false;
            bool isHidden = row.Field<bool?>("IsHidden") ?? false;

            string balance = row["Balance"]?.ToString() ?? string.Empty;
            string balanceState = row["BalanceState"]?.ToString() ?? string.Empty;
            string dateOfJoin = row["DateOfJoin"]?.ToString() ?? string.Empty;

            // ==========================
            // 3) تحديث الـ DGV
            // ==========================
            if (hasChildren)
            {
                LoadChildrenInDGV(currentTreeAccCode);  // التغيير هنا ليبنى على TreeAccCode
            }
            else
            {
                DGV.DataSource = null;
            }

            selectedRow = row;

            // ==========================
            // 4) تحديث التسميات
            // ==========================
            lblSelectedTreeNod.Text = $"{currentTreeAccCode} - {accName}";  // بدل AccID بـ TreeAccCode
            lblPathNode.Text = GetFullPathFromNode(selectedNode);
            lblNameNod.Text = $"{accName}";

            // ==========================
            // 5) التحقق من إمكانية إضافة حساب فرعي
            // ==========================
            bool canAddChild = !(isEnerAcc && !hasChildren);
            txtAccName.Enabled = canAddChild;

            if (!canAddChild)
            {
                txtAccName.Clear();
                lblParentAccName.Text = "لا يمكن اضافة حسابات فرعية هنا فهذا حساب نهائى";
                lblParentAccName.ForeColor = Color.Red;
                chkIsHasChildren.Enabled = false;
            }
            else
            {
                lblParentAccName.Text = accName;
                lblParentAccName.ForeColor = Color.Gray;
                chkIsHasChildren.Enabled = true;
            }

            lblIsHasChildren.Text = hasChildren ? "" : "هذا الحساب مازال ليس له فروع";

            // ==========================
            // 6) التحقق من إذا كان الأب (أو أجداده) = 12 (أصول ثابتة)
            // ==========================
            object hasDetailsObj = row["IsHasDetails"];

            if (hasDetailsObj == DBNull.Value || Convert.ToInt32(hasDetailsObj) == 0)
            {
                lblAccDataDetails.Text = "";
                tlpBtnExec.Enabled = false;
            }
            else if (Convert.ToInt32(hasDetailsObj) == 1)
            {
                bool hasFixedAssetParent = false;
                TreeNode? currentNode = selectedNode;

                while (currentNode != null)
                {
                    if (currentNode.Tag is DataRow parentRow &&
                        Convert.ToInt32(parentRow["TreeAccCode"]) == 12)  // التحقق على TreeAccCode
                    {
                        hasFixedAssetParent = true;
                        break;
                    }
                    currentNode = currentNode.Parent;
                }

                lblAccDataDetails.Text = hasFixedAssetParent
                    ? "بيانات الأصل الثابت"
                    : "بيانات شخصية";
                tlpBtnExec.Enabled = true;
            }

            // ==========================
            // 7) تحميل التقارير الخاصة بالحساب المحدد
            // ==========================
            LoadReportsForSelectedAccount();
        }

        private void treeViewAccounts_AfterSelect__(object sender, TreeViewEventArgs e)
        {
            // إذا لم يتم تحديد أي عقدة، اخرج من الدالة
            if (treeViewAccounts.SelectedNode == null) return;

            TreeNode selectedNode = treeViewAccounts.SelectedNode;

            // التأكد أن الـ Tag يحتوي على DataRow
            if (selectedNode.Tag is not DataRow row) return;

            // ==========================
            // 1) تمييز العقدة بالأحمر + خط أكبر
            // ==========================

            // أعد العقدة القديمة لشكلها الطبيعي
            if (activeNode != null)
            {
                activeNode.NodeFont = new Font("Times New Roman", 12, FontStyle.Bold);
                activeNode.ForeColor = Color.Black;
            }

            // حدد العقدة الجديدة
            activeNode = selectedNode;

            // عدل مظهرها (أحمر + حجم أكبر + Bold)
            activeNode.NodeFont = new Font(treeViewAccounts.Font.FontFamily,
                                           treeViewAccounts.Font.Size + 1,
                                           FontStyle.Bold);
            activeNode.ForeColor = Color.Red;

            // إعادة رسم الشجرة لتطبيق التغييرات
            treeViewAccounts.Refresh();

            // ==========================
            // 2) استخراج بيانات الحساب
            // ==========================
            int currentAccID = row.Field<int>("AccID");
            string accName = row["AccName"]?.ToString() ?? string.Empty;
            int? parentAccID = row["ParentAccID"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["ParentAccID"]);
            bool hasChildren = row.Field<bool?>("IsHasChildren") ?? false;
            bool hasDetails = row.Field<bool?>("IsHasDetails") ?? false;
            bool isEnerAcc = row.Field<bool?>("IsEnerAcc") ?? false;
            bool isHidden = row.Field<bool?>("IsHidden") ?? false;

            string balance = row["Balance"]?.ToString() ?? string.Empty;
            string balanceState = row["BalanceState"]?.ToString() ?? string.Empty;
            string dateOfJoin = row["DateOfJoin"]?.ToString() ?? string.Empty;

            // ==========================
            // 3) تحديث الـ DGV
            // ==========================
            if (hasChildren)
            {
                LoadChildrenInDGV(currentAccID);
            }
            else
            {
                DGV.DataSource = null;
            }

            // حفظ الصف المحدد
            selectedRow = row;

            // ==========================
            // 4) تحديث التسميات
            // ==========================
            lblSelectedTreeNod.Text = $"{currentAccID} - {accName}";
            lblPathNode.Text = GetFullPathFromNode(selectedNode);
            lblNameNod.Text = $"{accName}";
            // ==========================
            // 5) التحقق من إمكانية إضافة حساب فرعي
            // ==========================
            bool canAddChild = !(isEnerAcc && !hasChildren);
            txtAccName.Enabled = canAddChild;

            if (!canAddChild)
            {
                txtAccName.Clear();
                lblParentAccName.Text = "لا يمكن اضافة حسابات فرعية هنا فهذا حساب نهائى";
                lblParentAccName.ForeColor = Color.Red;

                chkIsHasChildren.Enabled = false;


            }
            else
            {
                lblParentAccName.Text = accName;
                lblParentAccName.ForeColor = Color.Gray;
                chkIsHasChildren.Enabled = true;

            }

            lblIsHasChildren.Text = hasChildren ? "" : "هذا الحساب مازال ليس له فروع";

            // ==========================
            // 6) التحقق من إذا كان الأب (أو أجداده) = 12 (أصول ثابتة)
            // ==========================
            object hasDetailsObj = row["IsHasDetails"];

            if (hasDetailsObj == DBNull.Value || Convert.ToInt32(hasDetailsObj) == 0)
            {
                lblAccDataDetails.Text = "";
                tlpBtnExec.Enabled = false;
            }
            else if (Convert.ToInt32(hasDetailsObj) == 1)
            {
                bool hasFixedAssetParent = false;
                TreeNode? currentNode = selectedNode;

                while (currentNode != null)
                {
                    if (currentNode.Tag is DataRow parentRow &&
                        Convert.ToInt32(parentRow["AccID"]) == 12)
                    {
                        hasFixedAssetParent = true;
                        break;
                    }
                    currentNode = currentNode.Parent;
                }

                lblAccDataDetails.Text = hasFixedAssetParent
                    ? "بيانات الأصل الثابت"
                    : "بيانات شخصية";
                tlpBtnExec.Enabled = true;
            }

            // ==========================
            // 7) تحميل التقارير الخاصة بالحساب المحدد
            // ==========================
            LoadReportsForSelectedAccount();
        }


        private bool isSearchActive = false;// هذا المغيير للتعطيل المؤقت عند البحث

        //وظيفة غلق العقدة الاساسية العير مفعلة
        private void treeViewAccounts_BeforeExpand_(object sender, TreeViewCancelEventArgs e)
        {
            if (isSearchActive)
                return; // أثناء البحث لا نفعل أي غلق للعقد الأخرى

            if (e.Node!.Tag is DataRow row)
            {
                if (row.Table.Columns.Contains("AccID") && int.TryParse(row["AccID"]?.ToString(), out int accID))
                {
                    int? parentAccID = (row.Table.Columns.Contains("ParentAccID") && row["ParentAccID"] != DBNull.Value)
                        ? Convert.ToInt32(row["ParentAccID"])
                        : (int?)null;

                    // إذا الحساب جذري أساسي من 1 إلى 5
                    if (parentAccID == null && accID >= 1 && accID <= 5)
                    {
                        // أغلق كل الجذور الأخرى
                        foreach (TreeNode rootNode in treeViewAccounts.Nodes)
                        {
                            if (rootNode != e.Node)
                                rootNode.Collapse();
                        }
                    }
                }
            }
        }
        //هذا القديم
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
 

        
        #endregion

        #region !!!!!!!!  ازرار الشاشة !!!!!!!!!


        private void btnNew_Click(object sender, EventArgs e)
        {
            txtAccName.Clear();
            chkIsHasChildren.Checked = false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (treeViewAccounts.SelectedNode?.Tag is DataRow row)
            {
                int accID = Convert.ToInt32(row["AccID"]);
                string? accName = row["AccName"].ToString();

                DialogResult confirm = MessageBox.Show(
                    $"هل أنت متأكد أنك تريد حذف الحساب: {accName} (ID={accID})؟",
                    "تأكيد الحذف",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm == DialogResult.No) return;

                string resultMsg = DBServiecs.Acc_DeleteAccount(accID);
                // الرسالة الراجعة تم التنفيذ
                // فلا يقوم باجراءات تحميل الشجرة وتحديد الاب
                MessageBox.Show(resultMsg, "نتيجة الحذف");

                // لو تم الحذف فعلاً → نرجع للأب
                if (!resultMsg.StartsWith("❌")) // يعني مش فشل
                {
                    int? parentAccID = row["ParentAccID"] != DBNull.Value ? Convert.ToInt32(row["ParentAccID"]) : (int?)null;

                    LoadAccountsTree();

                    if (parentAccID.HasValue)
                        HighlightAndExpandNode(parentAccID.Value);
                }

            }
        }

        // زر اضافة حساب الى الشجرة
        private void txtAccName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!string.IsNullOrWhiteSpace(txtAccName.Text))
                {
                    // نفذ الإضافة
                    Addchiled();

                    // أضف الاسم الجديد في قائمة الجلسة
                    lstAccAdded.Items.Add(txtAccName.Text);

                    // امسح التكست عشان يبقى جاهز لاسم جديد
                    txtAccName.Clear();

                    // رجّع الفوكس للتكست
                    txtAccName.Focus();

                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
        }


        // دالة الاضافة الى اشجرة
        private void Addchiled()
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

            string accName = txtAccName.Text.Trim();
            int parentAccID = Convert.ToInt32(selectedRow["AccID"]);
            int createByUserID = CurrentSession.UserID;

            // 🟢 استدعاء الإجراء
            string result = DBServiecs.Acc_AddAccount(accName, parentAccID, createByUserID);

            if (result.StartsWith("تم")) // يعني نجحت العملية
            {
                MessageBox.Show("تم حفظ الحساب بنجاح ✅");

                // 🟢 إعادة تحميل الشجرة
                LoadAccountsTree();

                // 🟢 البحث عن الأب المباشر وتحديده
                TreeNode? parentNode = FindNodeByAccID(treeViewAccounts.Nodes, parentAccID);
                if (parentNode != null)
                {
                    treeViewAccounts.SelectedNode = parentNode;
                    parentNode.EnsureVisible();
                }

                // 🟢 تحديث الجريد لإظهار الحساب الجديد
                LoadChildrenInDGV(parentAccID);


            }
            else
            {
                MessageBox.Show("فشل في الحفظ ❌\n" + result);
            }
        }

        // زر تعديل حساب
        private void btnModify_Click(object sender, EventArgs e)
        {
            if (treeViewAccounts.SelectedNode?.Tag is DataRow row)
            {
                int accID = Convert.ToInt32(row["AccID"]);

                using (frm_AccountModify frm = new frm_AccountModify(accID))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        // ✅ تم التعديل بنجاح
                        // نعيد تحميل الشجرة
                        LoadAccountsTree();

                        // نرجع ونحدد على الحساب اللي اتعدل

                        // الوقوف على نفس الحساب المعدل
                        HighlightAndExpandNode(frm.UpdatedAccID);
                    }
                }
            }
            else
            {
                MessageBox.Show("يرجى اختيار حساب أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        #endregion

        #region !!!!!!!! إعداد قوائم التقارير  !!!!!!!!!!

        // قوائم التقارير
        private ToolStripMenuItem tsmiCategoryReports = new();
        private ToolStripMenuItem tsmiGroupedReports = new();

        // شريط القوائم داخل Panel
        private MenuStrip? menuStrip1;

        // تهيئة MenuStrip داخل Panel
        private void SetupMenuStrip()
        {
            // إزالة أي شريط موجود مسبقًا
            if (menuStrip1 != null && pnlMenuContainer.Controls.Contains(menuStrip1))
                pnlMenuContainer.Controls.Remove(menuStrip1);

            MenuStrip mainMenu = new MenuStrip
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightSteelBlue,
                Font = new Font("Times New Roman", 14, FontStyle.Regular),
                RightToLeft = RightToLeft.Yes
            };

            // لتفعيل محاذاة من اليمين لليسار عند ظهور القوائم المنسدلة:
            mainMenu.LayoutStyle = ToolStripLayoutStyle.Flow; // أو HorizontalStackWithOverflow


            // إنشاء عناصر القوائم
            tsmiCategoryReports = new ToolStripMenuItem("تقارير الصنف المحدد ▼");
            tsmiGroupedReports = new ToolStripMenuItem("تقارير مجمعة للأصناف المحددة ▼");

            mainMenu.Items.Add(tsmiCategoryReports);
            mainMenu.Items.Add(tsmiGroupedReports);

            pnlMenuContainer.Controls.Add(mainMenu);
            mainMenu.Location = new Point(10, 5);

            menuStrip1 = mainMenu; // حفظ المرجع
        }

        // تحميل القوائم بناءً على الحساب المحدد
        private void LoadReportsForSelectedAccount()
        {
            int? topAccID = GetCurrentEntityID();
            if (topAccID.HasValue)
                LoadReports(topAccID.Value);
        }

        // تحميل البيانات من قاعدة البيانات ووضعها في القوائم
        private void LoadReports(int topAcc)
        {
            try
            {
                DataTable dt = DBServiecs.Reports_GetByTopAcc(topAcc, false);
                if (dt == null || dt.Rows.Count == 0)
                {
                    tsmiCategoryReports.DropDownItems.Clear();
                    tsmiGroupedReports.DropDownItems.Clear();
                    tsmiCategoryReports.DropDownItems.Add(new ToolStripMenuItem("لا توجد تقارير متاحة") { Enabled = false });
                    tsmiGroupedReports.DropDownItems.Add(new ToolStripMenuItem("لا توجد تقارير متاحة") { Enabled = false });
                    return;
                }

                // تقارير فردية
                DataRow[] singleReports = dt.Select("IsGrouped = 0");
                LoadMenuItems(tsmiCategoryReports, singleReports);

                // تقارير مجمعة
                DataRow[] groupedReports = dt.Select("IsGrouped = 1");
                LoadMenuItems(tsmiGroupedReports, groupedReports);
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء تحميل التقارير: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // تعبئة القائمة بعناصر من DataRow[] مع ترتيب الاسم تصاعديًا
        private void LoadMenuItems(ToolStripMenuItem parentMenu, DataRow[] rows)
        {
            parentMenu.DropDownItems.Clear();

            if (rows.Length == 0)
            {
                ToolStripMenuItem emptyItem = new("لا توجد تقارير متاحة") { Enabled = false };
                parentMenu.DropDownItems.Add(emptyItem);
                return;
            }

            // ترتيب الصفوف حسب اسم التقرير
            var sortedRows = rows.OrderBy(r => r["ReportDisplayName"]?.ToString()).ToArray();

            foreach (DataRow row in sortedRows)
            {
                string displayName = row["ReportDisplayName"]?.ToString() ?? "تقرير بدون اسم";
                string codeName = row["ReportCodeName"]?.ToString() ?? "";
                int reportId = Convert.ToInt32(row["ReportID"]);

                // تجهيز القاموس من البداية
                Dictionary<string, object> tagData = new()
        {
            { "ReportCodeName", codeName },
            { "ReportDisplayName", displayName },
            { "ReportID", reportId },
            { "IsGrouped", Convert.ToBoolean(row["IsGrouped"]) }
        };

                ToolStripMenuItem menuItem = new(displayName)
                {
                    Tag = tagData
                };
                menuItem.Click += ReportMenuItem_Click;

                parentMenu.DropDownItems.Add(menuItem);
            }
        }

        // معرف المستخدم الحالي
        int ID_user;

        // حدث النقر على أي تقرير
        private void ReportMenuItem_Click(object? sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem clickedItem || clickedItem.Tag is not Dictionary<string, object> tagData)
            {
                MessageBox.Show("بيانات التقرير غير صحيحة.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Dictionary<string, object> reportParameters = new(tagData)
        {
            { "UserID", ID_user }
        };

                using frmSettingReports previewForm = new frmSettingReports(reportParameters);
                previewForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء فتح شاشة إعداد التقرير: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // جلب كود الحساب الحالي من الشجرة
        private int? GetCurrentEntityID()
        {
            if (treeViewAccounts.SelectedNode?.Tag is DataRow row)
            {
                if (row["AccID"] != DBNull.Value && int.TryParse(row["AccID"].ToString(), out int id))
                    return id;
            }

            MessageBox.Show("⚠️ يجب اختيار الحساب قبل عرض التقرير.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return null;
        }

        #endregion


        #region !!!!!! تفاصيل الحساب (الأبناء) !!!!!!! 

        // يحدد الصف داخل الـ DGV بناءً على رقم الحساب TreeAccCode.
        private void HighlightRowByTreeAccCode(int treeAccCode)
        {
            if (DGV == null || DGV.Rows.Count == 0) return;

            foreach (DataGridViewRow row in DGV.Rows)
            {
                if (row.Cells["TreeAccCode"].Value != null &&
                    Convert.ToInt32(row.Cells["TreeAccCode"].Value) == treeAccCode)
                {
                    row.Selected = true;

                    // 🔹 إيجاد أول عمود ظاهر
                    DataGridViewColumn? firstVisibleColumn = DGV.Columns
                        .Cast<DataGridViewColumn>()
                        .FirstOrDefault(c => c.Visible);

                    if (firstVisibleColumn != null)
                    {
                        row.Cells[firstVisibleColumn.Index].Selected = true;
                        DGV.CurrentCell = row.Cells[firstVisibleColumn.Index];
                    }

                    // 🔹 يضمن ظهور الصف في الشاشة
                    DGV.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
        }

        // عرض سجل تفصيلي معين
        private void ShowDetail(int index)
        {
            if (dtDetails.Rows.Count == 0 || index < 0 || index >= dtDetails.Rows.Count)
                return;

            DataRow row = dtDetails.Rows[index];

            lblContactName.Text = row["ContactName"]?.ToString() ?? "";

            string phone = row["Phone"]?.ToString() ?? "";
            string mobile = row["Mobile"]?.ToString() ?? "";

            lblPhonAndAnther.Text = (!string.IsNullOrEmpty(mobile) && !string.IsNullOrEmpty(phone))
                ? $"هواتف: {mobile} + {phone}"
                : (!string.IsNullOrEmpty(mobile) ? $"هاتف: {mobile}" : (!string.IsNullOrEmpty(phone) ? $"هاتف: {phone}" : ""));

            lblClientEmail.Text = row["Email"]?.ToString() ?? "";
            lblClientAddress.Text = row["Address"]?.ToString() ?? "";
            lblAccDetailNote.Text = row["Notes"]?.ToString() ?? "";

            DateTime? createdDate = row["CreatedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["CreatedDate"]);
            DateTime? modifiedDate = row["ModifiedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ModifiedDate"]);

            lblCreateAndModifyDate.Text = $"{(createdDate.HasValue ? $"تاريخ الإنشاء: {createdDate:yyyy/MM/dd HH:mm}" : "تاريخ الإنشاء: غير متوفر")}\n" +
                                          $"{(modifiedDate.HasValue ? $"تاريخ التعديل: {modifiedDate:yyyy/MM/dd HH:mm}" : "تاريخ التعديل: غير متوفر")}";
        }

        // تنظيف الحقول عند عدم وجود بيانات
        private void ClearDetailFields()
        {
            lblContactName.Text = "";
            lblPhonAndAnther.Text = "";
            lblClientEmail.Text = "";
            lblClientAddress.Text = "";
            lblAccDetailNote.Text = "";
            lblCreateAndModifyDate.Text = "";
        }

        // زر التنقل بين التفاصيل
        private void btnNextDetail_Click(object sender, EventArgs e)
        {
            if (dtDetails == null || dtDetails.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد تفاصيل لعرضها", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            currentDetailIndex++;
            if (currentDetailIndex >= dtDetails.Rows.Count) currentDetailIndex = 0;

            ShowDetail(currentDetailIndex);
        }

        // زر الإضافة
        private void btnAddDetail_Click(object sender, EventArgs e)
        {
            if (DGV.CurrentRow == null)
            {
                MessageBox.Show("يجب تحديد حساب من الجدول قبل إضافة تفاصيل.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRowView? rowView = DGV.CurrentRow.DataBoundItem as DataRowView;
            if (rowView == null) return;

            DataRow row = rowView.Row;
            int treeAccCode = Convert.ToInt32(row["TreeAccCode"]);

            using (frm_AccountDetailAdd frm = new frm_AccountDetailAdd(treeAccCode))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    Acc_GetDetails(treeAccCode);
                    HighlightAndExpandNode(treeAccCode);
                    HighlightRowByTreeAccCode(treeAccCode);
                }
            }
        }

        DataTable dtDetails = new DataTable();
        int currentDetailIndex = -1;

        // تحميل تفاصيل الحساب لرقم معين
        private void Acc_GetDetails(int treeAccCode)
        {
            dtDetails = DBServiecs.Acc_GetDetails(treeAccCode);
            currentDetailIndex = dtDetails.Rows.Count > 0 ? 0 : -1;

            if (currentDetailIndex >= 0)
                ShowDetail(currentDetailIndex);
            else
                ClearDetailFields();
        }

        // زر التعديل
        private void btnModifyDetail_Click(object sender, EventArgs e)
        {
            if (dtDetails == null || dtDetails.Rows.Count == 0 || currentDetailIndex < 0)
            {
                MessageBox.Show("⚠️ لا يوجد تفاصيل لتعديلها.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRow row = dtDetails.Rows[currentDetailIndex];
            int treeAccCode = Convert.ToInt32(row["TreeAccCode"]);
            int detailID = Convert.ToInt32(row["DetailID"]);

            using (frm_AccountDetailAdd frm = new frm_AccountDetailAdd(treeAccCode, detailID))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    Acc_GetDetails(treeAccCode);
                    HighlightAndExpandNode(treeAccCode);
                    HighlightRowByTreeAccCode(treeAccCode);

                    for (int i = 0; i < dtDetails.Rows.Count; i++)
                    {
                        if (Convert.ToInt32(dtDetails.Rows[i]["DetailID"]) == detailID)
                        {
                            currentDetailIndex = i;
                            ShowDetail(currentDetailIndex);
                            break;
                        }
                    }
                }
            }
        }

        // زر الحذف
        private void btnDeleteDetail_Click(object sender, EventArgs e)
        {
            if (dtDetails == null || dtDetails.Rows.Count == 0 || currentDetailIndex < 0)
            {
                MessageBox.Show("⚠️ لا يوجد تفاصيل للحذف.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRow row = dtDetails.Rows[currentDetailIndex];
            int treeAccCode = Convert.ToInt32(row["TreeAccCode"]);
            int detailID = Convert.ToInt32(row["DetailID"]);

            var result = MessageBox.Show("هل تريد حذف هذا التفصيل؟", "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                string resultMsg = DBServiecs.Acc_DeleteDetails(detailID);

                if (!resultMsg.StartsWith("❌"))
                {
                    MessageBox.Show(resultMsg, "تم الحذف", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Acc_GetDetails(treeAccCode);

                    if (dtDetails.Rows.Count > 0)
                    {
                        if (currentDetailIndex >= dtDetails.Rows.Count)
                            currentDetailIndex = dtDetails.Rows.Count - 1;

                        ShowDetail(currentDetailIndex);
                    }
                    else
                    {
                        ClearDetailFields();
                    }

                    HighlightAndExpandNode(treeAccCode);
                    HighlightRowByTreeAccCode(treeAccCode);
                }
                else
                {
                    MessageBox.Show(resultMsg, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // إضافة حساب فرعي
        private void AddChildren()
        {
            if (DGV.CurrentRow == null)
            {
                MessageBox.Show("يجب اختيار حساب من الجدول لإضافة حساب فرعي له.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRowView? rowView = DGV.CurrentRow.DataBoundItem as DataRowView;
            if (rowView == null) return;

            DataRow row = rowView.Row;
            int parentTreeAccCode = Convert.ToInt32(row["TreeAccCode"]);

            string userInput;
            DialogResult inputResult = CustomMessageBox.ShowStringInputBox(out userInput, "من فضلك أدخل اسم الحساب:", "إضافة حساب فرعي");

            if (inputResult != DialogResult.OK || string.IsNullOrWhiteSpace(userInput))
            {
                MessageBox.Show("تم إلغاء الإضافة أو لم يتم إدخال اسم صالح.", "إلغاء", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string AccName = userInput.Trim();
            int CreateByUserID = CurrentSession.UserID;

            string result = DBServiecs.Acc_AddAccount(AccName, parentTreeAccCode, CreateByUserID);

            if (result.StartsWith("تم"))
            {
                MessageBox.Show("تم حفظ الحساب بنجاح ✅", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadAccountsTree();

                TreeNode? parentNode = FindNodeByTreeAccCode(treeViewAccounts.Nodes, parentTreeAccCode);
                // الان خطأ واحد فقط The name 'FindNodeByTreeAccCode' does not exist in the current context
                if (parentNode != null)
                {
                    parentNode.Expand();
                    TreeNode? newNode = parentNode.Nodes.Cast<TreeNode>().FirstOrDefault(n => n.Text == AccName);
                    if (newNode != null)
                    {
                        treeViewAccounts.SelectedNode = newNode;
                        newNode.EnsureVisible();
                    }
                }

                HighlightAndExpandNode(parentTreeAccCode);
                HighlightRowByTreeAccCode(parentTreeAccCode);
            }
            else
            {
                MessageBox.Show("فشل في الحفظ ❌\n" + result, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStripAddChildren_Click(object sender, EventArgs e)
        {
            AddChildren();
        }

        // دالة للبحث عن عقدة داخل TreeView باستخدام TreeAccCode
        private TreeNode? FindNodeByTreeAccCode(TreeNodeCollection nodes, int treeAccCode)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag is DataRow row)
                {
                    int nodeCode = Convert.ToInt32(row["TreeAccCode"]);
                    if (nodeCode == treeAccCode)
                        return node;
                }

                // البحث في الأبناء بشكل متكرر
                TreeNode? childResult = FindNodeByTreeAccCode(node.Nodes, treeAccCode);
                if (childResult != null)
                    return childResult;
            }

            return null; // لم يتم العثور على العقدة
        }

        #endregion
























        //#region !!!!!! تفاصيل الحساب الأبناء القديم !!!!!!! 

        //// يحدد الصف داخل الـ DGV بناءً على رقم الحساب AccID.
        //private void HighlightRowByAccID(int accID)
        //{
        //    if (DGV == null || DGV.Rows.Count == 0)
        //        return;

        //    foreach (DataGridViewRow row in DGV.Rows)
        //    {
        //        if (row.Cells["AccID"].Value != null &&
        //            Convert.ToInt32(row.Cells["AccID"].Value) == accID)
        //        {
        //            row.Selected = true;

        //            // 🔹 إيجاد أول عمود ظاهر
        //            DataGridViewColumn? firstVisibleColumn = DGV.Columns
        //                .Cast<DataGridViewColumn>()
        //                .FirstOrDefault(c => c.Visible);

        //            if (firstVisibleColumn != null)
        //            {
        //                row.Cells[firstVisibleColumn.Index].Selected = true;
        //                DGV.CurrentCell = row.Cells[firstVisibleColumn.Index];
        //            }

        //            // 🔹 يضمن ظهور الصف في الشاشة
        //            DGV.FirstDisplayedScrollingRowIndex = row.Index;
        //            break;
        //        }
        //    }
        //}




        //// عرض سجل تفصيلي معين
        //private void ShowDetail(int index)
        //{
        //    if (dtDetails.Rows.Count == 0 || index < 0 || index >= dtDetails.Rows.Count)
        //        return;

        //    DataRow row = dtDetails.Rows[index];

        //    int detailID = Convert.ToInt32(row["DetailID"]); // لو محتاجه لعمليات أخرى
        //    lblContactName.Text = row["ContactName"].ToString();

        //    string phone = row["Phone"]?.ToString() ?? "";
        //    string mobile = row["Mobile"]?.ToString() ?? "";

        //    if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(mobile))
        //        lblPhonAndAnther.Text = $"هواتف: {mobile} + {phone}";
        //    else if (!string.IsNullOrEmpty(mobile))
        //        lblPhonAndAnther.Text = $"هاتف: {mobile}";
        //    else if (!string.IsNullOrEmpty(phone))
        //        lblPhonAndAnther.Text = $"هاتف: {phone}";
        //    else
        //        lblPhonAndAnther.Text = "";

        //    lblClientEmail.Text = row["Email"].ToString();
        //    lblClientAddress.Text = row["Address"].ToString();
        //    lblAccDetailNote.Text = row["Notes"].ToString();
        //    DateTime? createdDate = row["CreatedDate"] == DBNull.Value
        //        ? (DateTime?)null
        //        : Convert.ToDateTime(row["CreatedDate"]);

        //    DateTime? modifiedDate = row["ModifiedDate"] == DBNull.Value
        //        ? (DateTime?)null
        //        : Convert.ToDateTime(row["ModifiedDate"]);

        //    string createdText = createdDate.HasValue
        //        ? $"تاريخ الإنشاء: {createdDate:yyyy/MM/dd HH:mm}"
        //        : "تاريخ الإنشاء: غير متوفر";

        //    string modifiedText = modifiedDate.HasValue
        //        ? $"تاريخ التعديل: {modifiedDate:yyyy/MM/dd HH:mm}"
        //        : "تاريخ التعديل: غير متوفر";

        //    lblCreateAndModifyDate.Text = $"{createdText}\n{modifiedText}";

        //}

        //// تنظيف الحقول عند عدم وجود بيانات
        //private void ClearDetailFields()
        //{
        //    lblContactName.Text = "";
        //    lblPhonAndAnther.Text = "";
        //    lblClientEmail.Text = "";
        //    lblClientAddress.Text = "";
        //    lblAccDetailNote.Text = "";
        //    lblCreateAndModifyDate.Text = "";
        //}

        //// زر التنقل بين التفاصيل
        //private void btnNextDetail_Click(object sender, EventArgs e)
        //{
        //    if (dtDetails == null || dtDetails.Rows.Count == 0)
        //    {
        //        MessageBox.Show("لا توجد تفاصيل لعرضها", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }

        //    // التحرك للسجل التالي
        //    currentDetailIndex++;

        //    // في حالة الوصول لآخر سجل → ارجع لأول واحد
        //    if (currentDetailIndex >= dtDetails.Rows.Count)
        //        currentDetailIndex = 0;

        //    // عرض التفاصيل على الليبلز
        //    ShowDetail(currentDetailIndex);
        //}

        //// زر الإضافة
        //private void btnAddDetail_Click(object sender, EventArgs e)
        //{
        //    // التحقق أولاً من اختيار صف
        //    if (DGV.CurrentRow == null)
        //    {
        //        MessageBox.Show("يجب تحديد حساب من الجدول قبل إضافة تفاصيل.",
        //                        "تنبيه",
        //                        MessageBoxButtons.OK,
        //                        MessageBoxIcon.Warning);
        //        return;
        //    }

        //    // الحصول على الصف المحدد
        //    DataRowView? rowView = DGV.CurrentRow.DataBoundItem as DataRowView;
        //    if (rowView == null)
        //    {
        //        MessageBox.Show("لم يتم العثور على بيانات صالحة للصف المحدد.",
        //                        "خطأ",
        //                        MessageBoxButtons.OK,
        //                        MessageBoxIcon.Error);
        //        return;
        //    }

        //    DataRow row = rowView.Row;
        //    int accID = Convert.ToInt32(row["AccID"]); // جلب معرف الحساب

        //    // فتح شاشة إضافة التفاصيل وتمرير accID
        //    using (frm_AccountDetailAdd frm = new frm_AccountDetailAdd(accID))
        //    {
        //        if (frm.ShowDialog() == DialogResult.OK)
        //        {
        //            // بعد الإضافة → إعادة تحميل البيانات
        //            Acc_GetDetails(accID);

        //            // الوقوف على نفس الحساب في الشجرة والجريد
        //            HighlightAndExpandNode(accID);
        //            HighlightRowByAccID(accID);
        //        }
        //    }
        //}

        //DataTable dtDetails = new DataTable();
        //int currentDetailIndex = -1;

        //// تحميل تفاصيل الحساب لرقم معين
        //private void Acc_GetDetails(int accID)
        //{
        //    dtDetails = DBServiecs.Acc_GetDetails(accID);
        //    currentDetailIndex = dtDetails.Rows.Count > 0 ? 0 : -1;

        //    if (currentDetailIndex >= 0)
        //        ShowDetail(currentDetailIndex);
        //    else
        //        ClearDetailFields();
        //}

        //// زر التعديل
        //private void btnModifyDetail_Click(object sender, EventArgs e)
        //{
        //    if (dtDetails == null || dtDetails.Rows.Count == 0 || currentDetailIndex < 0)
        //    {
        //        MessageBox.Show("⚠️ لا يوجد تفاصيل لتعديلها.", "تنبيه",
        //                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    DataRow row = dtDetails.Rows[currentDetailIndex]; // من جدول التفاصيل الحالي

        //    int accID = Convert.ToInt32(row["AccID"]);
        //    int detailID = Convert.ToInt32(row["DetailID"]);

        //    using (frm_AccountDetailAdd frm = new frm_AccountDetailAdd(accID, detailID))
        //    {
        //        if (frm.ShowDialog() == DialogResult.OK)
        //        {
        //            // بعد التعديل → إعادة تحميل التفاصيل
        //            Acc_GetDetails(accID);

        //            // الوقوف على نفس الحساب في الشجرة والجريد
        //            HighlightAndExpandNode(accID);
        //            HighlightRowByAccID(accID);

        //            // 🔹 البحث عن detailID المعدل والرجوع له
        //            for (int i = 0; i < dtDetails.Rows.Count; i++)
        //            {
        //                if (Convert.ToInt32(dtDetails.Rows[i]["DetailID"]) == detailID)
        //                {
        //                    currentDetailIndex = i;
        //                    ShowDetail(currentDetailIndex); // عرض التفاصيل على الليبلز
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //}

        //// زر الحذف
        //private void btnDeleteDetail_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (dtDetails == null || dtDetails.Rows.Count == 0 || currentDetailIndex < 0)
        //        {
        //            MessageBox.Show("⚠️ لا يوجد تفاصيل للحذف.", "تنبيه",
        //                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }

        //        // الحصول على السطر الحالي من التفاصيل
        //        DataRow row = dtDetails.Rows[currentDetailIndex];
        //        int detailID = Convert.ToInt32(row["DetailID"]);
        //        int accID = Convert.ToInt32(row["AccID"]);

        //        // تأكيد الحذف
        //        var result = MessageBox.Show("هل تريد حذف هذا التفصيل؟",
        //                                     "تأكيد الحذف",
        //                                     MessageBoxButtons.YesNo,
        //                                     MessageBoxIcon.Warning);

        //        if (result == DialogResult.Yes)
        //        {
        //            string resultMsg = DBServiecs.Acc_DeleteDetails(detailID);

        //            if (!resultMsg.StartsWith("❌"))
        //            {
        //                MessageBox.Show(resultMsg, "تم الحذف",
        //                                MessageBoxButtons.OK, MessageBoxIcon.Information);

        //                // إعادة تحميل تفاصيل الحساب
        //                Acc_GetDetails(accID);

        //                // إعادة ضبط المؤشر بعد الحذف:
        //                if (dtDetails.Rows.Count > 0)
        //                {
        //                    // لو فيه سجل بعد الحالي → نروح له
        //                    if (currentDetailIndex >= dtDetails.Rows.Count)
        //                        currentDetailIndex = dtDetails.Rows.Count - 1;

        //                    ShowDetail(currentDetailIndex);
        //                }
        //                else
        //                {
        //                    ClearDetailFields();
        //                }

        //                // الوقوف على نفس الحساب في الشجرة والجريد
        //                HighlightAndExpandNode(accID);
        //                HighlightRowByAccID(accID);
        //            }
        //            else
        //            {
        //                MessageBox.Show(resultMsg, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"❌ حدث خطأ أثناء الحذف: {ex.Message}", "خطأ",
        //                        MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        //private void AddChildren()
        //{
        //    // 1️⃣ التأكد أن فيه صف محدد في الجريد
        //    if (DGV.CurrentRow == null)
        //    {
        //        MessageBox.Show("يجب اختيار حساب من الجدول لإضافة حساب فرعي له.",
        //                        "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    DataRowView? rowView = DGV.CurrentRow.DataBoundItem as DataRowView;
        //    if (rowView == null) return;

        //    DataRow row = rowView.Row;
        //    int ParentAccID = Convert.ToInt32(row["AccID"]); // الأب من الجريد

        //    // 2️⃣ إدخال اسم الحساب الجديد
        //    string userInput;
        //    DialogResult inputResult = CustomMessageBox.ShowStringInputBox(
        //        out userInput,
        //        "من فضلك أدخل اسم الحساب:",
        //        "إضافة حساب فرعي"
        //    );

        //    if (inputResult != DialogResult.OK || string.IsNullOrWhiteSpace(userInput))
        //    {
        //        MessageBox.Show("تم إلغاء الإضافة أو لم يتم إدخال اسم صالح.",
        //                        "إلغاء", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }

        //    string AccName = userInput.Trim();
        //    int CreateByUserID = CurrentSession.UserID;

        //    // 3️⃣ استدعاء الإجراء المخزن لإضافة الحساب
        //    string result = DBServiecs.Acc_AddAccount(AccName, ParentAccID, CreateByUserID);

        //    // 4️⃣ التحقق من النتيجة
        //    if (result.StartsWith("تم")) // العملية نجحت
        //    {
        //        MessageBox.Show("تم حفظ الحساب بنجاح ✅",
        //                        "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

        //        // 🟢 إعادة تحميل الشجرة
        //        LoadAccountsTree();

        //        // 🟢 البحث عن العقدة الأب (ParentAccID)
        //        TreeNode? parentNode = FindNodeByAccID(treeViewAccounts.Nodes, ParentAccID);

        //        if (parentNode != null)
        //        {
        //            // فتح الأب
        //            parentNode.Expand();

        //            // البحث عن العقدة المضافة باسمها الجديد تحت الأب
        //            TreeNode? newNode = parentNode.Nodes
        //                                          .Cast<TreeNode>()
        //                                          .FirstOrDefault(n => n.Text == AccName);

        //            if (newNode != null)
        //            {
        //                treeViewAccounts.SelectedNode = newNode;
        //                newNode.EnsureVisible(); // يخليها تظهر
        //            }
        //        }

        //        // 🟢 تحديث عرض الجريد
        //        HighlightAndExpandNode(ParentAccID);
        //        HighlightRowByAccID(ParentAccID);
        //    }
        //    else
        //    {
        //        MessageBox.Show("فشل في الحفظ ❌\n" + result,
        //                        "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        //private void btnStripAddChildren_Click(object sender, EventArgs e)
        //{
        //    AddChildren();
        //}

        //#endregion





        private void DGV_SelectionChanged(object sender, EventArgs e)
        {
            if (DGV.CurrentRow == null) return;

            // جلب الصف المحدد
            DataRowView? rowView = DGV.CurrentRow.DataBoundItem as DataRowView;
            if (rowView == null) return;

            DataRow row = rowView.Row;

            // استخراج accID من العمود
            if (row["AccID"] != DBNull.Value)
            {
                int accID = Convert.ToInt32(row["AccID"]);

                // استدعاء تحميل التفاصيل
                Acc_GetDetails(accID);
            }
        }
    }
}
