using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels.Easy
{
    public class LevelsSequenceFactory
    {
        public static LevelsSequence Create()
        {
            return new LevelsSequence
            {
                new LevelFactory<IntroLevel>(),
                new LevelFactory<JumpsIntroLevel>(),
                new LevelFactory<RunDownLevel>(),
                new LevelFactory<ConfusingJumpsLevel>(),
                new LevelFactory<FirstMunchLevel>(),
            };
        }
    }
}
