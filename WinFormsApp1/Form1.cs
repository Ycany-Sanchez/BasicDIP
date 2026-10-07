namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private Bitmap? loaded;
        private Bitmap? processed;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void SetProcessed(Bitmap bmp, string message)
        {
            var old = processed;
            pictureBox2.Image = null;
            processed = bmp;
            pictureBox2.Image = processed;
            old?.Dispose();
            lblStatus.Text = message;
        }

        private bool RequireImage()
        {
            if (loaded == null)
            {
                MessageBox.Show("Open an image first.", "WinFormsApp1",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            pictureBox1.Image = null;
            pictureBox2.Image = null;
            loaded?.Dispose();
            processed?.Dispose();
            loaded = null;
            processed = null;
            loaded = new Bitmap(openFileDialog1.FileName);
            processed = new Bitmap(loaded);
            pictureBox1.Image = loaded;
            pictureBox2.Image = processed;
            lblStatus.Text = $"Loaded {openFileDialog1.FileName} ({loaded.Width}x{loaded.Height})";
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (processed == null) { RequireImage(); return; }
            using var dlg = new SaveFileDialog();
            dlg.Filter = "PNG|*.png|BMP|*.bmp|JPEG|*.jpg;*.jpeg";
            dlg.FileName = "processed.png";
            if (dlg.ShowDialog() != DialogResult.OK) return;
            var ext = Path.GetExtension(dlg.FileName).ToLowerInvariant();
            var fmt = ext == ".bmp" ? System.Drawing.Imaging.ImageFormat.Bmp
                : ext == ".jpg" || ext == ".jpeg" ? System.Drawing.Imaging.ImageFormat.Jpeg
                : System.Drawing.Imaging.ImageFormat.Png;
            processed.Save(dlg.FileName, fmt);
            lblStatus.Text = $"Saved {dlg.FileName}";
        }

        private void pixelCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!RequireImage()) return;
            Bitmap a = new Bitmap(loaded!);
            Bitmap b = new Bitmap(1, 1);
            HNUDIP.ImageProcess.copyImage(ref a, ref b);
            a.Dispose();
            SetProcessed(b, "Pixel copy applied.");
        }

        private void greyscalingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!RequireImage()) return;
            var bmp = new Bitmap(loaded!);
            ImageProcess2.BitmapFilter.GrayScale(bmp);
            SetProcessed(bmp, "Greyscaling applied.");
        }

        private void inversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!RequireImage()) return;
            var bmp = new Bitmap(loaded!);
            ImageProcess2.BitmapFilter.Invert(bmp);
            SetProcessed(bmp, "Inversion applied.");
        }

        private void mirrorHorizToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!RequireImage()) return;
            Bitmap a = new Bitmap(processed ?? loaded!);
            Bitmap b = new Bitmap(1, 1);
            HNUDIP.ImageProcess.Fliphorizontal(ref a, ref b);
            a.Dispose();
            SetProcessed(b, "Mirror Horiz applied.");
        }

        private void mirrorVertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!RequireImage()) return;
            Bitmap a = new Bitmap(processed ?? loaded!);
            Bitmap b = new Bitmap(1, 1);
            HNUDIP.ImageProcess.FlipVertical(ref a, ref b);
            a.Dispose();
            SetProcessed(b, "Mirror Vert applied.");
        }

        private void histToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!RequireImage()) return;
            Bitmap a = new Bitmap(loaded!);
            Bitmap b = new Bitmap(1, 1);
            HNUDIP.ImageProcess.Histogram(ref a, ref b);
            a.Dispose();
            SetProcessed(b, "Histogram generated.");
        }

        private void contrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!RequireImage()) return;
            var bmp = new Bitmap(loaded!);
            ImageProcess2.BitmapFilter.Contrast(bmp, (sbyte)trackContrast.Value);
            SetProcessed(bmp, $"Contrast {trackContrast.Value} applied.");
        }

        private void scaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!RequireImage()) return;
            Bitmap a = new Bitmap(loaded!);
            Bitmap b = new Bitmap(1, 1);
            HNUDIP.ImageProcess.Scale(ref a, ref b, Math.Max(1, a.Width / 2), Math.Max(1, a.Height / 2));
            a.Dispose();
            SetProcessed(b, "Scaled to half size.");
        }

        private void binaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!RequireImage()) return;
            Bitmap a = new Bitmap(loaded!);
            Bitmap b = new Bitmap(1, 1);
            HNUDIP.ImageProcess.Threshold(ref a, ref b, (int)numThreshold.Value);
            a.Dispose();
            SetProcessed(b, $"Binary threshold {(int)numThreshold.Value} applied.");
        }

        private void coinCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!RequireImage()) return;
            var result = CoinCounter.CountCoins(new Bitmap(loaded!));
            SetProcessed(result.Annotated,
                $"5c:{result.Count5C} 10c:{result.Count10C} 25c:{result.Count25C} P1:{result.Count1P} P5:{result.Count5P} = P{result.Total:F2}");
            MessageBox.Show(result.Summary, "Coin count",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (!RequireImage()) return;
            Bitmap src = new Bitmap(loaded!);
            Bitmap b = new Bitmap(1, 1);
            HNUDIP.ImageProcess.Brightness(ref src, ref b, trackBar1.Value);
            src.Dispose();
            SetProcessed(b, $"Brightness {trackBar1.Value}.");
        }

        private void trackContrast_Scroll(object sender, EventArgs e)
        {
            if (!RequireImage()) return;
            var bmp = new Bitmap(loaded!);
            ImageProcess2.BitmapFilter.Contrast(bmp, (sbyte)trackContrast.Value);
            SetProcessed(bmp, $"Contrast {trackContrast.Value}.");
        }
    }
}
