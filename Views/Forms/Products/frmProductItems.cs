
using Microsoft.CodeAnalysis;
using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses;
using MizanOriginalSoft.MainClasses.SearchClasses.MizanOriginalSoft.MainClasses.SearchClasses;
using MizanOriginalSoft.MainClasses.SearchClasses;
using MizanOriginalSoft.Views.Forms.MainForms;
using MizanOriginalSoft.Views.Reports;
using Signee.Views.Forms.Products;
using System.Data;
using System.Text;

namespace MizanOriginalSoft.Views.Forms.Products
{
    public partial class frmProductItems : Form
    {
        #region ===== Variables =====
        private DataTable? tblTree;
        public int? CategoryID { get; set; }
        public int Product_ID { get; private set; }
        public DataTable SelectedProducts { get; private set; } = new DataTable();
        private string ProdName = string.Empty;
        private string Note_Prod = string.Empty;
        private string ProdCodeOnSuplier = string.Empty;
        private string picProductPath = string.Empty;
        private ToolStripMenuItem tsmiCategoryReports = new();
        private ToolStripMenuItem tsmiGroupedReports = new();
        private DataRow? tblRow;
        private DataTable tblCategory = new();
        int ID_user;
        public bool newR = false;
        // متغيرات الشجرة وتحديدات الأصناف
        private TreeNode? lastSelectedNode = null;
        private readonly List<int> lastSelectedProductIds = new();
        //ربط بيانات المنتجات بعنصر DataGridView  ###
        private DataTable _tblProd = new();
        private DataTable? tblModify;
        private List<TreeNode> matchedNodes = new List<TreeNode>();
        private int currentMatchIndex = -1;

        // 🔹 اجعل الجدول والمجموعة متغيرات على مستوى الفورم
        private DataTable? tblSupplier;
        private AutoCompleteStringCollection? suppliersCollection;


        #endregion

        public frmProductItems()
        {
            InitializeComponent();
            ID_user = CurrentSession.UserID;
            SetupAutoCompleteSuppliers();
            SetupAutoCompleteCategories();
            FillUnits();
            tblModify = new DataTable();
        }
        private void frmProductItems_Load(object sender, EventArgs e)
        {
            InitializeTempDGV(); // ✅ تهيئة الجدول المؤقت
            LoadTreeAndSelectSpecificNode();

            treeViewCategories.AllowDrop = true; // مهم جداً لتفعيل الإفلات
            treeViewCategories.ItemDrag += treeViewCategories_ItemDrag;
            treeViewCategories.DragEnter += treeViewCategories_DragEnter;
            treeViewCategories.DragDrop += treeViewCategories_DragDrop;

            isFormLoaded = false;
            LoadProducts();
            ApplyColorTheme();
            isFormLoaded = true;
            SetupMenuStrip();//خاصة بقوائم التقارير
            LoadReports(200);//خاصة بقوائم التقارير
            DGV.ClearSelection();

            // 🔍 هنا نستدعي الدالة اللي تبحث عن الـControls عندها AutoSize = true
            CheckAutoSizeControls(this);
            LoadItemSettings();

        }


        // 🔍 دالة فحص AutoSize
        private void CheckAutoSizeControls(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Panel || ctrl is TableLayoutPanel || ctrl is FlowLayoutPanel)
                {
                    if (ctrl.AutoSize)
                    {
                        MessageBox.Show($"⚠ {ctrl.Name} عنده AutoSize = true");
                    }
                }

