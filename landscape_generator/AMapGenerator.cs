using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMath3;

namespace AMapGeneration
{
    class ALMCell
    {
        public AVector3[] normals { set; get; }
        public double[] light { set; get; }  //top, right, bottom, left
        public double[] heights { set; get; }   //conners and mid. top left, top right, bottom righr, bottom left mid

        public ALMCell()
        {
            normals = new AVector3[4];
            light = new double[4];
            heights = new double[5];
        }
    }

    class ALightMap
    {
        public int width { get; set; }
        public int height { get; set; }
        public AVertexMap v_map { set; get; }
        public ALMCell[,] map { set; get; }

        public ALightMap(AVertexMap v_map)
        {
            this.v_map = v_map;

            this.width = v_map.width - 1;
            this.height = v_map.height - 1;

            map = new ALMCell[width, height];
        }

        public Bitmap render_image(Bitmap texture)
        {
            Bitmap result = new Bitmap(width * texture.Width, height * texture.Height);
            Graphics g = Graphics.FromImage(result);

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++ )
                {
                    if (map[i, j].light[0] != 1)
                    {
                        Point[] points = new Point[3];
                        points[0] = new Point(i * texture.Width, j * texture.Height);
                        points[1] = new Point((i + 1) * texture.Width, j * texture.Height);
                        points[2] = new Point((2 * i + 1) * texture.Width / 2, (2 * j + 1) * texture.Height / 2);   //mid
                        g.FillPolygon(new SolidBrush(Color.Gray), points);
                    }

                    if (map[i, j].light[1] != 1)
                    {
                        Point[] points = new Point[3];
                        points[0] = new Point((i + 1) * texture.Width, j * texture.Height);
                        points[1] = new Point((i + 1) * texture.Width, (j + 1) * texture.Height);
                        points[2] = new Point((2 * i + 1) * texture.Width / 2, (2 * j + 1) * texture.Height / 2);   //mid
                        g.FillPolygon(new SolidBrush(Color.Gray), points);
                    }
                    
                    if (map[i, j].light[2] != 1)
                    {
                        Point[] points = new Point[3];
                        points[0] = new Point((i + 1) * texture.Width, (j + 1) * texture.Height);
                        points[1] = new Point(i * texture.Width, (j + 1) * texture.Height);
                        points[2] = new Point((2 * i + 1) * texture.Width / 2, (2 * j + 1) * texture.Height / 2);   //mid
                        g.FillPolygon(new SolidBrush(Color.Gray), points);
                    }
                    
                    if (map[i, j].light[3] != 1)
                    {
                        Point[] points = new Point[3];
                        points[0] = new Point(i * texture.Width, (j + 1) * texture.Height);
                        points[1] = new Point(i * texture.Width, j * texture.Height);
                        points[2] = new Point((2 * i + 1) * texture.Width / 2, (2 * j + 1) * texture.Height / 2);   //mid
                        g.FillPolygon(new SolidBrush(Color.Gray), points);
                    }
                }

            g.Dispose();
            return result;
        }

        public Bitmap render_cell(Bitmap result, Bitmap texture, int x, int y)  //todo ???
        {
            return result;
        }

