using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Fast_reading_project
{
    public partial class TongueTwisterForm : Form
    {
        private string _filePath;
        private int _count = 0;
        private Label _lblCount;
        private Label _lblText;

        public TongueTwisterForm(string filePath)
        {
            _filePath = filePath;
            InitializeManualComponents();
        }

        private void InitializeManualComponents()
        {
            this.Text = "Тренировка дикции";
            this.Size = new Size(800, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Panel header = UIStyle.CreateHeader(this, "СКОРОГОВОРКА");

            Button btnBack = new Button
            {
                Text = "← НАЗАД",
                Size = new Size(110, 35),
                Location = new Point(660, 18),
                BackColor = Color.FromArgb(60, 60, 65),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Click += (s, e) => this.Close();
            header.Controls.Add(btnBack);
            this.Controls.Add(header);

            _lblText = new Label
            {
                Location = new Point(40, 100),
                Size = new Size(720, 280),
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = UIStyle.SecondaryColor,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = File.Exists(_filePath) ? File.ReadAllText(_filePath) : "Текст не найден"
            };
            this.Controls.Add(_lblText);

            _lblCount = new Label
            {
                Text = "ПОВТОРЕНО: 0",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = UIStyle.AccentColor,
                Location = new Point(0, 400),
                Size = new Size(800, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(_lblCount);

            Button btnAdd = new Button
            {
                Text = "ПРОИЗНЕС БЕЗ ОШИБОК!",
                Size = new Size(360, 85),
                Location = new Point(220, 490),
                Font = new Font("Segoe UI Semibold", 14)
            };
            UIStyle.ApplyRoundedButton(btnAdd);
            btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(btnAdd);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            _count++;
            if (_count >= 10)
            {
                _lblCount.ForeColor = Color.Gold;
                _lblCount.Text = $"🏆 ПОВТОРЕНО: {_count} 🏆";
            }
            else
            {
                _lblCount.Text = $"ПОВТОРЕНО: {_count}";
                _lblCount.ForeColor = Color.OrangeRed;
                Timer t = new Timer { Interval = 300 };
                t.Tick += (s, args) => {
                    if (_count < 10) _lblCount.ForeColor = UIStyle.AccentColor;
                    t.Stop();
                };
                t.Start();
            }
        }
    }
}