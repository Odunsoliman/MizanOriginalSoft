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
            tableLayoutPanel5 = new TableLayoutPanel();
            btnNext = new Button();
            chkIsDefault = new CheckBox();
            btnDeletePhoto = new Button();
            tableLayoutPanel3 = new TableLayoutPanel();
            lblCategory = new Label();
            lblSuplierID = new Label();
            lblNote = new Label();
            lblRegistYear = new Label();
            tableLayoutPanel4 = new TableLayoutPanel();
            lblProductCode = new Label();
            lblUPrice = new Label();
            lblStock = new Label();
            lblProdName = new Label();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLarge).BeginInit();
            tableLayoutPanel5.SuspendLayout();
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
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(pictureBoxLarge, 0, 0);
            tableLayoutPanel2.Controls.Add(tableLayoutPanel5, 0, 1);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 59);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.Padding = new Padding(30, 30, 30, 5);
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 88F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 12F));
            tableLayoutPanel2.Size = new Size(794, 442);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // pictureBoxLarge
            // 
            pictureBoxLarge.BackColor = Color.White;
            pictureBoxLarge.Dock = DockStyle.Fill;
            pictureBoxLarge.Location = new Point(33, 33);
            pictureBoxLarge.Name = "pictureBoxLarge";
            pictureBoxLarge.Size = new Size(728, 352);
            pictureBoxLarge.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxLarge.TabIndex = 0;
            pictureBoxLarge.TabStop = false;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 3;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel5.Controls.Add(btnNext, 0, 0);
            tableLayoutPanel5.Controls.Add(chkIsDefault, 1, 0);
            tableLayoutPanel5.Controls.Add(btnDeletePhoto, 2, 0);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(33, 391);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 1;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel5.Size = new Size(728, 43);
            tableLayoutPanel5.TabIndex = 1;
            // 
            // btnNext
            // 
            btnNext.Dock = DockStyle.Fill;
            btnNext.Location = new Point(489, 3);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(236, 37);
            btnNext.TabIndex = 11;
            btnNext.Text = "<<< عرض الصور >>>";
            btnNext.UseVisualStyleBackColor = true;
            btnNext.Click += btnNext_Click;
            // 
            // chkIsDefault
            // 
            chkIsDefault.AutoSize = true;
            chkIsDefault.Dock = DockStyle.Fill;
            chkIsDefault.Location = new Point(247, 3);
            chkIsDefault.Name = "chkIsDefault";
            chkIsDefault.Size = new Size(236, 37);
            chkIsDefault.TabIndex = 10;
            chkIsDefault.Text = "تعيين كافتراضية";
            chkIsDefault.UseVisualStyleBackColor = true;
            chkIsDefault.CheckedChanged += chkIsDefault_CheckedChanged;
            // 
            // btnDeletePhoto
            // 
            btnDeletePhoto.Dock = DockStyle.Fill;
            btnDeletePhoto.Location = new Point(3, 3);
            btnDeletePhoto.Name = "btnDeletePhoto";
            btnDeletePhoto.Size = new Size(238, 37);
            btnDeletePhoto.TabIndex = 11;
            btnDeletePhoto.Text = "حذف صورة";
            btnDeletePhoto.UseVisualStyleBackColor = true;
            btnDeletePhoto.Click += btnDeletePhoto_Click;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 4;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel3.Controls.Add(lblCategory, 2, 0);
            tableLayoutPanel3.Controls.Add(lblSuplierID, 0, 0);
            tableLayoutPanel3.Controls.Add(lblNote, 1, 0);
            tableLayoutPanel3.Controls.Add(lblRegistYear, 3, 0);
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
            lblCategory.Location = new Point(164, 0);
            lblCategory.Name = "lblCategory";
            lblCategory.Size = new Size(152, 51);
            lblCategory.TabIndex = 9;
            lblCategory.Text = "Category";
            lblCategory.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblSuplierID
            // 
            lblSuplierID.Dock = DockStyle.Fill;
            lblSuplierID.Location = new Point(639, 0);
            lblSuplierID.Name = "lblSuplierID";
            lblSuplierID.Size = new Size(152, 51);
            lblSuplierID.TabIndex = 3;
            lblSuplierID.Text = "SuplierID";
            lblSuplierID.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblNote
            // 
            lblNote.Dock = DockStyle.Fill;
            lblNote.Location = new Point(322, 0);
            lblNote.Name = "lblNote";
            lblNote.Size = new Size(311, 51);
            lblNote.TabIndex = 8;
            lblNote.Text = "Note";
            lblNote.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblRegistYear
            // 
            lblRegistYear.Dock = DockStyle.Fill;
            lblRegistYear.Location = new Point(3, 0);
            lblRegistYear.Name = "lblRegistYear";
            lblRegistYear.Size = new Size(155, 51);
            lblRegistYear.TabIndex = 7;
            lblRegistYear.Text = "RegistYear";
            lblRegistYear.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 4;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel4.Controls.Add(lblProductCode, 0, 0);
            tableLayoutPanel4.Controls.Add(lblUPrice, 3, 0);
            tableLayoutPanel4.Controls.Add(lblStock, 2, 0);
            tableLayoutPanel4.Controls.Add(lblProdName, 1, 0);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(3, 3);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.Size = new Size(794, 50);
            tableLayoutPanel4.TabIndex = 3;
            // 
            // lblProductCode
            // 
            lblProductCode.Dock = DockStyle.Fill;
            lblProductCode.Location = new Point(678, 0);
            lblProductCode.Name = "lblProductCode";
            lblProductCode.Size = new Size(113, 50);
            lblProductCode.TabIndex = 6;
            lblProductCode.Text = "ProductCode";
            lblProductCode.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblUPrice
            // 
            lblUPrice.Dock = DockStyle.Fill;
            lblUPrice.Location = new Point(3, 0);
            lblUPrice.Name = "lblUPrice";
            lblUPrice.Size = new Size(194, 50);
            lblUPrice.TabIndex = 11;
            lblUPrice.Text = "label1";
            lblUPrice.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblStock
            // 
            lblStock.Dock = DockStyle.Fill;
            lblStock.Location = new Point(203, 0);
            lblStock.Name = "lblStock";
            lblStock.Size = new Size(192, 50);
            lblStock.TabIndex = 2;
            lblStock.Text = "ProductStock";
            lblStock.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblProdName
            // 
            lblProdName.Dock = DockStyle.Fill;
            lblProdName.Location = new Point(401, 0);
            lblProdName.Name = "lblProdName";
            lblProdName.Size = new Size(271, 50);
            lblProdName.TabIndex = 10;
            lblProdName.Text = "label1";
            lblProdName.TextAlign = ContentAlignment.MiddleCenter;
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
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel5.PerformLayout();
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
        private TableLayoutPanel tableLayoutPanel5;
        private Button btnNext;
        private CheckBox chkIsDefault;
        private Button btnDeletePhoto;
    }
}