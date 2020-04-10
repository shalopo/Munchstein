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
            Add(Platform.Concrete(new Point2(15.5, 3.1), width: 1.3, height: 0.3));
            //Add(Platform.Concrete(new Point2(16.5, 5.3), width: 0.3, height: 0.3));
        }

        protected override void PostBuild(Level level)
        {
            //level.Actor.Location = new Point2(14, 5);
            //level.Actor.Velocity = new Vector2(-2, -5);
            level.Actor.Size = 2;
        }
    }
}
