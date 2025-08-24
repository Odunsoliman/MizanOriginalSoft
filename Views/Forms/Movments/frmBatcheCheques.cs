#region @@@@@@@ Using @@@@@
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static ZXing.QrCode.Internal.Mode;
using TextBox = System.Windows.Forms.TextBox;
using ComboBox = System.Windows.Forms.ComboBox;
using System.Globalization;
using MizanOriginalSoft.MainClasses.OriginalClasses;
using MizanOriginalSoft.MainClasses;
using MizanOriginalSoft.Views.Forms.MainForms;
#endregion 

namespace MizanOriginalSoft.Views.Forms.Movments

{
    public partial class frmBatcheCheques : Form
    {

        #region Enum
        public enum TransactionType
        {
            BatchIn = 11,   // تحصيل
            BatchOut = 12,  // صرف
        }
        #endregion

        // تعديل الباني ليأخذ TransactionType مباشرة
        public frmBatcheCheques(TransactionType transactionType)
        {
            InitializeComponent();

            Type_ID = (int)transactionType;  // حفظ القيمة الرقمية إذا لزم الأمر
            createdByUsID = CurrentSession.UserID;

            switch (transactionType)
            {
                case TransactionType.BatchIn:
                    Text = "حافظة شيكات واردة";
                    lblbatchTitel.Text = "حافظة شيكات واردة رقم:";
                    break;

                case TransactionType.BatchOut:
                    Text = "حافظة شيكات صادرة";
                    lblbatchTitel.Text = "حافظة شيكات صادرة رقم:";
                    break;

                default:
                    lblbatchTitel.Text = "عملية غير معروفة";
                    break;
            }

            lblUS.Text = createdByUsID.ToString();
            ConnectEvents();
        }

        private void frmBatcheCheques_Load(object sender, EventArgs e)
        {
            DBServiecs.A_UpdateAllDataBase();
            GetBatchs();
            AttachAutoSaveEvents();

            // Placeholder نص افتراضي
            PlaceholderHelper.SetPlaceholder(txtAccName, "اضغط هنا Ctrl+F لاختيار الحساب");
            DGV.ClearSelection();
        }

        #region Constructor & Form Load
        private void ConnectEvents()
        {
            txtAccName.KeyDown += txtAccName_KeyDown;

            btnFrist.Click += btnFirst_Click;
            btnPrent.Click += btnPrent_Click;
            btnNew.Click += btnNew_Click;
            btnLast.Click += btnLast_Click;
            btnNext.Click += btnNext_Click;
            btnPrevious.Click += btnPrevious_Click;

            DGV.ContextMenuStrip = cmsChequeStatus;
        }
        #endregion

        #region Batch Loading & Display
        private void GetBatchs(int? focusBatchID = null)
        {
            tblBatchs = DBServiecs.ChequeBatches_Search("MovTypeID", Type_ID);
            tblBatchs.DefaultView.Sort = "BatchID ASC";
            tblBatchs = tblBatchs.DefaultView.ToTable();

            if (focusBatchID.HasValue)
            {
                for (int i = 0; i < tblBatchs.Rows.Count; i++)
                {
                    if (Convert.ToInt32(tblBatchs.Rows[i]["BatchID"]) == focusBatchID.Value)
                    {
                        ShowBatch(i);
                        return;
                    }
                }
            }

            // افتراضيًا انتقل إلى سند جديد
            ShowBatch(tblBatchs.Rows.Count);
        }

        private void ShowBatch(int index)
        {
            isNavigating = true;
            currentIndex = index;

            if (tblBatchs == null || index == tblBatchs.Rows.Count)
            {
                ClearBillFields();
                DGVStyl();
                isNavigating = false;
                return;
            }

            DataRow row = tblBatchs.Rows[index];

            Batch_ID = Convert.ToInt32(row["BatchID"]);
            ReUpdateRowFields(Batch_ID);

            AttachAutoSaveEvents();
            ReloadCheques();
            ApplyChequeColors();
            DGVStyl();

            if (!string.IsNullOrWhiteSpace(lblSaveStatus.Text))
                CalculateTotalAmountFromGridOnly();

            isNavigating = false;
            lblCountCheques.Text = "عدد الشيكات :  " + DGV.RowCount.ToString();
        }

