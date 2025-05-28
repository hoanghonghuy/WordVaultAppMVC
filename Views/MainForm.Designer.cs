namespace WordVaultAppMVC.Views
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel pnlMainContent;
        // Panels
        private System.Windows.Forms.Panel pnlHeader;
        //private System.Windows.Forms.Panel pnlSearchArea;
        //private System.Windows.Forms.Panel pnlResultArea;
        private System.Windows.Forms.Panel pnlFooter;

        // Controls
        //private System.Windows.Forms.TextBox txtSearch;
        //private System.Windows.Forms.Button btnSearch;
        //private System.Windows.Forms.Label lblPronunciation;
        //private System.Windows.Forms.Label lblMeaning;
        //private System.Windows.Forms.Button btnPlayAudio;
        private System.Windows.Forms.Label lblAppTitle;

        // ToolStrip
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnHome;
        private System.Windows.Forms.ToolStripButton btnTopicVocabulary;
        private System.Windows.Forms.ToolStripButton btnSettings;
        private System.Windows.Forms.ToolStripButton btnFavorite;
        private System.Windows.Forms.ToolStripButton btnDailyReview;
        private System.Windows.Forms.ToolStripButton btnQuiz;
        private System.Windows.Forms.ToolStripButton btnShuffle;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlMainContent = new System.Windows.Forms.Panel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblAppTitle = new System.Windows.Forms.Label();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnHome = new System.Windows.Forms.ToolStripButton();
            this.btnTopicVocabulary = new System.Windows.Forms.ToolStripButton();
            this.btnSettings = new System.Windows.Forms.ToolStripButton();
            this.btnFavorite = new System.Windows.Forms.ToolStripButton();
            this.btnDailyReview = new System.Windows.Forms.ToolStripButton();
            this.btnQuiz = new System.Windows.Forms.ToolStripButton();
            this.btnShuffle = new System.Windows.Forms.ToolStripButton();
            this.pnlFooter = new System.Windows.Forms.Panel();

            this.pnlHeader.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();

            // pnlMainContent
            this.pnlMainContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMainContent.Location = new System.Drawing.Point(0, 90);
            this.pnlMainContent.Name = "pnlMainContent";
            this.pnlMainContent.Padding = new System.Windows.Forms.Padding(10);
            this.pnlMainContent.Size = new System.Drawing.Size(800, 500);
            this.pnlMainContent.TabIndex = 5;

            // pnlHeader
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.pnlHeader.Controls.Add(this.lblAppTitle);
            this.pnlHeader.Controls.Add(this.toolStrip);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Size = new System.Drawing.Size(800, 90);

            // lblAppTitle
            this.lblAppTitle.AutoSize = true;
            this.lblAppTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.ForeColor = System.Drawing.Color.White;
            this.lblAppTitle.Location = new System.Drawing.Point(20, 50);
            this.lblAppTitle.Text = "📘 WordVault - Từ điển cá nhân";

            // toolStrip
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Size = new System.Drawing.Size(800, 38);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        this.btnHome,
        this.btnTopicVocabulary,
        this.btnSettings,
        this.btnFavorite,
        this.btnDailyReview,
        this.btnQuiz,
        this.btnShuffle
    });

            // ToolStrip Buttons
            this.btnHome.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnHome.Text = "Home";
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);

            this.btnTopicVocabulary.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnTopicVocabulary.Text = "Topic Vocabulary";
            this.btnTopicVocabulary.Click += new System.EventHandler(this.btnTopicVocabulary_Click);

            this.btnSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSettings.Text = "Settings";
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);

            this.btnFavorite.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnFavorite.Text = "⭐ Yêu thích";
            this.btnFavorite.Click += new System.EventHandler(this.btnFavorite_Click);

            this.btnDailyReview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDailyReview.Text = "📅 Học từ";
            this.btnDailyReview.Click += new System.EventHandler(this.btnDailyReview_Click);

            this.btnQuiz.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnQuiz.Text = "🧠 Quiz";
            this.btnQuiz.Click += new System.EventHandler(this.btnQuiz_Click);

            this.btnShuffle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnShuffle.Text = "🔀 Xáo từ";
            this.btnShuffle.Click += new System.EventHandler(this.btnShuffle_Click);

            // pnlFooter
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(236, 240, 241);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Size = new System.Drawing.Size(800, 30);

            // MainForm
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.pnlMainContent);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlFooter);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WordVault - English Vocabulary";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);

            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}