using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MizanOriginalSoft.MainClasses.OriginalClasses
{
    public static class CurrentSession
    {
        public static int UserID { get; set; }
        public static string UserName { get; set; } = string.Empty;

        public static bool IsAdmin => UserName.ToLower() == "admin";
    }

}

