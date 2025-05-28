using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;
using WordVaultAppMVC.Helpers;
using WordVaultAppMVC.Models;
using WordVaultAppMVC.Data;

namespace WordVaultAppMVC.Views.Controls
{
    public class HomeControl : UserControl
    {
        private TextBox txtSearch;
        private Label lblPronunciation;
        private Label lblMeaning;
        private Button btnSearch;
        private Button btnPlayAudio;
        private Button btnAddFavorite;
        private Button btnAddTopic;
        private WordDetails currentWord;

        public HomeControl()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Dock = DockStyle.Fill;

            var pnlSearch = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(20, 10, 20, 10) };
            var pnlResult = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            txtSearch = new TextBox
            {
                Font = new System.Drawing.Font("Segoe UI", 11F),
                Size = new System.Drawing.Size(300, 37),
                Location = new System.Drawing.Point(0, 15)
            };
            txtSearch.KeyDown += TxtSearch_KeyDown;

            btnSearch = new Button
            {
                Text = "Tìm kiếm",
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Size = new System.Drawing.Size(106, 37),
                Location = new System.Drawing.Point(320, 15)
            };
            btnSearch.Click += async (s, e) => await SearchAndDisplayWordAsync();

            lblPronunciation = new Label
            {
                Text = "Phát âm:",
                Font = new System.Drawing.Font("Segoe UI", 11F),
                Location = new System.Drawing.Point(0, 10),
                Size = new System.Drawing.Size(600, 25)
            };

            lblMeaning = new Label
            {
                Text = "Nghĩa tiếng Việt:",
                Font = new System.Drawing.Font("Segoe UI", 11F),
                Location = new System.Drawing.Point(0, 50),
                Size = new System.Drawing.Size(600, 80)
            };

            btnPlayAudio = new Button
            {
                Text = "🔊 Nghe phát âm",
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Size = new System.Drawing.Size(140, 45),
                Location = new System.Drawing.Point(5, 140)
            };
            btnPlayAudio.Click += BtnPlayAudio_Click;

            btnAddFavorite = new Button
            {
                Text = "⭐ Yêu thích",
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Size = new System.Drawing.Size(100, 45),
                Location = new System.Drawing.Point(160, 140)
            };
            btnAddFavorite.Click += BtnAddFavorite_Click;

            btnAddTopic = new Button
            {
                Text = "📚 Thêm vào chủ đề",
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Size = new System.Drawing.Size(160, 45),
                Location = new System.Drawing.Point(270, 140)
            };
            btnAddTopic.Click += BtnAddTopic_Click;

            pnlSearch.Controls.Add(txtSearch);
            pnlSearch.Controls.Add(btnSearch);
            pnlResult.Controls.Add(lblPronunciation);
            pnlResult.Controls.Add(lblMeaning);
            pnlResult.Controls.Add(btnPlayAudio);
            pnlResult.Controls.Add(btnAddFavorite);
            pnlResult.Controls.Add(btnAddTopic);

            this.Controls.Add(pnlResult);
            this.Controls.Add(pnlSearch);
        }

        private async void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                await SearchAndDisplayWordAsync();
            }
        }

        private async Task SearchAndDisplayWordAsync()
        {
            string searchTerm = txtSearch.Text.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                MessageBox.Show("Vui lòng nhập từ cần tìm.");
                return;
            }

            var result = await DictionaryApiClient.GetWordDetailsAsync(searchTerm);

            if (result != null)
            {
                result.Meaning = await DictionaryApiClient.TranslateToVietnamese(result.Meaning);
                lblPronunciation.Text = "Phát âm: " + result.Pronunciation;
                lblMeaning.Text = "Nghĩa tiếng Việt: " + result.Meaning;
                currentWord = result;
                SaveWordToDatabase(result);
            }
            else
            {
                MessageBox.Show("Không tìm thấy từ này trong từ điển.");
            }
        }

        private void SaveWordToDatabase(WordDetails wordDetails)
        {
            using (var connection = DatabaseContext.GetConnection())
            {
                connection.Open();
                var checkQuery = "SELECT COUNT(*) FROM dbo.Vocabulary WHERE Word = @Word";
                using (var checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@Word", wordDetails.Word);
                    int count = (int)checkCommand.ExecuteScalar();
                    if (count > 0) return;
                }

                var insertQuery = "INSERT INTO dbo.Vocabulary (Word, Pronunciation, AudioUrl, Meaning) VALUES (@Word, @Pronunciation, @AudioUrl, @Meaning)";
                using (var insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@Word", wordDetails.Word);
                    insertCommand.Parameters.AddWithValue("@Pronunciation", wordDetails.Pronunciation);
                    insertCommand.Parameters.AddWithValue("@AudioUrl", wordDetails.AudioUrl);
                    insertCommand.Parameters.AddWithValue("@Meaning", wordDetails.Meaning);
                    insertCommand.ExecuteNonQuery();
                }
            }
        }

        private void BtnPlayAudio_Click(object sender, EventArgs e)
        {
            string audioUrl = GetAudioUrlFromDb();
            if (!string.IsNullOrEmpty(audioUrl))
            {
                AudioHelper.PlayAudio(audioUrl);
            }
            else
            {
                MessageBox.Show("Không có âm thanh để phát.");
            }
        }

        private string GetAudioUrlFromDb()
        {
            string audioUrl = "";
            using (var connection = DatabaseContext.GetConnection())
            {
                connection.Open();
                var query = "SELECT AudioUrl FROM Vocabulary WHERE Word = @Word";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Word", txtSearch.Text.Trim());
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        audioUrl = result.ToString();
                    }
                }
            }
            return audioUrl;
        }

        private void BtnAddFavorite_Click(object sender, EventArgs e)
        {
            if (currentWord == null)
            {
                MessageBox.Show("Hãy tìm kiếm từ trước khi thêm vào Yêu thích.");
                return;
            }
            using (var conn = DatabaseContext.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("INSERT INTO FavoriteWords (WordId) SELECT Id FROM Vocabulary WHERE Word = @Word", conn);
                cmd.Parameters.AddWithValue("@Word", currentWord.Word);
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("⭐ Đã thêm vào Yêu thích!");
                }
                catch
                {
                    MessageBox.Show("Từ này đã có trong danh sách yêu thích.");
                }
            }
        }

        private void BtnAddTopic_Click(object sender, EventArgs e)
        {
            if (currentWord == null)
            {
                MessageBox.Show("Hãy tìm kiếm từ trước khi thêm vào chủ đề.");
                return;
            }
            var form = new AddToTopicForm(currentWord.Word);
            form.ShowDialog();
        }
    }
}
