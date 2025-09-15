namespace MizanOriginalSoft.Views.Forms.MainForms
{
    partial class frmGeneralSearch
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
            DGV = new DataGridView();
            tableLayoutPanel2 = new TableLayoutPanel();
            lblcountResulte = new Label();
            txtSearch = new TextBox();
            label1 = new Label();
            btnClearFilters = new Button();
            tlpDate = new TableLayoutPanel();
            dtpFrom = new DateTimePicker();
            dtpTo = new DateTimePicker();
            cbxUsers = new ComboBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            btnClose = new Button();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV).BeginInit();
            tableLayoutPanel2.SuspendLayout();
            tlpDate.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(lblTitel, 0, 0);
            tableLayoutPanel1.Controls.Add(DGV, 0, 3);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 2);
            tableLayoutPanel1.Controls.Add(tlpDate, 0, 1);
            tableLayoutPanel1.Controls.Add(btnClose, 0, 4);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(10);
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 9.090909F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 9.090909F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 9.090909F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 63.636364F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 9.090909F));
            tableLayoutPanel1.Size = new Size(882, 450);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // lblTitel
            // 
            lblTitel.AutoSize = true;
            lblTitel.Dock = DockStyle.Fill;
            lblTitel.Font = new Font("Times New Roman", 16F, FontStyle.Bold);
            lblTitel.ForeColor = Color.FromArgb(0, 0, 192);
            lblTitel.Location = new Point(13, 10);
            lblTitel.Name = "lblTitel";
            lblTitel.Size = new Size(856, 39);
            lblTitel.TabIndex = 1;
            lblTitel.Text = "label2";
            lblTitel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DGV
            // 
            DGV.AllowUserToAddRows = false;
            DGV.AllowUserToDeleteRows = false;
            DGV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV.Dock = DockStyle.Fill;
            DGV.Location = new Point(13, 130);
            DGV.Name = "DGV";
            DGV.ReadOnly = true;
            DGV.RowHeadersVisible = false;
            DGV.Size = new Size(856, 267);
            DGV.TabIndex = 2;
            DGV.CellDoubleClick += DGV_CellDoubleClick;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 4;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanel2.Controls.Add(lblcountResulte, 3, 0);
            tableLayoutPanel2.Controls.Add(txtSearch, 1, 0);
            tableLayoutPanel2.Controls.Add(label1, 0, 0);
            tableLayoutPanel2.Controls.Add(btnClearFilters, 2, 0);
            tableLayoutPanel2.Location = new Point(95, 91);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(774, 33);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // lblcountResulte
            // 
            lblcountResulte.AutoSize = true;
            lblcountResulte.Dock = DockStyle.Fill;
            lblcountResulte.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            lblcountResulte.ForeColor = Color.FromArgb(192, 0, 192);
            lblcountResulte.Location = new Point(3, 0);
            lblcountResulte.Name = "lblcountResulte";
            lblcountResulte.Size = new Size(305, 33);
            lblcountResulte.TabIndex = 3;
            lblcountResulte.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtSearch
            // 
            txtSearch.Dock = DockStyle.Fill;
            txtSearch.Font = new Font("Times New Roman", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtSearch.Location = new Point(353, 4);
            txtSearch.Margin = new Padding(4);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(340, 32);
            txtSearch.TabIndex = 2;
            txtSearch.TextAlign = HorizontalAlignment.Center;
            txtSearch.TextChanged += txtSearch_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            label1.ForeColor = Color.FromArgb(192, 0, 192);
            label1.Location = new Point(700, 0);
            label1.Name = "label1";
            label1.Size = new Size(71, 33);
            label1.TabIndex = 0;
            label1.Text = "بحث";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // btnClearFilters
            // 
            btnClearFilters.Dock = DockStyle.Fill;
            btnClearFilters.Location = new Point(314, 3);
            btnClearFilters.Name = "btnClearFilters";
            btnClearFilters.Size = new Size(32, 27);
            btnClearFilters.TabIndex = 4;
            btnClearFilters.Text = "x";
            btnClearFilters.UseVisualStyleBackColor = true;
            btnClearFilters.Click += btnClearFilters_Click;
            // 
            // tlpDate
            // 
            tlpDate.ColumnCount = 6;
            tlpDate.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 13F));
            tlpDate.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22F));
            tlpDate.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 13F));
            tlpDate.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22F));
            tlpDate.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 13F));
            tlpDate.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17F));
            tlpDate.Controls.Add(dtpFrom, 1, 0);
            tlpDate.Controls.Add(dtpTo, 3, 0);
            tlpDate.Controls.Add(cbxUsers, 5, 0);
            tlpDate.Controls.Add(label2, 0, 0);
            tlpDate.Controls.Add(label3, 2, 0);
            tlpDate.Controls.Add(label4, 4, 0);
            tlpDate.Dock = DockStyle.Fill;
            tlpDate.Location = new Point(13, 52);
            tlpDate.Name = "tlpDate";
            tlpDate.RowCount = 1;
            tlpDate.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpDate.Size = new Size(856, 33);
            tlpDate.TabIndex = 3;
            // 
            // dtpFrom
            // 
            dtpFrom.Dock = DockStyle.Fill;
            dtpFrom.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            dtpFrom.Location = new Point(560, 3);
            dtpFrom.Name = "dtpFrom";
            dtpFrom.Size = new Size(182, 26);
            dtpFrom.TabIndex = 0;
            dtpFrom.ValueChanged += dtpFrom_ValueChanged;
            // 
            // dtpTo
            // 
            dtpTo.Dock = DockStyle.Fill;
            dtpTo.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            dtpTo.Location = new Point(261, 3);
            dtpTo.Name = "dtpTo";
            dtpTo.Size = new Size(182, 26);
            dtpTo.TabIndex = 0;
            dtpTo.ValueChanged += dtpTo_ValueChanged;
            // 
            // cbxUsers
            // 
            cbxUsers.Dock = DockStyle.Fill;
            cbxUsers.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            cbxUsers.FormattingEnabled = true;
            cbxUsers.Location = new Point(3, 3);
            cbxUsers.Name = "cbxUsers";
            cbxUsers.Size = new Size(141, 27);
            cbxUsers.TabIndex = 1;
            cbxUsers.SelectedIndexChanged += cbxUsers_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            label2.Location = new Point(748, 0);
            label2.Name = "label2";
            label2.Size = new Size(105, 33);
            label2.TabIndex = 2;
            label2.Text = "تاريخ البداية : ";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            label3.Location = new Point(449, 0);
            label3.Name = "label3";
            label3.Size = new Size(105, 33);
            label3.TabIndex = 2;
            label3.Text = "تاريخ النهاية : ";
            label3.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Fill;
            label4.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            label4.Location = new Point(150, 0);
            label4.Name = "label4";
            label4.Size = new Size(105, 33);
            label4.TabIndex = 2;
            label4.Text = "محرر الفاتورة : ";
            label4.TextAlign = ContentAlignment.MiddleRight;
            // 
            // btnClose
            // 
            btnClose.Dock = DockStyle.Right;
            btnClose.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnClose.ForeColor = Color.Red;
            btnClose.Location = new Point(13, 403);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(75, 34);
            btnClose.TabIndex = 4;
            btnClose.Text = "خروج";
            btnClose.UseVisualStyleBackColor = true;
            // 
            // frmGeneralSearch
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(882, 450);
            ControlBox = false;
            Controls.Add(tableLayoutPanel1);
            Name = "frmGeneralSearch";
            RightToLeft = RightToLeft.Yes;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "البحث العام";
            Load += frmGeneralSearch_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DGV).EndInit();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tlpDate.ResumeLayout(false);
            tlpDate.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private Label lblTitel;
        private TextBox txtSearch;
        private Label label1;
        private DataGridView DGV;
        private Label lblcountResulte;
        private TableLayoutPanel tlpDate;
        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
        private ComboBox cbxUsers;
        private Label label2;
        private Label label3;
        private Label label4;
        private Button btnClearFilters;
        private Button btnClose;
    }
}