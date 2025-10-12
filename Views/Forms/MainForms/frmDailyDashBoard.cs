using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.MainForms
{
    public partial class frmDailyDashBoard : Form
    {
        private readonly Panel[] panels;
        private readonly int[] minWidths = { 100, 100, 100 };
        private readonly int[] maxWidths = { 400, 400, 400 };
        private int currentPanelIndex = 0;

        public frmDailyDashBoard()
        {
            InitializeComponent();

            // ✅ تهيئة المصفوفة بعد تحميل مكونات الفورم
            panels = new Panel[] { pnl0, pnl1, pnl2 };
        }

        private async void lblNext_Click(object sender, EventArgs e)
        {
            if (currentPanelIndex < panels.Length - 1)
            {
                await CollapsePanel(panels[currentPanelIndex], minWidths[currentPanelIndex]);
                currentPanelIndex++;
                await ExpandPanel(panels[currentPanelIndex], maxWidths[currentPanelIndex]);
            }
        }

        private async void lblPrev_Click(object sender, EventArgs e)
        {
            if (currentPanelIndex > 0)
            {
                await CollapsePanel(panels[currentPanelIndex], minWidths[currentPanelIndex]);
                currentPanelIndex--;
                await ExpandPanel(panels[currentPanelIndex], maxWidths[currentPanelIndex]);
            }
        }

        private async Task CollapsePanel(Panel pnl, int minWidth)
        {
            while (pnl.Width > minWidth)
            {
                pnl.Width -= 20;
                await Task.Delay(10);
            }
        }

        private async Task ExpandPanel(Panel pnl, int maxWidth)
        {
            while (pnl.Width < maxWidth)
            {
                pnl.Width += 20;
                await Task.Delay(10);
            }
        }

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
    }
}
