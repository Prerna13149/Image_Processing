using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV.UI;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using System.Drawing.Imaging;

namespace ia0
{
    public partial class Form1 : Form
    {
        private PictureBox pictureBox1 = new PictureBox();
        private PictureBox pictureBox2 = new PictureBox();
        private PictureBox pictureBox3 = new PictureBox();
        Image<Gray, float> orig = new Image<Gray, float>("C:\\sample\\as2\\lena.tif");
        /// <summary>
        /// 
        /// </summary>
        public Form1()
        {

            Image<Gray, float> image = new Image<Gray, float>("C:\\sample\\as2\\wrinkles.jpg");

            // Transform 1 channel grayscale image into 2 channel image
            Console.Write("Enter the value of cutoff frequency: ");
            int fc = Convert.ToInt32(Console.ReadLine());
            Console.Write("Enter the second frequency: ");
            int f1 = Convert.ToInt32(Console.ReadLine());

            Matrix<float> filter =make_butterworth(fc, f1); // 2 channels
 
            
            IntPtr complexImage = CvInvoke.cvCreateImage(image.Size, Emgu.CV.CvEnum.IPL_DEPTH.IPL_DEPTH_32F, 2);
            CvInvoke.cvSetImageCOI(complexImage, 1); // Select the channel to copy into
            CvInvoke.cvCopy(image, complexImage, IntPtr.Zero);
            CvInvoke.cvSetImageCOI(complexImage, 0); // Select all channels
            Matrix<float> forwardDft = new Matrix<float>(image.Rows, image.Cols, 2);
            CvInvoke.cvDFT(complexImage, forwardDft, Emgu.CV.CvEnum.CV_DXT.CV_DXT_FORWARD, 0);


            // frame portion
            Matrix<float> plane1 = new Matrix<float>(image.Rows, image.Cols, 2);
            Matrix<float> plane2 = new Matrix<float>(image.Rows, image.Cols, 2);
            Matrix<float> mag = new Matrix<float>(image.Rows, image.Cols, 2);
            Matrix<float> phase = new Matrix<float>(image.Rows, image.Cols, 2);




            // display the magnitude
            Matrix<float> filterDftMagnitude = GetDftMagnitude(filter);
            //SwitchQuadrants(ref forwardDftMagnitude);
            Matrix<float> outReal = new Matrix<float>(forwardDft.Size);
            //The imaginary part of the Fourier Transform
            Matrix<float> outIm = new Matrix<float>(forwardDft.Size);
            CvInvoke.cvSplit(forwardDft, outReal, outIm, IntPtr.Zero, IntPtr.Zero);
            plane1 = outReal;
            plane2 = outIm;

            pictureBox2.Image = Matrix2Bitmap(filterDftMagnitude);
            pictureBox2.Image.Save("C:\\sample\\as2\\low_pass_filter.tif");


            Image<Gray, float> filter1 = new Image<Gray, float>("C:\\sample\\as2\\low_pass_filter.tif");

            for (int i = 0; i < image.Rows; i++)
                for (int j = 0; j < image.Cols; j++)
                {
                    plane1[i, j] = outReal[i, j] * filter1.Data[i, j,0];
                    plane2[i, j] = outIm[i, j] * filter1.Data[i, j,0];
                }


            

            CvInvoke.cvMerge(plane1, plane2, IntPtr.Zero, IntPtr.Zero, forwardDft);

            
            Matrix<float> reverseDft1 = new Matrix<float>(forwardDft.Rows, forwardDft.Cols, 2);
            CvInvoke.cvDFT(forwardDft, reverseDft1, Emgu.CV.CvEnum.CV_DXT.CV_DXT_INV_SCALE, 0);
            Matrix<float> reverseDftMagnitude1 = GetDftMagnitude(reverseDft1);
            pictureBox3.Image = Matrix2Bitmap(reverseDftMagnitude1);
            pictureBox3.Image.Save("C:\\Sample\\as2\\lena_inverse.tiff", ImageFormat.Tiff);
        


            /////gaussian///
            Image<Gray, float> img1 = new Image<Gray, float>("C:\\sample\\lena.tif");
            Image<Gray, byte> filter5 = new Image<Gray, byte>(img1.Size);
            for (int i = 2; i < img1.Height - 2; i++)
            {
                for (int j = 2; j < img1.Width - 2; j++)//since we are using an average filter all values as 1 hence there is no need to multiply
                {
                    filter5.Data[i, j, 0] = (byte)(((img1.Data[i - 2, j - 2, 0]) + ((img1.Data[i - 2, j - 1, 0]) * 4) + ((img1.Data[i - 2, j, 0]) * 7) + ((img1.Data[i - 2, j + 1, 0]) * 4) + ((img1.Data[i - 2, j + 2, 0]) * 1)
                                                  + ((img1.Data[i - 1, j - 2, 0]) * 4) + ((img1.Data[i - 1, j - 1, 0]) * 7) + ((img1.Data[i - 1, j, 0]) * 16) + ((img1.Data[i - 1, j + 1, 0]) * 7) + ((img1.Data[i - 1, j + 2, 0]) * 4)
                                                  + ((img1.Data[i, j - 2, 0]) * 7) + ((img1.Data[i, j - 1, 0]) * 26) + ((img1.Data[i, j, 0]) * 41) + ((img1.Data[i, j + 1, 0]) * 26) + ((img1.Data[i, j + 2, 0]) * 7)
                                                  + ((img1.Data[i + 1, j - 2, 0]) * 4) + ((img1.Data[i + 1, j - 1, 0]) * 7) + ((img1.Data[i + 1, j, 0]) * 16) + ((img1.Data[i + 1, j + 1, 0]) * 7) + ((img1.Data[i + 1, j + 2, 0]) * 4)
                                                  + (img1.Data[i + 2, j - 2, 0]) + ((img1.Data[i + 2, j - 1, 0]) * 4) + ((img1.Data[i + 2, j, 0]) * 7) + ((img1.Data[i + 2, j + 1, 0]) * 4) + (img1.Data[i + 2, j + 2, 0])) / 273);
                }
            }
            filter5.Save("C:\\sample\\as2\\filter5_try1_new.tif");


        }

