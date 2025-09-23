
using MizanOriginalSoft.MainClasses.OriginalClasses.ReportsClasses;
using MizanOriginalSoft.MainClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MizanOriginalSoft.MainClasses.OriginalClasses;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;

namespace MizanOriginalSoft.Views.Reports
{
    public partial class frmSettingReports : Form
    {

        /* 
        السيناريو المطلوب فى الشاشة
        1-تحديد الفرع المطلوب عرض بياناته 
        2-و تحديد الطابعة التى سيتم استخدامها فى الطباعة 
        3-وتحديد بداية ونهاية الفترة الزمنية
        4- وايضا استقبال البرامترات الخاصة بكل تقرير 
            a-اسمه البرمجى ReportCodeName
            b- والاسم الظاهر للمستخدم ReportDisplayName
            c- ورقمه المعرف ReportID
        ثم عرض البيانات المطلوبة فى جريد باسم DGV
        تصفيتها وفلترتها داخلها لكل تقرير مراد داخلها كمعاينة لما سيتم طباعته
        عن طريق ادوات الشاشة التى تحدد تاريخ البداية والنهاية فى اداتين التاريخ والوقت
        ويمكن تغييرها عن طريق 7 ريديو بوتن بشكل سلت 
        او بالكتابة فيها مباشرة التواريخ قبل الضغط على مفتاح الطباعة
        وبعد الانتهاء من عرض البيانات المطلوبة بالشكل المطلوب داخل الجريد يتم طباعتها
        عن طريق الضغط على مفتاح الطباعة btnPrint بعد استكمال البارمترات المطلوبه للتقرير المراد
        اى ان الهدف الاساسى تحديد البرامترات  اللازمة سواء الممرة لهده الشاشة او المحددة من خلالها قبل الطباعة الفعلية

        والنقطة الفنية الان المراد ضبطها ان يتم حفظ الفترة الزمنية الذى قام المستخدم بتحديدها
        وهى تاريخ البداية والنهاية والريديو بوتن الذى حدده فى هذه الجلسة 
        حتى اذا عاد الى الجلسة التالية وجدها اختيارات افتراضية فلا يعيد اختيارها فى كل مرة الا اذا اراد
        ولذلك تم اضافة مفاتيح فى ملف التكست الرئيسى بهذه المعاملات يتم حفظ فيها اخر اختيارات له فى الشاشة 
        ثم يقرأها فى الجلسة التالية وتحديدها كوضع افتراضى من خلال كلاس AppSettings
        الذى يقرأ كل مفاتيح البرنامج الاساسية عند فتح البرنامج مرة واحدة

        فالمراد فى هذه النقطة ان يتم حفظ اى متغيرات يقوم بها المستخدم فى هذه الشاشة بشكل صامت
        ثم يعيد قراة هذه المتغيرات فى نفس الكلاس مرة اخرى عند غلق الشاشة
         حتى اذا فتح الشاشة بتقرير اخر يجد اخر تحديدات تم العمل عليها محددة بشكل افتراضى

        والان بهذا الكود 


      */
        #region ==== المتغيرات ====

        // معلمات التقرير
        private readonly string _repCodeName;
        private readonly string _repDisplayName;
        private readonly int _reportId;

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
        private void rdo_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is not RadioButton rdo) return;
            if (!rdo.Checked) return;

            // 🔹 نحفظ اسم الراديو في الإعدادات
            AppSettings.SaveOrUpdate("DefaultRdoCheck", rdo.Name);

            // 🔹 نغير التاريخ حسب الاختيار
            if (rdo == rdoAllPeriod) SetPeriodForAll();
            else if (rdo == rdoToDay) SetPeriodForToday();
            else if (rdo == rdoPreviousDay) SetPeriodForPreviousDay();
            else if (rdo == rdoThisMonth) SetPeriodForCurrentMonth();
            else if (rdo == rdoPreviousMonth) SetPeriodForPreviousMonth();
            else if (rdo == rdoThisYear) SetPeriodForCurrentYear();
            else if (rdo == rdoPreviousYear) SetPeriodForPreviousYear();

