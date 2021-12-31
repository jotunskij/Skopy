namespace Skopy
{
    public static class ReadProblemFile
    {

        public static Tuple<List<Tree>, List<Toy>> ReadFile(string filepath)
        {
            var trees = new List<Tree>();
            var toys = new List<Toy>();

            // Read file & row numbers
            var inputFile = filepath;

            var inputLines = File.ReadLines(inputFile).ToArray();
            var nrs = inputLines[0].Split(" ");
            var nrOfToys = int.Parse(nrs[0]);
            var nrOfTrees = int.Parse(nrs[1]);

            Utils.Print($"Nr of toys/trees: {nrOfToys}/{nrOfTrees}");

            // Read toy & tree coordinates
            for (int t = 1; t <= nrOfToys + nrOfTrees; t++)
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

            return new Tuple<List<Tree>, List<Toy>>(trees, toys);
        }

        public static double ReadAnswerFile(string filepath)
        {
            // Try to read the corresponding answer file to a given .in file
            var answerFile = Path.ChangeExtension(filepath, "ans");
            if (File.Exists(answerFile))
            {
                var answerLines = File.ReadLines(answerFile).ToArray();
                var answer = double.Parse(answerLines[0].Replace(".", ","));
                return answer;
            }
            return -1;
        }

    }
}
