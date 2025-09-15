
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
            SearchEntityType.SaleInvoices => "البحث في الفواتير البيع",
            SearchEntityType.PurchaseInvoices  => "البحث في الفواتيرالشراء",
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

                case SearchEntityType.SaleInvoices:
                    dt = DBServiecs.NewInvoice_GetInvoicesByType(1);
                    return Filter(dt, filter, new[] { "Inv_Counter", "AccName", "SellerName" });

                case SearchEntityType.PurchaseInvoices :
                    dt = DBServiecs.NewInvoice_GetInvoicesByType(3);
                    return Filter(dt, filter, new[] { "Inv_Counter", "AccName", "SellerName" });

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
                case SearchEntityType.SaleInvoices:
                    ApplySaleInvoiceGridFormatting(dgv);
                    break;
                case SearchEntityType.PurchaseInvoices :
                    ApplyPurchaseInvoicesGridFormatting(dgv);
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

        private void ApplySaleInvoiceGridFormatting(DataGridView dgv)
        {
            ShowColumn(dgv, "Inv_Counter", "رقم الفاتورة", 1f);
            ShowColumn(dgv, "AccName", "اسم العميل", 3f);
            ShowColumn(dgv, "SellerName", "البائع", 2f);
            ShowColumn(dgv, "Inv_Date", "التاريخ", 1.5f, "yyyy/MM/dd");
            ShowColumn(dgv, "NetTotal", "صافى الفاتورة", 1.5f, "N2");
        }
        private void ApplyPurchaseInvoicesGridFormatting(DataGridView dgv)
        {
            ShowColumn(dgv, "Inv_Counter", "رقم الفاتورة", 1f);
            ShowColumn(dgv, "AccName", "اسم المورد", 3f);
            ShowColumn(dgv, "SellerName", "المشتري", 2f);
            ShowColumn(dgv, "Inv_Date", "التاريخ", 1.5f, "yyyy/MM/dd");
            ShowColumn(dgv, "NetTotal", "صافى الفاتورة", 1.5f, "N2");
        }
    }
}




//using System;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Windows.Forms;

//namespace MizanOriginalSoft.MainClasses.SearchClasses
//{
//    public class GenericSearchProvider : ISearchProvider
//    {
//        private readonly SearchEntityType _type;
//        private readonly AccountKind? _accountKind;
//        private readonly int? _invoiceTypeId; // 🔹 نوع الفاتورة (بيع = 1، شراء = 3...)

//        // 🔹 خصائص للوصول للقيم
//        public SearchEntityType EntityType => _type;
//        public int? InvoiceTypeId => _invoiceTypeId;

//        public GenericSearchProvider(SearchEntityType type, AccountKind? accountKind = null, int? invoiceTypeId = null)
//        {
//            _type = type;
//            _accountKind = accountKind;
//            _invoiceTypeId = invoiceTypeId;
//        }

//        public string Title => _type switch
//        {
//            SearchEntityType.Accounts => "البحث في الحسابات",
//            SearchEntityType.Products => "البحث في الأصناف",
//            SearchEntityType.Categories => "البحث في التصنيفات",
//            SearchEntityType.Invoices => _invoiceTypeId == 3 ? "البحث في فواتير الشراء" : "البحث في فواتير البيع",
//            _ => "بحث عام"
//        };


//        public DataTable GetData(string filter)
//        {
//            DataTable dt = new();

//            switch (_type)
//            {
//                case SearchEntityType.Accounts:
//                    if (_accountKind == null) return dt;
//                    // استدعاء الدالة كما كانت: نرسل اسم النوع كنص
//                    dt = DBServiecs.MainAcc_GetParentAccounts(_accountKind.Value.ToString());
//                    return FilterWithExistingColumns(dt, filter, new[] { "AccID", "AccName", "Phone" });

//                case SearchEntityType.Products:
//                    dt = DBServiecs.Product_GetAll();
//                    return FilterWithExistingColumns(dt, filter, new[] { "ProductCode", "ProdName", "CategoryName" });

