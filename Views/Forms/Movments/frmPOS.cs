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
                // لون فاتح يعبر عن البيع
                tlpType.BackColor = Color.LightGreen;
            }
            else if (rdoResale.Checked)
            {
                // لون فاتح يعبر عن البيع المرتد
                tlpType.BackColor = Color.LightPink;
            }
            else
            {
                // لون افتراضي لو ما فيش اختيار
                tlpType.BackColor = SystemColors.Control;
            }
        }

    }
}
