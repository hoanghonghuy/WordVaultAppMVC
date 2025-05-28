using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Forms; // Cần cho MessageBox (cân nhắc tách riêng logic thông báo)
using WordVaultAppMVC.Data; // Để dùng DatabaseContext

namespace WordVaultAppMVC.Services // Hoặc WordVaultAppMVC.Data
{
    /// <summary>
    /// Cung cấp các dịch vụ quản lý dữ liệu cấp cao như sao lưu, phục hồi,
    /// và xóa dữ liệu học tập của cơ sở dữ liệu ứng dụng.
    /// </summary>
    /// <remarks>
    /// Các phương thức trong lớp này thường yêu cầu quyền hạn cao trên SQL Server.
    /// </remarks>
    public static class DataService // Dùng static cho tiện nếu không cần quản lý trạng thái
    {
        #region Public Static Methods

        /// <summary>
        /// Thực hiện sao lưu toàn bộ cơ sở dữ liệu ứng dụng vào một file .bak.
        /// </summary>
        /// <param name="backupFilePath">Đường dẫn đầy đủ (bao gồm tên file .bak) để lưu bản sao lưu.</param>
        /// <returns>True nếu sao lưu thành công, False nếu thất bại.</returns>
        public static bool BackupDatabase(string backupFilePath)
        {
            // Kiểm tra đường dẫn file có hợp lệ không.
            if (string.IsNullOrWhiteSpace(backupFilePath))
            {
                Debug.WriteLine("[WARN] BackupDatabase: Đường dẫn file sao lưu không hợp lệ.");
                return false;
            }

            SqlConnection conn = null;
            try
            {
                // Lấy kết nối thông thường đến CSDL ứng dụng.
                conn = DatabaseContext.GetConnection();
                conn.Open();

                // Lấy tên CSDL từ kết nối hiện tại.
                string dbName = conn.Database;
                if (string.IsNullOrWhiteSpace(dbName))
                {
                    // Không xác định được tên DB, không thể backup.
                    throw new InvalidOperationException("Không thể xác định tên cơ sở dữ liệu từ chuỗi kết nối.");
                }

                // Tạo câu lệnh T-SQL BACKUP DATABASE.
                // WITH FORMAT: Ghi đè file backup cũ nếu tồn tại.
                // NAME, DESCRIPTION: Thêm thông tin mô tả cho file backup.
                string backupCommand = $@"
                    BACKUP DATABASE [{dbName}]
                    TO DISK = @backupPath
                    WITH FORMAT,
                         NAME = N'{dbName}-Full Database Backup ({DateTime.Now:yyyy-MM-dd HH:mm})',
                         DESCRIPTION = N'Sao lưu toàn bộ cơ sở dữ liệu {dbName}';";

                Debug.WriteLine($"[INFO] Executing Backup: TO DISK = '{backupFilePath}'");

                // Thực thi lệnh backup.
                // Lưu ý: User chạy ứng dụng hoặc Service Account của SQL Server cần có quyền ghi vào thư mục chứa backupFilePath.
                using (SqlCommand cmd = new SqlCommand(backupCommand, conn))
                {
                    cmd.Parameters.AddWithValue("@backupPath", backupFilePath);
                    cmd.CommandTimeout = 300; // Đặt timeout 5 phút cho lệnh backup.
                    cmd.ExecuteNonQuery();
                }
                Debug.WriteLine($"[INFO] Backup database '{dbName}' successful to '{backupFilePath}'");
                return true;
            }
            catch (SqlException sqlEx) // Bắt lỗi SQL cụ thể
            {
                Debug.WriteLine($"[ERROR] Lỗi SQL khi sao lưu database: (Number: {sqlEx.Number}) {sqlEx.Message}");
                MessageBox.Show($"Lỗi SQL khi sao lưu cơ sở dữ liệu:\n{sqlEx.Message}\n\nKiểm tra quyền ghi file của SQL Server Service Account và đường dẫn sao lưu.", "Lỗi Sao lưu SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex) // Bắt các lỗi khác
            {
                Debug.WriteLine($"[ERROR] Lỗi không xác định khi sao lưu database: {ex.ToString()}");
                MessageBox.Show($"Lỗi không mong muốn khi sao lưu cơ sở dữ liệu:\n{ex.Message}", "Lỗi Sao lưu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                // Đảm bảo kết nối được đóng.
                conn?.Close();
            }
        }

        /// <summary>
        /// Thực hiện phục hồi cơ sở dữ liệu từ một file backup (.bak).
        /// !!! CẢNH BÁO: Thao tác này rất nguy hiểm, sẽ ghi đè toàn bộ dữ liệu hiện tại !!!
        /// Yêu cầu ứng dụng phải được khởi động lại sau khi phục hồi thành công.
        /// </summary>
        /// <param name="backupFilePath">Đường dẫn đầy đủ đến file .bak cần phục hồi.</param>
        /// <returns>True nếu phục hồi thành công, False nếu thất bại.</returns>
        public static bool RestoreDatabase(string backupFilePath)
        {
            // Kiểm tra đường dẫn file backup.
            if (string.IsNullOrWhiteSpace(backupFilePath))
            {
                Debug.WriteLine("[WARN] RestoreDatabase: Đường dẫn file sao lưu không hợp lệ.");
                return false;
            }
            if (!System.IO.File.Exists(backupFilePath)) // Kiểm tra file tồn tại
            {
                Debug.WriteLine($"[ERROR] RestoreDatabase: File backup không tồn tại tại '{backupFilePath}'.");
                MessageBox.Show($"File backup không tồn tại:\n{backupFilePath}", "Lỗi Phục hồi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


            SqlConnection masterConn = null; // Kết nối đến master DB
            string dbName = ""; // Tên CSDL cần phục hồi

            try
            {
                // 1. Lấy tên CSDL và tạo chuỗi kết nối đến master.
                using (SqlConnection tempConn = DatabaseContext.GetConnection()) // Lấy connection string gốc
                {
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(tempConn.ConnectionString);
                    dbName = builder.InitialCatalog; // Lấy tên DB từ connection string gốc
                    if (string.IsNullOrWhiteSpace(dbName))
                    {
                        throw new InvalidOperationException("Không thể xác định tên cơ sở dữ liệu từ chuỗi kết nối để phục hồi.");
                    }
                    builder.InitialCatalog = "master"; // Chuyển sang kết nối master
                    string masterConnStr = builder.ConnectionString;
                    masterConn = new SqlConnection(masterConnStr); // Tạo kết nối đến master
                }

                masterConn.Open(); // Mở kết nối đến master

                // 2. Đưa CSDL cần phục hồi về SINGLE_USER mode để ngắt mọi kết nối khác.
                // WITH ROLLBACK IMMEDIATE: Buộc ngắt các kết nối hiện có ngay lập tức.
                string singleUserCmd = $"ALTER DATABASE [{dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                Debug.WriteLine($"[INFO] Setting database '{dbName}' to SINGLE_USER mode...");
                using (SqlCommand cmdSU = new SqlCommand(singleUserCmd, masterConn))
                {
                    cmdSU.CommandTimeout = 120; // Timeout 2 phút
                    cmdSU.ExecuteNonQuery();
                    Debug.WriteLine($"[INFO] Database '{dbName}' is now in SINGLE_USER mode.");
                }

                // Có thể thêm delay nhỏ nếu cần thiết
                // System.Threading.Thread.Sleep(1000);

                // 3. Thực hiện lệnh RESTORE DATABASE.
                // WITH REPLACE: Ghi đè lên CSDL hiện có cùng tên.
                string restoreCommand = $"RESTORE DATABASE [{dbName}] FROM DISK = @backupPath WITH REPLACE;";
                Debug.WriteLine($"[INFO] Executing Restore from '{backupFilePath}'...");
                using (SqlCommand cmdRestore = new SqlCommand(restoreCommand, masterConn))
                {
                    cmdRestore.Parameters.AddWithValue("@backupPath", backupFilePath);
                    cmdRestore.CommandTimeout = 600; // Timeout 10 phút cho lệnh restore.
                    cmdRestore.ExecuteNonQuery();
                    Debug.WriteLine($"[INFO] Database '{dbName}' restore successful from '{backupFilePath}'.");
                }

                // 4. Đưa CSDL về MULTI_USER mode để cho phép kết nối lại.
                // Bước này nên được thực hiện ngay cả khi restore thành công hoặc thất bại (trong finally).
                // Tuy nhiên, để đảm bảo logic, thực hiện ở đây sau khi thành công.
                string multiUserCmd = $"ALTER DATABASE [{dbName}] SET MULTI_USER;";
                Debug.WriteLine($"[INFO] Setting database '{dbName}' back to MULTI_USER mode...");
                using (SqlCommand cmdMU = new SqlCommand(multiUserCmd, masterConn))
                {
                    cmdMU.CommandTimeout = 120; // Timeout 2 phút
                    cmdMU.ExecuteNonQuery();
                    Debug.WriteLine($"[INFO] Database '{dbName}' is now in MULTI_USER mode.");
                }

                return true; // Phục hồi thành công
            }
            catch (SqlException sqlEx) // Bắt lỗi SQL
            {
                Debug.WriteLine($"[ERROR] Lỗi SQL khi phục hồi database '{dbName}': (Number: {sqlEx.Number}) {sqlEx.Message}");
                MessageBox.Show($"Lỗi SQL nghiêm trọng khi phục hồi cơ sở dữ liệu '{dbName}':\n{sqlEx.Message}\n\nKiểm tra quyền hạn restore, file backup, và đảm bảo không có kết nối khác.", "Lỗi Phục hồi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Cố gắng đưa về MULTI_USER sau lỗi
                TrySetMultiUser(dbName);
                return false;
            }
            catch (Exception ex) // Bắt các lỗi khác
            {
                Debug.WriteLine($"[ERROR] Lỗi không xác định khi phục hồi database '{dbName}': {ex.ToString()}");
                MessageBox.Show($"Lỗi không mong muốn khi phục hồi cơ sở dữ liệu '{dbName}':\n{ex.Message}", "Lỗi Phục hồi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Cố gắng đưa về MULTI_USER sau lỗi
                TrySetMultiUser(dbName);
                return false;
            }
            finally
            {
                // Đóng kết nối đến master DB.
                masterConn?.Close();
            }
        }

        /// <summary>
        /// Xóa toàn bộ dữ liệu trong các bảng LearningStatuses và QuizResults.
        /// Sử dụng transaction để đảm bảo tính toàn vẹn.
        /// </summary>
        /// <returns>True nếu xóa thành công, False nếu có lỗi.</returns>
        public static bool ClearLearningData()
        {
            SqlConnection conn = null;
            SqlTransaction transaction = null;
            // Danh sách các bảng cần xóa dữ liệu
            string[] tablesToClear = { "dbo.LearningStatuses", "dbo.QuizResults" };

            try
            {
                conn = DatabaseContext.GetConnection();
                conn.Open();
                transaction = conn.BeginTransaction(); // Bắt đầu transaction

                foreach (string tableName in tablesToClear)
                {
                    string deleteCmd = $"DELETE FROM {tableName};";
                    Debug.WriteLine($"[INFO] Executing: {deleteCmd}");
                    using (SqlCommand cmd = new SqlCommand(deleteCmd, conn, transaction))
                    {
                        // Có thể đặt CommandTimeout nếu bảng lớn
                        cmd.ExecuteNonQuery();
                    }
                    Debug.WriteLine($"[INFO] Cleared data from table: {tableName}");
                }

                transaction.Commit(); // Hoàn tất transaction nếu không có lỗi
                Debug.WriteLine("[INFO] Learning data and quiz results cleared successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi xóa dữ liệu học tập: {ex.Message}");
                try
                {
                    transaction?.Rollback(); // Hoàn tác transaction nếu có lỗi
                    Debug.WriteLine("[WARN] Transaction rolled back due to error in ClearLearningData.");
                }
                catch (Exception rollbackEx)
                {
                    // Lỗi khi rollback thường hiếm gặp nhưng cần ghi log
                    Debug.WriteLine($"[ERROR] Lỗi nghiêm trọng khi rollback transaction xóa dữ liệu học tập: {rollbackEx.Message}");
                }

                MessageBox.Show($"Lỗi khi xóa dữ liệu học tập:\n{ex.Message}", "Lỗi Xóa Dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                // Đảm bảo kết nối được đóng.
                conn?.Close();
            }
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Cố gắng đặt lại cơ sở dữ liệu về chế độ MULTI_USER sau khi xảy ra lỗi phục hồi.
        /// </summary>
        /// <param name="dbName">Tên cơ sở dữ liệu.</param>
        private static void TrySetMultiUser(string dbName)
        {
            if (string.IsNullOrWhiteSpace(dbName)) return;

            Debug.WriteLine($"[INFO] Attempting to set database '{dbName}' back to MULTI_USER mode after error...");
            SqlConnection emergencyConn = null;
            try
            {
                // Tạo kết nối mới đến master DB
                using (SqlConnection tempConn = DatabaseContext.GetConnection())
                {
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(tempConn.ConnectionString);
                    builder.InitialCatalog = "master";
                    emergencyConn = new SqlConnection(builder.ConnectionString);
                }

                emergencyConn.Open();
                string multiUserCmd = $"ALTER DATABASE [{dbName}] SET MULTI_USER WITH ROLLBACK IMMEDIATE;"; // Thêm Rollback Immediate để chắc chắn
                using (SqlCommand cmdMU = new SqlCommand(multiUserCmd, emergencyConn))
                {
                    cmdMU.CommandTimeout = 120;
                    cmdMU.ExecuteNonQuery();
                    Debug.WriteLine($"[INFO] Successfully set database '{dbName}' back to MULTI_USER mode after error.");
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nhưng không ném ra ngoài, vì đây chỉ là cố gắng khắc phục
                Debug.WriteLine($"[ERROR] Failed to set database '{dbName}' back to MULTI_USER mode after error: {ex.Message}");
            }
            finally
            {
                emergencyConn?.Close();
            }
        }


        #endregion
    }
}