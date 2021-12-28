namespace Skopy
{
    public static class SkopySolver
    {

        public static double Solve(List<Toy> toys, List<Tree> trees)
        {
            var currentPos = new Coord() { X = 0, Y = 0 };
            var currentTree = new Coord() { X = 0, Y = 0 };
            var totalLength = 0.0;
            var longestLengthFromTree = 0.0;
            //return RecursiveSolve(toys, trees, currentPos, previousAnchor);

            foreach (var nextToy in toys)
            {
                Utils.Print($"Skopy is at {currentPos.X}, {currentPos.Y}");
                Utils.Print($"Anchored at {currentTree.X}, {currentTree.Y}");
                Utils.Print($"Next toy at {nextToy.Coord.X}, {nextToy.Coord.Y}");

                Coord? nextTree = null;
                var possibleTrees = trees.GetInTriangle(currentPos, currentTree, nextToy.Coord);
                Utils.Print($"Possible trees: {possibleTrees.Count}");

                if (possibleTrees.Count == 0)
                {
                    var distanceFromTree = Utils.GetDistance(currentTree, currentPos);
                    if (distanceFromTree > longestLengthFromTree)
                    {
                        longestLengthFromTree = distanceFromTree;
                        Utils.Print($"New longest length from tree is {longestLengthFromTree}");
                    }
                }
                else if (possibleTrees.Count == 1)
                {
                    nextTree = possibleTrees[0].Coord;
                }
                else
                {
                    while (possibleTrees.Count > 0)
                    {
                        nextTree = possibleTrees.GetClosestToLine(currentTree, currentPos).Coord;
                        Utils.Print($"Next tree at {nextTree}");
                        totalLength += Utils.GetDistance(currentTree, nextTree.Value);
                        Utils.Print($"Total length is now {totalLength}");
                        Coord? intersect = null;
                        int extension = 0;
                        while (!intersect.HasValue)
                        {
                            var leashLineEnd = Utils.ExtendLine(currentTree, nextTree.Value, extension += 1000);
                            intersect = Utils.LineIntersect(currentTree, leashLineEnd, currentPos, nextToy.Coord);
                            Utils.Print($"Found intersect at {intersect}");
                        }
                        currentTree = nextTree.Value;
                        currentPos = intersect.Value;
                        possibleTrees = possibleTrees.GetInTriangle(currentPos, currentTree, nextToy.Coord);
                        Utils.Print($"Possible trees after prune is {possibleTrees.Count}");
                    }
                    longestLengthFromTree = Utils.GetDistance(currentTree, nextToy.Coord);
                }

                currentPos = nextToy.Coord;
                Console.ReadKey();
            }

            Utils.Print($"No more toys");
            return totalLength + longestLengthFromTree;

        }

        public static double RecursiveSolve(
            List<Toy> toys,
            List<Tree> trees,
            Coord currentPos, 
            Coord previousAnchor)
        {
            Utils.Print($"Skopy is at {currentPos.X}, {currentPos.Y}");
            Utils.Print($"Anchored at {previousAnchor.X}, {previousAnchor.Y}");

            // Mark toy as chewed
            toys.MarkAsChewed(currentPos);

            // Find closest toy that is not chewed
            var closestToy = toys.GetClosestToCoord(currentPos);

            // We've chewed the last toy. Much sad :'(
            if (closestToy == null)
            {
                Utils.Print($"No more toys");
                return Utils.GetDistance(previousAnchor, currentPos);
            }

            Utils.Print($"Closest toy at {closestToy.Coord.X}, {closestToy.Coord.Y}");

            // Check triangle between previousAnchor, currentPos and nextToy for anchors
            var newAnchors = trees.GetInTriangle(currentPos, previousAnchor, closestToy.Coord);

            Coord? nextAnchor = null;
            if (newAnchors.Count > 1)
            {
                nextAnchor = newAnchors.GetClosestToLine(previousAnchor, closestToy.Coord).Coord;
                Utils.Print($"Multiple anchors");
            }
            else if (newAnchors.Count == 1)
            {
                nextAnchor = newAnchors[0].Coord;
                Utils.Print($"Next anchor at {nextAnchor.Value.X}, {nextAnchor.Value.Y}");
            }
            else
            {
                Utils.Print($"No anchors");
            }

            if (nextAnchor.HasValue)
                // We have a new anchor
                return Utils.GetDistance(previousAnchor, nextAnchor.Value) +
                    RecursiveSolve(toys, trees, closestToy.Coord, nextAnchor.Value);
            else
                return RecursiveSolve(toys, trees, closestToy.Coord, previousAnchor);
        }

    }
}
