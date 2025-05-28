// TODO: bắt đầu từ đây trở xuống là chưa refactor
using System;
using System.Collections.Generic; // Cần cho List (nếu dùng)
using System.Drawing;
using System.Windows.Forms;
using WordVaultAppMVC.Services; // Namespace của VocabularyService
using WordVaultAppMVC.Models;   // Namespace của Vocabulary model
using WordVaultAppMVC.Helpers;
using System.Linq;
using System.Diagnostics;  // Namespace của AudioHelper
// using WordVaultAppMVC.Controllers; // Thêm nếu muốn tích hợp LearningController

namespace WordVaultAppMVC.Views.Controls
{
    public class ShuffleStudyControl : UserControl
    {
        // --- UI Controls ---
        private Label lblWord;
        private Label lblMeaningDisplay; // Label mới để hiện nghĩa
        private Label lblPronunciationDisplay; // Label mới để hiện phát âm (optional)
        private Label lblRemainingWords;
        private Button btnFlipCard;     // Thay thế btnShowMeaning
        private Button btnCheckMeaning;
        private Button btnPlayAudio;    // Nút mới
        private Button btnNextWord;
        private TextBox txtMeaningInput; // TextBox thay cho InputBox (optional)
        private Label lblFeedback;       // Label cho kết quả CheckMeaning
        private TableLayoutPanel mainLayout;
        private Panel cardPanel;

        // --- Logic Fields ---
        // private string currentWord; // Không cần nữa nếu dùng currentVocabulary
        // private string currentWordId; // Không cần nữa nếu dùng currentVocabulary
        private Vocabulary currentVocabulary; // Lưu trữ object Vocabulary hiện tại
        private int remainingWordsCount;
        private readonly VocabularyService vocabularyService;
        // private readonly LearningController learningController; // Thêm nếu muốn cập nhật status

        public ShuffleStudyControl()
        {
            this.Dock = DockStyle.Fill;
            vocabularyService = new VocabularyService();
            // learningController = new LearningController(); // Khởi tạo nếu dùng
            remainingWordsCount = 10; // Số từ mặc định
            InitializeComponent();
            LoadNextWord(); // Tải từ đầu tiên
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = SystemColors.Control;

            // --- Main Layout ---
            mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Padding = new Padding(20) };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // Header
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Card Area
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // Buttons Area
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // Footer (Remaining)

