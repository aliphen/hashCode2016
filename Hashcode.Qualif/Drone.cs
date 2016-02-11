using System;

namespace Hashcode.Qualif
{
    public class Drone
    {
        public int X;
        public int Y;
        public int turn = 0;

        public Drone(Input input)
        {
            X = input.WareHouses[0].X;
            Y = input.WareHouses[0].Y;
        }

        public void Load(WareHouse wh)
        {
            X = wh.X;
            Y = wh.Y;
        }

        public void Deliver(Order o)
        {
            X = o.X;
            Y = o.Y;
        }
    }
}