//                case SearchEntityType.Categories:
//                    // حاول جلب التصنيفات إن كانت الدالة موجودة، وإلا اعد جدول فارغ
//                    try
//                    {
//                        // استبدل الاسم لو كان مختلفاً في DBServiecs
//                        dt = DBServiecs.Categories_GetAll();
//                    }
//                    catch
//                    {
//                        dt = new DataTable();
//                    }
//                    return FilterWithExistingColumns(dt, filter, new[] { "CategoryID", "CategoryName" });

//                case SearchEntityType.Invoices:
//                    // إذا تم تحديد نوع الفاتورة فاجلبه، وإلا اجلب كل الأنواع المعروفة (1 و 3) وادمجهم
//                    DataTable invoicesDt = new DataTable();
//                    if (_invoiceTypeId.HasValue)
//                    {
//                        invoicesDt = DBServiecs.NewInvoice_GetInvoicesByType(_invoiceTypeId.Value);
//                    }
//                    else
//                    {
//                        // fallback: جلب بيع وشراء ودمج
//                        DataTable dtSell = null;//Converting null literal or possible null value to non-nullable type.
//                        DataTable dtBuy = null;
//                        try { dtSell = DBServiecs.NewInvoice_GetInvoicesByType(1); } catch { /* ignore */ }
//                        try { dtBuy = DBServiecs.NewInvoice_GetInvoicesByType(3); } catch { /* ignore */ }

//                        if (dtSell != null && dtSell.Rows.Count > 0)
//                            invoicesDt = dtSell.Clone();
//                        else if (dtBuy != null && dtBuy.Rows.Count > 0)
//                            invoicesDt = dtBuy.Clone();
//                        else
//                            invoicesDt = new DataTable();

//                        if (dtSell != null && dtSell.Rows.Count > 0) invoicesDt.Merge(dtSell);
//                        if (dtBuy != null && dtBuy.Rows.Count > 0) invoicesDt.Merge(dtBuy);
//                    }

//                    // لو ما في بيانات رجع جدول فارغ بعد فلترة الأعمدة المتاحة
//                    if (invoicesDt == null || invoicesDt.Columns.Count == 0 || invoicesDt.Rows.Count == 0)
//                        return FilterWithExistingColumns(invoicesDt ?? new DataTable(), filter, new[] { "Inv_Counter", "AccName", "SellerName" });

//                    // ترتيب تنازلي حسب التاريخ مع تعامل آمن مع قيم التاريخ الفارغة/غير صالحة
//                    var ordered = invoicesDt.AsEnumerable()
//                        .OrderByDescending(r =>
//                        {
//                            if (DateTime.TryParse(r["Inv_Date"]?.ToString(), out DateTime d)) return d;
//                            return DateTime.MinValue;
//                        });

//                    var sortedDt = ordered.Any() ? ordered.CopyToDataTable() : invoicesDt.Clone();

//                    return FilterWithExistingColumns(sortedDt, filter, new[] { "Inv_Counter", "AccName", "SellerName" });

//                default:
//                    return dt;
//            }
//        }

//        // دالة مساعدة تتأكد أولاً أن الأعمدة المطلوبة موجودة في الجدول قبل عمل الفلترة
//        private DataTable FilterWithExistingColumns(DataTable dt, string filter, string[] requestedColumns)
//        {
//            if (dt == null || dt.Columns.Count == 0) return dt ?? new DataTable();
//            if (string.IsNullOrWhiteSpace(filter)) return dt;

//            // إلاعمدة الموجودة فعلاً
//            var existing = requestedColumns.Where(c => dt.Columns.Contains(c)).ToArray();
//            if (existing.Length == 0) return dt;

//            // بناء تعبير آمن للـ Select
//            string safe = filter.Replace("'", "''");
//            string expr = string.Join(" OR ",
//                existing.Select(c => $"CONVERT([{c}], 'System.String') LIKE '%{safe}%'"));

//            try
//            {
//                var rows = dt.Select(expr);
//                return rows.Length > 0 ? rows.CopyToDataTable() : dt.Clone();
//            }
//            catch
//            {
//                return dt;
//            }
//        }


//        //public DataTable GetData(string filter)
//        //{
//        //    DataTable dt = new();

