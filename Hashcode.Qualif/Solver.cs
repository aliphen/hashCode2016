using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace Hashcode.Qualif
{
    public class Solver
    {
        private static int newLineLenght = Environment.NewLine.Length;

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


            Array.Sort(input.Orders, (order, order1) =>

            {
                var totalWeigth1 = order.ItemsWanted.Sum(item => input.ProductTypes[item]);
                var totalWeigth2 = order1.ItemsWanted.Sum(item => input.ProductTypes[item]);

                if (totalWeigth1 == totalWeigth2)
                {
                    return order.NbItems.CompareTo(order1.NbItems);
                }
                else
                {
                    return totalWeigth1.CompareTo(totalWeigth2);
                }

            });

            for (int o = 0; o < input.Orders.Length; o++)
            {
                var order = input.Orders[o];
                for (int i = 0; i < order.ItemsWanted.Length; i++)
                {
                    //chooseDrone furthest in the past
                    Drone chosen = drones[0];
                    for (int d = 1; d < drones.Length; d++)
                    {
                        if (drones[d].turn < chosen.turn)
                            chosen = drones[d];
                    }
                    if (chosen.turn > input.NbTurns)
                    {
                        //can't do shit anymore
                        goto end;
                    }

                    var sbDeli = new StringBuilder();
                    var nbDeli = 0;
                    int previtem = -1;
                    int prevwh = -1;
                    var currentCount = 1;
                    while (i < order.ItemsWanted.Length)
                    {
                        var itemType = order.ItemsWanted[i];

                        //find warehouse with item in stock
                        int w;
                        WareHouse wh = null;
                        var whWithStock = new List<WareHouse>();
                        for (w = 0; w < input.NbWareHouses; w++)
                        {
                            wh = input.WareHouses[w];
                            if (wh.Stock[itemType] > 0)
                            {
                                whWithStock.Add(wh);
                            }
                        }


                        int minDist = Int32.MaxValue;
                        WareHouse nearestWh = null;
                        foreach (var ware in whWithStock)
                        {
                            if (Helper.Distance(ware.X, ware.Y, order.X, order.Y) < minDist)
                            {
                                minDist = Helper.Distance(ware.X, ware.Y, order.X, order.Y);
                                nearestWh = ware;
                             }
                        }
       
                        wh = nearestWh;
                        w = nearestWh.id;

                        var load = String.Format("{0} L {1} {2} {3}", chosen.id, w, itemType, 1);
                        if (!chosen.CheckLoad(wh, itemType))
                        {
                            //drone passed end of turns
                            i--; //treat object again
                            break;
                        }
                        wh.Stock[itemType]--;
                        chosen.Load(wh, itemType);

                        var deli = String.Format("{0} D {1} {2} {3}", chosen.id, order.id, itemType, 1);

                        if (itemType != previtem || prevwh != w)
                        {
                            currentCount = 1;
                            previtem = itemType;
                            prevwh = w;

                            solution.Builder.AppendLine(load);
                            nbCommands++;

                            nbDeli++;
                            sbDeli.AppendLine(deli);
                        }
                        else
                        {
                            chosen.turn--; //loading is free because it's in the same instruction
                            currentCount++;
                            solution.Builder.Remove(solution.Builder.Length - 1 - newLineLenght, newLineLenght + 1);//remove last char AND line break
                            solution.Builder.AppendLine(currentCount.ToString());

                            sbDeli.Remove(sbDeli.Length - 1 - newLineLenght, newLineLenght + 1);//remove last char AND line break
                            sbDeli.AppendLine(currentCount.ToString());
                        }

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
            }

        end:
            solution.Builder.Insert(0, nbCommands);
            return solution;
        }


        private const double FactorAppliedToLoad = 0;
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
