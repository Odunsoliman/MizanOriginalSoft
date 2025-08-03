using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses;
using System.Data;
using System.Text;


namespace MizanOriginalSoft.Views.Forms.MainForms

{
    public partial class frmGenralData : Form
    {


        private string configFilePath = "serverConnectionSettings.txt";

        public frmGenralData()
        {
            InitializeComponent();
        }

        private void frmGenralData_Load(object sender, EventArgs e)
        {
            tabControl.ItemSize = new Size(150, 40); // ضبط حجم عناصر التبويب
            LoadWarehouses();                        // تحميل الفروع إلى ComboBox
            LoadSettings();    //هل من الضرورى اعادة التحميل هنا                      // إعادة تحميل الإعدادات بعد تحميل الفروع
            tlpPading();                             // ضبط الحشوات (تصميم)
            UpdateLabelCount();                      // تحديث عداد ملصقات الطباعة أو العناصر
            TextBoxesInTabs();                       // إعداد مربعات النص ضمن التبويبات
            txtNameCo.Focus();                       // تركيز المؤشر على اسم الشركة
            txtNameCo.SelectAll();                   // تحديد كامل النص
            LoadBackupFiles();                       // تحميل النسخ الاحتياطية (تأكد من جاهزيتها)
            AttachTextBoxHandlers(this);             // ربط أحداث مربعات النص العامة

        }

        #region تحميل بيانات الفروع
        #endregion

        #region تحميل الإعدادات من الملف
        private void LoadSettings()
        {
            if (!File.Exists(configFilePath))
                return;

            string[] lines = File.ReadAllLines(configFilePath);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || !line.Contains("="))
                    continue;

                string key = line.Split('=')[0].Trim();
                string value = line.Substring(line.IndexOf('=') + 1).Trim();

                switch (key)
                {
                    case "serverName": txtServerName.Text = value; break;
                    case "DBName": txtDBName.Text = value; break;
                    case "RollPrinter": lblRollPrinter.Text = value; break;
                    case "BackupsPath": txtBackupsPath.Text = value; break;
                    case "maxBackups": txtMaxBackups.Text = value; break;
                    case "SheetPrinter": lblSheetPrinter.Text = value; break;
                    case "SheetRows": txtSheetRows.Text = value; break;
                    case "SheetCols": txtSheetCols.Text = value; break;
                    case "SheetMarginTop": txtMarginTop.Text = value; break;
                    case "SheetMarginBottom": txtMarginBottom.Text = value; break;
                    case "SheetMarginRight": txtMarginRight.Text = value; break;
                    case "SheetMarginLeft": txtMarginLeft.Text = value; break;
                    case "RollLabelWidth": txtRollLabelWidth.Text = value; break;
                    case "RollLabelHeight": txtRollLabelHeight.Text = value; break;
                    case "CompanyName": txtNameCo.Text = value; break;
                    case "CompanyPhon": txtPhon.Text = value; break;
                    case "CompanyAnthrPhon": txtAnthrPhon.Text = value; break;
                    case "SalesTax": txtSalesTax.Text = value; break;
                    case "CompanyAdreass": txtAdreass.Text = value; break;
                    case "EmailCo": txtCompanyEmail.Text = value; break;
                    case "CompanyLoGoFolder": lblLogoPath.Text = value; break;
                    case "LogoImagName": lblLogoImageName.Text = value; break;
                    case "DefaltWarehouseId":
                        if (int.TryParse(value, out int defWarehouseId))
                            cbxWarehouseId.SelectedValue = defWarehouseId;
                        break;
                }
            }

