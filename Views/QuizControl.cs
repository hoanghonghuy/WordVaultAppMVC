// 🔹 QuizControl.cs (nâng cấp: luu dap an rieng, chon cau, nop bai tinh diem + luyện lại từ sai)
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using WordVaultAppMVC.Data;
using WordVaultAppMVC.Models;
using WordVaultAppMVC.Controllers;

namespace WordVaultAppMVC.Views.Controls
{
    public class QuizControl : UserControl
    {
        private ComboBox cboTopic;
        private NumericUpDown numQuestionCount;
        private Button btnStart;
        private Label lblQuestion, lblProgress, lblFeedback;
        private RadioButton[] rdoOptions;
        private Button btnBack, btnNext, btnSubmitQuiz, btnRetryWrong;
        private FlowLayoutPanel pnlJumpButtons, pnlMatrix;

        private List<Vocabulary> questions;
        private Dictionary<int, string> userAnswers = new Dictionary<int, string>();
        private List<Vocabulary> wrongWords = new List<Vocabulary>();
        private int currentIndex = 0;

        public QuizControl()
        {
            InitializeComponent();
            LoadTopics();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;

            cboTopic = new ComboBox { Location = new System.Drawing.Point(20, 20), Width = 200 };
            numQuestionCount = new NumericUpDown { Location = new System.Drawing.Point(240, 20), Minimum = 1, Maximum = 100, Value = 5 };
            btnStart = new Button { Text = "Bắt đầu Quiz", Location = new System.Drawing.Point(360, 18) };
            btnStart.Click += BtnStart_Click;

            lblQuestion = new Label { Font = new System.Drawing.Font("Segoe UI", 14F), Location = new System.Drawing.Point(20, 70), AutoSize = true };
            lblProgress = new Label { Font = new System.Drawing.Font("Segoe UI", 10F), Location = new System.Drawing.Point(20, 110), AutoSize = true };

            rdoOptions = new RadioButton[4];
            for (int i = 0; i < 4; i++)
            {
                rdoOptions[i] = new RadioButton { Location = new System.Drawing.Point(40, 150 + i * 30), AutoSize = true };
                rdoOptions[i].CheckedChanged += Option_CheckedChanged;
                this.Controls.Add(rdoOptions[i]);
            }

            btnBack = new Button { Text = "⬅️ Quay lại", Location = new System.Drawing.Point(20, 280) };
            btnBack.Click += (sender, args) => { SaveAnswer(); currentIndex = (currentIndex - 1 + questions.Count) % questions.Count; LoadQuestion(); };

            btnNext = new Button { Text = "➡️ Tiếp", Location = new System.Drawing.Point(140, 280) };
            btnNext.Click += (sender, args) => { SaveAnswer(); currentIndex = (currentIndex + 1) % questions.Count; LoadQuestion(); };

            btnSubmitQuiz = new Button { Text = "📜 Nộp bài", Location = new System.Drawing.Point(260, 280) };
            btnSubmitQuiz.Click += SubmitQuiz;

            btnRetryWrong = new Button { Text = "🔁 Luyện lại từ sai", Location = new System.Drawing.Point(380, 280), Visible = false };
            btnRetryWrong.Click += RetryWrongAnswers;

            lblFeedback = new Label { Location = new System.Drawing.Point(20, 320), AutoSize = true };

            pnlJumpButtons = new FlowLayoutPanel
            {
                Location = new System.Drawing.Point(20, 360),
                Width = 500,
                Height = 60,
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight
            };

            pnlMatrix = new FlowLayoutPanel
            {
                Location = new System.Drawing.Point(20, 430),
                Width = 500,
                Height = 60,
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight
            };

            this.Controls.AddRange(new Control[]
            {
                cboTopic, numQuestionCount, btnStart, lblQuestion, lblProgress,
                btnBack, btnNext, btnSubmitQuiz, btnRetryWrong, lblFeedback,
                pnlJumpButtons, pnlMatrix
            });
        }

        private void LoadTopics()
        {
            cboTopic.Items.Clear();
            var topics = new TopicRepository().GetAllTopics();
            foreach (var topic in topics)
            {
                cboTopic.Items.Add(topic.Name);
            }
            if (cboTopic.Items.Count > 0) cboTopic.SelectedIndex = 0;
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            string selectedTopic = cboTopic.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedTopic)) return;

