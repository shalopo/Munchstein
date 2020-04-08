using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels.Easy
{
    public class TowerLeapLevel : ILevelFactory
    {
        public Level Create(ILevelContext levelControl)
        {
            var platforms = new List<Platform>
            {
                new Platform(new BoxBoundary(new Point2(3, 3), width: 5, height: 1))
            };

            for (int i = 0; i < 4; i++)
            {
                platforms.Add(new Platform(new BoxBoundary(new Point2(3, 3 + (i + 1) * 2), width: 5,  height: 0.1),  
                                                           is_passthrough: true));
            }

            platforms.Last().OnActorStanding += actor => levelControl.DisplayMessage("Believe in yourself!");

            platforms.Add(new Platform(new BoxBoundary(new Point2(12.9, 3), width: 10, height: 1)));

            platforms.Last().OnActorStanding += actor => levelControl.DisplayMessage("Born to jump, weren't you!");

            var level = new Level(levelControl, platforms);

            level.Actor.OnDeath += () => levelControl.DisplayMessage("That's a shame");

            return level;
        }
    }
}
