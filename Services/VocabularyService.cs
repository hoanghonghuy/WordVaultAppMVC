using System;
using System.Collections.Generic;
using System.Diagnostics; // Thêm using cho Debug
using System.Linq; // Thêm using cho .Any()
using WordVaultAppMVC.Data; // Namespace của VocabularyRepository
using WordVaultAppMVC.Models; // Namespace của Vocabulary model

namespace WordVaultAppMVC.Services
{
    /// <summary>
    /// Cung cấp các dịch vụ liên quan đến logic nghiệp vụ của Từ vựng (Vocabulary).
    /// Ví dụ: lấy từ ngẫu nhiên, xử lý logic phức tạp hơn liên quan đến từ vựng.
    /// </summary>
    public class VocabularyService
    {
        #region Fields

        private readonly VocabularyRepository _vocabularyRepository;

        // Khởi tạo Random một lần dưới dạng static readonly để đảm bảo tính ngẫu nhiên tốt hơn
        // và tránh các giá trị giống nhau nếu hàm GetRandomWord được gọi liên tục trong thời gian ngắn.
        private static readonly Random rnd = new Random();

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo một instance mới của VocabularyService.
        /// </summary>
        public VocabularyService()
        {
            // Khởi tạo Repository khi Service được tạo.
            // Cân nhắc sử dụng Dependency Injection trong các dự án lớn hơn.
            _vocabularyRepository = new VocabularyRepository();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Lấy một đối tượng Vocabulary ngẫu nhiên từ cơ sở dữ liệu.
        /// </summary>
        /// <returns>Một đối tượng Vocabulary ngẫu nhiên, hoặc null nếu không có từ nào hoặc có lỗi xảy ra.</returns>
        /// <remarks>
        /// **Lưu ý về hiệu năng:** Phương thức này hiện tại tải *tất cả* từ vựng vào bộ nhớ
        /// (`GetAllVocabulary`) rồi mới chọn ngẫu nhiên. Điều này có thể không hiệu quả
        /// nếu cơ sở dữ liệu có số lượng từ vựng rất lớn.
        /// Một cách tối ưu hơn có thể là lấy tổng số từ, tạo ID ngẫu nhiên trong phạm vi đó,
        /// và chỉ lấy một bản ghi từ CSDL, tuy nhiên sẽ phức tạp hơn trong việc xử lý ID bị xóa.
        /// Hoặc sử dụng các kỹ thuật như `TABLESAMPLE` của SQL Server nếu chấp nhận tính ngẫu nhiên gần đúng.
        /// </remarks>
        public Vocabulary GetRandomWord()
        {
            List<Vocabulary> vocabularies = null;
            try
            {
                // Gọi repository để lấy danh sách tất cả từ vựng.
                vocabularies = _vocabularyRepository.GetAllVocabulary();
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu không thể lấy dữ liệu từ repository.
                Debug.WriteLine($"[ERROR] GetRandomWord: Lỗi khi gọi GetAllVocabulary: {ex.Message}");
                // Trả về null để báo hiệu lỗi cho nơi gọi.
                return null;
            }

            // Kiểm tra xem danh sách có hợp lệ và có chứa phần tử không.
            if (vocabularies == null || !vocabularies.Any()) // Dùng Linq.Any() cho ngắn gọn và hiệu quả.
            {
                Debug.WriteLine("[WARN] GetRandomWord: Không tìm thấy từ vựng nào hoặc danh sách trả về là null.");
                return null; // Trả về null nếu không có từ vựng.
            }

            // Lấy một index ngẫu nhiên trong phạm vi của danh sách.
            int randomIndex = rnd.Next(vocabularies.Count);

            // Trả về đối tượng Vocabulary tại index ngẫu nhiên đó.
            Vocabulary randomWord = vocabularies[randomIndex];
            Debug.WriteLine($"[INFO] GetRandomWord: Returning random word: '{randomWord.Word}' (ID: {randomWord.Id})");
            return randomWord;
        }

        /// <summary>
        /// Lấy nghĩa của một từ vựng dựa trên ID của nó (dưới dạng chuỗi).
        /// </summary>
        /// <param name="wordId">ID của từ vựng (dạng chuỗi).</param>
        /// <returns>Nghĩa của từ nếu tìm thấy, ngược lại trả về một chuỗi thông báo lỗi hoặc không tìm thấy.</returns>
        /// <remarks>
        /// Phương thức này có thể hơi thừa vì logic tương tự có thể thực hiện trực tiếp
        /// tại nơi gọi bằng cách lấy Vocabulary object rồi truy cập thuộc tính Meaning.
        /// Tuy nhiên, giữ lại để tương thích với code hiện có.
        /// Cân nhắc chấp nhận tham số kiểu `int id` để đảm bảo kiểu dữ liệu.
        /// </remarks>
        public string GetWordMeaning(string wordId)
        {
            // Cố gắng chuyển đổi wordId (string) sang id (int).
            if (int.TryParse(wordId, out int id))
            {
                try
                {
                    // Gọi repository để lấy đối tượng Vocabulary theo Id số nguyên.
                    Vocabulary vocab = _vocabularyRepository.GetVocabularyById(id); // Đảm bảo dùng phương thức đúng của Repo

                    // Kiểm tra kết quả từ repository.
                    if (vocab != null)
                    {
                        // Trả về nghĩa, nếu nghĩa là null hoặc rỗng, trả về thông báo tương ứng.
                        return string.IsNullOrEmpty(vocab.Meaning)
                            ? $"Từ '{vocab.Word}' tồn tại nhưng không có nghĩa được lưu."
                            : vocab.Meaning;
                    }
                    else
                    {
                        // Không tìm thấy từ với ID đã cho.
                        return $"Không tìm thấy từ vựng với ID: {id}";
                    }
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi nếu có vấn đề khi truy cập repository.
                    Debug.WriteLine($"[ERROR] GetWordMeaning: Lỗi khi gọi GetVocabularyById cho ID (string) '{wordId}': {ex.Message}");
                    return "Lỗi hệ thống khi truy vấn nghĩa!"; // Trả về thông báo lỗi chung.
                }
            }
            else
            {
                // Trả về thông báo nếu wordId không phải là số hợp lệ.
                return "ID từ cung cấp không hợp lệ!";
            }
        }

        #endregion

        // Có thể thêm các phương thức nghiệp vụ khác tại đây, ví dụ:
        // - Lấy từ ngẫu nhiên nhưng ưu tiên từ "Đang học" hoặc "Chưa học".
        // - Phân tích thống kê học tập.
        // - Gợi ý từ cần ôn tập dựa trên thuật toán Spaced Repetition.
    }
}