namespace MizanOriginalSoft.Views.Forms.Products
{
    partial class frmImageViewer
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
            tableLayoutPanel2 = new TableLayoutPanel();
            pictureBoxLarge = new PictureBox();
            tableLayoutPanel3 = new TableLayoutPanel();
            lblCategory = new Label();
            lblSuplierID = new Label();
            lblStock = new Label();
            lblNote = new Label();
            lblRegistYear = new Label();
            tableLayoutPanel4 = new TableLayoutPanel();
            lblUPrice = new Label();
            lblProdName = new Label();
            lblProductCode = new Label();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLarge).BeginInit();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 1);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 0, 2);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel4, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Font = new Font("Times New Roman", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.Size = new Size(800, 561);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(pictureBoxLarge, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 59);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.Padding = new Padding(30);
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(794, 442);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // pictureBoxLarge
            // 
            pictureBoxLarge.BackColor = Color.White;
            pictureBoxLarge.Dock = DockStyle.Fill;
            pictureBoxLarge.Location = new Point(33, 33);
            pictureBoxLarge.Name = "pictureBoxLarge";
            pictureBoxLarge.Size = new Size(728, 376);
            pictureBoxLarge.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxLarge.TabIndex = 0;
            pictureBoxLarge.TabStop = false;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 5;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 26.4483624F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 26.3224182F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17.6322422F));
            tableLayoutPanel3.Controls.Add(lblCategory, 3, 0);
            tableLayoutPanel3.Controls.Add(lblSuplierID, 0, 0);
            tableLayoutPanel3.Controls.Add(lblStock, 1, 0);
            tableLayoutPanel3.Controls.Add(lblNote, 2, 0);
            tableLayoutPanel3.Controls.Add(lblRegistYear, 4, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 507);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(794, 51);
            tableLayoutPanel3.TabIndex = 2;
            // 
            // lblCategory
            // 
            lblCategory.Dock = DockStyle.Fill;
            lblCategory.Location = new Point(144, 0);
            lblCategory.Name = "lblCategory";
            lblCategory.Size = new Size(202, 51);
            lblCategory.TabIndex = 9;
            lblCategory.Text = "Category";
            lblCategory.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblSuplierID
            // 
            lblSuplierID.Dock = DockStyle.Fill;
            lblSuplierID.Location = new Point(758, 0);
            lblSuplierID.Name = "lblSuplierID";
            lblSuplierID.Size = new Size(33, 51);
            lblSuplierID.TabIndex = 3;
            lblSuplierID.Text = "SuplierID";
            lblSuplierID.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblStock
            // 
            lblStock.Dock = DockStyle.Fill;
            lblStock.Location = new Point(561, 0);
            lblStock.Name = "lblStock";
            lblStock.Size = new Size(191, 51);
            lblStock.TabIndex = 2;
            lblStock.Text = "ProductStock";
            lblStock.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblNote
            // 
            lblNote.Dock = DockStyle.Fill;
            lblNote.Location = new Point(352, 0);
            lblNote.Name = "lblNote";
            lblNote.Size = new Size(203, 51);
            lblNote.TabIndex = 8;
            lblNote.Text = "Note";
            lblNote.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblRegistYear
            // 
            lblRegistYear.Dock = DockStyle.Fill;
            lblRegistYear.Location = new Point(3, 0);
            lblRegistYear.Name = "lblRegistYear";
            lblRegistYear.Size = new Size(135, 51);
            lblRegistYear.TabIndex = 7;
            lblRegistYear.Text = "RegistYear";
            lblRegistYear.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 3;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55.9193954F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 21.15869F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22.795969F));
            tableLayoutPanel4.Controls.Add(lblUPrice, 1, 0);
            tableLayoutPanel4.Controls.Add(lblProdName, 0, 0);
            tableLayoutPanel4.Controls.Add(lblProductCode, 2, 0);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(3, 3);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.Size = new Size(794, 50);
            tableLayoutPanel4.TabIndex = 3;
            // 
            // lblUPrice
            // 
            lblUPrice.Dock = DockStyle.Fill;
            lblUPrice.Location = new Point(185, 0);
            lblUPrice.Name = "lblUPrice";
            lblUPrice.Size = new Size(162, 50);
            lblUPrice.TabIndex = 11;
            lblUPrice.Text = "label1";
            lblUPrice.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblProdName
            // 
            lblProdName.Dock = DockStyle.Fill;
            lblProdName.Location = new Point(353, 0);
            lblProdName.Name = "lblProdName";
            lblProdName.Size = new Size(438, 50);
            lblProdName.TabIndex = 10;
            lblProdName.Text = "label1";
            lblProdName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblProductCode
            // 
            lblProductCode.Dock = DockStyle.Fill;
            lblProductCode.Location = new Point(3, 0);
            lblProductCode.Name = "lblProductCode";
            lblProductCode.Size = new Size(176, 50);
            lblProductCode.TabIndex = 6;
            lblProductCode.Text = "ProductCode";
            lblProductCode.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // frmImageViewer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 561);
            Controls.Add(tableLayoutPanel1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmImageViewer";
            RightToLeft = RightToLeft.Yes;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "frmImageViewer";
            Load += frmImageViewer_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxLarge).EndInit();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private PictureBox pictureBoxLarge;
        private TableLayoutPanel tableLayoutPanel3;
        private Label lblSuplierID;
        private Label lblStock;
        private Label lblNote;
        private Label lblRegistYear;
        private Label lblProductCode;
        private TableLayoutPanel tableLayoutPanel4;
        private Label lblUPrice;
        private Label lblProdName;
        private Label lblCategory;
    }
}