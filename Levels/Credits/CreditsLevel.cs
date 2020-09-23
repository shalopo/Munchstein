using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels.Credits
{
    public class CreditsLevel : LevelBuilder
    {
        protected override void Build()
        {
        }

        protected override void PostBuild(Level level)
        {
            LevelContext.DisplayMessage("Nice to see you, come again!");

            level.Actor.Location = new Point2(15, 15);

            var rand = new Random();

            level.Actor.OnDeath += () =>
            {
                level.Actor.Location = new Point2(rand.Next() % 20 + 5, rand.Next() % 5 + 5);
                level.Actor.Velocity = new Vector2(rand.Next() % 20 - 5, rand.Next() % 20);
            };
        }
    }
}
