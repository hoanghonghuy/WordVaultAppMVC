using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WordVaultAppMVC.Data;     // Namespace chứa VocabularyRepository
using WordVaultAppMVC.Models;   // Namespace chứa Vocabulary model
using WordVaultAppMVC.Helpers;  // Namespace của AudioHelper
// using WordVaultAppMVC.Views; // Namespace của VocabularyDetailPanel (thường cùng namespace Controls)

namespace WordVaultAppMVC.Views.Controls
{
    /// <summary>
    /// UserControl hiển thị danh sách các từ vựng yêu thích
    /// và cho phép người dùng xem chi tiết, xóa khỏi danh sách hoặc nghe phát âm.
    /// </summary>
    public class FavoriteWordsControl : UserControl
    {
        #region UI Controls Fields

        private ListBox lstFavoriteWords;           // Hiển thị danh sách từ yêu thích
        private VocabularyDetailPanel vocabularyDetailPanel; // Hiển thị chi tiết từ được chọn
        private Button btnRemoveFavorite;        // Nút xóa khỏi danh sách yêu thích
        private Button btnPlayAudio;             // Nút nghe phát âm
        private TableLayoutPanel mainLayout;     // Layout chính của control
        private Label lblTitle;                  // Tiêu đề "Danh sách từ yêu thích"
        private FlowLayoutPanel buttonPanel;     // Panel chứa các nút action

        #endregion

        #region Logic Fields

