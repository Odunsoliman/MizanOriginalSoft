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
            dt = DBServiecs.TodayRpt_SalesAndExpenses(dtpTargetDate.Value);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                lblSalesCash.Text = Convert.ToDecimal(row["Cash_Sales"]).ToString("N2");
                lblElectronicSales.Text = Convert.ToDecimal(row["Electronic_Sales"]).ToString("N2");
                lblSalesCredit.Text = Convert.ToDecimal(row["Credit_Sales"]).ToString("N2");
                lblTotalCash.Text = Convert.ToDecimal(row["Total_Sales"]).ToString("N2");
                lblExpenses.Text = Convert.ToDecimal(row["Total_Cash_Expenses"]).ToString("N2");
                lblPurchases.Text = Convert.ToDecimal(row["Total_Purchases"]).ToString("N2");
                lblUnSaved_Invoice.Text = Convert.ToInt32 (row["UnSaved_Invoice_All"]).ToString("N2");
                //System.ArgumentException: 'Column 'Total_Cash_Expenses' does not belong to table .'
                //System.ArgumentException: 'Column 'Total_Purchases' does not belong to table .'
                //System.ArgumentException: 'Column 'UnSaved_Invoice_All' does not belong to table .'
            }
            else
            {
                lblSalesCash.Text = "0.00";
                lblElectronicSales.Text = "0.00";
                lblSalesCredit.Text = "0.00";
                lblTotalCash.Text = "0.00";
            }
        }


        private void dtpTargetDate_ValueChanged(object sender, EventArgs e)
        {
            GetData();
        }
    }
}


/*
 
 
        #region 🖱️ Mouse Effects

        #endregion
 
 */