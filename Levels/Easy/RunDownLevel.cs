using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels.Easy
{
    public class RunDownLevel : LevelBuilder
    {
        static readonly Hint RUN_DOWN_HINT = new Hint("For a hint, Press DOWN for 2 seconds");

        public bool DroppedFromFirstPlatform { get; set; }

        protected override void Build()
        {
            for (int i = 0; i < 2; i++)
            {
                Add(new Platform(new BoxBoundary(new Point2(3, 5 + i * 2), width: i + i * i / 6.0 + 3, height: 0.1),
                                                           is_passthrough: true));
            }

            var runDownPlatform = Platforms.Last();

            Add(new Platform(new BoxBoundary(runDownPlatform.Box.TopRight + new Vector2(0.3, 3.7), width: 1, height: 3)));
            Add(new Platform(new BoxBoundary(runDownPlatform.Box.TopRight + new Vector2(0.5, -2), width: 3, height: 1)));

            Platforms.Last().OnActorStanding += actor => LevelContext.DisplayMessage("Yay!");

            runDownPlatform.OnActorStanding += actor => LevelContext.DisplayHint(RUN_DOWN_HINT);
        }

        protected override void PostBuild(Level level)
        {
            level.Actor.OnDrop += () =>
            {
                if (level.Actor.CurrentPlatform == Platforms.First())
                {
                    LevelContext.SuppressHint(RUN_DOWN_HINT);

                    DroppedFromFirstPlatform = true;
                }
            };

            level.Actor.OnDeath += () =>
            {
                if (DroppedFromFirstPlatform)
                {
                    LevelContext.DisplayMessage("EPIC");
                }
            };
        }
    }
}