        private void ClearBillFields()
        {
            int newID = DBServiecs.ChequeBatches_GetNextBatchID();
            Batch_ID = newID;
            lblBatchID.Text = newID.ToString();

            string newVoucher = DBServiecs.ChequeBatches_GetNewBatchCode(Type_ID);
            lblBatchCode.Text = newVoucher;

            dtpBatchDate.Value = DateTime.Now;
            lblAccID.Text = string.Empty;
            lblMovTypeID.Text = Type_ID.ToString();
            txtNoteBatch.Text = string.Empty;
            lblSaveStatus.Text = string.Empty;
            lblTotalBatch.Text = "0.00";
            txtAccName.Text = string.Empty;
            txtAccName.Enabled = true;

            PlaceholderHelper.SetPlaceholder(txtAccName, "اضغط هنا Ctrl+F لاختيار الحساب");

            ReloadCheques();
        }

        private void ReUpdateRowFields(int batchID)
        {
            tblBatch = DBServiecs.ChequeBatches_Search("BatchID", batchID);

            DataRow[] rows = tblBatch.Select($"BatchID = {batchID}");
            if (rows.Length == 0)
            {
                MessageBox.Show("لم يتم العثور على الحركة المطلوبة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRow row = rows[0];

            lblBatchID.Text = row["BatchID"].ToString();
            dtpBatchDate.Value = Convert.ToDateTime(row["BatchDate"]);
            lblAccID.Text = row["AccID"].ToString();
            lblMovTypeID.Text = row["MovTypeID"].ToString();
            txtNoteBatch.Text = row["NoteBatch"].ToString();
            lblSaveStatus.Text = row["SaveStatus"].ToString();
            lblBatchCode.Text = row["BatchCode"].ToString();
            lblTotalBatch.Text = row["TotalBatch"].ToString();

            txtAccName.Text = row["AccName"].ToString();
            lblBalance.Text = row["Balance"].ToString();
            lblB_Status.Text = row["BalanceState"].ToString();
            lblFirstPhon.Text = row["FirstPhon"].ToString();
            lblAntherPhon.Text = row["AntherPhon"].ToString();
            lblClientAddress.Text = row["ClientAddress"].ToString();
            lblClientEmail.Text = row["ClientEmail"].ToString();
        }

        private void ReloadCheques()
        {
            tblChequ = DBServiecs.Cheques_GetByBatchID(Batch_ID) ?? new DataTable();

            DataTable sorted = (tblChequ.Rows.Count > 0)
                ? tblChequ.AsEnumerable()
                    .OrderBy(r => r.Field<int>("StatusCode"))
                    .ThenBy(r => r.IsNull("DueDate") ? DateTime.MaxValue : r.Field<DateTime>("DueDate"))
                    .CopyToDataTable()
                : tblChequ.Clone();

            DGV.DataSource = sorted;
            DGV.ClearSelection();
        }

        private void CalculateTotalAmountFromGridOnly()
        {
            float sum = 0;
            foreach (DataGridViewRow row in DGV.Rows)
            {
                if (!row.IsNewRow && float.TryParse(row.Cells["Amount"].Value?.ToString(), out float val))
                    sum += val;
            }

            lblTotalBatch.Text = sum.ToString("N2");
            Save();
        }
        #endregion

        #region Navigation
        private void NavigateTo(int targetIndex)
        {
            if (tblBatchs == null || tblBatchs.Rows.Count == 0) return;

            if (targetIndex >= 0 && targetIndex <= tblBatchs.Rows.Count)
                ShowBatch(targetIndex);

            DGV.ClearSelection();
        }

        private void btnFirst_Click(object? sender, EventArgs e) => NavigateTo(0);

        private void btnPrevious_Click(object? sender, EventArgs e) => NavigateTo(currentIndex - 1);

        private void btnNext_Click(object? sender, EventArgs e)
        {
            if (tblBatchs == null || tblBatchs.Rows.Count == 0) return;

            if (currentIndex < tblBatchs.Rows.Count - 1)
                NavigateTo(currentIndex + 1);
        }

        private void btnLast_Click(object? sender, EventArgs e)
        {
            int lastIndex = (tblBatchs?.Rows.Count ?? 0) - 1;
            if (lastIndex >= 0)
                NavigateTo(lastIndex);
        }

        private void btnNew_Click(object? sender, EventArgs e)
        {
            int newIndex = tblBatchs?.Rows.Count ?? 0;
            NavigateTo(newIndex);
        }
        #endregion













        #region @@@ Load Data & Print
        //اختيار الحساب و وضع بيانات الحساب فى اماكنها
        private void txtAccName_KeyDown(object? sender, KeyEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(lblSaveStatus.Text))
            {
                MessageBox.Show("الحافظة تم حفظها نهائيًا، لا يمكن التعديل.");
                return;
            }

            Batch_ID = Convert.ToInt32(lblBatchID.Text);

            if (e.Control && e.KeyCode == Keys.F)
            {
                frmSearch.SearchEntityType entityType;

                if (Type_ID == (int)TransactionType.BatchIn)   // 11
                    entityType = frmSearch.SearchEntityType.Customer;
                else if (Type_ID == (int)TransactionType.BatchOut) // 12
                    entityType = frmSearch.SearchEntityType.Supplier;
                else
                    entityType = frmSearch.SearchEntityType.AllAccounts;

                frmSearch searchForm = new frmSearch(Type_ID, entityType);

                if (searchForm.ShowDialog() == DialogResult.OK)
                {
                    // تعبئة بيانات الحساب المختار
                    txtAccName.Text = searchForm.SelectedName;
                    lblAccID.Text = searchForm.SelectedID;
                    lblAntherPhon.Text = searchForm.SelectedAntherPhon;
                    lblFirstPhon.Text = searchForm.SelectedFirstPhon;
                    lblClientAddress.Text = searchForm.SelectedClientAddress;
                    lblBalance.Text = searchForm.SelectedBalance.ToString();
                    lblB_Status.Text = searchForm.SelectedBalanceState;
                    lblClientEmail.Text = searchForm.SelectedClientEmail;
                }

                isFinalSave = false; isNavigating = false;
                Save();

                if (string.IsNullOrEmpty(searchForm.SelectedID) || searchForm.SelectedID == "0")
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    MessageBox.Show("لم يتم اختيار حساب صالح، يرجى المحاولة مرة أخرى", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    txtNoteBatch.Focus();
                }

                AttachAutoSaveEvents();
                e.SuppressKeyPress = true;
                return;
            }
        }

    
        private void btnPrent_Click(object? sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(lblSaveStatus.Text))
            {
                MessageBox.Show("جاري إعداد التقرير...", "طباعة", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("يجب حفظ السند أولاً قبل الطباعة.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion
     
        
        
        
        
        #region @@@@ DGV Apply Cheque Colors @@@



        private void DGVStyl()
        {
            // إعداد الأعمدة مع تنسيقات مخصصة
            var columnHeaders = new Dictionary<string, string>
    {
        { "ChequeNumber", "رقم الشيك" },
        { "DueDate", "تاريخ الاستحقاق" },
        { "Amount", "القيمة" },
        { "BankName", "اسم البنك" },
        { "Branch", "الفرع" },
        { "Notes", "ملاحظات" },
        { "Status", "الحالة" },
        { "StatusDate", "تاريخ الحالة" },
        { "RejectReason", "سبب الرفض" }
    };

            DGV.AutoGenerateColumns = false;
            DGV.Columns.Clear();

            foreach (var kvp in columnHeaders)
            {
                var column = new DataGridViewTextBoxColumn
                {
                    Name = kvp.Key,
                    HeaderText = kvp.Value,
                    DataPropertyName = kvp.Key,
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };

                // تنسيق التاريخ
                if (kvp.Key == "DueDate" || kvp.Key == "StatusDate")
                {
                    column.DefaultCellStyle.Format = "dd/MM/yyyy";
                }

                // تنسيق القيم
                if (kvp.Key == "Amount")
                {
                    column.DefaultCellStyle.Format = "N0";
                }

                DGV.Columns.Add(column);
            }

            // الأعمدة المخفية
            string[] hiddenCols = { "ChequeID", "Batch_ID", "StatusCode" };
            foreach (var colName in hiddenCols)
            {
                DGV.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = colName,
                    DataPropertyName = colName,
                    Visible = false
                });
            }

            ApplyGeneralGridStyle();
            ApplyChequeColors();
        }

        private void ApplyGeneralGridStyle()
        {
            DGV.ReadOnly = true;
            DGV.AllowUserToAddRows = false;
            DGV.AllowUserToDeleteRows = false;
            DGV.AllowUserToOrderColumns = false;
            DGV.RowHeadersVisible = false;
            DGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGV.MultiSelect = false;
            DGV.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            DGV.GridColor = Color.Black;

            DGV.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Times New Roman", 14),
                ForeColor = Color.Blue,
                BackColor = Color.LightYellow,
                SelectionBackColor = Color.SteelBlue,
                SelectionForeColor = Color.White
            };

            DGV.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Font = new Font("Times New Roman", 14, FontStyle.Bold),
                ForeColor = Color.Blue,
                BackColor = Color.LightGray,
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };
        }

