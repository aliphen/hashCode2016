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
    }
}

