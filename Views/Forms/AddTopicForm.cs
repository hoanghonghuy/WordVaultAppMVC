// AddToTopicForm.cs (có thêm tạo topic mới)
using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using WordVaultAppMVC.Data;

namespace WordVaultAppMVC.Views
{
    public partial class AddToTopicForm : Form
    {
        private readonly string word;

        public AddToTopicForm(string word)
        {
            InitializeComponent();
            this.word = word;
            lblWord.Text = $"Từ: {word}";
            LoadTopics();
        }

        private void LoadTopics()
        {
            cboTopics.Items.Clear();
            using (var conn = DatabaseContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT Id, Name FROM Topics", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cboTopics.Items.Add(new ComboBoxItem(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }
            if (cboTopics.Items.Count > 0)
                cboTopics.SelectedIndex = 0;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cboTopics.SelectedItem is ComboBoxItem selectedTopic)
            {
                AddWordToTopic(selectedTopic.Id);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một chủ đề.");
            }
        }

        private void AddWordToTopic(int topicId)
        {
            using (var conn = DatabaseContext.GetConnection())
            {
                conn.Open();

                var getWordIdCmd = new SqlCommand("SELECT Id FROM Vocabulary WHERE Word = @Word", conn);
                getWordIdCmd.Parameters.AddWithValue("@Word", word);
                var wordIdObj = getWordIdCmd.ExecuteScalar();

                if (wordIdObj == null)
                {
                    MessageBox.Show("Không tìm thấy từ này trong cơ sở dữ liệu.");
                    return;
                }

                int wordId = Convert.ToInt32(wordIdObj);

                var insertCmd = new SqlCommand("INSERT INTO VocabularyTopic (VocabularyId, TopicId) VALUES (@WordId, @TopicId)", conn);
                insertCmd.Parameters.AddWithValue("@WordId", wordId);
                insertCmd.Parameters.AddWithValue("@TopicId", topicId);

                try
                {
                    insertCmd.ExecuteNonQuery();
                    MessageBox.Show("📚 Đã thêm vào chủ đề thành công!");
                    this.Close();
                }
                catch (SqlException)
                {
                    MessageBox.Show("Từ này đã có trong chủ đề này rồi.");
                }
            }
        }

        private void btnCreateTopic_Click(object sender, EventArgs e)
        {
            string newTopic = txtNewTopic.Text.Trim();
            if (string.IsNullOrEmpty(newTopic))
            {
                MessageBox.Show("Vui lòng nhập tên chủ đề mới.");
                return;
            }

            using (var conn = DatabaseContext.GetConnection())
            {
                conn.Open();
                var checkCmd = new SqlCommand("SELECT COUNT(*) FROM Topics WHERE Name = @Name", conn);
                checkCmd.Parameters.AddWithValue("@Name", newTopic);
                int count = (int)checkCmd.ExecuteScalar();
                if (count > 0)
                {
                    MessageBox.Show("Chủ đề này đã tồn tại.");
                    return;
                }

                var insertCmd = new SqlCommand("INSERT INTO Topics (Name) VALUES (@Name)", conn);
                insertCmd.Parameters.AddWithValue("@Name", newTopic);
                insertCmd.ExecuteNonQuery();
            }

            LoadTopics();
            cboTopics.SelectedIndex = cboTopics.FindStringExact(newTopic);
            txtNewTopic.Clear();
            MessageBox.Show("✅ Đã tạo chủ đề mới.");
        }
    }

    public class ComboBoxItem
    {
        public int Id { get; }
        public string Name { get; }

        public ComboBoxItem(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => Name;
    }
}
