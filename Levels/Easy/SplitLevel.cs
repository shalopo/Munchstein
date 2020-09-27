using System.Linq;

namespace Munchstein.Levels.Easy
{
    public class SplitLevel : LevelBuilder
    {
        static readonly Hint SPLITTER_HINT = new Hint("Try landing on the orange thingy");

        Platform _splitter;

        protected override void Build()
        {
            Add(Platform.Concrete(new Point2(6, 3), width: 2));

            Add(Platform.PassThrough(new Point2(9, 5), width: 1));
            Add(Platform.PassThrough(new Point2(9, 4), width: 1));
            
            Add(Platform.OneWay(new Point2(9, 6), width: 8));
            Platforms.Last().OnActorLanding += actor =>
            {
                if (actor.Size == 1)
                {
                    LevelContext.DisplayHint(SPLITTER_HINT);
                }
            };

            Add(Platform.Flipper(new Point2(9, 6.9)));
            _splitter = Add(Platform.Splitter(new Point2(5, 5.7)));
            Add(Platform.Concrete(new Point2(3, 3.0), width: 1));

            Add(Platform.Concrete(new Point2(10, 6), width: 0.3, height: 1.4));
            Add(Platform.Concrete(new Point2(17.9, 7.3), width: 1.1));
            Add(Platform.Concrete(new Point2(10, 3), width: 4));
            Add(Platform.Concrete(new Point2(19, 7.0), height: 4, width: 0.3));
            Add(Platform.Concrete(new Point2(16, 3), width: 3));
        }

        protected override void PostBuild(Level level)
        {
            level.Door.Size = 2;

            var upperMunch = new Munch(new Point2(12, 7.5));
            var lowerMunch = new Munch(new Point2(11.5, 4.6));

            level.Munches.Add(upperMunch);
            level.Munches.Add(lowerMunch);

            level.OnActorMunch += (actor, munch) =>
            {
                if (actor.Size > 2)
                {
                    LevelContext.DisplayMessage("Not body shaming or anything, but you're quite large");
                }
                else if (actor.Orientation != ActorOrientation.SQUARE)
                {
                    if (munch == lowerMunch)
                    {
                        LevelContext.DisplayMessage("You're in for a disappointment mate");
                    }
                    else
                    {
                        LevelContext.DisplayMessage("Good luck with that one");
                    }
                }
            };

            _splitter.OnActorColliding += actor =>
            {
                LevelContext.DisplayMessage("Why like this...");
            };

            _splitter.OnActorSplit += actor =>
            {
                if (actor.Size == 1)
                {
                    LevelContext.DisplayMessage("Two are better than one!");
                    LevelContext.SuppressHint(SPLITTER_HINT);

                    level.SaveCheckpoint();
                }
                else
                {
                    LevelContext.DisplayMessage("This ain't gonna work");
                }
            };

            level.OnActorUnion += actor =>
            {
                if (actor.Size == 2 && actor.Orientation == ActorOrientation.TALL)
                {
                    LevelContext.DisplayMessage("Make rectangles great again!");
                }
                else
                {
                    LevelContext.DisplayMessage("What the hell is this?");
                }
            };
        }
    }
}