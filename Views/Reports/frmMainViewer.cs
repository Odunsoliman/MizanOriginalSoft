using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using MizanOriginalSoft.MainClasses;

namespace MizanOriginalSoft.Views.Reports
{
    public partial class frmMainViewer : Form
    {
        private string _reportName;
        private string _dataSetName;
        private DataTable _data;
        private Dictionary<string, object> _parameters;

        public frmMainViewer(string reportName, string dataSetName, DataTable data, Dictionary<string, object> parameters)
        {
            InitializeComponent();
            _reportName = reportName;
            _dataSetName = dataSetName;
            _data = data;
            _parameters = parameters;
        }

        private void frmMainViewer_Load(object sender, EventArgs e)
        {
            reportViewer1.Reset();
            string reportPath = Path.Combine(Application.StartupPath, "Reports", _reportName + ".rdlc");
            reportViewer1.LocalReport.ReportPath = reportPath;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(new ReportDataSource(_dataSetName, _data));

            // تمرير الباراميترات
            var reportParams = _parameters
                .Select(p => new ReportParameter(p.Key, p.Value?.ToString() ?? ""))
                .ToArray();
            reportViewer1.LocalReport.SetParameters(reportParams);

            reportViewer1.RefreshReport();
        }
    }


}

