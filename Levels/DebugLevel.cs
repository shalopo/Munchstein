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
            //Add(Platform.Concrete(new Point2(14.5, 2.1), width: 1.3, height: 0.3));
            //Add(Platform.Concrete(new Point2(16.5, 4.3), width: 0.3, height: 1.3));
            //Add(Platform.Concrete(new Point2(15, 5.6), width: 1.3, height: 0.3));

            Add(Platform.Concrete(new Point2(0, 10), width: 11));
            Add(Platform.Concrete(Platforms.Last().Box.TopLeft + new Vector2(6, 0.1), width: 0.1, 0.1));

            Add(Platform.Concrete(new Point2(14, 6), width: 0.1, 0.1));


            //for (int i = 0; i < 20; i++)
            //{
            //    Add(Platform.Concrete(new Point2(10 + 2 * i, i * 0.1 + 10 - i), width: 0.1, height: 0.1));
            //}
        }

        protected override void PostBuild(Level level)
        {
            level.Actor.CanChangeOrientation = true;
            level.Munch = new Munch(level.Actor.Location + Vector2.Y_UNIT * 2);
            //level.Actor.Orientation = ActorOrientation.FLAT;
            //level.Actor.Location = new Point2(28, 10);
            //level.Actor.Size = 2;
        }
    }
}
