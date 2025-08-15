/*
using System;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses.ReportsClasses;

namespace MizanOriginalSoft.Views.Reports
{
    public partial class frmSettingReports : Form
    {
        // المتغيرات الخاصة بتخزين بيانات التقرير ومعلومات الإعدادات
        private readonly string? _repCodeName;
        private readonly string? _repDisplayName;
        private readonly int _reportId;

        // مسار ملف الإعدادات التي سيتم قراءة/كتابة القيم فيه
        private readonly string SettingsFilePath = @"serverConnectionSettings.txt";

        // قاموس لتخزين معلمات التقرير المستلمة عند إنشاء النموذج
        private readonly Dictionary<string, object> _parameters;

        // المُنشئ يستقبل معلمات التقرير ويهيئ النموذج
        public frmSettingReports(Dictionary<string, object> parameters)
        {
            InitializeComponent();

            _parameters = parameters;

            // استخراج القيم الأساسية من المعلمات
            _repCodeName = _parameters["ReportCodeName"].ToString();
            _repDisplayName = _parameters["ReportDisplayName"].ToString();
            _reportId = Convert.ToInt32(_parameters["ReportID"]);

            // تعيين عنوان النموذج ليشمل اسم التقرير
            this.Text = $"إعدادات تقرير - {_repDisplayName}";
            lblTitel .Text = $"إعدادات تقرير - {_repDisplayName}";
            SetupEventHandlers();
        }

        // حدث تحميل النموذج - تحميل الطابعات، المستودعات، والقيم الافتراضية من الملف
        private void frmSettingReports_Load(object sender, EventArgs e)
        {
            LoadPrinters();
            LoadWarehouses();
            LoadDefaults();
        }

        // دالة لقراءة قيمة معينة من ملف الإعدادات
        private string ReadSettingValue(string key)
        {
            if (!File.Exists(SettingsFilePath))
                return string.Empty;

            foreach (var line in File.ReadAllLines(SettingsFilePath))
            {
                var parts = line.Split('=');
                if (parts.Length == 2 && parts[0].Trim() == key)
                    return parts[1].Trim();
            }

            return string.Empty;

        }

        // حدث الضغط على زر "كل الفترة" لتفعيل زر الراديو المقابل وعرض نص توضيحي.
        private void btnAllPeriod_Click(object sender, EventArgs e)
        {
            rdoAllPeriod.Checked = true;
            lblAllPeriod.Text = "كل الفترة";

            // قراءة تاريخ البداية والنهاية من ملف الإعدادات
            string startDateStr = ReadSettingValue("StartAccountsDate");
            string endDateStr = ReadSettingValue("EndAccountsDate");

            if (!string.IsNullOrEmpty(startDateStr) && DateTime.TryParse(startDateStr, out DateTime startDate))
            {
                dtpStart.Value = startDate;
            }
            else
            {
                dtpStart.Value = DateTime.Today;
                MessageBox.Show("تاريخ بداية الحسابات غير محدد أو غير صالح، تم استخدام تاريخ اليوم بدلاً منه.", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (!string.IsNullOrEmpty(endDateStr) && DateTime.TryParse(endDateStr, out DateTime endDate))
            {
                dtpEnd.Value = endDate;
            }
            else
            {
                dtpEnd.Value = DateTime.Today;
                MessageBox.Show("تاريخ نهاية الحسابات غير محدد أو غير صالح، تم استخدام تاريخ اليوم بدلاً منه.", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // تعديل دالة تعيين الفترة الكاملة لتستخدم ملف الإعدادات بشكل عام (اختياري)
        private void SetPeriodForAll()
        {
            if (!rdoAllPeriod.Checked) return;

            try
            {
                string startDateStr = ReadSettingValue("StartAccountsDate");
                string endDateStr = ReadSettingValue("EndAccountsDate");

                if (!string.IsNullOrEmpty(startDateStr) && DateTime.TryParse(startDateStr, out DateTime startDate))
                {
                    dtpStart.Value = startDate;
                }
                else
                {
                    dtpStart.Value = DateTime.Today;
                    MessageBox.Show("تاريخ بداية الحسابات غير محدد أو غير صالح، تم استخدام تاريخ اليوم بدلاً منه.", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                if (!string.IsNullOrEmpty(endDateStr) && DateTime.TryParse(endDateStr, out DateTime endDate))
                {
                    dtpEnd.Value = endDate;
                }
                else
                {
                    dtpEnd.Value = new DateTime(DateTime.Now.Year, 12, 31);
                    MessageBox.Show("تاريخ نهاية الحسابات غير محدد أو غير صالح، تم استخدام نهاية السنة الحالية بدلاً منه.", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                dtpStart.Value = DateTime.Today;
                dtpEnd.Value = new DateTime(DateTime.Now.Year, 12, 31);
                MessageBox.Show($"حدث خطأ أثناء جلب تواريخ الحسابات.\nتم استخدام التواريخ الافتراضية بدلاً منها.\n\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // دالة لتحميل الإعدادات من الملف مع تعيين القيم في الواجهة
        private void LoadDefaults()
        {
            if (!File.Exists(SettingsFilePath)) return;

            string savedRadioButtonName = ""; // اسم زر الراديو المحفوظ

            foreach (var line in File.ReadAllLines(SettingsFilePath))
            {
                var parts = line.Split('=');
                if (parts.Length != 2) continue;

                string key = parts[0].Trim();
                string value = parts[1].Trim();

                switch (key)
                {
                    case "DefaultPrinter":
                        cbxPrinters.SelectedItem = value;
                        break;
                    case "DefaultWarehouse":
                        if (int.TryParse(value, out var id))
                            cbxWarehouse.SelectedValue = id;
                        break;
                    case "DefaultStartDate":
                        if (DateTime.TryParse(value, out var start))
                            dtpStart.Value = start;
                        break;
                    case "DefaultEndDate":
                        if (DateTime.TryParse(value, out var end))
                            dtpEnd.Value = end;
                        break;
                    case "DefaultRdoCheck":
                        savedRadioButtonName = value;
                        break;
                }
            }

            if (!string.IsNullOrEmpty(savedRadioButtonName))
            {
                SetSelectedRadioButton(savedRadioButtonName);
            }
            else
            {
                SetSelectedRadioButton("rdoAllPeriod"); // قيمة افتراضية
            }
        }

        // دالة لحفظ الإعدادات مع حفظ اسم زر الراديو المختار
        private void SaveDefaults()
        {
            var lines = File.Exists(SettingsFilePath)
                ? File.ReadAllLines(SettingsFilePath).ToList()
                : new List<string>();

            UpdateOrAddLine(lines, "DefaultPrinter", cbxPrinters.SelectedItem?.ToString() ?? "");
            UpdateOrAddLine(lines, "DefaultWarehouse", cbxWarehouse.SelectedValue?.ToString() ?? "");
            UpdateOrAddLine(lines, "DefaultStartDate", dtpStart.Value.ToString("yyyy-MM-dd"));
            UpdateOrAddLine(lines, "DefaultEndDate", dtpEnd.Value.ToString("yyyy-MM-dd"));
            UpdateOrAddLine(lines, "DefaultRdoCheck", GetSelectedRadioButtonName());

            File.WriteAllLines(SettingsFilePath, lines);
        }

        // دالة للحصول على اسم زر الراديو المختار حاليًا
        private string GetSelectedRadioButtonName()
        {
            if (rdoAllPeriod.Checked) return "rdoAllPeriod";
            if (rdoToDay.Checked) return "rdoToDay";
            if (rdoPreviousDay.Checked) return "rdoPreviousDay";
            if (rdoPreviousMonth.Checked) return "rdoPreviousMonth";
            if (rdoThisMonth.Checked) return "rdoThisMonth";
            if (rdoThisYear.Checked) return "rdoThisYear";
            if (rdoPreviousYear.Checked) return "rdoPreviousYear";

            return "rdoAllPeriod"; // افتراضي
        }

        // دالة لتعيين زر الراديو حسب الاسم المحفوظ
        private void SetSelectedRadioButton(string radioButtonName)
        {
            switch (radioButtonName)
            {
                case "rdoAllPeriod": rdoAllPeriod.Checked = true; break;
                case "rdoToDay": rdoToDay.Checked = true; break;
                case "rdoPreviousDay": rdoPreviousDay.Checked = true; break;
                case "rdoPreviousMonth": rdoPreviousMonth.Checked = true; break;
                case "rdoThisMonth": rdoThisMonth.Checked = true; break;
                case "rdoThisYear": rdoThisYear.Checked = true; break;
                case "rdoPreviousYear": rdoPreviousYear.Checked = true; break;
                default: rdoAllPeriod.Checked = true; break;
            }
        }

        // تحديث السطر الخاص بمفتاح معين في القائمة أو إضافته إذا لم يكن موجودًا
        private void UpdateOrAddLine(List<string> lines, string key, string value)
        {
            string prefix = key + "=";
            int index = lines.FindIndex(line => line.StartsWith(prefix));
            string newLine = $"{key}={value}";

            if (index >= 0)
                lines[index] = newLine;  // تحديث السطر الموجود
            else
                lines.Add(newLine);      // إضافة السطر إذا غير موجود
        }

        // حدث الضغط على زر "انتقال" - تجهيز معلمات التقرير بناءً على الخيارات المختارة واستدعاء عرض التقرير
        private void btnGo_Click(object sender, EventArgs e)
        {
            string selectedValue = cbxPrinters.SelectedItem?.ToString() ?? "";

            string printMode = selectedValue switch
            {
                "معاينة" => "Preview",
                "تصدير إلى Excel" => "Excel",
                "تصدير إلى PDF" => "PDF",
                _ => "Printer"
            };

            if (string.IsNullOrEmpty(_repCodeName))
            {
                MessageBox.Show("اسم التقرير غير محدد، يرجى المحاولة لاحقاً.", "خطأ",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var parameters = new Dictionary<string, object>
            {
                ["ReportCodeName"] = _repCodeName,
                ["BranchID"] = cbxWarehouse.SelectedValue ?? DBNull.Value,
                ["StartDate"] = dtpStart.Value,
                ["EndDate"] = dtpEnd.Value,
                ["PrintMode"] = printMode,
                ["PrinterName"] = (printMode == "Printer") ? selectedValue : ""
            };

            ReportsManager.ShowReport(parameters);
        }

        // حدث الضغط على زر حفظ وإغلاق - حفظ الإعدادات وإغلاق النموذج
        private void btnSaveAndClose_Click(object sender, EventArgs e)
        {
            SaveDefaults();
            Close();
        }
        #region ======== المهام الرئيسية ========

        // إعداد معالجات الأحداث للعناصر المختلفة في النموذج.
        private void SetupEventHandlers()
        {
            // عند تغيير تاريخ البداية أو النهاية، نحسب عدد الأيام بينهما
            dtpStart.ValueChanged += (s, e) => CalculateDaysBetweenDates();
            dtpEnd.ValueChanged += (s, e) => CalculateDaysBetweenDates();

            // معالجات أحداث أزرار الراديو لتحديد الفترات الزمنية المختلفة
            rdoAllPeriod.CheckedChanged += (s, e) => SetPeriodForAll();
            rdoToDay.CheckedChanged += (s, e) => SetPeriodForToday();
            rdoPreviousDay.CheckedChanged += (s, e) => SetPeriodForPreviousDay();
            rdoPreviousMonth.CheckedChanged += (s, e) => SetPeriodForPreviousMonth();
            rdoThisMonth.CheckedChanged += (s, e) => SetPeriodForCurrentMonth();
            rdoThisYear.CheckedChanged += (s, e) => SetPeriodForCurrentYear();
            rdoPreviousYear.CheckedChanged += (s, e) => SetPeriodForPreviousYear();
        }

        // تحميل أسماء الطابعات المثبتة على الجهاز مع إضافة خيارات خاصة مثل المعاينة وتصدير Excel/PDF.
        private void LoadPrinters()
        {
            cbxPrinters.Items.Clear();

            // إضافة الطابعات المثبتة على الجهاز
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                cbxPrinters.Items.Add(printer);
            }

            // إضافة خيارات خاصة للطباعة والمعاينة والتصدير
            cbxPrinters.Items.Add("معاينة");
            cbxPrinters.Items.Add("تصدير إلى Excel");
            cbxPrinters.Items.Add("تصدير إلى PDF");

            // اختيار العنصر الأول كافتراضي
            cbxPrinters.SelectedIndex = 0;
        }

        // تحميل قائمة المستودعات من قاعدة البيانات وربطها بمربع الاختيار مع إضافة خيار "كل الفروع" برقم 0.
        private void LoadWarehouses()
        {
            try
            {
                DataTable dt = DBServiecs.Warehouse_GetAll();

                if (dt != null && dt.Rows.Count > 0)
                {
                    // إنشاء صف جديد يمثل خيار "كل الفروع" بقيمة 0
                    DataRow allBranchesRow = dt.NewRow();
                    allBranchesRow["WarehouseId"] = 0;
                    allBranchesRow["WarehouseName"] = "كل الفروع";

                    // إضافة صف "كل الفروع" في بداية الجدول
                    dt.Rows.InsertAt(allBranchesRow, 0);

                    // ربط البيانات بـ ComboBox
                    cbxWarehouse.DataSource = dt;
                    cbxWarehouse.DisplayMember = "WarehouseName";
                    cbxWarehouse.ValueMember = "WarehouseId";

                    cbxWarehouse.DropDownStyle = ComboBoxStyle.DropDownList;
                    cbxWarehouse.Enabled = true;

                    // تعيين الخيار الأول افتراضياً (وهو "كل الفروع")
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
        }

        // حساب عدد الأيام بين تاريخ البداية والنهاية وعرضها في التسمية.
        private void CalculateDaysBetweenDates()
        {
            TimeSpan span = dtpEnd.Value.Date - dtpStart.Value.Date;
            lblAmountOfDay.Text = $"{span.Days + 1} يوم";
        }

        // تعيين فترة اليوم الحالي فقط.
        private void SetPeriodForToday()
        {
            if (!rdoToDay.Checked) return;

            dtpStart.Value = DateTime.Today;
            dtpEnd.Value = DateTime.Today;
            lblAllPeriod.Text = "";
        }

        // تعيين فترة اليوم السابق فقط.
        private void SetPeriodForPreviousDay()
        {
            if (!rdoPreviousDay.Checked) return;

            DateTime yesterday = DateTime.Today.AddDays(-1);
            dtpStart.Value = yesterday;
            dtpEnd.Value = yesterday;
            lblAllPeriod.Text = "";
        }

        // تعيين فترة الشهر الحالي من أول يوم إلى آخر يوم.
        private void SetPeriodForCurrentMonth()
        {
            if (!rdoThisMonth.Checked) return;

            DateTime today = DateTime.Today;
            dtpStart.Value = new DateTime(today.Year, today.Month, 1);
            dtpEnd.Value = new DateTime(today.Year, today.Month,
                                      DateTime.DaysInMonth(today.Year, today.Month));
            lblAllPeriod.Text = "";
        }

        // تعيين فترة الشهر السابق من أول يوم إلى آخر يوم.
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

        // تعيين فترة السنة الحالية من أول يوم إلى آخر يوم.
        private void SetPeriodForCurrentYear()
        {
            if (!rdoThisYear.Checked) return;

            dtpStart.Value = new DateTime(DateTime.Now.Year, 1, 1);
            dtpEnd.Value = new DateTime(DateTime.Now.Year, 12, 31);
            lblAllPeriod.Text = "";
        }

        // تعيين فترة السنة السابقة من أول يوم إلى آخر يوم.
        private void SetPeriodForPreviousYear()
        {
            if (!rdoPreviousYear.Checked) return;

            int lastYear = DateTime.Now.Year - 1;
            dtpStart.Value = new DateTime(lastYear, 1, 1);
            dtpEnd.Value = new DateTime(lastYear, 12, 31);
            lblAllPeriod.Text = "";
        }

        #endregion

    }
}
*/

