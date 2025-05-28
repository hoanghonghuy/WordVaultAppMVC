using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WordVaultAppMVC.Models;
using WordVaultAppMVC.Data;  // Đảm bảo đã có using cho repository

namespace WordVaultAppMVC.Views
{
    public partial class QuizForm : Form
    {
        private List<Vocabulary> vocabularyList;  // Danh sách từ vựng
        private int currentQuestionIndex = 0;     // Chỉ số câu hỏi hiện tại
        private int score = 0;                    // Điểm số của người dùng
        private VocabularyRepository vocabRepo;   // Instance của repository

        public QuizForm()
        {
            InitializeComponent();
            vocabRepo = new VocabularyRepository();
            LoadVocabulary();
        }

        // Lấy danh sách từ vựng từ cơ sở dữ liệu
        private void LoadVocabulary()
        {
            vocabularyList = vocabRepo.GetAllVocabulary();
            if (vocabularyList.Count > 0)
            {
                DisplayQuestion();
            }
        }

        // Hiển thị câu hỏi tiếp theo
        private void DisplayQuestion()
        {
            if (currentQuestionIndex >= vocabularyList.Count)
            {
                MessageBox.Show($"Bài kiểm tra hoàn thành! Bạn đã đạt {score} điểm.");
                this.Close();
                return;
            }

            var question = vocabularyList[currentQuestionIndex];
            lblWord.Text = question.Word;  // Hiển thị từ vựng

            // Tạo các đáp án ngẫu nhiên (1 đúng, 3 sai)
            var correctAnswer = question.Meaning;
            var wrongAnswers = GetWrongAnswers(correctAnswer);

            var answers = new List<string> { correctAnswer };
            answers.AddRange(wrongAnswers);
            ShuffleList(answers); // Xáo trộn danh sách đáp án

            btnAnswer1.Text = answers[0];
            btnAnswer2.Text = answers[1];
            btnAnswer3.Text = answers[2];
            btnAnswer4.Text = answers[3];

            currentQuestionIndex++;
        }

        // Phương thức xáo trộn danh sách
        private void ShuffleList<T>(IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        // Lấy các đáp án sai từ danh sách từ vựng
        private List<string> GetWrongAnswers(string correctAnswer)
        {
            var wrongAnswers = new List<string>();
            foreach (var vocab in vocabularyList)
            {
                if (vocab.Meaning != correctAnswer)
                {
                    wrongAnswers.Add(vocab.Meaning);
                }
                if (wrongAnswers.Count >= 3) break;
            }
            return wrongAnswers;
        }

        // Kiểm tra đáp án
        private void CheckAnswer(string selectedAnswer)
        {
            var correctAnswer = vocabularyList[currentQuestionIndex - 1].Meaning;
            if (selectedAnswer == correctAnswer)
            {
                score++;
                MessageBox.Show("Đúng rồi!");
            }
            else
            {
                MessageBox.Show("Sai rồi!");
            }

            DisplayQuestion();
        }

        // Khi người dùng chọn câu trả lời
        private void btnAnswer1_Click(object sender, EventArgs e)
        {
            CheckAnswer(btnAnswer1.Text);
        }

        private void btnAnswer2_Click(object sender, EventArgs e)
        {
            CheckAnswer(btnAnswer2.Text);
        }

        private void btnAnswer3_Click(object sender, EventArgs e)
        {
            CheckAnswer(btnAnswer3.Text);
        }

        private void btnAnswer4_Click(object sender, EventArgs e)
        {
            CheckAnswer(btnAnswer4.Text);
        }
    }
}
