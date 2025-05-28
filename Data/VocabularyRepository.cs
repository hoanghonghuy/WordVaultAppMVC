using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq; // Required for .Any() if used
using WordVaultAppMVC.Models;
// Assuming DatabaseContext is in the same namespace or WordVaultAppMVC.Data
// using WordVaultAppMVC.Data;

namespace WordVaultAppMVC.Data
{
    /// <summary>
    /// Repository chịu trách nhiệm truy cập và quản lý dữ liệu Từ vựng (Vocabulary)
    /// cũng như các chức năng liên quan như Yêu thích (Favorites) và liên kết với Chủ đề (Topics).
    /// </summary>
    public class VocabularyRepository
    {
        #region Constants

        // Tên bảng trong cơ sở dữ liệu
        private const string VocabularyTableName = "dbo.Vocabulary"; // Thêm schema dbo.
        private const string FavoriteWordsTableName = "dbo.FavoriteWords";
        private const string TopicTableName = "dbo.Topics";
        private const string VocabTopicTableName = "dbo.VocabularyTopic";

        // Tên cột trong bảng Vocabulary - Sử dụng nameof() để an toàn khi đổi tên thuộc tính trong Model
        private const string ColId = nameof(Vocabulary.Id);
        private const string ColWord = nameof(Vocabulary.Word);
        private const string ColMeaning = nameof(Vocabulary.Meaning);
        private const string ColPronunciation = nameof(Vocabulary.Pronunciation);
        private const string ColAudioUrl = nameof(Vocabulary.AudioUrl);