        private void ApplyChequeColors()
        {
            for (int i = 0; i < DGV.Rows.Count; i++)
            {
                var row = DGV.Rows[i];
                if (row.IsNewRow) continue;

                int statusCode = Convert.ToInt32(row.Cells["StatusCode"].Value);

                // تلوين الأعمدة الثابتة
                string[] staticCols = { "Status", "StatusDate", "RejectReason" };
                foreach (var col in staticCols)
                    row.Cells[col].Style.ForeColor = Color.Black;

                // ألوان الحالة
                switch (statusCode)
                {
                    case 0: // مرفوض
                        row.DefaultCellStyle.BackColor = Color.MistyRose;
                        row.DefaultCellStyle.ForeColor = Color.DarkRed;
                        break;
                    case 1: // صالح
                        row.DefaultCellStyle.BackColor = (i % 2 == 0) ? Color.Honeydew : Color.White;
                        row.DefaultCellStyle.ForeColor = Color.DarkSlateGray;
                        break;
                    case 2: // مصروف
                        row.DefaultCellStyle.BackColor = Color.LightCyan;
                        row.DefaultCellStyle.ForeColor = Color.DarkGreen;
                        break;
                    case 3: // أرشيف
                        row.DefaultCellStyle.BackColor = Color.LightGray;
                        row.DefaultCellStyle.ForeColor = Color.DimGray;
                        break;
                }
            }
        }

