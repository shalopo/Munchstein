using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein
{
    public class LevelsSequence : IEnumerable<ILevelFactory>
    {
        List<ILevelFactory> _levelFactories = new List<ILevelFactory>();

        public int NumLevels => _levelFactories.Count;

        public void Add(ILevelFactory levelFactory) => _levelFactories.Add(levelFactory);
        public ILevelFactory this[int index] => _levelFactories[index];

        public IEnumerator<ILevelFactory> GetEnumerator()
        {
            return _levelFactories.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _levelFactories.GetEnumerator();
        }
    }
}
