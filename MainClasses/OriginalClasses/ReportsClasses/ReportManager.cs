using Microsoft.Reporting.WinForms;
using MizanOriginalSoft.Views.Reports;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace MizanOriginalSoft.MainClasses.OriginalClasses.ReportsClasses
{
    /// <summary>
    /// مدير التقارير:
    ///  - يتأكد إذا كان التقرير موجود وجاهز
    ///  - يجلب بيانات التقرير من DBServiecs
    ///  - يوفر إعدادات الأعمدة الخاصة بالجريد
    ///  - يتعامل مع العرض / الطباعة / التصدير
    /// </summary>
    public static class ReportsManager
    {
        #region حالة التقارير (جاهز أو تحت التطوير)

        // 🔹 قاموس يحدد حالة كل تقرير (جاهز أو لا)
        private static readonly Dictionary<string, bool> ReportsStatus =
            new(StringComparer.OrdinalIgnoreCase)
        {
            { "rep_item_card" , true },              // كارت صنف
            { "rep_purchase_details", true },        // تفصيلي مشتريات
            { "rep_purchase_net", true },            // صافي مشتريات
            { "rep_sales_details", true },           // تفصيلي مبيعات
            { "rep_sales_net", true },               // صافي مبيعات
            { "rep_sales_specific_items", true },    // مبيعات أصناف محددة

            // باقي التقارير (false = قيد التطوير)
            { "rep_customer_statement"  ,false },
            { "rep_customer_balances"   ,false },
            { "rep_sales_invoice_movements" ,false },
            // ... أكمل حسب حاجتك
        };

        #endregion

        #region دوال جلب البيانات لكل تقرير

        // 🔹 قاموس يربط كود التقرير بدالة تجيب البيانات
        private static readonly Dictionary<string, Func<Dictionary<string, object>, DataTable>> ReportLoaders =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["rep_item_card"] = (p) =>
                {
                    int itemId = Convert.ToInt32(p["ItemID"]);
                    // TODO: استبدل لاحقاً بدالة DBServiecs الحقيقية
                    // return DBServiecs.ItemCard_Get(itemId);
                    return new DataTable();
                },

                ["rep_purchase_details"] = (p) =>
                {
                    DateTime fromDate = Convert.ToDateTime(p["FromDate"]);
                    DateTime toDate = Convert.ToDateTime(p["ToDate"]);
                    // return DBServiecs.PurchaseDetails_Get(fromDate, toDate);
                    return new DataTable();
                },

                ["rep_purchase_net"] = (p) =>
                {
                    // return DBServiecs.PurchaseNet_GetAll();
                    return new DataTable();
                },

                ["rep_sales_details"] = (p) =>
                {
                    DateTime fromDate = Convert.ToDateTime(p["FromDate"]);
                    DateTime toDate = Convert.ToDateTime(p["ToDate"]);
                    // return DBServiecs.SalesDetails_Get(fromDate, toDate);
                    return new DataTable();
                },

                ["rep_sales_net"] = (p) =>
                {
                    // return DBServiecs.SalesNet_GetAll();
                    return new DataTable();
                },

                // أضف المزيد بنفس النمط...
            };

        #endregion

        #region عرض / طباعة / تصدير

        /// <summary>
        /// عرض أو طباعة أو تصدير التقرير حسب الـ parameters
        /// </summary>
        public static void ShowReport(Dictionary<string, object> parameters)
        {
            string repCodeName = parameters["ReportCodeName"]?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(repCodeName))
            {
                MessageBox.Show("لم يتم تحديد التقرير.");
                return;
            }

            // تحقق من وجود التقرير
            if (!ReportsStatus.TryGetValue(repCodeName, out bool isReady))
            {
                MessageBox.Show($"التقرير \"{repCodeName}\" غير موجود.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // تحقق من حالة التقرير
            if (!isReady)
            {
                MessageBox.Show($"التقرير \"{repCodeName}\" قيد التطوير.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // تجهيز البيانات
            string dataSetName = repCodeName.Replace("rep_", "ds_");
            DataTable data = GetReportData(repCodeName, parameters);

            string printMode = parameters["PrintMode"]?.ToString() ?? "Preview";
            switch (printMode)
            {
                case "Preview":
                    frmMainViewer viewer = new frmMainViewer(repCodeName, dataSetName, data, parameters);
                    viewer.ShowDialog();
                    break;

                case "Printer":
                    PrintReport(repCodeName, dataSetName, data, parameters["PrinterName"]?.ToString() ?? "");
                    break;

                case "Excel":
                    ExportToExcel(repCodeName, dataSetName, data);
                    break;

                case "PDF":
                    ExportToPdf(repCodeName, dataSetName, data);
                    break;
            }
        }

        // جلب البيانات
        public static DataTable GetReportData(string repCodeName, Dictionary<string, object> parameters)
        {
            return ReportLoaders.TryGetValue(repCodeName, out var loader)
                ? loader(parameters)
                : new DataTable();
        }

        private static void PrintReport(string reportName, string dataSetName, DataTable data, string printerName)
        {
            LocalReport report = new LocalReport
            {
                ReportPath = Path.Combine(Application.StartupPath, "Reports", reportName + ".rdlc")
            };
            report.DataSources.Clear();
            report.DataSources.Add(new ReportDataSource(dataSetName, data));

            LocalReportPrinter.Print(report, printerName);
        }

        private static void ExportToExcel(string reportName, string dataSetName, DataTable data)
        {
            // TODO: كود التصدير إلى Excel
        }

        private static void ExportToPdf(string reportName, string dataSetName, DataTable data)
        {
            // TODO: كود التصدير إلى PDF
        }

        #endregion

        #region إعدادات الأعمدة للجريد

        // 🔹 قاموس لإعدادات الأعمدة لكل تقرير
        private static readonly Dictionary<string, List<ColumnConfig>> ReportColumnSettings = new()
        {
            ["rep_sales_details"] = new List<ColumnConfig>
            {
                new ColumnConfig("ProdID", "كود المنتج", 80, 0),
                new ColumnConfig("ProdName", "اسم المنتج", 200, 1),
                new ColumnConfig("Amount", "الكمية", 100, 2),
                new ColumnConfig("Total", "الإجمالي", 120, 3)
            },
            ["rep_purchase_details"] = new List<ColumnConfig>
            {
                new ColumnConfig("InvoiceNo", "رقم الفاتورة", 100, 0),
                new ColumnConfig("SupplierName", "المورد", 200, 1),
                new ColumnConfig("Total", "الإجمالي", 120, 2)
            }
            // ... أضف باقي الإعدادات
        };

        /// <summary>
        /// استرجاع إعدادات الأعمدة لتقرير محدد
        /// </summary>
        public static List<ColumnConfig> GetColumnSettings(string reportCodeName)
        {
            return ReportColumnSettings.TryGetValue(reportCodeName, out var settings)
                ? settings
                : new List<ColumnConfig>();
        }

        // كلاس مساعد لتعريف إعدادات العمود
        public class ColumnConfig
        {
            public string ColumnName { get; }
            public string HeaderText { get; }
            public int Width { get; }
            public int DisplayIndex { get; }

            public ColumnConfig(string columnName, string headerText, int width, int displayIndex)
            {
                ColumnName = columnName;
                HeaderText = headerText;
                Width = width;
                DisplayIndex = displayIndex;
            }
        }

        #endregion
    }
}






/*
using Microsoft.Reporting.WinForms;
using MizanOriginalSoft.Views.Reports;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace MizanOriginalSoft.MainClasses.OriginalClasses.ReportsClasses
{
    public static class ReportsManager
    {



        // حالة كل تقرير (جاهز / تحت التطوير)
        private static readonly Dictionary<string, bool> ReportsStatus =
            new(StringComparer.OrdinalIgnoreCase)
        {
            { "rep_item_card" , true },              // كارت صنف
            { "rep_purchase_details", true },        // تفصيلي مشتريات
            { "rep_purchase_net", true },            // صافي مشتريات
            { "rep_sales_details", true },           // تفصيلي مبيعات
            { "rep_sales_net", true },               // صافي مبيعات
            { "rep_sales_specific_items", true },    // مبيعات أصناف محددة
            { "rep_customer_statement"  ,false },
            { "rep_customer_balances"   ,false },
            { "rep_sales_invoice_movements" ,false },
            { "rep_customer_aging_analysis" , false },
            { "rep_top_customers"   ,false },
            { "rep_total_customer_balance"  ,false },
            { "rep_supplier_statement"  ,false },
            { "rep_supplier_balances"   , false },
            { "rep_purchase_invoice_movements"  , false },
            { "rep_top_suppliers"   ,false },
            { "rep_supplier_purchase_invoices"  ,false },
            { "rep_total_supplier_purchases"    , false },
            { "rep_partner_current_movements"   ,false },
            { "rep_total_partner_balances"  , false },
            { "rep_partner_statement"   ,false },
            { "rep_partner_contributions"   ,false },
            { "rep_profit_distributions"    ,false },
            { "rep_employee_advances_deductions"    ,false },
            { "rep_total_salaries_advances" ,false },
            { "rep_employee_statement"  ,false },
            { "rep_salary_register" ,false },
            { "rep_attendance_report"   ,false },
            { "rep_allowances_deductions_summary"   ,false },
            { "rep_specific_expense"    ,false },
            { "rep_total_expenses_by_items" , false },
            { "rep_expense_movements"   , false },
            { "rep_expense_comparison"  ,false },
            { "rep_expense_type_analysis"   ,false },
            { "rep_cashbox_movements"   ,false },
            { "rep_cashboxes_summary"   , false },
            { "rep_cash_banks_summary"      , false },
            { "rep_bank_movements"  ,false },
            { "rep_cash_summary"    , false },
            { "rep_specific_asset"  ,false },
            { "rep_assets_by_category"  , false },
            { "rep_fixed_assets_register"   , false },
            { "rep_assets_depreciation" , false },
            { "rep_assets_movements"    , false },
            { "rep_creditor_statement"  ,false },
            { "rep_total_creditor_balances" , false },
            { "rep_creditors_list"  , false },
            { "rep_debt_aging_analysis" , false },
            { "rep_total_debtor_balances"   , false },
            { "rep_debtor_statement"    ,false },
            { "rep_debtors_list"    , false },
            { "rep_receivables_aging_analysis"  , false },
            { "rep_balance_sheet"   , false },
            { "rep_income_statement"    , false },
            { "rep_general_balance" , false },
            { "rep_cash_flow"   , false },
            { "rep_financial_ratios"    , false },
            { "rep_equity_changes"  , false },
            { "rep_financial_performance"   , false },
            { "rep_budget_comparison"   , false },
            { "rep_comparative_balance_sheet"   , false },
            { "rep_vertical_horizontal_analysis"    , false },
                                   //اضف  التقارير الجديدة هنا
 

            // ... باقي التقارير
        };

        // القاموس الخاص بدوال جلب البيانات لكل تقرير
        private static readonly Dictionary<string, Func<Dictionary<string, object>, DataTable>> ReportLoaders =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["rep_item_card"] = (p) =>
                {
                    int itemId = Convert.ToInt32(p["ItemID"]);
                    //return DBServiecs .ItemCard_Get(itemId);
                    return new DataTable(); // مؤقتًا لحد ما تكتب الدالة الحقيقية

                },

                ["rep_purchase_details"] = (p) =>
                {
                    DateTime fromDate = Convert.ToDateTime(p["FromDate"]);
                    DateTime toDate = Convert.ToDateTime(p["ToDate"]);
                    // return DBServiecs.PurchaseDetails_Get(fromDate, toDate);

                    return new DataTable(); // مؤقتًا لحد ما تكتب الدالة الحقيقية
                },


                ["rep_purchase_net"] = (p) =>
                {
                    //return DBServiecs.PurchaseNet_GetAll();
                    return new DataTable(); // مؤقتًا لحد ما تكتب الدالة الحقيقية
                },

                ["rep_sales_details"] = (p) =>
                {
                    DateTime fromDate = Convert.ToDateTime(p["FromDate"]);
                    DateTime toDate = Convert.ToDateTime(p["ToDate"]);
                    //return DBServiecs.SalesDetails_Get(fromDate, toDate);
                    return new DataTable(); // مؤقتًا لحد ما تكتب الدالة الحقيقية
                },

                ["rep_sales_net"] = (p) =>
                {
                    //return DBServiecs.SalesNet_GetAll();
                    return new DataTable(); // مؤقتًا لحد ما تكتب الدالة الحقيقية
                },

                // أضف تقارير جديدة هنا بنفس الشكل
            };

        // الدالة الرئيسية لعرض التقرير
        public static void ShowReport(Dictionary<string, object> parameters)
        {
            string repCodeName = parameters["ReportCodeName"]?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(repCodeName))
            {
                MessageBox.Show("لم يتم تحديد التقرير.");
                return;
            }

            if (!ReportsStatus.TryGetValue(repCodeName, out bool isReady))
            {
                MessageBox.Show($"التقرير \"{repCodeName}\" غير موجود في النظام.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!isReady)
            {
                MessageBox.Show($"التقرير \"{repCodeName}\" قيد الإنشاء والتطوير.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string dataSetName = repCodeName.Replace("rep_", "ds_");

            DataTable data = GetReportData(repCodeName, parameters);

            string printMode = parameters["PrintMode"]?.ToString() ?? "Preview";
            switch (printMode)
            {
                case "Preview":
                    frmMainViewer viewer = new frmMainViewer(repCodeName, dataSetName, data, parameters);
                    viewer.ShowDialog();
                    break;
                case "Printer":
                    PrintReport(repCodeName, dataSetName, data, parameters["PrinterName"]?.ToString() ?? "");
                    break;
                case "Excel":
                    ExportToExcel(repCodeName, dataSetName, data);
                    break;
                case "PDF":
                    ExportToPdf(repCodeName, dataSetName, data);
                    break;
            }
        }

        // جلب البيانات حسب التقرير
        private static DataTable GetReportData(string repCodeName, Dictionary<string, object> parameters)
        {
            return ReportLoaders.TryGetValue(repCodeName, out var loader)
                ? loader(parameters)
                : new DataTable();
        }

        private static void PrintReport(string reportName, string dataSetName, DataTable data, string printerName)
        {
            LocalReport report = new LocalReport();
            report.ReportPath = Path.Combine(Application.StartupPath, "Reports", reportName + ".rdlc");
            report.DataSources.Clear();
            report.DataSources.Add(new ReportDataSource(dataSetName, data));

            LocalReportPrinter.Print(report, printerName);
        }

        private static void ExportToExcel(string reportName, string dataSetName, DataTable data)
        {
            // TODO: كود التصدير إلى Excel
        }

        private static void ExportToPdf(string reportName, string dataSetName, DataTable data)
        {
            // TODO: كود التصدير إلى PDF
        }

        // إعدادات الأعمدة لكل تقرير
        private static readonly Dictionary<string, List<ColumnConfig>> ReportColumnSettings = new()
        {
            ["SalesReport"] = new List<ColumnConfig>
        {
            new ColumnConfig("ProdID", "كود المنتج", 80, 0),
            new ColumnConfig("ProdName", "اسم المنتج", 200, 1),
            new ColumnConfig("Amount", "الكمية", 100, 2),
            new ColumnConfig("Total", "الإجمالي", 120, 3)
        },
            ["StockReport"] = new List<ColumnConfig>
        {
            new ColumnConfig("WarehouseName", "المخزن", 150, 0),
            new ColumnConfig("ProdName", "اسم الصنف", 200, 1),
            new ColumnConfig("PrisentStock", "الرصيد", 100, 2)
        }
            // ... أضف باقي التقارير
        };

        // استرجاع الإعدادات لتقرير معين
        public static List<ColumnConfig> GetColumnSettings(string reportCodeName)
        {
            return ReportColumnSettings.TryGetValue(reportCodeName, out var settings)
                ? settings
                : new List<ColumnConfig>();
        }

        // كلاس مساعد لتعريف خصائص الأعمدة
        public class ColumnConfig
        {
            public string ColumnName { get; }
            public string HeaderText { get; }
            public int Width { get; }
            public int DisplayIndex { get; }

            public ColumnConfig(string columnName, string headerText, int width, int displayIndex)
            {
                ColumnName = columnName;
                HeaderText = headerText;
                Width = width;
                DisplayIndex = displayIndex;
            }
        }

    }
}


*/