namespace MizanOriginalSoft.Views.Forms.Products
{
    partial class frmPrintBarCode
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
            tableLayoutPanel3 = new TableLayoutPanel();
            lblNameProd = new Label();
            txtAmount = new TextBox();
            btnPlus = new Button();
            label1 = new Label();
            btnMinus = new Button();
            txtCodeProduct = new TextBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            btnClose = new Button();
            btnPrintBarCode = new Button();
            tableLayoutPanel4 = new TableLayoutPanel();
            rdoSheet = new RadioButton();
            rdoRoll = new RadioButton();
            tableLayoutPanel5 = new TableLayoutPanel();
            lblCountTects = new Label();
            lbl_CO = new Label();
            tableLayoutPanel6 = new TableLayoutPanel();
            btnDeleteAll = new Button();
            btnDeleteSelected = new Button();
            DGV = new DataGridView();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            tableLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 0, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 2);
            tableLayoutPanel1.Controls.Add(DGV, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.Size = new Size(800, 422);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 7;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 8F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 24F));
            tableLayoutPanel3.Controls.Add(lblNameProd, 2, 0);
            tableLayoutPanel3.Controls.Add(txtAmount, 4, 0);
            tableLayoutPanel3.Controls.Add(btnPlus, 3, 0);
            tableLayoutPanel3.Controls.Add(label1, 0, 0);
            tableLayoutPanel3.Controls.Add(btnMinus, 5, 0);
            tableLayoutPanel3.Controls.Add(txtCodeProduct, 1, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(794, 36);
            tableLayoutPanel3.TabIndex = 1;
            // 
            // lblNameProd
            // 
            lblNameProd.AutoSize = true;
            lblNameProd.Dock = DockStyle.Fill;
            lblNameProd.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblNameProd.Location = new Point(182, 0);
            lblNameProd.Name = "lblNameProd";
            lblNameProd.Size = new Size(456, 36);
            lblNameProd.TabIndex = 3;
            lblNameProd.Text = "اسم الصنف";
            lblNameProd.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtAmount
            // 
            txtAmount.Dock = DockStyle.Fill;
            txtAmount.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtAmount.Location = new Point(75, 3);
            txtAmount.Name = "txtAmount";
            txtAmount.Size = new Size(55, 29);
            txtAmount.TabIndex = 1;
            txtAmount.TextAlign = HorizontalAlignment.Center;
            // 
            // btnPlus
            // 
            btnPlus.Dock = DockStyle.Fill;
            btnPlus.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnPlus.Location = new Point(136, 3);
            btnPlus.Name = "btnPlus";
            btnPlus.Size = new Size(40, 30);
            btnPlus.TabIndex = 2;
            btnPlus.TabStop = false;
            btnPlus.Text = "+";
            btnPlus.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(736, 0);
            label1.Name = "label1";
            label1.Size = new Size(55, 36);
            label1.TabIndex = 0;
            label1.Text = "كود";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnMinus
            // 
            btnMinus.Dock = DockStyle.Fill;
            btnMinus.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnMinus.Location = new Point(29, 3);
            btnMinus.Name = "btnMinus";
            btnMinus.Size = new Size(40, 30);
            btnMinus.TabIndex = 2;
            btnMinus.TabStop = false;
            btnMinus.Text = "-";
            btnMinus.UseVisualStyleBackColor = true;
            // 
            // txtCodeProduct
            // 
            txtCodeProduct.Dock = DockStyle.Fill;
            txtCodeProduct.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtCodeProduct.Location = new Point(644, 3);
            txtCodeProduct.Name = "txtCodeProduct";
            txtCodeProduct.Size = new Size(86, 29);
            txtCodeProduct.TabIndex = 0;
            txtCodeProduct.TextAlign = HorizontalAlignment.Center;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 6;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.Controls.Add(btnClose, 5, 0);
            tableLayoutPanel2.Controls.Add(btnPrintBarCode, 0, 0);
            tableLayoutPanel2.Controls.Add(tableLayoutPanel4, 2, 0);
            tableLayoutPanel2.Controls.Add(tableLayoutPanel5, 3, 0);
            tableLayoutPanel2.Controls.Add(tableLayoutPanel6, 4, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 340);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(794, 79);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // btnClose
            // 
            btnClose.Dock = DockStyle.Fill;
            btnClose.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnClose.Location = new Point(3, 3);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(156, 73);
            btnClose.TabIndex = 6;
            btnClose.TabStop = false;
            btnClose.Text = "خروج";
            btnClose.UseVisualStyleBackColor = true;
            // 
            // btnPrintBarCode
            // 
            btnPrintBarCode.Dock = DockStyle.Fill;
            btnPrintBarCode.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnPrintBarCode.Location = new Point(599, 3);
            btnPrintBarCode.Name = "btnPrintBarCode";
            btnPrintBarCode.Size = new Size(192, 73);
            btnPrintBarCode.TabIndex = 3;
            btnPrintBarCode.TabStop = false;
            btnPrintBarCode.Text = "طباعة باركود";
            btnPrintBarCode.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 1;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Controls.Add(rdoSheet, 0, 1);
            tableLayoutPanel4.Controls.Add(rdoRoll, 0, 0);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(497, 3);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 2;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Size = new Size(89, 73);
            tableLayoutPanel4.TabIndex = 10;
            // 
            // rdoSheet
            // 
            rdoSheet.AutoSize = true;
            rdoSheet.Dock = DockStyle.Fill;
            rdoSheet.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold);
            rdoSheet.ForeColor = Color.FromArgb(192, 0, 192);
            rdoSheet.Location = new Point(3, 39);
            rdoSheet.Name = "rdoSheet";
            rdoSheet.Size = new Size(83, 31);
            rdoSheet.TabIndex = 10;
            rdoSheet.Text = "شيت";
            rdoSheet.UseVisualStyleBackColor = true;
            // 
            // rdoRoll
            // 
            rdoRoll.AutoSize = true;
            rdoRoll.Checked = true;
            rdoRoll.Dock = DockStyle.Fill;
            rdoRoll.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold);
            rdoRoll.ForeColor = Color.Blue;
            rdoRoll.Location = new Point(3, 3);
            rdoRoll.Name = "rdoRoll";
            rdoRoll.Size = new Size(83, 30);
            rdoRoll.TabIndex = 9;
            rdoRoll.TabStop = true;
            rdoRoll.Text = "رول";
            rdoRoll.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 1;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.Controls.Add(lblCountTects, 0, 1);
            tableLayoutPanel5.Controls.Add(lbl_CO, 0, 0);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(323, 3);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 2;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.Size = new Size(168, 73);
            tableLayoutPanel5.TabIndex = 12;
            // 
            // lblCountTects
            // 
            lblCountTects.AutoSize = true;
            lblCountTects.Dock = DockStyle.Fill;
            lblCountTects.Font = new Font("Times New Roman", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCountTects.ForeColor = Color.FromArgb(192, 0, 192);
            lblCountTects.Location = new Point(3, 36);
            lblCountTects.Name = "lblCountTects";
            lblCountTects.Size = new Size(162, 37);
            lblCountTects.TabIndex = 13;
            lblCountTects.Text = "0";
            lblCountTects.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbl_CO
            // 
            lbl_CO.AutoSize = true;
            lbl_CO.Dock = DockStyle.Fill;
            lbl_CO.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbl_CO.ForeColor = Color.Blue;
            lbl_CO.Location = new Point(3, 0);
            lbl_CO.Name = "lbl_CO";
            lbl_CO.Size = new Size(162, 36);
            lbl_CO.TabIndex = 12;
            lbl_CO.Text = "0";
            lbl_CO.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 1;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.Controls.Add(btnDeleteAll, 0, 1);
            tableLayoutPanel6.Controls.Add(btnDeleteSelected, 0, 0);
            tableLayoutPanel6.Dock = DockStyle.Fill;
            tableLayoutPanel6.Location = new Point(165, 3);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 2;
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel6.Size = new Size(152, 73);
            tableLayoutPanel6.TabIndex = 13;
            // 
            // btnDeleteAll
            // 
            btnDeleteAll.Dock = DockStyle.Fill;
            btnDeleteAll.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnDeleteAll.ForeColor = Color.Red;
            btnDeleteAll.Location = new Point(3, 39);
            btnDeleteAll.Name = "btnDeleteAll";
            btnDeleteAll.Size = new Size(146, 31);
            btnDeleteAll.TabIndex = 5;
            btnDeleteAll.TabStop = false;
            btnDeleteAll.Text = "حذف الكل";
            btnDeleteAll.UseVisualStyleBackColor = true;
            // 
            // btnDeleteSelected
            // 
            btnDeleteSelected.Dock = DockStyle.Fill;
            btnDeleteSelected.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnDeleteSelected.Location = new Point(3, 3);
            btnDeleteSelected.Name = "btnDeleteSelected";
            btnDeleteSelected.Size = new Size(146, 30);
            btnDeleteSelected.TabIndex = 4;
            btnDeleteSelected.TabStop = false;
            btnDeleteSelected.Text = "حذف المحدد";
            btnDeleteSelected.UseVisualStyleBackColor = true;
            // 
            // DGV
            // 
            DGV.AllowUserToAddRows = false;
            DGV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV.Dock = DockStyle.Fill;
            DGV.Location = new Point(3, 45);
            DGV.Name = "DGV";
            DGV.ReadOnly = true;
            DGV.RowHeadersVisible = false;
            DGV.RowHeadersWidth = 51;
            DGV.RowTemplate.Height = 26;
            DGV.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGV.Size = new Size(794, 289);
            DGV.TabIndex = 2;
            // 
            // frmPrintBarCode
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 422);
            ControlBox = false;
            Controls.Add(tableLayoutPanel1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmPrintBarCode";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "تجهيز البار كود";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel5.PerformLayout();
            tableLayoutPanel6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DGV).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCodeProduct;
        private System.Windows.Forms.Button btnPlus;
        private System.Windows.Forms.TextBox txtAmount;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.DataGridView DGV;
        private System.Windows.Forms.Button btnMinus;
        private System.Windows.Forms.Label lblNameProd;
        private System.Windows.Forms.Button btnPrintBarCode;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnDeleteSelected;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.RadioButton rdoSheet;
        private System.Windows.Forms.RadioButton rdoRoll;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label lblCountTects;
        private System.Windows.Forms.Label lbl_CO;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.Button btnDeleteAll;
    }
}