using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels.Easy
{
    public class ConfusingJumpsLevel : ILevelFactory
    {
        public Level Create(ILevelContext levelContext)
        {
            var deathTaunts = new DeathTaunts(levelContext);

            var platforms = new List<Platform>
            {
                new Platform(new BoxBoundary(new Point2(3, 3), width: 5, height: 1))
            };

            platforms[0].OnActorStanding += actor => levelContext.DisplayMessage("That doesn't look too bad...");
            deathTaunts.Add(platforms[0], "Seriously?");

            var num_steps = 5;

            for (int i = 0; i < num_steps; i++)
            {
                platforms.Add(new Platform(new BoxBoundary(new Point2(3, 3 + (i + 1) * 2), width: 5, height: 0.1),
                                                           is_passthrough: true));
            }

            deathTaunts.Add(platforms[1], "Now that's insulting");
            deathTaunts.Add(platforms[2], "You're obviously running out of ideas");
            deathTaunts.Add(platforms[3], "It sure looked promising, didn't it?");
            deathTaunts.Add(platforms[4], "That was an awkward attempt");
            deathTaunts.Add(platforms[5], "You were trained to do this right");

            var topLeftCube = new Point2(11.2, 13);
            platforms.Add(new Platform(new BoxBoundary(topLeftCube, width: 1, height: 1)));
            deathTaunts.Add(platforms.Last(), "Real smooth...");

            platforms.Add(new Platform(new BoxBoundary(topLeftCube + new Vector2(4, -4), width: 1, height: 1)));

            platforms.Add(new Platform(new BoxBoundary(topLeftCube + new Vector2(6.1, -6), width: 2, height: 2)));
            deathTaunts.Add(platforms.Last(), "Closette but not quite the cigarette");

            platforms.Add(new Platform(new BoxBoundary(topLeftCube + new Vector2(2, -7), width: 1, height: 1)));
            platforms.Last().OnActorStanding += actor => levelContext.DisplayMessage("This is getting exciting! Or is it...");
            deathTaunts.Add(platforms.Last(), "LOL. You knew this would fail");

            platforms.Add(new Platform(new BoxBoundary(topLeftCube + new Vector2(7, -2.1), width: 1, height: 1)));
            platforms.Last().OnActorStanding += actor => levelContext.DisplayMessage("Finally! You did another pointless move");
            deathTaunts.Add(platforms.Last(), "Are your hands sweaty?");

            platforms.Add(new Platform(new BoxBoundary(topLeftCube + new Vector2(2, -10), width: 1, height: 1)));
            platforms.Add(new Platform(new BoxBoundary(topLeftCube + new Vector2(6, -10), width: 2.3, height: 1)));

            platforms.Last().OnActorStanding += actor => levelContext.DisplayMessage("Deep in my heart I always believed in you");

            var level = new Level(levelContext, platforms);

            level.Actor.OnDeath += () => deathTaunts.NotifyDeath(level.Actor.LastPlatform);

            return level;
        }
    }
}
