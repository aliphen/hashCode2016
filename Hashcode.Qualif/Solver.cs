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
            solution.Builder.AppendLine();

            var drones = new Drone[input.NbDrones];
            for(int d = 0; d < input.NbDrones; d++)
            {
                drones[d] = new Drone(input, d);
            }

            for(int o = 0; o < input.Orders.Length; o++)
            {
                var order = input.Orders[o];
                for (int i = 0; i < order.ItemsWanted.Length; i++)
                {                    
                    //chooseDrone furthest in the past
                    Drone chosen = drones[0];
                    for (int d = 1; d < drones.Length; d++) {
                        if (drones[d].turn < chosen.turn)
                            chosen = drones[d];
                    }
                    if(chosen.turn > input.NbTurns)
                    {
                        //can't do shit anymore
                        goto end;
                    }

                    var itemType = order.ItemsWanted[i];

                    //find warehouse with item in stock
                    int w;
                    WareHouse wh = null;
                    for (w = 0; w < input.NbWareHouses; w++) {
                        wh = input.WareHouses[w];
                        if(wh.Stock[itemType] > 0)
                        {
                            wh.Stock[itemType]--;
                            break;
                        }
                    }

                    var load = String.Format("{0} L {1} {2} {3}", chosen.id, w, itemType, 1);
                    chosen.Load(wh, itemType);
                    var deli = String.Format("{0} D {1} {2} {3}", chosen.id, o, itemType, 1);
                    chosen.Deliver(order);

                    if(chosen.turn <= input.NbTurns)
                    {
                        //validate move
                        solution.Builder.AppendLine(load);
                        solution.Builder.AppendLine(deli);
                        nbCommands += 2;
                    }
                }
            }

          end:
            solution.Builder.Insert(0, nbCommands);
            return solution;
        }
	}
}
