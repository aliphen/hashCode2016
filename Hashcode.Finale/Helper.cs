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

        public static bool CanTakePicture(this Satellite s, Coords c, TimeRange t, out int pictureTurn)
        {
            pictureTurn = 0;

            if (s.CurrentTurn > t.End)
                return false;

            s = new Satellite(s); //clone
            if(s.CurrentTurn < t.Start)
            {
                s.Move(t.Start - s.CurrentTurn);
            }

            for (int turn = Math.Max(t.Start, s.CurrentTurn); turn < t.End; turn++)
            {
                if(c.IsInRange(s.Range, s.Pos))
                {
                    pictureTurn = turn;
                    return true;
                }
                s.Move(1);
            }

            return false;
        }

        public static bool IsInRange(this Satellite s, Coords c)
        {
            var dLat = Math.Abs(s.Pos.Lat - c.Lat);
            var dLon = Math.Abs(s.Pos.Lon - c.Lon);
            return dLat < s.MaxRot && dLon < s.MaxRot;
        }
    }
}
