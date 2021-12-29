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
                // TODO: We need to keep track of which way (clockwise, CC) each attachment is oriented
                // and take this into consideration when unattaching

                Utils.Print($"Skopy is at {currentPos.X}, {currentPos.Y}");
                Utils.Print($"Anchored at {TraverseList.GetCurrentTree().Coord.X}, {TraverseList.GetCurrentTree().Coord.Y}");
                Utils.Print($"Next toy at {nextToy.Coord.X}, {nextToy.Coord.Y}");
                Utils.Print($"Traverse list is {TraverseList.ToString()}");
                Console.ReadKey();

                Tree? latestRemovedTree = null;
                
                // Check backtracking
                while (true)
                {
                    // Get Skopys current heading
                    var heading = new Line(currentPos, nextToy.Coord);
                    // See if he'll intersect the backtracking line
                    var backTrackIntersect = TraverseList.FindBacktrackIntersect(heading);

                    Utils.Print($"Backtrack intersect is {backTrackIntersect}");
                    if (backTrackIntersect != null)
                    {
                        // Skopy is backtracking, let's make sure he doesn't get stuck
                        // on another tree before completing backtrack
                        var treesInBackTrack = trees.GetInTriangle(
                            currentPos, 
                            TraverseList.GetCurrentTree().Coord,
                            backTrackIntersect.Value, 
                            latestRemovedTree);
                        Utils.Print($"Trees in the way of backtrack is {treesInBackTrack.Count}");
                        if (treesInBackTrack.Count == 0)
                        {
                            // No trees, release latest leash > tree connection
                            Utils.Print($"Backtracking to {backTrackIntersect.Value} and removing last tree");
                            currentPos = backTrackIntersect.Value;
                            latestRemovedTree = TraverseList.HandleTreeTraversal(
                                TraverseList.GetCurrentTree(),
                                nextToy.Coord);
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

                // We've checked backtracking, let's go forward..
                Tree? nextTree = null;
                // Check all trees what could possibly be in the way
                var possibleTrees = trees.GetInTriangle(
                    currentPos, 
                    TraverseList.GetCurrentTree().Coord, 
                    nextToy.Coord,
                    latestRemovedTree);
                Utils.Print($"Possible trees: {possibleTrees.Count}");

                if (possibleTrees.Count == 1)
                {
                    // Only one tree in the way - attach to it and continue to toy
                    TraverseList.HandleTreeTraversal(possibleTrees[0], nextToy.Coord);
                }
                else
                {
                    // Multiple trees blocking
                    while (possibleTrees.Count > 0)
                    {
                        // Find the tree with the shallowest angle against the leash
                        nextTree = possibleTrees.GetClosestToLine(TraverseList.GetCurrentTree().Coord, currentPos);
                        Utils.Print($"Next tree at {nextTree}");
                        Coord? intersect = null;
                        // Locate where Skopy will be when attaching to tree
                        var leashLineEnd = Utils.ExtendLine(new Line(TraverseList.GetCurrentTree().Coord, nextTree.Coord), 100000);
                        Utils.FindIntersection(
                            leashLineEnd, 
                            new Line(currentPos, nextToy.Coord),
                            out bool _,
                            out bool segmentsIntersect,
                            out intersect, out Coord? _, out Coord? _);
                        Utils.Print($"Found intersect at {intersect}");
                        // Add tree to traverse list
                        TraverseList.HandleTreeTraversal(nextTree, nextToy.Coord);
                        // Place Skopy at the intersection
                        currentPos = intersect.Value;
                        // Get all possible trees in the new triangle formed
                        possibleTrees = possibleTrees.GetInTriangle(
                            currentPos, 
                            TraverseList.GetCurrentTree().Coord, 
                            nextToy.Coord, 
                            latestRemovedTree);
                        Utils.Print($"Possible trees after prune is {possibleTrees.Count}");
                    }
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
