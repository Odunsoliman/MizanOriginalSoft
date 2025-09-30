using MizanOriginalSoft.MainClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.Accounts
{
    public partial class frmAccounts : Form
    {
        private DataTable _allAccountsData = new DataTable();

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

            Dictionary<string, TreeNode> nodeDict = new Dictionary<string, TreeNode>();

            var parentRows = _allAccountsData.AsEnumerable()
                               .Where(r => r.Field<bool>("IsHasChildren"))
                               .ToList();

            foreach (DataRow row in parentRows)
            {
                string accName = row["AccName"] as string ?? string.Empty;
                if (string.IsNullOrWhiteSpace(accName)) continue;

                string treeCode = row["TreeAccCode"].ToString() ?? string.Empty;
                string? parentCode = row["ParentAccID"] != DBNull.Value ? row["ParentAccID"].ToString() : null;

                TreeNode node = new TreeNode(accName) { Tag = row };
                nodeDict[treeCode] = node;

                if (string.IsNullOrEmpty(parentCode))
                    treeViewAccounts.Nodes.Add(node);
                else if (nodeDict.TryGetValue(parentCode, out TreeNode? parentNode))
                    parentNode.Nodes.Add(node);
                else
                    treeViewAccounts.Nodes.Add(node);
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
                                                   return row.Field<int>("TreeAccCode");
                                               return 0;
                                           })
                                           .ToList();

            nodes.Clear();
            foreach (TreeNode node in nodeList)
            {
                nodes.Add(node);
                SortTreeNodes(node.Nodes);
            }
        }
        private void treeViewAccounts_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // إعادة تعيين جميع العقد
            ResetAllNodesStyle();

            // تطبيق التنسيق على العقدة المحددة
            e.Node.BackColor = SystemColors.Highlight;
            e.Node.ForeColor = Color.Red;
            e.Node.NodeFont = new Font(treeViewAccounts.Font.FontFamily, treeViewAccounts.Font.Size + 1, FontStyle.Bold);

            LoadChildAccountsToGrid(e.Node);
            DGVStyle();
        }

        private void ResetAllNodesStyle()
        {
            foreach (TreeNode node in treeViewAccounts.Nodes)
            {
                ResetNodeStyle(node);
            }
        }

        private void ResetNodeStyle(TreeNode node)
        {
            node.BackColor = treeViewAccounts.BackColor;
            node.ForeColor = treeViewAccounts.ForeColor;
            node.NodeFont = treeViewAccounts.Font;

            foreach (TreeNode childNode in node.Nodes)
            {
                ResetNodeStyle(childNode);
            }
        }
        private void LoadChildAccountsToGrid(TreeNode? selectedNode)
        {
            if (selectedNode?.Tag == null || _allAccountsData == null) return;

            DataRow selectedRow = (DataRow)selectedNode.Tag;
            int parentTreeCode = selectedRow.Field<int>("TreeAccCode");

            var filteredRows = _allAccountsData.AsEnumerable()
                .Where(r =>
                {
                    int? parentAccID = r.Field<int?>("ParentAccID");
                    bool isHasChildren = r.Field<bool>("IsHasChildren");
                    return parentAccID == parentTreeCode && !isHasChildren;
                });

            if (filteredRows.Any())
            {
                DataTable childAccounts = filteredRows.CopyToDataTable();

                if (!childAccounts.Columns.Contains("ParentName"))
                    childAccounts.Columns.Add("ParentName", typeof(string));
                if (!childAccounts.Columns.Contains("BalanceWithState"))
                    childAccounts.Columns.Add("BalanceWithState", typeof(string));

                foreach (DataRow row in childAccounts.Rows)
                {
                    string fullPath = GetSafeStringValue(row, "FullPath");
                    string[] pathParts = fullPath.Split(new string[] { " → " }, StringSplitOptions.None);
                    string parentName = pathParts.Length > 1 ? pathParts[pathParts.Length - 2] : "---";
                    row["ParentName"] = parentName;

                    decimal balance = GetSafeDecimalValue(row, "Balance");
                    string balanceState = GetSafeStringValue(row, "BalanceState");
                    string balanceWithState = FormatBalanceWithState(balance, balanceState);
                    row["BalanceWithState"] = balanceWithState;
                }

                DGV.DataSource = childAccounts;
            }
            else
            {
                DGV.DataSource = null;
            }
        }

        private string GetSafeStringValue(DataRow row, string columnName)
        {
            if (row == null || row.IsNull(columnName)) return string.Empty;
            try { return row.Field<string>(columnName) ?? string.Empty; }
            catch { return string.Empty; }
        }

        private decimal GetSafeDecimalValue(DataRow row, string columnName)
        {
            if (row == null || row.IsNull(columnName)) return 0;
            try { return row.Field<decimal>(columnName); }
            catch { return 0; }
        }

        private string FormatBalanceWithState(decimal balance, string balanceState)
        {
            if (balance == 0) return string.Empty;

            string formattedBalance = balance.ToString("N2");

            switch (balanceState?.ToLower())
            {
                case "مدين":
                case "debit":
                    return $"{formattedBalance} مدين";
                case "دائن":
                case "credit":
                    return $"{formattedBalance} دائن";
                default:
                    return formattedBalance;
            }
        }

        private void DGVStyle()
        {
            if (DGV.DataSource == null) return;

            foreach (DataGridViewColumn column in DGV.Columns)
            {
                column.Visible = false;
            }

            string[] columnOrder = { "AccName", "ParentName", "BalanceWithState" };

            foreach (string columnName in columnOrder)
            {
                if (DGV.Columns.Contains(columnName))
                {
                    DGV.Columns[columnName].Visible = true;
                }
            }

            DGV.Columns["AccName"].DisplayIndex = 0;
            DGV.Columns["ParentName"].DisplayIndex = 1;
            DGV.Columns["BalanceWithState"].DisplayIndex = 2;

            DGV.Columns["AccName"].HeaderText = "اسم الحساب";
            DGV.Columns["ParentName"].HeaderText = "اسم الأب";
            DGV.Columns["BalanceWithState"].HeaderText = "الرصيد";

            int totalWidth = DGV.ClientRectangle.Width;
            DGV.Columns["AccName"].Width = (int)(totalWidth * 0.5);
            DGV.Columns["ParentName"].Width = (int)(totalWidth * 0.25);
            DGV.Columns["BalanceWithState"].Width = (int)(totalWidth * 0.25);

            DGV.Font = new Font("Times New Roman", 12, FontStyle.Bold);
            DGV.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Bold);
            DGV.RowHeadersVisible = false;
            DGV.AllowUserToAddRows = false;
            DGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGV.ReadOnly = true;
            DGV.DefaultCellStyle.Font = new Font("Times New Roman", 11, FontStyle.Regular);
            DGV.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            DGV.RowsDefaultCellStyle.BackColor = Color.White;

            if (DGV.Columns.Contains("BalanceWithState"))
            {
                DGV.Columns["BalanceWithState"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter ;
            }

            DGV.Columns["AccName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft ;
            DGV.Columns["ParentName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            DGV.BorderStyle = BorderStyle.None;
            DGV.EnableHeadersVisualStyles = false;
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray;
            DGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DGV.GridColor = Color.Gray;
        }
    }
}





















