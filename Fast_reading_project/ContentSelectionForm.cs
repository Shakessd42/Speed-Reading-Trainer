using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Fast_reading_project
{
    public partial class ContentSelectionForm : Form
    {
        private int _grade;
        private string _type;

        public ContentSelectionForm(int grade, string type)
        {
            _grade = grade;
            _type = type;
            InitializeManualComponents();
        }

        private void InitializeManualComponents()
        {
            this.Text = $"Выбор: {_type}";
            this.Size = new Size(550, 800);
            this.BackColor = UIStyle.BackgroundColor;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Кнопка НАЗАД (внизу)
            Button btnBack = new Button
            {
                Text = "ВЕРНУТЬСЯ В МЕНЮ",
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.FromArgb(220, 220, 225),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 11)
            };
            btnBack.Click += (s, e) => this.Close();
            this.Controls.Add(btnBack);

            // Шапка
            this.Controls.Add(UIStyle.CreateHeader(this, _type.ToUpper()));

            // Список файлов
            FlowLayoutPanel flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(35, 95, 10, 20),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", $"Grade{_grade}", GetSubFolder(_type));

            if (Directory.Exists(path))
            {
                foreach (var file in Directory.GetFiles(path, "*.txt"))
                {
                    Panel pnl = new Panel { Size = new Size(460, 65), BackColor = Color.White, Margin = new Padding(0, 5, 0, 5) };
                    Button btn = new Button
                    {
                        Text = "  📖  " + Path.GetFileNameWithoutExtension(file),
                        Dock = DockStyle.Fill,
                        FlatStyle = FlatStyle.Flat,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Font = new Font("Segoe UI Semibold", 11)
                    };
                    btn.Click += (s, e) => { this.Hide(); OpenForm(file); this.Show(); };
                    pnl.Controls.Add(btn);
                    flow.Controls.Add(pnl);
                }
            }

            this.Controls.Add(flow);
            flow.BringToFront();
        }

        private string GetSubFolder(string t) => t == "Тексты" ? "Texts" : t == "Скороговорки" ? "TongueTwisters" : t == "Филворды" ? "WordSearch" : "Modifications";

        private void OpenForm(string f)
        {
            if (_type == "Филворды") new WordSearchForm(f).ShowDialog();
            else if (_type == "Скороговорки") new TongueTwisterForm(f).ShowDialog();
            else if (_type == "Модификации") new ModifiedReadingForm(f).ShowDialog();
            else new ReadingForm(f, false).ShowDialog();
        }
    }
}