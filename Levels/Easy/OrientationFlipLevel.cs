using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels.Easy
{
    public class OrientationFlipLevel : LevelBuilder
    {
        static readonly Hint POINT_HINT = new Hint("Try hitting the purple point");

        protected override void Build()
        {
            Add(Platform.OneWay(new Point2(2, 8), width: 9));
            Platforms.Last().OnActorStanding += actor =>
            {
                if (actor.Size == 1)
                {
                    LevelContext.DisplayHint(POINT_HINT);
                }
                else
                {
                    LevelContext.DisplayMessage("Don't think twice, it's alright");
                }
            };

            Add(Platform.OneWay(new Point2(2, 9), width: 2));
            Platforms.Last().OnActorStanding += actor => LevelContext.DisplayHint(POINT_HINT);

            Add(Platform.ConcretePoint(bottomLeft: new Point2(4.5, 9.9)));

            var lowVelocityFails = 0;
            var verticalVelocityUnnecessaryFails = 0;

            Platforms.Last().OnActorColliding += actor =>
            {
                if (actor.Orientation == ActorOrientation.FLAT)
                {
                    LevelContext.DisplayMessage("You're flat out missing the point.\n" + 
                        "There were two puns in that sentence.", seconds: 7);
                }
                else if (actor.Velocity.Y < -2)
                {
                    if (verticalVelocityUnnecessaryFails == 0)
                    {
                        LevelContext.DisplayMessage("You're heading in the wrong direction! I mean, generally in life, it's not a clue or anything", seconds: 7);
                    }
                    else
                    {
                        LevelContext.DisplayMessage("What are your expectations from doing this?");
                    }

                    verticalVelocityUnnecessaryFails++;
                }
                else if (actor.Velocity.Y > 0)
                {
                    LevelContext.DisplayMessage("Not like this!");
                }
                else if (actor.Velocity.X > 2.0)
                {
                    if (lowVelocityFails == 0)
                    {
                        LevelContext.DisplayMessage("Have you forgotten your breakfast?\n" + 
                            "They are STRONGLY recommended!", seconds: 10);
                    }
                    else
                    {
                        LevelContext.DisplayMessage("It's not about hitting the point as much as it's about hitting yourself with the point", seconds: 10);
                    }

                    lowVelocityFails++;
                }
            };

            Platforms.Last().OnActorChangedOrientation += actor =>
            {
                LevelContext.SuppressHint(POINT_HINT);

                if (actor.Size == 1)
                {
                    LevelContext.DisplayMessage("VROOM VROOM! All aboard on the bullet... ehm... brain?", seconds: 7);
                }
                else if (actor.Orientation == ActorOrientation.FLAT)
                {
                    LevelContext.DisplayMessage("I'll just assume that was by mistake. Wink, wink");
                }
                else
                {
                    LevelContext.DisplayMessage("You were once so naive.\n" + 
                        "Now you think you've got everything sorted out.", seconds: 7);
                }
            };

            Add(Platform.OneWay(new Point2(8, 9), width: 1.5));
            Add(Platform.OneWay(new Point2(7, 10), width: 3));
            Add(Platform.Concrete(new Point2(19, 2), width: 2));
            Platforms.Last().OnActorStanding += actor =>
            {
                if (actor.Orientation == ActorOrientation.FLAT)
                {
                    LevelContext.DisplayMessage("Are you fucked or what? Am I right guys?");
                }
            };

            Add(Platform.ConcretePoint(bottomLeft: new Point2(18.5, 2.9)));
            Platforms.Last().OnActorChangedOrientation += actor =>
            {
                if (actor.Size == 1)
                {
                    LevelContext.DisplayMessage("That was soooo down to earth man");
                }
                else
                {
                    LevelContext.DisplayMessage("Do you have De ja vu?", seconds: 2);
                }
            };

            Add(Platform.PassThrough(new Point2(20, 4), width: 1));
            Add(Platform.PassThrough(new Point2(20, 6), width: 1));
            Add(Platform.OneWay(new Point2(18, 8), width: 12));

            Add(Platform.ConcretePoint(bottomLeft: new Point2(26, 10.4)));
            Platforms.Last().OnActorChangedOrientation += actor =>
            {
                if (actor.Size == 1)
                {
                    LevelContext.DisplayMessage("You grew up so fast... sniff");
                }
                else
                {
                    LevelContext.DisplayMessage("Do you have De ja vu again??");
                }
            };

            Add(Platform.PassThrough(new Point2(8, 4), width: 2));
            Platforms.Last().OnActorStanding += actor => LevelContext.DisplayMessage(
                "If you liked this game so far, well... be prepared to hate your own life as well", seconds: 7);

            Add(Platform.Concrete(new Point2(2, 1), width: 9));
            Platforms.Last().OnActorStanding += actor =>
            {
                if (actor.Orientation == ActorOrientation.FLAT)
                {
                    LevelContext.DisplayMessage("It is possible that you will begin losing your mind just about... now", seconds: 7);
                }
            };

            Add(Platform.ConcretePoint(bottomLeft: new Point2(4, 2)));
            Platforms.Last().OnActorChangedOrientation += actor =>
            {
                LevelContext.DisplayMessage("You've figured something out on your own! That's new!");
            };

            Platforms.Last().OnActorColliding += actor =>
            {
                if (actor.Orientation == ActorOrientation.FLAT)
                {
                    LevelContext.DisplayMessage("You may press R once you are DOWN for it.\n" +
                        "R is for \"regret everything\"", seconds: 10);
                }
            };

            Add(Platform.Concrete(new Point2(12.8, 10), width: 0.2, height: 0.5));

            Platforms.Last().OnActorStanding += actor =>
            {
                if (actor.Size == 1)
                {
                    LevelContext.DisplayMessage("You have just unlocked a new ability: Uselesness", seconds: 7);
                }
                else if (actor.Orientation == ActorOrientation.FLAT)
                {
                    LevelContext.DisplayMessage("Well well well. Look who's back in town");
                }
            };

            Platforms.Last().OnActorColliding += actor =>
            {
                if (actor.Velocity.X >= 0)
                {
                    LevelContext.DisplayMessage("Yes, yes, but why?");
                }
                else
                {
                    LevelContext.DisplayMessage("It's a shame to see you go");
                }
            };

            Add(Platform.Concrete(new Point2(12.8, 16), width: 0.2, height: 0.2));
            Add(Platform.Concrete(new Point2(12.8, 15), width: 0.2, height: 2.7));
            Platforms.Last().OnActorColliding += actor =>
            {
                if (actor.Velocity.X < 0)
                {
                    LevelContext.DisplayMessage("I would pity you but I'm not human");
                }
            };

            Add(Platform.PassThrough(new Point2(21.5, 9.5), width: 4.5));

            Add(Platform.OneWay(new Point2(21, 11), width: 5));
            Platforms.Last().OnActorStanding += actor =>
            {
                if (actor.Size == 1)
                {
                    LevelContext.DisplayMessage("Did you lose something here?");
                }
            };

            Add(Platform.OneWay(new Point2(5, 10), width: 5));
            Add(Platform.OneWay(new Point2(7.25, 10.75), width: 0.5));
            Add(Platform.OneWay(new Point2(6.5, 12), width: 2.1));
            Add(Platform.PassThrough(new Point2(3, 12.9), width: 6));

            Add(Platform.OneWay(new Point2(2.5, 13.6), width: 7));
            Add(Platform.OneWay(new Point2(2, 14), width: 8));
            Platforms.Last().OnActorStanding += actor =>
            {
                if (actor.Size == 1)
                {
                    LevelContext.DisplayMessage("Remind me of your strategy...");
                }
                else if (actor.Orientation == ActorOrientation.TALL)
                {
                    LevelContext.DisplayMessage("The cake was a lie");
                }
            };

            Add(Platform.Concrete(new Point2(5, 12.3), width: 0.3, height: 1.8));

            Add(Platform.Concrete(new Point2(10, 12.8), width: 0.3, height: 0.6));
            Platforms.Last().OnActorStanding += actor =>
            {
                if (actor.Size == 1)
                {
                    LevelContext.DisplayMessage("Define 'pointless'");
                }
            };

            Add(Platform.Concrete(new Point2(10, 15.1), width: 0.3, height: 0.2));
            Platforms.Last().OnActorColliding += actor =>
            {
                if (actor.Size == 2)
                {
                    LevelContext.DisplayMessage("Muhahahaha");
                }
            };

            Add(Platform.Concrete(new Point2(2, 17.3), width: 10));

            Add(Platform.Concrete(new Point2(10.8, 11.3), width: 1.5));
            Platforms.Last().OnActorStanding += actor =>
            {
                if (actor.Size == 1)
                {
                    LevelContext.DisplayMessage("It's nice think that being a KNOB should be enough to open this door, isn't it?", seconds: 7);
                }
                else if (actor.Orientation == ActorOrientation.FLAT)
                {
                    LevelContext.DisplayMessage("May I help you?");
                }
            };
        }

        protected override void PostBuild(Level level)
        {
            level.Actor.CanChangeOrientation = true;
            level.Door.Size = 2;
            level.Door.Location -= Vector2.X_UNIT / 4;

            level.Munch = new Munch(new Point2(9, 2.5));

            level.Actor.OnMunch += munch =>
            {
                if (level.Actor.Orientation == ActorOrientation.TALL)
                {
                    LevelContext.DisplayMessage("You know what to do now... Right?");
                }
            };
        }
    }
}
