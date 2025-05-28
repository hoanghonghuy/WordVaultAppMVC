# WordVault - Từ điển Cá nhân (WinForms MVC) v1

Chào mừng bạn đến với WordVault, ứng dụng từ điển cá nhân được xây dựng bằng C# và Windows Forms, theo mô hình kiến trúc gần giống MVC. Ứng dụng này giúp bạn tra cứu, lưu trữ, quản lý và ôn tập từ vựng tiếng Anh một cách hiệu quả.

## Tính năng chính

* **Tra cứu từ vựng:** Tìm kiếm từ tiếng Anh thông qua API từ điển công cộng (`dictionaryapi.dev`).
* **Hiển thị chi tiết:** Xem nghĩa tiếng Việt (dịch tự động qua `mymemory.translated.net`), phiên âm (nếu có từ API), và nghe phát âm (nếu có link audio).
* **Lưu trữ cá nhân:** Lưu từ vựng đã tra cứu vào cơ sở dữ liệu SQL Server cục bộ.
* **Quản lý theo chủ đề:**
    * Tạo và quản lý các chủ đề từ vựng (bao gồm các chủ đề TOEIC mẫu).
    * Thêm/Xóa từ vào các chủ đề cụ thể.
    * Duyệt và tìm kiếm từ trong một chủ đề.
    * Chỉnh sửa thông tin từ vựng trực tiếp.
* **Từ yêu thích:** Đánh dấu và xem lại danh sách các từ vựng yêu thích.
* **Ôn tập Flashcard:**
    * **Ôn tập hàng ngày:** Lựa chọn số lượng từ để ôn tập theo phương pháp lật thẻ (flashcard).
    * **Học ngẫu nhiên:** Ôn tập ngẫu nhiên các từ trong toàn bộ từ điển.
    * Đánh dấu "Đã nhớ" / "Chưa nhớ" để cập nhật trạng thái học tập (Logic cập nhật trạng thái đã có, có thể tích hợp sâu hơn).
* **Kiểm tra (Quiz):** Làm bài kiểm tra trắc nghiệm dựa trên từ vựng đã lưu (chọn nghĩa đúng).
* **Cài đặt:**
    * Tùy chỉnh số lượng từ/câu hỏi mặc định cho ôn tập và quiz.
    * (Tùy chọn) Bật/tắt tự động phát âm thanh.
    * Quản lý dữ liệu: Sao lưu, Phục hồi cơ sở dữ liệu, Xóa lịch sử học tập.
* **Giao diện Responsive:** Các màn hình chính được thiết kế bằng `TableLayoutPanel`, `FlowLayoutPanel` để co giãn tốt hơn khi thay đổi kích thước cửa sổ.

## Cấu trúc Dự án

