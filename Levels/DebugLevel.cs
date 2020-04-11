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

            Add(Platform.Concrete(new Point2(0, 3), width: 30));

            for (int i = 0; i < 20; i++)
            {
                Add(Platform.Concrete(new Point2(0.5 + 3.5 * i, 6 + i * 0.1), width: 1));
                Add(Platform.Concrete(new Point2(0.5 + 3.5 * i, 10 + i * 2 * 0.1), width: 3.5));
            }
        }

        protected override void PostBuild(Level level)
        {
            level.Actor.Location = new Point2(5.5 - 0.00001, 3);
            //level.Actor.Velocity = new Vector2(-2, -5);
            level.Actor.Size = 2;
        }
    }
}
