using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein
{
    public interface ILevelContext
    {
        void NotifyLevelComplete();
        void DisplayMessage(string msg, int? seconds = null);
        void DisplayHint(Hint hint);
        void SuppressHint(Hint hint);
    }
}
