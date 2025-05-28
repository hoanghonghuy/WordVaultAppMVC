using System;
using System.Windows.Forms;
using WordVaultAppMVC.Models;

namespace WordVaultAppMVC.Views
{
    public partial class VocabularyDetailPanel : UserControl
    {
        public VocabularyDetailPanel()
        {
            InitializeComponent();
        }

        // Phương thức để hiển thị thông tin của một từ vựng
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
                lblWord.Text = "Từ: " + vocab.Word;
                lblMeaning.Text = "Nghĩa: " + vocab.Meaning;
                lblPronunciation.Text = "Phát âm: " + vocab.Pronunciation;
                lblAudioUrl.Text = "Audio URL: " + vocab.AudioUrl;
            }
        }
    }
}
