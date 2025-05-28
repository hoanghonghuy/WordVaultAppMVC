namespace WordVaultAppMVC.Views
{
    partial class VocabularyDetailPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Panel chứa toàn bộ nội dung
        private System.Windows.Forms.Panel pnlContainer;
        private System.Windows.Forms.Label lblWord;
        private System.Windows.Forms.Label lblMeaning;
        private System.Windows.Forms.Label lblPronunciation;
        private System.Windows.Forms.Label lblAudioUrl;

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

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.lblWord = new System.Windows.Forms.Label();
            this.lblMeaning = new System.Windows.Forms.Label();
            this.lblPronunciation = new System.Windows.Forms.Label();
            this.lblAudioUrl = new System.Windows.Forms.Label();
            this.pnlContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlContainer
            // 
            this.pnlContainer.BackColor = System.Drawing.Color.White;
            this.pnlContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlContainer.Controls.Add(this.lblWord);
            this.pnlContainer.Controls.Add(this.lblMeaning);
            this.pnlContainer.Controls.Add(this.lblPronunciation);
            this.pnlContainer.Controls.Add(this.lblAudioUrl);
            this.pnlContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContainer.Location = new System.Drawing.Point(0, 0);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Padding = new System.Windows.Forms.Padding(10);
            this.pnlContainer.Size = new System.Drawing.Size(400, 200);
            this.pnlContainer.TabIndex = 0;
            // 
            // lblWord
            // 
            this.lblWord.AutoSize = true;
            this.lblWord.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblWord.Location = new System.Drawing.Point(20, 20);
            this.lblWord.Name = "lblWord";
            this.lblWord.Size = new System.Drawing.Size(60, 28);
            this.lblWord.TabIndex = 0;
            this.lblWord.Text = "Từ: ";
            // 
            // lblMeaning
            // 
            this.lblMeaning.AutoSize = true;
            this.lblMeaning.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblMeaning.Location = new System.Drawing.Point(20, 60);
            this.lblMeaning.Name = "lblMeaning";
            this.lblMeaning.Size = new System.Drawing.Size(90, 28);
            this.lblMeaning.TabIndex = 1;
            this.lblMeaning.Text = "Nghĩa: ";
            // 
            // lblPronunciation
            // 
            this.lblPronunciation.AutoSize = true;
            this.lblPronunciation.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblPronunciation.Location = new System.Drawing.Point(20, 100);
            this.lblPronunciation.Name = "lblPronunciation";
            this.lblPronunciation.Size = new System.Drawing.Size(135, 28);
            this.lblPronunciation.TabIndex = 2;
            this.lblPronunciation.Text = "Phát âm: ";
            // 
            // lblAudioUrl
            // 
            this.lblAudioUrl.AutoSize = true;
            this.lblAudioUrl.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblAudioUrl.Location = new System.Drawing.Point(20, 140);
            this.lblAudioUrl.Name = "lblAudioUrl";
            this.lblAudioUrl.Size = new System.Drawing.Size(115, 28);
            this.lblAudioUrl.TabIndex = 3;
            this.lblAudioUrl.Text = "Audio URL: ";
            // 
            // VocabularyDetailPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlContainer);
            this.Name = "VocabularyDetailPanel";
            this.Size = new System.Drawing.Size(400, 200);
            this.pnlContainer.ResumeLayout(false);
            this.pnlContainer.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion
    }
}
