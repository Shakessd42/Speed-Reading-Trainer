using System;
using System.Drawing;
using System.Windows.Forms;

namespace Fast_reading_project
{
    public partial class ClassSelectionForm : Form
    {
        public ClassSelectionForm()
        {
            InitializeComponent();
            SetupCustomUI();
        }

        private void SetupCustomUI()
        {
            this.Text = "Выбор уровня";
            this.Size = new Size(450, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = UIStyle.BackgroundColor;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            this.Controls.Clear();
            this.Controls.Add(UIStyle.CreateHeader(this, "КЛАССЫ"));

            FlowLayoutPanel panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(40, 90, 40, 30),
                FlowDirection = FlowDirection.TopDown
            };

            for (int i = 1; i <= 4; i++)
            {
                Button btn = new Button { Text = $"{i} КЛАСС", Size = new Size(350, 80), Margin = new Padding(0, 10, 0, 10) };
                UIStyle.ApplyRoundedButton(btn);

                int grade = i;
                btn.Click += (s, e) => {
                    var nextForm = new ActivitySelectionForm(grade);
                    this.Hide();
                    nextForm.ShowDialog();
                    this.Show();
                };
                panel.Controls.Add(btn);
            }
            this.Controls.Add(panel);
        }
    }
}