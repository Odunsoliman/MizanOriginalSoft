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
        private bool isAnimating = false;

        public frmDailyDashBoard()
        {
            InitializeComponent();

            panels = new Panel[] { pnl0, pnl1, pnl2 };
            this.Load += frmDailyDashBoard_Load;
            this.Resize += frmDailyDashBoard_Resize;
        }

        private void frmDailyDashBoard_Load(object? sender, EventArgs e)
        {

        }

   


  
    
        private void frmDailyDashBoard_Resize(object? sender, EventArgs e)
        {

        }

  

        #region 🖱️ Mouse Effects
        private void lblNext_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Label lbl && lbl.Enabled)
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
                lbl.ForeColor = lbl.Enabled ? Color.Black : Color.Gray;
            }
        }
        private void lblPrev_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Label lbl && lbl.Enabled)
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
                lbl.ForeColor = lbl.Enabled ? Color.Black : Color.Gray;
            }
        }

        private void lbl_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Label lbl && lbl.Enabled)
            {
                lbl.BackColor = Color.LightBlue;
                lbl.ForeColor = Color.DarkBlue;
                lbl.Cursor = Cursors.Hand;
            }
        }

        private void lbl_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Label lbl)
            {
                lbl.BackColor = Color.Transparent;
                lbl.ForeColor = lbl.Enabled ? Color.Black : Color.Gray;
            }
        }
        #endregion


        private Panel? currentPanel = null;
        private Panel? targetPanel = null;
        private int targetHeight = 0;
        private bool isClosing = false;
        private readonly System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();
        //private readonly System.Windows.Forms.Timer innerTimer = new System.Windows.Forms.Timer();

        //private Panel? currentInnerPanel = null;
        //private Panel? targetInnerPanel = null;
        //private int innerTargetHeight = 0;
        //private bool isInnerClosing = false;
        private void TogglePanel(Panel panel)
        {
            if (animationTimer.Enabled) return;

            if (panel.Width  > panel.MinimumSize.Width )
            {
                targetPanel = panel;
                targetHeight = panel.MinimumSize.Width;
                isClosing = true;
            }
            else
            {
                if (currentPanel != null && currentPanel != panel)
                    currentPanel.Width = currentPanel.MinimumSize.Width;

                targetPanel = panel;
                targetHeight = panel.MaximumSize.Width;
                isClosing = false;
            }

            currentPanel = targetPanel;
            animationTimer.Start();
        }


    }
}



//using System;
//using System.Drawing;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace MizanOriginalSoft.Views.Forms.MainForms
//{
//    public partial class frmDailyDashBoard : Form
//    {
//        private readonly Panel[] panels;
//        private int currentPanelIndex = 0;
//        private bool isAnimating = false;

//        public frmDailyDashBoard()
//        {
//            InitializeComponent();

//            panels = new Panel[] { pnl0, pnl1, pnl2 };

//            // 🟢 تهيئة الحالة عند الفتح
//            this.Load += frmDailyDashBoard_Load;
//        }

//        private void frmDailyDashBoard_Load(object? sender, EventArgs e)
//        {
//            // تأكد من أن pnlContainer محمّلة
//            if (pnlContainer == null) return;

//            pnlContainer.SuspendLayout();

//            // 🔹 pnl0 مفتوحة
//            panels[0].Width = (int)(pnlContainer.Width * 0.98);

//            // 🔹 الباقي مغلق
//            for (int i = 1; i < panels.Length; i++)
//                panels[i].Width = (int)(pnlContainer.Width * 0.02);

//            pnlContainer.ResumeLayout();

//            // 🔹 تحديث حالة الأسهم
//            UpdateNavigationButtons();
//        }

//        private async void lblNext_Click(object sender, EventArgs e)
//        {
//            if (isAnimating || currentPanelIndex >= panels.Length - 1) return;

//            isAnimating = true;
//            pnlContainer.SuspendLayout();
//            await CollapsePanel(panels[currentPanelIndex]);
//            currentPanelIndex++;
//            await ExpandPanel(panels[currentPanelIndex]);
//            pnlContainer.ResumeLayout();
//            isAnimating = false;

