
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

