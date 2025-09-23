using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MizanOriginalSoft.MainClasses.OriginalClasses
{
    public static class AppSettings
    {
        private static readonly Dictionary<string, string> settings = new(StringComparer.OrdinalIgnoreCase);
        private static bool isLoaded = false;
        private static string settingsFilePath = string.Empty;

        // 📌 تحميل الملف
        public static void Load(string filePath)
        {
            settings.Clear();
            isLoaded = false;
            settingsFilePath = filePath;

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"❌ ملف الإعدادات غير موجود: {filePath}");

            foreach (var rawLine in File.ReadAllLines(filePath))
            {
                string line = rawLine.Trim();

                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#") || line.StartsWith("//") || line.StartsWith(";"))
                    continue;

                int equalIndex = line.IndexOf('=');
                if (equalIndex <= 0)
                    continue;

                string key = line.Substring(0, equalIndex).Trim();
                string value = line.Substring(equalIndex + 1).Trim();

                if (!string.IsNullOrEmpty(key))
                    settings[key] = value;
            }

            isLoaded = true;
        }

        private static void EnsureLoaded()
        {
            if (!isLoaded)
                throw new InvalidOperationException("⚠️ لم يتم تحميل ملف الإعدادات. استخدم AppSettings.Load() أولاً.");
        }

        // 📌 إعادة تحميل (Refresh) الإعدادات بعد الحفظ
        public static void ReloadSettings()
        {
            if (File.Exists(settingsFilePath))
                Load(settingsFilePath);
        }

        // 📌 دوال القراءة
        public static string? GetString(string key, string? defaultValue = null)
        {
            EnsureLoaded();
            return settings.TryGetValue(key, out string? value) ? value : defaultValue;
        }

        public static int GetInt(string key, int defaultValue = 0) =>
            int.TryParse(GetString(key), out var result) ? result : defaultValue;

        public static double GetDouble(string key, double defaultValue = 0) =>
            double.TryParse(GetString(key), out var result) ? result : defaultValue;

        public static bool GetBool(string key, bool defaultValue = false)
        {
            string? value = GetString(key);
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            value = value.Trim().ToLower();
            if (bool.TryParse(value, out bool boolResult))
                return boolResult;

            if (value == "1") return true;
            if (value == "0") return false;

            return defaultValue;
        }

        public static DateTime GetDateTime(string key, DateTime defaultValue) =>
            DateTime.TryParse(GetString(key), out var result) ? result : defaultValue;

        public static decimal GetDecimal(string key, decimal defaultValue = 0) =>
            decimal.TryParse(GetString(key), out var result) ? result : defaultValue;

        public static Dictionary<string, string> GetAllSettings()
        {
            EnsureLoaded();
            return new(settings);
        }

        // 📌 الحفظ مع التحديث التلقائي
        public static void SaveOrUpdate(string key, string value)
        {
            EnsureLoaded();

            var lines = File.ReadAllLines(settingsFilePath).ToList();
            bool updated = false;

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i].Trim();

                if (string.IsNullOrWhiteSpace(line) ||
                    line.StartsWith("#") || line.StartsWith("//") || line.StartsWith(";"))
                    continue;

                int equalIndex = line.IndexOf('=');
                if (equalIndex <= 0) continue;

                string currentKey = line.Substring(0, equalIndex).Trim();

                if (string.Equals(currentKey, key, StringComparison.OrdinalIgnoreCase))
                {
                    string prefix = lines[i].Substring(0, equalIndex + 1);
                    string oldValue = line.Substring(equalIndex + 1).Trim();

                    if (oldValue != value)
                    {
                        lines[i] = prefix + " " + value;
                        File.WriteAllLines(settingsFilePath, lines);
                    }

                    updated = true;
                    break;
                }
            }

            if (!updated)
            {
                lines.Add($"{key}={value}");
                File.WriteAllLines(settingsFilePath, lines);
            }

            // 📌 تحديث الكاش وإعادة التحميل
            ReloadSettings();
        }
    }
}
