using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses;
using MizanOriginalSoft.Views.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.Products
{
    public partial class frmReport_Preview : Form
    {

        #region  المتغيرات العامة لتخزين البيانات
//        public int ReportID { get; private set; }
        public int UserID { get; private set; }
        public int? EntityID { get; private set; } // Nullable لأنه قد يكون null
        public DataTable FilteredData { get; private set; }

        // باقي المتغيرات العامة التي تحتاجها
        //public DateTime StartDate { get; private set; }
        //public DateTime EndDate { get; private set; }

        private Dictionary<string, object> reportParameters;
        private int currentReportId;
        private string  currentReportName;
        #endregion 
        public frmReport_Preview(Dictionary<string, object> parameters)
        {
            InitializeComponent();
            this.reportParameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

            currentReportName = "تقرير افتراضي";  // تهيئة افتراضية مناسبة للسياق
            FilteredData = new DataTable();       // تجنب null

            LoadPrinters();
            LoadWarehouses();
            LoadDefaultSettings();
            SetupEventHandlers();
            CalculateDaysBetweenDates();
            LoadParametersToFields();

            this.Text = $"معاينة تقرير - {currentReportName} - كود: {currentReportId}";
        }

        #region ==== دوال تحميل بيانات القاموس ========
        private void LoadParametersToFields()
        {
            try
            {
                // تحميل ReportID
                if (reportParameters.TryGetValue("ReportID", out object? reportId) && reportId != null)
                {
                    currentReportId = Convert.ToInt32(reportId);
                }
                else
                {
                    currentReportId = 0;
                }

                // تحميل ReportName مع التعامل الآمن مع القيم الفارغة
                if (reportParameters.TryGetValue("ReportName", out object? reportName))
                {
                    currentReportName = reportName?.ToString() ?? "غير محدد";
                }
                else
                {
                    currentReportName = "غير محدد";
                }

                // تحميل UserID
                if (reportParameters.TryGetValue("UserID", out object? userId) && userId != null)
                {
                    UserID = Convert.ToInt32(userId);
                }
                else
                {
                    UserID = 0;
                }

                // تحميل EntityID مع التعامل مع القيم الفارغة
                if (reportParameters.TryGetValue("EntityID", out object? entityId) && entityId != null)
                {
                    EntityID = Convert.ToInt32(entityId);
                }
                else
                {
                    EntityID = null;
                }

                // تحميل FilteredData
                if (reportParameters.TryGetValue("FilteredData", out object? filteredData))
                {
                    FilteredData = filteredData as DataTable ?? new DataTable();
                }
                else
                {
                    FilteredData = new DataTable();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل معلمات التقرير: {ex.Message}");
                // يمكنك تسجيل الخطأ في ملف log هنا إذا لزم الأمر
            }
        }


        /*

*/

        #endregion



        #region ====== اختيار وظيفة فتح التقرير ==========
        private void btnPrintPreview_Click(object sender, EventArgs e)
        {
            // FilteredData   EntityID    ReportID
            InitializeReport();
        }
        private void InitializeReport()
        {
            try
            {
                switch (currentReportId)
                {
                    case 0:
                        rpt_GetProductCard(); // كارت صنف  
                        break;
                    case 1:
                        DisplayPurchaseMovementsReport();//"تفصيلى مشتريات"
                        break;
                    case 2:
                        MessgSetting("صافي مشتريات ");//
                        break;
                    case 3:
                        rpt_GetDetailedMovementsSales();// تفصيلى مبيعات"
                        break;
                    case 4:
                        MessgSetting("صافي مبيعات ");//
                        break;
                    case 5:
                        MessgSetting("الحالى ");//مبيعات اصناف محدده
                        break;
                    case 6:
                        MessgSetting("كشف حساب عميل ");
                        break;
                    #region 
                    case 7:
                        MessgSetting("ارصدة لعملاء ");//
                        break;
                    case 8:
                        MessgSetting("حركة فواتير العملاء ");//
                        break;
                    case 9:
                        MessgSetting("الحالى ");//
                        break;
                    case 10:
                        MessgSetting("العملاء الاكثر مبيعاً ");//
                        break;
                    case 11:
                        MessgSetting("الحالى ");//
                        break;
                    case 12:
                        MessgSetting("كشف حساب مورد ");//                                                                        break;
                        break;
                    case 13:
                        MessgSetting("ارصدة الموردين  ");//
                        break;
                    case 14:
                        MessgSetting("حركة فواتير المشتريات ");//
                        break;
                    case 15:
                        MessgSetting("الحالى ");//
                        break;
                    case 16:
                        MessgSetting("فواتير مشتريات مورد معين ");//
                        break;
                    case 17:
                        MessgSetting("اجمالى مشتريات الموردين ");//
                        break;
                    case 18:
                        MessgSetting("الحالى ");//
                        break;
                    case 19:
                        MessgSetting("الحالى ");//
                        break;
                    case 20:
                        MessgSetting("الحالى ");//
                        break;
                    case 21:
                        MessgSetting("الحالى ");//
                        break;
                    case 22:
                        MessgSetting("الحالى ");//
                        break;
                    case 23:
                        MessgSetting("الحالى ");//
                        break;
                    case 24:
                        MessgSetting("الحالى ");//
                        break;
                    case 25:
                        MessgSetting("الحالى ");//
                        break;
                    case 26:
                        MessgSetting("الحالى ");//
                        break;
                    case 27:
                        MessgSetting("الحالى ");//
                        break;
                    case 28:
                        MessgSetting("الحالى ");//
                        break;
                    case 29:
                        MessgSetting("الحالى ");//
                        break;
                    case 30:
                        MessgSetting("الحالى ");//
                        break;
                    case 31:
                        MessgSetting("الحالى ");//
                        break;
                    case 32:
                        MessgSetting("الحالى ");//
                        break;
                    case 33:
                        MessgSetting("الحالى ");//
                        break;
                    case 34:
                        MessgSetting("الحالى ");//
                        break;
                    case 35:
                        MessgSetting("الحالى ");//
                        break;
                    case 36:
                        MessgSetting("الحالى ");//
                        break;
                    case 37:
                        MessgSetting("الحالى ");//
                        break;
                    case 38:
                        MessgSetting("الحالى ");//
                        break;
                    case 39:
                        MessgSetting("الحالى ");//
                        break;
                    case 40:
                        MessgSetting("الحالى ");//
                        break;
                    case 41:
                        MessgSetting("الحالى ");//
                        break;
                    case 42:
                        MessgSetting("الحالى ");//
                        break;
                    case 43:
                        MessgSetting("الحالى ");//
                        break;
                    case 44:
                        MessgSetting("الحالى ");//
                        break;
                    case 45:
                        MessgSetting("الحالى ");//
                        break;
                    case 46:
                        MessgSetting("الحالى ");//
                        break;
                    case 47:
                        MessgSetting("الحالى ");//
                        break;
                    case 48:
                        MessgSetting("الحالى ");//
                        break;
                    case 49:
                        MessgSetting("الحالى ");//
                        break;
                    case 50:
                        MessgSetting("الحالى ");//
                        break;
                    case 51:
                        MessgSetting("الحالى ");//
                        break;
                    case 52:
                        MessgSetting("الحالى ");//
                        break;
                    #endregion 
                    default:
                        throw new NotSupportedException($"كود التقرير {currentReportId} غير مدعوم");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحضير التقرير: {ex.Message}", "خطأ",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }
        private void MessgSetting(string messg) { CustomMessageBox.ShowInformation("جاري اعداد التقرير ...."+ messg, "توقف"); }
        #region ############# تقارير الأصناف Product Reports ############

        /// <summary>
        /// تقرير كارت الصنف (مخزوني)
        /// </summary>
        private void rpt_GetProductCard()
        {
            const string reportFileName = "rptProductCard.rdlc";
            string reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views", "Reports", reportFileName);

            if (!File.Exists(reportPath))
            {
                MessageBox.Show($"ملف التقرير غير موجود في المسار:\n{reportPath}", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // تأكد أن EntityID ليس null، وإذا كان null ضع قيمة افتراضية مناسبة أو DBNull.Value
            object entityId = EntityID.HasValue ? EntityID.Value : DBNull.Value;

            var reportParameters = new Dictionary<string, object>
            {
                ["ReportName"] = reportFileName,
                ["DataSetName"] = "dsProductCard",
                ["ID"] = entityId,
                ["WarehouseID"] = cbxWarehouse?.SelectedValue ?? 1,
                ["StartDate"] = dtpStart.Value,
                ["EndDate"] = dtpEnd.Value,
                ["WayMove"] = "noKye"
            };

            using (var viewer = new frmMainViewer(reportParameters))
            {
                viewer.ShowDialog();
            }
        }


        /// <summary>
        /// تقرير الحركات التفصيلية - المشتريات
        /// </summary>
        private void DisplayPurchaseMovementsReport()
        {
            const string reportFileName = "rptGetDetailedMovements.rdlc";
            string reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views", "Reports", reportFileName);

            if (!File.Exists(reportPath))
            {
                MessageBox.Show($"ملف التقرير غير موجود في المسار:\n{reportPath}", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // التأكد من أن EntityID ليست null، وإن كانت null يتم تمرير DBNull.Value
            object entityId = EntityID.HasValue ? EntityID.Value : DBNull.Value;

            // التأكد من أن قيمة WarehouseID ليست null، وإن كانت null يتم تمرير القيمة الافتراضية 1
            object warehouseId = cbxWarehouse.SelectedValue ?? 1;

            var reportParameters = new Dictionary<string, object>
            {
                ["ReportName"] = reportFileName,
                ["DataSetName"] = "dsGetDetailedMovements",
                ["ID"] = entityId,
                ["WarehouseID"] = warehouseId,
                ["StartDate"] = dtpStart.Value,
                ["EndDate"] = dtpEnd.Value,
                ["WayMove"] = "Purchases"
            };

            using (var viewer = new frmMainViewer(reportParameters))
            {
                viewer.ShowDialog();
            }
        }


        /// <summary>
        /// تقرير الحركات التفصيلية - المبيعات
        /// </summary>
        // تقرير تفصيلي لحركات المبيعات للصنف
        private void rpt_GetDetailedMovementsSales()
        {
            string reportName = "rptGetDetailedMovements.rdlc";
            string reportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views", "Reports", reportName);

            // التحقق من وجود ملف التقرير
            if (!File.Exists(reportPath))
            {
                MessageBox.Show($"ملف التقرير غير موجود في المسار:\n{reportPath}", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            object entityId = (object?)EntityID ?? DBNull.Value;
            object? warehouseId = cbxWarehouse.SelectedValue ?? DBNull.Value;

            var parameters = new Dictionary<string, object>
    {
        { "ReportName", reportName },
        { "DataSetName", "dsGetDetailedMovements" },
        { "ID", entityId },
        { "WarehouseID", warehouseId },
        { "StartDate", dtpStart.Value },
        { "EndDate", dtpEnd.Value },
        { "WayMove", "Sales" }
    };

            using var viewer = new frmMainViewer(parameters);
            viewer.ShowDialog();
        }

        #endregion
        //private void rep_Products_Filtered()
        //{
        //    try
        //    {
        //        // إنشاء DataTable يحتوي على البيانات (مثال)
  
        //        var parameters = new Dictionary<string, object>
        //{
        //    { "ReportName", "repItemMovement.rdlc" },                       // اسم ملف التقرير
        //    { "DataSetName", "dsItemMovement" },                            // اسم مجموعة البيانات
        //    { "ID", EntityID },                                             // المعرف الرئيسي
        //    { "StartDate", dtpStart.Value },                                // تاريخ البداية
        //    { "EndDate",  dtpEnd.Value},                                    // تاريخ النهاية
        //    { "DataTable", FilteredData },                                  // جدول البيانات
        //    { "StoredProcedure", "rep_Products_Filtered" },                 // اسم الإجراء المخزن (اختياري)
        //    { "PrinterName", cbxPrinters .SelectedItem?.ToString() },       // اسم الطابعة (اختياري)
        //    { "WarehouseID", cbxWarehouse .SelectedValue}                   //  رقم الفرع
        //};

        //        // فتح نموذج العرض مع المعلمات
        //        frmMainViewer reportViewer = new frmMainViewer(parameters);
        //        reportViewer.Show();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"حدث خطأ أثناء تحضير التقرير: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        private void CustomerAccount()
        {
            CustomMessageBox.ShowInformation("جاري اعداد التقرير ....", "توقف");
        }
        private void SupplierAccount() { CustomMessageBox.ShowInformation("جاري اعداد التقرير ....", "توقف"); }
        private void TrialBalance() { CustomMessageBox.ShowInformation("جاري اعداد التقرير ....", "توقف"); }
        private void GeneralLedger() { CustomMessageBox.ShowInformation("جاري اعداد التقرير ....", "توقف"); }
        private void BankStatement() { CustomMessageBox.ShowInformation("جاري اعداد التقرير ....", "توقف"); }
        //private void DebitNote() 
        //{
        //    string reportName = "rptBar.rdlc";
        //    string reportPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views", "Reports", reportName);

        //    // التحقق من وجود ملف التقرير
        //    if (!System.IO.File.Exists(reportPath))
        //    {
        //        MessageBox.Show("ملف التقرير غير موجود في المسار:\n" + reportPath, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        return;
        //    }

        //    var parameters = new Dictionary<string, object>
        //    {
        //        { "ReportName", reportName },
        //        { "DataSetName", "dsBar" },
        //        { "ID", EntityID }, // رقم الصنف
        //        { "WarehouseID", cbxWarehouse.SelectedValue }, // رقم اكبر من 1 لمخزن معين أو 1 لكل المخازن
        //        { "StartDate", dtpStart.Value },
        //        { "EndDate", dtpEnd.Value },
        //        { "WayMove", "" }
        //    };

        //    frmMainViewer viewer = new frmMainViewer(parameters);
        //    viewer.ShowDialog();
        //}
        private void CreditNote() { CustomMessageBox.ShowInformation("جاري اعداد التقرير ....", "توقف"); }
        private void TradingAccount() { CustomMessageBox.ShowInformation("جاري اعداد التقرير ....", "توقف"); }
        private void IncomeStatement() { CustomMessageBox.ShowInformation("جاري اعداد التقرير ....", "توقف"); }
        private void BalanceSheet() { CustomMessageBox.ShowInformation("جاري اعداد التقرير ....", "توقف"); }
        private void CashFlow() { CustomMessageBox.ShowInformation("جاري اعداد التقرير ....", "توقف"); }
        private void EquityChange() { CustomMessageBox.ShowInformation("جاري اعداد التقرير ....", "توقف"); }
        private void FinancialPosition() { CustomMessageBox.ShowInformation("جاري اعداد التقرير ....", "توقف"); }
        private void InternalAudit() { CustomMessageBox.ShowInformation("جاري اعداد التقرير ....", "توقف"); }
        private void ItemCard() { CustomMessageBox.ShowInformation("جاري اعداد التقرير ....", "توقف"); }
        private void ItemMovement(){ CustomMessageBox.ShowInformation("جاري اعداد التقرير ....", "توقف"); }
        private void InventoryCount() { CustomMessageBox.ShowInformation("جاري اعداد التقرير ....", "توقف"); }
        private void MinimumStock() { CustomMessageBox.ShowInformation("جاري اعداد التقرير ....", "توقف"); }
        private void InactiveItems() { CustomMessageBox.ShowInformation("جاري اعداد التقرير ....", "توقف"); }
        private void ItemAnalysis() { CustomMessageBox.ShowInformation("جاري اعداد التقرير ....", "توقف"); }
        private void TopSellingItems() { CustomMessageBox.ShowInformation("جاري اعداد التقرير ....", "توقف"); }
#endregion 
        private void btnPrint_Click(object sender, EventArgs e)
        {
            InitializeReport();
            PrintReport();
        }
        /// طباعة التقرير مباشرة
        private void PrintReport()
        {
            // هنا يجب تنفيذ كود الطباعة المباشرة
            MessageBox.Show("جاري طباعة التقرير...");
        }
        /// معاينة التقرير قبل الطباعة
        private void PreviewReport()
        {
            // هنا يجب تنفيذ كود معاينة التقرير
            MessageBox.Show("جاري تحضير التقرير للمعاينة...");
        }
        #region ======== مهام رئسية ============
        /// إعداد معالجات الأحداث للعناصر المختلفة
        private void SetupEventHandlers()
        {
            // عند تغيير تاريخ البداية أو النهاية، نقوم بحساب عدد الأيام
            dtpStart.ValueChanged += (s, e) => CalculateDaysBetweenDates();
            dtpEnd.ValueChanged += (s, e) => CalculateDaysBetweenDates();

            // معالجات أحداث أزرار الراديو لتحديد الفترات الزمنية
            rdoAllPeriod.CheckedChanged += (s, e) => SetPeriodForAll();
            rdoToDay.CheckedChanged += (s, e) => SetPeriodForToday();
            rdoPreviousDay.CheckedChanged += (s, e) => SetPeriodForPreviousDay();
            rdoPreviousMonth.CheckedChanged += (s, e) => SetPeriodForPreviousMonth();
            rdoThisMonth.CheckedChanged += (s, e) => SetPeriodForCurrentMonth();
            rdoThisYear.CheckedChanged += (s, e) => SetPeriodForCurrentYear();
            rdoPreviousYear.CheckedChanged += (s, e) => SetPeriodForPreviousYear();

        }
        /// تحميل قائمة الطابعات المثبتة على النظام
        private void LoadPrinters()
        {
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                cbxPrinters.Items.Add(printer);
            }

            // تحديد الطابعة الافتراضية للنظام
            cbxPrinters.SelectedItem = new System.Drawing.Printing.PrinterSettings().PrinterName;
        }
        private void LoadWar55ehouses()
        {
            try
            {
                // مسح العناصر الحالية
                cbxWarehouse.Items.Clear();

                // جلب البيانات من قاعدة البيانات
                DataTable dt = DBServiecs.GenralData_GetWarehouse();

                // التحقق من وجود بيانات
                if (dt != null && dt.Rows.Count > 0)
                {
                    // تعيين مصدر البيانات لـ ComboBox
                    cbxWarehouse.DataSource = dt;
                    cbxWarehouse.DisplayMember = "WarehouseName"; // ما سيتم عرضه للمستخدم
                    cbxWarehouse.ValueMember = "WarehouseId";     // القيمة المخفية المرتبطة بكل عنصر

                    // اختيار العنصر الأول تلقائياً (اختياري)
                    if (cbxWarehouse.Items.Count > 0)
                        cbxWarehouse.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("لا توجد فروع مسجلة في النظام", "تحذير",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل الفروع: {ex.Message}", "خطأ",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //اريد بعد التعبئة جعل العميل لا يستطيع تغيير الاختيار فيه مع امكانية الاطلاع على اسماء الفروع كلها
        }
        private void LoadWarehouses()
        {
            try
            {
                cbxWarehouse.Items.Clear();
                DataTable dt = DBServiecs.GenralData_GetWarehouse();

                if (dt != null && dt.Rows.Count > 0)
                {
                    cbxWarehouse.DataSource = dt;
                    cbxWarehouse.DisplayMember = "WarehouseName";
                    cbxWarehouse.ValueMember = "WarehouseId";
                   
                    if (cbxWarehouse.Items.Count > 0)
                        cbxWarehouse.SelectedIndex = 0;

                    // جعل ComboBox للعرض فقط مع السماح بفتح القائمة
                    cbxWarehouse.DropDownStyle = ComboBoxStyle.DropDownList;
                    cbxWarehouse.Enabled = true;  // أو true حسب ما تريد
                //    cbxWarehouse.SelectedValue = 1;
                    // أو استخدام هذا الحدث لمنع التغيير:
                    cbxWarehouse.SelectedIndexChanged += (s, e) =>
                    {
                        cbxWarehouse.SelectedValue  = 1; // إعادة تعيين للقيمة الافتراضية
                    };
                }
                else
                {
                    MessageBox.Show("لا توجد فروع مسجلة في النظام", "تحذير",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل الفروع: {ex.Message}", "خطأ",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region  ============= وظائف تحديد الفترات الزمنية حسب أزرار الراديو =============
        // دالة مساعدة لتحديد زر الراديو المناسب
        private void SetSelectedRadioButton(string radioButtonName)
        {
            switch (radioButtonName)
            {
                case "rdoAllPeriod":
                    rdoAllPeriod.Checked = true;
                   
                    break;
                case "rdoToDay":
                    rdoToDay.Checked = true;
                   
                    break;
                case "rdoPreviousDay":
                    rdoPreviousDay.Checked = true;
                   
                    break;
                case "rdoPreviousMonth":
                    rdoPreviousMonth.Checked = true;
                   
                    break;
                case "rdoThisMonth":
                    rdoThisMonth.Checked = true;
                   
                    break;
                case "rdoThisYear":
                    rdoThisYear.Checked = true;
                    break;
                case "rdoPreviousYear":
                    rdoPreviousYear.Checked = true;
                    break;
                default:
                    // إذا لم يتم التعرف على الزر، يمكنك تحديد قيمة افتراضية
                    rdoToDay.Checked = true;
                    break;
            }
        }

        /// حساب عدد الأيام بين تاريخ البداية والنهاية وعرضها
        private void CalculateDaysBetweenDates()
        {
            TimeSpan span = dtpEnd.Value.Date - dtpStart.Value.Date;
            lblAmountOfDay.Text = $"{span.Days + 1} يوم";
        }
        /// تحديد الفترة الكاملة من تاريخ بداية الحسابات إلى نهاية السنة الحالية
        private void SetPeriodForAll()
        {
            if (!rdoAllPeriod.Checked) return;

            try
            {
                // جلب تاريخ بداية الحسابات من قاعدة البيانات
                DataTable dtStartDate = DBServiecs.GenralData_GetStartAccountsDate();

                // التحقق من وجود بيانات وعدم كون الجدول فارغاً
                if (dtStartDate != null && dtStartDate.Rows.Count > 0)
                {
                    // التحقق من عدم وجود قيمة DBNull
                    if (dtStartDate.Rows[0][0] != DBNull.Value)
                    {
                        try
                        {
                            DateTime startDate = Convert.ToDateTime(dtStartDate.Rows[0][0]);
                            dtpStart.Value = startDate;
                        }
                        catch (InvalidCastException ex)
                        {
                            HandleDateError("نوع البيانات غير صالح لتاريخ بداية الحسابات", ex);
                        }
                        catch (FormatException ex)
                        {
                            HandleDateError("تنسيق تاريخ بداية الحسابات غير صحيح", ex);
                        }
                    }
                    else
                    {
                        HandleNullDate();
                    }
                }
                else
                {
                    HandleEmptyData();
                }
            }
            catch (Exception ex)
            {
                HandleGeneralError("حدث خطأ غير متوقع أثناء جلب تاريخ بداية الحسابات", ex);
            }

            // تحديد نهاية السنة الحالية (لا تحتاج لمعالجة أخطاء)
            dtpEnd.Value = new DateTime(DateTime.Now.Year, 12, 31);
        }

        // ===== دوال مساعدة لمعالجة الأخطاء =====
        private void HandleDateError(string message, Exception ex)
        {
            dtpStart.Value = DateTime.Today;
            MessageBox.Show($"{message}\nتم استخدام تاريخ اليوم بدلاً منه",
                          "تحذير",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Warning);
        }

        private void HandleNullDate()
        {
            dtpStart.Value = DateTime.Today;
            MessageBox.Show("تاريخ بداية الحسابات غير محدد (NULL)\nتم استخدام تاريخ اليوم بدلاً منه",
                          "تحذير",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Warning);
        }

        private void HandleEmptyData()
        {
            dtpStart.Value = DateTime.Today;
            MessageBox.Show("لم يتم العثور على تاريخ بداية الحسابات\nتم استخدام تاريخ اليوم بدلاً منه",
                          "تحذير",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Warning);
        }

        private void HandleGeneralError(string message, Exception ex)
        {
            dtpStart.Value = DateTime.Today;
            MessageBox.Show($"{message}\nتم استخدام تاريخ اليوم بدلاً منه",
                          "خطأ",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Error);
           // LogError(ex); // يمكنك استبدالها بأسلوب تسجيل الأخطاء الخاص بك
        }

        /// تحديد فترة اليوم الحالي فقط
        private void SetPeriodForToday()
        {
            if (!rdoToDay.Checked) return;

            dtpStart.Value = DateTime.Today;
            dtpEnd.Value = DateTime.Today;
            lblAllPeriod.Text = "";
        }

        /// تحديد فترة اليوم السابق فقط
        private void SetPeriodForPreviousDay()
        {
            if (!rdoPreviousDay.Checked) return;

            DateTime yesterday = DateTime.Today.AddDays(-1);
            dtpStart.Value = yesterday;
            dtpEnd.Value = yesterday;
            lblAllPeriod.Text = "";
        }

        /// تحديد فترة الشهر الحالي من أول يوم إلى آخر يوم
        private void SetPeriodForCurrentMonth()
        {
            if (!rdoThisMonth.Checked) return;

            DateTime today = DateTime.Today;
            dtpStart.Value = new DateTime(today.Year, today.Month, 1);
            dtpEnd.Value = new DateTime(today.Year, today.Month,
                                      DateTime.DaysInMonth(today.Year, today.Month));
            lblAllPeriod.Text = "";
        }

        /// تحديد فترة الشهر السابق من أول يوم إلى آخر يوم
        private void SetPeriodForPreviousMonth()
        {
            if (!rdoPreviousMonth.Checked) return;

            DateTime firstDayOfLastMonth = DateTime.Today.AddMonths(-1);
            firstDayOfLastMonth = new DateTime(firstDayOfLastMonth.Year, firstDayOfLastMonth.Month, 1);

            dtpStart.Value = firstDayOfLastMonth;
            dtpEnd.Value = new DateTime(firstDayOfLastMonth.Year, firstDayOfLastMonth.Month,
                                       DateTime.DaysInMonth(firstDayOfLastMonth.Year, firstDayOfLastMonth.Month));
            lblAllPeriod.Text = "";
        }

        /// تحديد فترة السنة الحالية من أول يوم إلى آخر يوم
        private void SetPeriodForCurrentYear()
        {
            if (!rdoThisYear.Checked) return;

            dtpStart.Value = new DateTime(DateTime.Now.Year, 1, 1);
            dtpEnd.Value = new DateTime(DateTime.Now.Year, 12, 31);
            lblAllPeriod.Text = "";
        }

        /// تحديد فترة السنة السابقة من أول يوم إلى آخر يوم
        private void SetPeriodForPreviousYear()
        {
            if (!rdoPreviousYear.Checked) return;

            int lastYear = DateTime.Now.Year - 1;
            dtpStart.Value = new DateTime(lastYear, 1, 1);
            dtpEnd.Value = new DateTime(lastYear, 12, 31);
            lblAllPeriod.Text = "";
        }
        #endregion

        #region  حفظ و تحميل الإعدادات الافتراضية من قاعدة البيانات
        private void LoadDefaultSettings()
        {
            try
            {
                DataTable? dt = DBServiecs.GenralData_GetDefRepData();

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    // تعيين تواريخ البداية والنهاية من الإعدادات المحفوظة
                    dtpStart.Value = row["DefStartPeriod"] != DBNull.Value
                        ? Convert.ToDateTime(row["DefStartPeriod"])
                        : DateTime.Today;

                    dtpEnd.Value = row["DefEndPeriod"] != DBNull.Value
                        ? Convert.ToDateTime(row["DefEndPeriod"])
                        : DateTime.Today;

                    // تعيين الطابعة الافتراضية
                    if (row["DefaultPrinter"] != DBNull.Value)
                    {
                        string? printer = row["DefaultPrinter"]?.ToString();
                        if (!string.IsNullOrEmpty(printer) && cbxPrinters.Items.Contains(printer))
                            cbxPrinters.SelectedItem = printer;
                    }

                    // تعيين المخزن الافتراضي
                    string? warehouse = row["DefaultWarehouse"]?.ToString(); // تأكد من وجود هذا العمود في قاعدة البيانات
                    if (!string.IsNullOrEmpty(warehouse))
                    {
                        for (int i = 0; i < cbxWarehouse.Items.Count; i++)
                        {
                            var item = cbxWarehouse.Items[i];
                            if (item != null)
                            {
                                var propInfo = item.GetType().GetProperty(cbxWarehouse.ValueMember);
                                string? value = propInfo?.GetValue(item)?.ToString()?.Trim();

                                if (!string.IsNullOrEmpty(value) && value == warehouse)
                                {
                                    cbxWarehouse.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }

                    // تحديد زر الراديو المناسب من القيمة المحفوظة
                    if (dt.Columns.Contains("DefEndRdoChecked") && row["DefEndRdoChecked"] != DBNull.Value)
                    {
                        string? savedRadio = row["DefEndRdoChecked"]?.ToString();
                        if (!string.IsNullOrEmpty(savedRadio))
                            SetSelectedRadioButton(savedRadio);
                    }
                }
                else
                {
                    // إذا لم توجد إعدادات، نستخدم التاريخ الحالي
                    dtpStart.Value = DateTime.Today;
                    dtpEnd.Value = DateTime.Today;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء تحميل الإعدادات الافتراضية: " + ex.Message,
                              "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SaveAndClose()
        {
            try
            {
                // الحصول على اسم زر الراديو المحدد
                string selectedRadioName = GetSelectedRadioName();

                // الحصول على قيمة الطابعة المحددة
                string selectedPrinter = cbxPrinters.SelectedItem?.ToString() ?? string.Empty;

                // الحصول على قيمة الفرع المحدد بشكل صحيح
                string selectedWarehouse = string.Empty;

                if (cbxWarehouse.SelectedItem != null)
                {
                    if (cbxWarehouse.SelectedItem is DataRowView rowView)
                    {
                        // إذا كان العنصر المحدد من نوع DataRowView (في حالة الربط ببيانات)
                        selectedWarehouse = rowView["WarehouseName"]?.ToString() ?? string.Empty;
                    }
                    else
                    {
                        // إذا كان العنصر المحدد نصياً عادياً
                        selectedWarehouse = cbxWarehouse.SelectedItem?.ToString() ?? string.Empty;
                    }
                }

                // حفظ الإعدادات الحالية في قاعدة البيانات
                DBServiecs.GenralData_SaveDefRepData(
                    dtpStart.Value,
                    dtpEnd.Value,
                    selectedPrinter,
                    selectedWarehouse,
                    selectedRadioName
                );

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء حفظ الإعدادات: " + ex.Message, "خطأ",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                // يمكنك تسجيل الخطأ هنا إذا كنت تستخدم نظام تسجيل الأخطاء
            }
        }
        // دالة مساعدة للحصول على اسم زر الراديو المحدد
        private string GetSelectedRadioName()
        {
            if (rdoAllPeriod.Checked) return "rdoAllPeriod";
            if (rdoToDay.Checked) return "rdoToDay";
            if (rdoPreviousDay.Checked) return "rdoPreviousDay";
            if (rdoPreviousMonth.Checked) return "rdoPreviousMonth";
            if (rdoThisMonth.Checked) return "rdoThisMonth";
            if (rdoThisYear.Checked) return "rdoThisYear";
            if (rdoPreviousYear.Checked) return "rdoPreviousYear";

            return string.Empty; // في حالة عدم تحديد أي زر
        }
        private void btnSaveAndClose_Click(object sender, EventArgs e)
        {
            SaveAndClose();
        }


        #endregion

        private void btnAllPeriod_Click(object sender, EventArgs e)
        {
            rdoAllPeriod.Checked = true;
            lblAllPeriod.Text = "كل الفترة";
        }
    }
}
