namespace WordVaultAppMVC.Models
{
    /// <summary>
    /// Đại diện cho một chủ đề từ vựng.
    /// </summary>
    public class Topic
    {
        #region Properties

        /// <summary>
        /// ID định danh duy nhất cho chủ đề.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Tên của chủ đề từ vựng.
        /// </summary>
        public string Name { get; set; }

        #endregion

        // Constructor mặc định (không tham số) được tạo tự động nếu không định nghĩa constructor nào khác.
        // Có thể thêm constructor nếu cần:
        // public Topic(string name)
        // {
        //     Name = name;
        // }
    }
}