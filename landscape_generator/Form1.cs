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
            pictureBox1.Image = AGenerator.diamond_square(9, 60).toImage();
            //tmp.Save("pic.png");

        }
    }

    class AVertexMap
    {
        public double?[,] map { get; set; }
        public int width { get; set; }
        public int heigth { get; set; }

        public AVertexMap(int width, int heigth)
        {
            this.width = width;
            this.heigth = heigth;
            map = new double?[width, heigth];
        }

        public void clear()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < heigth; j++)
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
            ny = y >= heigth ? 2 * (heigth - 1) - y : ny;
            ny = y < 0 ? Math.Abs(y) : ny;

            return map[nx, ny];  //return point to avalibale area
        }

        public double? at(Point p)
        {
            return at(p.X,  p.Y);
        }

        public void setValue(int x, int y, double? value)
        {
            map[x, y] = value;
        }

        public void setValue(Point p, double? value)
        {
            setValue(p.X, p.Y, value);
        }

        public Bitmap toImage() //todo perevod oblasti znahenij
        {
            Bitmap img = new Bitmap(width, heigth);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < heigth; j++)
                {
                    int c;
                    if (map[i, j] == null)
                    {
                        c = 255;
                    }
                    else
                    {
                        c = 255 - (int)Math.Round(map[i, j].Value);
                    }

                    if (c < 0)
                        c = 0;
                    if (c > 255)
                        c = 255;
                    img.SetPixel(i, j, Color.FromArgb(c, c, c));
                }
            }

            return img;
        }
    }

    class AGenerator
    {

        public static AVertexMap diamond_square(int n, double noise)
        {
            int a = (int)Math.Pow(2, n) + 1;
            AVertexMap map = new AVertexMap(a, a);

            map.map[0, 0] = 200;
            map.map[a - 1, 0] = 100;
            map.map[0, a - 1] = 100;
            map.map[a - 1, a - 1] = 1;

            Random rand = new Random();
            //square_recursion(map, new Rectangle(0, 0, a - 1, a - 1), noise, ref rand);

            for (int i = a - 1; i != 0; i /= 2 )
            {
                deamond_square_iteration(map, i, noise, rand);
            }

            return map;
        }

        public static void deamond_square_iteration(AVertexMap map, int step, double noise, Random rand)
        {
            for (int i = 0; i < map.width; i += step)   //step 1
            {
                for (int j = 0; j < map.heigth; j += step)
                {
                    if ((i + j) % (2 * step) == 0 && map.at(i, j) == null)
                    {
                        double? value =
                            (map.at(i - step, j - step) +
                            map.at(i - step, j + step) +
                            map.at(i + step, j - step) +
                            map.at(i + step, j + step)) / 4 + (rand.NextDouble() * 2 - 1) * noise;

                        map.setValue(i, j, value);
                    }
                }
            }
            
            for (int i = 0; i < map.width; i += step)   //step 2
            {
                for (int j = 0; j < map.heigth; j += step)
                {
                    if ((i + j) % (2 * step) != 0 && map.at(i, j) == null)
                    {
                        double? value =
                            (map.at(i, j - step) +
                            map.at(i, j + step) +
                            map.at(i + step, j) +
                            map.at(i - step, j)) / 4 + (rand.NextDouble() * 2 - 1) * noise;

                        map.setValue(i, j, value);
                    }
                }
            }
        }

        public static void square_recursion(AVertexMap map, Rectangle rect, double noise, ref Random rand)
        {
            if (rect.Width == 1 && rect.Height == 1)
                return;

            //mid
            map.map[rect.X + rect.Width / 2, rect.Y + rect.Height / 2] =
                (map.map[rect.Left, rect.Top] +
                map.map[rect.Right, rect.Top] +
                map.map[rect.Left, rect.Bottom] +
                map.map[rect.Right, rect.Bottom]) / 4 + (rand.NextDouble() * 2 - 1) * noise;

            //lmid
            map.map[rect.Left, rect.Y + rect.Height / 2] =
                (map.map[rect.Left, rect.Top] +
                map.map[rect.Left, rect.Bottom]) / 2 + (rand.NextDouble() * 2 - 1) * noise;

            //umid
            map.map[rect.X + rect.Width / 2, rect.Top] =
                (map.map[rect.Left, rect.Top] +
                map.map[rect.Right, rect.Top]) / 2 + (rand.NextDouble() * 2 - 1) * noise;

            //rmid
            map.map[rect.Right, rect.Y + rect.Height / 2] =
                (map.map[rect.Right, rect.Top] +
                map.map[rect.Right, rect.Bottom]) / 2 + (rand.NextDouble() * 2 - 1) * noise;

            //dmid
            map.map[rect.X + rect.Width / 2, rect.Bottom] =
                (map.map[rect.Left, rect.Bottom] +
                map.map[rect.Right, rect.Bottom]) / 2 + (rand.NextDouble() * 2 - 1) * noise;

            square_recursion(map, new Rectangle(rect.X, rect.Y, rect.Width / 2, rect.Height / 2), noise, ref rand);
            square_recursion(map, new Rectangle(rect.X + rect.Width / 2, rect.Y, rect.Width / 2, rect.Height / 2), noise, ref rand);
            square_recursion(map, new Rectangle(rect.X, rect.Y + rect.Height / 2, rect.Width / 2, rect.Height / 2), noise, ref rand);
            square_recursion(map, new Rectangle(rect.X + rect.Width / 2, rect.Y + rect.Height / 2, rect.Width / 2, rect.Height / 2), noise, ref rand);
        }
    }
}
