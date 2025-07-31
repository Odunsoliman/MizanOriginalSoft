using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MizanOriginalSoft.Views.Forms.Products
{
    public partial class frm_ReturnedInvoice : Form
    {
        public DataTable  SelectedItems;
        public frm_ReturnedInvoice(int aa,int serInv,DataTable  tblInvoice,DataTable  tblDetails,int CurrentInvoiceID)
        {
            InitializeComponent();
        }
    }
}
