using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Fast_reading_project
{
    public static class UIStyle
    {
        public static Color PrimaryColor = Color.FromArgb(46, 204, 113);   // Зеленый
        public static Color SecondaryColor = Color.FromArgb(45, 45, 48); // Темный графит
        public static Color BackgroundColor = Color.FromArgb(242, 242, 247); // Светлый фон
        public static Color AccentColor = Color.FromArgb(52, 152, 219);  // ГОЛУБОЙ (исправлено)

        public static void ApplyRoundedButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = PrimaryColor;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI Semibold", 12);
            btn.Cursor = Cursors.Hand;

            GraphicsPath path = new GraphicsPath();
            int r = 20;
            path.AddArc(0, 0, r, r, 180, 90);
            path.AddArc(btn.Width - r, 0, r, r, 270, 90);
            path.AddArc(btn.Width - r, btn.Height - r, r, r, 0, 90);
            path.AddArc(0, btn.Height - r, r, r, 90, 90);
            btn.Region = new Region(path);
        }

        public static Panel CreateHeader(Form form, string title)
        {
            Panel pnl = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = SecondaryColor };
            Label lbl = new Label
            {
                Text = title,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 18)
            };
            pnl.Controls.Add(lbl);
            return pnl;
        }
    }
}