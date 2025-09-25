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

            return string.Join("→", parts);
        }

        private void treeViewAccounts_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewAccounts.SelectedNode != null)
            {
                TreeNode node = treeViewAccounts.SelectedNode;

                if (node.Tag is DataRow row)
                {
                    string? accID = row["AccID"].ToString();
                    string? accName = row["AccName"].ToString();

                    lblSelectedTreeNod.Text = accID + " - " + accName;

                    // بناء المسار من شجرة العقد
                    string fullPath = GetFullPathFromNode(node);

                    // عرض المستوى المستنتج من المسار المبني
                    lblPathNode.Text = GetLevelFromFullPath(fullPath).ToString();
                }
            }
        }


    }
}
