namespace Skopy
{
    public class Toy : BaseObject
    {
        // Required for json serialization
        public Toy() : base(0, 0) { }
        public Toy(int x, int y) : base(x, y) { }
    }
}
