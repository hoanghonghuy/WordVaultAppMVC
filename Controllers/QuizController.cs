using System;
using System.Collections.Generic;
using WordVaultAppMVC.Data;
using WordVaultAppMVC.Models;

namespace WordVaultAppMVC.Controllers
{
    /// <summary>
    /// Chịu trách nhiệm xử lý logic liên quan đến chức năng kiểm tra (Quiz).
    /// </summary>
    public class QuizController
    {
        #region Fields

        private readonly QuizRepository _quizRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo một instance mới của QuizController.
        /// </summary>
        public QuizController()
        {
            _quizRepository = new QuizRepository();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Lấy danh sách các câu hỏi Quiz từ cơ sở dữ liệu.
        /// </summary>
        /// <returns>Danh sách các đối tượng QuizQuestion.</returns>
        /// <remarks>
        /// Giả định rằng QuizRepository có phương thức GetAllQuizQuestions() trả về danh sách QuizQuestion.
        /// </remarks>
        public List<QuizQuestion> GetQuizQuestions()
        {
            // Gọi repository để lấy tất cả câu hỏi quiz.
            return _quizRepository.GetAllQuizQuestions();
        }

        /// <summary>
        /// Xử lý việc người dùng nộp đáp án cho một câu hỏi Quiz cụ thể.
        /// Ghi nhận kết quả và trả về việc đáp án có đúng hay không.
        /// </summary>
        /// <param name="quizId">ID của câu hỏi Quiz.</param>
        /// <param name="selectedOption">Số thứ tự của đáp án người dùng chọn (ví dụ: 1, 2, 3, 4).</param>
        /// <returns>True nếu đáp án đúng, False nếu sai.</returns>
        /// <exception cref="Exception">Ném ra nếu không tìm thấy câu hỏi Quiz với quizId đã cho.</exception>
        public bool SubmitQuizAnswer(int quizId, int selectedOption)
        {
            // Lấy thông tin câu hỏi quiz từ repository.
            QuizQuestion quizQuestion = _quizRepository.GetQuizQuestionById(quizId);
            if (quizQuestion == null)
            {
                // Xử lý trường hợp không tìm thấy câu hỏi.
                throw new Exception($"Câu hỏi quiz với ID {quizId} không tồn tại!");
            }

            // Xác định xem đáp án người dùng chọn có khớp với đáp án đúng không.
            bool isCorrect = (quizQuestion.CorrectOption == selectedOption);

            // Tạo đối tượng lưu kết quả cho lần làm quiz này.
            QuizResult result = new QuizResult
            {
                QuizId = quizId,
                IsCorrect = isCorrect,
                DateTaken = DateTime.Now,
                UserId = null // Gán UserId nếu có hệ thống người dùng
            };

            // Lưu kết quả vào cơ sở dữ liệu thông qua repository.
            _quizRepository.AddQuizResult(result);

            // Trả về kết quả kiểm tra đáp án.
            return isCorrect;
        }

        #endregion
    }

    #region Supporting Model (QuizQuestion)

    // Lưu ý: Lớp này có thể nên được chuyển vào thư mục /Models
    // Tuy nhiên, giữ lại ở đây theo cấu trúc file gốc bạn cung cấp.

    /// <summary>
    /// Đại diện cho một câu hỏi trong bài kiểm tra (Quiz).
    /// Bao gồm nội dung câu hỏi, các lựa chọn và đáp án đúng.
    /// </summary>
    public class QuizQuestion
    {
        /// <summary>
        /// ID định danh duy nhất cho câu hỏi Quiz.
        /// </summary>
        public int QuizId { get; set; }

        /// <summary>
        /// Nội dung của câu hỏi (thường là từ vựng cần tìm nghĩa).
        /// </summary>
        public string QuestionText { get; set; }

        /// <summary>
        /// Danh sách các lựa chọn đáp án (thường là các nghĩa).
        /// </summary>
        public List<string> Options { get; set; } = new List<string>(); // Khởi tạo để tránh null

        /// <summary>
        /// Số thứ tự (index + 1 hoặc giá trị định danh) của đáp án đúng trong danh sách Options.
        /// </summary>
        public int CorrectOption { get; set; }
    }

    #endregion
}