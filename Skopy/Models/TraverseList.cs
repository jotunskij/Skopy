using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skopy
{
    public class TraversalEntry
    {
        public Tree tree { get; set; }
        public int clockwiseRotations { get; set; }

        public TraversalEntry(Tree tree, int clockwiseRotations)
        {
            this.tree = tree;
            this.clockwiseRotations = clockwiseRotations;
        }

        public bool IsOrigin()
        {
            return tree.Coord.X == 0 && tree.Coord.Y == 0;
        }

        public override string ToString()
        {
            return $"{tree} with rotation {clockwiseRotations}";
        }
    }

    public class TraverseList
    {
        public List<TraversalEntry> entries { get; set; } = new List<TraversalEntry>();

        public string ToString()
        {
            return Environment.NewLine + string.Join(Environment.NewLine, entries);
        }

        private bool ClockwiseRotation(Tree tree, Coord nextToy)
        {
            var nrOfEntries = entries.Count;
            Line lastLeashLine;
            if (nrOfEntries == 1)
            {
                lastLeashLine = new Line(entries[nrOfEntries - 1].tree.Coord, tree.Coord);
            }
            else
            {
                lastLeashLine = new Line(entries[nrOfEntries - 2].tree.Coord, entries[nrOfEntries - 1].tree.Coord);
            }
            var treeToToyLine = new Line(entries[nrOfEntries - 1].tree.Coord, nextToy);
            var treeAngle = Utils.GetAngleForVector(Utils.LineToVector(treeToToyLine));
            var leashAngle = Utils.GetAngleForVector(Utils.LineToVector(lastLeashLine));
            var difference = Utils.GetAngleDifference(treeAngle, leashAngle);
            return difference < 0;
        }

        public Tree? HandleTreeTraversal(Tree tree, Coord nextToy)
        {
            var goingClockwise = ClockwiseRotation(tree, nextToy);
            var rotation = goingClockwise ? 1 : -1;
            var entryCount = entries.Count;
            var lastEntry = entries.Last();
            if (entryCount > 1 && lastEntry.tree == tree)
            {
                Utils.Print($"Modifying rotation of {tree} with {rotation}");
                entries[entryCount - 1] = new TraversalEntry(tree, lastEntry.clockwiseRotations += rotation);
            }
            else
            {
                Utils.Print($"Attaching to tree {tree} with rotation {rotation}");
                entries.Add(new TraversalEntry(tree, rotation));
            }
            
            if (!lastEntry.IsOrigin() && lastEntry.clockwiseRotations == 0)
            {
                var treeToRemove = lastEntry.tree.Copy();
                Utils.Print($"Removing tree {treeToRemove}");
                entries.RemoveAt(entryCount - 1);
                return treeToRemove;
            }
            return tree;
        }

        public Tree GetCurrentTree()
        {
            return entries.Last().tree;
        }

        public double GetLength()
        {
            if (entries.Count < 2)
                return 0;
            double length = 0;
            for (var i = 0; i < entries.Count - 1; i++)
            {
                length += Utils.GetDistance(entries[i].tree.Coord, entries[i + 1].tree.Coord);
            }
            return length;
        }
    }
}
