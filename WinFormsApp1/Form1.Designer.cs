namespace WinFormsApp1
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem filtersToolStripMenuItem;
        private ToolStripMenuItem pixelCopyToolStripMenuItem;
        private ToolStripMenuItem greyscalingToolStripMenuItem;
        private ToolStripMenuItem inversionToolStripMenuItem;
        private ToolStripMenuItem mirrorHorizToolStripMenuItem;
        private ToolStripMenuItem mirrorVertToolStripMenuItem;
        private ToolStripMenuItem histToolStripMenuItem;
        private ToolStripMenuItem contrastToolStripMenuItem;
        private ToolStripMenuItem scaleToolStripMenuItem;
        private ToolStripMenuItem binaryToolStripMenuItem;
        private TrackBar trackBar1;
        private TrackBar trackContrast;
        private NumericUpDown numThreshold;
        private FlowLayoutPanel filterPanel;
        private SplitContainer splitContainer1;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private OpenFileDialog openFileDialog1;
        private Label lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                pictureBox1.Image = null;
                pictureBox2.Image = null;
                loaded?.Dispose();
                processed?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            filtersToolStripMenuItem = new ToolStripMenuItem();
            pixelCopyToolStripMenuItem = new ToolStripMenuItem();
            greyscalingToolStripMenuItem = new ToolStripMenuItem();
            inversionToolStripMenuItem = new ToolStripMenuItem();
            mirrorHorizToolStripMenuItem = new ToolStripMenuItem();
            mirrorVertToolStripMenuItem = new ToolStripMenuItem();
            histToolStripMenuItem = new ToolStripMenuItem();
            contrastToolStripMenuItem = new ToolStripMenuItem();
            scaleToolStripMenuItem = new ToolStripMenuItem();
            binaryToolStripMenuItem = new ToolStripMenuItem();
            trackBar1 = new TrackBar();
            trackContrast = new TrackBar();
            numThreshold = new NumericUpDown();
            filterPanel = new FlowLayoutPanel();
            splitContainer1 = new SplitContainer();
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            openFileDialog1 = new OpenFileDialog();
            lblStatus = new Label();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackContrast).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numThreshold).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            //
            // menuStrip1
            //
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, filtersToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1000, 24);
            menuStrip1.TabIndex = 0;
            //
            // fileToolStripMenuItem
            //
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openToolStripMenuItem, saveToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            //
            // openToolStripMenuItem
            //
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(180, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            //
            // saveToolStripMenuItem
            //
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(180, 22);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            //
            // filtersToolStripMenuItem (DIP)
            //
            filtersToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                pixelCopyToolStripMenuItem, greyscalingToolStripMenuItem, inversionToolStripMenuItem,
                mirrorHorizToolStripMenuItem, mirrorVertToolStripMenuItem, histToolStripMenuItem,
                contrastToolStripMenuItem, scaleToolStripMenuItem, binaryToolStripMenuItem });
            filtersToolStripMenuItem.Name = "filtersToolStripMenuItem";
            filtersToolStripMenuItem.Size = new Size(50, 20);
            filtersToolStripMenuItem.Text = "DIP";
            //
            // DIP items in screenshot order
            //
            pixelCopyToolStripMenuItem.Name = "pixelCopyToolStripMenuItem"; pixelCopyToolStripMenuItem.Size = new Size(180, 22); pixelCopyToolStripMenuItem.Text = "pixel copy"; pixelCopyToolStripMenuItem.Click += pixelCopyToolStripMenuItem_Click;
            greyscalingToolStripMenuItem.Name = "greyscalingToolStripMenuItem"; greyscalingToolStripMenuItem.Size = new Size(180, 22); greyscalingToolStripMenuItem.Text = "greyscaling"; greyscalingToolStripMenuItem.Click += greyscalingToolStripMenuItem_Click;
            inversionToolStripMenuItem.Name = "inversionToolStripMenuItem"; inversionToolStripMenuItem.Size = new Size(180, 22); inversionToolStripMenuItem.Text = "Inversion"; inversionToolStripMenuItem.Click += inversionToolStripMenuItem_Click;
            mirrorHorizToolStripMenuItem.Name = "mirrorHorizToolStripMenuItem"; mirrorHorizToolStripMenuItem.Size = new Size(180, 22); mirrorHorizToolStripMenuItem.Text = "Mirror Horiz"; mirrorHorizToolStripMenuItem.Click += mirrorHorizToolStripMenuItem_Click;
            mirrorVertToolStripMenuItem.Name = "mirrorVertToolStripMenuItem"; mirrorVertToolStripMenuItem.Size = new Size(180, 22); mirrorVertToolStripMenuItem.Text = "Mirror Vert"; mirrorVertToolStripMenuItem.Click += mirrorVertToolStripMenuItem_Click;
            histToolStripMenuItem.Name = "histToolStripMenuItem"; histToolStripMenuItem.Size = new Size(180, 22); histToolStripMenuItem.Text = "hist"; histToolStripMenuItem.Click += histToolStripMenuItem_Click;
            contrastToolStripMenuItem.Name = "contrastToolStripMenuItem"; contrastToolStripMenuItem.Size = new Size(180, 22); contrastToolStripMenuItem.Text = "contrast"; contrastToolStripMenuItem.Click += contrastToolStripMenuItem_Click;
            scaleToolStripMenuItem.Name = "scaleToolStripMenuItem"; scaleToolStripMenuItem.Size = new Size(180, 22); scaleToolStripMenuItem.Text = "scale"; scaleToolStripMenuItem.Click += scaleToolStripMenuItem_Click;
            binaryToolStripMenuItem.Name = "binaryToolStripMenuItem"; binaryToolStripMenuItem.Size = new Size(180, 22); binaryToolStripMenuItem.Text = "binary"; binaryToolStripMenuItem.Click += binaryToolStripMenuItem_Click;
            //
            // trackContrast (left)
            //
            trackContrast.Location = new Point(3, 3);
            trackContrast.Maximum = 100;
            trackContrast.Minimum = -100;
            trackContrast.Name = "trackContrast";
            trackContrast.Size = new Size(200, 45);
            trackContrast.TabIndex = 0;
            trackContrast.Scroll += trackContrast_Scroll;
            //
            // trackBar1 (right = brightness)
            //
            trackBar1.Location = new Point(209, 3);
            trackBar1.Maximum = 100;
            trackBar1.Minimum = -100;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(200, 45);
            trackBar1.TabIndex = 1;
            trackBar1.Scroll += trackBar1_Scroll;
            //
            // numThreshold
            //
            numThreshold.Location = new Point(415, 12);
            numThreshold.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numThreshold.Name = "numThreshold";
            numThreshold.Size = new Size(60, 23);
            numThreshold.TabIndex = 2;
            numThreshold.Value = new decimal(new int[] { 128, 0, 0, 0 });
            //
            // filterPanel (sliders only)
            //
            filterPanel.AutoScroll = true;
            filterPanel.Controls.Add(trackContrast);
            filterPanel.Controls.Add(trackBar1);
            filterPanel.Controls.Add(numThreshold);
            filterPanel.Dock = DockStyle.Top;
            filterPanel.Location = new Point(0, 24);
            filterPanel.Name = "filterPanel";
            filterPanel.Size = new Size(1000, 56);
            filterPanel.TabIndex = 1;
            //
            // splitContainer1
            //
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 80);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Size = new Size(1000, 325);
            splitContainer1.SplitterDistance = 496;
            splitContainer1.TabIndex = 2;
            //
            // pictureBox1
            //
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(496, 325);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            //
            // pictureBox2
            //
            pictureBox2.BorderStyle = BorderStyle.FixedSingle;
            pictureBox2.Dock = DockStyle.Fill;
            pictureBox2.Location = new Point(0, 0);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(500, 325);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 0;
            pictureBox2.TabStop = false;
            //
            // openFileDialog1
            //
            openFileDialog1.Filter = "Images|*.bmp;*.png;*.jpg;*.jpeg;*.tif;*.tiff|All files|*.*";
            //
            // lblStatus
            //
            lblStatus.Dock = DockStyle.Bottom;
            lblStatus.Location = new Point(0, 405);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(1000, 45);
            lblStatus.TabIndex = 3;
            lblStatus.Text = "Open an image. Left slider = contrast, right slider = brightness, numeric = threshold.";
            //
            // Form1
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 450);
            Controls.Add(splitContainer1);
            Controls.Add(filterPanel);
            Controls.Add(lblStatus);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackContrast).EndInit();
            ((System.ComponentModel.ISupportInitialize)numThreshold).EndInit();
            splitContainer1.Panel1.Controls.Add(pictureBox1);
            splitContainer1.Panel2.Controls.Add(pictureBox2);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
