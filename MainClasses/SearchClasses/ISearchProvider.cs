using System.Data;
using System.Windows.Forms;

namespace MizanOriginalSoft.MainClasses.SearchClasses
{
    public interface ISearchProvider
    {
        /// <summary> عنوان شاشة البحث </summary>
        string Title { get; }

        /// <summary> إرجاع البيانات بناءً على الفلتر </summary>
        DataTable GetData(string filter);

        /// <summary> إرجاع الكود والاسم من الصف المحدد </summary>
        (string Code, string Name) GetSelectedItem(DataGridViewRow row);

        /// <summary> تطبيق تنسيقات الأعمدة على DataGridView </summary>
        void ApplyGridFormatting(DataGridView dgv);
    }
}


/*
 ثالثا :كلاس SearchEnums

using System;

namespace MizanOriginalSoft.MainClasses.SearchClasses
{
    /// <summary>
    /// نوع الكيان الذي سيتم البحث فيه
    /// </summary>
    public enum SearchEntityType
    {
        Accounts,    // البحث في الحسابات
        Products,    // البحث في الأصناف
        Categories,  // البحث في التصنيفات
        Invoices     // البحث في الفواتير
    }

    /// <summary>
    /// أنواع الحسابات (للعملاء/الموردين/الشركاء... إلخ)
    /// </summary>
    public enum AccountKind
    {
        Customers,   // العملاء
        Suppliers,   // الموردين
        Boths,       // كلاهما
        Parteners    // الشركاء
    }
}

 */



/*
 هذه الصورة العامة لكل الكلاسات فاين يكون الخلل

اولا كود الاستدعاء لمثال حسابات الموردين
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
                    lblSupplier_ID.Text = selectedAccount;  // اريد هنا ارجاه الكود
                    txtSupplierSelected .Text // هنا الاسم المختار

                }
            }
        }
 */

/*
 ثانيا شاشة البحث
using MizanOriginalSoft.MainClasses.SearchClasses;
using System;
using System.Data;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.MainForms
{
    public partial class frmGeneralSearch : Form
    {
        private readonly ISearchProvider _provider;

        public frmGeneralSearch(ISearchProvider provider)
        {
            InitializeComponent();
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));

            // ربط الحدث يدويًا
            this.Load += frmGeneralSearch_Load;
        }

        private void frmGeneralSearch_Load(object? sender, EventArgs e)
        {
            lblTitel.Text = _provider.Title;
            LoadData();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            var data = _provider.GetData(txtSearch.Text.Trim());
            DGV.DataSource = data;
            lblcountResulte.Text = $"عدد النتائج: {data.Rows.Count}";
            _provider.ApplyGridFormatting(DGV);
        }
        public class AccountSelection
        {
            public string? Code { get; set; }
            public string? Name { get; set; }
        }

        private void DGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string code = _provider.GetSelectedCode(DGV.Rows[e.RowIndex]);
                this.Tag = code;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}




 */



