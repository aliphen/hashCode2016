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
                var order = GetBestOrder(input);
                if (order == null)
                {
                    return solution; //no more order to deliver
                }

                var wh = input.WareHouses[0];
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
                for (int i = 0; i < loadedToDeliver.Count; i++)
                {
                    chosen.Deliver(order);
                    Helper.Assert(() => order.ItemsWanted[loadedToDeliver[i]] >= 0);

                    order.TotalWeight -= input.ProductTypes[order.ItemsWanted[loadedToDeliver[i]]];
                    order.ItemsWanted[loadedToDeliver[i]] = -1; //mark as delivered
                    order.NbItemsRemaining--;
                }
                if (order.NbItemsRemaining == 0)
                {
                    order.ItemsWanted = null;
                }
                solution.DoDeliver(chosen, order, order.NbItemsRemaining == 0);
                chosen.Move(wh); //send it back to the warehouse
            }
        }

        private static Order GetBestOrder(Input input)
        {
            int bestCost = Int32.MaxValue;
            Order best = null;
            foreach (var order in input.Orders)
            {
                if(order.ItemsWanted == null)
                    continue; //already delivered

                var totalWeight = order.TotalWeight;
                var wh = input.WareHouses[0];
                var cost = Helper.Distance(wh.X, wh.Y, order.X, order.Y);

                if (totalWeight >= input.MaxPayload)
                {
                    cost *= (totalWeight/input.MaxPayload + 1); //apply a malus for estimated nb of drones needed
                }
                //cost += (order.NbItemsRemaining*2); //time to pick and deliver everything

                if (cost < bestCost)
                {
                    bestCost = cost;
                    best = order;
                }
            }
            return best;
        }
    }
}
