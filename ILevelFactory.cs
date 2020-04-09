using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein
{
    public interface ILevelFactory
    {
        Level Create(ILevelContext levelContext);
    }
}
