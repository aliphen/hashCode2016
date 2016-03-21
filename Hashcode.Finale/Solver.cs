using System;
using System.Collections.Generic;

namespace Hashcode.Qualif
{
    internal class Solver
    {
        public static Solution Solve(Input input)
        {
            var solution = new Solution(input);

            var id = 0;
            var satellite = input.Satellites[id];

            var takenPicture = new List<Snapshot>();

            for (int i = 0; i < input.NbTurns; i++)
            {
                foreach (var pictCollection in input.Collections)
                {
                    foreach (var pict in pictCollection.Locations)
                    {
                        
                    }
                }
            }

            return solution;
        }
    }
}