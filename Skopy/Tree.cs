using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skopy
{
    public class Tree : BaseObject
    {
        public Tree(int x, int y) : base(x, y) { }

        public Tree Copy()
        {
            return new Tree(Coord.X, Coord.Y);
        }

    }
}
