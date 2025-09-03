using MizanOriginalSoft.MainClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MizanOriginalSoft.Views.Forms.MainForms.frmSearch;

namespace MizanOriginalSoft.Views.Forms.MainForms
{
    public class SearchResult
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public SearchResult(string code, string name)
        {
            Code = code;
            Name = name;
        }
    }

    public partial class frmOriginalSearch : Form
    {
        public enum SearchInWate
        {
            Product = 0,  //الاصناف
            Customer = 7, // العملاء
            Supplier = 14, // الموردين
            AllAccounts = 200, // العملاء + الموردين
            Product_Pieces = 201,//قطع الصنف 
            Invoice=1

        }

        #region Fields
        private SearchInWate _searchMode;
        private DataTable _dtResults = new DataTable();

        #endregion

        #region Constructor
        public frmOriginalSearch(SearchInWate searchMode)
        {
            InitializeComponent();
            _searchMode = searchMode;
        }
        #endregion

        #region Form Load
        private void frmOriginalSearch_Load(object sender, EventArgs e)
        {
            InitializeForm();
            LoadData();
        }
        #endregion

        #region Initialization
        private void InitializeForm()
        {
            // إعدادات عامة للفورم
            txtSearch.Focus();
            DGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGV.ReadOnly = true;
        }
        #endregion

        #region Load Data
        private void LoadData()
        {
            switch (_searchMode)
            {
                case SearchInWate.Product:
                    _dtResults = LoadProducts();
                    FormatProductGrid();
                    lblTitel.Text = "بحث الأصناف";
                    break;
                case SearchInWate.Customer:
                    _dtResults = LoadCustomers();
                    FormatAccountsGrid();
                    lblTitel.Text = "بحث العملاء جزء من اسم او هاتف اوعنوان";
                    break;
                case SearchInWate.Supplier:
                    _dtResults = LoadSuppliers();
                    FormatAccountsGrid();
                    lblTitel.Text = "بحث الموردين جزء من اسم او هاتف اوعنوان";
                    break;
                case SearchInWate.AllAccounts:
                    _dtResults = LoadAllAccounts();
                    FormatAccountsGrid();
                    lblTitel.Text = "بحث جميع الحسابات جزء من اسم او هاتف اوعنوان";
                    break;
                case SearchInWate.Product_Pieces:
                    _dtResults = LoadProductPieces();
                    FormatProductPiecesGrid();
                    lblTitel.Text = "بحث قطع الأصناف";
                    break;
                case SearchInWate.Invoice:
                    _dtResults = LoadInvoices();
                    FormatInvoiceGrid();
                    lblTitel.Text = "بحث الفواتير";
                    break;
                default:
                    _dtResults = new DataTable();
                    break;
            }


            lblcountResulte.Text = $"عدد النتائج: {_dtResults.Rows.Count}";
        }
        #endregion

        #region Data Retrieval

        private DataTable LoadProducts()
        {
            // مثال: استدعاء قاعدة البيانات لإحضار الأصناف
            _dtResults = DBServiecs.ProductSearch_GetAll();
            DGV.DataSource = _dtResults;

            return _dtResults;

   
        }


        private DataTable LoadCustomers()
        {
            _dtResults = DBServiecs.MainAcc_GetAccounts((int)SearchEntityType.Customer );
            DGV.DataSource = _dtResults;

            return _dtResults;
        }

        private DataTable LoadSuppliers()
        {
            _dtResults = DBServiecs.MainAcc_GetAccounts((int)SearchEntityType.Supplier);
            DGV.DataSource = _dtResults;

            return _dtResults;
        }

        private DataTable LoadAllAccounts()
        {
            _dtResults = DBServiecs.MainAcc_GetAccounts((int)SearchEntityType.Boths);
            DGV.DataSource = _dtResults;

            return _dtResults;

        }

        private DataTable LoadInvoices()
        {
            _dtResults = DBServiecs.MainAcc_GetAccounts((int)SearchEntityType.Boths);
            DGV.DataSource = _dtResults;

            return _dtResults;

        }
 

        private DataTable LoadProductPieces()
        {
            return DBServiecs.User_GetActiv();
        }

        #endregion

        #region Grid Formatting
        private void FormatProductGrid()
        {
            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            // أخفي جميع الأعمدة أولاً
            foreach (DataGridViewColumn col in DGV.Columns)
            {
                col.Visible = false;
            }
            // اظهار الأعمدة المطلوبة فقط وتغيير عناوينها
            if (DGV.Columns.Contains("ProductCode"))
            {
                DGV.Columns["ProductCode"].Visible = true;
                DGV.Columns["ProductCode"].HeaderText = "الكود";
                DGV.Columns["ProductCode"].FillWeight = 1;
            }
            if (DGV.Columns.Contains("ProdName"))
            {
                DGV.Columns["ProdName"].Visible = true;
                DGV.Columns["ProdName"].HeaderText = "اسم الصنف";
                DGV.Columns["ProdName"].FillWeight = 4;
            }
            if (DGV.Columns.Contains("U_Price"))
            {
                DGV.Columns["U_Price"].Visible = true;
                DGV.Columns["U_Price"].HeaderText = "اسم الصنف";
                DGV.Columns["U_Price"].FillWeight = 1;

            }

            ApplyDGVStyles();

            if (DGV.Columns.Contains("ProductCode"))
                DGV.Sort(DGV.Columns["ProductCode"], ListSortDirection.Ascending);
        }

        private void FormatAccountsGrid()
        {
            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // أخفي جميع الأعمدة أولاً
            foreach (DataGridViewColumn col in DGV.Columns)
            {
                col.Visible = false;
            }

            // اظهار الأعمدة المطلوبة فقط وتغيير عناوينها
            if (DGV.Columns.Contains("AccName"))
            {
                DGV.Columns["AccName"].Visible = true;
                DGV.Columns["AccName"].HeaderText = "الاسم";
            }

            if (DGV.Columns.Contains("FirstPhon"))
            {
                DGV.Columns["FirstPhon"].Visible = true;
                DGV.Columns["FirstPhon"].HeaderText = "الهاتف";
            }

            if (DGV.Columns.Contains("AntherPhon"))
            {
                DGV.Columns["AntherPhon"].Visible = true;
                DGV.Columns["AntherPhon"].HeaderText = "-";
            }

            // تطبيق التنسيقات العامة
            ApplyDGVStyles();

            // ترتيب حسب الاسم
            if (DGV.Columns.Contains("AccName"))
                DGV.Sort(DGV.Columns["AccName"], ListSortDirection.Ascending);
        }

        private void FormatInvoiceGrid()
        {
            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // أخفي جميع الأعمدة أولاً
            foreach (DataGridViewColumn col in DGV.Columns)
            {
                col.Visible = false;
            }

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

            // تطبيق التنسيقات العامة
            ApplyDGVStyles();

            // ترتيب حسب الاسم
            if (DGV.Columns.Contains("AccName"))
                DGV.Sort(DGV.Columns["AccName"], ListSortDirection.Ascending);
        }

        private void FormatProductPiecesGrid()
        {
            DGV.Columns["PieceID"].HeaderText = "كود القطعة";
            DGV.Columns["PieceName"].HeaderText = "اسم القطعة";
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
        #endregion

        #region Search Filter
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (_dtResults == null) return;

            string filter = txtSearch.Text.Trim().Replace("'", "''");
            DataView dv = _dtResults.DefaultView;

            switch (_searchMode)
            {
                case SearchInWate.Product:
                    dv.RowFilter = $"ProductName LIKE '%{filter}%' OR ProductID LIKE '%{filter}%'";
                    break;
                case SearchInWate.Customer:
                case SearchInWate.Supplier:
                case SearchInWate.AllAccounts:
                    dv.RowFilter = $"AccName LIKE '%{filter}%' OR FirstPhon LIKE '%{filter}%' OR AntherPhon LIKE '%{filter}%' OR ClientAddress LIKE '%{filter}%'";
                    break;
                case SearchInWate.Product_Pieces:
                    dv.RowFilter = $"PieceName LIKE '%{filter}%' OR PieceID LIKE '%{filter}%'";
                    break;
            }

            lblcountResulte.Text = $"عدد النتائج: {dv.Count}";
        }
        #endregion

        #region DGV Double Click
        private void DGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var selectedRow = DGV.Rows[e.RowIndex];
            string selectedCode = "";
            string selectedName = "";

            switch (_searchMode)
            {
                case SearchInWate.Product:
                    selectedCode = selectedRow.Cells["ProductID"].Value?.ToString() ?? "";
                    selectedName = selectedRow.Cells["ProductName"].Value?.ToString() ?? "";
                    break;

                case SearchInWate.Customer:
                case SearchInWate.Supplier:
                case SearchInWate.AllAccounts:
                    selectedCode = selectedRow.Cells["AccID"].Value?.ToString() ?? "";
                    selectedName = selectedRow.Cells["AccName"].Value?.ToString() ?? "";
                    break;

                case SearchInWate.Product_Pieces:
                    selectedCode = selectedRow.Cells["PieceID"].Value?.ToString() ?? "";
                    selectedName = selectedRow.Cells["PieceName"].Value?.ToString() ?? "";
                    break;
            }

            // تخزين النتيجة في Tag ككائن
            this.Tag = new SearchResult(selectedCode, selectedName);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }


        private void DGV_SelectionChanged(object sender, EventArgs e)
        {
            if (DGV.CurrentRow != null && DGV.CurrentRow.Cells.Count > 0)
            {
                var value = DGV.CurrentRow.Cells[0].Value;  // العمود رقم 0
                lblID.Text = value != null ? value.ToString() : string.Empty;
            }
        }

        private void DGV_DoubleClick(object sender, EventArgs e)
        {

        }


        #endregion

 

        /*
         اريد استخدام هذه الشاشة فى البحث من اى مكان فى البرنامج 
        الدخول عليها ب الضغط على ctrl+F من اى جزء معنى فى البرنامج 
        مبدئيا SearchInWate تحدد الاشياء التى سوف ابحث عنها بتمرير رقم معين اليها 
        يوجد فى الشاشة تكست  و2 ليبل وداتا جريد
        txtSearch , lblcountResulte ,lblTitel  , DGV 
        عند الفتح من مكان ما يتم تمرير SearchInWate فيتم بناء على ذلك استدعاء البيانات اللازمة للنوع وتعبئة DGV بها
        وكل نوع تعبئة يكون له تنسيق معين معرف فى دالة خاصة
        مبدئيا الاصناف بشكل مختلف عن الحسابات سواء عملاء او موردين
        وقطع الاصناف لها تنسيق خاص
        وامكانية الزيادة فى المستقبل
        عند النقر المزدوج على الجريد يتم نقل الكود المبحوث عنه الى الشاشة الرئيسية التى فتح منها واغلاق الشاشة
        لامكانية التطوير والمعالجة اريد كل عملية لها ريجون باسمها لسهولة معالجة الكود
        فما السيناريو الذى تقترحه لكي يكون كود محترف ببنية سهلة التطوير والصيانة
         */

    }
}
