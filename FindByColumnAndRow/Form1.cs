using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Tesseract;

namespace FindByColumnAndRow
{
    public partial class Form1 : Form
    {
        private Image _image;

        public Form1()
        {
            InitializeComponent();
            
        }

        /// <summary>
        ///     open image and get table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Filter =
                    @"Image Files (*.BMP;*.JPG;*.JPEG;*.GIF;*.PNG;)|*.BMP;*.JPG;*.JPEG;*.GIF;*.PNG|All Files (*.*)|*.*";
                openFileDialog1.Title = @"Select an image";
                if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
                _image = Image.FromStream(openFileDialog1.OpenFile());

                Size sourceSize = _image.Size, targetSize = pictureBoxOriginal.Size;
                var scale = Math.Max((float)targetSize.Width / sourceSize.Width, (float)targetSize.Height / sourceSize.Height);
                var rect = new RectangleF{X = 0, Y = 0, Width = scale * sourceSize.Width , Height = scale * sourceSize.Height };
                var g = pictureBoxOriginal.CreateGraphics();
                g.DrawImage(_image, rect);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        ///     Can currently find the location of most cells and columns
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGetBox_Click(object sender, EventArgs e)
        {
            var rectAndText = TableSearch.GetRectangleAndText(_image, int.Parse(textBoxColumn.Text), int.Parse(textBoxRow.Text));
            var croppedBitmap = new Image<Bgr, byte>((Bitmap) _image) {ROI = rectAndText.Item1};
            pictureBoxResult.Image = croppedBitmap.ToBitmap();
            pictureBoxResult.Width = croppedBitmap.Width;
            pictureBoxResult.Height = croppedBitmap.Height;
            //pictureBoxResult.Refresh();
            MessageBox.Show($"X: {rectAndText.Item1.X} \nY: {rectAndText.Item1.Y} \nWidth: {rectAndText.Item1.Width} \nHeight: {rectAndText.Item1.Height} \nString: \n{rectAndText.Item2}");
        }
    }
}