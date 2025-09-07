namespace MizanOriginalSoft .Views .Forms.Accounts
{
    partial class frm_AddAccount
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
            tableLayoutPanel4 = new TableLayoutPanel();
            lblNewAcc_In = new Label();
            tableLayoutPanel16 = new TableLayoutPanel();
            btnSave = new Button();
            tableLayoutPanel6 = new TableLayoutPanel();
            tableLayoutPanel1 = new TableLayoutPanel();
            label22 = new Label();
            lblAccID = new Label();
            lblDateOfJoin = new Label();
            label9 = new Label();
            label15 = new Label();
            label2 = new Label();
            txtAccName = new TextBox();
            tableLayoutPanel10 = new TableLayoutPanel();
            lblBalancStat = new Label();
            lblBalanc = new Label();
            txtFirstPhon = new TextBox();
            label3 = new Label();
            lbl = new Label();
            txtAntherPhon = new TextBox();
            label5 = new Label();
            txtAccNote = new TextBox();
            label1 = new Label();
            txtClientEmail = new TextBox();
            label17 = new Label();
            txtClientAddress = new TextBox();
            tableLayoutPanel4.SuspendLayout();
            tableLayoutPanel16.SuspendLayout();
            tableLayoutPanel6.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel10.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 1;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.Controls.Add(lblNewAcc_In, 0, 0);
            tableLayoutPanel4.Controls.Add(tableLayoutPanel16, 0, 2);
            tableLayoutPanel4.Controls.Add(tableLayoutPanel6, 0, 1);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(0, 0);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 3;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 75F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanel4.Size = new Size(800, 422);
            tableLayoutPanel4.TabIndex = 2;
            // 
            // lblNewAcc_In
            // 
            lblNewAcc_In.AutoSize = true;
            lblNewAcc_In.Dock = DockStyle.Fill;
            lblNewAcc_In.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblNewAcc_In.ForeColor = Color.Purple;
            lblNewAcc_In.Location = new Point(4, 0);
            lblNewAcc_In.Margin = new Padding(4, 0, 4, 0);
            lblNewAcc_In.Name = "lblNewAcc_In";
            lblNewAcc_In.Size = new Size(792, 42);
            lblNewAcc_In.TabIndex = 96;
            lblNewAcc_In.Text = "حساب جديد";
            lblNewAcc_In.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel16
            // 
            tableLayoutPanel16.ColumnCount = 4;
            tableLayoutPanel16.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel16.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel16.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel16.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel16.Controls.Add(btnSave, 3, 0);
            tableLayoutPanel16.Dock = DockStyle.Fill;
            tableLayoutPanel16.Location = new Point(3, 361);
            tableLayoutPanel16.Name = "tableLayoutPanel16";
            tableLayoutPanel16.RowCount = 1;
            tableLayoutPanel16.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel16.Size = new Size(794, 58);
            tableLayoutPanel16.TabIndex = 92;
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.Teal;
            btnSave.Dock = DockStyle.Fill;
            btnSave.Font = new Font("Times New Roman", 16F, FontStyle.Bold);
            btnSave.ForeColor = Color.Yellow;
            btnSave.Location = new Point(4, 4);
            btnSave.Margin = new Padding(4);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(192, 50);
            btnSave.TabIndex = 83;
            btnSave.Text = "حفــظ";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 2;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.60584F));
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 63.50365F));
            tableLayoutPanel6.Controls.Add(tableLayoutPanel1, 1, 0);
            tableLayoutPanel6.Controls.Add(label9, 0, 0);
            tableLayoutPanel6.Controls.Add(label15, 0, 1);
            tableLayoutPanel6.Controls.Add(label2, 0, 2);
            tableLayoutPanel6.Controls.Add(txtAccName, 1, 2);
            tableLayoutPanel6.Controls.Add(tableLayoutPanel10, 1, 1);
            tableLayoutPanel6.Controls.Add(txtFirstPhon, 1, 3);
            tableLayoutPanel6.Controls.Add(label3, 0, 3);
            tableLayoutPanel6.Controls.Add(lbl, 0, 4);
            tableLayoutPanel6.Controls.Add(txtAntherPhon, 1, 4);
            tableLayoutPanel6.Controls.Add(label5, 0, 5);
            tableLayoutPanel6.Controls.Add(txtAccNote, 1, 5);
            tableLayoutPanel6.Controls.Add(label1, 0, 6);
            tableLayoutPanel6.Controls.Add(txtClientEmail, 1, 6);
            tableLayoutPanel6.Controls.Add(label17, 0, 7);
            tableLayoutPanel6.Controls.Add(txtClientAddress, 1, 7);
            tableLayoutPanel6.Dock = DockStyle.Fill;
            tableLayoutPanel6.Location = new Point(3, 45);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 8;
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel6.Size = new Size(794, 310);
            tableLayoutPanel6.TabIndex = 93;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel1.Controls.Add(label22, 1, 0);
            tableLayoutPanel1.Controls.Add(lblAccID, 0, 0);
            tableLayoutPanel1.Controls.Add(lblDateOfJoin, 2, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(3, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(624, 32);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Dock = DockStyle.Fill;
            label22.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            label22.ForeColor = SystemColors.ActiveCaption;
            label22.Location = new Point(192, 0);
            label22.Margin = new Padding(4, 0, 4, 0);
            label22.Name = "label22";
            label22.Size = new Size(304, 32);
            label22.TabIndex = 85;
            label22.Text = "تاريخ الانشاء";
            label22.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblAccID
            // 
            lblAccID.AutoSize = true;
            lblAccID.Dock = DockStyle.Fill;
            lblAccID.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAccID.ForeColor = SystemColors.ActiveCaption;
            lblAccID.Location = new Point(504, 0);
            lblAccID.Margin = new Padding(4, 0, 4, 0);
            lblAccID.Name = "lblAccID";
            lblAccID.Size = new Size(116, 32);
            lblAccID.TabIndex = 52;
            lblAccID.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblDateOfJoin
            // 
            lblDateOfJoin.AutoSize = true;
            lblDateOfJoin.Dock = DockStyle.Fill;
            lblDateOfJoin.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            lblDateOfJoin.ForeColor = SystemColors.ActiveCaption;
            lblDateOfJoin.Location = new Point(4, 0);
            lblDateOfJoin.Margin = new Padding(4, 0, 4, 0);
            lblDateOfJoin.Name = "lblDateOfJoin";
            lblDateOfJoin.Size = new Size(180, 32);
            lblDateOfJoin.TabIndex = 84;
            lblDateOfJoin.Text = "0";
            lblDateOfJoin.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Dock = DockStyle.Fill;
            label9.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label9.ForeColor = SystemColors.ActiveCaption;
            label9.Location = new Point(634, 0);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(156, 38);
            label9.TabIndex = 58;
            label9.Text = "كود الحساب";
            label9.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Dock = DockStyle.Fill;
            label15.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label15.ForeColor = Color.Purple;
            label15.Location = new Point(634, 38);
            label15.Margin = new Padding(4, 0, 4, 0);
            label15.Name = "label15";
            label15.Size = new Size(156, 38);
            label15.TabIndex = 68;
            label15.Text = "رصيد الحساب";
            label15.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.FromArgb(0, 0, 192);
            label2.Location = new Point(634, 76);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(156, 38);
            label2.TabIndex = 54;
            label2.Text = "اسم  الحساب";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtAccName
            // 
            txtAccName.Dock = DockStyle.Fill;
            txtAccName.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtAccName.ForeColor = Color.FromArgb(0, 0, 192);
            txtAccName.Location = new Point(4, 80);
            txtAccName.Margin = new Padding(4);
            txtAccName.Name = "txtAccName";
            txtAccName.Size = new Size(622, 29);
            txtAccName.TabIndex = 47;
            txtAccName.TextAlign = HorizontalAlignment.Center;
            // 
            // tableLayoutPanel10
            // 
            tableLayoutPanel10.ColumnCount = 2;
            tableLayoutPanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel10.Controls.Add(lblBalancStat, 1, 0);
            tableLayoutPanel10.Controls.Add(lblBalanc, 0, 0);
            tableLayoutPanel10.Dock = DockStyle.Fill;
            tableLayoutPanel10.Location = new Point(3, 41);
            tableLayoutPanel10.Name = "tableLayoutPanel10";
            tableLayoutPanel10.RowCount = 1;
            tableLayoutPanel10.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel10.Size = new Size(624, 32);
            tableLayoutPanel10.TabIndex = 69;
            // 
            // lblBalancStat
            // 
            lblBalancStat.Dock = DockStyle.Fill;
            lblBalancStat.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblBalancStat.ForeColor = Color.Purple;
            lblBalancStat.Location = new Point(4, 0);
            lblBalancStat.Margin = new Padding(4, 0, 4, 0);
            lblBalancStat.Name = "lblBalancStat";
            lblBalancStat.Size = new Size(304, 32);
            lblBalancStat.TabIndex = 76;
            lblBalancStat.Text = "0";
            lblBalancStat.TextAlign = ContentAlignment.TopCenter;
            // 
            // lblBalanc
            // 
            lblBalanc.Dock = DockStyle.Fill;
            lblBalanc.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblBalanc.ForeColor = Color.Purple;
            lblBalanc.Location = new Point(316, 0);
            lblBalanc.Margin = new Padding(4, 0, 4, 0);
            lblBalanc.Name = "lblBalanc";
            lblBalanc.Size = new Size(304, 32);
            lblBalanc.TabIndex = 64;
            lblBalanc.Text = "0";
            lblBalanc.TextAlign = ContentAlignment.TopCenter;
            // 
            // txtFirstPhon
            // 
            txtFirstPhon.Dock = DockStyle.Left;
            txtFirstPhon.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtFirstPhon.ForeColor = Color.FromArgb(0, 0, 192);
            txtFirstPhon.Location = new Point(317, 118);
            txtFirstPhon.Margin = new Padding(4);
            txtFirstPhon.Name = "txtFirstPhon";
            txtFirstPhon.Size = new Size(309, 29);
            txtFirstPhon.TabIndex = 48;
            txtFirstPhon.TextAlign = HorizontalAlignment.Center;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.FromArgb(0, 0, 192);
            label3.Location = new Point(634, 114);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(156, 38);
            label3.TabIndex = 65;
            label3.Text = "الهاتف";
            label3.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbl
            // 
            lbl.AutoSize = true;
            lbl.Dock = DockStyle.Fill;
            lbl.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbl.ForeColor = Color.FromArgb(0, 0, 192);
            lbl.Location = new Point(634, 152);
            lbl.Margin = new Padding(4, 0, 4, 0);
            lbl.Name = "lbl";
            lbl.Size = new Size(156, 38);
            lbl.TabIndex = 66;
            lbl.Text = "هاتف آخر";
            lbl.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtAntherPhon
            // 
            txtAntherPhon.Dock = DockStyle.Left;
            txtAntherPhon.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtAntherPhon.ForeColor = Color.FromArgb(0, 0, 192);
            txtAntherPhon.Location = new Point(317, 156);
            txtAntherPhon.Margin = new Padding(4);
            txtAntherPhon.Name = "txtAntherPhon";
            txtAntherPhon.Size = new Size(309, 29);
            txtAntherPhon.TabIndex = 49;
            txtAntherPhon.TextAlign = HorizontalAlignment.Center;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Fill;
            label5.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.ForeColor = Color.FromArgb(0, 0, 192);
            label5.Location = new Point(634, 190);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(156, 38);
            label5.TabIndex = 67;
            label5.Text = "مذكرة";
            label5.TextAlign = ContentAlignment.TopRight;
            // 
            // txtAccNote
            // 
            txtAccNote.Dock = DockStyle.Fill;
            txtAccNote.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtAccNote.ForeColor = Color.FromArgb(0, 0, 192);
            txtAccNote.Location = new Point(4, 194);
            txtAccNote.Margin = new Padding(4);
            txtAccNote.Multiline = true;
            txtAccNote.Name = "txtAccNote";
            txtAccNote.Size = new Size(622, 30);
            txtAccNote.TabIndex = 50;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.FromArgb(0, 0, 192);
            label1.Location = new Point(634, 228);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(156, 38);
            label1.TabIndex = 68;
            label1.Text = "بريد الكترونى";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtClientEmail
            // 
            txtClientEmail.Dock = DockStyle.Fill;
            txtClientEmail.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtClientEmail.ForeColor = Color.FromArgb(0, 0, 192);
            txtClientEmail.Location = new Point(4, 232);
            txtClientEmail.Margin = new Padding(4);
            txtClientEmail.Name = "txtClientEmail";
            txtClientEmail.Size = new Size(622, 29);
            txtClientEmail.TabIndex = 71;
            txtClientEmail.TextAlign = HorizontalAlignment.Right;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Dock = DockStyle.Fill;
            label17.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label17.ForeColor = Color.FromArgb(0, 0, 192);
            label17.Location = new Point(634, 266);
            label17.Margin = new Padding(4, 0, 4, 0);
            label17.Name = "label17";
            label17.Size = new Size(156, 44);
            label17.TabIndex = 70;
            label17.Text = "عنوان";
            label17.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtClientAddress
            // 
            txtClientAddress.Dock = DockStyle.Fill;
            txtClientAddress.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtClientAddress.ForeColor = Color.FromArgb(0, 0, 192);
            txtClientAddress.Location = new Point(4, 270);
            txtClientAddress.Margin = new Padding(4);
            txtClientAddress.Name = "txtClientAddress";
            txtClientAddress.Size = new Size(622, 29);
            txtClientAddress.TabIndex = 72;
            // 
            // frm_AddAccount
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 422);
            Controls.Add(tableLayoutPanel4);
            Name = "frm_AddAccount";
            RightToLeft = RightToLeft.Yes;
            Text = "frm_AddAccount";
            Load += frm_AddAccount_Load;
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            tableLayoutPanel16.ResumeLayout(false);
            tableLayoutPanel6.ResumeLayout(false);
            tableLayoutPanel6.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel10.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel16;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label lblAccID;
        private System.Windows.Forms.Label lblDateOfJoin;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtAccName;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
        private System.Windows.Forms.Label lblBalancStat;
        private System.Windows.Forms.Label lblBalanc;
        private System.Windows.Forms.TextBox txtClientAddress;
        private System.Windows.Forms.TextBox txtClientEmail;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAntherPhon;
        private System.Windows.Forms.TextBox txtAccNote;
        private System.Windows.Forms.TextBox txtFirstPhon;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lbl;
        private System.Windows.Forms.Label lblNewAcc_In;
    }
}