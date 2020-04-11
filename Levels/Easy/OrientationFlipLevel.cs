using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels.Easy
{
    public class OrientationFlipLevel : LevelBuilder
    {
        Platform munchPlatform;

        protected override void Build()
        {
            Add(Platform.OneWay(new Point2(2, 8.5), width: 10));
            Add(Platform.ConcretePoint(bottomLeft: new Point2(6.5, 10)));
            Add(Platform.PassThrough(new Point2(6.5, 10), width: 4.5));
            Add(Platform.Concrete(new Point2(19, 3), width: 3));
            Add(Platform.ConcretePoint(bottomLeft: new Point2(19, 3.9)));
            Add(Platform.PassThrough(new Point2(22, 5), width: 1));
            Add(Platform.PassThrough(new Point2(23, 7), width: 1));
            Add(Platform.PassThrough(new Point2(18, 9), width: 10));
            Add(Platform.ConcretePoint(bottomLeft: new Point2(27, 9.9)));
            Add(Platform.PassThrough(new Point2(6, 5), width: 5));
            munchPlatform = Add(Platform.Concrete(new Point2(3, 1), width: 9));
            Add(Platform.ConcretePoint(bottomLeft: new Point2(4, 2)));
            Add(Platform.Concrete(new Point2(14, 12), width: 3));
            Add(Platform.Concrete(new Point2(7, 12.5), width: 6));
            Add(Platform.Concrete(new Point2(4, 12), width: 0.3, height: 2.5));
            Add(Platform.PassThrough(new Point2(1, 11.1), width: 3));
        }

        protected override void PostBuild(Level level)
        {
            //level.Actor.Size = 2;
            //level.Actor.Orientation = ActorOrientation.FLAT;
            level.Actor.CanChangeOrientation = true;
            level.Door.Size = 2;
            level.Door.Location -= Vector2.X_UNIT;

            level.Munch = new Munch(new Point2(9, 1.5));
        }
    }
}
