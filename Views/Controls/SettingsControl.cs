// File: SettingsControl.cs
using System;
using System.Windows.Forms;
using WordVaultAppMVC.Properties; // <<< Quan trọng: Using để truy cập Application Settings
using System.Diagnostics;
using WordVaultAppMVC.Services; // <<< Using để gọi DataService (Backup, Restore, Clear)

namespace WordVaultAppMVC.Views.Controls
{
    /// <summary>
    /// UserControl cho phép người dùng cấu hình các cài đặt ứng dụng
    /// và thực hiện các thao tác quản lý dữ liệu như sao lưu, phục hồi.
    /// </summary>
    // Đảm bảo là partial class nếu có file Designer đi kèm (SettingsControl.Designer.cs)
    public partial class SettingsControl : UserControl
    {
        #region UI Controls Fields

        // Layout Controls
        private TableLayoutPanel mainLayout;
        private GroupBox gbLearning; // GroupBox cho cài đặt học tập
        private GroupBox gbData;     // GroupBox cho quản lý dữ liệu

        // Learning Settings Controls
        private Label lblReviewCount;
        private NumericUpDown nudReviewCount; // Cài đặt số từ ôn tập mặc định
        private Label lblQuizCount;
        private NumericUpDown nudQuizCount;   // Cài đặt số câu Quiz mặc định
        private CheckBox chkAutoPlayAudio;    // Cài đặt tự động phát âm thanh

        // Data Management Controls
        private Button btnBackup;         // Nút sao lưu CSDL
        private Button btnRestore;        // Nút phục hồi CSDL
        private Button btnClearHistory;   // Nút xóa lịch sử học tập

        // Action Buttons
        private Button btnSaveSettings;   // Nút lưu các cài đặt

        #endregion

        #region Dependencies (Optional)

        // Có thể thêm các Repository nếu cần cho Clear History trực tiếp thay vì qua DataService
        // private readonly LearningStatusRepository _learningStatusRepo;
        // private readonly QuizRepository _quizResultRepo;

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo SettingsControl.
        /// </summary>
        public SettingsControl()
        {
            // Khởi tạo dependencies nếu cần
            // _learningStatusRepo = new LearningStatusRepository();
            // _quizResultRepo = new QuizRepository();

            InitializeComponent(); // Khởi tạo giao diện
            LoadSettings();        // Tải các cài đặt đã lưu lên giao diện
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Khởi tạo và cấu hình các thành phần giao diện người dùng (Controls).
        /// </summary>
        private void InitializeComponent()
        {
            // --- Khởi tạo Controls ---
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.gbLearning = new System.Windows.Forms.GroupBox();
            this.lblReviewCount = new System.Windows.Forms.Label();
            this.nudReviewCount = new System.Windows.Forms.NumericUpDown();
            this.lblQuizCount = new System.Windows.Forms.Label();
            this.nudQuizCount = new System.Windows.Forms.NumericUpDown();
            this.chkAutoPlayAudio = new System.Windows.Forms.CheckBox();
            this.gbData = new System.Windows.Forms.GroupBox();
            this.btnBackup = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnClearHistory = new System.Windows.Forms.Button();
            this.btnSaveSettings = new System.Windows.Forms.Button();

            // Tạm dừng layout để tối ưu hiệu suất khi thêm nhiều control
            this.mainLayout.SuspendLayout();
            this.gbLearning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudReviewCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudQuizCount)).BeginInit();
            this.gbData.SuspendLayout();
            this.SuspendLayout();

