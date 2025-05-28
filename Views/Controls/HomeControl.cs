using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WordVaultAppMVC.Data;     // Namespace của DatabaseContext và Repositories
using WordVaultAppMVC.Helpers;  // Namespace của DictionaryApiClient, AudioHelper
using WordVaultAppMVC.Models;   // Namespace của Vocabulary, WordDetails models

namespace WordVaultAppMVC.Views.Controls
{
    /// <summary>
    /// UserControl chính của ứng dụng, hiển thị giao diện tìm kiếm từ,
    /// kết quả tra cứu, và các nút chức năng liên quan.
    /// </summary>
    public class HomeControl : UserControl
    {
        #region UI Controls Fields

        // --- Search Area ---
        private TextBox txtSearch;
        private Button btnSearch;

        // --- Result Area ---
        private Label lblPronunciation;
        private Label lblMeaning;
        private TableLayoutPanel resultLayout; // Layout chứa pronunciation, meaning, buttonPanel
        private FlowLayoutPanel buttonPanel;   // Panel chứa các nút action (Nghe, Yêu thích, Thêm chủ đề)

        // --- Action Buttons ---
        private Button btnPlayAudio;
        private Button btnAddFavorite;
        private Button btnAddTopic;

        // --- Status & Footer ---
        private Label lblStatusMessage;          // Hiển thị thông báo trạng thái (tìm kiếm, lỗi, thành công)
        private System.Windows.Forms.Timer statusTimer; // Timer để tự động ẩn thông báo trạng thái
        private Panel pnlFooter;                 // Panel ở dưới cùng
        private Label lblVocabularyCount;        // Label hiển thị tổng số từ vựng trong DB

        // --- Layout Controls ---
        private TableLayoutPanel mainLayout; // Layout chính chia màn hình

        #endregion

        #region Logic Fields

