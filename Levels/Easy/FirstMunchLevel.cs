﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels.Easy
{
    public class FirstMunchLevel : LevelBuilder
    {
        Platform upperGate;

        protected override void Build()
        {
            Add(Platform.Concrete(new Point2(4, 3), width: 4, height: 0.3));
            Platforms.Last().OnActorStanding += actor => LevelContext.DisplayMessage("Find your inner peace");

            Add(Platform.Concrete(new Point2(11, 3), width: 1.5, height: 0.3));
            Add(Platform.OneWay(new Point2(7, 7), width: 5.7));
            Add(Platform.PassThrough(new Point2(16, 8), width: 1.2));
            Add(Platform.OneWay(new Point2(18.2, 5), width: 1.3));
            Add(Platform.OneWay(new Point2(10, 9), width: 4.2));

            Add(Platform.OneWay(new Point2(15.5, 9), width: 5.5));
            Platforms.Last().OnActorStanding += actor =>
            {
                if (actor.Size == 1)
                {
                    LevelContext.DisplayMessage("Just do it");
                }
            };

            Add(Platform.PassThrough(new Point2(14.5, 6), width: 2.3));
            Add(Platform.OneWay(new Point2(19, 8), width: 1.3));
            Add(Platform.OneWay(new Point2(15, 7), width: 1.8));
            Add(Platform.PassThrough(new Point2(9, 4), width: 2.5));
            Add(Platform.PassThrough(new Point2(10.5, 8), width: 2.6));
            Add(Platform.PassThrough(new Point2(18.4, 6), width: 1));
            Add(Platform.OneWay(new Point2(14, 5), width: 2.5));
            Add(Platform.PassThrough(new Point2(20, 7), width: 1));
            Add(Platform.PassThrough(new Point2(13.7, 4), width: 1));
            Add(Platform.Concrete(new Point2(16.5, 5.3), width: 0.3, height: 0.3));
            Add(Platform.PassThrough(new Point2(16.5, 2.3), width: 0.3));
            Add(Platform.Concrete(new Point2(15.5, 3.1), width: 1.3, height: 0.3));
            Add(Platform.Concrete(new Point2(17.8, 3), width: 1.7, height: 0.3));
            Add(Platform.Concrete(new Point2(17.8, 1.5), width: 1, height: 0.3));
            Add(Platform.Concrete(new Point2(18.3, 1), width: 0.6, height: 0.3));
            Add(Platform.PassThrough(new Point2(18.3, 2.3), width: 0.3));
            Add(Platform.Concrete(new Point2(19.5, 4), width: 0.3));
            Add(Platform.Concrete(new Point2(19.5, 2.7), width: 0.3, height: 1.5));

            Add(Platform.Concrete(new Point2(21, 8), width: 0.3, height: 0.5));
            upperGate = Platforms.Last();
            upperGate.OnActorStanding += actor => LevelContext.DisplayMessage("Ah! I see you are a man of culture");

            Add(Platform.Concrete(new Point2(21, 15), width: 0.3, height: 2.3));
            Add(Platform.Concrete(new Point2(21, 10.8), width: 0.3, height: 0.8));
            Add(Platform.Concrete(new Point2(23, 4), width: 4, height: 0.3));

            Platforms.Last().OnActorStanding += actor => LevelContext.DisplayMessage(
                "Or a woman of culture.\n" +
                "I don't know, I can't really see...");
        }

        protected override void PostBuild(Level level)
        {
            level.Munch = new Munch(new Point2(21.15, 9.9));
            level.Door.Size = 2;

            Platforms.Last().OnActorStanding += actor =>
            {
                if (actor.Size != level.Door.Size)
                {
                    LevelContext.DisplayMessage("You're too small, dummy!");
                }
            };

            level.Actor.OnMunch += (munch) =>
            {
                if (level.Actor.LastPlatform == upperGate)
                {
                    LevelContext.DisplayMessage("Stop trying to break the puzzle");
                }
                else
                {
                    LevelContext.DisplayMessage("Yum! Big boys can jump higher!");
                }

                level.Actor.Location = munch.Location - new Vector2(0.5, 0);
                level.Actor.Velocity = Vector2.ZERO;
            };

            level.Actor.OnJump += () =>
            {
                if (level.Actor.Size == 2)
                {
                    LevelContext.DisplayMessage("Jumping higher is a good thing! Right?...");
                }
            };
        }
    }
}