#region Using Directives
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO; // Cần cho Path, StreamWriter
using System.Configuration; // Cần cho ConfigurationManager
using WordVaultAppMVC.Models; // Namespace của Vocabulary model
using System.Text; // Cần cho Encoding.UTF8
using System.Windows.Forms; // Cần cho MessageBox 
#endregion

namespace WordVaultAppMVC.Services
{
    /// <summary>
    /// Cung cấp chức năng xuất dữ liệu từ vựng ra file định dạng TSV (Tab-Separated Values).
    /// </summary>
    /// <remarks>
    /// Lớp này truy cập trực tiếp vào CSDL và cấu hình, có thể được gọi từ dòng lệnh.
    /// </remarks>
    public static class VocabularyExporterService
    {
        #region Constants

        // Tên bảng và cột (nên thống nhất với VocabularyRepository)
        private const string VocabularyTableName = "dbo.Vocabulary"; // Thêm schema
        private const string ColId = "Id";
        private const string ColWord = "Word";
        private const string ColMeaning = "Meaning";
        private const string ColPronunciation = "Pronunciation";
        private const string ColAudioUrl = "AudioUrl";

        // Tên file xuất mặc định
        private const string OutputFileName = "vocabulary_export.tsv"; // Đổi thành .tsv cho rõ ràng

        #endregion

        #region Public Export Method

