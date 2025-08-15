using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MizanOriginalSoft.MainClasses.OriginalClasses
{
    public static class CurrentSession
    {
        // بيانات الجلسة للمستخدم
        public static int UserID { get; set; }
        public static string UserName { get; set; } = string.Empty;

        // رقم الفرع الحالي
        public static int WarehouseId { get; set; }

        // صلاحيات
        public static bool IsAdmin => UserName.ToLower() == "admin";

        // ✅ الإعدادات العامة من ملف السيرفر
        public static string? CompanyName { get; set; }
        public static DateTime? ExpiryDate { get; set; }
        public static DateTime? EndDate { get; set; }
        public static string? BackupsPath { get; set; }
        public static string? BackupDB { get; set; }
        public static string? GoogleDrivePath { get; set; }
        public static string? BackupGitPath { get; set; }

        // ⬅️ تحميل الإعدادات من الملف
        public static void LoadServerSettings(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("ملف الإعدادات غير موجود", filePath);

            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line) || !line.Contains("=")) continue;

                var parts = line.Split('=', 2);
                var key = parts[0].Trim();
                var value = parts[1].Trim();

                switch (key.ToLower())
                {
                    case "companyname":
                        CompanyName = value;
                        break;
                    case "expirydate":
                        if (DateTime.TryParseExact(value, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime exp))
                            ExpiryDate = exp;
                        break;
                    case "enddate":
                        if (DateTime.TryParseExact(value, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime end))
                            EndDate = end;
                        break;
                    case "defaultwarehouseid":
                        if (int.TryParse(value, out int warehouseId))
                            WarehouseId = warehouseId;
                        break;
                    case "backupspath":
                        BackupsPath = value;
                        break;
                    case "backupdb":
                        BackupDB = value;
                        break;
                    case "googledrivepath":
                        GoogleDrivePath = value;
                        break;
                    case "backupgitpath":
                        BackupGitPath = value;
                        break;
                }
            }
        }

    }
}
