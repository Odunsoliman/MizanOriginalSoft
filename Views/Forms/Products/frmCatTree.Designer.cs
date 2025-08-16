namespace MizanOriginalSoft.Views.Forms.Products
{
    partial class frmCatTree
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
            txtSearchTree = new TextBox();
            label1 = new Label();
            btnSave = new Button();
            treeViewCategories = new TreeView();
            lblSelectedTreeNod = new Label();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(txtSearchTree, 0, 1);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(btnSave, 0, 4);
            tableLayoutPanel1.Controls.Add(treeViewCategories, 0, 3);
            tableLayoutPanel1.Controls.Add(lblSelectedTreeNod, 0, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 4F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 6F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 6F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 76F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 8F));
            tableLayoutPanel1.Size = new Size(347, 671);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // txtSearchTree
            // 
            txtSearchTree.Dock = DockStyle.Fill;
            txtSearchTree.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold);
            txtSearchTree.Location = new Point(3, 29);
            txtSearchTree.Name = "txtSearchTree";
            txtSearchTree.Size = new Size(341, 29);
            txtSearchTree.TabIndex = 46;
            txtSearchTree.TextAlign = HorizontalAlignment.Center;
            txtSearchTree.TextChanged += txtSearchTree_TextChanged;
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.FromArgb(192, 0, 0);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(341, 26);
            label1.TabIndex = 45;
            label1.Text = "بحث";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.FromArgb(255, 255, 192);
            btnSave.Dock = DockStyle.Fill;
            btnSave.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSave.ForeColor = Color.FromArgb(0, 0, 192);
            btnSave.Location = new Point(4, 619);
            btnSave.Margin = new Padding(4);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(339, 48);
            btnSave.TabIndex = 44;
            btnSave.TabStop = false;
            btnSave.Text = "حفظ ";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // treeViewCategories
            // 
            treeViewCategories.BackColor = SystemColors.Control;
            treeViewCategories.BorderStyle = BorderStyle.None;
            treeViewCategories.Dock = DockStyle.Fill;
            treeViewCategories.Font = new Font("Times New Roman", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            treeViewCategories.LineColor = Color.Aqua;
            treeViewCategories.Location = new Point(3, 109);
            treeViewCategories.Name = "treeViewCategories";
            treeViewCategories.RightToLeft = RightToLeft.Yes;
            treeViewCategories.RightToLeftLayout = true;
            treeViewCategories.Size = new Size(341, 503);
            treeViewCategories.TabIndex = 5;
            treeViewCategories.AfterSelect += treeViewCategories_AfterSelect;
            // 
            // lblSelectedTreeNod
            // 
            lblSelectedTreeNod.BackColor = Color.Transparent;
            lblSelectedTreeNod.Dock = DockStyle.Fill;
            lblSelectedTreeNod.Font = new Font("Times New Roman", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSelectedTreeNod.ForeColor = Color.FromArgb(192, 0, 0);
            lblSelectedTreeNod.Location = new Point(3, 66);
            lblSelectedTreeNod.Name = "lblSelectedTreeNod";
            lblSelectedTreeNod.Size = new Size(341, 40);
            lblSelectedTreeNod.TabIndex = 42;
            lblSelectedTreeNod.Text = "الكل";
            lblSelectedTreeNod.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // frmCatTree
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(347, 671);
            Controls.Add(tableLayoutPanel1);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmCatTree";
            RightToLeftLayout = true;
            StartPosition = FormStartPosition.CenterScreen;
            Load += frmCatTree_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TreeView treeViewCategories;
        private Label lblSelectedTreeNod;
        private Button btnSave;
        private Label label1;
        private TextBox txtSearchTree;
    }
}