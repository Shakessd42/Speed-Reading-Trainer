namespace Fast_reading_project
{
    partial class ClassSelectionForm
    {
        // Поле остается IContainer
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // ВОТ ТУТ ИСПРАВЛЕНИЕ: создаем Container, а не IContainer
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 600);
            this.Text = "ClassSelectionForm";
        }
    }
}