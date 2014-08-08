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
            int cell_size = Convert.ToInt32(textBox_cell_size.Text);
            int map_size = Convert.ToInt32(textBox_map_size.Text);
            int seed = Convert.ToInt32(textBox_seed.Text);
            int smooth = Convert.ToInt32(textBox_smooth.Text);
            int levels = Convert.ToInt32(textBox_levels.Text);

            AVertexMap v_map = new AVertexMap(map_size + 1, map_size + 1, 1, seed, smooth, levels);
            AHeightMap h_map = new AHeightMap(v_map);
            ALightMap2 l_map = new ALightMap2(v_map, h_map);
            ATextureMap t_map = new ATextureMap(h_map, new Size(cell_size, cell_size));

            Bitmap res = t_map.generate_simple_map();
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
