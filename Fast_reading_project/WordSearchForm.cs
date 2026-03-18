using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;

namespace Fast_reading_project
{
    public partial class WordSearchForm : Form
    {
        private DataGridView _grid;
        private List<string> _wordsToFind;
        private HashSet<string> _foundWords = new HashSet<string>();
        private List<Point> _currentSelection = new List<Point>();

        private Label _lblWordsList;
        private Label _lblStatus;
        private const int _gridSize = 10;
        private Random _rnd = new Random();

        public WordSearchForm(string filePath)
        {
            // Загрузка слов с ограничением, чтобы они влезли в сетку 10x10
            if (File.Exists(filePath))
            {
                _wordsToFind = File.ReadAllLines(filePath)
                                   .Where(s => !string.IsNullOrWhiteSpace(s))
                                   .Select(s => s.Trim().ToUpper())
                                   .Take(8) // Оптимально 8 слов для такой сетки
                                   .ToList();
            }
            else
            {
                _wordsToFind = new List<string> { "ШКОЛА", "КЛАСС", "КНИГА", "УРОК" };
            }

            SetupForm();
            InitializeGrid();
            GeneratePuzzle();
            UpdateWordsLabel();
        }

        private void SetupForm()
        {
            this.Text = "Филворды — Тренировка внимательности";
            this.Size = new Size(1000, 750);
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // Запрет растягивания
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            _lblStatus = new Label
            {
                Dock = DockStyle.Top,
                Height = 60,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "Выбирайте буквы слова по порядку!",
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White
            };

            Panel pnlRight = new Panel
            {
                Dock = DockStyle.Right,
                Width = 260,
                BackColor = Color.FromArgb(245, 245, 247),
                Padding = new Padding(20)
            };

            _lblWordsList = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(64, 64, 64),
                Text = "СПИСОК СЛОВ:"
            };

            pnlRight.Controls.Add(_lblWordsList);
            this.Controls.Add(pnlRight);
            this.Controls.Add(_lblStatus);
        }

        private void InitializeGrid()
        {
            _grid = new DataGridView
            {
                Location = new Point(30, 90),
                Size = new Size(650, 580),
                AllowUserToAddRows = false,
                AllowUserToOrderColumns = false,
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false,
                ColumnHeadersVisible = false,
                RowHeadersVisible = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.CellSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                GridColor = Color.FromArgb(230, 230, 230),
                ScrollBars = ScrollBars.None
            };

            // Сначала создаем колонки и строки, чтобы не было ArgumentOutOfRangeException
            _grid.ColumnCount = _gridSize;
            _grid.RowCount = _gridSize;

            _grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                SelectionBackColor = Color.FromArgb(52, 152, 219),
                SelectionForeColor = Color.White
            };

            // Рассчитываем размеры
            int colWidth = _grid.Width / _gridSize;
            int rowHeight = _grid.Height / _gridSize;

            for (int i = 0; i < _gridSize; i++)
            {
                _grid.Columns[i].Width = colWidth;
                _grid.Rows[i].Height = rowHeight;
            }

