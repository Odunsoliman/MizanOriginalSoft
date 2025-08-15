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
