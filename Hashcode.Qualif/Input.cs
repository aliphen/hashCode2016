using System;
using System.Collections.Generic;
using System.IO;

namespace Hashcode.Qualif
{
    public class WareHouse{
        public int X;
        public int Y;
        public int[] Stock;
    }

    public class Order
    {
        public int X;
        public int Y;
        public int NbItems;
        /// <summary>
        /// index is type, content is quantity
        /// </summary>
        public int[] ItemsWanted;
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

        public WareHouse[] WareHouseLocation;
        public Order[] Orders;
	}
}

