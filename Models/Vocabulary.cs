namespace WordVaultAppMVC.Models
{
    /// <summary>
    /// Đại diện cho một mục từ vựng trong từ điển cá nhân.
    /// </summary>
    public class Vocabulary
    {
        #region Properties

        /// <summary>
        /// ID định danh duy nhất cho từ vựng trong cơ sở dữ liệu.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Từ vựng tiếng Anh.
        /// </summary>
        public string Word { get; set; }

        /// <summary>
        /// Nghĩa của từ vựng (thường là tiếng Việt).
        /// </summary>
        public string Meaning { get; set; }

        /// <summary>
        /// Phiên âm của từ vựng.
        /// Ví dụ: /ˈɛɡzæmpəl/
        /// </summary>
        public string Pronunciation { get; set; }

        /// <summary>
        /// URL (đường dẫn) đến file âm thanh phát âm của từ vựng.
        /// Có thể là null hoặc rỗng nếu không có âm thanh.
        /// </summary>
        public string AudioUrl { get; set; }

        #endregion

        // Constructor mặc định (không tham số) được tạo tự động.
        // Có thể thêm constructor nếu cần khởi tạo bắt buộc:
        // public Vocabulary(string word, string meaning)
        // {
        //     // Thêm kiểm tra null/empty nếu cần
        //     Word = word;
        //     Meaning = meaning;
        // }
    }
}