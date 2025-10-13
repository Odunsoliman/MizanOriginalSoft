using MizanOriginalSoft.MainClasses.OriginalClasses;
using MizanOriginalSoft.MainClasses;
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
    public partial class frmAccountMamger : Form
    {
        private DataTable _allAccountsData = new DataTable();
        private readonly int ID_user;
        public frmAccountMamger()
        {
            InitializeComponent();
            ID_user = CurrentSession.UserID; // ← هذا هو الموضع الصحيح
        }

        private void frmAccountMamger_Load(object sender, EventArgs e)
        {
            DBServiecs.A_UpdateAllDataBase();
            LoadAccountsTree(true);
            rdoAll.CheckedChanged += rdo_CheckedChanged;
            rdoDaeen.CheckedChanged += rdo_CheckedChanged;
            rdoMadeen.CheckedChanged += rdo_CheckedChanged;
            rdoEqual.CheckedChanged += rdo_CheckedChanged;
            SetupMenuStrip();
        }

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

        // تحميل القوائم بناءً على الحساب الشجرى المحدد
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
                string? parentCode = row["ParentTree"] != DBNull.Value ? row["ParentTree"].ToString() : null;

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
            txtSearch.Text = string.Empty;
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
            LoadReportsForSelectedAccount();
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

        // تعديل الحساب النهائى المختارة
        private void btnModifyAccFromGrid_Click(object sender, EventArgs e)
        {
            if (DGV.CurrentRow == null)
            {
                MessageBox.Show("يرجى اختيار حساب أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int accID = Convert.ToInt32(DGV.CurrentRow.Cells["AccID"].Value);

            using (frm_AccountModify frm = new frm_AccountModify(accID))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    // 🔹 نحصل على العقدة المحددة حالياً في الشجرة
                    TreeNode? selectedNode = treeViewAccounts.SelectedNode;

                    if (selectedNode != null)
                    {
                        // 1️⃣ إعادة تحميل بيانات الأبناء في الجريد بناءً على العقدة المحددة
                        LoadChildrenInDGV(selectedNode);
                    }

                    // 2️⃣ تحديد نفس الصف بعد التعديل
                    foreach (DataGridViewRow row in DGV.Rows)
                    {
                        if (Convert.ToInt32(row.Cells["AccID"].Value) == accID)
                        {
                            row.Selected = true;

                            // 🔹 البحث عن أول عمود ظاهر لتعيينه كخلية حالية
                            DataGridViewCell? firstVisibleCell = row.Cells
                                .Cast<DataGridViewCell>()
                                .FirstOrDefault(c => c.Visible);

                            if (firstVisibleCell != null)
                            {
                                DGV.CurrentCell = firstVisibleCell;
                            }

                            // 🔹 ضمان ظهور الصف في العرض
                            DGV.FirstDisplayedScrollingRowIndex = row.Index;
                            break;
                        }
                    }


                    // 3️⃣ (اختياري) إعادة تحميل الشجرة بالكامل لو أردت تحديث أسم الحساب في الجهة الأخرى
                    // LoadAccountsTree();

                    /*توجد مشكلة صغيرة وهى
                     فى شاشة التعديل يمكن تعديل الاب للحساب وبذلك يختلف العدة التى دخلت منها الى العقدة التى تم التعديل اليها 
                    واريد ان يذهب اليها لتحديدها
                     
                     */
                }
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
        #endregion




        #region !!! البحث العام في جميع الحسابات !!!

        // متغير لتحديد وضع البحث العام
        private bool _isGlobalSearchMode = false;

        // عند دخول مربع البحث → تفعيل وضع البحث العام
        private void txtSearch_Enter(object sender, EventArgs e)
        {
            _isGlobalSearchMode = true;
        }

        // عند مغادرة مربع البحث → العودة للوضع العادي (تحميل أبناء الفرع المحدد فقط)
        private void txtSearch_Leave(object sender, EventArgs e)
        {
            _isGlobalSearchMode = false;

            if (treeViewAccounts.SelectedNode != null)
                LoadChildrenInDGV(treeViewAccounts.SelectedNode);
        }

        // عند تغيير نص البحث
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (_isGlobalSearchMode)
            {
                PerformGlobalSearch(txtSearch.Text.Trim());
                return;
            }

            if (treeViewAccounts.SelectedNode != null)
                LoadChildrenInDGV(treeViewAccounts.SelectedNode);
        }

        // 🔍 دالة البحث العام
        private void PerformGlobalSearch(string searchText)
        {
            // إذا لم يُكتب أي شيء → تفريغ النتائج
            if (string.IsNullOrWhiteSpace(searchText))
            {
                DGV.DataSource = null;
                lblCountAndTotals.Text = "أدخل نص البحث...";
                return;
            }

            // ✅ جلب كل الأبناء الورقيين فقط (بدون تحديد ParentTree)
            DataTable dt = DBServiecs.Acc_GetChildren(null);

            // ✅ فلترة محلية بالاسم
            DataView dv = dt.DefaultView;
            dv.RowFilter = $"AccName LIKE '%{searchText.Replace("'", "''")}%'";

            // ✅ ربط النتيجة بالجريد
            DGV.DataSource = dv;
            DGVStyle();

            // ✅ عرض ملخص النتائج
            lblCountAndTotals.Text = $"نتائج البحث عن: {searchText} ({dv.Count:N0} نتيجة)";
        }

        #endregion



















        #region !!!!!! DGV !!!!!!!!!!!
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
            {
                DGV.Columns["BalanceWithState"].Width = (int)(totalWidth * 0.25);

                // 🔥 تنسيق الرصيد بفاصلة الآلاف والمنازل العشرية
                DGV.Columns["BalanceWithState"].DefaultCellStyle.Format = "N2";
                DGV.Columns["BalanceWithState"].DefaultCellStyle.FormatProvider =
                    System.Globalization.CultureInfo.GetCultureInfo("ar-SA"); // للغة العربية
            }

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

            // محاذاة الخلايا
            if (DGV.Columns.Contains("BalanceWithState"))
                DGV.Columns["BalanceWithState"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; // 🔥 تغيير إلى اليمين للأرقام

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

                    decimal totalBalance = 0; // 🔥 تغيير إلى decimal لدقة أفضل
                    if (DGV.Columns.Contains("Balance")) // 🔥 استخدام العمود الصحيح
                    {
                        foreach (DataGridViewRow row in DGV.Rows)
                        {
                            if (row.Cells["Balance"].Value != null &&
                                decimal.TryParse(row.Cells["Balance"].Value.ToString(), out decimal val))
                            {
                                totalBalance += val;
                            }
                        }
                    }

                    string balanceState = totalBalance > 0 ? "مدين" :
                                          totalBalance < 0 ? "دائن" : "متوازن";

                    // 🔥 تنسيق الإجمالي بفاصلة الآلاف
                    lblCountAndTotals.Text = $"عدد الحسابات : {countAccounts:N0}   " +
                                           $"بإجمالي رصيد : {Math.Abs(totalBalance):N2} ({balanceState})";
                }
                else
                {
                    lblCountAndTotals.Text = "لا توجد بيانات";
                }
            }
            catch (Exception ex)
            {
                lblCountAndTotals.Text = "خطأ في حساب الإجمالي";
                // يمكنك تسجيل الخطأ: Console.WriteLine(ex.Message);
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
        private void txtSearch_TextChanged_(object sender, EventArgs e)
        {
            if (treeViewAccounts.SelectedNode != null)
            {
                LoadChildrenInDGV(treeViewAccounts.SelectedNode);
            }
        }
        private bool _isSearchingInChild = false;
        private void txtSearch_Enter__(object sender, EventArgs e)
        {
            _isSearchingInChild = true;

        }

        private void txtSearch_Leave__(object sender, EventArgs e)
        {
            _isSearchingInChild = false;
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
        /*هنا تتم تعبئة الجريد من خلال تحديد فرع من الشجرة فيظهر ابنائه فى الجريد بوضع مثالى
         ما اريده عند البحث عن احد الابناء ليس شرطأ ان يكون احد ابناء الفرع المحدد 
        اريد ان يكون البحث فى كل الابناء مهما كانت فروعهم 
        وريد ايقاف التحميل من خلال الفرع بشكل مؤقت حتى انهى البحث والخروج من مربع البحث
        فما السيناريو الذى يجب اتباعه
         */

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

            // 🔹 نخزن ParentTree قبل الحذف عشان نرجع له
            int? parentTreeCode = selectedRow.Field<int?>("ParentTree");

            // استدعاء الإجراء المخزن
            string result = DBServiecs.Acc_DeleteAccount(treeAccCode);

            // 🔹 عرض الرسالة القادمة من الإجراء كما هي (سواء نجاح أو فشل)
            MessageBoxIcon icon;

            if (result.StartsWith("✅"))
                icon = MessageBoxIcon.Information;
            else if (result.StartsWith("⚠️"))
                icon = MessageBoxIcon.Warning;
            else if (result.StartsWith("❌"))
                icon = MessageBoxIcon.Error;
            else
                icon = MessageBoxIcon.None;

            // عرض الرسالة للمستخدم
            MessageBox.Show(result, "نتيجة العملية", MessageBoxButtons.OK, icon);

            // 🔹 في حالة النجاح فقط، نُحدث الشجرة والجريد
            if (result.StartsWith("✅"))
            {
                // إعادة تحميل الشجرة بالكامل
                LoadAccountsTree();

                // تحديد الأب إن وُجد
                if (parentTreeCode.HasValue)
                {
                    TreeNode? parentNode = FindTreeNodeByTreeCode(parentTreeCode.Value);
                    if (parentNode != null)
                    {
                        treeViewAccounts.SelectedNode = parentNode;
                        parentNode.Expand();
                        LoadChildrenInDGV(parentNode);
                    }
                }
                else
                {
                    // في حال لم يكن له أب (أي أنه كان جذرًا)
                    treeViewAccounts.SelectedNode = null;
                    DGV.DataSource = null;
                }
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

            // 🔹 نخزن ParentTree قبل الحذف
            int? parentTreeCode = selectedRow.Field<int?>("ParentTree");

            // 🔹 نحفظ رقم الصف الحالي عشان نحدد الصف التالي بعد الحذف
            int currentRowIndex = DGV.CurrentRow.Index;
            // استدعاء الإجراء المخزن
            string result = DBServiecs.Acc_DeleteAccount(treeAccCode);

            // 🔹 تحديد نوع الأيقونة حسب محتوى الرسالة
            MessageBoxIcon icon;
            if (result.StartsWith("✅"))
                icon = MessageBoxIcon.Information;
            else if (result.StartsWith("⚠️"))
                icon = MessageBoxIcon.Warning;
            else if (result.StartsWith("❌"))
                icon = MessageBoxIcon.Error;
            else
                icon = MessageBoxIcon.None;

            // 🔹 عرض الرسالة كما هي للمستخدم
            MessageBox.Show(result, "نتيجة العملية", MessageBoxButtons.OK, icon);

            // 🔹 إذا كانت العملية ناجحة فقط، حدّث الـ DGV
            if (result.StartsWith("✅"))
            {
                // الحصول على العقدة الحالية في الشجرة (الأب)
                TreeNode? currentParentNode = treeViewAccounts.SelectedNode;

                if (currentParentNode != null)
                {
                    // إعادة تحميل أبناء العقدة فقط في الجريد
                    LoadChildrenInDGV(currentParentNode);

                    // محاولة تحديد الصف التالي بعد الحذف
                    if (DGV.Rows.Count > 0)
                    {
                        int newIndex = currentRowIndex;
                        if (newIndex >= DGV.Rows.Count)
                            newIndex = DGV.Rows.Count - 1; // لو الحذف كان آخر صف

                        // 🔹 تحديد أول عمود ظاهر لتجنّب الخطأ
                        DataGridViewColumn? firstVisibleColumn = DGV.Columns
                            .Cast<DataGridViewColumn>()
                            .FirstOrDefault(c => c.Visible);

                        if (firstVisibleColumn != null)
                        {
                            try
                            {
                                DGV.CurrentCell = DGV.Rows[newIndex].Cells[firstVisibleColumn.Index];
                            }
                            catch
                            {
                                // تجاهل أي خطأ في حالة الخلية غير قابلة للتحديد
                            }
                        }
                    }
                }
                else
                {
                    // إذا لم يكن هناك عقدة محددة، نظّف الجريد
                    DGV.DataSource = null;
                }
            }
        }


        #endregion

        #region !!!!!!! AfterSelect  بعد تحديد عقدة !!!!!!!!!!!!!!
        private void DGV_SelectionChanged(object? sender, EventArgs? e)
        {
            // تحقق من وجود صف حالي محدد وأن قيمة AccID موجودة وليست فارغة
            if (DGV.CurrentRow != null && DGV.CurrentRow.Cells["AccID"].Value != null)
            {
                // 1. استخراج القيم من الصف المحدد
                string? accID = DGV.CurrentRow.Cells["AccID"].Value.ToString();
                string? SorceID = DGV.CurrentRow.Cells["SorceIDAcc"].Value.ToString();
                string? treeAccCode = DGV.CurrentRow.Cells["TreeAccCode"].Value.ToString();
                string? accTypeName = DGV.CurrentRow.Cells["AccTypeName"].Value.ToString();
                string? balance = DGV.CurrentRow.Cells["Balance"].Value.ToString();
                string? balanceState = DGV.CurrentRow.Cells["BalanceState"].Value.ToString();
                bool isEnerAcc = Convert.ToBoolean(DGV.CurrentRow.Cells["IsEnerAcc"].Value);
                bool isHidden = Convert.ToBoolean(DGV.CurrentRow.Cells["IsHidden"].Value);
                string? createByUserID = DGV.CurrentRow.Cells["CreateByUserID"].Value.ToString();

                // 🔴 التعديل لضمان التنسيق (تم وضعه سابقاً في الجزء العلوي)
                DateTime dateOfJoinObject = Convert.ToDateTime(DGV.CurrentRow.Cells["DateOfJoin"].Value);
                string formattedDate = dateOfJoinObject.ToShortDateString();


                // 2. تعبئة العناوين بالصيغ المطلوبة

                // 🔹 lblAccID_DGV.Text  
                lblAccID_DGV.Text = $"المعرف: {accID} | الكود الشجري: {treeAccCode} | طبيعته: {accTypeName} |  {SorceID} ";

                // 🔹 lblBalanceToDay.Text
                string enerAccText = isEnerAcc ? "حـ: داخلي" : string.Empty;
                lblBalanceToDay.Text = $"الرصيد الحالي: {balance} {balanceState} {enerAccText}".Trim();

                // 🔹 lblGenralData.Text
                // ✅ استخدام formattedDate هنا يضمن ظهور التاريخ فقط
                string hiddenText = isHidden ? "مخفي" : string.Empty;
                lblGenralData.Text = $"{hiddenText} | أُنشئ في: {formattedDate} | بواسطة: {createByUserID}";
            }
            else
            {
                // في حالة عدم وجود صف محدد، إفراغ جميع العناوين
                lblAccID_DGV.Text = string.Empty;
                lblBalanceToDay.Text = string.Empty;
                lblGenralData.Text = string.Empty;
            }
        }

        private void DGV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DGV_SelectionChanged(null, null);
        }

        private bool IsRootNodeInRange(TreeNode node)
        {
            if (node?.Tag is DataRow row)
            {
                int treeAccCode = row.Field<int>("TreeAccCode");
                int? ParentTree = row.Field<int?>("ParentTree");

                // التحقق إذا كانت عقدة جذرية (ليس لها والد) ورقمها بين 1-5
                return !ParentTree.HasValue && treeAccCode >= 1 && treeAccCode <= 5;
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
                int? ParentTree = row.Field<int?>("ParentTree");
                return !ParentTree.HasValue;
            }
            return false;
        }
        #endregion

        #region !!!!!  منطقة البحث فى الشجرة !!!!!!!!!!
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

        //الخروج من مربع البحث فى الشجرة
        private void txtSearchTree_Leave(object sender, EventArgs e) => _isSearching = false;

        // حدث الكتابة فى مربع البخث
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

        // وظيفة الغاء الهاى لايت 
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

        // وضع هاى لايت اثناء البحث
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

        private void btnAddChildren_Click(object sender, EventArgs e)
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

            string result = DBServiecs.Acc_AddParentAccount(accName, parentTreeAccCode, createByUserID);

            if (result.Contains("نجاح"))
            {
                MessageBox.Show("تم اضافة حساب الفرع الشجرى بنجاح ✅", "نجاح",
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
            if (treeViewAccounts.SelectedNode?.Tag is not DataRow selectedRow)
            {
                MessageBox.Show("يجب اختيار عقدة من الشجرة لإضافة حساب ابن لها.", "تنبيه",
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

            string result = DBServiecs.Acc_AddFinalAccount(accName, parentTreeAccCode, createByUserID);

            if (result.Contains("نجاح"))
            {
                MessageBox.Show("تم حفظ حساب الابن بنجاح ✅", "نجاح",
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

        #region !!!!!!!!!  التعديل فى الحسابات وتفاصيلها !!!!!!!!!!!!

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
        private void btnAddDetails_Click(object sender, EventArgs e)
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
                    //         HighlightAndExpandNode(treeAccCode);
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

                    HighlightRowByTreeAccCode(treeAccCode);
                }
                else
                {
                    MessageBox.Show(resultMsg, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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




    }
}