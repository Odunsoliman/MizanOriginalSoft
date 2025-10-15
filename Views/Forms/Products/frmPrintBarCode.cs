using System.Drawing;
using ZXing;
using ZXing.Common;

using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.IO;
using FormatException = System.FormatException;

using System.Xml.Linq;
using MizanOriginalSoft.MainClasses;
using ZXing.Windows.Compatibility;

namespace Signee.Views.Forms.Products
{
    public partial class frmPrintBarCode : Form
    {

        #region ---  تحميل الشاشة
        public frmPrintBarCode()
        {
            InitializeComponent();
        }
        private void frmPrintBarCode_Load(object sender, EventArgs e)
        {
            BarcodesGet();
            LoadSettingsFromFile();
            lblCountTects .Text = (sheetRows * sheetCols) + " لاصقة بالورقة".ToString ();
            lbl_CO .Text = companyName;

        }


        #endregion
 
        
        
      #region @@@@@@@ barcode document Zxing @@@@@@@    
        private void BarcodesGet()
        {
            DataTable dt = new DataTable();
            dt = DBServiecs.BarcodesGet();
            DGV.DataSource = dt;
            ApplyDGVStyles();
        }




        private string rollPrinterName = string.Empty;
        private string sheetPrinterName = string.Empty;

        private bool directPrint;
        private int currentPrintIndex;
        private DataTable tblCode = new DataTable();
        private PrintDocument printDoc = new PrintDocument();

        int sheetRows;
        int sheetCols;
        int sheetMarginTop;
        int sheetMarginBottom;
        int sheetMarginRight;
        int sheetMarginLeft;

        private int rollLabelWidth = 100;
        private int rollLabelHeight = 50;
        private string? companyName;


