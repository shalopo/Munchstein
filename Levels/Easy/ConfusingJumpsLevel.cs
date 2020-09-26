using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels.Easy
{
    public class ConfusingJumpsLevel : LevelBuilder
    {
        Platform _boop;

        protected override void Build()
        {
            Add(Platform.PassThrough(new Point2(8, 13), width: 3));

            Platforms[0].OnActorLanding += actor => LevelContext.DisplayMessage("That doesn't look too bad...");

            Add(Platform.Concrete(new Point2(15.35, 8.8), width: 0.5, height: 0.5));
            Platforms.Last().OnActorStanding += actor => LevelContext.DisplayMessage("If it fits, I sits");
            DeathTaunts.Add(Platforms.Last(), "Well yes, but actually no");

            Add(Platform.Concrete(new Point2(18, 7.4), width: 2, height: 2.2));
            DeathTaunts.Add(Platforms.Last(), "Absolutely but not really");

            Add(Platform.Concrete(new Point2(19.5, 7.5), width: 0.5, height: 0.1));
            DeathTaunts.Add(Platforms.Last(), "Maybe this isn't the right game for you");

            _boop = Add(Platform.Concrete(new Point2(18.8, 10.4), width: 0.9, height: 0.9));
            Platforms.Last().OnActorLanding += actor => LevelContext.DisplayMessage("This is exciting! Or is it...");
            DeathTaunts.Add(Platforms.Last(), "So cautious. Yet so dead");
            
            Add(Platform.Concrete(new Point2(13.1, 3), width: 1, height: 1));
            Platforms.Last().OnActorLanding += actor => LevelContext.DisplayMessage("Captain obvious to the rescue!");
            DeathTaunts.Add(Platforms.Last(), "LOL you knew this would fail");

            Add(Platform.Concrete(new Point2(22.0, 13.0), width: 0.3, height: 8.0));

            Add(Platform.Concrete(new Point2(18.3, 3), width: 2.5));

            Platforms.Last().OnActorLanding += actor => LevelContext.DisplayMessage("Deep in my heart I have always believed in you");
        }

        protected override void PostBuild(Level level)
        {
            int numBoops = 0;

            _boop.OnActorColliding += actor =>
            {
                if (actor.Velocity.Y > 0)
                {
                    switch (numBoops)
                    {
                        case 0:
                            LevelContext.DisplayMessage("BOOP");
                            break;
                        case 1:
                            LevelContext.DisplayMessage("BOOP BOOP");
                            break;
                        case 2:
                            LevelContext.DisplayMessage("Are you looking for mushrooms?");
                            break;
                        default:
                            break;
                    }

                    numBoops++;
                }
            };
        }
    }
}
