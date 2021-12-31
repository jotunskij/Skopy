using Skopy;

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

    // Validate test case if available
    var answer = ReadProblemFile.ReadAnswerFile(file);
    if (answer != -1)
    {
        Utils.Print($"Correct answer: {answer}");
    }

    // Print final answer
    Console.WriteLine(Math.Round(length.Value, 2));
}
