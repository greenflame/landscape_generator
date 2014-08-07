using AGeneration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AGeneration
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
            //ALightMap l_map = new ALightMap(v_map);
            //l_map.build();
            //pictureBox1.Image = l_map.render_image(scale_without_smoothing(new Bitmap("grass.png"), new Size(scale, scale)));

            //v_map.translate_range(0, 255);
            //pictureBox2.Image = scale_without_smoothing(v_map.to_image(), scale);

            AHeightMap h_map = new AHeightMap(v_map);
            ALightMap2 l_map = new ALightMap2(v_map, h_map);
            ATextureMap t_map = new ATextureMap();

            Bitmap res = t_map.generate_simple_map(size - 1, size - 1);
            l_map.render_light_layer(res);
            pictureBox1.Image = res;
        }

        public static Bitmap scale_without_smoothing(Bitmap img, int k)
        {
            Bitmap result = new Bitmap(img.Width * k, img.Height * k);
            for (int i = 0; i < result.Width; i++)
                for (int j = 0; j < result.Width; j++)
                {
                    result.SetPixel(i, j, img.GetPixel(i / k, j / k));
                }
            return result;
        }

        public static Bitmap scale_without_smoothing(Bitmap img, Size new_size)
        {
            Bitmap result = new Bitmap(new_size.Width, new_size.Height);
            for (int i = 0; i < result.Width; i++)
                for (int j = 0; j < result.Width; j++ )
                {
                    result.SetPixel(i, j, img.GetPixel(img.Width * i / result.Width, img.Height * j / result.Height));
                }
            return result;
        }
    }

}
