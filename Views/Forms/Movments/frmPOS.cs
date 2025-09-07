using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.Movments
{
    public partial class frmPOS : Form
    {
        public frmPOS()
        {
            InitializeComponent();
        }

        private void cbxPiece_ID_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lblMinLinth_Click(object sender, EventArgs e)
        {

        }

        private void rdoSale_CheckedChanged(object sender, EventArgs e)
        {
            tlpTyoe_color();
        }

        private void rdoResale_CheckedChanged(object sender, EventArgs e)
        {
            tlpTyoe_color();
        }

        private void tlpTyoe_color()
        {
            if (rdoSale.Checked)
            {
                // لون بيع: أخضر فاتح جدًا
                tlpType.BackColor = Color.FromArgb(230, 255, 230);
                tlpHader .BackColor = Color.FromArgb(230, 255, 230);
                lblTafqet.BackColor = Color.FromArgb(230, 255, 230);
            }
            else if (rdoResale.Checked)
            {
                // لون بيع مرتد: وردي فاتح جدًا
                tlpType.BackColor = Color.FromArgb(255, 230, 230);
                tlpHader.BackColor = Color.FromArgb(255, 230, 230);
                lblTafqet .BackColor = Color.FromArgb(255, 230, 230);
            }
            else
            {
                // اللون الافتراضي
                tlpType.BackColor = SystemColors.Control;
            }
        }

    }
}