        private object Mat_<T>(object padded)
        {
            throw new NotImplementedException();
        }


        // This will hold the DFT data
        private Matrix<float> fourier()
        {
            Image<Gray, float> image = orig.Convert<Gray, float>();
            IntPtr complexImage = CvInvoke.cvCreateImage(image.Size, Emgu.CV.CvEnum.IPL_DEPTH.IPL_DEPTH_32F, 2);

            CvInvoke.cvSetZero(complexImage);  // Initialize all elements to Zero
            CvInvoke.cvSetImageCOI(complexImage, 1);
            CvInvoke.cvCopy(image, complexImage, IntPtr.Zero);
            CvInvoke.cvSetImageCOI(complexImage, 0);
            //CvInvoke.cvReleaseImage(ref complexImage);
            Matrix<float> dft = new Matrix<float>(image.Rows, image.Cols, 2);
            CvInvoke.cvDFT(complexImage, dft, Emgu.CV.CvEnum.CV_DXT.CV_DXT_FORWARD, 0);
            CvInvoke.cvReleaseImage(ref complexImage);
            //The Real part of the Fourier Transform
            Matrix<float> outReal = new Matrix<float>(image.Size);
            //The imaginary part of the Fourier Transform
            Matrix<float> outIm = new Matrix<float>(image.Size);
            CvInvoke.cvSplit(dft, outReal, outIm, IntPtr.Zero, IntPtr.Zero);
            return dft;
        }

        private Matrix<float> Inverse_low_gauss(int d)//used for deblurring
        {
            Matrix<float> fft = fourier();
            Matrix<float> tmp4 = new Matrix<float>(fft.Rows, fft.Cols, 2);
            int l1 = tmp4.Rows / 2;
            int l2 = tmp4.Cols / 2;
            for (int i = 0; i < orig.Rows; i++)
                for (int j = 0; j < orig.Cols; j++)
                {
                    float Duv = (float)((Math.Pow(i - l1, 2) + Math.Pow(j - l2, 2)));//computing inverse of the gauss function
                    tmp4[i, j] = (float)(1 / (Math.Pow(Math.E, (-Duv / (2 * (d * d))))));//element wise inverse
                }

            return tmp4;
        }



