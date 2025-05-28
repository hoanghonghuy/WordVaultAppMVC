using System;
using System.Windows.Forms;
using WordVaultAppMVC.Services;

namespace WordVaultAppMVC.Views
{
    public partial class ShuffleStudyForm : Form
    {
        private string currentWord;
        private string currentWordId; // Để lưu trữ từ vựng hiện tại
        private int remainingWordsCount;
        private readonly VocabularyService vocabularyService; // Dịch vụ API

        public ShuffleStudyForm()
        {
            InitializeComponent();
            vocabularyService = new VocabularyService(); // Khởi tạo API service
            remainingWordsCount = 10; // Số từ ban đầu để học
            LoadNextWord();
        }

        // Tải từ vựng tiếp theo từ cơ sở dữ liệu hoặc API
        private void LoadNextWord()
        {
            if (remainingWordsCount > 0)
            {
                // Giả sử bạn đang lấy từ vựng ngẫu nhiên từ API hoặc cơ sở dữ liệu
                currentWord = vocabularyService.GetRandomWord(out currentWordId);
                lblWord.Text = "Từ hiện tại: " + currentWord;
                lblRemainingWords.Text = "Còn lại: " + remainingWordsCount + " từ";
            }
            else
            {
                lblWord.Text = "Không còn từ nào!";
                lblRemainingWords.Text = "";
            }
        }

        // Sự kiện khi nhấn nút "Tiếp theo"
        private void btnNextWord_Click(object sender, EventArgs e)
        {
            remainingWordsCount--;
            LoadNextWord();
        }

        // Sự kiện khi nhấn nút "Hiển thị nghĩa"
        private void btnShowMeaning_Click(object sender, EventArgs e)
        {
            string meaning = vocabularyService.GetWordMeaning(currentWordId);
            MessageBox.Show($"Nghĩa của từ '{currentWord}': {meaning}", "Nghĩa từ vựng", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Sự kiện khi nhấn nút "Kiểm tra nghĩa"
        private void btnCheckMeaning_Click(object sender, EventArgs e)
        {
            string userMeaning = Microsoft.VisualBasic.Interaction.InputBox("Nhập nghĩa của từ vựng này:", "Kiểm tra Nghĩa", "");

            if (string.IsNullOrEmpty(userMeaning))
            {
                MessageBox.Show("Vui lòng nhập nghĩa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string correctMeaning = vocabularyService.GetWordMeaning(currentWordId);
            if (userMeaning.Trim().Equals(correctMeaning, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Chính xác! Nghĩa đúng.", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Sai rồi! Nghĩa đúng là: {correctMeaning}", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
