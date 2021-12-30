namespace Skopy
{
    public class SkopySolver
    {
        public List<Toy> toys { get; set; }
        public List<Tree> trees { get; set; }
        public Coord currentPos { get; set; }
        public double longestLength { get; set; }
        public Toy? currentToy { get; set; }
        public TraverseList traverseList { get; set; } = new TraverseList();

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

        // Returns null while not solved
        public double? Solve()
        {
            if (currentToy is null)
                currentToy = toys.First();
            var nextToy = currentToy;
            Utils.Print($"Skopy is at {currentPos.X}, {currentPos.Y}");
            Utils.Print($"Anchored at {traverseList.GetCurrentTree().Coord.X}, {traverseList.GetCurrentTree().Coord.Y}");
            Utils.Print($"Next toy at {nextToy.Coord.X}, {nextToy.Coord.Y}");
            Utils.Print($"Traverse list is {traverseList.ToString()}");
            //Console.ReadKey();

            var possibleTrees = trees.GetInTriangle(
                currentPos, 
                traverseList.GetCurrentTree().Coord, 
                nextToy.Coord,
                null);
            Utils.Print($"Possible trees: {possibleTrees.Count}");

            while (possibleTrees.Count > 0)
            {
                var secondLastTree = traverseList.entries.First().tree.Coord;
                if (traverseList.entries.Count() > 2)
                {
                    secondLastTree = traverseList.entries[traverseList.entries.Count() - 2].tree.Coord;
                }
                var currentTree = traverseList.GetCurrentTree().Coord;
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
                                Utils.GetDistance(intersection, currentPos), 
                                possibleTree,
                                intersection));
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
                var lastRemoval = traverseList.HandleTreeTraversal(
                    closestIntersection.Tree, 
                    nextToy.Coord);
                currentPos = closestIntersection.Intersection;

                possibleTrees = trees.GetInTriangle(
                    currentPos,
                    traverseList.GetCurrentTree().Coord,
                    nextToy.Coord,
                    lastRemoval);
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

            if (currentToy.Coord == toys.Last().Coord)
            {
                // Much sad. No chew. :'(
                Utils.Print($"No more toys");
                return longestLength;
            }
            else
            {
                return null;
            }

        }

    }
}