        private Matrix<float> Gaussian(int d)
        {
            Matrix<float> fft = fourier();
            Matrix<float> tmp = new Matrix<float>(fft.Rows, fft.Cols, 2);
            int l1 = tmp.Rows / 2;
            int l2 = tmp.Cols / 2;
            for (int i = 0; i < orig.Rows; i++)
                for (int j = 0; j < orig.Cols; j++)
                {
                    float Duv = (float)((Math.Pow(i - l1, 2) + Math.Pow(j - l2, 2)));
                    tmp[i, j] = (float)(Math.Pow(Math.E, (-Duv / (2 * (d * d)))));
                }

            return tmp;
        }

        private Matrix<float> Low_pass(int d)//low pass filter with cutoff frequency
        {
            Matrix<float> ff = fourier();
            Matrix<float> tmp = new Matrix<float>(ff.Rows, ff.Cols, 2);
            int l1 = tmp.Rows / 2;//shifting the centre to the origin
            int l2 = tmp.Cols / 2;
            for (int i = 0; i < orig.Rows; i++)
                for (int j = 0; j < orig.Cols; j++)
                {
                    float Duv = (float)(Math.Sqrt(Math.Pow(i - l1, 2) + Math.Pow(j - l2, 2)));
                    if (Duv <= d)
                    {
                        tmp[i, j] = Duv;
                    }
                    else
                    {
                        tmp[i, j] = 0;
                    }
                }
            return tmp;
        }

        private Matrix<float> low_pass_inverse(int d)
        {
            Matrix<float> tmp4 = new Matrix<float>(orig.Rows, orig.Cols, 2);
            Matrix<float> tmp = new Matrix<float>(orig.Rows, orig.Cols, 2);
            int l1 = tmp.Rows / 2;
            int l2 = tmp.Cols / 2;//half of the length of the matrix
            for (int i = 0; i < tmp4.Rows; i++)
                for (int j = 0; j < tmp4.Cols; j++)
                {
                    float Duv = (float)(Math.Sqrt(Math.Pow(i - l1, 2) + Math.Pow(j - l2, 2)));//shifting the centre to origin
                    int x = (int)Duv;
                    if (Duv <= d & x != 0)
                    {
                        tmp[i, j] = Duv;
                        tmp4[i, j] = (float)Math.Pow(Duv, -1);
                    }
                    else
                    {
                        tmp[i, j] = 0;
                        tmp4[i, j] = 0;
                    }
                }

            return tmp4;
        }

        private Matrix<float> low_pass_inverse_weiner(int d, double k)//inverse of weiner function
        {
            Matrix<float> tmp4 = new Matrix<float>(orig.Rows, orig.Cols, 2);
            Matrix<float> tmp = new Matrix<float>(orig.Rows, orig.Cols, 2);
            int l1 = tmp.Rows / 2;
            int l2 = tmp.Cols / 2;
            for (int i = 0; i < tmp4.Rows; i++)
                for (int j = 0; j < tmp4.Cols; j++)
                {
                    float Duv = (float)(Math.Sqrt(Math.Pow(i - l1, 2) + Math.Pow(j - l2, 2)));//shifting the origin
                    int x = (int)Duv;
                    if (Duv <= d)
                    {
                        tmp[i, j] = Duv;
                        float c = (float)(Math.Pow(Duv, 2) + k);//inverse of weiner
                        tmp4[i, j] = (float)(Duv / c);
                    }
                    else
                    {
                        tmp[i, j] = 0;
                        tmp4[i, j] = 0;
                    }
                }

            return tmp4;
        }

        private Matrix<float> inverse_low_pass(int d)//inverse of low pass for deblurring
        {
            Matrix<float> ff = fourier();
            Matrix<float> tmp = new Matrix<float>(ff.Rows, ff.Cols, 2);
            Matrix<float> tmp5 = new Matrix<float>(ff.Rows, ff.Cols, 2);
            int l1 = tmp.Rows / 2;
            int l2 = tmp.Cols / 2;
            tmp = Low_pass(d);
            for (int i = 0; i < tmp.Rows; i++)
                for (int j = 0; j < tmp.Cols; j++)
                {


                    if (tmp[i, j] != 0)
                    {
                        tmp5[i, j] = (float)(1 / tmp[i, j]);
                    }

                }
            return tmp5;
        }

