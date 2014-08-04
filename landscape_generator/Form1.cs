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
            int scale = Convert.ToInt32(textBox_scale.Text);
            int size = Convert.ToInt32(textBox_size.Text);
            int seed = Convert.ToInt32(textBox_seed.Text);
            int smooth = Convert.ToInt32(textBox_smooth.Text);
            int levels = Convert.ToInt32(textBox_levels.Text);

            AVertexMap v_map = new AVertexMap(size, size);
            v_map.generate(1, seed, smooth, levels);
            ALightMap l_map = new ALightMap(v_map);
            l_map.build_map();
            pictureBox1.Image = l_map.render_image(new Bitmap(scale, scale));

            v_map.translate_range(0, 255);
            pictureBox2.Image = scale_without_smoothing(v_map.to_image(), scale);
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
