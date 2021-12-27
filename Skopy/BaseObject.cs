using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skopy
{
    public class BaseObject
    {
        public Coord coord { get; set; }

        public BaseObject(int x, int y)
        {
            coord = new Coord() { X = x, Y = y };
        }
    }
}