        private Matrix<float> high_pass(int d)//high pass filter with cutoff frequency as d
        {
            Matrix<float> ff = fourier();
            Matrix<float> tmp1 = new Matrix<float>(ff.Rows, ff.Cols, 2);
            int l1 = tmp1.Rows / 2;
            int l2 = tmp1.Cols / 2;
            for (int i = 0; i < orig.Rows; i++)
                for (int j = 0; j < orig.Cols; j++)
                {
                    float Duv = (float)(Math.Sqrt(Math.Pow(i - l1, 2) + Math.Pow(j - l2, 2)));
                    if (Duv > d)
                    {
                        tmp1[i, j] = Duv;//all values above the threshold are retained
                    }
                    else
                    {
                        tmp1[i, j] = 0;
                    }
                }
            return tmp1;
        }

        private Matrix<float> new_highpass_butterworth(int d, int n)
        {
            Matrix<float> ff = fourier();
            Matrix<float> tmp3 = new Matrix<float>(ff.Rows, ff.Cols, 2);
            int p1 = ff.Rows / 2;
            int p2 = ff.Cols / 2;
            double k = (Math.Pow(2, 0.5)) - 1;
            for (int i = 0; i < orig.Rows; i++)
                for (int j = 0; j < orig.Cols; j++)
                {
                    if (i != p1 && j != p2)
                    {
                        float Duv = (float)(Math.Sqrt(Math.Pow(i - p1, 2) + Math.Pow(j - p2, 2)));
                        tmp3[i, j] = (float)(1 / (1 + k * (Math.Pow((d / Duv), 2 * n))));
                    }
                }

            return tmp3;
        }

        private Matrix<float> band_pass_filter(int d1, int d2)
        {
            Matrix<float> tmp1 = new Matrix<float>(orig.Rows, orig.Cols, 2);
            int l1 = tmp1.Rows / 2;
            int l2 = tmp1.Cols / 2;

            for (int i = 0; i < orig.Rows; i++)
                for (int j = 0; j < orig.Cols; j++)
                {
                    float Duv = (float)(Math.Sqrt(Math.Pow(i - l1, 2) + Math.Pow(j - l2, 2)));
                    if (d1 < Duv && d2 > Duv)
                    {
                        tmp1[i, j] = Duv;
                    }
                    else
                    {
                        tmp1[i, j] = 0;
                    }
                }
            return tmp1;

        }

        private Matrix<float> band_reject_filter(int d1, int d2)//band reject filter
        {

            Matrix<float> tmp1 = new Matrix<float>(orig.Rows, orig.Cols, 2);
            int l1 = tmp1.Rows / 2;
            int l2 = tmp1.Cols / 2;

            for (int i = 0; i < orig.Rows; i++)
                for (int j = 0; j < orig.Cols; j++)
                {
                    float Duv = (float)(Math.Sqrt(Math.Pow(i - l1, 2) + Math.Pow(j - l2, 2)));
                    if (d1 < Duv && d2 > Duv)//frequencies in this range are rejected
                    {
                        tmp1[i, j] = 0;
                    }
                    else
                    {
                        tmp1[i, j] = Duv;//frequencies outside this range are retained
                    }
                }
            return tmp1;
        }


        private Matrix<float> gauss_high(int d)//gaussian high pass with cutoff as d
        {
            Matrix<float> ff = fourier();
            Matrix<float> tmp2 = new Matrix<float>(ff.Rows, ff.Cols, 2);
            int l1 = tmp2.Rows / 2;
            int l2 = tmp2.Cols / 2;
            for (int i = 0; i < orig.Rows; i++)
                for (int j = 0; j < orig.Cols; j++)
                {
                    float Duv = (float)(Math.Sqrt(Math.Pow(i - l1, 2) + Math.Pow(j - l2, 2)));
                    Duv = (float)(1 - Math.Pow(Math.E, (-Duv / (2 * (d * d)))));
                    tmp2[i, j] = Duv;
                }
            return tmp2;
        }

        private Matrix<float> make_butterworth_highpass(int Do, int n)
        {
            Matrix<float> ff = fourier();
            Matrix<float> tmp3 = new Matrix<float>(ff.Rows, ff.Cols, 2);
            int p1 = ff.Rows / 2;
            int p2 = ff.Cols / 2;
            for (int i = 0; i < orig.Rows; i++)
                for (int j = 0; j < orig.Cols; j++)
                {
                    if (i != p1 && j != p2)
                    {
                        float Duv = (float)(Math.Sqrt(Math.Pow(i - p1, 2) + Math.Pow(j - p2, 2)));
                        tmp3[i, j] = (float)(1 / (1 + Math.Pow((Do / Duv), 2 * n)));
                    }
                }

            return tmp3;
        }


