﻿using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace SS_OpenCV
{ 
    public partial class MainForm : Form
    {
        Image<Bgr, Byte> img = null; // working image
        Image<Bgr, Byte> imgUndo = null; // undo backup image - UNDO
        string title_bak = "";

        public MainForm()
        {
            InitializeComponent();
            title_bak = Text;
        }

        /// <summary>
        /// Opens a new image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                img = new Image<Bgr, byte>(openFileDialog1.FileName);
                Text = title_bak + " [" +
                        openFileDialog1.FileName.Substring(openFileDialog1.FileName.LastIndexOf("\\") + 1) +
                        "]";
                imgUndo = img.Copy();
                ImageViewer.Image = img;
                ImageViewer.Refresh();
            }
        }

        /// <summary>
        /// Saves an image with a new name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImageViewer.Image.Save(saveFileDialog1.FileName);
            }
        }

        /// <summary>
        /// Closes the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// restore last undo copy of the working image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (imgUndo == null) // verify if the image is already opened
                return; 
            Cursor = Cursors.WaitCursor;
            img = imgUndo.Copy();

            ImageViewer.Image = img;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Change visualization mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // zoom
            if (autoZoomToolStripMenuItem.Checked)
            {
                ImageViewer.SizeMode = PictureBoxSizeMode.Zoom;
                ImageViewer.Dock = DockStyle.Fill;
            }
            else // with scroll bars
            {
                ImageViewer.Dock = DockStyle.None;
                ImageViewer.SizeMode = PictureBoxSizeMode.AutoSize;
            }
        }

        /// <summary>
        /// Show authors form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AuthorsForm form = new AuthorsForm();
            form.ShowDialog();
        }

        /// <summary>
        /// Calculate the image negative
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void negativeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.Negative(img);

            ImageViewer.Image = img;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Call image convertion to gray scale
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            //copy Undo Image
            imgUndo = img.Copy();

            ImageClass.ConvertToGray(img);

            ImageViewer.Image = img;
            ImageViewer.Refresh(); // refresh image on the screen

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Call automated image processing check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void evalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EvalForm eval = new EvalForm();
            eval.ShowDialog();

        }

        private void ImageViewer_MouseMove(object sender, MouseEventArgs e)
        {
            int aux_x = 0;
            int aux_y = 0;
            if (ImageViewer.SizeMode == PictureBoxSizeMode.Zoom)
            {
                aux_x = (int)(e.X / ImageViewer.ZoomScale + ImageViewer.HorizontalScrollBar.Value * ImageViewer.ZoomScale);
                aux_y = (int)(e.Y / ImageViewer.ZoomScale + ImageViewer.VerticalScrollBar.Value * ImageViewer.ZoomScale);

            }
            else
            {
                aux_x = (int)(e.X / ImageViewer.ZoomScale + ImageViewer.HorizontalScrollBar.Value * ImageViewer.ZoomScale);
                aux_y = (int)(e.Y / ImageViewer.ZoomScale + ImageViewer.VerticalScrollBar.Value * ImageViewer.ZoomScale);
            }


            if (img != null && aux_y < img.Height && aux_x < img.Width)
                statusLabel.Text = "X:" + aux_x + "  Y:" + aux_y + " - BGR = (" + img.Data[aux_y, aux_x, 0] + "," + img.Data[aux_y, aux_x, 1] + "," + img.Data[aux_y, aux_x, 2] + ")";

        }

        private void nonUniformToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterForm ff = new FilterForm();
            
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor;

            if (ff.ShowDialog() == DialogResult.OK)
            {
                ImageClass.NonUniform(img, imgUndo, ff.matrix, ff.weight, ff.offset);
            }
        }

        private void histogramGrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (img == null) return;
            //Cursor = Cursors.WaitCursor;

            int[] hist = ImageClass.Histogram_Gray(img);

            Histogram Hist = new Histogram(hist);
            Hist.ShowDialog();
        }

        private void histogramRGBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) return;

            int[,] hist = ImageClass.Histogram_RGB(img);

            Histogram Hist = new Histogram(hist);
            Hist.ShowDialog();
        }
    }
}