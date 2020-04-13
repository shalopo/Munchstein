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
            Add(Platform.OneWay(new Point2(2, 7.5), width: 9.8));
            Add(Platform.ConcretePoint(bottomLeft: new Point2(6.5, 9)));
            Add(Platform.PassThrough(new Point2(6.5, 9), width: 4.5));
            Add(Platform.Concrete(new Point2(19, 2), width: 3));
            Add(Platform.ConcretePoint(bottomLeft: new Point2(19, 2.9)));
            Add(Platform.PassThrough(new Point2(22, 4), width: 1));
            Add(Platform.PassThrough(new Point2(23, 6), width: 1));
            Add(Platform.OneWay(new Point2(18, 8), width: 10));
            Add(Platform.ConcretePoint(bottomLeft: new Point2(26, 10.9)));
            Add(Platform.PassThrough(new Point2(8, 4), width: 3));
            munchPlatform = Add(Platform.Concrete(new Point2(1, 1), width: 11));
            Add(Platform.ConcretePoint(bottomLeft: new Point2(4, 2)));
            Add(Platform.Concrete(new Point2(7.5, 11.2), width: 5.5));
            Add(Platform.Concrete(new Point2(13, 9.9), width: 1, height: 0.3));
            Add(Platform.Concrete(new Point2(13, 13), width: 0.3, height: 0.9));
            Add(Platform.PassThrough(new Point2(22, 10), width: 4));
            Add(Platform.PassThrough(new Point2(21, 12), width: 8));
            Add(Platform.Concrete(new Point2(2, 8.8), width: 3));
            Add(Platform.PassThrough(new Point2(2, 9), width: 3));
            Add(Platform.PassThrough(new Point2(2, 11), width: 2));
            Add(Platform.PassThrough(new Point2(12, 14.5), width: 2));
        }

        protected override void PostBuild(Level level)
        {
            //level.Actor.Location = new Point2(8, 9);
            //level.Actor.Orientation = ActorOrientation.FLAT;
            //level.Actor.Size = 2;

            level.Actor.CanChangeOrientation = true;
            level.Door.Size = 2;
            level.Door.Location -= Vector2.X_UNIT;

            level.Munch = new Munch(new Point2(9, 2.5));
        }
    }
}
