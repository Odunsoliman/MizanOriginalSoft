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


/*

public static class ReportsManager
{
    private static DataTable GetReportData(string repCodeName, Dictionary<string, object> parameters)
    {
        // TODO: كتابة كود جلب البيانات من قاعدة البيانات
        return new DataTable();
    }
}





        #region ******************* دوال التقارير تحت الانتظار ******************


        private void rep_purchase_net() { MessageBox.Show("جاري إعداد تقرير صافي مشتريات ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_sales_details() { MessageBox.Show("جاري إعداد تقرير تفصيلي مبيعات ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_sales_net() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_sales_specific_items() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_customer_statement() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_customer_balances() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_sales_invoice_movements() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_customer_aging_analysis() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_top_customers() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_total_customer_balance() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_supplier_statement() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_supplier_balances() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_purchase_invoice_movements() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_top_suppliers() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_supplier_purchase_invoices() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_total_supplier_purchases() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_partner_current_movements() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_total_partner_balances() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_partner_statement() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_partner_contributions() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_profit_distributions() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_employee_advances_deductions() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_total_salaries_advances() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_employee_statement() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_salary_register() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_attendance_report() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_allowances_deductions_summary() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_specific_expense() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_total_expenses_by_items() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_expense_movements() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_expense_comparison() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_expense_type_analysis() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_cashbox_movements() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_cashboxes_summary() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_cash_banks_summary() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_bank_movements() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_cash_summary() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_specific_asset() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_assets_by_category() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_fixed_assets_register() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_assets_depreciation() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_assets_movements() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_creditor_statement() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_total_creditor_balances() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_creditors_list() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_debt_aging_analysis() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_total_debtor_balances() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_debtor_statement() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_debtors_list() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_receivables_aging_analysis() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_balance_sheet() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_income_statement() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_general_balance() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_cash_flow() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_financial_ratios() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_equity_changes() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_financial_performance() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_budget_comparison() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_comparative_balance_sheet() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_vertical_horizontal_analysis() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        private void rep_abc() { MessageBox.Show("جاري إعداد التقرير ....", "توقف", MessageBoxButtons.OK, MessageBoxIcon.Information); }

        #endregion
.
 


1	كارت صنف	                            200 	rep_item_card
2	تفصيلي مشتريات	200	                            rep_purchase_details
3	صافي مشتريات	200	                            rep_purchase_net
4	تفصيلي مبيعات	200	                            rep_sales_details
5	صافي مبيعات	200	                                rep_sales_net
6	مبيعات اصناف محدده	200	                        rep_sales_specific_items
7	كشف حساب عميل	7	                            rep_customer_statement
8	أرصدة العملاء	7	                            rep_customer_balances
9	حركة فواتير المبيعات والمردودات	7	            rep_sales_invoice_movements
10	تحليل أعمار الديون للعملاء	7	                rep_customer_aging_analysis
11	العملاء الأكثر مبيعات	7	                        rep_top_customers
12	رصيد العملاء الإجمالي	7	                        rep_total_customer_balance
13	كشف حساب مورد	14	                            rep_supplier_statement
14	أرصدة الموردين	14	                            rep_supplier_balances
15	حركة فواتير المشتريات	14	                    rep_purchase_invoice_movements
16	الموردون الأكثر تعاملًا	14	                    rep_top_suppliers
17	فواتير مشتريات من مورد معين	14	                rep_supplier_purchase_invoices
18	إجمالي مشتريات الموردين	14	                    rep_total_supplier_purchases
19	حركات جارية لشريك	39	                        rep_partner_current_movements
20	إجمالي أرصدة الشركاء	39	                    rep_total_partner_balances
21	كشف حساب شريك	39	                            rep_partner_statement
22	مساهمات الشركاء	39	                            rep_partner_contributions
23	توزيعات الأرباح	39	                            rep_profit_distributions
24	كشف سلف وخصومات موظف	22	                    rep_employee_advances_deductions
25	إجمالي الرواتب والسلف	22	                    rep_total_salaries_advances
26	كشف حساب موظف	22	                            rep_employee_statement
27	سجل الرواتب	22	                                rep_salary_register
28	كشف الحضور والانصراف	22	                        rep_attendance_report
29	ملخص الاستحقاقات والخصومات	22	                rep_allowances_deductions_summary
30	تقرير مصروف معين	19	                        rep_specific_expense
31	إجمالي المصروفات حسب البنود	19	                rep_total_expenses_by_items
32	حركة المصروفات	19	                            rep_expense_movements
33	مقارنة مصروفات الفترات	19                  	rep_expense_comparison
34	تحليل المصروفات حسب النوع	19	                rep_expense_type_analysis
35	كشف حركة صندوق	3	                            rep_cashbox_movements
36	تقرير الصناديق النقدية	3	                    rep_cashboxes_summary
37	تقرير البنوك النقدية	3	                    rep_cash_banks_summary
38	حركة البنك	3	                                rep_bank_movements
39	ملخص النقدية	3	                            rep_cash_summary
40	تقرير أصل محدد	9	                            rep_specific_asset
41	ملخص الأصول حسب التصنيف	9	                    rep_assets_by_category
42	سجل الأصول الثابتة	9	                        rep_fixed_assets_register
43	إهلاك الأصول	9	                                rep_assets_depreciation
44	حركة الأصول	9	                                rep_assets_movements
45	كشف حساب دائن	13	                            rep_creditor_statement
46	إجمالي أرصدة الدائنين	13	                    rep_total_creditor_balances
47	قائمة الدائنين	13	                            rep_creditors_list
48	تحليل أعمار الديون	13	                        rep_debt_aging_analysis
49	إجمالي أرصدة المدينين	6	                    rep_total_debtor_balances
50	كشف حساب مدين	6	                            rep_debtor_statement
51	قائمة المدينين	6	                            rep_debtors_list
52	تحليل أعمار الذمم المدينة	6	                rep_receivables_aging_analysis
53	المركز المالي	0	                            rep_balance_sheet
54	قائمة الأرباح والخسائر	0	                    rep_income_statement
55	الميزانية العمومية	0	                        rep_general_balance
56	قائمة التدفقات النقدية	0	                    rep_cash_flow
57	تحليل النسب المالية	0	                        rep_financial_ratios
58	قائمة التغيرات في حقوق الملكية	0	            rep_equity_changes
59	تحليل الأداء المالي	0	                        rep_financial_performance
60	تقرير مقارنة الموازنات	0	                    rep_budget_comparison
61	قائمة المركز المالي المقارن	0	                rep_comparative_balance_sheet
62	التحليل الرأسي والأفقي للقوائم المالية	0	    rep_vertical_horizontal_analysis

 */

