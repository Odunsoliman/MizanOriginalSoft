using MizanOriginalSoft.MainClasses.SearchClasses;
using MizanOriginalSoft.MainClasses;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.MainForms
{
    public partial class frmGeneralSearch : Form
    {
        private readonly ISearchProvider _provider;
        private bool _isInvoiceSearch;

        public frmGeneralSearch(ISearchProvider provider)
        {
            InitializeComponent();
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            Load += frmGeneralSearch_Load;
        }

        private void frmGeneralSearch_Load(object? sender, EventArgs e)
        {
            lblTitel.Text = _provider.Title;
            _isInvoiceSearch = _provider.Title.Contains("الفواتير");

            // 🔹 إظهار أدوات التاريخ والمستخدم فقط للفواتير
            tlpDate.Visible = _isInvoiceSearch;
            cbxUsers.Visible = _isInvoiceSearch;

            if (_isInvoiceSearch)
                LoadUsers();

            LoadData();
        }

        // 🔹 تحميل قائمة المستخدمين
        private void LoadUsers()
        {
            var users = DBServiecs.User_GetAll(); // افترض أن عندك دالة ترجع جدول المستخدمين
            cbxUsers.DataSource = users;
            cbxUsers.DisplayMember = "UserName";
            cbxUsers.ValueMember = "IDUser";
            cbxUsers.SelectedIndex = -1;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void cbxUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxUsers.Focused) LoadData();
        }

        private void dtpFrom_ValueChanged(object sender, EventArgs e)
        {
            if (dtpFrom.Focused) LoadData();
        }

        private void dtpTo_ValueChanged(object sender, EventArgs e)
        {
            if (dtpTo.Focused) LoadData();
        }

        private void btnClearFilters_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            cbxUsers.SelectedIndex = -1;
            dtpFrom.Value = DateTime.Now.AddMonths(-1);
            dtpTo.Value = DateTime.Now;
            LoadData();
        }

        private void LoadData()
        {
            var data = _provider.GetData(txtSearch.Text.Trim());

            // 🔹 فلترة إضافية للفواتير
            if (_isInvoiceSearch && data.Rows.Count > 0)
            {
                var query = data.AsEnumerable();

                // 🔹 فلترة بالمستخدم
                if (cbxUsers.SelectedIndex >= 0)
                {
                    var selectedUserId = cbxUsers.SelectedValue?.ToString();
                    if (!string.IsNullOrEmpty(selectedUserId))
                        query = query.Where(r => r["User_ID"].ToString() == selectedUserId);
                }

                // 🔹 فلترة بالتاريخ
                var from = dtpFrom.Value.Date;
                var to = dtpTo.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(r =>
                {
                    if (DateTime.TryParse(r["Inv_Date"]?.ToString(), out DateTime invDate))
                        return invDate >= from && invDate <= to;
                    return false;
                });

                // 🔹 فلترة بالحقل Saved
                query = query.Where(r => !string.IsNullOrEmpty(r["Saved"]?.ToString()));

                if (query.Any())
                    data = query.CopyToDataTable();
                else
                    data = data.Clone();
            }

            DGV.DataSource = data;
            lblcountResulte.Text = $"عدد النتائج: {data.Rows.Count}";
            _provider.ApplyGridFormatting(DGV);
        }

        private void DGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selected = _provider.GetSelectedItem(DGV.Rows[e.RowIndex]);
                this.Tag = selected;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
