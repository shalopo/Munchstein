using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein
{
    public struct Vector2
    {
        public Vector2(double x, double y) => (X, Y) = (x, y);

        public double X { get; private set; }
        public double Y { get; private set; }

        public bool IsZero => X == 0 && Y == 0;
        public double LengthSquared => X * X + Y * Y;
        public double Length => Math.Sqrt(LengthSquared);
        public double Slope => Y / X;

        public Vector2 YProjection => new Vector2(0, Y);
        public Vector2 XProjection => new Vector2(X, 0);

        public Vector2 YMirror => new Vector2(X, -Y);
        public Vector2 XMirror => new Vector2(-X, Y);

        public static Vector2 operator -(Vector2 v) => new Vector2(x: -v.X, y: -v.Y);
        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(x: a.X + b.X, y: a.Y + b.Y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => a + (-b);

        public static Vector2 operator *(double coefficient, Vector2 v) => new Vector2(x: v.X * coefficient, y: v.Y * coefficient);
        public static Vector2 operator *(Vector2 v, double coefficient) => coefficient * v;
        public static Vector2 operator /(Vector2 v, double coefficient) => 1 / coefficient * v;

        public static readonly Vector2 ZERO = new Vector2(0, 0);
        public static readonly Vector2 X_UNIT = new Vector2(1, 0);
        public static readonly Vector2 Y_UNIT = new Vector2(0, 1);

        public override string ToString()
        {
            return $"{{{X}, {Y}}}";
        }
    }
}
