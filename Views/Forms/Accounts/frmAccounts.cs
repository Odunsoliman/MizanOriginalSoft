using Microsoft.IdentityModel.Tokens.Configuration;
using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses;
using MizanOriginalSoft.Views.Forms.MainForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static System.ComponentModel.Design.ObjectSelectorEditor;

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
            DBServiecs.A_UpdateAllDataBase();
            LoadAccountsTree(true);
            rdoAll.CheckedChanged += rdo_CheckedChanged;
            rdoDaeen.CheckedChanged += rdo_CheckedChanged;
            rdoMadeen.CheckedChanged += rdo_CheckedChanged;
            rdoEqual.CheckedChanged += rdo_CheckedChanged;

        }

        #region !!!!!!!!!! عرض الشجرة والجريد !!!!!!!!!!!
        private void LoadAccountsTree(bool collapseAll = true)
        {
            treeViewAccounts.Nodes.Clear();
            _allAccountsData = DBServiecs.Acc_GetChart() ?? new DataTable();

            if (_allAccountsData.Rows.Count == 0) return;

            // قاموس لحفظ العقد حسب TreeAccCode
            Dictionary<string, TreeNode> nodeDict = new Dictionary<string, TreeNode>();

            foreach (DataRow row in _allAccountsData.Rows)
            {
                string accName = row["AccName"] as string ?? string.Empty;
                if (string.IsNullOrWhiteSpace(accName)) continue;

                string treeCode = row["TreeAccCode"].ToString() ?? string.Empty;
                string? parentCode = row["ParentAccID"] != DBNull.Value ? row["ParentAccID"].ToString() : null;

                TreeNode node = new TreeNode(accName) { Tag = row };
                nodeDict[treeCode] = node;

                if (string.IsNullOrEmpty(parentCode))
                {
                    // عقدة جذر
                    treeViewAccounts.Nodes.Add(node);
                }
                else if (nodeDict.TryGetValue(parentCode, out TreeNode? parentNode))
                {
                    // إضافة للاب الموجود
                    parentNode.Nodes.Add(node);
                }
                else
                {
                    // الأب مش موجود لسه → أضف كجذر مؤقت
                    treeViewAccounts.Nodes.Add(node);
                }
            }

            // ترتيب العقد حسب TreeAccCode
            SortTreeNodes(treeViewAccounts.Nodes);

            if (collapseAll)
                treeViewAccounts.CollapseAll();
            else
                treeViewAccounts.ExpandAll();
        }

        // ترتيب العقد أبجديًا أو حسب TreeAccCode
        private void SortTreeNodes(TreeNodeCollection nodes)
        {
            List<TreeNode> sorted = nodes.Cast<TreeNode>()
                                         .OrderBy(n => ((DataRow)n.Tag)["TreeAccCode"].ToString())
                                         .ToList();

            nodes.Clear();
            foreach (TreeNode n in sorted)
            {
                nodes.Add(n);
                if (n.Nodes.Count > 0)
                    SortTreeNodes(n.Nodes);
            }
        }

    
        // ✅ حدث يتم تنفيذه عند اختيار أي عقدة في الشجرة
        private TreeNode? _lastSelectedNode = null; // للاحتفاظ بالعقدة السابقة

        //اختيار العقدة من الشجرة
        private void treeViewAccounts_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // ① نتأكد أن هناك عقدة مختارة
            if (e.Node?.Tag == null) return;

            TreeNode selectedNode = e.Node;

            // ==========================
            // 1) تلوين العقدة المختارة بالخط الأحمر
            // ==========================
            if (_lastSelectedNode != null)
            {
                _lastSelectedNode.ForeColor = treeViewAccounts.ForeColor; // إعادة اللون الافتراضي
            }
            selectedNode.ForeColor = Color.Red;
            _lastSelectedNode = selectedNode;

            // ==========================
            // 2) استخراج الصف (DataRow) من العقدة
            // ==========================
            if (selectedNode.Tag is not DataRow row) return;

            int treeAccCode = row.Field<int>("TreeAccCode");   // الترقيم الشجري
            int accID = row.Field<int>("AccID");               // المفتاح الأساسي
            string accName = row["AccName"]?.ToString() ?? string.Empty;
            string accPath = row["FullPath"]?.ToString() ?? string.Empty;

            bool hasChildren = row.Field<bool?>("IsHasChildren") ?? false;
            bool hasDetails = row.Field<bool?>("IsHasDetails") ?? false;
            bool isEnerAcc = row.Field<bool?>("IsEnerAcc") ?? false;

            // ==========================
            // 3) تحديث الـ Labels
            // ==========================
            lblSelectedTreeNod.Text = $"{treeAccCode} - {accName}";
            lblPathNode.Text = accPath;
            lblAccID_Tree.Text = accID.ToString();
            lblAccID_DGV.Text = string.Empty;
            DGV.ClearSelection();

            // ==========================
            // 4) تحديث بيانات التفاصيل
            // ==========================
            if (!hasDetails)
            {
                lblAccDataDetails.Text = "";
                tlpBtnExec.Enabled = false;
            }
            else
            {
                bool hasFixedAssetParent = false;
                TreeNode? currentNode = selectedNode;

                // البحث في جميع الآباء حتى الجذر للتحقق من TreeAccCode = 12
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

                // تغيير النص بناءً على النتيجة
                lblAccDataDetails.Text = hasFixedAssetParent ? "بيانات الأصل الثابت" : "بيانات شخصية";
                tlpBtnExec.Enabled = true;

                // تغيير ارتفاع صفوف الـ TableLayoutPanel
                if (hasFixedAssetParent)
                {
                    tlpDetailsData.RowStyles[0].Height = 1;
                    tlpDetailsData.RowStyles[0].SizeType = SizeType.Percent;

                    tlpDetailsData.RowStyles[1].Height = 99;
                    tlpDetailsData.RowStyles[1].SizeType = SizeType.Percent;
                }
                else
                {
                    tlpDetailsData.RowStyles[0].Height = 99;
                    tlpDetailsData.RowStyles[0].SizeType = SizeType.Percent;

                    tlpDetailsData.RowStyles[1].Height = 1;
                    tlpDetailsData.RowStyles[1].SizeType = SizeType.Percent;
                }
            }

            // ==========================
            // 5) تحميل الأبناء في الـ DGV
            // ==========================
            LoadChildrenInDGV(selectedNode);

            // ==========================
            // 6) تحميل التقارير (ممكن تفعّله لاحقًا)
            // ==========================
            // LoadReportsForSelectedAccount();
        }

        // تعديل عقدة مختارة
        private void btnModify_Click(object sender, EventArgs e)
        {
            if (treeViewAccounts.SelectedNode?.Tag is DataRow row)
            {
                int accID = Convert.ToInt32(row["AccID"]);

                using (frm_AccountModify frm = new frm_AccountModify(accID))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        // 1️⃣ إعادة تحميل الشجرة بالكامل
                        LoadAccountsTree();

                        // 2️⃣ البحث عن العقدة التي تم تعديلها باستخدام AccID
                        TreeNode? nodeToSelect = FindNodeByAccID(treeViewAccounts.Nodes, accID);

                        // 3️⃣ تحديد العقدة إذا تم العثور عليها
                        if (nodeToSelect != null)
                        {
                            treeViewAccounts.SelectedNode = nodeToSelect;
                            nodeToSelect.EnsureVisible(); // لضمان ظهور العقدة في العرض
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("يرجى اختيار حساب أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // دالة مساعدة للبحث عن العقدة حسب AccID في أي مستوى
        private TreeNode? FindNodeByAccID(TreeNodeCollection nodes, int accID)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag is DataRow row && Convert.ToInt32(row["AccID"]) == accID)
                    return node;

                // البحث في الأبناء
                TreeNode? found = FindNodeByAccID(node.Nodes, accID);
                if (found != null)
                    return found;
            }
            return null;
        }

        //تعميل الابناء فى الجريد
        private void LoadChildrenInDGV(TreeNode selectedNode)
        {
            if (selectedNode?.Tag is not DataRow parentRow) return;

            // كود الحساب المختار
            int parentTreeAccCode = parentRow.Field<int>("TreeAccCode");
            string parentName = selectedNode.Text; // اسم الأب من الشجرة

            // استدعاء الإجراء المخزن لجلب الأبناء
            DataTable dt = DBServiecs.Acc_GetChildren(parentTreeAccCode);

            // ✅ إضافة عمود ParentName يدويًا لو مش موجود
            if (!dt.Columns.Contains("ParentName"))
                dt.Columns.Add("ParentName", typeof(string));

            // ✅ تعبئة العمود باسم الأب
            foreach (DataRow row in dt.Rows)
            {
                row["ParentName"] = parentName;
            }

            // -----------------------------
            // ✅ إنشاء DataView للتصفية
            // -----------------------------
            DataView dv = dt.DefaultView;

            // فلترة بالراديو بوتن
            List<string> filters = new List<string>();

            if (rdoDaeen.Checked)
                filters.Add("Balance < 0");
            else if (rdoMadeen.Checked)
                filters.Add("Balance > 0");
            else if (rdoEqual.Checked)
                filters.Add("Balance = 0");
            // لو rdoAll.Checked → مفيش شرط إضافي

            // فلترة بالبحث في الاسم
            string searchText = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                // LIKE مع % عشان يجيب أي جزء من الاسم
                filters.Add($"AccName LIKE '%{searchText.Replace("'", "''")}%'");
            }

            // تطبيق كل الفلاتر
            dv.RowFilter = filters.Count > 0 ? string.Join(" AND ", filters) : "";

            // ربط الجدول بالـ DGV
            DGV.DataSource = dv;
            DGVStyle();
        }

        //تنسيق الجريد
        private void DGVStyle()
        {
            // ① إفراغ النص قبل كل تحميل جديد
            lblCountAndTotals.Text = string.Empty;

            // ② إذا مفيش مصدر بيانات → خروج
            if (DGV.DataSource == null) return;

            // ③ إخفاء كل الأعمدة كبداية
            foreach (DataGridViewColumn column in DGV.Columns)
            {
                column.Visible = false;
            }

            // ④ الأعمدة اللي نحب نظهرها بالترتيب
            string[] columnOrder = { "AccName", "ParentName", "BalanceWithState" };

            foreach (string columnName in columnOrder)
            {
                if (DGV.Columns.Contains(columnName))
                {
                    DGV.Columns[columnName].Visible = true;
                }
            }

            // ⑤ إعادة ترتيب الأعمدة إذا كانت موجودة
            if (DGV.Columns.Contains("AccName"))
                DGV.Columns["AccName"].DisplayIndex = 0;

            if (DGV.Columns.Contains("ParentName"))
                DGV.Columns["ParentName"].DisplayIndex = 1;

            if (DGV.Columns.Contains("BalanceWithState"))
                DGV.Columns["BalanceWithState"].DisplayIndex = 2;

            // ⑥ تغيير عناوين الأعمدة
            if (DGV.Columns.Contains("AccName"))
                DGV.Columns["AccName"].HeaderText = "اسم الحساب";

            if (DGV.Columns.Contains("ParentName"))
                DGV.Columns["ParentName"].HeaderText = "اسم الأب";

            if (DGV.Columns.Contains("BalanceWithState"))
                DGV.Columns["BalanceWithState"].HeaderText = "الرصيد";

            // ⑦ تحديد عرض الأعمدة نسبيًا من عرض الـ DGV
            int totalWidth = DGV.ClientRectangle.Width;
            if (DGV.Columns.Contains("AccName"))
                DGV.Columns["AccName"].Width = (int)(totalWidth * 0.5);

            if (DGV.Columns.Contains("ParentName"))
                DGV.Columns["ParentName"].Width = (int)(totalWidth * 0.25);

            if (DGV.Columns.Contains("BalanceWithState"))
                DGV.Columns["BalanceWithState"].Width = (int)(totalWidth * 0.25);

            // ⑧ تنسيقات عامة
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
                DGV.Columns["BalanceWithState"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            if (DGV.Columns.Contains("AccName"))
                DGV.Columns["AccName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            if (DGV.Columns.Contains("ParentName"))
                DGV.Columns["ParentName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            DGV.BorderStyle = BorderStyle.None;
            DGV.EnableHeadersVisualStyles = false;
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray;
            DGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DGV.GridColor = Color.Gray;

            // ===============================
            // ✅ حساب عدد الحسابات والإجمالي
            // ===============================
            try
            {
                if (DGV.Rows.Count > 0)
                {
                    int countAccounts = DGV.Rows.Count;

                    float totalBalance = 0;
                    if (DGV.Columns.Contains("Balance"))
                    {
                        foreach (DataGridViewRow row in DGV.Rows)
                        {
                            if (row.Cells["Balance"].Value != null &&
                                float.TryParse(row.Cells["Balance"].Value.ToString(), out float val))
                            {
                                totalBalance += val;
                            }
                        }
                    }

                    string balanceState = totalBalance > 0 ? "مدين" :
                                          totalBalance < 0 ? "دائن" : "متوازن";

                    lblCountAndTotals.Text = $"عدد الحسابات : {countAccounts}   " +
                                             $"بإجمالي رصيد : {Math.Abs(totalBalance):N2} ({balanceState})";
                }
                else
                {
                    lblCountAndTotals.Text = "لا توجد بيانات";
                }
            }
            catch
            {
                lblCountAndTotals.Text = string.Empty;
            }
            DGV.ClearSelection();
        }

        // اختيارات اظهار طبيعة الارصدة
        private void rdo_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is RadioButton rdo && rdo.Checked)
            {
                if (treeViewAccounts.SelectedNode != null)
                {
                    LoadChildrenInDGV(treeViewAccounts.SelectedNode);
                }
            }
        }

        // دالة البحث فى الجريد
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (treeViewAccounts.SelectedNode != null)
            {
                LoadChildrenInDGV(treeViewAccounts.SelectedNode);
            }
        }

        #endregion 
        
        #region !!!!!!!! حذف حساب شجرى او ابن من الجريد  !!!!!!!!!!!!!!

        // حذف حساب من الشجرة
        private void btnDeleteAccFromTree_Click(object sender, EventArgs e)
        {
            if (treeViewAccounts.SelectedNode?.Tag is not DataRow selectedRow)
            {
                MessageBox.Show("يجب اختيار عقدة من الشجرة المراد حذفها.", "تنبيه",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult inputResult = CustomMessageBox.ShowQuestion("هل تريد حذف الحساب الشجرى المحدد؟", "تأكيد");

            if (inputResult == DialogResult.Cancel)
            {
                MessageBox.Show("تم إلغاء الحذف.", "إلغاء",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 🔹 TreeAccCode للحساب المراد حذفه
            int treeAccCode = selectedRow.Field<int>("TreeAccCode");

            // 🔹 نخزن ParentAccID قبل الحذف عشان نرجع له
            int? parentTreeCode = selectedRow.Field<int?>("ParentAccID");

            // استدعاء الإجراء المخزن
            var result = DBServiecs.Acc_DeleteAccount(treeAccCode);

            if (result.Code == 0) // نجاح
            {
                MessageBox.Show(result.Message, "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // إعادة تحميل الشجرة
                LoadAccountsTree();

                // 🔹 نحدد العقدة الأب بعد الحذف
                if (parentTreeCode.HasValue)
                {
                    TreeNode? parentNode = FindTreeNodeByTreeCode(parentTreeCode.Value);
                    if (parentNode != null)
                    {
                        parentNode.Expand();
                        treeViewAccounts.SelectedNode = parentNode;

                        // تحميل الأبناء في الجريد
                        LoadChildrenInDGV(parentNode);
                    }
                }
            }
            else
            {
                MessageBox.Show(result.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // حذف حساب ابن من الجريد
        private void btnDeleteAccFromDGV_Click(object sender, EventArgs e)
        {
            if (DGV.CurrentRow == null)
            {
                MessageBox.Show("يجب اختيار حساب من الجدول.", "تنبيه",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRowView? rowView = DGV.CurrentRow.DataBoundItem as DataRowView;
            if (rowView == null)
            {
                MessageBox.Show("لا يوجد بيانات صالحة للحذف.", "خطأ",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataRow selectedRow = rowView.Row;

            DialogResult inputResult = CustomMessageBox.ShowQuestion("هل تريد حذف الحساب المحدد من الجدول؟", "تأكيد");

            if (inputResult == DialogResult.Cancel)
            {
                MessageBox.Show("تم إلغاء الحذف.", "إلغاء",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 🔹 TreeAccCode للحساب المراد حذفه
            int treeAccCode = selectedRow.Field<int>("TreeAccCode");

            // 🔹 نخزن ParentAccID قبل الحذف
            int? parentTreeCode = selectedRow.Field<int?>("ParentAccID");

            // 🔹 نحفظ رقم الصف الحالي عشان نحدد الصف التالي بعد الحذف
            int currentRowIndex = DGV.CurrentRow.Index;

            // استدعاء الإجراء المخزن
            var result = DBServiecs.Acc_DeleteAccount(treeAccCode);

            if (result.Code == 0) // نجاح
            {
                MessageBox.Show(result.Message, "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // إعادة تحميل الشجرة
                LoadAccountsTree();

                if (parentTreeCode.HasValue)
                {
                    TreeNode? parentNode = FindTreeNodeByTreeCode(parentTreeCode.Value);
                    if (parentNode != null)
                    {
                        parentNode.Expand();
                        treeViewAccounts.SelectedNode = parentNode;

                        // إعادة تحميل الأبناء في الجريد
                        LoadChildrenInDGV(parentNode);

                        // تحديد الصف المناسب بعد الحذف
                        if (DGV.Rows.Count > 0)
                        {
                            int newIndex = currentRowIndex;
                            if (newIndex >= DGV.Rows.Count)
                                newIndex = DGV.Rows.Count - 1; // لو الحذف كان آخر واحد نختار الصف السابق

                            // ابحث عن أول عمود ظاهر
                            DataGridViewColumn? firstVisibleCol = DGV.Columns
                                .Cast<DataGridViewColumn>()
                                .FirstOrDefault(c => c.Visible);

                            if (firstVisibleCol != null)
                            {
                                DGV.CurrentCell = DGV.Rows[newIndex].Cells[firstVisibleCol.Index];
                            }
                        }

                    }
                }
            }
            else
            {
                MessageBox.Show(result.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #endregion
































        #region !!!!!!! AfterSelect  بعد تحديد عقدة !!!!!!!!!!!!!!

 



        private void DGV_SelectionChanged(object sender, EventArgs e)
        {
            if (DGV.CurrentRow != null && DGV.CurrentRow.Cells["AccID"].Value != null)
            {
                lblAccID_DGV.Text = DGV.CurrentRow.Cells["AccID"].Value.ToString();
            }
            else
            {
                lblAccID_DGV.Text = string.Empty; // في حالة ما فيش صف محدد
            }
        }

        private void DGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (DGV.CurrentRow != null && DGV.CurrentRow.Cells["AccID"].Value != null)
            {
                lblAccID_DGV.Text = DGV.CurrentRow.Cells["AccID"].Value.ToString();
            }
            else
            {
                lblAccID_DGV.Text = string.Empty; // في حالة ما فيش صف محدد
            }
        }

        private bool IsRootNodeInRange(TreeNode node)
        {
            if (node?.Tag is DataRow row)
            {
                int treeAccCode = row.Field<int>("TreeAccCode");
                int? parentAccID = row.Field<int?>("ParentAccID");

                // التحقق إذا كانت عقدة جذرية (ليس لها والد) ورقمها بين 1-5
                return !parentAccID.HasValue && treeAccCode >= 1 && treeAccCode <= 5;
            }
            return false;
        }

        private void CollapseOtherRootNodes(TreeNode currentNode)
        {
            foreach (TreeNode rootNode in treeViewAccounts.Nodes)
            {
                if (rootNode != currentNode && IsRootNodeInRange(rootNode))
                {
                    rootNode.Collapse();
                }
            }
        }


        private void treeViewAccounts_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            // إذا كان e هو null، لا نستطيع فعل أي شيء
            if (e == null) return;

            // إذا كان Node هو null، نستخدم الرسم الافتراضي
            if (e.Node == null)
            {
                e.DrawDefault = true;
                return;
            }

            bool isRootNode = IsRootNode(e.Node);

            if (isRootNode)
            {
                Rectangle expandedBounds = new Rectangle(
                    e.Bounds.X,
                    e.Bounds.Y,
                    e.Bounds.Width,
                    e.Bounds.Height + 15
                );

                e.Graphics.FillRectangle(Brushes.White, expandedBounds);

                TextRenderer.DrawText(e.Graphics, e.Node.Text, e.Node.NodeFont ?? treeViewAccounts.Font,
                                    e.Bounds, e.Node.ForeColor, TextFormatFlags.VerticalCenter);

                if ((e.State & TreeNodeStates.Selected) != 0)
                {
                    using (Pen selectPen = new Pen(Color.Red, 2))
                    {
                        e.Graphics.DrawRectangle(selectPen, e.Bounds);
                    }
                }
            }
            else
            {
                e.DrawDefault = true;
            }
        }

        private bool IsRootNode(TreeNode? node)
        {
            if (node?.Tag is DataRow row)
            {
                int? parentAccID = row.Field<int?>("ParentAccID");
                return !parentAccID.HasValue;
            }
            return false;
        }
        #endregion


























        #region !!!!! منطقة البحث  !!!!!!!!!!
        private bool _isSearching = false;
        private void treeViewAccounts_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (_isSearching) return; // لا نغلق العقد عند البحث
            if (e?.Node == null) return;

            // إغلاق الجذور الأخرى إذا كانت العقدة المفتوحة من الجذور 1-5
            if (IsRootNodeInRange(e.Node))
            {
                CollapseOtherRootNodes(e.Node);
            }
        }
        // دخول وخروج مربع البحث
        private void txtSearchTree_Enter(object sender, EventArgs e) => _isSearching = true;
        private void txtSearchTree_Leave(object sender, EventArgs e) => _isSearching = false;
        private void txtSearchTree_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string searchText = txtSearchTree.Text.Trim();

                // إلغاء التحديد السابق والهايلايت
                ClearAllHighlights();

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    treeViewAccounts.CollapseAll();
                    return;
                }

                // البحث في جميع العقد
                SearchAndHighlightNodes(searchText);
            }
            finally
            {
            }
        }

        private void ClearAllHighlights()
        {
            foreach (TreeNode node in treeViewAccounts.Nodes)
            {
                ClearNodeHighlight(node);
            }
        }

        private void ClearNodeHighlight(TreeNode node)
        {
            node.BackColor = treeViewAccounts.BackColor;
            node.ForeColor = treeViewAccounts.ForeColor;

            foreach (TreeNode child in node.Nodes)
            {
                ClearNodeHighlight(child);
            }
        }

        private void SearchAndHighlightNodes(string searchText)
        {
            bool foundAny = false;

            foreach (TreeNode rootNode in treeViewAccounts.Nodes)
            {
                bool foundInBranch = SearchInNodeAndChildren(rootNode, searchText);
                if (foundInBranch)
                {
                    foundAny = true;
                    rootNode.Expand(); // فتح العقدة التي تحتوي على نتائج
                }
                else
                {
                    rootNode.Collapse(); // طي العقدة التي لا تحتوي على نتائج
                }
            }

            // إذا لم يتم العثور على أي نتائج، نفتح جميع العقد لعرض الشجرة كاملة
            if (!foundAny)
            {
                treeViewAccounts.ExpandAll();
            }
        }

        private bool SearchInNodeAndChildren(TreeNode node, string searchText)
        {
            if (node?.Text == null) return false;

            bool foundInCurrent = false;
            bool foundInChildren = false;

            // البحث في العقدة الحالية
            if (node.Text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                node.BackColor = Color.Yellow;
                node.ForeColor = Color.Black;
                foundInCurrent = true;
            }
            else
            {
                node.BackColor = treeViewAccounts.BackColor;
                node.ForeColor = treeViewAccounts.ForeColor;
            }

            // البحث في الأبناء
            foreach (TreeNode childNode in node.Nodes)
            {
                bool foundInChild = SearchInNodeAndChildren(childNode, searchText);
                if (foundInChild)
                {
                    foundInChildren = true;
                    node.Expand(); // فتح العقدة الأم إذا وجد نتائج في الأبناء
                }
            }

            return foundInCurrent || foundInChildren;
        }
        #endregion
        #region !!!!!!!!  Add Account  !!!!!!!!

        private void btnAccAccount_Click(object sender, EventArgs e)
        {
            AddChildrenFromTree();
        }

        private void btnStripAddChildren_Click(object sender, EventArgs e)
        {
            AddChildrenFromDGV();
        }

        private void AddChildrenFromTree()
        {
            if (treeViewAccounts.SelectedNode?.Tag is not DataRow selectedRow)
            {
                MessageBox.Show("يجب اختيار عقدة من الشجرة لإضافة حساب فرعي لها.", "تنبيه",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string userInput;
            DialogResult inputResult = CustomMessageBox.ShowStringInputBox(out userInput,
                "من فضلك أدخل اسم الحساب:", "إضافة حساب فرعي");

            if (inputResult != DialogResult.OK || string.IsNullOrWhiteSpace(userInput))
            {
                MessageBox.Show("تم إلغاء الإضافة أو لم يتم إدخال اسم صالح.", "إلغاء",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string accName = userInput.Trim();
            int parentTreeAccCode = selectedRow.Field<int>("TreeAccCode");
            int createByUserID = CurrentSession.UserID;

            string result = DBServiecs.Acc_AddAccount(accName, parentTreeAccCode, createByUserID);

            if (result.StartsWith("تم"))
            {
                MessageBox.Show("تم حفظ الحساب بنجاح ✅", "نجاح",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);

                // حفظ العقدة المحددة حالياً
                TreeNode? selectedNode = treeViewAccounts.SelectedNode;
                int selectedTreeCode = selectedRow.Field<int>("TreeAccCode");

                // إعادة تحميل الشجرة
                LoadAccountsTree();

                // البحث عن العقدة الأصلية وفتحها
                TreeNode? parentNode = FindTreeNodeByTreeCode(selectedTreeCode);
                if (parentNode != null)
                {
                    parentNode.Expand();
                    treeViewAccounts.SelectedNode = parentNode;

                    // تحميل الأبناء في الجريد
                    LoadChildrenInDGV(parentNode);
                }
            }
            else
            {
                MessageBox.Show("فشل في الحفظ ❌\n" + result, "خطأ",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddChildrenFromDGV()
        {
            if (DGV.CurrentRow?.DataBoundItem is not DataRowView rowView)
            {
                MessageBox.Show("يجب اختيار حساب من الجدول لإضافة حساب فرعي له.", "تنبيه",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRow row = rowView.Row;
            int parentTreeAccCode = row.Field<int>("TreeAccCode");
            //System.ArgumentException: 'Column 'TreeAccCode' does not belong to table .'
            string userInput;
            DialogResult inputResult = CustomMessageBox.ShowStringInputBox(out userInput,
                "من فضلك أدخل اسم الحساب:", "إضافة حساب فرعي");

            if (inputResult != DialogResult.OK || string.IsNullOrWhiteSpace(userInput))
            {
                MessageBox.Show("تم إلغاء الإضافة أو لم يتم إدخال اسم صالح.", "إلغاء",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string accName = userInput.Trim();
            int createByUserID = CurrentSession.UserID;

            string result = DBServiecs.Acc_AddAccount(accName, parentTreeAccCode, createByUserID);

            if (result.StartsWith("تم"))
            {
                MessageBox.Show("تم حفظ الحساب بنجاح ✅", "نجاح",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);

                // حفظ المعلومات قبل إعادة التحميل
                int selectedTreeCode = parentTreeAccCode;

                // إعادة تحميل الشجرة
                LoadAccountsTree();

                // البحث عن العقدة الأصلية وتحديدها
                TreeNode? parentNode = FindTreeNodeByTreeCode(selectedTreeCode);
                if (parentNode != null)
                {
                    treeViewAccounts.SelectedNode = parentNode;
                    parentNode.Expand();

                    // تحميل الأبناء الجدد في الجريد
                    LoadChildrenInDGV(parentNode);

                    // تمرير التركيز إلى الجريد
                    DGV.Focus();
                }
            }
            else
            {
                MessageBox.Show("فشل في الحفظ ❌\n" + result, "خطأ",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private TreeNode? FindTreeNodeByTreeCode(int treeCode)
        {
            foreach (TreeNode node in treeViewAccounts.Nodes)
            {
                TreeNode? foundNode = FindTreeNodeRecursive(node, treeCode);
                if (foundNode != null)
                    return foundNode;
            }
            return null;
        }

        private TreeNode? FindTreeNodeRecursive(TreeNode currentNode, int treeCode)
        {
            // التحقق المبدئي
            if (currentNode == null) return null;

            if (currentNode.Tag is DataRow row && row.Field<int>("TreeAccCode") == treeCode)
                return currentNode;

            // استخدام for بدلاً من foreach لتجنب التحذير
            for (int i = 0; i < currentNode.Nodes.Count; i++)
            {
                TreeNode? foundNode = FindTreeNodeRecursive(currentNode.Nodes[i], treeCode);
                if (foundNode != null)
                    return foundNode;
            }

            return null;
        }

        #endregion





    }
}
