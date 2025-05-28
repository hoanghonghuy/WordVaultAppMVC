using System.Windows.Forms;

namespace WordVaultAppMVC.Views.Controls
{
    public class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            this.Dock = DockStyle.Fill;
            Label lbl = new Label
            {
                Text = "⚙️ Cài đặt ứng dụng (đang phát triển)",
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lbl);
        }
    }
}