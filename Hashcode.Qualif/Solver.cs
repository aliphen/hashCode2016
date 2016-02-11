using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Hashcode.Qualif
{
	public class Solver
	{
        public static Solution Solve(Input input)
        {
            int nbCommands = 0;
            var solution = new Solution();

            Helper.Assert(() => true, "optional message");

            for(int o = 0; o < input.Orders.Length; o++)
            {
                var order = input.Orders[o];
                for (int i = 0; i < order.ItemsWanted.Length; i++)
                {
                    //chooseDrone
                    var itemType = order.ItemsWanted[i];
                    solution.Builder.AppendLine("{0} L {1} {2} {3}", droneId, warehouseId, itemType, 1);
                    nbCommands++;
                    solution.Builder.AppendLine("{0} D {1} {2} {3}", droneId, o, itemType, 1);
                    nbCommands++;
                    //update drone status
                }
            }

            solution.Builder.Insert(0, nbCommands);
            return solution;
        }
	}
}