            _grid.CellClick += Grid_CellClick;
            this.Controls.Add(_grid);
        }

        private void GeneratePuzzle()
        {
            // Очистка
            for (int r = 0; r < _gridSize; r++)
                for (int c = 0; c < _gridSize; c++)
                    _grid[c, r].Value = "";

            // Размещение слов
            foreach (var word in _wordsToFind)
            {
                bool placed = false;
                for (int attempts = 0; attempts < 200 && !placed; attempts++)
                {
                    int dir = _rnd.Next(2); // 0 - гор, 1 - верт
                    int row = _rnd.Next(_gridSize);
                    int col = _rnd.Next(_gridSize);

                    if (CanPlaceWord(word, row, col, dir))
                    {
                        PlaceWord(word, row, col, dir);
                        placed = true;
                    }
                }
            }

            // Заполнение буквами
            string alphabet = "АБВГДЕЖЗИКЛМНОПРСТУФХЦЧШЩЭЮЯ";
            for (int r = 0; r < _gridSize; r++)
                for (int c = 0; c < _gridSize; c++)
                    if (_grid[c, r].Value == null || _grid[c, r].Value.ToString() == "")
                        _grid[c, r].Value = alphabet[_rnd.Next(alphabet.Length)];
        }

        private bool CanPlaceWord(string word, int row, int col, int dir)
        {
            if (dir == 0 && col + word.Length > _gridSize) return false;
            if (dir == 1 && row + word.Length > _gridSize) return false;

            for (int i = 0; i < word.Length; i++)
            {
                int r = dir == 1 ? row + i : row;
                int c = dir == 0 ? col + i : col;
                if (_grid[c, r].Value != null && _grid[c, r].Value.ToString() != "" && _grid[c, r].Value.ToString() != word[i].ToString())
                    return false;
            }
            return true;
        }

        private void PlaceWord(string word, int row, int col, int dir)
        {
            for (int i = 0; i < word.Length; i++)
                _grid[dir == 0 ? col + i : col, dir == 1 ? row + i : row].Value = word[i];
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Защита от клика по заголовкам или вне сетки
            if (e.RowIndex < 0 || e.RowIndex >= _gridSize || e.ColumnIndex < 0 || e.ColumnIndex >= _gridSize) return;

            var cell = _grid[e.ColumnIndex, e.RowIndex];
            if (cell.Style.BackColor == Color.FromArgb(46, 204, 113)) return; // Уже найдено

            Point clickPoint = new Point(e.ColumnIndex, e.RowIndex);
            if (_currentSelection.Contains(clickPoint)) return;

            // Проверка на соседство
            if (_currentSelection.Count > 0)
            {
                Point last = _currentSelection.Last();
                bool isNeighbor = (Math.Abs(last.X - clickPoint.X) == 1 && last.Y == clickPoint.Y) ||
                                  (Math.Abs(last.Y - clickPoint.Y) == 1 && last.X == clickPoint.X);

                if (!isNeighbor) ResetSelection();
            }

            _currentSelection.Add(clickPoint);
            cell.Style.BackColor = Color.FromArgb(255, 235, 156);
            CheckWordMatch();
        }

        private void CheckWordMatch()
        {
            string selectedStr = string.Concat(_currentSelection.Select(p => _grid[p.X, p.Y].Value.ToString()));

            if (_wordsToFind.Contains(selectedStr) && !_foundWords.Contains(selectedStr))
            {
                _foundWords.Add(selectedStr);
                MarkAsFound();
                _currentSelection.Clear();
                UpdateWordsLabel();
                if (_foundWords.Count == _wordsToFind.Count)
                    MessageBox.Show("Браво! Вы нашли все слова!", "Победа");
            }
            else if (_currentSelection.Count >= _wordsToFind.Max(w => w.Length))
            {
                ResetSelection();
            }
        }

        private void MarkAsFound()
        {
            foreach (var p in _currentSelection)
            {
                _grid[p.X, p.Y].Style.BackColor = Color.FromArgb(46, 204, 113);
                _grid[p.X, p.Y].Style.ForeColor = Color.White;
                _grid[p.X, p.Y].Style.SelectionBackColor = Color.FromArgb(46, 204, 113);
            }
        }

        private void ResetSelection()
        {
            foreach (var p in _currentSelection)
                if (_grid[p.X, p.Y].Style.BackColor != Color.FromArgb(46, 204, 113))
                    _grid[p.X, p.Y].Style.BackColor = Color.White;
            _currentSelection.Clear();
        }

        private void UpdateWordsLabel()
        {
            _lblWordsList.Text = "НУЖНО НАЙТИ:\n\n" +
                string.Join("\n", _wordsToFind.Select(w => _foundWords.Contains(w) ? "✅ " + w : "⬜ " + w));
        }
    }
}