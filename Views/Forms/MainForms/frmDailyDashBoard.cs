using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.MainForms
{
    public partial class frmDailyDashBoard : Form
    {
        private readonly Panel[] panels;
        private int currentPanelIndex = 0;
        private bool isAnimating = false; // 🔒 لتجنب بدء حركة جديدة أثناء حركة جارية

        public frmDailyDashBoard()
        {
            InitializeComponent();

            panels = new Panel[] { pnl0, pnl1, pnl2 };
        }

        private async void lblNext_Click(object sender, EventArgs e)
        {
            if (isAnimating) return; // ⛔ منع الحركة أثناء أخرى

            if (currentPanelIndex < panels.Length - 1)
            {
                isAnimating = true;
                pnlContainer.SuspendLayout(); // 🧱 إيقاف الترتيب المؤقت
                await CollapsePanel(panels[currentPanelIndex]);
                currentPanelIndex++;
                await ExpandPanel(panels[currentPanelIndex]);
                pnlContainer.ResumeLayout(); // ✅ استئناف الترتيب
                isAnimating = false;
            }
        }

        private async void lblPrev_Click(object sender, EventArgs e)
        {
            if (isAnimating) return;

            if (currentPanelIndex > 0)
            {
                isAnimating = true;
                pnlContainer.SuspendLayout();
                await CollapsePanel(panels[currentPanelIndex]);
                currentPanelIndex--;
                await ExpandPanel(panels[currentPanelIndex]);
                pnlContainer.ResumeLayout();
                isAnimating = false;
            }
        }

        private async Task CollapsePanel(Panel pnl)
        {
            int targetWidth = (int)(pnlContainer.Width * 0.02); // 2%
            while (pnl.Width > targetWidth)
            {
                pnl.Width = Math.Max(pnl.Width - 20, targetWidth);
                await Task.Delay(5);
            }
        }

        private async Task ExpandPanel(Panel pnl)
        {
            int targetWidth = (int)(pnlContainer.Width * 0.98); // 98%
            while (pnl.Width < targetWidth)
            {
                pnl.Width = Math.Min(pnl.Width + 20, targetWidth);
                await Task.Delay(5);
            }
            pnl.Width = targetWidth;
        }

        private void frmDailyDashBoard_Resize(object sender, EventArgs e)
        {
            if (pnlContainer == null || panels == null) return;

            pnlContainer.SuspendLayout();
            for (int i = 0; i < panels.Length; i++)
            {
                if (i == currentPanelIndex)
                    panels[i].Width = (int)(pnlContainer.Width * 0.98);
                else
                    panels[i].Width = (int)(pnlContainer.Width * 0.02);
            }
            pnlContainer.ResumeLayout();
        }

        #region 🖱️ Mouse Effects
        private void lblNext_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Label lbl)
            {
                lbl.BackColor = Color.LightBlue;
                lbl.ForeColor = Color.DarkBlue;
                lbl.Cursor = Cursors.Hand;
            }
        }

        private void lblNext_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Label lbl)
            {
                lbl.BackColor = Color.Transparent;
                lbl.ForeColor = Color.Black;
            }
        }

        private void lblPrev_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Label lbl)
            {
                lbl.BackColor = Color.LightBlue;
                lbl.ForeColor = Color.DarkBlue;
                lbl.Cursor = Cursors.Hand;
            }
        }

        private void lblPrev_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Label lbl)
            {
                lbl.BackColor = Color.Transparent;
                lbl.ForeColor = Color.Black;
            }
        }
        #endregion
    }
}
