using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels.Easy
{
    public class RunDownLevel : ILevelFactory
    {
        static readonly Hint RUN_DOWN_HINT = new Hint("For a hint, Press DOWN for 2 seconds");

        class SingleRunContext
        {
            public bool DroppedFromFirstPlatform { get; set; }
        }

        public Level Create(ILevelContext levelContext)
        {
            var platforms = new List<Platform>();

            for (int i = 0; i < 2; i++)
            {
                platforms.Add(new Platform(new BoxBoundary(new Point2(3, 5 + i * 2), width: i + i * i / 6.0 + 3,  height: 0.1),  
                                                           is_passthrough: true));
            }

            var runDownPlatform = platforms.Last();

            platforms.Add(new Platform(new BoxBoundary(runDownPlatform.Box.TopRight + new Vector2(0, 3.7), width: 1, height: 3)));
            platforms.Add(new Platform(new BoxBoundary(runDownPlatform.Box.TopRight + new Vector2(0.5, -2), width: 3, height: 1)));

            platforms.Last().OnActorStanding += actor => levelContext.DisplayMessage("Yay!");

            var level = new Level(levelContext, platforms);

            runDownPlatform.OnActorStanding += actor => levelContext.DisplayHint(RUN_DOWN_HINT);

            var context = new SingleRunContext();

            level.Actor.OnDrop += () =>
            {
                if (level.Actor.CurrentPlatform == platforms.First())
                {
                    levelContext.SuppressHint(RUN_DOWN_HINT);

                    context.DroppedFromFirstPlatform = true;
                }
            };

            level.Actor.OnDeath += () =>
            {
                if (context.DroppedFromFirstPlatform)
                {
                    levelContext.DisplayMessage("EPIC");
                }
            };

            return level;
        }
    }
}
