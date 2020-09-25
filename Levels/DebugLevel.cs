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
            Add(Platform.Concrete(new Point2(0, 5), width: 20));
        }

        protected override void PostBuild(Level level)
        {
            level.CanActorChangeOrientation = true;
            level.Actors.First().Orientation = ActorOrientation.SQUARE;
            level.Actors.Add(new Actor(level, new Point2(5, 5)));

            level.Munches.Add(new Munch(level.Actors.ToList()[1].Location + Vector2.Y_UNIT + 3 * Vector2.X_UNIT));
            level.Munches.Add(new Munch(level.Actors.ToList()[0].Location + Vector2.Y_UNIT + 5 * Vector2.X_UNIT));
            level.Munches.Add(new Munch(level.Actors.ToList()[1].Location + Vector2.Y_UNIT + 5 * Vector2.X_UNIT));
            level.Munches.Add(new Munch(level.Actors.ToList()[1].Location + Vector2.Y_UNIT + 7 * Vector2.X_UNIT));
            level.Munches.Add(new Munch(level.Actors.ToList()[1].Location + Vector2.Y_UNIT + 9 * Vector2.X_UNIT));
        }
    }
}