//using MizanOriginalSoft.MainClasses;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace MizanOriginalSoft.Views.Forms.Accounts
//{
//    public partial class frmAccounts : Form
//    {
//        private DataTable _allAccountsData; // لتخزين جميع البيانات
//        public frmAccounts()//تحذير 8618
//        {
//            InitializeComponent();
//        }

//        private void frmAccounts_Load(object sender, EventArgs e)
//        {
//            LoadAccountsTree();
//        }

//        private void LoadAccountsTree()
//        {
//            treeViewAccounts.Nodes.Clear();
//            _allAccountsData = DBServiecs.Acc_GetChart() ?? new DataTable();
//            if (_allAccountsData.Rows.Count == 0) return;

//            // Dictionary لتخزين العقد أثناء البناء
//            Dictionary<string, TreeNode> nodeDict = new Dictionary<string, TreeNode>();

//            // عرض الحسابات التي لها فروع فقط
//            var parentRows = _allAccountsData.AsEnumerable()
//                               .Where(r => r.Field<bool>("IsHasChildren"))
//                               .ToList();

//            foreach (DataRow row in parentRows)
//            {
//                string accName = row["AccName"] as string ?? string.Empty;
//                if (string.IsNullOrWhiteSpace(accName)) continue;

//                string? treeCode = row["TreeAccCode"].ToString();
//                string? parentCode = row["ParentAccID"] != DBNull.Value ? row["ParentAccID"].ToString() : null;

