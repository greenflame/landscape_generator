using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace landscape_generator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap tmp = map_generator.double_map_to_image(map_generator.diamond_square(10, 0));
            pictureBox1.Image = tmp;
            tmp.Save("pic.png");
        }
    }

    class map_generator
    {
        public static Bitmap double_map_to_image(double[,] map)
        {
            int w = map.GetLength(0), h = map.GetLength(1);
            Bitmap img = new Bitmap(w, h);

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    int c = 255 - (int)Math.Round(map[i, j]); ;
                    if (c < 0)
                        c = 0;
                    if (c > 255)
                        c = 255;
                    img.SetPixel(i, j, Color.FromArgb(c, c, c));
                }
            }

            return img;
        }

        public static double[,] diamond_square(int n, double noise)
        {
            int a = (int)Math.Pow(2, n) + 1;
            double[,] map = new double[a, a];

            for (int i = 0; i < a; i++)
                for (int j = 0; j < a; j++)
                {
                    map[i, j] = 0;
                }

            map[0, 0] = 200;
            map[0, a - 1] = 100;
            map[a - 1, a - 1] = 0;
            map[a - 1, 0] = 50;

            Random rand = new Random();
            ds_recursion(ref map, new Rectangle(0, 0, a - 1, a - 1), noise, ref rand);

            return map;
        }

        public static void ds_recursion(ref double[,] map, Rectangle rect, double noise, ref Random rand)
        {
            if (rect.Width == 1 && rect.Height == 1)
                return;

            //mid
            map[rect.X + rect.Width / 2, rect.Y + rect.Height / 2] =
                (map[rect.Left, rect.Top] +
                map[rect.Right, rect.Top] +
                map[rect.Left, rect.Bottom] +
                map[rect.Right, rect.Bottom]) / 4 + (rand.NextDouble() * 2 - 1) * noise;

            //lmid
            map[rect.Left, rect.Y + rect.Height / 2] =
                (map[rect.Left, rect.Top] +
                map[rect.Left, rect.Bottom]) / 2 + (rand.NextDouble() * 2 - 1) * noise;

            //umid
            map[rect.X + rect.Width / 2, rect.Top] =
                (map[rect.Left, rect.Top] +
                map[rect.Right, rect.Top]) / 2 + (rand.NextDouble() * 2 - 1) * noise;

            //rmid
            map[rect.Right, rect.Y + rect.Height / 2] =
                (map[rect.Right, rect.Top] +
                map[rect.Right, rect.Bottom]) / 2 + (rand.NextDouble() * 2 - 1) * noise;

            //dmid
            map[rect.X + rect.Width / 2, rect.Bottom] =
                (map[rect.Left, rect.Bottom] +
                map[rect.Right, rect.Bottom]) / 2 + (rand.NextDouble() * 2 - 1) * noise;

            ds_recursion(ref map, new Rectangle(rect.X, rect.Y, rect.Width / 2, rect.Height / 2), noise, ref rand);
            ds_recursion(ref map, new Rectangle(rect.X + rect.Width / 2, rect.Y, rect.Width / 2, rect.Height / 2), noise, ref rand);
            ds_recursion(ref map, new Rectangle(rect.X, rect.Y + rect.Height / 2, rect.Width / 2, rect.Height / 2), noise, ref rand);
            ds_recursion(ref map, new Rectangle(rect.X + rect.Width / 2, rect.Y + rect.Height / 2, rect.Width / 2, rect.Height / 2), noise, ref rand);
        }
    }
}
