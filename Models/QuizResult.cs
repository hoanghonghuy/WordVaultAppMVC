using System;

namespace WordVaultAppMVC.Models
{
    /// <summary>
    /// Đại diện cho kết quả của một lần trả lời câu hỏi trong bài kiểm tra (Quiz).
    /// </summary>
    public class QuizResult
    {
        #region Properties

        /// <summary>
        /// ID định danh duy nhất cho bản ghi kết quả Quiz.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID của câu hỏi Quiz mà kết quả này thuộc về.
        /// Tham chiếu đến QuizQuestions.QuizId.
        /// </summary>
        public int QuizId { get; set; }

        /// <summary>
        /// Cho biết câu trả lời có đúng hay không.
        /// True nếu đáp án đúng, False nếu sai.
        /// </summary>
        public bool IsCorrect { get; set; }

        /// <summary>
        /// Ngày giờ mà bài kiểm tra được thực hiện (hoặc câu trả lời này được ghi nhận).
        /// </summary>
        public DateTime DateTaken { get; set; }

        /// <summary>
        /// ID của người dùng đã thực hiện bài kiểm tra (nếu có).
        /// Có thể là null hoặc rỗng nếu không áp dụng hệ thống người dùng.
        /// </summary>
        public string UserId { get; set; }

        #endregion

        // Constructor có thể được thêm vào nếu cần giá trị mặc định
        // public QuizResult()
        // {
        //     DateTaken = DateTime.Now;
        // }
    }
}