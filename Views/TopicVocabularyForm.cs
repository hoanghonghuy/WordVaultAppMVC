using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WordVaultAppMVC.Controllers;
using WordVaultAppMVC.Models;

namespace WordVaultAppMVC.Views
{
    public partial class TopicVocabularyForm : Form
    {
        private readonly TopicController topicController;
        private readonly VocabularyController vocabularyController;
        private string currentTopic; // Tên chủ đề hiện tại
        private List<Vocabulary> vocabularyList;
        public TopicVocabularyForm(string topicName)
        {
            InitializeComponent();
            currentTopic = topicName;
            lblTopicTitle.Text = "Chủ đề: " + currentTopic;
            topicController = new TopicController();
            vocabularyController = new VocabularyController();
            LoadVocabulary();
        }

        // Tải danh sách từ vựng thuộc chủ đề hiện tại (giả sử lấy từ cơ sở dữ liệu)
        private void LoadVocabulary()
        {
            // Giả sử VocabularyController có phương thức lấy từ vựng theo chủ đề
            vocabularyList = vocabularyController.GetVocabularyByTopic(currentTopic);
            lstVocabulary.Items.Clear();
            if (vocabularyList != null)
            {
                foreach (var vocab in vocabularyList)
                {
                    lstVocabulary.Items.Add(vocab.Word + " - " + vocab.Meaning);
                }
                lblTotalVocabulary.Text = "Tổng số từ: " + vocabularyList.Count;
            }
            else
            {
                lblTotalVocabulary.Text = "Tổng số từ: 0";
            }
        }

        // Sự kiện khi nhấn nút "Thêm từ"
        private void btnAddVocabulary_Click(object sender, EventArgs e)
        {
            string newVocab = txtNewVocabulary.Text.Trim();
            if (string.IsNullOrEmpty(newVocab))
            {
                MessageBox.Show("Vui lòng nhập từ vựng cần thêm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Giả sử thêm từ vựng mới vào cơ sở dữ liệu và cập nhật danh sách
            // Bạn có thể mở rộng để nhập cả nghĩa, phát âm, URL audio, ...
            Vocabulary vocab = new Vocabulary
            {
                Word = newVocab,
                Meaning = "Chưa có nghĩa", // Tạm thời
                Pronunciation = "",
                AudioUrl = ""
            };

            vocabularyController.AddVocabulary(vocab.Word, vocab.Meaning, vocab.Pronunciation, vocab.AudioUrl);
            MessageBox.Show("Thêm từ vựng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtNewVocabulary.Clear();
            LoadVocabulary();
        }

        // Sự kiện khi nhấn nút "Xoá từ đã chọn"
        private void btnRemoveVocabulary_Click(object sender, EventArgs e)
        {
            int selectedIndex = lstVocabulary.SelectedIndex;
            if (selectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn từ vựng cần xoá.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lấy đối tượng Vocabulary dựa trên chỉ số được chọn
            var vocab = vocabularyList[selectedIndex];
            var confirm = MessageBox.Show($"Bạn có chắc muốn xóa từ \"{vocab.Word}\"?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                vocabularyController.RemoveVocabulary(vocab.Id);
                MessageBox.Show("Xóa từ vựng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadVocabulary();
            }
        }

        private void TopicVocabularyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //// Xử lý khi form đóng
            //var result = MessageBox.Show("Bạn có chắc chắn muốn thoát không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //if (result == DialogResult.No)
            //{
            //    e.Cancel = true; // Hủy thao tác đóng form
            //}
            //else
            //{
            //    Application.Exit(); // Thoát chương trình
            //}
        }
    }
}
