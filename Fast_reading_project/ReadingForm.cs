using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;

namespace Fast_reading_project
{
    public partial class ReadingForm : Form
    {
        private readonly string _filePath;
        private readonly bool _isTongueTwister;
        private Stopwatch _stopwatch;
        private Timer _uiTimer;

        private RichTextBox _rtbText;
        private Label _lblTimer;
        private Label _lblStatus;
        private Button _btnFinish;
        private Button _btnStart;

        private Button _btnZoomIn;
        private Button _btnZoomOut;
        private Label _lblFontSizeHint;

        private bool _isSelectingLastWord = false;
        private int _finalWordCount = 0;

        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        public ReadingForm(string filePath, bool isTongueTwister = false)
        {
            _filePath = filePath;
            _isTongueTwister = isTongueTwister;
            _stopwatch = new Stopwatch();
            _uiTimer = new Timer { Interval = 1000 };

            SetupFormSettings();
            InitializeCustomComponents();

            PrepareText(true);
        }

        private void SetupFormSettings()
        {
            this.Text = "Тренажер скорочтения";
            this.Size = new Size(950, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
        }

        private void InitializeCustomComponents()
        {
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 75,
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White
            };

            _lblTimer = new Label { Text = "Время: 00:00", Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(30, 22), AutoSize = true };
            _lblStatus = new Label { Text = "Настройте шрифт и нажмите 'Начать'", Font = new Font("Segoe UI", 10, FontStyle.Italic), Location = new Point(350, 27), AutoSize = true, ForeColor = Color.DarkGray };

            _btnZoomIn = CreateHeaderButton("+", new Point(180, 15));
            _btnZoomOut = CreateHeaderButton("-", new Point(235, 15));
            _lblFontSizeHint = new Label { Text = "Шрифт", Location = new Point(188, 52), Font = new Font("Segoe UI", 8), AutoSize = true };

            _btnZoomIn.Click += (s, e) => ChangeFontSize(2);
            _btnZoomOut.Click += (s, e) => ChangeFontSize(-2);

            _btnFinish = new Button
            {
                Text = "ЗАКОНЧИТЬ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(130, 40),
                Location = new Point(780, 18),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Cursor = Cursors.Hand
            };
            _btnFinish.Click += BtnFinish_Click;

            pnlHeader.Controls.AddRange(new Control[] { _lblTimer, _lblStatus, _btnFinish, _btnZoomIn, _btnZoomOut, _lblFontSizeHint });

            _rtbText = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 20),
                Padding = new Padding(80),
                Cursor = Cursors.Default
            };

            _rtbText.Text = File.Exists(_filePath) ? File.ReadAllText(_filePath) : "Текст не найден.";

            _rtbText.SelectionChanged += (s, e) => {
                if (_rtbText.SelectionLength > 0) _rtbText.SelectionLength = 0;
                HideCaret(_rtbText.Handle);
            };

            _rtbText.Click += RtbText_Click;

            _btnStart = new Button
            {
                Text = "НАЧАТЬ ЧТЕНИЕ",
                Size = new Size(320, 100),
                Font = new Font("Segoe UI Semibold", 20),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            _btnStart.FlatAppearance.BorderSize = 0;
            ApplyRoundCorners(_btnStart, 30);
            _btnStart.Click += (s, e) => StartReading();

            this.Controls.Add(_btnStart);
            this.Controls.Add(_rtbText);
            this.Controls.Add(pnlHeader);

            _btnStart.BringToFront();

           
            _uiTimer.Tick += (s, e) => {
                _lblTimer.Text = $"Время: {_stopwatch.Elapsed:mm\\:ss}";

                if (_stopwatch.Elapsed.TotalSeconds >= 60)
                {
                    BtnFinish_Click(this, EventArgs.Empty);
                }
            };

            this.Resize += (s, e) => CenterStartButton();
            CenterStartButton();
        }

        private void PrepareText(bool blur)
        {
            _rtbText.SelectAll();
            _rtbText.SelectionColor = blur ? Color.FromArgb(240, 240, 240) : Color.Black;
            if (_isTongueTwister) _rtbText.SelectionAlignment = HorizontalAlignment.Center;
            _rtbText.DeselectAll();
        }

        private Button CreateHeaderButton(string text, Point loc)
        {
            return new Button { Text = text, Location = loc, Size = new Size(45, 35), BackColor = Color.FromArgb(63, 63, 70), FlatStyle = FlatStyle.Flat, Font = new Font("Arial", 12, FontStyle.Bold), ForeColor = Color.White, Cursor = Cursors.Hand };
        }

        private void CenterStartButton()
        {
            _btnStart.Left = (this.ClientSize.Width - _btnStart.Width) / 2;
            _btnStart.Top = (this.ClientSize.Height - _btnStart.Height) / 2;
        }

        private void ChangeFontSize(float delta)
        {
            float newSize = _rtbText.Font.Size + delta;
            if (newSize > 10 && newSize < 80)
            {
                _rtbText.Font = new Font(_rtbText.Font.FontFamily, newSize);
                PrepareText(_btnStart.Visible);
            }
        }

        private void StartReading()
        {
            _btnStart.Visible = false;
            _btnFinish.Enabled = true;
            _btnZoomIn.Visible = false;
            _btnZoomOut.Visible = false;
            _lblFontSizeHint.Visible = false;

            PrepareText(false);

            _lblStatus.Text = "Читайте текст...";
            _lblStatus.ForeColor = Color.LightGreen;
            _stopwatch.Start();
            _uiTimer.Start();
        }

        private void BtnFinish_Click(object sender, EventArgs e)
        {
            // Чтобы избежать повторного срабатывания, если таймер и кнопка сработали одновременно
            if (_isSelectingLastWord) return;

            _stopwatch.Stop();
            _uiTimer.Stop();
            _btnFinish.Enabled = false;

            _isSelectingLastWord = true;
            _lblStatus.Text = "КЛИКНИТЕ НА ПОСЛЕДНЕЕ СЛОВО";
            _lblStatus.ForeColor = Color.Orange;
            _rtbText.Cursor = Cursors.Hand;

            MessageBox.Show("Время вышло или чтение окончено! Теперь кликните на последнее слово, которое успели прочитать.", "Финиш");
        }

        private void RtbText_Click(object sender, EventArgs e)
        {
            if (!_isSelectingLastWord) return;

            int charIndex = _rtbText.GetCharIndexFromPosition(_rtbText.PointToClient(Cursor.Position));
            int endOfWordIndex = charIndex;

            while (endOfWordIndex < _rtbText.Text.Length && !char.IsWhiteSpace(_rtbText.Text[endOfWordIndex]))
            {
                endOfWordIndex++;
            }

            string textProcessed = _rtbText.Text.Substring(0, endOfWordIndex);
            _finalWordCount = textProcessed.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;

            ShowFinalResult();
        }

        private void ShowFinalResult()
        {
            double minutes = _stopwatch.Elapsed.TotalMinutes;
            int wpm = (int)(_finalWordCount / (minutes < 0.01 ? 0.01 : minutes));
            MessageBox.Show($"Скорость: {wpm} слов/мин.", "Итог");
            this.Close();
        }

        private void ApplyRoundCorners(Control control, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(control.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(control.Width - radius, control.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, control.Height - radius, radius, radius, 90, 90);
            control.Region = new Region(path);
        }
    }
}