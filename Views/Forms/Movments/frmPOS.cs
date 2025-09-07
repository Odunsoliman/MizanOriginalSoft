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
            // ✅ تحميل ملف الإعدادات
            if (!AppSettingsIsLoaded())
            {
                string settingsPath = Path.Combine(Application.StartupPath, "AppSettings.txt");
                AppSettings.Load(settingsPath);
            }

            // ✅ قراءة الإعدادات
            LoadSettings();

            //// ✅ رسالة للتأكد من القيم
            //MessageBox.Show(
            //    $"NegativeStockSale = {allowNegativeStock}\n" +
            //    $"ReSaleByInvoiceSale = {reSaleByInvoiceSale}",
            //    "📌 قيم الإعدادات",
            //    MessageBoxButtons.OK,
            //    MessageBoxIcon.Information
            //);

            // ✅ ضبط الواجهة حسب الاختيار الافتراضي
            if (rdoSale.Checked)
                UpdateLabelsForSale();
            else if (rdoResale.Checked)
                UpdateLabelsForResale();
        }



        #region ========  ضبط اختيارات البيع والبيع المرتد ============
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
                lblTypeInv.Text = "فاتورة بيع رقم: ";
                lblTypeInvID.Text = "1";
            }
        }

        private void rdoResale_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoResale.Checked)
            {
                tlpTyoe_color();        // تغيير الألوان
                UpdateLabelsForResale();// تحديث النصوص
                lblTypeInv.Text = "فاتورة بيع مرتد رقم: ";
                lblTypeInvID.Text = "2";
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
            {
                lblCodeTitel.Text = " رقم فاتورة البيع";
                lblInvStat.Text = "البيع المرتد يكون عن طريق رقم فاتورة البيع الاصلية";
            }

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

        #endregion

        private void lblTypeInvID_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(lblSave.Text))
            {
                CustomMessageBox.ShowInformation("تم حفظ الفاتورة من قبل ولا يمكن تعديلها.", "تنبيه");
                return;
            }

            //int actualRowCount = DGV.Rows.Cast<DataGridViewRow>()
            //                      .Count(r => !r.IsNewRow &&
            //                                  r.Cells["ProductCode"].Value != null &&
            //                                  r.Cells["ProductCode"].Value != DBNull.Value);

            //if (actualRowCount == 0)
            //{
            //    CustomMessageBox.ShowInformation("لا توجد بيانات لحفظ الفاتورة.", "تنبيه");
            //    return;
            //}

            if (!int.TryParse(lblInv_ID.Text, out int invID))
            {
                CustomMessageBox.ShowInformation("رقم الفاتورة غير صالح.", "خطأ");
                return;
            }

            if (lblTypeInvID.Text == "1")
            {
                MessageBox.Show("سيتم الحفظ النهائي لفاتورة البيع.", "تنبيه");
            }
            else if (lblTypeInvID.Text == "2")
            {
                MessageBox.Show("سيتم الحفظ النهائي لفاتورة البيع المرتد.", "تنبيه");
            }
        }

    }
}
