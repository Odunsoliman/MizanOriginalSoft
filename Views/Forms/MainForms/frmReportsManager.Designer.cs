namespace MizanOriginalSoft.Views.Forms.MainForms
{
    partial class frmReportsManager
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
            lblTitle = new Label();
            tableLayoutPanel3 = new TableLayoutPanel();
            btnNew = new Button();
            btnDelete = new Button();
            label1 = new Label();
            cbxID_TopAcc = new ComboBox();
            txtReportDisplayName = new TextBox();
            label2 = new Label();
            label3 = new Label();
            txtReportCodeName = new TextBox();
            rdoIsGrouped = new RadioButton();
            chkIsGrouped = new CheckBox();
            rdoIndividual = new RadioButton();
            chkIsActivRep = new CheckBox();
            btnSave = new Button();
            tableLayoutPanel2 = new TableLayoutPanel();
            DGV = new DataGridView();
            tableLayoutPanel4 = new TableLayoutPanel();
            btnUP = new Button();
            btnDown = new Button();
            tableLayoutPanel5 = new TableLayoutPanel();
            txtNotes = new TextBox();
            label4 = new Label();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV).BeginInit();
            tableLayoutPanel4.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(lblTitle, 0, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 0, 1);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 3);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel5, 0, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
            tableLayoutPanel1.Size = new Size(1084, 450);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.Location = new Point(3, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(1078, 67);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "ادارة التقارير    Reports Management";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 6;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 13F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 7F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tableLayoutPanel3.Controls.Add(btnNew, 5, 2);
            tableLayoutPanel3.Controls.Add(btnDelete, 5, 1);
            tableLayoutPanel3.Controls.Add(label1, 0, 0);
            tableLayoutPanel3.Controls.Add(cbxID_TopAcc, 1, 0);
            tableLayoutPanel3.Controls.Add(txtReportDisplayName, 1, 1);
            tableLayoutPanel3.Controls.Add(label2, 0, 1);
            tableLayoutPanel3.Controls.Add(label3, 0, 2);
            tableLayoutPanel3.Controls.Add(txtReportCodeName, 1, 2);
            tableLayoutPanel3.Controls.Add(rdoIsGrouped, 4, 2);
            tableLayoutPanel3.Controls.Add(chkIsGrouped, 2, 2);
            tableLayoutPanel3.Controls.Add(rdoIndividual, 3, 2);
            tableLayoutPanel3.Controls.Add(chkIsActivRep, 3, 1);
            tableLayoutPanel3.Controls.Add(btnSave, 5, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 70);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 3;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel3.Size = new Size(1078, 84);
            tableLayoutPanel3.TabIndex = 1;
            // 
            // btnNew
            // 
            btnNew.Dock = DockStyle.Fill;
            btnNew.Location = new Point(3, 59);
            btnNew.Name = "btnNew";
            btnNew.Size = new Size(104, 22);
            btnNew.TabIndex = 5;
            btnNew.Text = "جديد";
            btnNew.UseVisualStyleBackColor = true;
            btnNew.Click += btnNew_Click;
            // 
            // btnDelete
            // 
            btnDelete.Dock = DockStyle.Fill;
            btnDelete.Location = new Point(3, 31);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(104, 22);
            btnDelete.TabIndex = 6;
            btnDelete.Text = "حذف";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(920, 0);
            label1.Name = "label1";
            label1.Size = new Size(155, 28);
            label1.TabIndex = 1;
            label1.Text = "تخصص التقرير";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // cbxID_TopAcc
            // 
            cbxID_TopAcc.Dock = DockStyle.Fill;
            cbxID_TopAcc.FormattingEnabled = true;
            cbxID_TopAcc.Location = new Point(543, 3);
            cbxID_TopAcc.Name = "cbxID_TopAcc";
            cbxID_TopAcc.Size = new Size(371, 30);
            cbxID_TopAcc.TabIndex = 0;
            cbxID_TopAcc.SelectedIndexChanged += cbxID_TopAcc_SelectedIndexChanged;
            cbxID_TopAcc.SelectionChangeCommitted += cbxID_TopAcc_SelectionChangeCommitted;
            // 
            // txtReportDisplayName
            // 
            txtReportDisplayName.Dock = DockStyle.Fill;
            txtReportDisplayName.Location = new Point(543, 31);
            txtReportDisplayName.Name = "txtReportDisplayName";
            txtReportDisplayName.Size = new Size(371, 29);
            txtReportDisplayName.TabIndex = 3;
            txtReportDisplayName.Enter += txtReportDisplayName_Enter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(920, 28);
            label2.Name = "label2";
            label2.Size = new Size(155, 28);
            label2.TabIndex = 2;
            label2.Text = "اسم الظهور";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Location = new Point(920, 56);
            label3.Name = "label3";
            label3.Size = new Size(155, 28);
            label3.TabIndex = 2;
            label3.Text = "اسم برمجى";
            label3.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtReportCodeName
            // 
            txtReportCodeName.Dock = DockStyle.Fill;
            txtReportCodeName.Location = new Point(543, 59);
            txtReportCodeName.Name = "txtReportCodeName";
            txtReportCodeName.Size = new Size(371, 29);
            txtReportCodeName.TabIndex = 3;
            txtReportCodeName.TextAlign = HorizontalAlignment.Right;
            txtReportCodeName.Enter += txtReportCodeName_Enter;
            // 
            // rdoIsGrouped
            // 
            rdoIsGrouped.AutoSize = true;
            rdoIsGrouped.Location = new Point(259, 59);
            rdoIsGrouped.Name = "rdoIsGrouped";
            rdoIsGrouped.Size = new Size(63, 22);
            rdoIsGrouped.TabIndex = 6;
            rdoIsGrouped.Text = "مجمع";
            rdoIsGrouped.UseVisualStyleBackColor = true;
            rdoIsGrouped.CheckedChanged += rdoIsGrouped_CheckedChanged;
            // 
            // chkIsGrouped
            // 
            chkIsGrouped.AutoSize = true;
            chkIsGrouped.Location = new Point(435, 59);
            chkIsGrouped.Name = "chkIsGrouped";
            chkIsGrouped.Size = new Size(102, 22);
            chkIsGrouped.TabIndex = 5;
            chkIsGrouped.Text = "تقرير جمعى";
            chkIsGrouped.UseVisualStyleBackColor = true;
            chkIsGrouped.Visible = false;
            // 
            // rdoIndividual
            // 
            rdoIndividual.AutoSize = true;
            rdoIndividual.Checked = true;
            rdoIndividual.Location = new Point(336, 59);
            rdoIndividual.Name = "rdoIndividual";
            rdoIndividual.Size = new Size(61, 22);
            rdoIndividual.TabIndex = 6;
            rdoIndividual.TabStop = true;
            rdoIndividual.Text = "فردي";
            rdoIndividual.UseVisualStyleBackColor = true;
            rdoIndividual.CheckedChanged += rdoIndividual_CheckedChanged;
            // 
            // chkIsActivRep
            // 
            chkIsActivRep.AutoSize = true;
            chkIsActivRep.Location = new Point(342, 31);
            chkIsActivRep.Name = "chkIsActivRep";
            chkIsActivRep.Size = new Size(55, 22);
            chkIsActivRep.TabIndex = 4;
            chkIsActivRep.Text = "فعال";
            chkIsActivRep.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Dock = DockStyle.Fill;
            btnSave.Location = new Point(3, 3);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(104, 22);
            btnSave.TabIndex = 4;
            btnSave.Text = "حفظ";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 90F));
            tableLayoutPanel2.Controls.Add(DGV, 1, 0);
            tableLayoutPanel2.Controls.Add(tableLayoutPanel4, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 205);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(1078, 242);
            tableLayoutPanel2.TabIndex = 1;
            // 
            // DGV
            // 
            DGV.AllowUserToAddRows = false;
            DGV.AllowUserToDeleteRows = false;
            DGV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV.Dock = DockStyle.Fill;
            DGV.Location = new Point(3, 3);
            DGV.Name = "DGV";
            DGV.ReadOnly = true;
            DGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGV.Size = new Size(965, 236);
            DGV.TabIndex = 0;
            DGV.SelectionChanged += DGV_SelectionChanged;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 37F));
            tableLayoutPanel4.Controls.Add(btnUP, 1, 1);
            tableLayoutPanel4.Controls.Add(btnDown, 1, 2);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(974, 3);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 4;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            tableLayoutPanel4.Size = new Size(101, 236);
            tableLayoutPanel4.TabIndex = 0;
            // 
            // btnUP
            // 
            btnUP.Dock = DockStyle.Fill;
            btnUP.Location = new Point(3, 50);
            btnUP.Name = "btnUP";
            btnUP.Size = new Size(31, 41);
            btnUP.TabIndex = 5;
            btnUP.Text = "▲";
            btnUP.UseVisualStyleBackColor = true;
            btnUP.Click += btnUP_Click;
            // 
            // btnDown
            // 
            btnDown.Dock = DockStyle.Fill;
            btnDown.Location = new Point(3, 97);
            btnDown.Name = "btnDown";
            btnDown.Size = new Size(31, 41);
            btnDown.TabIndex = 5;
            btnDown.Text = "▼";
            btnDown.UseVisualStyleBackColor = true;
            btnDown.Click += btnDown_Click;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 2;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 85F));
            tableLayoutPanel5.Controls.Add(txtNotes, 1, 0);
            tableLayoutPanel5.Controls.Add(label4, 0, 0);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(3, 160);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 1;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel5.Size = new Size(1078, 39);
            tableLayoutPanel5.TabIndex = 1;
            // 
            // txtNotes
            // 
            txtNotes.Dock = DockStyle.Fill;
            txtNotes.Location = new Point(3, 3);
            txtNotes.Name = "txtNotes";
            txtNotes.Size = new Size(911, 29);
            txtNotes.TabIndex = 6;
            txtNotes.Enter += txtNotes_Enter;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Fill;
            label4.Location = new Point(920, 0);
            label4.Name = "label4";
            label4.Size = new Size(155, 39);
            label4.TabIndex = 2;
            label4.Text = "ملاحظات عامة";
            label4.TextAlign = ContentAlignment.MiddleRight;
            // 
            // frmReportsManager
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1084, 450);
            Controls.Add(tableLayoutPanel1);
            Name = "frmReportsManager";
            RightToLeft = RightToLeft.Yes;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "frmReportsManager";
            Load += frmReportsManager_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DGV).EndInit();
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel5.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Label lblTitle;
        private TableLayoutPanel tableLayoutPanel2;
        private DataGridView DGV;
        private TableLayoutPanel tableLayoutPanel3;
        private Label label2;
        private ComboBox cbxID_TopAcc;
        private Label label1;
        private TextBox txtReportDisplayName;
   //     private TableLayoutPanel tableLayoutPanel4;
        private Button btnSave;
        private TextBox txtReportCodeName;
        private Label label3;
        private CheckBox chkIsActivRep;
        private Button btnNew;
        private CheckBox chkIsGrouped;
        private TextBox txtNotes;
        private TableLayoutPanel tableLayoutPanel5;
        private Label label4;
        private RadioButton rdoIsGrouped;
        private RadioButton rdoIndividual;
        private Button btnDelete;
        private TableLayoutPanel tableLayoutPanel4;
        private Button btnUP;
        private Button btnDown;
    }
}