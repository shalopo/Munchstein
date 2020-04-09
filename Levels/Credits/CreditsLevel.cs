using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels.Credits
{
    public class CreditsLevel : ILevelFactory
    {
        public Level Create(ILevelContext levelControl)
        {
            var level = new Level(levelControl, new List<Platform>());

            var rand = new Random();

            level.Actor.Location = new Point2(rand.Next() % 20 + 5, rand.Next() % 20 + 5);
            level.Actor.Velocity = new Vector2(rand.Next() % 10, rand.Next() % 20);

            return level;
        }
    }
}
