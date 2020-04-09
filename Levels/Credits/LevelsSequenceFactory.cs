using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein.Levels.Credits
{
    public class LevelsSequenceFactory
    {
        public static LevelsSequence Create()
        {
            return new LevelsSequence
            {
                new LevelFactory<CreditsLevel>(),
            };
        }
    }
}