        /// <summary>
        /// Thực hiện quy trình xuất toàn bộ từ vựng: Lấy dữ liệu từ CSDL và ghi ra file TSV.
        /// Hiển thị thông báo kết quả hoặc lỗi cho người dùng qua MessageBox.
        /// </summary>
        /// <remarks>
        /// Phương thức này có thể được gọi từ giao diện người dùng hoặc từ dòng lệnh (như trong Program.cs).
        /// Việc sử dụng MessageBox có thể không phù hợp nếu gọi từ tiến trình nền không có giao diện.
        /// </remarks>
        public static void ExportAllVocabularyToFile()
        {
            string connectionString = null;
            List<Vocabulary> vocabularyList = null;

            try
            {
                // 1. Lấy chuỗi kết nối
                connectionString = GetConnectionString();
                if (string.IsNullOrEmpty(connectionString))
                {
                    // Lỗi đã được log và thông báo trong GetConnectionString
                    return;
                }

                // 2. Lấy dữ liệu từ CSDL
                Debug.WriteLine("[Exporter] Bắt đầu lấy dữ liệu từ vựng...");
                vocabularyList = GetAllVocabularyData(connectionString);

                // Kiểm tra kết quả lấy dữ liệu
                if (vocabularyList == null)
                {
                    // Lỗi đã được log và thông báo trong GetAllVocabularyData
                    return;
                }
                else if (vocabularyList.Count == 0)
                {
                    Debug.WriteLine("[Exporter] Không tìm thấy từ vựng nào để xuất.");
                    MessageBox.Show("Không tìm thấy dữ liệu từ vựng nào để xuất.",
                                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 3. Ghi dữ liệu ra file
                Debug.WriteLine($"[Exporter] Tìm thấy {vocabularyList.Count} từ vựng. Bắt đầu ghi ra file...");
                WriteDataToFile(vocabularyList); // Hàm này sẽ hiển thị thông báo thành công/lỗi ghi file
            }
            catch (Exception ex) // Bắt các lỗi không mong muốn ở tầng cao nhất
            {
                Debug.WriteLine($"[ERROR] Lỗi không mong muốn trong quá trình xuất: {ex.ToString()}");
                MessageBox.Show($"Đã xảy ra lỗi không mong muốn trong quá trình xuất:\n{ex.Message}",
                                "Lỗi Xuất Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Debug.WriteLine("[Exporter] Quá trình xuất kết thúc.");
            }
        }
        #endregion

        #region Data Access (Private)

        /// <summary>
        /// Lấy chuỗi kết nối có tên 'WordVaultDb' từ file App.config.
        /// </summary>
        /// <returns>Chuỗi kết nối nếu hợp lệ, ngược lại trả về null.</returns>
        private static string GetConnectionString()
        {
            try
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings["WordVaultDb"];
                if (settings != null && !string.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    return settings.ConnectionString;
                }
                else
                {
                    Debug.WriteLine("[ERROR] GetConnectionString: Không tìm thấy hoặc chuỗi kết nối 'WordVaultDb' trống trong App.config.");
                    MessageBox.Show("LỖI: Không tìm thấy hoặc chuỗi kết nối 'WordVaultDb' không hợp lệ trong App.config.",
                                    "Lỗi Cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            catch (ConfigurationErrorsException configEx)
            {
                Debug.WriteLine($"[ERROR] GetConnectionString: Lỗi cấu hình App.config: {configEx.Message}");
                MessageBox.Show($"LỖI cấu hình App.config: {configEx.Message}", "Lỗi Cấu hình", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            catch (Exception ex) // Bắt các lỗi khác
            {
                Debug.WriteLine($"[ERROR] GetConnectionString: Lỗi không mong muốn: {ex.Message}");
                MessageBox.Show($"Lỗi không mong muốn khi đọc cấu hình: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả từ vựng từ cơ sở dữ liệu.
        /// </summary>
        /// <param name="connectionString">Chuỗi kết nối đến cơ sở dữ liệu.</param>
        /// <returns>Danh sách các đối tượng Vocabulary, hoặc null nếu có lỗi.</returns>
        private static List<Vocabulary> GetAllVocabularyData(string connectionString)
        {
            List<Vocabulary> vocabularies = new List<Vocabulary>();
            // Lấy tất cả các cột từ bảng Vocabulary.
            string query = $"SELECT {ColId}, {ColWord}, {ColMeaning}, {ColPronunciation}, {ColAudioUrl} FROM {VocabularyTableName}";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Map dữ liệu từ reader sang đối tượng Vocabulary.
                            Vocabulary vocab = MapReaderToVocabulary(reader);
                            if (vocab != null) // Thêm kiểm tra null
                            {
                                vocabularies.Add(vocab);
                            }
                        }
                    }
                }
                return vocabularies; // Trả về danh sách nếu thành công
            }
            catch (SqlException sqlEx) // Bắt lỗi SQL cụ thể
            {
                Debug.WriteLine($"[ERROR] GetAllVocabularyData: Lỗi SQL khi truy vấn CSDL: {sqlEx.Message}");
                MessageBox.Show($"LỖI SQL khi truy cập cơ sở dữ liệu: {sqlEx.Message}", "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null; // Trả về null nếu có lỗi CSDL
            }
            catch (Exception ex) // Bắt các lỗi khác
            {
                Debug.WriteLine($"[ERROR] GetAllVocabularyData: Lỗi không xác định: {ex.Message}");
                MessageBox.Show($"LỖI không xác định khi truy cập cơ sở dữ liệu: {ex.Message}", "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null; // Trả về null nếu có lỗi
            }
            // Không cần finally để đóng connection vì đã dùng using
        }

        /// <summary>
        /// Ánh xạ dữ liệu từ một hàng SqlDataReader sang đối tượng Vocabulary.
        /// </summary>
        /// <param name="reader">Đối tượng SqlDataReader đang đọc dữ liệu.</param>
        /// <returns>Đối tượng Vocabulary đã được điền dữ liệu, hoặc null nếu có lỗi.</returns>
        /// <remarks>
        /// Hàm này tương tự hàm trong VocabularyRepository, cân nhắc tái sử dụng nếu có thể.
        /// </remarks>
        private static Vocabulary MapReaderToVocabulary(SqlDataReader reader)
        {
            if (reader == null || reader.IsClosed) return null;
            try
            {
                // Lấy index các cột
                int idOrdinal = reader.GetOrdinal(ColId);
                int wordOrdinal = reader.GetOrdinal(ColWord);
                int meaningOrdinal = reader.GetOrdinal(ColMeaning);
                int pronunciationOrdinal = reader.GetOrdinal(ColPronunciation);
                int audioUrlOrdinal = reader.GetOrdinal(ColAudioUrl);

                // Tạo đối tượng và kiểm tra DBNull
                return new Vocabulary
                {
                    Id = reader.IsDBNull(idOrdinal) ? 0 : reader.GetInt32(idOrdinal),
                    Word = reader.IsDBNull(wordOrdinal) ? string.Empty : reader.GetString(wordOrdinal),
                    Meaning = reader.IsDBNull(meaningOrdinal) ? string.Empty : reader.GetString(meaningOrdinal),
                    Pronunciation = reader.IsDBNull(pronunciationOrdinal) ? string.Empty : reader.GetString(pronunciationOrdinal),
                    AudioUrl = reader.IsDBNull(audioUrlOrdinal) ? string.Empty : reader.GetString(audioUrlOrdinal)
                };
            }
            catch (IndexOutOfRangeException ioorex)
            {
                Debug.WriteLine($"[ERROR] MapReaderToVocabulary (Exporter): Tên cột không đúng. {ioorex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] MapReaderToVocabulary (Exporter): Lỗi không xác định. {ex.Message}");
                return null;
            }
        }
        #endregion

        #region File Export (Private)

        /// <summary>
        /// Ghi danh sách từ vựng vào file TSV tại thư mục chạy của ứng dụng.
        /// </summary>
        /// <param name="vocabularyList">Danh sách từ vựng cần ghi.</param>
        private static void WriteDataToFile(List<Vocabulary> vocabularyList)
        {
            // Lấy đường dẫn thư mục hiện tại của ứng dụng.
            string outputDirectory = AppDomain.CurrentDomain.BaseDirectory;
            // Kết hợp đường dẫn thư mục và tên file.
            string filePath = Path.Combine(outputDirectory, OutputFileName);

            Debug.WriteLine($"[Exporter] Bắt đầu ghi vào file: {filePath}");
            try
            {
                // Sử dụng StreamWriter với Encoding UTF8 để hỗ trợ tiếng Việt.
                // Tham số 'false' trong constructor nghĩa là ghi đè file cũ nếu tồn tại.
                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    // Ghi dòng tiêu đề (header).
                    writer.WriteLine($"{ColId}\t{ColWord}\t{ColMeaning}\t{ColPronunciation}\t{ColAudioUrl}");

                    // Ghi từng dòng dữ liệu từ vựng.
                    foreach (var vocab in vocabularyList)
                    {
                        // Tạo dòng TSV, escape các ký tự đặc biệt trong mỗi trường.
                        string line = string.Join("\t",
                            vocab.Id.ToString(), // Id không cần escape
                            EscapeTsvField(vocab.Word),
                            EscapeTsvField(vocab.Meaning),
                            EscapeTsvField(vocab.Pronunciation),
                            EscapeTsvField(vocab.AudioUrl)
                        );
                        writer.WriteLine(line);
                    }
                }
                // Thông báo thành công cho người dùng.
                MessageBox.Show($"Xuất dữ liệu thành công! ({vocabularyList.Count} từ)\nFile đã được lưu tại:\n{filePath}",
                                "Xuất Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Debug.WriteLine($"[Exporter] Ghi file thành công: {filePath}");
            }
            catch (IOException ioEx) // Bắt lỗi liên quan đến IO (file đang mở, không đủ quyền...)
            {
                Debug.WriteLine($"[ERROR] Lỗi IO khi ghi file '{filePath}': {ioEx.Message}");
                MessageBox.Show($"LỖI khi ghi file: {ioEx.Message}\nĐường dẫn: {filePath}\n\nHãy kiểm tra quyền ghi và đảm bảo file không đang được mở bởi ứng dụng khác.",
                               "Lỗi Ghi File (IO)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException authEx) // Bắt lỗi không có quyền ghi
            {
                Debug.WriteLine($"[ERROR] Lỗi quyền ghi file '{filePath}': {authEx.Message}");
                MessageBox.Show($"LỖI không có quyền ghi file:\n{filePath}\n\nVui lòng kiểm tra quyền hạn thư mục.",
                               "Lỗi Ghi File (Quyền)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) // Bắt các lỗi khác
            {
                Debug.WriteLine($"[ERROR] Lỗi không xác định khi ghi file '{filePath}': {ex.ToString()}");
                MessageBox.Show($"LỖI không xác định khi ghi file: {ex.Message}\nĐường dẫn: {filePath}",
                               "Lỗi Ghi File", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Chuẩn bị một trường dữ liệu để ghi vào file TSV bằng cách loại bỏ các ký tự đặc biệt
        /// (Tab, xuống dòng) có thể làm hỏng cấu trúc file.
        /// </summary>
        /// <param name="value">Chuỗi cần xử lý.</param>
        /// <returns>Chuỗi đã được làm sạch.</returns>
        private static string EscapeTsvField(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            // Thay thế ký tự Tab và các loại ký tự xuống dòng bằng khoảng trắng.
            // Cân nhắc thay bằng một chuỗi khác nếu muốn giữ lại thông tin xuống dòng (ví dụ: "\\n").
            return value.Replace('\t', ' ')        // Thay Tab bằng khoảng trắng
                        .Replace("\r\n", " ")   // Thay CR+LF bằng khoảng trắng
                        .Replace('\n', ' ')        // Thay LF bằng khoảng trắng
                        .Replace('\r', ' ');       // Thay CR bằng khoảng trắng
        }
        #endregion
    }
}