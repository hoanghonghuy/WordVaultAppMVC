using System;
using System.Collections.Generic;
using WordVaultAppMVC.Data;
using WordVaultAppMVC.Models;

namespace WordVaultAppMVC.Controllers
{
    /// <summary>
    /// Chịu trách nhiệm xử lý logic liên quan đến các chủ đề từ vựng.
    /// </summary>
    public class TopicController
    {
        #region Fields

        private readonly TopicRepository _topicRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo một instance mới của TopicController.
        /// </summary>
        public TopicController()
        {
            // Khởi tạo repository để tương tác với dữ liệu chủ đề.
            _topicRepository = new TopicRepository();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Lấy danh sách tất cả các chủ đề từ cơ sở dữ liệu.
        /// </summary>
        /// <returns>Một danh sách các đối tượng Topic.</returns>
        public List<Topic> GetTopics()
        {
            // Gọi repository để lấy tất cả chủ đề.
            return _topicRepository.GetAllTopics();
        }

        /// <summary>
        /// Thêm một chủ đề mới vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="topicName">Tên của chủ đề mới cần thêm.</param>
        /// <exception cref="ArgumentException">Ném ra nếu tên chủ đề là null, rỗng hoặc chỉ chứa khoảng trắng.</exception>
        public void AddTopic(string topicName)
        {
            // Kiểm tra tính hợp lệ của tên chủ đề trước khi thêm.
            if (string.IsNullOrWhiteSpace(topicName))
            {
                throw new ArgumentException("Tên chủ đề không được để trống.", nameof(topicName)); // Thêm nameof để chỉ rõ tham số gây lỗi
            }

            // Gọi repository để thêm chủ đề mới.
            _topicRepository.AddTopic(topicName);
        }

        #endregion
    }
}