        // Tên cột trong các bảng liên quan (phải khớp với schema CSDL)
        private const string FavColVocabId = "VocabularyId";
        private const string TopicColId = "Id"; // Thêm cột Id cho Topic
        private const string TopicColName = "Name";
        private const string VTColVocabId = "VocabularyId";
        private const string VTColTopicId = "TopicId";

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Ánh xạ dữ liệu từ một hàng SqlDataReader sang đối tượng Vocabulary.
        /// </summary>
        /// <param name="reader">Đối tượng SqlDataReader đang đọc dữ liệu.</param>
        /// <returns>Đối tượng Vocabulary đã được điền dữ liệu, hoặc null nếu có lỗi hoặc reader không hợp lệ.</returns>
        private Vocabulary MapReaderToVocabulary(SqlDataReader reader)
        {
            // Kiểm tra reader có hợp lệ không.
            if (reader == null || reader.IsClosed)
            {
                Debug.WriteLine("[WARN] MapReaderToVocabulary: SqlDataReader is null or closed.");
                return null;
            }

            try
            {
                // Lấy index các cột một lần để tối ưu.
                int idOrdinal = reader.GetOrdinal(ColId);
                int wordOrdinal = reader.GetOrdinal(ColWord);
                int meaningOrdinal = reader.GetOrdinal(ColMeaning);
                int pronunciationOrdinal = reader.GetOrdinal(ColPronunciation);
                int audioUrlOrdinal = reader.GetOrdinal(ColAudioUrl);

                // Tạo đối tượng Vocabulary và điền dữ liệu, kiểm tra DBNull.
                return new Vocabulary
                {
                    Id = reader.IsDBNull(idOrdinal) ? 0 : reader.GetInt32(idOrdinal),
                    Word = reader.IsDBNull(wordOrdinal) ? string.Empty : reader.GetString(wordOrdinal),
                    Meaning = reader.IsDBNull(meaningOrdinal) ? string.Empty : reader.GetString(meaningOrdinal),
                    Pronunciation = reader.IsDBNull(pronunciationOrdinal) ? string.Empty : reader.GetString(pronunciationOrdinal),
                    AudioUrl = reader.IsDBNull(audioUrlOrdinal) ? string.Empty : reader.GetString(audioUrlOrdinal)
                };
            }
            catch (IndexOutOfRangeException ioorex) // Bắt lỗi cụ thể nếu tên cột sai
            {
                Debug.WriteLine($"[ERROR] MapReaderToVocabulary: Tên cột không đúng hoặc không tồn tại. {ioorex.Message}");
                return null;
            }
            catch (Exception ex) // Bắt các lỗi khác
            {
                Debug.WriteLine($"[ERROR] MapReaderToVocabulary: Lỗi không xác định. {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Thêm tham số vào SqlCommand, xử lý giá trị null bằng DBNull.Value.
        /// </summary>
        /// <param name="cmd">SqlCommand cần thêm tham số.</param>
        /// <param name="parameterName">Tên tham số (bao gồm @).</param>
        /// <param name="dbType">Kiểu dữ liệu SQL.</param>
        /// <param name="size">Kích thước (nếu có).</param>
        /// <param name="value">Giá trị của tham số.</param>
        private void AddParameterWithValue(SqlCommand cmd, string parameterName, SqlDbType dbType, int size, object value)
        {
            SqlParameter param = new SqlParameter(parameterName, dbType, size);
            param.Value = value ?? DBNull.Value; // Xử lý null
                                                 // Xử lý chuỗi rỗng thành DBNull nếu cần thiết (tùy thuộc vào yêu cầu)
            if (value is string stringValue && string.IsNullOrEmpty(stringValue))
            {
                param.Value = DBNull.Value;
            }
            cmd.Parameters.Add(param);
        }
        // Overload không cần size
        private void AddParameterWithValue(SqlCommand cmd, string parameterName, SqlDbType dbType, object value)
        {
            SqlParameter param = new SqlParameter(parameterName, dbType);
            param.Value = value ?? DBNull.Value;
            if (value is string stringValue && string.IsNullOrEmpty(stringValue))
            {
                param.Value = DBNull.Value;
            }
            cmd.Parameters.Add(param);
        }


        #endregion

        #region CRUD Operations

        /// <summary>
        /// Lấy danh sách tất cả các từ vựng từ cơ sở dữ liệu.
        /// </summary>
        /// <returns>Danh sách các đối tượng Vocabulary.</returns>
        public List<Vocabulary> GetAllVocabulary()
        {
            List<Vocabulary> vocabularies = new List<Vocabulary>();
            string query = $"SELECT {ColId}, {ColWord}, {ColMeaning}, {ColPronunciation}, {ColAudioUrl} FROM {VocabularyTableName}";
            SqlConnection conn = null;

            try
            {
                conn = DatabaseContext.GetConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Vocabulary vocab = MapReaderToVocabulary(reader);
                        if (vocab != null) vocabularies.Add(vocab);
                    }
                }
                Debug.WriteLine($"[INFO] GetAllVocabulary: Found {vocabularies.Count} vocabularies.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi lấy tất cả Vocabulary: {ex.Message}");
                // Trả về danh sách rỗng khi có lỗi
                vocabularies.Clear();
            }
            finally
            {
                conn?.Close();
            }
            return vocabularies;
        }

        /// <summary>
        /// Lấy thông tin một từ vựng cụ thể dựa vào ID.
        /// </summary>
        /// <param name="id">ID của từ vựng cần lấy.</param>
        /// <returns>Đối tượng Vocabulary nếu tìm thấy, ngược lại trả về null.</returns>
        public Vocabulary GetVocabularyById(int id)
        {
            // Kiểm tra ID hợp lệ.
            if (id <= 0)
            {
                Debug.WriteLine($"[WARN] GetVocabularyById: Invalid ID requested: {id}");
                return null;
            }

            Vocabulary vocab = null;
            string query = $"SELECT {ColId}, {ColWord}, {ColMeaning}, {ColPronunciation}, {ColAudioUrl} FROM {VocabularyTableName} WHERE {ColId} = @Id";
            SqlConnection conn = null;

            try
            {
                conn = DatabaseContext.GetConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow)) // Chỉ mong đợi 1 dòng
                    {
                        if (reader.Read())
                        {
                            vocab = MapReaderToVocabulary(reader);
                        }
                        else
                        {
                            Debug.WriteLine($"[INFO] GetVocabularyById: No vocabulary found for ID: {id}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi lấy Vocabulary theo ID {id}: {ex.Message}");
            }
            finally
            {
                conn?.Close();
            }
            return vocab;
        }

        /// <summary>
        /// Lấy thông tin một từ vựng cụ thể dựa vào chính từ đó (Word).
        /// Tìm kiếm không phân biệt hoa thường và bỏ qua khoảng trắng thừa.
        /// </summary>
        /// <param name="word">Từ vựng cần tìm.</param>
        /// <returns>Đối tượng Vocabulary nếu tìm thấy, ngược lại trả về null.</returns>
        public Vocabulary GetVocabularyByWord(string word)
        {
            // Kiểm tra đầu vào.
            if (string.IsNullOrWhiteSpace(word))
            {
                Debug.WriteLine("[WARN] GetVocabularyByWord: Word parameter is null or whitespace.");
                return null;
            }

            Vocabulary vocab = null;
            // Nên thêm COLLATE vào mệnh đề WHERE để đảm bảo tìm kiếm không phân biệt hoa thường
            // Ví dụ: WHERE {ColWord} COLLATE SQL_Latin1_General_CP1_CI_AS = @Word
            string query = $"SELECT {ColId}, {ColWord}, {ColMeaning}, {ColPronunciation}, {ColAudioUrl} FROM {VocabularyTableName} WHERE {ColWord} = @Word";
            SqlConnection conn = null;

            try
            {
                conn = DatabaseContext.GetConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Trim() để loại bỏ khoảng trắng thừa trước khi tìm.
                    AddParameterWithValue(cmd, "@Word", SqlDbType.NVarChar, 100, word.Trim());
                    using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                        {
                            vocab = MapReaderToVocabulary(reader);
                        }
                        else
                        {
                            Debug.WriteLine($"[INFO] GetVocabularyByWord: No vocabulary found for Word: '{word}'");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi lấy Vocabulary theo Word '{word}': {ex.Message}");
            }
            finally
            {
                conn?.Close();
            }
            return vocab;
        }

        /// <summary>
        /// Thêm một từ vựng mới vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="vocab">Đối tượng Vocabulary chứa thông tin cần thêm.</param>
        /// <returns>Đối tượng Vocabulary đã được thêm (với ID được cập nhật) nếu thành công, ngược lại trả về null.</returns>
        public Vocabulary AddVocabulary(Vocabulary vocab)
        {
            // Kiểm tra đầu vào.
            if (vocab == null || string.IsNullOrWhiteSpace(vocab.Word) || string.IsNullOrWhiteSpace(vocab.Meaning))
            {
                Debug.WriteLine("[WARN] AddVocabulary(object): Dữ liệu đầu vào không hợp lệ (null hoặc thiếu Word/Meaning).");
                return null;
            }

            // Câu lệnh INSERT với OUTPUT để lấy ID vừa được tạo.
            string query = $@"INSERT INTO {VocabularyTableName} ({ColWord}, {ColMeaning}, {ColPronunciation}, {ColAudioUrl})
                              OUTPUT INSERTED.{ColId}
                              VALUES (@Word, @Meaning, @Pronunciation, @AudioUrl);";
            SqlConnection conn = null;

            try
            {
                conn = DatabaseContext.GetConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Thêm tham số, Trim() giá trị chuỗi.
                    AddParameterWithValue(cmd, "@Word", SqlDbType.NVarChar, 100, vocab.Word.Trim());
                    AddParameterWithValue(cmd, "@Meaning", SqlDbType.NVarChar, 500, vocab.Meaning.Trim());
                    AddParameterWithValue(cmd, "@Pronunciation", SqlDbType.NVarChar, 100, vocab.Pronunciation);
                    AddParameterWithValue(cmd, "@AudioUrl", SqlDbType.NVarChar, 300, vocab.AudioUrl);

                    // ExecuteScalar để lấy ID trả về từ OUTPUT INSERTED.Id.
                    object result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        vocab.Id = Convert.ToInt32(result);
                        Debug.WriteLine($"[INFO] Đã thêm từ '{vocab.Word}' với ID: {vocab.Id}");
                        return vocab; // Trả về object đã có ID.
                    }
                    else
                    {
                        Debug.WriteLine($"[WARN] AddVocabulary(object): Không thêm được từ '{vocab.Word}'. ExecuteScalar không trả về ID.");
                        return null;
                    }
                }
            }
            // Bắt lỗi trùng lặp nếu cột Word có UNIQUE constraint
            catch (SqlException sqlEx) when (sqlEx.Number == 2627 || sqlEx.Number == 2601)
            {
                Debug.WriteLine($"[WARN] AddVocabulary(object): Từ '{vocab.Word}' đã tồn tại.");
                // Có thể trả về từ đã tồn tại thay vì null nếu cần
                // return GetVocabularyByWord(vocab.Word);
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi thêm Vocabulary(object) cho '{vocab.Word}': {ex.Message}");
                return null;
            }
            finally
            {
                conn?.Close();
            }
        }

