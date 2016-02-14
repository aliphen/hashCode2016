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
            var fileName = "../../mother_of_all_warehouses.in";
            var score = 74500;

            //while (true)
            {
                Console.WriteLine("Solving for input file {0}", fileName);
                var input = Parser.Parse(fileName);

                var sw = Stopwatch.StartNew();
                var solution = Solver.Solve(input);
                Console.WriteLine(solution.Score + " - " + sw.ElapsedMilliseconds + "ms");

                //write output file if better than before
                if (solution.Score > score)
                {
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
