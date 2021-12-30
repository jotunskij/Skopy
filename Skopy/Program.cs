using Skopy;
using System.Diagnostics;

foreach (var file in args)
{
    // Load problem file
    var treesAndToys = ReadProblemFile.ReadFile(file.ToString());
    var skopySolver = new SkopySolver();

    // Solve
    skopySolver.Init(treesAndToys.Item2, treesAndToys.Item1);
    double? length = null;
    while (length is null)
        length = skopySolver.Solve();
    Utils.Print($"Solved length: {length}");

    // Validate test case if available
    if (Utils.IsDebug())
    {
        var answerFile = Path.ChangeExtension(file.ToString(), "ans");
        if (File.Exists(answerFile))
        {
            var answerLines = File.ReadLines(answerFile).ToArray();
            var answer = double.Parse(answerLines[0].Replace(".", ","));
            Utils.Print($"Correct answer: {answer}");
            if(answer != Math.Round(length.Value, 2)) {
                Utils.Print($"Failed testfile: {file.ToString()}");
                Debug.Assert(false);
            }
        }
    }

    Console.WriteLine(Math.Round(length.Value, 2));
}