        /// <summary>
        /// Thêm một từ vựng mới (phiên bản tương thích ngược, nhận tham số riêng).
        /// </summary>
        /// <param name="word">Từ vựng.</param>
        /// <param name="meaning">Nghĩa.</param>
        /// <param name="pronunciation">Phát âm.</param>
        /// <param name="audioUrl">URL âm thanh.</param>
        [Obsolete("Nên sử dụng phiên bản AddVocabulary(Vocabulary vocab) để thay thế.")] // Đánh dấu là cũ
        public void AddVocabulary(string word, string meaning, string pronunciation, string audioUrl)
        {
            // Tạo đối tượng Vocabulary và gọi phương thức AddVocabulary(Vocabulary).
            Vocabulary vocab = new Vocabulary
            {
                Word = word,
                Meaning = meaning,
                Pronunciation = pronunciation,
                AudioUrl = audioUrl
            };
            AddVocabulary(vocab); // Gọi phiên bản mới
        }

        /// <summary>
        /// Cập nhật thông tin một từ vựng đã có trong cơ sở dữ liệu.
        /// </summary>
        /// <param name="vocab">Đối tượng Vocabulary chứa thông tin cập nhật (phải có Id hợp lệ).</param>
        /// <returns>True nếu cập nhật thành công (ít nhất 1 dòng bị ảnh hưởng), False nếu không.</returns>
        public bool UpdateVocabulary(Vocabulary vocab)
        {
            // Kiểm tra đầu vào.
            if (vocab == null || vocab.Id <= 0)
            {
                Debug.WriteLine("[WARN] UpdateVocabulary(object): Dữ liệu đầu vào không hợp lệ (null hoặc Id <= 0).");
                return false;
            }
            // Gọi hàm xử lý nội bộ.
            return UpdateVocabularyInternal(vocab.Id, vocab.Word, vocab.Meaning, vocab.Pronunciation, vocab.AudioUrl);
        }

