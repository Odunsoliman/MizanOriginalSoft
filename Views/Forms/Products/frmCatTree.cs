using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses;
using System;
using System.Data;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.Products
{
    public partial class frmCatTree : Form
    {
        private DataTable? tblTree;
        private readonly DataTable _dtProducts;

        public frmCatTree(DataTable dtProducts)
        {
            InitializeComponent();
            _dtProducts = dtProducts;
        }

        private void frmCatTree_Load(object sender, EventArgs e)
        {
            LoadTreeAndSelectSpecificNode();
        }

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

            treeViewCategories.ExpandAll();
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
            int ID_user = CurrentSession.UserID;

            try
            {
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
