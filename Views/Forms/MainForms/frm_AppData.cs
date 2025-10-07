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
                tabMang.ItemSize = new Size(150, 40);
                FillcbxReturnSaleMode();
                FillcbxReturnPurchasesMode();
                tlpPading();
                UpdateLabelCount();
                TextBoxesInTabs();
                txtNameCo.Focus();
                txtNameCo.SelectAll();
                LoadBackupFiles();
                ApplyPermissionsToControls();
                LoadAllUsers();
                DGV_Users.SelectionChanged += DGV_Users_SelectionChanged;
                DGV_Users.RowPrePaint += DGV_Users_RowPrePaint;

                LoadUsers();
                cbxUsers.SelectedIndexChanged += CbxUsers_SelectedIndexChanged;
                DGVStyl();
                // تحميل القيم من AppSettings فقط
                DisplaySettings();
                LoadWarehouses();//خاص باعداد الفرع لهذه النسخة
                fillWarehouses();//خاص بالصلاحيات

            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء تحميل الإعدادات: " + ex.Message);
            }
        }

        #region !!!!!!!!!!! DisplaySettings  !!!!!!!!!!!!!!!

        // 🔹 عرض الإعدادات على الأدوات في الشاشة
        private void DisplaySettings()
        {
            //بيانات السيرفر
            txtServerName.Text = AppSettings.GetString("serverName", "");
            txtDBName.Text = AppSettings.GetString("DBName", "");
            txtBackupsPath.Text = AppSettings.GetString("BackupsPath", "");
            txtMaxBackups.Text = AppSettings.GetString("maxBackups", "");

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

            // 💰 السماح او عدمه للاتحكم فى نسبة الخصم
            txtSalesPercentage.Text = AppSettings.GetString("SalesPercentage", "14");
            txtMaxRateDiscount.Text = AppSettings.GetString("MaxRateDiscount", "14");
            rdoOpendMaxRateDiscount.Checked = AppSettings.GetBool("IsOpendMaxRateDiscount", false);
            rdoClosedMaxRateDiscount.Checked = !rdoOpendMaxRateDiscount.Checked;

        }


        #endregion

        #region !!!!!!!!!!!! Warehouse  !!!!!!!!!!!!!
        // 📋 تحميل قائمة الفروع في الكومبوبوكس.
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
        // 🧾 
        private void cbxWarehouseId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxWarehouseId.SelectedValue is int id)
            {
                AppSettings.SaveOrUpdate("ThisVersionIsForWarehouseId", id.ToString());
            }
        }

        // ➕ إضافة فرع جديد.
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

                // 💰 حفظ السماح او عدمه للتحكم فى نسبة الخصم
                AppSettings.SaveOrUpdate("SalesPercentage", txtSalesPercentage.Text);
                AppSettings.SaveOrUpdate("MaxRateDiscount", txtMaxRateDiscount .Text);
                AppSettings.SaveOrUpdate("IsOpendMaxRateDiscount", rdoOpendMaxRateDiscount.Checked.ToString());

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

        #region === تبويب الطابعات ===

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

        // تطبيق الحواف حسب القيم المدخلة.
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

        #region === تبويب السيرفر ===
        #region ✅  النسخ الاحتياطية

        // تحميل ملفات النسخ الاحتياطية من المسار المحدد في الإعدادات.
        private void LoadBackupFiles()
        {
            try
            {
                string settingsPath = Path.Combine(Application.StartupPath, "serverConnectionSettings.txt");

                if (!File.Exists(settingsPath))
                {
                    MessageBox.Show("❌ ملف الإعدادات غير موجود في: " + settingsPath);
                    return;
                }

                // تحميل الإعدادات لمرة واحدة
                AppSettings.Load(settingsPath);

                string? backupPath = AppSettings.GetString("BackupsPath", null);

                if (string.IsNullOrWhiteSpace(backupPath))
                {
                    MessageBox.Show("❌ مسار النسخ الاحتياطي غير محدد في الإعدادات");
                    return;
                }

                if (!Directory.Exists(backupPath))
                {
                    MessageBox.Show("❌ المجلد المحدد للنسخ الاحتياطية غير موجود: " + backupPath);
                    return;
                }

                var files = Directory.GetFiles(backupPath, "*.bak")
                                     .Select(f => new FileInfo(f))
                                     .OrderByDescending(f => f.CreationTime)
                                     .Select(f => new
                                     {
                                         FullName = f.FullName,
                                         DisplayName = $"نسخة بتاريخ {f.CreationTime:dd/MM/yyyy} الساعة {f.CreationTime:HH:mm:ss}"
                                     })
                                     .ToList();

                comboBoxBackups.DisplayMember = "DisplayName";
                comboBoxBackups.ValueMember = "FullName";
                comboBoxBackups.DataSource = files;

                if (files.Count == 0)
                    MessageBox.Show("❌ لا توجد نسخ احتياطية في المجلد المحدد");
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء تحميل النسخ الاحتياطية:\n" + ex.Message);
            }
        }

        // اختيار مجلد النسخ الاحتياطية من المستخدم.
        private void btnGetFolderBak_Click(object sender, EventArgs e)
        {
            try
            {
                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "اختر مجلد النسخ الاحتياطية";
                    folderDialog.ShowNewFolderButton = true;

                    if (!string.IsNullOrEmpty(txtBackupsPath.Text))
                    {
                        folderDialog.SelectedPath = txtBackupsPath.Text;
                    }

                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        txtBackupsPath.Text = folderDialog.SelectedPath;
                        SaveData(); // حفظ المسار مباشرة
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء اختيار المجلد: " + ex.Message,
                    "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // تنفيذ عملية استرجاع نسخة احتياطية مع إنشاء نسخة احتياطية مؤقتة أولاً.
        private async void btnRestoreBackup_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBoxBackups.SelectedValue == null)
                {
                    MessageBox.Show("❌ يرجى اختيار النسخة الاحتياطية التي ترغب في استرجاعها.");
                    return;
                }

                string settingsPath = Path.Combine(Application.StartupPath, "serverConnectionSettings.txt");

                AppSettings.Load(settingsPath); // تحميل الإعدادات

                var helper = new DatabaseBackupRestoreHelper(settingsPath);

                string? dbName = AppSettings.GetString("DBName", null);
                if (string.IsNullOrWhiteSpace(dbName))
                {
                    MessageBox.Show("❌ لم يتم العثور على اسم قاعدة البيانات في الإعدادات.");
                    return;
                }

                string? selectedBackupFile = comboBoxBackups.SelectedValue.ToString();
                if (string.IsNullOrWhiteSpace(selectedBackupFile))
                {
                    MessageBox.Show("❌ لم يتم تحديد مسار النسخة الاحتياطية بشكل صحيح.");
                    return;
                }

                var confirmResult = MessageBox.Show(
                    "⚠️ هل أنت متأكد من أنك تريد استرجاع هذه النسخة؟ سيتم عمل نسخة احتياطية أولًا.",
                    "تأكيد الاسترجاع", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmResult == DialogResult.No)
                    return;

                // 🟢 الخطوة 1: عمل نسخة احتياطية
                helper.BackupDatabase();
                MessageBox.Show("✅ تم إنشاء نسخة احتياطية من الوضع الحالي بنجاح.");

                // 🟢 الخطوة 2: نافذة تحميل مؤقتة
                frmLoading loadingForm = new frmLoading("جارٍ استرجاع النسخة الاحتياطية، الرجاء الانتظار...");
                loadingForm.Show();
                loadingForm.Refresh();

                // 🟢 الخطوة 3: استرجاع النسخة المحددة
                await Task.Run(() => helper.RestoreDatabase(selectedBackupFile));

                loadingForm.Close();

                MessageBox.Show("✅ تم استرجاع النسخة بنجاح. يُفضل إعادة تشغيل البرنامج.");
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ حدث خطأ أثناء استرجاع النسخة الاحتياطية:\n" + ex.Message,
                    "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // عند مغادرة الحقل يتم حفظ القيمة تلقائيًا.
        private void txtMaxBackups_Leave(object sender, EventArgs e)
        {
            SaveData();
        }
        #endregion


        #endregion

        #region === تبويب المستخدمين ===
        // ➕ إضافة .
        // 🗑️ حذف .
        // ✏️ تعديل .
        // 📋 تحميل.
        // ⭐ تعيين .
        // 🧾 تحديث .

        // ⭐ تعيين تنسيق جريد المستخدمين.
        private void StyleDGV_Users()
        {/*اريد المستخدم الغير مفعل IsActive=0 يظهر بلون بخلفية مميز*/
            DGV_Users.RowHeadersVisible = false;
            DGV_Users.ReadOnly = true;
            DGV_Users.AllowUserToAddRows = false;
            DGV_Users.AllowUserToDeleteRows = false;
            DGV_Users.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            foreach (DataGridViewColumn col in DGV_Users.Columns)
                col.Visible = false;

            if (DGV_Users.Columns.Contains("UserName"))
            {
                var col = DGV_Users.Columns["UserName"];
                col.Visible = true;
                col.HeaderText = "اسم المستخدم";
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            if (DGV_Users.Columns.Contains("FullName"))
            {
                var col = DGV_Users.Columns["FullName"];
                col.Visible = true;
                col.HeaderText = "المسمى الوظيفى";
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }


            DGV_Users.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;

            DGV_Users.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DGV_Users.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 14F, FontStyle.Bold);
            DGV_Users.ColumnHeadersHeight = 40;
            DGV_Users.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            DGV_Users.DefaultCellStyle.Font = new Font("Times New Roman", 14F, FontStyle.Bold);
            DGV_Users.DefaultCellStyle.ForeColor = Color.Black;
            DGV_Users.DefaultCellStyle.BackColor = Color.White;
        }

        private void DGV_Users_RowPrePaint(object? sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (DGV_Users.Rows[e.RowIndex].Cells["IsActive"].Value != null)
            {
                bool isActive = Convert.ToBoolean(DGV_Users.Rows[e.RowIndex].Cells["IsActive"].Value);
                if (!isActive)
                {
                    DGV_Users.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                    DGV_Users.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.DarkRed;
                }
            }
        }

        private void StyleDGV_Permissions()
        {
            DGV_Permissions.RowHeadersVisible = false;
            DGV_Permissions.ReadOnly = true;
            DGV_Permissions.AllowUserToAddRows = false;
            DGV_Permissions.AllowUserToDeleteRows = false;
            DGV_Permissions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            foreach (DataGridViewColumn col in DGV_Permissions.Columns)
                col.Visible = false;

            if (DGV_Permissions.Columns.Contains("PermissionNameAr"))
            {
                var col = DGV_Permissions.Columns["PermissionNameAr"];
                col.Visible = true;
                col.HeaderText = "الصلاحية";
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            if (DGV_Permissions.Columns.Contains("WarehouseName"))
            {
                var col = DGV_Permissions.Columns["WarehouseName"];
                col.Visible = true;
                col.HeaderText = "اسم الفرع";
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            DGV_Permissions.AlternatingRowsDefaultCellStyle.BackColor = Color.Honeydew;


            DGV_Permissions.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DGV_Permissions.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 13F, FontStyle.Bold);
            DGV_Permissions.ColumnHeadersHeight = 40;
            DGV_Permissions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            DGV_Permissions.DefaultCellStyle.Font = new Font("Times New Roman", 10F, FontStyle.Regular);
            DGV_Permissions.DefaultCellStyle.ForeColor = Color.Black;
            DGV_Permissions.DefaultCellStyle.BackColor = Color.White;
        }

        private void LoadAllUsers()
        {
            try
            {
                DGV_Users.DataSource = DBServiecs.User_GetAll();
                StyleDGV_Users();
                DGV_Users.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء تحميل المستخدمين: " + ex.Message);
            }
        }
        private void LoadPermissionsForUser(int userId)
        {
            try
            {
                DataTable dt = DBServiecs.Permission_GetFullForUser(userId);
                DGV_Permissions.DataSource = dt;
                StyleDGV_Permissions();

                // تعبئة بيانات المستخدم في الحقول
                DataRow? selectedUser = DBServiecs.User_GetOne(userId).AsEnumerable().FirstOrDefault();
                if (selectedUser != null)
                {
                    lblID_User.Text = selectedUser["IDUser"].ToString();
                    lblUserName.Text = selectedUser["UserName"].ToString();
                    if (string.IsNullOrWhiteSpace(selectedUser["FullName"]?.ToString()))
                    {
                        lblFullName.Text = "مسمى وظيفي غير محدد";
                        lblFullName.ForeColor = Color.DarkOrange; // اختر لونًا مميزًا مثل برتقالي أو أحمر غامق
                    }
                    else
                    {
                        lblFullName.Text = selectedUser["FullName"].ToString();
                        lblFullName.ForeColor = Color.Black; // اللون الطبيعي عند وجود قيمة
                    }

                    // ✅ تعيين النصوص بناءً على الحالة
                    bool isAdmin = Convert.ToBoolean(selectedUser["IsAdmin"]);
                    bool isActive = Convert.ToBoolean(selectedUser["IsActive"]);

                    // 🧩 التعامل مع الخاصيتين الجديدتين
                    lblIsAdmin.Text = isAdmin ? "المستخدم له خواص الأدمن" : "";
                    lblIsActive.Text = isActive ? "مستخدم فعال الآن" : "مستخدم تم تعطيله";

                    // (اختياري) تغيير اللون لتوضيح الحالة أكثر
                    lblIsAdmin.ForeColor = isAdmin ? Color.DarkRed : Color.Gray;
                    lblIsActive.ForeColor = isActive ? Color.Green : Color.OrangeRed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء تحميل الصلاحيات: " + ex.Message);
            }
        }

        //
        private void LoadUsers()
        {
            var usersTable = DBServiecs.User_GetAll();
            cbxUsers.DisplayMember = "UserName";
            cbxUsers.ValueMember = "IDUser";
            cbxUsers.DataSource = usersTable;
        }

        private void DGVStyl()
        {
            // تفعيل التنسيق المخصص للرأس
            DGV.EnableHeadersVisualStyles = false;

            // تنسيق رأس الجدول
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DGV.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 14, FontStyle.Bold);
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            DGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            // تنسيق الخلايا
            DGV.DefaultCellStyle.Font = new Font("Times New Roman", 12);
            DGV.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // تلوين الصفوف بشكل تبادلي
            DGV.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255); // لون أزرق فاتح جدًا
            DGV.RowsDefaultCellStyle.BackColor = Color.White;

            // خصائص أخرى
            DGV.RowHeadersVisible = false;
            DGV.AllowUserToAddRows = false;
            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // إعادة تسمية الأعمدة للعربية
            if (DGV.Columns.Contains("PermissionNameAr"))
                DGV.Columns["PermissionNameAr"].HeaderText = "اسم الصلاحية";

            if (DGV.Columns.Contains("IsAllowed"))
                DGV.Columns["IsAllowed"].HeaderText = "السماح";

            if (DGV.Columns.Contains("CanAdd"))
                DGV.Columns["CanAdd"].HeaderText = "إضافة";

            if (DGV.Columns.Contains("CanEdit"))
                DGV.Columns["CanEdit"].HeaderText = "تعديل";

            if (DGV.Columns.Contains("CanDelete"))
                DGV.Columns["CanDelete"].HeaderText = "حذف";

            // إخفاء الأعمدة التقنية
            if (DGV.Columns.Contains("PermissionName"))
                DGV.Columns["PermissionName"].Visible = false;

            if (DGV.Columns.Contains("PermissionID"))
                DGV.Columns["PermissionID"].Visible = false;

            if (DGV.Columns.Contains("WarehouseID"))
                DGV.Columns["WarehouseID"].Visible = false;
        }


        private void CbxUsers_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cbxUsers.SelectedValue is int selectedUserId)
                LoadPermissions(selectedUserId);
        }


        private void LoadPermissions(int userId)
        {
            int warehouseId = CurrentSession.WarehouseId;
            var permissions = DBServiecs.Permission_GetByUser(userId, warehouseId);
            DGV.DataSource = permissions;

            // مثال: جعل بعض الأعمدة غير قابلة للتعديل
            if (DGV.Columns.Contains("PermissionID"))
                DGV.Columns["PermissionID"].ReadOnly = true;
        }

        private void BtnSave_Click__(object sender, EventArgs e)
        {
            if (cbxUsers.SelectedValue == null || cbxWarehouses.SelectedValue == null)
            {
                MessageBox.Show("❌ يرجى اختيار المستخدم والفرع أولاً");
                return;
            }

            int userId = Convert.ToInt32(cbxUsers.SelectedValue);

            int warehouseId = Convert.ToInt32(cbxWarehouses.SelectedValue);
            // ✅ تابع التنفيذ هنا بعد التأكد من صحة رقم الفرع

            if (warehouseId < 1)
            {
                MessageBox.Show("⚠️ قم باختيار الفرع بشكل صحيح.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }



            // ✅ استخدام الدالة التي تُعيد المستخدم
            DataTable dt = DBServiecs.User_GetOne(userId);

            bool isAdmin = false;
            if (dt.Rows.Count > 0 && dt.Columns.Contains("IsAdmin"))
            {
                isAdmin = Convert.ToBoolean(dt.Rows[0]["IsAdmin"]);
            }

            // ✅ إذا كان أدمن: استدعاء واحد فقط ثم الخروج
            if (isAdmin)
            {
                if (DGV.Rows.Count > 0 && !DGV.Rows[0].IsNewRow)
                {
                    int permissionId = Convert.ToInt32(DGV.Rows[0].Cells["PermissionID"].Value);

                    DBServiecs.Permission_SetForUser(
                        userId,
                        permissionId,
                        true, true, true, true,
                        warehouseId
                    );
                }

                MessageBox.Show("✅ المستخدم أدمن وتم منحه جميع الصلاحيات تلقائيًا", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // المستخدم ليس أدمن → نحفظ كل صف من الـ DGV كالمعتاد
            foreach (DataGridViewRow row in DGV.Rows)
            {
                if (row.IsNewRow) continue;

                int permissionId = Convert.ToInt32(row.Cells["PermissionID"].Value);
                bool isAllowed = ConvertToBool(row.Cells["IsAllowed"].Value);
                bool canAdd = ConvertToBool(row.Cells["CanAdd"].Value);
                bool canEdit = ConvertToBool(row.Cells["CanEdit"].Value);
                bool canDelete = ConvertToBool(row.Cells["CanDelete"].Value);

                DBServiecs.Permission_SetForUser(
                    userId,
                    permissionId,
                    isAllowed,
                    canAdd,
                    canEdit,
                    canDelete,
                    warehouseId
                );
            }

            MessageBox.Show("✅ تم حفظ الصلاحيات بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private bool ConvertToBool(object value)
        {
            return value != null && value != DBNull.Value && Convert.ToBoolean(value);
        }


        #endregion

        #region === تبويب الصلاحيات ===
        private void tabMang_SelectedIndexChanged(object sender, EventArgs e)
        {
            // تحقق من التبويب الحالي
            TabPage? selectedTab = tabMang.SelectedTab;

            if (selectedTab == tabPageUserSetting || selectedTab == tabPageUserPermissions)
            {
                btnSave.Visible = false; // إخفاء الزر
            }
            else
            {
                btnSave.Visible = true; // إظهاره لباقي الصفحات
            }
        }

        private void fillWarehouses()
        {
            // 1️⃣ تحميل البيانات من قاعدة البيانات
            DataTable dtW = DBServiecs.Warehouse_GetAll();
            if (dtW == null || dtW.Rows.Count == 0) return;

            cbxWarehouses.DataSource = dtW;
            cbxWarehouses.DisplayMember = "WarehouseName"; // عدّل حسب اسم العمود الفعلي
            cbxWarehouses.ValueMember = "WarehouseId";

            // 🔒 منع الكتابة داخل الكمبوبوكس
            cbxWarehouses.DropDownStyle = ComboBoxStyle.DropDownList;

        }
        private void ApplyPermissionsToControls()
        {
            var allControls = GetAllControls(this);

            foreach (Control ctrl in allControls)
            {
                string controlName = ctrl.Name;

                if (string.IsNullOrWhiteSpace(controlName)) continue;

                if (UserPermissionsManager.Permissions.TryGetValue(controlName, out var permission))
                {
                    ctrl.Visible = permission.CanView;
                    ctrl.Enabled = permission.CanView;
                }
            }
        }

        private List<Control> GetAllControls(Control parent)
        {
            List<Control> controls = new List<Control>();

            foreach (Control child in parent.Controls)
            {
                controls.Add(child);
                controls.AddRange(GetAllControls(child));
            }

            return controls;
        }

        private void btnNewUser_Click(object? sender, EventArgs? e)
        {
            DGV_Users.ClearSelection();
            DGV_Permissions.DataSource = null;
            lblID_User.Text = "0";


        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (lblID_User.Text == "0")
            {
                MessageBox.Show("لا يوجد مستخدم محدد للحذف.");
                return;
            }

            int userId = Convert.ToInt32(lblID_User.Text);
            var confirm = MessageBox.Show("هل أنت متأكد من حذف المستخدم؟", "تأكيد", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                string result = DBServiecs.User_DeleteIfAllowed(userId);
                MessageBox.Show(result);
                LoadAllUsers();
                btnNewUser_Click(null, null); // تفريغ البيانات بعد الحذف
            }
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (lblID_User.Text == "0")
            {
                MessageBox.Show("لا يوجد مستخدم محدد.");
                return;
            }

            int userId = Convert.ToInt32(lblID_User.Text);
            var confirm = MessageBox.Show("هل تريد إعادة تعيين كلمة المرور إلى '00'؟", "تأكيد", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                string result = DBServiecs.User_ChangePassword(userId, "00");
                MessageBox.Show(result);
            }
        }

        private void DGV_Users_SelectionChanged(object? sender, EventArgs? e)
        {
            if (DGV_Users.CurrentRow != null && DGV_Users.CurrentRow.Index >= 0)
            {
                int userId = Convert.ToInt32(DGV_Users.CurrentRow.Cells["IDUser"].Value);
                LoadPermissionsForUser(userId);
            }
        }

 

        #endregion

        #region === تبويب البيع والشراء ===


        #endregion

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

        #region === التنقل باستخدام Enter بين الحقول ===

        // التنقل بين مربعات النص داخل التبويب بالضغط على Enter.
        private void HandleEnterKeyNavigation(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                if (sender is Control currentControl)
                {
                    var currentTabControl = FindParentTabControl(currentControl);

                    if (currentTabControl != null)
                    {
                        var tabControls = GetAllTextBoxes(currentTabControl.SelectedTab!)
                                          .OrderBy(c => c.TabIndex).ToList();

                        int currentIndex = tabControls.IndexOf((TextBox)currentControl);

                        if (currentIndex >= 0)
                        {
                            // الانتقال للمربع التالي أو الرجوع لأول واحد
                            int nextIndex = (currentIndex + 1) % tabControls.Count;
                            tabControls[nextIndex].Focus();
                            tabControls[nextIndex].SelectAll();
                        }
                    }
                }
            }
        }

        // الحصول على جميع مربعات النص داخل حاوية معينة.
        private List<TextBox> GetAllTextBoxes(Control parent)
        {
            var list = new List<TextBox>();
            foreach (Control c in parent.Controls)
            {
                if (c is TextBox tb)
                    list.Add(tb);
                else if (c.HasChildren)
                    list.AddRange(GetAllTextBoxes(c));
            }
            return list;
        }

        // ربط حدث KeyDown بجميع مربعات النص داخل التبويبات.
        private void TextBoxesInTabs()
        {
            foreach (TabPage tab in tabMang.TabPages)
            {
                foreach (TextBox tb in GetAllTextBoxes(tab))
                {
                    tb.KeyDown += HandleEnterKeyNavigation;
                }
            }
        }

        // البحث عن TabControl الذي يحتوي على العنصر.
        private TabControl? FindParentTabControl(Control? control)
        {
            while (control != null)
            {
                if (control.Parent is TabControl tab)
                    return tab;

                control = control.Parent;
            }
            return null;
        }

        #endregion

        #region 🔹 إعداد وضع البيع المرتد (ReturnSaleMode)

        // 🔹 ملء ComboBox بالقيم المتاحة (1، 2، 3) وربطه بالبيانات
        private void FillcbxReturnSaleMode()
        {
            cbxReturnSaleMode.DropDownStyle = ComboBoxStyle.DropDownList;

            var saleModes = new List<KeyValuePair<int, string>>
    {
        new KeyValuePair<int, string>(1, "حسب فاتورة البيع"),
        new KeyValuePair<int, string>(2, "بالكود مباشر"),
        new KeyValuePair<int, string>(3, "بالنظامين")
    };

            cbxReturnSaleMode.DataSource = saleModes;
            cbxReturnSaleMode.DisplayMember = "Value";
            cbxReturnSaleMode.ValueMember = "Key";

            // تحميل القيمة المحفوظة
            int savedMode = AppSettings.GetInt("ReturnSaleMode", 1);
            cbxReturnSaleMode.SelectedValue = savedMode;
        }

        // 🔹 ملء ComboBox بالقيم المتاحة (1، 2، 3) وربطه بالبيانات
        private void FillcbxReturnPurchasesMode()
        {
            cbxReturnPurchasesMode.DropDownStyle = ComboBoxStyle.DropDownList;

            var PurchModes = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(1, "حسب فاتورة الشراء"),
                new KeyValuePair<int, string>(2, "بالكود مباشر"),
                new KeyValuePair<int, string>(3, "بالنظامين")
            };

            cbxReturnPurchasesMode.DataSource = PurchModes;
            cbxReturnPurchasesMode.DisplayMember = "Value";
            cbxReturnPurchasesMode.ValueMember = "Key";

            // تحميل القيمة المحفوظة
            int savedMode = AppSettings.GetInt("ReturnPurchasesMode", 1);
            cbxReturnPurchasesMode.SelectedValue = savedMode;
        }

        #endregion

        private void btnUserPermissions_Click(object sender, EventArgs e)
        {
            if (cbxUsers.SelectedValue == null || cbxWarehouses.SelectedValue == null)
            {
                MessageBox.Show("❌ يرجى اختيار المستخدم والفرع أولاً");
                return;
            }

            int userId = Convert.ToInt32(cbxUsers.SelectedValue);

            int warehouseId = Convert.ToInt32(cbxWarehouses.SelectedValue);
            // ✅ تابع التنفيذ هنا بعد التأكد من صحة رقم الفرع

            if (warehouseId < 1)
            {
                MessageBox.Show("⚠️ قم باختيار الفرع بشكل صحيح.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }



            // ✅ استخدام الدالة التي تُعيد المستخدم
            DataTable dt = DBServiecs.User_GetOne(userId);

            bool isAdmin = false;
            if (dt.Rows.Count > 0 && dt.Columns.Contains("IsAdmin"))
            {
                isAdmin = Convert.ToBoolean(dt.Rows[0]["IsAdmin"]);
            }

            // ✅ إذا كان أدمن: استدعاء واحد فقط ثم الخروج
            if (isAdmin)
            {
                if (DGV.Rows.Count > 0 && !DGV.Rows[0].IsNewRow)
                {
                    int permissionId = Convert.ToInt32(DGV.Rows[0].Cells["PermissionID"].Value);

                    DBServiecs.Permission_SetForUser(
                        userId,
                        permissionId,
                        true, true, true, true,
                        warehouseId
                    );
                }

                MessageBox.Show("✅ المستخدم أدمن وتم منحه جميع الصلاحيات تلقائيًا", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // المستخدم ليس أدمن → نحفظ كل صف من الـ DGV كالمعتاد
            foreach (DataGridViewRow row in DGV.Rows)
            {
                if (row.IsNewRow) continue;

                int permissionId = Convert.ToInt32(row.Cells["PermissionID"].Value);
                bool isAllowed = ConvertToBool(row.Cells["IsAllowed"].Value);
                bool canAdd = ConvertToBool(row.Cells["CanAdd"].Value);
                bool canEdit = ConvertToBool(row.Cells["CanEdit"].Value);
                bool canDelete = ConvertToBool(row.Cells["CanDelete"].Value);

                DBServiecs.Permission_SetForUser(
                    userId,
                    permissionId,
                    isAllowed,
                    canAdd,
                    canEdit,
                    canDelete,
                    warehouseId
                );
            }

            MessageBox.Show("✅ تم حفظ الصلاحيات بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

    }
}
// ➕ إضافة .
// 🗑️ حذف .
// ✏️ تعديل .
// 📋 تحميل.
// ⭐ تعيين .
// 🧾 تحديث .
