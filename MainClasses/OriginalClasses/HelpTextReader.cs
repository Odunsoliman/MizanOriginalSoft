
using MizanOriginalSoft.Views.Forms.MainForms;
using System;
using System.IO;
using System.Text;

namespace MizanOriginalSoft.MainClasses.OriginalClasses
{
    public static class HelpTextReader
    {
        private static readonly string HelpFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HelpFiles");

        /// <summary>
        /// جلب نص المساعدة بناءً على اسم الشاشة أو اسم الأداة داخل الشاشة.
        /// </summary>
        public static string GetHelpText(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return "❌ المفتاح غير صالح.";

            string formName = key.Contains("&") ? key.Split('&')[0] : key;

            string helpFileName = $"Help_{formName}.txt";
            string fullPath = Path.Combine(HelpFolderPath, helpFileName);

            if (!File.Exists(fullPath))
                return $"⚠️ ملف المساعدة '{helpFileName}' غير موجود.";

            string[] lines = File.ReadAllLines(fullPath, Encoding.UTF8);
            StringBuilder result = new StringBuilder();

            // ✅ 1. إذا كانت المساعدة العامة (اسم الشاشة فقط)
            if (!key.Contains("&"))
            {
                StringBuilder section = new StringBuilder();
                bool isCapturing = false;

                foreach (string line in lines)
                {
                    // عند كل مفتاح جديد نبدأ قسم جديد
                    if (line.Trim().StartsWith(formName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (section.Length > 0)
                        {
                            result.Append(section);
                            result.AppendLine("\n-------------------------------\n");
                            section.Clear();
                        }

                        isCapturing = true;
                        continue;
                    }

                    if (isCapturing)
                    {
                        if (line.Trim() == "$")
                        {
                            isCapturing = false;
                            continue;
                        }

                        section.AppendLine(line);
                    }
                }

                if (section.Length > 0)
                    result.Append(section);

                return string.IsNullOrWhiteSpace(result.ToString())
                    ? "❌ لا يوجد شرح متاح لهذه الشاشة حتى الآن."
                    : result.ToString().Trim();
            }

            // ✅ 2. إذا كانت المساعدة تخص عنصر داخل الشاشة
            bool captureSingle = false;
            foreach (string line in lines)
            {
                if (line.Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    captureSingle = true;
                    continue;
                }

                if (captureSingle)
                {
                    if (line.Trim() == "$")
                        break;

                    result.AppendLine(line);
                }
            }

            return string.IsNullOrWhiteSpace(result.ToString())
                ? "❌ لا يوجد شرح متاح لهذا العنصر حتى الآن."
                : result.ToString().Trim();
        }

        public static string GetHelpVideoPath(string key)
        {
            string? baseDir = AppDomain.CurrentDomain.BaseDirectory;

            if (string.IsNullOrWhiteSpace(baseDir))
                return string.Empty;

            string? driveRoot = Path.GetPathRoot(baseDir);
            if (string.IsNullOrWhiteSpace(driveRoot))
                return string.Empty;

            string videoFolderPath = Path.Combine(driveRoot, "MizanHelpVideos");
            string videoPath = Path.Combine(videoFolderPath, key + ".mp4");

            return File.Exists(videoPath) ? videoPath : string.Empty;
        }

        public static void ShowHelpForControl(Form currentForm, object sender)
        {
            if (sender is Control control)
            {
                string controlKey = $"{currentForm.Name}&{control.Name}";
                frmHelp helpForm = new frmHelp(controlKey);
                helpForm.ShowDialog();
            }
        }
    }
}
