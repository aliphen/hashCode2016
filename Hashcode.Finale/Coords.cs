using System;

namespace Hashcode.Qualif
{
    public class Coords : IEquatable<Coords>
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

        public bool Equals(Coords other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Lat == other.Lat && Lon == other.Lon;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Coords)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Lat * 397) ^ Lon;
            }
        }

        public override string ToString()
        {
            return string.Format("φ {0} λ {1}", Lat, Lon);
        }
    }
}