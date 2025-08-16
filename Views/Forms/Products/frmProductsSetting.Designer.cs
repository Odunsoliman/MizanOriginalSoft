namespace MizanOriginalSoft.Views.Forms.Products
{
    partial class frmProductsSetting
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
            tlpHome = new TableLayoutPanel();
            tlpTop = new TableLayoutPanel();
            lblTitle = new Label();
            tableLayoutPanel6 = new TableLayoutPanel();
            label6 = new Label();
            tableLayoutPanel3 = new TableLayoutPanel();
            btnHelp = new Button();
            tlpHome.SuspendLayout();
            tlpTop.SuspendLayout();
            tableLayoutPanel6.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // tlpHome
            // 
            tlpHome.ColumnCount = 1;
            tlpHome.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpHome.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tlpHome.Controls.Add(tlpTop, 0, 0);
            tlpHome.Dock = DockStyle.Fill;
            tlpHome.Location = new Point(0, 0);
            tlpHome.Name = "tlpHome";
            tlpHome.RowCount = 3;
            tlpHome.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tlpHome.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
            tlpHome.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tlpHome.Size = new Size(1122, 706);
            tlpHome.TabIndex = 0;
            // 
            // tlpTop
            // 
            tlpTop.BackColor = Color.FromArgb(235, 255, 235);
            tlpTop.ColumnCount = 3;
            tlpTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 3F));
            tlpTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62.36559F));
            tlpTop.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34.67742F));
            tlpTop.Controls.Add(lblTitle, 1, 0);
            tlpTop.Controls.Add(tableLayoutPanel6, 2, 0);
            tlpTop.Dock = DockStyle.Fill;
            tlpTop.Location = new Point(3, 3);
            tlpTop.Name = "tlpTop";
            tlpTop.RowCount = 1;
            tlpTop.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpTop.Size = new Size(1116, 64);
            tlpTop.TabIndex = 11;
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.Font = new Font("Times New Roman", 26F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.Red;
            lblTitle.ImageAlign = ContentAlignment.BottomRight;
            lblTitle.Location = new Point(391, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(689, 64);
            lblTitle.TabIndex = 8;
            lblTitle.Text = "إعداد الاصناف والتصنيفات";
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 1;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel6.Controls.Add(label6, 0, 1);
            tableLayoutPanel6.Controls.Add(tableLayoutPanel3, 0, 0);
            tableLayoutPanel6.Dock = DockStyle.Fill;
            tableLayoutPanel6.Location = new Point(3, 3);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 2;
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            tableLayoutPanel6.Size = new Size(382, 58);
            tableLayoutPanel6.TabIndex = 9;
            // 
            // label6
            // 
            label6.BackColor = Color.Transparent;
            label6.Dock = DockStyle.Fill;
            label6.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.ForeColor = SystemColors.AppWorkspace;
            label6.Location = new Point(3, 34);
            label6.Name = "label6";
            label6.Size = new Size(376, 24);
            label6.TabIndex = 40;
            label6.Text = " للمساعدة الخاصة باداة معينة  ctrl + H";
            label6.TextAlign = ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 5;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            tableLayoutPanel3.Controls.Add(btnHelp, 4, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 3);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(376, 28);
            tableLayoutPanel3.TabIndex = 39;
            // 
            // btnHelp
            // 
            btnHelp.BackColor = Color.FromArgb(255, 255, 192);
            btnHelp.Dock = DockStyle.Fill;
            btnHelp.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            btnHelp.Location = new Point(4, 4);
            btnHelp.Margin = new Padding(4);
            btnHelp.Name = "btnHelp";
            btnHelp.Size = new Size(31, 20);
            btnHelp.TabIndex = 38;
            btnHelp.TabStop = false;
            btnHelp.Text = "?";
            btnHelp.UseVisualStyleBackColor = false;
            // 
            // frmProductsSetting
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1122, 706);
            Controls.Add(tlpHome);
            Name = "frmProductsSetting";
            RightToLeft = RightToLeft.Yes;
            Text = "frmProductsSetting";
            Load += frmProductsSetting_Load;
            tlpHome.ResumeLayout(false);
            tlpTop.ResumeLayout(false);
            tableLayoutPanel6.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tlpHome;
        private TableLayoutPanel tlpTop;
        private Label lblTitle;
        private TableLayoutPanel tableLayoutPanel6;
        private Label label6;
        private TableLayoutPanel tableLayoutPanel3;
        private Button btnHelp;
    }
}