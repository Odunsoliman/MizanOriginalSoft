using MizanOriginalSoft.MainClasses;
using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.MainForms
{
    public partial class frmDailyDashBoard : Form
    {


        public frmDailyDashBoard()
        {
            InitializeComponent();

            dtpTargetDate.Format = DateTimePickerFormat.Custom;
            dtpTargetDate.CustomFormat = "dddd dd MMMM yyyy";
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ar-EG");

        }
        private void frmDailyDashBoard_Load(object sender, EventArgs e)
        {
            SetDashboardCardColors();  // ضبط الالوان
            GetData();//احضار البيانات
        }

        //دالة ضبط الالوان
        private void SetDashboardCardColors()
        {
            // 🟩 المبيعات النقدية
            panelSalesCash.BackColor = ColorTranslator.FromHtml("#C8E6C9");

            // 🟩 المبيعات بفيزا
            panelElectronicSales.BackColor = ColorTranslator.FromHtml("#FFE0B2");

            // 🟦 المبيعات الآجلة
            panelSalesCredit.BackColor = ColorTranslator.FromHtml("#BBDEFB");

            // 🟧 المصروفات
            panelExpenses.BackColor = ColorTranslator.FromHtml("#FFE0B2");

            // 🟪 المشتريات
            panelPurchases.BackColor = ColorTranslator.FromHtml("#E1BEE7");

            // 🟨 إجمالي النقدية الحالي
            panelTotalCash.BackColor = ColorTranslator.FromHtml("#FFF9C4");

            // 🟥 المستحقات للتسديد
            panelToPay.BackColor = ColorTranslator.FromHtml("#FFCDD2");

            // 🟦 المستحقات للتحصيل
            panelToCollect.BackColor = ColorTranslator.FromHtml("#E3F2FD");

            // 🟩 أكبر صنف مبيعًا
            panelTopProduct.BackColor = ColorTranslator.FromHtml("#B2DFDB");

            // 🟫 أصناف راكدة
            panelSlowProducts.BackColor = ColorTranslator.FromHtml("#EEEEEE");

            // 🟠 نواقص الأصناف
            panelMissingStock.BackColor = ColorTranslator.FromHtml("#FFF3E0");

            // ⚫ تنسيق النص (اختياري)
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Panel pnl)
                {
                    pnl.ForeColor = Color.FromArgb(25, 25, 112); // أزرق غامق أنيق
                }
            }

        }

        DataTable dt = new DataTable();
        private void GetData()
        {
            dt=DBServiecs.TodayRpt_SalesSummary(dtpTargetDate.Value );
            lblSalesCash.Text = dt.ToString();//Cash_Sales
            lblElectronicSales.Text = dt.ToString();//Electronic_Sales
            lblSalesCredit .Text = dt.ToString();//Credit_Sales
            lblTotalCash.Text = dt.ToString();//Total_Sales

        }


    }
}


/*
 
 
        #region 🖱️ Mouse Effects

        #endregion
 
 */