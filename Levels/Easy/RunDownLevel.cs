﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels.Easy
{
    public class RunDownLevel : LevelBuilder
    {
        static readonly Hint RUN_DOWN_HINT = new Hint("For a hint, Press DOWN for 2 seconds");
        static readonly Hint ONE_WAY_HINT = new Hint("It's safe to step on the red platform");

        public bool DroppedFromFirstPlatform { get; set; }

        Platform _highOneWayPlatform;
        Platform _checkpoint;
        bool _steppedOnHighOneWay = false;


        protected override void Build()
        {
            for (int i = 0; i < 2; i++)
            {
                Add(Platform.PassThrough(new Point2(3, 5 + i * 2), width: i + i * i / 6.0 + 3));
            }

            var runDownPlatform = Platforms.Last();
            runDownPlatform.OnActorStanding += actor => LevelContext.DisplayHint(RUN_DOWN_HINT);

            Add(Platform.Concrete(runDownPlatform.Box.TopRight + new Vector2(0.3, 3.7), width: 1, height: 3));
            Platforms.Last().OnActorStanding += actor => LevelContext.DisplayMessage("Was it especially important to reach here?");

            Add(Platform.Concrete(runDownPlatform.Box.TopRight + new Vector2(0.5, -2), width:0.6));

            _checkpoint = Add(Platform.Concrete(new Point2(12, 5), width: 3.5));
            Platforms.Last().OnActorStanding += actor =>
            {
                LevelContext.SuppressHint(RUN_DOWN_HINT);
                LevelContext.DisplayMessage("Good!", seconds: 1);
            };

            Add(Platform.PassThrough(new Point2(12, 7), width: 3));

            Add(Platform.PassThrough(new Point2(12, 8), width: 3));
            Platforms.Last().OnActorStanding += actor => LevelContext.DisplayMessage("Aren't you a genius...");

            Add(Platform.PassThrough(new Point2(12, 9), width: 3));
            Platforms.Last().OnActorStanding += actor => LevelContext.DisplayHint(ONE_WAY_HINT);

            Add(Platform.OneWay(new Point2(11, 10), width: 9));

            Add(Platform.OneWay(new Point2(10, 11), width: 12));
            _highOneWayPlatform = Platforms.Last();

            _highOneWayPlatform.OnActorStanding += actor =>
            {
                _steppedOnHighOneWay = true;
                LevelContext.SuppressHint(ONE_WAY_HINT);
                LevelContext.DisplayMessage("But it may disappoint you");
            };

            Add(Platform.PassThrough(new Point2(21, 8), width: 1));
        }

        protected override void PostBuild(Level level)
        {
            level.OnActorDrop += actor =>
            {
                if (actor.CurrentPlatform == Platforms.First())
                {
                    LevelContext.SuppressHint(RUN_DOWN_HINT);

                    DroppedFromFirstPlatform = true;
                }
            };

            level.OnActorDeath += actor =>
            {
                if (DroppedFromFirstPlatform)
                {
                    LevelContext.DisplayMessage("EPIC");
                }
                else if (_steppedOnHighOneWay)
                {
                    LevelContext.DisplayMessage("Now I'm disappointed");
                }
            };

            _checkpoint.OnActorLanding += actor => level.SaveCheckpoint();
        }
    }
}
