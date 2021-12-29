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

        public override string ToString()
        {
            return Coord.ToString();
        }

        public static bool operator ==(BaseObject p1, BaseObject p2)
        {
            if (p1 is null && p2 is null)
                return true;
            if (p1 is null || p2 is null)
                return false;
            return p1.Coord == p2.Coord;
        }

        public static bool operator !=(BaseObject p1, BaseObject p2)
        {
            return p1.Coord != p2.Coord;
        }
    }
}