        #endregion

        #region @@@ Auto Save & Validation @@@

        private string originalValue = "";
        private bool isSaved;
        private readonly HashSet<string> excludedControls = new HashSet<string>
{
    "txtChequeNumber",
    "txtDueDate",
    "txtNotes",
    "txtBankName",
    "txtBranch",
    "txtAmount"
};

        private void AttachAutoSaveEvents()
        {
            isSaved = !string.IsNullOrWhiteSpace(lblSaveStatus.Text);
            bool hasAccount = int.TryParse(lblAccID.Text, out int accId) && accId > 200;

            if (isSaved)
            {
                // تعطيل جميع الأدوات، وتمكين قائمة السياق فقط
                txtAccName.Enabled = false;
                dtpBatchDate.Enabled = false;
                btnNewCheque.Enabled = false;
                cmsChequeStatus.Enabled = true;

                foreach (Control ctrl in GetAllControls(this))
                {
                    if (ctrl is TextBox || ctrl is ComboBox || ctrl is DateTimePicker)
                        ctrl.Enabled = false;
                }
                return;
            }

            // حالة عدم الحفظ
            txtAccName.Enabled = true;
            dtpBatchDate.Enabled = hasAccount;
            btnNewCheque.Enabled = hasAccount;
            cmsChequeStatus.Enabled = false;

            foreach (Control ctrl in GetAllControls(this))
            {
                if (ctrl is TextBox txt && txt != txtAccName)
                {
                    txt.Enter -= ValidateAccountBeforeEdit;
                    txt.Leave -= Control_Leave;
                    txt.Enter += ValidateAccountBeforeEdit;
                    txt.Leave += Control_Leave;
                    txt.Enabled = hasAccount;
                }
                else if (ctrl is ComboBox cbx)
                {
                    cbx.Enter -= ValidateAccountBeforeEdit;
                    cbx.Leave -= Control_Leave;
                    cbx.Enter += ValidateAccountBeforeEdit;
                    cbx.Leave += Control_Leave;
                    cbx.Enabled = hasAccount;
                }
                else if (ctrl is DateTimePicker dtp && dtp != dtpBatchDate)
                {
                    dtp.Enabled = hasAccount;
                }
            }
        }

