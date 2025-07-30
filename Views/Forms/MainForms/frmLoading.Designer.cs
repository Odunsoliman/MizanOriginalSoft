namespace MizanOriginalSoft.Views.Forms.MainForms
{
    partial class frmLoading
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
            lblMessage = new Label();
            progressBar1 = new ProgressBar();
            SuspendLayout();
            // 
            // lblMessage
            // 
            lblMessage.BackColor = Color.FromArgb(255, 224, 192);
            lblMessage.Dock = DockStyle.Fill;
            lblMessage.Font = new Font("Times New Roman", 16.2F, FontStyle.Bold);
            lblMessage.ForeColor = Color.FromArgb(128, 128, 255);
            lblMessage.Location = new Point(0, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(474, 162);
            lblMessage.TabIndex = 0;
            lblMessage.Text = "label1";
            lblMessage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // progressBar1
            // 
            progressBar1.Dock = DockStyle.Bottom;
            progressBar1.Location = new Point(0, 133);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(474, 29);
            progressBar1.TabIndex = 1;
            // 
            // frmLoading
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(474, 162);
            Controls.Add(progressBar1);
            Controls.Add(lblMessage);
            Name = "frmLoading";
            Text = "frmLoading";
            ResumeLayout(false);
        }

        #endregion

        private Label lblMessage;
        private ProgressBar progressBar1;
    }
}