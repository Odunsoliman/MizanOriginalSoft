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
                        dt = DBServiecs.Product_GetAll();
                        return Filter(dt, filter, new[] { "ProductCode", "ProdName" });

                    case SearchEntityType.Categories:
                        dt = new DataTable();
                        // TODO: جلب التصنيفات
                        return Filter(dt, filter, new[] { "CategoryID", "CategoryName" });

                    case SearchEntityType.Invoices:
                        dt = new DataTable();
                        dt = DBServiecs.NewInvoice_GetInvoicesByType(1);
                        return Filter(dt, filter, new[] { "Inv_ID", "Inv_Counter" });

                    default:
                        return new DataTable();
                }
            }

            private DataTable Filter(DataTable dt, string filter, string[] columns)
            {
                if (string.IsNullOrWhiteSpace(filter)) return dt;

                // تأمين الفلتر ضد علامات '
                string safeFilter = filter.Replace("'", "''");

                // تحويل كل الأعمدة لنص باستخدام CONVERT
                var expr = string.Join(" OR ",
                    columns.Select(c => $"CONVERT({c}, 'System.String') LIKE '%{safeFilter}%'"));

                try
                {
                    var rows = dt.Select(expr);
                    return rows.Length > 0 ? rows.CopyToDataTable() : dt.Clone();
                }
                catch (Exception ex)
                {
                    // Debugging
                    System.Diagnostics.Debug.WriteLine("Filter error: " + ex.Message);
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

            // 🔹 دالة توزيع التنسيقات
            public void ApplyGridFormatting(DataGridView dgv)
            {
                // 1️⃣ التنسيق الموحد أولاً
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
                dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                foreach (DataGridViewColumn col in dgv.Columns)
                    col.Visible = false;

                // 2️⃣ ثم توزيع التنسيقات
                if (_type == SearchEntityType.Accounts)
                    ApplyAccountsGridFormatting(dgv);
                else if (_type == SearchEntityType.Products)
                    ApplyProductsGridFormatting(dgv);
                else if (_type == SearchEntityType.Invoices )
                    ApplyInvoiceGridFormatting(dgv);
            }

            // 🔹 الدالة الحالية تبقى لحسابات فقط
            public void ApplyAccountsGridFormatting(DataGridView dgv)
            {
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

            // 🔹 دالة جديدة لتنسيق الأصناف
            public void ApplyProductsGridFormatting(DataGridView dgv)
            {
                void Show(string name, string header, float weight)
                {
                    if (!dgv.Columns.Contains(name)) return;
                    var c = dgv.Columns[name];
                    c.Visible = true;
                    c.HeaderText = header;
                    c.FillWeight = weight;
                }

                Show("ProductCode", "كود", 1f);
                Show("ProdName", "اسم الصنف", 3f);
                Show("RegistYear", "سنة", 1f);
                Show("U_Price", "السعر", 1f);
                Show("ProductStock", "الرصيد", 1f);
                Show("NoteProduct", "ملاحظات الصنف", 4f);
            }

            // 🔹 دالة جديدة لتنسيق الفواتير
            public void ApplyInvoiceGridFormatting(DataGridView dgv)
            {
                void Show(string name, string header, float weight)
                {
                    if (!dgv.Columns.Contains(name)) return;
                    var c = dgv.Columns[name];
                    c.Visible = true;
                    c.HeaderText = header;
                    c.FillWeight = weight;
                }

                Show("ProductCode", "كود", 1f);
                Show("ProdName", "اسم الصنف", 3f);
                Show("RegistYear", "سنة", 1f);
                Show("U_Price", "السعر", 1f);
                Show("ProductStock", "الرصيد", 1f);
                Show("NoteProduct", "ملاحظات الصنف", 4f);
            }



        }
    }

}
