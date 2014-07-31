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
            int map_size = Convert.ToInt32(textBox_size.Text);
            AVertexMap map = new AVertexMap(map_size, map_size);
            map.diamond_square(1, Convert.ToInt32(textBox_seed.Text));
            map.simple_smooth(Convert.ToInt32(textBox_smooth.Text));
            map.translate_range(0, Convert.ToInt32(textBox_levels.Text));
            map.round();
            map.translate_range(0, 255);

            pictureBox1.Image = scale_without_smoothing(map.toImage(), Convert.ToInt32(textBox_scale.Text));
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

    class AVertexMap
    {
        public double?[,] map { get; set; }
        public int width { get; set; }
        public int height { get; set; }

        public AVertexMap(int width, int heigth)
        {
            this.width = width;
            this.height = heigth;
            map = new double?[width, heigth];
        }

        public void clear()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    map[i, j] = null;
                }
            }
        }

        public double? at(int x, int y)
        {
            int nx = x;
            nx = x >= width ? 2 * (width - 1) - x : nx;
            nx = x < 0 ? Math.Abs(x) : nx;

            int ny = y;
            ny = y >= height ? 2 * (height - 1) - y : ny;
            ny = y < 0 ? Math.Abs(y) : ny;

            return map[nx, ny];  //return point to avalibale area
        }

        public double? at(Point p)
        {
            return at(p.X,  p.Y);
        }

        public void set_value(int x, int y, double? value)
        {
            map[x, y] = value;
        }

        public void set_value(Point p, double? value)
        {
            set_value(p.X, p.Y, value);
        }

        public void find_range(ref double min, ref double max)
        {
            min = map[0, 0].Value;
            max = map[0, 0].Value;
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    if (map[i, j].Value < min)
                    {
                        min = map[i, j].Value;
                    }
                    if (map[i, j].Value > max)
                    {
                        max = map[i, j].Value;
                    }
                }
        }

        public void translate_range(double new_min, double new_max)
        {
            double min = 0, max = 0;
            find_range(ref min, ref max);

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    map[i, j] = (map[i, j].Value - min) / (max - min) * (new_max - new_min) + new_min;
                }
        }

        public void round()
        {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    map[i, j] = Math.Round(map[i, j].Value);
                }
        }

        public void simple_smooth(int iterations)
        {
            for (int i =  0; i < iterations; i++)
            {
                simple_smooth_iteration();
            }
        }

        public void simple_smooth_iteration()
        {
            double?[,] smooth_map = new double?[width, height];
            
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    double tmp = 0;
                    for (int a = -1; a < 2; a++)
                        for (int b = -1; b < 2; b++)
                        {
                            tmp += at(i + a, j + b).Value;
                        }
                    tmp /= 9;
                    smooth_map[i, j] = tmp;
                }

            map = smooth_map;
        }

        public Bitmap toImage()
        {
            Bitmap img = new Bitmap(width, height);

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    int c = map[i, j] == null ? 255 : 255 - (int)map[i, j].Value;
                    img.SetPixel(i, j, Color.FromArgb(c, c, c));
                }

            return img;
        }

        public void diamond_square(double noise, int seed)
        {

            map[0, 0] = 0;
            map[width - 1, 0] = 0;
            map[0, height - 1] = 0;
            map[width - 1, height - 1] = 0;

            Random rand = new Random(seed);

            for (int i = (width - 1) / 2; i != 0; i /= 2)
            {
                deamond_square_iteration(i, noise, rand);
            }

        }

        public void deamond_square_iteration(int step, double noise, Random rand)
        {
            for (int i = 0; i < width; i += step)   //step 1, mid of cell generation
                for (int j = 0; j < height; j += step)
                    if ((i + j) % (2 * step) == 0 && at(i, j) == null)
                    {
                        double? value =
                            (at(i - step, j - step) +
                            at(i - step, j + step) +
                            at(i + step, j - step) +
                            at(i + step, j + step)) / 4 + (rand.NextDouble() * 2 - 1) * noise;

                        set_value(i, j, value);
                    }

            for (int i = 0; i < width; i += step)   //step 2, mids of edges generation
                for (int j = 0; j < height; j += step)
                    if ((i + j) % (2 * step) != 0 && at(i, j) == null)
                    {
                        double? value =
                            (at(i, j - step) +
                            at(i, j + step) +
                            at(i + step, j) +
                            at(i - step, j)) / 4 + (rand.NextDouble() * 2 - 1) * noise;

                        set_value(i, j, value);
                    }
        }

    }

}
