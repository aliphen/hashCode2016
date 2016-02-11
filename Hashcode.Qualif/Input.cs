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

        public bool HasItem(int productId)
        {
            return Stock[productId] > 0;
        }

        public bool CanFullfillOrder(Order order)
        {
            if (order.ItemsWanted.Any(productId => !HasItem(productId)))
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

        public Dictionary<int, Order> OrderIdToOrder;
	}
}

