using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein
{
    public class Platform
    {
        public static readonly double STANDING_THRESHOLD = 0.1;

        private Platform(BoxBoundary box) => Box = box;

        public static Platform Concrete(Point2 topLeft, double width, double height = 1)
        {
            return new Platform(new BoxBoundary(topLeft: topLeft, width: width, height: height));
        }

        public static Platform PassThrough(Point2 topLeft, double width)
        {
            return new Platform(new BoxBoundary(topLeft: topLeft, width: width, height: 0.01)) { IsPassThrough = true };
        }

        public BoxBoundary Box { get; private set; }
        public bool IsPassThrough { get; private set; }

        public event Action<Actor> OnActorStanding;

        public void NotifyActorStanding(Actor actor)
        {
            OnActorStanding?.Invoke(actor);
        }
    }
}
