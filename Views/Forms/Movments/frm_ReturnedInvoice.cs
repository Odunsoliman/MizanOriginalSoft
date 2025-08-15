using MizanOriginalSoft.MainClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MizanOriginalSoft.Views.Forms.Movments
{
    public partial class frm_ReturnedInvoice : Form
    {
        public DataTable SelectedItems { get; private set; } = new DataTable(); // تهيئة لتجنب التحذير

        private readonly DataTable _tblInvoice;
        private readonly DataTable _tblDetails;
        private readonly int _invTypeID;
        private readonly int _invCounter;
        private readonly int _currentInvoiceID;

        public frm_ReturnedInvoice(int invTypeID, int invCounter, DataTable tblInvoice, DataTable tblDetails, int currentInv_ID)
        {
            InitializeComponent();

            _invTypeID = invTypeID;
            _invCounter = invCounter;
            _tblInvoice = tblInvoice;
            _tblDetails = tblDetails;
            _currentInvoiceID = currentInv_ID;
        }

        private void frm_ReturnedInvoice_Load(object sender, EventArgs e)
        {
            // تعيين عنوان النافذة حسب نوع الفاتورة
            this.Text = _invTypeID == 1
                ? $"فاتورة مبيعات مرتدة للفاتورة رقم {_invCounter}"
                : $"فاتورة مشتريات مرتدة للفاتورة رقم {_invCounter}";

            // تعبئة بيانات الفاتورة في اللابل
            if (_tblInvoice.Rows.Count > 0)
            {
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
            }

            // ربط التفاصيل بالجريد
            DGV.DataSource = _tblDetails;

            if (!DGV.Columns.Contains("SelectItem"))
            {
                DataGridViewCheckBoxColumn chk = new()
                {
                    HeaderText = "اختر",
                    Name = "SelectItem",
                    Width = 60
                };
                DGV.Columns.Insert(0, chk);
            }

            DGVStyl(); // تنسيق الجريد
        }

        private void DGVStyl()
        {
            if (DGV.DataSource == null) return;

            DGV.RightToLeft = RightToLeft.Yes;

            // إعادة ترتيب وتنسيق الأعمدة
            string[] visibleColumnsOrder = { "SelectItem", "ProductCode", "ProdName", "PieceSelector", "UnitProd", "PriceMove", "Amount", "TotalRow", "GemDisVal", "NetRow" };

            Dictionary<string, string> headers = new()
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

            // إضافة عمود القطعة إن لم يوجد
            if (!DGV.Columns.Contains("PieceSelector"))
            {
                DataGridViewComboBoxColumn pieceColumn = new()
                {
                    Name = "PieceSelector",
                    HeaderText = "القطعة",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
                    FlatStyle = FlatStyle.Flat,
                    DisplayStyleForCurrentCellOnly = true
                };

                if (DGV.Columns.Contains("ProdName"))
                    DGV.Columns.Insert(DGV.Columns["ProdName"].Index + 1, pieceColumn);
                else
                    DGV.Columns.Add(pieceColumn);
            }

            // إخفاء كل الأعمدة مبدئيًا
            foreach (DataGridViewColumn col in DGV.Columns)
                col.Visible = false;

            // إظهار وتنسيق الأعمدة المحددة
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

                    if (colName is "TotalRow" or "NetRow")
                    {
                        col.DefaultCellStyle.Format = "N2";
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    }
                }
            }

            // التنسيق العام للجريد
            DGV.DefaultCellStyle.Font = new Font("Times New Roman", 14);
            DGV.DefaultCellStyle.ForeColor = Color.Blue;
            DGV.DefaultCellStyle.BackColor = Color.LightYellow;

            DGV.ColumnHeadersDefaultCellStyle.Font = new Font("Times New Roman", 12, FontStyle.Bold);
            DGV.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            DGV.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            DGV.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DGV.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            DGV.RowHeadersVisible = false;

            DGV.DefaultCellStyle.SelectionBackColor = Color.SteelBlue;
            DGV.DefaultCellStyle.SelectionForeColor = Color.White;

            DGV.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DGV.GridColor = Color.Black;

            // الترتيب حسب serInvDetail
            if (DGV.Columns.Contains("serInvDetail"))
                DGV.Sort(DGV.Columns["serInvDetail"], ListSortDirection.Ascending);
        }

        private void btnConfirmSelection_Click(object sender, EventArgs e)
        {
            if (DGV.DataSource is not DataTable dtOriginal)
            {
                MessageBox.Show("مصدر البيانات غير معروف.");
                return;
            }

            int insertedCount = 0;

            foreach (DataGridViewRow row in DGV.Rows)
            {
                if (!row.IsNewRow && Convert.ToBoolean(row.Cells["SelectItem"].Value) == true)
                {
                    // قراءة القيم من الصف
                    int pieceID = Convert.ToInt32(row.Cells["PieceID_fk"].Value);
                    float priceMove = Convert.ToSingle(row.Cells["PriceMove"].Value);
                    float amount = Convert.ToSingle(row.Cells["Amount"].Value);
                    float totalRow = Convert.ToSingle(row.Cells["TotalRow"].Value);
                    float discount = Convert.ToSingle(row.Cells["GemDisVal"].Value);
                    float comition = Convert.ToSingle(row.Cells["ComitionVal"].Value);
                    float netRow = Convert.ToSingle(row.Cells["NetRow"].Value);
                    int invID = Convert.ToInt32(row.Cells["Inv_ID_fk"].Value);

                    int returnType = _invTypeID == 1 ? 2 : (_invTypeID == 3 ? 4 : _invTypeID);

                    InvoiceDetails_Insert(returnType, _currentInvoiceID, pieceID, priceMove, amount, totalRow, discount, comition, netRow, invID);

                    insertedCount++;
                }
            }

            if (insertedCount == 0)
            {
                MessageBox.Show("الرجاء اختيار صنف واحد على الأقل.");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        // تنفيذ الإدراج لصف الفاتورة المرتدة
        public string InvoiceDetails_Insert(int typeID, int invoiceID, int pieceID, float priceMove, float amount,
                                            float totalRow, float discount, float comition, float netRow, int originalInvID)
        {
            return DBServiecs.InvoiceDetails_Insert(typeID, invoiceID, pieceID, priceMove, amount,
                                                    totalRow, discount, comition, netRow, originalInvID);
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSelectRows_Click(object sender, EventArgs e)
        {
            // يمكنك هنا لاحقًا برمجة تحديد الصفوف جميعها تلقائيًا مثلاً
        }
    }
}
