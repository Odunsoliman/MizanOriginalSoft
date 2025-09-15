using System.Windows.Forms;
using MizanOriginalSoft.Views.Forms.MainForms;

namespace MizanOriginalSoft.MainClasses.SearchClasses
{
    public static class SearchHelper
    {
        public static (string Code, string Name) ShowSearchDialog(ISearchProvider provider)
        {
            using var frm = new frmGeneralSearch(provider);

            if (frm.ShowDialog() == DialogResult.OK && frm.Tag is object obj)
            {
                if (obj is ValueTuple<string, string> tuple)
                    return tuple;
            }

            return (string.Empty, string.Empty);
        }
    }
}
