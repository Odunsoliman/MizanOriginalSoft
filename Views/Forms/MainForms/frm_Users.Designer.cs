namespace MizanOriginalSoft.Views.Forms.MainForms
{
    partial class frm_Users
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
            lblTitel = new Label();
            tableLayoutPanel12 = new TableLayoutPanel();
            label29 = new Label();
            txtUserName = new TextBox();
            label28 = new Label();
            txtFullName = new TextBox();
            btnResetPassword = new Button();
            btnSave_UserData = new Button();
            chkIsAdmin = new CheckBox();
            chkIsActive = new CheckBox();
            label2 = new Label();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel12.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(btnSave_UserData, 0, 2);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel12, 0, 1);
            tableLayoutPanel1.Controls.Add(lblTitel, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(4, 4, 4, 4);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(10, 10, 10, 30);
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 12.2994652F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 69.7860947F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 17.7631588F));
            tableLayoutPanel1.Size = new Size(391, 414);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // lblTitel
            // 
            lblTitel.Dock = DockStyle.Fill;
            lblTitel.Font = new Font("Times New Roman", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitel.Location = new Point(14, 10);
            lblTitel.Margin = new Padding(4, 0, 4, 0);
            lblTitel.Name = "lblTitel";
            lblTitel.Size = new Size(363, 46);
            lblTitel.TabIndex = 1;
            lblTitel.Text = "label1";
            lblTitel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel12
            // 
            tableLayoutPanel12.ColumnCount = 2;
            tableLayoutPanel12.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30.0595245F));
            tableLayoutPanel12.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 69.9404755F));
            tableLayoutPanel12.Controls.Add(label29, 0, 1);
            tableLayoutPanel12.Controls.Add(txtUserName, 1, 1);
            tableLayoutPanel12.Controls.Add(label2, 0, 5);
            tableLayoutPanel12.Controls.Add(label28, 0, 2);
            tableLayoutPanel12.Controls.Add(txtFullName, 1, 2);
            tableLayoutPanel12.Controls.Add(chkIsAdmin, 1, 3);
            tableLayoutPanel12.Controls.Add(chkIsActive, 1, 4);
            tableLayoutPanel12.Controls.Add(btnResetPassword, 1, 5);
            tableLayoutPanel12.Dock = DockStyle.Top;
            tableLayoutPanel12.Location = new Point(14, 60);
            tableLayoutPanel12.Margin = new Padding(4, 4, 4, 4);
            tableLayoutPanel12.Name = "tableLayoutPanel12";
            tableLayoutPanel12.RowCount = 7;
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Percent, 8.333333F));
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Percent, 8.333333F));
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Percent, 8.333333F));
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Percent, 8.333333F));
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Percent, 8.333333F));
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Percent, 8.333333F));
            tableLayoutPanel12.RowStyles.Add(new RowStyle(SizeType.Percent, 8.333333F));
            tableLayoutPanel12.Size = new Size(363, 253);
            tableLayoutPanel12.TabIndex = 3;
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Dock = DockStyle.Fill;
            label29.Location = new Point(258, 36);
            label29.Margin = new Padding(4, 0, 4, 0);
            label29.Name = "label29";
            label29.Size = new Size(101, 36);
            label29.TabIndex = 8;
            label29.Text = "اسم المستخدم";
            label29.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtUserName
            // 
            txtUserName.Dock = DockStyle.Fill;
            txtUserName.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold);
            txtUserName.Location = new Point(4, 40);
            txtUserName.Margin = new Padding(4, 4, 4, 4);
            txtUserName.Name = "txtUserName";
            txtUserName.Size = new Size(246, 29);
            txtUserName.TabIndex = 1;
            // 
            // label28
            // 
            label28.AutoSize = true;
            label28.Dock = DockStyle.Fill;
            label28.Location = new Point(258, 72);
            label28.Margin = new Padding(4, 0, 4, 0);
            label28.Name = "label28";
            label28.Size = new Size(101, 36);
            label28.TabIndex = 7;
            label28.Text = "المسمى الوظيفى";
            label28.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtFullName
            // 
            txtFullName.Dock = DockStyle.Fill;
            txtFullName.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold);
            txtFullName.Location = new Point(4, 76);
            txtFullName.Margin = new Padding(4, 4, 4, 4);
            txtFullName.Name = "txtFullName";
            txtFullName.Size = new Size(246, 29);
            txtFullName.TabIndex = 2;
            // 
            // btnResetPassword
            // 
            btnResetPassword.Dock = DockStyle.Fill;
            btnResetPassword.Location = new Point(4, 184);
            btnResetPassword.Margin = new Padding(4, 4, 4, 4);
            btnResetPassword.Name = "btnResetPassword";
            btnResetPassword.Size = new Size(246, 28);
            btnResetPassword.TabIndex = 5;
            btnResetPassword.Text = "كسر كلمة المرور";
            btnResetPassword.UseVisualStyleBackColor = true;
            // 
            // btnSave_UserData
            // 
            btnSave_UserData.Dock = DockStyle.Top;
            btnSave_UserData.Location = new Point(14, 321);
            btnSave_UserData.Margin = new Padding(4, 4, 4, 4);
            btnSave_UserData.Name = "btnSave_UserData";
            btnSave_UserData.Size = new Size(363, 39);
            btnSave_UserData.TabIndex = 4;
            btnSave_UserData.Text = "حفظ التعديل";
            btnSave_UserData.UseVisualStyleBackColor = true;
            btnSave_UserData.Click += btnSave_UserData_Click;
            // 
            // chkIsAdmin
            // 
            chkIsAdmin.AutoSize = true;
            chkIsAdmin.Dock = DockStyle.Fill;
            chkIsAdmin.Location = new Point(4, 112);
            chkIsAdmin.Margin = new Padding(4, 4, 4, 4);
            chkIsAdmin.Name = "chkIsAdmin";
            chkIsAdmin.Size = new Size(246, 28);
            chkIsAdmin.TabIndex = 10;
            chkIsAdmin.Text = "Is Admin";
            chkIsAdmin.UseVisualStyleBackColor = true;
            // 
            // chkIsActive
            // 
            chkIsActive.AutoSize = true;
            chkIsActive.Dock = DockStyle.Fill;
            chkIsActive.Location = new Point(4, 148);
            chkIsActive.Margin = new Padding(4, 4, 4, 4);
            chkIsActive.Name = "chkIsActive";
            chkIsActive.Size = new Size(246, 28);
            chkIsActive.TabIndex = 11;
            chkIsActive.Text = "Is Active";
            chkIsActive.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(258, 180);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(101, 36);
            label2.TabIndex = 1;
            label2.Text = "2222";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // frm_Users
            // 
            AutoScaleDimensions = new SizeF(9F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(391, 414);
            Controls.Add(tableLayoutPanel1);
            Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Margin = new Padding(4, 4, 4, 4);
            Name = "frm_Users";
            RightToLeft = RightToLeft.Yes;
            Text = "frm_Users";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel12.ResumeLayout(false);
            tableLayoutPanel12.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Label lblTitel;
        private TableLayoutPanel tableLayoutPanel12;
        private Label label29;
        private TextBox txtUserName;
        private Label label28;
        private TextBox txtFullName;
        private Button btnResetPassword;
        private Button btnSave_UserData;
        private CheckBox chkIsAdmin;
        private CheckBox chkIsActive;
        private Label label2;
    }
}