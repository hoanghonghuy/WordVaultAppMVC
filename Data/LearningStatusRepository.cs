using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics; // Thêm using cho Debug (thay vì Console)
using WordVaultAppMVC.Models;

namespace WordVaultAppMVC.Data
{
    /// <summary>
    /// Repository chịu trách nhiệm truy cập và quản lý dữ liệu trạng thái học tập (LearningStatus)
    /// trong cơ sở dữ liệu.
    /// </summary>
    public class LearningStatusRepository
    {
        #region Public Methods

        /// <summary>
        /// Lấy thông tin trạng thái học tập của một từ vựng cụ thể dựa vào WordId.
        /// </summary>
        /// <param name="wordId">ID (dưới dạng chuỗi) của từ vựng cần lấy trạng thái.</param>
        /// <returns>Đối tượng LearningStatus nếu tìm thấy, ngược lại trả về null.</returns>
        public LearningStatus GetLearningStatusByWordId(string wordId)
        {
            LearningStatus status = null;
            // Câu lệnh SQL để chọn bản ghi dựa trên WordId.
            string query = "SELECT Id, WordId, UserId, Status, DateLearned FROM dbo.LearningStatuses WHERE WordId = @WordId";

            try
            {
                // Sử dụng using để đảm bảo kết nối được đóng đúng cách.
                using (SqlConnection conn = DatabaseContext.GetConnection())
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Thêm tham số WordId để tránh SQL Injection.
                    cmd.Parameters.Add("@WordId", SqlDbType.NVarChar, 50).Value = wordId ?? (object)DBNull.Value; // Xử lý trường hợp wordId có thể null

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Map dữ liệu từ SqlDataReader sang đối tượng LearningStatus.
                            status = MapReaderToLearningStatus(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi thay vì chỉ in ra Console trong ứng dụng thực tế.
                Debug.WriteLine($"[ERROR] Lỗi khi lấy LearningStatus theo WordId='{wordId}': {ex.Message}");
                // Cân nhắc ném lại lỗi hoặc trả về null tùy thuộc vào yêu cầu xử lý lỗi của ứng dụng.
            }

            return status;
        }

        /// <summary>
        /// Thêm một bản ghi trạng thái học tập mới vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="status">Đối tượng LearningStatus chứa thông tin cần thêm.</param>
        public void AddLearningStatus(LearningStatus status)
        {
            // Kiểm tra đầu vào cơ bản.
            if (status == null || string.IsNullOrEmpty(status.WordId) || string.IsNullOrEmpty(status.Status))
            {
                Debug.WriteLine("[ERROR] AddLearningStatus: Thông tin trạng thái không hợp lệ (null hoặc thiếu trường bắt buộc).");
                return; // Hoặc ném ArgumentNullException
            }

            // Câu lệnh SQL INSERT.
            string query = @"INSERT INTO dbo.LearningStatuses (WordId, UserId, Status, DateLearned)
                             VALUES (@WordId, @UserId, @Status, @DateLearned)";

            try
            {
                using (SqlConnection conn = DatabaseContext.GetConnection())
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Thêm các tham số từ đối tượng status.
                    cmd.Parameters.Add("@WordId", SqlDbType.NVarChar, 50).Value = status.WordId;
                    // Xử lý UserId có thể null.
                    cmd.Parameters.Add("@UserId", SqlDbType.NVarChar, 50).Value = (object)status.UserId ?? DBNull.Value;
                    cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = status.Status;
                    // Xử lý DateLearned có thể là giá trị mặc định.
                    cmd.Parameters.Add("@DateLearned", SqlDbType.DateTime).Value = (status.DateLearned > DateTime.MinValue) ? (object)status.DateLearned : DBNull.Value;

                    conn.Open();
                    cmd.ExecuteNonQuery(); // Thực thi lệnh INSERT.
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi thêm LearningStatus cho WordId='{status.WordId}': {ex.Message}");
                // Cân nhắc ném lại lỗi.
            }
        }

        /// <summary>
        /// Cập nhật thông tin của một bản ghi trạng thái học tập đã tồn tại.
        /// Việc cập nhật dựa trên WordId.
        /// </summary>
        /// <param name="status">Đối tượng LearningStatus chứa thông tin cập nhật.</param>
        public void UpdateLearningStatus(LearningStatus status)
        {
            if (status == null || string.IsNullOrEmpty(status.WordId) || string.IsNullOrEmpty(status.Status))
            {
                Debug.WriteLine("[ERROR] UpdateLearningStatus: Thông tin trạng thái không hợp lệ (null hoặc thiếu trường bắt buộc).");
                return; // Hoặc ném ArgumentNullException
            }

            // Câu lệnh SQL UPDATE.
            string query = @"UPDATE dbo.LearningStatuses
                             SET UserId = @UserId, Status = @Status, DateLearned = @DateLearned
                             WHERE WordId = @WordId";

            try
            {
                using (SqlConnection conn = DatabaseContext.GetConnection())
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Thêm các tham số.
                    cmd.Parameters.Add("@UserId", SqlDbType.NVarChar, 50).Value = (object)status.UserId ?? DBNull.Value;
                    cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = status.Status;
                    cmd.Parameters.Add("@DateLearned", SqlDbType.DateTime).Value = (status.DateLearned > DateTime.MinValue) ? (object)status.DateLearned : DBNull.Value;
                    cmd.Parameters.Add("@WordId", SqlDbType.NVarChar, 50).Value = status.WordId;

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery(); // Thực thi lệnh UPDATE.

                    // Kiểm tra xem có bản ghi nào được cập nhật không (tùy chọn).
                    if (rowsAffected == 0)
                    {
                        Debug.WriteLine($"[WARN] UpdateLearningStatus: Không tìm thấy bản ghi nào với WordId='{status.WordId}' để cập nhật.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi cập nhật LearningStatus cho WordId='{status.WordId}': {ex.Message}");
                // Cân nhắc ném lại lỗi.
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả các bản ghi trạng thái học tập từ cơ sở dữ liệu.
        /// </summary>
        /// <returns>Một danh sách các đối tượng LearningStatus.</returns>
        public List<LearningStatus> GetAllLearningStatus()
        {
            List<LearningStatus> statuses = new List<LearningStatus>();
            string query = "SELECT Id, WordId, UserId, Status, DateLearned FROM dbo.LearningStatuses";

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
                            // Map dữ liệu và thêm vào danh sách.
                            LearningStatus status = MapReaderToLearningStatus(reader);
                            if (status != null) // Kiểm tra null phòng trường hợp Map lỗi
                            {
                                statuses.Add(status);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi lấy tất cả LearningStatus: {ex.Message}");
                // Trả về danh sách rỗng hoặc ném lỗi tùy yêu cầu.
            }

            return statuses;
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Ánh xạ dữ liệu từ một hàng SqlDataReader sang đối tượng LearningStatus.
        /// </summary>
        /// <param name="reader">Đối tượng SqlDataReader đang đọc dữ liệu.</param>
        /// <returns>Đối tượng LearningStatus đã được điền dữ liệu, hoặc null nếu có lỗi.</returns>
        private LearningStatus MapReaderToLearningStatus(SqlDataReader reader)
        {
            try
            {
                // Lấy index của các cột một lần để tối ưu.
                int idCol = reader.GetOrdinal("Id");
                int wordIdCol = reader.GetOrdinal("WordId");
                int userIdCol = reader.GetOrdinal("UserId");
                int statusCol = reader.GetOrdinal("Status");
                int dateLearnedCol = reader.GetOrdinal("DateLearned");

                return new LearningStatus
                {
                    // Kiểm tra DBNull trước khi lấy giá trị.
                    Id = reader.IsDBNull(idCol) ? 0 : reader.GetInt32(idCol),
                    WordId = reader.IsDBNull(wordIdCol) ? null : reader.GetString(wordIdCol),
                    UserId = reader.IsDBNull(userIdCol) ? null : reader.GetString(userIdCol),
                    Status = reader.IsDBNull(statusCol) ? null : reader.GetString(statusCol),
                    DateLearned = reader.IsDBNull(dateLearnedCol) ? DateTime.MinValue : reader.GetDateTime(dateLearnedCol)
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi map SqlDataReader sang LearningStatus: {ex.Message}");
                return null; // Trả về null nếu map lỗi.
            }
        }

        #endregion
    }
}