        //زر الطباعة المباشرة او المعاينه على رول او شيت مقسم الى تكتس A4 
        private void btnPrintBarCode_Click(object sender, EventArgs e)
        {
            tblCode = DBServiecs.sp_GetBarcodesToPrint();

            if (tblCode == null || tblCode.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد بيانات للطباعة");
                return;
            }

            LoadSettingsFromFile();

            DialogResult result = MessageBox.Show(
                "هل تريد الطباعة مباشرة؟ اضغط (Yes)\nلمعاينة التكت قبل الطباعة اضغط (No)",
                "اختيار نوع الطباعة",
                MessageBoxButtons.YesNoCancel);

            if (result == DialogResult.Cancel)
                return;

            directPrint = (result == DialogResult.Yes);
            currentPrintIndex = 0;
            printDoc = new PrintDocument();

            if (rdoRoll.Checked)
            {
                if (!string.IsNullOrEmpty(rollPrinterName) && IsPrinterInstalled(rollPrinterName))
                {
                    PrepareRollPrinting();
                }
                else
                {
                    MessageBox.Show($"الطابعة المحددة للطباعة على الرول غير متوفرة:\n{rollPrinterName}", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else if (rdoSheet.Checked)
            {
                if (!string.IsNullOrEmpty(sheetPrinterName) && IsPrinterInstalled(sheetPrinterName))
                {
                    PrepareSheetPrinting();
                }
                else
                {
                    MessageBox.Show($"الطابعة المحددة للطباعة على الورق غير متوفرة:\n{sheetPrinterName}", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            try
            {
                if (directPrint)
                {
                    printDoc.Print();
                }
                else
                {
                    PrintPreviewDialog preview = new PrintPreviewDialog();
                    preview.Document = printDoc;
                    preview.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء الطباعة: " + ex.Message);
            }
        }


        // استدعاء الإعدادات من الملف
        private void LoadSettingsFromFile()
        {
            string path = Path.Combine(Application.StartupPath, "serverConnectionSettings.txt");

            if (!File.Exists(path))
            {
                MessageBox.Show("ملف إعدادات الاتصال غير موجود: " + path);
                return;
            }

            string[] lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {
                if (line.StartsWith("RollPrinter="))
                    rollPrinterName = line.Substring("RollPrinter=".Length).Trim();
                else if (line.StartsWith("SheetPrinter="))
                    sheetPrinterName = line.Substring("SheetPrinter=".Length).Trim();
                else if (line.StartsWith("SheetRows="))
                    int.TryParse(line.Substring("SheetRows=".Length).Trim(), out sheetRows);
                else if (line.StartsWith("SheetCols="))
                    int.TryParse(line.Substring("SheetCols=".Length).Trim(), out sheetCols);
                else if (line.StartsWith("SheetMarginTop="))
                    int.TryParse(line.Substring("SheetMarginTop=".Length).Trim(), out sheetMarginTop);
                else if (line.StartsWith("SheetMarginBottom="))
                    int.TryParse(line.Substring("SheetMarginBottom=".Length).Trim(), out sheetMarginBottom);
                else if (line.StartsWith("SheetMarginRight="))
                    int.TryParse(line.Substring("SheetMarginRight=".Length).Trim(), out sheetMarginRight);
                else if (line.StartsWith("SheetMarginLeft="))
                    int.TryParse(line.Substring("SheetMarginLeft=".Length).Trim(), out sheetMarginLeft);
                else if (line.StartsWith("RollLabelWidth="))
                    int.TryParse(line.Substring("RollLabelWidth=".Length).Trim(), out rollLabelWidth);
                else if (line.StartsWith("RollLabelHeight="))
                    int.TryParse(line.Substring("RollLabelHeight=".Length).Trim(), out rollLabelHeight);
                else if (line.StartsWith("CompanyName="))
                    companyName = line.Substring("CompanyName=".Length).Trim();//
            }

            // إذا تم قراءة اسم الشركة بنجاح، قم بعرضه في التسمية (Label)
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                lbl_CO.Text = companyName;
            }
        }

        private Image GenerateBarcode(string code)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Height = 40,
                    Width = 130,
                    Margin = 0
                }
            };

            return writer.Write(code);
        }

        //اعداد الشيت للطباعة
        private void PrepareSheetPrinting()
        {
            printDoc.PrintPage += PrintLabels_A4Sheet;

            // تحويل حجم الورقة من mm إلى وحدة الطباعة (1 مم ≈ 3.937)
            int width = (int)(210 * 3.937);   // عرض A4 بالـ mm
            int height = (int)(297 * 3.937);  // طول A4 بالـ mm
            printDoc.DefaultPageSettings.PaperSize = new PaperSize("A4_MM", width, height);

            // ضبط الهوامش باستخدام القيم المحملة من الملف، مع التحويل من mm إلى وحدة الطباعة
            int marginLeft = (int)(sheetMarginLeft * 3.937);
            int marginRight = (int)(sheetMarginRight * 3.937);
            int marginTop = (int)(sheetMarginTop * 3.937);
            int marginBottom = (int)(sheetMarginBottom * 3.937);
            printDoc.DefaultPageSettings.Margins = new Margins(marginLeft, marginRight, marginTop, marginBottom);

            // تعيين اسم الطابعة
            printDoc.PrinterSettings.PrinterName = sheetPrinterName;
        }

        //طباعة التكت فى الشيت
        private void PrintLabels_A4Sheet(object sender, PrintPageEventArgs e)
        {
            int labelsPerRow = sheetCols;
            int labelsPerColumn = sheetRows;
            int labelsPerPage = labelsPerRow * labelsPerColumn;

            int printableWidth = e.MarginBounds.Width;
            int printableHeight = e.MarginBounds.Height;

            int labelWidth = printableWidth / labelsPerRow;
            int labelHeight = printableHeight / labelsPerColumn;

            Graphics? g = e.Graphics!;

            using (Font fontBig = new Font("Times New Roman", 8, FontStyle.Bold))
            using (Font fontSmall = new Font("Times New Roman", 7, FontStyle.Regular))
            using (Font fontSmaller = new Font("Times New Roman", 6.5f, FontStyle.Regular))
            {
                for (int row = 0; row < labelsPerColumn; row++)
                {
                    for (int col = 0; col < labelsPerRow; col++)
                    {
                        if (currentPrintIndex >= tblCode.Rows.Count)
                        {
                            e.HasMorePages = false;
                            return;
                        }

                        DataRow dataRow = tblCode.Rows[currentPrintIndex];
                        string? productCode = dataRow["ProductCode"].ToString();
                        string? prodName = dataRow["ProdName"].ToString();
                        string? price = Convert.ToDecimal(dataRow["U_Price"]).ToString("0.00");
                        string? suplierID = dataRow["SuplierID"].ToString();
                        string? fixedText = lbl_CO.Text;

                        int x = e.MarginBounds.Left + col * labelWidth;
                        int y = e.MarginBounds.Top + row * labelHeight;
                        Rectangle labelRect = new Rectangle(x, y, labelWidth, labelHeight);

                        int spacing = 2;
                        int barcodeHeight = 40;
                        int textLineHeight = (int)g.MeasureString(price, fontSmall!).Height;

                        // تقسيم اسم المنتج إلى سطر أو سطرين حسب العرض
                        float maxTextWidth = labelWidth - 10; // هامش داخلي
                        List<string> nameLines = SplitTextToFitLines(g, prodName ?? string.Empty, fontBig, fontSmaller, maxTextWidth, 2);

                        // حساب ارتفاع النص
                        float nameTotalHeight = 0;
                        foreach (var line in nameLines)
                        {
                            Font f = (nameLines.IndexOf(line) == 0) ? fontBig : fontSmaller;
                            nameTotalHeight += g.MeasureString(line, f).Height;
                        }

                        // حساب المسافة العلوية لتوسيط العناصر داخل التسمية
                        int topOffset = (labelHeight - (int)nameTotalHeight - spacing - barcodeHeight - spacing - textLineHeight) / 2;
                        float currentY = y + topOffset;

                        // رسم كل سطر من الاسم
                        foreach (var line in nameLines)
                        {
                            Font f = (nameLines.IndexOf(line) == 0) ? fontBig : fontSmaller;
                            SizeF size = g.MeasureString(line, f);
                            float textX = x + (labelWidth - size.Width) / 2;
                            g.DrawString(line, f, Brushes.Black, new PointF(textX, currentY));
                            currentY += size.Height;
                        }

                        // رسم الباركود
                        currentY += spacing;
                        Image barcodeImg = GenerateBarcode(productCode ?? string.Empty);

                        int barcodeX = x + (labelWidth - 130) / 2;
                        g.DrawImage(barcodeImg, barcodeX, (int)currentY, 130, barcodeHeight);

                        // رسم السطر الثالث (السعر - الشركة - المورد)
                        currentY += barcodeHeight + spacing;
                        g.DrawString(price + " LE", fontSmall, Brushes.Black, new PointF(x + 5, currentY));

                        SizeF coSize = g.MeasureString(fixedText, fontSmall);
                        float centerX = x + (labelWidth - coSize.Width) / 2;
                        g.DrawString(fixedText, fontSmall, Brushes.Black, new PointF(centerX, currentY));

                        SizeF supSize = g.MeasureString(suplierID, fontSmall);
                        float rightX = x + labelWidth - supSize.Width - 5;
                        g.DrawString(suplierID, fontSmall, Brushes.Black, new PointF(rightX, currentY));

                        currentPrintIndex++;
                    }
                }

                e.HasMorePages = currentPrintIndex < tblCode.Rows.Count;
            }
        }

        // تقسيم النص إلى سطر أو سطرين حسب العرض الفعلي فى الشيت
        private List<string> SplitTextToFitLines(Graphics g, string text, Font font1, Font font2, float maxWidth, int maxLines = 2)
        {
            List<string> lines = new List<string>();
            string[] words = text.Split(' ');
            string currentLine = "";

            for (int i = 0; i < words.Length; i++)
            {
                string testLine = string.IsNullOrEmpty(currentLine) ? words[i] : currentLine + " " + words[i];
                SizeF size = g.MeasureString(testLine, lines.Count == 0 ? font1 : font2);

                if (size.Width <= maxWidth)
                {
                    currentLine = testLine;
                }
                else
                {
                    lines.Add(currentLine.Trim());
                    currentLine = words[i];

                    if (lines.Count == maxLines - 1)
                    {
                        break; // لا نسمح بأكثر من سطرين
                    }
                }
            }

            if (lines.Count < maxLines && !string.IsNullOrEmpty(currentLine))
            {
                lines.Add(currentLine.Trim());
            }

            return lines;
        }

        //اختبار وجود طابعة مثبتة
        private bool IsPrinterInstalled(string printerName)
        {
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                if (printer.Equals(printerName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        // إعداد الرول للطباعة
        private void PrepareRollPrinting()
        {
            printDoc.PrintPage += PrintLabel_Roll;

            // تحويل العرض والارتفاع من ملم إلى 1/100 إنش (الوحدة المطلوبة في PaperSize)
            int width = (int)(rollLabelWidth * 3.937);
            int height = (int)(rollLabelHeight * 3.937);

            printDoc.DefaultPageSettings.PaperSize = new PaperSize("RollLabel_MM", width, height);

            // يمكن تخصيص الهوامش لاحقًا إذا أردت إضافتها من الإعدادات
            printDoc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

            printDoc.PrinterSettings.PrinterName = rollPrinterName;
        }

        //طباعة التكت فى الرول
        private void PrintLabel_Roll(object sender, PrintPageEventArgs e)
        {
            if (currentPrintIndex >= tblCode.Rows.Count)
            {
                e.HasMorePages = false;
                return;
            }

            DataRow row = tblCode.Rows[currentPrintIndex];

            string? productCode = row["ProductCode"].ToString();
            string? prodName = row["ProdName"].ToString();
            string price = Convert.ToDecimal(row["U_Price"]).ToString("0.00");
            string? suplierID = row["SuplierID"].ToString();
            string fixedText = lbl_CO.Text;

            Graphics? g = e.Graphics;
            int labelWidth = e.PageBounds.Width;
            int labelHeight = e.PageBounds.Height;

            using (Font fontBig = new Font("Times New Roman", 9, FontStyle.Bold))
            using (Font fontSmall = new Font("Times New Roman", 8, FontStyle.Regular))
            using (Font fontSmaller = new Font("Times New Roman", 7.5f, FontStyle.Regular))
            {
                int spacing = 2;
                int barcodeHeight = 40;

                // تقسيم الاسم إلى سطر أو سطرين كحد أقصى باستخدام الدالة المخصصة
                List<string> nameLines = SplitTextToFitLinesRoll(g!, prodName ?? string.Empty, fontBig, labelWidth - 10, 2);

                string line1 = nameLines.Count > 0 ? nameLines[0] : "";
                string line2 = nameLines.Count > 1 ? nameLines[1] : "";

                // قياس ارتفاع السطور
                SizeF line1Size = g!.MeasureString(line1, fontBig);
                SizeF line2Size = string.IsNullOrEmpty(line2) ? SizeF.Empty : g.MeasureString(line2, fontSmaller);

                int nameHeightTotal = (int)line1Size.Height + (int)line2Size.Height + (string.IsNullOrEmpty(line2) ? 0 : 1);

                // قياس ارتفاع السطر الثالث
                int textLineHeight = (int)g.MeasureString(price, fontBig).Height;

                // إجمالي ارتفاع المحتوى
                int totalContentHeight = nameHeightTotal + spacing + barcodeHeight + spacing + textLineHeight;

                int topMargin = (labelHeight - totalContentHeight) / 2;

                // رسم السطر الأول
                float line1X = (labelWidth - line1Size.Width) / 2;
                g.DrawString(line1, fontBig, Brushes.Black, new PointF(line1X, topMargin));

                float nameY = topMargin + line1Size.Height + 1;

                // رسم السطر الثاني إن وجد
                if (!string.IsNullOrEmpty(line2))
                {
                    float line2X = (labelWidth - line2Size.Width) / 2;
                    g.DrawString(line2, fontSmaller, Brushes.Black, new PointF(line2X, nameY));
                }

                // رسم الباركود
                int barcodeY = topMargin + nameHeightTotal + spacing;
                
                Image barcodeImg = GenerateBarcode(productCode ?? string.Empty);


                int barcodeX = (labelWidth - 130) / 2;
                g.DrawImage(barcodeImg, barcodeX, barcodeY, 130, barcodeHeight);

                // السطر الثالث
                float textY = barcodeY + barcodeHeight + spacing;

                g.DrawString(price + " LE", fontBig, Brushes.Black, new PointF(5, textY));

                SizeF centerTextSize = g.MeasureString(fixedText, fontSmall);
                float centerX = (labelWidth - centerTextSize.Width) / 2;
                g.DrawString(fixedText, fontSmall, Brushes.Black, new PointF(centerX, textY));

                SizeF rightTextSize = g.MeasureString(suplierID, fontSmall);
                float rightX = labelWidth - rightTextSize.Width - 5;
                g.DrawString(suplierID, fontSmall, Brushes.Black, new PointF(rightX, textY));
            }

            currentPrintIndex++;
            e.HasMorePages = (currentPrintIndex < tblCode.Rows.Count);
        }

        private List<string> SplitTextToFitLinesRoll(Graphics g, string text, Font font, int maxWidth, int maxLines)
        {
            List<string> lines = new List<string>();
            string[] words = text.Split(' ');
            string currentLine = "";

            foreach (string word in words)
            {
                string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                SizeF size = g.MeasureString(testLine, font);

                if (size.Width <= maxWidth)
                {
                    currentLine = testLine;
                }
                else
                {
                    if (!string.IsNullOrEmpty(currentLine))
                    {
                        lines.Add(currentLine);
                        if (lines.Count >= maxLines)
                            return lines;
                    }
                    currentLine = word;
                }
            }

            if (!string.IsNullOrEmpty(currentLine) && lines.Count < maxLines)
            {
                lines.Add(currentLine);
            }

            return lines;
        }



        #endregion


        #region --- تنسيق الجريد ------
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
        // العمود الأول: كود المنتج
        new {
            Name = "ProductCode",         // اسم العمود في مصدر البيانات
            Header = "كود الصنف",         // النص المعروض في رأس العمود
            FillWeight = 1,                // نسبة العرض النسبي (1 = أصغر عرض)
            Alignment = DataGridViewContentAlignment.MiddleCenter // محاذاة النص في المنتصف
        },
        
        // العمود الثاني: اسم المنتج (سيكون الأوسع)
        new {
            Name = "ProdName",
            Header = "اسم الصنف",
            FillWeight = 3,                // عرض أكبر (4 أضعاف العمود الأول)
            Alignment = DataGridViewContentAlignment.MiddleLeft   // محاذاة لليسار
        },
              
       
        // العمود الرابع: سعر البيع
        new {
            Name = "U_Price",
            Header = "سعر بيع",
            FillWeight = 1,
            Alignment = DataGridViewContentAlignment.MiddleCenter
        },
        
        // العمود السادس: التصنيف
        new {
            Name = "Amount",
            Header = "عدد التكت",
            FillWeight = 1,
            Alignment = DataGridViewContentAlignment.MiddleLeft
        },
        
        // العمود السابع: الرصيد المتاح
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
                // التحقق من وجود العمود في DataGridView قبل تعديله
                if (DGV.Columns.Contains(col.Name))
                {
                    DGV.Columns[col.Name].Visible = true;           // جعل العمود مرئياً
                    DGV.Columns[col.Name].HeaderText = col.Header;  // تعيين نص الرأس
                    DGV.Columns[col.Name].FillWeight = col.FillWeight; // تحديد العرض النسبي
                    DGV.Columns[col.Name].DefaultCellStyle.Alignment = col.Alignment; // ضبط المحاذاة
                }
            }

            // 5. تنسيقات الخلايا العامة
            DGV.DefaultCellStyle.Font = new Font("Times New Roman", 14);  // خط الخلايا الأساسي
            DGV.DefaultCellStyle.ForeColor = Color.Blue;                 // لون النص الأزرق
            DGV.DefaultCellStyle.BackColor = Color.LightYellow;          // خلفية صفراء فاتحة

            // 6. تنسيقات رؤوس الأعمدة
            DGV.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Bold); // خط عريض
            DGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;      // لون نص أزرق
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray; // خلفية رمادية فاتحة
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // محاذاة المنتصف

            // 8. معالجة النصوص في الخلايا لإزالة المسافات الزائدة
            DGV.CellFormatting += (sender, e) =>
            {
                // التحقق من أن العمود مرئي وأن الخلية تحتوي على قيمة
                if (DGV.Columns[e.ColumnIndex].Visible && e.Value != null)
                {
                    // إزالة المسافات من بداية ونهاية النص
                    e.Value = (e.Value?.ToString() ?? string.Empty).Trim();

                    // (إضافة اختيارية) معالجة خاصة لعمود السعر
                    if (DGV.Columns[e.ColumnIndex].Name == "U_Price")
                    {
                        // تنسيق الأرقام إذا لزم الأمر
                        if (decimal.TryParse(e.Value.ToString(), out decimal price))
                        {
                            e.Value = price.ToString("N2"); // عرض برقمين عشريين
                        }
                    }
                }
            };

            // (إضافة اختيارية) جعل الصفوف متناوبة الألوان
            DGV.AlternatingRowsDefaultCellStyle.BackColor = Color.LightCyan;
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

        #endregion

        #region ----  ازرار الشاشة -----
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPlus_Click(object sender, EventArgs e)
        {
            try
            {
                // التحقق من أن الحقل ليس فارغاً
                if (string.IsNullOrEmpty(txtAmount.Text))
                {
                    txtAmount.Text = "0";
                }

                // تحويل النص إلى رقم وإضافة 1
                int currentAmount = int.Parse(txtAmount.Text);
                currentAmount++;

                // تحديث النص بالقيمة الجديدة
                txtAmount.Text = currentAmount.ToString();
            }
            catch (FormatException)
            {
                MessageBox.Show("الرجاء إدخال قيمة رقمية صحيحة");
                txtAmount.Text = "0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ: " + ex.Message);
            }
        }

        private void btnMinus_Click(object sender, EventArgs e)
        {
            try
            {
                // التحقق من أن الحقل ليس فارغاً
                if (string.IsNullOrEmpty(txtAmount.Text))
                {
                    txtAmount.Text = "0";
                }

                // تحويل النص إلى رقم وطرح 1
                int currentAmount = int.Parse(txtAmount.Text);

                // التأكد من عدم النزول تحت الصفر
                if (currentAmount > 0)
                {
                    currentAmount--;
                }
                else
                {
                    MessageBox.Show("لا يمكن أن تكون الكمية أقل من الصفر");
                }

                // تحديث النص بالقيمة الجديدة
                txtAmount.Text = currentAmount.ToString();
            }
            catch (FormatException)
            {
                MessageBox.Show("الرجاء إدخال قيمة رقمية صحيحة");
                txtAmount.Text = "0";
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ: " + ex.Message);
            }
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
                    DBServiecs.sp_InsertBarcodesToPrint(productId, amount);

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
        private int productId;
        private void txtCodeProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // التحقق من أن الحقول ليست فارغة
                if (string.IsNullOrEmpty(txtCodeProduct.Text))
                {
                    MessageBox.Show("الرجاء إدخال كود المنتج ");
                    txtCodeProduct.Focus();
                    txtCodeProduct.SelectAll();
                    lblNameProd.Text = "";
                    return;
                }

                try
                {
                    // الحصول على كود المنتج من الحقل
                    string productCode = txtCodeProduct.Text.Trim();
                    // استدعاء الدالة للحصول على معرف المنتج
                    DataTable productData = DBServiecs.Product_GetIDByCode(productCode);

                    if (productData == null || productData.Rows.Count == 0)
                    {
                        MessageBox.Show("لم يتم العثور على المنتج");
                        txtCodeProduct.Focus();
                        txtCodeProduct.SelectAll();
                        lblNameProd.Text = "";
                        return;
                    }

                    // الحصول على معرف المنتج
                    productId = Convert.ToInt32(productData.Rows[0]["ID_Product"]);
                    lblNameProd.Text = productData.Rows[0]["ProdName"].ToString();
                    txtAmount.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("حدث خطأ: " + ex.Message);
                }
            }
        }
//        bool check;
        private void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            // 1. التحقق من وجود صفوف محددة للحذف
            if (DGV.SelectedRows.Count == 0)
            {
                MessageBox.Show("الرجاء تحديد عنصر واحد على الأقل للحذف", "لا توجد عناصر محددة",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. طلب تأكيد من المستخدم قبل الحذف
            var confirmResult = MessageBox.Show("هل أنت متأكد من حذف العناصر المحددة؟",
                                               "تأكيد الحذف",
                                               MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // إذا لم يؤكد المستخدم، نخرج من الدالة دون تنفيذ الحذف
            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            try
            {
                // 3. إنشاء جدول بيانات لتخزين الأرقام التعريفية للعناصر المحددة
                DataTable productIds = new DataTable();
                productIds.Columns.Add("ID_Product", typeof(int)); // إنشاء عمود للأرقام التعريفية

                // 4. ملء الجدول بالأرقام التعريفية للعناصر المحددة
                foreach (DataGridViewRow row in DGV.SelectedRows)
                {
                    // نضيف كل رقم تعريفي إلى الجدول
                    // ملاحظة: "ProductID" يجب أن يستبدل باسم العمود الحقيقي في قاعدة البيانات
                    productIds.Rows.Add(row.Cells["ID_Product"].Value);
                }

                // 5. استدعاء الدالة المسؤولة عن الحذف في قاعدة البيانات
                DBServiecs.sp_DeleteBarcodesByProductIDs(productIds);

                // إذا نجح الحذف، نعرض رسالة نجاح
                MessageBox.Show("تم حذف العناصر المحددة بنجاح", "تمت العملية",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 7. تحديث البيانات المعروضة بعد الحذف
                BarcodesGet();
            }
            catch (Exception ex)
            {
                // 8. معالجة أي أخطاء غير متوقعة
                MessageBox.Show($"حدث خطأ: {ex.Message}", "خطأ",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            // 1. التأكد من وجود بيانات في DataGridView
            if (DGV.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد بيانات لحذفها", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 2. تأكيد نية الحذف من المستخدم
            var confirmResult = MessageBox.Show("هل أنت متأكد أنك تريد حذف كل العناصر؟",
                                                "تأكيد الحذف الكلي",
                                                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirmResult != DialogResult.Yes)
            {
                return;
            }

            try
            {
                // 3. إنشاء DataTable لتجميع كل المعرفات
                DataTable productIds = new DataTable();
                productIds.Columns.Add("ID_Product", typeof(int));

                // 4. تعبئة الجدول بكل المعرفات في DGV
                foreach (DataGridViewRow row in DGV.Rows)
                {
                    if (row.IsNewRow) continue; // نتجاهل الصف الجديد (المخصص للإضافة)
                    productIds.Rows.Add(row.Cells["ID_Product"].Value);
                }

                // 5. تنفيذ الحذف في قاعدة البيانات
                DBServiecs.sp_DeleteBarcodesByProductIDs(productIds);

                // 6. إعلام المستخدم
                MessageBox.Show("تم حذف جميع العناصر بنجاح", "تمت العملية", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 7. تحديث البيانات
                BarcodesGet();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion


    }
}
