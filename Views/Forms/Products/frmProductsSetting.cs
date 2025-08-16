using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.Products
{
    public partial class frmProductsSetting : Form
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

        public frmProductsSetting()
        {
            InitializeComponent();
            ID_user = CurrentSession .UserID ;
            SetupAutoCompleteSuppliers();
            SetupAutoCompleteCategories();
            FillUnits();
            tblModify = new DataTable();
        }

        private void frmProductsSetting_Load(object sender, EventArgs e)
        {

        }


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

        private void LoadTreeAndSelectSpecificNode(int selectedID = 0)
        {
            tblTree = DBServiecs.Categories_GetAll();

            treeViewCategories.Nodes.Clear();

            DataTable dt = tblTree ?? new DataTable();

            foreach (DataRow row in dt.Rows)
            {// هنا تدخل شرط الاف 
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

        //تحميل الاصناف على الشاشة
        private void LoadProducts()
        {

            _tblProd = DBServiecs.Product_GetAll();
            BindProductDataToDGV();
        }
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

        //تحديث عدد الاصناف فى DGV  ###
        private void UpdateCount()
        {
            lblCount.Text = DGV?.RowCount.ToString() ?? "0";
        }
  
        #endregion




    }
}
