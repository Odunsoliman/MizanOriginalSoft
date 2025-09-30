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

                string? treeCode = row["TreeAccCode"].ToString();
                string? parentCode = row["ParentAccID"] != DBNull.Value ? row["ParentAccID"].ToString() : null;

                TreeNode? node = new TreeNode(accName) { Tag = row };
                nodeDict[treeCode] = node;

                if (string.IsNullOrEmpty(parentCode))
                    treeViewAccounts.Nodes.Add(node); // عقدة الجذر
                else if (nodeDict.TryGetValue(parentCode, out TreeNode? parentNode))
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
                                                   return row.Field<int>("TreeAccCode"); // كـ int
                                               return 0;
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
            DGVStyle();
        }

        private void LoadChildAccountsToGrid(TreeNode selectedNode)
        {
            if (selectedNode?.Tag == null || _allAccountsData == null) return;

            DataRow? selectedRow = selectedNode.Tag as DataRow;
            int parentTreeCode = selectedRow.Field<int>("TreeAccCode");

            var filteredRows = _allAccountsData.AsEnumerable()
                .Where(r =>
                    r.Field<int?>("ParentAccID") == parentTreeCode &&
                    !r.Field<bool>("IsHasChildren"));

            if (filteredRows.Any())
            {
                DataTable childAccounts = filteredRows.CopyToDataTable();

                // إضافة عمود جديد لاسم الأب فقط
                if (!childAccounts.Columns.Contains("ParentName"))
                {
                    childAccounts.Columns.Add("ParentName", typeof(string));
                }

                // تعبئة اسم الأب من FullPath (آخر جزء بعد السهم)
                foreach (DataRow row in childAccounts.Rows)
                {
                    string fullPath = row.Field<string>("FullPath") ?? "";
                    string[] pathParts = fullPath.Split(new string[] { " → " }, StringSplitOptions.None);
                    string parentName = pathParts.Length > 1 ? pathParts[pathParts.Length - 2] : "---";
                    row["ParentName"] = parentName;
                }

                DGV.DataSource = childAccounts;
            }
            else
            {
                DGV.DataSource = null;
            }
        }

        private void DGVStyle()
        {
            if (DGV.DataSource == null) return;

            // إخفاء جميع الأعمدة أولاً
            foreach (DataGridViewColumn column in DGV.Columns)
            {
                column.Visible = false;
            }

            // إظهار الأعمدة المطلوبة بالترتيب: اسم الحساب - اسم الأب - الرصيد - الحالة
            string[] columnOrder = { "AccName", "ParentName", "Balance", "BalanceState" };

            foreach (string columnName in columnOrder)
            {
                if (DGV.Columns.Contains(columnName))
                {
                    DGV.Columns[columnName].Visible = true;
                }
            }

            // ترتيب الأعمدة
            DGV.Columns["AccName"].DisplayIndex = 0;
            DGV.Columns["ParentName"].DisplayIndex = 1;
            DGV.Columns["Balance"].DisplayIndex = 2;
            DGV.Columns["BalanceState"].DisplayIndex = 3;

            // عناوين الأعمدة
            DGV.Columns["AccName"].HeaderText = "اسم الحساب";
            DGV.Columns["ParentName"].HeaderText = "اسم الأب";
            DGV.Columns["Balance"].HeaderText = "الرصيد";
            DGV.Columns["BalanceState"].HeaderText = "الحالة";

            // نسب العرض 2:1:1:1
            int totalWidth = DGV.ClientRectangle.Width;
            DGV.Columns["AccName"].Width = (int)(totalWidth * 0.4);   // 40%
            DGV.Columns["ParentName"].Width = (int)(totalWidth * 0.2); // 20%
            DGV.Columns["Balance"].Width = (int)(totalWidth * 0.2);   // 20%
            DGV.Columns["BalanceState"].Width = (int)(totalWidth * 0.2); // 20%

            // باقي الإعدادات تبقى كما هي...
            DGV.Font = new Font("Times New Roman", 12, FontStyle.Bold);
            DGV.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Bold);
            DGV.RowHeadersVisible = false;
            DGV.AllowUserToAddRows = false;
            DGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGV.ReadOnly = true;
            DGV.DefaultCellStyle.Font = new Font("Times New Roman", 11, FontStyle.Regular);
            DGV.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            DGV.RowsDefaultCellStyle.BackColor = Color.White;

            // تنسيق الرصيد
            if (DGV.Columns.Contains("Balance"))
            {
                DGV.Columns["Balance"].DefaultCellStyle.Format = "N2";
                DGV.Columns["Balance"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
        }
    }
}