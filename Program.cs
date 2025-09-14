using MizanOriginalSoft.MainClasses.OriginalClasses;

namespace MizanOriginalSoft
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string settingsPath = Path.Combine(Application.StartupPath, "serverConnectionSettings.txt");

            if (!File.Exists(settingsPath))
            {
                MessageBox.Show($"⚠️ الملف غير موجود:\n{settingsPath}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AppSettings.Load(settingsPath);

            ApplicationConfiguration.Initialize();
            Application.Run(new Views.Forms.MainForms.frmMainLogIn());
        }

    }
}
