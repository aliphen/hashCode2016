using System;
using System.Collections;
using System.Linq;
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

            var iteration = 0; //to ease debuging a particular step
            while (true)
            {
                iteration++;

                //chooseDrone furthest in the past
                Drone chosen = drones[0];
                for (int d = 1; d < drones.Length; d++)
                {
                    if (drones[d].turn < chosen.turn)
                        chosen = drones[d];
                }
                if (chosen.turn > input.NbTurns)
                {
                    Console.WriteLine("end of times reached");
                    return solution; //can't do shit anymore
                }

                //choose order
                WareHouse wh;
                var order = GetBestOrder(chosen, input, out wh);
                if (order == null)
                {
                    Console.WriteLine("everything is delivered !");
                    return solution; //no more order to deliver
                }

                if (wh != null)
                {
                    //go to warehouse and load everything
                    for (int i = 0; i < order.ItemsWanted.Length; i++)
                    {
                        var itemType = order.ItemsWanted[i];
                        if(itemType < 0)
                            continue;

                        if (!chosen.CheckLoad(wh, itemType))
                        {
                            //drone passed end of turns :(
                            break;
                        }
                        wh.Stock[itemType]--;
                        Helper.Assert(() => wh.Stock[itemType] >= 0);
                        chosen.Load(wh, itemType);
                        solution.LoadForDelivery(chosen, wh, order, itemType);
                    }
                    //everything is loaded
                    bool enoughTime = true;
                    for (int dd = 0; dd < order.NbItems; dd++)
                    {
                        if (!chosen.Deliver(order))
                        {
                            //drone passed end of turns
                            enoughTime = false;
                        }
                    }
                    if (enoughTime)
                    {
                        solution.DoDeliver(chosen, order, orderComplete: true);
                        order.ItemsWanted = null;
                    }
                }
                else //we'll have to go to several warehouses to load stuff
                {
                    var nbDeli = 0;
                    for (int i = 0; i < order.ItemsWanted.Length; i++)
                    {
                        var itemType = order.ItemsWanted[i];
                        if (itemType < 0) //already delivered
                            continue;

                        //find warehouse with item in stock
                        int minDist = Int32.MaxValue;
                        for (int w = 0; w < input.NbWareHouses; w++)
                        {
                            var currentwh = input.WareHouses[w];
                            if (currentwh.Stock[itemType] > 0)
                            {
                                var dist = Helper.Distance(chosen.X, chosen.Y, currentwh.X, currentwh.Y) + Helper.Distance(currentwh.X, currentwh.Y, order.X, order.Y);
                                if (dist < minDist)
                                {
                                    minDist = dist;
                                    wh = currentwh;
                                }
                            }
                        }

                        if (!chosen.CheckLoad(wh, itemType))
                        {
                            //drone passed end of turns or is full
                            break;
                        }
                        wh.Stock[itemType]--;
                        chosen.Load(wh, itemType);
                        solution.LoadForDelivery(chosen, wh, order, itemType);
                        nbDeli++;
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
                        int i;
                        for (i = 0; nbDeli > 0; i++)
                        {
                            if (order.ItemsWanted[i] >= 0)
                            {
                                order.ItemsWanted[i] = -1; //mark as delivered
                                nbDeli--;
                            }
                        }
                        var orderComplete = i == order.ItemsWanted.Length;
                        if (orderComplete)
                        {
                            Helper.Assert(() => order.ItemsWanted.All(it => it < 0));
                            order.ItemsWanted = null;
                        }
                        else
                        {
                            Helper.Assert(() => order.ItemsWanted.Any(it => it >= 0));
                        }
                        solution.DoDeliver(chosen, order, orderComplete);
                    }
                }
            }
        }

        private static Order GetBestOrder(Drone d, Input input, out WareHouse goThere)
        {
            goThere = null;

            int bestCost = Int32.MaxValue;
            Order best = null;
            foreach (var order in input.Orders)
            {
                if(order.ItemsWanted == null)
                    continue; //already delivered

                int cost = Int32.MaxValue;
                WareHouse bestWh = null;
                var totalWeight = order.ItemsWanted.Sum(item => item >= 0 ? input.ProductTypes[item] : 0);
                if (totalWeight < input.MaxPayload) //one drone can take care of this order
                {
                    var eligibleWareHouses = input.WareHouses.Where(wh => wh.CanFullfillOrder(order.ItemsWanted) == order.NbItemsRemaining);
                    if (eligibleWareHouses.Any()) //everything is in the same warehouse
                    {
                        foreach (var wh in eligibleWareHouses)
                        {
                            var dist = Helper.Distance(d.X, d.Y, wh.X, wh.Y) + Helper.Distance(wh.X, wh.Y, order.X, order.Y);
                            if (dist < cost)
                            {
                                cost = dist;
                                bestWh = wh;
                            }
                        }
                    }
                    else //one drone can do everything, but has to go to several warehouses
                    {
                        //simulate going to 3 different wh
                        int dist = 0;
                        var wh1 = input.WareHouses[Helper.Rand.Next(input.NbWareHouses)];
                        dist += Helper.Distance(d.X, d.Y, wh1.X, wh1.Y);
                        var wh2 = input.WareHouses[Helper.Rand.Next(input.NbWareHouses)];
                        dist += Helper.Distance(wh2.X, wh2.Y, wh1.X, wh1.Y);
                        var wh3 = input.WareHouses[Helper.Rand.Next(input.NbWareHouses)];
                        dist += Helper.Distance(wh2.X, wh2.Y, wh3.X, wh3.Y);
                        dist += Helper.Distance(order.X, order.Y, wh3.X, wh3.Y);
                        if (dist < cost)
                        {
                            cost = dist;
                        }
                    }
                }
                else //we'll need several drones to complete this order
                {
                    //simulate going to 3 different wh
                    int dist = 0;
                    var wh1 = input.WareHouses[Helper.Rand.Next(input.NbWareHouses)];
                    dist += Helper.Distance(d.X, d.Y, wh1.X, wh1.Y);
                    var wh2 = input.WareHouses[Helper.Rand.Next(input.NbWareHouses)];
                    dist += Helper.Distance(wh2.X, wh2.Y, wh1.X, wh1.Y);
                    var wh3 = input.WareHouses[Helper.Rand.Next(input.NbWareHouses)];
                    dist += Helper.Distance(wh2.X, wh2.Y, wh3.X, wh3.Y);
                    dist += Helper.Distance(order.X, order.Y, wh3.X, wh3.Y);
                    dist = (int)(dist * (double) totalWeight/input.MaxPayload) * 2; //apply a malus for estimated nb of drones needed
                    if (dist < cost)
                    {
                        cost = dist;
                    }
                }

                if (cost < bestCost)
                {
                    bestCost = cost;
                    best = order;
                    goThere = bestWh;
                }
            }
            return best;
        }
    }
}
