using System;
using System.Collections.Generic;
using System.IO;

namespace MizanOriginalSoft.MainClasses.OriginalClasses
{
    public class AppSettings
    {
        private readonly Dictionary<string, string> settings = new(StringComparer.OrdinalIgnoreCase);

        public AppSettings(string settingsFilePath)
        {
            if (!File.Exists(settingsFilePath))
                throw new FileNotFoundException("ملف الإعدادات غير موجود: " + settingsFilePath);

            foreach (var line in File.ReadAllLines(settingsFilePath))
            {
                if (string.IsNullOrWhiteSpace(line) || !line.Contains("=")) continue;

                var parts = line.Split(new[] { '=' }, 2);
                if (parts.Length == 2)
                    settings[parts[0].Trim()] = parts[1].Trim();
            }
        }

        public string? GetString(string key, string? defaultValue = null)
        {
            return settings.ContainsKey(key) ? settings[key] : defaultValue;
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            return settings.ContainsKey(key) && int.TryParse(settings[key], out int value) ? value : defaultValue;
        }
    }
}
