using System;
using System.Data;
using System.IO;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace MizanOriginalSoft.MainClasses.OriginalClasses
{
    public class DatabaseBackupRestoreHelper
    {
        private readonly string masterConnectionString;
        private readonly string serverName;

        public DatabaseBackupRestoreHelper(string settingsFilePath)
        {
            var appSettings = new AppSettings(settingsFilePath);
            serverName = appSettings.GetString("serverName", ".") ?? ".";
            masterConnectionString = $"Data Source={serverName};Initial Catalog=master;Integrated Security=True;TrustServerCertificate=True;";
        }

        public void BackupDatabase(string settingsFilePath)
        {
            var appSettings = new AppSettings(settingsFilePath);

            string? backupFolder = appSettings.GetString("BackupsPath", null);
            string? backupProc = appSettings.GetString("BackupDB", null);
            string? dbName = appSettings.GetString("DBName", null);

            if (string.IsNullOrWhiteSpace(backupFolder) || string.IsNullOrWhiteSpace(backupProc) || string.IsNullOrWhiteSpace(dbName))
            {
                Console.WriteLine("❌ بعض القيم المطلوبة غير محددة في ملف الإعداد.");
                return;
            }

            if (!Directory.Exists(backupFolder))
                Directory.CreateDirectory(backupFolder);

            using SqlConnection conn = new(masterConnectionString);
            using SqlCommand cmd = new(backupProc, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            string path = backupFolder.EndsWith(@"\") ? backupFolder : backupFolder + @"\";
            cmd.Parameters.AddWithValue("@FolderPath", path);

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                Console.WriteLine("✅ تم إنشاء النسخة الاحتياطية بنجاح.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ خطأ أثناء النسخ الاحتياطي: " + ex.Message);
            }
        }

        public void RestoreDatabase(string settingsFilePath, string backupFilePath)
        {
            var appSettings = new AppSettings(settingsFilePath);

            string? restoreProc = appSettings.GetString("RestoreDB", null);
            string? dbName = appSettings.GetString("DBName", null);

            if (string.IsNullOrWhiteSpace(restoreProc) || string.IsNullOrWhiteSpace(dbName))
            {
                Console.WriteLine("❌ اسم الإجراء أو اسم القاعدة غير محدد.");
                return;
            }

            using SqlConnection conn = new(masterConnectionString);
            using SqlCommand cmd = new(restoreProc, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@DBName", dbName);
            cmd.Parameters.AddWithValue("@BackupFilePath", backupFilePath);

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                Console.WriteLine("✅ تم تنفيذ الاسترجاع بنجاح.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ خطأ أثناء الاسترجاع: " + ex.Message);
            }
        }

        public void CleanOldBackups(string rootBackupFolder, string settingsFilePath)
        {
            var appSettings = new AppSettings(settingsFilePath);
            int maxFilesToKeep = appSettings.GetInt("maxBackups", 20);

            try
            {
                if (!Directory.Exists(rootBackupFolder))
                {
                    Console.WriteLine("📁 سيتم إنشاء مجلد النسخ الاحتياطية...");
                    Directory.CreateDirectory(rootBackupFolder);
                }

                var backupFiles = new DirectoryInfo(rootBackupFolder)
                    .GetFiles("*.bak")
                    .OrderByDescending(f => f.CreationTime)
                    .ToList();

                var filesToDelete = backupFiles.Skip(maxFilesToKeep);

                foreach (var file in filesToDelete)
                {
                    try
                    {
                        file.Delete();
                        Console.WriteLine("🗑️ حذف النسخة: " + file.Name);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("⚠️ لم يتم حذف النسخة: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ خطأ في تنظيف النسخ القديمة: " + ex.Message);
            }
        }


        public void CopyLatestBackupToSharedFolder(string sourceBackupFolder, string sharedFolderPath, string outputFileName)
        {
            try
            {
                if (!Directory.Exists(sourceBackupFolder))
                {
                    Console.WriteLine("❌ مجلد النسخ الاحتياطية غير موجود: " + sourceBackupFolder);
                    return;
                }

                if (!Directory.Exists(sharedFolderPath))
                {
                    Console.WriteLine("📁 إنشاء مجلد المشاركة: " + sharedFolderPath);
                    Directory.CreateDirectory(sharedFolderPath);
                }

                // استخراج أحدث ملف .bak
                var latestFile = new DirectoryInfo(sourceBackupFolder)
                                    .GetFiles("*.bak")
                                    .OrderByDescending(f => f.CreationTime)
                                    .FirstOrDefault();

                if (latestFile == null)
                {
                    Console.WriteLine("⚠️ لا توجد ملفات نسخ احتياطية في المجلد.");
                    return;
                }

                string destFilePath = Path.Combine(sharedFolderPath, outputFileName);

                // نسخ مع الاستبدال
                File.Copy(latestFile.FullName, destFilePath, true);

                Console.WriteLine($"✅ تم نسخ {latestFile.Name} إلى {destFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ خطأ أثناء نسخ النسخة إلى مجلد المشاركة: " + ex.Message);
            }
        }

    }
}
