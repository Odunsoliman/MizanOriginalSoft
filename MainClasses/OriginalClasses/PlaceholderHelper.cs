using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MizanOriginalSoft .MainClasses .OriginalClasses 
{
    public static class PlaceholderHelper
    {
        public static void SetPlaceholder(TextBox textBox, string placeholderText)
        {
            textBox.Tag = placeholderText;

            // إذا كان النص حقيقي (محمّل من قاعدة البيانات مثلاً)
            if (!string.IsNullOrWhiteSpace(textBox.Text) && textBox.Text != placeholderText)
            {
                textBox.ForeColor = Color.Black;
            }
            else
            {
                textBox.Text = placeholderText;
                textBox.ForeColor = Color.Gray;
            }

            // ضبط الاتجاه تلقائيًا إذا كان النص يحتوي على أحرف عربية
            textBox.RightToLeft = ContainsArabic(placeholderText) ? RightToLeft.Yes : RightToLeft.No;

            textBox.Enter += (s, e) =>
            {
                if (textBox.Text == placeholderText)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };

            textBox.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholderText;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }

        public static string GetRealText(TextBox textBox)
        {
            string placeholder = textBox.Tag?.ToString() ?? "";
            return textBox.Text == placeholder ? "" : textBox.Text;
        }

        private static bool ContainsArabic(string text)
        {
            return text.Any(c => c >= 0x0600 && c <= 0x06FF);
        }
    }

}


/*
     
تعليقات شارحة للاصتخدام للكلاس
----------------------------
 طرقيقة الاستخدام  في الفورم

private void Form_Load(object sender, EventArgs e)
{
    PlaceholderHelper.SetPlaceholder(txtAccNameضغط هنا, "ا Ctrl+F لاختيار الحساب");
    PlaceholderHelper.SetPlaceholder(txtAnotherBox, "أدخل رقم الفاتورة");
}


عند استخدام القيمة مثلاً عند الحفظ

string accName = PlaceholderHelper.GetRealText(txtAccName);

 */