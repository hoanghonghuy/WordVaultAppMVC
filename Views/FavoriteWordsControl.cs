using System.Windows.Forms;

namespace WordVaultAppMVC.Views.Controls
{
    public class FavoriteWordsControl : UserControl
    {
        public FavoriteWordsControl()
        {
            this.Dock = DockStyle.Fill;
            var form = new FavoriteWordsForm()
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill
            };
            this.Controls.Add(form);
            form.Show();
        }
    }
}