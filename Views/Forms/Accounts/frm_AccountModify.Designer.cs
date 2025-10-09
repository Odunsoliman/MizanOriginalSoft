namespace MizanOriginalSoft.Views.Forms.Accounts
{
    partial class frm_AccountModify
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
            lblIsEnerAcc = new Label();
            label2 = new Label();
            txtAccName = new TextBox();
            lblDateOfJoin = new Label();
            chkIsHidden = new CheckBox();
            tableLayoutPanel6 = new TableLayoutPanel();
            lblTreeAccCode = new Label();
            lblAccTypeID = new Label();
            chkIsForManger = new CheckBox();
            chkIsHasDetails = new CheckBox();
            lblParentTree = new Label();
            lblBalanceAndState = new Label();
            lblCreateByUserName = new Label();
            cbxParentTree = new ComboBox();
            label1 = new Label();
            lblTitetl_Item = new Label();
            tableLayoutPanel16 = new TableLayoutPanel();
            btnSave = new Button();
            btnClose = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel6.SuspendLayout();
            tableLayoutPanel16.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // lblIsEnerAcc
            // 
            lblIsEnerAcc.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblIsEnerAcc.ForeColor = SystemColors.ActiveCaption;
            lblIsEnerAcc.Location = new Point(4, 66);
            lblIsEnerAcc.Margin = new Padding(4, 0, 4, 0);
            lblIsEnerAcc.Name = "lblIsEnerAcc";
            lblIsEnerAcc.Size = new Size(281, 29);
            lblIsEnerAcc.TabIndex = 58;
            lblIsEnerAcc.Text = "هل حساب داخلى";
            lblIsEnerAcc.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.FromArgb(0, 0, 192);
            label2.Location = new Point(350, 0);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(89, 22);
            label2.TabIndex = 54;
            label2.Text = "اسم  الحساب";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtAccName
            // 
            txtAccName.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtAccName.ForeColor = Color.FromArgb(0, 0, 192);
            txtAccName.Location = new Point(4, 4);
            txtAccName.Margin = new Padding(4);
            txtAccName.Name = "txtAccName";
            txtAccName.Size = new Size(281, 29);
            txtAccName.TabIndex = 47;
            txtAccName.TextAlign = HorizontalAlignment.Center;
            // 
            // lblDateOfJoin
            // 
            lblDateOfJoin.AutoSize = true;
            lblDateOfJoin.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold);
            lblDateOfJoin.ForeColor = SystemColors.ActiveCaption;
            lblDateOfJoin.Location = new Point(235, 264);
            lblDateOfJoin.Name = "lblDateOfJoin";
            lblDateOfJoin.Size = new Size(51, 22);
            lblDateOfJoin.TabIndex = 75;
            lblDateOfJoin.Text = "التاريخ";
            lblDateOfJoin.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // chkIsHidden
            // 
            chkIsHidden.AutoSize = true;
            chkIsHidden.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold);
            chkIsHidden.ForeColor = Color.FromArgb(0, 0, 192);
            chkIsHidden.Location = new Point(179, 36);
            chkIsHidden.Name = "chkIsHidden";
            chkIsHidden.Size = new Size(107, 26);
            chkIsHidden.TabIndex = 73;
            chkIsHidden.Text = "حساب مخفى";
            chkIsHidden.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 2;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34.76298F));
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65.23702F));
            tableLayoutPanel6.Controls.Add(label2, 0, 0);
            tableLayoutPanel6.Controls.Add(txtAccName, 1, 0);
            tableLayoutPanel6.Controls.Add(chkIsHidden, 1, 1);
            tableLayoutPanel6.Controls.Add(lblTreeAccCode, 1, 11);
            tableLayoutPanel6.Controls.Add(lblAccTypeID, 1, 12);
            tableLayoutPanel6.Controls.Add(lblIsEnerAcc, 1, 2);
            tableLayoutPanel6.Controls.Add(chkIsForManger, 1, 3);
            tableLayoutPanel6.Controls.Add(chkIsHasDetails, 1, 4);
            tableLayoutPanel6.Controls.Add(lblParentTree, 1, 5);
            tableLayoutPanel6.Controls.Add(lblBalanceAndState, 1, 6);
            tableLayoutPanel6.Controls.Add(lblCreateByUserName, 1, 7);
            tableLayoutPanel6.Controls.Add(lblDateOfJoin, 1, 8);
            tableLayoutPanel6.Controls.Add(cbxParentTree, 1, 9);
            tableLayoutPanel6.Controls.Add(label1, 0, 9);
            tableLayoutPanel6.Location = new Point(3, 32);
            tableLayoutPanel6.Name = "tableLayoutPanel6";
            tableLayoutPanel6.RowCount = 14;
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 7.142857F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 7.142857F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 7.142857F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 7.142857F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 7.142857F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 7.142857F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 7.142857F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 7.142857F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 7.142857F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 7.142857F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 7.142857F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 7.142857F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 7.142857F));
            tableLayoutPanel6.RowStyles.Add(new RowStyle(SizeType.Percent, 7.142857F));
            tableLayoutPanel6.Size = new Size(443, 463);
            tableLayoutPanel6.TabIndex = 93;
            // 
            // lblTreeAccCode
            // 
            lblTreeAccCode.AutoSize = true;
            lblTreeAccCode.Location = new Point(248, 363);
            lblTreeAccCode.Name = "lblTreeAccCode";
            lblTreeAccCode.Size = new Size(38, 15);
            lblTreeAccCode.TabIndex = 78;
            lblTreeAccCode.Text = "label1";
            // 
            // lblAccTypeID
            // 
            lblAccTypeID.AutoSize = true;
            lblAccTypeID.Location = new Point(248, 396);
            lblAccTypeID.Name = "lblAccTypeID";
            lblAccTypeID.Size = new Size(38, 15);
            lblAccTypeID.TabIndex = 79;
            lblAccTypeID.Text = "label1";
            // 
            // chkIsForManger
            // 
            chkIsForManger.AutoSize = true;
            chkIsForManger.Location = new Point(186, 102);
            chkIsForManger.Name = "chkIsForManger";
            chkIsForManger.Size = new Size(100, 19);
            chkIsForManger.TabIndex = 76;
            chkIsForManger.Text = "خاص بالاداريين";
            chkIsForManger.UseVisualStyleBackColor = true;
            // 
            // chkIsHasDetails
            // 
            chkIsHasDetails.AutoSize = true;
            chkIsHasDetails.Location = new Point(136, 135);
            chkIsHasDetails.Name = "chkIsHasDetails";
            chkIsHasDetails.Size = new Size(150, 19);
            chkIsHasDetails.TabIndex = 76;
            chkIsHasDetails.Text = "هل توجد تفاصيل للحساب";
            chkIsHasDetails.UseVisualStyleBackColor = true;
            // 
            // lblParentTree
            // 
            lblParentTree.AutoSize = true;
            lblParentTree.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblParentTree.ForeColor = SystemColors.ActiveCaption;
            lblParentTree.Location = new Point(222, 165);
            lblParentTree.Margin = new Padding(4, 0, 4, 0);
            lblParentTree.Name = "lblParentTree";
            lblParentTree.Size = new Size(63, 22);
            lblParentTree.TabIndex = 58;
            lblParentTree.Text = "اسم الاب";
            lblParentTree.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblBalanceAndState
            // 
            lblBalanceAndState.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblBalanceAndState.ForeColor = SystemColors.ActiveCaption;
            lblBalanceAndState.Location = new Point(4, 198);
            lblBalanceAndState.Margin = new Padding(4, 0, 4, 0);
            lblBalanceAndState.Name = "lblBalanceAndState";
            lblBalanceAndState.Size = new Size(281, 29);
            lblBalanceAndState.TabIndex = 58;
            lblBalanceAndState.Text = "الرصيد";
            lblBalanceAndState.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblCreateByUserName
            // 
            lblCreateByUserName.AutoSize = true;
            lblCreateByUserName.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold);
            lblCreateByUserName.ForeColor = SystemColors.ActiveCaption;
            lblCreateByUserName.Location = new Point(232, 231);
            lblCreateByUserName.Name = "lblCreateByUserName";
            lblCreateByUserName.Size = new Size(54, 22);
            lblCreateByUserName.TabIndex = 75;
            lblCreateByUserName.Text = "المنشئ";
            lblCreateByUserName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cbxParentTree
            // 
            cbxParentTree.FormattingEnabled = true;
            cbxParentTree.Location = new Point(165, 300);
            cbxParentTree.Name = "cbxParentTree";
            cbxParentTree.Size = new Size(121, 23);
            cbxParentTree.TabIndex = 77;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold);
            label1.ForeColor = SystemColors.ActiveCaption;
            label1.Location = new Point(330, 297);
            label1.Name = "label1";
            label1.Size = new Size(110, 22);
            label1.TabIndex = 75;
            label1.Text = "نقل الى فرع اخر";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblTitetl_Item
            // 
            lblTitetl_Item.AutoSize = true;
            lblTitetl_Item.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitetl_Item.ForeColor = Color.FromArgb(0, 0, 192);
            lblTitetl_Item.Location = new Point(359, 0);
            lblTitetl_Item.Margin = new Padding(4, 0, 4, 0);
            lblTitetl_Item.Name = "lblTitetl_Item";
            lblTitetl_Item.Size = new Size(86, 22);
            lblTitetl_Item.TabIndex = 96;
            lblTitetl_Item.Text = "تعديل حساب";
            lblTitetl_Item.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel16
            // 
            tableLayoutPanel16.ColumnCount = 3;
            tableLayoutPanel16.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.7014618F));
            tableLayoutPanel16.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32.9853859F));
            tableLayoutPanel16.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel16.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel16.Controls.Add(btnSave, 2, 0);
            tableLayoutPanel16.Controls.Add(btnClose, 0, 0);
            tableLayoutPanel16.Location = new Point(3, 501);
            tableLayoutPanel16.Name = "tableLayoutPanel16";
            tableLayoutPanel16.RowCount = 1;
            tableLayoutPanel16.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel16.Size = new Size(443, 83);
            tableLayoutPanel16.TabIndex = 92;
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.Teal;
            btnSave.Font = new Font("Times New Roman", 16F, FontStyle.Bold);
            btnSave.ForeColor = Color.Yellow;
            btnSave.Location = new Point(4, 4);
            btnSave.Margin = new Padding(4);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(215, 75);
            btnSave.TabIndex = 83;
            btnSave.Text = "حفــظ";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.Yellow;
            btnClose.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnClose.Location = new Point(373, 4);
            btnClose.Margin = new Padding(4);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(66, 75);
            btnClose.TabIndex = 73;
            btnClose.TabStop = false;
            btnClose.Text = "اغلاق";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel16, 0, 2);
            tableLayoutPanel1.Controls.Add(lblTitetl_Item, 0, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel6, 0, 1);
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 4.940375F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 79.89779F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));
            tableLayoutPanel1.Size = new Size(449, 587);
            tableLayoutPanel1.TabIndex = 4;
            // 
            // frm_AccountModify
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(449, 587);
            Controls.Add(tableLayoutPanel1);
            Name = "frm_AccountModify";
            RightToLeft = RightToLeft.Yes;
            Text = "تعديل حساب";
            Load += frm_AccountModify_Load;
            tableLayoutPanel6.ResumeLayout(false);
            tableLayoutPanel6.PerformLayout();
            tableLayoutPanel16.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label lblIsEnerAcc;
        private Label label2;
        private TextBox txtAccName;
        private Label lblDateOfJoin;
        private CheckBox chkIsHidden;
        private TableLayoutPanel tableLayoutPanel6;
        private Label lblTitetl_Item;
        private TableLayoutPanel tableLayoutPanel16;
        private Button btnSave;
        private Button btnClose;
        private Label lblParentTree;
        private Label lblBalanceAndState;
        private Label lblCreateByUserName;
        private TableLayoutPanel tableLayoutPanel1;
        private CheckBox chkIsForManger;
        private CheckBox chkIsHasDetails;
        private ComboBox cbxParentTree;
        private Label lblTreeAccCode;
        private Label lblAccTypeID;
        private Label label1;
    }
}