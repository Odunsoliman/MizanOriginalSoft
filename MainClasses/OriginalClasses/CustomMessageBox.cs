using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MizanOriginalSoft.MainClasses.OriginalClasses
{
    public static class CustomMessageBox
    {
         private static Font messageFont = new Font("Times New Roman", 14, FontStyle.Bold); // تحديد الخط كـ Bold

        public static DialogResult ShowWarning(string message, string title)
        {
            return ShowMessage(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning, Color.Red, Color.MistyRose);

        }

        public static DialogResult ShowInformation(string message, string title)
        {
            return ShowMessage(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information, Color.Green, Color.Honeydew);
        }

        public static DialogResult ShowQuestion(string message, string title)
        {
            return ShowMessage(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, Color.Blue, Color.LightCyan);
        }
        public static DialogResult ShowOkNoMessage(string message, string title)
        {
            // استخدام MessageBoxButtons.OKCancel لعرض "OK" و "Cancel"
            DialogResult result = ShowMessage(message, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, Color.Blue, Color.LightCyan);

            // التحقق إذا كان المستخدم قد اختار Cancel واعتباره كاختيار "No"
            if (result == DialogResult.Cancel)
            {
                return DialogResult.No;
            }

            return result;
        }

        public static DialogResult ShowDeleteOrEditPrompt(string message, string title)
        {
            // إنشاء نموذج جديد لعرض الرسالة المخصصة
            Form messageBoxForm = new Form()
            {
                Text = title,
                StartPosition = FormStartPosition.CenterScreen,
                Width = 400,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false,
                BackColor = Color.LightYellow // لون الخلفية مناسب للرسالة
            };

            // حساب ارتفاع الرسالة بناءً على طول النص
            Label messageLabel = new Label()
            {
                Text = message,
                Font = messageFont,
                ForeColor = Color.Black,
                AutoSize = false,
                Width = 360,
                TextAlign = ContentAlignment.MiddleRight, // محاذاة النص من اليمين
                Location = new Point(20, 20)
            };
            messageLabel.Height = TextRenderer.MeasureText(message, messageFont, new Size(messageLabel.Width, 0), TextFormatFlags.WordBreak).Height;

            // تعيين ارتفاع النموذج بناءً على ارتفاع النص
            messageBoxForm.Height = messageLabel.Height + 120;

            // زر "حذف"
            Button deleteButton = new Button()
            {
                Text = "حذف",
                DialogResult = DialogResult.Yes,
                Location = new Point(80, messageBoxForm.Height - 70),
                Width = 100
            };

            // زر "تعديل"
            Button editButton = new Button()
            {
                Text = "تعديل",
                DialogResult = DialogResult.No,
                Location = new Point(220, messageBoxForm.Height - 70),
                Width = 100
            };

            messageBoxForm.Controls.Add(messageLabel);
            messageBoxForm.Controls.Add(deleteButton);
            messageBoxForm.Controls.Add(editButton);

            return messageBoxForm.ShowDialog();
        }
        public static bool VerifyAdminPassword()
        {
            using (Form passwordForm = new Form())
            {
                // إعدادات النموذج
                passwordForm.Text = "إدخال كلمة مرور المدير";
                passwordForm.Width = 300;
                passwordForm.Height = 150;
                passwordForm.StartPosition = FormStartPosition.CenterScreen;
                passwordForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                passwordForm.MinimizeBox = false;
                passwordForm.MaximizeBox = false;
                passwordForm.BackColor = Color.Red; // تعيين لون الخلفية
                passwordForm.ForeColor = Color.White; // تعيين لون الخط

                // تغيير لون شريط العنوان باستخدام P/Invoke
                passwordForm.HandleCreated += (s, e) =>
                {
                    NativeMethods.SetWindowTextColor(passwordForm.Handle, Color.Black);
                };

                // إعدادات Label كلمة المرور
                Label lblPassword = new Label()
                {
                    Left = 20,
                    Top = 20,
                    Text = "كلمة المرور:",
                    AutoSize = true,
                    Font = new Font("Times New Roman", 12, FontStyle.Bold),
                    ForeColor = Color.White // تعيين لون النص إلى الأبيض
                };

                // إعدادات TextBox لإدخال كلمة المرور
                TextBox txtPassword = new TextBox()
                {
                    Left = 20,
                    Top = 50,
                    Width = 240,
                    PasswordChar = '*',
                    Font = new Font("Times New Roman", 12, FontStyle.Bold),
                    ForeColor = Color.Black,
                    BackColor = Color.White // تعيين الخلفية إلى الأبيض
                };

                // إعدادات زر "موافق"
                Button btnOk = new Button()
                {
                    Text = "موافق",
                    Left = 170,
                    Width = 80,
                    Top = 80,
                    DialogResult = DialogResult.OK,
                    BackColor = Color.Black, // تعيين لون الخلفية إلى الأسود
                    ForeColor = Color.White // تعيين لون النص إلى الأبيض
                };

                // إضافة العناصر إلى النموذج
                passwordForm.Controls.Add(lblPassword);
                passwordForm.Controls.Add(txtPassword);
                passwordForm.Controls.Add(btnOk);

                passwordForm.AcceptButton = btnOk;

                // عرض النموذج والتحقق من كلمة المرور
                if (passwordForm.ShowDialog() == DialogResult.OK)
                {
                    string enteredPassword = txtPassword.Text;
                    string adminPassword = "admin"; // كلمة مرور المدير الحقيقية

                    if (enteredPassword == adminPassword)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        // دالة P/Invoke لتغيير لون شريط العنوان
        internal static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern bool SetWindowText(IntPtr hWnd, string lpString);

            public const int GWL_STYLE = -16;
            public const int WS_SYSMENU = 0x80000;

            public static void SetWindowTextColor(IntPtr hwnd, Color color)
            {
                // تغيير لون شريط العنوان (هنا يمكن إضافة أي تخصيص آخر يتعلق بشريط العنوان)
            }
        }
        private static DialogResult ShowMessage(string message, string title, MessageBoxButtons buttons, MessageBoxIcon icon, Color textColor, Color backgroundColor)
        {
            // إنشاء نموذج جديد لعرض الرسالة المخصصة
            Form messageBoxForm = new Form()
            {
                Text = title,
                StartPosition = FormStartPosition.CenterScreen,
                Width = 400,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false,
                BackColor = backgroundColor // تعيين لون الخلفية
            };

            // إضافة PictureBox لعرض الأيقونة
            PictureBox iconPictureBox = new PictureBox()
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point(20, 20),
                Size = new Size(50, 50)
            };

            // تحديد الأيقونة المناسبة بناءً على نوع الرسالة
            switch (icon)
            {
                case MessageBoxIcon.Warning:
                    iconPictureBox.Image = SystemIcons.Warning.ToBitmap();
                    break;
                case MessageBoxIcon.Information:
                    iconPictureBox.Image = SystemIcons.Information.ToBitmap();
                    break;
                case MessageBoxIcon.Question:
                    iconPictureBox.Image = SystemIcons.Question.ToBitmap();
                    break;
                case MessageBoxIcon.Error:
                    iconPictureBox.Image = SystemIcons.Error.ToBitmap();
                    break;
                default:
                    iconPictureBox.Image = SystemIcons.Information.ToBitmap(); // أيقونة افتراضية
                    break;
            }

            // حساب ارتفاع الرسالة بناءً على طول النص
            Label messageLabel = new Label()
            {
                Text = message,
                Font = messageFont,
                ForeColor = textColor,
                AutoSize = false,
                Width = 300, // تقليل العرض لاستيعاب الأيقونة
                TextAlign = ContentAlignment.MiddleRight, // محاذاة النص من اليمين
                Location = new Point(80, 20) // تم تغيير الموقع لاستيعاب الأيقونة
            };
            messageLabel.Height = TextRenderer.MeasureText(message, messageFont, new Size(messageLabel.Width, 0), TextFormatFlags.WordBreak).Height;

            // تعيين ارتفاع النموذج بناءً على ارتفاع النص
            messageBoxForm.Height = messageLabel.Height + 120;

            Button okButton = new Button()
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(150, messageBoxForm.Height - 70),
                Width = 100
            };

            if (buttons == MessageBoxButtons.YesNo)
            {
                okButton.Text = "Yes";
                Button noButton = new Button()
                {
                    Text = "No",
                    DialogResult = DialogResult.No,
                    Location = new Point(270, messageBoxForm.Height - 70),
                    Width = 100
                };
                messageBoxForm.Controls.Add(noButton);
            }

            // إضافة الأيقونة والرسالة إلى النموذج
            messageBoxForm.Controls.Add(iconPictureBox);
            messageBoxForm.Controls.Add(messageLabel);
            messageBoxForm.Controls.Add(okButton);

            return messageBoxForm.ShowDialog();
        }
        /*لدى رسائل مخصصة فى كلاس CustomMessageBox
         يمكن استخدامها بدل الانبوت بوكس
         */
        public static DialogResult ShowNumericInputBox(out int enteredValue, string message, string title)
        {
            enteredValue = 0;

            // إنشاء النموذج المخصص
            using (Form inputForm = new Form())
            {
                inputForm.Text = title;
                inputForm.Width = 300;
                inputForm.Height = 150;
                inputForm.StartPosition = FormStartPosition.CenterScreen;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MinimizeBox = false;
                inputForm.MaximizeBox = false;
                inputForm.BackColor = Color.LightYellow;

                // Label لعرض الرسالة
                Label lblMessage = new Label()
                {
                    Left = 20,
                    Top = 20,
                    Text = message,
                    AutoSize = true,
                    Font = messageFont
                };

                // TextBox لإدخال القيمة الرقمية
                TextBox txtInput = new TextBox()
                {
                    Left = 20,
                    Top = 50,
                    Width = 240,
                    Font = messageFont,
                    ForeColor = Color.Black,
                    BackColor = Color.White
                };

                // التحكم في الإدخال ليكون أرقامًا صحيحة فقط
                txtInput.KeyPress += (sender, e) =>
                {
                    // السماح فقط بالأرقام
                    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    {
                        e.Handled = true; // منع أي إدخال غير رقمي
                    }
                };

                // زر "موافق"
                Button btnOk = new Button()
                {
                    Text = "موافق",
                    Left = 170,
                    Width = 80,
                    Top = 80,
                    DialogResult = DialogResult.OK
                };

                inputForm.Controls.Add(lblMessage);
                inputForm.Controls.Add(txtInput);
                inputForm.Controls.Add(btnOk);

                // تحديد زر "موافق" كزر التأكيد
                inputForm.AcceptButton = btnOk;

                // عرض النموذج والحصول على القيمة المدخلة
                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    if (int.TryParse(txtInput.Text, out int result))
                    {
                        enteredValue = result;
                        return DialogResult.OK;
                    }
                }

                return DialogResult.Cancel;
            }
        }
        public static DialogResult ShowDecimalInputBox(out decimal enteredValue, string message, string title, string defaultValue = "0")
        {
            enteredValue = 0;

            // إنشاء النموذج المخصص
            using (Form inputForm = new Form())
            {
                inputForm.Text = title;
                inputForm.Width = 300;
                inputForm.Height = 150;
                inputForm.StartPosition = FormStartPosition.CenterScreen;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MinimizeBox = false;
                inputForm.MaximizeBox = false;
                inputForm.BackColor = Color.LightYellow;

                // Label لعرض الرسالة
                Label lblMessage = new Label()
                {
                    Left = 20,
                    Top = 20,
                    Text = message,
                    AutoSize = true,
                    Font = new Font("Arial", 10, FontStyle.Bold)
                };

                // TextBox لإدخال القيمة الرقمية
                TextBox txtInput = new TextBox()
                {
                    Left = 20,
                    Top = 50,
                    Width = 240,
                    Font = new Font("Arial", 10),
                    ForeColor = Color.Black,
                    BackColor = Color.White,
                    Text = defaultValue // قيمة افتراضية
                };

                // التحكم في الإدخال ليكون أرقامًا عشرية فقط
                txtInput.KeyPress += (sender, e) =>
                {
                    // السماح بالأرقام وعلامة النقطة العشرية
                    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                    {
                        e.Handled = true; // منع أي إدخال غير رقمي أو النقطة العشرية
                    }

                    // منع أكثر من نقطة عشرية
                    if (e.KeyChar == '.' && txtInput.Text.Contains("."))
                    {
                        e.Handled = true;
                    }
                };

                // زر "موافق"
                Button btnOk = new Button()
                {
                    Text = "موافق",
                    Left = 170,
                    Width = 80,
                    Top = 80,
                    DialogResult = DialogResult.OK
                };

                inputForm.Controls.Add(lblMessage);
                inputForm.Controls.Add(txtInput);
                inputForm.Controls.Add(btnOk);

                // تحديد زر "موافق" كزر التأكيد
                inputForm.AcceptButton = btnOk;

                // عرض النموذج والحصول على القيمة المدخلة
                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    if (decimal.TryParse(txtInput.Text, out decimal result))
                    {
                        enteredValue = result;
                        return DialogResult.OK;
                    }
                }

                return DialogResult.Cancel;
            }
        }

        public static DialogResult ShowFloatInputBox(out float enteredValue, string message, string title, string defaultValue = "0")
        {
            enteredValue = 0;

            // إنشاء النموذج المخصص
            using (Form inputForm = new Form())
            {
                inputForm.Text = title;
                inputForm.StartPosition = FormStartPosition.CenterScreen;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MinimizeBox = false;
                inputForm.MaximizeBox = false;
                inputForm.BackColor = Color.LightYellow;

                // Label لعرض الرسالة
                Label lblMessage = new Label()
                {
                    Text = message,
                    AutoSize = true,
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    MaximumSize = new Size(550, 0), // يحدد عرض النص ويجعل الالتفاف ممكنًا
                    Left = 20,
                    Top = 20
                };

                // تحديد الحجم المفضل للتسمية
                lblMessage.Size = lblMessage.PreferredSize;

                // TextBox لإدخال القيمة الرقمية
                TextBox txtInput = new TextBox()
                {
                    Left = 20,
                    Top = lblMessage.Bottom + 10, // وضع مربع النص أسفل النص مع مسافة
                    Width = 240,
                    Font = new Font("Arial", 10),
                    ForeColor = Color.Black,
                    BackColor = Color.White,
                    Text = defaultValue // قيمة افتراضية
                };

                // التحكم في الإدخال ليكون أرقامًا عشرية فقط
                txtInput.KeyPress += (sender, e) =>
                {
                    // السماح بالأرقام وعلامة النقطة العشرية
                    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                    {
                        e.Handled = true; // منع أي إدخال غير رقمي أو النقطة العشرية
                    }

                    // منع أكثر من نقطة عشرية
                    if (e.KeyChar == '.' && txtInput.Text.Contains("."))
                    {
                        e.Handled = true;
                    }
                };

                // زر "موافق"
                Button btnOk = new Button()
                {
                    Text = "موافق",
                    Left = 20,
                    Width = 80,
                    Top = txtInput.Bottom + 10, // وضع الزر أسفل مربع النص مع مسافة
                    DialogResult = DialogResult.OK
                };

                // إضافة العناصر إلى النموذج
                inputForm.Controls.Add(lblMessage);
                inputForm.Controls.Add(txtInput);
                inputForm.Controls.Add(btnOk);

                // ضبط ارتفاع النموذج بناءً على المحتويات
                inputForm.ClientSize = new Size(600, btnOk.Bottom + 20);

                // تحديد زر "موافق" كزر التأكيد
                inputForm.AcceptButton = btnOk;

                // عرض النموذج والحصول على القيمة المدخلة
                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    if (float.TryParse(txtInput.Text, out float result))
                    {
                        enteredValue = result;
                        return DialogResult.OK;
                    }
                }

                return DialogResult.Cancel;
            }
        }

        public static DialogResult ShowMonthInputBox(out int enteredMonth, string message, string title)
        {
            enteredMonth = 0;

            // إنشاء النموذج المخصص
            using (Form inputForm = new Form())
            {
                inputForm.Text = title;
                inputForm.Width = 300;
                inputForm.Height = 150;
                inputForm.StartPosition = FormStartPosition.CenterScreen;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MinimizeBox = false;
                inputForm.MaximizeBox = false;
                inputForm.BackColor = Color.LightYellow;

                // Label لعرض الرسالة
                Label lblMessage = new Label()
                {
                    Left = 20,
                    Top = 20,
                    Text = message,
                    AutoSize = true,
                    Font = new Font("Arial", 10, FontStyle.Regular)
                };

                // TextBox لإدخال قيمة الشهر
                TextBox txtInput = new TextBox()
                {
                    Left = 20,
                    Top = 50,
                    Width = 240,
                    Font = new Font("Arial", 10, FontStyle.Regular),
                    ForeColor = Color.Black,
                    BackColor = Color.White
                };

                // التحكم في الإدخال ليكون أرقامًا صحيحة فقط (من 1 إلى 12)
                txtInput.KeyPress += (sender, e) =>
                {
                    // السماح فقط بالأرقام
                    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    {
                        e.Handled = true; // منع أي إدخال غير رقمي
                    }
                };

                // زر "موافق"
                Button btnOk = new Button()
                {
                    Text = "موافق",
                    Left = 170,
                    Width = 80,
                    Top = 80,
                    DialogResult = DialogResult.OK
                };

                inputForm.Controls.Add(lblMessage);
                inputForm.Controls.Add(txtInput);
                inputForm.Controls.Add(btnOk);

                // تحديد زر "موافق" كزر التأكيد
                inputForm.AcceptButton = btnOk;

                // عرض النموذج والحصول على القيمة المدخلة
                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    if (int.TryParse(txtInput.Text, out int result) && result >= 1 && result <= 12)
                    {
                        enteredMonth = result;
                        return DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("يرجى إدخال رقم بين 1 و 12 يمثل الشهر.", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                return DialogResult.Cancel;
            }
        }

        public static DialogResult ShowStringInputBox(out string enteredString, string message, string title)
        {
            enteredString = string.Empty;

            // خط افتراضي للرسالة
            Font messageFont = new Font("Segoe UI", 10);

            using (Form inputForm = new Form())
            {
                inputForm.Text = title;
                inputForm.Width = 300;
                inputForm.Height = 150;
                inputForm.StartPosition = FormStartPosition.CenterScreen;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MinimizeBox = false;
                inputForm.MaximizeBox = false;
                inputForm.BackColor = Color.LightYellow;

                Label lblMessage = new Label()
                {
                    Left = 20,
                    Top = 20,
                    Text = message,
                    AutoSize = true,
                    Font = messageFont
                };

                TextBox txtInput = new TextBox()
                {
                    Left = 20,
                    Top = 50,
                    Width = 240,
                    Font = messageFont,
                    ForeColor = Color.Black,
                    BackColor = Color.White
                };

                Button btnOk = new Button()
                {
                    Text = "موافق",
                    Left = 170,
                    Width = 80,
                    Top = 80,
                    DialogResult = DialogResult.OK
                };

                inputForm.Controls.Add(lblMessage);
                inputForm.Controls.Add(txtInput);
                inputForm.Controls.Add(btnOk);
                inputForm.AcceptButton = btnOk;

                if (inputForm.ShowDialog() == DialogResult.OK)
                {
                    enteredString = txtInput.Text;
                    return DialogResult.OK;
                }

                return DialogResult.Cancel;
            }
        }

        public static DialogResult ShowPasswordInputBox(out string enteredPassword, string message, string title)
        {
            enteredPassword = string.Empty;

            // حفظ اللغة الحالية لاستعادتها لاحقًا
            InputLanguage previousInputLanguage = InputLanguage.CurrentInputLanguage;

            try
            {
                // تغيير اللغة إلى الإنجليزية
                InputLanguage.CurrentInputLanguage = InputLanguage.FromCulture(new System.Globalization.CultureInfo("en-US"));

                // إنشاء النموذج المخصص
                using (Form inputForm = new Form())
                {
                    inputForm.Text = title;
                    inputForm.Width = 300;
                    inputForm.Height = 150;
                    inputForm.StartPosition = FormStartPosition.CenterScreen;
                    inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                    inputForm.MinimizeBox = false;
                    inputForm.MaximizeBox = false;
                    inputForm.BackColor = Color.LightYellow;

                    // Label لعرض الرسالة
                    Label lblMessage = new Label()
                    {
                        Left = 20,
                        Top = 20,
                        Text = message,
                        AutoSize = true,
                        Font = new Font("Arial", 10, FontStyle.Regular)
                    };

                    // TextBox لإدخال كلمة المرور
                    TextBox txtPassword = new TextBox()
                    {
                        Left = 20,
                        Top = 50,
                        Width = 240,
                        Font = new Font("Arial", 10, FontStyle.Regular),
                        ForeColor = Color.Black,
                        BackColor = Color.White,
                        UseSystemPasswordChar = true // إظهار النص المدخل كـ *
                    };

                    // زر "موافق"
                    Button btnOk = new Button()
                    {
                        Text = "موافق",
                        Left = 170,
                        Width = 80,
                        Top = 80,
                        DialogResult = DialogResult.OK
                    };

                    // زر "إلغاء"
                    Button btnCancel = new Button()
                    {
                        Text = "إلغاء",
                        Left = 80,
                        Width = 80,
                        Top = 80,
                        DialogResult = DialogResult.Cancel
                    };

                    // إضافة العناصر إلى النموذج
                    inputForm.Controls.Add(lblMessage);
                    inputForm.Controls.Add(txtPassword);
                    inputForm.Controls.Add(btnOk);
                    inputForm.Controls.Add(btnCancel);

                    // تحديد زر "موافق" كزر التأكيد
                    inputForm.AcceptButton = btnOk;

                    // تحديد زر "إلغاء" كزر الإلغاء
                    inputForm.CancelButton = btnCancel;

                    // عرض النموذج والحصول على القيمة المدخلة
                    if (inputForm.ShowDialog() == DialogResult.OK)
                    {
                        enteredPassword = txtPassword.Text;
                        return DialogResult.OK;
                    }

                    return DialogResult.Cancel;
                }
            }
            finally
            {
                // استعادة اللغة الأصلية
                InputLanguage.CurrentInputLanguage = previousInputLanguage;
            }
        }
   
        // الخط المستخدم للرسائل

        /// <summary>
        /// نافذة إدخال كلمة مرور ومقارنتها بالقيمة الصحيحة
        /// </summary>
        /// <param name="correctPassword">كلمة المرور الصحيحة</param>
        /// <returns>True إذا كانت كلمة المرور مطابقة، False غير ذلك</returns>
        public static bool ShowPasswordBox(string correctPassword)
        {
            using (Form passwordForm = new Form())
            {
                passwordForm.Text = "إدخال كلمة المرور";
                passwordForm.Width = 300;
                passwordForm.Height = 160;
                passwordForm.StartPosition = FormStartPosition.CenterScreen;
                passwordForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                passwordForm.MinimizeBox = false;
                passwordForm.MaximizeBox = false;
                passwordForm.BackColor = Color.LightBlue;

                Label lblPassword = new Label()
                {
                    Left = 20,
                    Top = 20,
                    Text = "كلمة المرور:",
                    AutoSize = true,
                    Font = messageFont
                };

                TextBox txtPassword = new TextBox()
                {
                    Left = 20,
                    Top = 50,
                    Width = 240,
                    PasswordChar = '*',
                    Font = messageFont,
                    ForeColor = Color.Black,
                    BackColor = Color.White
                };

                // حفظ وإعداد اللغة
                InputLanguage originalLanguage = InputLanguage.CurrentInputLanguage;
                InputLanguage? englishLanguage = InputLanguage.InstalledInputLanguages
                    .OfType<InputLanguage>()
                    .FirstOrDefault(lang => lang.Culture.TwoLetterISOLanguageName == "en");

                if (englishLanguage != null)
                    InputLanguage.CurrentInputLanguage = englishLanguage;

                Button btnOk = new Button()
                {
                    Text = "موافق",
                    Left = 170,
                    Width = 80,
                    Top = 90,
                    DialogResult = DialogResult.OK
                };

                Button btnCancel = new Button()
                {
                    Text = "إلغاء",
                    Left = 80,
                    Width = 80,
                    Top = 90,
                    DialogResult = DialogResult.Cancel
                };

                passwordForm.Controls.Add(lblPassword);
                passwordForm.Controls.Add(txtPassword);
                passwordForm.Controls.Add(btnOk);
                passwordForm.Controls.Add(btnCancel);

                passwordForm.AcceptButton = btnOk;
                passwordForm.CancelButton = btnCancel;

                if (passwordForm.ShowDialog() == DialogResult.OK)
                {
                    InputLanguage.CurrentInputLanguage = originalLanguage;
                    return txtPassword.Text == correctPassword;
                }

                InputLanguage.CurrentInputLanguage = originalLanguage;
                return false;
            }
        }

        public static bool ShowNewPasswordBox(out string newPassword, string message)
        {
            newPassword = string.Empty;

            using (Form passwordForm = new Form())
            {
                passwordForm.Text = "إدخال كلمة المرور الجديدة";
                passwordForm.Width = 300;
                passwordForm.Height = 160;
                passwordForm.StartPosition = FormStartPosition.CenterScreen;
                passwordForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                passwordForm.MinimizeBox = false;
                passwordForm.MaximizeBox = false;
                passwordForm.BackColor = Color.LightBlue;

                Label lblMessage = new Label()
                {
                    Left = 20,
                    Top = 20,
                    Text = message,
                    AutoSize = true,
                    Font = messageFont
                };

                TextBox txtPassword = new TextBox()
                {
                    Left = 20,
                    Top = 50,
                    Width = 240,
                    PasswordChar = '*',
                    Font = messageFont,
                    ForeColor = Color.Black,
                    BackColor = Color.White
                };

                // تغيير اللغة إلى الإنجليزية
                InputLanguage originalLanguage = InputLanguage.CurrentInputLanguage;
                InputLanguage? englishLanguage = InputLanguage.InstalledInputLanguages
                    .OfType<InputLanguage>()
                    .FirstOrDefault(lang => lang.Culture.TwoLetterISOLanguageName == "en");

                if (englishLanguage != null)
                    InputLanguage.CurrentInputLanguage = englishLanguage;

                Button btnOk = new Button()
                {
                    Text = "موافق",
                    Left = 170,
                    Width = 80,
                    Top = 90,
                    DialogResult = DialogResult.OK
                };

                Button btnCancel = new Button()
                {
                    Text = "إلغاء",
                    Left = 80,
                    Width = 80,
                    Top = 90,
                    DialogResult = DialogResult.Cancel
                };

                passwordForm.Controls.Add(lblMessage);
                passwordForm.Controls.Add(txtPassword);
                passwordForm.Controls.Add(btnOk);
                passwordForm.Controls.Add(btnCancel);

                passwordForm.AcceptButton = btnOk;
                passwordForm.CancelButton = btnCancel;

                DialogResult result = passwordForm.ShowDialog();
                InputLanguage.CurrentInputLanguage = originalLanguage;

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    newPassword = txtPassword.Text;
                    return true;
                }

                return false;
            }
        }


    }
}
