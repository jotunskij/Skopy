namespace Skopy
{
    public class SkopySolver
    {
        public List<Toy> toys { get; set; } = new List<Toy>();
        public List<Tree> trees { get; set; } = new List<Tree>();
        public Coord currentPos { get; set; } = new Coord();
        public double longestLength { get; set; }
        public int currentToyIndex { get; set; } = 0;
        public TraverseList traverseList { get; set; } = new TraverseList();
        public bool solved { get; set; }
        public double answerFromAnsFile { get; set; }

        public Coord GetCurrentPos()
        {
            return currentPos;
        }

        public void Init(List<Toy> toys, List<Tree> trees)
        {
            this.toys = toys;
            this.trees = trees;
            currentPos = new Coord(0, 0);
            traverseList.entries.Add(new TraversalEntry(new Tree(0, 0), 1));
            longestLength = 0.0;
        }

        // Returns null while not solved, answer when solved
        public double? Solve()
        {
            // Get the next toy in the list
            var currentToy = toys[currentToyIndex];
            var nextToy = currentToy;
            Utils.Print($"Skopy is at {currentPos.X}, {currentPos.Y}");
            Utils.Print($"Anchored at {traverseList.GetCurrentTree().Coord.X}, {traverseList.GetCurrentTree().Coord.Y}");
            Utils.Print($"Next toy at {nextToy.Coord.X}, {nextToy.Coord.Y}");
            Utils.Print($"Traverse list is {traverseList.ToString()}");
            //Console.ReadKey();

            // Find all trees that are possible hits
            // using the triangle formed by Skopy, the currently attached tree and the next toy
            var possibleTrees = trees.GetInTriangle(
                currentPos, 
                traverseList.GetCurrentTree().Coord, 
                nextToy.Coord,
                null);
            Utils.Print($"Possible trees: {possibleTrees.Count}");

            while (possibleTrees.Count > 0)
            {
                // Get our second to last and last attached trees (if available)
                var secondLastTree = traverseList.entries.First().tree.Coord;
                if (traverseList.entries.Count() > 2)
                {
                    secondLastTree = traverseList.entries[traverseList.entries.Count() - 2].tree.Coord;
                }
                var currentTree = traverseList.GetCurrentTree().Coord;

                // Loop all the possible tree hits
                var treeDistances = new List<IntersectionStruct>();
                foreach (var possibleTree in possibleTrees)
                {
                    Coord? intersection;
                    bool intersects;
                    Line intersectionLine;

                    if (currentTree == possibleTree.Coord)
                    {
                        // We are passing the last tree, which means we are backtracking/detaching
                        // We'll extend a line from the second to last to the last attached tree
                        // to get a direction, then create a line from the last tree along that direction
                        // to get an intersection with Skopys current trajectory
                        var backtrackLine = Utils.ExtendLine(new Line(secondLastTree, possibleTree.Coord), 100000);
                        intersectionLine = new Line(possibleTree.Coord, backtrackLine.p2);
                    }
                    else
                    {
                        // We are passing a tree that is not the last one we passed, so we'll make a new
                        // attachment. Create a line between the tree and the latest attached tree to get
                        // the intersecting point with Skopys trajectory
                        var nextTreeLine = Utils.ExtendLine(new Line(currentTree, possibleTree.Coord), 100000);
                        intersectionLine = new Line(possibleTree.Coord, nextTreeLine.p2);
                    }

                    // Use the previously calculated intersection line to get at what point Skopy
                    // will either attach och detach from the tree
                    Utils.FindIntersection(
                        new Line(currentPos, nextToy.Coord),
                        intersectionLine,
                        out bool _,
                        out intersects,
                        out intersection,
                        out Coord? _,
                        out Coord? _);

                    if (intersects && intersection is not null)
                    {
                        // If the lines intersect, we'll add the intersection data to a list
                        treeDistances.Add(
                            new IntersectionStruct(
                                Utils.GetDistance(intersection, currentPos), 
                                possibleTree,
                                intersection));
                    }
                }

                // Make sure we only compute valid intersections
                var validIntersections = treeDistances
                    .Where(t => t.Intersection != currentPos)
                    .Count();
                if (validIntersections == 0)
                    break;

                // Get the closest intersection to Skopy
                var closestIntersection = treeDistances
                    .Where(t => t.Intersection != currentPos)
                    .MinBy(t => t.Distance);

                // Null reference safe guard
                if (closestIntersection is null)
                    break;

                // Process the attaching/detaching tree in our traversal list
                var lastModified = traverseList.HandleTreeTraversal(
                    closestIntersection.Tree, 
                    nextToy.Coord);
                
                // Place Skopy at our intersection
                currentPos = closestIntersection.Intersection;

                // Get new possible tree hits given Skopys new position,
                // making sure we don't include the newly processed tree in the calcuations
                possibleTrees = trees.GetInTriangle(
                    currentPos,
                    traverseList.GetCurrentTree().Coord,
                    nextToy.Coord,
                    lastModified);
            }

            // Place Skopy at next toy
            currentPos = nextToy.Coord;

            // *chew chew*

            // Keep track of current tree + farthest toy, if that should happen
            // to be longer than the completed "course"
            var distanceFromTree = Utils.GetDistance(traverseList.GetCurrentTree().Coord, currentPos);
            var totalDistance = traverseList.GetLength() + distanceFromTree;
            Utils.Print($"Current tree/total distance is {distanceFromTree}/{totalDistance}");
            if (totalDistance > longestLength)
            {
                longestLength = totalDistance;
                Utils.Print($"New longest length is {longestLength}");
            }

            if (currentToyIndex == toys.Count - 1)
            {
                // Much sad. No chew. :'(
                Utils.Print($"No more toys");
                solved = true;
                return longestLength;
            }
            else
            {
                // MOAR TOYS!!11
                currentToyIndex++;
                return null;
            }

        }

    }
}
