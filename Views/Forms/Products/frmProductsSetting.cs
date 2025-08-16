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

        #endregion
        #region ######### إعداد قوائم التقارير بناءً على ReportsMaster ########

        /// <summary>
        /// إنشاء شريط القوائم داخل Panel
        /// </summary>
        /// 
        private MenuStrip? menuStrip1;

        private void SetupMenuStrip()
        {
            menuStrip1 = new MenuStrip();
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


        /// <summary>
        /// تحميل القوائم بناءً على الحساب الممرر
        /// </summary>
        private void LoadReports(int topAcc)
        {
            try
            {
                DataTable dt = DBServiecs.Reports_GetByTopAcc(topAcc);

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

        /// <summary>
        /// تعبئة القائمة بعناصر من DataRow[]
        /// </summary>
        /// 
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

        private void LoadMenuItems_(ToolStripMenuItem parentMenu, DataRow[] rows)
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

                ToolStripMenuItem menuItem = new(displayName)
                {
                    Tag = codeName
                };
                menuItem.Click += ReportMenuItem_Click;

                parentMenu.DropDownItems.Add(menuItem);
            }
        }

        /// <summary>
        /// حدث النقر على أي تقرير
        /// </summary>
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

        private void ReportMenuItem_Click_(object? sender, EventArgs e)
        {
            /*هنا فى شاشة الاصناف توجد تقارير خاصة بصنف واخرى لمجموعة اصناف محددة فكيف يتم التعديل*/
            if (sender is not ToolStripMenuItem clickedItem || clickedItem.Tag is null)
            {
                MessageBox.Show("بيانات التقرير غير صحيحة.");
                return;
            }

            string reportCodeName = clickedItem.Tag.ToString() ?? "";
            if (string.IsNullOrEmpty(reportCodeName))
            {
                MessageBox.Show("لا يوجد اسم كود للتقرير.");
                return;
            }

            try
            {
                // تجهيز البيانات لتمريرها لشاشة المعاينة
                Dictionary<string, object> reportParameters = new()
        {
            { "ReportCodeName", reportCodeName },
            { "UserID", ID_user },
            { "EntityID", GetCurrentEntityID() ?? (object)DBNull.Value },
            { "FilteredData", GetFilteredData() }
        };

                using frmSettingReports previewForm = new frmSettingReports(reportParameters);
                previewForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء فتح التقرير: {ex.Message}");
            }
        }

        /// <summary>
        /// جلب كود الصنف الحالي
        /// </summary>
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

        /// <summary>
        /// جلب البيانات المفلترة
        /// </summary>
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

        private bool ContainsNode(TreeNode parent, TreeNode child)
        {
            if (child.Parent == null) return false;
            if (child.Parent.Equals(parent)) return true;
            return ContainsNode(parent, child.Parent);
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


    }
}
