using MizanOriginalSoft.MainClasses.OriginalClasses;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.MainForms
{
    public partial class frmHelp : Form
    {
        private readonly string helpKey;

        public frmHelp(string helpKey)
        {
            InitializeComponent();
            this.helpKey = helpKey;
        }

        private void frmHelp_Load(object sender, EventArgs e)
        {
            string helpText = HelpTextReader.GetHelpText(helpKey);
            DisplayFormattedHelp(helpText);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            HighlightSearchText(txtSearch.Text.Trim());
        }

        private void HighlightSearchText(string searchText)
        {
            // إعادة تعيين الخلفية لجميع النصوص
            rtxtHelp.SelectAll();
            rtxtHelp.SelectionBackColor = rtxtHelp.BackColor;

            if (string.IsNullOrWhiteSpace(searchText))
                return;

            int startIndex = 0;
            while (startIndex < rtxtHelp.TextLength)
            {
                int index = rtxtHelp.Text.IndexOf(searchText, startIndex, StringComparison.OrdinalIgnoreCase);
                if (index < 0)
                    break;

                rtxtHelp.Select(index, searchText.Length);
                rtxtHelp.SelectionBackColor = Color.Yellow;

                startIndex = index + searchText.Length;
            }

            rtxtHelp.Select(0, 0); // إزالة التحديد
        }

        private void DisplayFormattedHelp(string helpText)
        {
            rtxtHelp.Clear();

            string[] lines = helpText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    rtxtHelp.AppendText(Environment.NewLine);
                    continue;
                }

                if (line.StartsWith("@"))
                {
                    // عنوان الوظيفة
                    rtxtHelp.SelectionColor = Color.DarkBlue;
                    rtxtHelp.SelectionFont = new Font(rtxtHelp.Font, FontStyle.Bold);
                    rtxtHelp.AppendText(line.Substring(1).Trim() + Environment.NewLine);
                }
                else if (line.StartsWith("frm", StringComparison.OrdinalIgnoreCase))
                {
                    // تجاهل السطر المفتاحي
                    continue;
                }
                else
                {
                    // شرح الوظيفة
                    rtxtHelp.SelectionColor = Color.Purple;
                    rtxtHelp.SelectionFont = new Font(rtxtHelp.Font, FontStyle.Regular);
                    rtxtHelp.AppendText(line + Environment.NewLine);
                }
            }

            rtxtHelp.SelectionStart = 0;
            rtxtHelp.ScrollToCaret();
        }

        private void btnPlayVideo_Click(object sender, EventArgs e)
        {
            string videoPath = HelpTextReader.GetHelpVideoPath(helpKey);

            if (string.IsNullOrEmpty(videoPath))
            {
                MessageBox.Show("🎬 لا يوجد فيديو مساعدة متاح لهذه الوظيفة.", "تشغيل الفيديو", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                Process.Start("explorer", videoPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء محاولة تشغيل الفيديو:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
