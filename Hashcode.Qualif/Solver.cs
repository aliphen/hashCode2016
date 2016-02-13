using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Hashcode.Qualif
{
    public class Solver
    {
        public static Solution Solve(Input input)
        {
            var solution = new Solution(input);

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
                        return solution;
                    }

                    var sbDeli = new StringBuilder();
                    var nbDeli = 0;
                    while (i < order.ItemsWanted.Length)
                    {
                        var itemType = order.ItemsWanted[i];

                        //find warehouse with item in stock
                        int minDist = Int32.MaxValue;
                        WareHouse wh = null;
                        for (int w = 0; w < input.NbWareHouses; w++)
                        {
                            var current = input.WareHouses[w];
                            if (current.Stock[itemType] > 0)
                            {
                                if(Helper.Distance(current.X, current.Y, order.X, order.Y) < minDist)
                                {
                                    minDist = Helper.Distance(current.X, current.Y, order.X, order.Y);
                                    wh = current;
                                }
                            }
                        }

                        if (!chosen.CheckLoad(wh, itemType))
                        {
                            //drone passed end of turns or is full
                            i--; //treat object again
                            break;
                        }
                        wh.Stock[itemType]--;
                        chosen.Load(wh, itemType);
                        solution.LoadForDelivery(chosen, wh, order, itemType);
                        nbDeli++;
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
                        solution.DoDeliver(chosen, o: order, orderComplete: i == order.ItemsWanted.Length);
                    }
                }
            }
            return solution; //we only end up here is all orders are completed
        }
	}

    

}
