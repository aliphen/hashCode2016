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

            Solver.Solve(input);
        }
    }
}
