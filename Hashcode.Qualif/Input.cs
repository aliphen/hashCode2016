using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;

namespace Hashcode.Qualif
{
    public class WareHouse{
        public int X;
        public int Y;
        public int[] Stock;
        public int id;

        public bool HasItem(int productId, int count = 1)
        {
            return productId < 0 || Stock[productId] >= count;
        }

        /// <returns>true if warehouse contains *every* item for given order</returns>
        public bool CanFullfillOrder(Order order)
        {
            if (order.ItemsWanted.ToLookup(i => i, i => 1).Select(g => Tuple.Create(g.Key, g.Sum())).Any(t => !HasItem(t.Item1, t.Item2)))
            {
                return false;
            }
            return true;
        }
    }

    public class Order
    {
        public int X;
        public int Y;
        public int NbItems;
        public int[] ItemsWanted; //types of objects
        public int id;

        /// <summary> latest time of delivery to this location </summary>
        public int DeliveryTime;
    }

    /// <summary>
    /// contains an object representation of the input
    /// </summary>
	public class Input
	{
        public int R;
        public int C;
        public int NbDrones;
        public int NbTurns;
        public int MaxPayload;

        public int[] ProductTypes;

        public int NbWareHouses;

        public WareHouse[] WareHouses;
        public Order[] Orders;
	}
}

