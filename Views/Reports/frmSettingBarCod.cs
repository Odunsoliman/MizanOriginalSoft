using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.MainClasses.OriginalClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace MizanOriginalSoft.Views.Reports
{
    public partial class frmSettingBarCod : Form
    {
        private BarcodeGenerator barcodeGen = new BarcodeGenerator(); // كائن واحد للفورم

        public frmSettingBarCod()
        {
            InitializeComponent();
        }

        private void frmSettingBarCod_Load(object sender, EventArgs e)
        {
            /*
             تم رسم الشاشة بهذه الادوات
                private DataGridView DGV;
        private Button btnDeleteAll;
        private Label lblCountTects;
        private Label lbl_CO;
        private RadioButton rdoSheet;
        private RadioButton rdoRoll;
        private Button btnDeleteSelected;
        private Button btnClose;
        private Button btnPrintBarCode;
        private Button btnMinus;
        private TextBox txtCodeProduct;
        private TableLayoutPanel tableLayoutPanel4;
        private Button btnPlus;
        private Label label1;
        private TableLayoutPanel tableLayoutPanel5;
        private Label lblNameProd;
        private TextBox txtAmount;
        private TableLayoutPanel tableLayoutPanel6;
        private TableLayoutPanel tableLayoutPanel3;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel1;
    
            الهدف هو الاعداد لطباعة باركود او المعاينة بطريقتين

            اما تكون الطباعة على رول مواصفات هوامشه فى ملف تكست موجود مواصفاتها فى كلاس 
             */
        }




        private void txtAmount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // التحقق من أن الحقول ليست فارغة
                if (string.IsNullOrEmpty(txtCodeProduct.Text) || string.IsNullOrEmpty(txtAmount.Text))
                {
                    MessageBox.Show("الرجاء إدخال كود المنتج والكمية");
                    return;
                }

                try
                {
                    // الحصول على كمية المنتج من الحقل
                    int amount;
                    if (!int.TryParse(txtAmount.Text, out amount))
                    {
                        MessageBox.Show("الرجاء إدخال قيمة رقمية صحيحة للكمية");
                        return;
                    }
                    // استدعاء الإجراء المخزن لتحديث الباركود
              //      DBServiecs.sp_InsertBarcodesToPrint(productId, amount);

                    // تحديث عرض البيانات
                    BarcodesGet();

                    // تفريغ حقل الكمية للاستعداد للإدخال التالي
                    txtAmount.Clear();
                    txtCodeProduct.Clear();
                    txtCodeProduct.Focus();
                    lblNameProd.Text = "";

                }
                catch (Exception ex)
                {
                    MessageBox.Show("حدث خطأ: " + ex.Message);
                }
            }
        }



        private void BarcodesGet()
        {
            DataTable dt = new DataTable();
            dt = DBServiecs.BarcodesGet();
            DGV.DataSource = dt;
            ApplyDGVStyles();
        }

        private void ApplyDGVStyles()
        {
            // 1. ضبط نمط التحكم في حجم الأعمدة لملء المساحة المتاحة
            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 2. إخفاء جميع الأعمدة أولاً كخطوة تحضيرية
            foreach (DataGridViewColumn column in DGV.Columns)
            {
                column.Visible = false;
            }

            // 3. تعريف الأعمدة المراد إظهارها مع خصائص كل منها
            var visibleColumns = new[]
            {
        new {
            Name = "ProductCode",
            Header = "كود الصنف",
            FillWeight = 1,
            Alignment = DataGridViewContentAlignment.MiddleCenter
        },
        new {
            Name = "ProdName",
            Header = "اسم الصنف",
            FillWeight = 3,
            Alignment = DataGridViewContentAlignment.MiddleLeft
        },
        new {
            Name = "U_Price",
            Header = "سعر بيع",
            FillWeight = 1,
            Alignment = DataGridViewContentAlignment.MiddleCenter
        },
        new {
            Name = "Amount",
            Header = "عدد التكت",
            FillWeight = 1,
            Alignment = DataGridViewContentAlignment.MiddleLeft
        },
        new {
            Name = "ProductStock",
            Header = "الرصيد",
            FillWeight = 1,
            Alignment = DataGridViewContentAlignment.MiddleCenter
        }
    };

            // 4. تطبيق الإعدادات على الأعمدة المرئية
            foreach (var col in visibleColumns)
            {
                if (DGV.Columns.Contains(col.Name))
                {
                    var column = DGV.Columns[col.Name];
                    column.Visible = true;
                    column.HeaderText = col.Header;
                    column.FillWeight = col.FillWeight;
                    column.DefaultCellStyle.Alignment = col.Alignment;
                }
            }

            // 5. تنسيقات الخلايا العامة
            DGV.DefaultCellStyle.Font = new Font("Times New Roman", 14);
            DGV.DefaultCellStyle.ForeColor = Color.Blue;
            DGV.DefaultCellStyle.BackColor = Color.LightYellow;

            // 6. تنسيقات رؤوس الأعمدة
            DGV.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Bold);
            DGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // 7. معالجة تنسيق الخلايا
            DGV.CellFormatting += (sender, e) =>
            {
                if (e is null || e.Value is null || e.ColumnIndex < 0 || e.ColumnIndex >= DGV.Columns.Count)
                    return;

                var column = DGV.Columns[e.ColumnIndex];

                if (!column.Visible)
                    return;

                string valueStr = e.Value.ToString()?.Trim() ?? string.Empty;

                if (column.Name == "U_Price" && decimal.TryParse(valueStr, out decimal price))
                {
                    e.Value = price.ToString("N2");
                }
                else
                {
                    e.Value = valueStr;
                }
            };

            // 8. تلوين الصفوف بالتناوب
            DGV.AlternatingRowsDefaultCellStyle.BackColor = Color.LightCyan;

            // 9. تطبيق مظهر خاص حسب الثيم
            ApplyColorTheme();
        }
        private void ApplyColorTheme()
        {
            // الألوان الأساسية للنموذج
            this.BackColor = Color.FromArgb(245, 245, 240); // لون خلفية النموذج الرئيسي - بيج فاتح
            // أزرار التعديل (لون أزرق فاتح مع نص أزرق داكن)
            btnMinus.BackColor = Color.FromArgb(173, 216, 230); // أزرق فاتح (لون السماء)
            btnMinus.ForeColor = Color.DarkBlue; // نص أزرق داكن
            btnMinus.FlatStyle = FlatStyle.Flat; // نمط مسطح

            btnPlus.BackColor = Color.FromArgb(173, 216, 230); // نفس لون زر تعديل العنصر
            btnPlus.ForeColor = Color.DarkBlue;
            btnPlus.FlatStyle = FlatStyle.Flat;

            // زر الحذف (لون أحمر فاتح مع نص داكن)
            btnDeleteSelected.BackColor = Color.FromArgb(255, 200, 200); // أحمر فاتح (لون وردي خفيف)
            btnDeleteSelected.ForeColor = Color.DarkRed; // نص أحمر داكن
            btnDeleteSelected.FlatStyle = FlatStyle.Flat;

            // زر البحث المتقدم (لون أخضر فاتح مع نص داكن)
            btnPrintBarCode.BackColor = Color.FromArgb(200, 255, 200); // أخضر فاتح
            btnPrintBarCode.ForeColor = Color.DarkGreen; // نص أخضر داكن
            btnPrintBarCode.FlatStyle = FlatStyle.Flat;

            // أزرار المساعدة والإضافة الجديدة (لون رمادي مع نص أسود)
            btnClose.BackColor = Color.FromArgb(200, 200, 200); // رمادي فاتح
            btnClose.ForeColor = Color.Black;
            btnClose.FlatStyle = FlatStyle.Flat;

            // إعدادات الشبكة (DataGridView)
            DGV.BackgroundColor = Color.White; // خلفية بيضاء
            DGV.DefaultCellStyle.BackColor = Color.White; // خلفية الخلايا بيضاء
            DGV.DefaultCellStyle.ForeColor = Color.DarkSlateBlue; // نص الخلايا أزرق داكن
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230); // رؤوس الأعمدة رمادي فاتح
            DGV.EnableHeadersVisualStyles = false; // تعطيل الأنماط المرئية الافتراضية للرؤوس
        }


        // تحديث صورة الباركود
        //private void GenerateBarcode()
        //{
        //    string productCode = lblProductCode.Text.Trim();

        //    if (string.IsNullOrWhiteSpace(productCode))
        //    {
        //        MessageBox.Show("الرجاء إدخال كود المنتج.");
        //        return;
        //    }

        //    // توليد باركود صغير الحجم
        //    var barcodeImage = barcodeGen.Generate(productCode, width: 100, height: 30, margin: 1);

        //    PicBarcod.SizeMode = PictureBoxSizeMode.Zoom;
        //    PicBarcod.Image = barcodeImage;
        //}

        //private void btnPrintBarCode_Click(object sender, EventArgs e)
        //{
        //    tblCode = DBServiecs.sp_GetBarcodesToPrint();

        //    if (tblCode == null || tblCode.Rows.Count == 0)
        //    {
        //        MessageBox.Show("لا توجد بيانات للطباعة");
        //        return;
        //    }

        //    LoadSettingsFromFile();

        //    DialogResult result = MessageBox.Show(
        //        "هل تريد الطباعة مباشرة؟ اضغط (Yes)\nلمعاينة التكت قبل الطباعة اضغط (No)",
        //        "اختيار نوع الطباعة",
        //        MessageBoxButtons.YesNoCancel);

        //    if (result == DialogResult.Cancel)
        //        return;

        //    directPrint = (result == DialogResult.Yes);
        //    currentPrintIndex = 0;
        //    printDoc = new PrintDocument();

        //    // حدث الطباعة
        //    printDoc.PrintPage += (s, ev) =>
        //    {
        //        if (currentPrintIndex < tblCode.Rows.Count)
        //        {
        //            string? codeValue = tblCode.Rows[currentPrintIndex]["Barcode"]?.ToString();
        //            if (string.IsNullOrEmpty(codeValue))
        //            {
        //                currentPrintIndex++;
        //                ev.HasMorePages = (currentPrintIndex < tblCode.Rows.Count);
        //                return;
        //            }

        //            // التخلص من الصورة القديمة إذا وجدت
        //            BarcodeGeneratorImage?.Dispose();
        //            BarcodeGeneratorImage = null;

        //            // إنشاء صورة الباركود
        //            BarcodeGeneratorImage = BarcodeGenerator.Generate(codeValue, 250, 80);

        //            // تحديد مكان وحجم الرسم على الورقة
        //            Rectangle targetRect = new Rectangle(50, 50, 250, 80);
        //            var img = BarcodeGeneratorImage;
        //            if (img != null)
        //            {
        //                ev.Graphics.DrawImage(img, targetRect);
        //            }


        //            currentPrintIndex++;
        //            ev.HasMorePages = (currentPrintIndex < tblCode.Rows.Count);
        //        }
        //    };

        //    // اختيار الطابعة
        //    if (rdoRoll.Checked)
        //    {
        //        if (!string.IsNullOrEmpty(rollPrinterName) && IsPrinterInstalled(rollPrinterName))
        //        {
        //            printDoc.PrinterSettings.PrinterName = rollPrinterName;
        //        }
        //        else
        //        {
        //            MessageBox.Show($"الطابعة المحددة للطباعة على الرول غير متوفرة:\n{rollPrinterName}", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }
        //    }
        //    else if (rdoSheet.Checked)
        //    {
        //        if (!string.IsNullOrEmpty(sheetPrinterName) && IsPrinterInstalled(sheetPrinterName))
        //        {
        //            printDoc.PrinterSettings.PrinterName = sheetPrinterName;
        //        }
        //        else
        //        {
        //            MessageBox.Show($"الطابعة المحددة للطباعة على الورق غير متوفرة:\n{sheetPrinterName}", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }
        //    }

        //    // الطباعة أو المعاينة
        //    try
        //    {
        //        if (directPrint)
        //        {
        //            printDoc.Print();
        //        }
        //        else
        //        {
        //            using PrintPreviewDialog preview = new PrintPreviewDialog
        //            {
        //                Document = printDoc
        //            };
        //            preview.ShowDialog();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("حدث خطأ أثناء الطباعة: " + ex.Message);
        //    }
        //}

    }
}
/*
 بناء على الكلاس  كيف يتم ضبط الفورم
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using ZXing;
using ZXing.Common;

namespace MizanOriginalSoft.MainClasses.OriginalClasses
{
    public class BarcodeGenerator
    {
        public Image? BarcodeImage { get; private set; }
        public Image Generate(string code, int width, int height, int margin)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("قيمة الباركود فارغة أو غير صالحة.", nameof(code));

            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Width = width,
                    Height = height,
                    Margin = margin
                }
            };

            var pixelData = writer.Write(code);
            using var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb);
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                                             ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

            try
            {
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            BarcodeImage = (Image)bitmap.Clone();
            return BarcodeImage;
        }

        public void PrintBarcodes(DataTable data, PrinterSettings settings, Rectangle drawArea)
        {
            if (data == null || data.Rows.Count == 0)
                throw new InvalidOperationException("لا توجد بيانات للطباعة.");

            int index = 0;
            PrintDocument pd = new PrintDocument
            {
                PrinterSettings = settings
            };

            pd.PrintPage += (s, ev) =>
            {
                if (ev.Graphics == null)
                    return; // تأمين إضافي

                string? codeValue = data.Rows[index]["Barcode"]?.ToString();

                if (!string.IsNullOrWhiteSpace(codeValue))
                {
                    Image? img = Generate(codeValue, drawArea.Width, drawArea.Height, 2);
                    if (img != null)
                    {
                        ev.Graphics.DrawImage(img, drawArea);
                    }
                }

                index++;
                ev.HasMorePages = (index < data.Rows.Count);
            };


            pd.Print();
        }
    }
}

 */


