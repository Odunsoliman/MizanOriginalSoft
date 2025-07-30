
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.MainForms
{
    public partial class frmSearch : Form
    {
        public string? SelectedAccID { get; private set; }
        public string? SelectedID { get; private set; }
        public string? SelectedName { get; private set; }
        public string? SelectedFirstPhon { get; private set; }
        public string? SelectedAntherPhon { get; private set; }
        public string? SelectedClientEmail { get; private set; }
        public string? SelectedClientAddress { get; private set; }
        public float? SelectedBalance { get; private set; }
        public string? SelectedBalanceState { get; private set; }

        public object? iDGV
        {
            get => DGV.DataSource;
            set => DGV.DataSource = value;
        }


        public DataTable dt = new DataTable();

        private readonly SearchEntityType EntityType;
        private readonly int type_ID;
        private int? ProductID = null; // تعريف متغير داخلي لرقم الصنف

        public frmSearch(int typeId, SearchEntityType entityType, int? productId = null)
        {
            InitializeComponent();
            this.type_ID = typeId;
            this.EntityType = entityType;

            if (entityType == SearchEntityType.Pice && productId.HasValue)
                this.ProductID = productId;
        }

        public enum SearchEntityType
        {
            Product = 0,
            Customer = 7,
            Supplier = 14,
            Catigory = 20,
            AllAccounts = 200,
            Pice = 201
        }

        private void frmSearch_Load(object sender, EventArgs e)
        {
            T_Search();
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty,
                null, DGV, new object[] { true });

        }

        // تحميل البيانات في DataGridView حسب نوع البحث المحدد
        private void T_Search()
        {
            switch (EntityType)
            {
                case SearchEntityType.Product:
                    if (type_ID == 1 || type_ID == 3 || type_ID == 5)
                    {
                        dt = DBServiecs.ProductSearch_GetAll();
                        DGV.DataSource = dt;
                        DGVStylProd();
                        lblNameTable.Text = "الأصناف";
                    }
                    else if (type_ID == 2)
                    {
                        dt = DBServiecs.NewInvoice_GetOldInvoicesByType(1);
                        DGV.DataSource = dt;
                        DGVStylInvoic();
                        lblNameTable.Text = "فواتير البيع السابقة";
                    }
                    else if (type_ID == 4)
                    {
                        dt = DBServiecs.NewInvoice_GetOldInvoicesByType(3);
                        DGV.DataSource = dt;
                        DGVStylInvoic();
                        lblNameTable.Text = "فواتير الشراء السابقة";
                    }
                    else
                    {
                        CustomMessageBox.ShowWarning("نوع العملية غير مدعوم لعرض البيانات.", "تحذير");
                    }
                    break;

                case SearchEntityType.Customer:
                    dt = DBServiecs.MainAcc_GetAccounts((int)SearchEntityType.Supplier);
                    DGV.DataSource = dt;
                    DGVStylAcc();
                    lblNameTable.Text = "التصنيفات";
                    break;

                case SearchEntityType.Supplier:
                    dt = DBServiecs.MainAcc_GetAccounts((int)SearchEntityType.Supplier);
                    DGV.DataSource = dt;
                    DGVStylAcc();
                    lblNameTable.Text = "الموردين";
                    break;

                case SearchEntityType.AllAccounts:
                    dt = DBServiecs.MainAcc_GetAccounts((int)SearchEntityType.AllAccounts);
                    DGV.DataSource = dt;
                    DGVStylAcc();
                    lblNameTable.Text = "الموردين والعملاء";
                    break;

                case SearchEntityType.Catigory:
                    LoadAndDisplayCategories();
                    lblNameTable.Text = "التصنيفات";
                    break;

                case SearchEntityType.Pice:
                    if (ProductID.HasValue)
                    {
                        dt = DBServiecs.Product_GetPiecesByProductID(ProductID.Value);
                        DGV.DataSource = dt;
                        btnDelete.Visible  = true ;
                        StylPice();
                    }
                    else
                    {
                        CustomMessageBox.ShowWarning("لم يتم تحديد رقم الصنف لعرض القطع.", "تحذير");
                    }
                    break;


                default:
                    CustomMessageBox.ShowWarning("لم يتم تعريف نوع البحث!", "تحذير");
                    break;
            }

        }
        #region ********  Styl DGV **************

        //
        private void StylPice()
        {
            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // إخفاء جميع الأعمدة أولًا
            foreach (DataGridViewColumn col in DGV.Columns)
            {
                col.Visible = false;
            }

            // إظهار الأعمدة المطلوبة فقط
            if (DGV.Columns.Contains("Piece_ID"))
            {
                DGV.Columns["Piece_ID"].Visible = true;
                DGV.Columns["Piece_ID"].HeaderText = "كود القطعة";
                DGV.Sort(DGV.Columns["Piece_ID"], ListSortDirection.Ascending); // ترتيب حسب الكود
            }

            if (DGV.Columns.Contains("Piece_Length"))
            {
                DGV.Columns["Piece_Length"].Visible = true;
                DGV.Columns["Piece_Length"].HeaderText = "طول القطعة";
            }

            if (DGV.Columns.Contains("Gem_Discount"))
            {
                DGV.Columns["Gem_Discount"].Visible = true;
                DGV.Columns["Gem_Discount"].HeaderText = "الخصم الخاص";
            }

            ApplyDGVStyles();

            // استخراج اسم المنتج وسعره من أول صف في الجدول
            if (DGV.Rows.Count > 0)
            {
                object? cellProdName = DGV.Rows[0].Cells["ProdName"].Value;
                object? cellUPrice = DGV.Rows[0].Cells["U_Price"].Value;

                string prodName = cellProdName?.ToString() ?? "غير معروف";
                string uPrice = "0.00";

                if (decimal.TryParse(cellUPrice?.ToString(), out decimal price))
                    uPrice = price.ToString("N2");

                lblNameTable.Text = $"اسم المنتج: {prodName}  -  السعر: {uPrice}";
            }

            else
            {
                lblNameTable.Text = "لا توجد بيانات لاسم المنتج أو السعر.";
            }
        }


        // تنسيق جدول عرض الأصناف
        private void DGVStylProd()
        {
            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            DGV.Columns["ID_Product"].Visible = false;
            DGV.Columns["ProductCode"].Visible = true;
            DGV.Columns["ProdName"].Visible = true;
            DGV.Columns["U_Price"].Visible = true;

            DGV.Columns["ProductCode"].FillWeight = 1;
            DGV.Columns["ProdName"].FillWeight = 4;
            DGV.Columns["U_Price"].FillWeight = 1;

            DGV.Columns["ProductCode"].HeaderText = "كود";
            DGV.Columns["ProdName"].HeaderText = "اسم الصنف";
            DGV.Columns["U_Price"].HeaderText = "السعر";

            ApplyDGVStyles();

            if (DGV.Columns.Contains("ProductCode"))
                DGV.Sort(DGV.Columns["ProductCode"], ListSortDirection.Ascending);
        }

        // تنسيق جدول الحسابات (العملاء/الموردين)
        private void DGVStylAcc()
        {
            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            foreach (DataGridViewColumn col in DGV.Columns)
            {
                col.Visible = col.Name == "AccID" || col.Name == "AccName" || col.Name == "Balance" || col.Name == "BalanceState";
            }

            if (DGV.Columns.Contains("AccID"))
                DGV.Columns["AccID"].HeaderText = "رقم الحساب";

            if (DGV.Columns.Contains("AccName"))
                DGV.Columns["AccName"].HeaderText = "الاسم";

            if (DGV.Columns.Contains("Balance"))
                DGV.Columns["Balance"].HeaderText = "الرصيد";

            if (DGV.Columns.Contains("BalanceState"))
                DGV.Columns["BalanceState"].HeaderText = "-";

            ApplyDGVStyles();

            if (DGV.Columns.Contains("AccName"))
                DGV.Sort(DGV.Columns["AccName"], ListSortDirection.Ascending);
        }

        // تنسيق جدول الفواتير القديمة
        private void DGVStylInvoic()
        {
            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            foreach (DataGridViewColumn col in DGV.Columns)
                col.Visible = false;

            if (DGV.Columns.Contains("Inv_Counter"))
            {
                DGV.Columns["Inv_Counter"].Visible = true;
                DGV.Columns["Inv_Counter"].HeaderText = "رقم الفاتورة";
                DGV.Columns["Inv_Counter"].DisplayIndex = 0;
            }

            if (DGV.Columns.Contains("AccName"))
            {
                DGV.Columns["AccName"].Visible = true;
                DGV.Columns["AccName"].HeaderText = "اسم الحساب";
                DGV.Columns["AccName"].DisplayIndex = 1;
            }

            if (DGV.Columns.Contains("Inv_Date"))
            {
                DGV.Columns["Inv_Date"].Visible = true;
                DGV.Columns["Inv_Date"].HeaderText = "تاريخ الفاتورة";
                DGV.Columns["Inv_Date"].DisplayIndex = 2;
            }

            if (DGV.Columns.Contains("NetTotal"))
            {
                DGV.Columns["NetTotal"].Visible = true;
                DGV.Columns["NetTotal"].HeaderText = "القيمة النهائية";
                DGV.Columns["NetTotal"].DisplayIndex = 3;
            }

            ApplyDGVStyles();

            if (DGV.Columns.Contains("AccName"))
                DGV.Sort(DGV.Columns["AccName"], ListSortDirection.Ascending);
        }

        // تنسيق عام للـ DataGridView
        private void ApplyDGVStyles()
        {
            DGV.DefaultCellStyle.Font = new Font("Times New Roman", 14);
            DGV.DefaultCellStyle.ForeColor = Color.Blue;
            DGV.DefaultCellStyle.BackColor = Color.LightYellow;

            DGV.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Bold);
            DGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;

            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DGV.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        // تنسيق جدول الاصناف


        private void LoadAndDisplayCategories()
        {
            // 1. جلب البيانات من القاعدة
            DataTable dt = DBServiecs.Categories_GetAll();

            // 2. ترتيب البيانات هرميًا
            var orderedRows = GetOrderedRowsHierarchically(dt);

            // 3. إنشاء نسخة من الجدول مرتبة
            DataTable newTable = dt.Clone(); // نسخ البنية فقط

            foreach (var row in orderedRows)
                newTable.ImportRow(row); // نسخ الصفوف المرتبة

            // 4. عرضها في DataGridView
            DGV.DataSource = newTable;

            // 5. تنسيق الـ DataGridView
            StylizeCategoryGrid();
        }

        private List<DataRow> GetOrderedRowsHierarchically(DataTable table)
        {
            List<DataRow> orderedList = new List<DataRow>();

            // بناء علاقة الأبناء لكل أب
            var childrenLookup = table.AsEnumerable()
                .GroupBy(r => r.Field<int?>("ParentID") ?? -1) // التعامل مع null
                .ToDictionary(g => g.Key, g => g.ToList());

            // إضافة عمود العمق إذا لم يكن موجودًا
            if (!table.Columns.Contains("Depth"))
                table.Columns.Add("Depth", typeof(int));

            void AddChildren(int parentId, int depth)
            {
                if (!childrenLookup.ContainsKey(parentId)) return;

                foreach (var child in childrenLookup[parentId].OrderBy(r => r["CategoryName"].ToString()))
                {
                    child["Depth"] = depth;

                    string originalName = child["CategoryName"]?.ToString() ?? "";
                    string prefix = "  ";

                    // بادئة الاسم حسب المستوى
                    if (depth == 1)
                        prefix = "●  "; // ابن
                    else if (depth >= 2)
                        prefix = new string('-', depth) + "     "; // حفيد

                    // الإزاحة مع الاسم
                    child["CategoryName"] = prefix + originalName;

                    orderedList.Add(child);
                    AddChildren(child.Field<int>("CategoryID"), depth + 1);
                }
            }

            AddChildren(-1, 0); // الجذور

            return orderedList;
        }

        private void StylizeCategoryGrid()
        {
            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // إخفاء جميع الأعمدة ما عدا CategoryName
            foreach (DataGridViewColumn col in DGV.Columns)
            {
                col.Visible = col.Name == "CategoryName";
            }

            // ضبط رأس العمود
            if (DGV.Columns.Contains("CategoryName"))
            {
                DGV.Columns["CategoryName"].HeaderText = "التصنيف";
                DGV.Columns["CategoryName"].Visible = true;
            }

            // تنسيق مرئي
            DGV.DefaultCellStyle.Font = new Font("Courier New", 13); // خط موحّد لإظهار الإزاحة
            DGV.DefaultCellStyle.ForeColor = Color.Navy;
            DGV.DefaultCellStyle.BackColor = Color.White;
            DGV.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            DGV.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Bold);
            DGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void DGV_RowPrePaint_(object sender, DataGridViewRowPrePaintEventArgs e)
        { // هذا الحدث يخص هالة واحد فقط وهى عرض الكاتيجورى فكيف نتجاوزها فى الحالات الاخرى
            if (DGV.Rows[e.RowIndex].DataBoundItem is DataRowView rowView)
            {
                int depth = rowView.Row.Field<int>("Depth");//System.ArgumentException: 'Column 'Depth' does not belong to table .'

                // إعداد نمط الخلية
                var style = new DataGridViewCellStyle(DGV.DefaultCellStyle);

                switch (depth)
                {
                    case 0: // الأب
                        style.ForeColor = Color.DarkBlue;
                        style.Font = new Font("Times New Roman", 16, FontStyle.Bold);
                        break;

                    case 1: // الابن
                        style.ForeColor = Color.DarkGreen;
                        style.Font = new Font("Times New Roman", 14, FontStyle.Regular);
                        break;

                    default: // الأحفاد
                        style.ForeColor = Color.Red;
                        style.Font = new Font("Times New Roman", 12, FontStyle.Regular);
                        break;
                }

                DGV.Rows[e.RowIndex].DefaultCellStyle = style;
            }
        }

        #endregion 
        // الحدث عند النقر المزدوج على صف في الجدول
        private void DGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = DGV.Rows[e.RowIndex];

            if (EntityType == SearchEntityType.Product)
            {
                if (type_ID == 1 || type_ID == 3 || type_ID == 5)
                    SelectedID = row.Cells["ProductCode"]?.Value?.ToString() ?? string.Empty;
                else if (type_ID == 2 || type_ID == 4)
                    SelectedID = row.Cells["Inv_Counter"]?.Value?.ToString() ?? string.Empty;
            }
            else if (EntityType == SearchEntityType.Customer ||
                     EntityType == SearchEntityType.Supplier ||
                     EntityType == SearchEntityType.AllAccounts)
            {
                SelectedID = row.Cells["AccID"]?.Value?.ToString() ?? string.Empty;
                SelectedName = row.Cells["AccName"]?.Value?.ToString() ?? string.Empty;
                SelectedAntherPhon = row.Cells["AntherPhon"]?.Value?.ToString() ?? string.Empty;

                SelectedBalance = float.TryParse(row.Cells["Balance"]?.Value?.ToString(), out var balance)
                    ? balance : 0;

                SelectedBalanceState = row.Cells["BalanceState"]?.Value?.ToString() ?? string.Empty;
                SelectedClientAddress = row.Cells["ClientAddress"]?.Value?.ToString() ?? string.Empty;
                SelectedClientEmail = row.Cells["ClientEmail"]?.Value?.ToString() ?? string.Empty;
                SelectedFirstPhon = row.Cells["FirstPhon"]?.Value?.ToString() ?? string.Empty;
            }
            else if (EntityType == SearchEntityType.Catigory)
            {
                SelectedID = row.Cells["ProductCode"]?.Value?.ToString() ?? string.Empty;
                SelectedID = row.Cells["Inv_Counter"]?.Value?.ToString() ?? string.Empty;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // تصفية البيانات أثناء كتابة المستخدم في مربع البحث
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string EntityType = txtSearch.Text.Trim().Replace("'", "''");

            if (DGV.DataSource is DataTable localDt)
            {
                switch (EntityType)
                {
                    case "0":
                        localDt.DefaultView.RowFilter =
                            $"ProdName LIKE '%{EntityType}%' OR ProductCode LIKE '%{EntityType}%'";
                        break;

                    case "7":
                    case "14":
                    case "200":
                        localDt.DefaultView.RowFilter =
                            $"AccName LIKE '%{EntityType}%' OR FirstPhon LIKE '%{EntityType}%' OR AntherPhon LIKE '%{EntityType}%' OR ClientAddress LIKE '%{EntityType}%'";
                        break;

                    default:
                        localDt.DefaultView.RowFilter = string.Empty;
                        break;
                }
            }
            else
            {
                CustomMessageBox.ShowWarning("لم يتم العثور على مصدر بيانات مناسب للبحث!", "تحذير");
            }
        }
        #region *******  DGV_RowPrePaint  ***************
        private void DGV_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            // التأكد أن الصف صالح
            if (e.RowIndex < 0 || DGV.Rows.Count <= e.RowIndex)
                return;

            // تحديد طريقة التعامل حسب نوع البيانات
            switch (EntityType)
            {
                case SearchEntityType.Catigory:
                    HandleCategoryRowStyling(e.RowIndex);
                    break;

                case SearchEntityType.Pice:
                    // مثلًا: يمكنك تخصيص تنسيق معين للقطع إن أردت
                    break;

                case SearchEntityType.Supplier:
                case SearchEntityType.Customer:
                    // يمكن لاحقًا إضافة ألوان حسب الرصيد أو حالة الحساب
                    break;

                default:
                    // لا شيء - الحالات الأخرى لا تحتاج تنسيق خاص
                    break;
            }
        }

        private void HandleCategoryRowStyling(int rowIndex)
        {
            if (DGV.Rows[rowIndex].DataBoundItem is DataRowView rowView &&
                rowView.Row.Table.Columns.Contains("Depth"))
            {
                int depth = rowView.Row.Field<int>("Depth");

                var style = new DataGridViewCellStyle(DGV.DefaultCellStyle);

                switch (depth)
                {
                    case 0:
                        style.ForeColor = Color.DarkBlue;
                        style.Font = new Font("Times New Roman", 16, FontStyle.Bold);
                        break;
                    case 1:
                        style.ForeColor = Color.DarkGreen;
                        style.Font = new Font("Times New Roman", 14, FontStyle.Regular);
                        break;
                    default:
                        style.ForeColor = Color.Red;
                        style.Font = new Font("Times New Roman", 12, FontStyle.Regular);
                        break;
                }

                DGV.Rows[rowIndex].DefaultCellStyle = style;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // 1. التأكد من وجود رقم الصنف
            if (!ProductID.HasValue)
            {
                MessageBox.Show("رقم الصنف غير متوفر لهذه الشاشة.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int productId = ProductID.Value;

            // 2. التحقق من وجود المستخدم
            int currentUserId = CurrentSession.UserID;
            string currentUserName = CurrentSession.UserName;

            if (string.IsNullOrWhiteSpace(currentUserName))
            {
                MessageBox.Show("لا يمكن تحديد اسم المستخدم الحالي. يرجى إعادة تسجيل الدخول.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3. إدخال سبب الحذف
            string reason;
            DialogResult reasonResult = CustomMessageBox.ShowStringInputBox(out reason, "يرجى إدخال سبب الحذف:", "سبب الحذف");

            if (reasonResult != DialogResult.OK || string.IsNullOrWhiteSpace(reason) || reason.Length < 10)
            {
                MessageBox.Show("الرجاء إدخال سبب واضح يحتوي على 10 أحرف على الأقل.", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4. إدخال كلمة مرور الأدمن إذا لزم الأمر
            string adminPassword = string.Empty;
            if (CurrentSession.IsAdmin)
            {
                string enteredPassword;
                DialogResult passResult = CustomMessageBox.ShowPasswordInputBox(out enteredPassword, "يرجى إدخال كلمة مرور الأدمن:", "تحقق الأدمن");

                if (passResult != DialogResult.OK || string.IsNullOrWhiteSpace(enteredPassword))
                {
                    MessageBox.Show("لم يتم إدخال كلمة المرور.", "إلغاء", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                adminPassword = enteredPassword;
            }

            // 5. تأكيد الحذف
            DialogResult confirm = MessageBox.Show("هل أنت متأكد من حذف جميع القطع وحركات هذا الصنف؟", "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            try
            {
                string resultMessage = DBServiecs.Product_DeletePiecesAndMovementsByProductID(
                   ID_Product: productId,
                   adminPassword: adminPassword,
                   ExecutedByID: currentUserId,
                   reason: reason
               );

                if (resultMessage == "Good")
                {
                    CustomMessageBox.ShowInformation("تم حذف جميع القطع وما يخصها من حركات", "نجاح");
                    this.Close();
                }
                else
                {
                    CustomMessageBox.ShowWarning (resultMessage, "خطأ");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        #endregion
    }
}
