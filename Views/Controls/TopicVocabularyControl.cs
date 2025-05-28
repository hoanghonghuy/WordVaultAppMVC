// File: TopicVocabularyControl.cs (Đã sửa lỗi và hoàn thiện Sửa/Xóa)
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;
using WordVaultAppMVC.Data;
using WordVaultAppMVC.Models;
using System.Linq;
using System.Diagnostics; // Thêm using cho Debug nếu cần

namespace WordVaultAppMVC.Views.Controls
{
    public class TopicVocabularyControl : UserControl
    {
        // --- Khai báo UI Controls ---
        private ComboBox cboTopics;
        private ListView lstWords;
        private TextBox txtSearchWord;
        private Button btnSearchWord;
        private TableLayoutPanel mainLayout;
        private TableLayoutPanel topLayout;
        private FlowLayoutPanel actionButtonPanel; // Panel chứa nút Sửa/Xóa
        private Button btnEditWord;           // Nút Sửa
        private Button btnDeleteWord;         // Nút Xóa

        // --- Repositories cần thiết ---
        private readonly TopicRepository topicRepo;
        private readonly VocabularyRepository vocabRepo;

        public TopicVocabularyControl(string defaultTopic = null)
        {
            topicRepo = new TopicRepository();
            vocabRepo = new VocabularyRepository();
            InitializeComponent();
            LoadTopics();

            // Logic chọn topic ban đầu
            if (!string.IsNullOrEmpty(defaultTopic) && cboTopics.Items.Contains(defaultTopic))
            {
                cboTopics.SelectedItem = defaultTopic;
            }
            else if (cboTopics.Items.Count > 0)
            {
                cboTopics.SelectedIndex = 0;
            }
            else
            {
                // Nếu không có topic nào, có thể thông báo hoặc xử lý khác
                LoadWordsByTopic(); // Gọi để hiển thị trạng thái rỗng
            }

            UpdateButtonStates(); // Đặt trạng thái nút ban đầu
        }

