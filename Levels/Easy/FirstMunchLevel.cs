using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels.Easy
{
    public class FirstMunchLevel : LevelBuilder
    {
        protected override void Build()
        {
            Add(Platform.Concrete(new Point2(3, 3), width: 20));
            Add(Platform.Concrete(new Point2(20, 7), width: 3));
            Add(Platform.PassThrough(new Point2(20, 11), width: 3));
        }

        protected override void PostBuild(Level level)
        {
            level.Munch = new Munch(Platforms.First().Box.TopLeft + 5 * Vector2.X_UNIT);
            level.Door.Height = 2;
        }
    }
}
