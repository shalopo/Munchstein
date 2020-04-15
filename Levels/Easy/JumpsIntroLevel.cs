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
        static readonly Hint HOLD_JUMP_HINT = new Hint("Press and hold SPACE while moving to auto jump at the last moment");
        static readonly Hint MOMENTUM_HINT = new Hint("Keep up the good momentum, metaphorically speaking");

        protected override void Build()
        {
            DeathTaunts.Add(null, "What a shame, I had such high hopes for you");

            const double STEP_WIDTH = 1.2;

            Add(Platform.Concrete(new Point2(2.5, 4), width: STEP_WIDTH));
            Platforms.Last().OnActorStanding += actor => LevelContext.DisplayHint(JUMP_HINT);

            for (int i = 1; i < 5; i++)
            {
                Add(Platform.Concrete(new Point2(3 + i * i / 2.8 + 2 * i, 4 + i), width: STEP_WIDTH));
            }

            Platforms[2].OnActorStanding += actor => LevelContext.DisplayHint(HOLD_JUMP_HINT);

            Platforms[3].OnActorStanding += actor =>
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