        // دالة لإرجاع كل الكنترولات داخل الحاويات
        private IEnumerable<Control> GetAllControls(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                yield return ctrl;
                foreach (var child in GetAllControls(ctrl))
                    yield return child;
            }
        }

        // التحقق عند الدخول إلى أي تكست بوكس أو كومبو بوكس
        private void ValidateAccountBeforeEdit(object? sender, EventArgs e)
        {
            if (sender is null) return;

            if (isSaved)
            {
                CustomMessageBox.ShowInformation("تم حفظ الحافظة، لا يمكن التعديل.", "تم الحفظ");
                ((Control)sender).Enabled = false;
                return;
            }

            bool hasAccount = int.TryParse(lblAccID.Text, out int accId) && accId > 200;
            if (!hasAccount)
            {
                CustomMessageBox.ShowWarning("يرجى اختيار حساب أولاً قبل إدخال البيانات.", "تنبيه");
                ((Control)sender).Enabled = false;
                return;
            }

           ((Control)sender).Enabled = true;
            Control_Enter(sender, e);
        }

        // تخزين القيمة الأصلية عند الدخول
        private void Control_Enter(object sender, EventArgs e)
        {
            isFinalSave = false;

            if (sender is Control ctrl && excludedControls.Contains(ctrl.Name))
                return;

            switch (sender)
            {
                case TextBox txt:
                    originalValue = txt.Text;
                    break;
                case ComboBox cbx:
                    originalValue = cbx.SelectedValue?.ToString() ?? "";
                    break;
            }
        }

        // التحقق من التعديل عند الخروج
        private void Control_Leave(object? sender, EventArgs e)
        {
            // منع الحفظ التلقائي إذا كانت الحافظة محفوظة رسميًا
            if (!string.IsNullOrWhiteSpace(lblSaveStatus.Text))
                return;

            if (sender is Control ctrl && excludedControls.Contains(ctrl.Name))
                return;

            string newValue = sender switch
            {
                TextBox txt => txt.Text,
                ComboBox cbx => cbx.SelectedValue?.ToString() ?? "",
                _ => ""
            };

            if (originalValue != newValue)
                Save();
        }

        // فتح قائمة السياق عند النقر بزر الماوس الأيمن على DGV
        private void DGV_MouseDown(object sender, MouseEventArgs e)
        {
            // نتأكد أن الزر الأيمن هو الذي ضغط
            if (e.Button != MouseButtons.Right)
                return;

            // نحدد موقع النقر في الصفوف والأعمدة
            var hit = DGV.HitTest(e.X, e.Y);
            if (hit.RowIndex < 0)
                return;

            // نحدد الصف الذي تم النقر عليه
            DGV.ClearSelection();
            DGV.Rows[hit.RowIndex].Selected = true;

            // ابحث عن أول خلية مرئية في الصف
            DataGridViewCell? visibleCell = DGV.Rows[hit.RowIndex].Cells
                .Cast<DataGridViewCell>()
                .FirstOrDefault(c => c.Visible);

            // إذا وجدت خلية مرئية، اجعلها CurrentCell وافتح قائمة السياق
            if (visibleCell != null)
            {
                DGV.CurrentCell = visibleCell;
                cmsChequeStatus.Show(DGV, e.Location);
            }
        }


        #endregion



        #region @@@@@@@@@@@@@@@@@@@ Save @@@@@@@@@@@@@@@@

        //private string resultMessage;
        //private bool isFinalSave = false;

        // زر الحفظ النهائي
        private void btnSave_Click(object sender, EventArgs e)
        {
            // منع الحفظ التلقائي إذا كانت الحافظة محفوظة رسميًا
            if (!string.IsNullOrWhiteSpace(lblSaveStatus.Text))
            {
                CustomMessageBox.ShowInformation("الحافظة تم حفظها نهائيًا، لا يمكن التعديل.", "تم الحفظ");
                return;
            }

            isFinalSave = true;

            ItemSave();

            if (Acc_ID <= 0)
            {
                CustomMessageBox.ShowWarning("يرجى اختيار حساب صالح.", "تنبيه");
                return;
            }

            if (TotalBatch <= 0)
            {
                CustomMessageBox.ShowWarning("يرجى إدخال شيكات بقيمة قبل الحفظ.", "تنبيه");
                return;
            }

            Save();
        }

