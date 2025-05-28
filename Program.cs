using System;
using System.Collections.Generic; // Cần cho List nếu dùng trong Program.cs
using System.Linq; // Cần cho args.Contains
using System.Threading.Tasks;
using System.Windows.Forms;
using WordVaultAppMVC.Views; // Namespace của MainForm
using WordVaultAppMVC.Services;
using System.Diagnostics; // Namespace của VocabularyExporterService

namespace WordVaultAppMVC
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) // Đảm bảo có tham số string[] args
        {
            // --- KIỂM TRA THAM SỐ DÒNG LỆNH ---
            // Sử dụng StringComparer.OrdinalIgnoreCase để không phân biệt hoa thường
            if (args != null && args.Contains("--export-vocab", StringComparer.OrdinalIgnoreCase))
            {
                // Nếu có tham số --export-vocab, thực hiện xuất file
                try
                {
                    // Hiển thị thông báo bắt đầu (tùy chọn)
                    MessageBox.Show("Đang bắt đầu quá trình xuất dữ liệu từ vựng...", "Xuất dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Gọi phương thức xuất file từ Service
                    VocabularyExporterService.ExportAllVocabularyToFile();
                }
                catch (Exception ex)
                {
                    // Bắt lỗi tổng quát nếu có gì đó sai sót nghiêm trọng
                    MessageBox.Show($"Lỗi không mong muốn trong quá trình xuất:\n {ex.Message}", "Lỗi Nghiêm trọng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Debug.WriteLine($"[Program.Main --export-vocab] Unhandled exception: {ex}");
                }
                // Kết thúc chương trình sau khi xuất file, không chạy MainForm
                return;
            }

            // --- CHẠY ỨNG DỤNG BÌNH THƯỜNG ---
            // Nếu không có tham số đặc biệt, chạy ứng dụng WinForms như cũ
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}