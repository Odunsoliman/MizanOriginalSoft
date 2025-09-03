using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MizanOriginalSoft.MainClasses.SearchClasses
{
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
                DataTable dt;

                switch (_type)
                {
                    case SearchEntityType.Accounts:
                        if (_accountKind == null)
                            return new DataTable();

                        // جلب البيانات من قاعدة البيانات
                        dt = DBServiecs.MainAcc_GetParentAccounts(_accountKind.Value.ToString());

                        // تفعيل الفلترة
                        return Filter(dt, filter, new[] { "AccID", "AccName" });

                    case SearchEntityType.Products:
                        dt = new DataTable();
                        // TODO: جلب المنتجات
                        return Filter(dt, filter, new[] { "ProductID", "ProductName" });

                    case SearchEntityType.Categories:
                        dt = new DataTable();
                        // TODO: جلب التصنيفات
                        return Filter(dt, filter, new[] { "CategoryID", "CategoryName" });

                    case SearchEntityType.Invoices:
                        dt = new DataTable();
                        // TODO: جلب الفواتير
                        return Filter(dt, filter, new[] { "InvoiceID", "InvoiceNumber" });

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
                if (row == null || row.DataGridView == null)
                    return (string.Empty, string.Empty);

                var columns = row.DataGridView.Columns.Cast<DataGridViewColumn>();

                var codeCol = columns.FirstOrDefault(c =>
                    c.Name.Contains("id", StringComparison.OrdinalIgnoreCase) ||
                    c.Name.Contains("code", StringComparison.OrdinalIgnoreCase));

                var nameCol = columns.FirstOrDefault(c =>
                    c.Name.Contains("name", StringComparison.OrdinalIgnoreCase));

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

}
