namespace WordVaultAppMVC.Views
{
    partial class ShuffleStudyForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblAppTitle;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Label lblWord;
        private System.Windows.Forms.Button btnNextWord;
        private System.Windows.Forms.Button btnShowMeaning;
        private System.Windows.Forms.Button btnCheckMeaning;
        private System.Windows.Forms.Label lblRemainingWords;
        private System.Windows.Forms.Panel pnlFooter;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblAppTitle = new System.Windows.Forms.Label();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.lblWord = new System.Windows.Forms.Label();
            this.lblRemainingWords = new System.Windows.Forms.Label();
            this.btnNextWord = new System.Windows.Forms.Button();
            this.btnShowMeaning = new System.Windows.Forms.Button();
            this.btnCheckMeaning = new System.Windows.Forms.Button();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.pnlHeader.SuspendLayout();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.pnlHeader.Controls.Add(this.lblAppTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(540, 60);
            this.pnlHeader.TabIndex = 1;
            // 
            // lblAppTitle
            // 
            this.lblAppTitle.AutoSize = true;
            this.lblAppTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.ForeColor = System.Drawing.Color.White;
            this.lblAppTitle.Location = new System.Drawing.Point(20, 15);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(407, 45);
            this.lblAppTitle.TabIndex = 0;
            this.lblAppTitle.Text = "Học Ngẫu Nhiên Từ Vựng";
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.lblWord);
            this.pnlMain.Controls.Add(this.lblRemainingWords);
            this.pnlMain.Controls.Add(this.btnNextWord);
            this.pnlMain.Controls.Add(this.btnShowMeaning);
            this.pnlMain.Controls.Add(this.btnCheckMeaning);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 60);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(20);
            this.pnlMain.Size = new System.Drawing.Size(540, 245);
            this.pnlMain.TabIndex = 0;
            // 
            // lblWord
            // 
            this.lblWord.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblWord.ForeColor = System.Drawing.Color.Black;
            this.lblWord.Location = new System.Drawing.Point(0, 10);
            this.lblWord.Name = "lblWord";
            this.lblWord.Size = new System.Drawing.Size(480, 40);
            this.lblWord.TabIndex = 0;
            this.lblWord.Text = "Từ hiện tại: ";
            this.lblWord.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblRemainingWords
            // 
            this.lblRemainingWords.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblRemainingWords.ForeColor = System.Drawing.Color.Gray;
            this.lblRemainingWords.Location = new System.Drawing.Point(0, 200);
            this.lblRemainingWords.Name = "lblRemainingWords";
            this.lblRemainingWords.Size = new System.Drawing.Size(528, 42);
            this.lblRemainingWords.TabIndex = 4;
            this.lblRemainingWords.Text = "Còn lại: 9 từ";
            this.lblRemainingWords.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnNextWord
            // 
            this.btnNextWord.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnNextWord.Location = new System.Drawing.Point(340, 119);
            this.btnNextWord.Name = "btnNextWord";
            this.btnNextWord.Size = new System.Drawing.Size(120, 40);
            this.btnNextWord.TabIndex = 1;
            this.btnNextWord.Text = "Tiếp theo";
            this.btnNextWord.Click += new System.EventHandler(this.btnNextWord_Click);
            // 
            // btnShowMeaning
            // 
            this.btnShowMeaning.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnShowMeaning.Location = new System.Drawing.Point(200, 119);
            this.btnShowMeaning.Name = "btnShowMeaning";
            this.btnShowMeaning.Size = new System.Drawing.Size(120, 40);
            this.btnShowMeaning.TabIndex = 2;
            this.btnShowMeaning.Text = "Hiển thị nghĩa";
            this.btnShowMeaning.Click += new System.EventHandler(this.btnShowMeaning_Click);
            // 
            // btnCheckMeaning
            // 
            this.btnCheckMeaning.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCheckMeaning.Location = new System.Drawing.Point(60, 119);
            this.btnCheckMeaning.Name = "btnCheckMeaning";
            this.btnCheckMeaning.Size = new System.Drawing.Size(120, 40);
            this.btnCheckMeaning.TabIndex = 3;
            this.btnCheckMeaning.Text = "Kiểm tra nghĩa";
            this.btnCheckMeaning.Click += new System.EventHandler(this.btnCheckMeaning_Click);
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 305);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(540, 30);
            this.pnlFooter.TabIndex = 2;
            // 
            // ShuffleStudyForm
            // 
            this.ClientSize = new System.Drawing.Size(540, 335);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlFooter);
            this.Name = "ShuffleStudyForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Học Ngẫu Nhiên Từ Vựng";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
