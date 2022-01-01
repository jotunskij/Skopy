namespace Skopy
{
    public class Coord
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coord()
        {
        }

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
}
