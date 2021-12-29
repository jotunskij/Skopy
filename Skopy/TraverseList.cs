using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skopy
{
    public struct TraversalEntry
    {
        public Tree tree;
        public int count;
        public int orientation;

        public TraversalEntry(Tree tree, int orientation)
        {
            this.tree = tree;
            this.orientation = orientation;
            this.count = 1;
        }
    }

    public static class TraverseList
    {
        public static List<TraversalEntry> entries = new List<TraversalEntry>();

        public static void AddTree(Tree tree, int orientation)
        {
            if (entries.Count > 1 && entries.Last().tree == tree)
            {
                var lastEntry = entries.Last();
                lastEntry.count++;
            }
            else
            {
                entries.Add(new TraversalEntry(tree, orientation));
            }
        }

        public static Coord? FindBacktrackIntersect(Line line)
        {
            var nrOfTrees = entries.Count;

            if (nrOfTrees < 2)
                return null;

            var lastTree = entries[nrOfTrees - 1].tree;
            var secondLastTree = entries[nrOfTrees - 2].tree;
            var lastLeashLine = Utils.ExtendLine(new Line(secondLastTree.Coord, lastTree.Coord), 100000);
            lastLeashLine = new Line(lastTree.Coord, lastLeashLine.p2);
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
            return entries.Last().tree;
        }

        public static Tree? Backtrack()
        {
            if (entries.Count > 1)
            {
                var lastEntry = entries.Last();
                if (lastEntry.count > 1)
                {
                    lastEntry.count--;
                }
                else
                {
                    var treeToRemove = lastEntry.tree.Copy();
                    entries.Remove(lastEntry);
                    return treeToRemove;
                }
            }
            return null;
        }

        public static double GetLength()
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
