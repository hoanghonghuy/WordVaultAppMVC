using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics; // Sử dụng Debug thay vì Console
using WordVaultAppMVC.Controllers; // Namespace chứa QuizQuestion
using WordVaultAppMVC.Models;   // Namespace chứa QuizResult

namespace WordVaultAppMVC.Data
{
    /// <summary>
    /// Repository chịu trách nhiệm truy cập và quản lý dữ liệu liên quan đến Quiz
    /// (câu hỏi và kết quả) trong cơ sở dữ liệu.
    /// </summary>
    public class QuizRepository
    {
        #region Public Methods

        /// <summary>
        /// Lấy danh sách tất cả các câu hỏi Quiz từ bảng QuizQuestions.
        /// </summary>
        /// <returns>Danh sách các đối tượng QuizQuestion.</returns>
        public List<QuizQuestion> GetAllQuizQuestions()
        {
            List<QuizQuestion> questions = new List<QuizQuestion>();
            // Câu lệnh SQL để lấy tất cả câu hỏi.
            // Cần đảm bảo tên cột (Option1, Option2...) khớp với CSDL.
            string query = "SELECT QuizId, QuestionText, Option1, Option2, Option3, Option4, CorrectOption FROM dbo.QuizQuestions";

            try
            {
                using (SqlConnection conn = DatabaseContext.GetConnection())
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Map dữ liệu từ reader sang đối tượng QuizQuestion.
                            QuizQuestion question = MapReaderToQuizQuestion(reader);
                            if (question != null) // Thêm kiểm tra null phòng trường hợp map lỗi
                            {
                                questions.Add(question);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi lấy tất cả QuizQuestions: {ex.Message}");
                // Có thể ném lại lỗi hoặc trả về danh sách rỗng tùy yêu cầu.
            }
            return questions;
        }

        /// <summary>
        /// Lấy thông tin một câu hỏi Quiz cụ thể dựa vào ID.
        /// </summary>
        /// <param name="quizId">ID của câu hỏi Quiz cần lấy.</param>
        /// <returns>Đối tượng QuizQuestion nếu tìm thấy, ngược lại trả về null.</returns>
        public QuizQuestion GetQuizQuestionById(int quizId)
        {
            QuizQuestion question = null;
            // Câu lệnh SQL để lấy câu hỏi theo ID.
            string query = "SELECT QuizId, QuestionText, Option1, Option2, Option3, Option4, CorrectOption FROM dbo.QuizQuestions WHERE QuizId = @QuizId";

            try
            {
                using (SqlConnection conn = DatabaseContext.GetConnection())
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Thêm tham số QuizId.
                    cmd.Parameters.Add("@QuizId", SqlDbType.Int).Value = quizId;
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Map dữ liệu sang đối tượng QuizQuestion.
                            question = MapReaderToQuizQuestion(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi lấy QuizQuestion theo ID={quizId}: {ex.Message}");
                // Trả về null nếu có lỗi.
            }
            return question;
        }

        /// <summary>
        /// Thêm một bản ghi kết quả làm Quiz vào bảng QuizResults.
        /// </summary>
        /// <param name="result">Đối tượng QuizResult chứa thông tin kết quả cần thêm.</param>
        public void AddQuizResult(QuizResult result)
        {
            if (result == null)
            {
                Debug.WriteLine("[ERROR] AddQuizResult: Đối tượng QuizResult không được null.");
                return; // Hoặc ném ArgumentNullException
            }

            // Câu lệnh SQL INSERT kết quả.
            string query = "INSERT INTO dbo.QuizResults (QuizId, IsCorrect, DateTaken, UserId) VALUES (@QuizId, @IsCorrect, @DateTaken, @UserId)";

            try
            {
                using (SqlConnection conn = DatabaseContext.GetConnection())
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Thêm các tham số từ đối tượng result.
                    cmd.Parameters.Add("@QuizId", SqlDbType.Int).Value = result.QuizId;
                    cmd.Parameters.Add("@IsCorrect", SqlDbType.Bit).Value = result.IsCorrect;
                    cmd.Parameters.Add("@DateTaken", SqlDbType.DateTime).Value = (result.DateTaken > DateTime.MinValue) ? result.DateTaken : DateTime.Now; // Đảm bảo ngày hợp lệ
                    // Xử lý UserId có thể null hoặc rỗng.
                    cmd.Parameters.Add("@UserId", SqlDbType.NVarChar, 50).Value = string.IsNullOrEmpty(result.UserId) ? (object)DBNull.Value : result.UserId;

                    conn.Open();
                    cmd.ExecuteNonQuery(); // Thực thi lệnh INSERT.
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi thêm QuizResult cho QuizId={result.QuizId}: {ex.Message}");
                // Cân nhắc ném lại lỗi.
            }
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Ánh xạ dữ liệu từ một hàng SqlDataReader sang đối tượng QuizQuestion.
        /// </summary>
        /// <param name="reader">Đối tượng SqlDataReader đang đọc dữ liệu.</param>
        /// <returns>Đối tượng QuizQuestion đã được điền dữ liệu, hoặc null nếu có lỗi.</returns>
        private QuizQuestion MapReaderToQuizQuestion(SqlDataReader reader)
        {
            try
            {
                // Lấy index các cột để tối ưu và dễ quản lý hơn
                int idCol = reader.GetOrdinal("QuizId");
                int textCol = reader.GetOrdinal("QuestionText");
                int opt1Col = reader.GetOrdinal("Option1");
                int opt2Col = reader.GetOrdinal("Option2");
                int opt3Col = reader.GetOrdinal("Option3");
                int opt4Col = reader.GetOrdinal("Option4");
                int correctOptCol = reader.GetOrdinal("CorrectOption");

                // Kiểm tra null cho các cột Option trước khi ToString()
                string opt1 = reader.IsDBNull(opt1Col) ? string.Empty : reader.GetString(opt1Col);
                string opt2 = reader.IsDBNull(opt2Col) ? string.Empty : reader.GetString(opt2Col);
                string opt3 = reader.IsDBNull(opt3Col) ? string.Empty : reader.GetString(opt3Col);
                string opt4 = reader.IsDBNull(opt4Col) ? string.Empty : reader.GetString(opt4Col);

                return new QuizQuestion
                {
                    QuizId = reader.IsDBNull(idCol) ? 0 : reader.GetInt32(idCol),
                    QuestionText = reader.IsDBNull(textCol) ? string.Empty : reader.GetString(textCol),
                    Options = new List<string> { opt1, opt2, opt3, opt4 },
                    // Sử dụng Convert.ToInt32 an toàn hơn nếu cột CorrectOption có thể là DBNull
                    CorrectOption = reader.IsDBNull(correctOptCol) ? 0 : Convert.ToInt32(reader.GetValue(correctOptCol))
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi map SqlDataReader sang QuizQuestion: {ex.Message}");
                return null; // Trả về null nếu có lỗi mapping
            }
        }

        #endregion
    }
}