using System;
using System.Windows.Forms;
using WordVaultAppMVC.Views.Controls;

namespace WordVaultAppMVC.Views
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            LoadControl(new HomeControl()); // Mặc định là Home
        }

        private void LoadControl(UserControl control)
        {
            pnlMainContent.Controls.Clear();
            control.Dock = DockStyle.Fill;
            pnlMainContent.Controls.Add(control);
        }

        private void btnHome_Click(object sender, EventArgs e) => LoadControl(new HomeControl());
        private void btnTopicVocabulary_Click(object sender, EventArgs e) => LoadControl(new TopicVocabularyControl("Từ vựng TOEIC"));
        private void btnFavorite_Click(object sender, EventArgs e) => LoadControl(new FavoriteWordsControl());
        private void btnDailyReview_Click(object sender, EventArgs e) => LoadControl(new DailyReviewControl());
        private void btnQuiz_Click(object sender, EventArgs e) => LoadControl(new QuizControl());
        private void btnShuffle_Click(object sender, EventArgs e) => LoadControl(new ShuffleStudyControl());
        private void btnSettings_Click(object sender, EventArgs e) => LoadControl(new SettingsControl());

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
                e.Cancel = true;
        }
    }
}