// File: EditVocabularyForm.cs
using System;
using System.Drawing;
using System.Windows.Forms;
using WordVaultAppMVC.Data;
using WordVaultAppMVC.Models;

namespace WordVaultAppMVC.Views
{
    // Đảm bảo class là partial để liên kết với Designer
    public partial class EditVocabularyForm : Form
    {
        private readonly int _vocabularyId;
        private readonly VocabularyRepository _vocabRepo;
        private Vocabulary _currentVocab; // Lưu trữ object từ đang sửa

        // Constructor nhận ID của từ cần sửa
        public EditVocabularyForm(int vocabularyId)
        {
            _vocabularyId = vocabularyId;
            _vocabRepo = new VocabularyRepository();
            InitializeComponent(); // Gọi hàm khởi tạo giao diện từ Designer.cs
            LoadVocabularyData();  // Tải dữ liệu từ vựng lên Form
        }

        // Tải dữ liệu từ vựng dựa vào ID
        private void LoadVocabularyData()
        {
            try
            {
                _currentVocab = _vocabRepo.GetVocabularyById(_vocabularyId);
                if (_currentVocab != null)
                {
                    // Hiển thị dữ liệu lên các TextBox
                    txtWord.Text = _currentVocab.Word;
                    txtMeaning.Text = _currentVocab.Meaning;
                    txtPronunciation.Text = _currentVocab.Pronunciation;
                    txtAudioUrl.Text = _currentVocab.AudioUrl;

                    // Có thể đặt tiêu đề Form
                    this.Text = $"Chỉnh sửa Từ: {_currentVocab.Word}";
                }
                else
                {
                    MessageBox.Show("Không tìm thấy thông tin từ vựng với ID được cung cấp.", "Lỗi dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Abort; // Đặt kết quả để báo lỗi
                    this.Close(); // Đóng form nếu không load được data
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu từ vựng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
        }

        // Xử lý sự kiện khi nhấn nút Lưu
        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Lấy dữ liệu mới từ các TextBox
            string word = txtWord.Text.Trim();
            string meaning = txtMeaning.Text.Trim();
            string pronunciation = txtPronunciation.Text.Trim();
            string audioUrl = txtAudioUrl.Text.Trim();

            // Kiểm tra dữ liệu nhập cơ bản
            if (string.IsNullOrEmpty(word) || string.IsNullOrEmpty(meaning))
            {
                MessageBox.Show("Từ vựng và Nghĩa không được để trống.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtWord.Focus(); // Focus vào ô bị thiếu
                return;
            }

            // Xác nhận lại lần nữa trước khi lưu
            var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn lưu những thay đổi này?",
                                                "Xác nhận lưu",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    // Tạo đối tượng Vocabulary với dữ liệu đã cập nhật
                    // Đảm bảo giữ lại đúng Id
                    Vocabulary updatedVocab = new Vocabulary
                    {
                        Id = _vocabularyId, // Giữ nguyên ID gốc
                        Word = word,
                        Meaning = meaning,
                        Pronunciation = pronunciation,
                        AudioUrl = audioUrl
                    };

                    // Gọi Repository để thực hiện cập nhật vào DB
                    // Giả sử hàm UpdateVocabulary nhận vào một object Vocabulary
                    bool success = _vocabRepo.UpdateVocabulary(updatedVocab);

                    if (success)
                    {
                        MessageBox.Show("Cập nhật từ vựng thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK; // Đặt kết quả là OK
                        this.Close(); // Đóng Form
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật từ vựng thất bại. Vui lòng kiểm tra lại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        // Không đóng form để người dùng sửa lại
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi khi lưu từ vựng: {ex.Message}", "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // --- InitializeComponent sẽ được tạo trong file .Designer.cs ---
        // private void InitializeComponent() { ... }

    } // Kết thúc class EditVocabularyForm
} // Kết thúc namespace