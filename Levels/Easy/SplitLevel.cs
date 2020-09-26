namespace Munchstein.Levels.Easy
{
    public class SplitLevel : LevelBuilder
    {
        Platform checkpoint;

        protected override void Build()
        {
            Add(Platform.Concrete(new Point2(16, 3), width: 2));

            Add(Platform.PassThrough(new Point2(19, 5), width: 1));
            Add(Platform.PassThrough(new Point2(19, 4), width: 1));
            Add(Platform.OneWay(new Point2(19, 6), width: 8));
            Add(Platform.Flipper(new Point2(19, 6.9)));
            Add(Platform.Splitter(new Point2(15, 5.7)));
            checkpoint = Add(Platform.Concrete(new Point2(13, 3.0), width: 1));

            Add(Platform.Concrete(new Point2(20, 6), width: 0.3, height: 1.4));
            Add(Platform.Concrete(new Point2(27, 7.3), width: 2));
            Add(Platform.Concrete(new Point2(20, 3), width: 4));
            Add(Platform.Concrete(new Point2(29.0, 5.0), height: 2.0, width: 0.3));
            Add(Platform.Concrete(new Point2(26, 3), width: 3));
        }

        protected override void PostBuild(Level level)
        {
            level.Door.Size = 2;
            level.Munches.Add(new Munch(new Point2(22, 7.5)));
            level.Munches.Add(new Munch(new Point2(21.5, 4.6)));

            checkpoint.OnActorLanding += actor =>
            {
                if (checkpoint != null && actor.Size == 1 && actor.Orientation == ActorOrientation.SQUARE)
                {
                    level.SaveCheckpoint();
                    checkpoint = null;
                }
            };
        }
    }
}