//                TreeNode? node = new TreeNode(accName) { Tag = row };
//                nodeDict[treeCode] = node;

//                if (string.IsNullOrEmpty(parentCode))
//                    treeViewAccounts.Nodes.Add(node); // عقدة الجذر
//                else if (nodeDict.TryGetValue(parentCode, out TreeNode? parentNode))
//                    parentNode.Nodes.Add(node); // إضافة العقدة للوالد
//                else
//                    treeViewAccounts.Nodes.Add(node); // fallback في حالة عدم وجود والد
//            }

//            SortTreeNodes(treeViewAccounts.Nodes); // ترتيب العقد تصاعديًا حسب AccID
//            treeViewAccounts.CollapseAll();        // طي جميع الفروع
//        }

//        private void SortTreeNodes(TreeNodeCollection nodes)
//        {
//            List<TreeNode> nodeList = nodes.Cast<TreeNode>()
//                                           .OrderBy(n =>
//                                           {
//                                               if (n.Tag is DataRow row)
//                                                   return row.Field<int>("TreeAccCode"); // كـ int
//                                               return 0;
//                                           })
//                                           .ToList();

//            nodes.Clear();
//            foreach (TreeNode node in nodeList)
//            {
//                nodes.Add(node);
//                SortTreeNodes(node.Nodes); // ترتيب الأبناء
//            }
//        }

//        private void treeViewAccounts_AfterSelect(object sender, TreeViewEventArgs e)
//        {
//            LoadChildAccountsToGrid(e.Node);
//            DGVStyle();
//        }


//        private void LoadChildAccountsToGrid(TreeNode? selectedNode)
//        {
//            if (selectedNode?.Tag == null || _allAccountsData == null) return;

//            DataRow? selectedRow = selectedNode.Tag as DataRow;

//            // القراءة الآمنة مع التحقق من القيمة
//            int parentTreeCode = (selectedRow?.Field<int>("TreeAccCode")).GetValueOrDefault();

//            var filteredRows = _allAccountsData.AsEnumerable()
//                .Where(r =>
//                {
//                    int? parentAccID = r.Field<int?>("ParentAccID");
//                    bool isHasChildren = r.Field<bool>("IsHasChildren");

//                    return parentAccID == parentTreeCode && !isHasChildren;
//                });

//            // باقي الكود يبقى كما هو...
//            if (filteredRows.Any())
//            {
//                DataTable childAccounts = filteredRows.CopyToDataTable();

//                // إضافة الأعمدة الجديدة
//                if (!childAccounts.Columns.Contains("ParentName"))
//                    childAccounts.Columns.Add("ParentName", typeof(string));
//                if (!childAccounts.Columns.Contains("BalanceWithState"))
//                    childAccounts.Columns.Add("BalanceWithState", typeof(string));

//                // تعبئة البيانات
//                foreach (DataRow row in childAccounts.Rows)
//                {
//                    // اسم الأب من FullPath
//                    string fullPath = GetSafeStringValue(row, "FullPath");
//                    string[] pathParts = fullPath.Split(new string[] { " → " }, StringSplitOptions.None);
//                    string parentName = pathParts.Length > 1 ? pathParts[pathParts.Length - 2] : "---";
//                    row["ParentName"] = parentName;

//                    // الرصيد المدمج مع الحالة
//                    decimal balance = GetSafeDecimalValue(row, "Balance");
//                    string balanceState = GetSafeStringValue(row, "BalanceState");
//                    string balanceWithState = FormatBalanceWithState(balance, balanceState);
//                    row["BalanceWithState"] = balanceWithState;
//                }

//                DGV.DataSource = childAccounts;
//            }
//            else
//            {
//                DGV.DataSource = null;
//            }
//        }
//        // الدوال المساعدة لقراءة القيم بشكل آمن
//        private int? GetSafeIntValue(DataRow row, string columnName)
//        {
//            if (row == null || row.IsNull(columnName)) return null;
//            try { return row.Field<int>(columnName); }
//            catch { return null; }
//        }

