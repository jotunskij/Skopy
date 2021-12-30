namespace Skopy
{
    public static class SkopySolver
    {

        public static double Solve(List<Toy> toys, List<Tree> trees)
        {
            var currentPos = new Coord() { X = 0, Y = 0 };
            TraverseList.entries.Add(new TraversalEntry(new Tree(0, 0), 1));
            var longestLength = 0.0;

            foreach (var nextToy in toys)
            {
                Utils.Print($"Skopy is at {currentPos.X}, {currentPos.Y}");
                Utils.Print($"Anchored at {TraverseList.GetCurrentTree().Coord.X}, {TraverseList.GetCurrentTree().Coord.Y}");
                Utils.Print($"Next toy at {nextToy.Coord.X}, {nextToy.Coord.Y}");
                Utils.Print($"Traverse list is {TraverseList.ToString()}");
                //Console.ReadKey();

                // We've checked backtracking, let's go forward..
                Tree? nextTree = null;
                // Check all trees what could possibly be in the way
                var possibleTrees = trees.GetInTriangle(
                    currentPos, 
                    TraverseList.GetCurrentTree().Coord, 
                    nextToy.Coord,
                    null);
                Utils.Print($"Possible trees: {possibleTrees.Count}");

                while (possibleTrees.Count > 0)
                {
                    var secondLastTree = TraverseList.entries.First().tree.Coord;
                    if (TraverseList.entries.Count() > 2)
                    {
                        secondLastTree = TraverseList.entries[TraverseList.entries.Count() - 2].tree.Coord;
                    }
                    var currentTree = TraverseList.GetCurrentTree().Coord;
                    var treeDistances = new List<IntersectionStruct>();
                    foreach (var possibleTree in possibleTrees)
                    {
                        Coord? intersection;
                        bool intersects;
                        Line intersectionLine;

                        if (currentTree == possibleTree.Coord)
                        {
                            var backtrackLine = Utils.ExtendLine(new Line(secondLastTree, possibleTree.Coord), 100000);
                            intersectionLine = new Line(possibleTree.Coord, backtrackLine.p2);
                        }
                        else
                        {
                            var nextTreeLine = Utils.ExtendLine(new Line(currentTree, possibleTree.Coord), 100000);
                            intersectionLine = new Line(possibleTree.Coord, nextTreeLine.p2);
                        }

                        Utils.FindIntersection(
                            new Line(currentPos, nextToy.Coord),
                            intersectionLine,
                            out bool _,
                            out intersects,
                            out intersection,
                            out Coord? _,
                            out Coord? _);

                        if (intersects)
                        {
                            treeDistances.Add(
                                new IntersectionStruct(
                                    Utils.GetDistance(intersection.Value, currentPos), 
                                    possibleTree,
                                    intersection.Value));
                        }
                    }

                    var validIntersections = treeDistances
                        .Where(t => t.Intersection != currentPos)
                        .Count();
                    if (validIntersections == 0)
                        break;

                    var closestIntersection = treeDistances
                        .Where(t => t.Intersection != currentPos)
                        .MinBy(t => t.Distance);
                    var lastRemoval = TraverseList.HandleTreeTraversal(
                        closestIntersection.Tree, 
                        nextToy.Coord);
                    currentPos = closestIntersection.Intersection;

                    possibleTrees = trees.GetInTriangle(
                        currentPos,
                        TraverseList.GetCurrentTree().Coord,
                        nextToy.Coord,
                        lastRemoval);
                }

                // Place Skopy at next toy
                currentPos = nextToy.Coord;

                // *chew chew*

                // Keep track of current tree + farthest toy, if that should happen
                // to be longer than the completed "course"
                var distanceFromTree = Utils.GetDistance(TraverseList.GetCurrentTree().Coord, currentPos);
                var totalDistance = TraverseList.GetLength() + distanceFromTree;
                Utils.Print($"Current tree/total distance is {distanceFromTree}/{totalDistance}");
                if (totalDistance > longestLength)
                {
                    longestLength = totalDistance;
                    Utils.Print($"New longest length is {longestLength}");
                }

            }

            // Much sad. No chew. :'(
            Utils.Print($"No more toys");
            return longestLength;

        }

    }
}
