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
        }
        #region !!!!!!! بناء الشجرة  !!!!!!!
        private void LoadAccountsTree()
        {
            treeViewAccounts.Nodes.Clear();

            DataTable dt = DBServiecs.Acc_GetChart() ?? new DataTable();
            if (dt.Rows.Count == 0) return;

            // فلترة الحسابات التي لها أبناء فقط للشجرة
            var parentRows = dt.AsEnumerable()
                               .Where(r => r.Field<bool>("IsHasChildren") == true)
                               .ToList();

            foreach (DataRow row in parentRows)
            {
                if (row["FullPath"] == DBNull.Value || row["AccName"] == DBNull.Value || row["AccID"] == DBNull.Value)
                    continue;

                string fullPath = row["FullPath"] as string ?? string.Empty;
                string accName = row["AccName"] as string ?? string.Empty;

                if (string.IsNullOrWhiteSpace(fullPath) || string.IsNullOrWhiteSpace(accName))
                    continue;

                int level = GetLevelFromFullPath(fullPath);
                TreeNode node = new TreeNode(accName)
                {
                    Tag = row
                };

                if (level == 0)
                {
                    treeViewAccounts.Nodes.Add(node);
                }
                else
                {
                    TreeNode? parentNode = FindParentNode(treeViewAccounts.Nodes, fullPath, level - 1);
                    if (parentNode is not null)
                        parentNode.Nodes.Add(node);
                    else
                        treeViewAccounts.Nodes.Add(node); // fallback
                }
            }

            SortTreeNodes(treeViewAccounts.Nodes);
            treeViewAccounts.CollapseAll();
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

        // 🔹 دالة ترتيب العقد حسب AccID تصاعديًا (Recursive)
        private void SortTreeNodes(TreeNodeCollection nodes)
        {
            List<TreeNode> nodeList = nodes.Cast<TreeNode>()
                                           .OrderBy(n =>
                                           {
                                               if (n.Tag is DataRow row && int.TryParse(row["AccID"].ToString(), out int accID))
                                                   return accID;
                                               return int.MaxValue;
                                           })
                                           .ToList();

            nodes.Clear();
            foreach (TreeNode node in nodeList)
            {
                nodes.Add(node);
                SortTreeNodes(node.Nodes); // ترتيب الأبناء كمان
            }
        }
        // اين يتم استدعاء الدالة LoadChildrenInDGV
        private void LoadChildrenInDGV(int parentAccID)
        {
            DataTable dt = DBServiecs.Acc_GetLeafChildren(parentAccID); // هذه الدالة ترجع كل الأبناء
            DGV.DataSource = dt.DefaultView;
            DGVStyl();

        }

        private void DGVStyl()
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

        #region !!!!!!  عرض الحسابات  !!!!!!!!
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
        private TreeNode? activeNode; // العقدة النشطة (المميزة بالأحمر)

        private void treeViewAccounts_AfterSelect(object sender, TreeViewEventArgs e)
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
                activeNode.NodeFont = new Font(treeViewAccounts.Font, FontStyle.Regular);
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

                tlpData.Visible = false;
                btnNew.Visible = false;
                btnSave.Visible = false;
            }
            else
            {
                lblParentAccName.Text = accName;
                lblParentAccName.ForeColor = Color.Gray;
                chkIsHasChildren.Enabled = true;

                btnNew.Visible = true;
                btnSave.Visible = true;
            }

            lblIsHasChildren.Text = hasChildren ? "" : "هذا الحساب مازال ليس له فروع";

            // ==========================
            // 6) التحقق من إذا كان الأب (أو أجداده) = 12 (أصول ثابتة)
            // ==========================
            bool hasFixedAssetParent = false;
            TreeNode? currentNode = selectedNode;
            while (currentNode != null)
            {
                if (currentNode.Tag is DataRow parentRow && Convert.ToInt32(parentRow["AccID"]) == 12)
                {
                    hasFixedAssetParent = true;
                    break;
                }
                currentNode = currentNode.Parent;
            }

            // ==========================
            // 7) التعامل مع البيانات التفصيلية
            // ==========================
            if (hasDetails)
            {
                tlpData.Visible = true;
                btnDetails.Text = hasFixedAssetParent ? "بيانات الأصل الثابت" : "بيانات شخصية";

                // الصف الأول ثابت 10%
                tlpData.RowStyles[0].SizeType = SizeType.Percent;
                tlpData.RowStyles[0].Height = 10;

                if (btnDetails.Text == "بيانات شخصية")
                {
                    tlpData.RowStyles[1].SizeType = SizeType.Percent;
                    tlpData.RowStyles[1].Height = 90;

                    tlpData.RowStyles[2].SizeType = SizeType.Percent;
                    tlpData.RowStyles[2].Height = 0;
                }
                else // بيانات الأصل الثابت
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

            // ==========================
            // 8) تحميل التقارير الخاصة بالحساب المحدد
            // ==========================
            LoadReportsForSelectedAccount();
        }


        private bool isSearchActive = false;// هذا المغيير للتعطيل المؤقت عند البحث

        //وظيفة غلق العقدة الاساسية العير مفعلة
        private void treeViewAccounts_BeforeExpand(object sender, TreeViewCancelEventArgs e)
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
        //تعطيل وتفعيل اغلاق القوائم التلقائى
        private void txtSearchTree_Leave(object sender, EventArgs e)
        {
            isSearchActive = false; // إعادة تفعيل التعامل مع BeforeExpand
        }
        private void txtSearchTree_Enter(object sender, EventArgs e)
        {
            isSearchActive = true; // تعطيل التعامل مع BeforeExpand
        }

        #endregion

        #region !!!!!!!!  ازرار الشاشة !!!!!!!!!
        private void btnDetails_Click(object sender, EventArgs e)
        {
            // إذا كانت مخفية يظهرها، وإذا كانت ظاهرة يخفيها
            tlpPhon.Visible = !tlpPhon.Visible;

        }

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
            int ParentAccID = Convert.ToInt32(selectedRow["AccID"]);
            int CreateByUserID = CurrentSession.UserID;

            // 🟢 استدعاء الإجراء
            string result = DBServiecs.Acc_AddAccount(AccName, ParentAccID, CreateByUserID);

            if (result.StartsWith("تم")) // يعني نجحت العملية
            {
                MessageBox.Show("تم حفظ الحساب بنجاح ✅");

                // 🟢 حفظ الـ ID بتاع العقدة المحددة
                int currentNodeId = ParentAccID;

                // 🟢 إعادة تحميل الشجرة
                LoadAccountsTree();
                txtSearchTree.Text = AccName;
                // 🟢 البحث عن العقدة بنفس الـ ID
                TreeNode? node = FindNodeByAccID(treeViewAccounts.Nodes, currentNodeId);

                if (node != null)
                {
                    treeViewAccounts.SelectedNode = node;
                    node.EnsureVisible(); // يخليها تبان حتى لو داخل فرع مغلق
                }

                // 🟢 فتح وتحديد العقدة الأب
                HighlightAndExpandNode(currentNodeId);
                txtAccName.Clear();
                chkIsHasChildren.Checked = false;
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
    }
}
