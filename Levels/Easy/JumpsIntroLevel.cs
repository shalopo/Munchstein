using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels.Easy
{
    class JumpsIntroLevel : LevelBuilder
    {
        static readonly Hint JUMP_HINT = new Hint("Hit SPACE to jump");
        static readonly Hint MOMENTUM_HINT = new Hint("Keep up the good momentum, metaphorically speaking");

        protected override void Build()
        {
            DeathTaunts.Add(null, "What a shame, I had such high hopes for you");

            for (int i = 0; i < 5; i++)
            {
                Add(Platform.Concrete(new Point2(3 + i * i / 2.8 + 2 * i, 4 + i), width: 1.5));
            }

            Platforms[0].OnActorStanding += actor => LevelContext.DisplayHint(JUMP_HINT);
            Platforms[Platforms.Count - 2].OnActorStanding += actor =>
            {
                if (actor.Velocity.IsZero)
                {
                    LevelContext.DisplayHint(MOMENTUM_HINT);
                }
            };

            DeathTaunts.Add(Platforms[Platforms.Count - 2], "You snooze, you lose");

            Platforms.Last().OnActorStanding += actor => LevelContext.DisplayMessage("You're not too old for this after all!");
        }

        protected override void PostBuild(Level level)
        {
            level.Actor.OnJump += () =>
            {
                LevelContext.SuppressHint(JUMP_HINT);
                LevelContext.DisplayMessage("You got it!", seconds: 2);
            };
        }
    }
}
