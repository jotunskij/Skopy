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
        public int clockwiseRotations;

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
            return $"tree {tree} with rotation {clockwiseRotations}";
        }
    }

    public static class TraverseList
    {
        public static List<TraversalEntry> entries = new List<TraversalEntry>();

        public static string ToString()
        {
            return string.Join(Environment.NewLine, entries);
        }

        private static bool ClockwiseRotation(Tree tree, Coord nextToy)
        {
            var nrOfEntries = entries.Count;
            if (nrOfEntries > 1)
            {
                var lastLeashLine = new Line(entries[nrOfEntries - 1].tree.Coord, entries[nrOfEntries - 2].tree.Coord);
                var treeToToyLine = new Line(entries[nrOfEntries - 1].tree.Coord, nextToy);
                var leashArg = Utils.ArgumentOfVector(lastLeashLine);
                var toyArg = Utils.ArgumentOfVector(treeToToyLine);
                return leashArg > toyArg;
            }
            return false;
        }

        public static Tree? HandleTreeTraversal(Tree tree, Coord nextToy)
        {
            var goingClockwise = ClockwiseRotation(tree, nextToy);
            var rotation = goingClockwise ? 1 : -1;
            var entryCount = entries.Count;
            var lastEntry = entries.Last();
            if (entryCount > 1 && lastEntry.tree == tree)
            {
                lastEntry.clockwiseRotations += rotation;
            }
            else
            {
                entries.Add(new TraversalEntry(tree, rotation));
            }
            
            if (!lastEntry.IsOrigin() && lastEntry.clockwiseRotations == 0)
            {
                var treeToRemove = lastEntry.tree.Copy();
                entries.Remove(lastEntry);
                return treeToRemove;
            }
            return null;
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
