namespace Skopy
{

    public static class Utils
    {

        public static bool IsDebug()
        {
            return Environment.GetEnvironmentVariable("DEBUG") != null;
        }

        public static void Print(string msg)
        {
            if (IsDebug())
                Console.WriteLine(msg);
        }

        public static List<Tree> GetInTriangle(
            this List<Tree> trees, 
            Coord pos, 
            Coord anchor, 
            Coord toy,
            Tree? excludedTree = null)
        {
            if (trees == null || trees.Count == 0)
                return new List<Tree>();
            
            Print($"Getting trees contained in {pos}, {anchor}, {toy}");
            return trees
                .Where(t => PointInTriangle(t.Coord, pos, anchor, toy) &&
                    (excludedTree is null || excludedTree.Coord != t.Coord)).ToList();
        }

        public static double GetDistance(Coord p1, Coord p2)
        {
            return Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
        }

        public static Coord LineToVector(Line line)
        {
            return new Coord(line.p2.X - line.p1.X, line.p2.Y - line.p1.Y);
        }

        // https://math.stackexchange.com/questions/1649841/signed-angle-difference-without-conditions

        public static double GetAngleDifference(double a1, double a2)
        {
            return (a1 - a2 + 540) % 360 - 180;
        }

        // https://stackoverflow.com/questions/35271222/getting-the-angle-from-a-direction-vector

        public static double GetAngleForVector(Coord vector)
        {
            var angle = Math.Atan2(vector.Y, vector.X);
            var degrees = 180 * angle / Math.PI;
            return (360 + degrees) % 360;
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
                t1 = 0;
            else if (t1 > 1)
                t1 = 1;
            if (t2 < 0)
                t2 = 0;
            else if (t2 > 1)
                t2 = 1;

            close_p1 = new Coord((int)(l1.p1.X + dx12 * t1), (int)(l1.p1.Y + dy12 * t1));
            close_p2 = new Coord((int)(l2.p1.X + dx34 * t2), (int)(l2.p1.Y + dy34 * t2));
        }

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

    }
}