//            UpdateNavigationButtons();
//        }

//        private async void lblPrev_Click(object sender, EventArgs e)
//        {
//            if (isAnimating || currentPanelIndex <= 0) return;

//            isAnimating = true;
//            pnlContainer.SuspendLayout();
//            await CollapsePanel(panels[currentPanelIndex]);
//            currentPanelIndex--;
//            await ExpandPanel(panels[currentPanelIndex]);
//            pnlContainer.ResumeLayout();
//            isAnimating = false;

//            UpdateNavigationButtons();
//        }

//        private async Task CollapsePanel(Panel pnl)
//        {
//            int targetWidth = (int)(pnlContainer.Width * 0.02);
//            while (pnl.Width > targetWidth)
//            {
//                pnl.Width = Math.Max(pnl.Width - 20, targetWidth);
//                await Task.Delay(5);
//            }
//        }

//        private async Task ExpandPanel(Panel pnl)
//        {
//            int targetWidth = (int)(pnlContainer.Width * 0.98);
//            while (pnl.Width < targetWidth)
//            {
//                pnl.Width = Math.Min(pnl.Width + 20, targetWidth);
//                await Task.Delay(5);
//            }
//            pnl.Width = targetWidth;
//        }

//        private void frmDailyDashBoard_Resize(object sender, EventArgs e)
//        {
//            if (pnlContainer == null || panels == null) return;

//            pnlContainer.SuspendLayout();
//            for (int i = 0; i < panels.Length; i++)
//            {
//                if (i == currentPanelIndex)
//                    panels[i].Width = (int)(pnlContainer.Width * 0.98);
//                else
//                    panels[i].Width = (int)(pnlContainer.Width * 0.02);
//            }
//            pnlContainer.ResumeLayout();
//        }

//        private void UpdateNavigationButtons()
//        {
//            // 🔹 في أول بانل
//            if (currentPanelIndex == 0)
//            {
//                lblPrev.Enabled = false;
//                lblPrev.ForeColor = Color.Gray;
//                lblNext.Enabled = true;
//                lblNext.ForeColor = Color.Black;
//            }
//            // 🔹 في آخر بانل
//            else if (currentPanelIndex == panels.Length - 1)
//            {
//                lblNext.Enabled = false;
//                lblNext.ForeColor = Color.Gray;
//                lblPrev.Enabled = true;
//                lblPrev.ForeColor = Color.Black;
//            }
//            // 🔹 بينهما
//            else
//            {
//                lblPrev.Enabled = true;
//                lblNext.Enabled = true;
//                lblPrev.ForeColor = Color.Black;
//                lblNext.ForeColor = Color.Black;
//            }
//        }

//        #region 🖱️ Mouse Effects
//        private void lblNext_MouseEnter(object sender, EventArgs e)
//        {
//            if (sender is Label lbl && lbl.Enabled)
//            {
//                lbl.BackColor = Color.LightBlue;
//                lbl.ForeColor = Color.DarkBlue;
//                lbl.Cursor = Cursors.Hand;
//            }
//        }

//        private void lblNext_MouseLeave(object sender, EventArgs e)
//        {
//            if (sender is Label lbl)
//            {
//                lbl.BackColor = Color.Transparent;
//                lbl.ForeColor = lbl.Enabled ? Color.Black : Color.Gray;
//            }
//        }

//        private void lblPrev_MouseEnter(object sender, EventArgs e)
//        {
//            if (sender is Label lbl && lbl.Enabled)
//            {
//                lbl.BackColor = Color.LightBlue;
//                lbl.ForeColor = Color.DarkBlue;
//                lbl.Cursor = Cursors.Hand;
//            }
//        }

//        private void lblPrev_MouseLeave(object sender, EventArgs e)
//        {
//            if (sender is Label lbl)
//            {
//                lbl.BackColor = Color.Transparent;
//                lbl.ForeColor = lbl.Enabled ? Color.Black : Color.Gray;
//            }
//        }
//        #endregion
//    }
//}
