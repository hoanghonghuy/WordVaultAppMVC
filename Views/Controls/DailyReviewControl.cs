using System;
using System.Collections.Generic;
using System.Data.SqlClient; // Needed for SQL Exception if used directly
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WordVaultAppMVC.Data;     // For Repositories
using WordVaultAppMVC.Models;   // For Vocabulary, Topic models
using WordVaultAppMVC.Controllers; // Assuming LearningController is here
using WordVaultAppMVC.Helpers;     // Assuming AudioHelper is here

namespace WordVaultAppMVC.Views.Controls
{
    /// <summary>
    /// UserControl cung cấp chức năng ôn tập từ vựng hàng ngày theo kiểu flashcard.
    /// Cho phép người dùng chọn chủ đề, số lượng từ và đánh dấu trạng thái học tập.
    /// </summary>
    public class DailyReviewControl : UserControl
    {
        #region UI Controls Fields

        // --- Header Controls ---
        private ComboBox cboTopics;
        private NumericUpDown numWordCount;
        private Button btnStart;

        // --- Card Area Controls ---
        private Label lblWord;
        private Label lblPronunciation;
        private Label lblMeaning;
        private Button btnPlayAudio;
        private Panel cardPanel; // Panel chứa nội dung flashcard

        // --- Action Button Controls ---
        private Button btnShowDetails;
        private Button btnRemembered;
        private Button btnNotRemembered;

        // --- Footer/Status Controls ---
        private Label lblProgress; // Hiển thị tiến độ (ví dụ: 3/10)

        // --- Layout Controls ---
        private TableLayoutPanel mainLayout;

        #endregion

        #region Logic Fields

        private List<Vocabulary> currentWordList = new List<Vocabulary>(); // Danh sách từ đang ôn tập
        private int currentIndex = -1; // Index của từ hiện tại trong currentWordList
        private bool topicsLoadedSuccessfully = false; // Cờ đánh dấu đã tải chủ đề thành công chưa

