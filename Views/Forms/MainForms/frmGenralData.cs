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
            // ✅ السماح بالتعديل داخل شاشة الإعدادات
            AppSettings.EnableEditMode(nameof(frmGenralData));

            tabMang.ItemSize = new Size(150, 40);
            LoadWarehouses();
            FillcbxReturnSaleMode();
            LoadSettings();   // يفضل الإبقاء عليه لتحميل القيم
            tlpPading();
            UpdateLabelCount();
            TextBoxesInTabs();
            txtNameCo.Focus();
            txtNameCo.SelectAll();
            LoadBackupFiles();
            AttachTextBoxHandlers(this);
            ApplyPermissionsToControls();
            LoadAllUsers();
            DGV_Users.SelectionChanged += DGV_Users_SelectionChanged;
            DGV_Users.RowPrePaint += DGV_Users_RowPrePaint;

            LoadUsers();
            cbxUsers.SelectedIndexChanged += CbxUsers_SelectedIndexChanged;
            DGVStyl();
        }

        // 🔹 تعطيل التحرير عند إغلاق الشاشة
        private void frmGenralData_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppSettings.DisableEditMode();
        }


        #region *********  ApplyPermissions  ******************************
        private void ApplyPermissionsToControls()
        {
            var allControls = GetAllControls(this); // تأكد أنك أضفت هذه الدالة أدناه

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

        #endregion


        #region ====== إدارة المستخدمين والصلاحيات ======
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


        //Times New Roman
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
                    txtUserName.Text = selectedUser["UserName"].ToString();
                    txtFullName.Text = selectedUser["FullName"].ToString();

                    // ✅ تعيين الحالة بناءً على بيانات المستخدم
                    chkIsAdmin.Checked = Convert.ToBoolean(selectedUser["IsAdmin"]);
                    chkIsActive.Checked = Convert.ToBoolean(selectedUser["IsActive"]);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء تحميل الصلاحيات: " + ex.Message);
            }
        }

        private void btnNewUser_Click(object? sender, EventArgs? e)
        {
            DGV_Users.ClearSelection();
            DGV_Permissions.DataSource = null;
            lblID_User.Text = "0";
            txtUserName.Clear();
            txtFullName.Clear();

        }
        private void btnSave_UserData_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUserName.Text) || string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("يرجى إدخال اسم المستخدم والاسم الكامل.");
                return;
            }

            string username = txtUserName.Text.Trim();
            string fullName = txtFullName.Text.Trim();
            int userId = Convert.ToInt32(lblID_User.Text);

            // ✅ الربط الفعلي مع CheckBox
            bool isAdmin = chkIsAdmin.Checked;
            bool isActive = chkIsActive.Checked;

            string result;

            if (userId == 0)
            {
                result = DBServiecs.User_Add(username, fullName, CurrentSession.UserID); // لا تحتاج isAdmin و isActive عند الإضافة إذا كانت افتراضية
            }
            else
            {
                result = DBServiecs.User_Update(userId, username, fullName, isAdmin, isActive, CurrentSession.UserID);

            }

            MessageBox.Show(result);
            LoadAllUsers();
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

        #region ************  
        private void LoadUsers()
        {
            var usersTable = DBServiecs.User_GetAll();
            cbxUsers.DisplayMember = "FullName";
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
        private void BtnSave_Click(object sender, EventArgs e)
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


        private void LoadWarehouses_()
        {
            var table = DBServiecs.Warehouse_GetAll();
            cbxWarehouses.DisplayMember = "WarehouseName";
            cbxWarehouses.ValueMember = "WarehouseId";
            cbxWarehouses.DataSource = table;
        }

        #endregion 
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
                    case "IsSaleByNegativeStock":
                        if (bool.TryParse(value, out bool isNegativeStock))
                        {
                            chkIsSaleByNegativeStock.Checked = isNegativeStock;
                            lblTypeSaleStock.Text = isNegativeStock
                                ? "البيع على المكشوف"
                                : "البيع حسب الرصيد";
                        }
                        break;
                    case "ReturnSaleMode":
                        // 🔹 ضبط الكمبو على القيمة من الملف
                        if (int.TryParse(value, out int selectedMode))
                        {
                            cbxReturnSaleMode.SelectedValue = selectedMode;
                        }
                        break;

                    case "CompanyLoGoFolder": lblLogoPath.Text = value; break;
                    case "LogoImagName": lblLogoImageName.Text = value; break;
                    case "DefaultWarehouseId":
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


        /*توجد فكرة لا ادرى مدى فاعليتها 
             وهى ان الكمبوبكس يتم تعبئته عند الفتح بهذة الطريقة وتم ربطه بالتكست الذى يتم كتابة القيمة فيه 
            وقبل ذلك كنت اكتب يدويا فى التكست وكان يتم حفظ القيمة عند خروجى من التكست اما بعد ربطه بالكمبوبس لم اعد ادخل واغير القيم ثم اخرج فلا يتم الحفظ 
            فهل لو تم الحفظ بمجرد تغير قيمته يوفى الغرض دون مسح البيانات
             */

        #endregion

        #region حفظ الإعدادات إلى الملف
        private void btnSave_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtServerName.Text) || string.IsNullOrWhiteSpace(txtDBName.Text))
            {
                MessageBox.Show("يرجى تحديد السيرفر وقاعدة البيانات.");
                return;
            }
            SaveData();

        }
        #endregion

        #region 🔹 إعداد وضع البيع المرتد (ReturnSaleMode)

        // 🔹 ملء ComboBox بالقيم المتاحة (1، 2، 3) وربطه بالبيانات
        private void FillcbxReturnSaleMode()
        {
            cbxReturnSaleMode.DropDownStyle = ComboBoxStyle.DropDownList;

            var saleModes = new List<KeyValuePair<int, string>>
    {
        new KeyValuePair<int, string>(1, "البيع المرتد حسب الفاتورة"),
        new KeyValuePair<int, string>(2, "البيع المرتد بالكود مباشر"),
        new KeyValuePair<int, string>(3, "البيع المرتد بالنظامين")
    };

            cbxReturnSaleMode.DataSource = saleModes;
            cbxReturnSaleMode.DisplayMember = "Value";
            cbxReturnSaleMode.ValueMember = "Key";

            // عند تغيير القيمة نحفظ تلقائيًا
            cbxReturnSaleMode.SelectedIndexChanged += (s, e) =>
            {
                if (cbxReturnSaleMode.SelectedValue != null)
                {
                    int mode = (int)cbxReturnSaleMode.SelectedValue;
                    AppSettings.Set("ReturnSaleMode", mode.ToString());
                    AppSettings.Save();
                }
            };

            // تحميل القيمة المحفوظة
            int savedMode = AppSettings.GetInt("ReturnSaleMode", 1);
            cbxReturnSaleMode.SelectedValue = savedMode;
        }

        #endregion


        #region حفظ الإعدادات بصمت (بدون رسالة)

        // ربط أحداث التغيير تلقائيًا لمربعات النصوص والـ CheckBox داخل الحاوية.
        private void AttachControlHandlers(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is TextBox txt)
                {
                    txt.Tag = txt.Text; // حفظ القيمة الأصلية للمقارنة لاحقًا
                    txt.Leave += TextBox_Leave;
                }
                else if (ctrl is CheckBox chk)
                {
                    chk.Tag = chk.Checked; // حفظ القيمة الأصلية
                    chk.CheckedChanged += CheckBox_CheckedChanged;
                }
                else if (ctrl.HasChildren)
                {
                    AttachControlHandlers(ctrl); // تكرار على الأبناء
                }
            }
        }

        // ربط الحدث لكل TextBox و CheckBox.
        private void AttachTextBoxHandlers(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is TextBox txt)
                {
                    txt.Tag = txt.Text; // حفظ القيمة القديمة
                    txt.Leave += TextBox_Leave;
                }
                else if (ctrl is CheckBox chk)
                {
                    chk.Tag = chk.Checked; // حفظ القيمة القديمة
                    chk.CheckedChanged += CheckBox_CheckedChanged;
                }
                else if (ctrl.HasChildren)
                {
                    AttachTextBoxHandlers(ctrl); // Recursion
                }
            }
        }

        // تنفيذ الحفظ عند تغيير قيمة مربع النص.
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

        // تنفيذ الحفظ عند تغيير حالة الـ CheckBox.
        private void CheckBox_CheckedChanged(object? sender, EventArgs e)
        {
            if (sender is CheckBox chk)
            {
                // 🔹 تغيير النص حسب حالة الـ CheckBox
                lblTypeSaleStock.Text = chk.Checked ? "البيع على المكشوف" : "البيع حسب الرصيد";

                if ((chk.Tag is bool oldValue) && chk.Checked != oldValue)
                {
                    SaveData();
                    LoadSettings();
                    chk.Tag = chk.Checked;
                }
            }
        }

        private void SaveData()
        {
            // اقرأ كل الأسطر من الملف (مع التعليقات)
            List<string> lines = File.Exists(configFilePath)
                ? File.ReadAllLines(configFilePath).ToList()
                : new List<string>();

            // إعدادات جديدة عايزين نحفظها
            Dictionary<string, string> newSettings = new Dictionary<string, string>
            {
                ["serverName"] = txtServerName.Text,
                ["DBName"] = txtDBName.Text,
                ["maxBackups"] = txtMaxBackups.Text,
                ["BackupsPath"] = txtBackupsPath.Text,
                ["RollPrinter"] = lblRollPrinter.Text,
                ["SheetPrinter"] = lblSheetPrinter.Text,
                ["SheetRows"] = txtSheetRows.Text,
                ["SheetCols"] = txtSheetCols.Text,
                ["SheetMarginTop"] = txtMarginTop.Text,
                ["SheetMarginBottom"] = txtMarginBottom.Text,
                ["SheetMarginRight"] = txtMarginRight.Text,
                ["SheetMarginLeft"] = txtMarginLeft.Text,
                ["RollLabelWidth"] = txtRollLabelWidth.Text,
                ["RollLabelHeight"] = txtRollLabelHeight.Text,
                ["CompanyName"] = txtNameCo.Text,
                ["CompanyPhon"] = txtPhon.Text,
                ["CompanyAnthrPhon"] = txtAnthrPhon.Text,
                ["SalesTax"] = txtSalesTax.Text,
                ["CompanyAdreass"] = txtAdreass.Text,
                ["EmailCo"] = txtCompanyEmail.Text,
                ["IsSaleByNegativeStock"] = chkIsSaleByNegativeStock.Checked.ToString(),
                ["CompanyLoGoFolder"] = lblLogoPath.Text,
                ["LogoImagName"] = lblLogoImageName.Text,
                ["DefaultWarehouseId"] = cbxWarehouseId.SelectedValue?.ToString() ?? ""

            };

            // نحدث أو نضيف الإعدادات
            foreach (var setting in newSettings)
            {
                string key = setting.Key;
                string newValue = setting.Value;

                bool found = false;

                for (int i = 0; i < lines.Count; i++)
                {
                    // تخطي التعليقات والأسطر الفارغة
                    if (string.IsNullOrWhiteSpace(lines[i]) || lines[i].TrimStart().StartsWith("#"))
                        continue;

                    // لو السطر فيه نفس المفتاح نعدله
                    if (lines[i].StartsWith(key + "=", StringComparison.OrdinalIgnoreCase))
                    {
                        lines[i] = $"{key}={newValue}";
                        found = true;
                        break;
                    }
                }

                // لو المفتاح مش موجود نضيفه في الآخر
                if (!found)
                {
                    lines.Add($"{key}={newValue}");
                }
            }

            // نكتب الملف من جديد مع الحفاظ على التعليقات
            File.WriteAllLines(configFilePath, lines);

            // الكلاس AppSettings يقرأ مرة واحدة عند فتح البرنامج ملف  الستينج فيجب اعادة القرائة بعد تعديل اى اعداد
            LoadSettings();
        }

        private void txtReturnSaleMode_KeyPress(object sender, KeyPressEventArgs e)
        {
            // السماح بـ Backspace
            if (e.KeyChar == (char)Keys.Back)
                return;

            // السماح فقط بـ 1 أو 2 أو 3
            if (e.KeyChar != '1' && e.KeyChar != '2' && e.KeyChar != '3')
            {
                e.Handled = true; // منع الإدخال
            }
        }

        private void cbxReturnSaleMode_SelectedIndexChanged(object sender, EventArgs e)
        {
           /*شاشة الاعداد لم تعد تفتح تخرج الرسالة السابقة لما فتحت شاشة الاعداد
            وهذا الكود المسؤول عن الحفظ الصامت لكل ادوات الشاشة فاين الخلل
            */
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
            foreach (TabPage tab in tabMang.TabPages)
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
            TabPage? selectedTab = tabMang.SelectedTab;

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

        #endregion

        #region ✅ اختيار مجلد النسخ الاحتياطية

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
        // إضافة فرع جديد.
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

                    //
                    cbxWarehouses.DataSource = dt;
                    cbxWarehouses.DisplayMember = "WarehouseName";
                    cbxWarehouses.ValueMember = "WarehouseId";

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

            // ✅ التحقق من أن رقم الفرع ليس أقل من صفر
            if (selectedWarehouseId <= 0)
            {
                MessageBox.Show("❌ رقم الفرع غير صحيح .", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
