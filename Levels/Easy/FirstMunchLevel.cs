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
            Add(Platform.Concrete(new Point2(4, 3), width: 2));
            Add(Platform.Concrete(new Point2(9, 3), width: 2));
            Add(Platform.OneWay(new Point2(7, 7), width: 5.7));
            Add(Platform.PassThrough(new Point2(16.5, 8), width: 1));
            Add(Platform.OneWay(new Point2(18.2, 5), width: 1.8));
            Add(Platform.Concrete(new Point2(17.5, 3), width: 1));
            Add(Platform.Concrete(new Point2(19.5, 4), width: 1));
            Add(Platform.OneWay(new Point2(7, 9), width: 14));
            Add(Platform.OneWay(new Point2(14.3, 6), width: 2.6));
            Add(Platform.OneWay(new Point2(20, 8), width: 1));
            Add(Platform.Concrete(new Point2(8, 12), width: 0.3, height: 1.5));
            Add(Platform.PassThrough(new Point2(10.5, 4), width: 1));
            Add(Platform.PassThrough(new Point2(10.5, 8), width: 2.5));
            Add(Platform.PassThrough(new Point2(20, 7), width: 1));
            Add(Platform.PassThrough(new Point2(13.4, 4), width: 1.5));
            Add(Platform.Concrete(new Point2(21, 8), width: 3));
            Add(Platform.Concrete(new Point2(21, 17), width: 1, height: 7));
            Add(Platform.PassThrough(new Point2(22, 11), width: 2));
        }

        protected override void PostBuild(Level level)
        {
            level.Munch = new Munch(new Point2(19, 5));
            //level.Munch = new Munch(level.Actor.Location);
            level.Door.Height = 2;

            level.Actor.OnSizeUp += () =>
            {
                LevelContext.DisplayMessage("Big boys can jump higher!");
            };

            var bigJumps = 0;

            level.Actor.OnJump += () =>
            {
                if (level.Actor.Height > 1)
                {
                    switch (bigJumps)
                    {
                        case 0:
                            LevelContext.DisplayMessage("It's a good thing!");
                            break;
                        case 1:
                            LevelContext.DisplayMessage("Right?...");
                            break;
                        default:
                            break;
                    }

                    bigJumps++;
                }
            };
        }
    }
}
