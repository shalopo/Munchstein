using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein
{
    public class Munch
    {
        public Munch(Point2 location) => Location = location;

        public double Size => 1;

        public Point2 Location { get; private set; }
        public Box2 Box => new Box2(topLeft: new Point2(Location.X - Size / 2, Location.Y + Size), Size, Size);
    }
}