        /// <summary>
        /// Cập nhật thông tin một từ vựng (phiên bản tương thích ngược, nhận tham số riêng).
        /// </summary>
        /// <param name="id">ID của từ cần cập nhật.</param>
        /// <param name="word">Từ mới.</param>
        /// <param name="meaning">Nghĩa mới.</param>
        /// <param name="pronunciation">Phát âm mới.</param>
        /// <param name="audioUrl">URL âm thanh mới.</param>
        [Obsolete("Nên sử dụng phiên bản UpdateVocabulary(Vocabulary vocab) để thay thế.")]
        public void UpdateVocabulary(int id, string word, string meaning, string pronunciation, string audioUrl)
        {
            UpdateVocabularyInternal(id, word, meaning, pronunciation, audioUrl);
        }

        /// <summary>
        /// Xóa một từ vựng khỏi cơ sở dữ liệu và các liên kết của nó.
        /// </summary>
        /// <param name="id">ID của từ vựng cần xóa.</param>
        /// <returns>True nếu xóa thành công (ít nhất 1 dòng bị ảnh hưởng ở bảng Vocabulary), False nếu không.</returns>
        public bool DeleteVocabulary(int id)
        {
            if (id <= 0)
            {
                Debug.WriteLine($"[WARN] DeleteVocabulary: Invalid ID requested: {id}");
                return false;
            }

            int rowsAffected = 0;
            SqlConnection conn = null;
            SqlTransaction transaction = null; // Sử dụng transaction để đảm bảo tính toàn vẹn

            try
            {
                conn = DatabaseContext.GetConnection();
                conn.Open();
                transaction = conn.BeginTransaction(); // Bắt đầu transaction

                // 1. Xóa khỏi bảng FavoriteWords
                string deleteFavQuery = $"DELETE FROM {FavoriteWordsTableName} WHERE {FavColVocabId} = @Id";
                using (SqlCommand cmdFav = new SqlCommand(deleteFavQuery, conn, transaction))
                {
                    cmdFav.Parameters.AddWithValue("@Id", id);
                    cmdFav.ExecuteNonQuery();
                    Debug.WriteLine($"[INFO] DeleteVocabulary: Removed links from FavoriteWords for ID: {id}");
                }

                // 2. Xóa khỏi bảng VocabularyTopic
                string deleteVTQuery = $"DELETE FROM {VocabTopicTableName} WHERE {VTColVocabId} = @Id";
                using (SqlCommand cmdVT = new SqlCommand(deleteVTQuery, conn, transaction))
                {
                    cmdVT.Parameters.AddWithValue("@Id", id);
                    cmdVT.ExecuteNonQuery();
                    Debug.WriteLine($"[INFO] DeleteVocabulary: Removed links from VocabularyTopic for ID: {id}");
                }

                // 3. (Tùy chọn) Xóa khỏi LearningStatuses nếu WordId là INT
                // Nếu WordId trong LearningStatuses là INT, cần thêm lệnh xóa ở đây.
                // Nếu WordId là string, không cần làm gì ở đây.
                // string deleteLSQuery = "DELETE FROM dbo.LearningStatuses WHERE WordId = @Id"; // Giả sử WordId là INT
                // using (SqlCommand cmdLS = new SqlCommand(deleteLSQuery, conn, transaction)) { ... }


                // 4. Xóa khỏi bảng Vocabulary chính
                string deleteVocabQuery = $"DELETE FROM {VocabularyTableName} WHERE {ColId} = @Id";
                using (SqlCommand cmd = new SqlCommand(deleteVocabQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    rowsAffected = cmd.ExecuteNonQuery(); // Lấy số dòng bị ảnh hưởng từ bảng chính
                    Debug.WriteLine($"[INFO] DeleteVocabulary: Executed delete from Vocabulary for ID: {id}. Rows affected: {rowsAffected}");
                }

                transaction.Commit(); // Hoàn tất transaction nếu mọi thứ thành công
                Debug.WriteLine($"[INFO] DeleteVocabulary: Successfully deleted vocabulary and related data for ID: {id}");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi xóa Vocabulary ID {id}: {ex.Message}");
                try
                {
                    transaction?.Rollback(); // Hoàn tác nếu có lỗi
                    Debug.WriteLine($"[INFO] DeleteVocabulary: Transaction rolled back for ID: {id}");
                }
                catch (Exception rbEx)
                {
                    Debug.WriteLine($"[ERROR] Lỗi khi rollback transaction xóa Vocabulary ID {id}: {rbEx.Message}");
                }
                return false;
            }
            finally
            {
                conn?.Close();
            }
            return rowsAffected > 0; // Trả về true nếu từ gốc đã bị xóa
        }

        /// <summary>
        /// Đếm tổng số từ vựng trong cơ sở dữ liệu.
        /// </summary>
        /// <returns>Tổng số từ vựng.</returns>
        public int GetVocabularyCount()
        {
            int count = 0;
            string query = $"SELECT COUNT(*) FROM {VocabularyTableName}";
            SqlConnection conn = null;

            try
            {
                conn = DatabaseContext.GetConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        count = Convert.ToInt32(result);
                    }
                }
                Debug.WriteLine($"[INFO] GetVocabularyCount: Total count is {count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi đếm Vocabulary: {ex.Message}");
                count = 0; // Trả về 0 nếu có lỗi
            }
            finally
            {
                conn?.Close();
            }
            return count;
        }

