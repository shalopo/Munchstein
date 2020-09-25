using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels.Easy
{
    class IntroLevel : LevelBuilder
    {
        static readonly Hint RIGHT_HINT = new Hint("Hit the RIGHT arrow");
        static readonly Hint DOOR_HINT = new Hint("Yay! Hit the UP arrow to open the door");

        protected override void Build()
        {
            Add(Platform.Concrete(new Point2(1, 9.5), width: 2));

            for (int i = 0; i < 5; i++)
            {
                Add(Platform.Concrete(new Point2(3 + i * i / 6.5 + i * 2, 9.5 - i), width: 1.3));
            }

            Platforms[0].OnActorStanding += actor => LevelContext.DisplayHint(RIGHT_HINT);

            Platforms[2].OnActorStanding += actor =>
            {
                LevelContext.SuppressHint(RIGHT_HINT);
                LevelContext.DisplayMessage("Good. Was just making sure you have pulse", seconds: 2);
            };

            DeathTaunts.Add(Platforms[4], "Stop being so insecure!");
            DeathTaunts.Add(Platforms[5], "You're a goner");

            Platforms.Last().OnActorStanding += actor => LevelContext.DisplayHint(DOOR_HINT);
        }

        protected override void PostBuild(Level level)
        {
            level.CanActorJump = false;
            level.OnActorJump += actor => LevelContext.DisplayMessage("When exactly did we say you can jump?");
        }
    }
}
