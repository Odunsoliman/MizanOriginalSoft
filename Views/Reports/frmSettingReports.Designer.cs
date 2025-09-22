namespace MizanOriginalSoft.Views.Reports
{
    partial class frmSettingReports
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label5 = new Label();
            cbxPrinters = new ComboBox();
            label2 = new Label();
            btnAllPeriod = new Button();
            lblAmountOfDay = new Label();
            lblAllPeriod = new Label();
            dtpEnd = new DateTimePicker();
            lbl = new Label();
            dtpStart = new DateTimePicker();
            label1 = new Label();
            rdoPreviousYear = new RadioButton();
            rdoPreviousDay = new RadioButton();
            rdoThisYear = new RadioButton();
            rdoToDay = new RadioButton();
            rdoPreviousMonth = new RadioButton();
            rdoThisMonth = new RadioButton();
            rdoAllPeriod = new RadioButton();
            tableLayoutPanel2 = new TableLayoutPanel();
            tableLayoutPanel3 = new TableLayoutPanel();
            cbxWarehouse = new ComboBox();
            tlpDateRight = new TableLayoutPanel();
            btnGo = new Button();
            btnSaveAndClose = new Button();
            btnPrint = new Button();
            tlpButtons = new TableLayoutPanel();
            tableLayoutPanel1 = new TableLayoutPanel();
            lblTitel = new Label();
            tableLayoutPanel5 = new TableLayoutPanel();
            tableLayoutPanel6 = new TableLayoutPanel();
            dataGridView1 = new DataGridView();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tlpDateRight.SuspendLayout();
            tlpButtons.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            tableLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Fill;
            label5.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(219, 0);
            label5.Name = "label5";
            label5.Size = new Size(68, 48);
            label5.TabIndex = 14;
            label5.Text = "الطابعة";
            label5.TextAlign = ContentAlignment.MiddleRight;
            // 
            // cbxPrinters
            // 
            cbxPrinters.BackColor = Color.Khaki;
            cbxPrinters.Dock = DockStyle.Fill;
            cbxPrinters.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            cbxPrinters.FormattingEnabled = true;
            cbxPrinters.Location = new Point(3, 3);
            cbxPrinters.Name = "cbxPrinters";
            cbxPrinters.Size = new Size(210, 27);
            cbxPrinters.TabIndex = 16;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(999, 0);
            label2.Name = "label2";
            label2.Size = new Size(68, 48);
            label2.TabIndex = 14;
            label2.Text = "الفرع";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // btnAllPeriod
            // 
            btnAllPeriod.Dock = DockStyle.Fill;
            btnAllPeriod.Font = new Font("Times New Roman", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAllPeriod.Location = new Point(983, 3);
            btnAllPeriod.Name = "btnAllPeriod";
            btnAllPeriod.Size = new Size(90, 109);
            btnAllPeriod.TabIndex = 9;
            btnAllPeriod.Text = "كل الفترة";
            btnAllPeriod.UseVisualStyleBackColor = true;
            // 
            // lblAmountOfDay
            // 
            lblAmountOfDay.Dock = DockStyle.Left;
            lblAmountOfDay.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAmountOfDay.ForeColor = Color.Blue;
            lblAmountOfDay.Location = new Point(198, 36);
            lblAmountOfDay.Name = "lblAmountOfDay";
            lblAmountOfDay.Size = new Size(188, 36);
            lblAmountOfDay.TabIndex = 15;
            lblAmountOfDay.Text = "0";
            lblAmountOfDay.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblAllPeriod
            // 
            lblAllPeriod.Dock = DockStyle.Left;
            lblAllPeriod.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAllPeriod.ForeColor = Color.Blue;
            lblAllPeriod.Location = new Point(198, 0);
            lblAllPeriod.Name = "lblAllPeriod";
            lblAllPeriod.Size = new Size(188, 36);
            lblAllPeriod.TabIndex = 14;
            lblAllPeriod.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dtpEnd
            // 
            dtpEnd.CustomFormat = "yyyy   MM /d";
            dtpEnd.Dock = DockStyle.Fill;
            dtpEnd.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dtpEnd.Format = DateTimePickerFormat.Custom;
            dtpEnd.Location = new Point(3, 57);
            dtpEnd.Name = "dtpEnd";
            dtpEnd.RightToLeftLayout = true;
            dtpEnd.Size = new Size(153, 29);
            dtpEnd.TabIndex = 8;
            // 
            // lbl
            // 
            lbl.AutoSize = true;
            lbl.Dock = DockStyle.Fill;
            lbl.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbl.ForeColor = Color.Blue;
            lbl.Location = new Point(162, 54);
            lbl.Name = "lbl";
            lbl.Size = new Size(28, 55);
            lbl.TabIndex = 7;
            lbl.Text = "الى";
            lbl.TextAlign = ContentAlignment.TopRight;
            // 
            // dtpStart
            // 
            dtpStart.CustomFormat = "yyyy   MM /d";
            dtpStart.Dock = DockStyle.Fill;
            dtpStart.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dtpStart.Format = DateTimePickerFormat.Custom;
            dtpStart.Location = new Point(3, 3);
            dtpStart.Name = "dtpStart";
            dtpStart.RightToLeftLayout = true;
            dtpStart.Size = new Size(153, 29);
            dtpStart.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Blue;
            label1.Location = new Point(162, 0);
            label1.Name = "label1";
            label1.Size = new Size(28, 54);
            label1.TabIndex = 5;
            label1.Text = "من";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // rdoPreviousYear
            // 
            rdoPreviousYear.AutoSize = true;
            rdoPreviousYear.Font = new Font("Times New Roman", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rdoPreviousYear.ForeColor = Color.Blue;
            rdoPreviousYear.Location = new Point(461, 75);
            rdoPreviousYear.Name = "rdoPreviousYear";
            rdoPreviousYear.RightToLeft = RightToLeft.Yes;
            rdoPreviousYear.Size = new Size(87, 26);
            rdoPreviousYear.TabIndex = 6;
            rdoPreviousYear.Text = "عام سابق";
            rdoPreviousYear.UseVisualStyleBackColor = true;
            // 
            // rdoPreviousDay
            // 
            rdoPreviousDay.AutoSize = true;
            rdoPreviousDay.Font = new Font("Times New Roman", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rdoPreviousDay.ForeColor = Color.Blue;
            rdoPreviousDay.Location = new Point(461, 3);
            rdoPreviousDay.Name = "rdoPreviousDay";
            rdoPreviousDay.RightToLeft = RightToLeft.Yes;
            rdoPreviousDay.Size = new Size(87, 26);
            rdoPreviousDay.TabIndex = 4;
            rdoPreviousDay.Text = "يوم سابق";
            rdoPreviousDay.UseVisualStyleBackColor = true;
            // 
            // rdoThisYear
            // 
            rdoThisYear.AutoSize = true;
            rdoThisYear.Font = new Font("Times New Roman", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rdoThisYear.ForeColor = Color.Blue;
            rdoThisYear.Location = new Point(631, 75);
            rdoThisYear.Name = "rdoThisYear";
            rdoThisYear.RightToLeft = RightToLeft.Yes;
            rdoThisYear.Size = new Size(79, 26);
            rdoThisYear.TabIndex = 2;
            rdoThisYear.Text = "هذا العام";
            rdoThisYear.UseVisualStyleBackColor = true;
            // 
            // rdoToDay
            // 
            rdoToDay.AutoSize = true;
            rdoToDay.Checked = true;
            rdoToDay.Font = new Font("Times New Roman", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rdoToDay.ForeColor = Color.Blue;
            rdoToDay.Location = new Point(653, 3);
            rdoToDay.Name = "rdoToDay";
            rdoToDay.RightToLeft = RightToLeft.Yes;
            rdoToDay.Size = new Size(57, 26);
            rdoToDay.TabIndex = 3;
            rdoToDay.TabStop = true;
            rdoToDay.Text = "اليوم";
            rdoToDay.UseVisualStyleBackColor = true;
            // 
            // rdoPreviousMonth
            // 
            rdoPreviousMonth.AutoSize = true;
            rdoPreviousMonth.Font = new Font("Times New Roman", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rdoPreviousMonth.ForeColor = Color.Blue;
            rdoPreviousMonth.Location = new Point(453, 39);
            rdoPreviousMonth.Name = "rdoPreviousMonth";
            rdoPreviousMonth.RightToLeft = RightToLeft.Yes;
            rdoPreviousMonth.Size = new Size(95, 26);
            rdoPreviousMonth.TabIndex = 1;
            rdoPreviousMonth.Text = "شهر سابق";
            rdoPreviousMonth.UseVisualStyleBackColor = true;
            // 
            // rdoThisMonth
            // 
            rdoThisMonth.AutoSize = true;
            rdoThisMonth.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rdoThisMonth.ForeColor = Color.Blue;
            rdoThisMonth.Location = new Point(630, 39);
            rdoThisMonth.Name = "rdoThisMonth";
            rdoThisMonth.RightToLeft = RightToLeft.Yes;
            rdoThisMonth.Size = new Size(80, 23);
            rdoThisMonth.TabIndex = 5;
            rdoThisMonth.Text = "هذا الشهر";
            rdoThisMonth.UseVisualStyleBackColor = true;
            // 
            // rdoAllPeriod
            // 
            rdoAllPeriod.AutoSize = true;
            rdoAllPeriod.Font = new Font("Times New Roman", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rdoAllPeriod.ForeColor = Color.Blue;
            rdoAllPeriod.Location = new Point(716, 75);
            rdoAllPeriod.Name = "rdoAllPeriod";
            rdoAllPeriod.RightToLeft = RightToLeft.Yes;
            rdoAllPeriod.Size = new Size(56, 26);
            rdoAllPeriod.TabIndex = 0;
            rdoAllPeriod.Text = "كل الفترة";
            rdoAllPeriod.UseVisualStyleBackColor = true;
            rdoAllPeriod.Visible = false;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel2.Controls.Add(tableLayoutPanel3, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(1076, 54);
            tableLayoutPanel2.TabIndex = 26;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.BackColor = Color.LightGray;
            tableLayoutPanel3.ColumnCount = 5;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 7F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 7F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.Controls.Add(lblTitel, 2, 0);
            tableLayoutPanel3.Controls.Add(label5, 3, 0);
            tableLayoutPanel3.Controls.Add(cbxPrinters, 4, 0);
            tableLayoutPanel3.Controls.Add(label2, 0, 0);
            tableLayoutPanel3.Controls.Add(cbxWarehouse, 1, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(1070, 48);
            tableLayoutPanel3.TabIndex = 26;
            // 
            // cbxWarehouse
            // 
            cbxWarehouse.BackColor = Color.Khaki;
            cbxWarehouse.Dock = DockStyle.Fill;
            cbxWarehouse.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            cbxWarehouse.FormattingEnabled = true;
            cbxWarehouse.Location = new Point(785, 3);
            cbxWarehouse.Name = "cbxWarehouse";
            cbxWarehouse.Size = new Size(208, 27);
            cbxWarehouse.TabIndex = 16;
            // 
            // tlpDateRight
            // 
            tlpDateRight.ColumnCount = 4;
            tlpDateRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8F));
            tlpDateRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 21F));
            tlpDateRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 21F));
            tlpDateRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpDateRight.Controls.Add(lblAllPeriod, 3, 0);
            tlpDateRight.Controls.Add(lblAmountOfDay, 3, 1);
            tlpDateRight.Controls.Add(rdoPreviousYear, 2, 2);
            tlpDateRight.Controls.Add(rdoPreviousDay, 2, 0);
            tlpDateRight.Controls.Add(rdoThisYear, 1, 2);
            tlpDateRight.Controls.Add(rdoToDay, 1, 0);
            tlpDateRight.Controls.Add(rdoPreviousMonth, 2, 1);
            tlpDateRight.Controls.Add(rdoThisMonth, 1, 1);
            tlpDateRight.Controls.Add(rdoAllPeriod, 0, 2);
            tlpDateRight.Dock = DockStyle.Fill;
            tlpDateRight.Location = new Point(3, 3);
            tlpDateRight.Name = "tlpDateRight";
            tlpDateRight.RowCount = 3;
            tlpDateRight.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tlpDateRight.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tlpDateRight.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tlpDateRight.Size = new Size(775, 109);
            tlpDateRight.TabIndex = 25;
            // 
            // btnGo
            // 
            btnGo.Dock = DockStyle.Fill;
            btnGo.Font = new Font("Times New Roman", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGo.Location = new Point(723, 5);
            btnGo.Name = "btnGo";
            btnGo.Size = new Size(348, 46);
            btnGo.TabIndex = 1;
            btnGo.Text = "معاينة";
            btnGo.UseVisualStyleBackColor = true;
            btnGo.Click += btnGo_Click;
            // 
            // btnSaveAndClose
            // 
            btnSaveAndClose.Dock = DockStyle.Fill;
            btnSaveAndClose.Font = new Font("Times New Roman", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSaveAndClose.Location = new Point(5, 5);
            btnSaveAndClose.Name = "btnSaveAndClose";
            btnSaveAndClose.Size = new Size(350, 46);
            btnSaveAndClose.TabIndex = 1;
            btnSaveAndClose.Text = "خروج";
            btnSaveAndClose.UseVisualStyleBackColor = true;
            btnSaveAndClose.Click += btnSaveAndClose_Click;
            // 
            // btnPrint
            // 
            btnPrint.Dock = DockStyle.Fill;
            btnPrint.Font = new Font("Times New Roman", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnPrint.Location = new Point(365, 5);
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new Size(348, 46);
            btnPrint.TabIndex = 1;
            btnPrint.Text = "طباعة";
            btnPrint.UseVisualStyleBackColor = true;
            // 
            // tlpButtons
            // 
            tlpButtons.BackColor = Color.Khaki;
            tlpButtons.ColumnCount = 5;
            tlpButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tlpButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 4F));
            tlpButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tlpButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 4F));
            tlpButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33333F));
            tlpButtons.Controls.Add(btnGo, 0, 0);
            tlpButtons.Controls.Add(btnSaveAndClose, 4, 0);
            tlpButtons.Controls.Add(btnPrint, 2, 0);
            tlpButtons.Dock = DockStyle.Fill;
            tlpButtons.Location = new Point(3, 547);
            tlpButtons.Name = "tlpButtons";
            tlpButtons.Padding = new Padding(2);
            tlpButtons.RowCount = 1;
            tlpButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpButtons.Size = new Size(1076, 56);
            tlpButtons.TabIndex = 27;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(tlpButtons, 0, 3);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel5, 0, 1);
            tableLayoutPanel1.Controls.Add(dataGridView1, 0, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.Size = new Size(1082, 606);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // lblTitel
            // 
            lblTitel.AutoSize = true;
            lblTitel.BackColor = Color.Transparent;
            lblTitel.Dock = DockStyle.Fill;
            lblTitel.Font = new Font("Times New Roman", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitel.ForeColor = Color.Purple;
            lblTitel.Location = new Point(293, 0);
            lblTitel.Name = "lblTitel";
            lblTitel.Size = new Size(486, 48);
            lblTitel.TabIndex = 29;
            lblTitel.Text = "برجاء تحديد عوامل فترة التقرير";
            lblTitel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 3;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8.921933F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18.4944229F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 72.58364F));
            tableLayoutPanel5.Controls.Add(btnAllPeriod, 0, 0);
            tableLayoutPanel5.Controls.Add(tlpDateRight, 2, 0);
            tableLayoutPanel5.Controls.Add(tableLayoutPanel6, 1, 0);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(3, 63);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 1;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel5.Size = new Size(1076, 115);
            tableLayoutPanel5.TabIndex = 30;
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 2;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18.1347141F));
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 81.86529F));
            tableLayoutPanel6.Controls.Add(label1, 0, 0);
            tableLayoutPanel6.Controls.Add(lbl, 0, 1);
            tableLayoutPanel6.Controls.Add(dtpEnd, 1, 1);
            tableLayoutPanel6.Controls.Add(dtpStart, 1, 0);
            tableLayoutPanel6.Dock = DockStyle.Fill;
            tableLayoutPanel6.Location = new Point(784, 3);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 2;
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.Size = new Size(193, 109);
            tableLayoutPanel6.TabIndex = 10;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(3, 184);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.Size = new Size(1076, 357);
            dataGridView1.TabIndex = 31;
            // 
            // frmSettingReports
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1082, 606);
            Controls.Add(tableLayoutPanel1);
            Name = "frmSettingReports";
            RightToLeft = RightToLeft.Yes;
            Text = "frmSettingReports";
            Load += frmSettingReports_Load;
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            tlpDateRight.ResumeLayout(false);
            tlpDateRight.PerformLayout();
            tlpButtons.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel6.ResumeLayout(false);
            tableLayoutPanel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label label5;
        private ComboBox cbxPrinters;
        private Label label2;
        private Button btnAllPeriod;
        private Label lblAmountOfDay;
        private Label lblAllPeriod;
        private DateTimePicker dtpEnd;
        private Label lbl;
        private DateTimePicker dtpStart;
        private Label label1;
        private RadioButton rdoPreviousYear;
        private RadioButton rdoPreviousDay;
        private RadioButton rdoThisYear;
        private RadioButton rdoToDay;
        private RadioButton rdoPreviousMonth;
        private RadioButton rdoThisMonth;
        private RadioButton rdoAllPeriod;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
        private ComboBox cbxWarehouse;
        private TableLayoutPanel tlpDateRight;
        private Button btnGo;
        private Button btnSaveAndClose;
        private Button btnPrint;
        private TableLayoutPanel tlpButtons;
        private TableLayoutPanel tableLayoutPanel1;
        private Label lblTitel;
        private TableLayoutPanel tableLayoutPanel5;
        private TableLayoutPanel tableLayoutPanel6;
        private DataGridView dataGridView1;
    }
}