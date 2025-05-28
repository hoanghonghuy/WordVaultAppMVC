namespace WordVaultAppMVC.Views
{
    partial class TopicVocabularyForm
    {
        private System.ComponentModel.IContainer components = null;

        // Panels
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Panel pnlFooter;

        // Controls in Header
        private System.Windows.Forms.Label lblTopicTitle;

        // Controls in Content
        private System.Windows.Forms.ListBox lstVocabulary;
        private System.Windows.Forms.TextBox txtNewVocabulary;
        private System.Windows.Forms.Button btnAddVocabulary;
        private System.Windows.Forms.Button btnRemoveVocabulary;

        // Controls in Footer
        private System.Windows.Forms.Label lblTotalVocabulary;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblTopicTitle = new System.Windows.Forms.Label();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.lstVocabulary = new System.Windows.Forms.ListBox();
            this.txtNewVocabulary = new System.Windows.Forms.TextBox();
            this.btnAddVocabulary = new System.Windows.Forms.Button();
            this.btnRemoveVocabulary = new System.Windows.Forms.Button();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.lblTotalVocabulary = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.pnlHeader.Controls.Add(this.lblTopicTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(601, 60);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblTopicTitle
            // 
            this.lblTopicTitle.AutoSize = true;
            this.lblTopicTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTopicTitle.ForeColor = System.Drawing.Color.White;
            this.lblTopicTitle.Location = new System.Drawing.Point(20, 15);
            this.lblTopicTitle.Name = "lblTopicTitle";
            this.lblTopicTitle.Size = new System.Drawing.Size(326, 45);
            this.lblTopicTitle.TabIndex = 0;
            this.lblTopicTitle.Text = "Chủ đề: [Tên chủ đề]";
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.lstVocabulary);
            this.pnlContent.Controls.Add(this.txtNewVocabulary);
            this.pnlContent.Controls.Add(this.btnAddVocabulary);
            this.pnlContent.Controls.Add(this.btnRemoveVocabulary);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 60);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(20);
            this.pnlContent.Size = new System.Drawing.Size(601, 310);
            this.pnlContent.TabIndex = 1;
            // 
            // lstVocabulary
            // 
            this.lstVocabulary.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lstVocabulary.FormattingEnabled = true;
            this.lstVocabulary.ItemHeight = 32;
            this.lstVocabulary.Location = new System.Drawing.Point(20, 20);
            this.lstVocabulary.Name = "lstVocabulary";
            this.lstVocabulary.Size = new System.Drawing.Size(560, 132);
            this.lstVocabulary.TabIndex = 0;
            // 
            // txtNewVocabulary
            // 
            this.txtNewVocabulary.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtNewVocabulary.Location = new System.Drawing.Point(20, 190);
            this.txtNewVocabulary.Name = "txtNewVocabulary";
            this.txtNewVocabulary.Size = new System.Drawing.Size(360, 39);
            this.txtNewVocabulary.TabIndex = 1;
            // 
            // btnAddVocabulary
            // 
            this.btnAddVocabulary.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnAddVocabulary.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddVocabulary.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnAddVocabulary.ForeColor = System.Drawing.Color.White;
            this.btnAddVocabulary.Location = new System.Drawing.Point(400, 190);
            this.btnAddVocabulary.Name = "btnAddVocabulary";
            this.btnAddVocabulary.Size = new System.Drawing.Size(180, 39);
            this.btnAddVocabulary.TabIndex = 2;
            this.btnAddVocabulary.Text = "Thêm từ";
            this.btnAddVocabulary.UseVisualStyleBackColor = false;
            this.btnAddVocabulary.Click += new System.EventHandler(this.btnAddVocabulary_Click);
            // 
            // btnRemoveVocabulary
            // 
            this.btnRemoveVocabulary.BackColor = System.Drawing.Color.Firebrick;
            this.btnRemoveVocabulary.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveVocabulary.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnRemoveVocabulary.ForeColor = System.Drawing.Color.White;
            this.btnRemoveVocabulary.Location = new System.Drawing.Point(20, 240);
            this.btnRemoveVocabulary.Name = "btnRemoveVocabulary";
            this.btnRemoveVocabulary.Size = new System.Drawing.Size(204, 47);
            this.btnRemoveVocabulary.TabIndex = 3;
            this.btnRemoveVocabulary.Text = "Xoá từ đã chọn";
            this.btnRemoveVocabulary.UseVisualStyleBackColor = false;
            this.btnRemoveVocabulary.Click += new System.EventHandler(this.btnRemoveVocabulary_Click);
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.pnlFooter.Controls.Add(this.lblTotalVocabulary);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 370);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(601, 40);
            this.pnlFooter.TabIndex = 2;
            // 
            // lblTotalVocabulary
            // 
            this.lblTotalVocabulary.AutoSize = true;
            this.lblTotalVocabulary.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTotalVocabulary.ForeColor = System.Drawing.Color.Gray;
            this.lblTotalVocabulary.Location = new System.Drawing.Point(20, 10);
            this.lblTotalVocabulary.Name = "lblTotalVocabulary";
            this.lblTotalVocabulary.Size = new System.Drawing.Size(171, 28);
            this.lblTotalVocabulary.TabIndex = 0;
            this.lblTotalVocabulary.Text = "Tổng số từ: [số từ]";
            // 
            // TopicVocabularyForm
            // 
            this.ClientSize = new System.Drawing.Size(601, 410);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlFooter);
            this.Name = "TopicVocabularyForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản lý Từ Vựng theo Chủ Đề";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TopicVocabularyForm_FormClosing);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();
            this.pnlFooter.ResumeLayout(false);
            this.pnlFooter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
    }
}