//        private bool GetSafeBoolValue(DataRow row, string columnName)
//        {
//            if (row == null || row.IsNull(columnName)) return false;
//            try { return row.Field<bool>(columnName); }
//            catch { return false; }
//        }

//        private decimal GetSafeDecimalValue(DataRow row, string columnName)
//        {
//            if (row == null || row.IsNull(columnName)) return 0;
//            try { return row.Field<decimal>(columnName); }
//            catch { return 0; }
//        }

//        private string GetSafeStringValue(DataRow row, string columnName)
//        {
//            if (row == null || row.IsNull(columnName)) return string.Empty;
//            try { return row.Field<string>(columnName) ?? string.Empty; }
//            catch { return string.Empty; }
//        }
//        // دالة لتنسيق الرصيد مع الحالة
//        private string FormatBalanceWithState(decimal balance, string balanceState)
//        {
//            if (balance == 0)
//                return string.Empty; // إخفاء إذا كان صفر

//            string formattedBalance = balance.ToString("N2");

//            switch (balanceState?.ToLower())
//            {
//                case "مدين":
//                case "debit":
//                    return $"{formattedBalance} مدين";
//                case "دائن":
//                case "credit":
//                    return $"{formattedBalance} دائن";
//                default:
//                    return formattedBalance; // إذا لم تكن الحالة معروفة
//            }
//        }

//        private void DGVStyle()
//        {
//            if (DGV.DataSource == null) return;

//            // إخفاء جميع الأعمدة أولاً
//            foreach (DataGridViewColumn column in DGV.Columns)
//            {
//                column.Visible = false;
//            }

//            // إظهار الأعمدة المطلوبة بالترتيب: اسم الحساب - اسم الأب - الرصيد المدمج
//            string[] columnOrder = { "AccName", "ParentName", "BalanceWithState" };

//            foreach (string columnName in columnOrder)
//            {
//                if (DGV.Columns.Contains(columnName))
//                {
//                    DGV.Columns[columnName].Visible = true;
//                }
//            }

//            // ترتيب الأعمدة
//            DGV.Columns["AccName"].DisplayIndex = 0;
//            DGV.Columns["ParentName"].DisplayIndex = 1;
//            DGV.Columns["BalanceWithState"].DisplayIndex = 2;

//            // عناوين الأعمدة
//            DGV.Columns["AccName"].HeaderText = "اسم الحساب";
//            DGV.Columns["ParentName"].HeaderText = "اسم الأب";
//            DGV.Columns["BalanceWithState"].HeaderText = "الرصيد";

//            // نسب العرض 2:1:1
//            int totalWidth = DGV.ClientRectangle.Width;
//            DGV.Columns["AccName"].Width = (int)(totalWidth * 0.5);   // 50%
//            DGV.Columns["ParentName"].Width = (int)(totalWidth * 0.25); // 25%
//            DGV.Columns["BalanceWithState"].Width = (int)(totalWidth * 0.25); // 25%

//            // باقي الإعدادات
//            DGV.Font = new Font("Times New Roman", 12, FontStyle.Bold);
//            DGV.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Bold);
//            DGV.RowHeadersVisible = false;
//            DGV.AllowUserToAddRows = false;
//            DGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
//            DGV.ReadOnly = true;
//            DGV.DefaultCellStyle.Font = new Font("Times New Roman", 11, FontStyle.Regular);
//            DGV.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
//            DGV.RowsDefaultCellStyle.BackColor = Color.White;

//            // تنسيق عمود الرصيد المدمج
//            if (DGV.Columns.Contains("BalanceWithState"))
//            {
//                DGV.Columns["BalanceWithState"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
//            }

//            // تنسيق عمود اسم الحساب واسم الأب
//            DGV.Columns["AccName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
//            DGV.Columns["ParentName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

//            // تحسين المظهر
//            DGV.BorderStyle = BorderStyle.None;
//            DGV.EnableHeadersVisualStyles = false;
//            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkBlue;
//            DGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
//            DGV.GridColor = Color.Gray;
//        }



//    }
//}