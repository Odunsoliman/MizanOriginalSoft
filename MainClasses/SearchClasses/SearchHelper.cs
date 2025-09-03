using MizanOriginalSoft.Views.Forms.MainForms;
using System;
using System.Windows.Forms;
using static MizanOriginalSoft.Views.Forms.MainForms.frmSearch;

namespace MizanOriginalSoft.MainClasses.SearchClasses
{
    public static class SearchHelper
    {
        /// <summary>
        /// فتح شاشة البحث في الحسابات وإرجاع كود الحساب المختار.
        /// </summary>
        public static string SearchAccount(string accountType)
        {
            var provider = new AccountsSearchProvider(accountType);
            using (var frm = new frmGeneralSearch(provider))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                    return frm.Tag as string ?? string.Empty;

                return string.Empty;
            }
        }

        /// <summary>
        /// فتح شاشة البحث باستخدام Enum SearchEntityType
        /// </summary>
        public static string SearchAccount(SearchEntityType type)
        {
            return SearchAccount(type.ToString());
        }
    }
}




/*
using MizanOriginalSoft.Views.Forms.MainForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MizanOriginalSoft.Views.Forms.MainForms.frmSearch;

namespace MizanOriginalSoft.MainClasses.SearchClasses
{
    public static class SearchHelper
    {
        /// <summary>
        /// فتح شاشة البحث في الحسابات وإرجاع كود الحساب المختار.
        /// </summary>
        /// <param name="accountType">Customers / Suppliers / Boths / Parteners</param>
        /// <returns>كود الحساب أو نص فارغ</returns>
        public static string SearchAccount(string accountType)
        {
            var provider = new AccountsSearchProvider(accountType); // مش محتاج using
            using (var frm = new frmGeneralSearch(provider))
            {
                if (frm.ShowDialog() == DialogResult.OK)
                    return frm.Tag as string ?? string.Empty;

                return string.Empty;
            }
        }


        /// <summary>
        /// فتح شاشة البحث باستخدام Enum SearchEntityType
        /// </summary>
        public static string SearchAccount(SearchEntityType type)
        {
            return SearchAccount(type.ToString());
        }
    }


}
*/