            // --- Header ---
            var pnlHeader = new Panel { BackColor = Color.FromArgb(52, 152, 219), Dock = DockStyle.Fill, Height = 60 }; // Dock Fill trong ô TLP
            var lblAppTitle = new Label { Text = "🔀 Học Ngẫu Nhiên (Flashcard)", Font = new Font("Segoe UI", 16F, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(20, 15) };
            pnlHeader.Controls.Add(lblAppTitle);
            mainLayout.Controls.Add(pnlHeader, 0, 0);

            // --- Card Area ---
            cardPanel = new Panel { Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White, Padding = new Padding(20) };
            var cardLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, AutoSize = true };
            cardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F)); // Word
            cardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // Pronunciation (hidden)
            cardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F)); // Meaning (hidden)
            cardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // Play Audio button

            lblWord = new Label { Name = "lblWord", Text = "...", Font = new Font("Segoe UI", 28F, FontStyle.Bold), AutoSize = false, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
            cardLayout.Controls.Add(lblWord, 0, 0);

            lblPronunciationDisplay = new Label { Name = "lblPronunciationDisplay", Text = "", Font = new Font("Segoe UI", 12F, FontStyle.Italic), ForeColor = Color.Gray, AutoSize = true, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Visible = false };
            cardLayout.Controls.Add(lblPronunciationDisplay, 0, 1);

            lblMeaningDisplay = new Label { Name = "lblMeaningDisplay", Text = "", Font = new Font("Segoe UI", 14F), AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Visible = false };
            cardLayout.Controls.Add(lblMeaningDisplay, 0, 2);

            btnPlayAudio = new Button { Name = "btnPlayAudio", Text = "🔊 Nghe", FlatStyle = FlatStyle.Flat, AutoSize = true, Visible = false, Anchor = AnchorStyles.None };
            btnPlayAudio.FlatAppearance.BorderSize = 0; btnPlayAudio.Click += BtnPlayAudio_Click;
            cardLayout.Controls.Add(btnPlayAudio, 0, 3);

            cardPanel.Controls.Add(cardLayout);
            mainLayout.Controls.Add(cardPanel, 0, 1);

            // --- Buttons Area ---
            btnFlipCard = new Button { Name = "btnFlipCard", Text = "🔄 Lật thẻ / Xem nghĩa", Width = 180, AutoSize = true, BackColor = Color.Gold, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Visible = false };
            btnFlipCard.Click += BtnFlipCard_Click;

            // TextBox để nhập nghĩa (thay InputBox) - ẩn ban đầu
            txtMeaningInput = new TextBox { Name = "txtMeaningInput", Width = 200, Font = new Font("Segoe UI", 10F), Visible = false, Margin = new Padding(5, 0, 5, 0) };
            txtMeaningInput.KeyDown += TxtMeaningInput_KeyDown; // Cho phép nhấn Enter

            btnCheckMeaning = new Button { Name = "btnCheckMeaning", Text = "✔️ Kiểm tra", Width = 100, AutoSize = true, BackColor = Color.LightSkyBlue, Visible = false, Font = new Font("Segoe UI", 9F) };
            btnCheckMeaning.Click += BtnCheckMeaning_Click;

            // Label phản hồi cho việc kiểm tra nghĩa
            lblFeedback = new Label { Name = "lblFeedback", Text = "", AutoSize = true, ForeColor = Color.Red, Font = new Font("Segoe UI", 9F, FontStyle.Italic), MinimumSize = new Size(200, 0), Visible = false, Margin = new Padding(5, 5, 5, 0) };

            btnNextWord = new Button { Name = "btnNextWord", Text = "➡️ Tiếp theo", Width = 100, AutoSize = true, BackColor = Color.LightGray, Visible = false, Font = new Font("Segoe UI", 9F) };
            btnNextWord.Click += BtnNextWord_Click;

            var buttonFlowLayout = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, AutoSize = true, Padding = new Padding(0, 15, 0, 5) };
            // Căn giữa các nút
            buttonFlowLayout.Resize += (sender, args) => { buttonFlowLayout.SuspendLayout(); int totalWidth = buttonFlowLayout.Controls.Cast<Control>().Sum(c => c.Visible ? c.Width + c.Margin.Horizontal : 0); buttonFlowLayout.Padding = new Padding(Math.Max(0, (buttonFlowLayout.Width - totalWidth) / 2), buttonFlowLayout.Padding.Top, 0, buttonFlowLayout.Padding.Bottom); buttonFlowLayout.ResumeLayout(); };

            foreach (var ctl in new Control[] { btnFlipCard, txtMeaningInput, btnCheckMeaning, lblFeedback, btnNextWord }) { ctl.Margin = new Padding(5); }
            buttonFlowLayout.Controls.AddRange(new Control[] { btnFlipCard, txtMeaningInput, btnCheckMeaning, lblFeedback, btnNextWord });
            mainLayout.Controls.Add(buttonFlowLayout, 0, 2);

            // --- Footer ---
            lblRemainingWords = new Label { Name = "lblRemainingWords", Text = "Còn lại: ...", Font = new Font("Segoe UI", 10F), ForeColor = Color.Gray, AutoSize = true, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight };
            mainLayout.Controls.Add(lblRemainingWords, 0, 3);

            // --- Add Main Layout ---
            this.Controls.Add(mainLayout);
        }

        // --- LoadNextWord: Tải từ tiếp theo và reset UI ---
        private void LoadNextWord()
        {
            if (remainingWordsCount <= 0)
            {
                lblWord.Text = "🎉 Bạn đã hoàn thành!";
                lblRemainingWords.Text = "";
                DisableInteraction();
                return;
            }

            try
            {
                // **QUAN TRỌNG:** Giả định hàm này đã được sửa để trả về Vocabulary
                currentVocabulary = vocabularyService.GetRandomWord(); // Không còn out wordId

                if (currentVocabulary == null)
                {
                    MessageBox.Show("Không thể lấy từ ngẫu nhiên. Vui lòng kiểm tra dữ liệu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DisableInteraction();
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi khi gọi GetRandomWord: {ex.Message}");
                MessageBox.Show("Đã xảy ra lỗi khi lấy từ vựng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DisableInteraction();
                return;
            }


            // Reset UI về trạng thái ban đầu của Flashcard
            lblWord.Text = currentVocabulary.Word;
            lblPronunciationDisplay.Text = currentVocabulary.Pronunciation; // Gán sẵn nhưng vẫn ẩn
            lblMeaningDisplay.Text = currentVocabulary.Meaning;         // Gán sẵn nhưng vẫn ẩn
            lblRemainingWords.Text = $"Còn lại: {remainingWordsCount} từ";
            lblFeedback.Text = ""; // Xóa feedback cũ
            lblFeedback.Visible = false;
            txtMeaningInput.Text = ""; // Xóa input cũ

            lblPronunciationDisplay.Visible = false;
            lblMeaningDisplay.Visible = false;
            txtMeaningInput.Visible = false;
            btnCheckMeaning.Visible = false;
            btnNextWord.Visible = false;

            btnFlipCard.Visible = true; // Hiện nút lật thẻ
            btnFlipCard.Enabled = true;
            btnPlayAudio.Visible = !string.IsNullOrEmpty(currentVocabulary.AudioUrl); // Hiện nút nghe nếu có URL
            btnPlayAudio.Enabled = true;


        }

        // --- BtnFlipCard_Click: Lật thẻ để xem chi tiết ---
        private void BtnFlipCard_Click(object sender, EventArgs e)
        {
            lblPronunciationDisplay.Visible = true;
            lblMeaningDisplay.Visible = true;

            // Hiện các nút tương tác sau khi lật thẻ
            txtMeaningInput.Visible = true;
            btnCheckMeaning.Visible = true;
            btnNextWord.Visible = true;

            // Ẩn nút lật thẻ
            btnFlipCard.Visible = false;
            txtMeaningInput.Focus(); // Focus vào textbox để nhập
        }

        // --- BtnNextWord_Click: Chuyển sang từ tiếp theo ---
        private void BtnNextWord_Click(object sender, EventArgs e)
        {
            remainingWordsCount--;
            LoadNextWord();
        }

        // --- BtnPlayAudio_Click: Phát âm thanh ---
        private void BtnPlayAudio_Click(object sender, EventArgs e)
        {
            if (currentVocabulary != null && !string.IsNullOrEmpty(currentVocabulary.AudioUrl))
            {
                try { AudioHelper.PlayAudio(currentVocabulary.AudioUrl); }
                catch (Exception ex) { Debug.WriteLine($"Lỗi phát âm: {ex.Message}"); MessageBox.Show("Lỗi phát âm thanh.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            else
            {
                MessageBox.Show("Từ này không có âm thanh.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // --- BtnCheckMeaning_Click: Kiểm tra nghĩa người dùng nhập ---
        private void BtnCheckMeaning_Click(object sender, EventArgs e)
        {
            string userInput = txtMeaningInput.Text; // Lấy từ TextBox thay vì InputBox

            if (string.IsNullOrWhiteSpace(userInput))
            {
                lblFeedback.Text = "Vui lòng nhập nghĩa!";
                lblFeedback.ForeColor = Color.OrangeRed;
                lblFeedback.Visible = true;
                return;
            }

            if (currentVocabulary == null) return; // Không có từ để kiểm tra

            // So sánh (đã Trim và không phân biệt hoa thường)
            if (userInput.Trim().Equals(currentVocabulary.Meaning?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                lblFeedback.Text = "✅ Chính xác!";
                lblFeedback.ForeColor = Color.Green;
                lblFeedback.Visible = true;
                // Tùy chọn: Tự động chuyển từ tiếp theo nếu đúng?
                // Task.Delay(1000).ContinueWith(_ => { if (this.IsHandleCreated) this.Invoke((MethodInvoker)delegate { BtnNextWord_Click(null, null); }); });
            }
            else
            {
                lblFeedback.Text = $"❌ Sai rồi! Nghĩa đúng:\n{currentVocabulary.Meaning}";
                lblFeedback.ForeColor = Color.Red;
                lblFeedback.Visible = true;
            }
        }

        // --- Xử lý nhấn Enter trong TextBox kiểm tra nghĩa ---
        private void TxtMeaningInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnCheckMeaning_Click(sender, e); // Gọi hàm kiểm tra
                e.SuppressKeyPress = true; // Ngăn tiếng 'beep' khi nhấn Enter
            }
        }


        // --- DisableInteraction: Vô hiệu hóa các nút khi hết từ ---
        private void DisableInteraction()
        {
            btnFlipCard.Enabled = false;
            btnCheckMeaning.Enabled = false;
            btnNextWord.Enabled = false;
            btnPlayAudio.Enabled = false;
            txtMeaningInput.Enabled = false;
            lblFeedback.Visible = false;
        }
    }
}