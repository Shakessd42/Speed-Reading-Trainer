using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Fast_reading_project
{
    public partial class ModifiedReadingForm : Form
    {
        private string _originalText;
        private RichTextBox _rtb;

        public ModifiedReadingForm(string filePath)
        {
            _originalText = File.Exists(filePath) ? File.ReadAllText(filePath) : "Текст не найден";
            SetupForm();
        }

        private void SetupForm()
        {
            this.Text = "Тренажер модификаций текста";
            this.Size = new Size(1100, 820);
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Шапка
            Panel header = UIStyle.CreateHeader(this, "МОДИФИКАЦИИ ТЕКСТА");
            this.Controls.Add(header);

            // Панель фильтров
            FlowLayoutPanel pnlFilters = new FlowLayoutPanel
            {
                Location = new Point(50, 95),
                Size = new Size(1000, 70), 
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false 
            };

            string[] filters = { "Обычный", "Цифры", "Анаграмма", "Без пробелов", "Хакер" };

            foreach (var filter in filters)
            {
                Button btn = new Button
                {
                    Text = filter.ToUpper(),
                    Size = new Size(180, 50),
                    Margin = new Padding(0, 0, 15, 0),
                    Font = new Font("Segoe UI Semibold", 10),
                    Cursor = Cursors.Hand
                };
                UIStyle.ApplyRoundedButton(btn);
                btn.Click += (s, e) => ApplyModifier(filter);
                pnlFilters.Controls.Add(btn);
            }
            this.Controls.Add(pnlFilters);

            // Текстовое поле
            _rtb = new RichTextBox
            {
                Location = new Point(50, 170),
                Size = new Size(985, 520),
                Font = new Font("Segoe UI", 20),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(248, 249, 252),
                Text = _originalText
            };
            ApplyTextPadding();
            this.Controls.Add(_rtb);

            // Кнопка закрытия
            Button btnClose = new Button
            {
                Text = "ЗАКОНЧИТЬ ТРЕНИРОВКУ",
                Size = new Size(320, 55),
                Location = new Point((this.ClientSize.Width - 320) / 2, 710), // Авто-центровка
                BackColor = Color.FromArgb(230, 230, 235),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 11)
            };
            UIStyle.ApplyRoundedButton(btnClose);
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void ApplyModifier(string type)
        {
            switch (type)
            {
                case "Обычный": _rtb.Text = _originalText; break;
                case "Цифры": _rtb.Text = ToNumbers(_originalText); break;
                case "Анаграмма": _rtb.Text = Scramble(_originalText); break;
                case "Без пробелов": _rtb.Text = _originalText.Replace(" ", "").Replace("\n", "").Replace("\r", ""); break;
                case "Хакер": _rtb.Text = RandomCase(_originalText); break;
            }
            ApplyTextPadding();
        }

        private void ApplyTextPadding()
        {
            _rtb.SelectAll();
            _rtb.SelectionIndent = 25;
            _rtb.SelectionRightIndent = 25;
            _rtb.DeselectAll();
        }

        private string ToNumbers(string t) => t.ToLower()
            .Replace("о", "0").Replace("е", "3").Replace("а", "4").Replace("и", "1").Replace("т", "7");

        private string Scramble(string text)
        {
            Random r = new Random();
            return string.Join(" ", text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Select(word => {
                if (word.Length < 4) return word;
                char last = word[word.Length - 1];
                bool hasPunct = char.IsPunctuation(last);
                string w = hasPunct ? word.Substring(0, word.Length - 1) : word;
                if (w.Length < 4) return word;
                char[] mid = w.Substring(1, w.Length - 2).ToCharArray();
                return w[0] + new string(mid.OrderBy(x => r.Next()).ToArray()) + w[w.Length - 1] + (hasPunct ? last.ToString() : "");
            }));
        }

        private string RandomCase(string text)
        {
            Random r = new Random();
            return new string(text.Select(c => r.Next(2) == 0 ? char.ToUpper(c) : char.ToLower(c)).ToArray());
        }
    }
}