        private Matrix<float> make_butterworth(int Do, int n)
        {
            Matrix<float> ff = fourier();
            Matrix<float> tmp = new Matrix<float>(ff.Rows, ff.Cols, 2);

            Point center = new Point(tmp.Rows / 2, tmp.Cols / 2);

            for (int i = 0; i < orig.Rows; i++)
                for (int j = 0; j < orig.Cols; j++)
                {
                    float Duv = (float)(Math.Sqrt(Math.Pow(i - center.X, 2) + Math.Pow(j - center.Y, 2)));
                    tmp[i, j] = (float)(1 / (1 + Math.Pow((Duv / Do), 2 * n)));
                }

            return tmp;
        }



        private Bitmap Matrix2Bitmap(Matrix<float> matrix)
        {
            CvInvoke.cvNormalize(matrix, matrix, 0.0, 255.0, Emgu.CV.CvEnum.NORM_TYPE.CV_MINMAX, IntPtr.Zero);

            Image<Gray, float> image = new Image<Gray, float>(matrix.Size);
            matrix.CopyTo(image);

            return image.ToBitmap();
        }

        // Real part is magnitude, imaginary is phase. 
        // Here we compute log(sqrt(Re^2 + Im^2) + 1) to get the magnitude and 
        // rescale it so everything is visible
        private Matrix<float> GetDftMagnitude(Matrix<float> fftData)
        {
            //The Real part of the Fourier Transform
            Matrix<float> outReal = new Matrix<float>(fftData.Size);
            //The imaginary part of the Fourier Transform
            Matrix<float> outIm = new Matrix<float>(fftData.Size);
            CvInvoke.cvSplit(fftData, outReal, outIm, IntPtr.Zero, IntPtr.Zero);
            //return outReal;
            CvInvoke.cvPow(outReal, outReal, 2.0);
            CvInvoke.cvPow(outIm, outIm, 2.0);

            CvInvoke.cvAdd(outReal, outIm, outReal, IntPtr.Zero);
            CvInvoke.cvPow(outReal, outReal, 0.5);

            CvInvoke.cvAddS(outReal, new MCvScalar(1.0), outReal, IntPtr.Zero); // 1 + Mag
            CvInvoke.cvLog(outReal, outReal); // log(1 + Mag)            

            return outReal;
        }
        private Matrix<float> GetDftReal(Matrix<float> fftData)
        {
            Matrix<float> outReal = new Matrix<float>(fftData.Size);
            //The imaginary part of the Fourier Transform
            Matrix<float> outIm = new Matrix<float>(fftData.Size);
            CvInvoke.cvSplit(fftData, outReal, outIm, IntPtr.Zero, IntPtr.Zero);
            return outReal;

        }

        private Matrix<float> GetDftImg(Matrix<float> fftData)
        {
            Matrix<float> outReal = new Matrix<float>(fftData.Size);
            //The imaginary part of the Fourier Transform
            Matrix<float> outIm = new Matrix<float>(fftData.Size);
            CvInvoke.cvSplit(fftData, outReal, outIm, IntPtr.Zero, IntPtr.Zero);
            return outIm;

        }

        // We have to switch quadrants so that the origin is at the image center
        private void SwitchQuadrants(ref Matrix<float> matrix)
        {
            int cx = matrix.Cols / 2;
            int cy = matrix.Rows / 2;

            Matrix<float> q0 = matrix.GetSubRect(new Rectangle(0, 0, cx, cy));
            Matrix<float> q1 = matrix.GetSubRect(new Rectangle(cx, 0, cx, cy));
            Matrix<float> q2 = matrix.GetSubRect(new Rectangle(0, cy, cx, cy));
            Matrix<float> q3 = matrix.GetSubRect(new Rectangle(cx, cy, cx, cy));
            Matrix<float> tmp = new Matrix<float>(q0.Size);

            q0.CopyTo(tmp);
            q3.CopyTo(q0);
            tmp.CopyTo(q3);
            q1.CopyTo(tmp);
            q2.CopyTo(q1);
            tmp.CopyTo(q2);

        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void imageBox1_Click(object sender, EventArgs e)
        {

        }


    }
}
