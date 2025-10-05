using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses; // تأكد من المسار الصحيح لمساحة الأسماء لكلاس AppSettings

namespace MizanOriginalSoft.Views.Forms.MainForms
{
    public partial class frm_AppData : Form
    {
        public frm_AppData()
        {
            InitializeComponent();
        }

        private void frm_AppData_Load(object sender, EventArgs e)
        {
            try
            {
                // تحميل القيم من AppSettings فقط
                DisplaySettings();
                LoadWarehouses();
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء تحميل الإعدادات: " + ex.Message);
            }
        }

        private void LoadWarehouses()
        {
            // 1️⃣ تحميل البيانات من قاعدة البيانات
            DataTable dt = DBServiecs.Warehouse_GetAll();
            if (dt == null || dt.Rows.Count == 0) return;

            cbxWarehouseId.DataSource = dt;
            cbxWarehouseId.DisplayMember = "WarehouseName"; // عدّل حسب اسم العمود الفعلي
            cbxWarehouseId.ValueMember = "WarehouseId";

            // 🔒 منع الكتابة داخل الكمبوبوكس
            cbxWarehouseId.DropDownStyle = ComboBoxStyle.DropDownList;

            // 2️⃣ قراءة القيمة الافتراضية من ملف الإعداد
            int defaultId = AppSettings.GetInt("ThisVersionIsForWarehouseId", 0);
            cbxWarehouseId.SelectedValue = defaultId;
        }


        private void cbxWarehouseId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxWarehouseId.SelectedValue is int id)
            {
                AppSettings.SaveOrUpdate("ThisVersionIsForWarehouseId", id.ToString());
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            MessageBox.Show("تم حفظ إعدادات المخزن الافتراضي بنجاح.");
        }

        // 🔹 عرض الإعدادات على الأدوات في الشاشة
        private void DisplaySettings()
        {
            // 🏢 بيانات الشركة
            txtNameCo.Text = AppSettings.GetString("CompanyName", "");
            txtPhon.Text = AppSettings.GetString("CompanyPhon", "");
            txtAnthrPhon.Text = AppSettings.GetString("CompanyAnthrPhon", "");
            txtAdreass.Text = AppSettings.GetString("CompanyAdreass", "");
            txtCompanyEmail.Text = AppSettings.GetString("EmailCo", "");

            // 🖨️ إعدادات الطباعة
            lblRollPrinter.Text = AppSettings.GetString("RollPrinter", "");
            lblSheetPrinter.Text = AppSettings.GetString("SheetPrinter", "");
            txtSheetRows.Text = AppSettings.GetString("SheetRows", "6");
            txtSheetCols.Text = AppSettings.GetString("SheetCols", "10");
            txtMarginTop.Text = AppSettings.GetString("SheetMarginTop", "10");
            txtMarginBottom.Text = AppSettings.GetString("SheetMarginBottom", "10");
            txtMarginRight.Text = AppSettings.GetString("SheetMarginRight", "10");
            txtMarginLeft.Text = AppSettings.GetString("SheetMarginLeft", "10");
            txtRollLabelWidth.Text = AppSettings.GetString("RollLabelWidth", "50");
            txtRollLabelHeight.Text = AppSettings.GetString("RollLabelHeight", "25");

            // 💰 إعدادات الضريبة
            txtSalesTax.Text = AppSettings.GetString("SalesTax", "14");
            rdoAllowChangTax.Checked = AppSettings.GetBool("IsEnablToChangTax", false);
            rdoNotAllowChangTax.Checked = !rdoAllowChangTax.Checked;

            // 🛒 إعدادات البيع
            cbxReturnSaleMode.Text = AppSettings.GetString("ReturnSaleMode", "2");
            cbxReturnPurchasesMode.Text = AppSettings.GetString("ReturnPurchasesMode", "2");
            rdoAllowSaleByNegativeStock.Checked = AppSettings.GetBool("IsSaleByNegativeStock", false);
            rdoNotAllowSaleByNegativeStock.Checked = !rdoAllowSaleByNegativeStock.Checked;
        }

        // 🔹 حفظ التعديلات (بشكل مضبوط ومحدود)
        private void SaveData()
        {
            try
            {
                // 🏢 بيانات الشركة
                AppSettings.SaveOrUpdate("CompanyName", txtNameCo.Text);
                AppSettings.SaveOrUpdate("CompanyPhon", txtPhon.Text);
                AppSettings.SaveOrUpdate("CompanyAnthrPhon", txtAnthrPhon.Text);
                AppSettings.SaveOrUpdate("CompanyAdreass", txtAdreass.Text);
                AppSettings.SaveOrUpdate("EmailCo", txtCompanyEmail.Text);

                // 🖨️ إعدادات الطباعة
                AppSettings.SaveOrUpdate("RollPrinter", lblRollPrinter.Text);
                AppSettings.SaveOrUpdate("SheetPrinter", lblSheetPrinter.Text);
                AppSettings.SaveOrUpdate("SheetRows", txtSheetRows.Text);
                AppSettings.SaveOrUpdate("SheetCols", txtSheetCols.Text);
                AppSettings.SaveOrUpdate("SheetMarginTop", txtMarginTop.Text);
                AppSettings.SaveOrUpdate("SheetMarginBottom", txtMarginBottom.Text);
                AppSettings.SaveOrUpdate("SheetMarginRight", txtMarginRight.Text);
                AppSettings.SaveOrUpdate("SheetMarginLeft", txtMarginLeft.Text);
                AppSettings.SaveOrUpdate("RollLabelWidth", txtRollLabelWidth.Text);
                AppSettings.SaveOrUpdate("RollLabelHeight", txtRollLabelHeight.Text);

                // 💰 إعدادات الضريبة
                AppSettings.SaveOrUpdate("SalesTax", txtSalesTax.Text);
                AppSettings.SaveOrUpdate("IsEnablToChangTax", rdoAllowChangTax.Checked.ToString());

                // 🛒 إعدادات البيع
                AppSettings.SaveOrUpdate("ReturnSaleMode", cbxReturnSaleMode.Text);
                AppSettings.SaveOrUpdate("ReturnPurchasesMode", cbxReturnPurchasesMode.Text);
                AppSettings.SaveOrUpdate("IsSaleByNegativeStock", rdoAllowSaleByNegativeStock.Checked.ToString());

                MessageBox.Show("✅ تم حفظ الإعدادات بنجاح.", "حفظ الإعدادات", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("⚠️ حدث خطأ أثناء حفظ الإعدادات:\n" + ex.Message);
            }
        }

        // زر الحفظ في الواجهة
        private void btnSave_Click_(object sender, EventArgs e)
        {
            SaveData();
        }

        private void btnAddWarehouse_Click(object sender, EventArgs e)
        {
            AddWarehouse();
        }
        private void AddWarehouse()
        {
            string userInput;
            DialogResult inputResult = CustomMessageBox.ShowStringInputBox(out userInput,
                "من فضلك أدخل اسم الفرع:", "إضافة فرع");

            if (inputResult != DialogResult.OK || string.IsNullOrWhiteSpace(userInput))
            {
                MessageBox.Show("تم إلغاء الإضافة أو لم يتم إدخال اسم صالح.", "إلغاء",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int userId = CurrentSession.UserID;
            string message = DBServiecs.Warehouse_Add(userInput, userId);
            MessageBox.Show(message);
            LoadWarehouses(); // 🔄 تحديث القوائم بعد الإضافة.


        }


    }
}
