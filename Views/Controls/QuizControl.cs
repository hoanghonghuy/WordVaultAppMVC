using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WordVaultAppMVC.Data;     // Namespace của Repositories
using WordVaultAppMVC.Models;   // Namespace của Models (Vocabulary, Topic, QuizResult)
using WordVaultAppMVC.Controllers; // Namespace của LearningController

namespace WordVaultAppMVC.Views.Controls
{
    /// <summary>
    /// UserControl quản lý giao diện và logic cho chức năng làm bài kiểm tra (Quiz).
    /// </summary>
    public class QuizControl : UserControl
    {
        #region UI Controls Fields

        // --- Header Controls ---
        private ComboBox cboTopic;             // Chọn chủ đề cho Quiz
        private NumericUpDown numQuestionCount;  // Chọn số lượng câu hỏi
        private Button btnStart;               // Bắt đầu Quiz

        // --- Main Quiz Area (Left Panel) ---
        private Label lblQuestion;             // Hiển thị từ vựng (câu hỏi)
        private Label lblProgress;             // Hiển thị tiến độ (Câu x/y)
        private RadioButton[] rdoOptions;      // Mảng chứa các RadioButton cho lựa chọn đáp án
        private Button btnBack;                // Nút quay lại câu trước
        private Button btnNext;                // Nút đi tới câu tiếp theo
        private Button btnSubmitQuiz;          // Nút nộp bài

        // --- Sidebar (Right Panel) ---
        private Label lblScore;                // Hiển thị điểm số
        private Label lblAnsweredCount;        // Hiển thị số câu đã trả lời
        private Label lblTimer;                // Hiển thị thời gian làm bài
        private Button btnRetryWrong;          // Nút để làm lại các câu sai

        // --- Navigation (Bottom Panel) ---
        private FlowLayoutPanel pnlBottomJumpButtons; // Panel chứa các nút nhảy đến câu cụ thể

        // --- Layout Controls ---
        private TableLayoutPanel mainLayoutPanel;    // Layout chính chia 2 cột (Quiz Area, Sidebar)
        private TableLayoutPanel leftPanelLayout;    // Layout cho khu vực Quiz chính bên trái
        private TableLayoutPanel rightPanelLayout;   // Layout cho Sidebar bên phải
        private Timer quizTimer;               // Timer để đếm thời gian làm bài

        #endregion

        #region Logic Fields

        // --- Quiz Data ---
        private List<Vocabulary> questions;          // Danh sách các từ vựng (câu hỏi) cho lượt quiz hiện tại
        private Dictionary<int, List<string>> questionOptions = new Dictionary<int, List<string>>(); // Lưu các lựa chọn đáp án cho mỗi câu hỏi (index -> List<string>)
        private Dictionary<int, string> userAnswers = new Dictionary<int, string>(); // Lưu câu trả lời của người dùng (index -> selected meaning)
        private List<Vocabulary> wrongWords = new List<Vocabulary>(); // Danh sách các từ trả lời sai sau khi nộp bài

        // --- State ---
        private int currentIndex = 0;                // Chỉ số của câu hỏi hiện tại đang hiển thị
        private bool topicsLoadedSuccessfully = false; // Cờ đánh dấu đã tải chủ đề thành công chưa
        private int elapsedSeconds = 0;              // Số giây đã trôi qua kể từ khi bắt đầu quiz

