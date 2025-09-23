using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses.InvoicClasses;
using MizanOriginalSoft.Views.Forms.Accounts;
using MizanOriginalSoft.Views.Forms.Movments;
using MizanOriginalSoft.Views.Forms.Products;
using MizanOriginalSoft.Views.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace MizanOriginalSoft.Views.Forms.MainForms
{
    public partial class frmMainLogIn : Form
    {
        private Panel? currentPanel = null;
        private Panel? targetPanel = null;
        private int targetHeight = 0;
        private bool isClosing = false;
        private readonly System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer innerTimer = new System.Windows.Forms.Timer();

        private Panel? currentInnerPanel = null;
        private Panel? targetInnerPanel = null;
        private int innerTargetHeight = 0;
        private bool isInnerClosing = false;

        private Dictionary<string, UserPermissionInfo> userPermissions = new();
        private DataTable? tblUsers;
        private DataTable? tblUsVarfy;
        private DataTable? tblPermissions;

        private string username = string.Empty;
        private string password = string.Empty;
        private int us_id;
        private bool GoOn = true;

        public frmMainLogIn()
        {
            InitializeComponent();
            InitializePanelsMovement();
            InitializeInnerPanels();
            ConnectButtonEvents();
        }

        private void frmMainLogIn_Load(object sender, EventArgs e)
        {
            DBServiecs.A_UpdateAllDataBase();
            LoadAppInfo();
            SetupAutoComplete();
            txtUserName.Focus();
            LoadReports(0);
        }

        private void LoadAppInfo()
        {
            try
            { /*
               قمت هنا بتغيير اسم المفتاح فى ملف التكست الى ThisVersionIsForWarehouseId
                ولكن الدالة  var dt = DBServiecs.Warehouse_GetAll(); تحضر من القاعدة اسم العمود WarehouseId
                فاريد عند قرائة ThisVersionIsForWarehouseId من ملف التكست مساوات قيمته الرقمية ب WarehouseId حتى يحضر اسم الفرع بشكل صحيح
               */
                string filePath = Path.Combine(Application.StartupPath, "serverConnectionSettings.txt");
                CurrentSession.LoadServerSettings(filePath);

                if (CurrentSession.WarehouseId <= 0)
                {
                    MessageBox.Show("⚠️ لم يتم تعيين رقم فرع صحيح في ملف الإعدادات.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }

                this.Text = lblCo.Text = CurrentSession.CompanyName;

                var dt = DBServiecs.Warehouse_GetAll();
                var found = dt?.AsEnumerable().FirstOrDefault(r => r.Field<int>("WarehouseId") == CurrentSession.WarehouseId);
                lblWarehouse.Text = found != null
                    ? $"🔹 الفرع: {found.Field<string>("WarehouseName")} (رقم {CurrentSession.WarehouseId})"
                    : $"❌ لم يتم العثور على الفرع رقم {CurrentSession.WarehouseId}";

                if (CurrentSession.ExpiryDate.HasValue && DateTime.Now >= CurrentSession.ExpiryDate &&
                    CurrentSession.EndDate.HasValue && DateTime.Now < CurrentSession.EndDate)
                {
                    int daysLeft = (CurrentSession.EndDate.Value - DateTime.Now).Days;
                    lblExpiryDate.Visible = true;
                    lblExpiryDate.ForeColor = Color.Red;
                    lblExpiryDate.Text = $"لقد أوشكت صلاحية نسختك على الانتهاء. تاريخ الانتهاء: {CurrentSession.EndDate:dd-MM-yyyy}\nباقي: {daysLeft} يوم";
                }
                else
                {
                    lblExpiryDate.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء تحميل إعدادات التطبيق:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void SetupAutoComplete()
        {
            txtUserName.AutoCompleteCustomSource.Clear();
            LoadUsers();

            if (tblUsers is not null)
            {
                var usernames = tblUsers.AsEnumerable()
                    .Select(row => row.Field<string>("Username"))
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Cast<string>() // هذا يحول string? إلى string (لأن Where أزال القيم null)
                    .ToArray();

                txtUserName.AutoCompleteCustomSource.AddRange(usernames);
            }

            txtUserName.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtUserName.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }

        private void LoadUsers() => tblUsers = DBServiecs.User_GetActiv();

        /*تم تحديث الصلاحيات وربطها بصلاحيات المستخدم فى الفرع الحالى  فلابد من تعديل الكود التالى*/
        public void GetPermissions(int userId)
        {
            int warehouseId = CurrentSession.WarehouseId;
            tblPermissions = DBServiecs.Permission_GetByUser(userId, warehouseId);
        }

        private void SetPermissions(int userId)
        {

            // ✅ جلب صلاحيات المستخدم للفرع الحالي من CurrentSession
            int warehouseId = CurrentSession.WarehouseId;
            GetPermissions(userId); // تستخدم CurrentSession.WarehouseID تلقائيًا بداخلها


            // التحقق من وجود صلاحيات
            if (tblPermissions == null || tblPermissions.Rows.Count == 0)
            {
                pnlMain.Visible = false;
                CustomMessageBox.ShowWarning("لم يتم منح أي صلاحيات لهذا المستخدم.", "تنبيه");
                return;
            }

            // عرض رسالة ترحيبية
            CustomMessageBox.ShowInformation(
                $"أهلاً بك يا\n{CurrentSession.UserName}\nوقت ممتع معنا في ... برامج ميزان", "دخول ناجح");

            // تفعيل اللوحة الرئيسية وزر تغيير كلمة المرور
            pnlMain.Visible = true;
            btnUserUpdatePass.Enabled = true;
            btnChangUser.Enabled = true;

            lblUser.Text = CurrentSession.UserName;
            lblUserID.Text = userId.ToString();

            LogFalse();
            btn_MainBill_Click(null, null);
            GoOn = false;

            lblMasterRerpots.Visible = (userId == 1);

            // تفريغ القاموس الحالي
            userPermissions.Clear();

            // الحصول على جميع الكائنات داخل الفورم
            var allControls = GetAllControls(this);

            // تمر على كل صلاحية
            foreach (DataRow row in tblPermissions.Rows)
            {
                string? controlName = row["PermissionName"]?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(controlName)) continue;

                var permission = new UserPermissionInfo
                {
                    CanView = ConvertToBool(row["IsAllowed"]),
                    CanAdd = ConvertToBool(row["CanAdd"]),
                    CanEdit = ConvertToBool(row["CanEdit"]),
                    CanDelete = ConvertToBool(row["CanDelete"])
                };

                // حفظها في القاموس العام
                userPermissions[controlName] = permission;

                // البحث عن الكائن المطابق داخل الفورم
                var ctrl = allControls.FirstOrDefault(c => c.Name == controlName);
                if (ctrl != null)
                {
                    ctrl.Visible = permission.CanView;
                    ctrl.Enabled = permission.CanView;
                }
            }
        }


        private List<Control> GetAllControls(Control parent)
        {
            List<Control> controls = new List<Control>();
            foreach (Control control in parent.Controls)
            {
                controls.Add(control);
                controls.AddRange(GetAllControls(control));
            }
            return controls;
        }


        private bool ConvertToBool(object value)
        {
            return value != null && value != DBNull.Value && Convert.ToBoolean(value);
        }


        public UserPermissionInfo? GetPermissionFor(string buttonName) =>
            userPermissions.TryGetValue(buttonName, out var perm) ? perm : null;

        private void Login()
        {
            username = txtUserName.Text.Trim();
            password = txtPassword.Text.Trim();

            if (VerifyCredentials(username, password, out int ID))
                SetPermissions(ID);
            else
                CustomMessageBox.ShowWarning("كلمة المرور  او كلمة السر غير صحيحة.", "خطأ");

            us_id = Convert.ToInt32(lblUserID.Text);
        }

        private bool VerifyCredentials(string username, string password, out int ID)
        {
            ID = -1;
            UsersVaryfy();

            if (tblUsVarfy != null && tblUsVarfy.Rows.Count > 0)
            {
                ID = Convert.ToInt32(tblUsVarfy.Rows[0]["IDUser"]);
                string user_Name = tblUsVarfy.Rows[0]["UserName"]?.ToString() ?? string.Empty;

                us_id = ID;
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
            tblUsVarfy = DBServiecs.User_Varify(username, password);
        }

        //private IEnumerable<Control> GetAllControls(Control parent)
        //{
        //    foreach (Control control in parent.Controls)
        //    {
        //        yield return control;
        //        foreach (var child in GetAllControls(control))
        //            yield return child;
        //    }
        //}

        private void LogFalse()
        {
            tlpUsers.Visible = false;
            txtUserName.Clear();
            lblUser.Visible = true;
            lblUserID.Visible = true;
        }

        // حركات البانل (نفس الكود الموجود)
        private void InitializePanelsMovement()
        {
            animationTimer.Interval = 15;
            animationTimer.Tick += AnimationTimer_Tick;
            pnlSetting.Height = pnlSetting.MinimumSize.Height;
            pnlMovement.Height = pnlMovement.MinimumSize.Height;
            pnlBills.Height = pnlBills.MinimumSize.Height;
            pnlReports.Height = pnlReports.MinimumSize.Height;
            pnlMain.Visible = false;
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            if (targetPanel == null) return;
            int direction = (targetPanel.Height < targetHeight) ? +1 : -1;
            targetPanel.Height += direction * 20;
            if ((direction == 1 && targetPanel.Height >= targetHeight) || (direction == -1 && targetPanel.Height <= targetHeight))
            {
                targetPanel.Height = targetHeight;
                animationTimer.Stop();
                if (isClosing && currentPanel == targetPanel)
                    currentPanel = null;
            }
        }

        private void InitializeInnerPanels()
        {
            innerTimer.Interval = 15;
            innerTimer.Tick += InnerTimer_Tick;
            pnlMoreSetting.Height = pnlMoreSetting.MinimumSize.Height;
            pnlAccounts.Height = pnlAccounts.MinimumSize.Height;
        }

        private void InnerTimer_Tick(object? sender, EventArgs e)
        {
            if (targetInnerPanel == null) return;
            int direction = (targetInnerPanel.Height < innerTargetHeight) ? +1 : -1;
            targetInnerPanel.Height += direction * 15;
            if ((direction == 1 && targetInnerPanel.Height >= innerTargetHeight) || (direction == -1 && targetInnerPanel.Height <= innerTargetHeight))
            {
                targetInnerPanel.Height = innerTargetHeight;
                innerTimer.Stop();
                if (isInnerClosing && currentInnerPanel == targetInnerPanel)
                    currentInnerPanel = null;
            }
        }



        #region اختصارات وتحسينات واجهة الدخول

        private void txtUserName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)

        {
            if (e.KeyCode == Keys.Enter)
            {
                txtUserName.Text = txtUserName.Text.TrimEnd();
                e.IsInputKey = true;
                txtPassword.Focus();
            }
        }

        private void txtUserName_Enter(object sender, EventArgs e)
        {
            InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new CultureInfo("en-US"));
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && GoOn)
            {
                txtPassword.Text = txtPassword.Text.TrimEnd();
                e.Handled = true;
                Login();
            }
        }

        #endregion

        #region تغيير ألوان الأزرار الشفافة

        private void HighlightTransparentButton(Button clickedButton)
        {
            Font normalFont = new Font(clickedButton.Font.FontFamily, 12F, FontStyle.Bold);
            Font highlightedFont = new Font(clickedButton.Font.FontFamily, 14F, FontStyle.Bold);

            void ResetTransparentButtons(Control parent)
            {
                foreach (Control ctrl in parent.Controls)
                {
                    if (ctrl is Button btn && btn.BackColor == Color.Transparent)
                    {
                        btn.ForeColor = Color.DarkBlue;
                        btn.Font = normalFont;
                    }

                    if (ctrl.HasChildren)
                        ResetTransparentButtons(ctrl);
                }
            }

            ResetTransparentButtons(this);

            if (clickedButton != null)
            {
                clickedButton.ForeColor = Color.Red;
                clickedButton.Font = highlightedFont;
            }
        }

        #endregion

        #region وظائف التحكم في إظهار وإخفاء البانلز

        private void ToggleInnerPanel(Panel panel)
        {
            if (innerTimer.Enabled) return;

            if (panel.Height > panel.MinimumSize.Height)
            {
                targetInnerPanel = panel;
                innerTargetHeight = panel.MinimumSize.Height;
                isInnerClosing = true;
            }
            else
            {
                if (currentInnerPanel != null && currentInnerPanel != panel)
                    currentInnerPanel.Height = currentInnerPanel.MinimumSize.Height;

                targetInnerPanel = panel;
                innerTargetHeight = panel.MaximumSize.Height;
                isInnerClosing = false;
            }

            currentInnerPanel = targetInnerPanel;
            innerTimer.Start();
        }

        private void TogglePanel(Panel panel)
        {
            if (animationTimer.Enabled) return;

            if (panel.Height > panel.MinimumSize.Height)
            {
                targetPanel = panel;
                targetHeight = panel.MinimumSize.Height;
                isClosing = true;
            }
            else
            {
                if (currentPanel != null && currentPanel != panel)
                    currentPanel.Height = currentPanel.MinimumSize.Height;

                targetPanel = panel;
                targetHeight = panel.MaximumSize.Height;
                isClosing = false;
            }

            currentPanel = targetPanel;
            animationTimer.Start();
        }

        private void CloseAllFormsExceptMain()
        {
            foreach (Form openForm in Application.OpenForms.Cast<Form>().ToList())
            {
                if (openForm != this)
                    openForm.Close();
            }
        }

        private void OpenFormInPanel(Form frm)
        {
            if (panelContainer.Controls.Count > 0)
                panelContainer.Controls.RemoveAt(0);

            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            panelContainer.Controls.Add(frm);
            panelContainer.Tag = frm;
            frm.Show();
        }

        #endregion

        #region ربط كل الأزرار بالأحداث الخاصة بها

        private void ConnectButtonEvents()
        {
            var buttonEventMap = new Dictionary<Button, EventHandler>
            {
                [btnGenralData] = btnGenralData_Click,
                [btnProdSetting_] = btnProdSetting__Click,
                [btn_MoreSetting] = btn_MoreSetting_Click,
                [btn_MainSetting] = btn_MainSetting_Click,

                [btnCreditors] = btnCreditors_Click,
                [btnDebtors] = btnDebtors_Click,
                [btnFixedAssets] = btnFixedAssets_Click,
                [btnExpenses] = btnExpenses_Click,
                [btnEmployees] = btnEmployees_Click,
                [btnCashBox] = btnCashBox_Click,
                [btnPartners] = btnPartners_Click,
                [btnSuppliers] = btnSuppliers_Click,
                [btnCustomers] = btnCustomers_Click,
                [btn_Accounts] = btn_Accounts_Click,

                [btnIncreaseStock] = btnIncreaseStock_Click,
                [btnDecreaseStock] = btnDecreaseStock_Click,
                [btnGardStock] = btnGardStock_Click,
                [btnBackPrococh] = btnBackPrococh_Click,
                [btnPrococh] = btnPrococh_Click,
                [btnBackSales] = btnBackSales_Click,
                [btnSales] = btnSales_Click,
                [btn_MainMove] = btn_MainMove_Click,

                [btnCreditSettlement] = btnCreditSettlement_Click,
                [btnDebitSettlement] = btnDebitSettlement_Click,
                [btnChequeBatch_In] = btnChequeBatch_In_Click,
                [btnChequeBatch_Out] = btnChequeBatch_Out_Click,
                [btnCashIn] = btnCashIn_Click,
                [btnCashOut] = btnCashOut_Click,
                [btn_MainBill] = btn_MainBill_Click,

                [btnUserUpdatePass] = btnUserUpdatePass_Click,
                [btnChangUser] = btnChangUser_Click,


                [btn_MainReports] = btn_MainReports_Click
            };

            foreach (var pair in buttonEventMap)
                pair.Key.Click += pair.Value;
        }

        #endregion


        #region *****************   قائمة الاعدادات  **********************

        //زر رئيسى الاعدادات
        private void btn_MainSetting_Click(object? sender, EventArgs e)
        {
            TogglePanel(pnlSetting);
        }

        //زر رئيئسى الاعدادات الاخرى
        private void btn_MoreSetting_Click(object? sender, EventArgs e)
        {
            ToggleInnerPanel(pnlMoreSetting);
        }

        //الاعدادات - الاعدادات للاصناف
        private void btnProdSetting__Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn);
            }

            CloseAllFormsExceptMain();
            frmProductItems frm = new frmProductItems();
            OpenFormInPanel(frm);
        }
        /*
         * المفتاح السابق هو من يفتح الشاشة المعنية ولاحظت ان فى بداية فتح الشاشة يكون بها فراغ فى الاعلا بسيط ثن يزداد كلما نقرت على عقدة كلى الشجرة
         * 
         * فهل نظام هذا الفتح له تأثير
         *        private void CloseAllFormsExceptMain()
        {
            foreach (Form openForm in Application.OpenForms.Cast<Form>().ToList())
            {
                if (openForm != this)
                    openForm.Close();
            }
        }

                 private void OpenFormInPanel(Form frm)
        {
            if (panelContainer.Controls.Count > 0)
                panelContainer.Controls.RemoveAt(0);

            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            panelContainer.Controls.Add(frm);
            panelContainer.Tag = frm;
            frm.Show();
        }
         */
        //الاعدادات - الاعدادات العامة
        private void btnGenralData_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn);
            }
            CloseAllFormsExceptMain();
            frmGenralData frm = new frmGenralData();
            OpenFormInPanel(frm);


        }

        #endregion 

        #region  ******   اعدادالحسابات  *****************************

        //زر رئيئسى اعدادالحسابات العامة
        private void btn_Accounts_Click(object? sender, EventArgs e)
        {
            ToggleInnerPanel(pnlAccounts);
        }

        //الاعدادات -العملاء
        private void btnCustomers_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn);
            }
            int TopID = 3;
            frmMainAccounts frm = new frmMainAccounts(TopID);
            OpenFormInPanel(frm);
        }

        //الاعدادات -الموردين
        private void btnSuppliers_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn);
            }
            int TopID = 4;
            frmMainAccounts frm = new frmMainAccounts(TopID);
            OpenFormInPanel(frm);

        }

        //الاعدادات -جاري الشركاء
        private void btnPartners_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn);
            }
            int TopID = 40;
            frmMainAccounts frm = new frmMainAccounts(TopID);
            OpenFormInPanel(frm);

        }

        //الاعدادات -النقدية
        private void btnCashBox_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn);
            }
            int TopID = 18;
            frmMainAccounts frm = new frmMainAccounts(TopID);
            OpenFormInPanel(frm);

        }

        //الاعدادات -العاملون
        private void btnEmployees_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn);
            }
            int TopID = 8;
            frmMainAccounts frm = new frmMainAccounts(TopID);
            OpenFormInPanel(frm);
        }

        //الاعدادات -المصروفات
        private void btnExpenses_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn);
            }
            int TopID = 6;
            frmMainAccounts frm = new frmMainAccounts(TopID);
            OpenFormInPanel(frm);
        }

        //الاعدادات -الاصول الثابتة
        private void btnFixedAssets_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn);
            }
            int TopID = 21;
            frmMainAccounts frm = new frmMainAccounts(TopID);
            OpenFormInPanel(frm);
        }

        //الاعدادات -المدينين
        private void btnDebtors_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn);
            }
            int TopID = 52;
            frmMainAccounts frm = new frmMainAccounts(TopID);
            OpenFormInPanel(frm);
        }

        //الاعدادات -الدائنون
        private void btnCreditors_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn);
            }
            int TopID = 53;
            frmMainAccounts frm = new frmMainAccounts(TopID);
            OpenFormInPanel(frm);
        }

        #endregion

        #region *****************   قائمة الحركة  **********************

        //زر رئيسى لقائمة الحركة
        private void btn_MainMove_Click(object? sender, EventArgs e)
        {
            TogglePanel(pnlMovement);
        }

        // 🔹 فاتورة البيع
        private void btnSales_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn); // إبراز الزر
            }

            CloseAllFormsExceptMain(); // غلق كل الفورمز المفتوحة ما عدا الرئيسي

            // تحديد نوع الفاتورة الجديد باستخدام enum
            InvoiceType type = InvoiceType.Sale;

            // إنشاء الفورم الذكي وتهيئته
            frm_DynamicInvoice frm = new frm_DynamicInvoice();
            frm.InitializeInvoice(type); // هنا نمرر النوع بدلاً من رقم

            OpenFormInPanel(frm); // فتح الفورم داخل الـ Panel
        }

        // 🔹 فاتورة البيع المرتد
        private void btnBackSales_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn); // إبراز الزر
            }

            CloseAllFormsExceptMain(); // غلق كل الفورمز المفتوحة ما عدا الرئيسي

            // تحديد نوع الفاتورة الجديد باستخدام enum
            InvoiceType type = InvoiceType.SaleReturn;

            // إنشاء الفورم الذكي وتهيئته
            frm_DynamicInvoice frm = new frm_DynamicInvoice();
            frm.InitializeInvoice(type); // هنا نمرر النوع بدلاً من رقم

            OpenFormInPanel(frm); // فتح الفورم داخل الـ Panel




        }

        // 🔹 فاتورة المشتريات
        private void btnPrococh_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn); // إبراز الزر
            }

            CloseAllFormsExceptMain(); // غلق كل الفورمز المفتوحة ما عدا الرئيسي

            // تحديد نوع الفاتورة الجديد باستخدام enum
            InvoiceType type = InvoiceType.Purchase;

            // إنشاء الفورم الذكي وتهيئته
            frm_DynamicInvoice frm = new frm_DynamicInvoice();
            frm.InitializeInvoice(type); // هنا نمرر النوع بدلاً من رقم

            OpenFormInPanel(frm); // فتح الفورم داخل الـ Panel

        }

        // 🔹 فاتورة المشتريات المرتدة
        private void btnBackPrococh_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn); // إبراز الزر
            }

            CloseAllFormsExceptMain(); // غلق كل الفورمز المفتوحة ما عدا الرئيسي

            // تحديد نوع الفاتورة الجديد باستخدام enum
            InvoiceType type = InvoiceType.PurchaseReturn;

            // إنشاء الفورم الذكي وتهيئته
            frm_DynamicInvoice frm = new frm_DynamicInvoice();
            frm.InitializeInvoice(type); // هنا نمرر النوع بدلاً من رقم

            OpenFormInPanel(frm); // فتح الفورم داخل الـ Panel

        }

        // 🔹 اذن الجرد العام
        private void btnGardStock_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn); // إبراز الزر
            }

            CloseAllFormsExceptMain(); // غلق كل الفورمز المفتوحة ما عدا الرئيسي

            // تحديد نوع الفاتورة الجديد باستخدام enum
            InvoiceType type = InvoiceType.Inventory ;

            // إنشاء الفورم الذكي وتهيئته
            frm_DynamicInvoice frm = new frm_DynamicInvoice();
            frm.InitializeInvoice(type); // هنا نمرر النوع بدلاً من رقم

            OpenFormInPanel(frm); // فتح الفورم داخل الـ Panel
        }

        // 🔹 اذن خصم رصيد صنف
        private void btnDecreaseStock_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn); // إبراز الزر
            }

            CloseAllFormsExceptMain(); // غلق كل الفورمز المفتوحة ما عدا الرئيسي

            // تحديد نوع الفاتورة الجديد باستخدام enum
            InvoiceType type = InvoiceType.DeductStock ;

            // إنشاء الفورم الذكي وتهيئته
            frm_DynamicInvoice frm = new frm_DynamicInvoice();
            frm.InitializeInvoice(type); // هنا نمرر النوع بدلاً من رقم

            OpenFormInPanel(frm); // فتح الفورم داخل الـ Panel
        }

        // 🔹 اذن اضافة رصيد صنف
        private void btnIncreaseStock_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn); // إبراز الزر
            }

            CloseAllFormsExceptMain(); // غلق كل الفورمز المفتوحة ما عدا الرئيسي

            // تحديد نوع الفاتورة الجديد باستخدام enum
            InvoiceType type = InvoiceType.AddStock ;

            // إنشاء الفورم الذكي وتهيئته
            frm_DynamicInvoice frm = new frm_DynamicInvoice();
            frm.InitializeInvoice(type); // هنا نمرر النوع بدلاً من رقم

            OpenFormInPanel(frm); // فتح الفورم داخل الـ Panel
        }

        // 🔹 نقطة بيع الكاشير
        private void btnPOS_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn);
            }
            CloseAllFormsExceptMain();
            frmPOS frm = new frmPOS();
            OpenFormInPanel(frm);
        }
        #endregion

        #region *****************   قائمة السندات  **********************
        //زر رئيسى لقائمة السندات
        private void btn_MainBill_Click(object? sender, EventArgs? e)
        {
            TogglePanel(pnlBills);
        }
        /*
            Receipt = 9,  تحصيل
            Payment = 8,  صرف
            Dept = 13,    تسوية مدينة
            Credet = 14   تسوية دائنة
         */
        private void btnCashOut_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn);
            }
            CloseAllFormsExceptMain();
            int typMov = 8;
            frmCashTransaction frm = new frmCashTransaction(typMov);
            OpenFormInPanel(frm);
        }

        private void btnCashIn_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn);
            }
            CloseAllFormsExceptMain();
            int typMov = 9;
            frmCashTransaction frm = new frmCashTransaction(typMov);
            OpenFormInPanel(frm);
        }

        private void btnChequeBatch_In_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn);
            }

            CloseAllFormsExceptMain();

            // استخدم الـ Enum مباشرة
            frmBatcheCheques frm = new frmBatcheCheques(frmBatcheCheques.TransactionType.BatchIn);

            OpenFormInPanel(frm);
        }

        private void btnChequeBatch_Out_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn);
            }

            CloseAllFormsExceptMain();

            // استخدم الـ Enum مباشرة
            frmBatcheCheques frm = new frmBatcheCheques(frmBatcheCheques.TransactionType.BatchOut);

            OpenFormInPanel(frm);
        }

        private void btnDebitSettlement_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn);
            }
            CloseAllFormsExceptMain();
            int typMov = 13;
            frmCashTransaction frm = new frmCashTransaction(typMov);
            OpenFormInPanel(frm);
        }

        private void btnCreditSettlement_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn);
            }
            CloseAllFormsExceptMain();
            int typMov = 14;
            frmCashTransaction frm = new frmCashTransaction(typMov);
            OpenFormInPanel(frm);
        }

        #endregion

        #region *****************   قائمة التقارير  **********************
        //زر رئيسى لقائمة التقارير
        private void btn_MainReports_Click(object? sender, EventArgs e)
        {
            TogglePanel(pnlReports);
        }

        private void LoadReports(int topAcc)
        {
            DataTable dt = DBServiecs.Reports_GetByTopAcc(topAcc, false);

            DGV.DataSource = dt;

            DGV.RowHeadersVisible = false;
            DGV.ReadOnly = true;
            DGV.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);

            // إخفاء كل الأعمدة ما عدا ReportDisplayName
            foreach (DataGridViewColumn col in DGV.Columns)
            {
                col.Visible = (col.Name == "ReportDisplayName");
            }

            DGV.ClearSelection();

            DGV.DefaultCellStyle.Font = new Font("Times New Roman", 14, FontStyle.Bold);
            DGV.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Bold);
            DGV.DefaultCellStyle.ForeColor = Color.Black;

            if (DGV.Columns.Contains("ReportDisplayName"))
            {
                DGV.Columns["ReportDisplayName"].HeaderText = "اسم التقرير";
                DGV.Columns["ReportDisplayName"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }

            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // تعيين العمود ليملأ عرض الشبكة بالكامل
            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }


        #endregion

        #region **************** End *************************

        /*هل سيحتاج هذا الزر اى تعديل */

        private void btnEnd_Click_(object? sender, EventArgs e)
        {
            /* ك لماذا لا يتم اخذ باك اب عند الاغلاق ام انه يأخذها فى اماكن مختلفة*/
            try
            {
                string settingsPath = Path.Combine(Application.StartupPath, "serverConnectionSettings.txt");

                if (!File.Exists(settingsPath))
                {
                    MessageBox.Show("❌ ملف إعدادات الاتصال غير موجود!", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // ✅ تحميل الإعدادات إلى AppSettings مرة واحدة
                AppSettings.Load(settingsPath);

                var helper = new DatabaseBackupRestoreHelper(settingsPath);

                // ✅ 1. النسخ الاحتياطي
                helper.BackupDatabase();

                // ✅ 2. تنظيف النسخ القديمة
                string? backupFolder = AppSettings.GetString("BackupsPath", null);
                string? dbName = AppSettings.GetString("DBName", null);

                if (!string.IsNullOrWhiteSpace(backupFolder))
                {
                    helper.CleanOldBackups(backupFolder);

                    // ✅ 3. نسخ النسخة الأخيرة إلى مجلد مشترك باسم ثابت
                    helper.CopyLatestBackupToSharedFolder(
                        sourceBackupFolder: backupFolder,
                        sharedFolderPath: @"D:\BackupToPush", // يمكن استبداله بقراءة من AppSettings إذا لزم
                        outputFileName: "MizanOriginalDB.bak"
                    );
                }

                // ✅ 4. نسخ النسخة إلى Google Drive
                string? googleDrivePath = AppSettings.GetString("GoogleDrivePath", @"D:\ClintGoogleDrive");
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

                // ✅ 5. Git Push لمجلد المشروع
                string? projectPath = AppSettings.GetString("ProjectPath", null);
                if (!string.IsNullOrWhiteSpace(projectPath))
                {
                    ExecuteGitPush(projectPath);
                }

                // ✅ 6. Git Push لمجلد نسخ القواعد
                string? backupPushPath = AppSettings.GetString("BackupGitPath", null);
                if (!string.IsNullOrWhiteSpace(backupPushPath))
                {
                    ExecuteGitPush(backupPushPath);
                }

                // ✅ 7. تحديث بيانات القاعدة
                DBServiecs.A_UpdateAllDataBase();

                // ✅ 8. إنهاء البرنامج
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ حدث خطأ أثناء إنهاء التطبيق: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private void btnEnd_Click__(object? sender, EventArgs e)
        //{
        //    try
        //    {
        //        string settingsPath = Path.Combine(Application.StartupPath, "serverConnectionSettings.txt");

        //        if (!File.Exists(settingsPath))
        //        {
        //            MessageBox.Show("❌ ملف إعدادات الاتصال غير موجود!", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            return;
        //        }

        //        // تحميل الإعدادات
        //        AppSettings.Load(settingsPath);

        //        // قراءة الإعدادات بعد التحميل
        //        string? backupPath = AppSettings.GetString("BackupsPath", "غير موجود");
        //        string? serverName = AppSettings.GetString("serverName", "غير موجود");
        //        string? dbName = AppSettings.GetString("DBName", "غير موجود");
        //        string? googleDrivePath = AppSettings.GetString("GoogleDrivePath", "غير موجود");
        //        string? projectPath = AppSettings.GetString("ProjectPath", "غير موجود");
        //        string? backupGitPath = AppSettings.GetString("BackupGitPath", "غير موجود");

        //        // عرض الإعدادات
        //        MessageBox.Show(
        //            $"📂 BackupsPath: {backupPath}\n" +
        //            $"🖥 serverName: {serverName}\n" +
        //            $"🗄 DBName: {dbName}\n" +
        //            $"☁ GoogleDrivePath: {googleDrivePath}\n" +
        //            $"📁 ProjectPath: {projectPath}\n" +
        //            $"📁 BackupGitPath: {backupGitPath}",
        //            "الإعدادات المحملة"
        //        );

        //        var helper = new DatabaseBackupRestoreHelper(settingsPath);

        //        // 1. النسخ الاحتياطي
        //        helper.BackupDatabase();
        //        MessageBox.Show($"✅ تم إنشاء النسخة الاحتياطية في:\n{backupPath}", "Backup");

        //        // 2. تنظيف النسخ القديمة
        //        if (!string.IsNullOrWhiteSpace(backupPath))
        //        {
        //            helper.CleanOldBackups(backupPath);
        //            MessageBox.Show($"🗑 تم تنظيف النسخ القديمة في:\n{backupPath}", "Clean");
        //        }

        //        // 3. نسخ النسخة الأخيرة إلى مجلد ثابت
        //        helper.CopyLatestBackupToSharedFolder(
        //            sourceBackupFolder: backupPath,
        //            sharedFolderPath: @"D:\BackupToPush",
        //            outputFileName: "MizanOriginalDB.bak"
        //        );
        //        MessageBox.Show("📂 تم نسخ آخر نسخة إلى:\nD:\\BackupToPush\\MizanOriginalDB.bak", "Copy Shared");

        //        // 4. نسخة إلى Google Drive
        //        if (!string.IsNullOrWhiteSpace(backupPath) &&
        //            !string.IsNullOrWhiteSpace(dbName) &&
        //            !string.IsNullOrWhiteSpace(googleDrivePath))
        //        {
        //            helper.CopyBackupToGoogleDrive(
        //                sourceFolder: backupPath,
        //                googleDriveFolder: googleDrivePath,
        //                dbName: dbName
        //            );
        //            MessageBox.Show($"☁ تم نسخ النسخة إلى Google Drive:\n{googleDrivePath}", "Google Drive");
        //        }

        //        // 5. Git Push لمجلد المشروع
        //        if (!string.IsNullOrWhiteSpace(projectPath))
        //        {
        //            ExecuteGitPush(projectPath);
        //            MessageBox.Show($"📤 تم رفع مشروع Git من:\n{projectPath}", "Git Project");
        //        }

        //        // 6. Git Push لمجلد النسخ
        //        if (!string.IsNullOrWhiteSpace(backupGitPath))
        //        {
        //            ExecuteGitPush(backupGitPath);
        //            MessageBox.Show($"📤 تم رفع نسخ القواعد من:\n{backupGitPath}", "Git Backup");
        //        }

        //        // 7. تحديث القاعدة
        //        DBServiecs.A_UpdateAllDataBase();
        //        MessageBox.Show("🔄 تم تحديث بيانات القاعدة", "Update DB");

        //        // 8. إنهاء البرنامج
        //        Application.Exit();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("❌ حدث خطأ أثناء إنهاء التطبيق: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        private void btnEnd_Click(object? sender, EventArgs e)
        {
            try
            {
                string settingsPath = Path.Combine(Application.StartupPath, "serverConnectionSettings.txt");

                if (!File.Exists(settingsPath))
                    return;

                // ✅ تحميل الإعدادات بشكل آمن
                var settings = LoadSettings(settingsPath);
                if (settings.Count == 0)
                    return;

                var helper = new DatabaseBackupRestoreHelper(settingsPath);

                // 1. النسخ الاحتياطي
                helper.BackupDatabase();

                // 2. تنظيف النسخ القديمة + نسخ آخر نسخة إلى مجلد مشترك
                string backupFolder = settings.ContainsKey("BackupsPath") ? settings["BackupsPath"] : "";
                if (!string.IsNullOrWhiteSpace(backupFolder))
                {
                    helper.CleanOldBackups(backupFolder);
                    helper.CopyLatestBackupToSharedFolder(
                        sourceBackupFolder: backupFolder,
                        sharedFolderPath: @"D:\BackupToPush",
                        outputFileName: "MizanOriginalDB.bak"
                    );
                }

                // 3. نسخ النسخة إلى Google Drive
                string dbName = settings.ContainsKey("DBName") ? settings["DBName"] : "";
                string googleDrivePath = settings.ContainsKey("GoogleDrivePath") ? settings["GoogleDrivePath"] : "";
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

                // 4. Git Push لمجلد المشروع
                string projectPath = settings.ContainsKey("ProjectPath") ? settings["ProjectPath"] : "";
                if (!string.IsNullOrWhiteSpace(projectPath))
                {
                    ExecuteGitPush(projectPath);
                }

                // 5. Git Push لمجلد نسخ القواعد
                string backupPushPath = settings.ContainsKey("BackupGitPath") ? settings["BackupGitPath"] : "";
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
                // سجل الخطأ بدل ما تسكت عليه
                File.AppendAllText("error_log.txt", $"{DateTime.Now}: {ex}\n");
            }
        }

        /// <summary>
        /// تحميل الإعدادات بشكل آمن (يتجاهل التعليقات والسطور الخاطئة)
        /// </summary>
        private Dictionary<string, string> LoadSettings(string filePath)
        {
            var settings = new Dictionary<string, string>();

            if (!File.Exists(filePath))
                return settings;

            foreach (var rawLine in File.ReadAllLines(filePath))
            {
                string line = rawLine.Trim();

                // تجاهل السطور الفارغة أو التعليقات
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#") || line.StartsWith("//") || line.StartsWith(";"))
                    continue;

                int equalIndex = line.IndexOf('=');
                if (equalIndex <= 0)
                    continue;

                string key = line.Substring(0, equalIndex).Trim();
                string value = line.Substring(equalIndex + 1).Trim();

                if (!string.IsNullOrEmpty(key))
                    settings[key] = value;
            }

            return settings;
        }

        private void btnEnd_Click__(object? sender, EventArgs e)
        {
            try
            {
                string settingsPath = Path.Combine(Application.StartupPath, "serverConnectionSettings.txt");

                if (!File.Exists(settingsPath))
                    return;

                // تحميل الإعدادات
                AppSettings.Load(settingsPath);

                var helper = new DatabaseBackupRestoreHelper(settingsPath);

                // 1. النسخ الاحتياطي
                helper.BackupDatabase();

                // 2. تنظيف النسخ القديمة + نسخ آخر نسخة إلى مجلد مشترك
                string? backupFolder = AppSettings.GetString("BackupsPath", null);
                if (!string.IsNullOrWhiteSpace(backupFolder))
                {
                    helper.CleanOldBackups(backupFolder);
                    helper.CopyLatestBackupToSharedFolder(
                        sourceBackupFolder: backupFolder,
                        sharedFolderPath: @"D:\BackupToPush",
                        outputFileName: "MizanOriginalDB.bak"
                    );
                }

                // 3. نسخ النسخة إلى Google Drive
                string? dbName = AppSettings.GetString("DBName", null);
                string? googleDrivePath = AppSettings.GetString("GoogleDrivePath", null);
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

                // 4. Git Push لمجلد المشروع
                string? projectPath = AppSettings.GetString("ProjectPath", null);
                if (!string.IsNullOrWhiteSpace(projectPath))
                {
                    ExecuteGitPush(projectPath);
                }

                // 5. Git Push لمجلد نسخ القواعد
                string? backupPushPath = AppSettings.GetString("BackupGitPath", null);
                if (!string.IsNullOrWhiteSpace(backupPushPath))
                {
                    ExecuteGitPush(backupPushPath);
                }

                // 6. تحديث بيانات القاعدة
                DBServiecs.A_UpdateAllDataBase();

                // 7. إنهاء البرنامج
                Application.Exit();
            }
            catch
            {
                // بدون رسائل أو تنبيه
            }
        }

        //
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


        private void btnUserUpdatePass_Click(object? sender, EventArgs e)
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
                            DBServiecs.User_ChangePassword(us_id, newPassword);
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
            lblMasterRerpots.Visible = false;

            CustomMessageBox.ShowInformation($"لقد تمت مغادرتك يا\n{username}\nنسعد بعودتك الينا لاحقا فى برامج ميزان ...فى امان الله ", "خروج");

        }

        #endregion

        private void lblMasterRerpots_Click(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                HighlightTransparentButton(btn);
            }

            CloseAllFormsExceptMain();
            frmReportsManager frm = new frmReportsManager();
            OpenFormInPanel(frm);


        }

        private void DGV_DoubleClick(object sender, EventArgs e)
        {
            if (DGV.CurrentRow == null || DGV.CurrentRow.IsNewRow)
                return;

            // الحصول على بيانات الصف
            string reportCodeName = DGV.CurrentRow.Cells["ReportCodeName"].Value?.ToString() ?? "";
            string reportDisplayName = DGV.CurrentRow.Cells["ReportDisplayName"].Value?.ToString() ?? "";
            int reportId = Convert.ToInt32(DGV.CurrentRow.Cells["ReportID"].Value ?? 0);

            // تجهيز القاموس
            Dictionary<string, object> reportParameters = new Dictionary<string, object>
    {
        { "ReportCodeName", reportCodeName },
        { "ReportDisplayName", reportDisplayName },
        { "ReportID", reportId }
    };

            // فتح شاشة الإعدادات
            using frmSettingReports previewForm = new frmSettingReports(reportParameters);
            previewForm.ShowDialog();
        }
    }
}
