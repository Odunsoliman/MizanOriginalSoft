using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.MainForms
{
    public partial class frmDailyDashBoard : Form
    {
        public frmDailyDashBoard()
        {
            InitializeComponent();
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
