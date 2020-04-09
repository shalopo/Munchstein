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
            DeathTaunts.Add(null, "You should have stayed at school");

            Add(Platform.Concrete(new Point2(1, 9.5), width: 2));

            for (int i = 0; i < 5; i++)
            {
                Add(Platform.Concrete(new Point2(4 + i * i / 6.5 + i * 2, 12 - i), width: 5));
                Add(Platform.Concrete(Platforms.Last().Box.TopLeft - new Vector2(1, 2.5), width: 1.3));
            }

            Platforms[0].OnActorStanding += actor => LevelContext.DisplayHint(RIGHT_HINT);
            Platforms[4].OnActorStanding += actor => LevelContext.SuppressHint(RIGHT_HINT);

            Platforms.Last().OnActorStanding += actor => LevelContext.DisplayHint(DOOR_HINT);
        }

        protected override void PostBuild(Level level)
        {
            level.Actor.CanJump = false;
            level.Actor.OnDeath += () => DeathTaunts.NotifyDeath(level.Actor.LastPlatform);
        }
    }
}
