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
            LoadAccountsTree();
            rdoAll.CheckedChanged += rdo_CheckedChanged;
            rdoDaeen.CheckedChanged += rdo_CheckedChanged;
            rdoMadeen.CheckedChanged += rdo_CheckedChanged;
            rdoEqual.CheckedChanged += rdo_CheckedChanged;

        }


        /// <summary>
        /// تحميل شجرة الحسابات من قاعدة البيانات بشكل هرمي.
        /// - يبني الشجرة باستخدام TreeAccCode و ParentAccID.
        /// - أي حساب بدون ParentAccID يضاف كجذر.
        /// - أي حساب له ParentAccID يضاف كابن داخل أبيه.
        /// - يتم تخزين الصف DataRow داخل خاصية Tag لكل عقدة.
        /// </summary>
        private void LoadAccountsTree()
        {
            // ➊ تفريغ أي بيانات سابقة من TreeView
            treeViewAccounts.Nodes.Clear();
            // ➋ جلب بيانات الحسابات (الأب فقط) أو إنشاء DataTable فارغ إذا لم ترجع نتائج
            _allAccountsData = DBServiecs.Acc_GetChart() ?? new DataTable();
            if (_allAccountsData.Rows.Count == 0) return;

            // ➌  قاموس للاحتفاظ بالعقد باستخدام TreeAccCode كمفتاح
            Dictionary<string, TreeNode> nodeDict = new Dictionary<string, TreeNode>();
            // ➍ المرور على جميع الصفوف التي تم إرجاعها من قاعدة البيانات
            foreach (DataRow row in _allAccountsData.Rows)
            {
                // 1️⃣ قراءة اسم الحساب من العمود AccName
                //    - إذا كان فارغًا أو null → يتم تجاهل هذا الصف
                string accName = row["AccName"] as string ?? string.Empty;
                if (string.IsNullOrWhiteSpace(accName)) continue;

                // 2️⃣ جلب الكود الشجري (TreeAccCode) للحساب
                //    - هذا الكود يمثل المفتاح الفريد للعقدة في الشجرة
                string treeCode = row["TreeAccCode"].ToString() ?? string.Empty;

                // 3️⃣ جلب كود الأب (ParentAccID) إن وجد
                //    - إذا كان NULL → يعني أن هذا الحساب هو جذر رئيسي (Root)
                string? parentCode = row["ParentAccID"] != DBNull.Value ? row["ParentAccID"].ToString() : null;

                // 4️⃣ إنشاء عقدة جديدة (TreeNode) بالاسم
                //    - نربط السطر الكامل (DataRow) داخل خاصية Tag
                //      للاستفادة من باقي بيانات الصف لاحقًا (مثل الرصيد أو رقم الحساب)
                TreeNode node = new TreeNode(accName) { Tag = row };

                // 5️⃣ إضافة هذه العقدة إلى القاموس باستخدام TreeAccCode كمفتاح
                //    - الهدف: تسهيل الوصول إليها عند إضافة أبنائها لاحقًا
                nodeDict[treeCode] = node;

                // 6️⃣ تحديد موقع العقدة في الشجرة:
                //    - إذا لم يكن لها أب → إضافتها كعقدة جذرية (Root Node)
                if (string.IsNullOrEmpty(parentCode))
                    treeViewAccounts.Nodes.Add(node);

                //    - إذا كان لها أب موجود بالفعل في القاموس → أضفها كابن لهذا الأب
                else if (nodeDict.TryGetValue(parentCode, out TreeNode? parentNode))
                    parentNode.Nodes.Add(node);

                //    - إذا كان لها أب لكن لم يتم إيجاده (قد يكون بسبب ترتيب البيانات أو خطأ في قاعدة البيانات)
                //      → نضيفها كجذر مؤقتًا حتى لا تُفقد من الشجرة
                else
                    treeViewAccounts.Nodes.Add(node);
            }

            // 7️⃣ بعد بناء الشجرة، يتم ترتيب العقد حسب TreeAccCode
            SortTreeNodes(treeViewAccounts.Nodes);

            // 8️⃣ طي جميع الفروع (CollapseAll) 
            //    - الهدف: إظهار الجذور فقط للمستخدم عند فتح الشجرة لأول مرة
            treeViewAccounts.CollapseAll();

        }

        // ترتيب العقد  داخل شجرة الحسابات بشكل متسلسل.
        // الفكرة:
        // 1. يقوم بتحويل مجموعة العقد (TreeNodeCollection) إلى قائمة عادية ليسهل فرزها.
        // 2. يعتمد في الترتيب على العمود TreeAccCode المخزن داخل الـ DataRow الموجود في Tag.
        // 3. بعد الترتيب:
        //     - يتم تفريغ المجموعة الأصلية.
        //     - إعادة إضافة العقد بالترتيب الصحيح.
        // 4. الدالة تستدعي نفسها (Recursion) لترتيب الأبناء داخل كل عقدة.
        // 
        // ملاحظات:
        // - Tag لكل عقدة يفترض أنه يحتوي على DataRow من الجدول الأصلي.
        // - TreeAccCode يمثل الكود الشجري للحساب (int) وهو الأساس في ترتيب العقد.
        // - إذا لم يكن Tag = DataRow → العقدة تعامل كأنها TreeAccCode = 0.
        private void SortTreeNodes(TreeNodeCollection nodes)
        {
            // ➊ تحويل مجموعة العقد إلى List ليسهل التعامل معها والفرز
            List<TreeNode> nodeList = nodes.Cast<TreeNode>()
                                           .OrderBy(n =>
                                           {
                                               // إذا كانت العقدة تحتوي على DataRow في Tag → استخدم TreeAccCode
                                               if (n.Tag is DataRow row)
                                                   return row.Field<int>("TreeAccCode");

                                               // إذا لم يوجد DataRow → اعتبر القيمة 0 (لضمان عدم حدوث خطأ)
                                               return 0;
                                           })
                                           .ToList();

            // ➋ تفريغ المجموعة الأصلية من العقد
            nodes.Clear();

            // ➌ إعادة إضافة العقد بترتيب TreeAccCode الصحيح
            foreach (TreeNode node in nodeList)
            {
                nodes.Add(node);

                // ➍ استدعاء الدالة بشكل متكرر (Recursion) لترتيب الأبناء لكل عقدة
                SortTreeNodes(node.Nodes);
            }
        }
        // ✅ حدث يتم تنفيذه عند اختيار أي عقدة في الشجرة
        private TreeNode? _lastSelectedNode = null; // للاحتفاظ بالعقدة السابقة

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

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (treeViewAccounts.SelectedNode != null)
            {
                LoadChildrenInDGV(treeViewAccounts.SelectedNode);
            }
        }



















        #region !!!!!!! AfterSelect  بعد تحديد عقدة !!!!!!!!!!!!!!

        private void treeViewAccounts_AfterSelect_(object sender, TreeViewEventArgs e)
        {
            txtSearch.Clear();//الكود الاصلى

            // التحقق من أن e و e.Node ليسا null
            if (e?.Node == null) return;

            // إعادة تعيين التنسيق السابق
            if (_lastSelectedNode != null)
            {
                _lastSelectedNode.ForeColor = treeViewAccounts.ForeColor;
            }

            // تطبيق التنسيق الجديد
            e.Node.ForeColor = Color.Red;
            _lastSelectedNode = e.Node;

            LoadChildAccountsToGrid(e.Node);
            DGVStyle();

            //الكود الجديد

            // ==========================
            // 0) التحقق من وجود عقدة محددة
            // ==========================
            if (treeViewAccounts.SelectedNode == null)
                return;

            TreeNode selectedNode = treeViewAccounts.SelectedNode;

            // التأكد من أن الـ Tag يحتوي على DataRow
            if (selectedNode.Tag is not DataRow row)
                return;


            // ==========================
            // 2) استخراج بيانات الحساب
            // ==========================
            int treeAccCode = row.Field<int>("TreeAccCode");      // الترقيم الشجري الجديد
            int accID = row.Field<int>("AccID");                  // المفتاح الأساسي فقط
            string accName = row["AccName"]?.ToString() ?? string.Empty;
            string accPath = row["FullPath"]?.ToString() ?? string.Empty;

            bool hasChildren = row.Field<bool?>("IsHasChildren") ?? false;
            bool hasDetails = row.Field<bool?>("IsHasDetails") ?? false;
            bool isEnerAcc = row.Field<bool?>("IsEnerAcc") ?? false;


            // ==========================
            // 4) تحديث التسميات في الواجهة
            // ==========================
            lblSelectedTreeNod.Text = $"{treeAccCode} - {accName}";      // عرض TreeAccCode بدل AccID
            lblPathNode.Text = accPath;// المسار الكامل من الجذر إلى العقدة
            lblAccID_Tree.Text = accID.ToString();
            lblAccID_DGV.Text = string.Empty;
            DGV.ClearSelection();

            // ==========================
            // 6) التحقق من الأصول الثابتة (Parent = 12)
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

                // تغيير ارتفاع صفوف الـ TableLayoutPanel بناءً على النتيجة
                if (hasFixedAssetParent)
                {
                    // الصف الأول 1% والثاني 99%
                    tlpDetailsData.RowStyles[0].Height = 1;
                    tlpDetailsData.RowStyles[0].SizeType = SizeType.Percent;

                    tlpDetailsData.RowStyles[1].Height = 99;
                    tlpDetailsData.RowStyles[1].SizeType = SizeType.Percent;
                }
                else
                {
                    // الصف الأول 99% والثاني 1%
                    tlpDetailsData.RowStyles[0].Height = 99;
                    tlpDetailsData.RowStyles[0].SizeType = SizeType.Percent;

                    tlpDetailsData.RowStyles[1].Height = 1;
                    tlpDetailsData.RowStyles[1].SizeType = SizeType.Percent;
                }

            }

            // ==========================
            // 7) تحميل التقارير الخاصة بالحساب المحدد
            // ==========================
            //            LoadReportsForSelectedAccount();
        }


        private void LoadChildAccountsToGrid(TreeNode? selectedNode)
        {



            //if (selectedNode?.Tag == null || _allAccountsData == null) return;

            //DataRow selectedRow = (DataRow)selectedNode.Tag;
            //int parentTreeCode = selectedRow.Field<int>("TreeAccCode");

            //var filteredRows = _allAccountsData.AsEnumerable()
            //    .Where(r =>
            //    {
            //        int? parentAccID = r.Field<int?>("ParentAccID");
            //        bool isHasChildren = r.Field<bool>("IsHasChildren");
            //        return parentAccID == parentTreeCode && !isHasChildren && MatchRadioFilter(r);
            //    });

            //if (filteredRows.Any())
            //{
            //    DataTable childAccounts = filteredRows.CopyToDataTable();

            //    if (!childAccounts.Columns.Contains("ParentName"))
            //        childAccounts.Columns.Add("ParentName", typeof(string));
            //    if (!childAccounts.Columns.Contains("BalanceWithState"))
            //        childAccounts.Columns.Add("BalanceWithState", typeof(string));

            //    foreach (DataRow row in childAccounts.Rows)
            //    {
            //        string fullPath = GetSafeStringValue(row, "FullPath");
            //        string[] pathParts = fullPath.Split(new string[] { " → " }, StringSplitOptions.None);
            //        string parentName = pathParts.Length > 1 ? pathParts[pathParts.Length - 2] : "---";
            //        row["ParentName"] = parentName;

            //        decimal balance = GetSafeDecimalValue(row, "Balance");
            //        string balanceState = GetSafeStringValue(row, "BalanceState");
            //        string balanceWithState = FormatBalanceWithState(balance, balanceState);
            //        row["BalanceWithState"] = balanceWithState;
            //    }

            //    DGV.DataSource = childAccounts;
            //}
            //else
            //{
            //    DGV.DataSource = null;
            //}
        }




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
                    LoadChildAccountsToGrid(parentNode);
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
                    LoadChildAccountsToGrid(parentNode);

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

        private void btnDeleteAccFromTree_Click(object sender, EventArgs e)
        {
            if (treeViewAccounts.SelectedNode?.Tag is DataRow row)
            {
                int accID = Convert.ToInt32(row["AccID"]);
                DeleteAcc(accID);
            }
            else
            {
                MessageBox.Show("يرجى اختيار حساب من الشجرة أولاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDeleteAccFromDGV_Click(object sender, EventArgs e)
        {
            if (DGV.CurrentRow?.DataBoundItem is DataRowView rowView)
            {
                int accID = Convert.ToInt32(rowView.Row["AccID"]);
                DeleteAcc(accID);
            }
            else
            {
                MessageBox.Show("يرجى اختيار حساب من الجدول أولاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DeleteAcc(int accID)
        {
            // استدعاء الإجراء المخزن والحصول على الرسالة
            string outputMsg = DBServiecs.Acc_DeleteAccount(accID);

            // عرض رسالة النجاح أو الخطأ للمستخدم
            MessageBox.Show(outputMsg, "نتيجة العملية", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // إذا كانت الرسالة تشير إلى نجاح العملية، أعيد تحميل الشجرة أو الجدول
            if (!string.IsNullOrEmpty(outputMsg) && outputMsg.StartsWith("تم"))
            {
                LoadAccountsTree();

                if (treeViewAccounts.SelectedNode != null)
                    LoadChildAccountsToGrid(treeViewAccounts.SelectedNode);
            }
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