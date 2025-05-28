using System;
using System.Windows.Forms;
using WordVaultAppMVC.Services;

namespace WordVaultAppMVC.Views.Controls
{
    public class ShuffleStudyControl : UserControl
    {
        private Label lblWord, lblRemainingWords;
        private Button btnNextWord, btnShowMeaning, btnCheckMeaning;
        private string currentWord;
        private string currentWordId;
        private int remainingWordsCount;
        private readonly VocabularyService vocabularyService;

        public ShuffleStudyControl()
        {
            this.Dock = DockStyle.Fill;
            vocabularyService = new VocabularyService();
            remainingWordsCount = 10; // Số từ mặc định
            InitializeComponent();
            LoadNextWord();
        }

        private void InitializeComponent()
        {
            var pnlHeader = new Panel
            {
                BackColor = System.Drawing.Color.FromArgb(52, 152, 219),
                Dock = DockStyle.Top,
                Height = 60
            };

            var lblAppTitle = new Label
            {
                Text = "📚 Học Ngẫu Nhiên Từ Vựng",
                Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.White,
                AutoSize = true,
                Location = new System.Drawing.Point(20, 15)
            };
            pnlHeader.Controls.Add(lblAppTitle);

            var pnlMain = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            lblWord = new Label
            {
                Font = new System.Drawing.Font("Segoe UI", 14F),
                Text = "Từ hiện tại:",
                AutoSize = true,
                Location = new System.Drawing.Point(0, 10)
            };

            btnCheckMeaning = new Button
            {
                Text = "🧠 Kiểm tra nghĩa",
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Location = new System.Drawing.Point(60, 80),
                Size = new System.Drawing.Size(120, 40)
            };
            btnCheckMeaning.Click += BtnCheckMeaning_Click;

            btnShowMeaning = new Button
            {
                Text = "📖 Hiển thị nghĩa",
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Location = new System.Drawing.Point(200, 80),
                Size = new System.Drawing.Size(120, 40)
            };
            btnShowMeaning.Click += BtnShowMeaning_Click;

            btnNextWord = new Button
            {
                Text = "➡️ Tiếp theo",
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Location = new System.Drawing.Point(340, 80),
                Size = new System.Drawing.Size(120, 40)
            };
            btnNextWord.Click += BtnNextWord_Click;

            lblRemainingWords = new Label
            {
                Text = "Còn lại: 10 từ",
                Font = new System.Drawing.Font("Segoe UI", 10F),
                ForeColor = System.Drawing.Color.Gray,
                AutoSize = true,
                Location = new System.Drawing.Point(0, 150)
            };

            pnlMain.Controls.AddRange(new Control[]
            {
                lblWord,
                btnCheckMeaning,
                btnShowMeaning,
                btnNextWord,
                lblRemainingWords
            });

            this.Controls.Add(pnlMain);
            this.Controls.Add(pnlHeader);
        }

        private void LoadNextWord()
        {
            if (remainingWordsCount <= 0)
            {
                lblWord.Text = "🎉 Bạn đã hoàn thành buổi học!";
                lblRemainingWords.Text = "";
                DisableButtons();
                return;
            }

            currentWord = vocabularyService.GetRandomWord(out currentWordId);

            if (string.IsNullOrEmpty(currentWord))
            {
                MessageBox.Show("Không tìm thấy từ nào để học. Vui lòng kiểm tra dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisableButtons();
                return;
            }

            lblWord.Text = $"Từ hiện tại: {currentWord}";
            lblRemainingWords.Text = $"Còn lại: {remainingWordsCount} từ";
        }

        private void BtnNextWord_Click(object sender, EventArgs e)
        {
            remainingWordsCount--;
            LoadNextWord();
        }

        private void BtnShowMeaning_Click(object sender, EventArgs e)
        {
            string meaning = vocabularyService.GetWordMeaning(currentWordId);
            if (!string.IsNullOrEmpty(meaning))
            {
                MessageBox.Show($"📘 Nghĩa của từ '{currentWord}':\n\n{meaning}", "Nghĩa từ vựng", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Không có nghĩa nào được tìm thấy!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnCheckMeaning_Click(object sender, EventArgs e)
        {
            string userInput = Microsoft.VisualBasic.Interaction.InputBox(
                $"Bạn hãy nhập nghĩa của từ '{currentWord}':",
                "📝 Kiểm tra Nghĩa",
                ""
            );

            if (string.IsNullOrWhiteSpace(userInput))
            {
                MessageBox.Show("Vui lòng nhập nghĩa để kiểm tra!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string correctMeaning = vocabularyService.GetWordMeaning(currentWordId);

            if (userInput.Trim().Equals(correctMeaning, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("✅ Chính xác!", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"❌ Sai rồi!\nNghĩa đúng là:\n\n{correctMeaning}", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisableButtons()
        {
            btnNextWord.Enabled = false;
            btnShowMeaning.Enabled = false;
            btnCheckMeaning.Enabled = false;
        }
    }
}
