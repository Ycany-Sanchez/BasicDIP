namespace DIP
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private PictureBox pictureBoxOriginal;
        private PictureBox pictureBoxProcessed;
        private Button btnOpen;
        private Button btnSave;
        private Button btnGrayscale;
        private Button btnInvert;
        private Button btnThreshold;
        private Button btnBrightnessUp;
        private Button btnBrightnessDown;
        private Button btnHistogram;
        private Button btnRotate;
        private NumericUpDown numThreshold;
        private Label lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                pictureBoxOriginal.Image = null;
                pictureBoxProcessed.Image = null;
                _original?.Dispose();
                _processed?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 620);
            Text = "DIP Viewer";

            var topPanel = new FlowLayoutPanel();
            topPanel.Dock = DockStyle.Top;
            topPanel.Height = 60;
            topPanel.AutoScroll = true;

            btnOpen = new Button() { Text = "Open", Width = 80 };
            btnSave = new Button() { Text = "Save", Width = 80 };
            btnGrayscale = new Button() { Text = "Grayscale", Width = 80 };
            btnInvert = new Button() { Text = "Invert", Width = 80 };
            btnThreshold = new Button() { Text = "Threshold", Width = 80 };
            numThreshold = new NumericUpDown() { Minimum = 0, Maximum = 255, Value = 128, Width = 60 };
            btnBrightnessUp = new Button() { Text = "Bright +", Width = 80 };
            btnBrightnessDown = new Button() { Text = "Bright -", Width = 80 };
            btnHistogram = new Button() { Text = "Histogram", Width = 80 };
            btnRotate = new Button() { Text = "Rotate 90", Width = 80 };

            btnOpen.Click += btnOpen_Click;
            btnSave.Click += btnSave_Click;
            btnGrayscale.Click += btnGrayscale_Click;
            btnInvert.Click += btnInvert_Click;
            btnThreshold.Click += btnThreshold_Click;
            btnBrightnessUp.Click += btnBrightnessUp_Click;
            btnBrightnessDown.Click += btnBrightnessDown_Click;
            btnHistogram.Click += btnHistogram_Click;
            btnRotate.Click += btnRotate_Click;

            topPanel.Controls.Add(btnOpen);
            topPanel.Controls.Add(btnSave);
            topPanel.Controls.Add(btnGrayscale);
            topPanel.Controls.Add(btnInvert);
            topPanel.Controls.Add(btnThreshold);
            topPanel.Controls.Add(numThreshold);
            topPanel.Controls.Add(btnBrightnessUp);
            topPanel.Controls.Add(btnBrightnessDown);
            topPanel.Controls.Add(btnHistogram);
            topPanel.Controls.Add(btnRotate);

            var split = new SplitContainer();
            split.Dock = DockStyle.Fill;
            split.SplitterDistance = 490;

            pictureBoxOriginal = new PictureBox();
            pictureBoxOriginal.Dock = DockStyle.Fill;
            pictureBoxOriginal.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxOriginal.BorderStyle = BorderStyle.FixedSingle;

            pictureBoxProcessed = new PictureBox();
            pictureBoxProcessed.Dock = DockStyle.Fill;
            pictureBoxProcessed.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxProcessed.BorderStyle = BorderStyle.FixedSingle;

            split.Panel1.Controls.Add(pictureBoxOriginal);
            split.Panel2.Controls.Add(pictureBoxProcessed);

            lblStatus = new Label();
            lblStatus.Dock = DockStyle.Bottom;
            lblStatus.Height = 24;
            lblStatus.Text = "Open an image to start.";

            Controls.Add(split);
            Controls.Add(topPanel);
            Controls.Add(lblStatus);
        }
    }
}
