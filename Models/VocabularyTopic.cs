namespace WordVaultAppMVC.Models
{
    /// <summary>
    /// Đại diện cho một bản ghi liên kết giữa một Từ vựng (Vocabulary) và một Chủ đề (Topic).
    /// Đây là lớp mô hình cho bảng trung gian trong mối quan hệ nhiều-nhiều.
    /// </summary>
    public class VocabularyTopic
    {
        #region Properties

        /// <summary>
        /// ID của từ vựng liên quan.
        /// Tham chiếu đến Vocabulary.Id.
        /// </summary>
        public int VocabularyId { get; set; }

        /// <summary>
        /// ID của chủ đề liên quan.
        /// Tham chiếu đến Topics.Id.
        /// </summary>
        public int TopicId { get; set; }

        #endregion

        // Lớp này thường không cần constructor phức tạp vì nó chỉ chứa các khóa ngoại.
        // Constructor mặc định là đủ.
    }
}