//        //    switch (_type)
//        //    {
//        //        case SearchEntityType.Accounts:
//        //            if (_accountKind == null) return dt;
//        //            dt = DBServiecs.MainAcc_GetParentAccounts(_accountKind.Value.ToString());
//        //            return Filter(dt, filter, new[] { "AccID", "AccName", "Phone" });

//        //        case SearchEntityType.Products:
//        //            dt = DBServiecs.Product_GetAll();
//        //            return Filter(dt, filter, new[] { "ProductCode", "ProdName", "CategoryName" });

//        //        case SearchEntityType.Categories:
//        //            // 🔹 تصنيفات (إن وجدت)
//        //            return Filter(dt, filter, new[] { "CategoryID", "CategoryName" });

//        //        case SearchEntityType.Invoices:
//        //            int typeId = _invoiceTypeId ?? 1; // 🔹 افتراضي بيع لماذا تجعل افتراضى = 1 فيجعل دائما احضار فواتير البيع فقط فى كل الحالات
//        //            dt = DBServiecs.NewInvoice_GetInvoicesByType(typeId);

//        //            // 🔹 ترتيب الفواتير حسب التاريخ تنازلي
//        //            var sortedRows = dt.AsEnumerable()
//        //                .OrderByDescending(r =>
//        //                {
//        //                    DateTime.TryParse(r["Inv_Date"]?.ToString(), out DateTime date);
//        //                    return date;
//        //                });

//        //            dt = sortedRows.Any() ? sortedRows.CopyToDataTable() : dt.Clone();

//        //            return Filter(dt, filter, new[] { "Inv_Counter", "AccName", "SellerName" });

//        //        default:
//        //            return dt;
//        //    }
//        //}

//        private DataTable Filter(DataTable dt, string filter, string[] columns)
//        {
//            if (string.IsNullOrWhiteSpace(filter)) return dt;

//            string safeFilter = filter.Replace("'", "''");
//            string expr = string.Join(" OR ",
//                columns.Select(c => $"CONVERT({c}, 'System.String') LIKE '%{safeFilter}%'"));

//            try
//            {
//                var rows = dt.Select(expr);
//                return rows.Length > 0 ? rows.CopyToDataTable() : dt.Clone();
//            }
//            catch
//            {
//                return dt;
//            }
//        }

//        public (string Code, string Name) GetSelectedItem(DataGridViewRow row)
//        {
//            if (row == null || row.DataGridView == null)
//                return (string.Empty, string.Empty);

//            var columns = row.DataGridView.Columns.Cast<DataGridViewColumn>();

//            var codeCol = columns.FirstOrDefault(c =>
//                c.Name.Contains("id", StringComparison.OrdinalIgnoreCase) ||
//                c.Name.Contains("code", StringComparison.OrdinalIgnoreCase));

//            var nameCol = columns.FirstOrDefault(c =>
//                c.Name.Contains("name", StringComparison.OrdinalIgnoreCase));

//            string code = codeCol != null ? row.Cells[codeCol.Name].Value?.ToString() ?? "" : "";
//            string name = nameCol != null ? row.Cells[nameCol.Name].Value?.ToString() ?? "" : "";

//            return (code, name);
//        }

//        public void ApplyGridFormatting(DataGridView dgv)
//        {
//            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
//            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
//            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
//            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
//            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

//            foreach (DataGridViewColumn col in dgv.Columns)
//                col.Visible = false;

//            switch (_type)
//            {
//                case SearchEntityType.Accounts:
//                    ApplyAccountsGridFormatting(dgv);
//                    break;
//                case SearchEntityType.Products:
//                    ApplyProductsGridFormatting(dgv);
//                    break;
//                case SearchEntityType.Invoices:
//                    ApplyInvoiceGridFormatting(dgv);
//                    break;
//            }
//        }

//        private void ShowColumn(DataGridView dgv, string name, string header, float weight, string? format = null)
//        {
//            if (!dgv.Columns.Contains(name)) return;
//            var c = dgv.Columns[name];
//            c.Visible = true;
//            c.HeaderText = header;
//            c.FillWeight = weight;

//            if (!string.IsNullOrEmpty(format))
//            {
//                c.DefaultCellStyle.Format = format;
//                c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
//            }
//            else
//            {
//                c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
//            }

//            c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
//        }

