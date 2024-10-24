﻿using System;
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
using static System.Net.Mime.MediaTypeNames;

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
                byte blue, green, red;

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
        public static void Translation(Image<Bgr, byte> imgDestino, Image<Bgr, byte> imgOrigem, int dx, int dy)
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

        public static void Scale(Image<Bgr, byte> imgDestino, Image<Bgr, byte> imgOrigem, float scaleFactor)
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

        public static void Scale_point_xy(Image<Bgr, byte> imgDestino, Image<Bgr, byte> imgOrigem, float scaleFactor, int centerX, int centerY)
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
                        xo = (int)Math.Round((xd / scaleFactor) - ((width / 2.0) / scaleFactor - centerX));
                        yo = (int)Math.Round((yd / scaleFactor) - ((height / 2.0) / scaleFactor - centerY));
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
        public static void Mean(Image<Bgr, byte> imgDest, Image<Bgr, byte> imgOrig)
        {
            unsafe
            {
                MIplImage mO = imgOrig.MIplImage;
                MIplImage mD = imgDest.MIplImage;

                byte* dataPtrO = (byte*)mO.ImageData.ToPointer(); // Pointer to the origin image
                byte* dataPtrD = (byte*)mD.ImageData.ToPointer(); // Pointer to the origin image

                int widthOrigin = imgOrig.Width;
                int heightOrigin = imgOrig.Height;

                Image<Bgr, byte> imgAux = new Image<Bgr, byte>(widthOrigin + 2, heightOrigin + 2);
                CvInvoke.CopyMakeBorder(imgOrig, imgAux, 1, 1, 1, 1, Emgu.CV.CvEnum.BorderType.Reflect);

                MIplImage mA = imgAux.MIplImage;

                byte* dataPtrA = (byte*)mA.ImageData.ToPointer();

                int widthAux = imgAux.Width;
                int heightAux = imgAux.Height;

                int nChan = mO.NChannels; // number of channels - 3
                int widthstepD = mD.WidthStep;
                int widthstepO = mO.WidthStep;
                int widthstepA = mA.WidthStep;

                int blue, green, red;
                int xo, yo, i, j;
                for (xo = 1; xo < widthAux - 1; xo++)
                {
                    for (yo = 1; yo < heightAux - 1; yo++)
                    {
                        blue = 0;
                        green = 0;
                        red = 0;

                        for (i = -1; i <= 1; i++)
                        {
                            for (j = -1; j <= 1; j++)
                            {
                                blue  += (dataPtrA + (yo + j) * widthstepA + (xo + i) * nChan)[0];
                                green += (dataPtrA + (yo + j) * widthstepA + (xo + i) * nChan)[1];
                                red   += (dataPtrA + (yo + j) * widthstepA + (xo + i) * nChan)[2];
                            }
                        }
                        (dataPtrD + (yo - 1) * widthstepD + (xo - 1) * nChan)[0] = (byte)Math.Round(blue / 9.0);
                        (dataPtrD + (yo - 1) * widthstepD + (xo - 1) * nChan)[1] = (byte)Math.Round(green / 9.0);
                        (dataPtrD + (yo - 1) * widthstepD + (xo - 1) * nChan)[2] = (byte)Math.Round(red / 9.0);
                    }
                }
            }
        }
        void Mean_solutionB(Image<Bgr, byte> imgDest, Image<Bgr, byte> imgOrig)
        {

        }

        public static void NonUniform(Image<Bgr, byte> imgDest, Image<Bgr, byte> imgOrig, float[,] matrix, float matrixWeight, float offset)
        {
            unsafe
            {
                MIplImage mO = imgOrig.MIplImage;
                MIplImage mD = imgDest.MIplImage;

                byte* dataPtrO = (byte*)mO.ImageData.ToPointer(); // Pointer to the origin image
                byte* dataPtrD = (byte*)mD.ImageData.ToPointer(); // Pointer to the origin image

                int widthOrigin = imgOrig.Width;
                int heightOrigin = imgOrig.Height;

                Image<Bgr, byte> imgAux = new Image<Bgr, byte>(widthOrigin + 2, heightOrigin + 2);
                CvInvoke.CopyMakeBorder(imgOrig, imgAux, 1, 1, 1, 1, Emgu.CV.CvEnum.BorderType.Reflect);

                MIplImage mA = imgAux.MIplImage;

                byte* dataPtrA = (byte*)mA.ImageData.ToPointer();

                int widthAux = imgAux.Width;
                int heightAux = imgAux.Height;

                int nChan = mO.NChannels; // number of channels - 3
                int widthstepD = mD.WidthStep;
                int widthstepO = mO.WidthStep;
                int widthstepA = mA.WidthStep;

                float blue, green, red;
                int xo, yo, i, j;
                for (xo = 1; xo < widthAux-1; xo++)
                {
                    for (yo = 1; yo < heightAux-1; yo++)
                    {
                        blue = 0;
                        green = 0;
                        red = 0;

                        for (i = 0; i <= 2; i++)
                        {
                            for (j = 0; j <= 2; j++)
                            {
                                blue  += matrix[i, j] * (dataPtrA + (yo + j - 1) * widthstepA + (xo + i - 1) * nChan)[0];
                                green += matrix[i, j] * (dataPtrA + (yo + j - 1) * widthstepA + (xo + i - 1) * nChan)[1];
                                red   += matrix[i, j] * (dataPtrA + (yo + j - 1) * widthstepA + (xo + i - 1) * nChan)[2];
                            }
                        }
                        
                        blue  = (float)Math.Round(blue / matrixWeight + offset);
                        green = (float)Math.Round(green / matrixWeight + offset);
                        red   = (float)Math.Round(red / matrixWeight + offset);

                        blue  = (blue > 255 ? 255 : (blue < 0 ? 0 : blue));
                        green = (green > 255 ? 255 : (green < 0 ? 0 : green));
                        red   = (red > 255 ? 255   : (red < 0 ? 0 : red));

                        (dataPtrD + (yo - 1) * widthstepD + (xo - 1) * nChan)[0] = (byte)blue;
                        (dataPtrD + (yo - 1) * widthstepD + (xo - 1) * nChan)[1] = (byte)green;
                        (dataPtrD + (yo - 1) * widthstepD + (xo - 1) * nChan)[2] = (byte)red;
                    }
                }
            }
        }
        public static void Sobel(Image<Bgr, byte> imgDest, Image<Bgr, byte> imgOrig)
        //public static void NonUniform(Image<Bgr, byte> imgDest, Image<Bgr, byte> imgOrig, float[,] matrix, float matrixWeight, float offset)
        {
            unsafe
            {
                MIplImage mO = imgOrig.MIplImage;
                MIplImage mD = imgDest.MIplImage;

                byte* dataPtrO = (byte*)mO.ImageData.ToPointer(); // Pointer to the origin image
                byte* dataPtrD = (byte*)mD.ImageData.ToPointer(); // Pointer to the origin image

                int widthOrigin = imgOrig.Width;
                int heightOrigin = imgOrig.Height;

                float[,] Sx = new float[,] {
                    {1, 0, -1},
                    {2, 0, -2},
                    {1, 0, -1}
                };
                float[,] Sy = new float[,]
                {
                    {-1, -2, -1},
                    { 0,  0,  0},
                    { 1,  2,  1}
                };

                Image<Bgr, byte> imgAux = new Image<Bgr, byte>(widthOrigin + 2, heightOrigin + 2);
                CvInvoke.CopyMakeBorder(imgOrig, imgAux, 1, 1, 1, 1, Emgu.CV.CvEnum.BorderType.Reflect);

                MIplImage mA = imgAux.MIplImage;

                byte* dataPtrA = (byte*)mA.ImageData.ToPointer();

                int widthAux = imgAux.Width;
                int heightAux = imgAux.Height;

                int nChan = mO.NChannels; // number of channels - 3
                int widthstepD = mD.WidthStep;
                int widthstepO = mO.WidthStep;
                int widthstepA = mA.WidthStep;

                float blue, green, red, blueX, greenX, redX, blueY, greenY, redY;
                int xo, yo, i, j;
                for (xo = 1; xo < widthAux - 1; xo++)
                {
                    for (yo = 1; yo < heightAux - 1; yo++)
                    {
                        blueX = 0; blueY = 0;
                        greenX = 0; greenY = 0;
                        redX = 0; redY = 0;

                        for (i = 0; i <= 2; i++)
                        {
                            for (j = 0; j <= 2; j++)
                            {
                                blueX += Sx[i, j] * (dataPtrA + (yo + j - 1) * widthstepA + (xo + i - 1) * nChan)[0];
                                greenX += Sx[i, j] * (dataPtrA + (yo + j - 1) * widthstepA + (xo + i - 1) * nChan)[1];
                                redX += Sx[i, j] * (dataPtrA + (yo + j - 1) * widthstepA + (xo + i - 1) * nChan)[2];
                                blueY += Sy[i, j] * (dataPtrA + (yo + j - 1) * widthstepA + (xo + i - 1) * nChan)[0];
                                greenY += Sy[i, j] * (dataPtrA + (yo + j - 1) * widthstepA + (xo + i - 1) * nChan)[1];
                                redY += Sy[i, j] * (dataPtrA + (yo + j - 1) * widthstepA + (xo + i - 1) * nChan)[2];
                            }
                        }

                        blue = Math.Abs(blueX) + Math.Abs(blueY);
                        green = Math.Abs(greenX) + Math.Abs(greenY);
                        red = Math.Abs(redX) + Math.Abs(redY);

                        blue = (blue > 255 ? 255 : (blue < 0 ? 0 : blue));
                        green = (green > 255 ? 255 : (green < 0 ? 0 : green));
                        red = (red > 255 ? 255 : (red < 0 ? 0 : red));

                        (dataPtrD + (yo - 1) * widthstepD + (xo - 1) * nChan)[0] = (byte)blue;
                        (dataPtrD + (yo - 1) * widthstepD + (xo - 1) * nChan)[1] = (byte)green;
                        (dataPtrD + (yo - 1) * widthstepD + (xo - 1) * nChan)[2] = (byte)red;
                    }
                }
            }
        }
        public static void Diferentiation(Image<Bgr, byte> imgDest, Image<Bgr, byte>imgOrig)
        {
            unsafe
            {
                MIplImage mO = imgOrig.MIplImage;
                MIplImage mD = imgDest.MIplImage;

                byte* dataPtrO = (byte*)mO.ImageData.ToPointer(); // Pointer to the origin image
                byte* dataPtrD = (byte*)mD.ImageData.ToPointer(); // Pointer to the origin image

                int widthOrigin = imgOrig.Width;
                int heightOrigin = imgOrig.Height;

                Image<Bgr, byte> imgAux = new Image<Bgr, byte>(widthOrigin + 1, heightOrigin + 1);
                CvInvoke.CopyMakeBorder(imgOrig, imgAux, 0, 1, 0, 1, Emgu.CV.CvEnum.BorderType.Reflect);

                MIplImage mA = imgAux.MIplImage;

                byte* dataPtrA = (byte*)mA.ImageData.ToPointer();

                int widthAux = imgAux.Width;
                int heightAux = imgAux.Height;

                int nChan = mO.NChannels; // number of channels - 3
                int widthstepD = mD.WidthStep;
                int widthstepO = mO.WidthStep;
                int widthstepA = mA.WidthStep;

                float blue, green, red;
                int xo, yo;
                
                for (xo = 1; xo < widthAux; xo++)
                {
                    for (yo = 1; yo < heightAux ; yo++)
                    {
                        blue = 0;
                        green = 0;
                        red = 0;

                        blue  = Math.Abs((dataPtrA + (yo - 1) * widthstepA + (xo - 1) * nChan)[0] - (dataPtrA + (yo - 1) * widthstepA + (xo) * nChan)[0]) + 
                                Math.Abs((dataPtrA + (yo - 1) * widthstepA + (xo - 1) * nChan)[0] - (dataPtrA + (yo) * widthstepA + (xo - 1) * nChan)[0]);

                        green = Math.Abs((dataPtrA + (yo - 1) * widthstepA + (xo - 1) * nChan)[1] - (dataPtrA + (yo - 1) * widthstepA + (xo) * nChan)[1]) + 
                                Math.Abs((dataPtrA + (yo - 1) * widthstepA + (xo - 1) * nChan)[1] - (dataPtrA + (yo) * widthstepA + (xo - 1) * nChan)[1]);

                        red   = Math.Abs((dataPtrA + (yo - 1) * widthstepA + (xo - 1) * nChan)[2] - (dataPtrA + (yo - 1) * widthstepA + (xo) * nChan)[2]) + 
                                Math.Abs((dataPtrA + (yo - 1) * widthstepA + (xo - 1) * nChan)[2] - (dataPtrA + (yo) * widthstepA + (xo - 1) * nChan)[2]);

                        blue  = (blue > 255 ? 255 : blue);
                        green = (green > 255 ? 255 :green);
                        red   = (red > 255 ? 255 : red);

                        (dataPtrD + (yo - 1) * widthstepD + (xo - 1) * nChan)[0] = (byte)blue;
                        (dataPtrD + (yo - 1) * widthstepD + (xo - 1) * nChan)[1] = (byte)green;
                        (dataPtrD + (yo - 1) * widthstepD + (xo - 1) * nChan)[2] = (byte)red;
                    }
                }
            }
        }
        public static void Roberts(Image<Bgr, byte> imgDest, Image<Bgr, byte> imgOrig)
        //public static void Diferentiation(Image<Bgr, byte> imgDest, Image<Bgr, byte> imgOrig)
        {
            unsafe
            {
                MIplImage mO = imgOrig.MIplImage;
                MIplImage mD = imgDest.MIplImage;

                byte* dataPtrO = (byte*)mO.ImageData.ToPointer(); // Pointer to the origin image
                byte* dataPtrD = (byte*)mD.ImageData.ToPointer(); // Pointer to the origin image

                int widthOrigin = imgOrig.Width;
                int heightOrigin = imgOrig.Height;

                Image<Bgr, byte> imgAux = new Image<Bgr, byte>(widthOrigin + 1, heightOrigin + 1);
                CvInvoke.CopyMakeBorder(imgOrig, imgAux, 0, 1, 0, 1, Emgu.CV.CvEnum.BorderType.Reflect);

                MIplImage mA = imgAux.MIplImage;

                byte* dataPtrA = (byte*)mA.ImageData.ToPointer();

                int widthAux = imgAux.Width;
                int heightAux = imgAux.Height;

                int nChan = mO.NChannels; // number of channels - 3
                int widthstepD = mD.WidthStep;
                int widthstepO = mO.WidthStep;
                int widthstepA = mA.WidthStep;

                float blue, green, red;
                int xo, yo;

                for (xo = 1; xo < widthAux; xo++)
                {
                    for (yo = 1; yo < heightAux; yo++)
                    {
                        blue = 0;
                        green = 0;
                        red = 0;

                        blue = Math.Abs((dataPtrA + (yo - 1) * widthstepA + (xo - 1) * nChan)[0] - (dataPtrA + (yo) * widthstepA + (xo) * nChan)[0]) +
                                Math.Abs((dataPtrA + (yo - 1) * widthstepA + (xo) * nChan)[0] - (dataPtrA + (yo) * widthstepA + (xo - 1) * nChan)[0]);

                        green = Math.Abs((dataPtrA + (yo - 1) * widthstepA + (xo - 1) * nChan)[1] - (dataPtrA + (yo) * widthstepA + (xo) * nChan)[1]) +
                                Math.Abs((dataPtrA + (yo - 1) * widthstepA + (xo) * nChan)[1] - (dataPtrA + (yo) * widthstepA + (xo - 1) * nChan)[1]);

                        red = Math.Abs((dataPtrA + (yo - 1) * widthstepA + (xo - 1) * nChan)[2] - (dataPtrA + (yo) * widthstepA + (xo) * nChan)[2]) +
                                Math.Abs((dataPtrA + (yo - 1) * widthstepA + (xo) * nChan)[2] - (dataPtrA + (yo) * widthstepA + (xo - 1) * nChan)[2]);

                        blue = (blue > 255 ? 255 : blue);
                        green = (green > 255 ? 255 : green);
                        red = (red > 255 ? 255 : red);

                        (dataPtrD + (yo - 1) * widthstepD + (xo - 1) * nChan)[0] = (byte)blue;
                        (dataPtrD + (yo - 1) * widthstepD + (xo - 1) * nChan)[1] = (byte)green;
                        (dataPtrD + (yo - 1) * widthstepD + (xo - 1) * nChan)[2] = (byte)red;
                    }
                }
            }
        }

        public static void Median(Image<Bgr, byte> imgDest, Image<Bgr, byte> imgOrig)
        {
            unsafe
            {
                CvInvoke.MedianBlur(imgOrig, imgDest, 3);
            }   
        }

        public static int[] Histogram_Gray(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage mI = img.MIplImage;


                byte* dataPtr = (byte*)mI.ImageData.ToPointer(); // Pointer to the origin image
                
                int widthOrigin     = img.Width;
                int heightOrigin    = img.Height;

                int[] hist = new int[256];
                
                float blue, green, red;
                int xo, yo, gray;

                int nChan = mI.NChannels; // number of channels - 3
                int widthstep = mI.WidthStep;

                for (xo = 0; xo < widthOrigin; xo++ )
                {
                    for(yo = 0; yo < heightOrigin; yo++)
                    {
                        blue    = (dataPtr + yo * widthstep + xo * nChan)[0]; 
                        green   = (dataPtr + yo * widthstep + xo * nChan)[1];
                        red     = (dataPtr + yo * widthstep + xo * nChan)[2];

                        // convert to gray
                        gray = (int)Math.Round(((int)blue + green + red) / 3.0);
                        hist[gray]++; 
                    }
                }
                return hist;
            }
        }
        public static int[,] Histogram_RGB(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage mI = img.MIplImage;

                byte* dataPtr = (byte*)mI.ImageData.ToPointer(); // Pointer to the origin image

                int widthOrigin = img.Width;
                int heightOrigin = img.Height;

                int[,] hist = new int[3,256];

                float blue, green, red;
                int xo, yo;

                int nChan = mI.NChannels; // number of channels - 3
                int widthstep = mI.WidthStep;

                for (xo = 0; xo < widthOrigin; xo++)
                {
                    for (yo = 0; yo < heightOrigin; yo++)
                    {
                        blue = (dataPtr + yo * widthstep + xo * nChan)[0];
                        green = (dataPtr + yo * widthstep + xo * nChan)[1];
                        red = (dataPtr + yo * widthstep + xo * nChan)[2];

                        hist[0,(int)blue]++;
                        hist[1,(int)green]++;
                        hist[2,(int)red]++;
                    }
                }
                return hist;
            }
        }

    }
}