WordVaultAppMVC/
│
├── 📁 Controllers/           # Xử lý logic, điều phối giữa View và Data/Service
│   ├── VocabularyController.cs
│   ├── TopicController.cs
│   ├── QuizController.cs
│   └── LearningController.cs
│
├── 📁 Views/                 # Chứa tất cả các thành phần giao diện người dùng
│   │
│   ├── 📁 Controls/          # Các UserControl (thành phần UI nhỏ, tái sử dụng)
│   │   ├── HomeControl.cs             # Màn hình chính (tìm kiếm, hiển thị)
│   │   ├── TopicVocabularyControl.cs  # Quản lý từ theo chủ đề
│   │   ├── FavoriteWordsControl.cs    # Hiển thị từ yêu thích
│   │   ├── DailyReviewControl.cs      # Ôn tập hàng ngày (flashcard)
│   │   ├── ShuffleStudyControl.cs     # Học ngẫu nhiên (flashcard)
│   │   ├── QuizControl.cs             # Chức năng kiểm tra
│   │   ├── SettingsControl.cs         # Cài đặt ứng dụng
│   │   └── VocabularyDetailPanel.cs   # Panel hiển thị chi tiết từ
│   │
│   └── 📁 Forms/             # Các cửa sổ Form chính hoặc phụ (dialog)
│       ├── MainForm.cs                # Form chính chứa các Controls
│       ├── AddToTopicForm.cs          # Form dialog thêm từ vào chủ đề
│       ├── VocabularyListForm.cs      # Form hiển thị danh sách (ít dùng)
│       ├── ResultSummaryForm.cs       # Form hiển thị kết quả Quiz
│       └── EditVocabularyForm.cs      # Form chỉnh sửa từ vựng
│
├── 📁 Models/                # Định nghĩa các đối tượng dữ liệu (POCO)
│   ├── Vocabulary.cs
│   ├── Topic.cs
│   ├── VocabularyTopic.cs     # Model cho bảng trung gian
│   ├── LearningStatus.cs
│   ├── QuizResult.cs
│   └── WordDetails.cs         # Model cho dữ liệu trả về từ API
│
├── 📁 Data/                  # Truy cập dữ liệu (Repositories, DbContext)
│   ├── DatabaseContext.cs       # Quản lý kết nối DB
│   ├── VocabularyRepository.cs  # CRUD từ, xử lý Favorite
│   ├── TopicRepository.cs       # CRUD chủ đề, xử lý liên kết từ-chủ đề
│   ├── QuizRepository.cs
│   └── LearningStatusRepository.cs
│
├── 📁 Services/              # Chứa logic nghiệp vụ (Backup/Restore, Lấy từ ngẫu nhiên)
│   ├── VocabularyService.cs
│   └── DataService.cs
│
├── 📁 Helpers/               # Các lớp tiện ích chung
│   ├── AudioHelper.cs           # Hỗ trợ phát audio (WMPLib)
│   └── DictionaryApiClient.cs   # Tương tác với API từ điển (Newtonsoft.Json)
│
├── 📁 Resources/             # Lưu trữ tài nguyên (icon, ảnh, âm thanh...)
│   └── (Trống hoặc chứa tài nguyên của bạn)
│
├── 📄 Program.cs             # Điểm khởi chạy ứng dụng
├── 📄 App.config             # Chứa connection string và cấu hình khác
├── 📄 Settings.settings       # File lưu cài đặt ứng dụng
├── 📄 Settings.Designer.cs    # File code-behind cho Settings.settings
└── 📄 WordVaultAppMVC.csproj   # File project

## Công nghệ sử dụng

* **Ngôn ngữ:** C#
* **Framework:** .NET Framework 4.8 
* **Giao diện:** Windows Forms (WinForms)
* **Cơ sở dữ liệu:** SQL Server (Sử dụng ADO.NET: `SqlConnection`, `SqlCommand`, `SqlDataReader`)
* **Thư viện ngoài:**
    * Newtonsoft.Json (Thường được cài đặt qua NuGet): Để xử lý dữ liệu JSON từ API.
    * Interop.WMPLib (Thường được thêm qua COM Reference): Để phát âm thanh qua Windows Media Player.
* **APIs:**
    * `https://api.dictionaryapi.dev/`: Tra cứu từ điển Anh-Anh.
    * `https://api.mymemory.translated.net/`: Dịch nghĩa sang tiếng Việt.

## Cài đặt và Chạy thử

1.  **Yêu cầu:**
    * .NET Framework 4.8 (hoặc phiên bản tương thích với project).
    * Microsoft SQL Server (Phiên bản Express là đủ).
    * Visual Studio (để mở và build project).