        // --- Dependencies ---
        private readonly VocabularyRepository vocabRepo;
        private readonly TopicRepository topicRepo;
        private readonly LearningController learningController; // Controller để cập nhật trạng thái học

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo DailyReviewControl.
        /// </summary>
        public DailyReviewControl()
        {
            // Khởi tạo các repository và controller cần thiết.
            // Trong ứng dụng lớn hơn, nên sử dụng Dependency Injection.
            vocabRepo = new VocabularyRepository();
            topicRepo = new TopicRepository();
            learningController = new LearningController();

            InitializeComponent(); // Khởi tạo giao diện người dùng

            // Chỉ tải chủ đề khi không ở chế độ Design của Visual Studio.
            if (!this.DesignMode)
            {
                // Bất đồng bộ hóa việc load topic để tránh block UI thread nếu có thể
                // Hoặc đơn giản là gọi trực tiếp nếu việc load nhanh.
                LoadTopics();
            }

            // Đặt trạng thái ban đầu cho các controls
            EnableReviewControls(false);
            ResetUI();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Khởi tạo và cấu hình các thành phần giao diện người dùng (Controls).
        /// </summary>
        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = SystemColors.Control; // Nền xám nhạt

            // --- Main Layout ---
            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4, // Header, Card Area, Buttons, Progress
                Padding = new Padding(20)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // Row 0: Header
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // Row 1: Card Area (chiếm phần lớn)
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // Row 2: Buttons
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // Row 3: Progress Label

            #region Header Setup (Row 0)
            cboTopics = new ComboBox { Name = "cboTopics", Anchor = AnchorStyles.Left, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(3, 6, 3, 3), Font = new Font("Segoe UI", 9F) };
            numWordCount = new NumericUpDown { Name = "numWordCount", Width = 80, Minimum = 1, Maximum = 100, Value = 10, Anchor = AnchorStyles.Left, Margin = new Padding(3, 6, 3, 3), Font = new Font("Segoe UI", 9F), TextAlign = HorizontalAlignment.Center };
            btnStart = new Button { Name = "btnStart", Text = "Bắt đầu học", Anchor = AnchorStyles.Left, AutoSize = true, BackColor = Color.MediumSeaGreen, ForeColor = Color.White, Font = new Font("Segoe UI", 9F, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Margin = new Padding(10, 3, 3, 3), Height = 30 }; // Đặt chiều cao
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.Click += BtnStart_Click; // Gán sự kiện Click

            var headerPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, AutoSize = true, Padding = new Padding(0, 0, 0, 15) };
            headerPanel.Controls.AddRange(new Control[] {
                new Label { Text = "Chủ đề:", AutoSize = true, Margin = new Padding(0, 8, 3, 0), Font = new Font("Segoe UI", 9F) }, cboTopics,
                new Label { Text = "Số từ:", AutoSize = true, Margin = new Padding(10, 8, 3, 0), Font = new Font("Segoe UI", 9F) }, numWordCount,
                btnStart
            });
            mainLayout.Controls.Add(headerPanel, 0, 0);
            #endregion

            #region Card Area Setup (Row 1)
            cardPanel = new Panel { Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White, Padding = new Padding(15) };
            var cardLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, AutoSize = true };
            cardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F)); // Word (chiếm nhiều không gian)
            cardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // Play Button
            cardLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // Pronunciation
            cardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F)); // Meaning (chiếm phần còn lại)


            lblWord = new Label { Name = "lblWord", Text = "...", Font = new Font("Segoe UI", 26F, FontStyle.Bold), AutoSize = false, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
            cardLayout.Controls.Add(lblWord, 0, 0);

            btnPlayAudio = new Button { Name = "btnPlayAudio", Text = "🔊 Phát âm", FlatStyle = FlatStyle.Flat, AutoSize = true, Visible = false, Anchor = AnchorStyles.None, Margin = new Padding(5) };
            btnPlayAudio.FlatAppearance.BorderSize = 0; btnPlayAudio.Click += BtnPlayAudio_Click;
            cardLayout.Controls.Add(btnPlayAudio, 0, 1);

            lblPronunciation = new Label { Name = "lblPronunciation", Text = "", Font = new Font("Segoe UI", 12F, FontStyle.Italic), AutoSize = true, Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleCenter, Visible = false, ForeColor = Color.Gray, Padding = new Padding(0, 5, 0, 10) }; // Tăng padding dưới
            cardLayout.Controls.Add(lblPronunciation, 0, 2);

            lblMeaning = new Label { Name = "lblMeaning", Text = "", Font = new Font("Segoe UI", 14F), AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Visible = false }; // AutoSize=false để fill
            cardLayout.Controls.Add(lblMeaning, 0, 3);

            cardPanel.Controls.Add(cardLayout);
            mainLayout.Controls.Add(cardPanel, 0, 1);
            #endregion

            #region Button Area Setup (Row 2)
            btnShowDetails = new Button { Name = "btnShowDetails", Text = "🔍 Hiện chi tiết", Width = 140, Height = 35, AutoSize = false, BackColor = Color.LightSkyBlue, Visible = false, Font = new Font("Segoe UI", 9F) };
            btnShowDetails.Click += BtnShowDetails_Click;

            btnRemembered = new Button { Name = "btnRemembered", Text = "✅ Đã nhớ", Width = 120, Height = 35, AutoSize = false, BackColor = Color.PaleGreen, Visible = false, Font = new Font("Segoe UI", 9F) };
            btnRemembered.Click += BtnRemembered_Click;

            btnNotRemembered = new Button { Name = "btnNotRemembered", Text = "❌ Chưa nhớ", Width = 120, Height = 35, AutoSize = false, BackColor = Color.MistyRose, Visible = false, Font = new Font("Segoe UI", 9F) };
            btnNotRemembered.Click += BtnNotRemembered_Click;

            // Sử dụng FlowLayoutPanel để căn giữa các nút dễ dàng hơn
            var buttonFlowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight, // Chảy từ trái sang phải
                WrapContents = false,                      // Không xuống dòng
                AutoSize = true,                           // Chiều cao tự động
                Padding = new Padding(0, 15, 0, 5)       // Padding trên dưới
            };
            // Thêm các nút vào FlowLayoutPanel
            buttonFlowPanel.Controls.AddRange(new Control[] { btnShowDetails, btnRemembered, btnNotRemembered });
            // Đặt Margin cho các nút
            foreach (Control btn in buttonFlowPanel.Controls) { btn.Margin = new Padding(10, 0, 10, 0); }

            // Logic căn giữa các nút trong FlowLayoutPanel
            buttonFlowPanel.Resize += (sender, args) => {
                var panel = sender as FlowLayoutPanel;
                if (panel == null) return;
                int totalButtonWidth = panel.Controls.Cast<Control>().Where(c => c.Visible).Sum(c => c.Width + c.Margin.Horizontal);
                int leftPadding = Math.Max(0, (panel.ClientSize.Width - totalButtonWidth) / 2);
                panel.Padding = new Padding(leftPadding, panel.Padding.Top, panel.Padding.Right, panel.Padding.Bottom);
            };

            mainLayout.Controls.Add(buttonFlowPanel, 0, 2);
            #endregion

            #region Progress Area Setup (Row 3)
            lblProgress = new Label { Name = "lblProgress", Text = "Tiến độ: - / -", Font = new Font("Segoe UI", 9F), ForeColor = Color.Gray, AutoSize = true, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight };
            mainLayout.Controls.Add(lblProgress, 0, 3);
            #endregion

            // --- Add Main Layout to Control ---
            this.Controls.Add(mainLayout);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút "Bắt đầu học". Tải từ vựng và bắt đầu phiên ôn tập.
        /// </summary>
        private void BtnStart_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem chủ đề đã được tải và chọn chưa.
            if (!topicsLoadedSuccessfully || cboTopics.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một chủ đề hợp lệ để bắt đầu.", "Thiếu Chủ đề", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string selectedTopic = cboTopics.SelectedItem.ToString();
            int requestedCount = (int)numWordCount.Value; // Số lượng từ người dùng yêu cầu

            try
            {
                // Tải danh sách từ cho phiên ôn tập.
                currentWordList = LoadWordsForReview(selectedTopic, requestedCount);

                // Xử lý trường hợp không có từ hoặc không đủ từ.
                if (currentWordList == null || currentWordList.Count == 0)
                {
                    MessageBox.Show($"Không tìm thấy từ vựng nào phù hợp cho chủ đề '{selectedTopic}'.", "Không có từ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResetUI(); // Đặt lại giao diện
                    return;
                }
                else if (currentWordList.Count < requestedCount)
                {
                    // Thông báo nếu số lượng từ thực tế ít hơn yêu cầu.
                    MessageBox.Show($"Chủ đề '{selectedTopic}' chỉ có {currentWordList.Count} từ. Bắt đầu ôn tập với số lượng này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Khởi tạo trạng thái cho phiên ôn tập mới.
                currentIndex = -1; // Đặt lại index, ShowNextWord sẽ tăng lên 0
                EnableReviewControls(true); // Bật các control của giao diện ôn tập
                ShowNextWord(); // Hiển thị từ đầu tiên
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] BtnStart_Click: Lỗi khi tải từ vựng ôn tập: {ex.Message}");
                MessageBox.Show("Đã xảy ra lỗi khi chuẩn bị dữ liệu ôn tập. Vui lòng thử lại.", "Lỗi Tải Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetUI(); // Đặt lại giao diện nếu có lỗi
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút "Hiện chi tiết". Hiển thị nghĩa và phiên âm.
        /// </summary>
        private void BtnShowDetails_Click(object sender, EventArgs e)
        {
            // Hiển thị các label ẩn và các nút đánh giá.
            lblPronunciation.Visible = true;
            lblMeaning.Visible = true;
            btnRemembered.Visible = true;
            btnNotRemembered.Visible = true;

            // Ẩn nút "Hiện chi tiết".
            btnShowDetails.Visible = false;
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút "Phát âm". Gọi AudioHelper để phát âm thanh.
        /// </summary>
        private void BtnPlayAudio_Click(object sender, EventArgs e)
        {
            // Lấy URL âm thanh từ Tag của nút (đã được gán trong ShowNextWord).
            Button btn = sender as Button;
            string audioUrl = btn?.Tag as string;

            if (!string.IsNullOrEmpty(audioUrl))
            {
                try
                {
                    // Gọi hàm helper để phát âm thanh.
                    AudioHelper.PlayAudio(audioUrl);
                }
                catch (Exception ex) // Bắt lỗi từ AudioHelper hoặc lỗi khác
                {
                    Debug.WriteLine($"[ERROR] BtnPlayAudio_Click: Lỗi khi phát âm thanh từ '{audioUrl}': {ex.Message}");
                    MessageBox.Show("Không thể phát âm thanh. Đã xảy ra lỗi.", "Lỗi Phát Âm Thanh", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Thông báo nếu từ hiện tại không có URL âm thanh.
                Debug.WriteLine("[WARN] BtnPlayAudio_Click: Không có URL âm thanh cho từ này.");
                MessageBox.Show("Từ này không có file âm thanh.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút "Đã nhớ". Cập nhật trạng thái và chuyển từ tiếp theo.
        /// </summary>
        private void BtnRemembered_Click(object sender, EventArgs e)
        {
            // Lấy ID từ vựng từ Tag của nút.
            Button btn = sender as Button;
            if (btn?.Tag is int wordId && wordId > 0) // Thêm kiểm tra wordId > 0
            {
                // Cập nhật trạng thái trong CSDL là "Đã học".
                UpdateLearningStatus(wordId, "Đã học");
                // Hiển thị từ tiếp theo.
                ShowNextWord();
            }
            else
            {
                Debug.WriteLine("[WARN] BtnRemembered_Click: Không thể lấy WordId hợp lệ từ Tag của nút.");
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút "Chưa nhớ". Cập nhật trạng thái và chuyển từ tiếp theo.
        /// </summary>
        private void BtnNotRemembered_Click(object sender, EventArgs e)
        {
            // Lấy ID từ vựng từ Tag của nút.
            Button btn = sender as Button;
            if (btn?.Tag is int wordId && wordId > 0)
            {
                // Cập nhật trạng thái trong CSDL là "Đang học".
                UpdateLearningStatus(wordId, "Đang học");
                // Hiển thị từ tiếp theo.
                ShowNextWord();
            }
            else
            {
                Debug.WriteLine("[WARN] BtnNotRemembered_Click: Không thể lấy WordId hợp lệ từ Tag của nút.");
            }
        }

        #endregion

        #region Core Logic Methods

        /// <summary>
        /// Hiển thị từ vựng tiếp theo trong danh sách ôn tập lên giao diện.
        /// </summary>
        private void ShowNextWord()
        {
            currentIndex++; // Tăng chỉ số của từ hiện tại

            // Kiểm tra xem đã hết danh sách ôn tập chưa.
            if (currentWordList == null || currentIndex >= currentWordList.Count)
            {
                MessageBox.Show("🎉 Chúc mừng! Bạn đã hoàn thành phiên ôn tập này!", "Hoàn thành", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetUI(); // Đặt lại giao diện về trạng thái ban đầu
                return;
            }

            // Lấy từ vựng hiện tại.
            Vocabulary currentWord = currentWordList[currentIndex];

            // Cập nhật các label hiển thị thông tin từ.
            lblWord.Text = currentWord.Word ?? "N/A"; // Dùng ?? để tránh lỗi nếu Word là null
            lblPronunciation.Text = currentWord.Pronunciation ?? ""; // Hiển thị rỗng nếu không có
            lblMeaning.Text = currentWord.Meaning ?? "N/A"; // Hiển thị N/A nếu không có nghĩa

            // Cập nhật label tiến độ.
            lblProgress.Text = $"Tiến độ: {currentIndex + 1} / {currentWordList.Count}";

            // Lưu thông tin cần thiết (Audio URL, Word ID) vào Tag của các nút
            // để sử dụng trong các trình xử lý sự kiện (Event Handlers).
            btnPlayAudio.Tag = currentWord.AudioUrl;
            btnRemembered.Tag = currentWord.Id;
            btnNotRemembered.Tag = currentWord.Id;

            // Reset trạng thái hiển thị của flashcard (ẩn chi tiết, hiện nút Show).
            lblPronunciation.Visible = false;
            lblMeaning.Visible = false;
            btnRemembered.Visible = false;
            btnNotRemembered.Visible = false;
            btnShowDetails.Visible = true;

            // Chỉ hiển thị nút Play nếu có URL âm thanh.
            btnPlayAudio.Visible = !string.IsNullOrEmpty(currentWord.AudioUrl);
        }

        /// <summary>
        /// Gọi LearningController để cập nhật trạng thái học tập của từ vựng trong CSDL.
        /// </summary>
        /// <param name="wordId">ID của từ vựng.</param>
        /// <param name="status">Trạng thái mới ("Đã học" hoặc "Đang học").</param>
        private void UpdateLearningStatus(int wordId, string status)
        {
            try
            {
                // Gọi phương thức của LearningController.
                // Lưu ý: LearningController hiện tại nhận wordId là string.
                learningController.UpdateLearningStatus(wordId.ToString(), status);
                Debug.WriteLine($"[INFO] UpdateLearningStatus: Updated Word ID {wordId} to status '{status}'");
            }
            catch (Exception ex)
            {
                // Ghi log nếu có lỗi xảy ra trong quá trình cập nhật CSDL.
                // Thường không cần hiển thị MessageBox ở đây để tránh làm gián đoạn luồng ôn tập.
                Debug.WriteLine($"[ERROR] UpdateLearningStatus: Lỗi khi cập nhật trạng thái cho Word ID {wordId}: {ex.Message}");
            }
        }

        #endregion

        #region Data Loading Methods

        /// <summary>
        /// Tải danh sách các chủ đề từ TopicRepository và điền vào ComboBox.
        /// </summary>
        private void LoadTopics()
        {
            // Chỉ tải một lần.
            if (topicsLoadedSuccessfully) return;

            Debug.WriteLine("[INFO] LoadTopics: Bắt đầu tải danh sách chủ đề...");
            if (cboTopics == null)
            {
                Debug.WriteLine("[ERROR] LoadTopics: cboTopics is null.");
                return; // Không thể tiếp tục nếu ComboBox chưa được khởi tạo
            }

            cboTopics.Items.Clear(); // Xóa các mục cũ (nếu có)
            List<Topic> topics = null;

            try
            {
                // Lấy danh sách chủ đề từ repository.
                topics = topicRepo.GetAllTopics();

                if (topics != null)
                {
                    Debug.WriteLine($"[INFO] LoadTopics: Lấy được {topics.Count} chủ đề từ repository.");
                    // Lọc bỏ các chủ đề không hợp lệ (null hoặc tên rỗng).
                    var validTopics = topics.Where(t => t != null && !string.IsNullOrEmpty(t.Name)).ToList();

                    if (validTopics.Any())
                    {
                        // Thêm tên các chủ đề hợp lệ vào ComboBox.
                        foreach (var topic in validTopics)
                        {
                            cboTopics.Items.Add(topic.Name);
                        }
                        Debug.WriteLine($"[INFO] LoadTopics: Đã thêm {cboTopics.Items.Count} chủ đề vào ComboBox.");
                        topicsLoadedSuccessfully = true; // Đánh dấu đã tải thành công
                    }
                    else
                    {
                        Debug.WriteLine("[WARN] LoadTopics: Danh sách chủ đề từ repository rỗng hoặc không có tên hợp lệ.");
                    }
                }
                else
                {
                    // Trường hợp GetAllTopics trả về null.
                    Debug.WriteLine("[ERROR] LoadTopics: GetAllTopics() trả về null.");
                    MessageBox.Show("Không thể lấy danh sách chủ đề (kết quả rỗng).", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex) // Bắt lỗi trong quá trình tải chủ đề
            {
                Debug.WriteLine($"[ERROR] LoadTopics: Lỗi khi tải chủ đề: {ex.ToString()}");
                MessageBox.Show($"Đã xảy ra lỗi khi tải danh sách chủ đề:\n{ex.Message}", "Lỗi Tải Chủ Đề", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Đảm bảo topics là list rỗng để không gây lỗi ở dưới
                topics = new List<Topic>();
            }

            // Chọn mục đầu tiên nếu ComboBox có mục.
            if (cboTopics.Items.Count > 0)
            {
                cboTopics.SelectedIndex = 0;
                Debug.WriteLine("[INFO] LoadTopics: Đã chọn chủ đề đầu tiên trong ComboBox.");
            }
            else
            {
                Debug.WriteLine("[WARN] LoadTopics: ComboBox rỗng sau khi tải.");
                // Có thể thêm một mục mặc định như "(Không có chủ đề)" nếu muốn
                // cboTopics.Items.Add("(Không có chủ đề)");
                // cboTopics.SelectedIndex = 0;
                // cboTopics.Enabled = false; // Vô hiệu hóa nếu không có chủ đề
                btnStart.Enabled = false; // Vô hiệu hóa nút Start
            }
        }

        /// <summary>
        /// Tải danh sách từ vựng ngẫu nhiên thuộc chủ đề đã chọn để bắt đầu phiên ôn tập.
        /// </summary>
        /// <param name="topicName">Tên chủ đề được chọn.</param>
        /// <param name="count">Số lượng từ tối đa cần lấy.</param>
        /// <returns>Danh sách các đối tượng Vocabulary ngẫu nhiên, hoặc null nếu có lỗi.</returns>
        private List<Vocabulary> LoadWordsForReview(string topicName, int count)
        {
            Debug.WriteLine($"[INFO] LoadWordsForReview: Đang tải {count} từ cho chủ đề '{topicName}'...");
            var words = new List<Vocabulary>();

            try
            {
                using (var conn = DatabaseContext.GetConnection())
                {
                    conn.Open();
                    // Câu lệnh SQL lấy TOP N từ ngẫu nhiên thuộc chủ đề.
                    // Bao gồm các cột cần thiết: Id, Word, Pronunciation, Meaning, AudioUrl.
                    // ORDER BY NEWID() dùng để lấy ngẫu nhiên (hiệu năng có thể giảm với bảng rất lớn).
                    // Cân nhắc JOIN với LearningStatuses và ORDER BY Status nếu muốn ưu tiên từ chưa học/đang học.
                    string sql = $@"
                        SELECT TOP (@Count)
                               V.Id, V.Word, V.Pronunciation, V.Meaning, V.AudioUrl
                        FROM dbo.Vocabulary AS V
                        INNER JOIN dbo.VocabularyTopic AS VT ON V.Id = VT.VocabularyId
                        INNER JOIN dbo.Topics AS T ON T.Id = VT.TopicId
                        WHERE T.Name = @TopicName
                        ORDER BY NEWID()";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Count", count);
                        cmd.Parameters.AddWithValue("@TopicName", topicName ?? (object)DBNull.Value); // Xử lý topicName null

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Map dữ liệu sang Vocabulary object. Cần một hàm Map riêng hoặc gọi từ Repo.
                                // Tạm thời map trực tiếp ở đây cho đơn giản.
                                words.Add(new Vocabulary
                                {
                                    Id = reader.IsDBNull(reader.GetOrdinal("Id")) ? 0 : reader.GetInt32(reader.GetOrdinal("Id")),
                                    Word = reader.IsDBNull(reader.GetOrdinal("Word")) ? "N/A" : reader.GetString(reader.GetOrdinal("Word")),
                                    Pronunciation = reader.IsDBNull(reader.GetOrdinal("Pronunciation")) ? "" : reader.GetString(reader.GetOrdinal("Pronunciation")),
                                    Meaning = reader.IsDBNull(reader.GetOrdinal("Meaning")) ? "N/A" : reader.GetString(reader.GetOrdinal("Meaning")),
                                    AudioUrl = reader.IsDBNull(reader.GetOrdinal("AudioUrl")) ? "" : reader.GetString(reader.GetOrdinal("AudioUrl"))
                                });
                            }
                        }
                    }
                }
                Debug.WriteLine($"[INFO] LoadWordsForReview: Đã tải {words.Count} từ.");
                return words; // Trả về danh sách đã tải
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] LoadWordsForReview: Lỗi khi tải từ cho chủ đề '{topicName}': {ex.Message}");
                // Ném lại lỗi để nơi gọi (BtnStart_Click) xử lý và hiển thị thông báo.
                throw;
            }
        }

        #endregion

        #region UI Helper Methods

        /// <summary>
        /// Bật hoặc tắt các controls liên quan đến quá trình ôn tập.
        /// </summary>
        /// <param name="enable">True để bật, False để tắt.</param>
        private void EnableReviewControls(bool enable)
        {
            // Bật/tắt panel chứa flashcard và label tiến độ.
            cardPanel.Visible = enable;
            lblProgress.Visible = enable;

            // Trạng thái của các nút action sẽ được quản lý chi tiết hơn trong ShowNextWord/BtnShowDetails_Click.
            // Ở đây chỉ đảm bảo chúng bị ẩn khi không ôn tập.
            if (!enable)
            {
                btnShowDetails.Visible = false;
                btnPlayAudio.Visible = false;
                btnRemembered.Visible = false;
                btnNotRemembered.Visible = false;
            }

            // Vô hiệu hóa/Kích hoạt các control ở header.
            cboTopics.Enabled = !enable;
            numWordCount.Enabled = !enable;
            btnStart.Enabled = !enable; // Chỉ bật nút Start khi không trong quá trình ôn tập
        }

        /// <summary>
        /// Đặt lại giao diện về trạng thái ban đầu (trước khi bắt đầu ôn tập).
        /// </summary>
        private void ResetUI()
        {
            EnableReviewControls(false); // Tắt các control ôn tập

            // Reset nội dung các label.
            lblWord.Text = "...";
            lblPronunciation.Text = "";
            lblMeaning.Text = "";
            lblProgress.Text = "Tiến độ: - / -";

            // Xóa danh sách từ và reset index.
            currentWordList.Clear();
            currentIndex = -1;

            // Đảm bảo nút Start được bật lại nếu có chủ đề
            btnStart.Enabled = cboTopics.Items.Count > 0 && cboTopics.SelectedIndex >= 0;

        }

        #endregion
    }
}