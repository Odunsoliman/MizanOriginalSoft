using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MizanOriginalSoft.MainClasses.SearchClasses
{
    public interface ISearchProvider
    {
        string Title { get; }
        DataTable GetData(string filter);
        string GetSelectedCode(DataGridViewRow row);
        void ApplyGridFormatting(DataGridView dgv);
    }

}