        private readonly VocabularyRepository vocabRepo; // Repository để truy cập dữ liệu từ vựng/yêu thích
        private List<Vocabulary> favoriteList;      // Cache danh sách từ yêu thích hiện tại

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo một instance mới của FavoriteWordsControl.
        /// </summary>
        public FavoriteWordsControl()
        {
            vocabRepo = new VocabularyRepository(); // Khởi tạo repository
            InitializeComponent();                  // Khởi tạo giao diện

            // Gắn sự kiện Load để tải dữ liệu khi control được hiển thị lần đầu.
            // Việc này đảm bảo các control đã được tạo handle trước khi truy cập.
            this.Load += FavoriteWordsControl_Load;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Khởi tạo và cấu hình các thành phần giao diện người dùng (Controls).
        /// </summary>
        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = SystemColors.ControlLightLight; // Nền trắng

            // --- Main Layout (TableLayoutPanel 2 cột, 3 hàng) ---
            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2, // Cột 0: ListBox, Cột 1: DetailPanel
                RowCount = 3,    // Hàng 0: Title, Hàng 1: Content, Hàng 2: Buttons
                Padding = new Padding(15)
            };
            // Thiết lập tỉ lệ cột
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F)); // Cột danh sách
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F)); // Cột chi tiết
            // Thiết lập chiều cao hàng
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // Hàng Title tự động chiều cao
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // Hàng Content chiếm hết phần còn lại
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // Hàng Buttons tự động chiều cao

            // --- Tiêu đề (Hàng 0, kéo dài 2 cột) ---
            lblTitle = new Label
            {
                Text = "⭐ Danh sách từ yêu thích",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.DarkGoldenrod, // Màu nhấn
                AutoSize = true,
                Margin = new Padding(5, 5, 5, 15) // Padding dưới
            };
            mainLayout.Controls.Add(lblTitle, 0, 0);
            mainLayout.SetColumnSpan(lblTitle, 2); // Kéo dài qua 2 cột

            // --- ListBox hiển thị từ yêu thích (Cột 0, Hàng 1) ---
            lstFavoriteWords = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                IntegralHeight = false // Cho phép scroll mượt hơn
            };
            // Gán sự kiện khi người dùng chọn mục khác trong ListBox
            lstFavoriteWords.SelectedIndexChanged += LstFavoriteWords_SelectedIndexChanged;
            mainLayout.Controls.Add(lstFavoriteWords, 0, 1);

            // --- Panel hiển thị chi tiết từ (Cột 1, Hàng 1) ---
            // Tái sử dụng UserControl VocabularyDetailPanel đã tạo trước đó
            vocabularyDetailPanel = new VocabularyDetailPanel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle, // Thêm viền
                Padding = new Padding(10),
                Visible = false // Ban đầu ẩn đi, chỉ hiện khi có từ được chọn
            };
            mainLayout.Controls.Add(vocabularyDetailPanel, 1, 1);

            // --- Khu vực chứa các nút chức năng (Hàng 2, kéo dài 2 cột) ---
            btnRemoveFavorite = new Button
            {
                Text = "🗑️ Xóa khỏi Yêu thích",
                AutoSize = true, // Tự động kích thước theo nội dung
                Height = 30, // Đặt chiều cao cố định nếu muốn
                BackColor = Color.MistyRose,
                ForeColor = Color.DarkRed,
                Font = new Font("Segoe UI", 9F),
                Enabled = false, // Ban đầu bị vô hiệu hóa
                FlatStyle = FlatStyle.Flat // Giao diện phẳng
            };
            btnRemoveFavorite.FlatAppearance.BorderColor = Color.IndianRed;
            btnRemoveFavorite.FlatAppearance.BorderSize = 1;
            btnRemoveFavorite.Click += BtnRemoveFavorite_Click; // Gán sự kiện click

            btnPlayAudio = new Button
            {
                Text = "🔊 Nghe",
                AutoSize = true,
                Height = 30, // Đặt chiều cao cố định nếu muốn
                BackColor = Color.SkyBlue,
                Font = new Font("Segoe UI", 9F),
                Enabled = false, // Ban đầu bị vô hiệu hóa
                FlatStyle = FlatStyle.Flat
            };
            btnPlayAudio.FlatAppearance.BorderColor = Color.SteelBlue;
            btnPlayAudio.FlatAppearance.BorderSize = 1;
            btnPlayAudio.Click += BtnPlayAudio_Click; // Gán sự kiện click

            // Dùng FlowLayoutPanel để sắp xếp các nút
            buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight, // Các nút xếp từ trái sang phải
                WrapContents = false,                      // Không xuống dòng
                AutoSize = true,                           // Chiều cao tự động
                Padding = new Padding(0, 10, 0, 0)       // Padding phía trên
            };
            // Thêm các nút vào panel
            buttonPanel.Controls.Add(btnRemoveFavorite);
            buttonPanel.Controls.Add(btnPlayAudio);
            // Đặt khoảng cách giữa các nút
            foreach (Control btn in buttonPanel.Controls) { btn.Margin = new Padding(5); }

            mainLayout.Controls.Add(buttonPanel, 0, 2);
            mainLayout.SetColumnSpan(buttonPanel, 2); // Panel nút kéo dài 2 cột

            // --- Thêm Layout chính vào UserControl ---
            this.Controls.Add(mainLayout);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Xử lý sự kiện Load của UserControl. Được gọi khi control được hiển thị lần đầu.
        /// Thực hiện tải danh sách từ yêu thích.
        /// </summary>
        private void FavoriteWordsControl_Load(object sender, EventArgs e)
        {
            // Chỉ tải dữ liệu lần đầu tiên control được load.
            // Nếu muốn có nút Refresh, sẽ gọi LoadFavoriteWords() từ sự kiện click của nút đó.
            if (favoriteList == null)
            {
                LoadFavoriteWords();
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi người dùng chọn một mục trong danh sách từ yêu thích (ListBox).
        /// Hiển thị chi tiết của từ được chọn và cập nhật trạng thái các nút chức năng.
        /// </summary>
        private void LstFavoriteWords_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Lấy đối tượng Vocabulary được chọn từ ListBox.
            // Cần ép kiểu vì ListBox.SelectedItem trả về object.
            Vocabulary selectedVocab = lstFavoriteWords.SelectedItem as Vocabulary;

            // Nếu có một từ hợp lệ được chọn:
            if (selectedVocab != null)
            {
                // Hiển thị thông tin chi tiết của từ lên VocabularyDetailPanel.
                vocabularyDetailPanel.DisplayVocabulary(selectedVocab);
                vocabularyDetailPanel.Visible = true; // Cho panel hiển thị

                // Kích hoạt nút "Xóa khỏi Yêu thích".
                btnRemoveFavorite.Enabled = true;

                // Kiểm tra xem từ có URL âm thanh không.
                bool hasAudio = !string.IsNullOrEmpty(selectedVocab.AudioUrl);
                // Kích hoạt nút "Nghe" nếu có URL.
                btnPlayAudio.Enabled = hasAudio;
                // Lưu URL âm thanh vào Tag của nút để sử dụng khi nhấn nút.
                btnPlayAudio.Tag = hasAudio ? selectedVocab.AudioUrl : null;
            }
            else // Nếu không có từ nào được chọn (ví dụ: chọn dòng thông báo rỗng/lỗi)
            {
                // Ẩn panel chi tiết và vô hiệu hóa các nút.
                vocabularyDetailPanel.Visible = false;
                btnRemoveFavorite.Enabled = false;
                btnPlayAudio.Enabled = false;
                btnPlayAudio.Tag = null; // Xóa tag
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút "Xóa khỏi Yêu thích".
        /// Xác nhận với người dùng và gọi Repository để xóa từ khỏi danh sách yêu thích.
        /// </summary>
        private void BtnRemoveFavorite_Click(object sender, EventArgs e)
        {
            // Lấy từ đang được chọn.
            Vocabulary selectedVocab = lstFavoriteWords.SelectedItem as Vocabulary;
            if (selectedVocab == null)
            {
                Debug.WriteLine("[WARN] BtnRemoveFavorite_Click: selectedVocab is null.");
                return; // Không có gì để xóa
            }

            // Xác nhận lại với người dùng.
            DialogResult confirmResult = MessageBox.Show(
                $"Bạn có chắc muốn xóa từ '{selectedVocab.Word}' khỏi danh sách yêu thích?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    // Gọi phương thức RemoveFavorite của VocabularyRepository.
                    bool success = vocabRepo.RemoveFavorite(selectedVocab.Id);

                    if (success)
                    {
                        Debug.WriteLine($"[INFO] Successfully removed favorite for Word ID: {selectedVocab.Id}");
                        MessageBox.Show("Đã xóa khỏi danh sách yêu thích.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Tải lại danh sách để cập nhật giao diện.
                        LoadFavoriteWords();
                    }
                    else
                    {
                        // Có thể xảy ra nếu từ không tồn tại trong bảng FavoriteWords (ít khả năng nếu giao diện đồng bộ)
                        Debug.WriteLine($"[WARN] RemoveFavorite returned false for Word ID: {selectedVocab.Id}. Word might not have been a favorite.");
                        MessageBox.Show("Không thể xóa từ yêu thích. Có thể từ này không còn trong danh sách.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex) // Bắt lỗi từ repository hoặc lỗi khác
                {
                    Debug.WriteLine($"[ERROR] BtnRemoveFavorite_Click: Lỗi khi xóa favorite (ID: {selectedVocab.Id}): {ex.Message}");
                    MessageBox.Show("Đã xảy ra lỗi trong quá trình xóa từ yêu thích.", "Lỗi Hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện khi nhấn nút "Nghe". Phát âm thanh của từ đang được chọn.
        /// </summary>
        private void BtnPlayAudio_Click(object sender, EventArgs e)
        {
            // Lấy URL âm thanh từ Tag của nút (đã được gán trong SelectedIndexChanged).
            string audioUrl = btnPlayAudio.Tag as string;

            if (!string.IsNullOrEmpty(audioUrl))
            {
                try
                {
                    // Gọi AudioHelper để phát âm thanh.
                    AudioHelper.PlayAudio(audioUrl);
                }
                catch (Exception ex) // Bắt lỗi từ AudioHelper
                {
                    Debug.WriteLine($"[ERROR] BtnPlayAudio_Click: Lỗi khi phát âm thanh từ '{audioUrl}': {ex.Message}");
                    MessageBox.Show("Không thể phát âm thanh. Đã xảy ra lỗi.", "Lỗi Phát Âm Thanh", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Trường hợp này không nên xảy ra nếu logic Enabled của nút đúng.
                Debug.WriteLine("[WARN] BtnPlayAudio_Click: Nút được nhấn nhưng không có AudioUrl trong Tag.");
                MessageBox.Show("Từ này không có file âm thanh.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region Data Loading / UI Update Methods

        /// <summary>
        /// Tải danh sách các từ vựng yêu thích từ VocabularyRepository và cập nhật ListBox.
        /// </summary>
        private void LoadFavoriteWords()
        {
            Debug.WriteLine("[INFO] LoadFavoriteWords: Bắt đầu tải danh sách từ yêu thích...");
            try
            {
                // Gọi repository để lấy danh sách Vocabulary đã được đánh dấu yêu thích.
                favoriteList = vocabRepo.GetFavoriteVocabularies();

                // Xóa các mục hiện có trong ListBox trước khi thêm mới.
                lstFavoriteWords.Items.Clear();
                // Reset trạng thái các control khác.
                vocabularyDetailPanel.Visible = false;
                btnRemoveFavorite.Enabled = false;
                btnPlayAudio.Enabled = false;
                btnPlayAudio.Tag = null;
                lstFavoriteWords.Enabled = true; // Bật lại listbox phòng trường hợp trước đó bị disable

                // Kiểm tra xem có từ yêu thích nào không.
                if (favoriteList != null && favoriteList.Any()) // Dùng Any() hiệu quả hơn Count > 0
                {
                    // Cấu hình ListBox để hiển thị tên từ và lưu trữ toàn bộ đối tượng Vocabulary.
                    lstFavoriteWords.DisplayMember = nameof(Vocabulary.Word); // Hiển thị thuộc tính Word
                    lstFavoriteWords.ValueMember = nameof(Vocabulary.Id);    // Có thể dùng Id làm ValueMember (ít dùng ở đây)

                    // Thêm từng đối tượng Vocabulary vào ListBox.
                    foreach (var vocab in favoriteList)
                    {
                        lstFavoriteWords.Items.Add(vocab);
                    }
                    Debug.WriteLine($"[INFO] LoadFavoriteWords: Đã tải {favoriteList.Count} từ yêu thích.");

                    // Tùy chọn: Tự động chọn từ đầu tiên
                    if (lstFavoriteWords.Items.Count > 0)
                    {
                        lstFavoriteWords.SelectedIndex = 0;
                    }
                }
                else // Nếu không có từ yêu thích nào
                {
                    // Hiển thị thông báo trong ListBox và vô hiệu hóa nó.
                    lstFavoriteWords.Items.Add("(Chưa có từ yêu thích nào)");
                    lstFavoriteWords.DisplayMember = ""; // Reset display member
                    lstFavoriteWords.ValueMember = "";   // Reset value member
                    lstFavoriteWords.Enabled = false;
                    Debug.WriteLine("[INFO] LoadFavoriteWords: Không tìm thấy từ yêu thích nào.");
                }
            }
            catch (Exception ex) // Bắt lỗi khi tải dữ liệu
            {
                Debug.WriteLine($"[ERROR] LoadFavoriteWords: Lỗi khi tải danh sách yêu thích: {ex.Message}");
                MessageBox.Show("Đã xảy ra lỗi khi tải danh sách từ yêu thích. Vui lòng thử lại.", "Lỗi Tải Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Hiển thị lỗi trong ListBox
                lstFavoriteWords.Items.Clear();
                lstFavoriteWords.Items.Add("(Lỗi tải dữ liệu)");
                lstFavoriteWords.Enabled = false;
            }
        }

        #endregion
    }
}