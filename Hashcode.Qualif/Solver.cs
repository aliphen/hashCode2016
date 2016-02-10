using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Hashcode.Qualif
{
	public class Solver
	{
        public static void Solve(Input input)
        {
            var sb = new StringBuilder();
            int score = 0;

            //TODO : do stuff
            Debug.Assert(true, "optional message");

            //write output file
            using(var writer = new StreamWriter("../../" + score + ".out"))
            {
                writer.Write(sb);
            }
        }
	}
}
