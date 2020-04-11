using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein
{
    public struct Box2
    {
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

        public bool Contains(Point2 point)
        {
            return Left <= point.X && point.X <= Right &&
                   Bottom <= point.Y && point.Y <= Top;
        }

        public Vector2 CalcualteCollisionBox(Vector2 disposition, Box2 other)
        {
            if (Overlap(this + disposition, other))
            {
                if (disposition.X >= 0 && disposition.Y >= 0)
                {
                    return CalcualteCollisionBoxInternal(disposition, other);
                }
                else if (disposition.Y < 0 && disposition.X >= 0)
                {
                    return YMirror.CalcualteCollisionBox(disposition.YMirror, other.YMirror).YMirror;
                }
                else if (disposition.X < 0 && disposition.Y >= 0)
                {
                    return XMirror.CalcualteCollisionBox(disposition.XMirror, other.XMirror).XMirror;
                }
                else
                {
                    return -Mirror.CalcualteCollisionBox(-disposition, other.Mirror);
                }
            }
            else
            {
                return Vector2.ZERO;
            }
        }

        public static bool Overlap(Box2 a, Box2 b)
        {
            return a.Right > b.Left &&
                   a.Left < b.Right &&
                   a.Top > b.Bottom &&
                   a.Bottom < b.Top; 
        }

        public Vector2 CalcualteCollisionBoxInternal(Vector2 disposition, Box2 other)
        {
            var dispositioned = this + disposition;

            if (other.Top - Bottom <= 0.001 || other.Right - Left <= 0.001)
            {
                // mismatch due to floating point errors, ignore
                return Vector2.ZERO;
            }

            if (Right >= other.Left && Top < other.Bottom)
            {
                return new Vector2(0, dispositioned.Top - other.Bottom);
            }

            if (Top > other.Bottom)
            {
                return new Vector2(dispositioned.Right - other.Left, 0);
            }

            //if (Bottom >= other.Top || Left >= other.Right)
            //{
            //    // floating point rounding errors, ignore
            //    return Vector2.ZERO;
            //}

            return disposition.Slope > (other.BottomLeft - TopRight).Slope ?
                new Vector2(0, dispositioned.Top - other.Bottom) :
                new Vector2(dispositioned.Right - other.Left, 0);
        }
    }
}
