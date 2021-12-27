using Microsoft.Extensions.Logging;
using Skopy;
using System.Diagnostics;

var logger = new LoggerFactory().CreateLogger("Skopy");
var trees = new List<Tree>();
var toys = new List<Toy>();

// Read file & row numbers
var inputFile = args[0].ToString();
var inputLines = File.ReadLines(inputFile).ToArray();
var nrs = inputLines[0].Split(" ");
var nrOfToys = int.Parse(nrs[0]);
var nrOfTrees = int.Parse(nrs[1]);

logger.LogInformation($"Nr of toys/trees: {nrOfToys}/{nrOfTrees}");

// Read toy & tree coordinates
for(int t = 1; t <= nrOfToys; t++)
{
    var coords = inputLines[t].Split(" ");
    if (t <= nrOfToys)
        toys.Add(new Toy(int.Parse(coords[0]), int.Parse(coords[1])));
    else
        trees.Add(new Tree(int.Parse(coords[0]), int.Parse(coords[1])));
}
logger.LogInformation($"trees.Count: {trees.Count}, toys.Count: {toys.Count}");

// Pre-flight checks
Debug.Assert(nrOfToys == toys.Count);
Debug.Assert(nrOfTrees == trees.Count);

// Solve
var length = SkopySolver.Solve(toys, trees);
logger.LogInformation($"Solved length: {length}");

// Validate test case if available
var answerFile = Path.ChangeExtension(inputFile, "ans");
if (File.Exists(answerFile))
{
    var answerLines = File.ReadLines(answerFile).ToArray();
    var answer = double.Parse(answerLines[0].Replace(".", ","));
    Debug.Assert(Math.Round(answer, 2) == Math.Round(length, 2));
}

// Write solution file

