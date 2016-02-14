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
                else //we'll need several drones
                {
                    wh = input.WareHouses[0];
                    var loadedToDeliver = new List<int>();

                        for (int i = 0; i < order.ItemsWanted.Length; i++)
                        {
                            var itemType = order.ItemsWanted[i];
                            if (itemType < 0) //already delivered
                                continue;
                            
                            if (!chosen.CheckLoad(wh, itemType))
                            {
                                //not enough space to carry this item
                                continue;
                            }
                            wh.Stock[itemType]--;
                            chosen.Load(wh, itemType);
                            solution.LoadForDelivery(chosen, wh, order, itemType);
                            loadedToDeliver.Add(i);
                        
                    }
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
                    solution.DoDeliver(chosen, order, false);
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
                    var wh = input.WareHouses[0];
                    var dist = Helper.Distance(wh.X, wh.Y, order.X, order.Y);
                    if (dist < cost)
                    {
                        cost = dist;
                        bestWh = wh;
                    }
                }
                else //we'll need several drones to complete this order
                {
                    var wh = input.WareHouses[0];
                    var dist = Helper.Distance(wh.X, wh.Y, order.X, order.Y);
                    dist = dist * (totalWeight / input.MaxPayload + 1); //apply a malus for estimated nb of drones needed
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
