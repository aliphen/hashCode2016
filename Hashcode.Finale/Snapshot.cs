using System;

namespace Hashcode.Qualif
{
    public class Snapshot
    {
        public long Lat { get; set; }
        public long Lon { get; set; }
        public int Turn { get; set; }
        public int SatelliteId { get; set; }

        public Snapshot(long lat, long lon, int turn, int satelliteId)
        {
            Lat = lat;
            Lon = lon;
            Turn = turn;
            SatelliteId = satelliteId;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", Lat, Lon, Turn, SatelliteId);
        }
    }
}
