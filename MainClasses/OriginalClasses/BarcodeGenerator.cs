using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Windows.Forms;
using ZXing;
using ZXing.Common;

namespace MizanOriginalSoft.MainClasses.OriginalClasses
{
    public static class BarcodeGenerator
    {
        private static Image? _barcodeImage; // الآن يمكن أن تكون null بشكل صريح

        // توليد الباركود فقط
        public static Image Generate(string code, int width = 300, int height = 100, int margin = 2)
        {
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
            using Bitmap bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb);
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

            _barcodeImage = (Image)bitmap.Clone();
            return _barcodeImage;
        }

        // حفظ الباركود كـ PNG
        public static void SaveAsPng(string filePath)
        {
            if (_barcodeImage != null)
            {
                _barcodeImage.Save(filePath, ImageFormat.Png);
            }
            else
            {
                throw new InvalidOperationException("لم يتم إنشاء باركود لحفظه.");
            }
        }

        // طباعة الباركود مباشرة
        public static void Print()
        {
            if (_barcodeImage == null)
                throw new InvalidOperationException("لم يتم إنشاء باركود للطباعة.");

            Image imageToPrint = _barcodeImage;

            using PrintDocument pd = new PrintDocument();

            pd.PrintPage += delegate (object sender, PrintPageEventArgs e)
            {
                e.Graphics!.DrawImage(imageToPrint, new Point(100, 100));
            };

            using PrintDialog printDialog = new PrintDialog
            {
                Document = pd
            };

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }

    }
}
