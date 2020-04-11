using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein
{
    public struct Collision
    {
        public static readonly Collision NONE = new Collision(Vector2.ZERO, 0);

        public Collision(Vector2 vector, double lineLength) => (Vector, LineLength) = (vector, lineLength);

        public Vector2 Vector { get; private set; }
        public double LineLength { get; private set; }

        public Collision XMirror => new Collision(Vector.XMirror, LineLength);
        public Collision YMirror => new Collision(Vector.YMirror, LineLength);
        public Collision Mirror => new Collision(-Vector, LineLength);

        public bool IsNone => Vector.IsZero;
    }
}
