using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MizanOriginalSoft.MainClasses.OriginalClasses
{
    public static class UserPermissionsManager
    {
        public static Dictionary<string, UserPermissionInfo> Permissions { get; private set; } = new();

        // تحميل الصلاحيات بعد الدخول
        public static void LoadPermissions(DataTable tblPermissions)
        {
            Permissions.Clear();

            foreach (DataRow row in tblPermissions.Rows)
            {
                string? permissionName = row["PermissionName"]?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(permissionName)) continue;

                var info = new UserPermissionInfo
                {
                    CanView = Convert.ToBoolean(row["IsAllowed"]),
                    CanAdd = Convert.ToBoolean(row["CanAdd"]),
                    CanEdit = Convert.ToBoolean(row["CanEdit"]),
                    CanDelete = Convert.ToBoolean(row["CanDelete"])
                };


                Permissions[permissionName] = info;
            }
        }

        // استعلام الصلاحيات بشكل سهل في أي مكان
        public static bool CanView(string permissionName) =>
            Permissions.TryGetValue(permissionName, out var p) && p.CanView;

        public static bool CanAdd(string permissionName) =>
            Permissions.TryGetValue(permissionName, out var p) && p.CanAdd;

        public static bool CanEdit(string permissionName) =>
            Permissions.TryGetValue(permissionName, out var p) && p.CanEdit;

        public static bool CanDelete(string permissionName) =>
            Permissions.TryGetValue(permissionName, out var p) && p.CanDelete;
    }
}

