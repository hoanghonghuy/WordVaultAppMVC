using System.Configuration;
using System.Data.SqlClient;
using System; // Thêm using cho Exception

namespace WordVaultAppMVC.Data
{
    /// <summary>
    /// Cung cấp phương thức để lấy kết nối đến cơ sở dữ liệu SQL Server.
    /// Lớp này đọc chuỗi kết nối từ file App.config.
    /// </summary>
    public static class DatabaseContext
    {
        #region Private Static Fields

        // Chuỗi kết nối được đọc từ App.config khi lớp được tải lần đầu.
        // Đảm bảo key "WordVaultDb" tồn tại và có giá trị hợp lệ trong phần <connectionStrings> của App.config.
        private static readonly string connectionString = LoadConnectionString();

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Tạo và trả về một đối tượng SqlConnection mới sử dụng chuỗi kết nối đã được cấu hình.
        /// </summary>
        /// <returns>Một đối tượng SqlConnection mới, sẵn sàng để mở.</returns>
        /// <exception cref="InvalidOperationException">Ném ra nếu chuỗi kết nối không hợp lệ hoặc không thể đọc được từ App.config.</exception>
        public static SqlConnection GetConnection()
        {
            // Kiểm tra xem chuỗi kết nối đã được đọc thành công chưa.
            if (string.IsNullOrEmpty(connectionString))
            {
                // Nếu không đọc được chuỗi kết nối, không thể tạo connection.
                throw new InvalidOperationException("Không thể tạo kết nối CSDL do không tìm thấy hoặc chuỗi kết nối 'WordVaultDb' không hợp lệ trong App.config.");
            }
            // Mỗi lần gọi sẽ tạo một đối tượng SqlConnection mới.
            // Việc quản lý (mở, đóng, dispose) kết nối này thuộc về nơi gọi hàm GetConnection().
            return new SqlConnection(connectionString);
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Đọc chuỗi kết nối từ file cấu hình App.config.
        /// </summary>
        /// <returns>Chuỗi kết nối nếu tìm thấy, ngược lại trả về null.</returns>
        private static string LoadConnectionString()
        {
            try
            {
                // Lấy thông tin chuỗi kết nối có tên "WordVaultDb".
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["WordVaultDb"];
                if (settings != null && !string.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    // Trả về chuỗi kết nối nếu hợp lệ.
                    return settings.ConnectionString;
                }
                else
                {
                    // Ghi log hoặc xử lý trường hợp không tìm thấy hoặc chuỗi rỗng.
                    Console.WriteLine("Lỗi: Không tìm thấy hoặc chuỗi kết nối 'WordVaultDb' trống trong App.config.");
                    return null;
                }
            }
            catch (ConfigurationErrorsException configEx)
            {
                // Lỗi cụ thể liên quan đến cấu hình.
                Console.WriteLine($"Lỗi cấu hình App.config: {configEx.Message}");
                // Có thể ném lại lỗi hoặc hiển thị thông báo tùy theo yêu cầu ứng dụng.
                // throw new InvalidOperationException("Lỗi đọc cấu hình App.config.", configEx);
                return null;
            }
            catch (Exception ex)
            {
                // Bắt các lỗi không mong muốn khác.
                Console.WriteLine($"Lỗi không mong muốn khi đọc chuỗi kết nối: {ex.Message}");
                return null;
            }
        }

        #endregion
    }
}