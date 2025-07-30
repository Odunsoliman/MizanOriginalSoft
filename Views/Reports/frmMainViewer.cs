using Microsoft.Reporting.WinForms;
using MizanOriginalSoft.MainClasses;
using System;
using System.Data;
using System.Drawing.Printing;
using System.Security.Cryptography.Xml;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Reports
{
    public partial class frmMainViewer : Form
    {
        private Dictionary<string, object> parameters;//قاموس لحفظ معلمات التقرير
        private PrintDocument printDocument;//كائن للتحكم في عملية الطباعة

        public frmMainViewer(Dictionary<string, object> parameters)
        {
            InitializeComponent();

            // تعيين مقاس الشاشة لتقليد حجم ورقة A4
            this.Width = 1080;
            this.Height = 1123;
            // توسيط الشاشة
            this.StartPosition = FormStartPosition.CenterScreen;

            // اختياري: منع التكبير والتصغير
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // استخدام المعاملات حسب الحاجة
            string reportName = parameters["ReportName"]?.ToString() ?? string.Empty;
            string dataSetName = parameters["DataSetName"]?.ToString() ?? string.Empty;
            int idAcc = Convert.ToInt32(parameters["ID"]);
            //
            string WayMove = parameters["WayMove"]?.ToString() ?? string.Empty;

            // هنا يمكن إعداد تقرير الـ ReportViewer بناءً على هذه المعاملات
            //================
            this.parameters = parameters;//قاموس يحوي بيانات التقرير

            // استدعاء لتحميل التقرير عند إعداد الفورم
            this.Load += new EventHandler(frmMainViewer_Load);
            printDocument = new PrintDocument();
            //•	تهيئة كائن الطباعة بحجم A4 وهوامش محددة
            printDocument.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169); // A4 size in 1/100 mm (A4: 210mm x 297mm)
            printDocument.DefaultPageSettings.Margins = new Margins(100, 100, 100, 100); // 1 inch margins (100 x 100)
            printDocument.PrintPage += new PrintPageEventHandler(PrintPage);
        }

        private void PrintPage(object sender, PrintPageEventArgs e)//رسم محتوى الصفحة المطلوب طباعتها (حاليا يطبع نص تجريبي
        {
            
            if (e.Graphics != null)
            {
                e.Graphics.DrawString("Hello, A4 Print!", new Font("Arial", 20), Brushes.Black, new PointF(100, 100));
            }

        }


        private void frmMainViewer_Load(object? sender, EventArgs? e)
        {
            // التحقق من وجود المعلمات الأساسية المطلوبة
            if (parameters == null || !parameters.ContainsKey("ReportName") || !parameters.ContainsKey("DataSetName"))
            {
                MessageBox.Show("معلمات التقرير الأساسية مفقودة (ReportName أو DataSetName).");
                return;
            }

            // استخراج جميع المعلمات مع التحقق من وجودها
            string reportName = parameters["ReportName"]?.ToString() ?? string.Empty;
            string dataSetName = parameters["DataSetName"]?.ToString() ?? string.Empty;
            int id = parameters.TryGetValue("ID", out object? idObj) && idObj != null ? Convert.ToInt32(idObj) : 0;
            DateTime? startDate = parameters.TryGetValue("StartDate", out object? startObj) && startObj is DateTime dt1 ? dt1 : null;
            DateTime? endDate = parameters.TryGetValue("EndDate", out object? endObj) && endObj is DateTime dt2 ? dt2 : null;

            // التعامل الآمن مع DataTable
            DataTable? dataTable = parameters.TryGetValue("DataTable", out object? dtObj) && dtObj is DataTable dtVal ? dtVal : null;

            // التعامل الآمن مع StoredProcedure
            string? storedProcedure = parameters.TryGetValue("StoredProcedure", out object? spObj) ? spObj?.ToString() : null;

            // التعامل الآمن مع PrinterName
            string? printerName = parameters.TryGetValue("PrinterName", out object? prnObj) ? prnObj?.ToString() : null;

            // WarehouseID
            int warehouseID = parameters.TryGetValue("WarehouseID", out object? whObj) && whObj != null ? Convert.ToInt32(whObj) : 0;

            // WayMove
            string wayMove = parameters.TryGetValue("WayMove", out object? wmObj) ? wmObj?.ToString() ?? string.Empty : string.Empty;

            // التحقق من وجود بيانات في الجدول إذا كان ممرراً
            if (dataTable != null && dataTable.Rows.Count == 0)
            {
                MessageBox.Show("الجدول المقدم لا يحتوي على بيانات.");
                return;
            }

            // تعيين الطابعة إذا كانت معلمة موجودة
            if (!string.IsNullOrEmpty(printerName))
            {
                printDocument.PrinterSettings.PrinterName = printerName;
            }

            // استدعاء عرض التقرير مع جميع المعلمات
            if (!string.IsNullOrEmpty(storedProcedure) && dataTable != null)
            {
                ShowReport(reportName, dataSetName, id, startDate, endDate, dataTable, storedProcedure, warehouseID, wayMove);
            }
            else
            {
                MessageBox.Show("تعذر عرض التقرير: البيانات غير متوفرة.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        bool closeViewer;
        private void ShowReport(string reportName, string dataSetName, int id,
                       DateTime? startDate, DateTime? endDate,
                       DataTable dataTable, string storedProcedure, int WarehouseID, string WayMove)
        {
            try
            {
                // التحقق من وجود ملف التقرير
                string reportPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views", "Reports", reportName);

                if (!System.IO.File.Exists(reportPath))
                {
                    MessageBox.Show("لم يتم العثور على التقرير في المسار: " + reportPath);
                    closeViewer = true;
                    return;
                }
                if (closeViewer == true) { this.Close(); }
                // تعيين مسار التقرير
                this.reportViewer1.LocalReport.ReportPath = reportPath;

                // الحصول على البيانات
                DataTable dt = dataTable ?? GetDataForReport(dataSetName, id, startDate, endDate, storedProcedure);

                // التحقق من وجود بيانات
                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("لا توجد بيانات لعرضها.");
                    return;
                }

                // تعيين مصدر البيانات
                ReportDataSource rds = new ReportDataSource(dataSetName, dt);
                this.reportViewer1.LocalReport.DataSources.Clear();
                this.reportViewer1.LocalReport.DataSources.Add(rds);

                // تحديث التقرير
                this.reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء تحميل التقرير: " + ex.Message);
            }
        }
        private DataTable GetDataForReport(string dataSetName, int id,
                                         DateTime? startDate, DateTime? endDate,
                                         string storedProcedure)
        {
            DataTable dt = new DataTable();

            try
            {
                // الحالات الثابتة الأصلية
                switch (dataSetName)
                {
                    case "dsProductCard":// تقرير كارت صنف رقم =0
                        dt = GetProductCardData(parameters);
                        break;
                    case "dsGetDetailedMovements":
                        dt = GetDetailedMovementsReport(parameters);
                        break;
                    case "dsBar":
                        dt = Bar(parameters);
                        break;
                    case "b":
                        dt = GetDailyFee_AccountDataByDate(parameters);
                        break;
                    case "c":
                        dt = GetProduct_StockBySupplier(parameters);
                        break;
                    case "d":
                        dt = GetSupplierProductSales(parameters);
                        break;
                    case "g":
                        dt = GetSalesAnalysisByProductID(parameters);
                        break;
                    case "h":
                        dt = GetProductMovementSteps(parameters);
                        break;


                    default:
                        throw new ArgumentException("اسم مجموعة البيانات غير معروف");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء جلب البيانات: " + ex.Message);
            }

            return dt;
        }
        #region ========== دوال مخصصة لكل تقارير الاصناف =======================
        //  تقرير كارت صنف ورقمه =0
        private DataTable GetProductCardData(Dictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                int prodID = parameters.ContainsKey("ID") ? (int)parameters["ID"] : 0;
                int storeID = parameters.ContainsKey("WarehouseID") ? (int)parameters["WarehouseID"] : 0;

                // تحديد قيم افتراضية للتواريخ إذا كانت null
                DateTime defaultStartDate = DateTime.Today.AddMonths(-1); // افتراضيًا آخر شهر
                DateTime defaultEndDate = DateTime.Today;

                DateTime startDate = parameters.ContainsKey("StartDate") && parameters["StartDate"] != null
                    ? (DateTime)parameters["StartDate"]
                    : defaultStartDate;

                DateTime endDate = parameters.ContainsKey("EndDate") && parameters["EndDate"] != null
                    ? (DateTime)parameters["EndDate"]
                    : defaultEndDate;

                // استدعاء الدالة مع التواريخ غير Nullable
                dt = DBServiecs.GetProductCard_Set(prodID, storeID, startDate, endDate);
                dt = DBServiecs.GetProductCard();

            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء استرجاع بيانات كارت الصنف: " + ex.Message);
                // يمكنك تسجيل الخطأ في ملف log هنا إذا كنت بحاجة لذلك
            }
            return dt;
        }
        //التبديل بين تفصيلى مشتريات او تفصيلى مبيعات
        // عرض تقرير الحركات التفصيلية (مشتريات أو مبيعات)
        private DataTable GetDetailedMovementsReport(Dictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();

            try
            {
                int prodID = parameters.ContainsKey("ID") ? Convert.ToInt32(parameters["ID"]) : 0;
                int storeID = parameters.ContainsKey("WarehouseID") ? Convert.ToInt32(parameters["WarehouseID"]) : 0;

                // تحديد قيم افتراضية للتواريخ
                DateTime defaultStartDate = DateTime.Today.AddMonths(-1);
                DateTime defaultEndDate = DateTime.Today;

                DateTime startDate = parameters.ContainsKey("StartDate") && parameters["StartDate"] is DateTime
                    ? (DateTime)parameters["StartDate"]
                    : defaultStartDate;

                DateTime endDate = parameters.ContainsKey("EndDate") && parameters["EndDate"] is DateTime
                    ? (DateTime)parameters["EndDate"]
                    : defaultEndDate;

                string? wayMove = parameters.ContainsKey("WayMove") ? parameters["WayMove"]?.ToString() : null;

                if (wayMove == "Purchases" || wayMove == "Sales")
                {
                    dt = DBServiecs.rpt_GetDetailedMovementsReport(prodID, storeID, startDate, endDate, wayMove);
                }
                else
                {
                    MessageBox.Show("نوع الحركة غير معروف. يرجى التأكد من القيمة.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء استرجاع التقرير التفصيلي: " + ex.Message);
            }

            return dt;
        }

        private DataTable rpt_GetDetailedPurchases(Dictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                int prodID = parameters.ContainsKey("ID") ? (int)parameters["ID"] : 0;
                int storeID = parameters.ContainsKey("WarehouseID") ? (int)parameters["WarehouseID"] : 0;

                // تحديد قيم افتراضية للتواريخ إذا كانت null
                DateTime defaultStartDate = DateTime.Today.AddMonths(-1); // افتراضيًا آخر شهر
                DateTime defaultEndDate = DateTime.Today;

                DateTime startDate = parameters.ContainsKey("StartDate") && parameters["StartDate"] != null
                    ? (DateTime)parameters["StartDate"]
                    : defaultStartDate;

                DateTime endDate = parameters.ContainsKey("EndDate") && parameters["EndDate"] != null
                    ? (DateTime)parameters["EndDate"]
                    : defaultEndDate;

                // استدعاء الدالة مع التواريخ غير Nullable
                dt = DBServiecs.GetProductCard_Set(prodID, storeID, startDate, endDate);
                dt = DBServiecs.GetProductCard();

            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء استرجاع بيانات كارت الصنف: " + ex.Message);
                // يمكنك تسجيل الخطأ في ملف log هنا إذا كنت بحاجة لذلك
            }
            return dt;
        }

        private DataTable rpt_GetDetailedSales(Dictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                int prodID = parameters.ContainsKey("ID") ? (int)parameters["ID"] : 0;
                int storeID = parameters.ContainsKey("WarehouseID") ? (int)parameters["WarehouseID"] : 0;

                // تحديد قيم افتراضية للتواريخ إذا كانت null
                DateTime defaultStartDate = DateTime.Today.AddMonths(-1); // افتراضيًا آخر شهر
                DateTime defaultEndDate = DateTime.Today;

                DateTime startDate = parameters.ContainsKey("StartDate") && parameters["StartDate"] != null
                    ? (DateTime)parameters["StartDate"]
                    : defaultStartDate;

                DateTime endDate = parameters.ContainsKey("EndDate") && parameters["EndDate"] != null
                    ? (DateTime)parameters["EndDate"]
                    : defaultEndDate;

                // استدعاء الدالة مع التواريخ غير Nullable
                dt = DBServiecs.GetProductCard_Set(prodID, storeID, startDate, endDate);
                dt = DBServiecs.GetProductCard();

            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء استرجاع بيانات كارت الصنف: " + ex.Message);
                // يمكنك تسجيل الخطأ في ملف log هنا إذا كنت بحاجة لذلك
            }
            return dt;
        }

        #endregion

        private DataTable Bar(Dictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                int prodID = parameters.ContainsKey("ID") ? (int)parameters["ID"] : 0;
                int storeID = parameters.ContainsKey("WarehouseID") ? (int)parameters["WarehouseID"] : 0;

                // تحديد قيم افتراضية للتواريخ إذا كانت null
                DateTime defaultStartDate = DateTime.Today.AddMonths(-1); // افتراضيًا آخر شهر
                DateTime defaultEndDate = DateTime.Today;

                DateTime startDate = parameters.ContainsKey("StartDate") && parameters["StartDate"] != null
                    ? (DateTime)parameters["StartDate"]
                    : defaultStartDate;

                DateTime endDate = parameters.ContainsKey("EndDate") && parameters["EndDate"] != null
                    ? (DateTime)parameters["EndDate"]
                    : defaultEndDate;

                dt = DBServiecs.sp_GetBarcodesToPrint();

            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء استرجاع بيانات كارت الصنف: " + ex.Message);
                // يمكنك تسجيل الخطأ في ملف log هنا إذا كنت بحاجة لذلك
            }
            return dt;
        }


        #region لم يتم مراجعته
        //هذا التقرير لعرض...
        private DataTable GetProductMovementSteps(Dictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                int id = parameters.ContainsKey("ID") ? (int)parameters["ID"] : 0;
                //dt = DBServiecs.GetProductMovementSteps(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء استرجاع البيانات: " + ex.Message);
            }
            return dt;
        }
        //هذا التقرير لعرض...
        private DataTable GetSalesAnalysisByProductID(Dictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                int id = parameters.ContainsKey("ID") ? (int)parameters["ID"] : 0;
                //dt = DBServiecs.GetSalesAnalysisByProductID(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء استرجاع البيانات: " + ex.Message);
            }
            return dt;
        }
        //هذا التقرير لعرض...
        private DataTable GetAccBalanceData(Dictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                int id = parameters.ContainsKey("ID") ? (int)parameters["ID"] : 0;
                dt = DBServiecs.Account_Balance(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء استرجاع البيانات: " + ex.Message);
            }
            return dt;
        }
        //هذا التقرير لعرض...
        private DataTable GetDailyFee_AccountData(Dictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                int id = parameters.ContainsKey("ID") ? (int)parameters["ID"] : 0;
                dt = DBServiecs.DailyFee_Account(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء استرجاع البيانات: " + ex.Message);
            }
            return dt;
        }
        //هذا التقرير لعرض...
        private DataTable GetDailyFee_AccountDataByDate(Dictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                int id = parameters.ContainsKey("ID") ? (int)parameters["ID"] : 0;
                DateTime? startDate = parameters.ContainsKey("StartDate") ? (DateTime?)parameters["StartDate"] : null;
                DateTime? endDate = parameters.ContainsKey("EndDate") ? (DateTime?)parameters["EndDate"] : null;

                if (startDate.HasValue && endDate.HasValue)
                {
                    dt = DBServiecs.DailyFee_AccountByDate(id, startDate.Value, endDate.Value);
                }
                else
                {
                    MessageBox.Show("يجب تحديد تاريخ البدء وتاريخ الانتهاء.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء استرجاع البيانات: " + ex.Message);
            }
            return dt;
        }
        //هذا التقرير لعرض...
        private DataTable GetProduct_StockBySupplier(Dictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                int id = parameters.ContainsKey("ID") ? (int)parameters["ID"] : 0;
                //               dt = DBServiecs.Product_StockBySupplier(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء استرجاع البيانات: " + ex.Message);
            }
            return dt;
        }
        //هذا التقرير لعرض...
        private DataTable GetSupplierProductSales(Dictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                int id = parameters.ContainsKey("ID") ? (int)parameters["ID"] : 0;
                DateTime? startDate = parameters.ContainsKey("StartDate") ? (DateTime?)parameters["StartDate"] : null;
                DateTime? endDate = parameters.ContainsKey("EndDate") ? (DateTime?)parameters["EndDate"] : null;

                if (startDate.HasValue && endDate.HasValue)
                {
                    //                   dt = DBServiecs.GetSupplierProductSales(id, startDate.Value, endDate.Value);
                }
                else
                {
                    MessageBox.Show("يجب تحديد تاريخ البدء وتاريخ الانتهاء.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء استرجاع البيانات: " + ex.Message);
            }
            return dt;
        }
        #endregion 
    }

}
