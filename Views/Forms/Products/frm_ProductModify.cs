using System.IO;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MizanOriginalSoft.MainClasses.OriginalClasses;

using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.Views.Forms.MainForms;
using static MizanOriginalSoft.Views.Forms.MainForms.frmSearch;

namespace MizanOriginalSoft.Views.Forms.Products
{
    public partial class frm_ProductModify : Form
    {
        #region ********* متغيرات عامة للشاشة  ******************
        private string Note_Prod = string.Empty;
        private string picProductPath = string.Empty;

        private int ID_Product = 0;
        private string? ProdName = string.Empty;
        private int UnitID = 0;
        private float B_Price = 0f;
        private float U_Price = 0f;
        private string ProdCodeOnSuplier = string.Empty;
        private float MinLenth = 0f;
        private float MinStock = 0f;
        private int Category_id = 0;
        private int SuplierID = 0;




        private int _productId;
        public int CategoryID { get; private set; }
        #endregion 
        public frm_ProductModify(int productId)
        {
            InitializeComponent();
            _productId = productId;
            SetupAutoCompleteCategories();
            SetupAutoCompleteSuppliers();
            InitializeDecimalInput();
        }

        private void frm_ProductModify_Load(object sender, EventArgs e)
        {
            FillUnits();
            // تحميل بيانات المنتج حسب _productId
            LoadProductData(_productId);

        }
        private DataTable? tblSupplier;
        private DataTable? tblCategory;

        //// فتح المساعدة الخاصة بالأداة النشطة عند الضغط على Ctrl + H
        //private void ShowHelpForActiveControl()
        //{
        //    if (this.ActiveControl == null)
        //        return;

        //    // استخدام الرمز & بدلاً من _
        //    string controlKey = $"{this.Name}&{this.ActiveControl.Name}";
        //    frmHelp helpForm = new frmHelp(controlKey);
        //    helpForm.ShowDialog();
        //}


        #region **********  _KeyDown ***************

