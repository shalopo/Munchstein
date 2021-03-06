﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels
{
    public class DebugLevel : LevelBuilder
    {
        protected override void Build()
        {
            //Add(Platform.Splitter(new Point2(7, 7)));
            //Add(Platform.Splitter(new Point2(5, 5)));
            //Add(Platform.Splitter(new Point2(9, 5)));
            Add(Platform.Concrete(new Point2(0, 3), width: 20));
        }

        protected override void PostBuild(Level level)
        {
            level.Actors.First().Orientation = ActorOrientation.TALL;
            level.Actors.First().Size = 2;
            level.Actors.First().Location = new Point2(7, 3);

            var orientations = ActorOrientation.SQUARE;
            level.Actors.Add(new Actor(level, new Point2(7, 8)) { Orientation = orientations, Size = 2  });
            level.Actors.Add(new Actor(level, new Point2(7, 10)) { Orientation = orientations, Size = 1 });

            level.Munches.Add(new Munch(new Point2(7.4, 12.6)));
        }
    }
}
