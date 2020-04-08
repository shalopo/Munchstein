using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein
{
    public struct Point2
    {
        public Point2(double x, double y) => (X, Y) = (x, y);

        public double X { get; private set; }
        public double Y { get; private set; }

        public static Vector2 operator -(Point2 a, Point2 b) => new Vector2(a.X - b.X, a.Y - b.Y);
        public static Point2 operator +(Point2 p, Vector2 v) => new Point2(x: p.X + v.X, y: p.Y + v.Y);
        public static Point2 operator -(Point2 p, Vector2 v) => new Point2(x: p.X - v.X, y: p.Y - v.Y);

        public override string ToString()
        {
            return $"{{{X}, {Y}}}";
        }
    }
}
