using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skopy
{
    public struct Coord
    {
        public int X;
        public int Y;
    }

    public struct CoordF
    {
        public double X;
        public double Y;
    }

    public static class Utils
    {

        public static Toy? GetClosestToCoord(this List<Toy> toys, Coord pos)
        {
            if (toys == null || toys.Count == 0)
                return null;

            return toys.Where(t => !t.chewed).MinBy(t => GetDistance(pos, t.coord));
        }

        public static void MarkAsChewed(this List<Toy> toys, Coord pos)
        {
            if (toys == null || toys.Count == 0)
                return;

            var foundToy = toys.Where(t => t.coord.X == pos.X && t.coord.Y == pos.Y).FirstOrDefault();
            if (foundToy != null)
                foundToy.chewed = true;
        }

        public static Tree? GetClosestToLine(this List<Tree> trees, Coord start, Coord end)
        {
            if (trees == null || trees.Count == 0)
                return null;

            return trees.MinBy(t => FindDistanceToSegment(t.coord, start, end));
        }

        public static List<Tree> GetInTriangle(this List<Tree> trees, Coord p1, Coord p2, Coord p3)
        {
            if (trees == null || trees.Count == 0)
                return new List<Tree>();

            return trees.Where(t => PointInTriangle(t.coord, p1, p2, p3)).ToList();
        }

        public static double GetDistance(Coord p1, Coord p2)
        {
            return Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
        }

        // The following two functions were stolen from:
        // https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle

        public static double Sign(Coord p1, Coord p2, Coord p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        public static bool PointInTriangle(Coord pt, Coord v1, Coord v2, Coord v3)
        {
            double d1, d2, d3;
            bool has_neg, has_pos;

            d1 = Sign(pt, v1, v2);
            d2 = Sign(pt, v2, v3);
            d3 = Sign(pt, v3, v1);

            has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }

        // Stolen from:
        // http://csharphelper.com/blog/2016/09/find-the-shortest-distance-between-a-point-and-a-line-segment-in-c/

        private static double FindDistanceToSegment(
            Coord pt, Coord p1, Coord p2)
        {
            CoordF closest;
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = new CoordF() { X=p1.X, Y=p1.Y }; 
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            double t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = new CoordF() { X=p1.X, Y=p1.Y };
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new CoordF() { X = p2.X, Y = p2.Y };
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new CoordF() { X = p1.X + t * dx, Y = p1.Y + t * dy };
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }

    }
}
