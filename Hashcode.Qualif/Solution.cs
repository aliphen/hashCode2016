using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hashcode.Qualif
{
    public class Solution
    {
        public int Score = 0;
        public StringBuilder Builder = new StringBuilder();

        /// <summary>
        /// returns the solution ready to be written to the output file
        /// </summary>
        public override string ToString()
        {
            return Builder.ToString();
        }
    }
}
