using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.SearchClasses;
using MizanOriginalSoft.Views.Forms.MainForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.Products
{
    public partial class frmProductsAction : Form
    {

        #region ===== Variables =====
        private DataTable? tblCategory;
        private DataTable? tblSupplier;

        private float LastLeftWidth { get; set; } = 50;
        private float LastRightWidth { get; set; } = 50;
        private Dictionary<string, object> _parameters;
        public int ViewType { get; private set; }
        public int? CategoryID { get; set; }
        public int Product_ID { get; private set; }
        public DataTable SelectedProducts { get; private set; } = new DataTable();
        #endregion
        #region ====== variables =================

        #endregion



        public frmProductsAction(Dictionary<string, object> parameters)
        {
            InitializeComponent();
            _parameters = parameters;

            if (_parameters.ContainsKey("ScreenType"))
                ViewType = Convert.ToInt32(_parameters["ScreenType"]);

            if (_parameters.TryGetValue("SelectedProducts", out var value) && value is DataTable table)
            {
                SelectedProducts = table;
            }
            else
            {
                // تعيين قيمة افتراضية أو التعامل مع الخطأ
                SelectedProducts = new DataTable(); // أو null حسب حالتك
            }

        }
        private void frmProductsAction_Load(object sender, EventArgs e)
        {
            SetupAutoCompleteSuppliers();
            SetupAutoCompleteCategories();
            FillUnits();
            GetSelected_Productes();
        }
        
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void FillUnits()
        {
            DataTable dt = new DataTable();
            dt=DBServiecs.ProductGetUnits();
            cbxUnit_idSelected .DataSource = dt;
            cbxUnit_idSelected .DisplayMember = "UnitProd";
            cbxUnit_idSelected.ValueMember = "Unit_ID";
            cbxUnit_idSelected.SelectedIndex = -1;
        }
        private void GetSelected_Productes()
        {
            DataTable dt = new DataTable();
            dt=  DBServiecs.Product_GetSelected(SelectedProducts);
            DGV.DataSource = dt;
            ApplyDGVStyles();
            lblCountDGV .Text =DGV .RowCount .ToString ();
            DGV.ClearSelection ();
        }
        private void ApplyDGVStyles()
        {
            // 1. ضبط نمط التحكم في حجم الأعمدة لملء المساحة المتاحة
            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 2. إظهار HeaderRow (صف الرؤوس)
            DGV.ColumnHeadersVisible = true;
            DGV.RowHeadersVisible = true; // إظهار مؤشر الصفوف على الجانب إذا كنت تريده
            DGV.RowHeadersVisible  = false ;

            // 3. منع إضافة صفوف جديدة
            DGV.AllowUserToAddRows = false;

            // 4. منع التعديل على الخلايا
            DGV.ReadOnly = true;

            // 5. تحديد الصف بالكامل عند النقر عليه
            DGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // 6. إخفاء جميع الأعمدة أولاً
            foreach (DataGridViewColumn column in DGV.Columns)
            {
                column.Visible = false;
            }

            // 7. تعريف وإظهار الأعمدة المطلوبة فقط
            var visibleColumns = new[]
            {
        new {
            Name = "ProductCode",
            Header = "كود الصنف",
            FillWeight = 1,
            Alignment = DataGridViewContentAlignment.MiddleCenter
        },
        new {
            Name = "ProdName",
            Header = "اسم الصنف",
            FillWeight = 3,
            Alignment = DataGridViewContentAlignment.MiddleLeft
        },
        new {
            Name = "U_Price",
            Header = "سعر البيع",
            FillWeight = 1,
            Alignment = DataGridViewContentAlignment.MiddleCenter
        },
        new {
            Name = "B_Price",
            Header = "سعر الشراء",
            FillWeight = 1,
            Alignment = DataGridViewContentAlignment.MiddleCenter
        }
    };

            foreach (var col in visibleColumns)
            {
                if (DGV.Columns.Contains(col.Name))
                {
                    DGV.Columns[col.Name].Visible = true;
                    DGV.Columns[col.Name].HeaderText = col.Header;
                    DGV.Columns[col.Name].FillWeight = col.FillWeight;
                    DGV.Columns[col.Name].DefaultCellStyle.Alignment = col.Alignment;
                }
            }

            // 8. التنسيق العام للخلايا
            DGV.DefaultCellStyle.Font = new Font("Times New Roman", 14);
            DGV.DefaultCellStyle.ForeColor = Color.Blue;
            DGV.DefaultCellStyle.BackColor = Color.LightYellow;

            // 9. تنسيق رؤوس الأعمدة
            DGV.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Bold);
            DGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // 10. تباين الصفوف (لون للصفوف الزوجية)
            DGV.AlternatingRowsDefaultCellStyle.BackColor = Color.LightCyan;

            // 11. تنسيق الصف المحدد
            DGV.DefaultCellStyle.SelectionBackColor = Color.SteelBlue;
            DGV.DefaultCellStyle.SelectionForeColor = Color.White;

            // 12. معالجة النصوص في الخلايا
            DGV.CellFormatting += (sender, e) =>
            {
                if (DGV.Columns[e.ColumnIndex].Visible)
                {
                    string cellValue = e.Value?.ToString()?.Trim() ?? string.Empty;

                    if (DGV.Columns[e.ColumnIndex].Name == "U_Price" &&
                        decimal.TryParse(cellValue, out decimal price))
                    {
                        e.Value = price.ToString("N2");
                    }
                    else
                    {
                        e.Value = cellValue;
                    }
                }
            };


            // 12. معالجة النصوص في الخلايا
            DGV.CellFormatting += (sender, e) =>
            {
                if (DGV.Columns[e.ColumnIndex].Visible)
                {
                    string cellValue = e.Value?.ToString()?.Trim() ?? string.Empty;

                    if (DGV.Columns[e.ColumnIndex].Name == "B_Price" &&
                        decimal.TryParse(cellValue, out decimal price))
                    {
                        e.Value = price.ToString("N2");
                    }
                    else
                    {
                        e.Value = cellValue;
                    }
                }
            };

            // 13. تحسين مظهر الـ GridView
            DGV.EnableHeadersVisualStyles = false; // لتفعيل التنسيق المخصص للرؤوس
            DGV.BorderStyle = BorderStyle.FixedSingle;
            DGV.GridColor = Color.LightGray;
            DGV.ClearSelection();
        }

        #region ========= SetupAutoComplete =================
        private void SetupAutoCompleteSuppliers()
        {
            try
            {
                // جلب جميع الموردين
                tblSupplier = DBServiecs.Accounts_GetSupplier();

                // إنشاء مجموعة الإكمال التلقائي
                AutoCompleteStringCollection autoCompleteCollection = new AutoCompleteStringCollection();

                // التأكد من أن الجدول يحتوي على بيانات
                if (tblSupplier != null && tblSupplier.Rows.Count > 0)
                {
                    foreach (DataRow row in tblSupplier.Rows)
                    {
                        string? accName = row["AccName"]?.ToString(); // accName تقبل null الآن
                        if (!string.IsNullOrEmpty(accName))
                        {
                            autoCompleteCollection.Add(accName);
                        }
                    }
                }
                txtSupplierSelected.AutoCompleteCustomSource = autoCompleteCollection;
                txtSupplierSelected.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txtSupplierSelected.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء إعداد الإكمال التلقائي للموردين: {ex.Message}",
                              "خطأ",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void SetupAutoCompleteCategories()
        {
            try
            {
                // جلب جميع التصنيفات
                tblCategory = DBServiecs.Categories_GetAll();

                // إنشاء مجموعة الإكمال التلقائي
                AutoCompleteStringCollection autoCompleteCollection = new AutoCompleteStringCollection();

                // التأكد من أن الجدول يحتوي على بيانات

                if (tblCategory != null && tblCategory.Rows.Count > 0)
                {
                    foreach (DataRow row in tblCategory.Rows)
                    {
                        string? categoryName = row["CategoryName"]?.ToString();
                        if (!string.IsNullOrEmpty(categoryName))
                        {
                            autoCompleteCollection.Add(categoryName);
                        }
                    }
                }

                txtCategoriesSelected.AutoCompleteCustomSource = autoCompleteCollection;
                txtCategoriesSelected.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txtCategoriesSelected.AutoCompleteSource = AutoCompleteSource.CustomSource;


            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء إعداد الإكمال التلقائي للتصنيفات: {ex.Message}",
                              "خطأ",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        //وبناء على ما سبق ضبط زر الحفظ ليستقبل القيم بشكل صحيح
        private void btnSaveSelected_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. التحقق من أن هناك بيانات لتعديلها
                if (SelectedProducts == null || SelectedProducts.Rows.Count == 0)
                {
                    MessageBox.Show("لا توجد بيانات لتعديلها.");
                    return;
                }

                // 2. المتغيرات nullable لأن التعديل اختياري
                float? uPrice = null;
                float? bPrice = null;
                int? supplierID = null;
                int? categoryID = null;
                int? unitID = null;

                // 3. سعر البيع (يمكن تركه فارغاً)
                string txtU = txtU_PriceSelected.Text.Trim();
                if (!string.IsNullOrEmpty(txtU))
                {
                    if (float.TryParse(txtU, out float parsedU))
                        uPrice = parsedU;
                    else
                    {
                        MessageBox.Show("برجاء إدخال سعر بيع صحيح.");
                        txtU_PriceSelected.Focus();
                        return;
                    }
                }

                // 4. سعر الشراء (يمكن تركه فارغاً)
                string txtB = txtB_PriceSelected.Text.Trim();
                if (!string.IsNullOrEmpty(txtB))
                {
                    if (float.TryParse(txtB, out float parsedB))
                        bPrice = parsedB;
                    else
                    {
                        MessageBox.Show("برجاء إدخال سعر شراء صحيح.");
                        txtB_PriceSelected.Focus();
                        return;
                    }
                }
                // 5. رقم المورد (يمكن تركه فارغاً)
                if (int.TryParse(lblSupplier_ID.Text.Trim(), out int parsedSupp))
                    supplierID = parsedSupp;

                // 6. رقم التصنيف (يمكن تركه فارغاً)
                if (int.TryParse(lblCatID.Text.Trim(), out int parsedCat))
                    categoryID = parsedCat;


                // 7. رقم الوحدة (يمكن تركه فارغاً)
                if (cbxUnit_idSelected.SelectedValue != null && int.TryParse(cbxUnit_idSelected.SelectedValue.ToString(), out int parsedUnit))
                    unitID = parsedUnit;

                // 8. التحقق النهائي أن هناك على الأقل تعديل واحد
                if (uPrice == null && bPrice == null && supplierID == null && categoryID == null && unitID == null)
                {
                    MessageBox.Show("لم يتم إدخال أي قيمة لتعديلها.");
                    return;
                }

                // 9. إنشاء DataTable يحتوي على ProductID فقط من SelectedProducts
                DataTable productIdTable = new DataTable();
                productIdTable.Columns.Add("ProductID", typeof(int));
                foreach (DataRow row in SelectedProducts.Rows)
                {
                    if (row["ID_Product"] != DBNull.Value)
                        productIdTable.Rows.Add(Convert.ToInt32(row["ID_Product"]));
                }

                // 10. تنفيذ التحديث باستخدام DataTable
                DBServiecs.Products_UpdateSelectedItems(productIdTable, uPrice, bPrice, supplierID, categoryID, unitID);

                // 11. تأكيد
                MessageBox.Show("تم تحديث الأصناف بنجاح.");
                CategoryID = categoryID;
                this.DialogResult = DialogResult.OK;

                this.Close();
            }
            catch (Exception ex)
            {
                 MessageBox.Show("حدث خطأ أثناء الحفظ: " + ex.Message);
            }
        }

        #endregion
        private void txtSupplierSelected_Leave(object sender, EventArgs e)
        {
            if (tblSupplier == null) return;

            string selectedName = txtSupplierSelected.Text.Trim();

            DataRow[] matched = tblSupplier.Select($"AccName = '{selectedName.Replace("'", "''")}'");
            if (matched.Length > 0)
            {
                lblSupplier_ID.Text = matched[0]["AccID"].ToString();
            }
            else
            {
                lblSupplier_ID.Text = "";
            }
        }
        private void txtCategoriesSelected_Leave(object sender, EventArgs e)
        {
            if (tblCategory == null) return;

            string selectedName = txtCategoriesSelected.Text.Trim();

            DataRow[] matched = tblCategory.Select($"CategoryName = '{selectedName.Replace("'", "''")}'");
            if (matched.Length > 0)
            {
                lblCatID.Text = matched[0]["CategoryID"].ToString();
            }
            else
            {
                lblCatID.Text = "";
            }
        }

        private void txtU_PriceSelected_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // منع صوت التنبيه
                txtB_PriceSelected.Focus();
            }
        }
        private void txtB_PriceSelected_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtSupplierSelected.Focus();
            }
        }
        private void txtSupplierSelected_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtCategoriesSelected.Focus();
            }

            if (e.Control && e.KeyCode == Keys.F)
            {
                // استدعاء شاشة البحث باستخدام SearchHelper
                string selectedAccount = SearchHelper.SearchAccount("Suppliers");

                if (!string.IsNullOrEmpty(selectedAccount))
                {
                    lblSupplier_ID.Text = selectedAccount;
                }
            }
        }



        private void txtCategoriesSelected_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                cbxUnit_idSelected.Focus();
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
                        //                       txtCategory.Leave -= txtCategory_Leave;

                        // تعيين الاسم والرقم مباشرة
                        txtCategoriesSelected.Text = frm.SelectedCategoryName ?? string.Empty;
                        //      lblCategoryID.Text = frm.SelectedCategoryID.ToString();

                        // إعادة تفعيل الحدث
                     //   lblCatID .Leave += txtCategory_Leave;
                    }
                }

            }

        }

        private void cbxUnit_idSelected_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtU_PriceSelected.Focus(); // العودة للبداية
            }
        }


    }
}