            CalculateDaysBetweenDates();
        }

        private void LoadDefaults()
        {
            try
            {
                // ✅ الطابعة الافتراضية
                string? printer = AppSettings.GetString("DefaultPrinter");
                if (!string.IsNullOrEmpty(printer) && cbxPrinters.Items.Contains(printer))
                    cbxPrinters.SelectedItem = printer;

                // ✅ المستودع الافتراضي
                string? wh = AppSettings.GetString("DefaultWarehouseId");
                if (!string.IsNullOrEmpty(wh))
                    cbxWarehouse.SelectedValue = wh;

                // ✅ زر الراديو الافتراضي
                string? rdoName = AppSettings.GetString("DefaultRdoCheck", "rdoAllPeriod");
                RadioButton? rdo = this.Controls.Find(rdoName!, true).FirstOrDefault() as RadioButton;

                if (rdo != null)
                {
                    rdo.Checked = true;
                    rdo_CheckedChanged(rdo, EventArgs.Empty); // نستدعي الدالة العامة لتطبيق المنطق
                }
                else
                {
                    rdoAllPeriod.Checked = true;
                    rdo_CheckedChanged(rdoAllPeriod, EventArgs.Empty);
                }

                // ✅ تحميل التواريخ (لو لم تضبط بالراديو)
                if (dtpStart.Value == DateTime.MinValue)
                    dtpStart.Value = AppSettings.GetDateTime("StartAccountsDate", DateTime.Today);

                if (dtpEnd.Value == DateTime.MinValue)
                    dtpEnd.Value = AppSettings.GetDateTime("EndAccountsDate", DateTime.Today);

                CalculateDaysBetweenDates();
            }
            catch//System.FormatException: 'The input string 'System.Data.DataRowView' was not in a correct format.'
            {
                // fallback: لو حصل خطأ
                rdoAllPeriod.Checked = true;
                dtpStart.Value = DateTime.Today;
                dtpEnd.Value = DateTime.Today;
                CalculateDaysBetweenDates();
            }
        }

        // 🔹 إرجاع اسم الراديو المختار حالياً
        private string GetSelectedRadioButtonName()
        {
            return Controls.OfType<RadioButton>()
                           .FirstOrDefault(r => r.Checked)?.Name ?? "rdoAllPeriod";
        }

        // 🔹 الحفظ الصامت لكل الإعدادات
        private void SaveDataSilently()
        {
            AppSettings.SaveOrUpdate("DefaultPrinter", cbxPrinters.SelectedItem?.ToString() ?? "");
            AppSettings.SaveOrUpdate("DefaultWarehouseId", cbxWarehouse.SelectedValue?.ToString() ?? "");
            AppSettings.SaveOrUpdate("StartAccountsDate", dtpStart.Value.ToString("yyyy-MM-dd"));
            AppSettings.SaveOrUpdate("EndAccountsDate", dtpEnd.Value.ToString("yyyy-MM-dd"));
            AppSettings.SaveOrUpdate("DefaultRdoCheck", GetSelectedRadioButtonName());
        }

        #endregion


        #region ==== تحميل وحفظ الإعدادات مع الراديو والفترات ====

        private void SetupEventHandlers()
        {
            // التواريخ
            dtpStart.ValueChanged += dtpStart_ValueChanged;
            dtpEnd.ValueChanged += dtpEnd_ValueChanged;

            // جميع الراديوهات تمر على نفس الدالة العامة
            rdoAllPeriod.CheckedChanged += rdo_CheckedChanged;
            rdoToDay.CheckedChanged += rdo_CheckedChanged;
            rdoPreviousDay.CheckedChanged += rdo_CheckedChanged;
            rdoPreviousMonth.CheckedChanged += rdo_CheckedChanged;
            rdoThisMonth.CheckedChanged += rdo_CheckedChanged;
            rdoThisYear.CheckedChanged += rdo_CheckedChanged;
            rdoPreviousYear.CheckedChanged += rdo_CheckedChanged;

            // الكمبو بوكس
            cbxPrinters.SelectedIndexChanged += cbxPrinters_SelectedIndexChanged;
            cbxWarehouse.SelectedIndexChanged += cbxWarehouse_SelectedIndexChanged;
        }

        // 🔹 حفظ صامت للطابعة
        private void cbxPrinters_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cbxPrinters.SelectedItem != null)
                AppSettings.SaveOrUpdate("DefaultPrinter", cbxPrinters.SelectedItem?.ToString() ?? "");
        }

        // 🔹 حفظ صامت للمستودع
        private void cbxWarehouse_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cbxWarehouse.SelectedValue != null && cbxWarehouse.SelectedValue is int whId)
            {
                AppSettings.SaveOrUpdate("DefaultWarehouseId", whId.ToString());
            }
        }


        // 🔹 حفظ صامت للراديو + تحديد الفترة
        private void rdo_CheckedChanged_(object? sender, EventArgs e)
        {
            if (sender is not RadioButton rdo)
                return; // لو null أو مش RadioButton نخرج بأمان

            if (!rdo.Checked)
                return; // لازم يكون مفعّل فعلاً

            // ✅ حفظ اسم الراديو
            AppSettings.SaveOrUpdate("DefaultRdoCheck", rdo.Name);

            // ✅ تحديد الفترة
            if (rdo == rdoAllPeriod) SetPeriodForAll();
            else if (rdo == rdoToDay) SetPeriodForToday();
            else if (rdo == rdoPreviousDay) SetPeriodForPreviousDay();
            else if (rdo == rdoThisMonth) SetPeriodForCurrentMonth();
            else if (rdo == rdoPreviousMonth) SetPeriodForPreviousMonth();
            else if (rdo == rdoThisYear) SetPeriodForCurrentYear();
            else if (rdo == rdoPreviousYear) SetPeriodForPreviousYear();

            // ✅ تحديث الفرق بين التواريخ
            CalculateDaysBetweenDates();
        }


        // 🔹 عند تغيير التاريخ يدويًا
        private void dtpStart_ValueChanged(object? sender, EventArgs e)
        {
            AppSettings.SaveOrUpdate("StartAccountsDate", dtpStart.Value.ToString("yyyy-MM-dd"));
            CalculateDaysBetweenDates();
        }

        private void dtpEnd_ValueChanged(object? sender, EventArgs e)
        {
            AppSettings.SaveOrUpdate("EndAccountsDate", dtpEnd.Value.ToString("yyyy-MM-dd"));
            CalculateDaysBetweenDates();
        }

        // 🔹 حساب عدد الأيام
        private void CalculateDaysBetweenDates()
        {
            TimeSpan span = dtpEnd.Value.Date - dtpStart.Value.Date;
            lblAmountOfDay.Text = $"{span.Days + 1} يوم";
        }

        // ==== تواريخ الفترات ====
        private void SetPeriodForAll()
        {
            // ✅ البداية من المفتاح FixedGeneralStartDate (أو fallback لو مش موجود)
            dtpStart.Value = AppSettings.GetDateTime("FixedGeneralStartDate", DateTime.Today);

            // ✅ النهاية = آخر يوم في السنة الحالية
            int currentYear = DateTime.Today.Year;
            dtpEnd.Value = new DateTime(currentYear, 12, 31);

            lblAllPeriod.Text = "كل الفترة";
        }

        private void SetPeriodForAll_()
        {
            /*
             اريد عند اختيار كل الفترة ان يقرأ البداية من مفتاح FixedGeneralStartDate كبداية 
            اما نهاية تكون تاريخ اخر يوم فى السنة الحالية 31-12-السنة الحالية
             */
            dtpStart.Value = AppSettings.GetDateTime("StartAccountsDate", DateTime.Today);
            dtpEnd.Value = AppSettings.GetDateTime("EndAccountsDate", DateTime.Today);
            lblAllPeriod.Text = "كل الفترة";
        }

        private void SetPeriodForToday()
        {
            dtpStart.Value = DateTime.Today;
            dtpEnd.Value = DateTime.Today;
            lblAllPeriod.Text = "";
        }

        private void SetPeriodForPreviousDay()
        {
            DateTime yesterday = DateTime.Today.AddDays(-1);
            dtpStart.Value = yesterday;
            dtpEnd.Value = yesterday;
            lblAllPeriod.Text = "";
        }

        private void SetPeriodForCurrentMonth()
        {
            DateTime today = DateTime.Today;
            dtpStart.Value = new DateTime(today.Year, today.Month, 1);
            dtpEnd.Value = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            lblAllPeriod.Text = "";
        }

        private void SetPeriodForPreviousMonth()
        {
            DateTime firstDayOfLastMonth = DateTime.Today.AddMonths(-1);
            firstDayOfLastMonth = new DateTime(firstDayOfLastMonth.Year, firstDayOfLastMonth.Month, 1);

            dtpStart.Value = firstDayOfLastMonth;
            dtpEnd.Value = new DateTime(firstDayOfLastMonth.Year, firstDayOfLastMonth.Month,
                                       DateTime.DaysInMonth(firstDayOfLastMonth.Year, firstDayOfLastMonth.Month));
            lblAllPeriod.Text = "";
        }

        private void SetPeriodForCurrentYear()
        {
            dtpStart.Value = new DateTime(DateTime.Now.Year, 1, 1);
            dtpEnd.Value = new DateTime(DateTime.Now.Year, 12, 31);
            lblAllPeriod.Text = "";
        }

        private void SetPeriodForPreviousYear()
        {
            int lastYear = DateTime.Now.Year - 1;
            dtpStart.Value = new DateTime(lastYear, 1, 1);
            dtpEnd.Value = new DateTime(lastYear, 12, 31);
            lblAllPeriod.Text = "";
        }

        #endregion

        /*ما الذى ينقص فالهدف هو 
         عندما يتم اختيار فترة زمنية عن طريق الريديو بوتن فيكون فى لحظة الخيار يحفظ ما تم اختياره فى ملف الاعداد عن طريق كلاس اب سيتينج 
        وفى المرة القادمة لفتح الشاشة يجد المستخدم القيم الاخيرة التى كان عليها الاختيار فيكمل فى عمله على اساسها 
        اما الان عند الاختيار لا يتم الحفظ الصامت 
         */


        private void rdoToDay_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoThisMonth_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoThisYear_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoPreviousDay_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoPreviousMonth_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoPreviousYear_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoAllPeriod_CheckedChanged(object sender, EventArgs e)
        {

        }



        #region ==== دوال تحميل الطابعات والمستودعات  ====

        // تحميل أسماء الطابعات المثبتة وإضافة خيارات المعاينة والتصدير.
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

        // تحميل قائمة المستودعات من قاعدة البيانات مع إضافة خيار "كل الفروع".
        private void LoadWarehouses()
        {
            try
            {
                DataTable dt = DBServiecs.Warehouse_GetAll();

                if (dt != null && dt.Rows.Count > 0)
                {
                    // ➕ إضافة خيار "كل الفروع"
                    DataRow allBranchesRow = dt.NewRow();
                    allBranchesRow["WarehouseId"] = 0;
                    allBranchesRow["WarehouseName"] = "كل الفروع";
                    dt.Rows.InsertAt(allBranchesRow, 0);

                    cbxWarehouse.DataSource = dt;
                    cbxWarehouse.DisplayMember = "WarehouseName";
                    cbxWarehouse.ValueMember = "WarehouseId";
                    cbxWarehouse.DropDownStyle = ComboBoxStyle.DropDownList;
                    cbxWarehouse.Enabled = true;

                    // 📌 جلب آخر فرع محفوظ
                    int savedWhId = AppSettings.GetInt("DefaultWarehouseId", 0);

                    if (savedWhId > 0 && dt.AsEnumerable().Any(r => r.Field<int>("WarehouseId") == savedWhId))
                    {
                        cbxWarehouse.SelectedValue = savedWhId;
                    }
                    else
                    {
                        cbxWarehouse.SelectedIndex = 0; // الافتراضي "كل الفروع"
                    }
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

        // حفظ الإعدادات وإغلاق النموذج عند الضغط على زر الحفظ والإغلاق.
        private void btnSaveAndClose_Click(object sender, EventArgs e)
        {
            //SaveDataSilently();
            Close();
        }

        #endregion

        #region ==== ضبط أحداث واجهة المستخدم ====



        private void btnPrint_Click(object sender, EventArgs e)
        {
            // ReportsManager.ShowReport(parameters);
        }

        #endregion

        private void btnAllPeriod_Click(object sender, EventArgs e)
        {
            rdoAllPeriod .Checked = true;
        }
    }
}




