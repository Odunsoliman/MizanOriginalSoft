namespace MizanOriginalSoft.Views.Forms.Products
{
    partial class frm_ProductModify
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
            tlpNewProduct = new TableLayoutPanel();
            tableLayoutPanel12 = new TableLayoutPanel();
            tableLayoutPanel10 = new TableLayoutPanel();
            btnLoadPicProduct = new Button();
            tableLayoutPanel14 = new TableLayoutPanel();
            PicProduct = new PictureBox();
            lblPathProductPic = new Label();
            btnSave = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            btnHelp = new Button();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            lblCategory_id = new Label();
            lblSuppliers = new Label();
            tableLayoutPanel2 = new TableLayoutPanel();
            tableLayoutPanel7 = new TableLayoutPanel();
            lblID_Product = new Label();
            lblProductCode = new Label();
            tableLayoutPanel13 = new TableLayoutPanel();
            lblProductStock = new Label();
            label1 = new Label();
            txtProdName = new TextBox();
            txtNoteProduct = new TextBox();
            label13 = new Label();
            label19 = new Label();
            label18 = new Label();
            lblRegistYear = new Label();
            label10 = new Label();
            txtSuppliers = new TextBox();
            label8 = new Label();
            txtProdCodeOnSuplier = new TextBox();
            txtMinStock = new TextBox();
            txtMinLenth = new TextBox();
            cbxUnit_ID = new ComboBox();
            label16 = new Label();
            lblLinthText = new Label();
            lblMinLenth = new Label();
            labl = new Label();
            label9 = new Label();
            label14 = new Label();
            txtB_Price = new TextBox();
            txtU_Price = new TextBox();
            txtCategories = new TextBox();
            chkHiddinProd = new CheckBox();
            tlpNewProduct.SuspendLayout();
            tableLayoutPanel12.SuspendLayout();
            tableLayoutPanel10.SuspendLayout();
            tableLayoutPanel14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PicProduct).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel7.SuspendLayout();
            tableLayoutPanel13.SuspendLayout();
            SuspendLayout();
            // 
            // tlpNewProduct
            // 
            tlpNewProduct.ColumnCount = 2;
            tlpNewProduct.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpNewProduct.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpNewProduct.Controls.Add(tableLayoutPanel12, 1, 0);
            tlpNewProduct.Controls.Add(tableLayoutPanel2, 0, 0);
            tlpNewProduct.Dock = DockStyle.Fill;
            tlpNewProduct.Location = new Point(0, 0);
            tlpNewProduct.Name = "tlpNewProduct";
            tlpNewProduct.RowCount = 1;
            tlpNewProduct.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlpNewProduct.Size = new Size(1137, 548);
            tlpNewProduct.TabIndex = 14;
            // 
            // tableLayoutPanel12
            // 
            tableLayoutPanel12.ColumnCount = 1;
            tableLayoutPanel12.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel12.Controls.Add(tableLayoutPanel10, 0, 2);
            tableLayoutPanel12.Controls.Add(tableLayoutPanel14, 0, 1);
            tableLayoutPanel12.Controls.Add(lblPathProductPic, 0, 3);
            tableLayoutPanel12.Controls.Add(btnSave, 0, 4);
            tableLayoutPanel12.Controls.Add(tableLayoutPanel1, 0, 0);
            tableLayoutPanel12.Dock = DockStyle.Fill;
            tableLayoutPanel12.Location = new Point(3, 3);
            tableLayoutPanel12.Name = "tableLayoutPanel12";
            tableLayoutPanel12.Padding = new Padding(20, 0, 20, 19);
            tableLayoutPanel12.RowCount = 5;
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Percent, 8F));
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Percent, 11F));
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Percent, 11F));
            tableLayoutPanel12.Size = new Size(563, 542);
            tableLayoutPanel12.TabIndex = 13;
            // 
            // tableLayoutPanel10
            // 
            tableLayoutPanel10.BackColor = Color.Transparent;
            tableLayoutPanel10.ColumnCount = 1;
            tableLayoutPanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel10.Controls.Add(btnLoadPicProduct, 0, 0);
            tableLayoutPanel10.Dock = DockStyle.Fill;
            tableLayoutPanel10.Location = new Point(23, 357);
            tableLayoutPanel10.Name = "tableLayoutPanel10";
            tableLayoutPanel10.Padding = new Padding(9, 0, 9, 0);
            tableLayoutPanel10.RowCount = 1;
            tableLayoutPanel10.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel10.Size = new Size(517, 46);
            tableLayoutPanel10.TabIndex = 42;
            // 
            // btnLoadPicProduct
            // 
            btnLoadPicProduct.Dock = DockStyle.Fill;
            btnLoadPicProduct.Font = new Font("Times New Roman", 14F, FontStyle.Bold);
            btnLoadPicProduct.Location = new Point(12, 3);
            btnLoadPicProduct.Name = "btnLoadPicProduct";
            btnLoadPicProduct.Size = new Size(493, 40);
            btnLoadPicProduct.TabIndex = 4;
            btnLoadPicProduct.Text = "صورة المنتج";
            btnLoadPicProduct.UseVisualStyleBackColor = true;
            btnLoadPicProduct.Click += btnLoadPicProduct_Click;
            // 
            // tableLayoutPanel14
            // 
            tableLayoutPanel14.BackColor = Color.Transparent;
            tableLayoutPanel14.ColumnCount = 1;
            tableLayoutPanel14.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel14.Controls.Add(PicProduct, 0, 0);
            tableLayoutPanel14.Dock = DockStyle.Fill;
            tableLayoutPanel14.Location = new Point(23, 44);
            tableLayoutPanel14.Name = "tableLayoutPanel14";
            tableLayoutPanel14.Padding = new Padding(9);
            tableLayoutPanel14.RowCount = 1;
            tableLayoutPanel14.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel14.Size = new Size(517, 307);
            tableLayoutPanel14.TabIndex = 4;
            // 
            // PicProduct
            // 
            PicProduct.BackColor = Color.Transparent;
            PicProduct.BorderStyle = BorderStyle.FixedSingle;
            PicProduct.Dock = DockStyle.Fill;
            PicProduct.Location = new Point(12, 12);
            PicProduct.Name = "PicProduct";
            PicProduct.Size = new Size(493, 283);
            PicProduct.TabIndex = 4;
            PicProduct.TabStop = false;
            // 
            // lblPathProductPic
            // 
            lblPathProductPic.BackColor = Color.Transparent;
            lblPathProductPic.Dock = DockStyle.Top;
            lblPathProductPic.Font = new Font("Times New Roman", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPathProductPic.ForeColor = Color.DarkBlue;
            lblPathProductPic.Location = new Point(24, 406);
            lblPathProductPic.Margin = new Padding(4, 0, 4, 0);
            lblPathProductPic.Name = "lblPathProductPic";
            lblPathProductPic.Size = new Size(515, 22);
            lblPathProductPic.TabIndex = 40;
            lblPathProductPic.Text = "المسار";
            lblPathProductPic.TextAlign = ContentAlignment.MiddleRight;
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.FromArgb(255, 255, 192);
            btnSave.Dock = DockStyle.Top;
            btnSave.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSave.ForeColor = Color.FromArgb(0, 0, 192);
            btnSave.Location = new Point(24, 467);
            btnSave.Margin = new Padding(4);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(515, 31);
            btnSave.TabIndex = 43;
            btnSave.TabStop = false;
            btnSave.Text = "حفظ ";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 6;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66667F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.725338F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 7.73694372F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Controls.Add(btnHelp, 5, 0);
            tableLayoutPanel1.Controls.Add(label6, 4, 0);
            tableLayoutPanel1.Controls.Add(label5, 3, 0);
            tableLayoutPanel1.Controls.Add(label4, 2, 0);
            tableLayoutPanel1.Controls.Add(lblCategory_id, 1, 0);
            tableLayoutPanel1.Controls.Add(lblSuppliers, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(23, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(517, 35);
            tableLayoutPanel1.TabIndex = 44;
            // 
            // btnHelp
            // 
            btnHelp.BackColor = Color.FromArgb(255, 255, 192);
            btnHelp.Dock = DockStyle.Fill;
            btnHelp.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            btnHelp.Location = new Point(4, 4);
            btnHelp.Margin = new Padding(4);
            btnHelp.Name = "btnHelp";
            btnHelp.Size = new Size(33, 27);
            btnHelp.TabIndex = 62;
            btnHelp.TabStop = false;
            btnHelp.Text = "?";
            btnHelp.UseVisualStyleBackColor = false;
            btnHelp.Click += btnHelp_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Dock = DockStyle.Fill;
            label6.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.ForeColor = Color.FromArgb(0, 0, 192);
            label6.Location = new Point(45, 0);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(124, 35);
            label6.TabIndex = 61;
            label6.Text = "0";
            label6.TextAlign = ContentAlignment.MiddleLeft;
            label6.Visible = false;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Fill;
            label5.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.ForeColor = Color.FromArgb(0, 0, 192);
            label5.Location = new Point(177, 0);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(78, 35);
            label5.TabIndex = 60;
            label5.Text = "0";
            label5.TextAlign = ContentAlignment.MiddleLeft;
            label5.Visible = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Fill;
            label4.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = Color.FromArgb(0, 0, 192);
            label4.Location = new Point(263, 0);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(78, 35);
            label4.TabIndex = 59;
            label4.Text = "0";
            label4.TextAlign = ContentAlignment.MiddleLeft;
            label4.Visible = false;
            // 
            // lblCategory_id
            // 
            lblCategory_id.AutoSize = true;
            lblCategory_id.Dock = DockStyle.Fill;
            lblCategory_id.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCategory_id.ForeColor = Color.FromArgb(0, 0, 192);
            lblCategory_id.Location = new Point(349, 0);
            lblCategory_id.Margin = new Padding(4, 0, 4, 0);
            lblCategory_id.Name = "lblCategory_id";
            lblCategory_id.Size = new Size(78, 35);
            lblCategory_id.TabIndex = 58;
            lblCategory_id.Text = "0";
            lblCategory_id.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblSuppliers
            // 
            lblSuppliers.AutoSize = true;
            lblSuppliers.Dock = DockStyle.Fill;
            lblSuppliers.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSuppliers.ForeColor = Color.FromArgb(0, 0, 192);
            lblSuppliers.Location = new Point(435, 0);
            lblSuppliers.Margin = new Padding(4, 0, 4, 0);
            lblSuppliers.Name = "lblSuppliers";
            lblSuppliers.Size = new Size(78, 35);
            lblSuppliers.TabIndex = 57;
            lblSuppliers.Text = "0";
            lblSuppliers.TextAlign = ContentAlignment.MiddleLeft;
            lblSuppliers.Visible = false;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Controls.Add(tableLayoutPanel7, 0, 0);
            tableLayoutPanel2.Controls.Add(tableLayoutPanel13, 0, 1);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(572, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 8F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 92F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 19F));
            tableLayoutPanel2.Size = new Size(562, 542);
            tableLayoutPanel2.TabIndex = 12;
            // 
            // tableLayoutPanel7
            // 
            tableLayoutPanel7.ColumnCount = 2;
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel7.Controls.Add(lblID_Product, 1, 0);
            tableLayoutPanel7.Controls.Add(lblProductCode, 0, 0);
            tableLayoutPanel7.Dock = DockStyle.Fill;
            tableLayoutPanel7.Location = new Point(3, 3);
            tableLayoutPanel7.Name = "tableLayoutPanel7";
            tableLayoutPanel7.RowCount = 1;
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel7.Size = new Size(556, 37);
            tableLayoutPanel7.TabIndex = 44;
            // 
            // lblID_Product
            // 
            lblID_Product.BackColor = Color.Transparent;
            lblID_Product.FlatStyle = FlatStyle.Flat;
            lblID_Product.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblID_Product.ForeColor = Color.FromArgb(255, 128, 128);
            lblID_Product.Location = new Point(56, 0);
            lblID_Product.Margin = new Padding(4, 0, 4, 0);
            lblID_Product.Name = "lblID_Product";
            lblID_Product.Size = new Size(218, 33);
            lblID_Product.TabIndex = 27;
            lblID_Product.Text = "0";
            lblID_Product.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblProductCode
            // 
            lblProductCode.BackColor = Color.Transparent;
            lblProductCode.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblProductCode.ForeColor = Color.FromArgb(255, 128, 128);
            lblProductCode.Location = new Point(334, 0);
            lblProductCode.Margin = new Padding(4, 0, 4, 0);
            lblProductCode.Name = "lblProductCode";
            lblProductCode.RightToLeft = RightToLeft.No;
            lblProductCode.Size = new Size(218, 33);
            lblProductCode.TabIndex = 26;
            lblProductCode.Text = "1001";
            lblProductCode.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel13
            // 
            tableLayoutPanel13.ColumnCount = 2;
            tableLayoutPanel13.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 39.14989F));
            tableLayoutPanel13.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60.85011F));
            tableLayoutPanel13.Controls.Add(lblProductStock, 1, 4);
            tableLayoutPanel13.Controls.Add(label1, 0, 4);
            tableLayoutPanel13.Controls.Add(txtProdName, 1, 0);
            tableLayoutPanel13.Controls.Add(txtNoteProduct, 1, 1);
            tableLayoutPanel13.Controls.Add(label13, 0, 12);
            tableLayoutPanel13.Controls.Add(label19, 0, 0);
            tableLayoutPanel13.Controls.Add(label18, 0, 1);
            tableLayoutPanel13.Controls.Add(lblRegistYear, 1, 12);
            tableLayoutPanel13.Controls.Add(label10, 0, 2);
            tableLayoutPanel13.Controls.Add(txtSuppliers, 1, 2);
            tableLayoutPanel13.Controls.Add(label8, 0, 3);
            tableLayoutPanel13.Controls.Add(txtProdCodeOnSuplier, 1, 3);
            tableLayoutPanel13.Controls.Add(txtMinStock, 1, 11);
            tableLayoutPanel13.Controls.Add(txtMinLenth, 1, 10);
            tableLayoutPanel13.Controls.Add(cbxUnit_ID, 1, 9);
            tableLayoutPanel13.Controls.Add(label16, 0, 11);
            tableLayoutPanel13.Controls.Add(lblLinthText, 0, 10);
            tableLayoutPanel13.Controls.Add(lblMinLenth, 0, 9);
            tableLayoutPanel13.Controls.Add(labl, 0, 7);
            tableLayoutPanel13.Controls.Add(label9, 0, 6);
            tableLayoutPanel13.Controls.Add(label14, 0, 5);
            tableLayoutPanel13.Controls.Add(txtB_Price, 1, 7);
            tableLayoutPanel13.Controls.Add(txtU_Price, 1, 6);
            tableLayoutPanel13.Controls.Add(txtCategories, 1, 5);
            tableLayoutPanel13.Controls.Add(chkHiddinProd, 1, 8);
            tableLayoutPanel13.Dock = DockStyle.Top;
            tableLayoutPanel13.Location = new Point(3, 46);
            tableLayoutPanel13.Name = "tableLayoutPanel13";
            tableLayoutPanel13.Padding = new Padding(0, 0, 5, 0);
            tableLayoutPanel13.RowCount = 13;
            tableLayoutPanel13.RowStyles.Add(new RowStyle(SizeType.Percent, 7.691656F));
            tableLayoutPanel13.RowStyles.Add(new RowStyle(SizeType.Percent, 7.691656F));
            tableLayoutPanel13.RowStyles.Add(new RowStyle(SizeType.Percent, 7.692425F));
            tableLayoutPanel13.RowStyles.Add(new RowStyle(SizeType.Percent, 7.692425F));
            tableLayoutPanel13.RowStyles.Add(new RowStyle(SizeType.Percent, 7.692425F));
            tableLayoutPanel13.RowStyles.Add(new RowStyle(SizeType.Percent, 7.692425F));
            tableLayoutPanel13.RowStyles.Add(new RowStyle(SizeType.Percent, 7.692425F));
            tableLayoutPanel13.RowStyles.Add(new RowStyle(SizeType.Percent, 7.692425F));
            tableLayoutPanel13.RowStyles.Add(new RowStyle(SizeType.Percent, 7.692425F));
            tableLayoutPanel13.RowStyles.Add(new RowStyle(SizeType.Percent, 7.692425F));
            tableLayoutPanel13.RowStyles.Add(new RowStyle(SizeType.Percent, 7.692425F));
            tableLayoutPanel13.RowStyles.Add(new RowStyle(SizeType.Percent, 7.692425F));
            tableLayoutPanel13.RowStyles.Add(new RowStyle(SizeType.Percent, 7.692425F));
            tableLayoutPanel13.Size = new Size(556, 492);
            tableLayoutPanel13.TabIndex = 9;
            // 
            // lblProductStock
            // 
            lblProductStock.AutoSize = true;
            lblProductStock.Dock = DockStyle.Fill;
            lblProductStock.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblProductStock.ForeColor = Color.FromArgb(0, 0, 192);
            lblProductStock.Location = new Point(4, 148);
            lblProductStock.Margin = new Padding(4, 0, 4, 0);
            lblProductStock.Name = "lblProductStock";
            lblProductStock.Size = new Size(328, 37);
            lblProductStock.TabIndex = 56;
            lblProductStock.Text = "0";
            lblProductStock.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.FromArgb(0, 0, 192);
            label1.Location = new Point(340, 148);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(207, 37);
            label1.TabIndex = 55;
            label1.Text = "الرصيد الحالى";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtProdName
            // 
            txtProdName.Dock = DockStyle.Fill;
            txtProdName.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtProdName.ForeColor = Color.FromArgb(0, 0, 192);
            txtProdName.Location = new Point(4, 4);
            txtProdName.Margin = new Padding(4);
            txtProdName.Name = "txtProdName";
            txtProdName.Size = new Size(328, 29);
            txtProdName.TabIndex = 26;
            txtProdName.TextAlign = HorizontalAlignment.Center;
            txtProdName.KeyDown += txtProdName_KeyDown;
            // 
            // txtNoteProduct
            // 
            txtNoteProduct.Dock = DockStyle.Fill;
            txtNoteProduct.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtNoteProduct.ForeColor = Color.FromArgb(0, 0, 192);
            txtNoteProduct.Location = new Point(4, 41);
            txtNoteProduct.Margin = new Padding(4);
            txtNoteProduct.Name = "txtNoteProduct";
            txtNoteProduct.Size = new Size(328, 29);
            txtNoteProduct.TabIndex = 28;
            txtNoteProduct.TextAlign = HorizontalAlignment.Center;
            txtNoteProduct.KeyDown += txtNoteProduct_KeyDown;
            // 
            // label13
            // 
            label13.BackColor = Color.Transparent;
            label13.Dock = DockStyle.Fill;
            label13.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label13.ForeColor = Color.FromArgb(0, 0, 192);
            label13.Location = new Point(340, 444);
            label13.Margin = new Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new Size(207, 48);
            label13.TabIndex = 30;
            label13.Text = "البداية فى سنة";
            label13.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Dock = DockStyle.Fill;
            label19.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label19.ForeColor = Color.FromArgb(0, 0, 192);
            label19.Location = new Point(340, 0);
            label19.Margin = new Padding(4, 0, 4, 0);
            label19.Name = "label19";
            label19.Size = new Size(207, 37);
            label19.TabIndex = 0;
            label19.Text = "اسم الصنف";
            label19.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Dock = DockStyle.Fill;
            label18.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label18.ForeColor = Color.FromArgb(0, 0, 192);
            label18.Location = new Point(340, 37);
            label18.Margin = new Padding(4, 0, 4, 0);
            label18.Name = "label18";
            label18.Size = new Size(207, 37);
            label18.TabIndex = 27;
            label18.Text = "توصيف الصنف";
            label18.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblRegistYear
            // 
            lblRegistYear.BackColor = Color.Transparent;
            lblRegistYear.Dock = DockStyle.Left;
            lblRegistYear.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblRegistYear.ForeColor = Color.FromArgb(0, 0, 192);
            lblRegistYear.Location = new Point(173, 444);
            lblRegistYear.Margin = new Padding(4, 0, 4, 0);
            lblRegistYear.Name = "lblRegistYear";
            lblRegistYear.Size = new Size(159, 48);
            lblRegistYear.TabIndex = 29;
            lblRegistYear.Text = "0";
            lblRegistYear.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Dock = DockStyle.Fill;
            label10.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label10.ForeColor = Color.FromArgb(0, 0, 192);
            label10.Location = new Point(340, 74);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(207, 37);
            label10.TabIndex = 0;
            label10.Text = "اسم المورد";
            label10.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtSuppliers
            // 
            txtSuppliers.BackColor = Color.White;
            txtSuppliers.Dock = DockStyle.Fill;
            txtSuppliers.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtSuppliers.ForeColor = Color.FromArgb(0, 0, 192);
            txtSuppliers.Location = new Point(4, 78);
            txtSuppliers.Margin = new Padding(4);
            txtSuppliers.Name = "txtSuppliers";
            txtSuppliers.Size = new Size(328, 29);
            txtSuppliers.TabIndex = 49;
            txtSuppliers.TextAlign = HorizontalAlignment.Center;
            txtSuppliers.KeyDown += txtSuppliers_KeyDown;
            txtSuppliers.Leave += txtSuppliers_Leave;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Dock = DockStyle.Fill;
            label8.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label8.ForeColor = Color.FromArgb(0, 0, 192);
            label8.Location = new Point(340, 111);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(207, 37);
            label8.TabIndex = 0;
            label8.Text = "كود الصنف لدى المورد";
            label8.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtProdCodeOnSuplier
            // 
            txtProdCodeOnSuplier.Dock = DockStyle.Fill;
            txtProdCodeOnSuplier.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtProdCodeOnSuplier.ForeColor = Color.FromArgb(0, 0, 192);
            txtProdCodeOnSuplier.Location = new Point(4, 115);
            txtProdCodeOnSuplier.Margin = new Padding(4);
            txtProdCodeOnSuplier.Name = "txtProdCodeOnSuplier";
            txtProdCodeOnSuplier.Size = new Size(328, 29);
            txtProdCodeOnSuplier.TabIndex = 26;
            txtProdCodeOnSuplier.Text = "0";
            txtProdCodeOnSuplier.TextAlign = HorizontalAlignment.Center;
            txtProdCodeOnSuplier.KeyDown += txtProdCodeOnSuplier_KeyDown;
            // 
            // txtMinStock
            // 
            txtMinStock.BackColor = Color.White;
            txtMinStock.Dock = DockStyle.Left;
            txtMinStock.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtMinStock.ForeColor = Color.FromArgb(0, 0, 192);
            txtMinStock.Location = new Point(173, 411);
            txtMinStock.Margin = new Padding(4);
            txtMinStock.Name = "txtMinStock";
            txtMinStock.Size = new Size(159, 29);
            txtMinStock.TabIndex = 53;
            txtMinStock.TextAlign = HorizontalAlignment.Center;
            txtMinStock.KeyDown += txtMinStock_KeyDown;
            // 
            // txtMinLenth
            // 
            txtMinLenth.Dock = DockStyle.Left;
            txtMinLenth.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtMinLenth.ForeColor = Color.FromArgb(0, 0, 192);
            txtMinLenth.Location = new Point(173, 374);
            txtMinLenth.Margin = new Padding(4);
            txtMinLenth.Name = "txtMinLenth";
            txtMinLenth.Size = new Size(159, 29);
            txtMinLenth.TabIndex = 26;
            txtMinLenth.Text = "0";
            txtMinLenth.TextAlign = HorizontalAlignment.Center;
            txtMinLenth.KeyDown += txtMinLenth_KeyDown;
            // 
            // cbxUnit_ID
            // 
            cbxUnit_ID.Dock = DockStyle.Left;
            cbxUnit_ID.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cbxUnit_ID.ForeColor = Color.FromArgb(0, 0, 192);
            cbxUnit_ID.FormattingEnabled = true;
            cbxUnit_ID.Location = new Point(174, 336);
            cbxUnit_ID.Name = "cbxUnit_ID";
            cbxUnit_ID.Size = new Size(159, 28);
            cbxUnit_ID.TabIndex = 31;
            cbxUnit_ID.SelectedIndexChanged += cbxUnit_ID_SelectedIndexChanged;
            cbxUnit_ID.KeyDown += cbxUnit_ID_KeyDown;
            // 
            // label16
            // 
            label16.BackColor = Color.Transparent;
            label16.Dock = DockStyle.Fill;
            label16.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label16.ForeColor = Color.FromArgb(0, 0, 192);
            label16.Location = new Point(340, 407);
            label16.Margin = new Padding(4, 0, 4, 0);
            label16.Name = "label16";
            label16.Size = new Size(207, 37);
            label16.TabIndex = 30;
            label16.Text = "حد ادنى للرصيد";
            label16.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblLinthText
            // 
            lblLinthText.AutoSize = true;
            lblLinthText.Dock = DockStyle.Fill;
            lblLinthText.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLinthText.ForeColor = Color.FromArgb(0, 0, 192);
            lblLinthText.Location = new Point(340, 370);
            lblLinthText.Margin = new Padding(4, 0, 4, 0);
            lblLinthText.Name = "lblLinthText";
            lblLinthText.Size = new Size(207, 37);
            lblLinthText.TabIndex = 0;
            lblLinthText.Text = "اقل طول للفاضلة";
            lblLinthText.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblMinLenth
            // 
            lblMinLenth.AutoSize = true;
            lblMinLenth.Dock = DockStyle.Fill;
            lblMinLenth.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblMinLenth.ForeColor = Color.FromArgb(0, 0, 192);
            lblMinLenth.Location = new Point(340, 333);
            lblMinLenth.Margin = new Padding(4, 0, 4, 0);
            lblMinLenth.Name = "lblMinLenth";
            lblMinLenth.Size = new Size(207, 37);
            lblMinLenth.TabIndex = 1;
            lblMinLenth.Text = "الوحدة";
            lblMinLenth.TextAlign = ContentAlignment.MiddleRight;
            // 
            // labl
            // 
            labl.BackColor = Color.Transparent;
            labl.Dock = DockStyle.Fill;
            labl.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labl.ForeColor = Color.FromArgb(0, 0, 192);
            labl.Location = new Point(340, 259);
            labl.Margin = new Padding(4, 0, 4, 0);
            labl.Name = "labl";
            labl.Size = new Size(207, 37);
            labl.TabIndex = 25;
            labl.Text = "سعر شراء";
            labl.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            label9.BackColor = Color.Transparent;
            label9.Dock = DockStyle.Fill;
            label9.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label9.ForeColor = Color.FromArgb(0, 0, 192);
            label9.Location = new Point(340, 222);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(207, 37);
            label9.TabIndex = 25;
            label9.Text = "سعر بيع";
            label9.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Dock = DockStyle.Fill;
            label14.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label14.ForeColor = Color.FromArgb(0, 0, 192);
            label14.Location = new Point(340, 185);
            label14.Margin = new Padding(4, 0, 4, 0);
            label14.Name = "label14";
            label14.Size = new Size(207, 37);
            label14.TabIndex = 50;
            label14.Text = "التصنيف الشجرى";
            label14.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtB_Price
            // 
            txtB_Price.Dock = DockStyle.Left;
            txtB_Price.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtB_Price.ForeColor = Color.FromArgb(0, 0, 192);
            txtB_Price.Location = new Point(173, 263);
            txtB_Price.Margin = new Padding(4);
            txtB_Price.Name = "txtB_Price";
            txtB_Price.Size = new Size(159, 29);
            txtB_Price.TabIndex = 26;
            txtB_Price.Text = "0";
            txtB_Price.TextAlign = HorizontalAlignment.Center;
            txtB_Price.KeyDown += txtB_Price_KeyDown;
            // 
            // txtU_Price
            // 
            txtU_Price.Dock = DockStyle.Left;
            txtU_Price.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtU_Price.ForeColor = Color.FromArgb(0, 0, 192);
            txtU_Price.Location = new Point(173, 226);
            txtU_Price.Margin = new Padding(4);
            txtU_Price.Name = "txtU_Price";
            txtU_Price.Size = new Size(159, 29);
            txtU_Price.TabIndex = 26;
            txtU_Price.Text = "0";
            txtU_Price.TextAlign = HorizontalAlignment.Center;
            txtU_Price.KeyDown += txtU_Price_KeyDown;
            // 
            // txtCategories
            // 
            txtCategories.BackColor = Color.White;
            txtCategories.Dock = DockStyle.Left;
            txtCategories.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtCategories.ForeColor = Color.FromArgb(0, 0, 192);
            txtCategories.Location = new Point(173, 189);
            txtCategories.Margin = new Padding(4);
            txtCategories.Name = "txtCategories";
            txtCategories.Size = new Size(159, 29);
            txtCategories.TabIndex = 51;
            txtCategories.TextAlign = HorizontalAlignment.Center;
            txtCategories.KeyDown += txtCategories_KeyDown;
            txtCategories.Leave += txtCategories_Leave;
            // 
            // chkHiddinProd
            // 
            chkHiddinProd.AutoSize = true;
            chkHiddinProd.Location = new Point(236, 299);
            chkHiddinProd.Name = "chkHiddinProd";
            chkHiddinProd.Size = new Size(97, 19);
            chkHiddinProd.TabIndex = 54;
            chkHiddinProd.Text = "معطل ومخفى";
            chkHiddinProd.UseVisualStyleBackColor = true;
            // 
            // frm_ProductModify
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1137, 548);
            Controls.Add(tlpNewProduct);
            Name = "frm_ProductModify";
            RightToLeft = RightToLeft.Yes;
            Text = "تعديل صنف";
            Load += frm_ProductModify_Load;
            tlpNewProduct.ResumeLayout(false);
            tableLayoutPanel12.ResumeLayout(false);
            tableLayoutPanel10.ResumeLayout(false);
            tableLayoutPanel14.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PicProduct).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel7.ResumeLayout(false);
            tableLayoutPanel13.ResumeLayout(false);
            tableLayoutPanel13.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpNewProduct;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel12;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
        private System.Windows.Forms.Button btnLoadPicProduct;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel14;
        private System.Windows.Forms.PictureBox PicProduct;
        private System.Windows.Forms.Label lblPathProductPic;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel13;
        private System.Windows.Forms.TextBox txtProdName;
        private System.Windows.Forms.TextBox txtNoteProduct;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label lblRegistYear;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtSuppliers;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtProdCodeOnSuplier;
        private System.Windows.Forms.TextBox txtMinStock;
        private System.Windows.Forms.TextBox txtMinLenth;
        private System.Windows.Forms.ComboBox cbxUnit_ID;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lblLinthText;
        private System.Windows.Forms.Label lblMinLenth;
        private System.Windows.Forms.Label labl;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtB_Price;
        private System.Windows.Forms.TextBox txtU_Price;
        private System.Windows.Forms.TextBox txtCategories;
        private System.Windows.Forms.CheckBox chkHiddinProd;
        private System.Windows.Forms.Label lblProductStock;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblCategory_id;
        private System.Windows.Forms.Label lblSuppliers;
        private Label lblID_Product;
        private Label lblProductCode;
        private Button btnHelp;
    }
}