        // --- InitializeComponent (Đã sửa lỗi và hoàn thiện) ---
        private void InitializeComponent()
        {
            // Khởi tạo các controls
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.topLayout = new System.Windows.Forms.TableLayoutPanel();
            this.cboTopics = new System.Windows.Forms.ComboBox();
            this.txtSearchWord = new System.Windows.Forms.TextBox();
            this.btnSearchWord = new System.Windows.Forms.Button();
            this.lstWords = new System.Windows.Forms.ListView();
            this.actionButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnEditWord = new System.Windows.Forms.Button();
            this.btnDeleteWord = new System.Windows.Forms.Button();
            this.mainLayout.SuspendLayout();
            this.topLayout.SuspendLayout();
            this.actionButtonPanel.SuspendLayout();
            this.SuspendLayout();

            //
            // mainLayout
            //
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.topLayout, 0, 0);
            this.mainLayout.Controls.Add(this.lstWords, 0, 1);
            this.mainLayout.Controls.Add(this.actionButtonPanel, 0, 2);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.RowCount = 3;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F)); // Hàng 0: Fixed height
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F)); // Hàng 1: ListView
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // Hàng 2: Buttons
            this.mainLayout.Padding = new System.Windows.Forms.Padding(10);
            this.mainLayout.Size = new System.Drawing.Size(600, 450);
            this.mainLayout.TabIndex = 0;

            //
            // topLayout
            //
            this.topLayout.ColumnCount = 3;
            this.topLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.topLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.topLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F)); // Giữ chiều rộng cột nút Tìm
            this.topLayout.Controls.Add(this.cboTopics, 0, 0);
            this.topLayout.Controls.Add(this.txtSearchWord, 1, 0);
            this.topLayout.Controls.Add(this.btnSearchWord, 2, 0);
            this.topLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topLayout.Location = new System.Drawing.Point(13, 13); // Vị trí tương đối trong mainLayout (Dock sẽ override)
            this.topLayout.Name = "topLayout";
            this.topLayout.RowCount = 1;
            this.topLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.topLayout.Size = new System.Drawing.Size(574, 35); // Chiều cao sẽ được quyết định bởi hàng của mainLayout
            this.topLayout.TabIndex = 0;
            this.topLayout.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);

            //
            // cboTopics
            //
            this.cboTopics.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTopics.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTopics.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboTopics.FormattingEnabled = true;
            this.cboTopics.Location = new System.Drawing.Point(3, 6); // Vị trí tương đối
            this.cboTopics.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.cboTopics.Name = "cboTopics";
            this.cboTopics.Size = new System.Drawing.Size(194, 28); // Size sẽ bị Anchor override
            this.cboTopics.TabIndex = 0;
            this.cboTopics.SelectedIndexChanged += (s, e) => {
                LoadWordsByTopic();
                txtSearchWord.Clear();
                UpdateButtonStates();
            };

            //
            // txtSearchWord
            //
            this.txtSearchWord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearchWord.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtSearchWord.Location = new System.Drawing.Point(213, 3); // Vị trí tương đối
            this.txtSearchWord.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.txtSearchWord.Name = "txtSearchWord";
            this.txtSearchWord.Size = new System.Drawing.Size(228, 27); // Size sẽ bị Dock override
            this.txtSearchWord.TabIndex = 1;
            this.txtSearchWord.KeyDown += TxtSearchWord_KeyDown;

            //
            // btnSearchWord
            //
            this.btnSearchWord.AutoSize = false;
            this.btnSearchWord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSearchWord.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSearchWord.Location = new System.Drawing.Point(449, 3); // Vị trí tương đối
            this.btnSearchWord.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.btnSearchWord.Name = "btnSearchWord";
            this.btnSearchWord.Size = new System.Drawing.Size(122, 29); // Size sẽ bị Dock override
            this.btnSearchWord.TabIndex = 2;
            this.btnSearchWord.Text = "🔍 Tìm";
            this.btnSearchWord.UseVisualStyleBackColor = true;
            this.btnSearchWord.Click += BtnSearchWord_Click;

            //
            // lstWords
            //
            this.lstWords.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            new ColumnHeader { Text = "Từ vựng", Width = 160 },
            new ColumnHeader { Text = "Phát âm", Width = 150 },
            new ColumnHeader { Text = "Nghĩa tiếng Việt", Width = -2 }}); // Cột cuối tự giãn
            this.lstWords.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstWords.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lstWords.FullRowSelect = true;
            this.lstWords.GridLines = true;
            this.lstWords.HideSelection = false;
            this.lstWords.Location = new System.Drawing.Point(13, 58); // Cập nhật vị trí Y tương đối
            this.lstWords.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.lstWords.MultiSelect = false;
            this.lstWords.Name = "lstWords";
            this.lstWords.Size = new System.Drawing.Size(574, 324); // Size sẽ bị Dock override
            this.lstWords.TabIndex = 1;
            this.lstWords.UseCompatibleStateImageBehavior = false;
            this.lstWords.View = System.Windows.Forms.View.Details;
            this.lstWords.SelectedIndexChanged += LstWords_SelectedIndexChanged; // Gán sự kiện

            //
            // actionButtonPanel
            //
            // ***** BỎ Anchor = Right *****
            this.actionButtonPanel.AutoSize = true; // Giữ AutoSize
            this.actionButtonPanel.Controls.Add(this.btnEditWord);
            this.actionButtonPanel.Controls.Add(this.btnDeleteWord);
            this.actionButtonPanel.Dock = System.Windows.Forms.DockStyle.Fill; // <<< Đặt Dock Fill để nó căn giữa trong hàng AutoSize
            this.actionButtonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft; // <<< Đổi FlowDirection để nút xuất hiện bên phải
            this.actionButtonPanel.Location = new System.Drawing.Point(13, 388); // Vị trí tương đối
            this.actionButtonPanel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.actionButtonPanel.Name = "actionButtonPanel";
            this.actionButtonPanel.Size = new System.Drawing.Size(574, 36); // Size ví dụ
            this.actionButtonPanel.TabIndex = 2;
            // ***** KẾT THÚC SỬA LỖI HIỂN THỊ NÚT *****


            //
            // btnEditWord
            //
            this.btnEditWord.Anchor = System.Windows.Forms.AnchorStyles.Right; // Neo nút sang phải trong FlowLayoutPanel
            this.btnEditWord.AutoSize = true;
            this.btnEditWord.Enabled = false;
            this.btnEditWord.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnEditWord.Location = new System.Drawing.Point(114, 3); // Vị trí tương đối trong FlowLayoutPanel (sẽ tự điều chỉnh)
            this.btnEditWord.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3); // Bỏ margin phải lớn
            this.btnEditWord.Name = "btnEditWord";
            this.btnEditWord.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0); // Thêm Padding ngang cho nút
            this.btnEditWord.Size = new System.Drawing.Size(91, 30);
            this.btnEditWord.TabIndex = 0;
            this.btnEditWord.Text = "✏️ Sửa";
            this.btnEditWord.UseVisualStyleBackColor = true;
            this.btnEditWord.Click += BtnEditWord_Click;

            //
            // btnDeleteWord
            //
            this.btnDeleteWord.Anchor = System.Windows.Forms.AnchorStyles.Right; // Neo nút sang phải trong FlowLayoutPanel
            this.btnDeleteWord.AutoSize = true;
            this.btnDeleteWord.Enabled = false;
            this.btnDeleteWord.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnDeleteWord.ForeColor = System.Drawing.Color.Red;
            this.btnDeleteWord.Location = new System.Drawing.Point(17, 3); // Vị trí tương đối trong FlowLayoutPanel
            this.btnDeleteWord.Name = "btnDeleteWord";
            this.btnDeleteWord.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0); // Thêm Padding ngang cho nút
            this.btnDeleteWord.Size = new System.Drawing.Size(91, 30);
            this.btnDeleteWord.TabIndex = 1;
            this.btnDeleteWord.Text = "🗑️ Xóa";
            this.btnDeleteWord.UseVisualStyleBackColor = true;
            this.btnDeleteWord.Click += BtnDeleteWord_Click;

            //
            // TopicVocabularyControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainLayout);
            this.Name = "TopicVocabularyControl";
            this.Size = new System.Drawing.Size(600, 450);
            this.mainLayout.ResumeLayout(false);
            this.mainLayout.PerformLayout(); // Gọi PerformLayout cho TLP chính
            this.topLayout.ResumeLayout(false);
            this.topLayout.PerformLayout();
            this.actionButtonPanel.ResumeLayout(false);
            this.actionButtonPanel.PerformLayout(); // Gọi PerformLayout cho panel nút
            this.ResumeLayout(false);
            // Bỏ PerformLayout cuối cùng vì đã gọi cho mainLayout bao ngoài

        }

        // --- Các phương thức xử lý logic (Giữ nguyên và có sửa lỗi EnsureVisible) ---

        private void LoadTopics()
        {
            // ... (Code LoadTopics giữ nguyên) ...
            cboTopics.Items.Clear();
            try
            {
                using (var conn = DatabaseContext.GetConnection())
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT Name FROM Topics ORDER BY Name", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cboTopics.Items.Add(reader.GetString(0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách chủ đề: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadWordsByTopic()
        {
            // ... (Code LoadWordsByTopic giữ nguyên) ...
            lstWords.Items.Clear();
            string topic = cboTopics.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(topic))
            {
                UpdateButtonStates(); // Vẫn cập nhật trạng thái nút nếu list rỗng
                return;
            }

            try
            {
                using (var conn = DatabaseContext.GetConnection())
                {
                    conn.Open();
                    var sql = @"SELECT V.Word, V.Pronunciation, V.Meaning, V.Id
                                FROM Vocabulary V
                                JOIN VocabularyTopic VT ON V.Id = VT.VocabularyId
                                JOIN Topics T ON T.Id = VT.TopicId
                                WHERE T.Name = @Topic
                                ORDER BY V.Word";
                    var cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Topic", topic);
                    using (var reader = cmd.ExecuteReader())
                    {
                        lstWords.BeginUpdate();
                        while (reader.Read())
                        {
                            var item = new ListViewItem(reader.GetString(0));
                            item.SubItems.Add(reader.IsDBNull(1) ? "" : reader.GetString(1));
                            item.SubItems.Add(reader.IsDBNull(2) ? "" : reader.GetString(2));
                            item.Tag = reader.GetInt32(3);
                            lstWords.Items.Add(item);
                        }
                        lstWords.EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách từ vựng theo chủ đề: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Tự động điều chỉnh chiều rộng cột cuối cùng
            if (lstWords.Columns.Count > 0)
            {
                try
                { // Thêm try-catch nhỏ phòng trường hợp lỗi khi resize cột
                    lstWords.Columns[lstWords.Columns.Count - 1].Width = -2;
                }
                catch { }
            }
            UpdateButtonStates();
        }

        private void PerformSearch()
        {
            string searchTerm = txtSearchWord.Text.Trim();

            if (string.IsNullOrEmpty(searchTerm))
            {
                return;
            }

            bool found = false;
            foreach (ListViewItem item in lstWords.Items)
            {
                if (item.SubItems[0].Text.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    lstWords.SelectedItems.Clear();
                    item.Selected = true;
                    item.Focused = true;

                    // ***** ĐÃ SỬA LỖI CS7036 *****
                    lstWords.EnsureVisible(item.Index); // << Gọi từ lstWords với index
                    // ***** KẾT THÚC SỬA LỖI *****

                    lstWords.Focus();
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                MessageBox.Show($"Không tìm thấy từ nào chứa '{searchTerm}' trong chủ đề này.", "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lstWords.SelectedItems.Clear();
            }
        }

        private void BtnSearchWord_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void TxtSearchWord_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                PerformSearch();
                e.SuppressKeyPress = true;
            }
        }

        private void LstWords_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            bool itemSelected = lstWords.SelectedItems.Count > 0;
            btnEditWord.Enabled = itemSelected;
            btnDeleteWord.Enabled = itemSelected;
        }

        private int GetSelectedTopicId()
        {
            // ... (Code GetSelectedTopicId giữ nguyên) ...
            string topicName = cboTopics.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(topicName)) return -1;
            try
            {
                using (var conn = DatabaseContext.GetConnection())
                {
                    conn.Open();
                    using (var cmdTopic = new SqlCommand("SELECT Id FROM Topics WHERE Name = @Topic", conn))
                    {
                        cmdTopic.Parameters.AddWithValue("@Topic", topicName);
                        var result = cmdTopic.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            return (int)result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lấy ID chủ đề: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return -1;
        }

        private void BtnEditWord_Click(object sender, EventArgs e)
        {
            // ... (Code BtnEditWord_Click giữ nguyên) ...
            if (lstWords.SelectedItems.Count == 0) return;
            ListViewItem selectedItem = lstWords.SelectedItems[0];
            if (selectedItem.Tag is int vocabularyId && vocabularyId > 0)
            {
                using (var editForm = new EditVocabularyForm(vocabularyId))
                {
                    DialogResult result = editForm.ShowDialog(this.FindForm());
                    if (result == DialogResult.OK)
                    {
                        int selectedIndex = -1;
                        if (lstWords.SelectedIndices.Count > 0) selectedIndex = lstWords.SelectedIndices[0];
                        LoadWordsByTopic();
                        if (selectedIndex >= 0 && selectedIndex < lstWords.Items.Count)
                        {
                            lstWords.Items[selectedIndex].Selected = true;
                            lstWords.Items[selectedIndex].Focused = true;
                            lstWords.EnsureVisible(selectedIndex); // Sửa ở đây nữa cho chắc
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Không lấy được ID của từ cần sửa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnDeleteWord_Click(object sender, EventArgs e)
        {
            // ... (Code BtnDeleteWord_Click giữ nguyên) ...
            if (lstWords.SelectedItems.Count == 0) return;
            ListViewItem selectedItem = lstWords.SelectedItems[0];
            string wordText = selectedItem.SubItems[0].Text;
            if (selectedItem.Tag is int vocabularyId && vocabularyId > 0)
            {
                int topicId = GetSelectedTopicId();
                if (topicId == -1)
                {
                    MessageBox.Show("Không xác định được chủ đề hiện tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var confirmResult = MessageBox.Show($"Bạn có chắc muốn xóa từ '{wordText}' khỏi chủ đề '{cboTopics.SelectedItem}'?\n\n(Lưu ý: Thao tác này chỉ xóa liên kết từ khỏi chủ đề, không xóa từ gốc trong từ điển.)",
                                                   "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirmResult == DialogResult.Yes)
                {
                    try
                    {
                        // Giả định topicRepo đã có phương thức này
                        bool success = topicRepo.RemoveWordFromTopic(vocabularyId, topicId);
                        if (success)
                        {
                            MessageBox.Show("Đã xóa từ khỏi chủ đề thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadWordsByTopic();
                        }
                        else
                        {
                            MessageBox.Show("Xóa từ khỏi chủ đề thất bại (có thể từ không tồn tại trong chủ đề này).", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); // Thay đổi thông báo lỗi
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xóa từ khỏi chủ đề: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Không lấy được ID của từ cần xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

    } // Kết thúc class

    // --- Lớp EditVocabularyForm giữ nguyên như code trước ---
    // (Bạn cần đảm bảo đã tạo Form này trong project)
    // public class EditVocabularyForm : Form { ... }

} // Kết thúc namespace