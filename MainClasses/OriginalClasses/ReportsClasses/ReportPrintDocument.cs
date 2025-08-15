
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace MizanOriginalSoft.MainClasses.OriginalClasses.ReportsClasses
{
    public class ReportPrintDocument
    {
        //private readonly string _reportCodeName;
        //private readonly string _datasetName;
        //private readonly DataTable _data;
        //private readonly Dictionary<string, object> _parameters;

        //public ReportPrintDocument(string reportCodeName, string datasetName, DataTable data, Dictionary<string, object> parameters)
        //{
        //    _reportCodeName = reportCodeName;
        //    _datasetName = datasetName;
        //    _data = data;
        //    _parameters = parameters;
        //}

        //public void Print()
        //{
        //    try
        //    {
        //        // إنشاء LocalReport
        //        LocalReport report = new LocalReport();
        //        report.ReportPath = Path.Combine(Application.StartupPath, "Reports", _reportCodeName + ".rdlc");

        //        // إضافة مصدر البيانات
        //        report.DataSources.Clear();
        //        report.DataSources.Add(new ReportDataSource(_datasetName, _data));

        //        // تمرير الباراميترات إذا وجدت
        //        if (_parameters != null && _parameters.Count > 0)
        //        {
        //            var reportParams = _parameters
        //                .Select(p => new ReportParameter(p.Key, p.Value?.ToString() ?? ""));
        //            report.SetParameters(reportParams);
        //        }

        //        // توليد الملف بصيغة EMF (Enhanced Metafile) للطباعة
        //        Warning[] warnings;
        //        string[] streamIds;
        //        string mimeType;
        //        string encoding;
        //        string extension;

        //        byte[] bytes = report.Render(
        //            "IMAGE", // صيغة الطباعة
        //            "<DeviceInfo><OutputFormat>EMF</OutputFormat></DeviceInfo>",
        //            out mimeType, out encoding, out extension, out streamIds, out warnings
        //        );

        //        // حفظ مؤقت وطباعته
        //        using (var stream = new MemoryStream(bytes))
        //        {
        //            PrintDocument printDoc = new PrintDocument();
        //            printDoc.PrintPage += (sender, e) =>
        //            {
        //                using (Metafile pageImage = new Metafile(stream))
        //                {
        //                    e.Graphics.DrawImage(pageImage, e.PageBounds);//Dereference of a possibly null reference.
        //                }
        //            };

        //            printDoc.Print();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("خطأ أثناء الطباعة: " + ex.Message);
        //    }
        //}
    }
}