        private Vocabulary currentVocabulary; // Lưu trữ thông tin từ đang hiển thị trên giao diện
        private readonly VocabularyRepository vocabRepo; // Repository để tương tác với dữ liệu từ vựng

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo HomeControl.
        /// </summary>
        public HomeControl()
        {
            // Khởi tạo repository. Cân nhắc DI cho ứng dụng lớn.
            vocabRepo = new VocabularyRepository();

            InitializeComponent(); // Khởi tạo các thành phần giao diện

            // Đặt trạng thái ban đầu cho các nút action (vô hiệu hóa)
            UpdateActionButtonsState(false);

            // Tải và hiển thị tổng số từ vựng
            LoadVocabularyCount();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Khởi tạo và cấu hình các thành phần giao diện người dùng (Controls).
        /// Sử dụng TableLayoutPanel và FlowLayoutPanel để bố cục linh hoạt.
        /// </summary>
        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = SystemColors.ControlLightLight; // Nền trắng

            // --- Main Layout (1 cột, 3 hàng: Search, Result, Status) ---
            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3, // Hàng 0: Search Area, Hàng 1: Result Area, Hàng 2: Status Message
                Padding = new Padding(15), // Tăng Padding
                AutoScroll = true
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // Hàng Search tự động chiều cao
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // Hàng Result chiếm phần lớn
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // Hàng Status tự động chiều cao

            #region Search Area Setup (Row 0)
            // Sử dụng TableLayoutPanel (2 cột) để chứa TextBox và Button Search
            var searchTableLayout = new TableLayoutPanel
            {
                Name = "searchTableLayout",
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 15) // Padding dưới
            };
            searchTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // Cột TextBox chiếm phần lớn
            searchTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));      // Cột Button tự động kích thước

            // *** SỬA Ở ĐÂY: Đã xóa thuộc tính PlaceholderText ***
            txtSearch = new TextBox { Name = "txtSearch", Font = new Font("Segoe UI", 11F), Margin = new Padding(3), Dock = DockStyle.Fill };
            txtSearch.KeyDown += TxtSearch_KeyDown; // Gán sự kiện nhấn phím

            btnSearch = new Button { Name = "btnSearch", Text = "🔍 Tìm kiếm", Font = new Font("Segoe UI", 10F, FontStyle.Bold), AutoSize = true, Height = txtSearch.Height, BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(8, 3, 3, 3), Anchor = AnchorStyles.Left }; // Tăng Margin trái
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Click += async (s, e) => await SearchAndDisplayWordAsync(); // Gán sự kiện click (async lambda)

            searchTableLayout.Controls.Add(txtSearch, 0, 0);
            searchTableLayout.Controls.Add(btnSearch, 1, 0);
            mainLayout.Controls.Add(searchTableLayout, 0, 0);
            #endregion

            #region Result Area Setup (Row 1)
            // Sử dụng TableLayoutPanel (2 cột) để chứa Labels và Button Panel
            resultLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2, // Cột 0: Pronunciation & Meaning, Cột 1: Action Buttons
                RowCount = 2,    // Hàng 0: Pronunciation, Hàng 1: Meaning & Buttons
                Padding = new Padding(5),
                Visible = false // Ban đầu ẩn khu vực kết quả
            };
            resultLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F)); // Cột thông tin chữ
            resultLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F)); // Cột nút bấm
            resultLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));      // Hàng Pronunciation
            resultLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Hàng Meaning & Buttons

            lblPronunciation = new Label { Name = "lblPronunciation", Text = "Phát âm:", Font = new Font("Segoe UI", 11F, FontStyle.Italic), ForeColor = Color.DarkSlateGray, AutoSize = true, Dock = DockStyle.Top, Padding = new Padding(0, 5, 0, 10) };
            resultLayout.Controls.Add(lblPronunciation, 0, 0);
            resultLayout.SetColumnSpan(lblPronunciation, 2); // Kéo dài qua 2 cột

            lblMeaning = new Label { Name = "lblMeaning", Text = "Nghĩa tiếng Việt:", Font = new Font("Segoe UI", 12F), AutoSize = true, MaximumSize = new Size(450, 0), Dock = DockStyle.Top, Padding = new Padding(0, 5, 10, 5) }; // AutoSize=true, Dock=Top
            resultLayout.Controls.Add(lblMeaning, 0, 1);

            // Panel chứa các nút action (Nghe, Yêu thích, Thêm chủ đề)
            buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(10, 0, 0, 0), AutoSize = true };
            btnPlayAudio = new Button { Name = "btnPlayAudio", Text = "🔊 Nghe", Width = 150, Height = 35, Enabled = false, Font = new Font("Segoe UI", 9F), FlatStyle = FlatStyle.System };
            btnPlayAudio.Click += BtnPlayAudio_Click;
            btnAddFavorite = new Button { Name = "btnAddFavorite", Text = "⭐ Yêu thích", Width = 150, Height = 35, Enabled = false, Font = new Font("Segoe UI", 9F), FlatStyle = FlatStyle.System };
            btnAddFavorite.Click += BtnAddFavorite_Click;
            btnAddTopic = new Button { Name = "btnAddTopic", Text = "📚 Thêm vào chủ đề", Width = 150, Height = 35, Enabled = false, Font = new Font("Segoe UI", 9F), FlatStyle = FlatStyle.System };
            btnAddTopic.Click += BtnAddTopic_Click;
            foreach (var btn in new Button[] { btnPlayAudio, btnAddFavorite, btnAddTopic }) { btn.Margin = new Padding(0, 8, 0, 8); } // Tăng Margin dọc
            buttonPanel.Controls.AddRange(new Control[] { btnPlayAudio, btnAddFavorite, btnAddTopic });
            resultLayout.Controls.Add(buttonPanel, 1, 1); // Đặt vào cột 1, hàng 1

            mainLayout.Controls.Add(resultLayout, 0, 1); // Thêm resultLayout vào hàng 1 của mainLayout
            #endregion

            #region Status Message Setup (Row 2)
            lblStatusMessage = new Label { Name = "lblStatusMessage", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.Green, Font = new Font("Segoe UI", 9F, FontStyle.Italic), Visible = false, Height = 25 };
            mainLayout.Controls.Add(lblStatusMessage, 0, 2);

            // Timer để ẩn status message
            statusTimer = new System.Windows.Forms.Timer { Interval = 4000 }; // 4 giây
            statusTimer.Tick += StatusTimer_Tick;
            #endregion

            #region Footer Setup (Ngoài mainLayout)
            pnlFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 25,
                BackColor = SystemColors.Control,
                Padding = new Padding(15, 0, 15, 0) // Tăng Padding ngang
            };
            // TODO: bị tràn label, sửa sau.
            lblVocabularyCount = new Label
            {
                Name = "lblVocabularyCount",
                Text = "Số từ vựng: ...",
                Dock = DockStyle.Right,
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.DimGray,
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = false, // Cần false để Dock Right hoạt động đúng
                // Width sẽ tự điều chỉnh theo Dock
            };
            pnlFooter.Controls.Add(lblVocabularyCount);
            #endregion

            // --- Add Main Layout & Footer to UserControl ---
            this.Controls.Add(mainLayout); // Layout chính chứa search, result, status
            this.Controls.Add(pnlFooter);  // Footer chứa số lượng từ
            mainLayout.BringToFront();     // Đảm bảo mainLayout che phủ đúng
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Xử lý sự kiện KeyDown trên TextBox tìm kiếm.
        /// Nếu nhấn Enter, thực hiện tìm kiếm.
        /// </summary>
        private async void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Ngăn tiếng 'beep' khi nhấn Enter
                await SearchAndDisplayWordAsync(); // Gọi hàm tìm kiếm
            }
        }

        /// <summary>
        /// Xử lý sự kiện Click nút "Nghe". Phát âm thanh của từ hiện tại.
        /// </summary>
        private void BtnPlayAudio_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có từ hiện tại và URL âm thanh không.
            if (currentVocabulary == null || string.IsNullOrEmpty(currentVocabulary.AudioUrl))
            {
                ShowStatusMessage("Không có âm thanh cho từ này hoặc chưa tìm từ.", true);
                return;
            }
            try
            {
                // Gọi AudioHelper để phát âm thanh.
                AudioHelper.PlayAudio(currentVocabulary.AudioUrl);
            }
            catch (Exception ex) // Bắt lỗi từ AudioHelper
            {
                Debug.WriteLine($"[ERROR] BtnPlayAudio_Click: Lỗi phát âm thanh: {ex.Message}");
                ShowStatusMessage("Lỗi khi phát âm thanh.", true);
            }
        }

        /// <summary>
        /// Xử lý sự kiện Click nút "Yêu thích". Thêm từ hiện tại vào danh sách yêu thích.
        /// </summary>
        private void BtnAddFavorite_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có từ hiện tại và ID hợp lệ không.
            if (currentVocabulary == null || currentVocabulary.Id <= 0)
            {
                ShowStatusMessage("Chưa có từ hợp lệ để thêm vào yêu thích.", true);
                return;
            }
            try
            {
                // Gọi repository để thêm vào yêu thích.
                bool success = vocabRepo.AddFavorite(currentVocabulary.Id);
                if (success)
                {
                    // success=true cũng trả về khi từ đã tồn tại trong yêu thích.
                    ShowStatusMessage("⭐ Đã thêm vào Yêu thích / Đã tồn tại!", false);
                    // Cập nhật lại trạng thái nút (sẽ thành "Đã thích" và bị vô hiệu hóa).
                    UpdateActionButtonsState(true);
                }
                else // Ít khi xảy ra nếu logic IsFavorite và AddFavorite đồng bộ
                {
                    ShowStatusMessage("Lỗi khi thêm từ vào yêu thích.", true);
                }
            }
            catch (Exception ex) // Bắt lỗi từ repository
            {
                Debug.WriteLine($"[ERROR] BtnAddFavorite_Click: Lỗi khi thêm yêu thích: {ex.ToString()}");
                ShowStatusMessage("Lỗi không xác định khi thêm yêu thích.", true);
            }
        }

        /// <summary>
        /// Xử lý sự kiện Click nút "Thêm vào chủ đề". Mở form AddToTopicForm.
        /// </summary>
        private void BtnAddTopic_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có từ hiện tại và tên từ hợp lệ không.
            if (currentVocabulary == null || string.IsNullOrWhiteSpace(currentVocabulary.Word))
            {
                ShowStatusMessage("Chưa có từ hợp lệ để thêm vào chủ đề.", true);
                return;
            }
            try
            {
                // Tạo và hiển thị form AddToTopicForm dạng dialog.
                // Truyền tên từ hiện tại vào form.
                using (var form = new AddToTopicForm(currentVocabulary.Word))
                {
                    form.ShowDialog(this.FindForm()); // Hiển thị dialog, FindForm() tìm Form cha chứa control này
                }
                // Không cần làm gì sau khi form đóng ở đây.
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] BtnAddTopic_Click: Lỗi khi mở AddToTopicForm: {ex.ToString()}");
                ShowStatusMessage("Lỗi khi mở chức năng thêm vào chủ đề.", true);
            }
        }

        /// <summary>
        /// Xử lý sự kiện Tick của Timer. Tự động ẩn thông báo trạng thái.
        /// </summary>
        private void StatusTimer_Tick(object sender, EventArgs e)
        {
            statusTimer.Stop(); // Dừng timer
            // Sử dụng Invoke nếu cần cập nhật UI từ luồng khác (mặc dù Timer thường chạy trên UI thread)
            if (lblStatusMessage.InvokeRequired)
            {
                lblStatusMessage.Invoke(new Action(() =>
                {
                    lblStatusMessage.Visible = false;
                    lblStatusMessage.Text = "";
                }));
            }
            else
            {
                lblStatusMessage.Visible = false;
                lblStatusMessage.Text = "";
            }
        }

        #endregion

        #region Core Logic Methods

        /// <summary>
        /// Thực hiện tìm kiếm từ vựng dựa trên nội dung TextBox tìm kiếm.
        /// Quy trình: Gọi API -> Dịch nghĩa -> Lưu/Lấy từ DB -> Hiển thị kết quả.
        /// </summary>
        private async Task SearchAndDisplayWordAsync()
        {
            string searchTerm = txtSearch.Text.Trim();
            // Kiểm tra xem người dùng đã nhập từ khóa chưa.
            if (string.IsNullOrEmpty(searchTerm))
            {
                ShowStatusMessage("Vui lòng nhập từ cần tìm.", true); // Thông báo lỗi
                return;
            }

            // --- Chuẩn bị UI cho quá trình tìm kiếm ---
            currentVocabulary = null; // Reset từ hiện tại
            resultLayout.Visible = false; // Ẩn khu vực kết quả cũ
            lblPronunciation.Text = "Phát âm:";
            lblMeaning.Text = "Nghĩa tiếng Việt:";
            UpdateActionButtonsState(false); // Vô hiệu hóa các nút action
            ShowStatusMessage("Đang tìm kiếm...", false); // Hiển thị trạng thái đang tìm
            this.Cursor = Cursors.WaitCursor; // Đổi con trỏ chuột thành chờ

            WordDetails apiResult = null;
            try
            {
                // --- Bước 1: Gọi API từ điển ---
                Debug.WriteLine($"[INFO] Calling GetWordDetailsAsync for '{searchTerm}'...");
                apiResult = await DictionaryApiClient.GetWordDetailsAsync(searchTerm);
                Debug.WriteLine($"[INFO] GetWordDetailsAsync result for '{searchTerm}': {(apiResult == null ? "Not Found/Error" : "Found")}");

                // --- Bước 2: Xử lý kết quả từ API ---
                if (apiResult != null)
                {
                    // --- Bước 2a: Dịch nghĩa (nếu API trả về nghĩa tiếng Anh) ---
                    if (!string.IsNullOrEmpty(apiResult.Meaning))
                    {
                        ShowStatusMessage("Đang dịch nghĩa...", false);
                        Debug.WriteLine($"[INFO] Calling TranslateToVietnamese for '{apiResult.Meaning}'...");
                        apiResult.Meaning = await DictionaryApiClient.TranslateToVietnamese(apiResult.Meaning);
                        Debug.WriteLine($"[INFO] TranslateToVietnamese result: '{apiResult.Meaning}'");
                    }

                    // --- Bước 3: Tạo đối tượng Vocabulary từ kết quả API ---
                    Vocabulary vocabFromApi = new Vocabulary
                    {
                        Word = apiResult.Word ?? searchTerm, // Ưu tiên từ trả về từ API
                        Meaning = apiResult.Meaning,
                        Pronunciation = apiResult.Pronunciation,
                        AudioUrl = apiResult.AudioUrl
                    };

                    // --- Bước 4: Lưu hoặc Lấy/Cập nhật từ trong CSDL ---
                    ShowStatusMessage("Đang kiểm tra cơ sở dữ liệu...", false);
                    currentVocabulary = SaveOrGetWordFromDatabase(vocabFromApi);

                    // --- Bước 5: Cập nhật giao diện ---
                    if (currentVocabulary != null) // Nếu thành công (từ mới hoặc từ cũ)
                    {
                        lblPronunciation.Text = "Phát âm: " + (currentVocabulary.Pronunciation ?? "N/A");
                        lblMeaning.Text = "Nghĩa tiếng Việt: " + (currentVocabulary.Meaning ?? "N/A");
                        resultLayout.Visible = true; // Hiển thị khu vực kết quả
                        ShowStatusMessage($"Tìm thấy: {currentVocabulary.Word}", false); // Thông báo thành công
                        UpdateActionButtonsState(true); // Cập nhật trạng thái các nút action (Nghe, Yêu thích...)
                    }
                    else // Nếu có lỗi khi lưu/lấy từ DB
                    {
                        ShowStatusMessage("Lỗi khi lưu hoặc cập nhật từ vào cơ sở dữ liệu.", true);
                        UpdateActionButtonsState(false);
                    }
                }
                else // Nếu API không tìm thấy từ (đã có MessageBox từ API Client)
                {
                    // ShowStatusMessage($"Không tìm thấy từ '{searchTerm}'.", true); // Có thể không cần vì API Client đã thông báo
                    UpdateActionButtonsState(false);
                }
            }
            catch (Exception ex) // Bắt lỗi chung trong quá trình tìm kiếm
            {
                Debug.WriteLine($"[ERROR] SearchAndDisplayWordAsync: Lỗi không mong muốn: {ex.ToString()}");
                ShowStatusMessage($"Đã xảy ra lỗi trong quá trình tìm kiếm: {ex.Message}", true);
                UpdateActionButtonsState(false);
            }
            finally
            {
                // Luôn trả lại con trỏ chuột bình thường sau khi tìm kiếm xong.
                this.Cursor = Cursors.Default;
            }
        }

        #endregion

        #region Data Handling Methods

        /// <summary>
        /// Lưu một từ mới vào CSDL nếu chưa tồn tại, hoặc lấy/cập nhật thông tin
        /// (như AudioUrl, Pronunciation) nếu từ đã tồn tại.
        /// </summary>
        /// <param name="vocabFromApi">Đối tượng Vocabulary chứa thông tin lấy từ API (đã dịch).</param>
        /// <returns>Đối tượng Vocabulary từ CSDL (mới hoặc đã cập nhật), hoặc null nếu có lỗi.</returns>
        private Vocabulary SaveOrGetWordFromDatabase(Vocabulary vocabFromApi)
        {
            // Kiểm tra đầu vào cơ bản.
            if (vocabFromApi == null || string.IsNullOrWhiteSpace(vocabFromApi.Word))
            {
                Debug.WriteLine("[WARN] SaveOrGetWordFromDatabase: vocabFromApi is null or Word is empty.");
                return null;
            }

            try
            {
                // 1. Kiểm tra xem từ đã tồn tại trong CSDL chưa (dựa trên tên từ).
                Debug.WriteLine($"[INFO] SaveOrGetWordFromDatabase: Checking existence for '{vocabFromApi.Word}'...");
                Vocabulary existingVocab = vocabRepo.GetVocabularyByWord(vocabFromApi.Word);

                // 2. Xử lý nếu từ ĐÃ TỒN TẠI:
                if (existingVocab != null)
                {
                    Debug.WriteLine($"[INFO] Word '{vocabFromApi.Word}' exists (ID: {existingVocab.Id}). Checking for updates...");
                    bool needsDbUpdate = false; // Cờ đánh dấu có cần gọi Update vào DB không

                    // 2a. Cập nhật AudioUrl nếu trong DB chưa có và API có.
                    if (string.IsNullOrEmpty(existingVocab.AudioUrl) && !string.IsNullOrEmpty(vocabFromApi.AudioUrl))
                    {
                        Debug.WriteLine($"[INFO] -> Updating missing Audio URL: {vocabFromApi.AudioUrl}");
                        existingVocab.AudioUrl = vocabFromApi.AudioUrl;
                        needsDbUpdate = true;
                    }

                    // 2b. Cập nhật Pronunciation nếu trong DB chưa có và API có.
                    if (string.IsNullOrEmpty(existingVocab.Pronunciation) && !string.IsNullOrEmpty(vocabFromApi.Pronunciation))
                    {
                        Debug.WriteLine($"[INFO] -> Updating missing Pronunciation: {vocabFromApi.Pronunciation}");
                        existingVocab.Pronunciation = vocabFromApi.Pronunciation;
                        needsDbUpdate = true;
                    }

                    // 2c. (Tùy chọn) Cập nhật Meaning?
                    // Thường không nên tự động ghi đè nghĩa người dùng có thể đã sửa.
                    // Chỉ cập nhật nếu nghĩa trong DB rỗng và API có.
                    if (string.IsNullOrEmpty(existingVocab.Meaning) && !string.IsNullOrEmpty(vocabFromApi.Meaning))
                    {
                        Debug.WriteLine($"[INFO] -> Updating missing Meaning: {vocabFromApi.Meaning}");
                        existingVocab.Meaning = vocabFromApi.Meaning;
                        needsDbUpdate = true;
                    }


                    // 2d. Nếu có thông tin cần cập nhật, gọi UpdateVocabulary.
                    if (needsDbUpdate)
                    {
                        Debug.WriteLine($"[INFO] Calling UpdateVocabulary for ID: {existingVocab.Id}");
                        bool updateSuccess = vocabRepo.UpdateVocabulary(existingVocab); // Dùng phiên bản nhận object
                        if (!updateSuccess)
                        {
                            Debug.WriteLine($"[WARN] UpdateVocabulary failed for ID: {existingVocab.Id}.");
                            // Có thể thông báo lỗi hoặc bỏ qua
                        }
                        else
                        {
                            Debug.WriteLine($"[INFO] Vocabulary updated successfully in DB for ID: {existingVocab.Id}.");
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"[INFO] No updates needed for existing vocabulary ID: {existingVocab.Id}.");
                    }

                    // Trả về đối tượng từ CSDL (có thể đã được cập nhật trong bộ nhớ).
                    return existingVocab;
                }
                // 3. Xử lý nếu từ CHƯA TỒN TẠI:
                else
                {
                    Debug.WriteLine($"[INFO] Word '{vocabFromApi.Word}' does not exist. Adding new vocabulary...");
                    // Gọi AddVocabulary để thêm từ mới vào CSDL.
                    Vocabulary addedVocab = vocabRepo.AddVocabulary(vocabFromApi); // Dùng phiên bản trả về object có ID

                    if (addedVocab != null)
                    {
                        Debug.WriteLine($"[INFO] Successfully added new vocabulary '{addedVocab.Word}' with ID: {addedVocab.Id}.");
                        LoadVocabularyCount(); // Cập nhật lại số lượng từ hiển thị ở footer
                        return addedVocab; // Trả về đối tượng vừa thêm
                    }
                    else
                    {
                        Debug.WriteLine($"[ERROR] Failed to add vocabulary '{vocabFromApi.Word}' to database (AddVocabulary returned null).");
                        return null; // Trả về null nếu thêm thất bại
                    }
                }
            }
            catch (Exception ex) // Bắt lỗi chung trong quá trình xử lý DB
            {
                Debug.WriteLine($"[ERROR] SaveOrGetWordFromDatabase: Lỗi khi xử lý từ '{vocabFromApi.Word}': {ex.ToString()}");
                return null; // Trả về null nếu có lỗi
            }
        }

        /// <summary>
        /// Tải tổng số từ vựng từ Repository và cập nhật Label ở Footer.
        /// Xử lý việc cập nhật UI trên đúng luồng (thread-safe).
        /// </summary>
        private void LoadVocabularyCount()
        {
            try
            {
                // Lấy số lượng từ từ Repository.
                int count = vocabRepo.GetVocabularyCount();
                string countText = $"Có sẵn: {count} từ";
                Color textColor = Color.DimGray;

                // Cập nhật UI an toàn từ các luồng khác nhau.
                if (lblVocabularyCount.InvokeRequired)
                {
                    lblVocabularyCount.Invoke(new Action(() =>
                    {
                        lblVocabularyCount.Text = countText;
                        lblVocabularyCount.ForeColor = textColor;
                    }));
                }
                else // Nếu đang ở trên UI thread
                {
                    lblVocabularyCount.Text = countText;
                    lblVocabularyCount.ForeColor = textColor;
                }
                Debug.WriteLine($"[INFO] LoadVocabularyCount: Updated count to {count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] LoadVocabularyCount: Lỗi khi tải số lượng từ: {ex.Message}");
                // Hiển thị lỗi trên UI.
                string errorText = "Lỗi tải số lượng!";
                Color errorColor = Color.Red;
                if (lblVocabularyCount.InvokeRequired)
                {
                    lblVocabularyCount.Invoke(new Action(() => {
                        lblVocabularyCount.Text = errorText;
                        lblVocabularyCount.ForeColor = errorColor;
                    }));
                }
                else
                {
                    lblVocabularyCount.Text = errorText;
                    lblVocabularyCount.ForeColor = errorColor;
                }
            }
        }

        #endregion

        #region UI Helper Methods

        /// <summary>
        /// Cập nhật trạng thái Enabled và Text/Appearance của các nút action
        /// (Nghe, Yêu thích, Thêm chủ đề) dựa trên từ vựng hiện tại.
        /// </summary>
        /// <param name="enableBasic">True để bật các nút cơ bản (Thêm chủ đề), False để tắt hết.</param>
        private void UpdateActionButtonsState(bool enableBasic)
        {
            // Đảm bảo cập nhật UI trên đúng luồng.
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateActionButtonsState(enableBasic)));
                return;
            }

            // Nếu enableBasic là true VÀ có từ hiện tại hợp lệ:
            if (enableBasic && currentVocabulary != null && currentVocabulary.Id > 0)
            {
                // Nút Nghe: Bật nếu có AudioUrl.
                btnPlayAudio.Enabled = !string.IsNullOrEmpty(currentVocabulary.AudioUrl);

                // Nút Thêm chủ đề: Luôn bật nếu có từ hợp lệ.
                btnAddTopic.Enabled = true;

                // Nút Yêu thích: Kiểm tra trạng thái yêu thích từ DB.
                try
                {
                    bool isFavorite = vocabRepo.IsFavorite(currentVocabulary.Id);
                    if (isFavorite)
                    {
                        btnAddFavorite.Text = "❤️ Đã thích"; // Thay đổi text
                        btnAddFavorite.Enabled = false;      // Vô hiệu hóa
                        btnAddFavorite.BackColor = Color.LightPink; // Đổi màu nền
                    }
                    else
                    {
                        btnAddFavorite.Text = "⭐ Yêu thích"; // Trả về text gốc
                        btnAddFavorite.Enabled = true;       // Kích hoạt
                        btnAddFavorite.BackColor = SystemColors.Control; // Trả về màu gốc
                    }
                }
                catch (Exception ex) // Nếu lỗi khi kiểm tra IsFavorite
                {
                    Debug.WriteLine($"[ERROR] UpdateActionButtonsState: Lỗi khi kiểm tra IsFavorite cho ID {currentVocabulary.Id}: {ex.Message}");
                    // Đặt về trạng thái mặc định (cho phép thêm) để tránh khóa chức năng
                    btnAddFavorite.Text = "⭐ Yêu thích";
                    btnAddFavorite.Enabled = true;
                    btnAddFavorite.BackColor = SystemColors.Control;
                }
            }
            // Ngược lại (enableBasic=false HOẶC không có từ hợp lệ):
            else
            {
                // Vô hiệu hóa tất cả các nút và reset trạng thái nút Yêu thích.
                btnPlayAudio.Enabled = false;
                btnAddFavorite.Enabled = false;
                btnAddTopic.Enabled = false;
                btnAddFavorite.Text = "⭐ Yêu thích";
                btnAddFavorite.BackColor = SystemColors.Control;
            }
        }

        /// <summary>
        /// Hiển thị một thông báo trạng thái ngắn gọn ở cuối control.
        /// Thông báo sẽ tự động biến mất sau một khoảng thời gian (được định nghĩa bởi statusTimer).
        /// </summary>
        /// <param name="message">Nội dung thông báo.</param>
        /// <param name="isError">True nếu là thông báo lỗi (màu đỏ), False nếu là thông báo thường (màu xanh).</param>
        private void ShowStatusMessage(string message, bool isError)
        {
            // Đảm bảo cập nhật UI trên đúng luồng.
            if (lblStatusMessage.InvokeRequired)
            {
                lblStatusMessage.Invoke(new Action(() => ShowStatusMessage(message, isError)));
                return;
            }

            // Đặt nội dung và màu sắc.
            lblStatusMessage.Text = message;
            lblStatusMessage.ForeColor = isError ? Color.Red : Color.DarkGreen;
            lblStatusMessage.Visible = true; // Hiển thị label

            // Reset và bắt đầu timer để tự ẩn thông báo.
            statusTimer.Stop();
            statusTimer.Start();
        }

        #endregion
    }
}