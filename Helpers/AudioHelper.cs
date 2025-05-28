using System;
using WMPLib; // Windows Media Player library (cần thêm COM reference)
using System.Windows.Forms; // Cần cho MessageBox
using System.Diagnostics; // Cần cho Debug

namespace WordVaultAppMVC.Helpers
{
    /// <summary>
    /// Lớp tiện ích tĩnh để xử lý việc phát âm thanh từ URL.
    /// Sử dụng Windows Media Player COM component.
    /// </summary>
    public static class AudioHelper
    {
        #region Private Static Fields

        // Khởi tạo một instance duy nhất của WindowsMediaPlayer để tái sử dụng.
        // Điều này tránh việc tạo đối tượng mới mỗi lần phát âm thanh.
        private static WindowsMediaPlayer _player = new WindowsMediaPlayer();

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Phát âm thanh từ một URL được cung cấp.
        /// </summary>
        /// <param name="audioUrl">URL của file âm thanh cần phát (ví dụ: .mp3, .wav).</param>
        public static void PlayAudio(string audioUrl)
        {
            // Kiểm tra xem URL có hợp lệ không.
            if (string.IsNullOrEmpty(audioUrl))
            {
                Debug.WriteLine("[WARN] PlayAudio: audioUrl is null or empty.");
                // Thông báo cho người dùng thay vì chỉ return.
                // Cân nhắc không hiển thị MessageBox nếu việc không có URL là bình thường.
                MessageBox.Show("URL âm thanh không hợp lệ hoặc không được cung cấp.", "Thiếu URL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Gán URL cho player.
                _player.URL = audioUrl;
                // Bắt đầu phát.
                _player.controls.play();
                Debug.WriteLine($"[INFO] Playing audio from: {audioUrl}");
            }
            catch (System.Runtime.InteropServices.COMException comEx) // Bắt lỗi COM cụ thể
            {
                Debug.WriteLine($"[ERROR] Lỗi COM khi phát âm thanh từ '{audioUrl}': {comEx.Message}");
                MessageBox.Show($"Đã xảy ra lỗi COM khi cố gắng phát âm thanh.\nChi tiết: {comEx.Message}", "Lỗi Phát Âm Thanh (COM)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) // Bắt các lỗi khác
            {
                Debug.WriteLine($"[ERROR] Lỗi không xác định khi phát âm thanh từ '{audioUrl}': {ex.Message}");
                MessageBox.Show($"Lỗi không xác định khi phát âm thanh: {ex.Message}", "Lỗi Phát Âm Thanh", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Trong ứng dụng thực tế, nên ghi log chi tiết lỗi thay vì chỉ hiển thị MessageBox.
            }
        }

        #endregion

        // Cân nhắc thêm phương thức StopAudio() nếu cần
        // public static void StopAudio()
        // {
        //     try { _player?.controls?.stop(); } catch { /* Ignore stop errors */ }
        // }
    }
}