//        private void ApplyAccountsGridFormatting(DataGridView dgv)
//        {
//            ShowColumn(dgv, "AccID", "كود", 1f);
//            ShowColumn(dgv, "AccName", "اسم الحساب", 3f);
//            ShowColumn(dgv, "Balance", "الرصيد", 1f, "N2");
//            ShowColumn(dgv, "BalanceState", "--", 1f);
//        }

//        private void ApplyProductsGridFormatting(DataGridView dgv)
//        {
//            ShowColumn(dgv, "ProductCode", "كود", 1f);
//            ShowColumn(dgv, "ProdName", "اسم الصنف", 3f);
//            ShowColumn(dgv, "RegistYear", "سنة", 1f);
//            ShowColumn(dgv, "U_Price", "السعر", 1f, "N2");
//            ShowColumn(dgv, "ProductStock", "الرصيد", 1f, "N2");
//            ShowColumn(dgv, "NoteProduct", "ملاحظات الصنف", 4f);
//        }

//        private void ApplyInvoiceGridFormatting(DataGridView dgv)
//        {
//            ShowColumn(dgv, "Inv_Counter", "رقم الفاتورة", 1f);
//            ShowColumn(dgv, "AccName", "اسم العميل/المورد", 3f);
//            ShowColumn(dgv, "SellerName", "البائع", 2f);
//            ShowColumn(dgv, "Inv_Date", "التاريخ", 1.5f, "yyyy/MM/dd");
//            ShowColumn(dgv, "NetTotal", "صافى الفاتورة", 1.5f, "N2");
//        }
//    }
//}






////using System;
////using System.Data;
////using System.Drawing;
////using System.Linq;
////using System.Windows.Forms;

////namespace MizanOriginalSoft.MainClasses.SearchClasses
////{
////    public class GenericSearchProvider : ISearchProvider
////    {
////        private readonly SearchEntityType _type;
////        private readonly AccountKind? _accountKind;
////        private readonly int _invoiceTypeId; // 1 = بيع, 3 = شراء

////        public GenericSearchProvider(SearchEntityType type, AccountKind? accountKind = null, int invoiceTypeId = 1)
////        {
////            _type = type;
////            _accountKind = accountKind;
////            _invoiceTypeId = invoiceTypeId;
////        }

////        public string Title => _type switch
////        {
////            SearchEntityType.Accounts => "البحث في الحسابات",
////            SearchEntityType.Products => "البحث في الأصناف",
////            SearchEntityType.Categories => "البحث في التصنيفات",
////            SearchEntityType.Invoices => _invoiceTypeId == 1 ? "بحث في فواتير البيع" :
////                                         _invoiceTypeId == 3 ? "بحث في فواتير الشراء" : "بحث في الفواتير",
////            _ => "بحث عام"
////        };

////        public DataTable GetData(string filter)
////        {
////            DataTable dt = new();

////            switch (_type)
////            {
////                case SearchEntityType.Accounts:
////                    if (_accountKind == null) return dt;
////                    dt = DBServiecs.MainAcc_GetParentAccounts(_accountKind.Value.ToString());
////                    return Filter(dt, filter, new[] { "AccID", "AccName", "Phone" });

////                case SearchEntityType.Products:
////                    dt = DBServiecs.Product_GetAll();
////                    return Filter(dt, filter, new[] { "ProductCode", "ProdName", "CategoryName" });

////                case SearchEntityType.Categories:
////                    // 🔹 إضافة جلب بيانات التصنيفات لاحقًا
////                    return Filter(dt, filter, new[] { "CategoryID", "CategoryName" });

////                case SearchEntityType.Invoices:
////                    dt = DBServiecs.NewInvoice_GetInvoicesByType(_invoiceTypeId);

////                    // 🔹 ترتيب تنازلي بالتاريخ
////                    var orderedRows = dt.AsEnumerable()
////                        .OrderByDescending(r => r.Field<DateTime>("Inv_Date"));
////                    DataTable orderedDt = dt.Clone();
////                    foreach (var row in orderedRows)
////                        orderedDt.ImportRow(row);

////                    return Filter(orderedDt, filter, new[] { "Inv_Counter", "AccName", "SellerName" });

////                default:
////                    return dt;
////            }
////        }

