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

        public bool CheckLoad(WareHouse wh, int itemType)
        {
            var tmpturn = turn + Helper.Distance(X, Y, wh.X, wh.Y) + 1;
            if (tmpturn < input.NbTurns)
            {
                turn++; //send it to the future
                return false;
            }

            var tmppayload = payload + input.ProductTypes[itemType];
            return tmppayload < input.MaxPayload;
        }

        public bool Load(WareHouse wh, int itemType)
        {
            turn += Helper.Distance(X, Y, wh.X, wh.Y) + 1;
            X = wh.X;
            Y = wh.Y;
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

