
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

namespace MizanOriginalSoft.Views.Reports
{
    public partial class frmSettingReports : Form
    {
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
        private void LoadDefaults()
        {
            try
            {
                // الطابعة الافتراضية
                string? printer = AppSettings.GetString("DefaultPrinter");
                if (!string.IsNullOrEmpty(printer) && cbxPrinters.Items.Contains(printer))
                    cbxPrinters.SelectedItem = printer;

                // المستودع الافتراضي
                string? wh = AppSettings.GetString("DefaultWarehouseId");
                if (!string.IsNullOrEmpty(wh))
                    cbxWarehouse.SelectedValue = wh;

                // الراديو الافتراضي
                string? rdoName = AppSettings.GetString("DefaultRdoCheck");
                RadioButton? rdo = this.Controls.Find(rdoName!, true).FirstOrDefault() as RadioButton;

                if (rdo != null)
                    rdo.Checked = true;

                // التواريخ الافتراضية
                dtpStart.Value = AppSettings.GetDateTime("StartAccountsDate", DateTime.Today);
                dtpEnd.Value = AppSettings.GetDateTime("EndAccountsDate", DateTime.Today);

                CalculateDaysBetweenDates();
            }
            catch { }
        }

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

        // الحصول على اسم زر الراديو المختار حالياً.
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

        private void SaveDataSilently()
        {
            // ✅ الحفظ بصمت عند التغيير فقط
            AppSettings.SaveOrUpdate("DefaultPrinter", cbxPrinters.SelectedItem?.ToString() ?? "");
            AppSettings.SaveOrUpdate("DefaultWarehouseId", cbxWarehouse.SelectedValue?.ToString() ?? "");
            AppSettings.SaveOrUpdate("DefaultStartDate", dtpStart.Value.ToString("yyyy-MM-dd"));
            AppSettings.SaveOrUpdate("DefaultEndDate", dtpEnd.Value.ToString("yyyy-MM-dd"));
            AppSettings.SaveOrUpdate("DefaultRdoCheck", GetSelectedRadioButtonName());
        }


        #endregion

        #region ==== تحميل وحفظ الإعدادات مع الراديو والفترات ====
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

        // حفظ صامت للطابعة
        private void cbxPrinters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxPrinters.SelectedItem != null)
            AppSettings.SaveOrUpdate("DefaultPrinter", cbxPrinters.SelectedItem?.ToString() ?? "");

        }

        // حفظ صامت للمستودع
        private void cbxWarehouse_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxWarehouse.SelectedValue != null)
                AppSettings.SaveOrUpdate("DefaultWarehouseId", cbxWarehouse.SelectedValue?.ToString() ?? "");
        }

        // حفظ صامت للراديو + ضبط التواريخ
        private void rdo_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rdo = (RadioButton)sender;
            if (!rdo.Checked) return;

            AppSettings.SaveOrUpdate("DefaultRdoCheck", rdo.Name);

            if (rdo == rdoAllPeriod) SetPeriodForAll();
            else if (rdo == rdoToDay) SetPeriodForToday();
            else if (rdo == rdoPreviousDay) SetPeriodForPreviousDay();
            else if (rdo == rdoThisMonth) SetPeriodForCurrentMonth();
            else if (rdo == rdoPreviousMonth) SetPeriodForPreviousMonth();
            else if (rdo == rdoThisYear) SetPeriodForCurrentYear();
            else if (rdo == rdoPreviousYear) SetPeriodForPreviousYear();

            CalculateDaysBetweenDates();
        }

        // عند تغيير التاريخ يدويًا
        private void dtpStart_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.SaveOrUpdate("StartAccountsDate", dtpStart.Value.ToString("yyyy-MM-dd"));
            CalculateDaysBetweenDates();
        }

        private void dtpEnd_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.SaveOrUpdate("EndAccountsDate", dtpEnd.Value.ToString("yyyy-MM-dd"));
            CalculateDaysBetweenDates();
        }

        private void CalculateDaysBetweenDates()
        {
            TimeSpan span = dtpEnd.Value.Date - dtpStart.Value.Date;
            lblAmountOfDay.Text = $"{span.Days + 1} يوم";
        }

        private void SetPeriodForAll()
        {
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

        #endregion

        #region ==== دوال أزرار التقرير ====

        // تجهيز معلمات التقرير وطلب عرضه عند الضغط على زر "انتقال".
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

        // حفظ الإعدادات وإغلاق النموذج عند الضغط على زر الحفظ والإغلاق.
        private void btnSaveAndClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region ==== ضبط أحداث واجهة المستخدم ====


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