            // تحميل الشعار إذا كان المسار صحيحاً
            if (!string.IsNullOrEmpty(lblLogoPath.Text) && !string.IsNullOrEmpty(lblLogoImageName.Text))
            {
                string logoPath = Path.Combine(lblLogoPath.Text, lblLogoImageName.Text);
                if (File.Exists(logoPath))
                    picLogoCo.Image = Image.FromFile(logoPath);
            }
        }
        #endregion

        #region حفظ الإعدادات إلى الملف
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtServerName.Text) || string.IsNullOrWhiteSpace(txtDBName.Text))
            {
                MessageBox.Show("يرجى تحديد السيرفر وقاعدة البيانات.");
                return;
            }

            try
            {
                List<string> settings = new List<string>
        {
            $"serverName={txtServerName.Text}",
            $"DBName={txtDBName.Text}",
            $"maxBackups={txtMaxBackups.Text}",
            $"BackupsPath={txtBackupsPath.Text}",
            $"RollPrinter={lblRollPrinter.Text}",
            $"SheetPrinter={lblSheetPrinter.Text}",
            $"SheetRows={txtSheetRows.Text}",
            $"SheetCols={txtSheetCols.Text}",
            $"SheetMarginTop={txtMarginTop.Text}",
            $"SheetMarginBottom={txtMarginBottom.Text}",
            $"SheetMarginRight={txtMarginRight.Text}",
            $"SheetMarginLeft={txtMarginLeft.Text}",
            $"RollLabelWidth={txtRollLabelWidth.Text}",
            $"RollLabelHeight={txtRollLabelHeight.Text}",
            $"CompanyName={txtNameCo.Text}",
            $"CompanyPhon={txtPhon.Text}",
            $"CompanyAnthrPhon={txtAnthrPhon.Text}",
            $"SalesTax={txtSalesTax.Text}",
            $"CompanyAdreass={txtAdreass.Text}",
            $"EmailCo={txtCompanyEmail.Text}",
            $"CompanyLoGoFolder={lblLogoPath.Text}",
            $"LogoImagName={lblLogoImageName.Text}",
            $"DefaltWarehouseId={(cbxWarehouseId.SelectedValue ?? "")}"
        };

                File.WriteAllLines(configFilePath, settings);
                LoadSettings(); // إعادة تحميل الإعدادات بعد الحفظ

                MessageBox.Show("✅ تم حفظ الإعدادات بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowWarning($"حدث خطأ أثناء حفظ الإعدادات:\n{ex.Message}", "خطأ");
            }
        }
        #endregion

        #region حفظ الإعدادات بصمت (بدون رسالة)
        private void SaveData()
        {
            List<string> settings = new List<string>
    {
        $"serverName={txtServerName.Text}",
        $"DBName={txtDBName.Text}",
        $"maxBackups={txtMaxBackups.Text}",
        $"BackupsPath={txtBackupsPath.Text}",
        $"RollPrinter={lblRollPrinter.Text}",
        $"SheetPrinter={lblSheetPrinter.Text}",
        $"SheetRows={txtSheetRows.Text}",
        $"SheetCols={txtSheetCols.Text}",
        $"SheetMarginTop={txtMarginTop.Text}",
        $"SheetMarginBottom={txtMarginBottom.Text}",
        $"SheetMarginRight={txtMarginRight.Text}",
        $"SheetMarginLeft={txtMarginLeft.Text}",
        $"RollLabelWidth={txtRollLabelWidth.Text}",
        $"RollLabelHeight={txtRollLabelHeight.Text}",
        $"CompanyName={txtNameCo.Text}",
        $"CompanyPhon={txtPhon.Text}",
        $"CompanyAnthrPhon={txtAnthrPhon.Text}",
        $"SalesTax={txtSalesTax.Text}",
        $"CompanyAdreass={txtAdreass.Text}",
        $"EmailCo={txtCompanyEmail.Text}",
        $"CompanyLoGoFolder={lblLogoPath.Text}",
        $"LogoImagName={lblLogoImageName.Text}",
        $"DefaltWarehouseId={(cbxWarehouseId.SelectedValue ?? "")}"
    };

            File.WriteAllLines(configFilePath, settings);
        }
        #endregion

        #region === مرفقات خاصة بمربعات النصوص ===

        /// <summary>
        /// ربط حدث الخروج (Leave) بجميع مربعات النص داخل الحاوية.
        /// عند الخروج من أي مربع نص وتغيير القيمة، يتم حفظ التغييرات تلقائيًا.
        /// </summary>
        private void AttachTextBoxHandlers(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is TextBox txt)
                {
                    txt.Tag = txt.Text; // حفظ القيمة الأصلية للمقارنة لاحقًا
                    txt.Leave += TextBox_Leave;
                }
                else if (ctrl.HasChildren)
                {
                    AttachTextBoxHandlers(ctrl); // تكرار على الأبناء (Recursive)
                }
            }
        }

        /// <summary>
        /// تنفيذ الحفظ عند تغيير قيمة مربع النص.
        /// </summary>
        private void TextBox_Leave(object? sender, EventArgs e)
        {
            if (sender is TextBox txt)
            {
                if ((txt.Tag is string oldValue) && txt.Text != oldValue)
                {
                    SaveData(); // حفظ التغييرات
                    txt.Tag = txt.Text; // تحديث القيمة المرجعية
                }
            }
        }

        #endregion

        #region === التنقل باستخدام Enter بين الحقول ===

        /// <summary>
        /// التنقل بين مربعات النص داخل التبويب بالضغط على Enter.
        /// </summary>
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

        /// <summary>
        /// الحصول على جميع مربعات النص داخل حاوية معينة.
        /// </summary>
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

        /// <summary>
        /// ربط حدث KeyDown بجميع مربعات النص داخل التبويبات.
        /// </summary>
        private void TextBoxesInTabs()
        {
            foreach (TabPage tab in tabControl.TabPages)
            {
                foreach (TextBox tb in GetAllTextBoxes(tab))
                {
                    tb.KeyDown += HandleEnterKeyNavigation;
                }
            }
        }

        /// <summary>
        /// البحث عن TabControl الذي يحتوي على العنصر.
        /// </summary>
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

        #region === إعدادات A4 Sheet: الحواف واللاصقات ===

        /// <summary>
        /// حساب عدد اللاصقات المعروضة في الورقة.
        /// </summary>
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
                        picLogoCo.SizeMode = PictureBoxSizeMode.StretchImage;
                        SaveData();

                        MessageBox.Show("✅ تم تغيير صورة اللوجو بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        #region ✅ التنقل بين التبويبات

        /// <summary>
        /// عند تغيير التبويب، يتم التركيز تلقائيًا على أول مربع نص فيه.
        /// </summary>
        private void tabControl_SelectedIndexChanged(object? sender, EventArgs e)
        {
            TabPage? selectedTab = tabControl.SelectedTab;

            if (selectedTab != null)
            {
                var textBoxes = GetAllTextBoxes(selectedTab)
                    .OrderBy(tb => tb.TabIndex)
                    .ToList();

                if (textBoxes.Count > 0)
                    textBoxes[0].Focus();
            }
        }

        #endregion

        #region ✅ تحميل النسخ الاحتياطية

        /// <summary>
        /// تحميل ملفات النسخ الاحتياطية من المسار المحدد في الإعدادات.
        /// </summary>
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

                string? backupPath = new AppSettings(settingsPath).GetString("BackupsPath", null);

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

        #endregion

        #region ✅ اختيار مجلد النسخ الاحتياطية

        /// <summary>
        /// اختيار مجلد النسخ الاحتياطية من المستخدم.
        /// </summary>
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

        #endregion

        #region ✅ استرجاع النسخة الاحتياطية

        /// <summary>
        /// تنفيذ عملية استرجاع نسخة احتياطية مع إنشاء نسخة احتياطية مؤقتة أولاً.
        /// </summary>
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
                var helper = new DatabaseBackupRestoreHelper(settingsPath);
                var appSettings = new AppSettings(settingsPath);

                string? dbName = appSettings.GetString("DBName", "");
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

                // الخطوة 1: إنشاء نسخة احتياطية حالية
                helper.BackupDatabase(settingsPath);
                MessageBox.Show("✅ تم إنشاء نسخة احتياطية من الوضع الحالي بنجاح.");

                // الخطوة 2: إظهار نافذة تحميل مؤقتة
                frmLoading loadingForm = new frmLoading("جارٍ استرجاع النسخة الاحتياطية، الرجاء الانتظار...");
                loadingForm.Show();
                loadingForm.Refresh();

                // الخطوة 3: تنفيذ الاسترجاع في مهمة منفصلة
                await Task.Run(() => helper.RestoreDatabase(dbName, selectedBackupFile));

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

        #endregion

        #region ✅ حفظ عدد النسخ الاحتياطية القصوى

        /// <summary>
        /// عند مغادرة الحقل يتم حفظ القيمة تلقائيًا.
        /// </summary>
        private void txtMaxBackups_Leave(object sender, EventArgs e)
        {
            SaveData();
        }

        #endregion

        #region ✅ إدارة الفروع (المخازن)

        /// <summary>
        /// إضافة فرع جديد.
        /// </summary>
        private void btnAddWarehouse_Click(object sender, EventArgs e)
        {
            string name = txtWarehouseName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("❌ يرجى إدخال اسم الفرع أولًا.");
                txtWarehouseName.Focus();
                return;
            }

            int userId = CurrentSession.UserID;
            string message = DBServiecs.Warehouse_Add(name, userId);
            MessageBox.Show(message);
            LoadWarehouses();// كتبت هذه الدالة لاعادة تحميل الكمبوبكس
        }

        /// <summary>
        /// حذف الفرع المحدد.
        /// </summary>
        private void btnDeleteWarehous_Click(object sender, EventArgs e)
        {
            if (cbxWarehouseId.SelectedValue == null)
            {
                MessageBox.Show("❌ يرجى اختيار الفرع المراد حذفه.");
                return;
            }

            int warehouseId = Convert.ToInt32(cbxWarehouseId.SelectedValue);
            int userId = CurrentSession.UserID;

            DialogResult confirm = MessageBox.Show("هل أنت متأكد من حذف الفرع المحدد؟",
                "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            string message = DBServiecs.Warehouse_Delete(warehouseId, userId);
            MessageBox.Show(message);
            LoadWarehouses();
        }

        /// <summary>
        /// تعديل اسم الفرع المحدد.
        /// </summary>
        private void btnRenamWarehous_Click(object sender, EventArgs e)
        {
            if (cbxWarehouseId.SelectedValue == null)
            {
                MessageBox.Show("❌ يرجى اختيار الفرع المراد تعديله.");
                return;
            }

            string newName = txtWarehouseName.Text.Trim();
            if (string.IsNullOrWhiteSpace(newName))
            {
                MessageBox.Show("❌ يرجى إدخال الاسم الجديد للفرع.");
                txtWarehouseName.Focus();
                return;
            }

            int warehouseId = Convert.ToInt32(cbxWarehouseId.SelectedValue);
            int userId = CurrentSession.UserID;

            string message = DBServiecs.Warehouse_UpdateName(warehouseId, newName, userId);
            MessageBox.Show(message);
            LoadWarehouses();
        }
        private void LoadWarehouses()
        {
            try
            {
                DataTable dt = DBServiecs.Warehouse_GetAll();

                if (dt != null && dt.Rows.Count > 0)
                {
                    // ✅ إنشاء صف اختياري "اختر الفرع..."
                    DataRow defaultRow = dt.NewRow();
                    defaultRow["WarehouseId"] = -1;
                    defaultRow["WarehouseName"] = "اختر الفرع...";
                    dt.Rows.InsertAt(defaultRow, 0); // إدراجه في أول الصفوف

                    // ✅ ربط الكمبوبوكس
                    cbxWarehouseId.DataSource = null;
                    cbxWarehouseId.DataSource = dt;
                    cbxWarehouseId.DisplayMember = "WarehouseName";
                    cbxWarehouseId.ValueMember = "WarehouseId";
                    cbxWarehouseId.SelectedIndex = 0; // جعل "اختر الفرع..." هو الظاهر
                }
                else
                {
                    cbxWarehouseId.DataSource = null;
                    MessageBox.Show("لا توجد فروع مسجلة في النظام", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء تحميل الفروع:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnSetAsDefaultWarehouse_Click(object sender, EventArgs e)
        {
            if (cbxWarehouseId.SelectedValue == null)
            {
                MessageBox.Show("❌ يرجى اختيار فرع من القائمة أولًا.", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // الحصول على رقم الفرع المختار
            int selectedWarehouseId = Convert.ToInt32(cbxWarehouseId.SelectedValue);

            DialogResult confirm = MessageBox.Show(
                $"هل تريد تعيين الفرع رقم {selectedWarehouseId} كفرع افتراضي لهذه النسخة؟\n" +
                "سيتم تفعيل التخصيص عند إعادة تشغيل البرنامج.",
                "تأكيد التخصيص",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                UpdateDefaultWarehouseId(selectedWarehouseId);
            }
        }

        private void UpdateDefaultWarehouseId(int newId)
        {
            try
            {
                string filePath = Path.Combine(Application.StartupPath, "serverConnectionSettings.txt");
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("⚠️ ملف الإعدادات غير موجود!", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var lines = File.ReadAllLines(filePath).ToList();
                bool found = false;

                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].StartsWith("DefaultWarehouseId=", StringComparison.OrdinalIgnoreCase))
                    {
                        lines[i] = $"DefaultWarehouseId={newId}";
                        found = true;
                        break;
                    }
                }

                if (!found)
                    lines.Add($"DefaultWarehouseId={newId}");

                File.WriteAllLines(filePath, lines, Encoding.UTF8);

                MessageBox.Show("✅ تم حفظ التخصيص بنجاح.\nيجب إعادة تشغيل البرنامج لتفعيل التغييرات.",
                                "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء حفظ التخصيص:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

    }
}
