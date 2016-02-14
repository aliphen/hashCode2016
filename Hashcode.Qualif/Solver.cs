using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace Hashcode.Qualif
{
    public class Solver
    {
        private static double _multidroneMalus;

        public static Solution Solve(Input input)
        {
            _multidroneMalus = 1.0 + Helper.Rand.NextDouble(); //choose a random malus between 1 and 2 for this run

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
                        
                        wh.Stock[itemType]--;
                        Helper.Assert(() => wh.Stock[itemType] >= 0);
                        chosen.Load(wh, itemType);
                        solution.LoadForDelivery(chosen, wh, order, itemType);
                    }
                    //everything is loaded
                    for (int dd = 0; dd < order.NbItemsRemaining; dd++)
                    {
                        chosen.Deliver(order);
                    }
                    solution.DoDeliver(chosen, order, orderComplete: true);
                    order.ItemsWanted = null;
                }
                else //we'll have to go to several warehouses to load stuff OR we'll need several drones
                {
                    var loadedToDeliver = new List<int>();
                    var itemsToDeliver = (int[]) order.ItemsWanted.Clone();
                    while(itemsToDeliver.Any(it => it >= 0))
                    {
                        //best warehouse with item in stock
                        double bestScore = Double.PositiveInfinity;
                        WareHouse bestwh = null;
                        List<int> availableItems = null;
                        for (int w = 0; w < input.NbWareHouses; w++)
                        {
                            var currentwh = input.WareHouses[w];
                            var items = currentwh.GetFulfillable(itemsToDeliver);
                            if (items.Count > 0)
                            {
                                var score = (double) Helper.Distance(chosen.X, chosen.Y, currentwh.X, currentwh.Y)/items.Count; //time per item
                                if (score < bestScore)
                                {
                                    bestScore = score;
                                    bestwh = currentwh;
                                    availableItems = items;
                                }
                            }
                        }
                        wh = bestwh;

                        for (int i = 0; i < availableItems.Count; i++)
                        {
                            var itemType = order.ItemsWanted[availableItems[i]];
                            if (itemType < 0) //already delivered
                                continue;
                            
                            if (!chosen.CheckLoad(wh, itemType))
                            {
                                //drone passed end of turns or is full
                                goto deliver; //maybe we could stash one or two more small items, but whatever
                            }
                            wh.Stock[itemType]--;
                            itemsToDeliver[availableItems[i]] = -1;
                            chosen.Load(wh, itemType);
                            solution.LoadForDelivery(chosen, wh, order, itemType);
                            loadedToDeliver.Add(availableItems[i]);
                        }
                    }
                deliver:
                    for (int dd = 0; dd < loadedToDeliver.Count; dd++)
                    {
                        chosen.Deliver(order);
                    }
                    for (int i = 0; i < loadedToDeliver.Count; i++)
                    {
                        Helper.Assert(() => order.ItemsWanted[loadedToDeliver[i]] >= 0);

                        order.TotalWeight -= input.ProductTypes[order.ItemsWanted[loadedToDeliver[i]]];
                        order.ItemsWanted[loadedToDeliver[i]] = -1; //mark as delivered
                        order.NbItemsRemaining--;
                    }
                    var orderComplete = order.NbItemsRemaining == 0;
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
                var totalWeight = order.TotalWeight;
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
                        var wh = input.WareHouses[Helper.Rand.Next(input.NbWareHouses)];
                        dist += Helper.Distance(d.X, d.Y, wh.X, wh.Y) * 3;
                        dist += Helper.Distance(order.X, order.Y, wh.X, wh.Y);
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
                    var wh = input.WareHouses[Helper.Rand.Next(input.NbWareHouses)];
                    dist += Helper.Distance(d.X, d.Y, wh.X, wh.Y)*3;
                    dist += Helper.Distance(order.X, order.Y, wh.X, wh.Y);
                    dist = (int)(_multidroneMalus * dist * totalWeight/input.MaxPayload); //apply a malus for estimated nb of drones needed
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