        private void txtProdName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtNoteProduct.Focus();
            }
        }

        private void txtNoteProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtSuppliers.Focus();
            }
        }

        // الحدث الذي يتعامل مع ضغطات المفاتيح داخل txtNewItemSuppliers
        private void txtSuppliers_KeyDown(object sender, KeyEventArgs e)
        {
            // عند الضغط على Enter يتم الانتقال إلى حقل كود المنتج لدى المورد
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtProdCodeOnSuplier.Focus();
            }
            if (e.Control && e.KeyCode == Keys.H)
            {
                HelpTextReader.ShowHelpForControl(this, sender);
                e.SuppressKeyPress = true;
            }

            // عند الضغط على Ctrl + F يتم فتح شاشة البحث عن الموردين
            if (e.Control && e.KeyCode == Keys.F)
            {

                e.SuppressKeyPress = true;


                // رقم 14 يمثل الموردين
                //  فتح نموذج البحث العام
                frmSearch searchForm = new frmSearch(14, SearchEntityType.Supplier);

                if (searchForm.ShowDialog() == DialogResult.OK)
                {
                    // إذا اختار المستخدم موردًا من نتيجة البحث
                    txtSuppliers.Text = searchForm.SelectedName;
                    lblSuppliers.Text = searchForm.SelectedID;

                }
            }
        }

        private void txtProdCodeOnSuplier_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtCategories.Focus();
            }
        }
        private void txtCategories_KeyDown(object sender, KeyEventArgs e)
        {

            // عند الضغط على Enter يتم الانتقال إلى حقل كود المنتج لدى المورد
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtU_Price.Focus();
            }

            if (e.Control && e.KeyCode == Keys.H)
            {

                HelpTextReader.ShowHelpForControl(this, sender);
                e.SuppressKeyPress = true;
            }

            // عند الضغط على Ctrl + F يتم فتح شاشة البحث عن الموردين
            if (e.Control && e.KeyCode == Keys.F)
            {

                e.SuppressKeyPress = true;

                //    int typeId = 20;

                //  فتح نموذج البحث العام
                frmSearch searchForm = new frmSearch(20, SearchEntityType.Catigory);

                if (searchForm.ShowDialog() == DialogResult.OK)
                {
                    // إذا اختار المستخدم موردًا من نتيجة البحث
                    txtCategories.Text = searchForm.SelectedName;
                    lblCategory_id.Text = searchForm.SelectedID;

                }
            }
        }

        private void txtU_Price_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtB_Price.Focus();
            }
        }

        private void txtB_Price_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                cbxUnit_ID.Focus();
            }
        }

        private void cbxUnit_ID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtMinLenth.Focus();
            }
            if (e.Control && e.KeyCode == Keys.H)
            {
                HelpTextReader.ShowHelpForControl(this, sender);
                e.SuppressKeyPress = true;
            }

        }

        private void txtMinLenth_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtMinStock.Focus();
            }
        }

        private void txtMinStock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtProdName.Focus();
            }
        }



        #endregion 



        //
        //تعبئة مربع الموردين تعبئة تلقائية  ###
        private void SetupAutoCompleteSuppliers()
        {
            // جلب جميع الموردين
            tblSupplier = DBServiecs.Accounts_GetSupplier();
            // إنشاء مجموعة الإكمال التلقائي
            AutoCompleteStringCollection autoCompleteCollection = new AutoCompleteStringCollection();//'new' expression can be simplified
            // التأكد من أن الجدول يحتوي على بيانات
            if (tblSupplier != null && tblSupplier.Rows.Count > 0)
            {
                // إضافة أسماء المستخدمين إلى مجموعة الإكمال التلقائي
                foreach (DataRow row in tblSupplier.Rows)
                {
                    string? accName = row["AccName"]?.ToString();
                    if (!string.IsNullOrEmpty(accName))
                    {
                        autoCompleteCollection.Add(accName);
                    }
                }
            }

            // إعداد خصائص مربع النص لاستخدام الإكمال التلقائي
            txtSuppliers.AutoCompleteCustomSource = autoCompleteCollection;
            txtSuppliers.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtSuppliers.AutoCompleteSource = AutoCompleteSource.CustomSource;

        }
        /*
        */


        private void txtSuppliers_Leave(object sender, EventArgs e)
        {
            if (tblSupplier == null) return;

            string selectedName = txtSuppliers.Text.Trim();

            // السماح بترك الحقل فارغًا
            if (string.IsNullOrEmpty(selectedName))
            {
                lblSuppliers.Text = "";
                return;
            }

            // البحث عن اسم المورد في الجدول
            DataRow[] matched = tblSupplier.Select($"AccName = '{selectedName.Replace("'", "''")}'");

            if (matched.Length > 0)
            {
                // تم العثور على الاسم
                lblSuppliers.Text = matched[0]["AccID"].ToString();
            }
            else
            {
                // لم يتم العثور على الاسم → تنبيه المستخدم
                MessageBox.Show("الاسم الذي أدخلته غير موجود في قائمة الموردين.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSuppliers.Focus();
                txtSuppliers.SelectAll();
                txtSuppliers.Text = "";
            }
        }



        //تعبئة مربع التصنيفات تعبئة تلقائية  ###
        private void SetupAutoCompleteCategories()
        {
            // جلب جميع التصنيفات
            tblCategory = DBServiecs.Categories_GetAll();
            // إنشاء مجموعة الإكمال التلقائي
            AutoCompleteStringCollection autoCompleteCollection = new AutoCompleteStringCollection();//'new' expression can be simplified
            // التأكد من أن الجدول يحتوي على بيانات
            if (tblCategory != null && tblCategory.Rows.Count > 0)
            {
                // إضافة أسماء المستخدمين إلى مجموعة الإكمال التلقائي
                foreach (DataRow row in tblCategory.Rows)
                {
                    string? accName = row["CategoryName"]?.ToString();
                    if (!string.IsNullOrEmpty(accName))
                    {
                        autoCompleteCollection.Add(accName);
                    }

                }
            }

            // إعداد خصائص مربع النص لاستخدام الإكمال التلقائي
            txtCategories.AutoCompleteCustomSource = autoCompleteCollection;
            txtCategories.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtCategories.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }


        private void txtCategories_Leave(object sender, EventArgs e)
        {
            if (tblCategory == null) return;

            string selectedName = txtCategories.Text.Trim();

            // السماح بترك الحقل فارغًا
            if (string.IsNullOrEmpty(selectedName))
            {
                lblCategory_id.Text = "";
                return;
            }

            // البحث عن اسم المورد في الجدول
            DataRow[] matched = tblCategory.Select($"CategoryName = '{selectedName.Replace("'", "''")}'");

            if (matched.Length > 0)
            {
                // تم العثور على الاسم
                lblCategory_id.Text = matched[0]["CategoryID"].ToString();
            }
            else
            {
                // لم يتم العثور على الاسم → تنبيه المستخدم
                MessageBox.Show("الاسم الذي أدخلته غير موجود في قائمة التصنيفات.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategories.Focus();
                txtCategories.SelectAll();
                txtCategories.Text = "";
            }
        }



        private void FillUnits()
        {
            DataTable dt = DBServiecs.ProductGetUnits();

            cbxUnit_ID.DataSource = dt;
            cbxUnit_ID.DisplayMember = "UnitProd";
            cbxUnit_ID.ValueMember = "Unit_ID";

            // منع الكتابة داخل ComboBox
            cbxUnit_ID.DropDownStyle = ComboBoxStyle.DropDownList;

            // تحديد العنصر الثاني كمحدد افتراضي (إذا كان موجودًا)
            if (cbxUnit_ID.Items.Count > 1)
                cbxUnit_ID.SelectedIndex = 1;
        }
        private void LoadProductData(int productId)
        {
            try
            {
                // جلب بيانات المنتج من قاعدة البيانات
                DataTable dt = DBServiecs.Product_GetOneID(productId);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    // تعيين القيم للنصوص الأساسية
                    lblID_Product.Text = row["ID_Product"]?.ToString() ?? "";
                    lblProductCode.Text = row["ProductCode"]?.ToString() ?? "";
                    txtProdName.Text = row["ProdName"]?.ToString() ?? "";
                    txtProdCodeOnSuplier.Text = row["ProdCodeOnSuplier"]?.ToString() ?? "";

                    // تعيين القيم الرقمية (مع تنسيق للعرض)
                    txtB_Price.Text = row["B_Price"] != DBNull.Value ? Convert.ToDecimal(row["B_Price"]).ToString("N2") : "0.00";
                    txtU_Price.Text = row["U_Price"] != DBNull.Value ? Convert.ToDecimal(row["U_Price"]).ToString("N2") : "0.00";
                    txtMinLenth.Text = row["MinLenth"]?.ToString() ?? "";
                    txtMinStock.Text = row["MinStock"]?.ToString() ?? "";

                    // تعيين القيم الأخرى
                    lblSuppliers.Text = row["SuplierID"]?.ToString() ?? "";
                    lblRegistYear.Text = row["RegistYear"]?.ToString() ?? "";
                    lblProductStock.Text = row["ProductStock"]?.ToString() ?? "";
                    lblCategory_id.Text = row["Category_id"]?.ToString() ?? "";
                    txtCategories.Text = row["CategoryName"]?.ToString() ?? "";
                    txtSuppliers.Text = row["AccName"]?.ToString() ?? "";
                    txtNoteProduct.Text = row["noteProduct"]?.ToString() ?? "";

                    // تعيين وحدة القياس
                    if (row["UnitID"] != DBNull.Value)
                    {
                        cbxUnit_ID.SelectedValue = row["UnitID"].ToString();
                        cbxUnit_ID.Text = row["UnitProd"]?.ToString() ?? "";
                    }

                    // تعيين حالة الاختفاء
                    chkHiddinProd.Checked = row["HiddinProd"] != DBNull.Value && Convert.ToBoolean(row["HiddinProd"]);

                    // --- تحميل صورة المنتج ---
                    string? imagePath = row["PicProduct"] is DBNull or null ? null : row["PicProduct"].ToString();

                    if (!string.IsNullOrWhiteSpace(imagePath))
                    {
                        // حفظ المسار في الليبل
                        lblPathProductPic.Text = imagePath;

                        if (File.Exists(imagePath))
                        {
                            // الصورة موجودة - عرض الصورة الأصلية
                            PicProduct.Image = Image.FromFile(imagePath);
                            PicProduct.SizeMode = PictureBoxSizeMode.StretchImage;
                        }
                        else
                        {
                            // الصورة غير موجودة في المسار - عرض صورة توضيحية تحتوي على نص
                            PicProduct.Image = ImageHelper.CreateTextImage("الصورة غير متوفرة", PicProduct.Width, PicProduct.Height);
                            PicProduct.SizeMode = PictureBoxSizeMode.CenterImage;
                        }
                    }
                    else
                    {
                        // لم يتم إدخال صورة للمنتج - عرض صورة نصية توضح ذلك
                        PicProduct.Image = ImageHelper.CreateTextImage("لا توجد صورة", PicProduct.Width, PicProduct.Height);
                        PicProduct.SizeMode = PictureBoxSizeMode.CenterImage;
                        lblPathProductPic.Text = "";
                    }
                }
                else
                {
                    // لم يتم العثور على المنتج في قاعدة البيانات
                    MessageBox.Show("لم يتم العثور على بيانات المنتج", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                // التعامل مع أي خطأ أثناء التحميل
                MessageBox.Show($"حدث خطأ أثناء جلب بيانات المنتج: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GetData()
        {
            try
            {
                ID_Product = int.TryParse(lblID_Product.Text, out int id) ? id : 0;
                ProdName = txtProdName.Text.Trim();
                Note_Prod = txtNoteProduct.Text.Trim();
                UnitID = cbxUnit_ID.SelectedValue == null ? 0 : Convert.ToInt32(cbxUnit_ID.SelectedValue);
                B_Price = float.TryParse(txtB_Price.Text, out float bPrice) ? bPrice : 0f;
                U_Price = float.TryParse(txtU_Price.Text, out float uPrice) ? uPrice : 0f;
                ProdCodeOnSuplier = txtProdCodeOnSuplier.Text.Trim();
                MinLenth = float.TryParse(txtMinLenth.Text, out float minL) ? minL : 0f;
                MinStock = float.TryParse(txtMinStock.Text, out float minS) ? minS : 0f;
                Category_id = int.TryParse(txtCategories.Text, out int catId) ? catId : 0;
                SuplierID = int.TryParse(txtSuppliers.Text, out int supId) ? supId : 0;
                picProductPath = lblPathProductPic.Text.Trim();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ غير متوقع: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private bool ValidateInputs()
        {//lblPathProductPic  كيف اتأكد يحتوي على مسار صحيح قبل التمرير.
            if (string.IsNullOrWhiteSpace(txtProdName.Text))
            {
                MessageBox.Show("يجب إدخال اسم الصنف", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProdName.Focus();
                return false;
            }

            if (cbxUnit_ID.SelectedValue == null)
            {
                MessageBox.Show("يجب اختيار وحدة القياس", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbxUnit_ID.Focus();
                return false;
            }

            if (!float.TryParse(txtB_Price.Text, out _))
            {
                MessageBox.Show("سعر الشراء غير صالح", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtB_Price.Focus();
                return false;
            }

            if (!float.TryParse(txtU_Price.Text, out _))
            {
                MessageBox.Show("سعر البيع غير صالح", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtU_Price.Focus();
                return false;
            }

            // يمكن إضافة تحقق من باقي الحقول حسب الحاجة

            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // قراءة البيانات
                GetData();

                // تحقق من صحة البيانات
                if (!ValidateInputs())
                    return;

                int result = DBServiecs.Product_UpdateItem(
                    ID_Product,
                    ProdName!,
                    UnitID,
                    B_Price,
                    U_Price,
                    false, // أو true حسب حقل HiddinProd إن وجد لديك CheckBox
                    ProdCodeOnSuplier!,
                    MinLenth,
                    MinStock,
                    Category_id,
                    SuplierID,
                    Note_Prod!,
                    picProductPath! // المسار النهائي للصورة
                );

                if (result > 0)
                {
                    MessageBox.Show("تم حفظ الصنف بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("فشل في حفظ بيانات الصنف", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء حفظ البيانات: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLoadPicProduct_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "اختر صورة المنتج";
                ofd.Filter = "ملفات الصور (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // عرض الصورة في الـ PictureBox
                    PicProduct.Image = Image.FromFile(ofd.FileName);
                    PicProduct.SizeMode = PictureBoxSizeMode.StretchImage;

                    // عرض المسار في الليبل
                    lblPathProductPic.Text = ofd.FileName;
                }
            }
        }



        private int previousUnitId;

        private void cbxUnit_ID_SelectedIndexChanged(object sender, EventArgs e)
        {
            // التأكد من وجود قيمة صالحة
            if (cbxUnit_ID.SelectedValue == null || !int.TryParse(cbxUnit_ID.SelectedValue.ToString(), out int unitId))
                return;

            // التحقق من التحويل من متر (1) إلى وحدة أخرى
            if (previousUnitId == 1 && unitId != 1)
            {
                // جلب القطع المرتبطة بالمنتج
                if (!int.TryParse(lblID_Product.Text, out int prodId))
                    return;

                DataTable dt = DBServiecs.Product_GetPiecesByProductID(prodId);

                if (dt != null && dt.Rows.Count > 1)
                {
                    // إظهار تنبيه
                    MessageBox.Show("لا يمكن تغيير الوحدة لأن هناك قطع مرتبطة بهذا المنتج. يجب حذفها أولاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    // فتح نموذج عرض القطع
                    frmSearch frm = new frmSearch(201, SearchEntityType.Pice, prodId); // 123 هو رقم الصنف
                    frm.ShowDialog();


                    // بعد العودة من شاشة القطع، تأكد إن كانت القطع لا تزال موجودة
                    dt = DBServiecs.Product_GetPiecesByProductID(prodId); // إعادة استخدام نفس المتغير dt

                    if (dt != null && dt.Rows.Count > 1)
                    {
                        // إعادة الوحدة إلى متر لأن القطع لم تُحذف
                        cbxUnit_ID.SelectedValue = 1;
                        return;
                    }
                }
            }

            // باقي منطق التحكم في الحقول
            if (unitId == 1)
            {
                txtMinLenth.ReadOnly = false;
                lblMinLenth.ForeColor = Color.DarkBlue; // اللون الطبيعي
            }
            else
            {
                txtMinLenth.ReadOnly = true;
                txtMinLenth.Text = "0";
                lblMinLenth.ForeColor = Color.Gray; // لون باهت للدلالة على التعطيل
            }

            // تحديث القيمة السابقة
            previousUnitId = unitId;
        }

        private void cbxUnit_ID_SelectedIndexChanged_(object sender, EventArgs e)
        {
            /*
 فى حالة التحول من الوحدة متر
اذا كان  التعديل من المتر =1 الى اى قيمة اخرى اكبر من 1
يجب البحث فى جدول القطع عن الصنف  بواسطة الدالة 
            dt=DBServiecs .Product_GetPicDetailsByID (lblID_Product .Text );
            لو كان له اكثر من قطعة لا يجوز التحويل الا بعد حذف القطع التابعة 
            عن طريق فتح الشاشة 
                // رقم 201 يمثل عرض القطع
                //  فتح نموذج البحث العام
                frmSearch searchForm = new frmSearch(201, SearchEntityType.Supplier);
وهذه  شاشة تسمح له بالاطلاع على القطع وامكانية الحذف لو اراد ولا تكون صلاحية الحذف الا للادمن فقط
            مكيف يكون سيناريو ذلك مع الاحتفاظ بباقى كود الاداة
            cbxUnit_ID_SelectedIndexChanged
 */
            // التأكد من وجود قيمة صالحة
            if (cbxUnit_ID.SelectedValue == null || !int.TryParse(cbxUnit_ID.SelectedValue.ToString(), out int unitId))
                return;

            if (unitId == 1)
            {
                txtMinLenth.ReadOnly = false;
                lblMinLenth.ForeColor = Color.DarkBlue; // اللون الطبيعي
            }
            else
            {
                txtMinLenth.ReadOnly = true;
                txtMinLenth.Text = "0";
                lblMinLenth.ForeColor = Color.Gray; // لون باهت للدلالة على التعطيل

            }
        }

        private void InitializeDecimalInput()
        {
            // منع كتابة الأحرف والرموز
            txtB_Price.KeyPress += OnlyAllowDecimalInput;//Nullability of reference types in type of parameter 'sender' of 'void frm_ProductModify.OnlyAllowDecimalInput(object sender, KeyPressEventArgs e)' doesn't match the target delegate 'KeyPressEventHandler' (possibly because of nullability attributes).
            txtU_Price.KeyPress += OnlyAllowDecimalInput;
            txtMinLenth.KeyPress += OnlyAllowDecimalInput;
            txtMinStock.KeyPress += OnlyAllowDecimalInput;

            // تنسيق السعرين عند الخروج من الحقل
            txtB_Price.Leave += FormatDecimalValueOnLeave;
            txtU_Price.Leave += FormatDecimalValueOnLeave;
        }

        private void FormatDecimalValueOnLeave(object? sender, EventArgs e)
        {
            if (sender is not TextBox textBox)
                return;

            if (float.TryParse(textBox.Text, out float value))
            {
                // تنسيق الرقم ليظهر بصيغة 00.00
                textBox.Text = value.ToString("N2");
            }
        }
        private void OnlyAllowDecimalInput(object? sender, KeyPressEventArgs e)
        {
            if (sender is not TextBox textBox)
                return;

            // السماح فقط بالأرقام والنقطة
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // عدم السماح بأكثر من نقطة عشرية
            if (e.KeyChar == '.' && textBox.Text.Contains('.'))
            {
                e.Handled = true;
            }

            // عدم السماح بأن تكون النقطة أول حرف
            if (e.KeyChar == '.' && textBox.SelectionStart == 0)
            {
                e.Handled = true;
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            frmHelp helpForm = new frmHelp(this.Name);
            helpForm.ShowDialog();
        }
    }
}
