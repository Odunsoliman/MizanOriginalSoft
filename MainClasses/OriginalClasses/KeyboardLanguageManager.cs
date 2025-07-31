using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Signee.DB.MainClasses
{
    public class KeyboardLanguageManager
    {
        #region ======== تغيير لغة الكتابة  =================
        // استيراد دوال API لتغيير لغة الإدخال
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);

        private const uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
        private const uint KLF_ACTIVATE = 1;

        public readonly IntPtr ArabicLayout = LoadKeyboardLayout("00000401", KLF_ACTIVATE); // اللغة العربية
        public readonly IntPtr EnglishLayout = LoadKeyboardLayout("00000409", KLF_ACTIVATE); // اللغة الإنجليزية

        private Form _form;

        public KeyboardLanguageManager(Form form)
        {
            _form = form;
        }

        private void ChangeKeyboardLayout(IntPtr layout)
        {
            PostMessage(_form.Handle, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, layout);
        }

        public void SetArabicLanguage()
        {
            ChangeKeyboardLayout(ArabicLayout);
        }

        public void SetEnglishLanguage()
        {
            ChangeKeyboardLayout(EnglishLayout);
        }
        #endregion
    }
}
