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
        private readonly int _productId;

        public frmImageViewer(int productId)
        {
            InitializeComponent();
            _productId = productId;
        }
        //لماذا لا يمر على frmImageViewer_Load
        private void frmImageViewer_Load(object sender, EventArgs e)
        {
            LoadProductDetails(_productId);
        }

        private void LoadProductDetails(int idProduct)
        {
            DataTable dt = DBServiecs.Product_GetPicDetailsByID(idProduct);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("لم يتم العثور على بيانات المنتج.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRow row = dt.Rows[0];
            //
            lblProdName.Text = row["ProdName"]?.ToString();
            lblProductCode.Text = $"الكود: {row["ProductCode"]?.ToString()}";
            string? categoryName = row["CategoryName"]?.ToString()?.Trim();

            if (string.IsNullOrEmpty(categoryName) || categoryName == ".")
            {
                lblCategory.Text = "التصنيف: عام";
            }
            else
            {
                lblCategory.Text = $"التصنيف: {categoryName}";
            }

            lblRegistYear.Text = $"سنة: {row["RegistYear"]?.ToString()}";
            lblUPrice.Text = $"السعر: {row["U_Price"]?.ToString()}";
            lblStock.Text = $"المخزون: {row["ProductStock"]?.ToString()}";
            lblNote.Text = row["NoteProduct"]?.ToString();
            lblSuplierID.Text = row["SuplierID"]?.ToString();

            string? imagePath = row["PicProduct"]?.ToString();
            if (!string.IsNullOrWhiteSpace(imagePath) && File.Exists(imagePath))
            {
                pictureBoxLarge.Image = Image.FromFile(imagePath);

                /*اريد اظهار الصورة فى منتصف عرضيا وطوليا pictureBoxLarge*/
            }
            else
            {
                pictureBoxLarge.Image = ImageHelper.CreateTextImage("الصورة غير متوفرة", pictureBoxLarge.Width, pictureBoxLarge.Height);
            }
        }

   
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

 
 
 
