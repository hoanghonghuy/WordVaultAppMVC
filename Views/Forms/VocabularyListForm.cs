using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WordVaultAppMVC.Data;
using WordVaultAppMVC.Models;
using WordVaultAppMVC.Views; // Đảm bảo rằng VocabularyDetailPanel nằm trong namespace này

namespace WordVaultAppMVC.Views
{
    public partial class VocabularyListForm : Form
    {
        private VocabularyRepository _vocabRepo = new VocabularyRepository();
        private List<Vocabulary> _vocabList;

        public VocabularyListForm()
        {
            InitializeComponent();
            LoadVocabulary();
        }

        // Tải danh sách từ vựng từ cơ sở dữ liệu
        private void LoadVocabulary()
        {
            _vocabList = _vocabRepo.GetAllVocabulary();
            listBoxVocabulary.Items.Clear();

            foreach (var vocab in _vocabList)
            {
                listBoxVocabulary.Items.Add(vocab.Word);
            }

            if (_vocabList.Count > 0)
            {
                listBoxVocabulary.SelectedIndex = 0;
            }
            UpdateTotalLabel();
        }

        // Cập nhật label hiển thị tổng số từ vựng
        private void UpdateTotalLabel()
        {
            lblTotalVocabulary.Text = "Tổng số từ: " + _vocabList.Count;
        }

        // Khi người dùng chọn một từ trong ListBox, hiển thị chi tiết bằng VocabularyDetailPanel
        private void listBoxVocabulary_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBoxVocabulary.SelectedIndex;
            if (index >= 0 && index < _vocabList.Count)
            {
                vocabularyDetailPanel.DisplayVocabulary(_vocabList[index]);
            }
        }

        // Nút làm mới danh sách từ vựng
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadVocabulary();
        }

        // Nút xóa từ vựng đã chọn
        private void btnDelete_Click(object sender, EventArgs e)
        {
            int index = listBoxVocabulary.SelectedIndex;
            if (index < 0)
            {
                MessageBox.Show("Vui lòng chọn từ vựng cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var vocab = _vocabList[index];
            var confirm = MessageBox.Show($"Bạn có chắc muốn xóa từ \"{vocab.Word}\"?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                _vocabRepo.DeleteVocabulary(vocab.Id);
                MessageBox.Show("Xóa từ vựng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadVocabulary();
            }
        }
    }
}
