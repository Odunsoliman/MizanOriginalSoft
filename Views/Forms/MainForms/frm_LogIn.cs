using Microsoft.Data.SqlClient;
using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses;
using MizanOriginalSoft.Views.Forms.Products;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace MizanOriginalSoft.Views.Forms.MainForms
{
    public partial class frm_LogIn : Form
    {
        /*
         اهداف الشاشة
        1-جمع بيانات المالك وعرضها
        2- ملئ تلقائى للمستخدميين فى التكست
        3- اختبار المستخدم المختار وفتح الشاشة فى حال صحة المستخدم وكلمة المرور
        4-فتح الازرار وغلقها حسب الصلاحيات
        5-التغذية البصرية لالوان الازرار عند الاستخدام
        6-فتح الشاشات المتعددة من خلال ازرارها




         */


        public frm_LogIn()
        {
            InitializeComponent();
            SetupAutoComplete(); // تهيئة خاصية الإكمال التلقائي لأسماء المستخدمين
            InitializePanelsMovement();
            InitializeInnerPanels();
        }


        private void frm_LogIn_Load(object sender, EventArgs e)
        {
            DBServiecs.A_UpdateAllDataBase();
            LoadAppInfo();         // جلب اسم مالك البرنامج من ملف الإعدادات

             
            txtUserName.Focus();     // وضع التركيز على مربع اسم المستخدم

        }

        #region **********  وظائف الدخول والتحقق من المستخدمين والصلاحيات ***************

        private void LoadAppInfo()
        {
            try
            {
                string filePath = Path.Combine(Application.StartupPath, "serverConnectionSettings.txt");

                if (!File.Exists(filePath))
                {
                    MessageBox.Show("ملف إعدادات الاتصال غير موجود!", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string? ownerName = null;
                DateTime expiryDate = DateTime.MinValue;
                DateTime endDate = DateTime.MinValue;

                foreach (string line in File.ReadAllLines(filePath))
                {
                    if (line.StartsWith("CompanyName="))
                    {
                        ownerName = line.Substring("CompanyName=".Length).Trim();
                    }
                    else if (line.StartsWith("ExpiryDate="))
                    {
                        DateTime.TryParseExact(
                            line.Substring("ExpiryDate=".Length).Trim(),
                            "dd-MM-yyyy",
                            null,
                            System.Globalization.DateTimeStyles.None,
                            out expiryDate
                        );
                    }
                    else if (line.StartsWith("EndDate="))
                    {
                        DateTime.TryParseExact(
                            line.Substring("EndDate=".Length).Trim(),
                            "dd-MM-yyyy",
                            null,
                            System.Globalization.DateTimeStyles.None,
                            out endDate
                        );
                    }
                }

                // عرض اسم المالك
                if (!string.IsNullOrWhiteSpace(ownerName))
                {
                    this.Text = ownerName;
                    lblCo.Text = ownerName;
                }

                // عرض تنبيه انتهاء الصلاحية إذا كنا وصلنا إلى ExpiryDate أو تجاوزناها
                if (expiryDate != DateTime.MinValue && DateTime.Now >= expiryDate && endDate > DateTime.Now)
                {
                    lblExpiryDate.Visible = true;
                    lblExpiryDate.ForeColor = Color.Red;

                    int daysRemaining = (endDate - DateTime.Now).Days;
                    lblExpiryDate.Text = $"لقد أوشكت صلاحية نسختك على الانتهاء. تاريخ الانتهاء: {endDate:dd-MM-yyyy}\n" +
                                         $"باقي على الانتهاء: {daysRemaining} يوم";
                }
                else
                {
                    lblExpiryDate.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء تحميل معلومات التطبيق: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // جدول المستخدمين (يتم تحميله تلقائيًا عند فتح الشاشة)
        private DataTable? tblUsers;

        // جدول التحقق من بيانات المستخدم
        private DataTable? tblUsVarfy;

        // جدول الصلاحيات للمستخدم
        private DataTable? tblPermissions;
        private string password = string.Empty;
        private string username = string.Empty;

        // جلب اسم صاحب البرنامج من ملف الإعدادات وتعيينه على عنوان النموذج والليبل


        // تهيئة خاصية الإكمال التلقائي لمربع اسم المستخدم بناءً على قاعدة البيانات
        private void SetupAutoComplete()
        {
            var autoCompleteCollection = new AutoCompleteStringCollection();

            LoadUsers(); // تحميل المستخدمين من قاعدة البيانات

            if (tblUsers is not null && tblUsers.Rows.Count > 0)
            {
                foreach (DataRow row in tblUsers.Rows)
                {
                    string? username = row["Username"]?.ToString();
                    if (!string.IsNullOrEmpty(username))
                        autoCompleteCollection.Add(username);
                }
            }

            txtUserName.AutoCompleteCustomSource = autoCompleteCollection;
            txtUserName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtUserName.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }

        // تحميل جميع المستخدمين من قاعدة البيانات
        private void LoadUsers()
        {
            tblUsers = DBServiecs.LogIn_GetUsers();
        }

        // التحقق من اسم المستخدم وكلمة المرور
        public void VerifyUser()
        {
            username = txtUserName.Text.Trim();
            password = txtPassword.Text.Trim();

            tblUsVarfy = DBServiecs.LogIn_UsersVarify(username, password);
        }

        // جلب الصلاحيات للمستخدم حسب الـ ID
        public void LoadPermissions(int userId)
        {
            tblPermissions = DBServiecs.LogIn_GetPermissions(userId);
        }

        //// تحديث أرصدة البيانات
        //public string UpdateAllBalances()
        //{
        //    return DBServiecs.UpdateAllBalances();
        //}

        // تغيير كلمة مرور المستخدم
        public string UpdateUserPassword(int userId, string newPassword)
        {
            return DBServiecs.LogIn_UsersUpdatPass(userId, newPassword);
        }
        int us_id;
        bool GoOn = true;
        // زر الدخول 
        private void Login()
        {
            username = txtUserName.Text;
            password = txtPassword.Text;
            if (VerifyCredentials(username, password, out int ID))
            {
                SetPermissions(ID);


            }
            else
            {
                CustomMessageBox.ShowWarning("كلمة المرور  او كلمة السر غير صحيحة.", "خطأ");
            }
            us_id = Convert.ToInt32(lblUserID.Text);
        }
        //التحقق من البيانات
        private bool VerifyCredentials(string username, string password, out int ID)
        {
            ID = -1;

            // استدعاء الدالة للتحقق من بيانات المستخدم
            UsersVaryfy();

            if (tblUsVarfy != null && tblUsVarfy.Rows.Count > 0)
            {
                ID = Convert.ToInt32(tblUsVarfy.Rows[0]["IDUser"]);
                string user_Name = tblUsVarfy.Rows[0]["UserName"]?.ToString() ?? string.Empty;

                us_id = ID;

                // تعيين بيانات الجلسة العامة
                CurrentSession.UserID = us_id;
                CurrentSession.UserName = user_Name;

                return true;
            }

            return false;
        }


        public void UsersVaryfy()
        {
            username = txtUserName.Text.Trim();
            password = txtPassword.Text.Trim();
            tblUsVarfy = DBServiecs.LogIn_UsersVarify(username, password);
        }

        private void SetPermissions(int ID)
        {
            GetPermissions(ID);

            DataTable? tblPerm = tblPermissions;
            if (tblPerm == null || tblPerm.Rows.Count == 0)
            {
                pnlMain.Visible = false;
                CustomMessageBox.ShowWarning("لم يتم منح أي صلاحيات لهذا المستخدم.", "تنبيه");
                return;
            }

            CustomMessageBox.ShowInformation($"اهلا بك يا\n{username}\nوقت ممتع معنا فى ...  برامج ميزان", "دخول ناجح");

            pnlMain.Visible = true;
            btnUserUpdatePass.Enabled = true;
            btnChangUser.Enabled = true;

            if (tblUsVarfy != null && tblUsVarfy.Rows.Count > 0)
                lblUser.Text = Convert.ToString(tblUsVarfy.Rows[0][1]);

            lblUserID.Text = ID.ToString();
            LogFalse();
            btn_MainBill_Click(null, null);

            /*خط احمر تحتها*/
            GoOn = false;

            lblErrors.Visible = ID == 1;

            var allButtons = GetAllControls(this).OfType<Button>();

            foreach (DataRow row in tblPerm.Rows)
            {
                // التأكد من أن اسم الزر ليس فارغًا أو يحتوي على null
                string? buttonName = row["ButtonName"]?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(buttonName))
                    continue;

                // تحويل قيمة السماح إلى Boolean مع التعامل مع احتمالية null
                bool isAllw = false;
                if (row["IsAllowed"] != DBNull.Value)
                {
                    isAllw = Convert.ToBoolean(row["IsAllowed"]);
                }

                // البحث عن الزر بالاسم المحدد
                Control[] controls = this.Controls.Find(buttonName, true);
                if (controls.Length > 0 && controls[0] is Button button)
                {
                    button.Enabled = isAllw;
                }
            }


        }

        public void GetPermissions(int id)
        {
            tblPermissions = DBServiecs.LogIn_GetPermissions(id);
        }

        private void LogFalse()
        {
            tlpUsers .Visible = false;
            txtUserName.Text = "";
            lblUser.Visible = true;
            lblUserID.Visible = true;
        }

        // الصلاحيات لكل مهامة 0-1-2
        private IEnumerable<Control> GetAllControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                yield return control;
                foreach (var child in GetAllControls(control)) // البحث في العناصر الفرعية
                {
                    yield return child;
                }
            }
        }

        //التحكم فى الساعة
        private void tmrTimeToDay_Tick(object sender, EventArgs e)
        {// ضبط الثقافة إلى العربية
            CultureInfo arabicCulture = new CultureInfo("ar-AE");

            // الحصول على اليوم باللغة العربية
            string dayName = DateTime.Now.ToString("dddd", arabicCulture);

            // الحصول على التاريخ باللغة العربية
            string date = DateTime.Now.ToString("d MMMM yyyy", arabicCulture);

            // الحصول على الوقت باللغة العربية
            string time = DateTime.Now.ToString("h:mm tt", arabicCulture);

            // إعداد النص النهائي
            lblTime.Text = $"{dayName} {date} - الساعة {time}";
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (GoOn == true)
                {
                    txtPassword.Text = txtPassword.Text.TrimEnd();
                    e.Handled = true; // منع الصوت الافتراضي للزر Enter

                    Login();
                }
                else
                { }
            }
        }
        private void txtUserName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtUserName.Text = txtUserName.Text.TrimEnd();
                e.IsInputKey = true; // السماح بمعالجة المفتاح Enter
                txtPassword.Focus(); // نقل التركيز إلى txtPassword
            }
        }
        private void txtUserName_Enter(object sender, EventArgs e)
        {
            // تغيير تخطيط لوحة المفاتيح إلى الإنجليزية
            InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new CultureInfo("en-US"));
        }
        #endregion



        #region **********  وظائف حركة البنلز وتغيير الالوان وتمييز الزر ***************

        //وظيفة تحريك البانل الرئيسية***************************
        private Panel? currentPanel = null;
        private Panel? targetPanel = null;
        private int targetHeight = 0;
        private int step = 20; // سرعة التحريك
        private bool isClosing = false;
        private System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();
        private void InitializePanelsMovement()
        {
            animationTimer.Interval = 15;
            animationTimer.Tick += AnimationTimer_Tick;

            // ضبط الارتفاعات الابتدائية
            pnlSetting.Height = pnlSetting.MinimumSize.Height;
            pnlMovement.Height = pnlMovement.MinimumSize.Height;
            pnlBills.Height = pnlBills.MinimumSize.Height;
            pnlReports.Height = pnlReports.MinimumSize.Height;
            pnlMain.Visible = false;
        }
        private void TogglePanel(Panel panel)
        {
            if (animationTimer.Enabled) return;

            if (panel.Height > panel.MinimumSize.Height)
            {
                // البانل مفتوح: اغلقه فقط
                targetPanel = panel;
                targetHeight = panel.MinimumSize.Height;
                isClosing = true;
            }
            else
            {
                // اغلق البانل الحالي إن وجد
                if (currentPanel != null && currentPanel != panel)
                {
                    currentPanel.Height = currentPanel.MinimumSize.Height;
                }

                // افتح البانل المطلوب
                targetPanel = panel;
                targetHeight = panel.MaximumSize.Height;
                isClosing = false;
            }

            currentPanel = targetPanel;
            animationTimer.Start();
        }
        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            if (targetPanel == null)
                return;

            int direction = (targetPanel.Height < targetHeight) ? +1 : -1;
            targetPanel.Height += direction * step;

            if ((direction == 1 && targetPanel.Height >= targetHeight) ||
                (direction == -1 && targetPanel.Height <= targetHeight))
            {
                targetPanel.Height = targetHeight;
                animationTimer.Stop();

                if (isClosing)
                {
                    if (currentPanel == targetPanel)
                        currentPanel = null;
                }
            }
        }


        //وظيفة تحريك البانل الداخلية******************************
        private Panel? currentInnerPanel = null;
        private Panel? targetInnerPanel = null;
        private int innerTargetHeight = 0;
        private bool isInnerClosing = false;
        private int innerStep = 15;
        private System.Windows.Forms.Timer innerTimer = new System.Windows.Forms.Timer();
        private void InitializeInnerPanels()
        {
            innerTimer.Interval = 15;
            innerTimer.Tick += InnerTimer_Tick;

            // اضبط الطول الابتدائي
            pnlMoreSetting.Height = pnlMoreSetting.MinimumSize.Height;
            pnlAccounts.Height = pnlAccounts.MinimumSize.Height;
        }
        private void ToggleInnerPanel(Panel panel)
        {
            if (innerTimer.Enabled) return;

            if (panel.Height > panel.MinimumSize.Height)
            {
                // البانل مفتوح: أغلقه فقط
                targetInnerPanel = panel;
                innerTargetHeight = panel.MinimumSize.Height;
                isInnerClosing = true;
            }
            else
            {
                // أغلق البانل الآخر إن وُجد
                if (currentInnerPanel != null && currentInnerPanel != panel)
                {
                    currentInnerPanel.Height = currentInnerPanel.MinimumSize.Height;
                }

                // افتح البانل المطلوب
                targetInnerPanel = panel;
                innerTargetHeight = panel.MaximumSize.Height;
                isInnerClosing = false;
            }

            currentInnerPanel = targetInnerPanel;
            innerTimer.Start();
        }
        private void InnerTimer_Tick(object? sender, EventArgs e)
        {
            if (targetInnerPanel == null) return;

            int direction = (targetInnerPanel.Height < innerTargetHeight) ? +1 : -1;
            targetInnerPanel.Height += direction * innerStep;

            if ((direction == 1 && targetInnerPanel.Height >= innerTargetHeight) ||
                (direction == -1 && targetInnerPanel.Height <= innerTargetHeight))
            {
                targetInnerPanel.Height = innerTargetHeight;
                innerTimer.Stop();

                if (isInnerClosing)
                {
                    if (currentInnerPanel == targetInnerPanel)
                        currentInnerPanel = null;
                }
            }
        }


        //وظيفة تغير الالوان عند الاستخدام************************************
        private void HighlightTransparentButton(Button clickedButton)
        {

            Font normalFont = new Font(clickedButton.Font.FontFamily, 12F, FontStyle.Bold);  // الخط العادي
            Font highlightedFont = new Font(clickedButton.Font.FontFamily, 14F, FontStyle.Bold);  // الخط المميز

            // دالة داخلية لفحص كل الكنترولات داخل الفورم بشكل متكرر (بما في ذلك البانلات المتداخلة)
            void ResetTransparentButtons(Control parent)
            {
                foreach (Control ctrl in parent.Controls)
                {
                    if (ctrl is Button btn && btn.BackColor == Color.Transparent)
                    {
                        btn.ForeColor = Color.DarkBlue; // الوضع الافتراضي
                        btn.Font = normalFont;

                    }

                    // التحقق من وجود عناصر داخلية أخرى مثل Panel داخل Panel
                    if (ctrl.HasChildren)
                    {
                        ResetTransparentButtons(ctrl);
                    }
                }
            }

            // تطبيق الدالة على الفورم بأكمله
            ResetTransparentButtons(this);

            // تمييز الزر الحالي
            if (clickedButton != null)
            {
                clickedButton.ForeColor = Color.Red;
                clickedButton.Font = highlightedFont;

            }
        }

        #endregion

        #region ******************* وظائف الفتح والاغلاق  ***************************
        private void CloseAllFormsExceptMain()
        {
            Form mainForm = this;

            for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
            {
                Form openForm = Application.OpenForms[i]!;
                if (openForm != mainForm)
                {
                    openForm.Close();
                }
            }
        }

        private void OpenFormInPanel(Form frm)
        {

            // إغلاق النموذج الفرعي الحالي إن وجد
            if (this.panelContainer.Controls.Count > 0)
                this.panelContainer.Controls.RemoveAt(0);

            // إعداد النموذج الفرعي
            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            this.panelContainer.Controls.Add(frm);
            this.panelContainer.Tag = frm;
            frm.Show();
        }
        #endregion

        #region *****************   قائمة الاعدادات  **********************

        //زر رئيسى الاعدادات
        private void btn_MainSetting_Click(object sender, EventArgs e)
        {
            TogglePanel(pnlSetting);
        }
        //زر رئيئسى الاعدادات العامة
        private void btn_MoreSetting_Click(object sender, EventArgs e)
        {
            ToggleInnerPanel(pnlMoreSetting);
        }

        //الاعدادات - الاعدادات العامة
        private void btnProdSetting__Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);

            CloseAllFormsExceptMain();
            int idUser=Convert .ToInt32  (lblUserID.Text) ;
            frmProductItems frm = new frmProductItems(idUser);
            OpenFormInPanel(frm);
        }

        private void btnPermissions_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnGenralData_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
            CloseAllFormsExceptMain();
            frmGenralData frm = new frmGenralData();
            OpenFormInPanel(frm);


        }
        /*انتهاء القائمة الداخلية للاعدادات العامة*/

        // زر رئيئسى اعدادالحسابات العامة *******************************************
        private void btn_Accounts_Click(object sender, EventArgs e)
        {
            ToggleInnerPanel(pnlAccounts);
        }

        //الاعدادات -الحسابات العامة -الازرار التابعة للحسابات
        private void btnCustomers_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnSuppliers_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnPartners_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }
        private void btnCashBox_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);

        }
        private void btnEmployees_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnExpenses_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnFixedAssets_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnDebtors_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnCreditors_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }
        /*انتهاء القائمة الداخلية لاعدادات الحسابات*/

        #endregion

        #region *****************   قائمة الحركة  **********************

        //زر رئيسى لقائمة الحركة
        private void btn_MainMove_Click(object sender, EventArgs e)
        {
            TogglePanel(pnlMovement);
        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnBackSales_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnPrococh_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnBackPrococh_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnGardStock_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnDecreaseStock_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnIncreaseStock_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        #endregion

        #region *****************   قائمة السندات  **********************
        //زر رئيسى لقائمة السندات
        private void btn_MainBill_Click(object? sender, EventArgs? e)
        {
            TogglePanel(pnlBills);
        }

        private void btnCashOut_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnCashIn_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnChequeBatch_In_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnChequeBatch_Out_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnDebitSettlement_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnCreditSettlement_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        #endregion

        #region *****************   قائمة التقارير  **********************
        //زر رئيسى لقائمة التقارير
        private void btn_MainReports_Click(object sender, EventArgs e)
        {
            TogglePanel(pnlReports);
        }

        private void btnRep01_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnRep02_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnRep03_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnRep04_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnRep05_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnRep06_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnRep07_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnRep08_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnRep09_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }

        private void btnRep10_Click(object sender, EventArgs e)
        {
            HighlightTransparentButton((Button)sender);
        }


        #endregion



        #region **************** End *************************

        /*هل سيحتاج هذا الزر اى تعديل */
        private void btnEnd_Click_(object sender, EventArgs e)
        {
            try
            {
                string settingsPath = Path.Combine(Application.StartupPath, "serverConnectionSettings.txt");

                if (!File.Exists(settingsPath))
                {
                    MessageBox.Show("❌ ملف إعدادات الاتصال غير موجود!", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var appSettings = new AppSettings(settingsPath);
                var helper = new DatabaseBackupRestoreHelper(settingsPath);

                // 1. النسخ الاحتياطي
                helper.BackupDatabase(settingsPath);

                // 2. تنظيف النسخ القديمة
                string? backupFolder = appSettings.GetString("BackupsPath", null);

                if (!string.IsNullOrWhiteSpace(backupFolder))
                {
                    helper.CleanOldBackups(backupFolder, settingsPath);

                    // 3. نسخ النسخة الأخيرة إلى مجلد مشترك باسم ثابت
                    helper.CopyLatestBackupToSharedFolder(
                        sourceBackupFolder: backupFolder,
                        sharedFolderPath: @"D:\BackupToPush", // يمكنك أيضًا قراءته من الإعدادات لو أردت
                        outputFileName: "MizanOriginalDB.bak"
                    );
                }

                // 4. تنفيذ Git Push لمجلد المشروع (من ملف الإعدادات)
                string? projectPath = appSettings.GetString("ProjectPath", null);
                if (!string.IsNullOrWhiteSpace(projectPath))
                {
                    ExecuteGitPush(projectPath);
                }

                // 5. تنفيذ Git Push لمجلد نسخ القواعد (من ملف الإعدادات)
                string? backupPushPath = appSettings.GetString("BackupGitPath", null);
                if (!string.IsNullOrWhiteSpace(backupPushPath))
                {
                    ExecuteGitPush(backupPushPath);
                }

                // 6. تحديث بيانات القاعدة
                DBServiecs.A_UpdateAllDataBase();

                // 7. إنهاء البرنامج
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ حدث خطأ أثناء إنهاء التطبيق: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            try
            {
                string settingsPath = Path.Combine(Application.StartupPath, "serverConnectionSettings.txt");

                if (!File.Exists(settingsPath))
                {
                    MessageBox.Show("❌ ملف إعدادات الاتصال غير موجود!", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var appSettings = new AppSettings(settingsPath);
                var helper = new DatabaseBackupRestoreHelper(settingsPath);

                // 1. النسخ الاحتياطي
                helper.BackupDatabase(settingsPath);

                // 2. تنظيف النسخ القديمة
                string? backupFolder = appSettings.GetString("BackupsPath", null);
                string? dbName = appSettings.GetString("DBName", null);

                if (!string.IsNullOrWhiteSpace(backupFolder))
                {
                    helper.CleanOldBackups(backupFolder, settingsPath);

                    // 3. نسخ النسخة الأخيرة إلى مجلد مشترك باسم ثابت
                    helper.CopyLatestBackupToSharedFolder(
                        sourceBackupFolder: backupFolder,
                        sharedFolderPath: @"D:\BackupToPush", // أو من ملف الإعدادات إن أردت
                        outputFileName: "MizanOriginalDB.bak"
                    );
                }

                // 4. نسخ النسخة إلى Google Drive
                string? googleDrivePath = appSettings.GetString("GoogleDrivePath", @"D:\ClintGoogleDrive");
                if (!string.IsNullOrWhiteSpace(backupFolder) &&
                    !string.IsNullOrWhiteSpace(dbName) &&
                    !string.IsNullOrWhiteSpace(googleDrivePath))
                {
                    helper.CopyBackupToGoogleDrive(
                        sourceFolder: backupFolder,
                        googleDriveFolder: googleDrivePath,
                        dbName: dbName
                    );
                }

                // 5. تنفيذ Git Push لمجلد المشروع (من ملف الإعدادات)
                string? projectPath = appSettings.GetString("ProjectPath", null);
                if (!string.IsNullOrWhiteSpace(projectPath))
                {
                    ExecuteGitPush(projectPath);
                }

                // 6. تنفيذ Git Push لمجلد نسخ القواعد (من ملف الإعدادات)
                string? backupPushPath = appSettings.GetString("BackupGitPath", null);
                if (!string.IsNullOrWhiteSpace(backupPushPath))
                {
                    ExecuteGitPush(backupPushPath);
                }

                // 7. تحديث بيانات القاعدة
                DBServiecs.A_UpdateAllDataBase();

                // 8. إنهاء البرنامج
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ حدث خطأ أثناء إنهاء التطبيق: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExecuteGitPush(string workingDirectory)
        {
            try
            {
                var gitProcess = new System.Diagnostics.Process();
                gitProcess.StartInfo.FileName = "cmd.exe";
                gitProcess.StartInfo.Arguments = "/C git add . && git commit -m \"Auto Backup - " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\" && git push";
                gitProcess.StartInfo.WorkingDirectory = workingDirectory;
                gitProcess.StartInfo.CreateNoWindow = true;
                gitProcess.StartInfo.UseShellExecute = false;
                gitProcess.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ خطأ أثناء تنفيذ Git Push:\n" + ex.Message);
            }
        }


        private void btnUserUpdatePass_Click(object sender, EventArgs e)
        {
            // المتغيرات لتخزين كلمات المرور
            string newPassword;

            // طلب من المستخدم إدخال كلمة المرور القديمة باستخدام ShowPasswordBox
            bool isOldPasswordCorrect = CustomMessageBox.ShowPasswordBox(password);

            // التحقق مما إذا كانت كلمة المرور القديمة قد تم إدخالها بنجاح
            if (isOldPasswordCorrect)
            {
                // طلب إدخال الكلمة الجديدة باستخدام ShowNewPasswordBox
                if (CustomMessageBox.ShowNewPasswordBox(out newPassword, "من فضلك ادخل الكلمة الجديدة"))
                {
                    // طلب من المستخدم إدخال الكلمة الجديدة مرة أخرى للتأكيد
                    string confirmPassword;
                    if (CustomMessageBox.ShowNewPasswordBox(out confirmPassword, "من فضلك أعد إدخال الكلمة الجديدة للتأكيد"))
                    {
                        // التحقق مما إذا كانت كلمة المرور الجديدة وتأكيدها متطابقين
                        if (newPassword == confirmPassword)
                        {
                            // تحديث المستخدم بكلمة المرور الجديدة
                            DBServiecs.LogIn_UsersUpdatPass(us_id, newPassword);
                            CustomMessageBox.ShowInformation("تم تحديث كلمة المرور بنجاح", "نجاح");
                            pnlMain.Visible = false;
                            btnChangUser_Click(null, null);
                        }
                        else
                        {
                            CustomMessageBox.ShowWarning("كلمة المرور الجديدة غير متطابقة. يرجى المحاولة مرة أخرى.", "خطأ");
                        }
                    }
                }
            }
            else
            {
                CustomMessageBox.ShowWarning("كلمة المرور القديمة غير صحيحة.", "خطأ");
            }
        }

        //زر تغيير المستخدم
        private void btnChangUser_Click(object? sender, EventArgs? e)
        {
            CloseAllFormsExceptMain();
            tlpUsers.Visible = true;
            lblUser.Text = "";
            pnlMain.Visible = false;
            SetupAutoComplete();
            btnUserUpdatePass.Enabled = false;
            btnChangUser.Enabled = false;
            GoOn = true;
            lblUserID.Text = "0";
            txtUserName.Focus();
            txtPassword.Text = "";
            lblErrors.Visible = false;

            CustomMessageBox.ShowInformation($"لقد تمت مغادرتك يا\n{username}\nنسعد بعودتك الينا لاحقا فى برامج ميزان ...فى امان الله ", "خروج");

        }

        private void lblErrors_Click(object sender, EventArgs e)
        {

        }

        #endregion

    }


}
