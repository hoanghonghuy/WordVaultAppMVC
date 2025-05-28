using System;
using System.Windows.Forms;
using WordVaultAppMVC.Models;
using System.Drawing; // Thêm using này nếu chưa có

namespace WordVaultAppMVC.Views
{
    // Đảm bảo khai báo 'partial class' để liên kết với file .Designer.cs
    public partial class VocabularyDetailPanel : UserControl
    {
        // Các biến control sẽ được tự động khai báo trong file .Designer.cs
        // private System.Windows.Forms.TableLayoutPanel detailTableLayout;
        // private System.Windows.Forms.Label lblWord;
        // private System.Windows.Forms.Label lblMeaning;
        // private System.Windows.Forms.Label lblPronunciation;
        // private System.Windows.Forms.Label lblAudioUrl;

        public VocabularyDetailPanel()
        {
            InitializeComponent(); // Gọi hàm InitializeComponent từ file .Designer.cs
        }

        // Phương thức để hiển thị thông tin của một từ vựng (Giữ nguyên logic)
        public void DisplayVocabulary(Vocabulary vocab)
        {
            if (vocab == null)
            {
                lblWord.Text = "Từ: ";
                lblMeaning.Text = "Nghĩa: ";
                lblPronunciation.Text = "Phát âm: ";
                lblAudioUrl.Text = "Audio URL: ";
            }
            else
            {
                // Sử dụng toán tử ?? để xử lý null phòng trường hợp data bị thiếu
                lblWord.Text = "Từ: " + (vocab.Word ?? "N/A");
                lblMeaning.Text = "Nghĩa: " + (vocab.Meaning ?? "N/A");
                lblPronunciation.Text = "Phát âm: " + (vocab.Pronunciation ?? "N/A");
                lblAudioUrl.Text = "Audio URL: " + (vocab.AudioUrl ?? "N/A");
            }
            // Gọi hàm điều chỉnh layout sau khi cập nhật text
            AdjustLabelLayout();
        }

        // Hàm phụ trợ để điều chỉnh layout label
        private void AdjustLabelLayout()
        {
            // Kiểm tra detailTableLayout đã được khởi tạo chưa (phòng trường hợp lỗi)
            if (this.detailTableLayout != null)
            {
                // Yêu cầu TableLayoutPanel cập nhật lại layout của nó và các control con
                this.detailTableLayout.PerformLayout();
            }
        }

        // Các phương thức xử lý sự kiện khác (nếu có) giữ nguyên ở đây...

    }
}