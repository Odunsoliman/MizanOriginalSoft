namespace MizanOriginalSoft .Views .Forms .MainForms 
{
    partial class frm_Journal
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
            btnClose = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)DGV).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // DGV
            // 
            DGV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV.Dock = DockStyle.Fill;
            DGV.Enabled = false;
            DGV.Location = new Point(3, 43);
            DGV.Name = "DGV";
            DGV.RowHeadersWidth = 51;
            DGV.RowTemplate.Height = 26;
            DGV.Size = new Size(1268, 211);
            DGV.TabIndex = 0;
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.FromArgb(255, 128, 128);
            btnClose.Dock = DockStyle.Fill;
            btnClose.Location = new Point(1236, 3);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(29, 28);
            btnClose.TabIndex = 1;
            btnClose.Text = "×";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Controls.Add(DGV, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(10, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 15.74803F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 84.25197F));
            tableLayoutPanel1.Size = new Size(1274, 257);
            tableLayoutPanel1.TabIndex = 2;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 2.786885F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 97.21311F));
            tableLayoutPanel2.Controls.Add(btnClose, 0, 0);
            tableLayoutPanel2.Controls.Add(label1, 1, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(1268, 34);
            tableLayoutPanel2.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Times New Roman", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Red;
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(1227, 34);
            label1.TabIndex = 2;
            label1.Text = "قيد محاسبى";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // frm_Journal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(1294, 276);
            ControlBox = false;
            Controls.Add(tableLayoutPanel1);
            Name = "frm_Journal";
            Padding = new Padding(10, 0, 10, 19);
            RightToLeft = RightToLeft.Yes;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "قيد محاسبي";
            Load += frm_Journal_Load;
            ((System.ComponentModel.ISupportInitialize)DGV).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView DGV;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
    }
}