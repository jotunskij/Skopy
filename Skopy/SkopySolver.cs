namespace Skopy
{
    public class SkopySolver
    {
        public List<Toy> Toys { get; set; } = new List<Toy>();
        public List<Tree> Trees { get; set; } = new List<Tree>();
        public Coord CurrentPos { get; set; } = new Coord();
        public double LongestLength { get; set; }
        public int CurrentToyIndex { get; set; } = 0;
        public TraverseList TraverseList { get; set; } = new TraverseList();
        public bool Solved { get; set; }
        public double AnswerFromAnsFile { get; set; }

        public Coord GetCurrentPos()
        {
            return CurrentPos;
        }

        public void Init(List<Toy> toys, List<Tree> trees)
        {
            this.Toys = toys;
            this.Trees = trees;
            CurrentPos = new Coord(0, 0);
            TraverseList.Entries.Add(new TraversalEntry(new Tree(0, 0), 1));
            LongestLength = 0.0;
        }

        // Returns null while not solved, answer when solved
        public double? Solve()
        {
            // Get the next toy in the list
            var currentToy = Toys[CurrentToyIndex];
            var nextToy = currentToy;
            Utils.Print($"Skopy is at {CurrentPos.X}, {CurrentPos.Y}");
            Utils.Print($"Anchored at {TraverseList.GetCurrentTree().Coord.X}, {TraverseList.GetCurrentTree().Coord.Y}");
            Utils.Print($"Next toy at {nextToy.Coord.X}, {nextToy.Coord.Y}");
            Utils.Print($"Traverse list is {TraverseList}");
            //Console.ReadKey();

            // Find all trees that are possible hits
            // using the triangle formed by Skopy, the currently attached tree and the next toy
            var possibleTrees = Trees.GetInTriangle(
                CurrentPos, 
                TraverseList.GetCurrentTree().Coord, 
                nextToy.Coord,
                null);
            Utils.Print($"Possible trees: {possibleTrees.Count}");

            while (possibleTrees.Count > 0)
            {
                // Get our second to last and last attached trees (if available)
                var secondLastTree = TraverseList.Entries.First().Tree.Coord;
                if (TraverseList.Entries.Count > 2)
                {
                    secondLastTree = TraverseList.Entries[^2].Tree.Coord;
                }
                var currentTree = TraverseList.GetCurrentTree().Coord;

                // Loop all the possible tree hits
                var treeDistances = new List<IntersectionStruct>();
                foreach (var possibleTree in possibleTrees)
                {
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
                        new Line(CurrentPos, nextToy.Coord),
                        intersectionLine,
                        out bool _,
                        out bool intersects,
                        out Coord? intersection,
                        out Coord? _,
                        out Coord? _);

                    if (intersects && intersection is not null)
                    {
                        // If the lines intersect, we'll add the intersection data to a list
                        treeDistances.Add(
                            new IntersectionStruct(
                                Utils.GetDistance(intersection, CurrentPos), 
                                possibleTree,
                                intersection));
                    }
                }

                // Make sure we only compute valid intersections
                var validIntersections = treeDistances
                    .Where(t => t.Intersection != CurrentPos)
                    .Count();
                if (validIntersections == 0)
                    break;

                // Get the closest intersection to Skopy
                var closestIntersection = treeDistances
                    .Where(t => t.Intersection != CurrentPos)
                    .MinBy(t => t.Distance);

                // Null reference safe guard
                if (closestIntersection is null)
                    break;

                // Process the attaching/detaching tree in our traversal list
                var lastModified = TraverseList.HandleTreeTraversal(
                    closestIntersection.Tree, 
                    nextToy.Coord);
                
                // Place Skopy at our intersection
                CurrentPos = closestIntersection.Intersection;

                // Get new possible tree hits given Skopys new position,
                // making sure we don't include the newly processed tree in the calcuations
                possibleTrees = Trees.GetInTriangle(
                    CurrentPos,
                    TraverseList.GetCurrentTree().Coord,
                    nextToy.Coord,
                    lastModified);
            }

            // Place Skopy at next toy
            CurrentPos = nextToy.Coord;

            // *chew chew*

            // Keep track of current tree + farthest toy, if that should happen
            // to be longer than the completed "course"
            var distanceFromTree = Utils.GetDistance(TraverseList.GetCurrentTree().Coord, CurrentPos);
            var totalDistance = TraverseList.GetLength() + distanceFromTree;
            Utils.Print($"Current tree/total distance is {distanceFromTree}/{totalDistance}");
            if (totalDistance > LongestLength)
            {
                LongestLength = totalDistance;
                Utils.Print($"New longest length is {LongestLength}");
            }

            if (CurrentToyIndex == Toys.Count - 1)
            {
                // Much sad. No chew. :'(
                Utils.Print($"No more toys");
                Solved = true;
                return LongestLength;
            }
            else
            {
                // MOAR TOYS!!11
                CurrentToyIndex++;
                return null;
            }

        }

    }
}
