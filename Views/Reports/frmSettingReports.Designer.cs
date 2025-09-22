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
            lblTitel = new Label();
            cbxWarehouse = new ComboBox();
            tlpDateRight = new TableLayoutPanel();
            btnSaveAndClose = new Button();
            btnPrint = new Button();
            tlpButtons = new TableLayoutPanel();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel5 = new TableLayoutPanel();
            dataGridView1 = new DataGridView();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tlpDateRight.SuspendLayout();
            tlpButtons.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Fill;
            label5.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(249, 66);
            label5.Name = "label5";
            label5.Size = new Size(57, 34);
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
            cbxPrinters.Location = new Point(3, 69);
            cbxPrinters.Name = "cbxPrinters";
            cbxPrinters.Size = new Size(240, 27);
            cbxPrinters.TabIndex = 16;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(249, 0);
            label2.Name = "label2";
            label2.Size = new Size(57, 33);
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
            btnAllPeriod.Size = new Size(90, 100);
            btnAllPeriod.TabIndex = 9;
            btnAllPeriod.Text = "كل الفترة";
            btnAllPeriod.UseVisualStyleBackColor = true;
            // 
            // lblAmountOfDay
            // 
            lblAmountOfDay.Dock = DockStyle.Fill;
            lblAmountOfDay.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAmountOfDay.ForeColor = Color.Blue;
            lblAmountOfDay.Location = new Point(783, 66);
            lblAmountOfDay.Name = "lblAmountOfDay";
            lblAmountOfDay.Size = new Size(140, 34);
            lblAmountOfDay.TabIndex = 15;
            lblAmountOfDay.Text = "0";
            lblAmountOfDay.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblAllPeriod
            // 
            lblAllPeriod.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAllPeriod.ForeColor = Color.Blue;
            lblAllPeriod.Location = new Point(929, 66);
            lblAllPeriod.Name = "lblAllPeriod";
            lblAllPeriod.Size = new Size(42, 34);
            lblAllPeriod.TabIndex = 14;
            lblAllPeriod.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dtpEnd
            // 
            dtpEnd.CustomFormat = "yyyy   MM /d";
            dtpEnd.Dock = DockStyle.Fill;
            dtpEnd.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dtpEnd.Format = DateTimePickerFormat.Custom;
            dtpEnd.Location = new Point(783, 36);
            dtpEnd.Name = "dtpEnd";
            dtpEnd.RightToLeftLayout = true;
            dtpEnd.Size = new Size(140, 29);
            dtpEnd.TabIndex = 8;
            // 
            // lbl
            // 
            lbl.AutoSize = true;
            lbl.Dock = DockStyle.Fill;
            lbl.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbl.ForeColor = Color.Blue;
            lbl.Location = new Point(929, 33);
            lbl.Name = "lbl";
            lbl.Size = new Size(42, 33);
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
            dtpStart.Location = new Point(783, 3);
            dtpStart.Name = "dtpStart";
            dtpStart.RightToLeftLayout = true;
            dtpStart.Size = new Size(140, 29);
            dtpStart.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Blue;
            label1.Location = new Point(929, 0);
            label1.Name = "label1";
            label1.Size = new Size(42, 33);
            label1.TabIndex = 5;
            label1.Text = "من";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // rdoPreviousYear
            // 
            rdoPreviousYear.AutoSize = true;
            rdoPreviousYear.Font = new Font("Times New Roman", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rdoPreviousYear.ForeColor = Color.Blue;
            rdoPreviousYear.Location = new Point(513, 69);
            rdoPreviousYear.Name = "rdoPreviousYear";
            rdoPreviousYear.RightToLeft = RightToLeft.Yes;
            rdoPreviousYear.Size = new Size(87, 26);
            rdoPreviousYear.TabIndex = 6;
            rdoPreviousYear.Text = "عام سابق";
            rdoPreviousYear.UseVisualStyleBackColor = true;
            rdoPreviousYear.CheckedChanged += rdoPreviousYear_CheckedChanged;
            // 
            // rdoPreviousDay
            // 
            rdoPreviousDay.AutoSize = true;
            rdoPreviousDay.Font = new Font("Times New Roman", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rdoPreviousDay.ForeColor = Color.Blue;
            rdoPreviousDay.Location = new Point(513, 3);
            rdoPreviousDay.Name = "rdoPreviousDay";
            rdoPreviousDay.RightToLeft = RightToLeft.Yes;
            rdoPreviousDay.Size = new Size(87, 26);
            rdoPreviousDay.TabIndex = 4;
            rdoPreviousDay.Text = "يوم سابق";
            rdoPreviousDay.UseVisualStyleBackColor = true;
            rdoPreviousDay.CheckedChanged += rdoPreviousDay_CheckedChanged;
            // 
            // rdoThisYear
            // 
            rdoThisYear.AutoSize = true;
            rdoThisYear.Font = new Font("Times New Roman", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rdoThisYear.ForeColor = Color.Blue;
            rdoThisYear.Location = new Point(669, 69);
            rdoThisYear.Name = "rdoThisYear";
            rdoThisYear.RightToLeft = RightToLeft.Yes;
            rdoThisYear.Size = new Size(79, 26);
            rdoThisYear.TabIndex = 2;
            rdoThisYear.Text = "هذا العام";
            rdoThisYear.UseVisualStyleBackColor = true;
            rdoThisYear.CheckedChanged += rdoThisYear_CheckedChanged;
            // 
            // rdoToDay
            // 
            rdoToDay.AutoSize = true;
            rdoToDay.Checked = true;
            rdoToDay.Font = new Font("Times New Roman", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rdoToDay.ForeColor = Color.Blue;
            rdoToDay.Location = new Point(691, 3);
            rdoToDay.Name = "rdoToDay";
            rdoToDay.RightToLeft = RightToLeft.Yes;
            rdoToDay.Size = new Size(57, 26);
            rdoToDay.TabIndex = 3;
            rdoToDay.TabStop = true;
            rdoToDay.Text = "اليوم";
            rdoToDay.UseVisualStyleBackColor = true;
            rdoToDay.CheckedChanged += rdoToDay_CheckedChanged;
            // 
            // rdoPreviousMonth
            // 
            rdoPreviousMonth.AutoSize = true;
            rdoPreviousMonth.Font = new Font("Times New Roman", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rdoPreviousMonth.ForeColor = Color.Blue;
            rdoPreviousMonth.Location = new Point(505, 36);
            rdoPreviousMonth.Name = "rdoPreviousMonth";
            rdoPreviousMonth.RightToLeft = RightToLeft.Yes;
            rdoPreviousMonth.Size = new Size(95, 26);
            rdoPreviousMonth.TabIndex = 1;
            rdoPreviousMonth.Text = "شهر سابق";
            rdoPreviousMonth.UseVisualStyleBackColor = true;
            rdoPreviousMonth.CheckedChanged += rdoPreviousMonth_CheckedChanged;
            // 
            // rdoThisMonth
            // 
            rdoThisMonth.AutoSize = true;
            rdoThisMonth.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rdoThisMonth.ForeColor = Color.Blue;
            rdoThisMonth.Location = new Point(668, 36);
            rdoThisMonth.Name = "rdoThisMonth";
            rdoThisMonth.RightToLeft = RightToLeft.Yes;
            rdoThisMonth.Size = new Size(80, 23);
            rdoThisMonth.TabIndex = 5;
            rdoThisMonth.Text = "هذا الشهر";
            rdoThisMonth.UseVisualStyleBackColor = true;
            rdoThisMonth.CheckedChanged += rdoThisMonth_CheckedChanged;
            // 
            // rdoAllPeriod
            // 
            rdoAllPeriod.AutoSize = true;
            rdoAllPeriod.Font = new Font("Times New Roman", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rdoAllPeriod.ForeColor = Color.Blue;
            rdoAllPeriod.Location = new Point(754, 69);
            rdoAllPeriod.Name = "rdoAllPeriod";
            rdoAllPeriod.RightToLeft = RightToLeft.Yes;
            rdoAllPeriod.Size = new Size(23, 26);
            rdoAllPeriod.TabIndex = 0;
            rdoAllPeriod.Text = "كل الفترة";
            rdoAllPeriod.UseVisualStyleBackColor = true;
            rdoAllPeriod.Visible = false;
            rdoAllPeriod.CheckedChanged += rdoAllPeriod_CheckedChanged;
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
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel3.Controls.Add(lblTitel, 0, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(1070, 48);
            tableLayoutPanel3.TabIndex = 26;
            // 
            // lblTitel
            // 
            lblTitel.AutoSize = true;
            lblTitel.BackColor = Color.Transparent;
            lblTitel.Dock = DockStyle.Fill;
            lblTitel.Font = new Font("Times New Roman", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitel.ForeColor = Color.Purple;
            lblTitel.Location = new Point(3, 0);
            lblTitel.Name = "lblTitel";
            lblTitel.Size = new Size(1064, 48);
            lblTitel.TabIndex = 29;
            lblTitel.Text = "برجاء تحديد عوامل فترة التقرير";
            lblTitel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cbxWarehouse
            // 
            cbxWarehouse.BackColor = Color.Khaki;
            cbxWarehouse.Dock = DockStyle.Fill;
            cbxWarehouse.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            cbxWarehouse.FormattingEnabled = true;
            cbxWarehouse.Location = new Point(3, 3);
            cbxWarehouse.Name = "cbxWarehouse";
            cbxWarehouse.Size = new Size(240, 27);
            cbxWarehouse.TabIndex = 16;
            // 
            // tlpDateRight
            // 
            tlpDateRight.ColumnCount = 8;
            tlpDateRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5F));
            tlpDateRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            tlpDateRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3F));
            tlpDateRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15.2258062F));
            tlpDateRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17.8064518F));
            tlpDateRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.4229975F));
            tlpDateRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6.46817255F));
            tlpDateRight.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.0513344F));
            tlpDateRight.Controls.Add(label1, 0, 0);
            tlpDateRight.Controls.Add(cbxWarehouse, 7, 0);
            tlpDateRight.Controls.Add(label2, 6, 0);
            tlpDateRight.Controls.Add(cbxPrinters, 7, 2);
            tlpDateRight.Controls.Add(label5, 6, 2);
            tlpDateRight.Controls.Add(lbl, 0, 1);
            tlpDateRight.Controls.Add(dtpEnd, 1, 1);
            tlpDateRight.Controls.Add(rdoPreviousYear, 4, 2);
            tlpDateRight.Controls.Add(dtpStart, 1, 0);
            tlpDateRight.Controls.Add(rdoPreviousDay, 4, 0);
            tlpDateRight.Controls.Add(rdoThisYear, 3, 2);
            tlpDateRight.Controls.Add(rdoToDay, 3, 0);
            tlpDateRight.Controls.Add(rdoPreviousMonth, 4, 1);
            tlpDateRight.Controls.Add(rdoThisMonth, 3, 1);
            tlpDateRight.Controls.Add(rdoAllPeriod, 2, 2);
            tlpDateRight.Controls.Add(lblAmountOfDay, 1, 2);
            tlpDateRight.Controls.Add(lblAllPeriod, 0, 2);
            tlpDateRight.Dock = DockStyle.Fill;
            tlpDateRight.Location = new Point(3, 3);
            tlpDateRight.Name = "tlpDateRight";
            tlpDateRight.RowCount = 3;
            tlpDateRight.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tlpDateRight.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tlpDateRight.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tlpDateRight.Size = new Size(974, 100);
            tlpDateRight.TabIndex = 25;
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
            btnPrint.Font = new Font("Times New Roman", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnPrint.Location = new Point(723, 5);
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new Size(348, 46);
            btnPrint.TabIndex = 1;
            btnPrint.Text = "طباعة";
            btnPrint.UseVisualStyleBackColor = true;
            btnPrint.Click += btnPrint_Click;
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
            tlpButtons.Controls.Add(btnSaveAndClose, 4, 0);
            tlpButtons.Controls.Add(btnPrint, 0, 0);
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
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 18.4818478F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 61.38614F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.Size = new Size(1082, 606);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 2;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 9F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 91F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel5.Controls.Add(btnAllPeriod, 0, 0);
            tableLayoutPanel5.Controls.Add(tlpDateRight, 1, 0);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(3, 63);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 1;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel5.Size = new Size(1076, 106);
            tableLayoutPanel5.TabIndex = 30;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(3, 175);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.Size = new Size(1076, 366);
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
        private Button btnSaveAndClose;
        private Button btnPrint;
        private TableLayoutPanel tlpButtons;
        private TableLayoutPanel tableLayoutPanel1;
        private Label lblTitel;
        private TableLayoutPanel tableLayoutPanel5;
        private DataGridView dataGridView1;
    }
}