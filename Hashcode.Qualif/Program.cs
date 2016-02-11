using System;
using System.Diagnostics;
using System.IO;

namespace Hashcode.Qualif
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            const string fileName = "../../busy_day.in";
//            const string fileName = "../../mother_of_all_warehouses.in";
//            const string fileName = "../../redundancy.in";
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
