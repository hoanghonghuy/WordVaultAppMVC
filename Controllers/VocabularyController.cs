using System;
using System.Collections.Generic;
using WordVaultAppMVC.Data;
using WordVaultAppMVC.Models;

namespace WordVaultAppMVC.Controllers
{
    /// <summary>
    /// Chịu trách nhiệm xử lý logic liên quan đến việc quản lý từ vựng (Vocabulary).
    /// </summary>
    public class VocabularyController
    {
        #region Fields

        private readonly VocabularyRepository _vocabularyRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo một instance mới của VocabularyController.
        /// </summary>
        public VocabularyController()
        {
            // Khởi tạo repository để tương tác với dữ liệu từ vựng.
            _vocabularyRepository = new VocabularyRepository();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Lấy danh sách tất cả các từ vựng có trong cơ sở dữ liệu.
        /// </summary>
        /// <returns>Một danh sách các đối tượng Vocabulary.</returns>
        public List<Vocabulary> GetAllVocabulary()
        {
            // Gọi repository để lấy tất cả từ vựng.
            return _vocabularyRepository.GetAllVocabulary();
        }

        /// <summary>
        /// Lấy danh sách các từ vựng thuộc về một chủ đề cụ thể.
        /// </summary>
        /// <param name="topicName">Tên của chủ đề cần lọc từ vựng.</param>
        /// <returns>Danh sách các từ vựng thuộc chủ đề đã cho.</returns>
        public List<Vocabulary> GetVocabularyByTopic(string topicName) // Đổi tên tham số thành topicName cho rõ nghĩa
        {
            // Gọi repository để lấy từ vựng theo tên chủ đề.
            return _vocabularyRepository.GetVocabularyByTopic(topicName);
        }

        /// <summary>
        /// Xóa một từ vựng khỏi cơ sở dữ liệu dựa vào ID của nó.
        /// </summary>
        /// <param name="id">ID của từ vựng cần xóa.</param>
        /// <remarks>
        /// Thao tác này sẽ xóa vĩnh viễn từ vựng và các liên kết của nó (ví dụ: khỏi chủ đề, yêu thích).
        /// Cần đảm bảo VocabularyRepository.DeleteVocabulary xử lý các liên kết này.
        /// </remarks>
        public void RemoveVocabulary(int id)
        {
            // Gọi repository để xóa từ vựng theo ID.
            // Repository nên xử lý việc xóa các bản ghi liên quan (ví dụ: trong VocabularyTopic, FavoriteWords).
            _vocabularyRepository.DeleteVocabulary(id);
        }

        /// <summary>
        /// Thêm một từ vựng mới vào cơ sở dữ liệu.
        /// </summary>
        /// <param name="word">Từ vựng (không được để trống).</param>
        /// <param name="meaning">Nghĩa của từ.</param>
        /// <param name="pronunciation">Phiên âm (có thể null hoặc rỗng).</param>
        /// <param name="audioUrl">URL file âm thanh (có thể null hoặc rỗng).</param>
        /// <exception cref="ArgumentException">Ném ra nếu 'word' là null, rỗng hoặc chỉ chứa khoảng trắng.</exception>
        /// <remarks>
        /// Phiên bản này của AddVocabulary có 4 tham số riêng biệt.
        /// VocabularyRepository cũng có một phiên bản nhận vào đối tượng Vocabulary.
        /// </remarks>
        public void AddVocabulary(string word, string meaning, string pronunciation, string audioUrl)
        {
            // Kiểm tra tính hợp lệ của từ vựng trước khi thêm.
            if (string.IsNullOrWhiteSpace(word))
            {
                // Meaning cũng nên được kiểm tra tương tự nếu bắt buộc
                // if (string.IsNullOrWhiteSpace(meaning))
                // {
                //     throw new ArgumentException("Nghĩa không được để trống.", nameof(meaning));
                // }
                throw new ArgumentException("Từ vựng không được để trống.", nameof(word));
            }

            // Gọi repository để thêm từ vựng mới.
            _vocabularyRepository.AddVocabulary(word, meaning, pronunciation, audioUrl);
        }

        #endregion
    }
}