using AGeneration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
            ALightMap l_map = new ALightMap(v_map);

            pictureBox1.Image = l_map.to_image(cell_size, cell_size);
            
            //v_map.translate_range(0, 255);
            //pictureBox1.Image = scale_without_smoothing(v_map.to_image(), cell_size);
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

        private void button2_Click(object sender, EventArgs e)
        {
            const int size = 128;
            int s = size - 1;

            Point mid = new Point((size - 1) / 2, (size - 1) / 2);

            Point[] u_polygon = new Point[3] { new Point(0, 0), new Point(s, 0), mid };
            Point[] r_polygon = new Point[3] { new Point(s, 0), new Point(s, s), mid };
            Point[] d_polygon = new Point[3] { new Point(s, s), new Point(0, s), mid };
            Point[] l_polygon = new Point[3] { new Point(0, s), new Point(0, 0), mid };

            Directory.CreateDirectory("textures");

            for (double i = 0.5; i < 1.6; i += 0.1)
            {
                if (i.ToString() == "1") continue;

                Bitmap b = new Bitmap(size, size);  //creating image
                Graphics g = Graphics.FromImage(b);

                Color c = i > 1 ?
                    Color.FromArgb((int)Math.Round(255 * (i - 1)), Color.White) :
                    Color.FromArgb((int)Math.Round(255 * (1 - i)), Color.Black);  //color
                SolidBrush br = new SolidBrush(c);

                g.Clear(Color.FromArgb(0, 0, 0, 0));
                g.FillPolygon(br, u_polygon);
                b.Save("textures/u" + (i * 10).ToString() + ".png");
                richTextBox1.AppendText("u" + (i * 10).ToString() + ", ");

                g.Clear(Color.FromArgb(0, 0, 0, 0));
                g.FillPolygon(br, r_polygon);
                b.Save("textures/r" + (i * 10).ToString() + ".png");
                richTextBox1.AppendText("r" + (i * 10).ToString() + ", ");

                g.Clear(Color.FromArgb(0, 0, 0, 0));
                g.FillPolygon(br, d_polygon);
                b.Save("textures/d" + (i * 10).ToString() + ".png");
                richTextBox1.AppendText("d" + (i * 10).ToString() + ", ");

                g.Clear(Color.FromArgb(0, 0, 0, 0));
                g.FillPolygon(br, l_polygon);
                b.Save("textures/l" + (i * 10).ToString() + ".png");
                richTextBox1.AppendText("l" + (i * 10).ToString() + ", ");

                g.Dispose();
                b.Dispose();
            }
        }
    }

}
