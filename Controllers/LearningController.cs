using System;
using System.Linq;
using System.Windows.Forms;
using WordVaultAppMVC.Models;
using WordVaultAppMVC.Data;
using System.Collections.Generic;

namespace WordVaultAppMVC.Controllers
{
    /// <summary>
    /// Chịu trách nhiệm xử lý logic liên quan đến việc học và phân loại từ vựng.
    /// </summary>
    public class LearningController
    {
        #region Fields

        private readonly VocabularyRepository _vocabularyRepository;
        private readonly LearningStatusRepository _learningStatusRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo một instance mới của LearningController với các repository cần thiết.
        /// </summary>
        public LearningController()
        {
            // Khởi tạo các repository để tương tác với tầng dữ liệu.
            _vocabularyRepository = new VocabularyRepository();
            _learningStatusRepository = new LearningStatusRepository();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Phân loại từ vựng là "Đã học" hoặc "Đang học" dựa trên việc người dùng nhập đúng nghĩa hay không.
        /// </summary>
        /// <param name="wordId">ID của từ vựng (dưới dạng chuỗi).</param>
        /// <param name="userMeaning">Nghĩa của từ do người dùng nhập.</param>
        public void ClassifyVocabulary(string wordId, string userMeaning)
        {
            // Kiểm tra và chuyển đổi wordId sang kiểu int.
            // Giả định wordId có thể đến từ nguồn không đảm bảo là số (ví dụ: Tag của control).
            if (!int.TryParse(wordId, out int id))
            {
                MessageBox.Show("ID từ không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Lấy thông tin từ vựng từ repository.
            var word = _vocabularyRepository.GetWordById(id); // Sử dụng phương thức mới GetVocabularyById nếu đã đổi tên
            if (word == null)
            {
                MessageBox.Show("Từ vựng không tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string correctMeaning = word.Meaning;

            // Kiểm tra xem người dùng có nhập nghĩa không.
            if (string.IsNullOrEmpty(userMeaning))
            {
                MessageBox.Show("Vui lòng nhập nghĩa của từ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // So sánh nghĩa người dùng nhập với nghĩa đúng (không phân biệt hoa thường, bỏ khoảng trắng thừa).
            if (userMeaning.Trim().Equals(correctMeaning?.Trim(), StringComparison.OrdinalIgnoreCase)) // Thêm Trim() cho correctMeaning để chắc chắn
            {
                // Nếu đúng, cập nhật trạng thái là "Đã học".
                UpdateLearningStatus(wordId, "Đã học"); // wordId vẫn là string ở đây để nhất quán với DB/Model hiện tại
                MessageBox.Show("Chính xác! Từ vựng đã được phân loại là đã học.", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Nếu sai, cập nhật trạng thái là "Đang học".
                UpdateLearningStatus(wordId, "Đang học");
                MessageBox.Show($"Sai rồi! Nghĩa đúng là: {correctMeaning}", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Cập nhật hoặc thêm mới trạng thái học tập của một từ vựng.
        /// </summary>
        /// <param name="wordId">ID của từ vựng (dưới dạng chuỗi).</param>
        /// <param name="status">Trạng thái mới (ví dụ: "Đã học", "Đang học").</param>
        public void UpdateLearningStatus(string wordId, string status)
        {
            // Tìm trạng thái hiện có của từ.
            var existingStatus = _learningStatusRepository.GetLearningStatusByWordId(wordId);

            if (existingStatus != null)
            {
                // Nếu đã tồn tại, cập nhật trạng thái.
                existingStatus.Status = status;
                // Cân nhắc cập nhật cả DateLearned nếu cần: existingStatus.DateLearned = DateTime.Now;
                _learningStatusRepository.UpdateLearningStatus(existingStatus);
            }
            else
            {
                // Nếu chưa tồn tại, thêm mới trạng thái.
                var newStatus = new LearningStatus
                {
                    WordId = wordId, // Lưu ID dưới dạng string theo thiết kế hiện tại của Model/DB
                    Status = status,
                    DateLearned = DateTime.Now,
                    UserId = null // Hoặc gán UserId nếu có logic quản lý người dùng
                };
                _learningStatusRepository.AddLearningStatus(newStatus);
            }
        }

        /// <summary>
        /// Lấy và hiển thị danh sách các từ vựng đã được đánh dấu là "Đã học".
        /// </summary>
        public void GetLearnedVocabulary()
        {
            // Lấy danh sách từ đã học.
            var learnedWords = GetVocabularyByStatus("Đã học");

            // Hiển thị kết quả.
            if (learnedWords.Any()) // Sử dụng Any() để kiểm tra danh sách có phần tử không
            {
                string learnedWordsList = string.Join(Environment.NewLine, learnedWords.Select(w => w.Word));
                MessageBox.Show($"Các từ vựng đã học:\n{learnedWordsList}", "Danh sách từ vựng đã học", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Chưa có từ vựng nào được học!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Lấy và hiển thị danh sách các từ vựng đã được đánh dấu là "Đang học".
        /// </summary>
        public void GetLearningVocabulary()
        {
            // Lấy danh sách từ đang học.
            var learningWords = GetVocabularyByStatus("Đang học");

            // Hiển thị kết quả.
            if (learningWords.Any())
            {
                string learningWordsList = string.Join(Environment.NewLine, learningWords.Select(w => w.Word));
                MessageBox.Show($"Các từ vựng đang học:\n{learningWordsList}", "Danh sách từ vựng đang học", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Không có từ vựng nào đang học!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Lấy và hiển thị danh sách các từ vựng được đánh dấu là "Chưa học".
        /// </summary>
        /// <remarks>
        /// Lưu ý: Cần đảm bảo rằng trạng thái "Chưa học" được quản lý và lưu trong DB.
        /// Nếu không, phương thức này có thể luôn trả về danh sách rỗng.
        /// </remarks>
        public void GetUnlearnedVocabulary()
        {
            // Lấy danh sách từ chưa học.
            var unlearnedWords = GetVocabularyByStatus("Chưa học");

            // Hiển thị kết quả.
            if (unlearnedWords.Any())
            {
                string unlearnedWordsList = string.Join(Environment.NewLine, unlearnedWords.Select(w => w.Word));
                MessageBox.Show($"Các từ vựng chưa học:\n{unlearnedWordsList}", "Danh sách từ vựng chưa học", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Tất cả từ vựng đã được học hoặc không có từ nào ở trạng thái 'Chưa học'!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Phương thức hỗ trợ lấy danh sách các đối tượng Vocabulary dựa trên trạng thái học tập.
        /// </summary>
        /// <param name="status">Trạng thái cần lọc (ví dụ: "Đã học", "Đang học").</param>
        /// <returns>Danh sách các từ vựng khớp với trạng thái.</returns>
        private List<Vocabulary> GetVocabularyByStatus(string status)
        {
            try
            {
                return _learningStatusRepository.GetAllLearningStatus()
                    .Where(ls => ls.Status == status)
                    .Select(ls =>
                    {
                        // Thử chuyển đổi WordId (string) sang int để lấy thông tin Vocabulary.
                        if (int.TryParse(ls.WordId, out int id))
                        {
                            // Gọi repository để lấy chi tiết từ vựng bằng ID số nguyên.
                            // Đảm bảo GetWordById hoặc GetVocabularyById tồn tại và trả về Vocabulary.
                            return _vocabularyRepository.GetWordById(id); // Hoặc _vocabularyRepository.GetVocabularyById(id)
                        }
                        return null; // Trả về null nếu WordId không hợp lệ.
                    })
                    .Where(w => w != null) // Loại bỏ các kết quả null (do WordId không hợp lệ).
                    .ToList(); // Chuyển kết quả thành danh sách.
            }
            catch (Exception ex)
            {
                // Ghi log hoặc hiển thị lỗi nếu có vấn đề khi truy vấn dữ liệu.
                Console.WriteLine($"Error fetching vocabulary by status '{status}': {ex.Message}");
                MessageBox.Show($"Đã xảy ra lỗi khi lấy danh sách từ '{status}'.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Vocabulary>(); // Trả về danh sách rỗng nếu có lỗi.
            }
        }

        #endregion
    }
}