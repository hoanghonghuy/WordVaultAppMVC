using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using WordVaultAppMVC.Data;
using WordVaultAppMVC.Models;

namespace WordVaultAppMVC.Views.Controls
{
    public class DailyReviewControl : UserControl
    {
        private ComboBox cboTopics;
        private NumericUpDown numWordCount;
        private Button btnStart;
        private Label lblWord;
        private Label lblMeaning;
        private Label lblPronunciation;
        private Button btnNext;

        private List<WordDetails> currentWordList = new List<WordDetails>();
        private int currentIndex = -1;

        public DailyReviewControl()
        {
            InitializeComponent();
            LoadTopics();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            cboTopics = new ComboBox { Location = new System.Drawing.Point(20, 20), Width = 200 };
            numWordCount = new NumericUpDown { Location = new System.Drawing.Point(240, 20), Width = 80, Minimum = 1, Maximum = 100, Value = 5 };
            btnStart = new Button { Text = "Bắt đầu học", Location = new System.Drawing.Point(340, 20) };
            btnStart.Click += BtnStart_Click;
            lblWord = new Label { Text = "Từ:", Font = new System.Drawing.Font("Segoe UI", 16F), Location = new System.Drawing.Point(20, 80), AutoSize = true };
            lblPronunciation = new Label { Text = "Phát âm:", Location = new System.Drawing.Point(20, 120), AutoSize = true };
            lblMeaning = new Label { Text = "Nghĩa:", Location = new System.Drawing.Point(20, 160), AutoSize = true };
            btnNext = new Button { Text = "Tiếp theo", Location = new System.Drawing.Point(20, 200) };
            btnNext.Click += BtnNext_Click;
            this.Controls.AddRange(new Control[] { cboTopics, numWordCount, btnStart, lblWord, lblPronunciation, lblMeaning, btnNext });
        }

        private void LoadTopics()
        {
            using (var conn = DatabaseContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT Name FROM Topics", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) cboTopics.Items.Add(reader.GetString(0));
                }
            }
            if (cboTopics.Items.Count > 0) cboTopics.SelectedIndex = 0;
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            var topic = cboTopics.SelectedItem?.ToString();
            currentWordList = GetWordsByTopic(topic, (int)numWordCount.Value);
            currentIndex = -1;
            BtnNext_Click(null, null);
        }

        private List<WordDetails> GetWordsByTopic(string topic, int count)
        {
            var words = new List<WordDetails>();
            using (var conn = DatabaseContext.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT TOP (@Count) V.Word, V.Pronunciation, V.Meaning
                               FROM Vocabulary V
                               JOIN VocabularyTopic VT ON V.Id = VT.VocabularyId
                               JOIN Topics T ON T.Id = VT.TopicId
                               WHERE T.Name = @Topic
                               ORDER BY NEWID()";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Count", count);
                    cmd.Parameters.AddWithValue("@Topic", topic);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            words.Add(new WordDetails { Word = reader.GetString(0), Pronunciation = reader.GetString(1), Meaning = reader.GetString(2) });
                    }
                }
            }
            return words;
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (currentWordList.Count == 0) return;
            currentIndex++;
            if (currentIndex >= currentWordList.Count)
            {
                MessageBox.Show("Đã hết từ để học.");
                return;
            }
            var w = currentWordList[currentIndex];
            lblWord.Text = "Từ: " + w.Word;
            lblPronunciation.Text = "Phát âm: " + w.Pronunciation;
            lblMeaning.Text = "Nghĩa: " + w.Meaning;
        }
    }
}