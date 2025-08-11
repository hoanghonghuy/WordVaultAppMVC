
# WordVault - Personal Dictionary (WinForms MVC) v1

Welcome to WordVault, a personal dictionary application built with C# and Windows Forms, following a near MVC architectural model. This application helps you look up, store, manage, and review English vocabulary effectively.

## Main Features

* **Vocabulary Lookup:** Search English words using a public dictionary API (`dictionaryapi.dev`).
* **Detailed Display:** View Vietnamese meanings (translated via `mymemory.translated.net`), phonetics (if available from the API), and listen to pronunciations (if audio links are available).
* **Personal Storage:** Save looked-up vocabulary into a local SQL Server database.
* **Topic Management:**
    * Create and manage vocabulary topics (including sample TOEIC topics).
    * Add/Remove words to specific topics.
    * Browse and search words within a topic.
    * Edit vocabulary information directly.
* **Favorite Words:** Mark and review your favorite vocabulary.
* **Flashcard Review:**
    * **Daily Review:** Select the number of words to review using flashcard method.
    * **Random Study:** Review random words from the entire dictionary.
    * Mark as "Remembered" / "Not Remembered" to update learning status (logic exists and can be extended).
* **Quiz:** Take multiple-choice quizzes based on saved vocabulary (choose the correct meaning).
* **Settings:**
    * Customize default number of words/questions for review and quiz.
    * (Optional) Enable/disable automatic audio playback.
    * Data management: Backup, Restore database, Clear learning history.
* **Responsive UI:** Main screens use `TableLayoutPanel` and `FlowLayoutPanel` for better resizing behavior.

## Project Structure

WordVaultAppMVC/
│
├── 📁 Controllers/         # Handles logic, coordination between View and Data/Service
│   ├── VocabularyController.cs
│   ├── TopicController.cs
│   ├── QuizController.cs
│   └── LearningController.cs
│
├── 📁 Views/               # Contains all user interface components
│   │
│   ├── 📁 Controls/        # UserControls (reusable small UI components)
│   │   ├── HomeControl.cs
│   │   ├── TopicVocabularyControl.cs
│   │   ├── FavoriteWordsControl.cs
│   │   ├── DailyReviewControl.cs
│   │   ├── ShuffleStudyControl.cs
│   │   ├── QuizControl.cs
│   │   ├── SettingsControl.cs
│   │   └── VocabularyDetailPanel.cs
│   │
│   └── 📁 Forms/           # Main or sub dialog forms
│       ├── MainForm.cs
│       ├── AddToTopicForm.cs
│       ├── VocabularyListForm.cs
│       ├── ResultSummaryForm.cs
│       └── EditVocabularyForm.cs
│
├── 📁 Models/              # Data objects (POCO)
│   ├── Vocabulary.cs
│   ├── Topic.cs
│   └── ...
│
├── 📁 Data/                # Data access (Repositories)
│   ├── DatabaseContext.cs
│   ├── VocabularyRepository.cs
│   ├── TopicRepository.cs
│   └── ...
│
├── 📁 Services/            # Business logic (Backup/Restore, etc.)
│   ├── VocabularyService.cs
│   └── DataService.cs
│
├── 📁 Helpers/             # Utility classes
│   ├── AudioHelper.cs
│   └── DictionaryApiClient.cs
│
├── 📁 Resources/           # Store resources (icons, images, sounds...)
│
├── 📄 Program.cs           # Application entry point
├── 📄 App.config           # Contains connection string and other settings
└── 📄 WordVaultAppMVC.csproj # Project file


## Technologies Used

* **Language:** C#
* **Framework:** .NET Framework 4.8
* **UI:** Windows Forms (WinForms)
* **Database:** SQL Server (Using ADO.NET: `SqlConnection`, `SqlCommand`, `SqlDataReader`)
* **External Libraries:**
    * Newtonsoft.Json (usually via NuGet): For parsing JSON from APIs.
    * Interop.WMPLib (via COM Reference): To play audio via Windows Media Player.
* **APIs:**
    * `https://api.dictionaryapi.dev/`: English-English dictionary lookup.
    * `https://api.mymemory.translated.net/`: Translation to Vietnamese.

## Installation & Testing

1. **Requirements:**
    * .NET Framework 4.8 (or compatible version).
    * Microsoft SQL Server (Express version is sufficient).
    * Visual Studio (to open and build the project).

2. **Database Setup:**
    * Open SQL Server Management Studio (SSMS).
    * Connect to your SQL Server instance (e.g., `DataSource\SQLEXPRESS`).
    * Create a new database named `WordVaultDb` (or a preferred name).
    * Open a new Query window for `WordVaultDb`.
    * Copy and run all SQL script content (from `SQLServer:` in file `WordVaultAppMVC.txt`) to create required tables (`Vocabulary`, `Topics`, `VocabularyTopic`, `QuizQuestions`, `QuizResults`, `LearningStatuses`, `FavoriteWords`).
    * (Optional) Run additional script for sample TOEIC data.

3. **Connection String Configuration:**
    * Open `App.config` in the project.
    * Locate the `<connectionStrings>` section.
    * Edit the `connectionString` value for `WordVaultDb` to match your SQL Server configuration:
        * **`Data Source`**: SQL Server instance name (e.g., `DataSource\SQLEXPRESS`, `(localdb)\MSSQLLocalDB`, or `.` for default instance).
        * **`Initial Catalog`**: Your database name (e.g., `WordVaultDb`).
        * **Authentication:**
            * **Windows Authentication (recommended for local):** Ensure `Integrated Security=True;`. Omit `User ID=...;Password=...;`. Windows account running the app must have DB access.
            ```xml
            <add name="WordVaultDb" connectionString="Data Source=DataSource\SQLEXPRESS;Initial Catalog=WordVaultDb;Integrated Security=True" providerName="System.Data.SqlClient"/>
            ```
            * **SQL Server Authentication:** Provide `User ID=your_sql_username;Password=your_sql_password;` and remove `Integrated Security=True;`. Ensure the SQL user has database permissions.
            ```xml
            <add name="WordVaultDb" connectionString="Data Source=DataSource\SQLEXPRESS;Initial Catalog=WordVaultDb;User ID=userId;Password=password" providerName="System.Data.SqlClient"/>
            ```

4. **Build & Run:**
    * Open the project in Visual Studio.
    * Build the project (Build -> Build Solution or Ctrl+Shift+B).
    * Run the application (Debug -> Start Debugging or F5).

## Application Settings

* Default settings for review/quiz word counts can be changed in the **Settings** screen. These values are saved in the user's `*.settings` file.
* The current APIs used are free to access.

## Usage

* Launch the application (run the `.exe` after building or launch from Visual Studio).
* Use buttons on the toolbar (ToolStrip) at the top of `MainForm` to navigate: Home, Topic Vocabulary, Settings, Favorites, Study, Quiz, Shuffle.
* **Home:** Enter English word in the search box and press Enter or click "Search". Result (meaning, phonetics, audio) will appear. Function buttons (Listen, Favorite, Add to topic) will also show.
* **Topic Vocabulary:** Select a topic from ComboBox. Word list will appear. Enter word in search box and press Enter or click "Find" to search in the current list. Select a word and click "Edit" or "Delete" to manage.
* **Settings:** Change preferences and click "Save Settings". Use data management buttons (requires backup/restore/clear logic implemented).
* Other screens: Follow instructions on the UI.

---
