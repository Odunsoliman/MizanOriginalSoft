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
                TextBoxesInTabs();
                LoadBackupFiles();
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


        #endregion


        #region === تبويب الصلاحيات ===


        #endregion



        #region === تبويب البيع والشراء ===


        #endregion




        #region === احتياطي: KeyDown لربطه بالتنقل لاحقًا ===

        private void txtRollLabelWidth_KeyDown(object sender, KeyEventArgs e)  { }
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

    }
}
// ➕ إضافة فرع جديد.
// 🗑️ حذف الفرع المحدد.
// ✏️ تعديل اسم الفرع المحدد.
// 📋 تحميل قائمة الفروع في الكومبوبوكس.
// ⭐ تعيين الفرع الحالي كافتراضي لهذه النسخة.
// 🧾 تحديث ملف الإعدادات بالفرع الافتراضي.
