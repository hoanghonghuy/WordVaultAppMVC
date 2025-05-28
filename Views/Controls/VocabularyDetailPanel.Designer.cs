namespace WordVaultAppMVC.Views
{
    partial class VocabularyDetailPanel
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // --- KHAI BÁO CONTROLS MỚI ---
        // private System.Windows.Forms.Panel pnlContainer; // <<< XÓA dòng này
        private System.Windows.Forms.TableLayoutPanel detailTableLayout; // <<< THÊM dòng này
        private System.Windows.Forms.Label lblWord;
        private System.Windows.Forms.Label lblMeaning;
        private System.Windows.Forms.Label lblPronunciation;
        private System.Windows.Forms.Label lblAudioUrl;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // Khởi tạo các controls cơ bản
            this.detailTableLayout = new System.Windows.Forms.TableLayoutPanel(); // <<< THAY pnlContainer = ... bằng dòng này
            this.lblWord = new System.Windows.Forms.Label();
            this.lblMeaning = new System.Windows.Forms.Label();
            this.lblPronunciation = new System.Windows.Forms.Label();
            this.lblAudioUrl = new System.Windows.Forms.Label();
            // Tạm dừng layout để cấu hình
            this.detailTableLayout.SuspendLayout();
            this.SuspendLayout();

            //
            // detailTableLayout (Layout chính thay thế pnlContainer)
            //
            this.detailTableLayout.BackColor = System.Drawing.Color.White; // Giữ màu nền
            this.detailTableLayout.ColumnCount = 1; // 1 cột
            this.detailTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.detailTableLayout.Controls.Add(this.lblWord, 0, 0);          // Thêm các label vào TLP
            this.detailTableLayout.Controls.Add(this.lblMeaning, 0, 1);
            this.detailTableLayout.Controls.Add(this.lblPronunciation, 0, 2);
            this.detailTableLayout.Controls.Add(this.lblAudioUrl, 0, 3);
            this.detailTableLayout.Dock = System.Windows.Forms.DockStyle.Fill; // TLP lấp đầy UserControl
            this.detailTableLayout.Location = new System.Drawing.Point(0, 0);
            this.detailTableLayout.Name = "detailTableLayout";
            this.detailTableLayout.Padding = new System.Windows.Forms.Padding(10); // Giữ Padding
            this.detailTableLayout.RowCount = 4; // 4 hàng
            this.detailTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // Hàng tự động chiều cao
            this.detailTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.detailTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.detailTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            // Kích thước Size ở đây chỉ là khi thiết kế, Dock=Fill sẽ quyết định kích thước thực tế
            this.detailTableLayout.Size = new System.Drawing.Size(400, 200);
            this.detailTableLayout.TabIndex = 0;

            //
            // lblWord (Cấu hình lại)
            //
            // Neo vào 2 cạnh trái phải của ô chứa trong TLP
            this.lblWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWord.AutoSize = true; // Tự động chiều cao
            this.lblWord.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            // Bỏ Location cố định
            this.lblWord.Name = "lblWord";
            this.lblWord.Size = new System.Drawing.Size(380, 28); // Size không còn quan trọng như trước
            this.lblWord.TabIndex = 0;
            this.lblWord.Text = "Từ: ";
            this.lblWord.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            //
            // lblMeaning (Cấu hình lại)
            //
            this.lblMeaning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMeaning.AutoSize = true;
            this.lblMeaning.Font = new System.Drawing.Font("Segoe UI", 12F);
            // Bỏ Location cố định
            this.lblMeaning.Name = "lblMeaning";
            this.lblMeaning.Size = new System.Drawing.Size(380, 28);
            this.lblMeaning.TabIndex = 1;
            this.lblMeaning.Text = "Nghĩa: ";
            this.lblMeaning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            //
            // lblPronunciation (Cấu hình lại)
            //
            this.lblPronunciation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPronunciation.AutoSize = true;
            this.lblPronunciation.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic);
            this.lblPronunciation.ForeColor = System.Drawing.Color.Gray;
            // Bỏ Location cố định
            this.lblPronunciation.Name = "lblPronunciation";
            this.lblPronunciation.Size = new System.Drawing.Size(380, 28);
            this.lblPronunciation.TabIndex = 2;
            this.lblPronunciation.Text = "Phát âm: ";
            this.lblPronunciation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            //
            // lblAudioUrl (Cấu hình lại)
            //
            this.lblAudioUrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAudioUrl.AutoSize = true;
            this.lblAudioUrl.Font = new System.Drawing.Font("Segoe UI", 10F); // Font nhỏ hơn cho URL
            this.lblAudioUrl.ForeColor = System.Drawing.Color.SteelBlue; // Màu khác
            // Bỏ Location cố định
            this.lblAudioUrl.Name = "lblAudioUrl";
            this.lblAudioUrl.Size = new System.Drawing.Size(380, 23);
            this.lblAudioUrl.TabIndex = 3;
            this.lblAudioUrl.Text = "Audio URL: ";
            this.lblAudioUrl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            //
            // VocabularyDetailPanel
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F); // Giữ nguyên hoặc chỉnh theo project
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            // Thay vì thêm pnlContainer, thêm detailTableLayout
            this.Controls.Add(this.detailTableLayout); // <<< THAY dòng Controls.Add(this.pnlContainer);
            this.Name = "VocabularyDetailPanel";
            this.Size = new System.Drawing.Size(400, 200); // Kích thước ban đầu

            // Kết thúc tạm dừng layout
            this.detailTableLayout.ResumeLayout(false);
            this.detailTableLayout.PerformLayout();
            // Bỏ dòng pnlContainer.ResumeLayout
            this.ResumeLayout(false);

        }

        #endregion
    }
}