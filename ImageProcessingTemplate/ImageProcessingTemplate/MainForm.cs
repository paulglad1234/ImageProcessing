using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using BitmapExtensions;

namespace ImageProcessingTemplate
{
    public partial class MainForm : Form
    {
        private Bitmap _left = new Bitmap(@"C:\Users\Pavel\Desktop\ImageProcessing\Pictures\For Hough.jpg");
        private Bitmap _middle;
        private Bitmap _right;

        private Bitmap LeftBitmap
        {
            get => _left;
            set
            {
                _left = value;
                pictureBoxLeft.Image = _left;
            }
        }

        private Bitmap MiddleBitmap
        {
            get => _middle;
            set
            {
                _middle = value;
                pictureBoxMiddle.Image = _middle;
            }
        }

        private Bitmap RightBitmap
        {
            get => _right;
            set
            {
                _right = value;
                pictureBoxRight.Image = _right;
            }
        }

        public MainForm()
        {
            InitializeComponent();
            pictureBoxLeft.Image = _left;
        }

        private void openLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            LeftBitmap = new Bitmap(openFileDialog1.FileName);
        }

        private void saveFromMiddleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;
            MiddleBitmap.Save(saveFileDialog1.FileName);
        }

        private void saveFromRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;
            RightBitmap.Save(saveFileDialog1.FileName);
        }

        /// <summary>
        /// Меняет левое изображение со средним
        /// </summary>
        private void leftMiddleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var temp = LeftBitmap;
            LeftBitmap = MiddleBitmap;
            MiddleBitmap = temp;
        }

        /// <summary>
        /// Меняет среднее изображение с правым
        /// </summary>
        private void middleRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var temp = MiddleBitmap;
            MiddleBitmap = RightBitmap;
            RightBitmap = temp;
        }

        /// <summary>
        /// Меняет правое изображение с левым
        /// </summary>
        private void leftRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var temp = RightBitmap;
            RightBitmap = LeftBitmap;
            LeftBitmap = temp;
        }

        private void expandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RightBitmap = LeftBitmap.SizePlus((int) pixelsToExpandNumericUpDown.Value);
        }

        private void flatWaveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MiddleBitmap = Generator.FlatWave((int) flatWaveWidthNumericUpDown.Value,
                (int) flatWaveHeightNumericUpDown.Value,
                (int) flatWaveUNumericUpDown.Value * 0.01f, (int) flatWaveVNumericUpDown.Value * 0.01f);
        }

        private void maxwellTriangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MiddleBitmap = Generator.MaxwellTriangle;
        }

        private void noiseLevelNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Noises.Intensity = (int) noiseLevelNumericUpDown.Value;
        }

        private void additiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MiddleBitmap = LeftBitmap.AdditiveNoise();
        }

        private void whiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MiddleBitmap = LeftBitmap.WhiteNoise();
        }

        private void saltToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MiddleBitmap = LeftBitmap.SaltNoise();
        }

        private void pepperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MiddleBitmap = LeftBitmap.PepperNoise();
        }

        private void saltPepperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MiddleBitmap = LeftBitmap.SaltPepperNoise();
        }

        private void greyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MiddleBitmap = LeftBitmap.GreyGaussianNoise();
        }

        private void coloredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MiddleBitmap = LeftBitmap.ColoredGaussianNoise();
        }

        private void linearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RightBitmap = MiddleBitmap.LinearLowFrequencyFilter((int) lowFrequencyFilterMaskNumericUpDown.Value);
            filterQualityRatingTextBox.Text = Filters.FilterQualityRating(LeftBitmap, RightBitmap)
                .ToString(CultureInfo.InvariantCulture);
        }

        private void recursiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RightBitmap = MiddleBitmap.RecursiveLowFrequencyFilter((int) lowFrequencyFilterMaskNumericUpDown.Value);
            filterQualityRatingTextBox.Text = Filters.FilterQualityRating(LeftBitmap, RightBitmap)
                .ToString(CultureInfo.InvariantCulture);
        }

        private void squaredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RightBitmap = MiddleBitmap.SquaredMedianFilter((int) medianFilterWindowSizeNumericUpDown.Value);
            filterQualityRatingTextBox.Text = Filters.FilterQualityRating(LeftBitmap, RightBitmap)
                .ToString(CultureInfo.InvariantCulture);
        }

        private void crossedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RightBitmap = MiddleBitmap.CrossedMedianFilter((int) medianFilterWindowSizeNumericUpDown.Value);
            filterQualityRatingTextBox.Text = Filters.FilterQualityRating(LeftBitmap, RightBitmap)
                .ToString(CultureInfo.InvariantCulture);
        }

        private void highFrequencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MiddleBitmap = LeftBitmap.HighFrequencyFilter((int) highFrequencyFilterMaskNumericUpDown.Value);
        }

        private void verticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MiddleBitmap = LeftBitmap.VerticalGradient();
        }

        private void horizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MiddleBitmap = LeftBitmap.HorizontalGradient();
        }

        private void moduleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MiddleBitmap = LeftBitmap.ModuleGradient();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            MiddleBitmap = LeftBitmap.Laplacian1();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            MiddleBitmap = LeftBitmap.Laplacian2();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            MiddleBitmap = LeftBitmap.Laplacian3();
        }

        private void houghToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MiddleBitmap = LeftBitmap.Hough();
        }
    }
}
