using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMath3
{

    class AVector3
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }

        public AVector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public AVector3(APoint3 begin, APoint3 end)
        {
            x = end.x - begin.x;
            y = end.y - begin.y;
            z = end.z - begin.z;
        }

        public void turn_up()
        {
            if (z < 0)
            {
                x *= -1;
                y *= -1;
                z *= -1;
            }
        }
    }

    class APoint3
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }

        public APoint3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    class APlane3
    {
        public double a { get; set; }
        public double b { get; set; }
        public double c { get; set; }
        public double d { get; set; }

        public APlane3(double a, double b, double c, double d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        internal AVector3 normal()
        {
            return new AVector3(a, b, c);
        }
    }

}
