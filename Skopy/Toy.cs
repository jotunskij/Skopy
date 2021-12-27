using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skopy
{
    public class Toy : BaseObject
    {
        public bool chewed { get; set; }

        public Toy(int x, int y) : base(x, y) { }
    }
}
