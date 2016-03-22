using System;

namespace Hashcode.Qualif
{
    public class Snapshot
    {
        public int Lat { get; set; }
        public int Lon { get; set; }
        public int Turn { get; set; }
        public int SatelliteId { get; set; }

        public Snapshot(int lat, int lon, int turn, int satelliteId)
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
