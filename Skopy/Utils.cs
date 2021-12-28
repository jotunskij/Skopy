namespace Skopy
{
    public struct Coord
    {
        public int X;
        public int Y;

        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(Coord p1, Coord p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(Coord p1, Coord p2)
        {
            return p1.X != p2.X || p1.Y != p2.Y;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

    public struct Line
    {
        public Line(Coord p1, Coord p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public Coord p1;
        public Coord p2;

        public override string ToString()
        {
            return $"{p1} -> {p2}";
        }
    }

    public static class TraverseList
    {
        public static List<Tree> trees = new List<Tree>();

        public static Coord? FindBacktrackIntersect(Line line)
        {
            var nrOfTrees = trees.Count;

            if (nrOfTrees < 2)
                return null;

            var lastLeashLine = Utils.ExtendLine(new Line(trees[nrOfTrees - 2].Coord, trees[nrOfTrees - 1].Coord), 100000);
            Utils.Print($"Finding backtrack intersection between {line} and {lastLeashLine}");
            Utils.FindIntersection(
                lastLeashLine,
                line,
                out bool _,
                out bool segmentsIntersects,
                out Coord? intersect, out Coord? _, out Coord? _);
            if (segmentsIntersects)
                return intersect;
            else
                return null;
        }

        public static Tree GetCurrentTree()
        {
            return trees.Last();
        }

        public static Tree? Backtrack()
        {
            if (trees.Count > 1)
            {
                var lastIndex = trees.Count - 1;
                var treeToRemove = trees[lastIndex].Copy();
                trees.RemoveAt(lastIndex);
                return treeToRemove;
            }
            return null;
        }

        public static double GetLength()
        {
            if (trees.Count < 2)
                return 0;
            double length = 0;
            for (var i = 0; i < trees.Count - 1; i++)
            {
                length += Utils.GetDistance(trees[i].Coord, trees[i + 1].Coord);
            }
            return length;
        }
    }

    public static class Utils
    {

        public static void Print(string msg)
        {
            Console.WriteLine(msg);
        }

        public static Toy? GetClosestToCoord(this List<Toy> toys, Coord pos)
        {
            if (toys == null || toys.Count == 0)
                return null;

            return toys.Where(t => !t.chewed).MinBy(t => GetDistance(pos, t.Coord));
        }

        public static void MarkAsChewed(this List<Toy> toys, Coord pos)
        {
            if (toys == null || toys.Count == 0)
                return;

            var foundToy = toys.Where(t => t.Coord.X == pos.X && t.Coord.Y == pos.Y).FirstOrDefault();
            if (foundToy != null)
            {
                Print($"Marked toy as chewed at {foundToy.Coord.X}, {foundToy.Coord.Y}");
                foundToy.chewed = true;
            }
        }

        public static Tree? GetClosestToLine(this List<Tree> trees, Coord start, Coord end)
        {
            if (trees == null || trees.Count == 0)
                return null;

            return trees.MinBy(t => AngleBetween(start, end, t.Coord));
            //return trees.MinBy(t => FindDistanceToSegment(t.Coord, start, end));
        }

        public static List<Tree> GetInTriangle(
            this List<Tree> trees, 
            Coord pos, 
            Coord anchor, 
            Coord toy, 
            Tree? latestRemovedTree)
        {
            if (trees == null || trees.Count == 0)
                return new List<Tree>();
            
            Print($"Getting trees contained in {pos}, {anchor}, {toy}");
            return trees
                .Where(t => PointInTriangle(t.Coord, pos, anchor, toy) && 
                t.Coord != anchor &&
                (latestRemovedTree == null || t.Coord != latestRemovedTree.Coord)).ToList();
        }

        public static double GetDistance(Coord p1, Coord p2)
        {
            return Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
        }

        // https://stackoverflow.com/questions/1211212/how-to-calculate-an-angle-from-three-points

        public static double AngleBetween(Coord origin, Coord pt1, Coord pt2)
        {
            var angle = Math.Atan2(pt2.Y - origin.Y, pt2.X - origin.X) -
                Math.Atan2(pt1.Y - origin.Y, pt1.X - origin.X);
            if (angle < 0) 
                angle = 2 * Math.PI + angle;
            return angle;
        }

        // https://stackoverflow.com/questions/13458992/angle-between-two-vectors-2d

        public static double AngleBetween(Coord vector1, Coord vector2)
        {
            double sin = vector1.X * vector2.Y - vector2.X * vector1.Y;
            double cos = vector1.X * vector2.X + vector1.Y * vector2.Y;

            var angle = Math.Atan2(sin, cos) * (180 / Math.PI);
            Print($"Got angle {angle} for {vector2}");
            return angle;
        }

        // https://stackoverflow.com/questions/7740507/extend-a-line-segment-a-specific-distance

        public static Line ExtendLine(Line line, double lengthFactor)
        {
            var lenAB = Math.Sqrt(Math.Pow(line.p1.X - line.p2.X, 2.0) + Math.Pow(line.p1.Y - line.p2.Y, 2.0));
            var newEnd = new Coord();
            newEnd.X = (int)(line.p2.X + (line.p2.X - line.p1.X) / lenAB * lengthFactor);
            newEnd.Y = (int)(line.p2.Y + (line.p2.Y - line.p1.Y) / lenAB * lengthFactor);
            return new Line(line.p1, newEnd);
        }

        // http://csharphelper.com/blog/2014/08/determine-where-two-lines-intersect-in-c/

        public static void FindIntersection(
            Line l1, Line l2,
            out bool lines_intersect, out bool segments_intersect,
            out Coord? intersection,
            out Coord? close_p1, out Coord? close_p2)
        {
            // Get the segments' parameters.
            float dx12 = l1.p2.X - l1.p1.X;
            float dy12 = l1.p2.Y - l1.p1.Y;
            float dx34 = l2.p2.X - l2.p1.X;
            float dy34 = l2.p2.Y - l2.p1.Y;

            // Solve for t1 and t2
            float denominator = (dy12 * dx34 - dx12 * dy34);

            float t1 =
                ((l1.p1.X - l2.p1.X) * dy34 + (l2.p1.Y - l1.p1.Y) * dx34)
                    / denominator;
            if (float.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                lines_intersect = false;
                segments_intersect = false;
                intersection = null;
                close_p1 = null;
                close_p2 = null;
                return;
            }
            lines_intersect = true;

            float t2 =
                ((l2.p1.X - l1.p1.X) * dy12 + (l1.p1.Y - l2.p1.Y) * dx12)
                    / -denominator;

            // Find the point of intersection.
            intersection = new Coord((int)(l1.p1.X + dx12 * t1), (int)(l1.p1.Y + dy12 * t1));

            // The segments intersect if t1 and t2 are between 0 and 1.
            segments_intersect =
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));

            // Find the closest points on the segments.
            if (t1 < 0)
            {
                t1 = 0;
            }
            else if (t1 > 1)
            {
                t1 = 1;
            }

            if (t2 < 0)
            {
                t2 = 0;
            }
            else if (t2 > 1)
            {
                t2 = 1;
            }

            close_p1 = new Coord((int)(l1.p1.X + dx12 * t1), (int)(l1.p1.Y + dy12 * t1));
            close_p2 = new Coord((int)(l2.p1.X + dx34 * t2), (int)(l2.p1.Y + dy34 * t2));
        }

        // https://rosettacode.org/wiki/Find_the_intersection_of_two_lines#C.23

        /*public static Coord? LineIntersect(Line l1, Line l2)
        {
            double a1 = l1.p2.Y - l1.p1.Y;
            double b1 = l1.p1.X - l1.p2.X;
            double c1 = a1 * l1.p1.X + b1 * l1.p1.Y;

            double a2 = l2.p2.Y - l2.p1.Y;
            double b2 = l2.p1.X - l2.p2.X;
            double c2 = a2 * l2.p1.X + b2 * l2.p1.Y;

            double delta = a1 * b2 - a2 * b1;
            //If lines are parallel, the result will be (NaN, NaN).
            return delta == 0 ? null
                : new Coord() { X = (int)((b2 * c1 - b1 * c2) / delta), Y = (int)((a1 * c2 - a2 * c1) / delta) };
        }*/

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
            Coord closest;
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = new Coord() { X=p1.X, Y=p1.Y }; 
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
                closest = new Coord() { X=p1.X, Y=p1.Y };
                dx = pt.X - p1.X;
                dy = pt.Y - p1.Y;
            }
            else if (t > 1)
            {
                closest = new Coord() { X = p2.X, Y = p2.Y };
                dx = pt.X - p2.X;
                dy = pt.Y - p2.Y;
            }
            else
            {
                closest = new Coord() { X = (int)(p1.X + t * dx), Y = (int)(p1.Y + t * dy) };
                dx = pt.X - closest.X;
                dy = pt.Y - closest.Y;
            }

            return Math.Sqrt(dx * dx + dy * dy);
        }

    }
}