////        private DataTable Filter(DataTable dt, string filter, string[] columns)
////        {
////            if (string.IsNullOrWhiteSpace(filter)) return dt;

////            string safeFilter = filter.Replace("'", "''");
////            string expr = string.Join(" OR ",
////                columns.Select(c => $"CONVERT({c}, 'System.String') LIKE '%{safeFilter}%'"));

////            try
////            {
////                var rows = dt.Select(expr);
////                return rows.Length > 0 ? rows.CopyToDataTable() : dt.Clone();
////            }
////            catch
////            {
////                return dt;
////            }
////        }

////        public (string Code, string Name) GetSelectedItem(DataGridViewRow row)
////        {
////            if (row == null || row.DataGridView == null)
////                return (string.Empty, string.Empty);

////            var columns = row.DataGridView.Columns.Cast<DataGridViewColumn>();

////            var codeCol = columns.FirstOrDefault(c =>
////                c.Name.Contains("id", StringComparison.OrdinalIgnoreCase) ||
////                c.Name.Contains("code", StringComparison.OrdinalIgnoreCase));

////            var nameCol = columns.FirstOrDefault(c =>
////                c.Name.Contains("name", StringComparison.OrdinalIgnoreCase));

////            string code = codeCol != null ? row.Cells[codeCol.Name].Value?.ToString() ?? "" : "";
////            string name = nameCol != null ? row.Cells[nameCol.Name].Value?.ToString() ?? "" : "";

////            return (code, name);
////        }

////        public void ApplyGridFormatting(DataGridView dgv)
////        {
////            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
////            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
////            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
////            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
////            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

////            foreach (DataGridViewColumn col in dgv.Columns)
////                col.Visible = false;

////            switch (_type)
////            {
////                case SearchEntityType.Accounts:
////                    ApplyAccountsGridFormatting(dgv);
////                    break;
////                case SearchEntityType.Products:
////                    ApplyProductsGridFormatting(dgv);
////                    break;
////                case SearchEntityType.Invoices:
////                    ApplyInvoiceGridFormatting(dgv);
////                    break;
////            }
////        }

////        private void ShowColumn(DataGridView dgv, string name, string header, float weight, string? format = null)
////        {
////            if (!dgv.Columns.Contains(name)) return;
////            var c = dgv.Columns[name];
////            c.Visible = true;
////            c.HeaderText = header;
////            c.FillWeight = weight;

////            if (!string.IsNullOrEmpty(format))
////            {
////                c.DefaultCellStyle.Format = format;
////                c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
////            }
////            else
////            {
////                c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
////            }

////            c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
////        }

////        private void ApplyAccountsGridFormatting(DataGridView dgv)
////        {
////            ShowColumn(dgv, "AccID", "كود", 1f);
////            ShowColumn(dgv, "AccName", "اسم الحساب", 3f);
////            ShowColumn(dgv, "Balance", "الرصيد", 1f, "N2");
////            ShowColumn(dgv, "BalanceState", "--", 1f);
////        }

////        private void ApplyProductsGridFormatting(DataGridView dgv)
////        {
////            ShowColumn(dgv, "ProductCode", "كود", 1f);
////            ShowColumn(dgv, "ProdName", "اسم الصنف", 3f);
////            ShowColumn(dgv, "RegistYear", "سنة", 1f);
////            ShowColumn(dgv, "U_Price", "السعر", 1f, "N2");
////            ShowColumn(dgv, "ProductStock", "الرصيد", 1f, "N2");
////            ShowColumn(dgv, "NoteProduct", "ملاحظات الصنف", 4f);
////        }

////        private void ApplyInvoiceGridFormatting(DataGridView dgv)
////        {
////            ShowColumn(dgv, "Inv_Counter", "رقم الفاتورة", 1f);
////            ShowColumn(dgv, "AccName", "اسم العميل/المورد", 3f);
////            ShowColumn(dgv, "SellerName", "البائع", 2f);
////            ShowColumn(dgv, "Inv_Date", "التاريخ", 1.5f, "yyyy/MM/dd");
////            ShowColumn(dgv, "NetTotal", "صافى الفاتورة", 1.5f, "N2");
////        }
////    }
////}

