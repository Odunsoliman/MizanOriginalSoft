using System;
using System.IO;

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

                    // ✅ دعم المفتاح القديم
                    case "defaultwarehouseid":
                        if (int.TryParse(value, out int oldWhId))
                            WarehouseId = oldWhId;
                        break;

                    // ✅ دعم المفتاح الجديد
                    case "thisversionisforwarehouseid":
                        if (int.TryParse(value, out int newWhId))
                            WarehouseId = newWhId;
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

/*
اريد معرفة مدى تطابق قرائة الملف مع البيانات المسجلة فيه

وهى كما يلى 


# ==============================
# إعدادات الاتصال بقاعدة البيانات
# ==============================
serverName=DESKTOP-EE70K28\SQLEXPRESS
DBName=MizanOriginalDB
BackupDB=Original_BackupDatabase
RestoreDB=Original_RestoreDatabase

# ==============================
# إعدادات النسخ الاحتياطي
# ==============================
maxBackups=10
BackupsPath=D:\MizanOriginalSoft\DataBaseApp\BakUpDB

# ==============================
# إعدادات رفع السحابى
# ==============================
# 📌 المسار المحلي لمجلد Google Drive على جهازك لرفع النسخ الاحتياطية تلقائيًا إلى السحابة
GoogleDrivePath=G:\
# 📌 مسار مشروع البرنامج الذي سيتم رفعه على Git عند الإغلاق للمزامنة مع المستودع
ProjectPath=D:\MizanOriginalSoft
# 📌 مسار مجلد مخصص لنسخ القواعد التي سيتم رفعها على Git (يمكن تركه فارغ إذا لم تستخدم هذه الميزة)
BackupGitPath=

# ==============================
# إعدادات الطباعة
# ==============================
RollPrinter=Samsung SCX-3400 Series
SheetPrinter=Samsung SCX-3400 Series

# ------------------------------
# إعدادات تخطيط الورق (Sheet Printing)
# ------------------------------
SheetRows=11
SheetCols=5
SheetMarginTop=24
SheetMarginBottom=24
SheetMarginRight=24
SheetMarginLeft=24

# ------------------------------
# إعدادات طباعة الرول (Roll Printing)
# ------------------------------
RollLabelWidth=50
RollLabelHeight=25

# ==============================
# بيانات الشركة
# ==============================
CompanyName=Sondos 4 kids
CompanyPhon=0001020506025
CompanyAnthrPhon=01201201205000
CompanyAdreass=5ش عبد الخالق ثروت العتبة وسط البلد القاهرة
EmailCo=Sondos 4 kids@gmail.com

# ------------------------------
# إعدادات الضرائب
# ------------------------------
SalesTax=0.14
IsEnablToChangTax=false
MaxRateDiscount=0.15

# ------------------------------
# إعدادات الشعار (Logo)
# ------------------------------
CompanyLoGoFolder=D:\MizanSoft\MizanLoom\Signee\Signee\bin\Debug
LogoImagName=Mizan Logo.PNG

# ==============================
# إعدادات المستودعات
# ==============================
DefaultWarehouseId=1
DefaultPrinter=Samsung SCX-3400 Series
DefaultWarehouse=0
DefaultStartDate=2025-01-01
DefaultEndDate=2025-12-31
DefaultRdoCheck=rdoThisYear


# ==============================
# اعدادات البيع والمردودات
# نظام الفواتير المرتدة فى المبيعات
# Mode=1 InvoiceOnly عن طريق فاتورة البيع الاصلية فقط
# Mode=2 FreeMode عن طريق كتابة اى كود صنف بحرية
# Mode=3 MixedMode عن طريق النظامين ايهما يختار المستخدم
# ==============================
ReturnSaleMode=
IsSaleByNegativeStock=False

*/
