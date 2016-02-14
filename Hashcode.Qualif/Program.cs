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
                "../../busy_day.in",
                //"../../mother_of_all_warehouses.in",
                "../../redundancy.in",
            };
            var scores = new[] {105996/*, 74500*/, 96988};

            while (true)
            {
                for (int i = 0; i < inputs.Length; i++)
                {
                    var fileName = inputs[i];
                    var input = Parser.Parse(fileName);

                    var sw = Stopwatch.StartNew();
                    var solution = Solver.Solve(input);
                    Console.WriteLine(fileName[6] + "\t" + solution.Score + "\t" + sw.ElapsedMilliseconds + "ms");

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
