namespace MizanOriginalSoft.Views.Forms.Products
{
    partial class frmReport_Preview
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
            tableLayoutPanel1 = new TableLayoutPanel();
            lblTitel = new Label();
            tlpButtons = new TableLayoutPanel();
            btnPrintPreview = new Button();
            btnSaveAndClose = new Button();
            btnPrint = new Button();
            tlpDateRight = new TableLayoutPanel();
            rdoPreviousYear = new RadioButton();
            rdoPreviousDay = new RadioButton();
            rdoThisYear = new RadioButton();
            rdoToDay = new RadioButton();
            rdoPreviousMonth = new RadioButton();
            rdoThisMonth = new RadioButton();
            rdoAllPeriod = new RadioButton();
            tableLayoutPanel2 = new TableLayoutPanel();
            tableLayoutPanel3 = new TableLayoutPanel();
            label5 = new Label();
            cbxPrinters = new ComboBox();
            label2 = new Label();
            cbxWarehouse = new ComboBox();
            tableLayoutPanel4 = new TableLayoutPanel();
            btnAllPeriod = new Button();
            tlpDate2 = new TableLayoutPanel();
            lblAmountOfDay = new Label();
            lblAllPeriod = new Label();
            dtpEnd = new DateTimePicker();
            lbl = new Label();
            dtpStart = new DateTimePicker();
            label1 = new Label();
            lblReportName = new Label();
            tableLayoutPanel1.SuspendLayout();
            tlpButtons.SuspendLayout();
            tlpDateRight.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tlpDate2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(lblReportName, 0, 0);
            tableLayoutPanel1.Controls.Add(lblTitel, 0, 2);
            tableLayoutPanel1.Controls.Add(tlpButtons, 0, 7);
            tableLayoutPanel1.Controls.Add(tlpDateRight, 0, 5);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 1);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel4, 0, 4);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(10, 9);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 8;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 13F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 2F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 28F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 2F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanel1.Size = new Size(691, 485);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // lblTitel
            // 
            lblTitel.AutoSize = true;
            lblTitel.BackColor = Color.LightGray;
            lblTitel.Dock = DockStyle.Fill;
            lblTitel.Font = new Font("Times New Roman", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitel.ForeColor = Color.Purple;
            lblTitel.Location = new Point(3, 96);
            lblTitel.Name = "lblTitel";
            lblTitel.Size = new Size(685, 63);
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
            tlpButtons.Controls.Add(btnPrintPreview, 0, 0);
            tlpButtons.Controls.Add(btnSaveAndClose, 4, 0);
            tlpButtons.Controls.Add(btnPrint, 2, 0);
            tlpButtons.Dock = DockStyle.Fill;
            tlpButtons.Location = new Point(3, 412);
            tlpButtons.Name = "tlpButtons";
            tlpButtons.Padding = new Padding(2);
            tlpButtons.RowCount = 1;
            tlpButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpButtons.Size = new Size(685, 70);
            tlpButtons.TabIndex = 27;
            // 
            // btnPrintPreview
            // 
            btnPrintPreview.Dock = DockStyle.Fill;
            btnPrintPreview.Font = new Font("Times New Roman", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnPrintPreview.Location = new Point(462, 5);
            btnPrintPreview.Name = "btnPrintPreview";
            btnPrintPreview.Size = new Size(218, 60);
            btnPrintPreview.TabIndex = 1;
            btnPrintPreview.Text = "معاينة";
            btnPrintPreview.UseVisualStyleBackColor = true;
            btnPrintPreview.Click += btnPrintPreview_Click;
            // 
            // btnSaveAndClose
            // 
            btnSaveAndClose.Dock = DockStyle.Fill;
            btnSaveAndClose.Font = new Font("Times New Roman", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSaveAndClose.Location = new Point(5, 5);
            btnSaveAndClose.Name = "btnSaveAndClose";
            btnSaveAndClose.Size = new Size(219, 60);
            btnSaveAndClose.TabIndex = 1;
            btnSaveAndClose.Text = "خروج";
            btnSaveAndClose.UseVisualStyleBackColor = true;
            btnSaveAndClose.Click += btnSaveAndClose_Click;
            // 
            // btnPrint
            // 
            btnPrint.Dock = DockStyle.Fill;
            btnPrint.Font = new Font("Times New Roman", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnPrint.Location = new Point(234, 5);
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new Size(218, 60);
            btnPrint.TabIndex = 1;
            btnPrint.Text = "طباعة";
            btnPrint.UseVisualStyleBackColor = true;
            btnPrint.Click += btnPrint_Click;
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
            tlpDateRight.Location = new Point(3, 268);
            tlpDateRight.Name = "tlpDateRight";
            tlpDateRight.RowCount = 3;
            tlpDateRight.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tlpDateRight.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tlpDateRight.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tlpDateRight.Size = new Size(685, 129);
            tlpDateRight.TabIndex = 25;
            // 
            // rdoPreviousYear
            // 
            rdoPreviousYear.AutoSize = true;
            rdoPreviousYear.Font = new Font("Times New Roman", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            rdoPreviousYear.ForeColor = Color.Blue;
            rdoPreviousYear.Location = new Point(235, 89);
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
            rdoPreviousDay.Location = new Point(235, 3);
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
            rdoThisYear.Location = new Point(421, 89);
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
            rdoToDay.Location = new Point(443, 3);
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
            rdoPreviousMonth.Location = new Point(227, 46);
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
            rdoThisMonth.Location = new Point(420, 46);
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
            rdoAllPeriod.Location = new Point(599, 89);
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
            tableLayoutPanel2.Location = new Point(3, 51);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(685, 42);
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
            tableLayoutPanel3.Size = new Size(679, 36);
            tableLayoutPanel3.TabIndex = 26;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Fill;
            label5.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(270, 0);
            label5.Name = "label5";
            label5.Size = new Size(68, 36);
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
            cbxPrinters.Size = new Size(261, 27);
            cbxPrinters.TabIndex = 16;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(608, 0);
            label2.Name = "label2";
            label2.Size = new Size(68, 36);
            label2.TabIndex = 14;
            label2.Text = "الفرع";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // cbxWarehouse
            // 
            cbxWarehouse.BackColor = Color.Khaki;
            cbxWarehouse.Dock = DockStyle.Fill;
            cbxWarehouse.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            cbxWarehouse.FormattingEnabled = true;
            cbxWarehouse.Location = new Point(344, 3);
            cbxWarehouse.Name = "cbxWarehouse";
            cbxWarehouse.Size = new Size(258, 27);
            cbxWarehouse.TabIndex = 16;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 19.26952F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80.73048F));
            tableLayoutPanel4.Controls.Add(btnAllPeriod, 0, 0);
            tableLayoutPanel4.Controls.Add(tlpDate2, 1, 0);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(3, 171);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Size = new Size(685, 91);
            tableLayoutPanel4.TabIndex = 29;
            // 
            // btnAllPeriod
            // 
            btnAllPeriod.Dock = DockStyle.Fill;
            btnAllPeriod.Font = new Font("Times New Roman", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAllPeriod.Location = new Point(557, 3);
            btnAllPeriod.Name = "btnAllPeriod";
            btnAllPeriod.Size = new Size(125, 85);
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
            tlpDate2.Size = new Size(548, 85);
            tlpDate2.TabIndex = 8;
            // 
            // lblAmountOfDay
            // 
            lblAmountOfDay.AutoSize = true;
            lblAmountOfDay.Dock = DockStyle.Fill;
            lblAmountOfDay.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAmountOfDay.ForeColor = Color.Blue;
            lblAmountOfDay.Location = new Point(3, 42);
            lblAmountOfDay.Name = "lblAmountOfDay";
            lblAmountOfDay.Size = new Size(202, 43);
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
            lblAllPeriod.Size = new Size(202, 42);
            lblAllPeriod.TabIndex = 14;
            lblAllPeriod.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dtpEnd
            // 
            dtpEnd.CustomFormat = "yyyy   MM /d";
            dtpEnd.Dock = DockStyle.Fill;
            dtpEnd.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dtpEnd.Format = DateTimePickerFormat.Custom;
            dtpEnd.Location = new Point(211, 45);
            dtpEnd.Name = "dtpEnd";
            dtpEnd.RightToLeftLayout = true;
            dtpEnd.Size = new Size(264, 29);
            dtpEnd.TabIndex = 8;
            // 
            // lbl
            // 
            lbl.AutoSize = true;
            lbl.Dock = DockStyle.Fill;
            lbl.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbl.ForeColor = Color.Blue;
            lbl.Location = new Point(481, 42);
            lbl.Name = "lbl";
            lbl.Size = new Size(64, 43);
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
            dtpStart.Location = new Point(211, 3);
            dtpStart.Name = "dtpStart";
            dtpStart.RightToLeftLayout = true;
            dtpStart.Size = new Size(264, 29);
            dtpStart.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Blue;
            label1.Location = new Point(481, 0);
            label1.Name = "label1";
            label1.Size = new Size(64, 42);
            label1.TabIndex = 5;
            label1.Text = "من";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblReportName
            // 
            lblReportName.AutoSize = true;
            lblReportName.BackColor = Color.LightGray;
            lblReportName.Dock = DockStyle.Fill;
            lblReportName.Font = new Font("Times New Roman", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblReportName.ForeColor = Color.Purple;
            lblReportName.Location = new Point(3, 0);
            lblReportName.Name = "lblReportName";
            lblReportName.Size = new Size(685, 48);
            lblReportName.TabIndex = 30;
            lblReportName.Text = "برجاء تحديد عوامل فترة التقرير";
            lblReportName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // frmReport_Preview
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(711, 503);
            ControlBox = false;
            Controls.Add(tableLayoutPanel1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmReport_Preview";
            Padding = new Padding(10, 9, 10, 9);
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "frmReport_Preview";
            Load += frmReport_Preview_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tlpButtons.ResumeLayout(false);
            tlpDateRight.ResumeLayout(false);
            tlpDateRight.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            tableLayoutPanel4.ResumeLayout(false);
            tlpDate2.ResumeLayout(false);
            tlpDate2.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tlpDate2;
        private System.Windows.Forms.Label lblAmountOfDay;
        private System.Windows.Forms.Label lblAllPeriod;
        private System.Windows.Forms.DateTimePicker dtpEnd;
        private System.Windows.Forms.Label lbl;
        private System.Windows.Forms.DateTimePicker dtpStart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tlpDateRight;
        private System.Windows.Forms.RadioButton rdoToDay;
        private System.Windows.Forms.RadioButton rdoAllPeriod;
        private System.Windows.Forms.RadioButton rdoThisYear;
        private System.Windows.Forms.RadioButton rdoPreviousMonth;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tlpButtons;
        private System.Windows.Forms.Button btnPrintPreview;
        private System.Windows.Forms.Button btnSaveAndClose;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.RadioButton rdoPreviousYear;
        private System.Windows.Forms.RadioButton rdoThisMonth;
        private System.Windows.Forms.RadioButton rdoPreviousDay;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbxPrinters;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbxWarehouse;
        private System.Windows.Forms.Label lblTitel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Button btnAllPeriod;
        private Label lblReportName;
    }
}