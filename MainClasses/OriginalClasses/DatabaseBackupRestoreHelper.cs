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
            AppSettings.Load(settingsFilePath); // تحميل الإعدادات مرة واحدة فقط

            serverName = AppSettings.GetString("serverName", ".") ?? ".";
            masterConnectionString = $"Data Source={serverName};Initial Catalog=master;Integrated Security=True;TrustServerCertificate=True;";
        }

        public void BackupDatabase()
        {
            string? backupFolder = AppSettings.GetString("BackupsPath");
            string? backupProc = AppSettings.GetString("BackupDB");
            string? dbName = AppSettings.GetString("DBName");

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
            cmd.Parameters.AddWithValue("@DBName", dbName);

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                Console.WriteLine("✅ تم إنشاء النسخة الاحتياطية بنجاح.");
            }
            /*Microsoft.Data.SqlClient.SqlException: 'A severe error occurred on the current command.  The results, if any, should be discarded.
❌ خطأ أثناء النسخ الاحتياطي: BACKUP DATABASE is terminating abnormally.'*/
            catch (Exception ex)
            {
                Console.WriteLine("❌ خطأ أثناء النسخ الاحتياطي: " + ex.Message);
            }
        }

        public void RestoreDatabase(string backupFilePath)
        {
            string? restoreProc = AppSettings.GetString("RestoreDB");
            string? dbName = AppSettings.GetString("DBName");

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

        public void CleanOldBackups(string rootBackupFolder)
        {
            int maxFilesToKeep = AppSettings.GetInt("maxBackups", 20);

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
                File.Copy(latestFile.FullName, destFilePath, true);

                Console.WriteLine($"✅ تم نسخ {latestFile.Name} إلى {destFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ خطأ أثناء نسخ النسخة إلى مجلد المشاركة: " + ex.Message);
            }
        }

        public void CopyBackupToGoogleDrive(string sourceFolder, string googleDriveFolder, string dbName)
        {
            try
            {
                string? latestBackup = Directory.GetFiles(sourceFolder, "*.bak")
                    .OrderByDescending(File.GetLastWriteTime)
                    .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(latestBackup) || !File.Exists(latestBackup))
                {
                    Console.WriteLine("❌ لا توجد نسخ احتياطية صالحة في المجلد.");
                    return;
                }

                if (!Directory.Exists(googleDriveFolder))
                    Directory.CreateDirectory(googleDriveFolder);

                string destPath = Path.Combine(googleDriveFolder, dbName + ".bak");

                File.Copy(latestBackup, destPath, overwrite: true);
                Console.WriteLine("✅ تم نسخ النسخة إلى مجلد Google Drive.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ خطأ أثناء النسخ إلى Google Drive: " + ex.Message);
            }
        }
    }
}
