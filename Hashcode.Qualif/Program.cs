using System;
using System.Diagnostics;
using System.IO;

namespace Hashcode.Qualif
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var inputs = new string[3]
            {
                "../../busy_day.in",
                "../../mother_of_all_warehouses.in",
                "../../redundancy.in"
            };


            foreach (var fileName in inputs)
            {
                Console.WriteLine("Solving for input file {0}", fileName);
                var input = Parser.Parse(fileName);

                var sw = Stopwatch.StartNew();
                var solution = Solver.Solve(input);
                Console.WriteLine("Done, " + sw.ElapsedMilliseconds + "ms elapsed");

                //write output file
                var outputFile = Path.GetFileNameWithoutExtension(fileName) + "-" + solution.Score + ".out";
                using (var writer = new StreamWriter("../../" + outputFile))
                {
                    writer.Write(solution);
                }
                Console.WriteLine("file dumped : " + outputFile);  
            }
            
        }
    }
}
