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

        public int HasItem(int productId, int count = 1)
        {
            if (productId < 0)
                return 0;
            return Math.Min(Stock[productId], count);
        }

        /// <returns> true if warehouse contains *every* item for given order </returns>
        public int CanFullfillOrder(int[] items)
        {
            return items
                .ToLookup(i => i, i => 1)
                .Select(g => Tuple.Create(g.Key, g.Sum()))
                .Sum(t => HasItem(t.Item1, t.Item2));
        }

        /// <summary> set items that are in stock to -1 in given array </summary>
        /// <returns> the indexes of the stuff to load </returns>
        public List<int> GetFulfillable(int[] items)
        {
            var canDo = new List<int>();
            for (int i = 0; i < items.Length; i++)
            {
                if (HasItem(items[i]) > 0)
                    canDo.Add(i);
            }
            return canDo;
        }
    }

    public class Order
    {
        public int X;
        public int Y;
        public int NbItemsRemaining;
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

