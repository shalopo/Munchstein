using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein
{
    public class Door
    {
        public Door(Point2 location) => Location = location;

        public Point2 Location { get; private set; }

        internal int Size { get; set; } = 1;
        public double Height => Size;
        public double Width => Size / 2.0;
        public Box2 Box => new Box2(new Point2(Location.X - Width / 2, Location.Y + Height), Width, Height);
    }
}
