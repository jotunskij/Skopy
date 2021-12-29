namespace Skopy
{
    public static class SkopySolver
    {

        public static double Solve(List<Toy> toys, List<Tree> trees)
        {
            var currentPos = new Coord() { X = 0, Y = 0 };
            TraverseList.AddTree(new Tree(0, 0), 0);
            var longestLength = 0.0;

            foreach (var nextToy in toys)
            {
                // TODO: We need to keep track of which way (clockwise, CC) each attachment is oriented
                // and take this into consideration when unattaching

                Utils.Print($"Skopy is at {currentPos.X}, {currentPos.Y}");
                Utils.Print($"Anchored at {TraverseList.GetCurrentTree().Coord.X}, {TraverseList.GetCurrentTree().Coord.Y}");
                Utils.Print($"Next toy at {nextToy.Coord.X}, {nextToy.Coord.Y}");
                Utils.Print($"Traverse list has {TraverseList.entries.Count} elements");
                Console.ReadKey();

                Tree? latestRemovedTree = null; 
                while (true)
                {
                    var heading = new Line(currentPos, nextToy.Coord);
                    var backTrackIntersect = TraverseList.FindBacktrackIntersect(heading);

                    Utils.Print($"Backtrack intersect is {backTrackIntersect}");
                    if (backTrackIntersect != null)
                    {
                        var treesInBackTrack = trees.GetInTriangle(
                            currentPos, 
                            TraverseList.GetCurrentTree().Coord,
                            backTrackIntersect.Value, 
                            latestRemovedTree);
                        Utils.Print($"Trees in the way of backtrack is {treesInBackTrack.Count}");
                        if (treesInBackTrack.Count == 0)
                        {
                            Utils.Print($"Backtracking to {backTrackIntersect.Value} and removing last tree");
                            currentPos = backTrackIntersect.Value;
                            latestRemovedTree = TraverseList.Backtrack();
                        }
                        else
                        {
                            // We have a tree that we will attach to before releasing last tree
                            break;
                        }
                    }
                    else
                    {
                        // No intersect with last traverse line
                        break;
                    }
                }

                Tree? nextTree = null;
                var possibleTrees = trees.GetInTriangle(
                    currentPos, 
                    TraverseList.GetCurrentTree().Coord, 
                    nextToy.Coord,
                    latestRemovedTree);
                Utils.Print($"Possible trees: {possibleTrees.Count}");

                if (possibleTrees.Count == 1)
                {
                    TraverseList.AddTree(possibleTrees[0], 0);
                }
                else
                {
                    while (possibleTrees.Count > 0)
                    {
                        nextTree = possibleTrees.GetClosestToLine(TraverseList.GetCurrentTree().Coord, currentPos);
                        Utils.Print($"Next tree at {nextTree}");
                        Coord? intersect = null;
                        var leashLineEnd = Utils.ExtendLine(new Line(TraverseList.GetCurrentTree().Coord, nextTree.Coord), 100000);
                        Utils.FindIntersection(
                            leashLineEnd, 
                            new Line(currentPos, nextToy.Coord),
                            out bool _,
                            out bool segmentsIntersect,
                            out intersect, out Coord? _, out Coord? _);
                        Utils.Print($"Found intersect at {intersect}");
                        TraverseList.AddTree(nextTree, 0);
                        currentPos = intersect.Value;
                        possibleTrees = possibleTrees.GetInTriangle(
                            currentPos, 
                            TraverseList.GetCurrentTree().Coord, 
                            nextToy.Coord, 
                            latestRemovedTree);
                        Utils.Print($"Possible trees after prune is {possibleTrees.Count}");
                    }
                }

                currentPos = nextToy.Coord;

                var distanceFromTree = Utils.GetDistance(TraverseList.GetCurrentTree().Coord, currentPos);
                var totalDistance = TraverseList.GetLength() + distanceFromTree;
                Utils.Print($"Current tree/total distance is {distanceFromTree}/{totalDistance}");
                if (totalDistance > longestLength)
                {
                    longestLength = totalDistance;
                    Utils.Print($"New longest length is {longestLength}");
                }

            }

            Utils.Print($"No more toys");
            return longestLength;

        }

    }
}
