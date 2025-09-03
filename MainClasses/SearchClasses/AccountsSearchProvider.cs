using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace MizanOriginalSoft.MainClasses.SearchClasses
{
    public class AccountsSearchProvider : ISearchProvider
    {
        private readonly string _accountType; // Customers, Suppliers, Boths, Parteners

        public AccountsSearchProvider(string accountType)
        {
            _accountType = accountType ?? throw new ArgumentNullException(nameof(accountType));
        }

        public string Title => "البحث في الحسابات";

        public DataTable GetData(string filter)
        {
            // لو مفيش فلتر، رجّع كل الحسابات حسب النوع
            DataTable dt = DBServiecs.MainAcc_GetParentAccounts(_accountType);

            if (string.IsNullOrWhiteSpace(filter))
                return dt;

            // فلترة بالاسم أو رقم التليفون
            string expression = $"AccName LIKE '%{filter.Replace("'", "''")}%' " +
                                $"OR FirstPhon LIKE '%{filter.Replace("'", "''")}%' " +
                                $"OR AntherPhon LIKE '%{filter.Replace("'", "''")}%'";
            try
            {
                DataRow[] rows = dt.Select(expression);
                if (rows.Length > 0)
                    return rows.CopyToDataTable();
                else
                    return dt.Clone(); // يرجع جدول فاضي بنفس الأعمدة
            }
            catch
            {
                return dt; // في حالة أي خطأ رجّع الجدول الأصلي
            }
        }

        public string GetSelectedCode(DataGridViewRow row)
        {
            if (row?.Cells["AccID"]?.Value == null)
                return string.Empty;

            return row.Cells["AccID"].Value.ToString() ?? string.Empty;
        }

        public void ApplyGridFormatting(DataGridView dgv)
        {
            // أولاً: إظهار فقط الأعمدة المطلوبة
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                col.Visible = false; // إخفاء الكل
            }

            // إعداد الأعمدة المطلوبة
            if (dgv.Columns.Contains("AccID"))
            {
                dgv.Columns["AccID"].Visible = true;
                dgv.Columns["AccID"].HeaderText = "كود";
                dgv.Columns["AccID"].FillWeight = 20; // نسبة العرض
            }

            if (dgv.Columns.Contains("AccName"))
            {
                dgv.Columns["AccName"].Visible = true;
                dgv.Columns["AccName"].HeaderText = "اسم الحساب";
                dgv.Columns["AccName"].FillWeight = 60; // نسبة العرض
            }

            if (dgv.Columns.Contains("Balance"))
            {
                dgv.Columns["Balance"].Visible = true;
                dgv.Columns["Balance"].HeaderText = "الرصيد";
                dgv.Columns["Balance"].FillWeight = 20; // نسبة العرض
            }

            if (dgv.Columns.Contains("BalanceState"))
            {
                dgv.Columns["BalanceState"].Visible = true;
                dgv.Columns["BalanceState"].HeaderText = "--";
                dgv.Columns["BalanceState"].FillWeight = 20; // نسبة العرض
            }

            // إعداد خصائص الجدول
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
        }

    }
}




/*
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MizanOriginalSoft.MainClasses.SearchClasses
{
    public class AccountsSearchProvider : ISearchProvider
    {
        private readonly string _accountType; // Customers, Suppliers, Boths, Parteners

        public AccountsSearchProvider(string accountType)
        {
            _accountType = accountType ?? throw new ArgumentNullException(nameof(accountType));
        }

        public string Title => "البحث في الحسابات";

        public DataTable GetData(string filter)
        {
            // استدعاء الإجراء المخزن وتمرير نوع الحساب
            return DBServiecs.MainAcc_GetParentAccounts(_accountType);
        }

        public string GetSelectedCode(DataGridViewRow row)
        {
            if (row?.Cells["AccID"]?.Value == null)
                return string.Empty;

            return row.Cells["AccID"].Value.ToString() ?? string.Empty;
        }

        public void ApplyGridFormatting(DataGridView dgv)
        {
            if (!dgv.Columns.Contains("AccID") || !dgv.Columns.Contains("AccName"))
                return;

            dgv.Columns["AccID"].HeaderText = "كود الحساب";
            dgv.Columns["AccName"].HeaderText = "اسم الحساب";

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
    }

}

*/