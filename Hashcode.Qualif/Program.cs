using System;
using System.IO;

namespace Hashcode.Qualif
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            const string fileName = "../../file.in"; //put input file where sources are
            var input = Parser.Parse(fileName);
            
            var solution = Solver.Solve(input);

            //write output file
            using (var writer = new StreamWriter("../../" + solution.Score + ".out"))
            {
                writer.Write(solution);
            }
        }
    }
}
