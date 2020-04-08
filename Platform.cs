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

        public Platform(BoxBoundary box, bool is_passthrough = false) => (Box, IsPassThrough) = (box, is_passthrough);

        public BoxBoundary Box { get; private set; }
        public bool IsPassThrough { get; private set; }

        public event Action<Actor> OnActorStanding;

        public void NotifyActorStanding(Actor actor)
        {
            OnActorStanding?.Invoke(actor);
        }
    }
}
