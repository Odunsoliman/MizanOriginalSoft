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

        #region !!!!!!!!!!! DisplaySettings  !!!!!!!!!!!!!!!
        private void DisplaySettings()
        {
            // 🏢 بيانات الشركة
            txtNameCo.Text = AppSettings.GetString("CompanyName", "");
            txtPhon.Text = AppSettings.GetString("CompanyPhon", "");
            txtAnthrPhon.Text = AppSettings.GetString("CompanyAnthrPhon", "");
            txtAdreass.Text = AppSettings.GetString("CompanyAdreass", "");
            txtCompanyEmail.Text = AppSettings.GetString("EmailCo", "");

            // 🖼️ شعار الشركة
            string? logoFileName = AppSettings.GetString("LogoImagName", null);
            string? logoFolder = AppSettings.GetString("CompanyLoGoFolder", null);

            string defaultLogoFileName = AppSettings.GetString("DefaulLogoImagName", "Mizan Logo.PNG") ?? "Mizan Logo.PNG";
            string defaultLogoFolder = AppSettings.GetString("DefaulCompanyLoGoFolder", Path.Combine(Application.StartupPath, "HelpFiles"))
                                       ?? Path.Combine(Application.StartupPath, "HelpFiles");

            //            lblLogoImageName.Text = logoFileName ?? defaultLogoFileName;
            lblLogoImageName.Text = Path.GetFileNameWithoutExtension(logoFileName ?? defaultLogoFileName);

            lblLogoPath.Text = logoFolder ?? defaultLogoFolder;

            string logoFullPath;
            if (!string.IsNullOrWhiteSpace(logoFolder) && !string.IsNullOrWhiteSpace(logoFileName))
            {
                logoFullPath = Path.Combine(logoFolder, logoFileName);
                if (!File.Exists(logoFullPath))
                {
                    logoFullPath = Path.Combine(defaultLogoFolder, defaultLogoFileName);
                }
            }
            else
            {
                logoFullPath = Path.Combine(defaultLogoFolder, defaultLogoFileName);
            }

            // تحميل الصورة في PictureBox
            if (File.Exists(logoFullPath))
            {
                if (picLogoCo.Image != null)
                {
                    picLogoCo.Image.Dispose();
                    picLogoCo.Image = null;
                }
                picLogoCo.Image = Image.FromFile(logoFullPath);
                picLogoCo.SizeMode = PictureBoxSizeMode.Zoom; // حفظ النسبة عند تكبير/تصغير
            }
            else
            {
                picLogoCo.Image = null;
            }

            // ✅ عرض أبعاد الـ PictureBox (وليس الصورة) في lblImagSize
            lblImagSize.Text = $"المقاس الطلوب لصورة اللوجو:    عرض: {picLogoCo.Width}px × ارتفاع: {picLogoCo.Height}px او ما يوازيه حتى لا تفقد دقة الصورة";

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

        // 🔹 عرض الإعدادات على الأدوات في الشاشة
        private void DisplaySettings_()
        {
            // 🏢 بيانات الشركة
            txtNameCo.Text = AppSettings.GetString("CompanyName", "");
            txtPhon.Text = AppSettings.GetString("CompanyPhon", "");
            txtAnthrPhon.Text = AppSettings.GetString("CompanyAnthrPhon", "");
            txtAdreass.Text = AppSettings.GetString("CompanyAdreass", "");
            txtCompanyEmail.Text = AppSettings.GetString("EmailCo", "");

            // 🖼️ شعار الشركة
            string? logoFileName = AppSettings.GetString("LogoImagName", null);
            string? logoFolder = AppSettings.GetString("CompanyLoGoFolder", null);

            // افتراضي إذا لم يتم تحديد أي صورة
            string defaultLogoFileName = AppSettings.GetString("DefaulLogoImagName", "Mizan Logo.PNG") ?? "Mizan Logo.PNG";
            string defaultLogoFolder = AppSettings.GetString("DefaulCompanyLoGoFolder", Path.Combine(Application.StartupPath, "HelpFiles"))
                                       ?? Path.Combine(Application.StartupPath, "HelpFiles");

            lblLogoImageName.Text = logoFileName ?? defaultLogoFileName;
            lblLogoPath.Text = logoFolder ?? defaultLogoFolder;

            // تحديد المسار النهائي للصورة
            string logoFullPath;

            // إذا كانت الصورة المخصصة موجودة
            if (!string.IsNullOrWhiteSpace(logoFolder) && !string.IsNullOrWhiteSpace(logoFileName))
            {
                logoFullPath = Path.Combine(logoFolder, logoFileName);
                if (!File.Exists(logoFullPath))
                {
                    // لم توجد الصورة المخصصة، استخدم الافتراضية
                    logoFullPath = Path.Combine(defaultLogoFolder, defaultLogoFileName);
                }
            }
            else
            {
                // استخدم الصورة الافتراضية مباشرة
                logoFullPath = Path.Combine(defaultLogoFolder, defaultLogoFileName);
            }

            // تحميل الصورة
            if (File.Exists(logoFullPath))
            {
                if (picLogoCo.Image != null)
                {
                    picLogoCo.Image.Dispose();
                    picLogoCo.Image = null;
                }
                picLogoCo.Image = Image.FromFile(logoFullPath);
            }
            else
            {
                picLogoCo.Image = null; // أو ضع صورة افتراضية مدمجة بالبرنامج
            }

            // بعد تحميل الصورة في PictureBox
            if (File.Exists(logoFullPath))
            {
                if (picLogoCo.Image != null)
                {
                    picLogoCo.Image.Dispose();
                    picLogoCo.Image = null;
                }

                Image img = Image.FromFile(logoFullPath);
                picLogoCo.Image = img;

                // عرض أبعاد الصورة بالبكسل
                lblImagSize.Text = $"W:{img.Width} × H:{img.Height} px";
            }
            else
            {
                picLogoCo.Image = null;
                lblImagSize.Text = "لا توجد صورة";
            }

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

        #endregion

        #region !!!!!!!!!!!! Warehouse  !!!!!!!!!!!!!
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

        //اضافة فرع او مخزن الى قاعدة البيانات
        private void btnAddWarehouse_Click(object sender, EventArgs e)
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

        // 🗑️ حذف الفرع المحدد.
        private void btnDeleteWarehous_Click(object sender, EventArgs e)
        {
            if (cbxWarehouseId.SelectedValue == null)
            {
                MessageBox.Show("❌ يرجى اختيار الفرع المراد حذفه.");
                return;
            }

            int warehouseId = Convert.ToInt32(cbxWarehouseId.SelectedValue);
            int userId = CurrentSession.UserID;

            // ⚠️ تأكيد الحذف من المستخدم.
            DialogResult confirm = MessageBox.Show("هل أنت متأكد من حذف الفرع المحدد؟",
                "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            string message = DBServiecs.Warehouse_Delete(warehouseId, userId);
            MessageBox.Show(message);
            LoadWarehouses(); // 🔄 تحديث القوائم بعد الحذف.
        }

        // ✏️ تعديل اسم الفرع المحدد.
        private void btnRenamWarehous_Click(object sender, EventArgs e)
        {
            if (cbxWarehouseId.SelectedValue == null)
            {
                MessageBox.Show("❌ يرجى اختيار الفرع المراد تعديله.");
                return;
            }

            string userInput;
            DialogResult inputResult = CustomMessageBox.ShowStringInputBox(out userInput,
                "من فضلك أدخل اسم الفرع الجديد:", "تعديل ");

            if (inputResult != DialogResult.OK || string.IsNullOrWhiteSpace(userInput))
            {
                MessageBox.Show("تم إلغاء التعديل أو لم يتم إدخال اسم صالح.", "إلغاء",
                               MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }



            int warehouseId = Convert.ToInt32(cbxWarehouseId.SelectedValue);
            int userId = CurrentSession.UserID;

            string message = DBServiecs.Warehouse_UpdateName(warehouseId, userInput, userId);
            MessageBox.Show(message);
            LoadWarehouses(); // 🔄 إعادة التحميل بعد التعديل.
        }

        #endregion

        #region !!!!!!!!!!!!!  ادوات الحفظ !!!!!!!!!!!!!!

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
                AppSettings.SaveOrUpdate("CompanyLoGoFolder", lblLogoPath.Text);
                AppSettings.SaveOrUpdate("LogoImagName", lblLogoImageName.Text);

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
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }


        #endregion

        #region === تغيير اللوجو ===
        private void btnChangLogo_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                openFileDialog.Title = "اختر صورة اللوجو";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    lblLogoPath.Text = Path.GetDirectoryName(openFileDialog.FileName);
                    lblLogoImageName.Text = Path.GetFileName(openFileDialog.FileName);

                    try
                    {
                        picLogoCo.Image = Image.FromFile(openFileDialog.FileName);
                        picLogoCo.SizeMode = PictureBoxSizeMode.Zoom;
                        SaveData();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("حدث خطأ أثناء تحميل الصورة: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        lblLogoPath.Text = "";
                        lblLogoImageName.Text = "";
                    }
                }
            }
        }



        private void btnDeleteLogo_Click(object sender, EventArgs e)
        {
            // إعادة ضبط اللوجو على الافتراضي
            string defaultLogoFileName = AppSettings.GetString("DefaulLogoImagName", "Mizan Logo.PNG") ?? "Mizan Logo.PNG";
            string defaultLogoFolder = AppSettings.GetString("DefaulCompanyLoGoFolder", Path.Combine(Application.StartupPath, "HelpFiles"))
                                       ?? Path.Combine(Application.StartupPath, "HelpFiles");

            // تحديث الكلاس والملف
            AppSettings.SaveOrUpdate("LogoImagName", "");
            AppSettings.SaveOrUpdate("CompanyLoGoFolder", "");

            // تحديث العرض على الشاشة
            lblLogoImageName.Text = "";
            lblLogoPath.Text = "";

            string logoFullPath = Path.Combine(defaultLogoFolder, defaultLogoFileName);

            if (File.Exists(logoFullPath))
            {
                if (picLogoCo.Image != null)
                {
                    picLogoCo.Image.Dispose();
                    picLogoCo.Image = null;
                }

                picLogoCo.Image = Image.FromFile(logoFullPath);
                picLogoCo.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            else
            {
                picLogoCo.Image = null;
            }

            MessageBox.Show("✅ تم حذف اللوجو المخصص وإرجاع الصورة الافتراضية.", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region === إعدادات A4 Sheet: الحواف واللاصقات ===

        // حساب عدد اللاصقات المعروضة في الورقة.
        private void UpdateLabelCount()
        {
            if (int.TryParse(txtSheetRows.Text, out int rows) &&
                int.TryParse(txtSheetCols.Text, out int cols))
            {
                lblCountLables.Text = $"عدد اللاصقات : {rows * cols}";
            }
            else
            {
                lblCountLables.Text = "اكتب رقم صحيح";
            }
        }

        /// <summary>
        /// تطبيق الحواف حسب القيم المدخلة.
        /// </summary>
        private void tlpPading()
        {
            try
            {
                int top = string.IsNullOrEmpty(txtMarginTop.Text) ? 0 : int.Parse(txtMarginTop.Text);
                int bottom = string.IsNullOrEmpty(txtMarginBottom.Text) ? 0 : int.Parse(txtMarginBottom.Text);
                int left = string.IsNullOrEmpty(txtMarginLeft.Text) ? 0 : int.Parse(txtMarginLeft.Text);
                int right = string.IsNullOrEmpty(txtMarginRight.Text) ? 0 : int.Parse(txtMarginRight.Text);

                tlpPeper.Padding = new Padding(left, top, right, bottom);
            }
            catch (FormatException)
            {
                MessageBox.Show("الرجاء إدخال أرقام صحيحة فقط", "خطأ في الإدخال", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            pnlMargen.BorderStyle = BorderStyle.None;

            pnlMargen.Paint += (sender, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, pnlMargen.ClientRectangle,
                    Color.Blue, 1, ButtonBorderStyle.Solid,        // أعلى
                    Color.GreenYellow, 2, ButtonBorderStyle.Solid, // يمين
                    Color.Blue, 1, ButtonBorderStyle.Solid,        // أسفل
                    Color.GreenYellow, 2, ButtonBorderStyle.Solid  // يسار
                );
            };
        }

        // إعادة الحساب عند تغيير القيم
        private void txtSheetRows_TextChanged(object sender, EventArgs e) => UpdateLabelCount();
        private void txtSheetCols_TextChanged(object sender, EventArgs e) => UpdateLabelCount();
        private void txtMarginTop_TextChanged(object sender, EventArgs e) => tlpPading();

        #endregion

        #region === إعدادات الطابعات ===
        private void btnLoadRollPrinter_Click(object sender, EventArgs e)
        {
            using (PrintDialog printDialog = new PrintDialog())
            {
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    lblRollPrinter.Text = printDialog.PrinterSettings.PrinterName;
                    SaveData();
                }
            }
        }
        private void btnLoadSheetPrinter_Click(object sender, EventArgs e)
        {
            using (PrintDialog printDialog = new PrintDialog())
            {
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    lblSheetPrinter.Text = printDialog.PrinterSettings.PrinterName;
                    SaveData();
                }
            }
        }

        #endregion

        private void txtMarginRight_TextChanged(object sender, EventArgs e)
        {

        }

        #region === احتياطي: KeyDown لربطه بالتنقل لاحقًا ===

        private void txtRollLabelWidth_KeyDown(object sender, KeyEventArgs e) { }
        private void txtRollLabelHeight_KeyDown(object sender, KeyEventArgs e) { }
        private void txtSheetRows_KeyDown(object sender, KeyEventArgs e) { }
        private void txtSheetCols_KeyDown(object sender, KeyEventArgs e) { }
        private void txtMarginTop_KeyDown(object sender, KeyEventArgs e) { }
        private void txtMarginBottom_KeyDown(object sender, KeyEventArgs e) { }
        private void txtMarginRight_KeyDown(object sender, KeyEventArgs e) { }
        private void txtMarginLeft_KeyDown(object sender, KeyEventArgs e) { }

        #endregion

 


    }
}
