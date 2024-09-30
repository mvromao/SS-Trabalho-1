using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Drawing;
using System.Management.Instrumentation;
using System.Windows.Forms;
using Emgu.CV.ImgHash;
using System.Linq;
using Emgu.CV.XFeatures2D;
using System.Diagnostics.Eventing.Reader;

namespace SS_OpenCV
{
    class ImageClass
    {

        /// <summary>
        /// Image Negative using EmguCV library
        /// Slower method
        /// </summary>
        /// <param name="img">Image</param>
        public static void NegativeLibrary(Image<Bgr, byte> img)
        {
            int x, y;

            Bgr aux;
            for (y = 0; y < img.Height; y++)
            {
                for (x = 0; x < img.Width; x++)
                {
                    // acesso pela biblioteca : mais lento 
                    aux = img[y, x];
                    img[y, x] = new Bgr(255 - aux.Blue, 255 - aux.Green, 255 - aux.Red);
                }
            }
        }

        public static void Negative(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                byte blue, green, red, gray;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int xd, yd;

                if (nChan == 3) // image in RGB
                {
                    for (yd = 0; yd < height; yd++)
                    {
                        for (xd = 0; xd < width; xd++)
                        {
                            //retrieve 3 colour components
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];

                            // store in the image
                            dataPtr[0] = (byte)(255 - blue);
                            dataPtr[1] = (byte)(255 - green);
                            dataPtr[2] = (byte)(255 - red);

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }


        /// <summary>
        /// Convert to gray
        /// Direct access to memory - faster method
        /// </summary>
        /// <param name="img">image</param>
        public static void ConvertToGray(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.ImageData.ToPointer(); // Pointer to the image
                byte blue, green, red, gray;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.NChannels; // number of channels - 3
                int padding = m.WidthStep - m.NChannels * m.Width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrieve 3 colour components
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];

                            // convert to gray
                            gray = (byte)Math.Round(((int)blue + green + red) / 3.0);

                            // store in the image
                            dataPtr[0] = gray;
                            dataPtr[1] = gray;
                            dataPtr[2] = gray;

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }
        public static void Rotation(Image<Bgr, byte> imgDestino, Image<Bgr, byte> imgOrigem, float angle)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage mO = imgOrigem.MIplImage;
                MIplImage mD = imgDestino.MIplImage;

                byte* dataPtrO = (byte*)mO.ImageData.ToPointer(); // Pointer to the origin image
                byte* dataPtrD = (byte*)mD.ImageData.ToPointer(); // Pointer to the origin image

                int width = imgOrigem.Width;
                int height = imgOrigem.Height;
                int nChan = mO.NChannels; // number of channels - 3
                int padding = mO.WidthStep - mO.NChannels * mO.Width; // alinhament bytes (padding)
                int widthstepD = mD.WidthStep;
                int widthstepO = mO.WidthStep;

                int xd, yd, xo, yo;
                double sin = Math.Sin(angle), cos = Math.Cos(angle);
                for (yd = 0; yd < height; yd++)
                {
                    for (xd = 0; xd < width; xd++)
                    {
                        xo = (int)(Math.Round((xd - width / 2.0) * cos - (height / 2.0 - yd) * sin + width / 2.0));
                        yo = (int)(Math.Round(height / 2.0 - (xd - width / 2.0) * sin - (height / 2.0 - yd) * cos));
                        if (xo < 0 || yo < 0 || xo >= width || yo >= height)
                        {
                            (dataPtrD + yd * widthstepD + xd * nChan)[0] = 0;
                            (dataPtrD + yd * widthstepD + xd * nChan)[1] = 0;
                            (dataPtrD + yd * widthstepD + xd * nChan)[2] = 0;
                        }
                        else
                        {
                            (dataPtrD + yd * widthstepD + xd * nChan)[0] = (byte)(dataPtrO + yo * widthstepO + xo * nChan)[0];
                            (dataPtrD + yd * widthstepD + xd * nChan)[1] = (byte)(dataPtrO + yo * widthstepO + xo * nChan)[1];
                            (dataPtrD + yd * widthstepD + xd * nChan)[2] = (byte)(dataPtrO + yo * widthstepO + xo * nChan)[2];
                        }
                    }
                }
            }
        }
        public static void Translation(Image<Bgr, byte> imgDestino,Image<Bgr, byte> imgOrigem, int dx, int dy)
        {
            unsafe
            {
                MIplImage mO = imgOrigem.MIplImage;
                MIplImage mD = imgDestino.MIplImage;

                byte* dataPtrO = (byte*)mO.ImageData.ToPointer(); // Pointer to the origin image
                byte* dataPtrD = (byte*)mD.ImageData.ToPointer(); // Pointer to the origin image

                int width = imgOrigem.Width;
                int height = imgOrigem.Height;
                int nChan = mO.NChannels; // number of channels - 3
                int padding = mO.WidthStep - mO.NChannels * mO.Width; // alinhament bytes (padding)
                int widthstepD = mD.WidthStep;
                int widthstepO = mO.WidthStep;

                int xd, yd, xo, yo;
                for (yd = 0; yd < height; yd++)
                {
                    for (xd = 0; xd < width; xd++)
                    {
                        xo = xd - dx;
                        yo = yd - dy;
                        if (xo < 0 || yo < 0 || xo >= width || yo >= height)
                        {
                            (dataPtrD + yd * widthstepD + xd * nChan)[0] = 0;
                            (dataPtrD + yd * widthstepD + xd * nChan)[1] = 0;
                            (dataPtrD + yd * widthstepD + xd * nChan)[2] = 0;
                        }
                        else
                        {
                            (dataPtrD + yd * widthstepD + xd * nChan)[0] = (byte)(dataPtrO + yo * widthstepO + xo * nChan)[0];
                            (dataPtrD + yd * widthstepD + xd * nChan)[1] = (byte)(dataPtrO + yo * widthstepO + xo * nChan)[1];
                            (dataPtrD + yd * widthstepD + xd * nChan)[2] = (byte)(dataPtrO + yo * widthstepO + xo * nChan)[2];
                        }
                    }
                }
            }
        }

