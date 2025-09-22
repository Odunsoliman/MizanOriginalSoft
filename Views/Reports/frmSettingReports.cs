
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

                    // 📌 نستدعي الدالة العامة لتطبيق نفس منطق الراديو (تحديد التواريخ + الحفظ)
                    rdo_CheckedChanged(rdo, EventArgs.Empty);
                }
                else
                {
                    // fallback في حالة ما اتلاقاش الراديو (نحط الافتراضي)
                    rdoAllPeriod.Checked = true;
                    rdo_CheckedChanged(rdoAllPeriod, EventArgs.Empty);
                }

                // ✅ تحميل التواريخ إذا لم يتم ضبطها من الراديو
                if (dtpStart.Value == DateTime.MinValue)
                    dtpStart.Value = AppSettings.GetDateTime("StartAccountsDate", DateTime.Today);

                if (dtpEnd.Value == DateTime.MinValue)
                    dtpEnd.Value = AppSettings.GetDateTime("EndAccountsDate", DateTime.Today);

                // ✅ حساب عدد الأيام
                CalculateDaysBetweenDates();
            }
            catch
            {
                // في حالة أي خطأ: نبدأ بالقيم الافتراضية
                rdoAllPeriod.Checked = true;
                dtpStart.Value = DateTime.Today;
                dtpEnd.Value = DateTime.Today;
                CalculateDaysBetweenDates();
            }
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
        // حفظ صامت للراديو + ضبط التواريخ
        private void rdo_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rdo = (RadioButton)sender;
            if (!rdo.Checked) return; // نتأكد أن الراديو تم تفعيله فعلاً

            // ✅ حفظ اسم الراديو في ملف الإعدادات
            AppSettings.SaveOrUpdate("DefaultRdoCheck", rdo.Name);

            // ✅ تحديد الفترة الزمنية بناءً على الراديو المختار
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

        // حفظ الإعدادات وإغلاق النموذج عند الضغط على زر الحفظ والإغلاق.
        private void btnSaveAndClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region ==== ضبط أحداث واجهة المستخدم ====



        private void btnPrint_Click(object sender, EventArgs e)
        {
            // ReportsManager.ShowReport(parameters);
        }


        #endregion

    }
}




