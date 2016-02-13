using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Hashcode.Qualif
{
    public class Solution
    {
        private readonly Input _input;
        private readonly StringBuilder[] _deliveryOrders;
        private readonly StringBuilder _validatedCommands = new StringBuilder();

        public int Score { get; private set; }

        public Solution(Input input)
        {
            _input = input;
            _deliveryOrders = new StringBuilder[input.NbDrones];
            for (int i = 0; i < input.NbDrones; i++)
                _deliveryOrders[i] = new StringBuilder();
        }

        /// <summary>
        /// adds the load command immediately to the commands queue,
        /// and stores the delivery command for later
        /// </summary>
        public void LoadForDelivery(Drone d, WareHouse wh, Order o, int item)
        {
            var load = String.Format("{0} L {1} {2} {3}", d.id, wh.id, item, 1);
            var deli = String.Format("{0} D {1} {2} {3}", d.id, o.id, item, 1);
            _validatedCommands.AppendLine(load);
            _deliveryOrders[d.id].AppendLine(deli);
        }

        /// <summary>
        /// executes the deliver commands stashed for this drone
        /// </summary>
        public void DoDeliver(Drone d, Order o, bool orderComplete)
        {
            if (d.turn > o.DeliveryTime)
                o.DeliveryTime = d.turn;

            _validatedCommands.Append(_deliveryOrders[d.id].ToString());
            _deliveryOrders[d.id].Clear();
            if (orderComplete)
            {
                Score += GetScoreForDeliveryOn(o.DeliveryTime-1);
                
                //assert we have as many delivery commands for this order as items wanted
                Helper.Assert(() => _validatedCommands.ToString().Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries).Count(l =>
                {
                    //count number of deliveries for given order
                    var linecontent = l.Split(' ');
                    return linecontent[1] == "D" && Int32.Parse(linecontent[2]) == o.id;
                }) == o.NbItems);
            }
        }

        /// <summary>
        /// returns the solution ready to be written to the output file
        /// </summary>
        public override string ToString()
        {
            var commands = _validatedCommands.ToString();
            var nbCommands = commands.Count(c => c == '\n');
            return nbCommands + Environment.NewLine + commands;
        }

        private int GetScoreForDeliveryOn(int turn)
        {
            var totalTurns = _input.NbTurns;
            return (int)Math.Ceiling(100*(double)(totalTurns - turn)/totalTurns);
        }

        public class Tests
        {
            [TestCase(15, ExpectedResult = 91, Description = "example from the pb statement")]
            [TestCase(159, ExpectedResult = 1, Description = "example from the pb statement")]
            [TestCase(80, ExpectedResult = 50, Description = "division without remainder")]
            [TestCase(0, ExpectedResult = 100, Description = "max score on first turn")]
            public int TestScoreForTurn(int turn)
            {
                var s = new Solution(new Input {NbTurns = 160});
                return s.GetScoreForDeliveryOn(turn);
            }
        }
    }
}
