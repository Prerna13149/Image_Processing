#include "opencv2/imgproc/imgproc.hpp"
#include "opencv2/highgui/highgui.hpp"
#include <opencv2/opencv.hpp>

#include <stdlib.h>
#include <stdio.h>
#include <math.h>
#include <iostream>

using namespace cv;
using namespace std;

Mat createGaussianKernel(int m, int n)// gaussian kernel
{
    // set standard deviation to 1.0
    double sigma = 1.0;
    double r, s = 2.0 * sigma * sigma;
 
    // sum is used for normalization
    double sum = 0.0;

    Mat gKernel(m, n, CV_8U);
 
    // generate 5x5 kernel
    for (int x = -1*m/2; x <= m/2; x++)
    {
        for(int y = -1*n/2; y <= n/2; y++)
        {
            r = sqrt(x*x + y*y);
            gKernel.at<uchar>(x + 2, y + 2) = (exp(-(r*r)/s))/(M_PI * s);
            sum += gKernel.at<uchar>(x + 2, y + 2);
        }
    }
 
    // normalize the Kernel
    for(int i = 0; i < gKernel.rows; ++i)
        for(int j = 0; j < gKernel.rows; ++j)
            gKernel.at<uchar>(i,j) /= sum;

    return gKernel; 
}

int main(int argc, char const *argv[])
{
	string imgAdd = "/home/ritvik/CPP/OpenCV/IA_Assign3/beads2.jpg";// path to beads2.jpg
	Mat src = imread(imgAdd, 0);
	if( !src.data )
	{
		printf( "\nNo image data. Try entering a valid image path.\n " );
		return -1;
	}

	float magKeyPoints[8][8];
	float orientKeyPoints[8][8];

	Mat gKernel = createGaussianKernel(src.rows, src.rows);			//rows and cols are same

	for(int i=0;i<8;i++)
	{
		for(int j=0;j<8;j++)
		{
			//float keyPointHist[]
			float pixelMag[src.cols/8][src.rows/8];
			float pixelOri[src.cols/8][src.rows/8];

			for (int x = src.rows/-16; x < src.rows/16; ++x)
			{
				for (int y = src.cols/-16; y < src.cols/16; ++y)
				{
					float mag4[4];
					float ori;
					mag4[0] = src.at<uchar>(i*src.rows/8 + x + 1, i*src.rows/8 + y + 1) - src.at<uchar>(i*src.rows/8 + x - 1, i*src.rows/8 + y - 1);
					mag4[1] = src.at<uchar>(i*src.rows/8 + x, i*src.rows/8 + y + 1) - src.at<uchar>(i*src.rows/8 + x, i*src.rows/8 + y - 1);
					mag4[2] = src.at<uchar>(i*src.rows/8 + x - 1, i*src.rows/8 + y + 1) - src.at<uchar>(i*src.rows/8 + x + 1, i*src.rows/8 + y - 1);
					mag4[3] = src.at<uchar>(i*src.rows/8 + x + 1, i*src.rows/8 + y) - src.at<uchar>(i*src.rows/8 + x - 1, i*src.rows/8 + y);

					float verComp = -1*mag4[0]/1.414 + mag4[2]/1.414 + mag4[3];
					float horComp = mag4[0]/1.414 + mag4[2]/1.414 + mag4[1];
					pixelMag[x][y] = verComp*verComp + horComp*horComp;
					pixelOri[x][y] = verComp/horComp;

					pixelMag[x][y] = pixelMag[x][y] * gKernel.at<uchar>(i*src.rows/8 + x, i*src.rows/8 + y);				
				}
			}
		}
	}
	return 0;

}