        // تحميل القيم إلى المتغيرات
        private void ItemSave()
        {
            Batch_ID = int.TryParse(lblBatchID.Text, out int id) ? id : 0;
            Batch_Date = dtpBatchDate.Value;
            Acc_ID = int.TryParse(lblAccID.Text, out int acc) ? acc : 0;
            NoteBatch = txtNoteBatch.Text;
            TotalBatch = float.TryParse(lblTotalBatch.Text, out float total) ? total : 0;
            SaveStatus = isFinalSave ? "تم الحفظ" : lblSaveStatus.Text.Trim();
            BatchCode = lblBatchCode.Text;
        }

        #region Fields
        private int Batch_ID;
        private DateTime? Batch_Date;
        private int? Acc_ID;
        private string? NoteBatch;
        private float? TotalBatch;
        private string? SaveStatus;
        private string? BatchCode;
        private bool isFinalSave;
 //       private string? resultMessage;

        private int Type_ID;
        private int createdByUsID;
        private bool isNavigating = false;

        private int currentIndex = 0;
        private DataTable tblChequ = new DataTable();
        private DataTable? tblBatchs;
        private DataTable? tblBatch;
        #endregion

        // تنفيذ الحفظ
        private void Save()
        {
            // 🚫 تجاهل الحفظ التلقائي إذا كان تنقّلًا
            if (isNavigating)
                return;

            // منع الحفظ التلقائي إذا كانت الحافظة محفوظة رسميًا
            if (!isFinalSave && !string.IsNullOrWhiteSpace(lblSaveStatus.Text))
            {
                CustomMessageBox.ShowInformation("الحافظة تم حفظها نهائيًا، لا يمكن التعديل.", "تم الحفظ");
                return;
            }

            // حفظ العناصر إذا لم يكن حفظ نهائي
            if (!isFinalSave)
                ItemSave();

            // تمرير قيم افتراضية إذا كانت القيم null لتجنب التحذيرات
            string note = NoteBatch ?? string.Empty;
            string saveStatus = SaveStatus ?? string.Empty;
            string batchCode = BatchCode ?? string.Empty;

            bool isSaved = DBServiecs.ChequeBatches_InsertOrUpdate(
                Batch_ID,
                Batch_Date,
                Acc_ID,
                Type_ID,
                note,
                TotalBatch,
                saveStatus,
                batchCode,
                createdByUsID,
                out string? resultMessage
            );

            // التعامل مع الرسالة الناتجة بعد الحفظ النهائي
            if (isFinalSave && isSaved && !string.IsNullOrWhiteSpace(resultMessage))
            {
                lblSaveStatus.Text = resultMessage;
                CustomMessageBox.ShowInformation(resultMessage, "تم الحفظ");
                AttachAutoSaveEvents();
            }
        }

        #endregion

        #region @@@ تعريف أحداث النقر على عناصر السياق  @@@@@@@@@@@@@@@@@@@

