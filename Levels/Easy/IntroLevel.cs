using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels.Easy
{
    class IntroLevel : ILevelFactory
    {
        static readonly Hint RIGHT_HINT = new Hint("Hit the RIGHT arrow");
        static readonly Hint DOOR_HINT = new Hint("Yay! Hit the UP arrow to open the door");

        public Level Create(ILevelContext levelContext)
        {
            var deathTaunts = new DeathTaunts(levelContext);
            deathTaunts.Add(null, "You should have stayed at school");

            var platforms = new List<Platform>();

            platforms.Add(new Platform(new BoxBoundary(new Point2(1, 9.5), width: 2, height: 1)));

            for (int i = 0; i < 5; i++)
            {
                platforms.Add(new Platform(new BoxBoundary(new Point2(4 + i * i / 6.5 + i * 2, 12 - i),
                                                           width: 5,
                                                           height: 1)));
                
                platforms.Add(new Platform(new BoxBoundary(platforms.Last().Box.TopLeft - new Vector2(1, 2.5),
                                                           width: 1.3,
                                                           height: 1)));
            }

            platforms[0].OnActorStanding += actor => levelContext.DisplayHint(RIGHT_HINT);
            platforms[4].OnActorStanding += actor => levelContext.SuppressHint(RIGHT_HINT);

            platforms.Last().OnActorStanding += actor => levelContext.DisplayHint(DOOR_HINT);

            var level = new Level(levelContext, platforms);

            level.Actor.OnDeath += () => deathTaunts.NotifyDeath(level.Actor.LastPlatform);
            level.Actor.OnJump += () => levelContext.DisplayMessage("Hey! We haven't learned to jump yet!");

            return level;
        }
    }
}
