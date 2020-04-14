using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels.Easy
{
    public class OrientationFlipLevel : LevelBuilder
    {
        protected override void Build()
        {
            Add(Platform.OneWay(new Point2(2, 8), width: 9.8));
            Add(Platform.ConcretePoint(bottomLeft: new Point2(4.5, 9.9)));
            Add(Platform.PassThrough(new Point2(7, 9), width: 2.7));
            Add(Platform.OneWay(new Point2(7, 10), width: 3));
            Add(Platform.Concrete(new Point2(19, 2), width: 3));
            Add(Platform.ConcretePoint(bottomLeft: new Point2(19, 2.9)));
            Add(Platform.PassThrough(new Point2(22, 4), width: 1));
            Add(Platform.PassThrough(new Point2(23, 6), width: 1));
            Add(Platform.OneWay(new Point2(18, 8), width: 11));
            Add(Platform.ConcretePoint(bottomLeft: new Point2(26, 10.9)));
            Add(Platform.PassThrough(new Point2(8, 4), width: 3));
            Add(Platform.Concrete(new Point2(2, 1), width: 9));
            Add(Platform.ConcretePoint(bottomLeft: new Point2(4, 2)));
            Add(Platform.Concrete(new Point2(13, 10), width: 0.5, height: 0.3));
            Add(Platform.Concrete(new Point2(13, 13.4), width: 0.3, height: 1.1));
            Add(Platform.PassThrough(new Point2(22, 10), width: 4));
            Add(Platform.PassThrough(new Point2(21, 12), width: 7));
            Add(Platform.OneWay(new Point2(5, 10), width: 5.3));
            Add(Platform.PassThrough(new Point2(7.25, 11), width: 0.5));
            Add(Platform.OneWay(new Point2(7.3, 12), width: 1.3));
            Add(Platform.PassThrough(new Point2(2, 12.9), width: 1));
            Add(Platform.PassThrough(new Point2(5, 12.9), width: 4));
            Add(Platform.OneWay(new Point2(2, 14), width: 8));
            Add(Platform.PassThrough(new Point2(2, 9), width: 2));
            Add(Platform.Concrete(new Point2(5, 12.5), width: 0.3, height: 2));
            Add(Platform.Concrete(new Point2(5.3, 12), width: 2));
            Add(Platform.Concrete(new Point2(5, 15.2), width: 0.3));
            Add(Platform.Concrete(new Point2(10, 12.9), width: 0.3, height: 0.7));
            Add(Platform.Concrete(new Point2(10, 17), width: 0.3, height: 2.1));
            Add(Platform.Concrete(new Point2(2, 18), width: 8));
            Add(Platform.Concrete(new Point2(11, 11.3), width: 2));
        }

        protected override void PostBuild(Level level)
        {
            //level.Actor.Location = new Point2(10, 9);
            //level.Actor.Orientation = ActorOrientation.FLAT;
            //level.Actor.Size = 2;

            level.Actor.CanChangeOrientation = true;
            level.Door.Size = 2;
            level.Door.Location -= Vector2.X_UNIT / 2;

            level.Munch = new Munch(new Point2(9, 2.5));
        }
    }
}
