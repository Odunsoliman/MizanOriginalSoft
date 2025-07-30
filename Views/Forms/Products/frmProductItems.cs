
using Microsoft.CodeAnalysis;
using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses;
using MizanOriginalSoft.Views.Forms.MainForms;
using System.Data;
using System.Text;
using static MizanOriginalSoft.Views.Forms.MainForms.frmSearch;

namespace MizanOriginalSoft.Views.Forms.Products
{
    public partial class frmProductItems : Form
    {
        #region ===== Variables =====
        private DataTable? tblTree;
        private DataTable? tblSupplier;
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
        private DataTable? _tblProd = new();
        private DataTable? tblModify;
        private DataTable? dtProducts;
        private List<TreeNode> matchedNodes = new List<TreeNode>();
        private int currentMatchIndex = -1;

        #endregion
        public frmProductItems(int idUser)
        {
            InitializeComponent();
            ID_user = idUser;
            SetupAutoCompleteSuppliers();
            SetupAutoCompleteCategories();
            FillUnits();
            tblModify = new DataTable();
        }
        private void frmProductItems_Load(object sender, EventArgs e)
        {
            tblTree = DBServiecs.Categories_GetAll();
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
            LoadReports();//خاصة بقوائم التقارير
            DGV.ClearSelection();
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
        //تعبئة مربع الموردين تعبئة تلقائية  ###
        private void SetupAutoCompleteSuppliers()
        {
            // جلب جميع الموردين
            tblSupplier = DBServiecs.Accounts_GetSupplier();
            // إنشاء مجموعة الإكمال التلقائي
            AutoCompleteStringCollection autoCompleteCollection = new AutoCompleteStringCollection();//'new' expression can be simplified
            // التأكد من أن الجدول يحتوي على بيانات
            if (tblSupplier != null && tblSupplier.Rows.Count > 0)
            {
                // إضافة أسماء المستخدمين إلى مجموعة الإكمال التلقائي
                foreach (DataRow row in tblSupplier.Rows)
                {
                    string? accName = row["AccName"]?.ToString();
                    if (!string.IsNullOrEmpty(accName))
                    {
                        autoCompleteCollection.Add(accName);
                    }
                }
            }

            // إعداد خصائص مربع النص لاستخدام الإكمال التلقائي
            txtSuppliers.AutoCompleteCustomSource = autoCompleteCollection;
            txtSuppliers.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtSuppliers.AutoCompleteSource = AutoCompleteSource.CustomSource;

            // إعداد خصائص مربع النص لاستخدام الإكمال التلقائي
            txtNewItemSuppliers.AutoCompleteCustomSource = autoCompleteCollection;
            txtNewItemSuppliers.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtNewItemSuppliers.AutoCompleteSource = AutoCompleteSource.CustomSource;
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
            txtCategories.AutoCompleteCustomSource = autoCompleteCollection;
            txtCategories.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtCategories.AutoCompleteSource = AutoCompleteSource.CustomSource;

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

        //حدث تشيك بوكس يسمح او لا يسمح بتعديل ضم الاصناف الى تصنيف اخر  ###
        private void chkTreeEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTreeEnable.Checked)
            {
                // تفعيل الفلترة حسب العقدة المحددة
                TreeNode selectedNode = treeViewCategories.SelectedNode;
                if (selectedNode != null && selectedNode.Tag != null)
                {
                    int selectedCategoryId = Convert.ToInt32(selectedNode.Tag);

                    if (rdoByNode.Checked)
                    {
                        FilterProductsByCategory(selectedCategoryId);
                    }
                    else if (rdoByNodeAndHisChild.Checked)
                    {
                        FilterProductsByCategoryAndHisChild(selectedNode);
                    }
                }
            }
            else
            {
                // عرض كل المنتجات بدون فلترة
                LoadProducts();
                chkTreeEnable.ForeColor = Color.Red;
                chkTreeEnable.Text = "معطلة";
            }
        }
        private void lblAllTree_Click(object sender, EventArgs e)
        {
            frmProductItems_Load(this, EventArgs.Empty);

            ClearSearch(); if (tlpAdvanceSearch.Visible == true) tlpAdvanceSearch.Visible = false;
        }
        
