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

        public void Move(WareHouse wh)
        {
            turn += Helper.Distance(X, Y, wh.X, wh.Y);
            X = wh.X;
            Y = wh.Y;
        }

        public bool CheckLoad(WareHouse wh, int itemType)
        {
            var tmppayload = payload + input.ProductTypes[itemType];
            return tmppayload < input.MaxPayload;
        }

        public bool Load(WareHouse wh, int itemType)
        {
            Move(wh);
            turn++;
            payload += input.ProductTypes[itemType];

            return turn <= input.NbTurns;
        }

        public bool Deliver(Order o)
        {
            turn += Helper.Distance(X, Y, o.X, o.Y) + 1;
            X = o.X;
            Y = o.Y;
            //assume we deliver everything
            payload = 0;

            return turn <= input.NbTurns;
        }
    }
}

