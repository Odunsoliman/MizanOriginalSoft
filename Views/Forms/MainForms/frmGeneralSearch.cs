using MizanOriginalSoft.MainClasses.SearchClasses;
using System;
using System.Data;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.MainForms
{
    public partial class frmGeneralSearch : Form
    {
        private readonly ISearchProvider _provider;

        public frmGeneralSearch(ISearchProvider provider)
        {
            InitializeComponent();
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            Load += frmGeneralSearch_Load;
            txtSearch.TextChanged += txtSearch_TextChanged;
            DGV.CellDoubleClick += DGV_CellDoubleClick;
        }

        private void frmGeneralSearch_Load(object? sender, EventArgs e)
        {
            lblTitel.Text = _provider.Title;
            LoadData();
        }

        private void txtSearch_TextChanged(object? sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            var data = _provider.GetData(txtSearch.Text.Trim());
            DGV.DataSource = data;
            lblcountResulte.Text = $"عدد النتائج: {data.Rows.Count}";
            _provider.ApplyGridFormatting(DGV);
        }

        private void DGV_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            
            if (e.RowIndex < 0) return;

            var selected = _provider.GetSelectedItem(DGV.Rows[e.RowIndex]);
            this.Tag = selected;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
