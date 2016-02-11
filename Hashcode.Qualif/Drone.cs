using System;

namespace Hashcode.Qualif
{
    public class Drone
    {
        private Input input;

        public int X;
        public int Y;
        public int turn = 0;
        public int id;
        public int payload;

        public Drone(Input input, int id)
        {
            this.input = input;
            this.id = id;
            X = input.WareHouses[0].X;
            Y = input.WareHouses[0].Y;
        }

        public void Load(WareHouse wh, int itemType)
        {
            turn += Helper.Distance(X, Y, wh.X, wh.Y) + 1;
            X = wh.X;
            Y = wh.Y;
            payload += input.ProductTypes[itemType];
        }

        public void Deliver(Order o)
        {
            turn += Helper.Distance(X, Y, o.X, o.Y) + 1;
            X = o.X;
            Y = o.Y;
            //assume we deliver everything
            payload = 0;
        }
    }
}

