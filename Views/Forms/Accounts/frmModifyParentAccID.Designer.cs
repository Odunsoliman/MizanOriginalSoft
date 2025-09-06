namespace MizanOriginalSoft.Views.Forms.Accounts
{
    partial class frmModifyParentAccID
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
            DGVSelectedAcc = new DataGridView();
            lblCountSelected = new Label();
            DGV = new DataGridView();
            lblTitel = new Label();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGVSelectedAcc).BeginInit();
            ((System.ComponentModel.ISupportInitialize)DGV).BeginInit();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(DGVSelectedAcc, 1, 1);
            tableLayoutPanel1.Controls.Add(lblCountSelected, 1, 0);
            tableLayoutPanel1.Controls.Add(DGV, 0, 1);
            tableLayoutPanel1.Controls.Add(lblTitel, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(10, 10);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 90F));
            tableLayoutPanel1.Size = new Size(879, 640);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // DGVSelectedAcc
            // 
            DGVSelectedAcc.AllowUserToAddRows = false;
            DGVSelectedAcc.AllowUserToDeleteRows = false;
            DGVSelectedAcc.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGVSelectedAcc.Dock = DockStyle.Fill;
            DGVSelectedAcc.Enabled = false;
            DGVSelectedAcc.Location = new Point(3, 67);
            DGVSelectedAcc.Name = "DGVSelectedAcc";
            DGVSelectedAcc.ReadOnly = true;
            DGVSelectedAcc.RowHeadersVisible = false;
            DGVSelectedAcc.Size = new Size(434, 570);
            DGVSelectedAcc.TabIndex = 3;
            // 
            // lblCountSelected
            // 
            lblCountSelected.AutoSize = true;
            lblCountSelected.Dock = DockStyle.Fill;
            lblCountSelected.Location = new Point(3, 0);
            lblCountSelected.Name = "lblCountSelected";
            lblCountSelected.Size = new Size(434, 64);
            lblCountSelected.TabIndex = 2;
            lblCountSelected.Text = "label2";
            lblCountSelected.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DGV
            // 
            DGV.AllowUserToAddRows = false;
            DGV.AllowUserToDeleteRows = false;
            DGV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV.Dock = DockStyle.Fill;
            DGV.Location = new Point(443, 67);
            DGV.Name = "DGV";
            DGV.ReadOnly = true;
            DGV.RowHeadersVisible = false;
            DGV.Size = new Size(433, 570);
            DGV.TabIndex = 0;
            DGV.CellDoubleClick += DGV_CellDoubleClick;
            DGV.SelectionChanged += DGV_SelectionChanged;
            // 
            // lblTitel
            // 
            lblTitel.AutoSize = true;
            lblTitel.Dock = DockStyle.Fill;
            lblTitel.Location = new Point(443, 0);
            lblTitel.Name = "lblTitel";
            lblTitel.Size = new Size(433, 64);
            lblTitel.TabIndex = 1;
            lblTitel.Text = "يتم النقل الى الحساب الرئيسى ";
            lblTitel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // frmModifyParentAccID
            // 
            AutoScaleDimensions = new SizeF(11F, 22F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(899, 660);
            Controls.Add(tableLayoutPanel1);
            Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Margin = new Padding(5, 4, 5, 4);
            Name = "frmModifyParentAccID";
            Padding = new Padding(10);
            RightToLeft = RightToLeft.Yes;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "تعديل اب حساب";
            Load += frmModifyParentAccID_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DGVSelectedAcc).EndInit();
            ((System.ComponentModel.ISupportInitialize)DGV).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private DataGridView DGV;
        private Label lblTitel;
        private DataGridView DGVSelectedAcc;
        private Label lblCountSelected;
    }
}