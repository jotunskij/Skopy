using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skopy
{
    public static class SkopySolver
    {

        public static double Solve(List<Toy> toys, List<Tree> trees)
        {
            var currentPos = new Coord() { X=0, Y=0 };
            var previousAnchor = new Coord() { X = 0, Y = 0 };
            var previousLength = 0.0;
            return RecursiveSolve(toys, trees, currentPos, previousAnchor, previousLength);
        }

        public static double RecursiveSolve(
            List<Toy> toys,
            List<Tree> trees,
            Coord currentPos, 
            Coord previousAnchor,
            double previousLength)
        {
            // Find closest toy that is not chewed
            var closestToy = toys.GetClosestToCoord(currentPos);
            // We've chewed the last toy. Much sad :'(
            if (closestToy == null)
                return previousLength + Utils.GetDistance(previousAnchor, currentPos);

            // Check triangle between previousAnchor, currentPos and nextToy for anchors
            var newAnchors = trees.GetInTriangle(currentPos, previousAnchor, closestToy.coord);
           
            Coord? nextAnchor = null;
            if (newAnchors.Count > 1)
            {
                nextAnchor = newAnchors.GetClosestToLine(previousAnchor, closestToy.coord).coord;
            }
            else if (newAnchors.Count == 1)
            {
                nextAnchor = newAnchors[0].coord;
            }

            // Mark toy as chewed
            toys.MarkAsChewed(currentPos);

            if (nextAnchor.HasValue)
                // We have a new anchor
                return previousLength + 
                    Utils.GetDistance(currentPos, nextAnchor.Value) +
                    Utils.GetDistance(nextAnchor.Value, closestToy.coord) +
                    RecursiveSolve(toys, trees, closestToy.coord, nextAnchor.Value, previousLength);
            else
                return previousLength +
                    RecursiveSolve(toys, trees, closestToy.coord, previousAnchor, previousLength);
        }

        private static double GetDistance()
        {
            throw new NotImplementedException();
        }
    }
}
