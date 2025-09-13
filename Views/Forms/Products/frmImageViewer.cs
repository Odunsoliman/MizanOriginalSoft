using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.Products
{
    public partial class frmImageViewer : Form
    {
        private int _productId;
        private DataTable _photos;  // كل الصور من قاعدة البيانات
        private int _currentIndex = 0;

        public frmImageViewer(int productId)
        {
            InitializeComponent();
            _productId = productId;
            _photos = new DataTable(); // ✅ تهيئة DataTable
        }

        private void frmImageViewer_Load(object sender, EventArgs e)
        {
            LoadPhotos();
            ShowPhoto(0);

            LoadPhotos();
            ShowPhoto(0); // 👈 بداية من أول صورة
            BtnStyl();
            LoadItemData();
        }
        private void BtnStyl()
        {

            // 🔹 زر التالي
            btnNext.Text = "🔄";  // رمز تحديث/تدوير إذا تريد دورة الصور
            btnNext.Font = new Font("Segoe UI Emoji", 12);

            // 🔹 زر الحذف
            btnDeletePhoto.Text = "🗑️"; // رمز سلة مهملات
            btnDeletePhoto.Font = new Font("Segoe UI Emoji", 14, FontStyle.Regular);
            btnDeletePhoto.BackColor = Color.LightCoral; // لون مميز
            btnDeletePhoto.ForeColor = Color.White;      // لون النص
            btnDeletePhoto.FlatStyle = FlatStyle.Flat;   // شكل بسيط
        }

        DataTable _tblProd = new DataTable();
        private void LoadItemData()
        {
            _tblProd = DBServiecs.Product_GetAll();
            DataRow[] rows = _tblProd.Select($"ID_Product = {_productId}");

            if (rows.Length == 0) return;

            DataRow row = rows[0];

            lblCategory.ForeColor = Color.Green;
            lblCategory.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblCategory.Text = $"الفئة: {row["CategoryName"]?.ToString() ?? ""}";

            lblNote.ForeColor = Color.Gray;
            lblNote.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lblNote.Text = $"ملاحظات: {row["NoteProduct"]?.ToString() ?? ""}";

            lblProdName.ForeColor = Color.DarkBlue;
            lblProdName.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblProdName.Text = $"اسم الصنف: {row["ProdName"]?.ToString() ?? ""}";

            lblProductCode.ForeColor = Color.Black;
            lblProductCode.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblProductCode.Text = $"كود الصنف: {row["ProductCode"]?.ToString() ?? ""}";

            lblRegistYear.ForeColor = Color.Black;
            lblRegistYear.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblRegistYear.Text = $"سنة التسجيل: {row["RegistYear"]?.ToString() ?? ""}";

            lblStock.ForeColor = Color.DarkRed;
            lblStock.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblStock.Text = $"المخزون: {row["ProductStock"]?.ToString() ?? ""}";

            lblSuplierID.ForeColor = Color.Purple;
            lblSuplierID.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            lblSuplierID.Text = $"المورد: {row["SuplierID"]?.ToString() ?? ""}";

            lblUPrice.ForeColor = Color.Brown;
            lblUPrice.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            decimal price = 0;
            decimal.TryParse(row["U_Price"]?.ToString(), out price);
            lblUPrice.Text = $"سعر الوحدة: {price:N2} LE";


            // تحميل الصورة الافتراضية
            string defaultPath = row["PicProduct"]?.ToString() ?? "";
            if (File.Exists(defaultPath))
            {
                pictureBoxLarge.Image = Image.FromFile(defaultPath);
                pictureBoxLarge.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                pictureBoxLarge.Image = ImageHelper.CreateTextImage("الصورة غير متوفرة", pictureBoxLarge.Width, pictureBoxLarge.Height);
            }
        }


        // 🔹 تحميل الصور من قاعدة البيانات
        private void LoadPhotos()
        {
            _photos = DBServiecs.Product_GetPhotos(_productId);

            if (_photos.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد صور لهذا المنتج.");
                this.Close();
            }
        }



        private bool _isUpdatingCheckBox = false; // 🔹 متغير للتحكم
        
        // 🔹 عرض صورة معينة حسب الفهرس
        private void ShowPhoto(int index)
        {
            if (_photos.Rows.Count == 0) return;

            if (index < 0 || index >= _photos.Rows.Count)
                index = 0;

            _currentIndex = index;

            var row = _photos.Rows[_currentIndex];
            string path = row["ImagePath"].ToString() ?? "";
            bool isDefault = Convert.ToBoolean(row["IsDefault"]);

            if (File.Exists(path))
            {
                pictureBoxLarge.Image = Image.FromFile(path);
                pictureBoxLarge.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                pictureBoxLarge.Image = null;
            }

            // 🔥 عشان ما يدخلش CheckedChanged بالغلط
            _isUpdatingCheckBox = true;
            chkIsDefault.Checked = isDefault;
            _isUpdatingCheckBox = false;
        }

        // 🔹 عند تغيير التشيك بوكس
        private void chkIsDefault_CheckedChanged(object sender, EventArgs e)
        {
            if (_isUpdatingCheckBox || _photos.Rows.Count == 0) return;

            int photoId = Convert.ToInt32(_photos.Rows[_currentIndex]["PhotoID"]);

            if (chkIsDefault.Checked)
            {
                DBServiecs.Product_SetDefaultPhoto(_productId, photoId);

                foreach (DataRow row in _photos.Rows)
                    row["IsDefault"] = false;

                _photos.Rows[_currentIndex]["IsDefault"] = true;
            }
            else
            {
                chkIsDefault.Checked = true;
                MessageBox.Show("يجب أن تكون هناك صورة افتراضية واحدة على الأقل.");
            }
        }


   
        // 🔹 زر التالي
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_photos.Rows.Count == 0) return;

            int newIndex = (_currentIndex + 1) % _photos.Rows.Count;
            ShowPhoto(newIndex);
        }

        // 🔹 عند تغيير التشيك بوكس
        private void chkIsDefault_CheckedChanged_(object sender, EventArgs e)
        {
            if (_photos.Rows.Count == 0) return;

            int photoId = Convert.ToInt32(_photos.Rows[_currentIndex]["PhotoID"]);

            if (chkIsDefault.Checked)
            {// لا يمر هنا اطلاقا
                DBServiecs.Product_SetDefaultPhoto(_productId, photoId);

                // ✅ تحديث العمود الصحيح
                foreach (DataRow row in _photos.Rows)
                    row["IsDefault"] = false;

                _photos.Rows[_currentIndex]["IsDefault"] = true;
            }
            else
            {
                chkIsDefault.Checked = true;
                MessageBox.Show("يجب أن تكون هناك صورة افتراضية واحدة على الأقل.");
            }
        }


        // 🔹 زر الحذف
        private void btnDeletePhoto_Click(object sender, EventArgs e)
        {
            if (_photos.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد صور لحذفها.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int photoId = Convert.ToInt32(_photos.Rows[_currentIndex]["PhotoID"]);

            var confirm = MessageBox.Show(
                "هل أنت متأكد أنك تريد حذف هذه الصورة؟",
                "تأكيد الحذف",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    bool result = DBServiecs.Product_DeletePhoto(photoId);

                    if (result) // ✅ تحقق من النتيجة كـ bool
                    {
                        MessageBox.Show("تم حذف الصورة بنجاح.", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        _photos = DBServiecs.Product_GetPhotos(_productId);

                        if (_photos.Rows.Count > 0)
                        {
                            int newIndex = _currentIndex >= _photos.Rows.Count ? _photos.Rows.Count - 1 : _currentIndex;
                            ShowPhoto(newIndex);
                        }
                        else
                        {
                            pictureBoxLarge.Image = null;
                            _currentIndex = 0;
                        }
                    }
                    else
                    {
                        MessageBox.Show("حدث خطأ أثناء الحذف.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("حدث خطأ:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}




