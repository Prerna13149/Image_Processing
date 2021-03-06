
﻿using System;
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


namespace ia0
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Image<Gray, byte> img1 = new Image<Gray, byte>("C:\\sample\\a.tif");// input image 1
            Image<Gray, byte> img2 = new Image<Gray, byte>("C:\\sample\\b.tif");// input image 2

            //adding two images
            Image<Gray, byte> result = new Image<Gray, byte>(img1.Size);
            Image<Gray, byte> res = new Image<Gray, byte>(img2.Size);
            for (int i = 0; i < img1.Height; i++)
            {
                for (int j = 0; j < img1.Width; j++)
                {
                    result.Data[i, j, 0] = (byte)(img1.Data[i, j, 0] + img2.Data[i, j, 0]);
                    if (result.Data[i, j, 0]>255)//if value is greater than 255, set it to 255
                    {
                        result.Data[i, j, 0] = 255;
                    }
                }
            }

            result.Save("C:\\sample\\add.tif");//result image is written to this location

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


    }
}

