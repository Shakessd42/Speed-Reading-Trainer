using System;
using System.Drawing;
using System.Windows.Forms;

namespace Fast_reading_project
{
    public partial class ActivitySelectionForm : Form
    {
        private int _grade;

        public ActivitySelectionForm(int grade)
        {
            _grade = grade;
            InitializeManualComponents();
        }

        private void InitializeManualComponents()
        {
            //Настройки окна
            this.Text = $"{_grade} Класс - Выбор задания";
            this.Size = new Size(500, 650);
            this.BackColor = UIStyle.BackgroundColor;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            //Шапка
            this.Controls.Add(UIStyle.CreateHeader(this, $"КЛАСС: {_grade}"));

            //Список активностей
            string[] activities = { "Тексты", "Скороговорки", "Филворды", "Модификации" };

            int startY = 120;
            int buttonWidth = 350;
            int buttonHeight = 80;
            int spacing = 20;

            foreach (string activity in activities)
            {
                //Если это 1 или 2 класс, пропускаем создание кнопки Модификации
                if ((_grade == 1 || _grade == 2) && activity == "Модификации")
                {
                    continue;
                }

                Button btn = new Button
                {
                    Text = activity.ToUpper(),
                    Size = new Size(buttonWidth, buttonHeight),
                    Location = new Point((this.ClientSize.Width - buttonWidth) / 2, startY),
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };

                UIStyle.ApplyRoundedButton(btn);
                btn.TextAlign = ContentAlignment.MiddleCenter;

                btn.Click += (s, e) =>
                {
                    this.Hide();
                    new ContentSelectionForm(_grade, activity).ShowDialog();
                    this.Show();
                };

                this.Controls.Add(btn);
                startY += buttonHeight + spacing;
            }

            //Кнопка Назад
            Button btnBack = new Button
            {
                Text = "← НАЗАД К КЛАССАМ",
                Size = new Size(200, 40),
                Location = new Point((this.ClientSize.Width - 200) / 2, 540),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9)
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Click += (s, e) => this.Close();
            this.Controls.Add(btnBack);
        }
    }
}