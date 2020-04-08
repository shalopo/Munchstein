using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels.Easy
{
    class JumpsIntroLevel : ILevelFactory
    {
        static readonly Hint JUMP_HINT = new Hint("Hit SPACE to jump");
        static readonly Hint MOMENTUM_HINT = new Hint("Keep up the good momentum, metaphorically speaking");

        public Level Create(ILevelContext levelContext)
        {
            var deathTaunts = new DeathTaunts(levelContext);
            deathTaunts.Add(null, "What a shame, I had such high hopes for you");

            var platforms = new List<Platform>();

            var step_width = 1.5;
            var step_height = 1;

            for (int i = 0; i < 5; i++)
            {
                platforms.Add(new Platform(new BoxBoundary(new Point2(3 + i * i / 2.6 + i * 1.3 * step_width, 3 + (i + 1) * step_height),
                                                           width: step_width,
                                                           height: 1)));
            }

            platforms[0].OnActorStanding += actor => levelContext.DisplayHint(JUMP_HINT);
            platforms[platforms.Count - 2].OnActorStanding += actor =>
            {
                if (actor.Velocity.IsZero)
                {
                    levelContext.DisplayHint(MOMENTUM_HINT);
                }
            };

            deathTaunts.Add(platforms[platforms.Count - 2], "You snooze, you lose");

            var level = new Level(levelContext, platforms);

            level.Actor.OnDeath += () => deathTaunts.NotifyDeath(level.Actor.LastPlatform);

            level.Actor.OnJump += () =>
            {
                levelContext.SuppressHint(JUMP_HINT);
                levelContext.DisplayMessage("You got it!", seconds: 2);
            };

            return level;
        }
    }
}
