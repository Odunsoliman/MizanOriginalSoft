using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MizanOriginalSoft.MainClasses.OriginalClasses 
{
    public static class InputValidationHelper
    {
        // السماح بالأرقام والنقطة العشرية فقط
        public static void AllowOnlyNumbersAndDecimal(object? sender, KeyPressEventArgs e)
        {
            if (sender is not TextBox textBox)
                return; // إذا لم يكن TextBox، لا تكمل

            // السماح بالأرقام والنقطة العشرية فقط
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // السماح بنقطة عشرية واحدة فقط
            if (e.KeyChar == '.' && textBox.Text.Contains('.'))
            {
                e.Handled = true;
            }

            // تأكد من عدم تكرار الاشتراك في الحدث
            textBox.TextChanged -= TextBox_TextChanged;
            textBox.TextChanged += TextBox_TextChanged;
        }

        // دالة مستقلة للتعامل مع TextChanged
        private static void TextBox_TextChanged(object? sender, EventArgs e)
        {
            if (sender is not TextBox textBox)
                return;

            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "0";
                textBox.SelectionStart = textBox.Text.Length; // لإبقاء المؤشر بعد الرقم
            }
        }

        // السماح بالأرقام الصحيحة فقط
        public static void AllowOnlyNumbers(object? sender, KeyPressEventArgs e)
        {
            if (sender is not TextBox textBox)
                return; // إن لم يكن TextBox، لا تكمل

            // السماح بالأرقام فقط
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            // تأكد من عدم تكرار الاشتراك في الحدث
            textBox.TextChanged -= TextBox_TextChanged;
            textBox.TextChanged += TextBox_TextChanged;
        }

     
    }
}
