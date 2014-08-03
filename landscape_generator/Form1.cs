using AMapGeneration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AMath3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //int map_size = Convert.ToInt32(textBox_size.Text);
            //AVertexMap map = new AVertexMap(map_size, map_size);
            //map.diamond_square(1, Convert.ToInt32(textBox_seed.Text));
            //map.simple_smooth(Convert.ToInt32(textBox_smooth.Text));
            //map.translate_range(0, Convert.ToInt32(textBox_levels.Text));
            //map.round();
            //map.translate_range(0, 255);

            //pictureBox1.Image = scale_without_smoothing(map.to_image(), Convert.ToInt32(textBox_scale.Text));
        }

        public static Bitmap scale_without_smoothing(Bitmap img, int k)
        {
            Bitmap result = new Bitmap(img.Width * k, img.Height * k);
            for (int i = 0; i < result.Width; i++)
                for (int j = 0; j < result.Width; j++ )
                {
                    result.SetPixel(i, j, img.GetPixel(i / k, j / k));
                }
            return result;
        }
    }

}