                // لو فيه عناصر داخلية (Nested) نفحصها برضه
                if (ctrl.HasChildren)
                {
                    CheckAutoSizeControls(ctrl);
                }
            }
        }


        #region *******  Help **************

        // فتح المساعدة الخاصة بالشاشة الحالية عند الضغط على زر "مساعدة"
        private void btnHelp_Click(object sender, EventArgs e)
        {
            frmHelp helpForm = new frmHelp(this.Name);
            helpForm.ShowDialog();
        }

        // فتح المساعدة الخاصة بالأداة النشطة عند الضغط على Ctrl + H
        private void ShowHelpForActiveControl()
        {
            if (this.ActiveControl == null)
                return;

            // استخدام الرمز & بدلاً من _
            string controlKey = $"{this.Name}&{this.ActiveControl.Name}";
            frmHelp helpForm = new frmHelp(controlKey);
            helpForm.ShowDialog();
        }

        // اعتراض الضغط على Ctrl + H في جميع الأدوات لفتح المساعدة الخاصة بها
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.H))
            {
                ShowHelpForActiveControl();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion

        #region ========= SetupAutoComplete and Fill =================


        private void SetupAutoCompleteSuppliers()
        {
            // تحميل الموردين مرة واحدة فقط
            if (suppliersCollection == null)
            {
                tblSupplier = DBServiecs.Accounts_GetSupplier();
                suppliersCollection = new AutoCompleteStringCollection();

                if (tblSupplier != null && tblSupplier.Rows.Count > 0)
                {
                    foreach (DataRow row in tblSupplier.Rows)
                    {
                        string? accName = row["AccName"]?.ToString();
                        if (!string.IsNullOrEmpty(accName))
                            suppliersCollection.Add(accName);
                    }
                }

                // Debug: طباعة عدد الموردين للتأكد من أن البيانات اتحملت
                System.Diagnostics.Debug.WriteLine($"📌 عدد الموردين المحملين: {suppliersCollection.Count}");
            }

            // 🔹 ربط نفس المصدر بكلا التكست بوكس
            SetupAutoCompleteForTextBox(txtSuppliers);
            SetupAutoCompleteForTextBox(txtNewItemSuppliers);
        }

        // 🔹 دالة مساعدة لتهيئة خاصية الـ AutoComplete
        private void SetupAutoCompleteForTextBox(TextBox textBox)
        {
            textBox.AutoCompleteCustomSource = suppliersCollection;
            textBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }



        //هاذا يستخدم فى اعادة البحث بالشتراك مع txtSeaarchProd
        private void txtSuppliers_TextChanged(object sender, EventArgs e)
        {
            txtSeaarchProd_TextChanged(this, EventArgs.Empty);
        }
        // ومفاتيح الكى داون شبه موحدة باختلاف سلوك الانتر 
        private void txtSuppliers_KeyDown(object sender, KeyEventArgs e)
        {
            // عند الضغط على Enter يتم الانتقال إلى حقل كود المنتج لدى المورد
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtSeaarchProd.Focus();
            }

            if (e.Control && e.KeyCode == Keys.H)
            {
                ShowHelpForActiveControl();
            }

            // عند الضغط على Ctrl + F يتم فتح شاشة البحث عن الموردين
            if (e.Control && e.KeyCode == Keys.F)
            {
                var provider = new GenericSearchProvider(SearchEntityType.Accounts, AccountKind.Suppliers);
                var result = SearchHelper.ShowSearchDialog(provider);

                if (!string.IsNullOrEmpty(result.Code))
                {
                    lblSuppliersID.Text = result.Code;
                    txtNewItemSuppliers.Text = result.Name;

                }
            }
        }








        //تعبئة مربع التصنيفات تعبئة تلقائية  ###
        private void SetupAutoCompleteCategories()
        {
            // جلب جميع التصنيفات
            tblCategory = DBServiecs.Categories_GetAll();
            // إنشاء مجموعة الإكمال التلقائي
            AutoCompleteStringCollection autoCompleteCollection = new AutoCompleteStringCollection();//'new' expression can be simplified
            // التأكد من أن الجدول يحتوي على بيانات
            if (tblCategory != null && tblCategory.Rows.Count > 0)
            {
                // إضافة أسماء المستخدمين إلى مجموعة الإكمال التلقائي
                foreach (DataRow row in tblCategory.Rows)
                {
                    string? accName = row["CategoryName"]?.ToString();
                    if (!string.IsNullOrEmpty(accName))
                    {
                        autoCompleteCollection.Add(accName);
                    }

                }
            }

            // إعداد خصائص مربع النص لاستخدام الإكمال التلقائي
            txtCategory.AutoCompleteCustomSource = autoCompleteCollection;
            txtCategory.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtCategory.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }

        private void RefreshForm()
        {
            tblTree = DBServiecs.Categories_GetAll();
            LoadTreeAndSelectSpecificNode();
            LoadProducts();
        }

        #endregion

        #region ********  Format  ********************

        // النظام العام للالوان على مجمل الشاشة  ###
        private void ApplyColorTheme()
        {
            // الألوان الأساسية للنموذج
            this.BackColor = Color.FromArgb(245, 245, 240); // لون خلفية النموذج الرئيسي - بيج فاتح
            panelDetails.BackColor = Color.FromArgb(255, 255, 240); // لون لوحة التفاصيل - أبيض عاجي
            panelList.BackColor = Color.FromArgb(250, 250, 250); // لون لوحة القائمة - أبيض فاتح
            treeViewCategories.BackColor = Color.FromArgb(250, 250, 250); // لون شجرة التصنيفات - أبيض فاتح

            // إعدادات العناوين الرئيسية
            lblTitle.ForeColor = Color.FromArgb(0, 100, 0); // لون العنوان - أخضر داكن
            lblTitle.Font = new Font("Tahoma", 20, FontStyle.Bold); // خط العنوان

            // أزرار التعديل (لون أزرق فاتح مع نص أزرق داكن)
            btnModifyItem.BackColor = Color.FromArgb(173, 216, 230); // أزرق فاتح (لون السماء)
            btnModifyItem.ForeColor = Color.DarkBlue; // نص أزرق داكن
            btnModifyItem.FlatStyle = FlatStyle.Flat; // نمط مسطح

            // زر الحذف (لون أحمر فاتح مع نص داكن)
            btnDelete.BackColor = Color.FromArgb(255, 200, 200); // أحمر فاتح (لون وردي خفيف)
            btnDelete.ForeColor = Color.DarkRed; // نص أحمر داكن
            btnDelete.FlatStyle = FlatStyle.Flat;

            // زر البحث المتقدم (لون أخضر فاتح مع نص داكن)
            btnAdvanceSearch.BackColor = Color.FromArgb(200, 255, 200); // أخضر فاتح
            btnAdvanceSearch.ForeColor = Color.DarkGreen; // نص أخضر داكن
            btnAdvanceSearch.FlatStyle = FlatStyle.Flat;

            btnHelp.BackColor = Color.FromArgb(200, 200, 200); // نفس لون زر الإضافة الجديدة
            btnHelp.ForeColor = Color.Black;
            btnHelp.FlatStyle = FlatStyle.Flat;

            // إعدادات الشبكة (DataGridView)
            DGV.BackgroundColor = Color.White; // خلفية بيضاء
            DGV.DefaultCellStyle.BackColor = Color.White; // خلفية الخلايا بيضاء
            DGV.DefaultCellStyle.ForeColor = Color.DarkSlateBlue; // نص الخلايا أزرق داكن
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230); // رؤوس الأعمدة رمادي فاتح
            DGV.EnableHeadersVisualStyles = false; // تعطيل الأنماط المرئية الافتراضية للرؤوس
        }

        // إعدادات اللون الحاوية للشجرة  ###
        private void panelList_Paint(object sender, PaintEventArgs e)
        {
            // إعدادات اللون وسمك الحد
            Color borderColor = Color.LightGreen;
            int borderWidth = 1;

            // رسم الإطار حول الحدود الخارجية
            ControlPaint.DrawBorder(e.Graphics, panelList.ClientRectangle,
                borderColor, borderWidth, ButtonBorderStyle.Solid,
                borderColor, borderWidth, ButtonBorderStyle.Solid,
                borderColor, borderWidth, ButtonBorderStyle.Solid,
                borderColor, borderWidth, ButtonBorderStyle.Solid);

        }


        #endregion

        #region ********** Search Tree Node ***********


        private void txtSearchTree_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearchTree.Text.Trim().ToLower();

            matchedNodes.Clear();
            currentMatchIndex = -1;

            // إعادة تعيين الألوان وإغلاق كل الفروع
            ResetNodeColorsAndCollapse(treeViewCategories.Nodes);

            if (string.IsNullOrEmpty(searchText))
                return;

            // البحث وتلوين النتائج وفتح الفروع التي تحتوي نتائج
            SearchAndHighlightNodes(treeViewCategories.Nodes, searchText);

            // اختيار أول نتيجة
            if (matchedNodes.Count > 0)
            {
                currentMatchIndex = 0;
                var node = matchedNodes[0];
                treeViewCategories.SelectedNode = node;
                node.EnsureVisible();
            }
        }
        private void ResetNodeColorsAndCollapse(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                node.BackColor = treeViewCategories.BackColor;
                node.ForeColor = treeViewCategories.ForeColor;
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

        #region ========== Tree  =====================

        private void LoadTreeAndSelectSpecificNode(int selectedID = 0)
        {
            // تحميل البيانات
            DataTable dt = DBServiecs.Categories_GetAll() ?? new DataTable();

            // تأكد أننا على الـ UI thread
            if (treeViewCategories.InvokeRequired)
            {
                treeViewCategories.Invoke(new Action(() => BuildTree(dt, selectedID)));
            }
            else
            {
                BuildTree(dt, selectedID);
            }
        }

        // دالة خاصة لبناء الشجرة
        private void BuildTree(DataTable dt, int selectedID)
        {
            treeViewCategories.BeginUpdate();
            treeViewCategories.Nodes.Clear();

            foreach (DataRow row in dt.Rows)
            {
                if (row["ParentID"] == DBNull.Value || Convert.ToInt32(row["ParentID"]) == 0)
                {
                    TreeNode parentNode = new TreeNode(row["CategoryName"].ToString())
                    {
                        Tag = Convert.ToInt32(row["CategoryID"])
                    };
                    treeViewCategories.Nodes.Add(parentNode);
                    AddChildNodes(dt, parentNode);
                }
            }

            treeViewCategories.CollapseAll(); // إغلاق الشجرة بعد التحميل

            if (selectedID > 0)
                SelectNodeById(treeViewCategories.Nodes, selectedID);

            treeViewCategories.EndUpdate();
        }

        // دالة إضافة الفروع
        private void AddChildNodes(DataTable dt, TreeNode parentNode)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (row["ParentID"] != DBNull.Value && Convert.ToInt32(row["ParentID"]) == (int)parentNode.Tag)
                {
                    TreeNode childNode = new TreeNode(row["CategoryName"].ToString())
                    {
                        Tag = Convert.ToInt32(row["CategoryID"])
                    };
                    parentNode.Nodes.Add(childNode);
                    AddChildNodes(dt, childNode);
                }
            }
        }

        // دالة البحث عن نود وتحديده
        private void SelectNodeById(TreeNodeCollection nodes, int categoryId)
        {
            foreach (TreeNode node in nodes)
            {
                if ((int)node.Tag == categoryId)
                {
                    treeViewCategories.SelectedNode = node;
                    node.EnsureVisible();
                    node.Expand();
                    return;
                }
                SelectNodeById(node.Nodes, categoryId);
            }
        }

        //حدث الضغط بالماوس على الشجرة  ##
        private void treeViewCategories_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode clickedNode = treeViewCategories.GetNodeAt(e.X, e.Y);

                if (clickedNode != null)
                {
                    treeViewCategories.SelectedNode = clickedNode;
                }
                else
                {
                    treeViewCategories.SelectedNode = null;
                }
                //if (chkTreeEnable.Checked) //{ } else 
                //{ LoadProducts(); }
            }
        }

        //حدث اصافة تصنيف جديد
        private void AddRootCategory_Click(object sender, EventArgs e)
        {
            int Cat_ID = 0;
            if (treeViewCategories.SelectedNode != null)
            {
                Cat_ID = Convert.ToInt32(treeViewCategories.SelectedNode.Tag);
            }

            if (CustomMessageBox.ShowStringInputBox(out string name, "أدخل اسم التصنيف", "إضافة") == DialogResult.OK &&
              !string.IsNullOrWhiteSpace(name))
            {
                DBServiecs.Categories_Insert(name, Cat_ID, ID_user);

                // تحميل البيانات من جديد داخل tblTree
                tblTree = DBServiecs.Categories_GetAll();

                // التحقق من أن الجدول ليس null ويحتوي على بيانات
                if (tblTree != null && tblTree.Rows.Count > 0)
                {
                    int maxID = tblTree.AsEnumerable().Max(r => r.Field<int>("CategoryID"));
                    LoadTreeAndSelectSpecificNode(maxID);
                }
                else
                {
                    MessageBox.Show("فشل في تحميل التصنيفات بعد الإضافة.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


        }

        //حدث تعديل اسم تصنيف  ###
        private void btnEditCategory_Click(object sender, EventArgs e)
        {
            if (treeViewCategories.SelectedNode == null)
            {
                CustomMessageBox.ShowInformation("الرجاء تحديد تصنيف لتعديله.", "تنبيه");
                return;
            }

            TreeNode selectedNode = treeViewCategories.SelectedNode;
            int Cat_ID = Convert.ToInt32(selectedNode.Tag);
            string oldName = selectedNode.Text;

            if (CustomMessageBox.ShowStringInputBox(out string newName, "أدخل الاسم الجديد بدلا من الاسم  " + oldName, "تعديل") == DialogResult.OK)
            {
                if (!string.IsNullOrWhiteSpace(newName) && newName != oldName)
                {
                    DBServiecs.Categories_UpdateName(Cat_ID, oldName, newName, ID_user);
                    RefreshForm();
                    LoadTreeAndSelectSpecificNode(Cat_ID);
                }
            }
        }

        // حدث حذف عقدة من الشجرة
        private void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            if (treeViewCategories.SelectedNode == null)
            {
                MessageBox.Show("الرجاء تحديد تصنيف لحذفه.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            TreeNode selectedNode = treeViewCategories.SelectedNode;

            int Cat_ID = Convert.ToInt32(selectedNode.Tag);
            string name = selectedNode.Text;

            var confirm = MessageBox.Show($"هل أنت متأكد من حذف التصنيف: {name}؟", "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                string result = DBServiecs.Categories_Delete(Cat_ID, ID_user);

                if (result.Contains("تم حذف التصنيف"))
                {
                    MessageBox.Show(result, "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // إعادة تحميل التصنيفات بعد الحذف
                    RefreshForm();
                }
                else
                {
                    MessageBox.Show(result, "خطأ أثناء الحذف", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void toolStripChangeCat_Click(object sender, EventArgs e)
        {
            if (DGV.SelectedRows.Count == 0)
            {
                MessageBox.Show("الرجاء تحديد صنف واحد على الأقل من الجدول.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int currentProductId = Convert.ToInt32(DGV.SelectedRows[0].Cells["ID_Product"].Value);

            DataTable dtProducts = new DataTable();
            dtProducts.Columns.Add("ID_Product", typeof(int));

            foreach (DataGridViewRow row in DGV.SelectedRows)
            {
                int productId = Convert.ToInt32(row.Cells["ID_Product"].Value);
                dtProducts.Rows.Add(productId);
            }

            using (frmCatTree frm = new frmCatTree(dtProducts))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    // ✅ افتح شاشة الانتظار في الـ UI
                    using (frmLoading loading = new frmLoading("⏳ يرجى الانتظار..."))
                    {
                        loading.Show();
                        loading.Refresh();

                        // ✅ نفذ العملية الثقيلة في الخلفية
                        await Task.Run(() =>
                        {
                            LoadProducts();
                            // أي عملية ثانية ثقيلة هنا
                        });

                        loading.Close();
                    }

                    // بعد التحديث رجّع المؤشر
                    foreach (DataGridViewRow row in DGV.Rows)
                    {
                        if (Convert.ToInt32(row.Cells["ID_Product"].Value) == currentProductId)
                        {
                            DGV.ClearSelection();
                            row.Selected = true;
                            DGV.CurrentCell = row.Cells["ProductCode"];
                            DGV.FirstDisplayedScrollingRowIndex = Math.Max(0, row.Index - 6);
                            break;
                        }
                    }
                }
            }
        }

        // بدء عملية السحب
        private void treeViewCategories_ItemDrag(object? sender, ItemDragEventArgs e)
        {
            if (e.Item is TreeNode draggedNode)
            {
                // 👈 منع سحب العقدة الأساسية رقم 1
                if (draggedNode.Tag is int categoryId && categoryId == 1)
                {
                    return; // تجاهل السحب
                }

                DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

        // عند إسقاط العنصر داخل TreeView
        private void treeViewCategories_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetData(typeof(TreeNode)) is TreeNode draggedNode)
            {
                // 👈 منع إسقاط العقدة الأساسية رقم 1
                if (draggedNode.Tag is int categoryId && categoryId == 1)
                {
                    return;
                }

                Point targetPoint = treeViewCategories.PointToClient(new Point(e.X, e.Y));
                TreeNode? targetNode = treeViewCategories.GetNodeAt(targetPoint);

                if (targetNode != null && !draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode))
                {
                    draggedNode.Remove();
                    targetNode.Nodes.Add(draggedNode);
                    targetNode.Expand();

                    if (draggedNode.Tag is int CategoryID && targetNode.Tag is int NewParentID)
                    {
                        // تحديث قاعدة البيانات
                        // UpdateCategoryParent(CategoryID, NewParentID);
                    }
                }
            }
        }
        // عند دخول العنصر إلى منطقة TreeView
        private void treeViewCategories_DragEnter(object? sender, DragEventArgs e)
        {
            if (e?.Data?.GetDataPresent(typeof(TreeNode)) == true)
                e.Effect = DragDropEffects.Move;
            else
                e!.Effect = DragDropEffects.None;
        }


        private bool ContainsNode(TreeNode parent, TreeNode child)
        {
            if (child.Parent == null) return false;
            if (child.Parent.Equals(parent)) return true;
            return ContainsNode(parent, child.Parent);
        }

        private void ExpandAndSearch(TreeNodeCollection nodes, string searchText)
        {
            foreach (TreeNode node in nodes)
            {
                node.Expand();

                // تطابق جزئي
                if (node.Text.ToLower().Contains(searchText))
                {
                    matchedNodes.Add(node); // أضف للمطابقات
                }

                if (node.Nodes.Count > 0)
                    ExpandAndSearch(node.Nodes, searchText);
            }
        }
        private void HighlightMatchingNodes(TreeNode node, string searchText)
        {
            // تحقق من تطابق جزئي (بأي جزء من الاسم)
            if (node.Text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                node.ForeColor = Color.Red; // تغيير لون النص للمطابقة
                node.BackColor = Color.Yellow; // اختياري: تغيير الخلفية
            }
            else
            {
                node.ForeColor = Color.Black; // إعادة الوضع الطبيعي
                node.BackColor = Color.FromArgb(192, 255, 192); // إعادة الخلفية
            }

            // التكرار على العقد الفرعية (إن وجدت)
            foreach (TreeNode child in node.Nodes)
            {
                HighlightMatchingNodes(child, searchText);
            }
        }
        private void txtSearchTree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && matchedNodes.Count > 0)
            {
                e.SuppressKeyPress = true;

                currentMatchIndex++;

                if (currentMatchIndex >= matchedNodes.Count)
                    currentMatchIndex = 0;

                var node = matchedNodes[currentMatchIndex];
                treeViewCategories.SelectedNode = node;
                node.EnsureVisible();
            }
            if (e.Control && e.KeyCode == Keys.H)
            {
                ShowHelpForActiveControl();
            }
        }
        private void SearchNodes(TreeNodeCollection nodes, string searchText)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text.ToLower().Contains(searchText))
                {
                    matchedNodes.Add(node);
                    node.BackColor = Color.Yellow;
                    node.ForeColor = Color.Red;
                }

                if (node.Nodes.Count > 0)
                    SearchNodes(node.Nodes, searchText);
            }
        }
        private void ResetNodeColors(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                //    node.BackColor = Color.Transparent ;
                node.BackColor = Color.FromArgb(192, 255, 192);

                node.ForeColor = Color.Black;

                if (node.Nodes.Count > 0)
                    ResetNodeColors(node.Nodes);
            }
        }
        //وظيفة تغيير لون الخاص بالريديو بوتن  ###
        private void UpdateRadioButtonColors()
        {
            Color trueColor = Color.FromArgb(192, 64, 0);
            Color falseColor = Color.Blue;

            rdoByNodeAndHisChild.ForeColor = rdoByNodeAndHisChild.Checked ? trueColor : falseColor;
            rdoByNode.ForeColor = rdoByNode.Checked ? trueColor : falseColor;
        }
        #endregion

        #region ========= DGV =================

        private void BindProductDataToDGV()
        {
            if (DGV.InvokeRequired)
            {
                DGV.Invoke(new Action(BindProductDataToDGV));
                return;
            }

            if (_tblProd != null && _tblProd.Rows.Count > 0)
            {
                DGV.DataSource = _tblProd;
            }
            else
            {
                DGV.DataSource = null;
            }

            UpdateCount();
            ApplyDGVStyles();
        }

        //تحميل الاصناف على الشاشة
        private void LoadProducts()
        {

            _tblProd = DBServiecs.Product_GetAll();
            BindProductDataToDGV();
        }

        // تطبيق التنسيقات المخصصة على DGV  ###
        private void ApplyDGVStyles()
        {
            // 1. ضبط نمط التحكم في حجم الأعمدة لملء المساحة المتاحة
            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 2. إخفاء جميع الأعمدة أولاً كخطوة تحضيرية
            foreach (DataGridViewColumn column in DGV.Columns)
            {
                column.Visible = false;
            }

            // 3. تعريف الأعمدة المراد إظهارها مع خصائص كل منها
            var visibleColumns = new[]
            {
        new
        {
            Name = "ProductCode",
            Header = "كود الصنف",
            FillWeight = 1,
            Alignment = DataGridViewContentAlignment.MiddleCenter
        },
        new
        {
            Name = "ProdName",
            Header = "اسم الصنف",
            FillWeight = 3,
            Alignment = DataGridViewContentAlignment.MiddleLeft
        },
        new
        {
            Name = "SuplierName",
            Header = "مورد",
            FillWeight = 1,
            Alignment = DataGridViewContentAlignment.MiddleLeft
        },
        new
        {
            Name = "U_Price",
            Header = "سعر بيع",
            FillWeight = 1,
            Alignment = DataGridViewContentAlignment.MiddleCenter
        },
        new
        {
            Name = "UnitProd",
            Header = "الوحدة",
            FillWeight = 1,
            Alignment = DataGridViewContentAlignment.MiddleCenter
        },
        new
        {
            Name = "CategoryName",
            Header = "التصنيف",
            FillWeight = 1,
            Alignment = DataGridViewContentAlignment.MiddleLeft
        },
        new
        {
            Name = "ProductStock",
            Header = "الرصيد",
            FillWeight = 1,
            Alignment = DataGridViewContentAlignment.MiddleCenter
        }
    };

            // 4. تطبيق الإعدادات على الأعمدة المرئية
            foreach (var col in visibleColumns)
            {
                if (DGV.Columns.Contains(col.Name))
                {
                    var column = DGV.Columns[col.Name];
                    column.Visible = true;
                    column.HeaderText = col.Header;
                    column.FillWeight = col.FillWeight;
                    column.DefaultCellStyle.Alignment = col.Alignment;
                }
            }

            // 5. تنسيقات الخلايا العامة
            DGV.DefaultCellStyle.Font = new Font("Times New Roman", 14);
            DGV.DefaultCellStyle.ForeColor = Color.Blue;
            DGV.DefaultCellStyle.BackColor = Color.LightYellow;

            // 6. تنسيقات رؤوس الأعمدة
            DGV.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Bold);
            DGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // 7. إضافة حدث لتلوين الصفوف
            DGV.RowPrePaint += DGV_RowPrePaint;

            // 8. معالجة النصوص في الخلايا
            DGV.CellFormatting += (sender, e) =>
            {
                if (e is null || e.Value is null) return;

                var col = DGV.Columns[e.ColumnIndex];
                if (col.Visible)
                {
                    string valueStr = e.Value.ToString()?.Trim() ?? string.Empty;
                    if (col.Name == "U_Price" && decimal.TryParse(valueStr, out decimal price))
                    {
                        e.Value = price.ToString("N2");
                    }
                    else
                    {
                        e.Value = valueStr;
                    }
                }
            };

            // 9. جعل الصفوف متناوبة الألوان
            DGV.AlternatingRowsDefaultCellStyle.BackColor = Color.LightCyan;
        }

        //حدث تحديث الوان اسطر DGV  ###
        private void DGV_RowPrePaint(object? sender, DataGridViewRowPrePaintEventArgs e)
        {
            // مثال: إذا كان الرصيد أقل من الحد الأدنى، غيّر لون الصف
            if (DGV.Rows[e.RowIndex].Cells["ProductStock"].Value != null &&
                DGV.Rows[e.RowIndex].Cells["MinLenth"].Value != null)
            {
                int stock = Convert.ToInt32(DGV.Rows[e.RowIndex].Cells["ProductStock"].Value);
                int minLength = Convert.ToInt32(DGV.Rows[e.RowIndex].Cells["MinLenth"].Value);

                if (stock < minLength)
                {
                    DGV.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightPink;
                }
            }
        }

        // تحديث صورة الباركود
        private void GenerateBarcode()
        {
            string productCode = lblProductCode.Text.Trim();

            // إنشاء كائن من BarcodeGenerator
            var generator = new BarcodeGenerator();

            // توليد باركود صغير الحجم
            var barcodeImage = generator.Generate(productCode, 100, 30, 1);

            PicBarcod.SizeMode = PictureBoxSizeMode.Zoom;
            PicBarcod.Image = barcodeImage;
        }
        #endregion

        private void btnNew_Click(object sender, EventArgs e)
        {
            tblRow = (_tblProd != null && _tblProd.Rows.Count > 0)
                        ? _tblProd.Rows[0]
                        : _tblProd?.NewRow(); // لا يوجد خطأ لأن tblRow يقبل null

            if (tblRow != null && _tblProd?.Rows.Count > 0)
            {
                DGV.ClearSelection();
            }

            newR = true;
        }


        #region تعديل صنف أو مجموعة أصناف
        private void btnModifyItem_Click(object sender, EventArgs e)
        {
            // ✅ احفظ العقدة المحددة وحالة التوسيع
            TreeNode selectedNode = treeViewCategories.SelectedNode;
            bool nodeExpanded = selectedNode != null && selectedNode.IsExpanded;

            try
            {
                if (DGV.Columns["ID_Product"] == null)
                {
                    MessageBox.Show("لا يوجد عمود ID_Product في الجدول", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // اجمع المنتجات المحددة
                List<int> selectedProductIds = new List<int>();
                foreach (DataGridViewRow row in DGV.SelectedRows)
                {
                    if (row.Cells["ID_Product"].Value != null)
                        selectedProductIds.Add(Convert.ToInt32(row.Cells["ID_Product"].Value));
                }

                if (selectedProductIds.Count == 0)
                {
                    MessageBox.Show("لم يتم تحديد أي صنف للتعديل", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (selectedProductIds.Count == 1) // تعديل فردي
                {
                    int productId = selectedProductIds[0];

                    using (frm_ProductModify frm = new frm_ProductModify(productId))
                    {
                        frm.StartPosition = FormStartPosition.CenterParent;
                        frm.ShowInTaskbar = false;

                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            // ✅ 1- إعادة تحميل المنتجات
                            LoadProducts();

                            // ✅ 2- إعادة الفلترة حسب العقدة السابقة
                            if (selectedNode != null)
                            {
                                treeViewCategories.SelectedNode = selectedNode;
                                if (nodeExpanded) selectedNode.Expand();
                                ApplyCategoryFilter(selectedNode);
                            }

                            // ✅ 3- إعادة تحديد الصنف المعدل إن كان مازال ضمن نفس التصنيف
                            ReselectAndCenterRow(productId);
                        }
                    }
                }
                else // تعديل جماعي
                {
                    DataTable selectedProducts = new DataTable();
                    selectedProducts.Columns.Add("ID_Product", typeof(int));
                    foreach (int id in selectedProductIds)
                        selectedProducts.Rows.Add(id);

                    var parameters = new Dictionary<string, object>
            {
                { "ScreenType", 1 },
                { "SelectedProducts", selectedProducts }
            };

                    using (frmProductsAction frm = new frmProductsAction(parameters))
                    {
                        frm.StartPosition = FormStartPosition.CenterParent;
                        frm.ShowInTaskbar = false;

                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            // ✅ 1- إعادة تحميل المنتجات
                            LoadProducts();

                            // ✅ 2- إعادة الفلترة حسب العقدة السابقة
                            if (selectedNode != null)
                            {
                                treeViewCategories.SelectedNode = selectedNode;
                                if (nodeExpanded) selectedNode.Expand();
                                ApplyCategoryFilter(selectedNode);
                            }

                            // ✅ 3- إعادة تحديد أول صنف من الأصناف المعدلة
                            ReselectAndCenterRow(selectedProductIds[0]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تنفيذ التعديل: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ✅ نفس الدالة كما هي
        private bool ReselectAndCenterRow(int productId)
        {
            foreach (DataGridViewRow row in DGV.Rows)
            {
                if (row.Cells["ID_Product"].Value != null &&
                    Convert.ToInt32(row.Cells["ID_Product"].Value) == productId)
                {
                    row.Selected = true;

                    var firstVisibleCell = row.Cells.Cast<DataGridViewCell>()
                                                    .FirstOrDefault(c => c.Visible);

                    if (firstVisibleCell != null)
                        DGV.CurrentCell = firstVisibleCell;

                    int rowIndex = row.Index;
                    int halfVisible = DGV.DisplayedRowCount(false) / 2;
                    int firstRow = Math.Max(0, rowIndex - halfVisible);
                    DGV.FirstDisplayedScrollingRowIndex = firstRow;

                    return true;
                }
            }
            return false;
        }

        #endregion

        #region ******************  DGV ********************
        //احداث DGV
        private bool isFormLoaded = false;

        // حدث تغير الاختيارات على الشاشة فى DGV   ###
        private void DGV_SelectionChanged(object sender, EventArgs e)
        {
            if (!isFormLoaded) return; // تجاهل الحدث إذا لم يتم تحميل الفورم بعد

            if (DGV.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = DGV.SelectedRows[0];

                object? idValue = selectedRow.Cells["ID_Product"]?.Value;
                object? codeValue = selectedRow.Cells["ProductCode"]?.Value;
                object? imagePath = selectedRow.Cells["PicProduct"]?.Value;
                object? registYear = selectedRow.Cells["RegistYear"]?.Value;
                object? noteProd = selectedRow.Cells["NoteProduct"]?.Value;

                lblID_Product.Text = idValue?.ToString() ?? string.Empty;
                lblProductCode.Text = codeValue?.ToString()?.Trim() ?? string.Empty;
                lblRegist_Year.Text = registYear?.ToString() ?? string.Empty;
                lblNoteProduct.Text = noteProd?.ToString() ?? string.Empty;

                // تحميل الصورة إن وجدت
                if (imagePath != null)
                {
                    string path = imagePath.ToString()!;
                    if (File.Exists(path))
                    {
                        picProd.Image = Image.FromFile(path);
                    }
                    else
                    {
                        picProd.Image = ImageHelper.CreateTextImage("الصورة غير متوفرة", picProd.Width, picProd.Height);
                    }
                }
                else
                {
                    picProd.Image = ImageHelper.CreateTextImage("الصورة غير متوفرة", picProd.Width, picProd.Height);
                }


                GenerateBarcode();
            }
        }

        private void SelectProductAfterRefresh(int productId)
        {
            if (DGV.DataSource is DataTable)
            {
                // تأكد أن الجدول يحتوي على صفوف بعد التحديث
                DGV.SuspendLayout();

                foreach (DataGridViewRow row in DGV.Rows)
                {
                    if (row.Cells["ID_Product"].Value != null &&
                        Convert.ToInt32(row.Cells["ID_Product"].Value) == productId)
                    {
                        row.Selected = true;

                        // تحديد خلية مناسبة
                        if (DGV.Columns.Contains("ProductCode") && DGV.Columns["ProductCode"].Visible)
                        {
                            DGV.CurrentCell = row.Cells["ProductCode"];
                        }
                        else
                        {
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                if (cell.Visible)
                                {
                                    DGV.CurrentCell = cell;
                                    break;
                                }
                            }
                        }

                        // إظهار الصف المحدد في منتصف الشاشة
                        int visibleRows = DGV.DisplayedRowCount(false);
                        int firstDisplayed = Math.Max(row.Index - visibleRows / 2, 0);
                        DGV.FirstDisplayedScrollingRowIndex = firstDisplayed;

                        break;
                    }
                }

                DGV.ResumeLayout();
            }
        }
        #endregion

        private void RefreshData()
        {
            try
            {
                LoadProducts(); // تحميل فعلي من SQL
                LoadTreeAndSelectSpecificNode();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحديث البيانات: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private TreeNode? FindNodeById(TreeNodeCollection nodes, string id)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Name == id) return node;

                TreeNode? found = FindNodeById(node.Nodes, id);
                if (found != null) return found;
            }
            return null;
        }
        private void btnNewItem_Click(object sender, EventArgs e)
        {
            try
            {
                string categoryName = "الكل";
                int categoryId = 0;

                // الحصول على التصنيف المحدد إن وجد
                if (treeViewCategories.SelectedNode != null && treeViewCategories.SelectedNode.Tag is int)
                {
                    categoryId = (int)treeViewCategories.SelectedNode.Tag;
                    categoryName = treeViewCategories.SelectedNode.Text;
                }

                // إعداد معلمات النافذة
                Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "ScreenType", 3 }, // نوع الشاشة (إضافة جديد)
            { "Cat_ID", categoryId },
            { "Cat_Name", categoryName }
        };

                using (frmProductsAction frm = new frmProductsAction(parameters))
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.ShowInTaskbar = false;

                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        // بعد الإضافة الناجحة، إعادة تحميل البيانات مع استعادة التصنيف فقط
                        RefreshAndRestoreSelection(restoreProducts: false);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء محاولة إضافة صنف جديد: {ex.Message}",
                              "خطأ",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }
        private void RefreshAndRestoreSelection(bool restoreProducts = true)
        {
            try
            {
                // حفظ العقدة المحددة قبل التحديث
                TreeNode? nodeToSelect = lastSelectedNode;

                // تحديث البيانات
                frmProductItems_Load(this, EventArgs.Empty);
                DGV.Refresh();

                // استعادة تحديد العقدة في الشجرة
                if (nodeToSelect != null)
                {
                    treeViewCategories.SelectedNode = FindTreeNode(treeViewCategories.Nodes, nodeToSelect);
                    treeViewCategories.Focus();
                }

                // استعادة تحديد الأصناف إذا طلبنا ذلك
                if (restoreProducts && lastSelectedProductIds.Count > 0)
                {
                    RestoreProductSelection(lastSelectedProductIds);
                }

                MessageBox.Show("تم تحديث البيانات بنجاح", "تحديث", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحديث البيانات: {ex.Message}",
                              "خطأ",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }
        private TreeNode? FindTreeNode(TreeNodeCollection nodes, TreeNode nodeToFind)
        {
            foreach (TreeNode node in nodes)
            {
                // المقارنة بناء على خاصية Tag والنص
                if (node.Tag != null && nodeToFind.Tag != null &&
                    node.Tag.Equals(nodeToFind.Tag) && node.Text == nodeToFind.Text)
                {
                    return node;
                }

                // البحث في العقد الفرعية
                TreeNode? foundNode = FindTreeNode(node.Nodes, nodeToFind);
                if (foundNode != null) return foundNode;
            }
            return null;
        }
        private void RestoreProductSelection(List<int> productIds)
        {
            if (DGV.Columns["ID_Product"] == null || productIds == null || productIds.Count == 0)
                return;

            DGV.ClearSelection(); // مسح التحديدات الحالية

            foreach (DataGridViewRow row in DGV.Rows)
            {
                if (row.Cells["ID_Product"].Value != null)
                {
                    int productId = Convert.ToInt32(row.Cells["ID_Product"].Value);
                    if (productIds.Contains(productId))
                    {
                        row.Selected = true;
                        // جعل الصف المحدد مرئياً إذا كان واحد فقط
                        if (DGV.SelectedRows.Count == 1)
                        {
                            DGV.FirstDisplayedScrollingRowIndex = row.Index;
                        }
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (DGV.SelectedRows.Count == 0)
                {
                    MessageBox.Show("لم يتم تحديد أي أصناف للحذف", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult confirmResult;

                if (DGV.SelectedRows.Count == 1)
                {
                    var row = DGV.SelectedRows[0];
                    string productCode = row.Cells["ProductCode"]?.Value?.ToString() ?? "غير معروف";
                    string productName = row.Cells["ProdName"]?.Value?.ToString() ?? "بدون اسم";

                    confirmResult = MessageBox.Show(
                        $"هل أنت متأكد من أنك تريد حذف الصنف التالي؟\n\nالرمز: {productCode}\nالاسم: {productName}",
                        "تأكيد الحذف",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2);
                }
                else
                {
                    confirmResult = MessageBox.Show(
                        $"هل أنت متأكد من أنك تريد حذف {DGV.SelectedRows.Count} صنف؟",
                        "تأكيد الحذف",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2);
                }

                if (confirmResult != DialogResult.Yes)
                    return;

                int deletedCount = 0;
                int notDeletedCount = 0;
                StringBuilder notDeletedDetails = new StringBuilder();

                foreach (DataGridViewRow row in DGV.SelectedRows)
                {
                    if (row.Cells["ID_Product"].Value == null)
                    {
                        notDeletedCount++;
                        continue;
                    }

                    int ID_Product = Convert.ToInt32(row.Cells["ID_Product"].Value);
                    string productCode = row.Cells["ProductCode"]?.Value?.ToString() ?? "غير معروف";

                    string result = DBServiecs.Product_Delete(ID_Product);

                    if (result.Contains("تم الحذف"))
                    {
                        deletedCount++;
                    }
                    else
                    {
                        notDeletedCount++;
                        notDeletedDetails.AppendLine($"- الصنف {productCode}: {result}");
                    }
                }

                string message = $"تم حذف {deletedCount} صنف بنجاح";

                if (notDeletedCount > 0)
                {
                    message += $"\n\nلم يتم حذف {notDeletedCount} صنف للأسباب التالية:\n{notDeletedDetails}";
                }

                MessageBox.Show(message, "نتيجة الحذف", MessageBoxButtons.OK,
                    notDeletedCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

                if (deletedCount > 0)
                {
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء عملية الحذف: {ex.Message}",
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void Pic_Click(object sender, EventArgs e)
        {
            frmPrintBarCode frm = new frmPrintBarCode();
            frm.ShowDialog();
        }
        private void btnBarcod_Click(object sender, EventArgs e)
        {
            frmPrintBarCode frm = new frmPrintBarCode();
            frm.ShowDialog();
        }


        #region ######### إعداد قوائم التقارير بناءً على ReportsMaster ########

        // إنشاء شريط القوائم داخل Panel
        private MenuStrip? menuStrip1;

        //القوائم (تقارير الصنف المحدد ▼ و تقارير مجمعة للأصناف المحددة ▼) تصطف من اليمين للشمال بدل العكس.
        private void SetupMenuStrip()
        {
            this.Controls.Add(menuStrip1);
            MenuStrip mainMenu = new MenuStrip
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightSteelBlue,
                Font = new Font("Times New Roman", 14, FontStyle.Regular)
            };

            tsmiCategoryReports = new ToolStripMenuItem("تقارير الصنف المحدد ▼");
            tsmiGroupedReports = new ToolStripMenuItem("تقارير مجمعة للأصناف المحددة ▼");

            mainMenu.Items.Add(tsmiCategoryReports);
            mainMenu.Items.Add(tsmiGroupedReports);

            pnlMenuContainer.Controls.Add(mainMenu);
            mainMenu.Location = new Point(10, 5);

        }


        // تحميل القوائم بناءً على الحساب الممرر
        private void LoadReports(int topAcc)
        {
            try
            {
                DataTable dt = DBServiecs.Reports_GetByTopAcc(topAcc, false);


                // تقارير فردية
                DataRow[] singleReports = dt.Select("IsGrouped = 0");
                LoadMenuItems(tsmiCategoryReports, singleReports);

                // تقارير مجمعة
                DataRow[] groupedReports = dt.Select("IsGrouped = 1");
                LoadMenuItems(tsmiGroupedReports, groupedReports);
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء تحميل التقارير: " + ex.Message);
            }
        }

        // تعبئة القائمة بعناصر من DataRow[]
        private void LoadMenuItems(ToolStripMenuItem parentMenu, DataRow[] rows)
        {
            parentMenu.DropDownItems.Clear();

            if (rows.Length == 0)
            {
                ToolStripMenuItem emptyItem = new("لا توجد تقارير متاحة") { Enabled = false };
                parentMenu.DropDownItems.Add(emptyItem);
                return;
            }

            foreach (DataRow row in rows)
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
                MessageBox.Show("بيانات التقرير غير صحيحة.");
                return;
            }

            try
            {
                // نسخة من البيانات الأساسية
                Dictionary<string, object> reportParameters = new(tagData)
        {
            { "UserID", ID_user }
        };

                bool isGrouped = Convert.ToBoolean(tagData["IsGrouped"]);

                if (isGrouped)
                {
                    reportParameters["FilteredData"] = GetFilteredData();
                }
                else
                {
                    reportParameters["EntityID"] = GetCurrentEntityID() ?? (object)DBNull.Value;
                }

                using frmSettingReports previewForm = new frmSettingReports(reportParameters);
                previewForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء فتح التقرير: {ex.Message}");
            }
        }


        // جلب كود الصنف الحالي
        private int? GetCurrentEntityID()
        {
            if (int.TryParse(lblID_Product.Text, out int id))
                return id;
            else
            {
                MessageBox.Show("⚠️ يجب اختيار صنف قبل عرض التقرير.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
        }


        #endregion


        #region  طريقة العرض والبحث والفلترة

        // 🟢 أحداث الراديو كلها تستدعي نفس الفلترة
        private void rdoPlusStok_CheckedChanged(object sender, EventArgs e) => ApplyAllFilters();
        private void rdoMinusStok_CheckedChanged(object sender, EventArgs e) => ApplyAllFilters();
        private void rdoZeroStok_CheckedChanged(object sender, EventArgs e) => ApplyAllFilters();
        private void rdoAllStok_CheckedChanged(object sender, EventArgs e) => ApplyAllFilters();

        // 🔹 الشرط الخاص بالمخزون
        private string GetStockCondition()
        {
            if (rdoMinusStok.Checked) return "[ProductStock] < 0";
            if (rdoPlusStok.Checked) return "[ProductStock] > 0";
            if (rdoZeroStok.Checked) return "[ProductStock] = 0";
            return "1=1"; // الكل
        }

        // 🔹 جمع كل شروط الفلترة (بحث + راديو + شجرة + أكواد)

        private void ApplyAllFilters()
        {
            try
            {
                DataTable baseTable = _tblProd ?? new DataTable();//العدد هنا 4494
                DataView dv = new DataView(baseTable);

                List<string> conditions = new List<string>();

                // 1️⃣ شرط المخزون
                conditions.Add(GetStockCondition());

                // 2️⃣ التصنيف من الشجرة
                if (lastSelectedNode != null)
                {
                    if (rdoByNode.Checked && lastSelectedNode.Tag != null)
                    {
                        if (int.TryParse(lastSelectedNode.Tag.ToString(), out int catId))
                            conditions.Add($"Category_id = {catId}");
                    }
                    else if (rdoByNodeAndHisChild.Checked)
                    {
                        List<int> ids = CollectCategoryIds(lastSelectedNode);
                        if (ids.Any())
                            conditions.Add($"Category_id IN ({string.Join(",", ids)})");
                    }
                }

                // 3️⃣ البحث بالنص
                string searchText = txtSeaarchProd.Text.Trim();
                if (!string.IsNullOrEmpty(searchText))
                {
                    string[] words = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string word in words)
                    {
                        string escaped = word.Replace("'", "''");
                        conditions.Add($"(ProdName LIKE '%{escaped}%' OR NoteProduct LIKE '%{escaped}%')");
                    }
                }

                // 4️⃣ البحث بالسعر
                if (decimal.TryParse(txtSeaarchProdPrice.Text.Trim(), out decimal price))
                    conditions.Add($"U_Price = {price}");

                // 5️⃣ المورد
                if (!string.IsNullOrEmpty(txtSuppliers.Text.Trim()))
                {
                    string escaped = txtSuppliers.Text.Trim().Replace("'", "''");
                    conditions.Add($"SuplierName LIKE '%{escaped}%'");
                }

                // 6️⃣ نطاق الأكواد
                string fromCode = txtFromCode.Text.Trim();
                string toCode = txtToCode.Text.Trim();
                if (!string.IsNullOrEmpty(fromCode) && !string.IsNullOrEmpty(toCode))
                    conditions.Add($"ProductCode >= {fromCode} AND ProductCode <= {toCode}");
                else if (!string.IsNullOrEmpty(fromCode))
                    conditions.Add($"ProductCode >= {fromCode}");
                else if (!string.IsNullOrEmpty(toCode))
                    conditions.Add($"ProductCode <= {toCode}");

                // تطبيق الشروط
                dv.RowFilter = string.Join(" AND ", conditions);

                DGV.DataSource = dv;
                ApplyDGVStyles();

                tblModify = dv.ToTable();
                UpdateCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء تطبيق الفلاتر: " + ex.Message);
            }
        }

        // 🔹 دالة مساعدة لجمع أبناء العقدة


        private List<int> CollectCategoryIds(TreeNode? parentNode)
        {
            List<int> ids = new List<int>();

            void Collect(TreeNode? node)
            {
                if (node == null)
                    return;

                if (node.Tag != null && int.TryParse(node.Tag.ToString(), out int id))
                    ids.Add(id);

                foreach (TreeNode child in node.Nodes)
                    Collect(child);
            }

            Collect(parentNode);
            return ids;
        }


        // 🔹 حدث اختيار العقدة من الشجرة
        private void treeViewCategories_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                // 📌 إلغاء البحث بالرينج
                txtFromCode.Text = string.Empty;
                txtToCode.Text = string.Empty;

                TreeNode selectedNode = e?.Node ?? treeViewCategories.SelectedNode;

                // لو مفيش عقدة مختارة
                if (selectedNode == null)
                {
                    lastSelectedNode = null;
                    SetCategoryDisplay(string.Empty); // تنظيف النص
                    ApplyAllFilters(); // فلترة عامة بدون عقدة
                    return;
                }

                // تخزين العقدة المختارة
                lastSelectedNode = selectedNode;

                // تحديث عرض اسم التصنيف
                SetCategoryDisplay(selectedNode.Text);

                // تطبيق كل الفلاتر (بحث + سعر + مورد + أكواد + شجرة)
                ApplyAllFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء اختيار التصنيف: " + ex.Message);
            }
        }

        private void treeViewCategories_AfterSelect_(object sender, TreeViewEventArgs e)
        {/*اريد عند الضغط هنا الغاء البحث بالرينج
          وتفريغ txtFromCode;txtToCode ;
          */
            try
            {
                TreeNode selectedNode = e?.Node ?? treeViewCategories.SelectedNode;

                // لو مفيش عقدة مختارة
                if (selectedNode == null)
                {
                    lastSelectedNode = null;
                    SetCategoryDisplay(string.Empty); // تنظيف النص
                    ApplyAllFilters(); // فلترة عامة بدون عقدة
                    return;
                }

                // تخزين العقدة المختارة
                lastSelectedNode = selectedNode;

                // تحديث عرض اسم التصنيف
                SetCategoryDisplay(selectedNode.Text);

                // تطبيق كل الفلاتر (بحث + سعر + مورد + أكواد + شجرة)
                ApplyAllFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء اختيار التصنيف: " + ex.Message);
            }
        }

        #endregion








        #region *********  فلترة مستقلة بالرينج ولكن ملتزمة بالارصدة وتلغى كل الفلترة الاخرى ******

        private void txtFromCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string code = txtFromCode.Text;
                ClearAllSearchFieldsExceptRange();
                txtFromCode.Text = code;

                ApplyAllFilters(); // ✅ هي اللي هتطبق نطاق الأكواد كجزء من الفلترة
                txtToCode.Focus();
                txtToCode.SelectAll();
            }
        }

        private void txtToCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string code = txtToCode.Text;
                ClearAllSearchFieldsExceptRange();
                txtToCode.Text = code;

                ApplyAllFilters(); // ✅
                txtFromCode.Focus();
                txtFromCode.SelectAll();
            }
        }

        private void ClearAllSearchFieldsExceptRange()
        {
            txtSeaarchProd.Text = string.Empty;
            txtSeaarchProdPrice.Text = string.Empty;
            txtSuppliers.Text = string.Empty;

            // إلغاء اختيار العقدة
            treeViewCategories.SelectedNode = null;
            lblSelectedTreeNod.Text = string.Empty;

            LoadProducts();
            // إغلاق كل التفرعات
            foreach (TreeNode node in treeViewCategories.Nodes)
                node.Collapse(true);

            ApplyAllFilters(); // ✅ دالة جديدة بدل FilterProductsBySearchText
        }

        #endregion


        #region   ********  فلترة بالبحث العادى مع السعر والمورد والتصنيف وتلغى الفلترة بالرينج ****

        private void txtSeaarchProd_TextChanged(object sender, EventArgs e)
        {
            txtFromCode.Text = "";
            txtToCode.Text = "";
            ApplyAllFilters();
        }



        #endregion 













        #region  طريقة العرض والبحث والفلترة


        // 🔹 فلترة حسب العقدة والراديو
        private void ApplyCategoryFilter(TreeNode? selectedNode)
        {
            try
            {
                if (selectedNode == null)
                {
                    LoadProducts(); // في حالة لم يتم اختيار عقدة
                    return;
                }

                string category = selectedNode.Text;

            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء تصفية التصنيفات: " + ex.Message);
            }
        }


        // 🔹 إعادة تنفيذ فلترة الشجرة عند تغيير اختيار العرض
        private void rdoByNode_CheckedChanged(object sender, EventArgs e)
        {
            TriggerTreeViewSelection();
            UpdateRadioButtonColors();
        }

        private void rdoByNodeAndHisChild_CheckedChanged(object sender, EventArgs e)
        {
            TriggerTreeViewSelection();
            UpdateRadioButtonColors();
        }

        // 🔹 إعادة محاكاة اختيار العقدة (لما المستخدم يغير الراديو مثلًا)
        private void TriggerTreeViewSelection()
        {
            if (treeViewCategories.SelectedNode != null)
            {
                var args = new TreeViewEventArgs(treeViewCategories.SelectedNode);
                treeViewCategories_AfterSelect(treeViewCategories, args);
            }
        }




        // دالة مساعدة لتحديث اسم التصنيف الظاهر في الواجهة
        private void SetCategoryDisplay(string categoryName)
        {
            lblSelectedTreeNod.Text = categoryName;
            txtCategory.Text = categoryName;
        }

        // دالة مساعدة لتحميل كل المنتجات من جديد
        private void LoadAllProducts_()
        {

        }

        // جدول وسيط يحفظ نتيجة فلترة الشجرة
        private DataTable tblFilteredByTree = new DataTable();

        //Field 'frmProductItems.tblFilteredByTree' is never assigned to, and will always have its default value null


        // 🔎 البحث داخل نتيجة الشجرة فقط
        private void FilterProductsBySearchText()
        {
            try
            {
                DataTable baseTable = tblFilteredByTree ?? _tblProd ?? new DataTable();
                DataView dv = new DataView(baseTable);

                List<string> conditions = new List<string>();

                // 1. الكلمات الجزئية
                string searchText = txtSeaarchProd.Text.Trim();
                if (!string.IsNullOrEmpty(searchText))
                {
                    string[] words = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string word in words)
                    {
                        string escaped = word.Replace("'", "''");
                        conditions.Add($"(ProdName LIKE '%{escaped}%' OR NoteProduct LIKE '%{escaped}%')");
                    }
                }

                // 2. السعر
                string priceText = txtSeaarchProdPrice.Text.Trim();
                if (decimal.TryParse(priceText, out decimal price))
                {
                    conditions.Add($"U_Price = {price}");
                }

                // 3. المورد
                string supplierText = txtSuppliers.Text.Trim();
                if (!string.IsNullOrEmpty(supplierText))
                {
                    string escaped = supplierText.Replace("'", "''");
                    conditions.Add($"SuplierName LIKE '%{escaped}%'");
                }

                string rowFilter = string.Join(" AND ", conditions);
                dv.RowFilter = rowFilter;

                DGV.DataSource = dv;
                ApplyDGVStyles();

                // حفظ البيانات المفلترة للعمليات الأخرى
                tblModify = dv.ToTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء التصفية: " + ex.Message);
            }
        }

        #endregion








        #region  طريقة العرض والبحث والبحث المتداخل

        // جلب البيانات المفلترة
        private DataTable GetFilteredData()
        {
            DataTable result = new DataTable();
            result.Columns.Add("ID", typeof(int));
            result.Columns.Add("Name", typeof(string));

            DataGridView? sourceGrid = DGV;
            string idColumn = "ID_Product";
            string nameColumn = "ProdName";

            if (sourceGrid != null)
            {
                if (sourceGrid.SelectedRows.Count > 1)
                {
                    foreach (DataGridViewRow row in sourceGrid.SelectedRows)
                    {
                        if (!row.IsNewRow && row.Cells[idColumn].Value != null)
                        {
                            result.Rows.Add(
                                Convert.ToInt32(row.Cells[idColumn].Value),
                                row.Cells[nameColumn].Value?.ToString() ?? ""
                            );
                        }
                    }
                }
                else
                {
                    foreach (DataGridViewRow row in sourceGrid.Rows)
                    {
                        if (!row.IsNewRow && row.Cells[idColumn].Value != null)
                        {
                            result.Rows.Add(
                                Convert.ToInt32(row.Cells[idColumn].Value),
                                row.Cells[nameColumn].Value?.ToString() ?? ""
                            );
                        }
                    }
                }
            }

            return result;
        }



        #endregion

        #region ========== Search Prod ===========================

        private void txtSeaarchProd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.H)
            {
                ShowHelpForActiveControl();
            }
        }
        private void txtSeaarchProd_TextChanged_(object sender, EventArgs e)
        {
            txtFromCode.Text = "";
            txtToCode.Text = "";//مسح الرينج
            FilterProductsBySearchText();
            UpdateCount();
        }
        private void txtSeaarchProdPrice_TextChanged(object sender, EventArgs e)
        {
            txtSeaarchProd_TextChanged(this, EventArgs.Empty);
        }

        private void txtSeaarchProdSupplier_TextChanged(object sender, EventArgs e)
        {
            txtSeaarchProd_TextChanged(this, EventArgs.Empty);
        }
        private void txtCategories_TextChanged(object sender, EventArgs e)
        {
            txtSeaarchProd_TextChanged(this, EventArgs.Empty);
        }
        private void txtCategory_id_TextChanged(object sender, EventArgs e)
        {
            txtSeaarchProd_TextChanged(this, EventArgs.Empty);
        }

        private void btnAdvanceSearch_Click(object sender, EventArgs e)
        {
            if (tlpAdvanceSearch.Visible)
            {
                tlpAdvanceSearch.Visible = false;
                txtFromCode.Text = string.Empty;
                txtToCode.Text = string.Empty;
                LoadProducts();
            }
            else
            {
                tlpAdvanceSearch.Visible = true;
            }
        }

        //ضبط اجماليات الاصناف المفلترة
        private void UpdateCount()
        {
            try
            {
                if (DGV?.Rows == null || DGV.Rows.Count == 0)
                {
                    lblCountAndTotalStoc.Text = "عدد الأصناف : 0 وقيمتها : 0.00";
                    return;
                }

                int count = 0;
                decimal totalValue = 0;

                foreach (DataGridViewRow row in DGV.Rows)
                {
                    if (row.IsNewRow) continue;

                    count++;

                    decimal price = 0;
                    decimal stock = 0;

                    if (row.Cells["U_Price"].Value != null)
                        decimal.TryParse(row.Cells["U_Price"].Value.ToString(), out price);

                    if (row.Cells["ProductStock"].Value != null)
                        decimal.TryParse(row.Cells["ProductStock"].Value.ToString(), out stock);

                    totalValue += price * stock;
                }

                lblCountAndTotalStoc.Text = $"عدد الأصناف : {count} وقيمتها : {totalValue:N2}";
            }
            catch (Exception ex)
            {
                lblCountAndTotalStoc.Text = "خطأ في الحساب";
                MessageBox.Show("حدث خطأ أثناء حساب المخزون: " + ex.Message);
            }
        }

        // وظيفة الغاء عوامل البحث المتقدم عند اختيار الرينج من كود الى كود ###
        private void ClearSearch()
        {
            txtSeaarchProd.Text = string.Empty;
            txtSeaarchProdPrice.Text = string.Empty;
            txtFromCode.Text = string.Empty;
            txtToCode.Text = string.Empty;
            txtSuppliers.Text = string.Empty;

            // إعادة تحميل البيانات الأصلية بدون تصفية
            if (_tblProd != null)
            {
                DGV.DataSource = _tblProd;
                tblModify = _tblProd.Copy();
            }
            else
            {
                // إذا كانت _tblProd غير مهيأة، عيّن DataSource إلى null و tblModify إلى جدول فارغ
                DGV.DataSource = null;
                tblModify = new DataTable();
            }

            UpdateCount();
        }

        #endregion ---------------------------------------------

        #region ***********  خاص باضافة الاصناف  ********************

        // 🟦 متغيرات إعدادات
        private decimal MaxRateDiscount = 0m;  // خاص بنسبة الاوكازيون 
        private decimal SalesPercentage = 0m;  // خاص بنسب سعر البيع
        // 🔍  تحميل الإعدادات
        private void LoadItemSettings()
        {
            try
            {
                // 🟦 قراءة القيم من ملف الإعدادات
                MaxRateDiscount = AppSettings.GetDecimal("MaxRateDiscount", 0.10m); // 10% افتراضياً
                SalesPercentage = AppSettings.GetDecimal("SalesPercentage", 0.10m); // 10% افتراضياً

                // 🟦 عند التحميل توضع الاعدادات فى اماكنها
                txtMaxRateDiscount.Text = MaxRateDiscount.ToString();
                txtSalesPercentage.Text = SalesPercentage.ToString();

            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowWarning($"خطأ أثناء تحميل إعدادات الفاتورة:\n{ex.Message}", "خطأ");
            }
        }

        // 🔹 حدث Leave للتحقق من الاسم
        private void txtNewItemSuppliers_Leave(object sender, EventArgs e)
        {
            if (tblSupplier == null) return;

            string selectedName = txtNewItemSuppliers.Text.Trim();

            // السماح بترك الحقل فارغًا
            if (string.IsNullOrEmpty(selectedName))
            {
                lblSuppliersID.Text = "";
                return;
            }

            // البحث عن المورد
            DataRow[] matched = tblSupplier.Select($"AccName = '{selectedName.Replace("'", "''")}'");

            if (matched.Length > 0)
            {
                lblSuppliersID.Text = matched[0]["AccID"].ToString();
            }
            else
            {
                MessageBox.Show("الاسم الذي أدخلته غير موجود في قائمة الموردين.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewItemSuppliers.Focus();
                txtNewItemSuppliers.SelectAll();
                lblSuppliersID.Text = "";
            }
        }


        private void txtNewItemSuppliers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)// للانتقال
            {
                e.SuppressKeyPress = true;
                txtProdCodeOnSuplier.Focus();
            }

            if (e.Control && e.KeyCode == Keys.H) // لفتح المساعدة
            {
                ShowHelpForActiveControl();
            }

            if (e.Control && e.KeyCode == Keys.F)// Ctrl + F لفتح شاشة البحث
            {
                var provider = new GenericSearchProvider(SearchEntityType.Accounts, AccountKind.Suppliers);
                var result = SearchHelper.ShowSearchDialog(provider);

                if (!string.IsNullOrEmpty(result.Code))
                {
                    lblSuppliersID.Text = result.Code;
                    txtNewItemSuppliers.Text = result.Name;

                }
            }

        }


        private void txtB_Price_TextChanged(object sender, EventArgs e)
        {
            // ✅ التحقق من صحة المدخلات
            if (!decimal.TryParse(txtB_Price.Text, out decimal bPrice))
                bPrice = 0;

            if (!decimal.TryParse(txtSalesPercentage.Text, out decimal salesPercentage))
                salesPercentage = 0;

            if (!decimal.TryParse(txtMaxRateDiscount.Text, out decimal maxRateDiscount))
                maxRateDiscount = 0;

            // 🔹 حساب سعر البيع
            // لو salesPercentage = 0.35 => يعني 35%
            decimal uPrice = bPrice + (salesPercentage * bPrice);
            txtU_Price.Text = uPrice.ToString("0.00");

            // 🔹 نسبة الزيادة في الليبل
            lblU_PricePercentage.Text = $"{Math.Round(salesPercentage * 100, 0)}%";

            // 🔹 حساب السعر بعد الخصم
            decimal dPrice = uPrice - (maxRateDiscount * uPrice);
            txtD_Price.Text = dPrice.ToString("0.00");

            // 🔹 نسبة الخصم في الليبل
            lblD_PricePercentage.Text = $"{Math.Round(maxRateDiscount * 100, 0)}%";
        }
        private void txtU_Price_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtB_Price.Text, out decimal bPrice))
                bPrice = 0;
            if (!decimal.TryParse(txtU_Price.Text, out decimal uPrice))
                uPrice = 0;

            if (bPrice > 0)
            {
                // نسبة الزيادة = (سعر البيع - سعر الشراء) / سعر الشراء
                decimal salesPercentage = (uPrice - bPrice) / bPrice;
                lblU_PricePercentage.Text = $"{Math.Round(salesPercentage * 100, 0)}%";
            }
            else
            {
                lblU_PricePercentage.Text = "0%";
            }
        }

        private void txtD_Price_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtU_Price.Text, out decimal uPrice))
                uPrice = 0;
            if (!decimal.TryParse(txtD_Price.Text, out decimal dPrice))
                dPrice = 0;

            if (uPrice > 0)
            {
                // نسبة الخصم = (سعر البيع - سعر بعد الخصم) / سعر البيع
                decimal discountPercentage = (uPrice - dPrice) / uPrice;
                lblD_PricePercentage.Text = $"{Math.Round(discountPercentage * 100, 0)}%";
            }
            else
            {
                lblD_PricePercentage.Text = "0%";
            }
        }

        private void txtSalesPercentage_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtB_Price.Text, out decimal bPrice))
                bPrice = 0;
            if (!decimal.TryParse(txtSalesPercentage.Text, out decimal salesPercentage))
                salesPercentage = 0;
            if (!decimal.TryParse(txtMaxRateDiscount.Text, out decimal maxRateDiscount))
                maxRateDiscount = 0;

            // 🔹 حساب سعر البيع
            decimal uPrice = bPrice + (salesPercentage * bPrice);
            txtU_Price.Text = uPrice.ToString("0.00");

            // 🔹 نسبة الزيادة
            lblU_PricePercentage.Text = $"{Math.Round(salesPercentage * 100, 0)}%";

            // 🔹 حساب السعر بعد الخصم
            decimal dPrice = uPrice - (maxRateDiscount * uPrice);
            txtD_Price.Text = dPrice.ToString("0.00");

            // 🔹 نسبة الخصم
            lblD_PricePercentage.Text = $"{Math.Round(maxRateDiscount * 100, 0)}%";
        }

        private void txtMaxRateDiscount_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtB_Price.Text, out decimal bPrice))
                bPrice = 0;
            if (!decimal.TryParse(txtSalesPercentage.Text, out decimal salesPercentage))
                salesPercentage = 0;
            if (!decimal.TryParse(txtMaxRateDiscount.Text, out decimal maxRateDiscount))
                maxRateDiscount = 0;

            // 🔹 حساب سعر البيع
            decimal uPrice = bPrice + (salesPercentage * bPrice);
            txtU_Price.Text = uPrice.ToString("0.00");

            // 🔹 نسبة الزيادة
            lblU_PricePercentage.Text = $"{Math.Round(salesPercentage * 100, 0)}%";

            // 🔹 حساب السعر بعد الخصم
            decimal dPrice = uPrice - (maxRateDiscount * uPrice);
            txtD_Price.Text = dPrice.ToString("0.00");

            // 🔹 نسبة الخصم
            lblD_PricePercentage.Text = $"{Math.Round(maxRateDiscount * 100, 0)}%";
        }


        private void FillUnits()
        {
            DataTable dt = DBServiecs.ProductGetUnits();

            cbxUnit_ID.DataSource = dt;
            cbxUnit_ID.DisplayMember = "UnitProd";
            cbxUnit_ID.ValueMember = "Unit_ID";

            // منع الكتابة داخل ComboBox
            cbxUnit_ID.DropDownStyle = ComboBoxStyle.DropDownList;

            // تحديد العنصر الثاني كمحدد افتراضي (إذا كان موجودًا)
            if (cbxUnit_ID.Items.Count > 1)
                cbxUnit_ID.SelectedIndex = 1;
        }

        // الحدث الذي يتغير فيه الاختيار داخل ComboBox
        private void cbxUnit_ID_SelectedIndexChanged(object sender, EventArgs e)
        {

            // التأكد من وجود قيمة صالحة
            if (cbxUnit_ID.SelectedValue == null || !int.TryParse(cbxUnit_ID.SelectedValue.ToString(), out int unitId))
                return;

            if (unitId == 1)
            {
                txtMinLenth.ReadOnly = false;
                lblMinLenth.ForeColor = Color.DarkBlue; // اللون الطبيعي
            }
            else
            {
                txtMinLenth.ReadOnly = true;
                txtMinLenth.Text = "0";
                lblMinLenth.ForeColor = Color.Gray; // لون باهت للدلالة على التعطيل

            }
        }

        private int UnitID;
        private float B_Price;
        private float U_Price;
        private float D_Price;
        private float MinLenth;
        private float MinStock;
        private int Category_id;
        private int SuplierID;


        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                GetData();

                // التحقق من صحة البيانات المدخلة
                if (!ValidateInputs())
                {
                    return;
                }

                int result = 0;

                result = DBServiecs.Product_InsertItem(ProdName, UnitID, B_Price, U_Price, D_Price, ProdCodeOnSuplier,
                                            MinLenth, MinStock, Category_id, SuplierID, Note_Prod, picProductPath);

                if (result > 0)
                {
                    ResetFormForNewEntry();
                    if (result > 0)
                    {
                        MessageBox.Show("تم إضافة الصنف بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ResetFormForNewEntry(); // تفريغ النموذج لإدخال صنف جديد

                        // 🔹 إضافة الصنف الجديد إلى الجدول المؤقت
                        DataRow newRow = tempAddedItems.NewRow();
                        newRow["ProdName"] = ProdName;
                        newRow["U_Price"] = U_Price;
                        tempAddedItems.Rows.Add(newRow);

                        LoadProducts(); // إعادة تحميل المنتجات من القاعدة (اختياري)
                    }


                    LoadProducts();
                    // ✅ مسح التحديد بعد الإضافة
                    DGV_AddItem.ClearSelection();
                }
                else
                {
                    MessageBox.Show("فشل في إضافة الصنف", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء حفظ البيانات: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void GetData()
        {
            try
            {
                ProdName = txtProdName.Text.Trim();
                Note_Prod = txtNoteProduct.Text.Trim();
                UnitID = cbxUnit_ID.SelectedValue == null ? 0 : Convert.ToInt32(cbxUnit_ID.SelectedValue);
                B_Price = float.TryParse(txtB_Price.Text, out float bPrice) ? bPrice : 0f;
                U_Price = float.TryParse(txtU_Price.Text, out float uPrice) ? uPrice : 0f;
                D_Price = float.TryParse(txtD_Price.Text, out float dPrice) ? dPrice : 0f;

                ProdCodeOnSuplier = txtProdCodeOnSuplier.Text.Trim();
                MinLenth = float.TryParse(txtMinLenth.Text, out float minL) ? minL : 0f;
                MinStock = float.TryParse(txtMinStock.Text, out float minS) ? minS : 0f;
                Category_id = int.TryParse(txtCategory.Text, out int catId) ? catId : 0;
                SuplierID = int.TryParse(txtNewItemSuppliers.Text, out int supId) ? supId : 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ غير متوقع: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtProdName.Text))
            {
                MessageBox.Show("يجب إدخال اسم الصنف", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProdName.Focus();
                return false;
            }

            if (cbxUnit_ID.SelectedValue == null)
            {
                MessageBox.Show("يجب اختيار وحدة القياس", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbxUnit_ID.Focus();
                return false;
            }

            if (!float.TryParse(txtB_Price.Text, out _))
            {
                MessageBox.Show("سعر الشراء غير صالح", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtB_Price.Focus();
                return false;
            }

            if (!float.TryParse(txtU_Price.Text, out _))
            {
                MessageBox.Show("سعر البيع غير صالح", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtU_Price.Focus();
                return false;
            }

            // يمكن إضافة تحقق من باقي الحقول حسب الحاجة

            return true;
        }
        private void ResetFormForNewEntry()
        {
            txtProdName.Clear();
            txtNoteProduct.Clear();
            txtB_Price.Text = "0";
            txtU_Price.Text = "0";
            txtProdCodeOnSuplier.Clear();
            txtProdName.Focus();

        }

        // اضافة الاصناف المضافة بشكل مؤقت
        private DataTable tempAddedItems = new DataTable();
        private void InitializeTempDGV()
        {
            // إضافة الأعمدة
            tempAddedItems.Columns.Add("ProdName", typeof(string));
            tempAddedItems.Columns.Add("U_Price", typeof(decimal));

            DGV_AddItem.DataSource = tempAddedItems;

            // عناوين الأعمدة
            DGV_AddItem.Columns[0].HeaderText = "اسم الصنف";
            DGV_AddItem.Columns[1].HeaderText = "سعر الوحدة";

            // منع إضافة صفوف جديدة
            DGV_AddItem.AllowUserToAddRows = false;

            // ضبط الأعمدة لتملأ الجريد بالكامل
            DGV_AddItem.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DGV_AddItem.Columns[0].FillWeight = 80;
            DGV_AddItem.Columns[1].FillWeight = 20;

            // جعل الجريد للقراءة فقط
            DGV_AddItem.ReadOnly = true;

            // منع تحديد الخلايا
            DGV_AddItem.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGV_AddItem.MultiSelect = false;

            // ✅ مسح التحديد بعد الإضافة
            DGV_AddItem.ClearSelection();

            // تلوين الصفوف
            DGV_AddItem.RowsDefaultCellStyle.BackColor = Color.LightGray;
            DGV_AddItem.RowsDefaultCellStyle.ForeColor = Color.Black;
            DGV_AddItem.AlternatingRowsDefaultCellStyle.BackColor = Color.LightCyan;

            // تعطيل التفاعل مع الجريد (اختياري)
            DGV_AddItem.Enabled = false;
        }




        private void btnLoadPicProduct_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(lblID_Product.Text, out int productId))
            {
                MessageBox.Show("⚠️ لم يتم تحديد الصنف.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "اختر صور المنتج";
                ofd.Filter = "ملفات الصور (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
                ofd.Multiselect = true; // ✅ السماح باختيار أكثر من صورة

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    bool isFirstImage = true;

                    foreach (string filePath in ofd.FileNames)
                    {
                        try
                        {
                            // ✅ إضافة الصورة في قاعدة البيانات
                            DBServiecs.Product_AddPhoto(
                                productId,
                                filePath,
                                isFirstImage // أول صورة مضافة ستكون افتراضية
                            );

                            isFirstImage = false; // باقي الصور ليست افتراضية
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"حدث خطأ أثناء إضافة الصورة:\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    MessageBox.Show("✅ تم إضافة الصور بنجاح.", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // ✅ تحديث الصورة الظاهرة في PictureBox
                    if (ofd.FileNames.Length > 0)
                    {
                        //PicProduct.Image = Image.FromFile(ofd.FileNames[0]);
                        //PicProduct.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
            }
        }





        private void txtProdName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtNoteProduct.Focus();
            }
        }

        private void txtNoteProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtNewItemSuppliers.Focus();
            }
        }

        // الحدث الذي يتعامل مع ضغطات المفاتيح داخل txtNewItemSuppliers

        private void txtProdCodeOnSuplier_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtCategory.Focus();
            }
        }

        private void txtCategory_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtB_Price.Focus();
            }

            if (e.Control && e.KeyCode == Keys.H)
            {
                HelpTextReader.ShowHelpForControl(this, sender);
                e.SuppressKeyPress = true;
            }

            if (e.Control && e.KeyCode == Keys.F)
            {
                e.SuppressKeyPress = true;

                // فتح frmCatTree في وضع SelectCategory فقط
                using (var frm = new frmCatTree(frmCatTree.FrmCatTreeMode.SelectCategory))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        // تعطيل أي حدث Leave مؤقتًا
                        txtCategory.Leave -= txtCategory_Leave;

                        // تعيين الاسم والرقم مباشرة
                        txtCategory.Text = frm.SelectedCategoryName ?? string.Empty;
                        lblCategoryID.Text = frm.SelectedCategoryID.ToString();

                        // إعادة تفعيل الحدث
                        txtCategory.Leave += txtCategory_Leave;
                    }
                }

            }

        }

        private void txtB_Price_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtU_Price.Focus();
            }
        }
        private void txtU_Price_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtD_Price.Focus();
            }
        }
        private void txtD_Price_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                cbxUnit_ID.Focus();
                cbxUnit_ID.DroppedDown = true; // 🔹 فتح القائمة تلقائيًا
            }
        }

        private void cbxUnit_ID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                // 🔹 تحقق من القيمة المختارة
                if (cbxUnit_ID.SelectedValue != null && cbxUnit_ID.SelectedValue.ToString() == "1")
                {
                    txtMinLenth.Focus();
                }
                else
                {
                    txtMinStock.Focus();
                }
            }
        }

        private void txtMinLenth_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtMinStock.Focus();
            }
        }

        private void txtMinStock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtProdName.Focus();
            }
        }






        private void txtCategory_Leave(object? sender, EventArgs e)
        {
            if (tblCategory == null) return;

            string selectedName = txtCategory.Text.Trim();

            DataRow[] matched = tblCategory.Select($"CategoryName = '{selectedName.Replace("'", "''")}'");
            if (matched.Length > 0)
            {
                lblCategoryID.Text = matched[0]["CategoryID"].ToString();
            }
            else
            {
                lblCategoryID.Text = "";
            }
        }

        private void picProd_Click(object sender, EventArgs e)
        {
            if (int.TryParse(lblID_Product.Text, out int id))
            {
                var viewer = new frmImageViewer(id);
                viewer.ShowDialog();
                // ✅ 1- إعادة تحميل المنتجات
                LoadProducts();
            }
            /*فى شاشة الاصناف
             يتم فتح frmImageViewer لعرض صور الصنف 
            اريد بعد غلق frmImageViewer يتم تحديث بيانات الصنف المختار وتغيير الصورة الى الصورة الافتراضية 
            ويقف على السطر المحدد من قبل
             */
        }

        /*
         private void DGV_SelectionChanged(object sender, EventArgs e)
        {
            if (!isFormLoaded) return; // تجاهل الحدث إذا لم يتم تحميل الفورم بعد

            if (DGV.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = DGV.SelectedRows[0];

                object? idValue = selectedRow.Cells["ID_Product"]?.Value;
                object? codeValue = selectedRow.Cells["ProductCode"]?.Value;
                object? imagePath = selectedRow.Cells["PicProduct"]?.Value;
                object? registYear = selectedRow.Cells["RegistYear"]?.Value;
                object? noteProd = selectedRow.Cells["NoteProduct"]?.Value;

                lblID_Product.Text = idValue?.ToString() ?? string.Empty;
                lblProductCode.Text = codeValue?.ToString()?.Trim() ?? string.Empty;
                lblRegist_Year.Text = registYear?.ToString() ?? string.Empty;
                lblNoteProduct.Text = noteProd?.ToString() ?? string.Empty;

                // تحميل الصورة إن وجدت
                if (imagePath != null)
                {
                    string path = imagePath.ToString()!;
                    if (File.Exists(path))
                    {
                        picProd.Image = Image.FromFile(path);
                    }
                    else
                    {
                        picProd.Image = ImageHelper.CreateTextImage("الصورة غير متوفرة", picProd.Width, picProd.Height);
                    }
                }
                else
                {
                    picProd.Image = ImageHelper.CreateTextImage("الصورة غير متوفرة", picProd.Width, picProd.Height);
                }


                GenerateBarcode();
            }
        }

         */


        #endregion




        private void btnClearDGV_Click(object sender, EventArgs e)
        {
            tempAddedItems.Clear();
        }

    }
}