            // --- Cấu hình mainLayout ---
            this.mainLayout.ColumnCount = 1; // 1 cột chính
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.gbLearning, 0, 0);      // GroupBox Học tập ở hàng 0
            this.mainLayout.Controls.Add(this.gbData, 0, 1);          // GroupBox Dữ liệu ở hàng 1
            this.mainLayout.Controls.Add(this.btnSaveSettings, 0, 2); // Nút Lưu ở hàng 2
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.Padding = new System.Windows.Forms.Padding(20);
            this.mainLayout.RowCount = 3; // 3 hàng
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // Hàng 0 tự động chiều cao
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // Hàng 1 tự động chiều cao
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F)); // Hàng 2 co giãn, đẩy nút Save xuống dưới
            this.mainLayout.Size = new System.Drawing.Size(550, 450); // Kích thước ví dụ
            this.mainLayout.TabIndex = 0;

            // --- Cấu hình gbLearning (GroupBox Cài đặt Học tập) ---
            this.gbLearning.AutoSize = true;
            this.gbLearning.Controls.Add(this.lblReviewCount);
            this.gbLearning.Controls.Add(this.nudReviewCount);
            this.gbLearning.Controls.Add(this.lblQuizCount);
            this.gbLearning.Controls.Add(this.nudQuizCount);
            this.gbLearning.Controls.Add(this.chkAutoPlayAudio);
            this.gbLearning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbLearning.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.gbLearning.Location = new System.Drawing.Point(23, 23);
            this.gbLearning.Margin = new System.Windows.Forms.Padding(3, 3, 3, 15); // Khoảng cách dưới
            this.gbLearning.Name = "gbLearning";
            this.gbLearning.Padding = new System.Windows.Forms.Padding(10, 10, 10, 15);
            //this.gbLearning.Size = new System.Drawing.Size(504, 160); // Size sẽ tự động vì AutoSize=true, Dock=Fill
            this.gbLearning.TabIndex = 0;
            this.gbLearning.TabStop = false;
            this.gbLearning.Text = "Cài đặt Học tập & Ôn tập";

            // --- Các controls trong gbLearning ---
            this.lblReviewCount.AutoSize = true;
            this.lblReviewCount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblReviewCount.Location = new System.Drawing.Point(15, 40);
            this.lblReviewCount.Name = "lblReviewCount";
            this.lblReviewCount.Size = new System.Drawing.Size(169, 20); // Cập nhật Size nếu cần
            this.lblReviewCount.TabIndex = 0;
            this.lblReviewCount.Text = "Số từ ôn tập/ngày mặc định:"; // Rõ nghĩa hơn

            this.nudReviewCount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.nudReviewCount.Location = new System.Drawing.Point(210, 38);
            this.nudReviewCount.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.nudReviewCount.Maximum = new decimal(new int[] { 50, 0, 0, 0 }); // Giới hạn max hợp lý hơn
            this.nudReviewCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudReviewCount.Name = "nudReviewCount";
            this.nudReviewCount.Size = new System.Drawing.Size(70, 27); // Điều chỉnh Size
            this.nudReviewCount.TabIndex = 1;
            this.nudReviewCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudReviewCount.Value = new decimal(new int[] { 10, 0, 0, 0 }); // Giá trị mặc định

            this.lblQuizCount.AutoSize = true;
            this.lblQuizCount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblQuizCount.Location = new System.Drawing.Point(15, 80);
            this.lblQuizCount.Name = "lblQuizCount";
            this.lblQuizCount.Size = new System.Drawing.Size(158, 20); // Cập nhật Size nếu cần
            this.lblQuizCount.TabIndex = 2;
            this.lblQuizCount.Text = "Số câu Quiz mặc định:";

            this.nudQuizCount.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.nudQuizCount.Location = new System.Drawing.Point(210, 78);
            this.nudQuizCount.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.nudQuizCount.Maximum = new decimal(new int[] { 50, 0, 0, 0 }); // Giới hạn max hợp lý hơn
            this.nudQuizCount.Minimum = new decimal(new int[] { 5, 0, 0, 0 });  // Quiz nên có ít nhất vài câu
            this.nudQuizCount.Name = "nudQuizCount";
            this.nudQuizCount.Size = new System.Drawing.Size(70, 27); // Điều chỉnh Size
            this.nudQuizCount.TabIndex = 3;
            this.nudQuizCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudQuizCount.Value = new decimal(new int[] { 10, 0, 0, 0 }); // Giá trị mặc định

            this.chkAutoPlayAudio.AutoSize = true;
            this.chkAutoPlayAudio.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkAutoPlayAudio.Location = new System.Drawing.Point(15, 120);
            this.chkAutoPlayAudio.Name = "chkAutoPlayAudio";
            this.chkAutoPlayAudio.Size = new System.Drawing.Size(250, 24); // Cập nhật Size nếu cần
            this.chkAutoPlayAudio.TabIndex = 4;
            this.chkAutoPlayAudio.Text = "Tự động phát âm thanh khi xem từ";
            this.chkAutoPlayAudio.UseVisualStyleBackColor = true;

            // --- Cấu hình gbData (GroupBox Quản lý Dữ liệu) ---
            this.gbData.AutoSize = true;
            this.gbData.Controls.Add(this.btnBackup);
            this.gbData.Controls.Add(this.btnRestore);
            this.gbData.Controls.Add(this.btnClearHistory);
            this.gbData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbData.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.gbData.Location = new System.Drawing.Point(23, gbLearning.Bottom + 15); // Tính toán vị trí Y dựa trên gbLearning
            this.gbData.Margin = new System.Windows.Forms.Padding(3, 3, 3, 15);
            this.gbData.Name = "gbData";
            this.gbData.Padding = new System.Windows.Forms.Padding(10);
            //this.gbData.Size = new System.Drawing.Size(504, 135); // Size sẽ tự động
            this.gbData.TabIndex = 1;
            this.gbData.TabStop = false;
            this.gbData.Text = "Quản lý Dữ liệu";

            // --- Các controls trong gbData ---
            this.btnBackup.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnBackup.Location = new System.Drawing.Point(15, 35);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(180, 35); // Tăng Width
            this.btnBackup.TabIndex = 0;
            this.btnBackup.Text = "📁 Sao lưu Dữ liệu...";
            this.btnBackup.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBackup.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnBackup.UseVisualStyleBackColor = true;
            this.btnBackup.Click += BtnBackup_Click; // Gán sự kiện

            this.btnRestore.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnRestore.Location = new System.Drawing.Point(210, 35); // Điều chỉnh X
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(180, 35); // Tăng Width
            this.btnRestore.TabIndex = 1;
            this.btnRestore.Text = "🔄 Phục hồi Dữ liệu...";
            this.btnRestore.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRestore.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += BtnRestore_Click; // Gán sự kiện

            this.btnClearHistory.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnClearHistory.ForeColor = System.Drawing.Color.DarkRed; // Màu đỏ cảnh báo
            this.btnClearHistory.Location = new System.Drawing.Point(15, 85); // Xuống hàng mới
            this.btnClearHistory.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.btnClearHistory.Name = "btnClearHistory";
            this.btnClearHistory.Size = new System.Drawing.Size(240, 35); // Tăng Width
            this.btnClearHistory.TabIndex = 2;
            this.btnClearHistory.Text = "❌ Xóa Trạng thái Học & Kết quả Quiz..."; // Rõ nghĩa hơn
            this.btnClearHistory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClearHistory.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnClearHistory.UseVisualStyleBackColor = true;
            this.btnClearHistory.Click += BtnClearHistory_Click; // Gán sự kiện


            // --- Cấu hình btnSaveSettings ---
            this.btnSaveSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right))); // Neo góc dưới phải
            this.btnSaveSettings.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSaveSettings.Location = new System.Drawing.Point(367, 387); // Điều chỉnh vị trí
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(160, 40);
            this.btnSaveSettings.TabIndex = 2; // TabIndex cho nhóm control cuối
            this.btnSaveSettings.Text = "💾 Lưu Cài đặt";
            this.btnSaveSettings.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSaveSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSaveSettings.UseVisualStyleBackColor = true;
            this.btnSaveSettings.Click += BtnSaveSettings_Click; // Gán sự kiện

            // --- Cấu hình SettingsControl ---
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F); // Hoặc kích thước project của Đại ca
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.mainLayout);
            this.Name = "SettingsControl";
            this.Size = new System.Drawing.Size(550, 450);

            // Kết thúc tạm dừng layout
            this.mainLayout.ResumeLayout(false);
            this.mainLayout.PerformLayout(); // Áp dụng layout cho mainLayout
            this.gbLearning.ResumeLayout(false);
            this.gbLearning.PerformLayout(); // Áp dụng layout cho gbLearning
            ((System.ComponentModel.ISupportInitialize)(this.nudReviewCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudQuizCount)).EndInit();
            this.gbData.ResumeLayout(false);
            // PerformLayout không cần cho gbData vì nút đặt thủ công vị trí
            this.ResumeLayout(false);
        }

        #endregion

        #region Settings Logic

        /// <summary>
        /// Tải các giá trị cài đặt từ Properties.Settings và hiển thị lên các controls.
        /// Xử lý lỗi nếu không đọc được cài đặt.
        /// </summary>
        private void LoadSettings()
        {
            try
            {
                // Đọc giá trị từ lớp Settings (được tạo tự động từ file Settings.settings).
                // Sử dụng Math.Max/Min để đảm bảo giá trị nằm trong khoảng hợp lệ của NumericUpDown.
                nudReviewCount.Value = Math.Max(nudReviewCount.Minimum, Math.Min(nudReviewCount.Maximum, Settings.Default.DefaultReviewWordCount));
                nudQuizCount.Value = Math.Max(nudQuizCount.Minimum, Math.Min(nudQuizCount.Maximum, Settings.Default.DefaultQuizQuestionCount));
                chkAutoPlayAudio.Checked = Settings.Default.AutoPlayAudio;
                Debug.WriteLine("[INFO] LoadSettings: Cài đặt đã được tải thành công.");
            }
            catch (Exception ex) // Bắt lỗi nếu file settings bị lỗi hoặc không đọc được
            {
                Debug.WriteLine($"[ERROR] LoadSettings: Lỗi khi tải cài đặt: {ex.Message}. Sẽ sử dụng giá trị mặc định.");
                MessageBox.Show($"Lỗi khi tải cài đặt: {ex.Message}\nSẽ sử dụng giá trị mặc định.", "Lỗi Cài đặt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // Gán giá trị mặc định cứng nếu đọc lỗi để đảm bảo control có giá trị.
                nudReviewCount.Value = 10;
                nudQuizCount.Value = 10; // Giảm mặc định Quiz
                chkAutoPlayAudio.Checked = false;
            }
        }

        /// <summary>
        /// Lưu các giá trị cài đặt hiện tại từ controls vào Properties.Settings.
        /// </summary>
        private void SaveSettings()
        {
            try
            {
                Debug.WriteLine("[INFO] SaveSettings: Bắt đầu lưu cài đặt...");
                // Gán giá trị từ controls vào các thuộc tính của Settings.Default.
                Settings.Default.DefaultReviewWordCount = (int)nudReviewCount.Value;
                Settings.Default.DefaultQuizQuestionCount = (int)nudQuizCount.Value;
                Settings.Default.AutoPlayAudio = chkAutoPlayAudio.Checked;

                // Gọi Save() để ghi các thay đổi vào file cấu hình người dùng.
                Settings.Default.Save();
                Debug.WriteLine("[INFO] SaveSettings: Cài đặt đã được lưu thành công.");
                MessageBox.Show("Đã lưu cài đặt thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) // Bắt lỗi nếu không thể lưu cài đặt
            {
                Debug.WriteLine($"[ERROR] SaveSettings: Lỗi khi lưu cài đặt: {ex.Message}");
                MessageBox.Show($"Lỗi khi lưu cài đặt: {ex.Message}", "Lỗi Lưu Cài đặt", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Xử lý sự kiện Click nút "Lưu Cài đặt".
        /// </summary>
        private void BtnSaveSettings_Click(object sender, EventArgs e)
        {
            SaveSettings(); // Gọi hàm lưu cài đặt
        }

        /// <summary>
        /// Xử lý sự kiện Click nút "Sao lưu Dữ liệu". Mở dialog lưu file và gọi DataService.
        /// </summary>
        private void BtnBackup_Click(object sender, EventArgs e)
        {
            // Sử dụng SaveFileDialog để người dùng chọn vị trí và tên file backup.
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "SQL Server Backup files (*.bak)|*.bak"; // Chỉ cho phép lưu file .bak
                sfd.Title = "Chọn vị trí lưu bản Sao lưu Cơ sở dữ liệu";
                // Đặt tên file mặc định có chứa ngày giờ.
                sfd.FileName = $"WordVaultBackup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";

                // Hiển thị dialog và kiểm tra xem người dùng có nhấn OK không.
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string backupPath = sfd.FileName; // Lấy đường dẫn file đã chọn.
                    Debug.WriteLine($"[INFO] BtnBackup_Click: User selected path: {backupPath}");
                    this.Cursor = Cursors.WaitCursor; // Hiển thị con trỏ chờ.

                    try
                    {
                        // Gọi phương thức BackupDatabase từ DataService.
                        bool success = DataService.BackupDatabase(backupPath);
                        if (success)
                        {
                            // DataService đã hiển thị thông báo thành công.
                            Debug.WriteLine("[INFO] BtnBackup_Click: Backup reported successful by DataService.");
                        }
                        // Ngược lại, DataService đã hiển thị thông báo lỗi.
                    }
                    catch (Exception ex) // Bắt lỗi không mong muốn nếu DataService ném ra (dù không nên)
                    {
                        Debug.WriteLine($"[ERROR] BtnBackup_Click: Lỗi không mong muốn: {ex.ToString()}");
                        MessageBox.Show($"Lỗi không mong muốn trong quá trình sao lưu: {ex.Message}", "Lỗi Sao lưu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default; // Luôn trả con trỏ về bình thường.
                    }
                }
                else
                {
                    Debug.WriteLine("[INFO] BtnBackup_Click: User cancelled SaveFileDialog.");
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện Click nút "Phục hồi Dữ liệu". Hiển thị cảnh báo, mở dialog chọn file và gọi DataService.
        /// </summary>
        private void BtnRestore_Click(object sender, EventArgs e)
        {
            // Hiển thị cảnh báo rất quan trọng cho người dùng.
            DialogResult confirm = MessageBox.Show(
                "!!! CẢNH BÁO !!!\n\n" +
                "Phục hồi dữ liệu sẽ XÓA SẠCH và THAY THẾ TOÀN BỘ dữ liệu từ vựng, chủ đề, lịch sử học tập,... hiện tại bằng dữ liệu từ file sao lưu.\n\n" +
                "Hành động này KHÔNG THỂ HOÀN TÁC.\n\n" +
                "Ứng dụng sẽ cần KHỞI ĐỘNG LẠI sau khi phục hồi thành công.\n\n" +
                "BẠN CÓ CHẮC CHẮN MUỐN TIẾP TỤC?",
                "XÁC NHẬN PHỤC HỒI DỮ LIỆU",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning, // Dùng icon Warning
                MessageBoxDefaultButton.Button2); // Mặc định chọn No

            if (confirm == DialogResult.Yes)
            {
                // Sử dụng OpenFileDialog để người dùng chọn file backup (.bak).
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "SQL Server Backup files (*.bak)|*.bak|All files (*.*)|*.*"; // Cho phép xem cả file khác
                    ofd.Title = "Chọn file Sao lưu (.bak) để Phục hồi";
                    ofd.CheckFileExists = true; // Đảm bảo file tồn tại

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        string backupPath = ofd.FileName; // Lấy đường dẫn file đã chọn.
                        Debug.WriteLine($"[INFO] BtnRestore_Click: User selected file: {backupPath}");
                        this.Cursor = Cursors.WaitCursor;

                        try
                        {
                            // Gọi phương thức RestoreDatabase từ DataService.
                            bool success = DataService.RestoreDatabase(backupPath);
                            if (success)
                            {
                                // DataService đã hiển thị thông báo thành công và tự khởi động lại ứng dụng.
                                Debug.WriteLine("[INFO] BtnRestore_Click: Restore reported successful by DataService. Application should restart.");
                                // Không cần làm gì thêm ở đây vì ứng dụng sẽ restart.
                            }
                            // Ngược lại, DataService đã hiển thị thông báo lỗi.
                        }
                        catch (Exception ex) // Bắt lỗi không mong muốn nếu DataService ném ra
                        {
                            Debug.WriteLine($"[ERROR] BtnRestore_Click: Lỗi không mong muốn: {ex.ToString()}");
                            MessageBox.Show($"Lỗi không mong muốn trong quá trình phục hồi: {ex.Message}", "Lỗi Phục hồi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            this.Cursor = Cursors.Default; // Trả lại con trỏ (dù có thể ứng dụng sẽ restart)
                        }
                    }
                    else
                    {
                        Debug.WriteLine("[INFO] BtnRestore_Click: User cancelled OpenFileDialog.");
                    }
                }
            }
            else
            {
                Debug.WriteLine("[INFO] BtnRestore_Click: User chose not to restore.");
            }
        }

        /// <summary>
        /// Xử lý sự kiện Click nút "Xóa Trạng thái Học tập". Hiển thị cảnh báo và gọi DataService.
        /// </summary>
        private void BtnClearHistory_Click(object sender, EventArgs e)
        {
            // Hiển thị cảnh báo rất quan trọng.
            DialogResult confirm = MessageBox.Show(
                "!!! CẢNH BÁO !!!\n\n" +
                "Hành động này sẽ XÓA VĨNH VIỄN TOÀN BỘ:\n" +
                "  - Trạng thái học tập ('Đã học', 'Đang học',...) của tất cả các từ.\n" +
                "  - Toàn bộ kết quả các bài Quiz đã làm.\n\n" +
                "Dữ liệu từ vựng và chủ đề gốc sẽ KHÔNG bị ảnh hưởng.\n\n" +
                "Hành động này KHÔNG THỂ HOÀN TÁC.\n\n" +
                "BẠN CÓ CHẮC CHẮN MUỐN TIẾP TỤC?",
                "XÁC NHẬN XÓA DỮ LIỆU HỌC TẬP",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Error, // Dùng icon Error cho mức độ nghiêm trọng
                MessageBoxDefaultButton.Button2); // Mặc định chọn No

            if (confirm == DialogResult.Yes)
            {
                Debug.WriteLine("[INFO] BtnClearHistory_Click: User confirmed clearing learning data.");
                this.Cursor = Cursors.WaitCursor;
                try
                {
                    // Gọi phương thức ClearLearningData từ DataService.
                    bool success = DataService.ClearLearningData();
                    if (success)
                    {
                        // DataService đã hiển thị thông báo thành công.
                        Debug.WriteLine("[INFO] BtnClearHistory_Click: ClearLearningData reported successful by DataService.");
                        // Cân nhắc: Có thể cần thông báo cho các màn hình khác (Quiz, Review) để cập nhật nếu chúng đang mở.
                    }
                    // Ngược lại, DataService đã hiển thị thông báo lỗi.
                }
                catch (Exception ex) // Bắt lỗi không mong muốn nếu DataService ném ra
                {
                    Debug.WriteLine($"[ERROR] BtnClearHistory_Click: Lỗi không mong muốn: {ex.ToString()}");
                    MessageBox.Show($"Lỗi không mong muốn khi xóa dữ liệu học tập: {ex.Message}", "Lỗi Xóa Dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Cursor = Cursors.Default; // Luôn trả lại con trỏ.
                }
            }
            else
            {
                Debug.WriteLine("[INFO] BtnClearHistory_Click: User chose not to clear data.");
            }
        }

        #endregion

    } // Kết thúc class SettingsControl

    // Khai báo partial class nếu có file Designer đi kèm
    // public partial class SettingsControl { }

} // Kết thúc namespace