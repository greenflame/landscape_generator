using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMath3;

namespace AMapGeneration
{
    class AMapGenerator
    {
        public AVertexMap v_map { set; get; }
        public ALightMap l_map { set; get; }

        int result_width, result_height, seed, smooth, levels;
        double noise;

        public AMapGenerator(int width, int height, int seed, int smooth, int levels, double noise)
        {
            this.result_width = width;
            this.result_height = height;
            this.seed = seed;
            this.smooth = smooth;
            this.levels = levels;
            this.noise = noise;
        }

        public void build_light_map()
        {
            for (int i = 0; i < wi)
        }


    }

    class ALMPolygon
    {
        public AVector3 normal { set; get; }
        public double light { set; get; }
    }

    class ALMCell
    {
        public ALMPolygon[] polygons { set; get; }  //top, right, bottom, left
        public double[] heights { set; get; }   //conners and mid. top left, top right, bottom righr, bottom left mid
    }

    class ALightMap
    {
        public int width { get; set; }
        public int height { get; set; }
        public ALMCell[,] map { set; get; }

        public ALightMap(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public void build_from_vertex_map(AVertexMap v_map)
        {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    double[] h = new double[5]; //conners and mid heights
                    h[0] = v_map.get(i, j).Value;
                    h[1] = v_map.get(i + 1, j).Value;
                    h[2] = v_map.get(i + 1, j + 1).Value;
                    h[3] = v_map.get(i, j + 1).Value;
                    h[4] = Math.Floor((h[0] + h[1] + h[2] + h[3]) / 4);

                    map[i, j].heights = h;

                    //todo and other all..
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
