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
    public partial class frmAccounts : Form
    {
        private DataTable _allAccountsData; // لتخزين جميع البيانات

        public frmAccounts()
        {
            InitializeComponent();
        }

        private void frmAccounts_Load(object sender, EventArgs e)
        {
            LoadAccountsTree();
        }

        private void LoadAccountsTree()
        {
            treeViewAccounts.Nodes.Clear();
            _allAccountsData = DBServiecs.Acc_GetChart() ?? new DataTable();
            if (_allAccountsData.Rows.Count == 0) return;

            // Dictionary لتخزين العقد أثناء البناء
            Dictionary<string, TreeNode> nodeDict = new Dictionary<string, TreeNode>();

            // عرض الحسابات التي لها فروع فقط
            var parentRows = _allAccountsData.AsEnumerable()
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
                    treeViewAccounts.Nodes.Add(node); // عقدة الجذر
                else if (nodeDict.TryGetValue(parentCode, out TreeNode parentNode))
                    parentNode.Nodes.Add(node); // إضافة العقدة للوالد
                else
                    treeViewAccounts.Nodes.Add(node); // fallback في حالة عدم وجود والد
            }

            SortTreeNodes(treeViewAccounts.Nodes); // ترتيب العقد تصاعديًا حسب AccID
            treeViewAccounts.CollapseAll();        // طي جميع الفروع
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

        private void treeViewAccounts_AfterSelect(object sender, TreeViewEventArgs e)
        {
            LoadChildAccountsToGrid(e.Node);
        }

        private void LoadChildAccountsToGrid(TreeNode selectedNode)
        {
            if (selectedNode?.Tag == null || _allAccountsData == null) return;

            DataRow selectedRow = selectedNode.Tag as DataRow;
            string parentTreeCode = selectedRow["TreeAccCode"].ToString();

            // فلترة الأبناء الذين ليس لهم أبناء (ورقيات)
            var childAccounts = _allAccountsData.AsEnumerable()
                .Where(r =>
                    r.Field<string>("ParentAccID")?.ToString() == parentTreeCode &&
                    !r.Field<bool>("IsHasChildren"))
                .CopyToDataTable();

            DGV .DataSource = childAccounts;

            // إخفاء الأعمدة غير الضرورية إذا لزم الأمر
            if (DGV.Columns.Contains("TreeView"))
                DGV.Columns["TreeView"].Visible = false;
        }

        /*
         الدالة DataTable dt = DBServiecs.Acc_GetChart() ?? new DataTable();
         احضرت كل الشجرة بما فيهم أباء وأبناء ولكن تم عرض فقط الآباء في الشجرة وهذا مطلوب
         والآن في الـ DataGridView أظهر الأبناء الذين ليس لهم أبناء تحت أبائهم المحدد في الشجرة
         */
    }
}