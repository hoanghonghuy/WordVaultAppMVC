using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WordVaultAppMVC.Models; // Giả sử bạn có model Vocabulary hoặc dịch vụ tương ứng
using WordVaultAppMVC.Services;

namespace WordVaultAppMVC.Views
{
    public partial class DailyReviewForm : Form
    {
        private List<Vocabulary> dailyWords;
        private int currentIndex;
        private readonly VocabularyService vocabularyService;

        public DailyReviewForm()
        {
            InitializeComponent();
            vocabularyService = new VocabularyService();
            LoadDailyWords();
            currentIndex = 0;
            DisplayCurrentWord();
        }

        // Giả sử phương thức này lấy danh sách từ vựng cần ôn tập hàng ngày (có thể từ database hoặc dịch vụ)
        private void LoadDailyWords()
        {
            // Ví dụ: Sử dụng VocabularyService để lấy danh sách từ vựng. 
            // Ở đây, ta dùng danh sách tạm thời để demo.
            dailyWords = new List<Vocabulary>
            {
                new Vocabulary { Id = 1, Word = "Apple", Meaning = "A fruit", Pronunciation = "/ˈæp.əl/", AudioUrl = "http://example.com/apple.mp3" },
                new Vocabulary { Id = 2, Word = "Banana", Meaning = "A yellow fruit", Pronunciation = "/bəˈnæn.ə/", AudioUrl = "http://example.com/banana.mp3" },
                new Vocabulary { Id = 3, Word = "Orange", Meaning = "A citrus fruit", Pronunciation = "/ˈɒr.ɪndʒ/", AudioUrl = "http://example.com/orange.mp3" }
                // Thêm các từ khác nếu cần
            };
        }

        // Hiển thị từ hiện tại lên giao diện
        private void DisplayCurrentWord()
        {
            if (dailyWords != null && dailyWords.Count > 0 && currentIndex < dailyWords.Count)
            {
                Vocabulary currentWord = dailyWords[currentIndex];
                lblCurrentWord.Text = "Từ hiện tại: " + currentWord.Word;
                lblMeaningDisplay.Text = "Nghĩa: " + currentWord.Meaning;
                lblRemainingWords.Text = "Còn lại: " + (dailyWords.Count - currentIndex - 1) + " từ";
            }
            else
            {
                lblCurrentWord.Text = "Không còn từ nào!";
                lblMeaningDisplay.Text = "";
                lblRemainingWords.Text = "";
            }
        }

        // Sự kiện khi nhấn nút "Từ kế tiếp"
        private void btnNextWord_Click(object sender, EventArgs e)
        {
            currentIndex++;
            DisplayCurrentWord();
        }

        // Sự kiện khi nhấn nút "Hiển thị nghĩa" (hiển thị qua MessageBox hoặc cập nhật lblMeaningDisplay)
        private void btnShowMeaning_Click(object sender, EventArgs e)
        {
            if (dailyWords != null && currentIndex < dailyWords.Count)
            {
                Vocabulary currentWord = dailyWords[currentIndex];
                MessageBox.Show($"Nghĩa của từ '{currentWord.Word}': {currentWord.Meaning}", "Nghĩa từ vựng", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
