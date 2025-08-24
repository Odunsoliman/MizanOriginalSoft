using System;
using System.Drawing;
using System.Windows.Forms;

namespace MizanOriginalSoft .Views.Forms.MainForms
{
    public partial class DateInputForm : Form
    {


        // الخاصية العامة التي ترجع التاريخ المختار
        public DateTime SelectedDate => dtp.Value;

        public DateInputForm(string title)
        {
            InitializeComponent();
            lblTitle.Text = title;
            // لا نحدد MinDate هنا، حتى نترك التحكم للمنادِي
        }

   
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
