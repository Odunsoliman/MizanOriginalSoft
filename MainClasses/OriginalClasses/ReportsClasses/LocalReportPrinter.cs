using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Reporting.WinForms;
using System.Collections.Generic;
using System.Drawing;

namespace MizanOriginalSoft.MainClasses.OriginalClasses.ReportsClasses
{
    public static class LocalReportPrinter
    {
        private static List<Stream>? _pages; 
        private static int _currentPageIndex;
        public static void Print(LocalReport report, string printerName)
        {
            _pages = new List<Stream>();
            _currentPageIndex = 0;

            Warning[] warnings;
            string[] streamIds;
            string mimeType, encoding, extension;

            // تحويل التقرير إلى صورة EMF
            report.Render(
                "Image",
                @"<DeviceInfo>
                    <OutputFormat>EMF</OutputFormat>
                    <PageWidth>8.27in</PageWidth>
                    <PageHeight>11.69in</PageHeight>
                    <MarginTop>0.25in</MarginTop>
                    <MarginLeft>0.25in</MarginLeft>
                    <MarginRight>0.25in</MarginRight>
                    <MarginBottom>0.25in</MarginBottom>
                  </DeviceInfo>",
                out mimeType, out encoding, out extension,
                out streamIds, out warnings);
            //Microsoft.Reporting.WinForms.LocalProcessingException: 'An error occurred during local report processing.'

            foreach (var streamId in streamIds)
            {
                var pageStream = new MemoryStream(report.Render("Image", null, out mimeType, out encoding, out extension, out streamIds, out warnings));
                _pages.Add(pageStream);
            }

            using (var printDoc = new PrintDocument())
            {
                printDoc.PrinterSettings.PrinterName = printerName;
                printDoc.PrintPage += PrintPage;
                printDoc.EndPrint += (s, e) =>
                {
                    if (_pages != null)
                    {
                        foreach (var stream in _pages)
                            stream.Dispose();
                        _pages.Clear();
                    }
                };
                printDoc.Print();
            }
        }

        private static void PrintPage(object? sender, PrintPageEventArgs ev)
        {
            if (ev == null || ev.Graphics == null)
                return;

            // التأكد من أن القائمة موجودة وبها صفحات
            if (_pages == null || _pages.Count == 0 || _currentPageIndex < 0 || _currentPageIndex >= _pages.Count)
            {
                ev.HasMorePages = false;
                return;
            }

            Stream? pageStream = _pages[_currentPageIndex];
            if (pageStream != null)
            {
                pageStream.Position = 0;
                using (Metafile pageImage = new Metafile(pageStream))
                {
                    ev.Graphics.DrawImage(pageImage, ev.PageBounds);
                }
            }
            else
            {
                ev.Graphics.DrawString(
                    "صفحة مفقودة أو غير صالحة",
                    new Font("Arial", 14, FontStyle.Bold),
                    Brushes.Red,
                    ev.PageBounds
                );
            }

            _currentPageIndex++;
            ev.HasMorePages = (_currentPageIndex < _pages.Count);
        }



    }
}
