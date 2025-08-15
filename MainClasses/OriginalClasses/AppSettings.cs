using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MizanOriginalSoft.MainClasses.OriginalClasses
{
    public static class AppSettings
    {
        private static readonly Dictionary<string, string> settings = new(StringComparer.OrdinalIgnoreCase);
        private static bool isLoaded = false;
        private static string settingsFilePath = string.Empty;

        // 🔐 وضع التعديل
        private static bool isEditMode = false;

        public static event Action<string, string>? SettingChanged;

        public static void Load(string filePath)
        {
            settings.Clear();
            isLoaded = false;
            settingsFilePath = filePath;

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"❌ ملف الإعدادات غير موجود: {filePath}");

            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line) || !line.Contains("="))
                    continue;

                var parts = line.Split(new[] { '=' }, 2);
                if (parts.Length == 2)
                    settings[parts[0].Trim()] = parts[1].Trim();
            }

            isLoaded = true;
        }

        private static void EnsureLoaded()
        {
            if (!isLoaded)
                throw new InvalidOperationException("⚠️ لم يتم تحميل ملف الإعدادات. استخدم AppSettings.Load() أولاً.");
        }

        // 📌 تفعيل وضع التعديل (من شاشة الإعدادات فقط)
        public static void EnableEditMode(string callerFormName)
        {
            if (callerFormName == "frmGenralData")
                isEditMode = true;
            else
                throw new UnauthorizedAccessException("❌ لا يمكن تعديل الإعدادات إلا من شاشة الإعدادات.");
        }

        // 📌 تعطيل وضع التعديل (بعد الحفظ)
        public static void DisableEditMode()
        {
            isEditMode = false;
        }

        public static string? GetString(string key, string? defaultValue = null)
        {
            EnsureLoaded();
            return settings.TryGetValue(key, out string? value) ? value : defaultValue;
        }

        public static int GetInt(string key, int defaultValue = 0) =>
            int.TryParse(GetString(key), out var result) ? result : defaultValue;

        public static double GetDouble(string key, double defaultValue = 0) =>
            double.TryParse(GetString(key), out var result) ? result : defaultValue;

        public static bool GetBool(string key, bool defaultValue = false) =>
            bool.TryParse(GetString(key), out var result) ? result : defaultValue;

        public static DateTime GetDateTime(string key, DateTime defaultValue) =>
            DateTime.TryParse(GetString(key), out var result) ? result : defaultValue;

        public static decimal GetDecimal(string key, decimal defaultValue = 0) =>
            decimal.TryParse(GetString(key), out var result) ? result : defaultValue;

        public static void Set(string key, string value)
        {
            EnsureLoaded();
            if (!isEditMode)
                throw new UnauthorizedAccessException("❌ لا يمكن تعديل الإعدادات إلا من شاشة الإعدادات.");

            settings[key] = value;
            SettingChanged?.Invoke(key, value);
        }

        public static void Remove(string key)
        {
            EnsureLoaded();
            if (!isEditMode)
                throw new UnauthorizedAccessException("❌ لا يمكن حذف الإعدادات إلا من شاشة الإعدادات.");

            if (settings.Remove(key))
                SettingChanged?.Invoke(key, string.Empty);
        }

        public static void Save(string? filePath = null)
        {
            EnsureLoaded();
            if (!isEditMode)
                throw new UnauthorizedAccessException("❌ لا يمكن حفظ الإعدادات إلا من شاشة الإعدادات.");

            var targetPath = filePath ?? settingsFilePath;
            File.WriteAllLines(targetPath, settings.Select(kv => $"{kv.Key}={kv.Value}"));
        }

        public static Dictionary<string, string> GetAllSettings()
        {
            EnsureLoaded();
            return new(settings);
        }
    }

}
/*
 هل الكلاس يقرئ القيمة RestoreDB=Original_RestoreDatabase
الموجودة الان فى الملف
 */