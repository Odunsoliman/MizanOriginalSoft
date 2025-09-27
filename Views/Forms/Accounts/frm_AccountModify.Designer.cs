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
            lblBalanceAndState = new Label();
            label4 = new Label();
            lblCreateByUserName = new Label();
            label6 = new Label();
            chkIsHasChildren = new CheckBox();
            lblAccountPath = new Label();
            tableLayoutPanelIsAssets = new TableLayoutPanel();
            txtFixedAssetsValue = new TextBox();
            label13 = new Label();
            chkIsEndedFixedAssets = new CheckBox();
            rdoIsEndedFixedAssets_No = new RadioButton();
            label20 = new Label();
            lblAnnuallyInstallment = new Label();
            label21 = new Label();
            txtFixedAssetsAge = new TextBox();
            label19 = new Label();
            txtDepreciationRateAnnually = new TextBox();
            label14 = new Label();
            lblMonthlyInstallment = new Label();
            rdoIsEndedFixedAssets_Yes = new RadioButton();
            lblFixedAssetsEndDate = new Label();
            label5 = new Label();
            label3 = new Label();
            lbl = new Label();
            lblErrorPhon = new Label();
            lblErrorAntherPhon = new Label();
            tableLayoutPanelMain = new TableLayoutPanel();
            lblTitetl_Item = new Label();
            tableLayoutPanel16 = new TableLayoutPanel();
            btnSave = new Button();
            btnClose = new Button();
            tlpData = new TableLayoutPanel();
            tableLayoutPanelHasDetait = new TableLayoutPanel();
            txtClientAddress = new TextBox();
            txtClientEmail = new TextBox();
            label17 = new Label();
            label1 = new Label();
            txtAntherPhon = new TextBox();
            txtAccNote = new TextBox();
            txtFirstPhon = new TextBox();
            tableLayoutPanel6.SuspendLayout();
            tableLayoutPanelIsAssets.SuspendLayout();
            tableLayoutPanelMain.SuspendLayout();
            tableLayoutPanel16.SuspendLayout();
            tlpData.SuspendLayout();
            tableLayoutPanelHasDetait.SuspendLayout();
            SuspendLayout();
            // 
            // lblIsEnerAcc
            // 
            lblIsEnerAcc.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblIsEnerAcc.ForeColor = SystemColors.ActiveCaption;
            lblIsEnerAcc.Location = new Point(257, 105);
            lblIsEnerAcc.Margin = new Padding(4, 0, 4, 0);
            lblIsEnerAcc.Name = "lblIsEnerAcc";
            lblIsEnerAcc.Size = new Size(92, 22);
            lblIsEnerAcc.TabIndex = 58;
            lblIsEnerAcc.Text = "هل حساب داخلى";
            lblIsEnerAcc.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.FromArgb(0, 0, 192);
            label2.Location = new Point(386, 0);
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
            txtAccName.Location = new Point(63, 4);
            txtAccName.Margin = new Padding(4);
            txtAccName.Name = "txtAccName";
            txtAccName.Size = new Size(286, 29);
            txtAccName.TabIndex = 47;
            txtAccName.TextAlign = HorizontalAlignment.Center;
            // 
            // lblDateOfJoin
            // 
            lblDateOfJoin.AutoSize = true;
            lblDateOfJoin.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold);
            lblDateOfJoin.ForeColor = SystemColors.ActiveCaption;
            lblDateOfJoin.Location = new Point(299, 140);
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
            chkIsHidden.Location = new Point(243, 38);
            chkIsHidden.Name = "chkIsHidden";
            chkIsHidden.Size = new Size(107, 26);
            chkIsHidden.TabIndex = 73;
            chkIsHidden.Text = "حساب مخفى";
            chkIsHidden.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel6
            // 
            tableLayoutPanel6.ColumnCount = 2;
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22.1757374F));
            tableLayoutPanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 61.50628F));
            tableLayoutPanel6.Controls.Add(lblBalanceAndState, 1, 7);
            tableLayoutPanel6.Controls.Add(lblIsEnerAcc, 1, 3);
            tableLayoutPanel6.Controls.Add(label2, 0, 0);
            tableLayoutPanel6.Controls.Add(txtAccName, 1, 0);
            tableLayoutPanel6.Controls.Add(lblDateOfJoin, 1, 4);
            tableLayoutPanel6.Controls.Add(label4, 0, 4);
            tableLayoutPanel6.Controls.Add(chkIsHidden, 1, 1);
            tableLayoutPanel6.Controls.Add(lblCreateByUserName, 1, 5);
            tableLayoutPanel6.Controls.Add(label6, 0, 5);
            tableLayoutPanel6.Controls.Add(chkIsHasChildren, 1, 2);
            tableLayoutPanel6.Controls.Add(lblAccountPath, 1, 6);
            tableLayoutPanel6.Dock = DockStyle.Fill;
            tableLayoutPanel6.Location = new Point(3, 61);
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
            tableLayoutPanel6.Size = new Size(479, 285);
            tableLayoutPanel6.TabIndex = 93;
            // 
            // lblBalanceAndState
            // 
            lblBalanceAndState.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblBalanceAndState.ForeColor = SystemColors.ActiveCaption;
            lblBalanceAndState.Location = new Point(257, 245);
            lblBalanceAndState.Margin = new Padding(4, 0, 4, 0);
            lblBalanceAndState.Name = "lblBalanceAndState";
            lblBalanceAndState.Size = new Size(92, 22);
            lblBalanceAndState.TabIndex = 58;
            lblBalanceAndState.Text = "الرصيد";
            lblBalanceAndState.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.ForeColor = SystemColors.ActiveCaption;
            label4.Location = new Point(383, 140);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(92, 22);
            label4.TabIndex = 58;
            label4.Text = "تم الانشاء فى";
            label4.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblCreateByUserName
            // 
            lblCreateByUserName.AutoSize = true;
            lblCreateByUserName.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold);
            lblCreateByUserName.ForeColor = SystemColors.ActiveCaption;
            lblCreateByUserName.Location = new Point(296, 175);
            lblCreateByUserName.Name = "lblCreateByUserName";
            lblCreateByUserName.Size = new Size(54, 22);
            lblCreateByUserName.TabIndex = 75;
            lblCreateByUserName.Text = "المنشئ";
            lblCreateByUserName.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.ForeColor = SystemColors.ActiveCaption;
            label6.Location = new Point(395, 175);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(80, 22);
            label6.TabIndex = 58;
            label6.Text = "قام بالانشاء";
            label6.TextAlign = ContentAlignment.MiddleRight;
            // 
            // chkIsHasChildren
            // 
            chkIsHasChildren.AutoSize = true;
            chkIsHasChildren.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold);
            chkIsHasChildren.ForeColor = Color.FromArgb(0, 0, 192);
            chkIsHasChildren.Location = new Point(228, 73);
            chkIsHasChildren.Name = "chkIsHasChildren";
            chkIsHasChildren.Size = new Size(122, 26);
            chkIsHasChildren.TabIndex = 73;
            chkIsHasChildren.Text = "حساب له فروع";
            chkIsHasChildren.UseVisualStyleBackColor = true;
            // 
            // lblAccountPath
            // 
            lblAccountPath.AutoSize = true;
            lblAccountPath.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold);
            lblAccountPath.ForeColor = SystemColors.ActiveCaption;
            lblAccountPath.Location = new Point(300, 210);
            lblAccountPath.Name = "lblAccountPath";
            lblAccountPath.Size = new Size(50, 22);
            lblAccountPath.TabIndex = 75;
            lblAccountPath.Text = "المسار";
            lblAccountPath.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanelIsAssets
            // 
            tableLayoutPanelIsAssets.AutoSize = true;
            tableLayoutPanelIsAssets.BackColor = Color.Transparent;
            tableLayoutPanelIsAssets.ColumnCount = 3;
            tableLayoutPanelIsAssets.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 37.0689659F));
            tableLayoutPanelIsAssets.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24.3534489F));
            tableLayoutPanelIsAssets.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38.5775871F));
            tableLayoutPanelIsAssets.Controls.Add(txtFixedAssetsValue, 1, 1);
            tableLayoutPanelIsAssets.Controls.Add(label13, 0, 1);
            tableLayoutPanelIsAssets.Controls.Add(chkIsEndedFixedAssets, 1, 0);
            tableLayoutPanelIsAssets.Controls.Add(rdoIsEndedFixedAssets_No, 1, 6);
            tableLayoutPanelIsAssets.Controls.Add(label20, 0, 5);
            tableLayoutPanelIsAssets.Controls.Add(lblAnnuallyInstallment, 1, 5);
            tableLayoutPanelIsAssets.Controls.Add(label21, 0, 3);
            tableLayoutPanelIsAssets.Controls.Add(txtFixedAssetsAge, 1, 3);
            tableLayoutPanelIsAssets.Controls.Add(label19, 0, 2);
            tableLayoutPanelIsAssets.Controls.Add(txtDepreciationRateAnnually, 1, 2);
            tableLayoutPanelIsAssets.Controls.Add(label14, 0, 4);
            tableLayoutPanelIsAssets.Controls.Add(lblMonthlyInstallment, 1, 4);
            tableLayoutPanelIsAssets.Controls.Add(rdoIsEndedFixedAssets_Yes, 1, 7);
            tableLayoutPanelIsAssets.Controls.Add(lblFixedAssetsEndDate, 2, 7);
            tableLayoutPanelIsAssets.Dock = DockStyle.Top;
            tableLayoutPanelIsAssets.Location = new Point(3, 126);
            tableLayoutPanelIsAssets.Name = "tableLayoutPanelIsAssets";
            tableLayoutPanelIsAssets.RowCount = 8;
            tableLayoutPanelIsAssets.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanelIsAssets.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanelIsAssets.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanelIsAssets.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanelIsAssets.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanelIsAssets.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanelIsAssets.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanelIsAssets.RowStyles.Add(new RowStyle(SizeType.Percent, 14.2857141F));
            tableLayoutPanelIsAssets.Size = new Size(473, 57);
            tableLayoutPanelIsAssets.TabIndex = 4;
            // 
            // txtFixedAssetsValue
            // 
            txtFixedAssetsValue.Dock = DockStyle.Fill;
            txtFixedAssetsValue.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtFixedAssetsValue.ForeColor = Color.FromArgb(0, 0, 192);
            txtFixedAssetsValue.Location = new Point(187, 24);
            txtFixedAssetsValue.Margin = new Padding(4);
            txtFixedAssetsValue.Name = "txtFixedAssetsValue";
            txtFixedAssetsValue.Size = new Size(107, 29);
            txtFixedAssetsValue.TabIndex = 80;
            txtFixedAssetsValue.Text = "0";
            txtFixedAssetsValue.TextAlign = HorizontalAlignment.Center;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Dock = DockStyle.Fill;
            label13.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label13.ForeColor = Color.FromArgb(0, 0, 192);
            label13.Location = new Point(302, 20);
            label13.Margin = new Padding(4, 0, 4, 0);
            label13.Name = "label13";
            label13.Size = new Size(167, 5);
            label13.TabIndex = 69;
            label13.Text = "قيمة شراء الاصل";
            label13.TextAlign = ContentAlignment.MiddleRight;
            // 
            // chkIsEndedFixedAssets
            // 
            chkIsEndedFixedAssets.AutoSize = true;
            chkIsEndedFixedAssets.Enabled = false;
            chkIsEndedFixedAssets.Location = new Point(280, 3);
            chkIsEndedFixedAssets.Name = "chkIsEndedFixedAssets";
            chkIsEndedFixedAssets.Size = new Size(15, 14);
            chkIsEndedFixedAssets.TabIndex = 88;
            chkIsEndedFixedAssets.UseVisualStyleBackColor = true;
            chkIsEndedFixedAssets.Visible = false;
            // 
            // rdoIsEndedFixedAssets_No
            // 
            rdoIsEndedFixedAssets_No.AutoSize = true;
            rdoIsEndedFixedAssets_No.Checked = true;
            rdoIsEndedFixedAssets_No.Dock = DockStyle.Fill;
            rdoIsEndedFixedAssets_No.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            rdoIsEndedFixedAssets_No.ForeColor = Color.FromArgb(0, 0, 192);
            rdoIsEndedFixedAssets_No.Location = new Point(186, 48);
            rdoIsEndedFixedAssets_No.Name = "rdoIsEndedFixedAssets_No";
            rdoIsEndedFixedAssets_No.Size = new Size(109, 1);
            rdoIsEndedFixedAssets_No.TabIndex = 0;
            rdoIsEndedFixedAssets_No.TabStop = true;
            rdoIsEndedFixedAssets_No.Text = "تحت الشغيل";
            rdoIsEndedFixedAssets_No.UseVisualStyleBackColor = true;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Dock = DockStyle.Fill;
            label20.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label20.ForeColor = Color.FromArgb(0, 0, 192);
            label20.Location = new Point(302, 40);
            label20.Margin = new Padding(4, 0, 4, 0);
            label20.Name = "label20";
            label20.Size = new Size(167, 5);
            label20.TabIndex = 79;
            label20.Text = "قسط سنوى";
            label20.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblAnnuallyInstallment
            // 
            lblAnnuallyInstallment.AutoSize = true;
            lblAnnuallyInstallment.Dock = DockStyle.Fill;
            lblAnnuallyInstallment.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAnnuallyInstallment.ForeColor = Color.FromArgb(0, 0, 192);
            lblAnnuallyInstallment.Location = new Point(187, 40);
            lblAnnuallyInstallment.Margin = new Padding(4, 0, 4, 0);
            lblAnnuallyInstallment.Name = "lblAnnuallyInstallment";
            lblAnnuallyInstallment.Size = new Size(107, 5);
            lblAnnuallyInstallment.TabIndex = 80;
            lblAnnuallyInstallment.Text = "0";
            lblAnnuallyInstallment.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Dock = DockStyle.Fill;
            label21.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label21.ForeColor = Color.FromArgb(0, 0, 192);
            label21.Location = new Point(302, 30);
            label21.Margin = new Padding(4, 0, 4, 0);
            label21.Name = "label21";
            label21.Size = new Size(167, 5);
            label21.TabIndex = 75;
            label21.Text = "العمر الافتراضى بالشهر";
            label21.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtFixedAssetsAge
            // 
            txtFixedAssetsAge.Dock = DockStyle.Fill;
            txtFixedAssetsAge.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtFixedAssetsAge.ForeColor = Color.FromArgb(0, 0, 192);
            txtFixedAssetsAge.Location = new Point(187, 34);
            txtFixedAssetsAge.Margin = new Padding(4);
            txtFixedAssetsAge.Name = "txtFixedAssetsAge";
            txtFixedAssetsAge.Size = new Size(107, 29);
            txtFixedAssetsAge.TabIndex = 83;
            txtFixedAssetsAge.Text = "0";
            txtFixedAssetsAge.TextAlign = HorizontalAlignment.Center;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Dock = DockStyle.Fill;
            label19.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label19.ForeColor = Color.FromArgb(0, 0, 192);
            label19.Location = new Point(302, 25);
            label19.Margin = new Padding(4, 0, 4, 0);
            label19.Name = "label19";
            label19.Size = new Size(167, 5);
            label19.TabIndex = 73;
            label19.Text = "اهلاك سنوى%";
            label19.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtDepreciationRateAnnually
            // 
            txtDepreciationRateAnnually.Dock = DockStyle.Fill;
            txtDepreciationRateAnnually.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtDepreciationRateAnnually.ForeColor = Color.FromArgb(0, 0, 192);
            txtDepreciationRateAnnually.Location = new Point(187, 29);
            txtDepreciationRateAnnually.Margin = new Padding(4);
            txtDepreciationRateAnnually.Name = "txtDepreciationRateAnnually";
            txtDepreciationRateAnnually.Size = new Size(107, 29);
            txtDepreciationRateAnnually.TabIndex = 82;
            txtDepreciationRateAnnually.Text = "0";
            txtDepreciationRateAnnually.TextAlign = HorizontalAlignment.Center;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Dock = DockStyle.Fill;
            label14.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label14.ForeColor = Color.FromArgb(0, 0, 192);
            label14.Location = new Point(302, 35);
            label14.Margin = new Padding(4, 0, 4, 0);
            label14.Name = "label14";
            label14.Size = new Size(167, 5);
            label14.TabIndex = 79;
            label14.Text = "قسط شهرى";
            label14.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblMonthlyInstallment
            // 
            lblMonthlyInstallment.AutoSize = true;
            lblMonthlyInstallment.Dock = DockStyle.Fill;
            lblMonthlyInstallment.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblMonthlyInstallment.ForeColor = Color.FromArgb(0, 0, 192);
            lblMonthlyInstallment.Location = new Point(187, 35);
            lblMonthlyInstallment.Margin = new Padding(4, 0, 4, 0);
            lblMonthlyInstallment.Name = "lblMonthlyInstallment";
            lblMonthlyInstallment.Size = new Size(107, 5);
            lblMonthlyInstallment.TabIndex = 81;
            lblMonthlyInstallment.Text = "0";
            lblMonthlyInstallment.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // rdoIsEndedFixedAssets_Yes
            // 
            rdoIsEndedFixedAssets_Yes.AutoSize = true;
            rdoIsEndedFixedAssets_Yes.Dock = DockStyle.Fill;
            rdoIsEndedFixedAssets_Yes.Font = new Font("Times New Roman", 12F, FontStyle.Bold);
            rdoIsEndedFixedAssets_Yes.ForeColor = Color.FromArgb(0, 0, 192);
            rdoIsEndedFixedAssets_Yes.Location = new Point(186, 53);
            rdoIsEndedFixedAssets_Yes.Name = "rdoIsEndedFixedAssets_Yes";
            rdoIsEndedFixedAssets_Yes.Size = new Size(109, 1);
            rdoIsEndedFixedAssets_Yes.TabIndex = 0;
            rdoIsEndedFixedAssets_Yes.Text = "منتهى/كهنة";
            rdoIsEndedFixedAssets_Yes.UseVisualStyleBackColor = true;
            // 
            // lblFixedAssetsEndDate
            // 
            lblFixedAssetsEndDate.AutoSize = true;
            lblFixedAssetsEndDate.Location = new Point(142, 50);
            lblFixedAssetsEndDate.Name = "lblFixedAssetsEndDate";
            lblFixedAssetsEndDate.Size = new Size(38, 7);
            lblFixedAssetsEndDate.TabIndex = 89;
            lblFixedAssetsEndDate.Text = "label4";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Right;
            label5.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.ForeColor = Color.FromArgb(0, 0, 192);
            label5.Location = new Point(347, 51);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(39, 32);
            label5.TabIndex = 67;
            label5.Text = "مذكرة";
            label5.TextAlign = ContentAlignment.TopRight;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.FromArgb(0, 0, 192);
            label3.Location = new Point(425, 19);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(44, 16);
            label3.TabIndex = 65;
            label3.Text = "الهاتف";
            label3.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbl
            // 
            lbl.AutoSize = true;
            lbl.Dock = DockStyle.Right;
            lbl.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lbl.ForeColor = Color.FromArgb(0, 0, 192);
            lbl.Location = new Point(347, 35);
            lbl.Margin = new Padding(4, 0, 4, 0);
            lbl.Name = "lbl";
            lbl.Size = new Size(60, 16);
            lbl.TabIndex = 66;
            lbl.Text = "هاتف آخر";
            lbl.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblErrorPhon
            // 
            lblErrorPhon.AutoSize = true;
            lblErrorPhon.Dock = DockStyle.Fill;
            lblErrorPhon.ForeColor = Color.Red;
            lblErrorPhon.Location = new Point(3, 19);
            lblErrorPhon.Name = "lblErrorPhon";
            lblErrorPhon.Size = new Size(59, 16);
            lblErrorPhon.TabIndex = 73;
            lblErrorPhon.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblErrorAntherPhon
            // 
            lblErrorAntherPhon.AutoSize = true;
            lblErrorAntherPhon.Dock = DockStyle.Fill;
            lblErrorAntherPhon.ForeColor = Color.Red;
            lblErrorAntherPhon.Location = new Point(3, 35);
            lblErrorAntherPhon.Name = "lblErrorAntherPhon";
            lblErrorAntherPhon.Size = new Size(59, 16);
            lblErrorAntherPhon.TabIndex = 74;
            lblErrorAntherPhon.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanelMain
            // 
            tableLayoutPanelMain.ColumnCount = 1;
            tableLayoutPanelMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelMain.Controls.Add(lblTitetl_Item, 0, 0);
            tableLayoutPanelMain.Controls.Add(tableLayoutPanel16, 0, 4);
            tableLayoutPanelMain.Controls.Add(tlpData, 0, 3);
            tableLayoutPanelMain.Controls.Add(tableLayoutPanel6, 0, 1);
            tableLayoutPanelMain.Dock = DockStyle.Fill;
            tableLayoutPanelMain.Location = new Point(0, 0);
            tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            tableLayoutPanelMain.RowCount = 5;
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 9.433963F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 47.1698151F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 1.88679278F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 31.1320763F));
            tableLayoutPanelMain.RowStyles.Add(new RowStyle(SizeType.Percent, 10.3773594F));
            tableLayoutPanelMain.Size = new Size(485, 617);
            tableLayoutPanelMain.TabIndex = 3;
            // 
            // lblTitetl_Item
            // 
            lblTitetl_Item.AutoSize = true;
            lblTitetl_Item.Dock = DockStyle.Fill;
            lblTitetl_Item.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitetl_Item.ForeColor = Color.FromArgb(0, 0, 192);
            lblTitetl_Item.Location = new Point(4, 0);
            lblTitetl_Item.Margin = new Padding(4, 0, 4, 0);
            lblTitetl_Item.Name = "lblTitetl_Item";
            lblTitetl_Item.Size = new Size(477, 58);
            lblTitetl_Item.TabIndex = 96;
            lblTitetl_Item.Text = "تعديل حساب";
            lblTitetl_Item.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel16
            // 
            tableLayoutPanel16.ColumnCount = 3;
            tableLayoutPanel16.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel16.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel16.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel16.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel16.Controls.Add(btnSave, 2, 0);
            tableLayoutPanel16.Controls.Add(btnClose, 0, 0);
            tableLayoutPanel16.Dock = DockStyle.Fill;
            tableLayoutPanel16.Location = new Point(3, 555);
            tableLayoutPanel16.Name = "tableLayoutPanel16";
            tableLayoutPanel16.RowCount = 1;
            tableLayoutPanel16.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel16.Size = new Size(479, 59);
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
            btnSave.Size = new Size(233, 51);
            btnSave.TabIndex = 83;
            btnSave.Text = "حفــظ";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.Yellow;
            btnClose.Dock = DockStyle.Fill;
            btnClose.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnClose.Location = new Point(364, 4);
            btnClose.Margin = new Padding(4);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(111, 51);
            btnClose.TabIndex = 73;
            btnClose.TabStop = false;
            btnClose.Text = "اغلاق";
            btnClose.UseVisualStyleBackColor = false;
            // 
            // tlpData
            // 
            tlpData.ColumnCount = 1;
            tlpData.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpData.Controls.Add(tableLayoutPanelHasDetait, 0, 0);
            tlpData.Controls.Add(tableLayoutPanelIsAssets, 0, 1);
            tlpData.Dock = DockStyle.Fill;
            tlpData.Location = new Point(3, 363);
            tlpData.Name = "tlpData";
            tlpData.RowCount = 2;
            tlpData.RowStyles.Add(new RowStyle(SizeType.Percent, 66.20879F));
            tlpData.RowStyles.Add(new RowStyle(SizeType.Percent, 33.79121F));
            tlpData.Size = new Size(479, 186);
            tlpData.TabIndex = 95;
            // 
            // tableLayoutPanelHasDetait
            // 
            tableLayoutPanelHasDetait.AutoSize = true;
            tableLayoutPanelHasDetait.BackColor = Color.Transparent;
            tableLayoutPanelHasDetait.ColumnCount = 3;
            tableLayoutPanelHasDetait.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 31.8807335F));
            tableLayoutPanelHasDetait.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68.11926F));
            tableLayoutPanelHasDetait.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 64F));
            tableLayoutPanelHasDetait.Controls.Add(txtClientAddress, 1, 5);
            tableLayoutPanelHasDetait.Controls.Add(txtClientEmail, 1, 4);
            tableLayoutPanelHasDetait.Controls.Add(label17, 0, 5);
            tableLayoutPanelHasDetait.Controls.Add(label1, 0, 4);
            tableLayoutPanelHasDetait.Controls.Add(txtAntherPhon, 1, 2);
            tableLayoutPanelHasDetait.Controls.Add(txtAccNote, 1, 3);
            tableLayoutPanelHasDetait.Controls.Add(txtFirstPhon, 1, 1);
            tableLayoutPanelHasDetait.Controls.Add(label5, 0, 3);
            tableLayoutPanelHasDetait.Controls.Add(label3, 0, 1);
            tableLayoutPanelHasDetait.Controls.Add(lbl, 0, 2);
            tableLayoutPanelHasDetait.Controls.Add(lblErrorPhon, 2, 1);
            tableLayoutPanelHasDetait.Controls.Add(lblErrorAntherPhon, 2, 2);
            tableLayoutPanelHasDetait.Dock = DockStyle.Top;
            tableLayoutPanelHasDetait.Location = new Point(3, 3);
            tableLayoutPanelHasDetait.Name = "tableLayoutPanelHasDetait";
            tableLayoutPanelHasDetait.RowCount = 6;
            tableLayoutPanelHasDetait.RowStyles.Add(new RowStyle(SizeType.Absolute, 19F));
            tableLayoutPanelHasDetait.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66667F));
            tableLayoutPanelHasDetait.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66667F));
            tableLayoutPanelHasDetait.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33333F));
            tableLayoutPanelHasDetait.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66667F));
            tableLayoutPanelHasDetait.RowStyles.Add(new RowStyle(SizeType.Percent, 16.66667F));
            tableLayoutPanelHasDetait.Size = new Size(473, 117);
            tableLayoutPanelHasDetait.TabIndex = 2;
            // 
            // txtClientAddress
            // 
            txtClientAddress.Dock = DockStyle.Fill;
            txtClientAddress.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtClientAddress.ForeColor = Color.FromArgb(0, 0, 192);
            txtClientAddress.Location = new Point(69, 103);
            txtClientAddress.Margin = new Padding(4);
            txtClientAddress.Name = "txtClientAddress";
            txtClientAddress.Size = new Size(270, 29);
            txtClientAddress.TabIndex = 72;
            txtClientAddress.TextAlign = HorizontalAlignment.Center;
            // 
            // txtClientEmail
            // 
            txtClientEmail.Dock = DockStyle.Fill;
            txtClientEmail.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtClientEmail.ForeColor = Color.FromArgb(0, 0, 192);
            txtClientEmail.Location = new Point(69, 87);
            txtClientEmail.Margin = new Padding(4);
            txtClientEmail.Name = "txtClientEmail";
            txtClientEmail.Size = new Size(270, 29);
            txtClientEmail.TabIndex = 71;
            txtClientEmail.TextAlign = HorizontalAlignment.Center;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Dock = DockStyle.Right;
            label17.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label17.ForeColor = Color.FromArgb(0, 0, 192);
            label17.Location = new Point(347, 99);
            label17.Margin = new Padding(4, 0, 4, 0);
            label17.Name = "label17";
            label17.Size = new Size(42, 18);
            label17.TabIndex = 70;
            label17.Text = "عنوان";
            label17.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Right;
            label1.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.FromArgb(0, 0, 192);
            label1.Location = new Point(347, 83);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(78, 16);
            label1.TabIndex = 68;
            label1.Text = "بريد الكترونى";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtAntherPhon
            // 
            txtAntherPhon.Dock = DockStyle.Fill;
            txtAntherPhon.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtAntherPhon.ForeColor = Color.FromArgb(0, 0, 192);
            txtAntherPhon.Location = new Point(69, 39);
            txtAntherPhon.Margin = new Padding(4);
            txtAntherPhon.Name = "txtAntherPhon";
            txtAntherPhon.Size = new Size(270, 29);
            txtAntherPhon.TabIndex = 49;
            txtAntherPhon.TextAlign = HorizontalAlignment.Center;
            // 
            // txtAccNote
            // 
            txtAccNote.Dock = DockStyle.Fill;
            txtAccNote.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtAccNote.ForeColor = Color.FromArgb(0, 0, 192);
            txtAccNote.Location = new Point(69, 55);
            txtAccNote.Margin = new Padding(4);
            txtAccNote.Multiline = true;
            txtAccNote.Name = "txtAccNote";
            txtAccNote.Size = new Size(270, 24);
            txtAccNote.TabIndex = 50;
            txtAccNote.TextAlign = HorizontalAlignment.Center;
            // 
            // txtFirstPhon
            // 
            txtFirstPhon.Dock = DockStyle.Fill;
            txtFirstPhon.Font = new Font("Times New Roman", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtFirstPhon.ForeColor = Color.FromArgb(0, 0, 192);
            txtFirstPhon.Location = new Point(69, 23);
            txtFirstPhon.Margin = new Padding(4);
            txtFirstPhon.Name = "txtFirstPhon";
            txtFirstPhon.Size = new Size(270, 29);
            txtFirstPhon.TabIndex = 48;
            txtFirstPhon.TextAlign = HorizontalAlignment.Center;
            // 
            // frm_AccountModify
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(485, 617);
            Controls.Add(tableLayoutPanelMain);
            Name = "frm_AccountModify";
            RightToLeft = RightToLeft.Yes;
            Text = "تعديل حساب";
            Load += frm_AccountModify_Load;
            tableLayoutPanel6.ResumeLayout(false);
            tableLayoutPanel6.PerformLayout();
            tableLayoutPanelIsAssets.ResumeLayout(false);
            tableLayoutPanelIsAssets.PerformLayout();
            tableLayoutPanelMain.ResumeLayout(false);
            tableLayoutPanelMain.PerformLayout();
            tableLayoutPanel16.ResumeLayout(false);
            tlpData.ResumeLayout(false);
            tlpData.PerformLayout();
            tableLayoutPanelHasDetait.ResumeLayout(false);
            tableLayoutPanelHasDetait.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label lblIsEnerAcc;
        private Label label2;
        private TextBox txtAccName;
        private Label lblDateOfJoin;
        private CheckBox chkIsHidden;
        private TableLayoutPanel tableLayoutPanel6;
        private TableLayoutPanel tableLayoutPanelIsAssets;
        private TextBox txtFixedAssetsValue;
        private Label label13;
        private CheckBox chkIsEndedFixedAssets;
        private RadioButton rdoIsEndedFixedAssets_No;
        private Label label20;
        private Label lblAnnuallyInstallment;
        private Label label21;
        private TextBox txtFixedAssetsAge;
        private Label label19;
        private TextBox txtDepreciationRateAnnually;
        private Label label14;
        private Label lblMonthlyInstallment;
        private RadioButton rdoIsEndedFixedAssets_Yes;
        private Label lblFixedAssetsEndDate;
        private Label label5;
        private Label label3;
        private Label lbl;
        private Label lblErrorPhon;
        private Label lblErrorAntherPhon;
        private TableLayoutPanel tableLayoutPanelMain;
        private Label lblTitetl_Item;
        private TableLayoutPanel tableLayoutPanel16;
        private Button btnSave;
        private Button btnClose;
        private TableLayoutPanel tlpData;
        private TableLayoutPanel tableLayoutPanelHasDetait;
        private TextBox txtClientAddress;
        private TextBox txtClientEmail;
        private Label label17;
        private Label label1;
        private TextBox txtAntherPhon;
        private TextBox txtAccNote;
        private TextBox txtFirstPhon;
        private Label lblAccountPath;
        private CheckBox chkIsHasChildren;
        private Label label4;
        private Label lblBalanceAndState;
        private Label label6;
        private Label lblCreateByUserName;
    }
}