using Microsoft.Reporting.WinForms;
using MizanOriginalSoft.Views.Reports;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace MizanOriginalSoft.MainClasses.OriginalClasses.ReportsClasses
{
    public static class ReportsManager
    {
        // قائمة التقارير وحالة تطويرها (true = جاهز، false = تحت التطوير)
        private static readonly Dictionary<string, bool> ReportsStatus = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase)
    {
         
        { "rep_item_card" ,true  },                               //كارت صنف
        { "rep_purchase_details	"   ,true },                     //تفصيلي مشتريات
        { "rep_purchase_net"    ,true },                         //صافي مشتريات
        { "rep_sales_details"   ,true },                         //تفصيلي مبيعات
        { "rep_sales_net"   ,true },                             //صافي مبيعات
        { "rep_sales_specific_items"    ,true },                 //مبيعات اصناف محدده
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
        { "rep_supplier_purchase_invoices"	,false },
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
        { "rep_cash_banks_summary"		, false },
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
    };
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
                MessageBox.Show($"التقرير \"\" غير موجود في النظام.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!isReady)
            {
                MessageBox.Show($"التقرير \"{repCodeName}\" قيد الإنشاء والتطوير.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
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


        private static DataTable GetReportData(string repCodeName, Dictionary<string, object> parameters)
        {
            // TODO: كتابة كود جلب البيانات من قاعدة البيانات بناءً على التقرير
            // يمكن هنا استخدام Stored Procedure أو أي طريقة مناسبة
            return new DataTable();
        }

        private static void PrintReport(string reportName, string dataSetName, DataTable data, string printerName)
        {
            LocalReport report = new LocalReport();
            report.ReportPath = Path.Combine(Application.StartupPath, "Reports", reportName + ".rdlc");
            report.DataSources.Clear();
            report.DataSources.Add(new ReportDataSource(dataSetName, data));

            // طباعة باستخدام الكلاس الجديد
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
    }


}

