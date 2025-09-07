using MizanOriginalSoft.MainClasses.OriginalClasses;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.Movments
{
    public partial class frmPOS : Form
    {
        // 🔹 متغير يحدد إذا كان مسموح البيع بدون رصيد (على المكشوف)
        private bool allowNegativeStock;

        // 🔹 متغير يحدد إذا كان المرتجع يشترط إدخال رقم فاتورة البيع
        private bool reSaleByInvoiceSale;

        public frmPOS()
        {
            InitializeComponent();
        }

        private void frmPOS_Load(object sender, EventArgs e)
        {
            // ✅ تحميل ملف الإعدادات مرة واحدة فقط
            if (!AppSettingsIsLoaded())
            {
                string settingsPath = Path.Combine(Application.StartupPath, "serverConnectionSettings.txt");
                AppSettings.Load(settingsPath);
            }

            // ✅ قراءة الإعدادات
            LoadSettings();

            // ✅ ضبط الواجهة حسب الاختيار الافتراضي
            if (rdoSale.Checked)
                UpdateLabelsForSale();
            else if (rdoResale.Checked)
                UpdateLabelsForResale();
        }

        // ✅ التحقق إذا كان AppSettings متحمل
        private bool AppSettingsIsLoaded()
        {
            try
            {
                // لو حاولنا قراءة أي قيمة من غير تحميل هيعمل Exception
                AppSettings.GetAllSettings();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ✅ تحميل القيم من ملف الإعدادات
        private void LoadSettings()
        {
            allowNegativeStock = AppSettings.GetBool("NegativeStockSale");
            reSaleByInvoiceSale = AppSettings.GetBool("ReSaleByInvoiceSale");
        }

        private void rdoSale_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoSale.Checked)
            {
                tlpTyoe_color();       // تغيير الألوان
                UpdateLabelsForSale(); // تحديث النصوص
            }
        }

        private void rdoResale_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoResale.Checked)
            {
                tlpTyoe_color();        // تغيير الألوان
                UpdateLabelsForResale();// تحديث النصوص
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
            lblInvStat.Text = ""; // ممكن تكتب "مرتجع" لو تحب

            if (reSaleByInvoiceSale)
                lblCodeTitel.Text = "ادخل رقم فاتورة البيع";
            else
                lblCodeTitel.Text = "ادخل كود الصنف";
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
    }
}
