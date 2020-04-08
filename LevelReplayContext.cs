using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein
{
    public class LevelReplayContext
    {
        readonly HashSet<string> _messagesUsed = new HashSet<string>();
        readonly HashSet<Hint> _hintsSuppressed = new HashSet<Hint>();

        public bool Oneshot(string msg)
        {
            if (_messagesUsed.Contains(msg))
            {
                return false;
            }

            _messagesUsed.Add(msg);
            return true;
        }

        public bool CanShowHint(Hint hint)
        {
            return !_hintsSuppressed.Contains(hint);
        }

        public void SuppressHint(Hint hint)
        {
            _hintsSuppressed.Add(hint);
        }
    }
}
