namespace Skopy
{
    public class TraversalEntry
    {
        public Tree Tree { get; set; }
        public int ClockwiseRotations { get; set; }

        public TraversalEntry(Tree tree, int clockwiseRotations)
        {
            this.Tree = tree;
            this.ClockwiseRotations = clockwiseRotations;
        }

        public bool IsOrigin()
        {
            return Tree.Coord.X == 0 && Tree.Coord.Y == 0;
        }

        public override string ToString()
        {
            return $"{Tree} with rotation {ClockwiseRotations}";
        }
    }

    public class TraverseList
    {
        public List<TraversalEntry> Entries { get; set; } = new List<TraversalEntry>();

        public override string ToString()
        {
            return Environment.NewLine + string.Join(Environment.NewLine, Entries);
        }

        // To calculate wether a tree hit is a clockwise or counter clockwise rotation
        // we'll compare the two angles formed by:
        // 1) second to last attached tree -> last attached tree
        // 2) last attached tree -> next toy
        // If the angle will increase when moving from the tree-to-tree line to the
        // next toy line, we are moving in a counter clockwise direction, and vice versa
        private bool ClockwiseRotation(Tree tree, Coord nextToy)
        {
            var nrOfEntries = Entries.Count;
            Line lastLeashLine;
            if (nrOfEntries == 1)
            {
                lastLeashLine = new Line(Entries[nrOfEntries - 1].Tree.Coord, tree.Coord);
            }
            else
            {
                lastLeashLine = new Line(Entries[nrOfEntries - 2].Tree.Coord, Entries[nrOfEntries - 1].Tree.Coord);
            }
            var treeToToyLine = new Line(Entries[nrOfEntries - 1].Tree.Coord, nextToy);
            var treeAngle = Utils.GetAngleForVector(Utils.LineToVector(treeToToyLine));
            var leashAngle = Utils.GetAngleForVector(Utils.LineToVector(lastLeashLine));
            var difference = Utils.GetAngleDifference(treeAngle, leashAngle);
            return difference < 0;
        }

        // Handles tree hits for the leash
        public Tree? HandleTreeTraversal(Tree tree, Coord nextToy)
        {
            // Get rotation around the tree, -1 for CCW and 1 for CW
            var goingClockwise = ClockwiseRotation(tree, nextToy);
            var rotation = goingClockwise ? 1 : -1;

            var entryCount = Entries.Count;
            var lastEntry = Entries.Last();
            if (entryCount > 1 && lastEntry.Tree == tree)
            {
                // We are passing the tree we attached to last, which either means we're
                // backtracking or we are passing it a second time (one entire revolution around the tree)
                // We'll adjust the rotation of the traversal entry with the -1/1 from the rotation
                Utils.Print($"Modifying rotation of {tree} with {rotation}");
                Entries[entryCount - 1] = new TraversalEntry(tree, lastEntry.ClockwiseRotations += rotation);
            }
            else
            {
                // A new tree - add it to the list
                Utils.Print($"Attaching to tree {tree} with rotation {rotation}");
                Entries.Add(new TraversalEntry(tree, rotation));
            }
            
            if (!lastEntry.IsOrigin() && lastEntry.ClockwiseRotations == 0)
            {
                // If the last tree in the list has a rotation value of 0, that
                // means the leash has unravelled and that Skopy has detached from the
                // tree. Remove it from the list.
                var treeToRemove = lastEntry.Tree.Copy();
                Utils.Print($"Removing tree {treeToRemove}");
                Entries.RemoveAt(entryCount - 1);
                return treeToRemove;
            }
            return tree;
        }

        public Tree GetCurrentTree()
        {
            return Entries.Last().Tree;
        }

        // Calculates the complete distance of the current traversal list
        public double GetLength()
        {
            if (Entries.Count < 2)
                return 0;
            double length = 0;
            for (var i = 0; i < Entries.Count - 1; i++)
            {
                length += Utils.GetDistance(Entries[i].Tree.Coord, Entries[i + 1].Tree.Coord);
            }
            return length;
        }
    }
}
