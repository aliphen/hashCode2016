namespace Hashcode.Qualif
{
    public class Coords
    {
        public const int NinetyDegrees = 324000;
        public const int OneEightyDegrees = 2 * NinetyDegrees;
        public const int ThreeSixtyDegrees = 4 * NinetyDegrees;

        /// <summary> aka φ </summary>
        public int Lat;

        /// <summary> aka λ </summary>
        public int Lon;

        public Coords()
        {

        }

        public Coords(int lat, int lon)
        {
            Lat = lat;
            Lon = lon;
        }

        public Coords(Coords c)
            : this(c.Lat, c.Lon)
        {
        }

        public bool IsInRange(Range range, Coords origin)
        {
            return origin.Lat + range.DeltaLatMin <= Lat
                   && origin.Lat + range.DeltaLatMax >= Lat
                   && origin.Lon + range.DeltaLonMin <= Lon
                   && origin.Lon + range.DeltaLonMax >= Lon;
        }

        public override string ToString()
        {
            return string.Format("φ {0} λ {1}", Lat, Lon);
        }
    }
}