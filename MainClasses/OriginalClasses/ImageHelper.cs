
 using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace MizanOriginalSoft.MainClasses.OriginalClasses
{
    public static class ImageHelper
    {
        /// <summary>
        /// توليد صورة تحتوي على نص معين (تستخدم كصورة بديلة عند غياب الصورة الأصلية)
        /// </summary>
        /// <param name="text">النص المطلوب عرضه داخل الصورة</param>
        /// <param name="width">عرض الصورة</param>
        /// <param name="height">ارتفاع الصورة</param>
        /// <param name="backgroundColor">لون الخلفية (اختياري)</param>
        /// <param name="textColor">لون النص (اختياري)</param>
        /// <returns>كائن صورة Image</returns>
        public static Image CreateTextImage(
            string text,
            int width,
            int height,
            Color? backgroundColor = null,
            Color? textColor = null)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(backgroundColor ?? Color.LightGray);

                using Font font = new Font("Arial", 14, FontStyle.Bold);
                using Brush brush = new SolidBrush(textColor ?? Color.DarkRed);
                StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                g.DrawString(text, font, brush, new RectangleF(0, 0, width, height), format);
            }

            return bmp;
        }
    }
}
