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

            Add(Platform.Concrete(new Point2(14.8, 8.8), width: 1, height: 1));
            DeathTaunts.Add(Platforms.Last(), "Well yes, but actually no");

            Add(Platform.Concrete(new Point2(18, 7.4), width: 2, height: 2.2));
            DeathTaunts.Add(Platforms.Last(), "Absolutely but not really");

            Add(Platform.Concrete(new Point2(18.6, 7.55), width: 0.5, height: 0.15));
            Platforms.Last().OnActorStanding += actor => LevelContext.DisplayMessage("If it fits, I sits");

            Add(Platform.Concrete(new Point2(17.85, 6.2), width: 0.15, height: 0.5));
            Add(Platform.Concrete(new Point2(19, 5.2), width: 0.5, height: 0.15));
            Add(Platform.Concrete(new Point2(20.0, 6.9), width: 0.15, height: 0.5));

            _boop = Add(Platform.Concrete(new Point2(18.7, 10.4), width: 1, height: 1));
            Platforms.Last().OnActorLanding += actor => LevelContext.DisplayMessage("This is exciting!");
            DeathTaunts.Add(Platforms.Last(), "Did you really think it would be that easy?");
            
            Add(Platform.Concrete(new Point2(13, 3), width: 1, height: 1));
            Platforms.Last().OnActorLanding += actor => LevelContext.DisplayMessage("Captain obvious to the rescue!");
            DeathTaunts.Add(Platforms.Last(), "LOL you knew this would fail");

            Add(Platform.Concrete(new Point2(18.1, 3), width: 3.2));

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
                            if (actor.Velocity.X > 0)
                            {
                                LevelContext.DisplayMessage("Maybe you're on to something");
                            }
                            else
                            {
                                LevelContext.DisplayMessage("Do you need DIRECTIONs?");
                            }
                            break;
                    }

                    numBoops++;
                }
            };
        }
    }
}
