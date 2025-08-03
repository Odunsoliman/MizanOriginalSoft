using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses;
using System.Data;


namespace MizanOriginalSoft.Views.Forms.MainForms

{
    public partial class frmGenralData : Form
    {
        private string configFilePath = "serverConnectionSettings.txt";

        public frmGenralData()
        {
            InitializeComponent();
            LoadSettings();
        }
        private void frmGenralData_Load(object sender, EventArgs e)
        {
            tabControl.ItemSize = new Size(150, 40); // العرض = 150، الارتفاع = 40
            LoadSettings();
            tlpPading();
            UpdateLabelCount();
            TextBoxesInTabs();
            txtNameCo .Focus ();
            txtNameCo.SelectAll ();
            LoadBackupFiles();//لماذا يحدث خطأ عندما اطلب تحميل هذه الدالة 
            AttachTextBoxHandlers(this);
        }



        private void AttachTextBoxHandlers(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is TextBox txt)
                {
                    txt.Tag = txt.Text;
                    txt.Leave += TextBox_Leave;
                }
                else if (ctrl.HasChildren)
                {
                    AttachTextBoxHandlers(ctrl); // recursive
                }
            }
        }

        private void TextBox_Leave(object? sender, EventArgs e)
        {
            if (sender is TextBox txt)
            {
                if ((txt.Tag is string oldValue) && txt.Text != oldValue)
                {
                    SaveData();
                    txt.Tag = txt.Text;
                }
            }
        }

        #region @@@@ Enter Navigation @@@@

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
                            .OrderBy(c => c.TabIndex)
                            .ToList();

                        if (currentControl is TextBox currentTextBox)
                        {
                            int currentIndex = tabControls.IndexOf(currentTextBox);

                            if (currentIndex >= 0)
                            {
                                if (currentIndex < tabControls.Count - 1)
                                {
                                    tabControls[currentIndex + 1].Focus();
                                }
                                else
                                {
                                    tabControls[0].Focus();
                                    tabControls[0].SelectAll();
                                }
                            }
                        }
                    }
                }
            }
        }

        private List<TextBox> GetAllTextBoxes(Control parent)
        {
            var textBoxes = new List<TextBox>();

            foreach (Control control in parent.Controls)
            {
                if (control is TextBox tb)
                    textBoxes.Add(tb);
                else if (control.HasChildren)
                    textBoxes.AddRange(GetAllTextBoxes(control));
            }

            return textBoxes;
        }

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

        private void LoadWarehouses()
        {
            try
            {
                cbxWarehouseId .Items.Clear();
                DataTable dt = DBServiecs.Warehouse_GetAll();

                if (dt != null && dt.Rows.Count > 0)
                {
                    cbxWarehouseId.DataSource = dt;
                    cbxWarehouseId.DisplayMember = "WarehouseName";
                    cbxWarehouseId.ValueMember = "WarehouseId";

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
                    case "serverName":
                        txtServerName.Text = value;
                        break;
                    case "DBName":
                        txtDBName.Text = value;
                        break;
                    case "RollPrinter":
                        lblRollPrinter.Text = value;
                        break;
                    case "BackupsPath":
                        txtBackupsPath .Text = value;
                        break;
                    case "maxBackups":
                        txtMaxBackups .Text = value;
                        break;
                    case "SheetPrinter":
                        lblSheetPrinter.Text = value;
                        break;
                    case "SheetRows":
                        txtSheetRows.Text = value;
                        break;
                    case "SheetCols":
                        txtSheetCols.Text = value;
                        break;
                    case "SheetMarginTop":
                        txtMarginTop.Text = value;
                        break;
                    case "SheetMarginBottom":
                        txtMarginBottom.Text = value;
                        break;
                    case "SheetMarginRight":
                        txtMarginRight.Text = value;
                        break;
                    case "SheetMarginLeft":
                        txtMarginLeft.Text = value;
                        break;
                    case "RollLabelWidth":
                        txtRollLabelWidth.Text = value;
                        break;
                    case "RollLabelHeight":
                        txtRollLabelHeight.Text = value;
                        break;
                    case "CompanyName":
                        txtNameCo .Text = value;
                        break;
                    case "CompanyPhon":
                        txtPhon .Text = value;
                        break;
                    case "CompanyAnthrPhon":
                        txtAnthrPhon .Text = value;
                        break;
                    case "SalesTax":
                        txtSalesTax.Text = value;
                        break;
                    case "CompanyAdreass":
                        txtAdreass .Text = value;
                        break;
                    case "EmailCo":
                        txtCompanyEmail.Text = value;
                        break;
                    case "CompanyLoGoFolder":
                        lblLogoPath .Text = value;
                        break;
                    case "LogoImagName":
                        lblLogoImageName.Text = value;
                        break;
                    case "DefaltWarehouseId":
                        cbxWarehouseId .SelectedValue  = value;
                        break;
                }
            }

            // تحميل الصورة إذا كان مسارها موجوداً
            if (!string.IsNullOrEmpty(lblLogoPath.Text) && !string.IsNullOrEmpty(lblLogoImageName.Text))
            {
                string logoPath = Path.Combine(lblLogoPath.Text, lblLogoImageName.Text);
                if (File.Exists(logoPath))
                {
                    picLogoCo .Image = Image.FromFile(logoPath);
                }
            }
        }
        // حفظ الإعدادات في الملف
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtServerName.Text) ||
                string.IsNullOrWhiteSpace(txtDBName.Text))
            {
                MessageBox.Show("يرجى تحديد السيرفر وقاعدة البيانات.");
                return;
            }

            List<string> settings = new List<string>
                {
                    $"serverName={txtServerName.Text}",
                    $"DBName={txtDBName.Text}",
                    $"maxBackups={txtMaxBackups .Text}",
                    $"BackupsPath={txtBackupsPath .Text}",

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
                    $"LogoImagName={lblLogoImageName.Text}"
                };

            try
            {
                File.WriteAllLines(configFilePath, settings);
     //           CustomMessageBox.ShowInformation("تم حفظ الإعدادات بنجاح", "حفظ");

                // تحميل الإعدادات الجديدة مباشرة بعد الحفظ
                LoadSettings();
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowWarning ($"حدث خطأ أثناء حفظ الإعدادات:\n{ex.Message}", "خطأ");
            }
        }      

        private void SaveData()
        {

            List<string> settings = new List<string>
                {
                    $"serverName={txtServerName.Text}",
                    $"DBName={txtDBName.Text}",
                    $"maxBackups={txtMaxBackups .Text}",
                    $"BackupsPath={txtBackupsPath .Text}",

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
                    $"LogoImagName={lblLogoImageName.Text}"
                };

                File.WriteAllLines(configFilePath, settings);
             
        }
        #region @@@ A4 Sheet @@@
        private void UpdateLabelCount()
        {
            int rows, cols;
            if (int.TryParse(txtSheetRows.Text, out rows) && int.TryParse(txtSheetCols.Text, out cols))
            {
                lblCountLables.Text = "عدد اللاصقات : " + (rows * cols).ToString();
            }
            else
            {
                lblCountLables.Text = "اكتب رقم صحيح";
            }
        }
        private void txtSheetRows_TextChanged(object sender, EventArgs e)
        {
            UpdateLabelCount();
        }

        private void txtSheetCols_TextChanged(object sender, EventArgs e)
        {
            UpdateLabelCount();
        }
        private void tlpPading()
        {
            try
            {
                // الحصول على القيم من TextBoxes وتحويلها إلى أعداد صحيحة
                int top = string.IsNullOrEmpty(txtMarginTop.Text) ? 0 : int.Parse(txtMarginTop.Text);
                int bottom = string.IsNullOrEmpty(txtMarginBottom.Text) ? 0 : int.Parse(txtMarginBottom.Text);
                int left = string.IsNullOrEmpty(txtMarginLeft.Text) ? 0 : int.Parse(txtMarginLeft.Text);
                int right = string.IsNullOrEmpty(txtMarginRight.Text) ? 0 : int.Parse(txtMarginRight.Text);

                // تطبيق القيم الجديدة على Padding
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
            pnlMargen.BorderStyle = BorderStyle.None; // يجب تعطيل BorderStyle الافتراضي
            pnlMargen.Paint += (sender, e) => {
                ControlPaint.DrawBorder(e.Graphics, pnlMargen.ClientRectangle,
                    Color.Blue, 1, ButtonBorderStyle.Solid, // أعلى
                    Color.GreenYellow , 2, ButtonBorderStyle.Solid, // يمين
                    Color.Blue, 1, ButtonBorderStyle.Solid, // أسفل
                    Color.GreenYellow , 2, ButtonBorderStyle.Solid); // يسار
            };
        }

        private void txtMarginTop_TextChanged(object sender, EventArgs e)
        {
            tlpPading();
        }

        #endregion
        #region @@@@ Genral Settings @@@@

        // تحميل الإعدادات المحفوظة سابقًا من الملف
        private void LoadSet22tings()
        {/*هذه الوظيفة تجلب بعض البيانات من الملف النصى واريد جلب باقى البيانات الناقصةمن هذه البيانات
          serverName=DESKTOP-EE70K28\SQLEXPRESS
DBName=Signee

RollPrinter=Samsung SCX-3400 Series
SheetPrinter=Samsung SCX-3400 Series
SheetRows=11
SheetCols=5
SheetMarginTop=0
SheetMarginBottom=0
SheetMarginRight=0
SheetMarginLeft=0
RollLabelWidth=50
RollLabelHeight=25

CompanyName=Signee
CompanyPhon=01020506025
CompanyAnthrPhon=01201201205
SalesTax=0.14
CompanyAdreass=ش عبد الخالق ثروت وسط البلد القاهرة
EmailCo=Signee@gmail.com
CompanyLoGoFolder=D:\MizanSoft\MizanLoom\Signee\Signee\bin\Debug
LogoImagName=Signee.png

          */
            if (!File.Exists(configFilePath))
                return;

            string[] lines = File.ReadAllLines(configFilePath);
            foreach (string line in lines)
            {
                if (line.StartsWith("serverName="))
                    txtServerName.Text = line.Replace("serverName=", "").Trim();
                else if (line.StartsWith("DBName="))
                    txtDBName.Text = line.Replace("DBName=", "").Trim();
                else if (line.StartsWith("RollPrinter="))
                    lblRollPrinter.Text = line.Replace("RollPrinter=", "").Trim();
                else if (line.StartsWith("SheetPrinter="))
                    lblSheetPrinter.Text = line.Replace("SheetPrinter=", "").Trim();
                else if (line.StartsWith("SheetRows="))
                    txtSheetRows.Text = line.Replace("SheetRows=", "").Trim();
                else if (line.StartsWith("SheetCols="))
                    txtSheetCols.Text = line.Replace("SheetCols=", "").Trim();
                else if (line.StartsWith("SheetMarginTop="))
                    txtMarginTop.Text = line.Replace("SheetMarginTop=", "").Trim();
                else if (line.StartsWith("SheetMarginBottom="))
                    txtMarginBottom.Text = line.Replace("SheetMarginBottom=", "").Trim();
                else if (line.StartsWith("SheetMarginRight="))
                    txtMarginRight.Text = line.Replace("SheetMarginRight=", "").Trim();
                else if (line.StartsWith("SheetMarginLeft="))
                    txtMarginLeft.Text = line.Replace("SheetMarginLeft=", "").Trim();

            }
        }

        private void btnLoadRollPrinter_Click(object sender, EventArgs e)
        {
            using (PrintDialog printDialog = new PrintDialog())
            {
                printDialog.AllowSomePages = false;
                printDialog.ShowHelp = false;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    // تعيين اسم الطابعة
                    lblRollPrinter.Text = printDialog.PrinterSettings.PrinterName;

                    // حفظ التغييرات مباشرة
                    SaveData();
                }
            }
        }


        private void btnLoadSheetPrinter_Click(object sender, EventArgs e)
        {
            // إنشاء كائن من نوع PrintDialog
            using (PrintDialog printDialog = new PrintDialog())
            {
                printDialog.AllowSomePages = false;
                printDialog.ShowHelp = false;

                // عرض الحوار للمستخدم
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    // الحصول على اسم الطابعة المختارة
                    lblSheetPrinter.Text = printDialog.PrinterSettings.PrinterName;

                    // حفظ التغييرات مباشرة
                    SaveData();
                }
            }
        }
        #endregion


        private void btnChangLogo_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                openFileDialog.Title = "اختر صورة اللوجو";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    lblLogoPath.Text = Path.GetDirectoryName(openFileDialog.FileName);
                    lblLogoImageName.Text = Path.GetFileName(openFileDialog.FileName); // تخزين اسم الملف فقط

                    try
                    {
                        picLogoCo.Image = Image.FromFile(openFileDialog.FileName);
                        picLogoCo.SizeMode = PictureBoxSizeMode.StretchImage;

                        // حفظ التغييرات مباشرة
                        SaveData();
                        // عرض رسالة نجاح
                        MessageBox.Show("تم تغيير صورة اللوجو بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("حدث خطأ أثناء تحميل الصورة: " + ex.Message,
                                      "خطأ",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Error);
                        lblLogoPath.Text = string.Empty;
                        lblLogoImageName.Text = string.Empty;
                    }
                }
            }
        }

        #region 
        private void txtRollLabelWidth_KeyDown(object sender, KeyEventArgs e)
        {
            //اريد عند النقر على انتر ينتقل للذى بعدة
        }

        private void txtRollLabelHeight_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtSheetRows_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtSheetCols_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtMarginTop_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtMarginBottom_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtMarginRight_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtMarginLeft_KeyDown(object sender, KeyEventArgs e)
        {

        }
        #endregion







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

        private void LoadBackupFiles()
        {
            try
            {
                string settingsPath = Path.Combine(Application.StartupPath, "serverConnectionSettings.txt");

                if (!File.Exists(settingsPath))
                {
                    MessageBox.Show("ملف الإعدادات غير موجود في: " + settingsPath);
                    return;
                }

                string? backupPath = new AppSettings(settingsPath).GetString("BackupsPath", null);

                if (string.IsNullOrWhiteSpace(backupPath))
                {
                    MessageBox.Show("مسار النسخ الاحتياطي غير محدد في الإعدادات");
                    return;
                }

                if (!Directory.Exists(backupPath))
                {
                    MessageBox.Show("المجلد المحدد للنسخ الاحتياطية غير موجود: " + backupPath);
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
                {
                    MessageBox.Show("لا توجد نسخ احتياطية في المجلد المحدد");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء تحميل النسخ الاحتياطية: " + ex.Message);
            }
        }

 

        private void btnGetFolderBak_Click(object sender, EventArgs e)
        {
            try
            {
                // إنشاء وتكوين حوار اختيار المجلد
                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "اختر مجلد النسخ الاحتياطية";
                    folderDialog.ShowNewFolderButton = true; // السماح بإنشاء مجلد جديد

                    // تعيين المسار الابتدائي إذا كان موجوداً
                    if (!string.IsNullOrEmpty(txtBackupsPath.Text))
                    {
                        folderDialog.SelectedPath = txtBackupsPath.Text;
                    }

                    // عرض الحوار وفحص نتيجة الاختيار
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        // عرض المسار المحدد في مربع النص
                        txtBackupsPath.Text = folderDialog.SelectedPath;

                        // (اختياري) حفظ المسار في ملف الإعدادات
                        //SaveBackupPathToSettings(folderDialog.SelectedPath);
                        SaveData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء اختيار المجلد: " + ex.Message,
                              "خطأ",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }
        private async void btnRestoreBackup_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBoxBackups.SelectedValue == null)
                {
                    MessageBox.Show("يرجى اختيار النسخة الاحتياطية التي ترغب في استرجاعها.");
                    return;
                }

                string settingsPath = Path.Combine(Application.StartupPath, "serverConnectionSettings.txt");
                var helper = new DatabaseBackupRestoreHelper(settingsPath);
                var appSettings = new AppSettings(settingsPath);

                string? dbName = appSettings.GetString("DBName", "");
                if (string.IsNullOrWhiteSpace(dbName))
                {
                    MessageBox.Show("لم يتم العثور على اسم قاعدة البيانات في الإعدادات.");
                    return;
                }

                string? selectedBackupFile = comboBoxBackups.SelectedValue.ToString();
                if (string.IsNullOrWhiteSpace(selectedBackupFile))
                {
                    MessageBox.Show("لم يتم تحديد مسار النسخة الاحتياطية بشكل صحيح.");
                    return;
                }

                var confirmResult = MessageBox.Show(
                    "هل أنت متأكد من أنك تريد استرجاع هذه النسخة؟ سيتم عمل نسخة احتياطية قبل الاسترجاع.",
                    "تأكيد الاسترجاع",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmResult == DialogResult.No)
                    return;

                // 1. إنشاء نسخة احتياطية أولاً
                helper.BackupDatabase(settingsPath);
                MessageBox.Show("تم إنشاء نسخة احتياطية من الوضع الحالي بنجاح.");

                // 2. عرض نافذة انتظار مؤقتة
                frmLoading loadingForm = new frmLoading("جارٍ استرجاع النسخة الاحتياطية، الرجاء الانتظار...");
                loadingForm.Show();
                loadingForm.Refresh();

                // 3. تنفيذ الاسترجاع في مهمة غير متزامنة لتجنب تجميد الواجهة
                await Task.Run(() => helper.RestoreDatabase(dbName, selectedBackupFile));

                // 4. إغلاق نافذة التحميل
                loadingForm.Close();

                MessageBox.Show("تم استرجاع النسخة بنجاح. يُفضل إعادة تشغيل البرنامج.");
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء استرجاع النسخة الاحتياطية:\n" + ex.Message,
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void txtMaxBackups_Leave(object sender, EventArgs e)
        {
            SaveData();
        }
    }
}
