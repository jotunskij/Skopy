namespace Skopy
{
    public class IntersectionStruct
    {
        public double Distance { get; set; }
        public Tree Tree { get; set; }
        public Coord Intersection { get; set; }

        public IntersectionStruct(double d, Tree t, Coord i)
        {
            Distance = d;
            Tree = t;
            Intersection = i;
        }
    }
}
