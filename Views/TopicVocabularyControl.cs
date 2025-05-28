using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using WordVaultAppMVC.Data;
using WordVaultAppMVC.Models;

namespace WordVaultAppMVC.Views.Controls
{
    public class TopicVocabularyControl : UserControl
    {
        private ComboBox cboTopics;
        private ListView lstWords;
        private TextBox txtNewWord;
        private Button btnAdd;

        public TopicVocabularyControl(string defaultTopic = null)
        {
            InitializeComponent();
            LoadTopics();

            if (!string.IsNullOrEmpty(defaultTopic))
                cboTopics.SelectedItem = defaultTopic;
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;

            cboTopics = new ComboBox { Location = new System.Drawing.Point(20, 20), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cboTopics.SelectedIndexChanged += (s, e) => LoadWordsByTopic();

            txtNewWord = new TextBox { Location = new System.Drawing.Point(240, 20), Width = 200 };
            btnAdd = new Button { Text = "➕ Thêm từ", Location = new System.Drawing.Point(460, 18), Width = 100 };
            btnAdd.Click += BtnAdd_Click;

            lstWords = new ListView
            {
                View = View.Details,
                Location = new System.Drawing.Point(20, 70),
                Size = new System.Drawing.Size(540, 300),
                FullRowSelect = true
            };
            lstWords.Columns.Add("Từ vựng", 160);
            lstWords.Columns.Add("Phát âm", 150);
            lstWords.Columns.Add("Nghĩa tiếng Việt", 220);

            this.Controls.AddRange(new Control[] { cboTopics, txtNewWord, btnAdd, lstWords });
        }

        private void LoadTopics()
        {
            cboTopics.Items.Clear();
            using (var conn = DatabaseContext.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT Name FROM Topics", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cboTopics.Items.Add(reader.GetString(0));
                }
            }
            if (cboTopics.Items.Count > 0) cboTopics.SelectedIndex = 0;
        }

        private void LoadWordsByTopic()
        {
            lstWords.Items.Clear();
            string topic = cboTopics.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(topic)) return;

            using (var conn = DatabaseContext.GetConnection())
            {
                conn.Open();
                var sql = @"SELECT V.Word, V.Pronunciation, V.Meaning
                             FROM Vocabulary V
                             JOIN VocabularyTopic VT ON V.Id = VT.VocabularyId
                             JOIN Topics T ON T.Id = VT.TopicId
                             WHERE T.Name = @Topic";
                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Topic", topic);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var item = new ListViewItem(reader.GetString(0));
                    item.SubItems.Add(reader.IsDBNull(1) ? "" : reader.GetString(1));
                    item.SubItems.Add(reader.IsDBNull(2) ? "" : reader.GetString(2));
                    lstWords.Items.Add(item);
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string word = txtNewWord.Text.Trim();
            string topic = cboTopics.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(word) || string.IsNullOrEmpty(topic))
            {
                MessageBox.Show("Vui lòng nhập từ và chọn chủ đề.");
                return;
            }

            int wordId = -1;
            int topicId = -1;

            using (var conn = DatabaseContext.GetConnection())
            {
                conn.Open();
                // Lấy Id của từ (nếu có)
                var cmd = new SqlCommand("SELECT Id FROM Vocabulary WHERE Word = @Word", conn);
                cmd.Parameters.AddWithValue("@Word", word);
                var result = cmd.ExecuteScalar();
                if (result == null)
                {
                    MessageBox.Show("Từ chưa có trong cơ sở dữ liệu. Hãy tìm kiếm trước ở Home.");
                    return;
                }
                wordId = (int)result;

                cmd = new SqlCommand("SELECT Id FROM Topics WHERE Name = @Topic", conn);
                cmd.Parameters.AddWithValue("@Topic", topic);
                topicId = (int)(cmd.ExecuteScalar() ?? -1);

                if (wordId == -1 || topicId == -1) return;

                cmd = new SqlCommand("SELECT COUNT(*) FROM VocabularyTopic WHERE VocabularyId = @VId AND TopicId = @TId", conn);
                cmd.Parameters.AddWithValue("@VId", wordId);
                cmd.Parameters.AddWithValue("@TId", topicId);
                int exists = (int)cmd.ExecuteScalar();
                if (exists > 0)
                {
                    MessageBox.Show("Từ đã nằm trong chủ đề này.");
                    return;
                }

                cmd = new SqlCommand("INSERT INTO VocabularyTopic (VocabularyId, TopicId) VALUES (@VId, @TId)", conn);
                cmd.Parameters.AddWithValue("@VId", wordId);
                cmd.Parameters.AddWithValue("@TId", topicId);
                cmd.ExecuteNonQuery();
            }

            txtNewWord.Clear();
            LoadWordsByTopic();
        }
    }
}