        public void build_map()
        {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    map[i, j] = new ALMCell();
                    build_cell_heights(i, j);
                    build_cell_normals(i, j);
                    build_cell_light(i, j);
                }
        }

        private void build_cell_light(int x, int y) //todo xm)...
        {
            for (int i = 0; i < 4; i++)
            {
                if (map[x, y].normals[i].x == 0 && map[x, y].normals[i].y == 0)
                {
                    map[x, y].light[i] = 1;
                }
                else
                {
                    map[x, y].light[i] = 0.5;
                }

            }
        }

        public AVector3 normal(APoint3 p1, APoint3 p2, APoint3 p3)
        {
            AVector3 v1 = new AVector3(p1, p2);
            AVector3 v2 = new AVector3(p1, p3);
            v1.turn_up();
            v2.turn_up();

            return new AVector3(v1.y * v2.z - v2.y * v1.z, v2.x * v1.z - v1.x * v2.z, v1.x * v2.y - v2.x * v1.y);   //todo check
        }

        public void build_cell_normals(int x, int y)
        {
            AVector3[] p = new AVector3[4];

            p[0] = normal(
                new APoint3(-0.5, -0.5, map[x, y].heights[0]),
                new APoint3(0.5, -0.5, map[x, y].heights[1]),
                new APoint3(0, 0, map[x, y].heights[4]));

            p[1] = normal(
                new APoint3(0.5, -0.5, map[x, y].heights[1]),
                new APoint3(0.5, 0.5, map[x, y].heights[2]),
                new APoint3(0, 0, map[x, y].heights[4]));

            p[2] = normal(
                new APoint3(0.5, 0.5, map[x, y].heights[2]),
                new APoint3(-0.5, 0.5, map[x, y].heights[3]),
                new APoint3(0, 0, map[x, y].heights[4]));

            p[3] = normal(
                new APoint3(-0.5, 0.5, map[x, y].heights[3]),
                new APoint3(-0.5, -0.5, map[x, y].heights[0]),
                new APoint3(0, 0, map[x, y].heights[4]));

            map[x, y].normals = p;
        }

        public void build_cell_heights(int x, int y)
        {
            map[x, y].heights[0] = v_map.get(x, y).Value;   //conners and mid heights
            map[x, y].heights[1] = v_map.get(x + 1, y).Value;
            map[x, y].heights[2] = v_map.get(x + 1, y + 1).Value;
            map[x, y].heights[3] = v_map.get(x, y + 1).Value;
            map[x, y].heights[4] = /*Math.Floor*/(
                (map[x, y].heights[0] +
                map[x, y].heights[1] +
                map[x, y].heights[2] +
                map[x, y].heights[3]) / 4);

            if (map[x, y].heights[4] % 0.5 != 0)
            {
                map[x, y].heights[4] = Math.Round(map[x, y].heights[4]);
            }
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

        public void generate(double noise, int seed, int smooth, int levels)
        {
            diamond_square(noise, seed);    //vertex map generation
            simple_smooth(smooth);
            translate_range(0, levels - 1);
            round();
        }

        public double? get(int x, int y)
        {
            int nx = x;
            nx = x >= width ? 2 * (width - 1) - x : nx;
            nx = x < 0 ? Math.Abs(x) : nx;

            int ny = y;
            ny = y >= height ? 2 * (height - 1) - y : ny;
            ny = y < 0 ? Math.Abs(y) : ny;

            return map[nx, ny];  //return point to avalibale area
        }

        public double? get(Point p)
        {
            return get(p.X, p.Y);
        }

        public void set(int x, int y, double? value)
        {
            map[x, y] = value;
        }

        public void set(Point p, double? value)
        {
            set(p.X, p.Y, value);
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

        public void round()
        {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    map[i, j] = Math.Round(map[i, j].Value);
                }
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

        public void simple_smooth(int iterations)
        {
            for (int i = 0; i < iterations; i++)
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
                            tmp += get(i + a, j + b).Value;
                        }
                    tmp /= 9;
                    smooth_map[i, j] = tmp;
                }

            map = smooth_map;
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
                    if ((i + j) % (2 * step) == 0 && get(i, j) == null)
                    {
                        double? value =
                            (get(i - step, j - step) +
                            get(i - step, j + step) +
                            get(i + step, j - step) +
                            get(i + step, j + step)) / 4 + (rand.NextDouble() * 2 - 1) * noise;

                        set(i, j, value);
                    }

            for (int i = 0; i < width; i += step)   //step 2, mids of edges generation
                for (int j = 0; j < height; j += step)
                    if ((i + j) % (2 * step) != 0 && get(i, j) == null)
                    {
                        double? value =
                            (get(i, j - step) +
                            get(i, j + step) +
                            get(i + step, j) +
                            get(i - step, j)) / 4 + (rand.NextDouble() * 2 - 1) * noise;

                        set(i, j, value);
                    }
        }

        public Bitmap to_image()
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

    }

}