2.  **Thiết lập Cơ sở dữ liệu:**
    * Mở SQL Server Management Studio (SSMS).
    * Kết nối đến instance SQL Server của bạn (ví dụ: `DataSource\SQLEXPRESS`).
    * Tạo một database mới tên là `WordVaultDb` (hoặc tên bạn muốn).
    * Mở một cửa sổ Query mới cho database `WordVaultDb`.
    * Sao chép và chạy toàn bộ nội dung script SQL tạo bảng (phần `SQLServer:` trong file `WordVaultAppMVC.txt` [source: 2401-2411]) để tạo các bảng cần thiết (`Vocabulary`, `Topics`, `VocabularyTopic`, `QuizQuestions`, `QuizResults`, `LearningStatuses`, `FavoriteWords`).
    * (Tùy chọn) Chạy script SQL bổ sung dữ liệu TOEIC mẫu nếu muốn có dữ liệu ban đầu.

3.  **Cấu hình Connection String:**
    * Mở file `App.config` trong project.
    * Tìm đến phần `<connectionStrings>`.
    * Chỉnh sửa giá trị `connectionString` của `WordVaultDb` cho phù hợp với cấu hình SQL Server của bạn:
        * **`Data Source`**: Tên instance SQL Server (ví dụ: `DataSource\SQLEXPRESS`, `(localdb)\MSSQLLocalDB`, `.` nếu là default instance).
        * **`Initial Catalog`**: Tên database bạn đã tạo (ví dụ: `WordVaultDb`).
        * **Xác thực:**
            * **Windows Authentication (Khuyến nghị nếu chạy local):** Đảm bảo có `Integrated Security=True;`. Bạn có thể bỏ phần `User ID=...;Password=...;`. Tài khoản Windows chạy ứng dụng cần có quyền truy cập vào database.
            ```xml
            <add name="WordVaultDb" connectionString="Data Source=DataSource\SQLEXPRESS;Initial Catalog=WordVaultDb;Integrated Security=True" providerName="System.Data.SqlClient"/>
            ```
            * **SQL Server Authentication:** Đảm bảo có `User ID=your_sql_username;Password=your_sql_password;` và bỏ `Integrated Security=True;`. User SQL này cần được tạo và cấp quyền trên database.
            ```xml
            <add name="WordVaultDb" connectionString="Data Source=DataSource\SQLEXPRESS;Initial Catalog=WordVaultDb;User ID=userId;Password=password" providerName="System.Data.SqlClient"/>
            ```

4.  **Build và Chạy:**
    * Mở project bằng Visual Studio.
    * Build project (Build -> Build Solution hoặc Ctrl+Shift+B).
    * Chạy ứng dụng (Debug -> Start Debugging hoặc F5).

## Cấu hình Ứng dụng

* Các cài đặt như số từ ôn tập/quiz mặc định có thể được thay đổi trong màn hình **Settings** của ứng dụng. Các giá trị này được lưu trong file `*.settings` của người dùng.
* Các API được sử dụng hiện tại là miễn phí.

## Sử dụng

* Khởi chạy ứng dụng (chạy file `.exe` sau khi build hoặc chạy từ Visual Studio).
* Sử dụng các nút trên thanh công cụ (ToolStrip) ở đầu cửa sổ chính (`MainForm`) để điều hướng giữa các chức năng: Home, Topic Vocabulary, Settings, Yêu thích, Học từ, Quiz, Xáo từ.
* **Home:** Nhập từ tiếng Anh vào ô tìm kiếm và nhấn Enter hoặc nút "Tìm kiếm". Kết quả (nghĩa, phiên âm, audio) sẽ hiển thị. Các nút chức năng (Nghe, Yêu thích, Thêm vào chủ đề) sẽ hiện ra.
* **Topic Vocabulary:** Chọn chủ đề từ ComboBox. Danh sách từ trong chủ đề sẽ hiện ra. Nhập từ vào ô tìm kiếm và nhấn "Tìm" hoặc Enter để tìm từ trong danh sách hiện tại. Chọn một từ và nhấn "Sửa" hoặc "Xóa" để quản lý.
* **Settings:** Thay đổi các cài đặt và nhấn "Lưu Cài đặt". Sử dụng các nút quản lý dữ liệu (cần cài đặt logic backup/restore/clear).
* Các màn hình khác: Làm theo hướng dẫn trên giao diện.

---


