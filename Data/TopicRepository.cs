using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms;
using WordVaultAppMVC.Models; // Namespace của Topic model
// using WordVaultAppMVC.Data; // DatabaseContext nằm cùng namespace nên không cần using này

namespace WordVaultAppMVC.Data
{
    /// <summary>
    /// Repository chịu trách nhiệm truy cập và quản lý dữ liệu Chủ đề (Topic)
    /// và các liên kết giữa Từ vựng và Chủ đề trong cơ sở dữ liệu.
    /// </summary>
    public class TopicRepository
    {
        #region Public Methods

        /// <summary>
        /// Lấy danh sách tất cả các chủ đề từ bảng Topics, sắp xếp theo tên.
        /// </summary>
        /// <returns>Danh sách các đối tượng Topic.</returns>
        public List<Topic> GetAllTopics()
        {
            List<Topic> topics = new List<Topic>();
            // Lấy kết nối một cách an toàn.
            SqlConnection conn = null;
            // Câu lệnh SQL để lấy Id và Name, sắp xếp theo Name.
            string query = "SELECT Id, Name FROM dbo.Topics ORDER BY Name";

            try
            {
                conn = DatabaseContext.GetConnection();
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Map dữ liệu từ reader sang đối tượng Topic.
                        Topic topic = MapReaderToTopic(reader);
                        if (topic != null) // Kiểm tra null phòng trường hợp map lỗi
                        {
                            topics.Add(topic);
                        }
                    }
                }
                Debug.WriteLine($"[INFO] GetAllTopics: Found {topics.Count} topics.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi lấy danh sách Topics: {ex.Message}");
                // Cân nhắc ném lại lỗi hoặc trả về danh sách rỗng tùy logic ứng dụng.
                // throw;
            }
            finally
            {
                // Đảm bảo kết nối được đóng ngay cả khi có lỗi.
                conn?.Close(); // Sử dụng toán tử ?. để kiểm tra null an toàn
            }
            return topics;
        }

        /// <summary>
        /// Thêm một chủ đề mới vào bảng Topics.
        /// </summary>
        /// <param name="topicName">Tên của chủ đề mới cần thêm.</param>
        /// <remarks>
        /// Phương thức này sẽ tự động cắt bỏ khoảng trắng thừa ở đầu và cuối tên chủ đề.
        /// Cân nhắc thêm ràng buộc UNIQUE trên cột Name trong CSDL để tránh trùng lặp tên chủ đề.
        /// </remarks>
        public void AddTopic(string topicName)
        {
            // Kiểm tra tên chủ đề có hợp lệ không.
            if (string.IsNullOrWhiteSpace(topicName))
            {
                Debug.WriteLine("[WARN] AddTopic: Tên chủ đề không được để trống.");
                // Ném lỗi để báo hiệu cho nơi gọi hàm biết thao tác không thành công.
                throw new ArgumentException("Tên chủ đề không được để trống.", nameof(topicName));
            }

            SqlConnection conn = null;
            // Câu lệnh INSERT, sử dụng OUTPUT INSERTED.Id để có thể lấy ID vừa tạo nếu cần.
            // Nếu không cần ID, có thể bỏ phần OUTPUT.
            string query = "INSERT INTO dbo.Topics (Name) /* OUTPUT INSERTED.Id */ VALUES (@Name)";

            try
            {
                conn = DatabaseContext.GetConnection();
                conn.Open();

                // (Tùy chọn) Kiểm tra trùng lặp trước khi thêm
                // string checkQuery = "SELECT COUNT(*) FROM dbo.Topics WHERE Name = @Name";
                // using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn)) { ... }

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Thêm tham số Name, Trim() để loại bỏ khoảng trắng thừa.
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = topicName.Trim();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Debug.WriteLine($"[INFO] Đã thêm chủ đề mới: '{topicName.Trim()}'");
                    }
                    else
                    {
                        Debug.WriteLine($"[WARN] AddTopic: Không thêm được chủ đề '{topicName.Trim()}'. ExecuteNonQuery trả về 0.");
                    }
                    // Nếu cần lấy ID vừa thêm:
                    // object newId = cmd.ExecuteScalar();
                    // if (newId != null) { Debug.WriteLine($"New Topic ID: {newId}"); }
                }
            }
            catch (SqlException sqlEx) // Bắt lỗi SQL cụ thể
            {
                // Kiểm tra lỗi trùng lặp (số lỗi 2627 hoặc 2601 tùy phiên bản SQL Server và loại constraint)
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                {
                    Debug.WriteLine($"[WARN] AddTopic: Chủ đề '{topicName.Trim()}' đã tồn tại.");
                    // Thông báo cho người dùng hoặc bỏ qua thay vì ném lỗi
                    MessageBox.Show($"Chủ đề '{topicName.Trim()}' đã tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Debug.WriteLine($"[ERROR] Lỗi SQL khi thêm chủ đề '{topicName.Trim()}': {sqlEx.Message}");
                    // Ném lại lỗi để tầng trên xử lý
                    throw;
                }
            }
            catch (Exception ex) // Bắt các lỗi khác
            {
                Debug.WriteLine($"[ERROR] Lỗi không xác định khi thêm chủ đề '{topicName.Trim()}': {ex.Message}");
                // Ném lại lỗi để tầng trên xử lý
                throw;
            }
            finally
            {
                conn?.Close();
            }
        }

        /// <summary>
        /// Xóa liên kết giữa một từ vựng và một chủ đề khỏi bảng trung gian VocabularyTopic.
        /// </summary>
        /// <param name="vocabularyId">ID của từ vựng.</param>
        /// <param name="topicId">ID của chủ đề.</param>
        /// <returns>True nếu xóa thành công (ít nhất 1 dòng bị ảnh hưởng), False nếu không hoặc có lỗi.</returns>
        public bool RemoveWordFromTopic(int vocabularyId, int topicId)
        {
            // Kiểm tra ID đầu vào.
            if (vocabularyId <= 0 || topicId <= 0)
            {
                Debug.WriteLine($"[WARN] RemoveWordFromTopic: ID không hợp lệ (VocabularyId={vocabularyId}, TopicId={topicId}).");
                return false;
            }

            int rowsAffected = 0;
            SqlConnection conn = null;
            // Câu lệnh DELETE khỏi bảng liên kết.
            string query = "DELETE FROM dbo.VocabularyTopic WHERE VocabularyId = @VocabularyId AND TopicId = @TopicId";

            try
            {
                conn = DatabaseContext.GetConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Thêm các tham số ID.
                    cmd.Parameters.Add("@VocabularyId", SqlDbType.Int).Value = vocabularyId;
                    cmd.Parameters.Add("@TopicId", SqlDbType.Int).Value = topicId;

                    // Thực thi và lấy số dòng bị ảnh hưởng.
                    rowsAffected = cmd.ExecuteNonQuery();
                    Debug.WriteLine($"[INFO] RemoveWordFromTopic: Attempted removal for VocabID={vocabularyId}, TopicID={topicId}. Rows affected: {rowsAffected}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi xóa liên kết từ khỏi chủ đề (VocabID={vocabularyId}, TopicID={topicId}): {ex.Message}");
                return false; // Trả về false nếu có lỗi.
            }
            finally
            {
                conn?.Close();
            }

            // Trả về true nếu có ít nhất một dòng đã bị xóa.
            return rowsAffected > 0;
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Ánh xạ dữ liệu từ một hàng SqlDataReader sang đối tượng Topic.
        /// </summary>
        /// <param name="reader">Đối tượng SqlDataReader đang đọc dữ liệu.</param>
        /// <returns>Đối tượng Topic đã được điền dữ liệu, hoặc null nếu có lỗi.</returns>
        private Topic MapReaderToTopic(SqlDataReader reader)
        {
            try
            {
                int idCol = reader.GetOrdinal("Id");
                int nameCol = reader.GetOrdinal("Name");

                return new Topic
                {
                    Id = reader.IsDBNull(idCol) ? 0 : reader.GetInt32(idCol),
                    Name = reader.IsDBNull(nameCol) ? string.Empty : reader.GetString(nameCol)
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi map SqlDataReader sang Topic: {ex.Message}");
                return null;
            }
        }

        #endregion

        // Cân nhắc thêm các phương thức khác nếu cần:
        // public bool UpdateTopic(Topic topic) { ... }
        // public bool DeleteTopic(int topicId) { ... } // Xóa hẳn chủ đề (cần xử lý các liên kết)
        // public bool AddWordToTopic(int vocabularyId, int topicId) { ... } // Thêm liên kết
    }
}