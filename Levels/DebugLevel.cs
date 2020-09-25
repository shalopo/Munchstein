using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels
{
    public class DebugLevel : LevelBuilder
    {
        protected override void Build()
        {
            Add(Platform.Concrete(new Point2(0, 10), width: 20));
        }

        protected override void PostBuild(Level level)
        {
            level.CanActorChangeOrientation = true;
            level.Actor.Orientation = ActorOrientation.SQUARE;
            level.Munch = new Munch(level.Actor.Location + Vector2.Y_UNIT);
        }
    }
}