        private void itemStatusRejected_Click(object sender, EventArgs e)
        {
            string reason;

            // استخدام نافذتك المخصصة
            if (CustomMessageBox.ShowStringInputBox(out reason, "يرجى إدخال سبب الرفض:", "سبب الرفض") == DialogResult.OK)
            {
                if (!string.IsNullOrWhiteSpace(reason))
                {
                    UpdateChequeStatus(0, "مرفوض", reason);
                }
            }
        }
        private void itemStatusPending_Click(object sender, EventArgs e)
        {
            if (DGV.CurrentRow == null || DGV.CurrentRow.IsNewRow) return;

            var row = DGV.CurrentRow;

            if (row.Cells["ChequeID"].Value == null || row.Cells["ChequeID"].Value == DBNull.Value)
                return;

            int chequeID = Convert.ToInt32(row.Cells["ChequeID"].Value);

            // ===== طلب تاريخ استحقاق جديد =====
            DateTime newDueDate = DateTime.Today.AddDays(1);
            using (var dateForm = new DateInputForm("أدخل تاريخ استحقاق جديد (يجب أن يكون مستقبليًا):"))
            {//The type or namespace name 'DateInputForm' could not be found (are you missing a using directive or an assembly reference?)
                if (dateForm.ShowDialog() == DialogResult.OK)
                {
                    newDueDate = dateForm.SelectedDate;
                    if (newDueDate <= DateTime.Today)
                    {
                        MessageBox.Show("يجب أن يكون التاريخ في المستقبل.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            // ===== طلب سبب التغيير =====
            string reason;
            if (CustomMessageBox.ShowStringInputBox(out reason, "يرجى إدخال سبب تغيير تاريخ الاستحقاق:", "سبب التغيير") != DialogResult.OK
                || string.IsNullOrWhiteSpace(reason))
            {
                MessageBox.Show("لم يتم إدخال سبب صالح.", "إلغاء", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // ===== تحديث شامل باستخدام الدالة الجديدة =====
            bool updated = DBServiecs.Cheques_UpdateDueDate(
                chequeID,
                "تحت التحصيل",
                newDueDate,
                reason,
                1,
                out string msg2
            );

            if (updated)
            {
                row.Cells["DueDate"].Value = newDueDate;
                row.Cells["Status"].Value = "تحت التحصيل";
                row.Cells["StatusDate"].Value = DateTime.Now;
                row.Cells["StatusCode"].Value = 1;
                row.Cells["RejectReason"].Value = reason;

                MessageBox.Show("تم تحديث حالة الشيك وتاريخ الاستحقاق بنجاح.", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("حدث خطأ أثناء التحديث:\n" + msg2, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void itemStatusCollected_Click(object sender, EventArgs e)
        {
            UpdateChequeStatus(2, "تم التحصيل");
        }

        private void itemStatusCancelled_Click(object sender, EventArgs e)
        {
            UpdateChequeStatus(3, "ملغي");
        }

        //تحديث حالة الشيك
        private void UpdateChequeStatus(int statusCode, string statusText, string? rejectReason = null)
        {
            if (DGV.CurrentRow == null || DGV.CurrentRow.IsNewRow) return;

            var row = DGV.CurrentRow;

            // التحقق من وجود قيمة صالحة في ChequeID
            if (row.Cells["ChequeID"].Value == null || row.Cells["ChequeID"].Value == DBNull.Value) return;

            int chequeID = Convert.ToInt32(row.Cells["ChequeID"].Value);

            // تمرير قيمة فارغة بدلاً من null لتجنب التحذير
            string safeRejectReason = rejectReason ?? string.Empty;

            bool updated = DBServiecs.Cheques_UpdateStatus(
                chequeID,
                statusText,
                DateTime.Now,
                safeRejectReason,
                statusCode,
                out string msg
            );

            if (updated)
            {
                row.Cells["Status"].Value = statusText;
                row.Cells["StatusDate"].Value = DateTime.Now;
                row.Cells["StatusCode"].Value = statusCode;
                row.Cells["RejectReason"].Value = string.IsNullOrWhiteSpace(rejectReason)
                    ? (object)DBNull.Value
                    : rejectReason;

                MessageBox.Show("تم تحديث حالة الشيك بنجاح.", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("حدث خطأ أثناء تحديث حالة الشيك:\n" + msg, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region @@@@ التعامل مع ادراج الشيكات  @@@@@@@@@@@@@@@@@@@@@@@@@
        private void DGV_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && DGV.CurrentRow != null && !DGV.CurrentRow.IsNewRow)
            {
                // التأكد من عدم الحفظ النهائي
                if (!string.IsNullOrWhiteSpace(lblSaveStatus.Text))
                {
                    CustomMessageBox.ShowInformation("لا يمكن حذف الشيك بعد حفظ الحافظة نهائيًا.", "تم الحفظ");
                    return;
                }

                // تأكيد المستخدم
                DialogResult result = MessageBox.Show("هل أنت متأكد من رغبتك في حذف هذا الشيك؟", "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes) return;

                // الحصول على معرف الشيك
                if (int.TryParse(DGV.CurrentRow.Cells["ChequeID"].Value?.ToString(), out int chequeID))
                {
                    // استدعاء دالة الحذف
                    bool isDeleted = DBServiecs.Cheques_Delete(chequeID, out string msg);

                    if (isDeleted)
                    {
                        // حذف الصف من الشبكة
                        DGV.Rows.Remove(DGV.CurrentRow);
                        CustomMessageBox.ShowInformation("تم حذف الشيك بنجاح.", "حذف");
                    }
                    else
                    {
                        CustomMessageBox.ShowWarning ("حدث خطأ أثناء الحذف: " + msg, "خطأ");
                    }
                }
            }
        }



        private void SelectAllAndMoveNext(Control current, Control next)
        {
            next.Focus();
        }

        private void txtNoteBatch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SelectAllAndMoveNext(txtNoteBatch, txtChequeNumber);
        }

        private void txtChequeNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SelectAllAndMoveNext(txtChequeNumber, txtDueDate);
        }

        private void txtDueDate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // إكمال السنة تلقائياً إذا كتب يوم/شهر فقط
                if (DateTime.TryParseExact(txtDueDate.Text, "d/M", null, DateTimeStyles.None, out DateTime partialDate))
                {
                    txtDueDate.Text = new DateTime(DateTime.Today.Year, partialDate.Month, partialDate.Day).ToString("dd/MM/yyyy");
                }

                if (!DateTime.TryParseExact(txtDueDate.Text, "d/M/yyyy", null, DateTimeStyles.None, out _))
                {
                    MessageBox.Show("صيغة التاريخ غير صحيحة، استخدم يوم/شهر/سنة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDueDate.SelectAll();
                    return;
                }

                SelectAllAndMoveNext(txtDueDate, txtAmount);
            }
        }

        private void txtAmount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!float.TryParse(txtAmount.Text, out _))
                {
                    MessageBox.Show("الرجاء إدخال مبلغ صحيح", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtAmount.SelectAll();
                    return;
                }

                SelectAllAndMoveNext(txtAmount, txtBankName);
            }
        }

        private void txtBankName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SelectAllAndMoveNext(txtBankName, txtBranch);
        }

        private void txtBranch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SelectAllAndMoveNext(txtBranch, txtNotes);
        }


        private void txtNotes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // استخراج البيانات
                int batchID = int.TryParse(lblBatchID.Text, out var id) ? id : 0;
                string chequeNumber = txtChequeNumber.Text.Trim();
                float amount = float.TryParse(txtAmount.Text, out var amt) ? amt : 0;
                string bank = txtBankName.Text.Trim();
                string branch = txtBranch.Text.Trim();
                string notes = txtNotes.Text.Trim();
                int statusCode = 1;
                DateTime? dueDate = DateTime.TryParseExact(txtDueDate.Text, "d/M/yyyy", null, DateTimeStyles.None, out var dt)
                    ? dt
                    : (DateTime?)null;
                    // التحقق من صحة التاريخ
                if (dueDate == null)
                {
                    MessageBox.Show("تاريخ غير صحيح", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // إدراج
                bool saved = DBServiecs.Cheques_Insert(
                    batchID,
                    chequeNumber,
                    dueDate,
                    amount,
                    bank,
                    branch,
                    notes,
                    statusCode,
                    out string msg
                );

                if (saved)
                {
                    ReloadCheques();
                    btnNewCheque_Click(null, null); // تهيئة الحقول
                }
                else
                {
                    MessageBox.Show("فشل الحفظ: " + msg, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            CalculateTotalAmountFromGridOnly();

        }

        private void btnNewCheque_Click(object? sender, EventArgs? e)
        {
            txtChequeNumber.Clear();
            txtDueDate.Clear();
            txtAmount.Clear();
            txtBankName.Clear();
            txtBranch.Clear();
            txtNotes.Clear();
            txtChequeNumber.Focus();
            lblCountCheques.Text = "عدد الشيكات :  " + DGV.RowCount.ToString();
            ApplyChequeColors ();
        }


        #endregion

        private void btnJournal_Click(object sender, EventArgs e)
        {
            // تحقق من أن السند محفوظ (أي أن lblSave تحتوي على نص)
            if (!string.IsNullOrWhiteSpace(lblSaveStatus.Text))
            {
                int billNo;
                int invTypeId;

                // تأكد من أن رقم السند ونوع الفاتورة متوفران بشكل صحيح
                if (int.TryParse(lblBatchID .Text, out billNo) && int.TryParse(lblMovTypeID.Text, out invTypeId))
                {
                    frm_Journal journalForm = new frm_Journal(billNo, invTypeId);
                    journalForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("تأكد من رقم السند ونوع العملية", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("يجب حفظ السند أولًا قبل عرض القيد المحاسبي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

    }
}
/*

 */