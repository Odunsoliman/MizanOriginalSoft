using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses;
using System;
using System.Data;
using System.Windows.Forms;
using ZXing.QrCode.Internal;

namespace MizanOriginalSoft.Views.Forms.Products
{
    public partial class frmCatTree : Form
    {
        private DataTable? tblTree;
        private DataTable? _dtProducts; // أضف ? لجعلها nullable
        public int? SelectedCategoryID { get; private set; } = null;
        public string? SelectedCategoryName { get; private set; } = null;

        public enum FrmCatTreeMode
        {
            EditProducts,//اريد هنا ان يتغير الكابشن لزر الحفظ الى كلمة   - تغيير التصنيف
            SelectCategory//اريد هنا ان يتغير الكابشن لزر الحفظ الى كلمة   - اختيار التصنيف
        }

        public FrmCatTreeMode Mode { get; set; } = FrmCatTreeMode.EditProducts;
        private void SetSaveButtonText()
        {
            switch (Mode)
            {
                case FrmCatTreeMode.EditProducts:
                    btnSave.Text = "✔️ تغيير التصنيف";
                    break;

                case FrmCatTreeMode.SelectCategory:
                    btnSave.Text = "✔️ اختيار التصنيف";
                    break;
            }
        }

        public frmCatTree(DataTable dtProducts, FrmCatTreeMode mode = FrmCatTreeMode.EditProducts)
        {
            InitializeComponent();
            _dtProducts = dtProducts;
            Mode = mode;

            SetSaveButtonText();
        }

        public frmCatTree(FrmCatTreeMode mode)
        {
            InitializeComponent();
            Mode = mode;
            // تغيير Caption لزر الحفظ حسب الوضع
            SetSaveButtonText();
        }

        private void frmCatTree_Load(object sender, EventArgs e)
        {
            LoadTreeAndSelectSpecificNode();
        }

        #region ********** Search Tree Node ***********
        private List<TreeNode> matchedNodes = new List<TreeNode>();
        private void txtSearchTree_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearchTree.Text.Trim().ToLower();

            matchedNodes.Clear();
  //          currentMatchIndex = -1;// وهو مستخدم هنا فلماذ التحذير اعلا

            // إعادة تعيين الألوان وإغلاق كل الفروع
            ResetNodeColorsAndCollapse(treeViewCategories.Nodes);

            if (string.IsNullOrEmpty(searchText))
                return;

            // البحث وتلوين النتائج وفتح الفروع التي تحتوي نتائج
            SearchAndHighlightNodes(treeViewCategories.Nodes, searchText);

            // اختيار أول نتيجة
            if (matchedNodes.Count > 0)
            {
  //              currentMatchIndex = 0;
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


        private void treeViewCategories_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewCategories.SelectedNode != null)
                lblSelectedTreeNod.Text = treeViewCategories.SelectedNode.Text;
            else
                lblSelectedTreeNod.Text = string.Empty;
        }

        // تحميل الشجرة
        private void LoadTreeAndSelectSpecificNode(int selectedID = 0)
        {
            tblTree = DBServiecs.Categories_GetAll();
            treeViewCategories.Nodes.Clear();

            DataTable dt = tblTree ?? new DataTable();

            foreach (DataRow row in dt.Rows)
            {
                if (row["ParentID"] == DBNull.Value || Convert.ToInt32(row["ParentID"]) == 0)
                {
                    var parentNode = new TreeNode(row["CategoryName"].ToString())
                    {
                        Tag = Convert.ToInt32(row["CategoryID"])
                    };
                    treeViewCategories.Nodes.Add(parentNode);
                    AddChildNodes(dt, parentNode);
                }
            }

            if (selectedID > 0)
                SelectNodeById(treeViewCategories.Nodes, selectedID);
            treeViewCategories.CollapseAll();

        }

        // تحميل الفروع
        private void AddChildNodes(DataTable dt, TreeNode parentNode)
        {
            int parentId = Convert.ToInt32(parentNode.Tag);
            foreach (DataRow row in dt.Select($"ParentID = {parentId}"))
            {
                var childNode = new TreeNode(row["CategoryName"].ToString())
                {
                    Tag = Convert.ToInt32(row["CategoryID"])
                };
                parentNode.Nodes.Add(childNode);
                AddChildNodes(dt, childNode);
            }
        }

        // اختيار عقدة حسب ID
        private void SelectNodeById(TreeNodeCollection nodes, int id)
        {
            foreach (TreeNode node in nodes)
            {
                if (Convert.ToInt32(node.Tag) == id)
                {
                    treeViewCategories.SelectedNode = node;
                    return;
                }

                if (node.Nodes.Count > 0)
                    SelectNodeById(node.Nodes, id);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (treeViewCategories.SelectedNode == null)
            {
                MessageBox.Show("الرجاء اختيار تصنيف من الشجرة.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int selectedCategoryId = Convert.ToInt32(treeViewCategories.SelectedNode.Tag);
            string selectedCategoryName = treeViewCategories.SelectedNode.Text;

            if (Mode == FrmCatTreeMode.SelectCategory)
            {
                // الوضع الجديد: اختيار تصنيف فقط
                SelectedCategoryID = selectedCategoryId;
                SelectedCategoryName = selectedCategoryName;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                // الوضع الأصلي: تعديل المنتجات
                if (_dtProducts == null)
                {
                    MessageBox.Show("لا توجد منتجات للتحديث.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    int ID_user = CurrentSession.UserID;
                    DBServiecs.Product_UpdateCategory(_dtProducts, selectedCategoryId, ID_user);

                    MessageBox.Show("✔️ تم تحديث الأصناف بنجاح.");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("حدث خطأ أثناء تحديث الأصناف:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
