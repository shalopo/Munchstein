using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein
{
    public struct Box2
    {
        // considers floating point errors
        public const double COLLISION_THRESHOLD = 0.001;

        public Box2(Point2 topLeft, double width, double height)
        {
            TopLeft = topLeft;
            Width = width;
            Height = height;
        }

        public Point2 TopLeft { get; private set; } 
        public double Width { get; private set; }
        public double Height { get; private set; }

        public double Top => TopLeft.Y;
        public double Left => TopLeft.X;
        public double Bottom => Top - Height;
        public double Right => Left + Width;
        public Point2 BottomLeft => new Point2(Left, Bottom);
        public Point2 BottomRight => new Point2(Right, Bottom);
        public Point2 TopRight => new Point2(Right, Top);
        public Vector2 Size => new Vector2(Width, Height);

        private Box2 YMirror => new Box2(new Point2(Left, -Bottom), Width, Height);
        private Box2 XMirror => new Box2(new Point2(-Right, Top), Width, Height);
        private Box2 Mirror => new Box2(new Point2(-Right, -Bottom), Width, Height);

        public static Box2 operator +(Box2 box, Vector2 disposition) => 
            new Box2(box.TopLeft + disposition, width: box.Width, height: box.Height);

        public static Box2 operator -(Box2 box, Vector2 disposition) => box + (-disposition);

        public Vector2 CalcualteCollisionVector(Vector2 disposition, Box2 other)
        {
            if (Overlap(this, other))
            {
                if (disposition.X >= 0 && disposition.Y >= 0)
                {
                    return CalcualteCollisionVectorInternal(disposition, other);
                }
                else if (disposition.Y < 0 && disposition.X >= 0)
                {
                    return YMirror.CalcualteCollisionVector(disposition.YMirror, other.YMirror).YMirror;
                }
                else if (disposition.X < 0 && disposition.Y >= 0)
                {
                    return XMirror.CalcualteCollisionVector(disposition.XMirror, other.XMirror).XMirror;
                }
                else
                {
                    return -Mirror.CalcualteCollisionVector(-disposition, other.Mirror);
                }
            }
            else
            {
                return Vector2.ZERO;
            }
        }

        public static bool Overlap(Box2 a, Box2 b)
        {
            return a.Right - b.Left >= COLLISION_THRESHOLD &&
                   b.Right - a.Left >= COLLISION_THRESHOLD &&
                   a.Top - b.Bottom >= COLLISION_THRESHOLD &&
                   b.Top - a.Bottom >= COLLISION_THRESHOLD;
        }

        public Vector2 CalcualteCollisionVectorInternal(Vector2 disposition, Box2 other)
        {
            var preDispositioned = this - disposition;

            if (other.Top - preDispositioned.Bottom <= COLLISION_THRESHOLD || other.Right - preDispositioned.Left <= COLLISION_THRESHOLD)
            {
                // mismatch due to floating point errors, ignore
                return Vector2.ZERO;
            }

            if (other.Left - preDispositioned.Right <= COLLISION_THRESHOLD && preDispositioned.Top - other.Bottom < COLLISION_THRESHOLD)
            {
                return new Vector2(0, Top - other.Bottom);
            }

            if (preDispositioned.Top - other.Bottom > COLLISION_THRESHOLD)
            {
                return new Vector2(Right - other.Left, 0);
            }

            return disposition.Slope > (other.BottomLeft - TopRight).Slope ?
                new Vector2(0, Top - other.Bottom) :
                new Vector2(Right - other.Left, 0);
        }
    }
}