        // --- Dependencies ---
        private readonly VocabularyRepository vocabRepo; // Repository để lấy dữ liệu từ vựng
        private readonly TopicRepository topicRepo;     // Repository để lấy dữ liệu chủ đề
        private readonly LearningController learningController; // Controller để cập nhật trạng thái học tập khi nộp bài

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo QuizControl.
        /// </summary>
        public QuizControl()
        {
            // Khởi tạo các dependency. Cân nhắc DI.
            vocabRepo = new VocabularyRepository();
            topicRepo = new TopicRepository();
            learningController = new LearningController(); // Khởi tạo LearningController ở đây

            InitializeComponent(); // Khởi tạo giao diện

            // Chỉ tải chủ đề khi không ở chế độ Design.
            if (!this.DesignMode)
            {
                LoadTopics();
            }

            // Đặt trạng thái ban đầu
            EnableQuizControls(false);
            ResetQuizInfoLabels();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Khởi tạo và cấu hình các thành phần giao diện người dùng (Controls).
        /// </summary>
        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = SystemColors.ControlLightLight;
            this.Padding = new Padding(15);

            // --- Main Layout (2 cột: Quiz Area 65%, Sidebar 35%; 3 hàng: Header, Content, Bottom Nav) ---
            mainLayoutPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, BackColor = Color.Transparent };
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            mainLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // Hàng 0: Header
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // Hàng 1: Content Area
            mainLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 55F));  // Hàng 2: Bottom Jump Buttons

            #region Header Setup (Row 0)
            // Panel chứa ComboBox chọn chủ đề, NumericUpDown chọn số câu, và nút Start
            var headerFlowPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, AutoSize = true, Padding = new Padding(0, 0, 0, 10) };
            cboTopic = new ComboBox { Name = "cboTopic", Anchor = AnchorStyles.Left, Margin = new Padding(3, 6, 3, 3), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9F) }; // Tăng Width
            numQuestionCount = new NumericUpDown { Name = "numQuestionCount", Minimum = 1, Maximum = 100, Value = 10, Width = 70, Anchor = AnchorStyles.Left, Margin = new Padding(3, 6, 3, 3), Font = new Font("Segoe UI", 9F), TextAlign = HorizontalAlignment.Center }; // Giảm Value, tăng Width
            btnStart = new Button { Name = "btnStart", Text = "Bắt đầu Quiz", Anchor = AnchorStyles.Left, Margin = new Padding(10, 3, 3, 3), AutoSize = true, BackColor = Color.MediumSeaGreen, ForeColor = Color.White, Font = new Font("Segoe UI", 9F, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Height = 30 };
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.Click += BtnStart_Click;
            headerFlowPanel.Controls.AddRange(new Control[] {
                new Label { Text = "Chủ đề:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 8, 3, 0), Font = new Font("Segoe UI", 9F) }, cboTopic,
                new Label { Text = "Số câu:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(10, 8, 3, 0), Font = new Font("Segoe UI", 9F) }, numQuestionCount,
                btnStart
            });
            mainLayoutPanel.Controls.Add(headerFlowPanel, 0, 0);
            mainLayoutPanel.SetColumnSpan(headerFlowPanel, 2); // Header kéo dài 2 cột
            #endregion

            #region Left Panel Setup (Quiz Area - Column 0, Row 1)
            // Panel chứa câu hỏi, các lựa chọn đáp án, và nút điều hướng Back/Next/Submit
            leftPanelLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Padding = new Padding(5, 0, 20, 0) };
            leftPanelLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // Hàng 0: Câu hỏi (Từ)
            leftPanelLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // Hàng 1: Tiến độ
            leftPanelLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // Hàng 2: Các lựa chọn đáp án
            leftPanelLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // Hàng 3: Nút điều hướng

            lblQuestion = new Label { Name = "lblQuestion", Font = new Font("Segoe UI", 18F, FontStyle.Bold), AutoSize = true, Dock = DockStyle.Top, Text = "Từ: ..." };
            lblProgress = new Label { Name = "lblProgress", Font = new Font("Segoe UI", 10F), ForeColor = Color.Gray, AutoSize = true, Dock = DockStyle.Top, Text = "Câu 0/0", Padding = new Padding(0, 0, 0, 20) };
            leftPanelLayout.Controls.Add(lblQuestion, 0, 0);
            leftPanelLayout.Controls.Add(lblProgress, 0, 1);

            // Panel chứa các RadioButton lựa chọn
            var optionsFlowPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, AutoSize = true, WrapContents = false, Padding = new Padding(0, 5, 0, 5) };
            rdoOptions = new RadioButton[4];
            for (int i = 0; i < 4; i++)
            {
                rdoOptions[i] = new RadioButton { Name = $"rdoOption{i + 1}", Font = new Font("Segoe UI", 11F), Padding = new Padding(5), Margin = new Padding(0, 5, 0, 5), AutoSize = true, Visible = false }; // Ban đầu ẩn
                rdoOptions[i].CheckedChanged += Option_CheckedChanged; // Lưu câu trả lời khi chọn
                rdoOptions[i].CheckedChanged += RadioButton_StyleOnChange; // Đổi style khi chọn
                optionsFlowPanel.Controls.Add(rdoOptions[i]);
            }
            leftPanelLayout.Controls.Add(optionsFlowPanel, 0, 2);

            // Panel chứa các nút điều hướng Back/Next/Submit
            var navButtonFlowPanel = new FlowLayoutPanel { Dock = DockStyle.Top, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, AutoSize = true, Padding = new Padding(0, 15, 0, 0) };
            btnBack = new Button { Name = "btnBack", Text = "← Quay lại", Width = 100, AutoSize = true, BackColor = Color.WhiteSmoke, Font = new Font("Segoe UI", 9F), FlatStyle = FlatStyle.Flat, Margin = new Padding(0, 5, 5, 5) };
            btnBack.FlatAppearance.BorderColor = Color.LightGray; btnBack.FlatAppearance.BorderSize = 1;
            btnBack.Click += (s, e) => JumpHandler(-1); // Sự kiện Back

            btnNext = new Button { Name = "btnNext", Text = "Tiếp theo >", Width = 100, AutoSize = true, BackColor = Color.WhiteSmoke, Font = new Font("Segoe UI", 9F), FlatStyle = FlatStyle.Flat, Margin = new Padding(5, 5, 5, 5) };
            btnNext.FlatAppearance.BorderColor = Color.LightGray; btnNext.FlatAppearance.BorderSize = 1;
            btnNext.Click += (s, e) => JumpHandler(1); // Sự kiện Next

            btnSubmitQuiz = new Button { Name = "btnSubmitQuiz", Text = "Nộp bài", Width = 100, AutoSize = true, BackColor = Color.DodgerBlue, ForeColor = Color.White, Font = new Font("Segoe UI", 9F, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Margin = new Padding(15, 5, 5, 5) }; // Tăng margin trái
            btnSubmitQuiz.FlatAppearance.BorderSize = 0;
            btnSubmitQuiz.Click += SubmitQuiz; // Sự kiện Nộp bài

            navButtonFlowPanel.Controls.AddRange(new Control[] { btnBack, btnNext, btnSubmitQuiz });
            leftPanelLayout.Controls.Add(navButtonFlowPanel, 0, 3);
            mainLayoutPanel.Controls.Add(leftPanelLayout, 0, 1); // Thêm Left Panel vào Main Layout
            #endregion

            #region Right Panel Setup (Sidebar - Column 1, Row 1)
            // Panel chứa thông tin thống kê (Điểm, Đã trả lời, Thời gian) và nút Luyện lại
            rightPanelLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 6, BackColor = Color.FromArgb(240, 243, 247), Padding = new Padding(15) };
            rightPanelLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Hàng 0: Title
            rightPanelLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Hàng 1: Score
            rightPanelLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Hàng 2: Answered Count
            rightPanelLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Hàng 3: Timer
            rightPanelLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Hàng 4: Khoảng trống
            rightPanelLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Hàng 5: Retry Button

            var lblStatsTitle = new Label { Text = "Thống kê Quiz", Font = new Font("Segoe UI", 10F, FontStyle.Bold), AutoSize = true, Dock = DockStyle.Top, Padding = new Padding(0, 0, 0, 10) }; // Tăng padding dưới
            lblScore = new Label { Name = "lblScore", Text = "Điểm: __ / __", Font = new Font("Segoe UI", 9F), AutoSize = true, Dock = DockStyle.Top, Padding = new Padding(5, 0, 0, 10) };
            lblAnsweredCount = new Label { Name = "lblAnsweredCount", Text = "Đã trả lời: 0 / 0", Font = new Font("Segoe UI", 9F), AutoSize = true, Dock = DockStyle.Top, Padding = new Padding(5, 0, 0, 10) };
            lblTimer = new Label { Name = "lblTimer", Text = "Thời gian: 00:00", Font = new Font("Segoe UI", 9F), AutoSize = true, Dock = DockStyle.Top, Padding = new Padding(5, 0, 0, 10) };
            rightPanelLayout.Controls.Add(lblStatsTitle, 0, 0);
            rightPanelLayout.Controls.Add(lblScore, 0, 1);
            rightPanelLayout.Controls.Add(lblAnsweredCount, 0, 2);
            rightPanelLayout.Controls.Add(lblTimer, 0, 3);

            // Nút Luyện lại các câu sai (ban đầu ẩn)
            btnRetryWrong = new Button { Name = "btnRetryWrong", Text = "Luyện lại từ sai", Dock = DockStyle.Bottom, Height = 40, Visible = false, BackColor = Color.RoyalBlue, ForeColor = Color.White, Font = new Font("Segoe UI", 10F, FontStyle.Bold), FlatStyle = FlatStyle.Flat };
            btnRetryWrong.FlatAppearance.BorderSize = 0;
            btnRetryWrong.Click += RetryWrongAnswers;
            rightPanelLayout.Controls.Add(btnRetryWrong, 0, 5); // Đặt vào hàng cuối
            mainLayoutPanel.Controls.Add(rightPanelLayout, 1, 1); // Thêm Right Panel vào Main Layout
            #endregion

            #region Bottom Navigation Panel Setup (Row 2)
            // Panel chứa các nút nhỏ để nhảy đến câu hỏi cụ thể
            pnlBottomJumpButtons = new FlowLayoutPanel { Name = "pnlBottomJumpButtons", AutoScroll = true, Dock = DockStyle.Fill, WrapContents = false, FlowDirection = FlowDirection.LeftToRight, Padding = new Padding(5, 10, 5, 5), BackColor = Color.WhiteSmoke };
            mainLayoutPanel.Controls.Add(pnlBottomJumpButtons, 0, 2);
            mainLayoutPanel.SetColumnSpan(pnlBottomJumpButtons, 2); // Panel này kéo dài 2 cột
            #endregion

            // --- Timer Setup ---
            quizTimer = new Timer { Interval = 1000 }; // Tick mỗi giây
            quizTimer.Tick += QuizTimer_Tick; // Gán sự kiện Tick

            // --- Add Main Layout ---
            this.Controls.Add(mainLayoutPanel);
            mainLayoutPanel.BringToFront();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Xử lý sự kiện Tick của Timer, cập nhật thời gian làm bài.
        /// </summary>
        private void QuizTimer_Tick(object sender, EventArgs e)
        {
            elapsedSeconds++; // Tăng số giây đã trôi qua
            // Định dạng thời gian thành chuỗi MM:SS
            TimeSpan time = TimeSpan.FromSeconds(elapsedSeconds);
            lblTimer.Text = $"Thời gian: {time:mm\\:ss}"; // Sử dụng định dạng tùy chỉnh
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút "Bắt đầu Quiz".
        /// Chuẩn bị dữ liệu câu hỏi, đáp án và khởi động Quiz.
        /// </summary>
        private void BtnStart_Click(object sender, EventArgs e)
        {
            // --- Validate Input ---
            if (!topicsLoadedSuccessfully || cboTopic.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một chủ đề hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string selectedTopic = cboTopic.SelectedItem.ToString();
            int requestedCount = (int)numQuestionCount.Value;

            // --- Reset UI for New Quiz ---
            ResetQuizInfoLabels(requestedCount); // Đặt lại các label điểm, thời gian,...
            questions = null;                    // Xóa danh sách câu hỏi cũ
            questionOptions.Clear();             // Xóa bộ đáp án cũ
            userAnswers.Clear();                 // Xóa câu trả lời cũ
            pnlBottomJumpButtons.Controls.Clear(); // Xóa nút jump cũ
            btnRetryWrong.Visible = false;        // Ẩn nút luyện lại
            wrongWords.Clear();                  // Xóa danh sách từ sai cũ
            currentIndex = 0;                    // Đặt lại index câu hỏi


            // --- Load Quiz Data ---
            Debug.WriteLine($"[INFO] BtnStart_Click: Loading {requestedCount} questions for topic '{selectedTopic}'...");
            try
            {
                // Lấy danh sách từ vựng cho chủ đề đã chọn
                List<Vocabulary> vocabForTopic = vocabRepo.GetVocabularyByTopic(selectedTopic);

                // Kiểm tra xem có đủ từ vựng không
                if (vocabForTopic == null || vocabForTopic.Count < requestedCount)
                {
                    // Cần ít nhất 4 từ để tạo đáp án nhiễu, và đủ số lượng yêu cầu
                    if (vocabForTopic == null || vocabForTopic.Count < 4)
                    {
                        MessageBox.Show($"Chủ đề '{selectedTopic}' cần có ít nhất 4 từ vựng để tạo Quiz.", "Không đủ từ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"Chủ đề '{selectedTopic}' không có đủ {requestedCount} từ vựng (chỉ có {vocabForTopic.Count}).\nVui lòng chọn số lượng nhỏ hơn hoặc bổ sung từ.", "Không đủ từ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    return; // Dừng lại nếu không đủ từ
                }

                // Chọn ngẫu nhiên số lượng câu hỏi yêu cầu
                questions = vocabForTopic.OrderBy(v => Guid.NewGuid()).Take(requestedCount).ToList();
                Debug.WriteLine($"[INFO] BtnStart_Click: Selected {questions.Count} questions.");

                // Lấy tất cả các từ khác (không nằm trong câu hỏi) để tạo đáp án nhiễu
                List<Vocabulary> allOtherVocab = vocabRepo.GetAllVocabulary() ?? new List<Vocabulary>();
                var questionIds = new HashSet<int>(questions.Select(q => q.Id));
                // Lọc ra các từ có nghĩa, không phải là câu hỏi hiện tại
                allOtherVocab = allOtherVocab.Where(v => v != null && !questionIds.Contains(v.Id) && !string.IsNullOrEmpty(v.Meaning)).ToList();
                Debug.WriteLine($"[INFO] BtnStart_Click: Found {allOtherVocab.Count} other vocab items for generating distractors.");


                // --- Generate Options for Each Question ---
                // Cân nhắc chuyển logic này ra hàm riêng
                GenerateQuestionOptions(questions, allOtherVocab);

                // --- Final Setup & Start ---
                EnableQuizControls(true); // Bật các control làm quiz
                CreateJumpButtons(questions.Count); // Tạo các nút jump
                StartQuizTimer(); // Bắt đầu đếm giờ
                LoadQuestion(); // Tải câu hỏi đầu tiên
                UpdateJumpButtonStyles(); // Cập nhật style nút jump
                Debug.WriteLine("[INFO] BtnStart_Click: Quiz started.");

            }
            catch (Exception ex) // Bắt lỗi trong quá trình chuẩn bị dữ liệu
            {
                Debug.WriteLine($"[ERROR] BtnStart_Click: Lỗi khi chuẩn bị Quiz: {ex.ToString()}");
                MessageBox.Show("Đã xảy ra lỗi khi tải dữ liệu Quiz. Vui lòng thử lại.", "Lỗi Tải Quiz", MessageBoxButtons.OK, MessageBoxIcon.Error);
                EnableQuizControls(false); // Tắt các control quiz nếu lỗi
                ResetQuizInfoLabels();
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi người dùng chọn một RadioButton đáp án.
        /// Lưu lại câu trả lời và cập nhật giao diện.
        /// </summary>
        private void Option_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton RdoClick = sender as RadioButton;
            // Chỉ xử lý khi RadioButton được chọn (Checked)
            if (RdoClick != null && RdoClick.Checked)
            {
                // Bỏ chọn các RadioButton khác (mặc dù WinForms thường tự làm điều này trong cùng container)
                // foreach (RadioButton RdoItem in rdoOptions)
                // {
                //     if (RdoItem != null && RdoItem != RdoClick && RdoItem.Checked)
                //     {
                //         RdoItem.Checked = false;
                //     }
                // }

                // Lưu câu trả lời hiện tại
                SaveAnswer();
                // Cập nhật style cho các nút jump (đánh dấu câu đã trả lời)
                UpdateJumpButtonStyles();
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút Jump ở dưới cùng.
        /// Chuyển đến câu hỏi tương ứng.
        /// </summary>
        private void JumpButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            // Lấy index từ Tag của nút
            if (clickedButton != null && clickedButton.Tag is int index)
            {
                // Kiểm tra index hợp lệ và chuyển câu hỏi
                if (index >= 0 && index < (questions?.Count ?? 0))
                {
                    SaveAnswer(); // Lưu câu trả lời hiện tại trước khi chuyển
                    currentIndex = index; // Cập nhật index hiện tại
                    LoadQuestion(); // Tải câu hỏi mới
                    UpdateJumpButtonStyles(); // Cập nhật style nút jump
                }
                else
                {
                    Debug.WriteLine($"[WARN] JumpButton_Click: Invalid index {index} requested.");
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút "Nộp bài".
        /// Tính điểm, hiển thị kết quả và cập nhật trạng thái học tập.
        /// </summary>
        private void SubmitQuiz(object sender, EventArgs e)
        {
            StopQuizTimer(); // Dừng đếm giờ khi nộp bài

            if (questions == null || questions.Count == 0)
            {
                Debug.WriteLine("[WARN] SubmitQuiz: No questions available to submit.");
                return;
            }

            // Xác nhận nếu chưa trả lời hết câu hỏi
            int answeredCount = userAnswers.Count;
            if (answeredCount < questions.Count)
            {
                DialogResult confirm = MessageBox.Show(
                    $"Bạn mới trả lời {answeredCount}/{questions.Count} câu.\nBạn có chắc chắn muốn nộp bài không?",
                    "Xác nhận Nộp bài",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm == DialogResult.No)
                {
                    StartQuizTimer(); // Bật lại timer nếu không nộp
                    return; // Không nộp bài
                }
            }

            // --- Tính toán kết quả ---
            int correctCount = 0;
            wrongWords.Clear(); // Xóa danh sách từ sai cũ
            var resultDetails = new List<(Vocabulary vocab, string userAnswer, bool isCorrect)>(); // List để lưu chi tiết kết quả

            for (int i = 0; i < questions.Count; i++)
            {
                Vocabulary word = questions[i];
                userAnswers.TryGetValue(i, out string selectedAnswer); // Lấy câu trả lời của user
                selectedAnswer = selectedAnswer ?? "(Chưa trả lời)"; // Mặc định nếu chưa trả lời
                string correctAnswer = word.Meaning?.Trim(); // Lấy đáp án đúng

                // So sánh đáp án (không phân biệt hoa thường)
                bool isCorrect = !string.IsNullOrEmpty(correctAnswer) &&
                                 string.Equals(selectedAnswer.Trim(), correctAnswer, StringComparison.OrdinalIgnoreCase);

                // Thêm vào danh sách chi tiết kết quả
                resultDetails.Add((word, selectedAnswer, isCorrect));

                // Cập nhật trạng thái học tập và danh sách từ sai
                try
                {
                    if (isCorrect)
                    {
                        correctCount++;
                        learningController.UpdateLearningStatus(word.Id.ToString(), "Đã học");
                    }
                    else
                    {
                        wrongWords.Add(word); // Thêm vào danh sách từ sai
                        learningController.UpdateLearningStatus(word.Id.ToString(), "Đang học");
                    }
                }
                catch (Exception ex) // Bắt lỗi khi cập nhật learning status
                {
                    Debug.WriteLine($"[ERROR] SubmitQuiz: Lỗi khi cập nhật LearningStatus cho Word ID {word.Id}: {ex.Message}");
                    // Có thể log lỗi nhưng không nên dừng quá trình nộp bài
                }
            }

            // --- Hiển thị kết quả ---
            lblScore.Text = $"Điểm: {correctCount} / {questions.Count}"; // Cập nhật điểm số
            Debug.WriteLine($"[INFO] SubmitQuiz: Score = {correctCount}/{questions.Count}. Wrong answers = {wrongWords.Count}");

            // Hiển thị Form tổng kết chi tiết (nếu có kết quả)
            if (resultDetails.Any())
            {
                using (var summaryForm = new ResultSummaryForm(resultDetails))
                {
                    summaryForm.ShowDialog(this.FindForm()); // Hiển thị form tổng kết
                }
            }

            // Hiển thị nút "Luyện lại từ sai" nếu có câu sai
            btnRetryWrong.Visible = wrongWords.Any();

            // --- Kết thúc Quiz ---
            EnableQuizControls(false); // Vô hiệu hóa các control làm quiz
            UpdateJumpButtonStylesAfterSubmit(); // Cập nhật style nút jump sau khi nộp bài
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút "Luyện lại từ sai".
        /// Bắt đầu một lượt Quiz mới chỉ với các câu đã trả lời sai.
        /// </summary>
        private void RetryWrongAnswers(object sender, EventArgs e)
        {
            if (wrongWords == null || !wrongWords.Any())
            {
                Debug.WriteLine("[WARN] RetryWrongAnswers: Không có từ sai để luyện lại.");
                return; // Không có gì để làm
            }

            Debug.WriteLine($"[INFO] RetryWrongAnswers: Starting retry quiz with {wrongWords.Count} wrong words.");

            // --- Chuẩn bị cho lượt Retry ---
            questions = new List<Vocabulary>(wrongWords); // Lấy danh sách câu hỏi từ danh sách sai
            int retryCount = questions.Count;
            ResetQuizInfoLabels(retryCount);     // Reset các label điểm, thời gian,...
            questionOptions.Clear();             // Xóa bộ đáp án cũ
            userAnswers.Clear();                 // Xóa câu trả lời cũ
            pnlBottomJumpButtons.Controls.Clear(); // Xóa nút jump cũ
            wrongWords.Clear();                  // Xóa danh sách từ sai cũ (cho lượt retry này)
            btnRetryWrong.Visible = false;        // Ẩn nút retry
            currentIndex = 0;                    // Bắt đầu từ câu đầu tiên


            // --- Tạo lại đáp án cho các câu hỏi Retry ---
            try
            {
                List<Vocabulary> allOtherVocab = vocabRepo.GetAllVocabulary() ?? new List<Vocabulary>();
                var currentQuestionIds = new HashSet<int>(questions.Select(q => q.Id));
                allOtherVocab = allOtherVocab.Where(v => v != null && !currentQuestionIds.Contains(v.Id) && !string.IsNullOrEmpty(v.Meaning)).ToList();
                Debug.WriteLine($"[INFO] RetryWrongAnswers: Found {allOtherVocab.Count} other vocab items for distractors.");

                GenerateQuestionOptions(questions, allOtherVocab); // Tạo lại bộ đáp án
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] RetryWrongAnswers: Lỗi khi tạo đáp án cho lượt retry: {ex.ToString()}");
                MessageBox.Show("Đã xảy ra lỗi khi chuẩn bị dữ liệu luyện lại. Vui lòng thử lại.", "Lỗi Luyện Lại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                EnableQuizControls(false);
                ResetQuizInfoLabels();
                return;
            }


            // --- Bắt đầu lượt Retry ---
            EnableQuizControls(true);             // Bật các control làm quiz
            CreateJumpButtons(retryCount);        // Tạo lại nút jump
            StartQuizTimer();                     // Bắt đầu lại timer
            LoadQuestion();                       // Tải câu hỏi đầu tiên
            UpdateJumpButtonStyles();             // Cập nhật style nút jump
            Debug.WriteLine("[INFO] RetryWrongAnswers: Retry quiz started.");
        }


        /// <summary>
        /// Xử lý sự kiện thay đổi lựa chọn RadioButton (để thay đổi style).
        /// </summary>
        private void RadioButton_StyleOnChange(object sender, EventArgs e)
        {
            RadioButton currentRadio = sender as RadioButton;
            if (currentRadio == null) return;

            // Đặt lại style cho tất cả các RadioButton khác
            foreach (RadioButton rdo in rdoOptions)
            {
                if (rdo != null && rdo != currentRadio)
                {
                    rdo.ForeColor = SystemColors.ControlText;
                    rdo.Font = new Font(rdo.Font, FontStyle.Regular);
                }
            }

            // Đặt style cho RadioButton được chọn/bỏ chọn
            if (currentRadio.Checked)
            {
                currentRadio.ForeColor = Color.DodgerBlue; // Màu khi được chọn
                currentRadio.Font = new Font(currentRadio.Font, FontStyle.Bold);
            }
            else
            {
                currentRadio.ForeColor = SystemColors.ControlText; // Màu khi không được chọn
                currentRadio.Font = new Font(currentRadio.Font, FontStyle.Regular);
            }
        }


        #endregion

        #region Quiz Flow Logic

        /// <summary>
        /// Xử lý việc chuyển câu hỏi tới lui (Back/Next).
        /// </summary>
        /// <param name="direction"> -1 để quay lại, 1 để đi tới.</param>
        private void JumpHandler(int direction)
        {
            if (questions == null || questions.Count == 0) return;

            SaveAnswer(); // Lưu câu trả lời hiện tại

            int newIndex = currentIndex + direction;

            // Kiểm tra xem index mới có hợp lệ không
            if (newIndex >= 0 && newIndex < questions.Count)
            {
                currentIndex = newIndex; // Cập nhật index
                LoadQuestion(); // Tải câu hỏi mới
                UpdateJumpButtonStyles(); // Cập nhật style nút jump
            }
        }

        /// <summary>
        /// Tải dữ liệu và hiển thị câu hỏi tại vị trí currentIndex.
        /// </summary>
        private void LoadQuestion()
        {
            // Kiểm tra trạng thái quiz
            if (questions == null || questions.Count == 0)
            {
                EnableQuizControls(false);
                lblQuestion.Text = "Không có câu hỏi.";
                return;
            }
            // Đảm bảo currentIndex hợp lệ
            if (currentIndex < 0 || currentIndex >= questions.Count)
            {
                Debug.WriteLine($"[WARN] LoadQuestion: currentIndex {currentIndex} is out of bounds (0-{questions.Count - 1}). Resetting to 0.");
                currentIndex = 0;
            }

            EnableQuizControls(true); // Đảm bảo các control được bật

            // Lấy câu hỏi và đáp án hiện tại
            Vocabulary currentQ = questions[currentIndex];
            List<string> currentOptions = questionOptions.ContainsKey(currentIndex)
                                         ? questionOptions[currentIndex]
                                         : new List<string>(); // Tránh lỗi nếu không có options

            if (!currentOptions.Any()) { Debug.WriteLine($"[WARN] LoadQuestion: No options found for index {currentIndex}."); }


            // Cập nhật UI
            lblQuestion.Text = $"Từ: {currentQ.Word ?? "N/A"}";
            lblProgress.Text = $"Câu {currentIndex + 1}/{questions.Count}";
            lblAnsweredCount.Text = $"Đã trả lời: {userAnswers.Count} / {questions.Count}"; // Cập nhật số câu đã trả lời

            // Lấy câu trả lời đã lưu (nếu có)
            userAnswers.TryGetValue(currentIndex, out string savedAnswer);
            savedAnswer = savedAnswer?.Trim(); // Trim để so sánh

            // Cập nhật các RadioButton
            for (int i = 0; i < rdoOptions.Length; i++)
            {
                if (rdoOptions[i] == null) continue; // Bỏ qua nếu RadioButton null

                // Tạm thời gỡ bỏ event handler để tránh trigger khi thay đổi Checked
                rdoOptions[i].CheckedChanged -= Option_CheckedChanged;
                rdoOptions[i].CheckedChanged -= RadioButton_StyleOnChange;

                if (i < currentOptions.Count)
                {
                    rdoOptions[i].Text = currentOptions[i]; // Gán text đáp án
                    rdoOptions[i].Visible = true;         // Hiển thị RadioButton

                    // Kiểm tra xem đáp án này có phải là đáp án đã lưu không
                    bool shouldBeChecked = !string.IsNullOrEmpty(savedAnswer) &&
                                           string.Equals(rdoOptions[i].Text?.Trim(), savedAnswer, StringComparison.OrdinalIgnoreCase);
                    rdoOptions[i].Checked = shouldBeChecked;
                }
                else // Nếu không đủ 4 đáp án
                {
                    rdoOptions[i].Text = "";           // Xóa text
                    rdoOptions[i].Visible = false;     // Ẩn RadioButton
                    rdoOptions[i].Checked = false;     // Bỏ chọn
                }
                // Gắn lại event handler
                rdoOptions[i].CheckedChanged += Option_CheckedChanged;
                rdoOptions[i].CheckedChanged += RadioButton_StyleOnChange;
                // Cập nhật lại style ngay lập tức
                RadioButton_StyleOnChange(rdoOptions[i], EventArgs.Empty);
            }

            // Cập nhật trạng thái nút Back/Next
            EnableQuizControls(true); // Gọi lại để cập nhật trạng thái nút Back/Next
                                      // Đảm bảo nút Jump hiện tại được highlight
            UpdateJumpButtonStyles();
        }


        /// <summary>
        /// Lưu câu trả lời được chọn hiện tại vào Dictionary userAnswers.
        /// </summary>
        private void SaveAnswer()
        {
            // Kiểm tra xem quiz có đang diễn ra không
            if (questions == null || currentIndex < 0 || currentIndex >= questions.Count)
            {
                Debug.WriteLine("[WARN] SaveAnswer: Cannot save answer, quiz not active or index out of bounds.");
                return;
            }

            bool answerChanged = false; // Cờ để kiểm tra xem câu trả lời có thay đổi không
            string previousAnswer = userAnswers.ContainsKey(currentIndex) ? userAnswers[currentIndex] : null;

            // Tìm RadioButton đang được chọn
            var selectedOption = rdoOptions?.FirstOrDefault(r => r != null && r.Checked);
            string currentSelectedText = selectedOption?.Text?.Trim(); // Lấy text đã trim

            // Nếu có lựa chọn được chọn
            if (currentSelectedText != null)
            {
                // Chỉ lưu nếu câu trả lời mới khác câu trả lời cũ hoặc chưa có câu trả lời cũ
                if (previousAnswer == null || !previousAnswer.Equals(currentSelectedText, StringComparison.OrdinalIgnoreCase))
                {
                    userAnswers[currentIndex] = currentSelectedText;
                    answerChanged = true;
                    Debug.WriteLine($"[INFO] SaveAnswer: Saved answer for index {currentIndex}: '{currentSelectedText}'");
                }
            }
            // Nếu không có lựa chọn nào được chọn
            else
            {
                // Chỉ xóa nếu trước đó đã có câu trả lời được lưu
                if (previousAnswer != null)
                {
                    userAnswers.Remove(currentIndex);
                    answerChanged = true;
                    Debug.WriteLine($"[INFO] SaveAnswer: Removed answer for index {currentIndex}");
                }
            }

            // Chỉ cập nhật label số câu đã trả lời nếu có sự thay đổi
            if (answerChanged && questions != null)
            {
                lblAnsweredCount.Text = $"Đã trả lời: {userAnswers.Count} / {questions.Count}";
            }
        }


        #endregion

        #region Data Loading & Preparation

        /// <summary>
        /// Tải danh sách các chủ đề từ TopicRepository và điền vào ComboBox.
        /// </summary>
        private void LoadTopics()
        {
            // Chỉ tải một lần
            if (topicsLoadedSuccessfully) return;

            Debug.WriteLine("[INFO] LoadTopics: Bắt đầu tải danh sách chủ đề...");
            if (cboTopic == null) { Debug.WriteLine("[ERROR] LoadTopics: cboTopic is null."); return; }

            cboTopic.Items.Clear();
            List<Topic> topics = null;
            bool loadSuccess = false;

            try
            {
                topics = topicRepo.GetAllTopics();
                if (topics != null)
                {
                    Debug.WriteLine($"[INFO] LoadTopics: Lấy được {topics.Count} chủ đề.");
                    var validTopics = topics.Where(t => t != null && !string.IsNullOrEmpty(t.Name)).ToList();
                    if (validTopics.Any())
                    {
                        foreach (var topic in validTopics) { cboTopic.Items.Add(topic.Name); }
                        Debug.WriteLine($"[INFO] LoadTopics: Đã thêm {cboTopic.Items.Count} chủ đề vào ComboBox.");
                        loadSuccess = true;
                    }
                    else { Debug.WriteLine("[WARN] LoadTopics: Danh sách chủ đề rỗng hoặc không hợp lệ."); }
                }
                else { Debug.WriteLine("[ERROR] LoadTopics: GetAllTopics() trả về null."); }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] LoadTopics: Lỗi khi tải: {ex.ToString()}");
                MessageBox.Show($"Đã xảy ra lỗi khi tải danh sách chủ đề:\n{ex.Message}", "Lỗi Tải Chủ Đề", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            topicsLoadedSuccessfully = loadSuccess;

            // Cấu hình ComboBox và nút Start sau khi tải
            if (cboTopic.Items.Count > 0)
            {
                cboTopic.SelectedIndex = 0;
                btnStart.Enabled = true; // Bật nút Start nếu có chủ đề
                Debug.WriteLine("[INFO] LoadTopics: Đã chọn chủ đề đầu tiên.");
            }
            else
            {
                Debug.WriteLine("[WARN] LoadTopics: ComboBox rỗng.");
                cboTopic.Items.Add("(Không có chủ đề)"); // Thêm mục thông báo
                cboTopic.SelectedIndex = 0;
                cboTopic.Enabled = false; // Vô hiệu hóa ComboBox
                btnStart.Enabled = false; // Vô hiệu hóa nút Start
            }
        }

        /// <summary>
        /// Tạo bộ các lựa chọn đáp án (1 đúng, 3 sai) cho danh sách câu hỏi.
        /// </summary>
        /// <param name="quizQuestions">Danh sách các câu hỏi Vocabulary.</param>
        /// <param name="distractorPool">Danh sách các từ Vocabulary khác để lấy đáp án sai.</param>
        private void GenerateQuestionOptions(List<Vocabulary> quizQuestions, List<Vocabulary> distractorPool)
        {
            if (quizQuestions == null || distractorPool == null)
            {
                Debug.WriteLine("[ERROR] GenerateQuestionOptions: Input lists cannot be null.");
                return;
            }

            questionOptions.Clear(); // Xóa bộ đáp án cũ
            var random = new Random();

            Debug.WriteLine($"[INFO] GenerateQuestionOptions: Generating options for {quizQuestions.Count} questions using a pool of {distractorPool.Count} distractors.");

            for (int i = 0; i < quizQuestions.Count; i++)
            {
                Vocabulary currentQuestion = quizQuestions[i];
                string correctAnswer = currentQuestion.Meaning?.Trim();

                // Lấy 3 đáp án sai ngẫu nhiên từ pool, đảm bảo không trùng đáp án đúng và không rỗng
                var wrongAnswers = distractorPool
                    .Where(v => !string.IsNullOrEmpty(v.Meaning) && !v.Meaning.Trim().Equals(correctAnswer, StringComparison.OrdinalIgnoreCase)) // Loại bỏ đáp án trùng và rỗng
                    .OrderBy(_ => random.Next()) // Xáo trộn ngẫu nhiên
                    .Select(v => v.Meaning.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase) // Loại bỏ trùng lặp sau khi trim và không phân biệt hoa thường
                    .Take(3)
                    .ToList();

                // Nếu không đủ 3 đáp án sai từ pool, tạo đáp án giả
                while (wrongAnswers.Count < 3)
                {
                    string fakeAnswer = $"Đáp án giả {random.Next(100, 999)}";
                    // Đảm bảo đáp án giả không trùng với đáp án đúng hoặc các đáp án sai đã có
                    if (!fakeAnswer.Equals(correctAnswer, StringComparison.OrdinalIgnoreCase) &&
                        !wrongAnswers.Contains(fakeAnswer, StringComparer.OrdinalIgnoreCase))
                    {
                        wrongAnswers.Add(fakeAnswer);
                    }
                }

                // Tạo danh sách đầy đủ 4 lựa chọn (1 đúng, 3 sai)
                var allOptions = new List<string>();
                if (!string.IsNullOrEmpty(correctAnswer)) // Chỉ thêm đáp án đúng nếu nó không rỗng
                {
                    allOptions.Add(correctAnswer);
                }
                else // Nếu đáp án đúng bị rỗng, thêm 1 đáp án giả nữa
                {
                    Debug.WriteLine($"[WARN] GenerateQuestionOptions: Correct answer for '{currentQuestion.Word}' is null or empty. Adding extra fake answer.");
                    string fakeAnswer = $"Đáp án giả {random.Next(1000, 1999)}";
                    if (!wrongAnswers.Contains(fakeAnswer, StringComparer.OrdinalIgnoreCase)) wrongAnswers.Add(fakeAnswer);
                    // Cần đảm bảo vẫn đủ 4 options tổng cộng
                    while (wrongAnswers.Count < 4) wrongAnswers.Add($"Đáp án giả {random.Next(2000, 2999)}");
                    allOptions.AddRange(wrongAnswers.Take(4)); // Lấy đủ 4 đáp án sai/giả
                    allOptions = allOptions.Distinct(StringComparer.OrdinalIgnoreCase).ToList(); // Đảm bảo không trùng
                    while (allOptions.Count < 4) allOptions.Add($"Đáp án giả {random.Next(3000, 3999)}"); // Bổ sung nếu thiếu
                }

                // Nếu đáp án đúng không rỗng, thêm các đáp án sai vào
                if (!string.IsNullOrEmpty(correctAnswer))
                {
                    allOptions.AddRange(wrongAnswers);
                }


                // Xáo trộn thứ tự các lựa chọn
                questionOptions[i] = allOptions.OrderBy(_ => random.Next()).ToList();
                // Debug.WriteLine($"[DEBUG] Options for index {i} ('{currentQuestion.Word}'): {string.Join(" | ", questionOptions[i])}");

            }
            Debug.WriteLine("[INFO] GenerateQuestionOptions: Finished generating options.");
        }


        /// <summary>
        /// Tạo các nút Jump ở dưới cùng để điều hướng nhanh giữa các câu hỏi.
        /// </summary>
        /// <param name="questionCount">Tổng số câu hỏi.</param>
        private void CreateJumpButtons(int questionCount)
        {
            pnlBottomJumpButtons.Controls.Clear(); // Xóa nút cũ
            if (questionCount <= 0) return;

            Debug.WriteLine($"[INFO] CreateJumpButtons: Creating {questionCount} jump buttons.");
            for (int i = 0; i < questionCount; i++)
            {
                var btnBottom = new Button
                {
                    Text = (i + 1).ToString(), // Số thứ tự câu hỏi
                    Width = 35,
                    Height = 30,
                    Tag = i, // Lưu index vào Tag để JumpButton_Click sử dụng
                    Margin = new Padding(2),
                    Font = new Font("Segoe UI", 8F),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.Gainsboro // Màu nền mặc định
                };
                btnBottom.FlatAppearance.BorderColor = Color.DarkGray;
                btnBottom.FlatAppearance.BorderSize = 1;
                btnBottom.Click += JumpButton_Click; // Gán sự kiện chung
                pnlBottomJumpButtons.Controls.Add(btnBottom);
            }
        }


        #endregion

        #region UI Helper Methods

        /// <summary>
        /// Bật hoặc tắt các controls liên quan đến quá trình làm Quiz.
        /// Đồng thời quản lý trạng thái của Timer.
        /// </summary>
        /// <param name="enable">True để bật, False để tắt.</param>
        private void EnableQuizControls(bool enable)
        {
            // Kích hoạt/Vô hiệu hóa các RadioButton lựa chọn
            foreach (var rdo in rdoOptions)
            {
                if (rdo != null) rdo.Enabled = enable;
            }

            // Kích hoạt/Vô hiệu hóa các nút điều hướng quiz
            bool hasQuestions = questions != null && questions.Count > 0;
            btnBack.Enabled = enable && hasQuestions && currentIndex > 0;
            btnNext.Enabled = enable && hasQuestions && currentIndex < questions.Count - 1;
            btnSubmitQuiz.Enabled = enable && hasQuestions;
            pnlBottomJumpButtons.Enabled = enable && hasQuestions;

            // Vô hiệu hóa/Kích hoạt các control ở header
            cboTopic.Enabled = !enable;
            numQuestionCount.Enabled = !enable;
            btnStart.Enabled = !enable; // Nút Start chỉ bật khi không làm quiz

            // Nút Retry chỉ bật khi nó hiển thị VÀ không đang làm quiz
            btnRetryWrong.Enabled = btnRetryWrong.Visible && !enable;

            // Quản lý Timer
            if (enable && hasQuestions)
            {
                // Chỉ Start nếu Timer chưa chạy
                // if (!quizTimer.Enabled) { StartQuizTimer(); } // Logic start đã chuyển vào BtnStart/Retry
            }
            else
            {
                StopQuizTimer(); // Dừng Timer nếu không làm quiz hoặc không có câu hỏi
            }
        }


        /// <summary>
        /// Cập nhật giao diện (màu sắc, font) của các nút Jump ở dưới cùng.
        /// </summary>
        private void UpdateJumpButtonStyles()
        {
            UpdateJumpPanel(pnlBottomJumpButtons, false); // Gọi hàm helper
        }

        /// <summary>
        /// Cập nhật giao diện của các nút Jump sau khi đã nộp bài (hiển thị đúng/sai).
        /// </summary>
        private void UpdateJumpButtonStylesAfterSubmit()
        {
            UpdateJumpPanel(pnlBottomJumpButtons, true); // Gọi hàm helper với tham số showResult = true
        }

        /// <summary>
        /// Hàm helper cập nhật giao diện cho các nút trong panel Jump.
        /// </summary>
        /// <param name="panel">FlowLayoutPanel chứa các nút Jump.</param>
        /// <param name="showResult">True nếu đang hiển thị kết quả sau khi nộp bài.</param>
        private void UpdateJumpPanel(FlowLayoutPanel panel, bool showResult = false)
        {
            if (panel == null) return;

            foreach (Control ctrl in panel.Controls)
            {
                if (ctrl is Button btn && btn.Tag is int index)
                {
                    bool isAnswered = userAnswers.ContainsKey(index);
                    bool isCorrect = false;
                    if (showResult && isAnswered && index < questions?.Count) // Chỉ kiểm tra đúng/sai nếu showResult=true
                    {
                        string correctAnswer = questions[index].Meaning?.Trim();
                        isCorrect = !string.IsNullOrEmpty(correctAnswer) &&
                                    string.Equals(userAnswers[index].Trim(), correctAnswer, StringComparison.OrdinalIgnoreCase);
                    }


                    // Đặt màu nền và style dựa trên trạng thái
                    if (index == currentIndex && !showResult) // Câu hiện tại (khi chưa nộp bài)
                    {
                        btn.BackColor = Color.LightSteelBlue;
                        btn.Font = new Font(btn.Font, FontStyle.Bold);
                    }
                    else if (showResult) // Sau khi nộp bài
                    {
                        if (isAnswered)
                        {
                            btn.BackColor = isCorrect ? Color.LightGreen : Color.MistyRose; // Xanh lá nếu đúng, Hồng nếu sai
                            btn.Font = new Font(btn.Font, FontStyle.Regular);
                        }
                        else
                        {
                            btn.BackColor = Color.LightGray; // Xám nhạt nếu chưa trả lời
                            btn.Font = new Font(btn.Font, FontStyle.Regular);
                        }
                    }
                    else // Các câu khác (khi chưa nộp bài)
                    {
                        // Đặt màu khác nếu đã trả lời
                        btn.BackColor = isAnswered ? Color.White : Color.Gainsboro;
                        btn.Font = new Font(btn.Font, FontStyle.Regular);
                    }

                    // Reset border (tùy chọn)
                    btn.FlatAppearance.BorderColor = Color.DarkGray;
                    btn.FlatAppearance.BorderSize = 1;
                    if (index == currentIndex && !showResult)
                    { // Thêm viền đậm cho câu hiện tại
                        btn.FlatAppearance.BorderColor = Color.RoyalBlue;
                        btn.FlatAppearance.BorderSize = 2;
                    }
                }
            }
            // Đảm bảo panel vẽ lại nếu cần
            panel.Invalidate();
        }


        /// <summary>
        /// Đặt lại nội dung các Label hiển thị thông tin Quiz (Điểm, Thời gian,...).
        /// </summary>
        /// <param name="totalQuestions">Tổng số câu hỏi cho lượt quiz (nếu có).</param>
        private void ResetQuizInfoLabels(int totalQuestions = 0)
        {
            lblScore.Text = $"Điểm: __ / {(totalQuestions > 0 ? totalQuestions.ToString() : "__")}";
            lblAnsweredCount.Text = $"Đã trả lời: 0 / {(totalQuestions > 0 ? totalQuestions.ToString() : "0")}";
            lblTimer.Text = "Thời gian: 00:00";
            lblQuestion.Text = "Từ: ...";
            lblProgress.Text = "Câu 0/0";
            elapsedSeconds = 0; // Reset biến đếm giây
        }

        /// <summary>
        /// Bắt đầu hoặc tiếp tục Timer đếm thời gian.
        /// </summary>
        private void StartQuizTimer()
        {
            if (!quizTimer.Enabled)
            {
                quizTimer.Start();
                Debug.WriteLine("[INFO] Quiz Timer Started.");
            }
        }

        /// <summary>
        /// Dừng Timer đếm thời gian.
        /// </summary>
        private void StopQuizTimer()
        {
            if (quizTimer.Enabled)
            {
                quizTimer.Stop();
                Debug.WriteLine("[INFO] Quiz Timer Stopped.");
            }
        }


        #endregion
    }
}