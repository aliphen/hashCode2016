using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hashcode.Qualif
{
    public class Solution
    {
        private readonly Input _input;
        private readonly StringBuilder[] _deliveryOrders;

        public int Score = 0;
        public StringBuilder Builder = new StringBuilder();

        public Solution(Input input)
        {
            _input = input;
            _deliveryOrders = new StringBuilder[input.NbDrones];
        }

        /// <summary>
        /// adds the load command immediately to the commands queue,
        /// and stores the delivery command for later
        /// </summary>
        public void LoadFor(Drone d, WareHouse wh, Order o, int item)
        {
            var load = String.Format("{0} L {1} {2} {3}", d.id, wh.id, item, 1);
            var deli = String.Format("{0} D {1} {2} {3}", d.id, o.id, item, 1);
            Builder.AppendLine(load);
            _deliveryOrders[d.id].AppendLine(deli);
        }

        /// <summary>
        /// executes the deliver commands stashed for this drone
        /// </summary>
        public void DoDeliver(Drone d)
        {
            Builder.Append(_deliveryOrders[d.id].ToString());
            _deliveryOrders[d.id].Clear();
        }

        /// <summary>
        /// returns the solution ready to be written to the output file
        /// </summary>
        public override string ToString()
        {
            var commands = Builder.ToString();
            var nbCommands = commands.Count(c => c == '\n');
            return nbCommands + Environment.NewLine + commands;
        }
    }
}
