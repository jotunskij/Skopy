using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skopy
{
    public class BaseObject
    {
        public Coord Coord { get; set; }

        public BaseObject(int x, int y)
        {
            Coord = new Coord() { X = x, Y = y };
        }
    }
}
