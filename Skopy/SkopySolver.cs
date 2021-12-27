namespace Skopy
{
    public static class SkopySolver
    {

        public static double Solve(List<Toy> toys, List<Tree> trees)
        {
            var currentPos = new Coord() { X = 0, Y = 0 };
            var currentAnchor = new Coord() { X = 0, Y = 0 };
            var totalLength = 0.0;
            var longestLengthFromAnchor = 0.0;
            //return RecursiveSolve(toys, trees, currentPos, previousAnchor);

            foreach (var nextToy in toys)
            {
                Utils.Print($"Skopy is at {currentPos.X}, {currentPos.Y}");
                Utils.Print($"Anchored at {currentAnchor.X}, {currentAnchor.Y}");
                Utils.Print($"Next toy at {nextToy.coord.X}, {nextToy.coord.Y}");

                // Check triangle between previousAnchor, currentPos and nextToy for anchors
                var newAnchors = trees.GetInTriangle(currentPos, currentAnchor, nextToy.coord);

                // anchors = get all anchors in original triangle
                // if anchors
                //  find closest one
                //  calculate new triangle currentAnchor -> hit tree -> dog position -> next toy
                //  (line intersection between currentAnchor -> hit tree and current pos -> next toy)
                //  add currentAnchor -> hit tree to totalLength
                //  repeat with new triangle instead of original triangle, reusing set of found trees
                // if no anchors
                //  dog reaches new toy

                Coord? nextAnchor = null;
                if (newAnchors.Count > 1)
                {
                    //foreach (var a in newAnchors)
                    //    Utils.Print($"Found anchor {a.coord.X}, {a.coord.Y}");
                    nextAnchor = newAnchors.GetClosestToLine(currentAnchor, currentPos).coord;
                    Utils.Print($"Multiple anchors, using {nextAnchor.Value.X}, {nextAnchor.Value.Y}");
                }
                else if (newAnchors.Count == 1)
                {
                    nextAnchor = newAnchors[0].coord;
                    Utils.Print($"Next anchor at {nextAnchor.Value.X}, {nextAnchor.Value.Y}");
                }
                else
                {
                    Utils.Print($"No anchors from {currentPos.X}, {currentPos.Y} to {nextToy.coord.X}, {nextToy.coord.Y}");
                }

                if (nextAnchor.HasValue)
                {
                    // We have a new anchor
                    totalLength += Utils.GetDistance(currentAnchor, nextAnchor.Value);
                    Utils.Print($"New length is {totalLength}");
                    longestLengthFromAnchor = Utils.GetDistance(nextAnchor.Value, nextToy.coord);
                    Utils.Print($"Initialized longest length from anchor to {longestLengthFromAnchor}");
                    currentAnchor = nextAnchor.Value;
                }
                else
                {
                    var distanceFromAnchor = Utils.GetDistance(currentAnchor, currentPos);
                    if (distanceFromAnchor > longestLengthFromAnchor)
                    {
                        longestLengthFromAnchor = distanceFromAnchor;
                        Utils.Print($"New longest length from anchor is {longestLengthFromAnchor}");
                    }
                }
                currentPos = nextToy.coord;
                //Console.ReadKey();
            }

            Utils.Print($"No more toys");
            return totalLength + longestLengthFromAnchor;

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

            Utils.Print($"Closest toy at {closestToy.coord.X}, {closestToy.coord.Y}");

            // Check triangle between previousAnchor, currentPos and nextToy for anchors
            var newAnchors = trees.GetInTriangle(currentPos, previousAnchor, closestToy.coord);

            Coord? nextAnchor = null;
            if (newAnchors.Count > 1)
            {
                nextAnchor = newAnchors.GetClosestToLine(previousAnchor, closestToy.coord).coord;
                Utils.Print($"Multiple anchors");
            }
            else if (newAnchors.Count == 1)
            {
                nextAnchor = newAnchors[0].coord;
                Utils.Print($"Next anchor at {nextAnchor.Value.X}, {nextAnchor.Value.Y}");
            }
            else
            {
                Utils.Print($"No anchors");
            }

            if (nextAnchor.HasValue)
                // We have a new anchor
                return Utils.GetDistance(previousAnchor, nextAnchor.Value) +
                    RecursiveSolve(toys, trees, closestToy.coord, nextAnchor.Value);
            else
                return RecursiveSolve(toys, trees, closestToy.coord, previousAnchor);
        }

    }
}