/*
 لدى هذا الكلاس ولا ادرى مدى فائدته فى الفكرة الاتية
انا اريد انشاء فورم لتجهيز الطباعة للبار كود للاصناف باسم frmSettingBarCod
 والهدف من هذه الشاشة اضافة الاكواد التى اريد طباعتها كبار كود بالعدد المطلوب طباعته من كل كود
  الوسائل المتوفرة
1- طباعة على شيت A4 
2- طباعة على رول
3- يوجد ملف تكست مساعد به اسطر تحدد الهوامش والمقاسات لاختلاف A4 فى عدد القطع التى يمكن طباعتها فى الشيت الواحد
    وكذلك الهوامش وحجم الطباعة على الرول
SheetRows=11
SheetCols=5
SheetMarginTop=24
SheetMarginBottom=24
SheetMarginRight=24
SheetMarginLeft=24
RollLabelWidth=50
RollLabelHeight=25
ويمكن تغير القيم من شاشة اخرى للاعداد واريد هنا قرائة الهوامش من الملف
D:\MizanOriginalSoft\bin\Debug\net8.0-windows\serverConnectionSettings.txt
4- يتم اضافة اعداد الاصناف الى قاعدة البيانات والتى تعرض فى DGV بواسطة الدالة
   والدوال  المساعدة فى تحديث البيانات والتنسيق
 ثم يأتى دور مفتاح تنفيذ الطباعة على الشيت او الرول بما فيهما من اعدادات الهوامش كالاتى


واخيرا هل كود الكلاس منضبط ام يجب تعديله لطباعة الباركود بشكل سهل ومباشر 
وكيف يكون الكود فى الشاشة وارتباطها بالكلاس وبالملف التكست الذى يحوى الهوامش واحجام الورق
 
 */



