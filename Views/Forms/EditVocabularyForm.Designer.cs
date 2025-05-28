// File: EditVocabularyForm.Designer.cs
namespace WordVaultAppMVC.Views
{
    partial class EditVocabularyForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Khai báo các controls sẽ dùng
        private System.Windows.Forms.Label lblWordLabel;
        private System.Windows.Forms.TextBox txtWord;
        private System.Windows.Forms.Label lblMeaningLabel;
        private System.Windows.Forms.TextBox txtMeaning;
        private System.Windows.Forms.Label lblPronunciationLabel;
        private System.Windows.Forms.TextBox txtPronunciation;
        private System.Windows.Forms.Label lblAudioUrlLabel;
        private System.Windows.Forms.TextBox txtAudioUrl;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblWordLabel = new System.Windows.Forms.Label();
            this.txtWord = new System.Windows.Forms.TextBox();
            this.lblMeaningLabel = new System.Windows.Forms.Label();
            this.txtMeaning = new System.Windows.Forms.TextBox();
            this.lblPronunciationLabel = new System.Windows.Forms.Label();
            this.txtPronunciation = new System.Windows.Forms.TextBox();
            this.lblAudioUrlLabel = new System.Windows.Forms.Label();
            this.txtAudioUrl = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblWordLabel
            //
            this.lblWordLabel.AutoSize = true;
            this.lblWordLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblWordLabel.Location = new System.Drawing.Point(30, 33);
            this.lblWordLabel.Name = "lblWordLabel";
            this.lblWordLabel.Size = new System.Drawing.Size(63, 20);
            this.lblWordLabel.TabIndex = 0;
            this.lblWordLabel.Text = "Từ vựng:";
            //
            // txtWord
            //
            this.txtWord.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWord.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtWord.Location = new System.Drawing.Point(130, 30);
            this.txtWord.Margin = new System.Windows.Forms.Padding(3, 3, 30, 3); // Thêm margin phải
            this.txtWord.Name = "txtWord";
            this.txtWord.Size = new System.Drawing.Size(240, 27);
            this.txtWord.TabIndex = 1;
            //
            // lblMeaningLabel
            //
            this.lblMeaningLabel.AutoSize = true;
            this.lblMeaningLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMeaningLabel.Location = new System.Drawing.Point(30, 73);
            this.lblMeaningLabel.Name = "lblMeaningLabel";
            this.lblMeaningLabel.Size = new System.Drawing.Size(50, 20);
            this.lblMeaningLabel.TabIndex = 2;
            this.lblMeaningLabel.Text = "Nghĩa:";
            //
            // txtMeaning
            //
            this.txtMeaning.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMeaning.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtMeaning.Location = new System.Drawing.Point(130, 70);
            this.txtMeaning.Margin = new System.Windows.Forms.Padding(3, 3, 30, 3);
            this.txtMeaning.Multiline = true;
            this.txtMeaning.Name = "txtMeaning";
            this.txtMeaning.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMeaning.Size = new System.Drawing.Size(240, 60); // Tăng chiều cao ô nghĩa
            this.txtMeaning.TabIndex = 3;
            //
            // lblPronunciationLabel
            //
            this.lblPronunciationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPronunciationLabel.AutoSize = true;
            this.lblPronunciationLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPronunciationLabel.Location = new System.Drawing.Point(30, 146); // Điều chỉnh vị trí Y
            this.lblPronunciationLabel.Name = "lblPronunciationLabel";
            this.lblPronunciationLabel.Size = new System.Drawing.Size(68, 20);
            this.lblPronunciationLabel.TabIndex = 4;
            this.lblPronunciationLabel.Text = "Phát âm:";
            //
            // txtPronunciation
            //
            this.txtPronunciation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPronunciation.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPronunciation.Location = new System.Drawing.Point(130, 143); // Điều chỉnh vị trí Y
            this.txtPronunciation.Margin = new System.Windows.Forms.Padding(3, 3, 30, 3);
            this.txtPronunciation.Name = "txtPronunciation";
            this.txtPronunciation.Size = new System.Drawing.Size(240, 27);
            this.txtPronunciation.TabIndex = 5;
            //
            // lblAudioUrlLabel
            //
            this.lblAudioUrlLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAudioUrlLabel.AutoSize = true;
            this.lblAudioUrlLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblAudioUrlLabel.Location = new System.Drawing.Point(30, 186); // Điều chỉnh vị trí Y
            this.lblAudioUrlLabel.Name = "lblAudioUrlLabel";
            this.lblAudioUrlLabel.Size = new System.Drawing.Size(80, 20);
            this.lblAudioUrlLabel.TabIndex = 6;
            this.lblAudioUrlLabel.Text = "Audio URL:";
            //
            // txtAudioUrl
            //
            this.txtAudioUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAudioUrl.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtAudioUrl.Location = new System.Drawing.Point(130, 183); // Điều chỉnh vị trí Y
            this.txtAudioUrl.Margin = new System.Windows.Forms.Padding(3, 3, 30, 3);
            this.txtAudioUrl.Name = "txtAudioUrl";
            this.txtAudioUrl.Size = new System.Drawing.Size(240, 27);
            this.txtAudioUrl.TabIndex = 7;
            //
            // btnSave
            //
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.Location = new System.Drawing.Point(130, 235); // Điều chỉnh vị trí Y
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(115, 35);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "Lưu thay đổi";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click); // Gán sự kiện trong file .cs
            //
            // btnCancel
            //
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel; // Đặt DialogResult
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCancel.Location = new System.Drawing.Point(255, 235); // Điều chỉnh vị trí Y
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(115, 35);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Hủy bỏ";
            this.btnCancel.UseVisualStyleBackColor = true;
            // Không cần gán sự kiện Click vì đã đặt DialogResult
            //
            // EditVocabularyForm
            //
            this.AcceptButton = this.btnSave; // Nút Save là nút mặc định khi nhấn Enter
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel; // Nút Cancel là nút mặc định khi nhấn Escape
            this.ClientSize = new System.Drawing.Size(400, 290); // Điều chỉnh kích thước cửa sổ
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtAudioUrl);
            this.Controls.Add(this.lblAudioUrlLabel);
            this.Controls.Add(this.txtPronunciation);
            this.Controls.Add(this.lblPronunciationLabel);
            this.Controls.Add(this.txtMeaning);
            this.Controls.Add(this.lblMeaningLabel);
            this.Controls.Add(this.txtWord);
            this.Controls.Add(this.lblWordLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditVocabularyForm";
            this.ShowIcon = false; // Ẩn icon của Form
            this.ShowInTaskbar = false; // Không hiển thị trên Taskbar
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent; // Hiển thị giữa Form cha
            this.Text = "Chỉnh sửa Từ vựng"; // Tiêu đề mặc định
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}