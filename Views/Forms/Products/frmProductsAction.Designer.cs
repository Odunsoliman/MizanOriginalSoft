namespace MizanOriginalSoft.Views.Forms.Products
{
    partial class frmProductsAction
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
            tlpMain = new TableLayoutPanel();
            tableLayoutPanel4 = new TableLayoutPanel();
            tableLayoutPanel7 = new TableLayoutPanel();
            label12 = new Label();
            lblCountDGV = new Label();
            tableLayoutPanel5 = new TableLayoutPanel();
            btnCancelSelected = new Button();
            btnSaveSelected = new Button();
            tableLayoutPanel3 = new TableLayoutPanel();
            lblCatID = new Label();
            lblSupplier_ID = new Label();
            txtU_PriceSelected = new TextBox();
            label3 = new Label();
            label15 = new Label();
            cbxUnit_idSelected = new ComboBox();
            label1 = new Label();
            txtCategoriesSelected = new TextBox();
            txtSupplierSelected = new TextBox();
            label2 = new Label();
            label10 = new Label();
            txtB_PriceSelected = new TextBox();
            DGV = new DataGridView();
            tableLayoutPanel1 = new TableLayoutPanel();
            lblTitel = new Label();
            tlpMain.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tableLayoutPanel7.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tlpMain
            // 
            tlpMain.ColumnCount = 1;
            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpMain.Controls.Add(tableLayoutPanel4, 0, 0);
            tlpMain.Dock = DockStyle.Fill;
            tlpMain.Location = new Point(3, 68);
            tlpMain.Name = "tlpMain";
            tlpMain.RowCount = 1;
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlpMain.Size = new Size(704, 588);
            tlpMain.TabIndex = 10;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 1;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.Controls.Add(tableLayoutPanel7, 0, 1);
            tableLayoutPanel4.Controls.Add(tableLayoutPanel5, 0, 3);
            tableLayoutPanel4.Controls.Add(tableLayoutPanel3, 0, 2);
            tableLayoutPanel4.Controls.Add(DGV, 0, 0);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(3, 3);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 4;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel4.Size = new Size(698, 582);
            tableLayoutPanel4.TabIndex = 10;
            // 
            // tableLayoutPanel7
            // 
            tableLayoutPanel7.ColumnCount = 2;
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 77.50439F));
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22.49561F));
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel7.Controls.Add(label12, 0, 0);
            tableLayoutPanel7.Controls.Add(lblCountDGV, 1, 0);
            tableLayoutPanel7.Dock = DockStyle.Fill;
            tableLayoutPanel7.Location = new Point(3, 294);
            tableLayoutPanel7.Name = "tableLayoutPanel7";
            tableLayoutPanel7.RowCount = 1;
            tableLayoutPanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel7.Size = new Size(692, 52);
            tableLayoutPanel7.TabIndex = 42;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Dock = DockStyle.Fill;
            label12.Font = new Font("Times New Roman", 16F, FontStyle.Bold);
            label12.ForeColor = Color.Fuchsia;
            label12.Location = new Point(160, 0);
            label12.Margin = new Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new Size(528, 52);
            label12.TabIndex = 42;
            label12.Text = "عدد الاصناف التى سيتم تعديلها بشكل مجمع";
            label12.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblCountDGV
            // 
            lblCountDGV.AutoSize = true;
            lblCountDGV.Dock = DockStyle.Fill;
            lblCountDGV.Font = new Font("Times New Roman", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCountDGV.ForeColor = Color.Fuchsia;
            lblCountDGV.Location = new Point(4, 0);
            lblCountDGV.Margin = new Padding(4, 0, 4, 0);
            lblCountDGV.Name = "lblCountDGV";
            lblCountDGV.Size = new Size(148, 52);
            lblCountDGV.TabIndex = 41;
            lblCountDGV.Text = "0";
            lblCountDGV.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.BackColor = SystemColors.ControlLight;
            tableLayoutPanel5.ColumnCount = 2;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel5.Controls.Add(btnCancelSelected, 1, 0);
            tableLayoutPanel5.Controls.Add(btnSaveSelected, 0, 0);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(3, 526);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 1;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel5.Size = new Size(692, 53);
            tableLayoutPanel5.TabIndex = 41;
            // 
            // btnCancelSelected
            // 
            btnCancelSelected.BackColor = Color.FromArgb(255, 255, 192);
            btnCancelSelected.Dock = DockStyle.Fill;
            btnCancelSelected.Font = new Font("Times New Roman", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancelSelected.Location = new Point(4, 4);
            btnCancelSelected.Margin = new Padding(4);
            btnCancelSelected.Name = "btnCancelSelected";
            btnCancelSelected.Size = new Size(338, 45);
            btnCancelSelected.TabIndex = 39;
            btnCancelSelected.TabStop = false;
            btnCancelSelected.Text = "الغاء";
            btnCancelSelected.UseVisualStyleBackColor = false;
            btnCancelSelected.Click += btnClose_Click;
            // 
            // btnSaveSelected
            // 
            btnSaveSelected.BackColor = Color.FromArgb(255, 255, 192);
            btnSaveSelected.Dock = DockStyle.Fill;
            btnSaveSelected.Font = new Font("Times New Roman", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSaveSelected.Location = new Point(350, 4);
            btnSaveSelected.Margin = new Padding(4);
            btnSaveSelected.Name = "btnSaveSelected";
            btnSaveSelected.Size = new Size(338, 45);
            btnSaveSelected.TabIndex = 41;
            btnSaveSelected.TabStop = false;
            btnSaveSelected.Text = "حفظ التعديل المجمع";
            btnSaveSelected.UseVisualStyleBackColor = false;
            btnSaveSelected.Click += btnSaveSelected_Click;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 5;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.83082F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.5287F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 37.91541F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10.7413F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 33F));
            tableLayoutPanel3.Controls.Add(lblCatID, 2, 3);
            tableLayoutPanel3.Controls.Add(lblSupplier_ID, 2, 2);
            tableLayoutPanel3.Controls.Add(txtU_PriceSelected, 1, 0);
            tableLayoutPanel3.Controls.Add(label3, 0, 0);
            tableLayoutPanel3.Controls.Add(label15, 0, 4);
            tableLayoutPanel3.Controls.Add(cbxUnit_idSelected, 1, 4);
            tableLayoutPanel3.Controls.Add(label1, 0, 3);
            tableLayoutPanel3.Controls.Add(txtCategoriesSelected, 1, 3);
            tableLayoutPanel3.Controls.Add(txtSupplierSelected, 1, 2);
            tableLayoutPanel3.Controls.Add(label2, 0, 2);
            tableLayoutPanel3.Controls.Add(label10, 0, 1);
            tableLayoutPanel3.Controls.Add(txtB_PriceSelected, 1, 1);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 352);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 5;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 20.0008F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 20.0008F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 20.0008F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 19.9988F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 19.9988F));
            tableLayoutPanel3.Size = new Size(692, 168);
            tableLayoutPanel3.TabIndex = 10;
            // 
            // lblCatID
            // 
            lblCatID.AutoSize = true;
            lblCatID.Dock = DockStyle.Fill;
            lblCatID.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCatID.ForeColor = Color.Blue;
            lblCatID.Location = new Point(109, 99);
            lblCatID.Margin = new Padding(4, 0, 4, 0);
            lblCatID.Name = "lblCatID";
            lblCatID.Size = new Size(241, 33);
            lblCatID.TabIndex = 71;
            lblCatID.Text = "0";
            lblCatID.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblSupplier_ID
            // 
            lblSupplier_ID.AutoSize = true;
            lblSupplier_ID.Dock = DockStyle.Fill;
            lblSupplier_ID.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSupplier_ID.ForeColor = Color.Blue;
            lblSupplier_ID.Location = new Point(109, 66);
            lblSupplier_ID.Margin = new Padding(4, 0, 4, 0);
            lblSupplier_ID.Name = "lblSupplier_ID";
            lblSupplier_ID.Size = new Size(241, 33);
            lblSupplier_ID.TabIndex = 68;
            lblSupplier_ID.Text = "0";
            lblSupplier_ID.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtU_PriceSelected
            // 
            txtU_PriceSelected.BackColor = Color.White;
            txtU_PriceSelected.Dock = DockStyle.Fill;
            txtU_PriceSelected.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtU_PriceSelected.ForeColor = Color.Purple;
            txtU_PriceSelected.Location = new Point(358, 4);
            txtU_PriceSelected.Margin = new Padding(4);
            txtU_PriceSelected.Name = "txtU_PriceSelected";
            txtU_PriceSelected.Size = new Size(160, 29);
            txtU_PriceSelected.TabIndex = 44;
            txtU_PriceSelected.TextAlign = HorizontalAlignment.Center;
            txtU_PriceSelected.KeyDown += txtU_PriceSelected_KeyDown;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.Blue;
            label3.Location = new Point(526, 0);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(162, 33);
            label3.TabIndex = 35;
            label3.Text = "توحيد سعر البيع";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Dock = DockStyle.Fill;
            label15.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label15.ForeColor = Color.Blue;
            label15.Location = new Point(526, 132);
            label15.Margin = new Padding(4, 0, 4, 0);
            label15.Name = "label15";
            label15.Size = new Size(162, 36);
            label15.TabIndex = 60;
            label15.Text = "توحيد الوحدة";
            label15.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cbxUnit_idSelected
            // 
            cbxUnit_idSelected.Dock = DockStyle.Fill;
            cbxUnit_idSelected.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            cbxUnit_idSelected.ForeColor = Color.FromArgb(0, 0, 192);
            cbxUnit_idSelected.FormattingEnabled = true;
            cbxUnit_idSelected.Location = new Point(357, 135);
            cbxUnit_idSelected.Name = "cbxUnit_idSelected";
            cbxUnit_idSelected.Size = new Size(162, 28);
            cbxUnit_idSelected.TabIndex = 61;
            cbxUnit_idSelected.KeyDown += cbxUnit_idSelected_KeyDown;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Blue;
            label1.Location = new Point(526, 99);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(162, 33);
            label1.TabIndex = 38;
            label1.Text = "توحيد التصنيف";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtCategoriesSelected
            // 
            txtCategoriesSelected.BackColor = Color.White;
            txtCategoriesSelected.Dock = DockStyle.Fill;
            txtCategoriesSelected.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtCategoriesSelected.ForeColor = Color.Purple;
            txtCategoriesSelected.Location = new Point(358, 103);
            txtCategoriesSelected.Margin = new Padding(4);
            txtCategoriesSelected.Name = "txtCategoriesSelected";
            txtCategoriesSelected.Size = new Size(160, 29);
            txtCategoriesSelected.TabIndex = 49;
            txtCategoriesSelected.TextAlign = HorizontalAlignment.Center;
            txtCategoriesSelected.KeyDown += txtCategoriesSelected_KeyDown;
            txtCategoriesSelected.Leave += txtCategoriesSelected_Leave;
            // 
            // txtSupplierSelected
            // 
            txtSupplierSelected.BackColor = Color.White;
            txtSupplierSelected.Dock = DockStyle.Fill;
            txtSupplierSelected.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtSupplierSelected.ForeColor = Color.Purple;
            txtSupplierSelected.Location = new Point(358, 70);
            txtSupplierSelected.Margin = new Padding(4);
            txtSupplierSelected.Name = "txtSupplierSelected";
            txtSupplierSelected.Size = new Size(160, 29);
            txtSupplierSelected.TabIndex = 47;
            txtSupplierSelected.TextAlign = HorizontalAlignment.Center;
            txtSupplierSelected.KeyDown += txtSupplierSelected_KeyDown;
            txtSupplierSelected.Leave += txtSupplierSelected_Leave;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.Blue;
            label2.Location = new Point(526, 66);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(162, 33);
            label2.TabIndex = 37;
            label2.Text = "توحيد المورد";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Dock = DockStyle.Fill;
            label10.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label10.ForeColor = Color.Blue;
            label10.Location = new Point(526, 33);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(162, 33);
            label10.TabIndex = 34;
            label10.Text = "توحيد سعر الشراء";
            label10.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtB_PriceSelected
            // 
            txtB_PriceSelected.BackColor = Color.White;
            txtB_PriceSelected.Dock = DockStyle.Fill;
            txtB_PriceSelected.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtB_PriceSelected.ForeColor = Color.Purple;
            txtB_PriceSelected.Location = new Point(358, 37);
            txtB_PriceSelected.Margin = new Padding(4);
            txtB_PriceSelected.Name = "txtB_PriceSelected";
            txtB_PriceSelected.Size = new Size(160, 29);
            txtB_PriceSelected.TabIndex = 36;
            txtB_PriceSelected.TextAlign = HorizontalAlignment.Center;
            txtB_PriceSelected.KeyDown += txtB_PriceSelected_KeyDown;
            // 
            // DGV
            // 
            DGV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV.Dock = DockStyle.Fill;
            DGV.Location = new Point(3, 3);
            DGV.Name = "DGV";
            DGV.RowHeadersWidth = 51;
            DGV.RowTemplate.Height = 26;
            DGV.Size = new Size(692, 285);
            DGV.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(lblTitel, 0, 0);
            tableLayoutPanel1.Controls.Add(tlpMain, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(20, 19);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 90F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 19F));
            tableLayoutPanel1.Size = new Size(710, 659);
            tableLayoutPanel1.TabIndex = 11;
            // 
            // lblTitel
            // 
            lblTitel.AutoSize = true;
            lblTitel.BackColor = SystemColors.ControlLight;
            lblTitel.Dock = DockStyle.Fill;
            lblTitel.Font = new Font("Times New Roman", 16F, FontStyle.Bold);
            lblTitel.ForeColor = Color.Fuchsia;
            lblTitel.Location = new Point(4, 0);
            lblTitel.Margin = new Padding(4, 0, 4, 0);
            lblTitel.Name = "lblTitel";
            lblTitel.Size = new Size(702, 65);
            lblTitel.TabIndex = 41;
            lblTitel.Text = "توحيد قيم مجموعة اصناف";
            lblTitel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // frmProductsAction
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(750, 697);
            ControlBox = false;
            Controls.Add(tableLayoutPanel1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmProductsAction";
            Padding = new Padding(20, 19, 20, 19);
            RightToLeft = RightToLeft.Yes;
            Text = "انشاء وتعديل الاصناف";
            Load += frmProductsAction_Load;
            tlpMain.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel7.ResumeLayout(false);
            tableLayoutPanel7.PerformLayout();
            tableLayoutPanel5.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DGV).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtB_PriceSelected;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblTitel;
        private System.Windows.Forms.TextBox txtU_PriceSelected;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtCategoriesSelected;
        private System.Windows.Forms.TextBox txtSupplierSelected;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.DataGridView DGV;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Button btnCancelSelected;
        private System.Windows.Forms.Button btnSaveSelected;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.Label lblCountDGV;
        private System.Windows.Forms.ComboBox cbxUnit_idSelected;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblCatID;
        private System.Windows.Forms.Label lblSupplier_ID;
    }
}