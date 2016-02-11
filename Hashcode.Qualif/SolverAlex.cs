/*
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Hashcode.Qualif
{
	public class SolverAlex
	{
        public static Solution Solve(Input input)
        {
            int nbCommands = 0;
            var solution = new Solution();
            solution.Builder.AppendLine();

            var drones = new Drone[input.NbDrones];
            for (int d = 0; d < input.NbDrones; d++)
            {
                drones[d] = new Drone(input, d);
            }


           /* foreach (var drone in drones)
            {
                var orderedScoresForDrone = ScoreAllOrders(input, drone);
                // send drone for best delivery based on that

            }#1#
            while (true)
            {
                //chooseDrone furthest in the past
                Drone chosen = drones[0];
                for (int d = 1; d < drones.Length; d++)
                {
                    if (drones[d].turn < chosen.turn)
                        chosen = drones[d];
                }

                // find best order for this drone
                var orderedScoresForDrone = ScoreAllOrders(input, chosen);
                var bestOrder = orderedScoresForDrone[orderedScoresForDrone.Keys.ToList()[0]];

                if (chosen.turn > input.NbTurns)
                {
                    // TODO mark as inactive, continue and check if all inactive
                    //can't do shit anymore
                    goto end;
                }

                var sbDeli = new StringBuilder();
                var nbDeli = 0;
                for (int i = 0; i < bestOrder.ItemsWanted.Length; i++)
                {
                    var itemType = bestOrder.ItemsWanted[i];

                    //find warehouse with item in stock
                    int w;
                    WareHouse wh = null;
                    for (w = 0; w < input.NbWareHouses; w++)
                    {
                        wh = input.WareHouses[w];
                        if (wh.Stock[itemType] > 0)
                        {
                            break;
                        }
                    }

                    var load = String.Format("{0} L {1} {2} {3}", chosen.id, w, itemType, 1);
                    if (!chosen.CheckLoad(wh, itemType))
                    {
                        //drone passed end of turns
                        i--; //treat object again
                        break;
                    }
                    wh.Stock[itemType]--;
                    chosen.Load(wh, itemType);
                    nbCommands++;
                    solution.Builder.AppendLine(load);

                    var deli = String.Format("{0} D {1} {2} {3}", chosen.id, o, itemType, 1);
                    nbDeli++;
                    sbDeli.AppendLine(deli);
                    i++;
                }

                bool enoughTime = true;
                for (int dd = 0; dd < nbDeli; dd++)
                {
                    if (!chosen.Deliver(order))
                    {
                        //drone passed end of turns
                        enoughTime = false;
                    }
                }
                if (enoughTime)
                {
                    solution.Builder.Append(sbDeli.ToString());
                    nbCommands += nbDeli;
                }


            }

        end:
            solution.Builder.Insert(0, nbCommands);
            return solution;
        }

        public static KeyValuePair<Order, int> GetNextDeliveryToDo(Dictionary<int, Order> orderedOrders) // order to itemid
        {
            var orderedListOforders = orderedOrders.Keys.ToList();

        }


        public static Dictionary<int, Order> ScoreAllOrders(Input input, Drone drone)
        {
            var orderScores = new double[input.Orders.Length];
            var i = 0;
            foreach (var order in input.Orders)
            {
                orderScores[i] = ScoreOrder(order, input, drone);
                i++;
            }
            // order the dico of orders according to that score
            return input.OrderIdToOrder.OrderBy(d => orderScores[d.Key]).ToDictionary(pair => pair.Key, pair => pair.Value);
        }



        private const double FactorAppliedToLoad = 0.2;
        // Note : impossible orders are worth int.MaxValue
        public static double ScoreOrder(Order order, Input input, Drone drone)
        {
            double score = 0;
            foreach (var item in order.ItemsWanted)
            {
                if (item == -1) // already delivered
                    continue;
                
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
*/