using MizanOriginalSoft.MainClasses.OriginalClasses.ReportsClasses;
using MizanOriginalSoft.MainClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Reports
{
    public partial class frmSettingReports : Form
    {
        #region ==== المتغيرات ====

        // معلمات التقرير
        private readonly string _repCodeName;
        private readonly string _repDisplayName;
        private readonly int _reportId;

        // مسار ملف الإعدادات
        private readonly string SettingsFilePath = @"serverConnectionSettings.txt";

        // معلمات التقرير المرسلة للنموذج
        private readonly Dictionary<string, object> _parameters;

        #endregion

        #region ==== المُنشئ ====

        public frmSettingReports(Dictionary<string, object> parameters)
        {
            InitializeComponent();

            _parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));

            // محاولة استخراج القيم الأساسية من المعلمات مع تأمين من null
            _repCodeName = _parameters.TryGetValue("ReportCodeName", out var codeName) ? codeName?.ToString() ?? "" : "";
            _repDisplayName = _parameters.TryGetValue("ReportDisplayName", out var displayName) ? displayName?.ToString() ?? "" : "";
            _reportId = _parameters.TryGetValue("ReportID", out var id) && int.TryParse(id?.ToString(), out int tempId) ? tempId : 0;

            this.Text = $"إعدادات تقرير - {_repDisplayName}";
            lblTitel.Text = $"إعدادات تقرير - {_repDisplayName}";

            SetupEventHandlers();
        }

        #endregion

        #region ==== أحداث النموذج ====

        private void frmSettingReports_Load(object sender, EventArgs e)
        {
            LoadPrinters();
            LoadWarehouses();
            LoadDefaults();
        }

        #endregion

        #region ==== دوال قراءة وكتابة ملف الإعدادات ====

        /// <summary>
        /// قراءة قيمة إعداد من ملف الإعدادات حسب المفتاح.
        /// ترجع سلسلة فارغة إذا لم يوجد المفتاح أو الملف.
        /// </summary>
        private string ReadSettingValue(string key)
        {
            if (!File.Exists(SettingsFilePath))
                return string.Empty;

            foreach (var line in File.ReadAllLines(SettingsFilePath))
            {
                var parts = line.Split('=');
                if (parts.Length == 2 && parts[0].Trim() == key)
                    return parts[1].Trim();
            }

            return string.Empty;
        }

        /// <summary>
        /// تحديث أو إضافة سطر في قائمة الإعدادات حسب المفتاح والقيمة.
        /// </summary>
        private void UpdateOrAddLine(List<string> lines, string key, string value)
        {
            string prefix = key + "=";
            int index = lines.FindIndex(line => line.StartsWith(prefix));
            string newLine = $"{key}={value}";

            if (index >= 0)
                lines[index] = newLine;
            else
                lines.Add(newLine);
        }

        /// <summary>
        /// تحميل الإعدادات من ملف النص وتعيين القيم في الواجهة.
        /// </summary>
        private void LoadDefaults()
        {
            if (!File.Exists(SettingsFilePath)) return;

            string savedRadioButtonName = "";

            foreach (var line in File.ReadAllLines(SettingsFilePath))
            {
                var parts = line.Split('=');
                if (parts.Length != 2) continue;

                string key = parts[0].Trim();
                string value = parts[1].Trim();

                switch (key)
                {
                    case "DefaultPrinter":
                        cbxPrinters.SelectedItem = value;
                        break;
                    case "DefaultWarehouse":
                        if (int.TryParse(value, out int warehouseId))
                            cbxWarehouse.SelectedValue = warehouseId;
                        break;
                    case "DefaultStartDate":
                        if (DateTime.TryParse(value, out DateTime startDate))
                            dtpStart.Value = startDate;
                        break;
                    case "DefaultEndDate":
                        if (DateTime.TryParse(value, out DateTime endDate))
                            dtpEnd.Value = endDate;
                        break;
                    case "DefaultRdoCheck":
                        savedRadioButtonName = value;
                        break;
                }
            }

            if (!string.IsNullOrEmpty(savedRadioButtonName))
                SetSelectedRadioButton(savedRadioButtonName);
            else
                SetSelectedRadioButton("rdoAllPeriod");
        }

        /// <summary>
        /// حفظ الإعدادات الحالية في ملف الإعدادات مع تحديث السطور دون حذف باقي المحتويات.
        /// </summary>
        private void SaveDefaults()
        {
            var lines = File.Exists(SettingsFilePath)
                ? File.ReadAllLines(SettingsFilePath).ToList()
                : new List<string>();

            UpdateOrAddLine(lines, "DefaultPrinter", cbxPrinters.SelectedItem?.ToString() ?? "");
            UpdateOrAddLine(lines, "DefaultWarehouse", cbxWarehouse.SelectedValue?.ToString() ?? "");
            UpdateOrAddLine(lines, "DefaultStartDate", dtpStart.Value.ToString("yyyy-MM-dd"));
            UpdateOrAddLine(lines, "DefaultEndDate", dtpEnd.Value.ToString("yyyy-MM-dd"));
            UpdateOrAddLine(lines, "DefaultRdoCheck", GetSelectedRadioButtonName());

            File.WriteAllLines(SettingsFilePath, lines);
        }

        #endregion

        #region ==== دوال التعامل مع أزرار الراديو والفترات ====

        /// <summary>
        /// تعيين زر الراديو المختار بناءً على الاسم المحفوظ.
        /// </summary>
        private void SetSelectedRadioButton(string radioButtonName)
        {
            switch (radioButtonName)
            {
                case "rdoAllPeriod": rdoAllPeriod.Checked = true; break;
                case "rdoToDay": rdoToDay.Checked = true; break;
                case "rdoPreviousDay": rdoPreviousDay.Checked = true; break;
                case "rdoPreviousMonth": rdoPreviousMonth.Checked = true; break;
                case "rdoThisMonth": rdoThisMonth.Checked = true; break;
                case "rdoThisYear": rdoThisYear.Checked = true; break;
                case "rdoPreviousYear": rdoPreviousYear.Checked = true; break;
                default: rdoAllPeriod.Checked = true; break;
            }
        }

        /// <summary>
        /// الحصول على اسم زر الراديو المختار حالياً.
        /// </summary>
        private string GetSelectedRadioButtonName()
        {
            if (rdoAllPeriod.Checked) return "rdoAllPeriod";
            if (rdoToDay.Checked) return "rdoToDay";
            if (rdoPreviousDay.Checked) return "rdoPreviousDay";
            if (rdoPreviousMonth.Checked) return "rdoPreviousMonth";
            if (rdoThisMonth.Checked) return "rdoThisMonth";
            if (rdoThisYear.Checked) return "rdoThisYear";
            if (rdoPreviousYear.Checked) return "rdoPreviousYear";

            return "rdoAllPeriod";
        }

        /// <summary>
        /// عند تغيير حالة زر الراديو الخاص بـ "كل الفترة"، يتم تعيين التواريخ من ملف الإعدادات.
        /// </summary>
        private void btnAllPeriod_Click(object sender, EventArgs e)
        {
            rdoAllPeriod.Checked = true;
            lblAllPeriod.Text = "كل الفترة";

            // جلب تواريخ البداية والنهاية من ملف الإعدادات
            string startDateStr = ReadSettingValue("StartAccountsDate");
            string endDateStr = ReadSettingValue("EndAccountsDate");

            if (!string.IsNullOrEmpty(startDateStr) && DateTime.TryParse(startDateStr, out DateTime startDate))
                dtpStart.Value = startDate;
            else
            {
                dtpStart.Value = DateTime.Today;
                MessageBox.Show("تاريخ بداية الحسابات غير محدد أو غير صالح، تم استخدام تاريخ اليوم بدلاً منه.", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (!string.IsNullOrEmpty(endDateStr) && DateTime.TryParse(endDateStr, out DateTime endDate))
                dtpEnd.Value = endDate;
            else
            {
                dtpEnd.Value = DateTime.Today;
                MessageBox.Show("تاريخ نهاية الحسابات غير محدد أو غير صالح، تم استخدام تاريخ اليوم بدلاً منه.", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// تعيين فترة "كل الفترة" عند تحديد زر الراديو المعني (يتم استدعاؤه تلقائياً عند CheckedChanged).
        /// يعيد تعيين التواريخ حسب الملف مع معالجة الأخطاء.
        /// </summary>
        private void SetPeriodForAll()
        {
            if (!rdoAllPeriod.Checked) return;

            try
            {
                string startDateStr = ReadSettingValue("StartAccountsDate");
                string endDateStr = ReadSettingValue("EndAccountsDate");

                if (!string.IsNullOrEmpty(startDateStr) && DateTime.TryParse(startDateStr, out DateTime startDate))
                    dtpStart.Value = startDate;
                else
                {
                    dtpStart.Value = DateTime.Today;
                    MessageBox.Show("تاريخ بداية الحسابات غير محدد أو غير صالح، تم استخدام تاريخ اليوم بدلاً منه.", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                if (!string.IsNullOrEmpty(endDateStr) && DateTime.TryParse(endDateStr, out DateTime endDate))
                    dtpEnd.Value = endDate;
                else
                {
                    dtpEnd.Value = new DateTime(DateTime.Now.Year, 12, 31);
                    MessageBox.Show("تاريخ نهاية الحسابات غير محدد أو غير صالح، تم استخدام نهاية السنة الحالية بدلاً منه.", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                dtpStart.Value = DateTime.Today;
                dtpEnd.Value = new DateTime(DateTime.Now.Year, 12, 31);
                MessageBox.Show($"حدث خطأ أثناء جلب تواريخ الحسابات.\nتم استخدام التواريخ الافتراضية بدلاً منها.\n\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // باقي دوال تعيين الفترات الزمنية لأزرار الراديو الأخرى (تواريخ اليوم، الشهر، السنة، سابق، لاحق...) 
        // مثل SetPeriodForToday، SetPeriodForPreviousDay، SetPeriodForCurrentMonth، SetPeriodForPreviousMonth، SetPeriodForCurrentYear، SetPeriodForPreviousYear
        // يمكنك إضافتها بنفس النمط المذكور أعلاه.

        #endregion

        #region ==== دوال تحميل الطابعات والمستودعات وحساب الأيام ====

        /// <summary>
        /// تحميل أسماء الطابعات المثبتة وإضافة خيارات المعاينة والتصدير.
        /// </summary>
        private void LoadPrinters()
        {
            cbxPrinters.Items.Clear();

            foreach (string printer in PrinterSettings.InstalledPrinters)
                cbxPrinters.Items.Add(printer);

            cbxPrinters.Items.Add("معاينة");
            cbxPrinters.Items.Add("تصدير إلى Excel");
            cbxPrinters.Items.Add("تصدير إلى PDF");

            if (cbxPrinters.Items.Count > 0)
                cbxPrinters.SelectedIndex = 0;
        }

        /// <summary>
        /// تحميل قائمة المستودعات من قاعدة البيانات مع إضافة خيار "كل الفروع".
        /// </summary>
        private void LoadWarehouses()
        {
            try
            {
                DataTable dt = DBServiecs.Warehouse_GetAll();

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow allBranchesRow = dt.NewRow();
                    allBranchesRow["WarehouseId"] = 0;
                    allBranchesRow["WarehouseName"] = "كل الفروع";
                    dt.Rows.InsertAt(allBranchesRow, 0);

                    cbxWarehouse.DataSource = dt;
                    cbxWarehouse.DisplayMember = "WarehouseName";
                    cbxWarehouse.ValueMember = "WarehouseId";

                    cbxWarehouse.DropDownStyle = ComboBoxStyle.DropDownList;
                    cbxWarehouse.Enabled = true;

                    if (cbxWarehouse.Items.Count > 0)
                        cbxWarehouse.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("لا توجد فروع مسجلة في النظام", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cbxWarehouse.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل الفروع: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbxWarehouse.Enabled = false;
            }
        }

        /// <summary>
        /// حساب عدد الأيام بين تاريخ البداية والنهاية وعرضها في تسمية.
        /// </summary>
        private void CalculateDaysBetweenDates()
        {
            TimeSpan span = dtpEnd.Value.Date - dtpStart.Value.Date;
            lblAmountOfDay.Text = $"{span.Days + 1} يوم";
        }

        #endregion

        #region ==== دوال أزرار التقرير ====

        /// <summary>
        /// تجهيز معلمات التقرير وطلب عرضه عند الضغط على زر "انتقال".
        /// </summary>
        private void btnGo_Click(object sender, EventArgs e)
        {
            string selectedValue = cbxPrinters.SelectedItem?.ToString() ?? "";

            string printMode = selectedValue switch
            {
                "معاينة" => "Preview",
                "تصدير إلى Excel" => "Excel",
                "تصدير إلى PDF" => "PDF",
                _ => "Printer"
            };

            if (string.IsNullOrEmpty(_repCodeName))
            {
                MessageBox.Show("اسم التقرير غير محدد، يرجى المحاولة لاحقاً.", "خطأ",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var parameters = new Dictionary<string, object>
            {
                ["ReportCodeName"] = _repCodeName,
                ["BranchID"] = cbxWarehouse.SelectedValue ?? DBNull.Value,
                ["StartDate"] = dtpStart.Value,
                ["EndDate"] = dtpEnd.Value,
                ["PrintMode"] = printMode,
                ["PrinterName"] = (printMode == "Printer") ? selectedValue : ""
            };

            ReportsManager.ShowReport(parameters);
        }

        /// <summary>
        /// حفظ الإعدادات وإغلاق النموذج عند الضغط على زر الحفظ والإغلاق.
        /// </summary>
        private void btnSaveAndClose_Click(object sender, EventArgs e)
        {
            SaveDefaults();
            Close();
        }

        #endregion

        #region ==== ضبط أحداث واجهة المستخدم ====

        /// <summary>
        /// ربط معالجات الأحداث للعناصر الرئيسية في النموذج.
        /// </summary>
        private void SetupEventHandlers()
        {
            dtpStart.ValueChanged += (s, e) => CalculateDaysBetweenDates();
            dtpEnd.ValueChanged += (s, e) => CalculateDaysBetweenDates();

            rdoAllPeriod.CheckedChanged += (s, e) => SetPeriodForAll();
            rdoToDay.CheckedChanged += (s, e) => SetPeriodForToday();
            rdoPreviousDay.CheckedChanged += (s, e) => SetPeriodForPreviousDay();
            rdoPreviousMonth.CheckedChanged += (s, e) => SetPeriodForPreviousMonth();
            rdoThisMonth.CheckedChanged += (s, e) => SetPeriodForCurrentMonth();
            rdoThisYear.CheckedChanged += (s, e) => SetPeriodForCurrentYear();
            rdoPreviousYear.CheckedChanged += (s, e) => SetPeriodForPreviousYear();
        }

        #endregion

        #region ==== دوال الفترات الزمنية الأخرى (مثال) ====

        private void SetPeriodForToday()
        {
            if (!rdoToDay.Checked) return;

            dtpStart.Value = DateTime.Today;
            dtpEnd.Value = DateTime.Today;
            lblAllPeriod.Text = "";
        }

        private void SetPeriodForPreviousDay()
        {
            if (!rdoPreviousDay.Checked) return;

            DateTime yesterday = DateTime.Today.AddDays(-1);
            dtpStart.Value = yesterday;
            dtpEnd.Value = yesterday;
            lblAllPeriod.Text = "";
        }

        private void SetPeriodForCurrentMonth()
        {
            if (!rdoThisMonth.Checked) return;

            DateTime today = DateTime.Today;
            dtpStart.Value = new DateTime(today.Year, today.Month, 1);
            dtpEnd.Value = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            lblAllPeriod.Text = "";
        }

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

        private void SetPeriodForCurrentYear()
        {
            if (!rdoThisYear.Checked) return;

            dtpStart.Value = new DateTime(DateTime.Now.Year, 1, 1);
            dtpEnd.Value = new DateTime(DateTime.Now.Year, 12, 31);
            lblAllPeriod.Text = "";
        }

        private void SetPeriodForPreviousYear()
        {
            if (!rdoPreviousYear.Checked) return;

            int lastYear = DateTime.Now.Year - 1;
            dtpStart.Value = new DateTime(lastYear, 1, 1);
            dtpEnd.Value = new DateTime(lastYear, 12, 31);
            lblAllPeriod.Text = "";
        }

        #endregion
    }
}


/*
 ===============================
ملف شرح كود frmSettingReports.cs
===============================

1. الهدف:
   - هذا النموذج مسؤول عن إعدادات عرض التقارير.
   - يتيح اختيار الطابعة، الفرع (المستودع)، فترة التاريخ، ونوع الطباعة (معاينة، تصدير، طباعة).

2. أهم المتغيرات:
   - _repCodeName: اسم التقرير البرمجي.
   - _repDisplayName: اسم التقرير المعروض للمستخدم.
   - _reportId: رقم تعريف التقرير.
   - SettingsFilePath: مسار ملف الإعدادات النصي.
   - _parameters: معلمات التقرير الممررة للنموذج.

3. تحميل الإعدادات (LoadDefaults):
   - يقرأ الملف النصي settingsFilePath.
   - يعين القيم المحفوظة مثل الطابعة، الفرع، تواريخ البداية والنهاية، وزر الراديو المحدد.
   - إذا لم توجد قيمة لزر الراديو يتم اختيار "rdoAllPeriod" افتراضياً.

4. حفظ الإعدادات (SaveDefaults):
   - يحفظ الإعدادات الحالية في نفس الملف النصي.
   - يتم تحديث أو إضافة السطور حسب المفاتيح والقيم الحالية.

5. قراءة قيمة معينة من الملف (ReadSettingValue):
   - تستخدم للبحث عن مفتاح معين وإرجاع القيمة المرتبطة به.

6. التعامل مع أزرار الراديو (RadioButtons):
   - كل زر يمثل فترة زمنية معينة (اليوم، الشهر، السنة، كل الفترة، إلخ).
   - عند اختيار زر معين يتم تعيين تواريخ البداية والنهاية تلقائياً.
   - الدوال SetPeriodForAll، SetPeriodForToday، SetPeriodForPreviousDay، إلخ.

7. تحميل الطابعات (LoadPrinters):
   - يعرض كل الطابعات المثبتة بالإضافة إلى خيارات خاصة (معاينة، تصدير Excel، تصدير PDF).

8. تحميل المستودعات (LoadWarehouses):
   - يجلب قائمة المستودعات من قاعدة البيانات ويضيف خيار "كل الفروع" برقم 0.

9. حفظ وتشغيل التقرير (btnSaveAndClose_Click, btnGo_Click):
   - حفظ الإعدادات عند الإغلاق.
   - تجهيز معلمات التقرير وطلب عرضه/طباعته حسب الخيارات المحددة.

10. ملاحظات إضافية:
    - تم تقسيم الكود إلى مناطق #region لتسهيل التنظيم.
    - معالجات الأحداث مركزة في SetupEventHandlers().
    - يدعم الملف النصي تخزين الإعدادات بشكل بسيط (مفتاح=قيمة).
    - رسائل التنبيه تظهر عند وجود مشاكل في قراءة التواريخ أو غيرها.

===============================
يمكنك تعديل هذا الملف لتوثيق أي تحديثات مستقبلية بسهولة.
===============================

 */



/*
 * لقد ابتعدت عن هذا السيناريو
 * 
 * 
 السيناريو المعتمد فى التقارير
1- التقرير الذى اريده من اي شاشة يتم تمريره الى هذه الشاشة الريسية فى استدعاء اى تقرير كان فى البرنامج 
    ويكون ذلك بتمرير للشاشة ثلاث متغيرات وهى string repCodeName, string repDisplayName, int currentReportId
2- عند فتح الشاشة يتم تحميل القيم الافتراضية من ملف تكست والتى يتم تغيريها عند علق الشاشة لتصبح افتراضية للمرة القادمة يحفظ فى 
    هذا الملف التكست هذه القيم دون التأثير على باقى القيم المختلفة التى احتاجها فى باقى شاشات البرنامج
3- يتم تحميل لستة بالفروع ليتم اختيار الفرع المراد عرض التقرير له
4- يتم تحميل لستة بالطابعات المثبتة فى الجهاز لاختيار المستخدم ايها سوف يطبع عليها فى حال الطباعة المباشرةويضاف اليها
    ثلاث اسطر 
            - سطر معاينة
            - سطر تصدير الى الاكسيل
            - سطر تصدير الى بى دي اف
5- بعد ضبط الفترة الزمنية والفرع وطريقة الطباعة يتم اختيار التقرير من خلال كلاس خاص 
    يخزن به جميع الدوال الخاصة بعرض التقارير المختلفة والتمرير اليه كل 
    المتغيرات التى اتخذت فى الشاشة فى قاموس به ويتم اختيار فقط من هذه المتغيرات الممرة ما يخص التقرير المراد من متغيرات خاصة به 
    حسب تصميم كل تقرير
            -اسم التقرير البرمجى الذى يبدأ ب rep_
                مع وجود دالة تضع اسم للداتا سيت باسم نفس التقرير مع استبدال المقدمة للاسم من rep_ الى ds_
            - الفترة الزمنية من الى 
            - طريقة الطباعة هل على طابعة مثبتة تم اختيارها ام معاينة ام تصدير لاكسيل ام الى بى دي اف
            - الفرع المراد بيانات التقرير عليه واذا كانت قيمته الممرة =0 فهذا يعنى كل الفروع مجتمعة
ويتم بعد ذلك ارسال الى اداة ريبورت فيور فى حالى المعاينة
او الطباعة المباشرة على الطابعة المختارة
او التصدير المباشر الى اكسيل
او الى بى دى اف

اريد ضبط هذا السيناريو فى الشاشة والكلاسات التى يمكن استخدامها والغاء كل ما سبق
 */

