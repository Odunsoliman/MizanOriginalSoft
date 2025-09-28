namespace MizanOriginalSoft.Views.Forms.Accounts
{
    partial class frm_AccountDetailAdd
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
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            txtContactName = new TextBox();
            txtPhone = new TextBox();
            txtMobile = new TextBox();
            txtEmail = new TextBox();
            txtAddress = new TextBox();
            txtNotes = new TextBox();
            tableLayoutPanel3 = new TableLayoutPanel();
            lblTitel = new Label();
            btnCancel = new Button();
            btnSave = new Button();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 1);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 0, 2);
            tableLayoutPanel1.Controls.Add(lblTitel, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tableLayoutPanel1.Location = new Point(10, 10);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.Size = new Size(538, 430);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.37594F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 74.62406F));
            tableLayoutPanel2.Controls.Add(label6, 0, 5);
            tableLayoutPanel2.Controls.Add(label5, 0, 4);
            tableLayoutPanel2.Controls.Add(label4, 0, 3);
            tableLayoutPanel2.Controls.Add(label3, 0, 2);
            tableLayoutPanel2.Controls.Add(label2, 0, 1);
            tableLayoutPanel2.Controls.Add(label1, 0, 0);
            tableLayoutPanel2.Controls.Add(txtContactName, 1, 0);
            tableLayoutPanel2.Controls.Add(txtPhone, 1, 1);
            tableLayoutPanel2.Controls.Add(txtMobile, 1, 2);
            tableLayoutPanel2.Controls.Add(txtEmail, 1, 3);
            tableLayoutPanel2.Controls.Add(txtAddress, 1, 4);
            tableLayoutPanel2.Controls.Add(txtNotes, 1, 5);
            tableLayoutPanel2.Dock = DockStyle.Top;
            tableLayoutPanel2.Location = new Point(3, 46);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 6;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 16.666666F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 16.666666F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 16.666666F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 16.666666F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 16.666666F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 16.666666F));
            tableLayoutPanel2.Size = new Size(532, 207);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Dock = DockStyle.Fill;
            label6.Location = new Point(400, 170);
            label6.Name = "label6";
            label6.Padding = new Padding(0, 0, 20, 0);
            label6.Size = new Size(129, 37);
            label6.TabIndex = 9;
            label6.Text = "ملاحظات";
            label6.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Fill;
            label5.Location = new Point(400, 136);
            label5.Name = "label5";
            label5.Padding = new Padding(0, 0, 20, 0);
            label5.Size = new Size(129, 34);
            label5.TabIndex = 8;
            label5.Text = "العنوان";
            label5.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Fill;
            label4.Location = new Point(400, 102);
            label4.Name = "label4";
            label4.Padding = new Padding(0, 0, 20, 0);
            label4.Size = new Size(129, 34);
            label4.TabIndex = 7;
            label4.Text = "البريد الاكترونى";
            label4.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Location = new Point(400, 68);
            label3.Name = "label3";
            label3.Padding = new Padding(0, 0, 20, 0);
            label3.Size = new Size(129, 34);
            label3.TabIndex = 6;
            label3.Text = "المحمول";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(400, 34);
            label2.Name = "label2";
            label2.Padding = new Padding(0, 0, 20, 0);
            label2.Size = new Size(129, 34);
            label2.TabIndex = 5;
            label2.Text = "الهاتف";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(400, 0);
            label1.Name = "label1";
            label1.Padding = new Padding(0, 0, 20, 0);
            label1.Size = new Size(129, 34);
            label1.TabIndex = 3;
            label1.Text = "اسم ";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtContactName
            // 
            txtContactName.Dock = DockStyle.Fill;
            txtContactName.Location = new Point(3, 3);
            txtContactName.Name = "txtContactName";
            txtContactName.Size = new Size(391, 29);
            txtContactName.TabIndex = 4;
            // 
            // txtPhone
            // 
            txtPhone.Dock = DockStyle.Fill;
            txtPhone.Location = new Point(3, 37);
            txtPhone.Name = "txtPhone";
            txtPhone.Size = new Size(391, 29);
            txtPhone.TabIndex = 4;
            // 
            // txtMobile
            // 
            txtMobile.Dock = DockStyle.Fill;
            txtMobile.Location = new Point(3, 71);
            txtMobile.Name = "txtMobile";
            txtMobile.Size = new Size(391, 29);
            txtMobile.TabIndex = 4;
            // 
            // txtEmail
            // 
            txtEmail.Dock = DockStyle.Fill;
            txtEmail.Location = new Point(3, 105);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(391, 29);
            txtEmail.TabIndex = 4;
            txtEmail.TextAlign = HorizontalAlignment.Right;
            // 
            // txtAddress
            // 
            txtAddress.Dock = DockStyle.Fill;
            txtAddress.Location = new Point(3, 139);
            txtAddress.Name = "txtAddress";
            txtAddress.Size = new Size(391, 29);
            txtAddress.TabIndex = 4;
            // 
            // txtNotes
            // 
            txtNotes.Dock = DockStyle.Fill;
            txtNotes.Location = new Point(3, 173);
            txtNotes.Name = "txtNotes";
            txtNotes.Size = new Size(391, 29);
            txtNotes.TabIndex = 4;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Controls.Add(btnCancel, 0, 0);
            tableLayoutPanel3.Controls.Add(btnSave, 1, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(3, 390);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Size = new Size(532, 37);
            tableLayoutPanel3.TabIndex = 1;
            // 
            // lblTitel
            // 
            lblTitel.AutoSize = true;
            lblTitel.Dock = DockStyle.Fill;
            lblTitel.Location = new Point(3, 0);
            lblTitel.Name = "lblTitel";
            lblTitel.Size = new Size(532, 43);
            lblTitel.TabIndex = 2;
            lblTitel.Text = "اضافة تفاصيل الى الحساب";
            lblTitel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnCancel
            // 
            btnCancel.Dock = DockStyle.Fill;
            btnCancel.Location = new Point(269, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(260, 31);
            btnCancel.TabIndex = 0;
            btnCancel.Text = "خروج";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSave
            // 
            btnSave.Dock = DockStyle.Fill;
            btnSave.Location = new Point(3, 3);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(260, 31);
            btnSave.TabIndex = 1;
            btnSave.Text = "حفظ";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // frm_AccountDetailAdd
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(558, 450);
            Controls.Add(tableLayoutPanel1);
            Name = "frm_AccountDetailAdd";
            Padding = new Padding(10);
            RightToLeft = RightToLeft.Yes;
            Text = "frm_AccountDetailAdd";
            Load += frm_AccountDetailAdd_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
        private Label lblTitel;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private TextBox txtContactName;
        private TextBox txtPhone;
        private TextBox txtMobile;
        private TextBox txtEmail;
        private TextBox txtAddress;
        private TextBox txtNotes;
        private Button btnCancel;
        private Button btnSave;
    }
}