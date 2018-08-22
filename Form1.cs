using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageAnalysis
{
    public partial class Form1 : Form
    {
        string imagePath = "";
        int[] center= {-1,-1};
        public Form1()
        {
            InitializeComponent();          
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Setting up the file dialog
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Open TIFF image";
            openFileDialog1.DefaultExt = "tif";
            openFileDialog1.Filter = "TIFF files (*.tif)|*.tif|All files (*.*)|*.*";

            //Display file dialog and get result.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                imagePath = openFileDialog1.FileName;
                Console.WriteLine(imagePath);
                pictureBox1.Image = Image.FromFile(imagePath);
                analyseImageToolStripMenuItem.Enabled = true;

                //Reset center pixel
                center[0] = -1;
                center[1] = -1;
            }
        }
        /// <summary>
        /// This function attempts to find the bright spot
        /// of the given image and return it's offset.
        /// It assumes the user has provided the correct image
        /// and that there is no noise (additional bright spots 
        /// other then the intended one)
        /// </summary>
        /// <param name="img">Bitmap image to be analysed</param>
        private void findOffset(Bitmap img)
        {
            int mostLeftBrightPixel = img.Width;
            int mostRightBrightPixel = 0;
            int mostTopBrightPixel = img.Height;
            int mostBottomBrightPixel = 0;
            int brightestPixelValue = 0;
            int darkestPixelValue = 255;
            int[] brightestPixelLocation = { 0, 0 };
            int[] darkestPixelLocation = { 0, 0 };

            //find the max and min value of pixel color
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    Color pixel = img.GetPixel(i, j);
                    int pixelToArgbVal = pixel.R;
                    if (brightestPixelValue < pixelToArgbVal)
                    {
                        brightestPixelValue = pixelToArgbVal;
                        brightestPixelLocation[0] = i;
                        brightestPixelLocation[1] = j;
                    }
                    if (darkestPixelValue > pixelToArgbVal)
                    {
                        darkestPixelValue = pixelToArgbVal;
                        darkestPixelLocation[0] = i;
                        darkestPixelLocation[1] = j;
                    }
                }
            }

            int averageArgb = (brightestPixelValue + darkestPixelValue) / 2;

            //find the center of the brightspot
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    Color pixel = img.GetPixel(i, j);
                    if (pixel.R > averageArgb)
                    {
                        if (mostLeftBrightPixel > i)
                        {
                            mostLeftBrightPixel = i;
                        }
                        if (mostRightBrightPixel < i)
                        {
                            mostRightBrightPixel = i;
                        }
                        if (mostTopBrightPixel > j)
                        {
                            mostTopBrightPixel = j;
                        }
                        if (mostBottomBrightPixel < j)
                        {
                            mostBottomBrightPixel = j;
                        }
                    }           
                }
            }

            center[0] = (mostLeftBrightPixel + mostRightBrightPixel) / 2;
            center[1] = (mostBottomBrightPixel + mostTopBrightPixel) / 2;
        }
       
        private void analyseImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Some rules about the image could go here or some prior manipulation
            if (center[0] == -1)
            {
                Bitmap img = new Bitmap(imagePath);
                findOffset(img);
            }
            string message = "Center found at " + center[0] + "," + center[1];
            message += '.';           
            ShowDialog(message, "Center");                                                                                                                              
        }

        //Message dialog (Grabbed from stackoverflow)
        public static void ShowDialog(string text, string caption)
        {
            Form prompt = new Form();
            prompt.Width = 250;
            prompt.Height = 150;
            prompt.Text = caption;
            Label textLabel = new Label() { Left = 20, Top = 20, Text = text };
            textLabel.Width = 300;
            Button confirmation = new Button() { Text = "Ok", Left = 25, Width = 100, Top = 50 };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.ShowDialog();
        }
    }
}
