using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hashcode.Qualif
{
    public static class Helper
    {
        public static Random Rand = new Random();

        public static void Assert(Func<bool> test, string message = "")
        {
#if DEBUG
            Debug.Assert(test(), message);
#endif
        }

        // Distance to a client
        public static int Distance(int x1, int y1, int x2, int y2)
        {
            return (int) Math.Ceiling(Math.Sqrt((x1 - x2)* (x1 - x2) + (y1 - y2)* (y1 - y2)));
        }
    }
}
