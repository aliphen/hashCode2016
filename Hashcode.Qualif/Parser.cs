using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hashcode.Qualif
{
    public static class Parser
    {
        public static Input Parse(string fileName)
        {
            var input = new Input();
            using(var reader = new StreamReader(fileName))
            {
                var inputParams = reader.ReadLine().Split(' ').Select(Int32.Parse).ToArray();
                input.R = inputParams[0];
                input.C = inputParams[1];
                input.NbDrones = inputParams[2];
                input.NbTurns = inputParams[3];
                input.MaxPayload = inputParams[4];

                reader.ReadLine(); //nb product types, osef
                input.ProductTypes = reader.ReadLine().Split(' ').Select(Int32.Parse).ToArray();

                input.NbWareHouses = Int32.Parse(reader.ReadLine());
                input.WareHouses = new WareHouse[input.NbWareHouses];

                for(int i = 0; i < input.NbWareHouses; i++)
                {
                    var coords = reader.ReadLine().Split(' ').Select(Int32.Parse).ToArray();
                    input.WareHouses[i] = new WareHouse{ 
                        X = coords[0], 
                        Y = coords[1],
                        id = i,
                        Stock = reader.ReadLine().Split(' ').Select(Int32.Parse).ToArray(),
                    };
                }

                var nbOrders = Int32.Parse(reader.ReadLine());
                input.Orders = new Order[nbOrders];
                input.OrderIdToOrder = new Dictionary<int, Order>(nbOrders);
                for(int i = 0; i < nbOrders; i++)
                {
                    var coords = reader.ReadLine().Split(' ').Select(Int32.Parse).ToArray();
                    input.Orders[i] = new Order {
                        X = coords[0], 
                        Y = coords[1],
                        id = i,
                        NbItems = Int32.Parse(reader.ReadLine()),
                        ItemsWanted = reader.ReadLine().Split(' ').Select(Int32.Parse).ToArray(),
                    };
                    Array.Sort(input.Orders[i].ItemsWanted);
                    input.OrderIdToOrder.Add(i, input.Orders[i]);
                }
            }
            return input;
        }
    }
}

