using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MizanOriginalSoft.MainClasses.SearchClasses
{
    public class ProductsSearchProvider : ISearchProvider
    {
        public string Title => "البحث في الأصناف";

        public DataTable GetData(string filter)
        {
            // استدعاء الإجراء المخزن الخاص بالأصناف
            return DBServiecs.ProductSearch_GetAll();
        }

        public string GetSelectedCode(DataGridViewRow row)
        {
            if (row?.Cells["ProductID"]?.Value == null)
                return string.Empty;

            return row.Cells["ProductID"].Value.ToString() ?? string.Empty;
        }

        public void ApplyGridFormatting(DataGridView dgv)
        {
            dgv.Columns["ProductID"].HeaderText = "كود الصنف";
            dgv.Columns["ProductName"].HeaderText = "اسم الصنف";
            dgv.Columns["CategoryName"].HeaderText = "التصنيف";
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.Beige;
        }
    }

}
