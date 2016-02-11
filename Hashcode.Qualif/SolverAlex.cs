using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Hashcode.Qualif
{
	public class SolverAlex
	{
       /* public static Solution Solve(Input input)
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
        }*/

        public static double[] ScoreAllOrder(Input input, Drone drone)
        {
            var orderScores = new double[input.Orders.Length];
            var i = 0;
            foreach (var order in input.Orders)
            {
                orderScores[i] = ScoreOrder(order, input, drone);
                i++;
            }

            // Sort to have the smallest score first
            return orderScores.OrderBy(d => d).ToArray();
        }



        private const double FactorAppliedToLoad = 0.2;
        // Note : impossible orders are worth int.MaxValue
        public static double ScoreOrder(Order order, Input input, Drone drone)
        {
            double score = 0;
            foreach (var item in order.ItemsWanted)
            {
                // find closest warehouse containing the item.
                WareHouse closestWh = null;
                int closestWhDist = int.MaxValue;
                foreach (var wh in input.WareHouses)
                {
                    if (wh.Stock[item] > 0) // has item
                    {
                        var dist = Helper.Distance(wh.X, wh.Y, drone.X, drone.Y);
                        if (dist < closestWhDist) // closer !
                        {
                            closestWh = wh;
                            closestWhDist = dist;
                        }
                    }
                }

                if (closestWh == null)
                {
                    score = int.MaxValue;
                    break;
                }

                // Score is : move to WH, load item, move to order, deliver.
                // Add load with a multiplicator.
                var distanceToWh = Helper.Distance(drone.X, drone.Y, closestWh.X, closestWh.Y) + 1;
                var whToOrder = Helper.Distance(closestWh.X, closestWh.Y, order.X, order.Y) + 1;

                score += distanceToWh + whToOrder + FactorAppliedToLoad * input.ProductTypes[item];
            }
            return score;
        }
	}
}