            int count = (int)numQuestionCount.Value;
            questions = new VocabularyRepository()
                .GetVocabularyByTopic(selectedTopic)
                .OrderBy(_ => Guid.NewGuid()).Take(count).ToList();

            currentIndex = 0;
            userAnswers.Clear();
            pnlJumpButtons.Controls.Clear();
            pnlMatrix.Controls.Clear();
            btnRetryWrong.Visible = false;
            wrongWords.Clear();

            for (int i = 0; i < questions.Count; i++)
            {
                var btn = new Button { Text = (i + 1).ToString(), Width = 40, Height = 30, Tag = i };
                btn.Click += (s, ev) => { SaveAnswer(); currentIndex = (int)((Button)s).Tag; LoadQuestion(); };
                pnlJumpButtons.Controls.Add(btn);
            }

            LoadQuestion();
        }

        private void LoadQuestion()
        {
            if (questions == null || questions.Count == 0 || currentIndex >= questions.Count) return;

            var q = questions[currentIndex];
            lblQuestion.Text = "Từ: " + q.Word;
            lblProgress.Text = $"Câu {currentIndex + 1}/{questions.Count}";
            lblFeedback.Text = "";

            var repo = new VocabularyRepository();
            var wrongAnswers = repo.GetAllVocabulary()
                .Where(v => v.Word != q.Word && !string.IsNullOrEmpty(v.Meaning))
                .Select(v => v.Meaning)
                .Distinct()
                .OrderBy(_ => Guid.NewGuid())
                .Take(3)
                .ToList();

            wrongAnswers.Add(q.Meaning);
            var allAnswers = wrongAnswers.OrderBy(_ => Guid.NewGuid()).Take(4).ToList();

            for (int i = 0; i < rdoOptions.Length; i++)
            {
                rdoOptions[i].CheckedChanged -= Option_CheckedChanged;
                rdoOptions[i].Text = i < allAnswers.Count ? allAnswers[i] : "";
                rdoOptions[i].Checked = userAnswers.TryGetValue(currentIndex, out var saved) && saved == rdoOptions[i].Text;
                rdoOptions[i].CheckedChanged += Option_CheckedChanged;
            }
        }

        private void Option_CheckedChanged(object sender, EventArgs e)
        {
            SaveAnswer();
        }

        private void SaveAnswer()
        {
            var selected = rdoOptions.FirstOrDefault(r => r.Checked);
            if (selected != null)
            {
                userAnswers[currentIndex] = selected.Text;
            }
        }

        private void SubmitQuiz(object sender, EventArgs e)
        {
            int correct = 0;
            wrongWords.Clear();

            for (int i = 0; i < questions.Count; i++)
            {
                var word = questions[i];
                var selectedAnswer = userAnswers.ContainsKey(i) ? userAnswers[i] : null;
                bool isCorrect = selectedAnswer == word.Meaning;
                if (isCorrect)
                {
                    correct++;
                    new LearningController().UpdateLearningStatus(word.Id.ToString(), "Đã học");
                }
                else
                {
                    wrongWords.Add(word);
                    new LearningController().UpdateLearningStatus(word.Id.ToString(), "Đang học");
                }
            }

            MessageBox.Show($"✅ Hoàn thành Quiz.\nĐiểm: {correct}/{questions.Count} đúng.", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnRetryWrong.Visible = wrongWords.Count > 0;
        }

        private void RetryWrongAnswers(object sender, EventArgs e)
        {
            questions = wrongWords;
            currentIndex = 0;
            userAnswers.Clear();
            pnlJumpButtons.Controls.Clear();
            pnlMatrix.Controls.Clear();

            for (int i = 0; i < questions.Count; i++)
            {
                var btn = new Button { Text = (i + 1).ToString(), Width = 40, Height = 30, Tag = i };
                btn.Click += (s, ev) => { SaveAnswer(); currentIndex = (int)((Button)s).Tag; LoadQuestion(); };
                pnlJumpButtons.Controls.Add(btn);
            }

            LoadQuestion();
        }
    }
}
