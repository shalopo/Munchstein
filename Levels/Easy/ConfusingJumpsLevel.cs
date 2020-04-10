using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels.Easy
{
    public class ConfusingJumpsLevel : LevelBuilder
    {
        protected override void Build()
        {
            Add(Platform.Concrete(new Point2(3, 3), width: 5));

            Platforms[0].OnActorStanding += actor => LevelContext.DisplayMessage("That doesn't look too bad...");
            DeathTaunts.Add(Platforms[0], "Seriously?");

            var num_steps = 5;

            for (int i = 0; i < num_steps; i++)
            {
                Add(Platform.PassThrough(new Point2(3, 3 + (i + 1) * 2), width: 5));
            }

            DeathTaunts.Add(Platforms[1], "You're not grasping the gravity of the situation");
            DeathTaunts.Add(Platforms[2], "You're obviously running out of ideas");
            DeathTaunts.Add(Platforms[3], "It sure looked promising, didn't it?");
            DeathTaunts.Add(Platforms[4], "That was an awkward attempt");
            DeathTaunts.Add(Platforms[5], "You were trained to do this right");

            var topLeftCube = new Point2(11.2, 13);
            Add(Platform.Concrete(topLeftCube, width: 1));
            DeathTaunts.Add(Platforms.Last(), "Real smooth...");

            Add(Platform.Concrete(topLeftCube + new Vector2(4, -4), width: 1));

            Add(Platform.Concrete(topLeftCube + new Vector2(6.1, -6), width: 2, height: 2));
            DeathTaunts.Add(Platforms.Last(), "Closette but not quite the cigarette");

            Add(Platform.Concrete(topLeftCube + new Vector2(2, -7), width: 1));
            Platforms.Last().OnActorStanding += actor => LevelContext.DisplayMessage("This is getting exciting! Or is it...");
            DeathTaunts.Add(Platforms.Last(), "LOL. You knew this would fail");

            Add(Platform.Concrete(topLeftCube + new Vector2(7, -2.1), width: 1));
            Platforms.Last().OnActorStanding += actor => LevelContext.DisplayMessage("Finally! You did another pointless move");
            DeathTaunts.Add(Platforms.Last(), "Are your hands sweaty?");

            Add(Platform.Concrete(topLeftCube + new Vector2(2, -10), width: 1));
            Add(Platform.Concrete(topLeftCube + new Vector2(6, -10), width: 2.3));

            Platforms.Last().OnActorStanding += actor => LevelContext.DisplayMessage("Deep in my heart I have always believed in you");
        }

        protected override void PostBuild(Level level)
        {
            level.Actor.OnDeath += () => DeathTaunts.NotifyDeath(level.Actor.LastPlatform);
        }
    }
}
