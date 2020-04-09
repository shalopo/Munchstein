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
                new IntroLevel(),
                new JumpsIntroLevel(),
                new RunDownLevel(),
                new ConfusingJumpsLevel(),
            };
        }
    }
}
