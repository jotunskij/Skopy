﻿using Skopy;
using System.Diagnostics;

var trees = new List<Tree>();
var toys = new List<Toy>();

// Read file & row numbers
var inputFile = args[0].ToString();
var inputLines = File.ReadLines(inputFile).ToArray();
var nrs = inputLines[0].Split(" ");
var nrOfToys = int.Parse(nrs[0]);
var nrOfTrees = int.Parse(nrs[1]);

Utils.Print($"Nr of toys/trees: {nrOfToys}/{nrOfTrees}");

// Read toy & tree coordinates
for(int t = 1; t <= nrOfToys + nrOfTrees; t++)
{
    var coords = inputLines[t].Split(" ");
    if (t <= nrOfToys)
    {
        toys.Add(new Toy(int.Parse(coords[0]), int.Parse(coords[1])));
        Utils.Print($"Added toy at: {coords[0]}, {coords[1]}");
    }
    else
    {
        trees.Add(new Tree(int.Parse(coords[0]), int.Parse(coords[1])));
        Utils.Print($"Added tree at: {coords[0]}, {coords[1]}");
    }
}
Utils.Print($"trees.Count: {trees.Count}, toys.Count: {toys.Count}");
Utils.Print("--- SETUP DONE ---" + Environment.NewLine);

// Pre-flight checks
Debug.Assert(nrOfToys == toys.Count);
Debug.Assert(nrOfTrees == trees.Count);

// Solve
var length = SkopySolver.Solve(toys, trees);
Utils.Print($"Solved length: {length}");

// Validate test case if available
var answerFile = Path.ChangeExtension(inputFile, "ans");
if (File.Exists(answerFile))
{
    var answerLines = File.ReadLines(answerFile).ToArray();
    var answer = double.Parse(answerLines[0].Replace(".", ","));
    //Debug.Assert(Math.Round(answer, 2) == Math.Round(length, 2));
    Utils.Print($"My answer: {length}, correct answer: {answer}");
}

// Write solution file
