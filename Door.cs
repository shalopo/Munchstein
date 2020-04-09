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

        public double Height => 1;
        public double Width => Height / 2;
        public BoxBoundary Box => new BoxBoundary(new Point2(Location.X - Width / 2, Location.Y + Height), Width, Height);
    }
}
