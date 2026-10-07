using System.Drawing.Imaging;

namespace DIP
{
    public partial class Form1 : Form
    {
        private Bitmap? _original;
        private Bitmap? _processed;

        public Form1()
        {
            InitializeComponent();
        }

        private void SetProcessed(Bitmap bmp, string message)
        {
            var old = _processed;
            pictureBoxProcessed.Image = null;
            _processed = bmp;
            pictureBoxProcessed.Image = _processed;
            old?.Dispose();
            lblStatus.Text = message;
        }

        private bool RequireImage()
        {
            if (_original == null)
            {
                MessageBox.Show("Open an image first.", "DIP",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void btnOpen_Click(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog();
            dlg.Filter = "Images|*.bmp;*.png;*.jpg;*.jpeg;*.tif;*.tiff|All files|*.*";
            if (dlg.ShowDialog() != DialogResult.OK) return;

            pictureBoxOriginal.Image = null;
            pictureBoxProcessed.Image = null;
            _original?.Dispose();
            _processed?.Dispose();
            _original = null;
            _processed = null;
            _original = new Bitmap(dlg.FileName);
            _processed = new Bitmap(_original);
            pictureBoxOriginal.Image = _original;
            pictureBoxProcessed.Image = _processed;
            lblStatus.Text = $"Loaded {dlg.FileName} ({_original.Width}x{_original.Height})";
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            if (_processed == null) { RequireImage(); return; }
            using var dlg = new SaveFileDialog();
            dlg.Filter = "PNG|*.png|BMP|*.bmp|JPEG|*.jpg;*.jpeg";
            dlg.FileName = "processed.png";
            if (dlg.ShowDialog() != DialogResult.OK) return;
            var ext = Path.GetExtension(dlg.FileName).ToLowerInvariant();
            var fmt = ext == ".bmp" ? ImageFormat.Bmp : ext == ".jpg" || ext == ".jpeg" ? ImageFormat.Jpeg : ImageFormat.Png;
            _processed.Save(dlg.FileName, fmt);
            lblStatus.Text = $"Saved {dlg.FileName}";
        }

        private void btnGrayscale_Click(object? sender, EventArgs e)
        {
            if (!RequireImage()) return;
            var bmp = new Bitmap(_original!);
            ImageProcess2.BitmapFilter.GrayScale(bmp);
            SetProcessed(bmp, "Grayscale applied.");
        }

        private void btnInvert_Click(object? sender, EventArgs e)
        {
            if (!RequireImage()) return;
            var bmp = new Bitmap(_original!);
            ImageProcess2.BitmapFilter.Invert(bmp);
            SetProcessed(bmp, "Invert applied.");
        }

        private void btnThreshold_Click(object? sender, EventArgs e)
        {
            if (!RequireImage()) return;
            Bitmap a = new Bitmap(_original!);
            Bitmap b = new Bitmap(1, 1);
            HNUDIP.ImageProcess.Threshold(ref a, ref b, (int)numThreshold.Value);
            SetProcessed(b, $"Threshold {(int)numThreshold.Value} applied.");
        }

        private void btnBrightnessUp_Click(object? sender, EventArgs e)
        {
            if (!RequireImage()) return;
            Bitmap a = _processed ?? _original!;
            Bitmap b = new Bitmap(1, 1);
            var src = new Bitmap(a);
            HNUDIP.ImageProcess.Brightness(ref src, ref b, 30);
            SetProcessed(b, "Brightness +30.");
        }

        private void btnBrightnessDown_Click(object? sender, EventArgs e)
        {
            if (!RequireImage()) return;
            Bitmap a = _processed ?? _original!;
            Bitmap b = new Bitmap(1, 1);
            var src = new Bitmap(a);
            HNUDIP.ImageProcess.Brightness(ref src, ref b, -30);
            SetProcessed(b, "Brightness -30.");
        }

        private void btnHistogram_Click(object? sender, EventArgs e)
        {
            if (!RequireImage()) return;
            Bitmap a = new Bitmap(_original!);
            Bitmap b = new Bitmap(1, 1);
            HNUDIP.ImageProcess.Histogram(ref a, ref b);
            SetProcessed(b, "Histogram generated.");
        }

        private void btnRotate_Click(object? sender, EventArgs e)
        {
            if (!RequireImage()) return;
            Bitmap a = new Bitmap(_processed ?? _original!);
            Bitmap b = new Bitmap(1, 1);
            HNUDIP.ImageProcess.Rotate(ref a, ref b, 90);
            SetProcessed(b, "Rotated 90 deg.");
        }
    }
}
