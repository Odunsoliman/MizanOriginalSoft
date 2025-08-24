namespace MizanOriginalSoft.Views.Forms.MainForms
{
    partial class frmOriginalSearch
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
            DGV = new DataGridView();
            lblTitel = new Label();
            lblcountResulte = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            label1 = new Label();
            lblID = new Label();
            txtSearch = new TextBox();
            ((System.ComponentModel.ISupportInitialize)DGV).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // DGV
            // 
            DGV.AllowUserToAddRows = false;
            DGV.AllowUserToDeleteRows = false;
            DGV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV.Dock = DockStyle.Fill;
            DGV.Location = new Point(5, 119);
            DGV.Margin = new Padding(5, 4, 5, 4);
            DGV.Name = "DGV";
            DGV.ReadOnly = true;
            DGV.Size = new Size(927, 481);
            DGV.TabIndex = 0;
            DGV.CellDoubleClick += DGV_CellDoubleClick;
            DGV.SelectionChanged += DGV_SelectionChanged;
            DGV.DoubleClick += DGV_DoubleClick;
            // 
            // lblTitel
            // 
            lblTitel.AutoSize = true;
            lblTitel.Dock = DockStyle.Fill;
            lblTitel.Location = new Point(5, 0);
            lblTitel.Margin = new Padding(5, 0, 5, 0);
            lblTitel.Name = "lblTitel";
            lblTitel.Size = new Size(927, 68);
            lblTitel.TabIndex = 1;
            lblTitel.Text = "label1";
            lblTitel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblcountResulte
            // 
            lblcountResulte.AutoSize = true;
            lblcountResulte.Location = new Point(873, 604);
            lblcountResulte.Margin = new Padding(5, 0, 5, 0);
            lblcountResulte.Name = "lblcountResulte";
            lblcountResulte.Size = new Size(59, 22);
            lblcountResulte.TabIndex = 2;
            lblcountResulte.Text = "label2";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(lblTitel, 0, 0);
            tableLayoutPanel1.Controls.Add(lblcountResulte, 0, 3);
            tableLayoutPanel1.Controls.Add(DGV, 0, 2);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(5, 4, 5, 4);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10.3092785F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 7.216495F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 74.22681F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 8.247422F));
            tableLayoutPanel1.Size = new Size(937, 660);
            tableLayoutPanel1.TabIndex = 4;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(label1, 0, 0);
            tableLayoutPanel2.Controls.Add(lblID, 2, 0);
            tableLayoutPanel2.Controls.Add(txtSearch, 1, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(5, 72);
            tableLayoutPanel2.Margin = new Padding(5, 4, 5, 4);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(927, 39);
            tableLayoutPanel2.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(840, 0);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(82, 39);
            label1.TabIndex = 5;
            label1.Text = "بحث";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblID
            // 
            lblID.AutoSize = true;
            lblID.Location = new Point(401, 0);
            lblID.Margin = new Padding(5, 0, 5, 0);
            lblID.Name = "lblID";
            lblID.Size = new Size(59, 22);
            lblID.TabIndex = 2;
            lblID.Text = "label2";
            // 
            // txtSearch
            // 
            txtSearch.Dock = DockStyle.Fill;
            txtSearch.Location = new Point(470, 4);
            txtSearch.Margin = new Padding(5, 4, 5, 4);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(360, 29);
            txtSearch.TabIndex = 4;
            txtSearch.TextChanged += txtSearch_TextChanged;
            // 
            // frmOriginalSearch
            // 
            AutoScaleDimensions = new SizeF(11F, 22F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(937, 660);
            Controls.Add(tableLayoutPanel1);
            Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Margin = new Padding(5, 4, 5, 4);
            Name = "frmOriginalSearch";
            RightToLeft = RightToLeft.Yes;
            Text = "frmOriginalSearch";
            Load += frmOriginalSearch_Load;
            ((System.ComponentModel.ISupportInitialize)DGV).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView DGV;
        private Label lblTitel;
        private Label lblcountResulte;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private Label label1;
        private TextBox txtSearch;
        private Label lblID;
    }
}