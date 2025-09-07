using MizanOriginalSoft.MainClasses.OriginalClasses;
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
            if (rdoSale.Checked)
            {
                tlpTyoe_color(); // تغيير الألوان
                UpdateLabelsForSale(); // تحديث النصوص للـ بيع
            }
        }

        private void rdoResale_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoResale.Checked)
            {
                tlpTyoe_color(); // تغيير الألوان
                UpdateLabelsForResale(); // تحديث النصوص للـ مرتجع
            }
        }

        // 🔹 تحديث النصوص لو اخترت "بيع"
        private void UpdateLabelsForSale()
        {
            if (allowNegativeStock)
                lblInvStat.Text = "البيع على مكشوف";
            else
                lblInvStat.Text = "البيع حسب الرصيد";

            lblCodeTitel.Text = "ادخل كود الصنف";
        }

        // 🔹 تحديث النصوص لو اخترت "مرتجع"
        private void UpdateLabelsForResale()
        {
            lblInvStat.Text = ""; // أو أي نص تراه مناسب

            if (reSaleByInvoiceSale)
                lblCodeTitel.Text = "ادخل رقم فاتورة البيع";
            else
                lblCodeTitel.Text = "ادخل كود الصنف";
        }

        private void frmPOS_Load(object sender, EventArgs e)
        {
            appsett();

            // 🔥 عرض النصوص مباشرة حسب الاختيار الافتراضي
            if (rdoSale.Checked)
                UpdateLabelsForSale();
            else if (rdoResale.Checked)
                UpdateLabelsForResale();
        }











        // 🔹 دالة تغيير ألوان واجهة الشاشة بناءً على نوع العملية المحدد
        private void tlpTyoe_color()
        {
            if (rdoSale.Checked)
            {
                // 🌿 لون أخضر فاتح جدًا لو العملية "بيع"
                tlpType.BackColor = Color.FromArgb(230, 255, 230);
                tlpHader.BackColor = Color.FromArgb(230, 255, 230);
                lblTafqet.BackColor = Color.FromArgb(230, 255, 230);
            }
            else if (rdoResale.Checked)
            {
                // 🌸 لون وردي فاتح جدًا لو العملية "بيع مرتجع"
                tlpType.BackColor = Color.FromArgb(255, 230, 230);
                tlpHader.BackColor = Color.FromArgb(255, 230, 230);
                lblTafqet.BackColor = Color.FromArgb(255, 230, 230);
            }
        }

        // 🔹 متغير يحدد إذا كان مسموح البيع بدون رصيد (على المكشوف)
        private bool allowNegativeStock;

        // 🔹 متغير يحدد إذا كان المرتجع يشترط إدخال رقم فاتورة البيع
        private bool reSaleByInvoiceSale;

        // 🔥 دالة تحميل الإعدادات من ملف AppSettings.txt
        private void appsett()
        {
            /*
              🔑 هنا نربط الخصائص بالإعدادات المخزنة:
              - إذا كان NegativeStockSale = true ➜ مسموح البيع بدون رصيد
              - إذا كان ReSaleByInvoiceSale = true ➜ المرتجع يشترط إدخال فاتورة البيع
            */

            // 📌 قراءة الإعداد الخاص بالسماح بالبيع بدون رصيد
            allowNegativeStock = AppSettings.GetBool("NegativeStockSale");

            if (allowNegativeStock)
            {
                lblInvStat.Text = "البيع على مكشوف"; // 🔹 يظهر في الواجهة
            }
            else
            {
                lblInvStat.Text = "البيع حسب الرصيد"; // 🔹 البيع مرتبط بوجود رصيد
            }

            // 📌 قراءة الإعداد الخاص بالمرتجع إذا كان يحتاج إدخال فاتورة
            reSaleByInvoiceSale = AppSettings.GetBool("ReSaleByInvoiceSale");

            if (reSaleByInvoiceSale)
            {
                lblCodeTitel.Text = "ادخل رقم فاتورة البيع"; // 🔹 يطلب فاتورة
            }
            else
            {
                lblCodeTitel.Text = "ادخل كود الصنف"; // 🔹 يكتفي بكود الصنف
            }
        }

 

    }
}
