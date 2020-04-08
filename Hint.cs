using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Munchstein
{
    public class Hint
    {
        public Hint(string msg) => Message = msg;

        public string Message { get; set; }
    }
}
