namespace WordVaultAppMVC.Views
{
    partial class VocabularyListForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ListBox listBoxVocabulary;
        private WordVaultAppMVC.Views.VocabularyDetailPanel vocabularyDetailPanel;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblTotalVocabulary;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.listBoxVocabulary = new System.Windows.Forms.ListBox();
            // Khởi tạo vocabularyDetailPanel
            this.vocabularyDetailPanel = new WordVaultAppMVC.Views.VocabularyDetailPanel();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblTotalVocabulary = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listBoxVocabulary
            // 
            this.listBoxVocabulary.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.listBoxVocabulary.FormattingEnabled = true;
            this.listBoxVocabulary.ItemHeight = 23;
            this.listBoxVocabulary.Location = new System.Drawing.Point(20, 20);
            this.listBoxVocabulary.Name = "listBoxVocabulary";
            this.listBoxVocabulary.Size = new System.Drawing.Size(220, 372);
            this.listBoxVocabulary.TabIndex = 0;
            this.listBoxVocabulary.SelectedIndexChanged += new System.EventHandler(this.listBoxVocabulary_SelectedIndexChanged);
            // 
            // vocabularyDetailPanel
            // 
            this.vocabularyDetailPanel.Location = new System.Drawing.Point(260, 20);
            this.vocabularyDetailPanel.Name = "vocabularyDetailPanel";
            this.vocabularyDetailPanel.Size = new System.Drawing.Size(400, 200);
            this.vocabularyDetailPanel.TabIndex = 1;
            // 
            // btnDelete
            // 
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnDelete.Location = new System.Drawing.Point(260, 240);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(180, 40);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Xóa từ";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.Location = new System.Drawing.Point(450, 240);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(180, 40);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblTotalVocabulary
            // 
            this.lblTotalVocabulary.AutoSize = true;
            this.lblTotalVocabulary.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTotalVocabulary.ForeColor = System.Drawing.Color.Gray;
            this.lblTotalVocabulary.Location = new System.Drawing.Point(260, 300);
            this.lblTotalVocabulary.Name = "lblTotalVocabulary";
            this.lblTotalVocabulary.Size = new System.Drawing.Size(200, 23);
            this.lblTotalVocabulary.TabIndex = 4;
            this.lblTotalVocabulary.Text = "Tổng số từ: [số từ]";
            // 
            // VocabularyListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.ClientSize = new System.Drawing.Size(700, 420);
            this.Controls.Add(this.lblTotalVocabulary);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.vocabularyDetailPanel);
            this.Controls.Add(this.listBoxVocabulary);
            this.Name = "VocabularyListForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Danh sách từ vựng";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
