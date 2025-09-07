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
            // 📌 تحميل ملف الإعدادات مرة واحدة عند بدء البرنامج
            string settingsPath = Path.Combine(Application.StartupPath, "serverConnectionSettings.txt");
            AppSettings.Load(settingsPath);

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Views.Forms.MainForms.frmMainLogIn());
        }
    }
}