        //تحميل شجرة التصنيفات  ###
        private void LoadTreeAndSelectSpecificNode(int selectedID = 0)
        {
            treeViewCategories.Nodes.Clear();

            DataTable dt = tblTree ?? new DataTable();

            foreach (DataRow row in dt.Rows)
            {
                if (row["ParentID"] == DBNull.Value || Convert.ToInt32(row["ParentID"]) == 0)
                {
                    TreeNode parentNode = new TreeNode(row["CategoryName"].ToString());//'new' expression can be simplified
                    parentNode.Tag = Convert.ToInt32(row["CategoryID"]);
                    treeViewCategories.Nodes.Add(parentNode);
                    AddChildNodes(dt, parentNode);
                }
            }

            if (selectedID > 0)
                SelectNodeById(treeViewCategories.Nodes, selectedID);
        }

        //تحميل الفروع داخل الشجرة  ###
        private void AddChildNodes(DataTable dt, TreeNode parentNode)
        {
            int parentId = Convert.ToInt32(parentNode.Tag);
            foreach (DataRow row in dt.Select($"ParentID = {parentId}"))
            {
                TreeNode childNode = new TreeNode(row["CategoryName"].ToString());//'new' expression can be simplified
                childNode.Tag = Convert.ToInt32(row["CategoryID"]);
                parentNode.Nodes.Add(childNode);
                AddChildNodes(dt, childNode);
            }
        }
        // وظيفة اختيار عقدة بواسطة رقم المعرف
        private void SelectNodeById(TreeNodeCollection nodes, int id)
        {
            foreach (TreeNode node in nodes)
            {
                if (Convert.ToInt32(node.Tag) == id)
                {
                    treeViewCategories.SelectedNode = node;
                    //treeViewCategories.Focus();
                    return;
                }

                if (node.Nodes.Count > 0)
                    SelectNodeById(node.Nodes, id);
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

        private void btnIncludeToCategory_Click(object sender, EventArgs e)
        {
            if (treeViewCategories.SelectedNode == null)
            {
                MessageBox.Show("الرجاء تحديد تصنيف من الشجرة أولاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DGV.SelectedRows.Count == 0)
            {
                MessageBox.Show("الرجاء تحديد صنف واحد على الأقل من الجدول.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("هل تريد تضمين الأصناف المحددة إلى التصنيف المحدد؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            int selectedCategoryId = Convert.ToInt32(treeViewCategories.SelectedNode.Tag);

            // إنشاء أو تهيئة DataTable
            if (dtProducts == null)
                dtProducts = new DataTable();
            else
                dtProducts.Clear();

            if (!dtProducts.Columns.Contains("ID_Product"))
                dtProducts.Columns.Add("ID_Product", typeof(int));

            // إضافة المنتجات المحددة
            foreach (DataGridViewRow row in DGV.SelectedRows)
            {
                int productId = Convert.ToInt32(row.Cells["ID_Product"].Value);
                dtProducts.Rows.Add(productId);
            }

            // استدعاء التحديث
            DBServiecs.Product_UpdateCategory(dtProducts, selectedCategoryId, ID_user);

            // تحديث العرض
            LoadProducts();
            LoadTreeAndSelectSpecificNode();
            txtSeaarchProd_TextChanged(this, EventArgs.Empty);
        }

        // بدء عملية السحب
        private void treeViewCategories_ItemDrag(object? sender, ItemDragEventArgs e)
        {
            if (e.Item != null)
            {
                DoDragDrop(e.Item, DragDropEffects.Move);
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

        // عند إسقاط العنصر داخل TreeView
        private void treeViewCategories_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetData(typeof(TreeNode)) is TreeNode draggedNode)
            {
                Point targetPoint = treeViewCategories.PointToClient(new Point(e.X, e.Y));
                TreeNode? targetNode = treeViewCategories.GetNodeAt(targetPoint);

                if (targetNode != null && !draggedNode.Equals(targetNode) && !ContainsNode(draggedNode, targetNode))
                {
                    // إزالة العنصر من موقعه القديم
                    draggedNode.Remove();

                    // إضافته إلى العقدة الجديدة
                    targetNode.Nodes.Add(draggedNode);
                    targetNode.Expand();

                    // تحديث البيانات حسب الحاجة
                    if (draggedNode.Tag is int CategoryID && targetNode.Tag is int NewParentID)
                    {
                        // تنفيذ منطق التحديث في قاعدة البيانات هنا
                        // مثل: UpdateCategoryParent(CategoryID, NewParentID);
                    }
                }
            }
        }

        // عند إفلات العنصر داخل الشجرة

        //-----------------------------------------------------------
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

        //تحديث صورة الباركود
        private void GenerateBarcode()
        {
            string productCode = lblProductCode.Text.Trim();

            // توليد باركود صغير الحجم
            var barcodeImage = BarcodeGenerator.Generate(productCode, width: 100, height: 30, margin: 1);

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



        #region ========== Search Prod ===========================
        private void txtSeaarchProd_TextChanged(object sender, EventArgs e)
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
        private void txtSuppliers_TextChanged(object sender, EventArgs e)
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
        private void FilterProductsBySearchText()
        {
            DataView dv = new DataView(_tblProd);  // عرفه خارج try

            try
            {
                List<string> conditions = new List<string>();

                // 1. الكلمات الجزئية في اسم المنتج
                string searchText = txtSeaarchProd.Text.Trim();
                if (!string.IsNullOrEmpty(searchText))
                {
                    string[] words = searchText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string word in words)
                    {
                        string escaped = word.Replace("'", "''");
                        conditions.Add($"ProdName LIKE '%{escaped}%'");
                    }
                }

                // 2. السعر
                string priceText = txtSeaarchProdPrice.Text.Trim();
                if (decimal.TryParse(priceText, out decimal price))
                {
                    conditions.Add($"U_Price = {price}");
                }

                // 3. اسم المورد
                string supplierText = txtSuppliers.Text.Trim();
                if (!string.IsNullOrEmpty(supplierText))
                {
                    string escaped = supplierText.Replace("'", "''");
                    conditions.Add($"SuplierName LIKE '%{escaped}%'");
                }

                // 4. رقم الفئة (الإضافة الجديدة)
                string categoryIdText = txtCategories.Text.Trim();
                if (!string.IsNullOrEmpty(categoryIdText))
                {
                    string escaped = categoryIdText.Replace("'", "''");
                    conditions.Add($"CategoryName LIKE '%{escaped}%'");
                }

                string rowFilter = string.Join(" AND ", conditions);
                dv.RowFilter = rowFilter;

                DGV.DataSource = dv;

                // نسخ البيانات المفلترة
                tblModify = dv.ToTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء التصفية: " + ex.Message);
            }
        }
        private void txtFromCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string code = txtFromCode.Text;
                // 1. مسح كل حقول البحث الأخرى
                ClearAllSearchFieldsExceptRange();
                txtFromCode.Text = code;
                // 2. تطبيق تصفية نطاق الأكواد
                FilterByCodeRange();
                txtToCode.Focus();
                txtToCode.SelectAll();
            }
        }
        private void txtToCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string code = txtToCode.Text;
                // 1. مسح كل حقول البحث الأخرى
                ClearAllSearchFieldsExceptRange();
                txtToCode.Text = code;
                // 2. تطبيق تصفية نطاق الأكواد
                FilterByCodeRange();
                txtFromCode.Focus();
                txtFromCode.SelectAll();
            }
        }
        private void ClearAllSearchFieldsExceptRange()
        {
            txtSeaarchProd.Text = "";
            txtSeaarchProdPrice.Text = "";
        }
        private void FilterByCodeRange()
        {
            DataView dv = new DataView(_tblProd);

            try
            {
                string fromCode = txtFromCode.Text.Trim();
                string toCode = txtToCode.Text.Trim();

                string filter = "";

                if (!string.IsNullOrEmpty(fromCode) && !string.IsNullOrEmpty(toCode))
                {
                    filter = $"ProductCode >= {fromCode} AND ProductCode <= {toCode}";
                }
                else if (!string.IsNullOrEmpty(fromCode))
                {
                    filter = $"ProductCode >= {fromCode}";
                }
                else if (!string.IsNullOrEmpty(toCode))
                {
                    filter = $"ProductCode <= {toCode}";
                }

                dv.RowFilter = filter;
                DGV.DataSource = dv;
                tblModify = dv.ToTable();

                UpdateCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء التصفية حسب النطاق: " + ex.Message);
            }
        }      //اظهار واخفاء البحث المتقدم
        private void btnAdvanceSearch_Click(object sender, EventArgs e)
        {
            tlpAdvanceSearch.Visible = !tlpAdvanceSearch.Visible;
            ClearSearch();
        }
        //تعبئة كمبوبكس الموردين

        //تحديث عدد الاصناف فى DGV  ###
        private void UpdateCount()

        {
            lblCount.Text = DGV?.RowCount.ToString() ?? "0";
        }


        // أضف هذا الحدث للتعامل مع ضغط المفاتيح

        #endregion ---------------------------------------------




        #region تعديل صنف أو مجموعة أصناف

        private void btnModifyItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (DGV.Columns["ID_Product"] == null)
                {
                    MessageBox.Show("لا يوجد عمود ID_Product في الجدول", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

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

                int? currentProductId = null;
                if (DGV.CurrentRow != null && DGV.CurrentRow.Cells["ID_Product"].Value != null)
                    currentProductId = Convert.ToInt32(DGV.CurrentRow.Cells["ID_Product"].Value);

                if (selectedProductIds.Count == 1) // تعديل فردي
                {
                    int productId = selectedProductIds[0];

                    using (frm_ProductModify frm = new frm_ProductModify(productId))
                    {
                        frm.StartPosition = FormStartPosition.CenterParent;
                        frm.ShowInTaskbar = false;

                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            RefreshData();

                            if (frm.CategoryID > 0)
                            {
                                ApplyCategorySelectionAndFilter(frm.CategoryID, productId);
                                LoadTreeAndSelectSpecificNode(frm.CategoryID);
                            }
                            else if (currentProductId.HasValue)
                            {
                                SelectProductAfterRefresh(currentProductId.Value);
                            }
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
                            RefreshData();

                            int catID = frm.CategoryID.HasValue ? frm.CategoryID.Value : 0;

                            if (catID > 0)
                            {
                                ApplyCategorySelectionAndFilter(catID, selectedProductIds[0]);
                                LoadTreeAndSelectSpecificNode(catID);
                            }
                            else
                            {
                                SelectProductAfterRefresh(selectedProductIds[0]);
                            }
                        }
                    }
                }
                LoadAllProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تنفيذ التعديل: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

                lblID_Product.Text = idValue?.ToString() ?? string.Empty;
                lblProductCode.Text = codeValue?.ToString()?.Trim() ?? string.Empty;
                lblRegist_Year.Text = registYear?.ToString() ?? string.Empty;

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
            if (DGV.DataSource is DataTable dataTable)
            {
                foreach (DataGridViewRow row in DGV.Rows)
                {
                    if (row.Cells["ID_Product"].Value != null &&
                        Convert.ToInt32(row.Cells["ID_Product"].Value) == productId)
                    {
                        row.Selected = true;

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

                        DGV.FirstDisplayedScrollingRowIndex = row.Index;
                        break;
                    }
                }
            }
        }
        #endregion

        #region تطبيق التصنيف وتحديث الفلترة بعد التعديل
        private void ApplyCategorySelectionAndFilter(int categoryId, int? productIdToSelect = null)
        {
            TreeNode[] nodes = treeViewCategories.Nodes.Find(categoryId.ToString(), true);
            if (nodes.Length > 0)
            {
                TreeNode selectedNode = nodes[0];
                treeViewCategories.SelectedNode = selectedNode;
                selectedNode.EnsureVisible();

                if (selectedNode.Tag != null)
                {
                    int selectedCategoryId = Convert.ToInt32(selectedNode.Tag);

                    if (selectedCategoryId == 0)
                    {
                        frmProductItems_Load(this, EventArgs.Empty);
                        lblSelectedTreeNod.Text = "الكل";
                    }
                    else
                    {
                        if (rdoByNode.Checked)
                            FilterProductsByCategory(selectedCategoryId);
                        else if (rdoByNodeAndHisChild.Checked)
                            FilterProductsByCategoryAndHisChild(selectedNode);
                    }

                    UpdateCount();

                    if (productIdToSelect.HasValue)
                        SelectProductAfterRefresh(productIdToSelect.Value);
                }
            }
        }
        #endregion



        private void RefreshData()
        {
            try
            {
                frmProductItems_Load(this, EventArgs.Empty); // أو دالة تحميل البيانات الخاصة بك
                DGV.Refresh();
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
        /// /////////// //////////////////////////////////////////


        // زر فك ارتباط الشجرة بالاصناف للتحكم فى ربط الاصناف بتصنيف اخر  ###
        private void btnMdifyTree_Click(object sender, EventArgs e)
        {
            chkTreeEnable.Checked = !chkTreeEnable.Checked;

            chkTreeEnable_CheckedChanged(this, EventArgs.Empty);
            if (chkTreeEnable.Checked)
            {
                btnMdifyTree.Text = "تعديل التصنيف";
                // لون الزر فى هذه الحالة (لون أخضر فاتح مع نص داكن)
                btnMdifyTree.BackColor = Color.LightCyan;
                btnMdifyTree.ForeColor = Color.DarkRed; // 
            }
            else
            {
                btnMdifyTree.Text = "ضم الاصناف للتصنيف";
                // لون الزر فى هذه الحالة 
                btnMdifyTree.BackColor = Color.FromArgb(255, 200, 200); // أحمر فاتح (لون وردي خفيف)
                btnMdifyTree.ForeColor = Color.DarkRed; // نص أحمر داكن

                // كود يتم تنفيذه عند الإلغاء

            }
        }

        private void txtFromCode_TextChanged(object sender, EventArgs e)
        {
            string code = txtFromCode.Text;
            // 1. مسح كل حقول البحث الأخرى
            ClearAllSearchFieldsExceptRange();
            txtFromCode.Text = code;
            // 2. تطبيق تصفية نطاق الأكواد
            FilterByCodeRange();

        }

        private void txtToCode_TextChanged(object sender, EventArgs e)
        {
            string code = txtFromCode.Text;
            // 1. مسح كل حقول البحث الأخرى
            ClearAllSearchFieldsExceptRange();
            txtFromCode.Text = code;
            // 2. تطبيق تصفية نطاق الأكواد
            FilterByCodeRange();
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

        // اعداد قوائم التقارير الخاصة بالاصناف  ###
        #region ######### New Report Tools  خاصة بقوائم التقارير ########

        // إنشاء شريط القوائم داخل الـ Panel ###
        private void SetupMenuStrip()
        {

            MenuStrip mainMenu = new MenuStrip();
            mainMenu.Dock = DockStyle.Fill;
            mainMenu.BackColor = Color.LightSteelBlue;
            // تعيين الخط المطلوب لشريط القوائم
            mainMenu.Font = new Font("Times New Roman", 14, FontStyle.Regular);

            // القائمة الأولى: تقارير الصنف
            tsmiCategoryReports = new ToolStripMenuItem("تقارير الصنف المحدد ▼");
            // القائمة الثانية: التقارير المجمعة
            tsmiGroupedReports = new ToolStripMenuItem("تقارير مجمعة للاصناف المحددة ▼");
            // إضافة القوائم إلى شريط القوائم
            mainMenu.Items.Add(tsmiCategoryReports);
            mainMenu.Items.Add(tsmiGroupedReports);

            // إضافة شريط القوائم إلى الـ Panel
            pnlMenuContainer.Controls.Add(mainMenu);
            mainMenu.Location = new Point(10, 5);

            // تكوين الـ DataGridView
            DGV.Dock = DockStyle.Fill;
        }

        // تحميل تقارير الفردية والمجمعة ###
        private void LoadReports()
        {
            try
            {
                // تحميل تقارير الصنف الفردية (ForItems = true)
                DataTable dtCategoryReports = DBServiecs.RepMenu_Products(true, false);

                LoadMenuItemsFromDataTable(tsmiCategoryReports, dtCategoryReports);

                // تحميل التقارير المجمعة للأصناف (ForItemsGroup = true)
                DataTable dtGroupedReports = DBServiecs.RepMenu_Products(false, true);

                LoadMenuItemsFromDataTable(tsmiGroupedReports, dtGroupedReports);
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء تحميل التقارير: " + ex.Message);
            }
        }

        // تحميل تقارير الصنف الفردية  ###
        private void LoadMenuItemsFromDataTable(ToolStripMenuItem parentMenu, DataTable? data)
        {
            // مسح العناصر القديمة إن وجدت
            parentMenu.DropDownItems.Clear();

            if (data == null || data.Rows.Count == 0)
            {
                ToolStripMenuItem emptyItem = new("لا توجد تقارير متاحة")
                {
                    Enabled = false
                };
                parentMenu.DropDownItems.Add(emptyItem);
                return;
            }

            // إضافة العناصر الجديدة
            foreach (DataRow row in data.Rows)
            {
                string reportName = row["ReportName"]?.ToString() ?? "تقرير غير معروف";
                object? reportID = row["ReportID"];

                ToolStripMenuItem menuItem = new(reportName)
                {
                    Tag = reportID
                };

                menuItem.Click += MenuItem_Click!;
                parentMenu.DropDownItems.Add(menuItem);
            }
        }


        // حدث النقر واستدعاء تقرير ما من القائمة  ###    
        private void MenuItem_Click(object sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem clickedItem || clickedItem.Tag is null)
            {
                MessageBox.Show("عنصر القائمة لا يحتوي على بيانات التقرير المطلوبة");
                return;
            }

            try
            {
                // إنشاء القاموس بما يتوافق مع نوع الدالة المطلوبة
                Dictionary<string, object> reportParameters = new()
        {
            { "ReportID", Convert.ToInt32(clickedItem.Tag) }, // كود التقرير
            { "ReportName", clickedItem.Text?.Trim() ?? string.Empty }, // اسم التقرير
            { "UserID", ID_user }, // كود المستخدم
            { "EntityID", DBNull.Value }, // سيتم تغييره لاحقًا
            { "FilteredData", new DataTable() } // سيتم تغييره لاحقًا
        };

                // تعبئة المعطيات الأساسية
                bool success = FillCommonParameters(reportParameters);

                // ✅ إيقاف تام إذا لم تنجح التعبئة (مثلاً لم يتم اختيار صنف)
                if (!success)
                    return;

                using frmReport_Preview previewForm = new(reportParameters);
                previewForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحضير التقرير: {ex.Message}");
            }
        }
        // دالة مبسطة لتعبئة المعطيات المشتركة ###
        private bool FillCommonParameters(Dictionary<string, object> parameters)
        {
            try
            {
                // 1. كود الكيان الرئيسي
                int? entityId = GetCurrentEntityID();
                if (entityId == null)
                    return false; // توقف تام

                parameters["EntityID"] = entityId.Value;

                // 2. البيانات المفلترة
                parameters["FilteredData"] = GetFilteredData();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تجهيز معطيات التقرير: {ex.Message}");
                return false;
            }
        }

  
        // جلب كود الصنف الحالى ###
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




        // جلب البيانات المفلتر عليها  ###
        private DataTable GetFilteredData()
        {
            DataTable result = new DataTable();
            result.Columns.Add("ID", typeof(int));
            result.Columns.Add("Name", typeof(string));

            // تحديد مصدر البيانات حسب الشاشة
            DataGridView? sourceGrid = DGV; // تأمين أنها غير null
            string idColumn = "ID_Product"; // يجب التعديل حسب اسم العمود الحقيقي
            string nameColumn = "ProdName"; // نفس الأمر

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
        // اعداد شجرة التصنيفات للاصناف  ###
        #region ####### Tree Methods ########3

        // عند تغيير اختيار عرض الأصناف حسب الفرع المحدد ###
        private void rdoByNode_CheckedChanged(object sender, EventArgs e)
        {
            TriggerTreeViewSelection();
            UpdateRadioButtonColors();
        }

        // عند تغيير اختيار عرض الأصناف حسب الفرع وأبنائه ###
        private void rdoByNodeAndHisChild_CheckedChanged(object sender, EventArgs e)
        {
            TriggerTreeViewSelection();
            UpdateRadioButtonColors();
        }

        // محاكي يدوي لاختيار العقدة الحالية في الشجرة
        private void TriggerTreeViewSelection()
        {
            if (treeViewCategories.SelectedNode != null)
            {
                var args = new TreeViewEventArgs(treeViewCategories.SelectedNode);
                treeViewCategories_AfterSelect(treeViewCategories, args);
            }
        }
        // الحدث الذي يتم تنفيذه عند تحديد عقدة في شجرة التصنيفات
        private void treeViewCategories_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                // إعادة تعيين البحث وعرض الفلاتر
                ClearSearch();
                tlpAdvanceSearch.Visible = false;

                // الحصول على العقدة المحددة
                TreeNode selectedNode = e?.Node ?? treeViewCategories.SelectedNode;

                // التأكد من صحة العقدة
                if (selectedNode == null)
                {
                    SetCategoryDisplay(string.Empty);
                    return;
                }

                // تحديث اسم التصنيف الظاهر
                SetCategoryDisplay(selectedNode.Text);

                // إذا كان خيار التصفية غير مفعل أو لا يوجد معرف في العقدة، الخروج
                if (!chkTreeEnable.Checked || selectedNode.Tag == null)
                    return;

                // تحويل القيمة المرتبطة بالعقدة إلى رقم
                if (!int.TryParse(selectedNode.Tag.ToString(), out int selectedCategoryId))
                    return;

                // في حالة اختيار "الكل" (وليس رقم 0 كما سابقًا بل 1)
                if (selectedCategoryId == 1)
                {
                    LoadAllProducts();
                    SetCategoryDisplay("الكل");
                }
                else
                {
                    // اختيار نوع التصفية بناءً على الاختيار
                    if (rdoByNode.Checked)
                        FilterProductsByCategory(selectedCategoryId);
                    else if (rdoByNodeAndHisChild.Checked)
                        FilterProductsByCategoryAndHisChild(selectedNode);
                }

                // تحديث عدد النتائج
                UpdateCount();

                // حفظ العقدة المحددة
                lastSelectedNode = selectedNode;
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء تصفية التصنيفات: " + ex.Message);
            }
        }

        // دالة مساعدة لتحديث اسم التصنيف الظاهر في الواجهة
        private void SetCategoryDisplay(string categoryName)
        {
            lblSelectedTreeNod.Text = categoryName;
            txtCategory.Text = categoryName;
        }

        // دالة مساعدة لتحميل كل المنتجات من جديد
        private void LoadAllProducts()
        {
            frmProductItems_Load(this, EventArgs.Empty);

        }

        // فلترة الأصناف حسب التصنيف فقط ###
        private void FilterProductsByCategory(int categoryId)
        {
            DataTable allProducts = _tblProd ?? new DataTable();
            if (allProducts.Rows.Count == 0)
            {
                DGV.DataSource = null;
                return;
            }

            var filtered = allProducts.AsEnumerable()
                .Where(r => (r.Field<int?>("Category_id") ?? 0) == categoryId);

            DGV.DataSource = filtered.Any() ? filtered.CopyToDataTable() : null;
            ApplyDGVStyles();
        }

        // فلترة الأصناف حسب التصنيف وجميع أبنائه ###
        private void FilterProductsByCategoryAndHisChild(TreeNode parentNode)
        {
            DataTable allProducts = _tblProd ?? new DataTable();
            if (allProducts.Rows.Count == 0 || parentNode == null)
            {
                DGV.DataSource = null;
                return;
            }

            List<int> categoryIds = new List<int>();

            void CollectCategoryIds(TreeNode node)
            {
                if (node?.Tag != null && int.TryParse(node.Tag.ToString(), out int id))
                {
                    categoryIds.Add(id);
                    foreach (TreeNode child in node.Nodes)
                        CollectCategoryIds(child);
                }
            }

            CollectCategoryIds(parentNode);

            var filtered = allProducts.AsEnumerable()
                .Where(r => categoryIds.Contains(r.Field<int?>("Category_id") ?? 0));

            DGV.DataSource = filtered.Any() ? filtered.CopyToDataTable() : null;
            ApplyDGVStyles();
        }

        #endregion

        // وظيفة الغاء عوامل البحث المتقدم واعادة تحميل البيانات ###
        #region ############ Search Methods ##########
        private void ClearSearch()
        {
            txtSeaarchProd.Text = string.Empty;
            txtSeaarchProdPrice.Text = string.Empty;
            txtFromCode.Text = string.Empty;
            txtToCode.Text = string.Empty;
            txtCategories.Text = string.Empty;
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



        #endregion

      

        #region @@@@@@@@@@@@@ New Item @@@@@@@@@@@@@@@@
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
                lblMinLenth.ForeColor = Color.DarkBlue ; // اللون الطبيعي
            }
            else
            {
                txtMinLenth.ReadOnly = true;
                txtMinLenth.Text = "0";
                lblMinLenth.ForeColor = Color.Gray; // لون باهت للدلالة على التعطيل

            }
        }

        private int ID_Product;

        private int UnitID;
        private float B_Price;
        private float U_Price;
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

                result = DBServiecs.Product_InsertItem(ProdName, UnitID, B_Price, U_Price, ProdCodeOnSuplier,
                                            MinLenth, MinStock, Category_id, SuplierID, Note_Prod, picProductPath);

                if (result > 0)
                {
                    MessageBox.Show("تم إضافة الصنف بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetFormForNewEntry(); // تفريغ النموذج لإدخال صنف جديد
                    LoadAllProducts();
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
                ID_Product = int.TryParse(lblID_NweProduct.Text, out int id) ? id : 0;
                ProdName = txtProdName.Text.Trim();
                Note_Prod = txtNoteProduct.Text.Trim();
                UnitID = cbxUnit_ID.SelectedValue == null ? 0 : Convert.ToInt32(cbxUnit_ID.SelectedValue);
                B_Price = float.TryParse(txtB_Price.Text, out float bPrice) ? bPrice : 0f;
                U_Price = float.TryParse(txtU_Price.Text, out float uPrice) ? uPrice : 0f;
                ProdCodeOnSuplier = txtProdCodeOnSuplier.Text.Trim();
                MinLenth = float.TryParse(txtMinLenth.Text, out float minL) ? minL : 0f;
                MinStock = float.TryParse(txtMinStock.Text, out float minS) ? minS : 0f;
                Category_id = int.TryParse(txtCategory.Text, out int catId) ? catId : 0;
                SuplierID = int.TryParse(txtNewItemSuppliers.Text, out int supId) ? supId : 0;
                picProductPath = lblPathProductPic.Text.Trim();
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
            txtMinLenth.Text = "0";
            txtMinStock.Text = "0";
            txtCategory.Text = "";
            txtNewItemSuppliers.Text = "";
            cbxUnit_ID.SelectedIndex = -1;
            lblPathProductPic.Text = "..";
            txtProdName.Focus();
            PicProduct.Image = null;

        }



        private void btnLoadPicProduct_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "اختر صورة المنتج";
                ofd.Filter = "ملفات الصور (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // عرض الصورة في الـ PictureBox
                    PicProduct.Image = Image.FromFile(ofd.FileName);
                    PicProduct.SizeMode = PictureBoxSizeMode.StretchImage;

                    // عرض المسار في الليبل
                    lblPathProductPic.Text = ofd.FileName;
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
        private void txtNewItemSuppliers_KeyDown(object sender, KeyEventArgs e)
        {
            // عند الضغط على Enter يتم الانتقال إلى حقل كود المنتج لدى المورد
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtProdCodeOnSuplier.Focus();
            }

            if (e.Control && e.KeyCode == Keys.H)
            {
                ShowHelpForActiveControl();
            }

            // عند الضغط على Ctrl + F يتم فتح شاشة البحث عن الموردين
            if (e.Control && e.KeyCode == Keys.F)
            {

                e.SuppressKeyPress = true;

                // تحديد نوع الحساب (مثلاً 14 يمثل الموردين)
                int typeId = 14;

              //  فتح نموذج البحث العام
                frmSearch searchForm = new frmSearch(14, SearchEntityType.Supplier);

                if (searchForm.ShowDialog() == DialogResult.OK)
                {
                    // إذا اختار المستخدم موردًا من نتيجة البحث
                    txtNewItemSuppliers.Text = searchForm.SelectedName;
                    lblSuppliersID.Text = searchForm.SelectedID;

                    // (اختياري) تحميل بيانات إضافية للمورد المختار إن احتجت
                    DataTable result = DBServiecs.MainAcc_GetAccounts(typeId);
                    if (result != null && result.Rows.Count > 0)
                    {
                        // هنا يمكنك استخدام بيانات إضافية إذا لزم الأمر
                    }
                }
            }
        }

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
                txtU_Price.Focus();
            }
        }

        private void txtU_Price_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtB_Price.Focus();
            }
        }

        private void txtB_Price_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                cbxUnit_ID.Focus();
            }
        }

        private void cbxUnit_ID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtMinLenth.Focus();
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

        #endregion

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

            // البحث عن اسم المورد في الجدول
            DataRow[] matched = tblSupplier.Select($"AccName = '{selectedName.Replace("'", "''")}'");

            if (matched.Length > 0)
            {
                // تم العثور على الاسم
                lblSuppliersID.Text = matched[0]["AccID"].ToString();
            }
            else
            {
                // لم يتم العثور على الاسم → تنبيه المستخدم
                MessageBox.Show("الاسم الذي أدخلته غير موجود في قائمة الموردين.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewItemSuppliers.Focus();
                txtNewItemSuppliers.SelectAll();
                lblSuppliersID.Text = "";
            }
        }


        private void txtCategory_Leave(object sender, EventArgs e)
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
            }
        }

    }
}
