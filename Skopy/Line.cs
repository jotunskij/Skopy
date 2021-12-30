using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skopy
{
    public class Line
    {
        public Coord p1 { get; set; }
        public Coord p2 { get; set; }

        public Line(Coord p1, Coord p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public Line(int p1X, int p1Y, int p2X, int p2Y)
        {
            p1 = new Coord(p1X, p1Y);
            p2 = new Coord(p2X, p2Y);
        }

        public override string ToString()
        {
            return $"{p1} -> {p2}";
        }
    }
}
