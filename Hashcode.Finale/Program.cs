using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Hashcode.Qualif
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var inputs = new[]
            {
                "../../forever_alone.in",
                "../../constellation.in",
                "../../overlap.in",
                "../../weekend.in",
            };
            var scores = new[] {0, 0, 0, 0};

            //while (true)
            {
                for (int i = 0; i < inputs.Length; i++)
                {
                    var fileName = inputs[i];
                    var input = Parser.Parse(fileName);

                    var sw = Stopwatch.StartNew();
                    var solution = Solver.Solve(input);
                    Console.WriteLine(fileName.Substring(6, 7) + "\t" + solution.Score + "\t" + sw.ElapsedMilliseconds + "ms");

                    //write output file if better than before
                    if (solution.Score > scores[i])
                    {
                        scores[i] = solution.Score;
                        var outputFile = Path.GetFileNameWithoutExtension(fileName) + "-" + solution.Score + ".out";
                        using (var writer = new StreamWriter("../../" + outputFile))
                        {
                            writer.Write(solution);
                        }
                        Console.WriteLine("[dumped]");
                    }
                }
            }
        }
    }
}