        public static void Scale(Image<Bgr, byte> imgDestino,Image<Bgr, byte> imgOrigem, float scaleFactor)
        {
            unsafe
            {
                MIplImage mO = imgOrigem.MIplImage;
                MIplImage mD = imgDestino.MIplImage;

                byte* dataPtrO = (byte*)mO.ImageData.ToPointer(); // Pointer to the origin image
                byte* dataPtrD = (byte*)mD.ImageData.ToPointer(); // Pointer to the origin image

                int width = imgOrigem.Width;
                int height = imgOrigem.Height;
                int nChan = mO.NChannels; // number of channels - 3
                int padding = mO.WidthStep - mO.NChannels * mO.Width; // alinhament bytes (padding)
                int widthstepD = mD.WidthStep;
                int widthstepO = mO.WidthStep;

                int xd, yd, xo, yo;
                for (yd = 0; yd < height; yd++)
                {
                    for (xd = 0; xd < width; xd++)
                    {
                        xo = (int)Math.Round(xd / scaleFactor);
                        yo = (int)Math.Round(yd / scaleFactor);
                        if (xo < 0 || yo < 0 || xo >= width || yo >= height)
                        {
                            (dataPtrD + yd * widthstepD + xd * nChan)[0] = 0;
                            (dataPtrD + yd * widthstepD + xd * nChan)[1] = 0;
                            (dataPtrD + yd * widthstepD + xd * nChan)[2] = 0;
                        }
                        else
                        {
                            (dataPtrD + yd * widthstepD + xd * nChan)[0] = (byte)(dataPtrO + yo * widthstepO + xo * nChan)[0];
                            (dataPtrD + yd * widthstepD + xd * nChan)[1] = (byte)(dataPtrO + yo * widthstepO + xo * nChan)[1];
                            (dataPtrD + yd * widthstepD + xd * nChan)[2] = (byte)(dataPtrO + yo * widthstepO + xo * nChan)[2];
                        }
                    }
                }
            }
        }

        public static void Scale_point_xy(Image<Bgr, byte> imgDestino,Image<Bgr, byte> imgOrigem, float scaleFactor, int centerX, int centerY)

        {
            unsafe
            {
                MIplImage mO = imgOrigem.MIplImage;
                MIplImage mD = imgDestino.MIplImage;

                byte* dataPtrO = (byte*)mO.ImageData.ToPointer(); // Pointer to the origin image
                byte* dataPtrD = (byte*)mD.ImageData.ToPointer(); // Pointer to the origin image

                int width = imgOrigem.Width;
                int height = imgOrigem.Height;
                int nChan = mO.NChannels; // number of channels - 3
                int padding = mO.WidthStep - mO.NChannels * mO.Width; // alinhament bytes (padding)
                int widthstepD = mD.WidthStep;
                int widthstepO = mO.WidthStep;

                int xd, yd, xo, yo;
                for (yd = 0; yd < height; yd++)
                {
                    for (xd = 0; xd < width; xd++)
                    {
                        xo = (int)Math.Round((xd / scaleFactor) - ((width/2) / scaleFactor - centerX));
                        yo = (int)Math.Round((yd / scaleFactor) - ((height/2) / scaleFactor - centerY));
                        if (xo < 0 || yo < 0 || xo >= width || yo >= height)
                        {
                            (dataPtrD + yd * widthstepD + xd * nChan)[0] = 0;
                            (dataPtrD + yd * widthstepD + xd * nChan)[1] = 0;
                            (dataPtrD + yd * widthstepD + xd * nChan)[2] = 0;
                        }
                        else
                        {
                            (dataPtrD + yd * widthstepD + xd * nChan)[0] = (byte)(dataPtrO + yo * widthstepO + xo * nChan)[0];
                            (dataPtrD + yd * widthstepD + xd * nChan)[1] = (byte)(dataPtrO + yo * widthstepO + xo * nChan)[1];
                            (dataPtrD + yd * widthstepD + xd * nChan)[2] = (byte)(dataPtrO + yo * widthstepO + xo * nChan)[2];
                        }
                    }
                }
            }
        }
    }
}
