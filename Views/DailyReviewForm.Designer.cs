namespace WordVaultAppMVC.Views
{
    partial class DailyReviewForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Panels
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Panel pnlFooter;

        // Controls in Header
        private System.Windows.Forms.Label lblFormTitle;

        // Controls in Content
        private System.Windows.Forms.Label lblCurrentWord;
        private System.Windows.Forms.Button btnShowMeaning;
        private System.Windows.Forms.Button btnNextWord;
        private System.Windows.Forms.Label lblMeaningDisplay;
        private System.Windows.Forms.Label lblRemainingWords;

        // Controls in Footer (có thể thêm sau)
        // ...

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

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblFormTitle = new System.Windows.Forms.Label();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.lblCurrentWord = new System.Windows.Forms.Label();
            this.btnShowMeaning = new System.Windows.Forms.Button();
            this.btnNextWord = new System.Windows.Forms.Button();
            this.lblMeaningDisplay = new System.Windows.Forms.Label();
            this.lblRemainingWords = new System.Windows.Forms.Label();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.pnlHeader.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.pnlHeader.Controls.Add(this.lblFormTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(600, 60);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblFormTitle
            // 
            this.lblFormTitle.AutoSize = true;
            this.lblFormTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblFormTitle.ForeColor = System.Drawing.Color.White;
            this.lblFormTitle.Location = new System.Drawing.Point(20, 15);
            this.lblFormTitle.Name = "lblFormTitle";
            this.lblFormTitle.Size = new System.Drawing.Size(244, 45);
            this.lblFormTitle.TabIndex = 0;
            this.lblFormTitle.Text = "Ôn Tập Hàng Ngày";
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.lblRemainingWords);
            this.pnlContent.Controls.Add(this.lblMeaningDisplay);
            this.pnlContent.Controls.Add(this.btnNextWord);
            this.pnlContent.Controls.Add(this.btnShowMeaning);
            this.pnlContent.Controls.Add(this.lblCurrentWord);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 60);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(20);
            this.pnlContent.Size = new System.Drawing.Size(600, 300);
            this.pnlContent.TabIndex = 1;
            // 
            // lblCurrentWord
            // 
            this.lblCurrentWord.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblCurrentWord.Location = new System.Drawing.Point(20, 20);
            this.lblCurrentWord.Name = "lblCurrentWord";
            this.lblCurrentWord.Size = new System.Drawing.Size(560, 50);
            this.lblCurrentWord.TabIndex = 0;
            this.lblCurrentWord.Text = "Từ hiện tại: ";
            this.lblCurrentWord.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnShowMeaning
            // 
            this.btnShowMeaning.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnShowMeaning.Location = new System.Drawing.Point(100, 90);
            this.btnShowMeaning.Name = "btnShowMeaning";
            this.btnShowMeaning.Size = new System.Drawing.Size(180, 45);
            this.btnShowMeaning.TabIndex = 1;
            this.btnShowMeaning.Text = "Hiển thị nghĩa";
            this.btnShowMeaning.UseVisualStyleBackColor = true;
            this.btnShowMeaning.Click += new System.EventHandler(this.btnShowMeaning_Click);
            // 
            // btnNextWord
            // 
            this.btnNextWord.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnNextWord.Location = new System.Drawing.Point(320, 90);
            this.btnNextWord.Name = "btnNextWord";
            this.btnNextWord.Size = new System.Drawing.Size(180, 45);
            this.btnNextWord.TabIndex = 2;
            this.btnNextWord.Text = "Từ kế tiếp";
            this.btnNextWord.UseVisualStyleBackColor = true;
            this.btnNextWord.Click += new System.EventHandler(this.btnNextWord_Click);
            // 
            // lblMeaningDisplay
            // 
            this.lblMeaningDisplay.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblMeaningDisplay.Location = new System.Drawing.Point(20, 150);
            this.lblMeaningDisplay.Name = "lblMeaningDisplay";
            this.lblMeaningDisplay.Size = new System.Drawing.Size(560, 50);
            this.lblMeaningDisplay.TabIndex = 3;
            this.lblMeaningDisplay.Text = "Nghĩa: ";
            this.lblMeaningDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblRemainingWords
            // 
            this.lblRemainingWords.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblRemainingWords.ForeColor = System.Drawing.Color.Gray;
            this.lblRemainingWords.Location = new System.Drawing.Point(20, 220);
            this.lblRemainingWords.Name = "lblRemainingWords";
            this.lblRemainingWords.Size = new System.Drawing.Size(560, 30);
            this.lblRemainingWords.TabIndex = 4;
            this.lblRemainingWords.Text = "Còn lại: 0 từ";
            this.lblRemainingWords.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(236, 240, 241);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 360);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(600, 40);
            this.pnlFooter.TabIndex = 2;
            // 
            // DailyReviewForm
            // 
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlFooter);
            this.Name = "DailyReviewForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ôn Tập Hàng Ngày";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlContent.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion
    }
}
