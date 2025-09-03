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
            tableLayoutPanel2 = new TableLayoutPanel();
            lblcountResulte = new Label();
            txtSearch = new TextBox();
            label1 = new Label();
            DGV = new DataGridView();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(lblTitel, 0, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 1);
            tableLayoutPanel1.Controls.Add(DGV, 0, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(10);
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
            tableLayoutPanel1.Size = new Size(800, 450);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // lblTitel
            // 
            lblTitel.AutoSize = true;
            lblTitel.Dock = DockStyle.Fill;
            lblTitel.Location = new Point(13, 10);
            lblTitel.Name = "lblTitel";
            lblTitel.Size = new Size(774, 43);
            lblTitel.TabIndex = 1;
            lblTitel.Text = "label2";
            lblTitel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            tableLayoutPanel2.Controls.Add(lblcountResulte, 2, 0);
            tableLayoutPanel2.Controls.Add(txtSearch, 1, 0);
            tableLayoutPanel2.Controls.Add(label1, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(13, 56);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(774, 37);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // lblcountResulte
            // 
            lblcountResulte.AutoSize = true;
            lblcountResulte.Dock = DockStyle.Fill;
            lblcountResulte.Location = new Point(3, 0);
            lblcountResulte.Name = "lblcountResulte";
            lblcountResulte.Size = new Size(343, 37);
            lblcountResulte.TabIndex = 3;
            lblcountResulte.Text = "بحث";
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
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(700, 0);
            label1.Name = "label1";
            label1.Size = new Size(71, 37);
            label1.TabIndex = 0;
            label1.Text = "بحث";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // DGV
            // 
            DGV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV.Dock = DockStyle.Fill;
            DGV.Location = new Point(13, 99);
            DGV.Name = "DGV";
            DGV.Size = new Size(774, 338);
            DGV.TabIndex = 2;
            // 
            // frmGeneralSearch
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tableLayoutPanel1);
            Name = "frmGeneralSearch";
            RightToLeft = RightToLeft.Yes;
            Text = "البحث العام";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DGV).EndInit();
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
    }
}