        #endregion

        #region Topic Operations

        /// <summary>
        /// Lấy danh sách các từ vựng thuộc về một chủ đề cụ thể, sắp xếp theo từ.
        /// </summary>
        /// <param name="topicName">Tên của chủ đề cần lọc.</param>
        /// <returns>Danh sách các đối tượng Vocabulary.</returns>
        public List<Vocabulary> GetVocabularyByTopic(string topicName)
        {
            var vocabularyList = new List<Vocabulary>();
            if (string.IsNullOrWhiteSpace(topicName))
            {
                Debug.WriteLine("[WARN] GetVocabularyByTopic: topicName is null or whitespace.");
                return vocabularyList; // Trả về danh sách rỗng nếu tên chủ đề không hợp lệ
            }

            // Câu lệnh JOIN giữa Vocabulary, VocabularyTopic và Topics.
            string query = $@"
                SELECT V.{ColId}, V.{ColWord}, V.{ColMeaning}, V.{ColPronunciation}, V.{ColAudioUrl}
                FROM {VocabularyTableName} AS V
                INNER JOIN {VocabTopicTableName} AS VT ON V.{ColId} = VT.{VTColVocabId}
                INNER JOIN {TopicTableName} AS T ON VT.{VTColTopicId} = T.{TopicColId}
                WHERE T.{TopicColName} = @TopicName
                ORDER BY V.{ColWord}";

            SqlConnection conn = null;
            try
            {
                conn = DatabaseContext.GetConnection();
                conn.Open();
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    AddParameterWithValue(command, "@TopicName", SqlDbType.NVarChar, 100, topicName);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Vocabulary vocab = MapReaderToVocabulary(reader);
                            if (vocab != null) vocabularyList.Add(vocab);
                        }
                    }
                }
                Debug.WriteLine($"[INFO] GetVocabularyByTopic: Found {vocabularyList.Count} vocabularies for topic '{topicName}'.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi lấy Vocabulary theo chủ đề '{topicName}': {ex.Message}");
                vocabularyList.Clear(); // Xóa danh sách nếu có lỗi
            }
            finally
            {
                conn?.Close();
            }
            return vocabularyList;
        }

        #endregion

        #region Favorite Operations

        /// <summary>
        /// Lấy danh sách tất cả các từ vựng đã được đánh dấu là yêu thích, sắp xếp theo từ.
        /// </summary>
        /// <returns>Danh sách các đối tượng Vocabulary.</returns>
        public List<Vocabulary> GetFavoriteVocabularies()
        {
            var favoriteList = new List<Vocabulary>();
            // Câu lệnh JOIN giữa Vocabulary và FavoriteWords.
            string query = $@"
                SELECT V.{ColId}, V.{ColWord}, V.{ColMeaning}, V.{ColPronunciation}, V.{ColAudioUrl}
                FROM {VocabularyTableName} AS V
                INNER JOIN {FavoriteWordsTableName} AS F ON V.{ColId} = F.{FavColVocabId}
                ORDER BY V.{ColWord}";

            SqlConnection conn = null;
            try
            {
                conn = DatabaseContext.GetConnection();
                conn.Open();
                Debug.WriteLine("[INFO] Executing GetFavoriteVocabularies query.");
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Vocabulary vocab = MapReaderToVocabulary(reader);
                        if (vocab != null) favoriteList.Add(vocab);
                    }
                }
                Debug.WriteLine($"[INFO] GetFavoriteVocabularies: Found {favoriteList.Count} favorites.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi lấy danh sách Favorite Vocabularies: {ex.Message}");
                favoriteList.Clear(); // Xóa danh sách nếu có lỗi
            }
            finally
            {
                conn?.Close();
            }
            return favoriteList;
        }

        /// <summary>
        /// Kiểm tra xem một từ vựng có được đánh dấu là yêu thích hay không.
        /// </summary>
        /// <param name="vocabularyId">ID của từ vựng cần kiểm tra.</param>
        /// <returns>True nếu từ đó là yêu thích, False nếu không hoặc có lỗi.</returns>
        public bool IsFavorite(int vocabularyId)
        {
            if (vocabularyId <= 0) return false;

            // Đếm số bản ghi trong FavoriteWords khớp với ID.
            string query = $"SELECT COUNT(1) FROM {FavoriteWordsTableName} WHERE {FavColVocabId} = @VocabularyId";
            SqlConnection conn = null;
            try
            {
                conn = DatabaseContext.GetConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VocabularyId", vocabularyId); // AddWithValue đơn giản hơn cho kiểu INT
                    object result = cmd.ExecuteScalar();
                    // Trả về true nếu có ít nhất 1 bản ghi (Count > 0).
                    return (result != null && result != DBNull.Value && Convert.ToInt32(result) > 0);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi kiểm tra IsFavorite cho ID {vocabularyId}: {ex.Message}");
                return false; // Trả về false nếu có lỗi.
            }
            finally
            {
                conn?.Close();
            }
        }

        /// <summary>
        /// Đánh dấu một từ vựng là yêu thích.
        /// </summary>
        /// <param name="vocabularyId">ID của từ vựng cần đánh dấu.</param>
        /// <returns>True nếu thêm thành công hoặc từ đã tồn tại, False nếu có lỗi.</returns>
        public bool AddFavorite(int vocabularyId)
        {
            if (vocabularyId <= 0) return false;

            // Câu lệnh INSERT vào bảng FavoriteWords.
            string query = $"INSERT INTO {FavoriteWordsTableName} ({FavColVocabId}) VALUES (@VocabularyId)";
            SqlConnection conn = null;
            try
            {
                conn = DatabaseContext.GetConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VocabularyId", vocabularyId);
                    Debug.WriteLine($"[INFO] Attempting AddFavorite for VocabularyId: {vocabularyId}");
                    cmd.ExecuteNonQuery();
                    Debug.WriteLine($"[INFO] Successfully added favorite for VocabularyId: {vocabularyId}");
                    return true;
                }
            }
            catch (SqlException sqlEx) // Bắt lỗi SQL cụ thể
            {
                // Kiểm tra lỗi trùng khóa chính (Primary Key Violation)
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                {
                    Debug.WriteLine($"[INFO] AddFavorite ignored for ID {vocabularyId}: Already exists.");
                    return true; // Coi như thành công nếu đã tồn tại
                }
                else
                {
                    Debug.WriteLine($"[ERROR] Lỗi SQL khi thêm Favorite cho ID {vocabularyId}: {sqlEx.Message}");
                    return false; // Lỗi SQL khác
                }
            }
            catch (Exception ex) // Bắt các lỗi khác
            {
                Debug.WriteLine($"[ERROR] Lỗi không xác định khi thêm Favorite cho ID {vocabularyId}: {ex.Message}");
                return false;
            }
            finally
            {
                conn?.Close();
            }
        }

        /// <summary>
        /// Xóa một từ vựng khỏi danh sách yêu thích.
        /// </summary>
        /// <param name="vocabularyId">ID của từ vựng cần xóa.</param>
        /// <returns>True nếu xóa thành công (ít nhất 1 dòng bị ảnh hưởng), False nếu không hoặc có lỗi.</returns>
        public bool RemoveFavorite(int vocabularyId)
        {
            if (vocabularyId <= 0) return false;

            // Câu lệnh DELETE khỏi bảng FavoriteWords.
            string query = $"DELETE FROM {FavoriteWordsTableName} WHERE {FavColVocabId} = @VocabularyId";
            int rowsAffected = 0;
            SqlConnection conn = null;
            try
            {
                conn = DatabaseContext.GetConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@VocabularyId", vocabularyId);
                    Debug.WriteLine($"[INFO] Executing RemoveFavorite for VocabularyId: {vocabularyId}");
                    rowsAffected = cmd.ExecuteNonQuery();
                    Debug.WriteLine($"[INFO] Rows affected by RemoveFavorite: {rowsAffected}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi xóa Favorite cho VocabularyId {vocabularyId}: {ex.Message}");
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

        #region Utility / Compatibility Methods

        /// <summary>
        /// Lấy thông tin từ vựng theo ID (Phương thức tương thích, gọi GetVocabularyById).
        /// </summary>
        /// <param name="id">ID của từ vựng.</param>
        /// <returns>Đối tượng Vocabulary hoặc null.</returns>
        [Obsolete("Nên sử dụng GetVocabularyById(int id) trực tiếp.")] // Đánh dấu là cũ
        public Vocabulary GetWordById(int id)
        {
            // Đơn giản là gọi hàm đã có và được chuẩn hóa hơn.
            return GetVocabularyById(id);
        }

        #endregion

        #region Internal Update Helper
        /// <summary>
        /// Hàm nội bộ thực hiện cập nhật thông tin từ vựng.
        /// </summary>
        private bool UpdateVocabularyInternal(int id, string word, string meaning, string pronunciation, string audioUrl)
        {
            // Kiểm tra các tham số cơ bản
            if (id <= 0 || string.IsNullOrWhiteSpace(word) || string.IsNullOrWhiteSpace(meaning))
            {
                Debug.WriteLine($"[WARN] UpdateVocabularyInternal: Invalid input (Id={id}, Word='{word}', Meaning='{meaning}').");
                return false;
            }

            int rowsAffected = 0;
            SqlConnection conn = null;
            // Câu lệnh UPDATE.
            string query = $@"UPDATE {VocabularyTableName} SET
                                {ColWord} = @Word,
                                {ColMeaning} = @Meaning,
                                {ColPronunciation} = @Pronunciation,
                                {ColAudioUrl} = @AudioUrl
                              WHERE {ColId} = @Id";
            try
            {
                conn = DatabaseContext.GetConnection();
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Thêm tham số.
                    AddParameterWithValue(cmd, "@Id", SqlDbType.Int, id); // Sử dụng AddParameterWithValue mới
                    AddParameterWithValue(cmd, "@Word", SqlDbType.NVarChar, 100, word.Trim());
                    AddParameterWithValue(cmd, "@Meaning", SqlDbType.NVarChar, 500, meaning.Trim());
                    AddParameterWithValue(cmd, "@Pronunciation", SqlDbType.NVarChar, 100, pronunciation);
                    AddParameterWithValue(cmd, "@AudioUrl", SqlDbType.NVarChar, 300, audioUrl);

                    rowsAffected = cmd.ExecuteNonQuery(); // Thực thi lệnh UPDATE.
                    Debug.WriteLine($"[INFO] UpdateVocabularyInternal: Executed update for ID {id}. Rows affected: {rowsAffected}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Lỗi khi cập nhật Vocabulary ID {id}: {ex.Message}");
                return false; // Trả về false nếu có lỗi.
            }
            finally
            {
                conn?.Close();
            }
            // Trả về true nếu có ít nhất một dòng đã được cập nhật.
            return rowsAffected > 0;
        }
        #endregion
    }
}