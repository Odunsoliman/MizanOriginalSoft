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
            DataTable dt = new();

            switch (_type)
            {
                case SearchEntityType.Accounts:
                    if (_accountKind == null) return dt;
                    dt = DBServiecs.MainAcc_GetParentAccounts(_accountKind.Value.ToString());
                    return Filter(dt, filter, new[] { "AccID", "AccName", "Phone" });

                case SearchEntityType.Products:
                    dt = DBServiecs.Product_GetAll();
                    return Filter(dt, filter, new[] { "ProductCode", "ProdName", "CategoryName" });

                case SearchEntityType.Categories:
                    // TODO: إضافة جلب بيانات التصنيفات
                    return Filter(dt, filter, new[] { "CategoryID", "CategoryName" });

                case SearchEntityType.Invoices:
                    dt = DBServiecs.NewInvoice_GetInvoicesByType(1);
                    return Filter(dt, filter, new[] { "Inv_Counter", "AccName", "SalesPersonName" });

                default:
                    return dt;
            }
        }

        private DataTable Filter(DataTable dt, string filter, string[] columns)
        {
            if (string.IsNullOrWhiteSpace(filter)) return dt;

            string safeFilter = filter.Replace("'", "''");
            string expr = string.Join(" OR ",
                columns.Select(c => $"CONVERT({c}, 'System.String') LIKE '%{safeFilter}%'"));

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
            if (row == null || row.DataGridView == null)
                return (string.Empty, string.Empty);

            var columns = row.DataGridView.Columns.Cast<DataGridViewColumn>();

            var codeCol = columns.FirstOrDefault(c =>
                c.Name.Contains("id", StringComparison.OrdinalIgnoreCase) ||
                c.Name.Contains("code", StringComparison.OrdinalIgnoreCase));

            var nameCol = columns.FirstOrDefault(c =>
                c.Name.Contains("name", StringComparison.OrdinalIgnoreCase));

            string code = codeCol != null ? row.Cells[codeCol.Name].Value?.ToString() ?? "" : "";
            string name = nameCol != null ? row.Cells[nameCol.Name].Value?.ToString() ?? "" : "";

            return (code, name);
        }

        public void ApplyGridFormatting(DataGridView dgv)
        {
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewColumn col in dgv.Columns)
                col.Visible = false;

            switch (_type)
            {
                case SearchEntityType.Accounts:
                    ApplyAccountsGridFormatting(dgv);
                    break;
                case SearchEntityType.Products:
                    ApplyProductsGridFormatting(dgv);
                    break;
                case SearchEntityType.Invoices:
                    ApplyInvoiceGridFormatting(dgv);
                    break;
            }
        }

        private void ShowColumn(DataGridView dgv, string name, string header, float weight, string? format = null)
        {
            if (!dgv.Columns.Contains(name)) return;
            var c = dgv.Columns[name];
            c.Visible = true;
            c.HeaderText = header;
            c.FillWeight = weight;

            if (!string.IsNullOrEmpty(format))
            {
                c.DefaultCellStyle.Format = format;
                c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            else
            {
                c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void ApplyAccountsGridFormatting(DataGridView dgv)
        {
            ShowColumn(dgv, "AccID", "كود", 1f);
            ShowColumn(dgv, "AccName", "اسم الحساب", 3f);
            ShowColumn(dgv, "Balance", "الرصيد", 1f, "N2");
            ShowColumn(dgv, "BalanceState", "--", 1f);
        }

        private void ApplyProductsGridFormatting(DataGridView dgv)
        {
            ShowColumn(dgv, "ProductCode", "كود", 1f);
            ShowColumn(dgv, "ProdName", "اسم الصنف", 3f);
            ShowColumn(dgv, "RegistYear", "سنة", 1f);
            ShowColumn(dgv, "U_Price", "السعر", 1f, "N2");
            ShowColumn(dgv, "ProductStock", "الرصيد", 1f, "N2");
            ShowColumn(dgv, "NoteProduct", "ملاحظات الصنف", 4f);
        }

        private void ApplyInvoiceGridFormatting(DataGridView dgv)
        {
            ShowColumn(dgv, "Inv_Counter", "رقم الفاتورة", 1f);
            ShowColumn(dgv, "AccName", "اسم العميل/المورد", 3f);
            ShowColumn(dgv, "SalesPersonName", "البائع", 2f);
            ShowColumn(dgv, "Inv_Date", "التاريخ", 1.5f, "yyyy/MM/dd");
            ShowColumn(dgv, "NetTotal", "صافى الفاتورة", 1.5f, "N2");
        }
    }
}
