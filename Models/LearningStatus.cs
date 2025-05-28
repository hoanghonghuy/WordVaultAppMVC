using System;

namespace WordVaultAppMVC.Models
{
    /// <summary>
    /// Đại diện cho trạng thái học tập của một từ vựng cụ thể,
    /// có thể liên kết với một người dùng (tùy chọn).
    /// </summary>
    public class LearningStatus
    {
        #region Properties

        /// <summary>
        /// ID định danh duy nhất cho bản ghi trạng thái học tập.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID của từ vựng liên quan.
        /// Lưu ý: Hiện tại đang là kiểu string (NVARCHAR trong DB),
        /// cân nhắc đổi sang int nếu ID từ vựng trong bảng Vocabulary là int
        /// để đảm bảo tính nhất quán và có thể tạo Foreign Key.
        /// </summary>
        public string WordId { get; set; }

        /// <summary>
        /// ID của người dùng liên quan (nếu có).
        /// Có thể là null hoặc rỗng nếu không áp dụng hệ thống người dùng.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Trạng thái học tập hiện tại của từ vựng.
        /// Ví dụ: "Chưa học", "Đang học", "Đã học".
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Ngày giờ mà trạng thái này được cập nhật hoặc từ vựng được học/ôn tập lần cuối.
        /// </summary>
        public DateTime DateLearned { get; set; }

        #endregion

        // Có thể thêm Constructor nếu cần khởi tạo giá trị mặc định
        // public LearningStatus()
        // {
        //     DateLearned = DateTime.Now;
        //     Status = "Chưa học"; // Ví dụ
        // }
    }
}