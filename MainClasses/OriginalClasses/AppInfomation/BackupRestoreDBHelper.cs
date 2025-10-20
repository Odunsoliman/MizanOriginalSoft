using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;

namespace MizanOriginalSoft.MainClasses.OriginalClasses.AppInfomation
{
    internal class BackupRestoreDBHelper
    {
        private readonly string _configPath;

        public BackupRestoreDBHelper(string configPath)
        {
            _configPath = configPath;
        }

        // 🔹 تحميل الإعدادات من ملف التكست
        private Dictionary<string, string> LoadSettings()
        {
            if (!File.Exists(_configPath))
                throw new FileNotFoundException("❌ ملف الإعدادات غير موجود: " + _configPath);

            var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var line in File.ReadAllLines(_configPath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#") || !line.Contains("=")) continue;
                var parts = line.Split(new[] { '=' }, 2);
                settings[parts[0].Trim()] = parts[1].Trim();
            }
            return settings;
        }

        // 🔹 إنشاء ConnectionString
        private string BuildConnectionString(string serverName, string dbName)
        {
            return $"Server={serverName};Database={dbName};Integrated Security=True;TrustServerCertificate=True;";
        }

        // 🔹 تنظيف النسخ القديمة (الإبقاء على آخر 10 فقط)
        private void CleanOldBackups(string backupFolder)
        {
            var files = new DirectoryInfo(backupFolder)
                .GetFiles("*.bak")
                .OrderByDescending(f => f.CreationTime)
                .ToList();

            if (files.Count > 10)
            {
                foreach (var file in files.Skip(10))
                {
                    try { file.Delete(); } catch { }
                }
            }
        }

        // 🔹 إنشاء النسخة الاحتياطية بدون صلاحيات SQL إضافية
        public void BackupDatabase()
        {
            try
            {
                var settings = LoadSettings();

                string server = settings["ServerName"];
                string dbName = settings["DBName"];
                string backupFolder = settings["BackupFolder"];

                // تأكد من وجود المجلد
                Directory.CreateDirectory(backupFolder);

                // تنظيف النسخ القديمة
                CleanOldBackups(backupFolder);

                // اسم النسخة
                string fileName = $"{dbName}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.bak";
                string fullPath = Path.Combine(backupFolder, fileName);

                // ⚠️ SQL Server Express لا يدعم COMPRESSION
                string sql = $@"
                    BACKUP DATABASE [{dbName}]
                    TO DISK = N'{fullPath}'
                    WITH INIT, STATS = 5;
                ";

                using (var connection = new SqlConnection(BuildConnectionString(server, "master")))
                {
                    connection.Open();
                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                Console.WriteLine($"✅ تم إنشاء النسخة الاحتياطية بنجاح: {fullPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطأ أثناء النسخ الاحتياطي: {ex.Message}");
            }
        }

        // 🔹 استعادة قاعدة البيانات
        public void RestoreDatabase(string backupFilePath)
        {
            try
            {
                var settings = LoadSettings();
                string server = settings["ServerName"];
                string dbName = settings["DBName"];

                string sql = $@"
                    ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    RESTORE DATABASE [{dbName}] FROM DISK = N'{backupFilePath}' WITH REPLACE;
                    ALTER DATABASE [{dbName}] SET MULTI_USER;
                ";

                using (var connection = new SqlConnection(BuildConnectionString(server, "master")))
                {
                    connection.Open();
                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                Console.WriteLine($"✅ تم استعادة قاعدة البيانات بنجاح من: {backupFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطأ أثناء استعادة قاعدة البيانات: {ex.Message}");
            }
        }

        // 🔹 نسخ آخر نسخة احتياطية إلى مجلد Google Drive
        public void CopyBackupToGoogleDrive(string sourceFolder, string googleDriveFolder, string dbName)
        {
            try
            {
                // تأكد أن المسارين موجودان
                if (!Directory.Exists(sourceFolder))
                    throw new DirectoryNotFoundException($"❌ مجلد النسخ الاحتياطية غير موجود: {sourceFolder}");

                if (!Directory.Exists(googleDriveFolder))
                    throw new DirectoryNotFoundException($"❌ مجلد Google Drive غير موجود: {googleDriveFolder}");

                // الحصول على أحدث ملف .bak
                var latestBackup = new DirectoryInfo(sourceFolder)
                    .GetFiles($"{dbName}_*.bak")
                    .OrderByDescending(f => f.CreationTime)
                    .FirstOrDefault();

                if (latestBackup == null)
                    throw new FileNotFoundException("⚠️ لم يتم العثور على أي ملف نسخ احتياطي في المجلد المحدد.");

                // تحديد المسار الجديد داخل Google Drive
                string destPath = Path.Combine(googleDriveFolder, latestBackup.Name);

                // نسخ الملف (استبدال في حال وجوده)
                File.Copy(latestBackup.FullName, destPath, overwrite: true);

                Console.WriteLine($"✅ تم نسخ النسخة الاحتياطية إلى Google Drive: {destPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطأ أثناء النسخ إلى Google Drive: {ex.Message}");
            }
        }

    }
}
