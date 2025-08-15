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
            tableLayoutPanel4 = new TableLayoutPanel();
            btnAllPeriod = new Button();
            tlpDate2 = new TableLayoutPanel();
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
            lblTitel = new Label();
            tlpButtons = new TableLayoutPanel();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel4.SuspendLayout();
            tlpDate2.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tlpDateRight.SuspendLayout();
            tlpButtons.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Fill;
            label5.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(251, 0);
            label5.Name = "label5";
            label5.Size = new Size(63, 42);
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
            cbxPrinters.Size = new Size(242, 27);
            cbxPrinters.TabIndex = 16;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(566, 0);
            label2.Name = "label2";
            label2.Size = new Size(63, 42);
            label2.TabIndex = 14;
            label2.Text = "الفرع";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 19.26952F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80.73048F));
            tableLayoutPanel4.Controls.Add(btnAllPeriod, 0, 0);
            tableLayoutPanel4.Controls.Add(tlpDate2, 1, 0);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(3, 142);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Size = new Size(638, 93);
            tableLayoutPanel4.TabIndex = 29;
            // 
            // btnAllPeriod
            // 
            btnAllPeriod.Dock = DockStyle.Fill;
            btnAllPeriod.Font = new Font("Times New Roman", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAllPeriod.Location = new Point(519, 3);
            btnAllPeriod.Name = "btnAllPeriod";
            btnAllPeriod.Size = new Size(116, 87);
            btnAllPeriod.TabIndex = 9;
            btnAllPeriod.Text = "كل الفترة";
            btnAllPeriod.UseVisualStyleBackColor = true;
            btnAllPeriod.Click += btnAllPeriod_Click;
            // 
            // tlpDate2
            // 
            tlpDate2.BackColor = Color.Transparent;
            tlpDate2.ColumnCount = 3;
            tlpDate2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.76596F));
            tlpDate2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 49.29078F));
            tlpDate2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 37.76596F));
            tlpDate2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tlpDate2.Controls.Add(lblAmountOfDay, 2, 1);
            tlpDate2.Controls.Add(lblAllPeriod, 2, 0);
            tlpDate2.Controls.Add(dtpEnd, 1, 1);
            tlpDate2.Controls.Add(lbl, 0, 1);
            tlpDate2.Controls.Add(dtpStart, 1, 0);
            tlpDate2.Controls.Add(label1, 0, 0);
            tlpDate2.Dock = DockStyle.Fill;
            tlpDate2.Location = new Point(3, 3);
            tlpDate2.Name = "tlpDate2";
            tlpDate2.RowCount = 2;
            tlpDate2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlpDate2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlpDate2.Size = new Size(510, 87);
            tlpDate2.TabIndex = 8;
            // 
            // lblAmountOfDay
            // 
            lblAmountOfDay.AutoSize = true;
            lblAmountOfDay.Dock = DockStyle.Fill;
            lblAmountOfDay.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAmountOfDay.ForeColor = Color.Blue;
            lblAmountOfDay.Location = new Point(3, 43);
            lblAmountOfDay.Name = "lblAmountOfDay";
            lblAmountOfDay.Size = new Size(188, 44);
            lblAmountOfDay.TabIndex = 15;
            lblAmountOfDay.Text = "0";
            lblAmountOfDay.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblAllPeriod
            // 
            lblAllPeriod.AutoSize = true;
            lblAllPeriod.Dock = DockStyle.Fill;
            lblAllPeriod.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAllPeriod.ForeColor = Color.Blue;
            lblAllPeriod.Location = new Point(3, 0);
            lblAllPeriod.Name = "lblAllPeriod";
            lblAllPeriod.Size = new Size(188, 43);
            lblAllPeriod.TabIndex = 14;
            lblAllPeriod.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dtpEnd
            // 
            dtpEnd.CustomFormat = "yyyy   MM /d";
            dtpEnd.Dock = DockStyle.Fill;
            dtpEnd.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dtpEnd.Format = DateTimePickerFormat.Custom;
            dtpEnd.Location = new Point(197, 46);
            dtpEnd.Name = "dtpEnd";
            dtpEnd.RightToLeftLayout = true;
            dtpEnd.Size = new Size(245, 29);
            dtpEnd.TabIndex = 8;
            // 
            // lbl
            // 
            lbl.AutoSize = true;
            lbl.Dock = DockStyle.Fill;
            lbl.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbl.ForeColor = Color.Blue;
            lbl.Location = new Point(448, 43);
            lbl.Name = "lbl";
            lbl.Size = new Size(59, 44);
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
            dtpStart.Location = new Point(197, 3);
            dtpStart.Name = "dtpStart";
            dtpStart.RightToLeftLayout = true;
            dtpStart.Size = new Size(245, 29);
            dtpStart.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Blue;
            label1.Location = new Point(448, 0);
            label1.Name = "label1";
            label1.Size = new Size(59, 43);
            label1.TabIndex = 5;
            label1.Text = "من";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // rdoPreviousYear
            // 
            rdoPreviousYear.AutoSize = true;
            rdoPreviousYear.Font = new Font("Times New Roman", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rdoPreviousYear.ForeColor = Color.Blue;
            rdoPreviousYear.Location = new Point(213, 83);
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
            rdoPreviousDay.Location = new Point(213, 3);
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
            rdoThisYear.Location = new Point(387, 83);
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
            rdoToDay.Location = new Point(409, 3);
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
            rdoPreviousMonth.Location = new Point(205, 43);
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
            rdoThisMonth.Location = new Point(386, 43);
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
            rdoAllPeriod.Location = new Point(552, 83);
            rdoAllPeriod.Name = "rdoAllPeriod";
            rdoAllPeriod.RightToLeft = RightToLeft.Yes;
            rdoAllPeriod.Size = new Size(83, 26);
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
            tableLayoutPanel2.Size = new Size(638, 48);
            tableLayoutPanel2.TabIndex = 26;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.BackColor = Color.LightGray;
            tableLayoutPanel3.ColumnCount = 4;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 39F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 39F));
            tableLayoutPanel3.Controls.Add(label5, 2, 0);
            tableLayoutPanel3.Controls.Add(cbxPrinters, 3, 0);
            tableLayoutPanel3.Controls.Add(label2, 0, 0);
            tableLayoutPanel3.Controls.Add(cbxWarehouse, 1, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(632, 42);
            tableLayoutPanel3.TabIndex = 26;
            // 
            // cbxWarehouse
            // 
            cbxWarehouse.BackColor = Color.Khaki;
            cbxWarehouse.Dock = DockStyle.Fill;
            cbxWarehouse.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            cbxWarehouse.FormattingEnabled = true;
            cbxWarehouse.Location = new Point(320, 3);
            cbxWarehouse.Name = "cbxWarehouse";
            cbxWarehouse.Size = new Size(240, 27);
            cbxWarehouse.TabIndex = 16;
            // 
            // tlpDateRight
            // 
            tlpDateRight.ColumnCount = 3;
            tlpDateRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 26.57431F));
            tlpDateRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 26.07053F));
            tlpDateRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 47.35516F));
            tlpDateRight.Controls.Add(rdoPreviousYear, 2, 2);
            tlpDateRight.Controls.Add(rdoPreviousDay, 2, 0);
            tlpDateRight.Controls.Add(rdoThisYear, 1, 2);
            tlpDateRight.Controls.Add(rdoToDay, 1, 0);
            tlpDateRight.Controls.Add(rdoPreviousMonth, 2, 1);
            tlpDateRight.Controls.Add(rdoThisMonth, 1, 1);
            tlpDateRight.Controls.Add(rdoAllPeriod, 0, 2);
            tlpDateRight.Dock = DockStyle.Fill;
            tlpDateRight.Location = new Point(3, 241);
            tlpDateRight.Name = "tlpDateRight";
            tlpDateRight.RowCount = 3;
            tlpDateRight.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tlpDateRight.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tlpDateRight.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tlpDateRight.Size = new Size(638, 120);
            tlpDateRight.TabIndex = 25;
            // 
            // btnGo
            // 
            btnGo.Dock = DockStyle.Fill;
            btnGo.Font = new Font("Times New Roman", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGo.Location = new Point(431, 5);
            btnGo.Name = "btnGo";
            btnGo.Size = new Size(202, 52);
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
            btnSaveAndClose.Size = new Size(204, 52);
            btnSaveAndClose.TabIndex = 1;
            btnSaveAndClose.Text = "خروج";
            btnSaveAndClose.UseVisualStyleBackColor = true;
            btnSaveAndClose.Click += btnSaveAndClose_Click;
            // 
            // btnPrint
            // 
            btnPrint.Dock = DockStyle.Fill;
            btnPrint.Font = new Font("Times New Roman", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnPrint.Location = new Point(219, 5);
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new Size(202, 52);
            btnPrint.TabIndex = 1;
            btnPrint.Text = "طباعة";
            btnPrint.UseVisualStyleBackColor = true;
            // 
            // lblTitel
            // 
            lblTitel.AutoSize = true;
            lblTitel.BackColor = Color.FromArgb(192, 255, 192);
            lblTitel.Dock = DockStyle.Fill;
            lblTitel.Font = new Font("Times New Roman", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitel.ForeColor = Color.Purple;
            lblTitel.Location = new Point(3, 54);
            lblTitel.Name = "lblTitel";
            lblTitel.Size = new Size(638, 67);
            lblTitel.TabIndex = 28;
            lblTitel.Text = "برجاء تحديد عوامل فترة التقرير";
            lblTitel.TextAlign = ContentAlignment.MiddleCenter;
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
            tlpButtons.Location = new Point(3, 385);
            tlpButtons.Name = "tlpButtons";
            tlpButtons.Padding = new Padding(2);
            tlpButtons.RowCount = 1;
            tlpButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpButtons.Size = new Size(638, 62);
            tlpButtons.TabIndex = 27;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(lblTitel, 0, 1);
            tableLayoutPanel1.Controls.Add(tlpButtons, 0, 6);
            tableLayoutPanel1.Controls.Add(tlpDateRight, 0, 4);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel4, 0, 3);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 7;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 12F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 4F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 22F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 28F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 4F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanel1.Size = new Size(644, 450);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // frmSettingReports
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(644, 450);
            Controls.Add(tableLayoutPanel1);
            Name = "frmSettingReports";
            RightToLeft = RightToLeft.Yes;
            Text = "frmSettingReports";
            Load += frmSettingReports_Load;
            tableLayoutPanel4.ResumeLayout(false);
            tlpDate2.ResumeLayout(false);
            tlpDate2.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            tlpDateRight.ResumeLayout(false);
            tlpDateRight.PerformLayout();
            tlpButtons.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label label5;
        private ComboBox cbxPrinters;
        private Label label2;
        private TableLayoutPanel tableLayoutPanel4;
        private Button btnAllPeriod;
        private TableLayoutPanel tlpDate2;
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
        private Label lblTitel;
        private TableLayoutPanel tlpButtons;
        private TableLayoutPanel tableLayoutPanel1;
    }
}