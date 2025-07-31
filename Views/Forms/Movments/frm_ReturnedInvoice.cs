using MizanOriginalSoft.MainClasses;
using Signee.DB;
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
    public partial class frm_ReturnedInvoice : Form
    {
        /*
         اريد هنا استقبال DataTable tblInvoice 
        واستخراج وعرض الحقول الاتية
            lblInv_Counter;
            lblInv_ID;

            lblAccID;
            lblAccName;
            lblFirstPhon;
            lblAntherPhon;
            
            lblUserID;
            lblSellerID;
            lblWarehouseName;

            lblCount;
            lblDiscount;
            lblDiscountRate;
            lblValueAdded;
            lblAdditionalRate;
            lblTaxVal;
            lblTaxRate;

            lblNetTotal;
            lblNoteInvoice;
            lblPayment_Cash;
            lblPayment_Electronic;
            lblPayment_Note;
            lblRemainingOnAcc;


    
            lblTotalInv;
            
            lblTotalValueAfterTax;
          
        هذه اسماء الادوات والحقول نفس الاسماء بعد ازالة lbl
            



        وتسمية عنوان الشاشة ب 
        InvType_ID=1 فاتورة مبيعات مرتدة للفاتورة رقم Inv_Counter
        وفى حالة 
        InvType_ID=3 فاتورة مشتريات مرتدة للفاتورة رقم Inv_Counter

        ثم تعبئة DGV بالدالة
            DataTable tblInvDetails = DBServiecs.NewInvoice_GetInvoiceDetails(Inv_ID);
            DGV.DataSource = tblInvDetails;
         */

        public DataTable SelectedItems { get; private set; }
        private DataTable _tblInvoice;
        private DataTable _tblDetails;
        private int _invTypeID;
        private int _invCounter;
        private int _currentInvoiceID;

        public frm_ReturnedInvoice(int invTypeID, int invCounter, DataTable tblInvoice, DataTable tblDetails,int curentInv_ID)
        {
            InitializeComponent();

            _invTypeID = invTypeID;
            _invCounter = invCounter;
            _tblInvoice = tblInvoice;
            _tblDetails = tblDetails;
            _currentInvoiceID = curentInv_ID;
        }


        private void frm_ReturnedInvoice_Load(object sender, EventArgs e)
        {
            // ضبط العنوان
            this.Text = _invTypeID == 1
                ? $"فاتورة مبيعات مرتدة للفاتورة رقم {_invCounter}"
                : $"فاتورة مشتريات مرتدة للفاتورة رقم {_invCounter}";

            // تعبئة الحقول العامة
            DataRow row = _tblInvoice.Rows[0];

            lblInv_Counter.Text = row["Inv_Counter"].ToString();
            lblInv_ID.Text = row["Inv_ID"].ToString();

            lblAccID.Text = row["Acc_ID"].ToString();
            lblAccName.Text = row["AccName"].ToString();
            lblFirstPhon.Text = row["FirstPhon"].ToString();
            lblAntherPhon.Text = row["AntherPhon"].ToString();
            lblUserID.Text = row["User_ID"].ToString();
            lblSellerID.Text = row["Seller_ID"].ToString();
            lblWarehouseName.Text = row["WarehouseName"].ToString();

            lblDiscount.Text = row["Discount"].ToString();
            lblValueAdded.Text = row["ValueAdded"].ToString();
            lblTaxVal.Text = row["TaxVal"].ToString();

            lblNetTotal.Text = row["NetTotal"].ToString();
            lblNoteInvoice.Text = row["NoteInvoice"].ToString();
            lblPayment_Cash.Text = row["Payment_Cash"].ToString();
            lblPayment_Electronic.Text = row["Payment_Electronic"].ToString();
            lblPayment_Note.Text = row["Payment_Note"].ToString();
            lblRemainingOnAcc.Text = row["RemainingOnAcc"].ToString();

            lblTotalInv.Text = row["TotalValue"].ToString();
            lblTotalValueAfterTax.Text = row["TotalValueAfterTax"].ToString();

            // تعبئة الجدول
            DGV.DataSource = _tblDetails;
            if (!DGV.Columns.Contains("SelectItem"))
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                chk.HeaderText = "اختر";
                chk.Name = "SelectItem";
                chk.Width = 60;
                DGV.Columns.Insert(0, chk);
            }

            // تنسيق الجدول
            DGVStyl();
        }

        private void DGVStyl()
        {
            if (DGV.DataSource == null) return;

            // ترتيب الأعمدة من اليمين لليسار
            DGV.RightToLeft = RightToLeft.Yes;

            // إضافة عمود تحديد إذا لم يكن موجودًا
            if (!DGV.Columns.Contains("SelectItem"))
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                chk.Name = "SelectItem";
                chk.HeaderText = "اختيار";
                chk.Width = 60;
                chk.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                chk.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                chk.ReadOnly = false;
                DGV.Columns.Insert(0, chk);
            }

            // إخفاء جميع الأعمدة مؤقتًا
            foreach (DataGridViewColumn col in DGV.Columns)
            {
                col.Visible = false;
            }

            // الأعمدة المراد عرضها وترتيبها
            string[] visibleColumnsOrder = { "SelectItem", "ProductCode", "ProdName", "PieceSelector", "UnitProd", "PriceMove", "Amount", "TotalRow", "GemDisVal", "NetRow" };

            Dictionary<string, string> headers = new Dictionary<string, string>
    {
        { "SelectItem", "اختيار" },
        { "ProductCode", "الكود" },
        { "ProdName", "اسم الصنف" },
        { "PieceSelector", "القطعة" },
        { "UnitProd", "الوحدة" },
        { "PriceMove", "السعر" },
        { "Amount", "الكمية" },
        { "TotalRow", "الإجمالي" },
        { "GemDisVal", "الخصم" },
        { "NetRow", "الصافي" }
    };

            // إضافة عمود القطع (ComboBox) إذا لم يكن موجودًا
            if (!DGV.Columns.Contains("PieceSelector"))
            {
                DataGridViewComboBoxColumn pieceColumn = new DataGridViewComboBoxColumn
                {
                    Name = "PieceSelector",
                    HeaderText = "القطعة",
                    DataPropertyName = "", // غير مرتبط ببيانات
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
                    FlatStyle = FlatStyle.Flat,
                    DisplayStyleForCurrentCellOnly = true
                };

                // إضافته بعد عمود ProdName إن وُجد
                if (DGV.Columns.Contains("ProdName"))
                    DGV.Columns.Insert(DGV.Columns["ProdName"].Index + 1, pieceColumn);
                else
                    DGV.Columns.Add(pieceColumn);
            }

            // إظهار وتنسيق الأعمدة المطلوبة
            foreach (string colName in visibleColumnsOrder)
            {
                if (DGV.Columns.Contains(colName))
                {
                    var col = DGV.Columns[colName];
                    col.Visible = true;
                    col.HeaderText = headers[colName];
                    col.FillWeight = colName == "ProdName" ? 3 : 1;
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }

            // تنسيق خاص لبعض الأعمدة الرقمية
            if (DGV.Columns.Contains("TotalRow"))
            {
                var col = DGV.Columns["TotalRow"];
                col.DefaultCellStyle.Format = "N2";
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (DGV.Columns.Contains("NetRow"))
            {
                var col = DGV.Columns["NetRow"];
                col.DefaultCellStyle.Format = "N2";
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            // تنسيق عام للجدول
            DGV.DefaultCellStyle.Font = new Font("Times New Roman", 14);
            DGV.DefaultCellStyle.ForeColor = Color.Blue;
            DGV.DefaultCellStyle.BackColor = Color.LightYellow;

            // تنسيق رؤوس الأعمدة
            DGV.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Bold);
            DGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DGV.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            DGV.RowHeadersVisible = false;

            // تنسيق التحديد
            DGV.DefaultCellStyle.SelectionBackColor = Color.SteelBlue;
            DGV.DefaultCellStyle.SelectionForeColor = Color.White;

            // الترتيب إن وُجد عمود serInvDetail
            if (DGV.Columns.Contains("serInvDetail"))
            {
                DGV.Sort(DGV.Columns["serInvDetail"], ListSortDirection.Ascending);
            }

            // عرض الأعمدة تلقائيًا
            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // لون خطوط الشبكة
            DGV.GridColor = Color.Black;
        }



        private void btnConfirmSelection_Click(object sender, EventArgs e)
        {
            DataTable dtOriginal = DGV.DataSource as DataTable;
            if (dtOriginal == null)
            {
                MessageBox.Show("مصدر البيانات غير معروف.");
                return;
            }

            int insertedCount = 0;

            foreach (DataGridViewRow row in DGV.Rows)
            {
                if (!row.IsNewRow && Convert.ToBoolean(row.Cells["SelectItem"].Value) == true)
                {
                    // استخراج القيم من الصف
                 int   PieceID_fk = Convert.ToInt32(row.Cells["PieceID_fk"].Value);
                 float    PriceMove = Convert.ToSingle(row.Cells["PriceMove"].Value);
                 float     Amount = Convert.ToSingle(row.Cells["Amount"].Value);
                 float     TotalRow = Convert.ToSingle(row.Cells["TotalRow"].Value);
                 float   GemDisVal = Convert.ToSingle(row.Cells["GemDisVal"].Value);
                 float    ComitionVal = Convert.ToSingle(row.Cells["ComitionVal"].Value);
                 float    NetRow = Convert.ToSingle(row.Cells["NetRow"].Value);
                 int      Inv_ID = Convert.ToInt32(row.Cells["Inv_ID_fk"].Value);
                    if (_invTypeID == 1)
                        InvoiceDetails_Insert(2, _currentInvoiceID, PieceID_fk, PriceMove, Amount, TotalRow, GemDisVal, ComitionVal, NetRow, Inv_ID);
                    else if (_invTypeID == 3)
                        InvoiceDetails_Insert(4, _currentInvoiceID, PieceID_fk, PriceMove, Amount, TotalRow, GemDisVal, ComitionVal, NetRow, Inv_ID);
                    insertedCount++;
                }
            }

            if (insertedCount == 0)
            {
                MessageBox.Show("الرجاء اختيار صنف واحد على الأقل.");
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        public string InvoiceDetails_Insert(int Type_ID,int _currentInvoiceID,int PieceID_fk,float  PriceMove,float  Amount,
           float  TotalRow,float  GemDisVal,float  ComitionVal,float  NetRow,int Inv_ID)
        {

            string message = DBServiecs.InvoiceDetails_Insert(
                Type_ID, _currentInvoiceID, PieceID_fk, PriceMove, Amount,
                TotalRow, GemDisVal, ComitionVal, NetRow, Inv_ID);
            return message;
        }
        private void btnSelectRows_Click(object sender, EventArgs e)
        {

        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /*
         اريد فى frm_ReturnedInvoice
       وسيلة لاختيار احد او بعض الاصناف من  الفاتورة المعروضة  واخذ هذه القطع المرتجعة وعرضعا فى الشاشة الرئيسية
        ما هو السيناريو الجيد لهذه الوظيفة
         */


    }
}