/*
 رابعا: كلاس GenericSearchProvider

using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MizanOriginalSoft.MainClasses.SearchClasses
{
    public class GenericSearchProvider : ISearchProvider
    {
        private readonly SearchEntityType _type;
        private readonly AccountKind? _accountKind;

        public GenericSearchProvider(SearchEntityType type, AccountKind? accountKind = null)
        {
            _type = type;
            _accountKind = accountKind;
        }

        public string Title => _type switch
        {
            SearchEntityType.Accounts => "البحث في الحسابات",
            SearchEntityType.Products => "البحث في الأصناف",
            SearchEntityType.Categories => "البحث في التصنيفات",
            SearchEntityType.Invoices => "البحث في الفواتير",
            _ => "بحث عام"
        };

        public DataTable GetData(string filter)
        {
            switch (_type)
            {
                case SearchEntityType.Accounts:
                    if (_accountKind == null)
                        return new DataTable();

                    // TODO: استدعاء بيانات الحسابات من قاعدة البيانات
                    // مثال: return DBServices.MainAcc_GetParentAccounts(_accountKind.Value.ToString());

                    var accounts = new DataTable();
                    accounts.Columns.Add("AccID");
                    accounts.Columns.Add("AccName");
                    accounts.Columns.Add("Balance");
                    accounts.Columns.Add("BalanceState");
                    // إضافة بيانات تجريبية
                    accounts.Rows.Add("1001", "شركة الاختبار", "1000", "دائن");
                    accounts.Rows.Add("1002", "مورد تجريبي", "500", "مدين");
                    return Filter(accounts, filter, new[] { "AccID", "AccName" });

                case SearchEntityType.Products:
                    // TODO: استدعاء بيانات المنتجات
                    return new DataTable();

                case SearchEntityType.Categories:
                    // TODO: استدعاء بيانات التصنيفات
                    return new DataTable();

                case SearchEntityType.Invoices:
                    // TODO: استدعاء بيانات الفواتير
                    return new DataTable();

                default:
                    return new DataTable();
            }
        }

        private DataTable Filter(DataTable dt, string filter, string[] columns)
        {
            if (string.IsNullOrWhiteSpace(filter)) return dt;

            var expr = string.Join(" OR ",
                columns.Select(c => $"{c} LIKE '%{filter.Replace("'", "''")}%'"));

            try
            {
                var rows = dt.Select(expr);
                return rows.Length > 0 ? rows.CopyToDataTable() : dt.Clone();
            }
            catch
            {
                return dt;
            }
        }

        public (string Code, string Name) GetSelectedItem(DataGridViewRow row)
        {
            if (row == null) return (string.Empty, string.Empty);

            var codeCol = row.DataGridView.Columns.Cast<DataGridViewColumn>()
                .FirstOrDefault(c => c.Name.ToLower().Contains("id") || c.Name.ToLower().Contains("code"));

            var nameCol = row.DataGridView.Columns.Cast<DataGridViewColumn>()
                .FirstOrDefault(c => c.Name.ToLower().Contains("name"));

            var code = codeCol != null ? row.Cells[codeCol.Name].Value?.ToString() ?? "" : "";
            var name = nameCol != null ? row.Cells[nameCol.Name].Value?.ToString() ?? "" : "";

            return (code, name);
        }

        public void ApplyGridFormatting(DataGridView dgv)
        {
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            if (_type == SearchEntityType.Accounts)
            {
                foreach (DataGridViewColumn col in dgv.Columns)
                    col.Visible = false;

                void Show(string name, string header, float weight)
                {
                    if (!dgv.Columns.Contains(name)) return;
                    var c = dgv.Columns[name];
                    c.Visible = true;
                    c.HeaderText = header;
                    c.FillWeight = weight;
                }

                Show("AccID", "كود", 1f);
                Show("AccName", "اسم الحساب", 3f);
                Show("Balance", "الرصيد", 1f);
                Show("BalanceState", "--", 1f);
            }
        }
    }
}

 */

/*
 اخيرا: كلاس SearchHelper

using System.Windows.Forms;
using MizanOriginalSoft.Views.Forms.MainForms;

namespace MizanOriginalSoft.MainClasses.SearchClasses
{
    public static class SearchHelper
    {
        /// <summary>
        /// دالة عامة لفتح شاشة البحث لأي نوع (حساب، صنف، فاتورة...)
        /// </summary>
        /// <param name="provider">مزود البحث (GenericSearchProvider أو أي Provider آخر)</param>
        /// <returns>Tuple يحتوي الكود والاسم</returns>
        public static (string Code, string Name) ShowSearchDialog(ISearchProvider provider)
        {
            using (var frm = new frmGeneralSearch(provider))
            {
                if (frm.ShowDialog() == DialogResult.OK && frm.Tag != null)
                {
                    // frm.Tag سيحمل Tuple (Code, Name)
                    return ((string Code, string Name))frm.Tag;
                }

                return (string.Empty, string.Empty);
            }
        }
    }
}

 */

/*اريد نسخة كاملة نظيفة لغرض البحث العام حسب نوع البحث مع الاشارة لاماكن وضع دوال استدعاء البيانات المختلفة وتكون معلقة حتى نحدثها واحدا بعد الاخر*/

