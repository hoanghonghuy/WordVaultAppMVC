using System.Collections.Generic; // Cần cho List<string>

namespace WordVaultAppMVC.Models
{
    /// <summary>
    /// Đại diện cho thông tin chi tiết của một từ, thường được lấy từ API bên ngoài.
    /// Lớp này có thể chứa nhiều thông tin hơn lớp Vocabulary cơ bản.
    /// </summary>
    public class WordDetails
    {
        #region Properties

        /// <summary>
        /// Từ vựng gốc.
        /// </summary>
        public string Word { get; set; }

        /// <summary>
        /// Phiên âm của từ.
        /// </summary>
        public string Pronunciation { get; set; }

        /// <summary>
        /// URL đến file âm thanh phát âm.
        /// </summary>
        public string AudioUrl { get; set; }

        /// <summary>
        /// Nghĩa chính hoặc nghĩa đầu tiên tìm thấy của từ.
        /// </summary>
        public string Meaning { get; set; }

        // --- Các thuộc tính mở rộng (Optional) ---
        // Có thể được sử dụng nếu API trả về nhiều thông tin hơn
        // hoặc nếu muốn hiển thị chi tiết hơn trong tương lai.

        /// <summary>
        /// Danh sách tất cả các nghĩa tìm thấy (nếu có).
        /// Được khởi tạo để tránh lỗi null.
        /// </summary>
        public List<string> AllMeanings { get; set; } = new List<string>();

        /// <summary>
        /// Loại từ (ví dụ: "noun", "verb", "adjective").
        /// </summary>
        public string PartOfSpeech { get; set; }

        /// <summary>
        /// Danh sách các câu ví dụ sử dụng từ (nếu có).
        /// Được khởi tạo để tránh lỗi null.
        /// </summary>
        public List<string> ExampleSentences { get; set; } = new List<string>();

        #endregion

        // Constructor mặc định.
        // Có thể thêm constructor nếu cần.
    }
}