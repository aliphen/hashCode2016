using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

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

    public class Range
    {
        public int DeltaLatMin;
        public int DeltaLatMax;
        public int DeltaLonMin;
        public int DeltaLonMax;

        public Range()
        {

        }

        public Range(Range range)
        {
            DeltaLatMin = range.DeltaLatMin;
            DeltaLatMax = range.DeltaLatMax;
            DeltaLonMin = range.DeltaLonMin;
            DeltaLonMax = range.DeltaLonMax;
        }

        public override string ToString()
        {
            return string.Format("φ {0},{1} λ {2},{3}", DeltaLatMin, DeltaLatMax, DeltaLonMin, DeltaLonMax);
        }
    }

    public class Satellite
    {
        public Range Range = new Range();
        public Coords CurrentRot = new Coords();
        public int CurrentTurn;
        public readonly Coords Pos;
        public int Speed;
        public readonly int RotSpeed;
        public readonly int MaxRot;
        public readonly int Id;

        public Satellite(int lat, int lon, int speed, int rotSpeed, int maxRot, int id)
        {
            Pos = new Coords { Lat = lat, Lon = lon };
            Speed = speed;
            RotSpeed = rotSpeed;
            MaxRot = maxRot;
            Id = id;
        }

        public Satellite(Satellite s)
            : this(s.Pos.Lat, s.Pos.Lon, s.Speed, s.RotSpeed, s.MaxRot, s.Id)
        {
            CurrentRot = new Coords(s.CurrentRot);
            CurrentTurn = s.CurrentTurn;
            Range = new Range(s.Range);
        }

        public void Move(int nbTurns)
        {
            CurrentTurn += nbTurns;

            Pos.Lat += Speed * nbTurns;
            Pos.Lon += Input.EarthRotationSpeed * nbTurns;

            while (true)
            {
                if (Pos.Lat > Coords.NinetyDegrees) //satellite flew over the North Pole
                {
                    Pos.Lat = Coords.OneEightyDegrees - Pos.Lat;
                    Pos.Lon -= Coords.OneEightyDegrees;
                    Speed = -Speed;
                }
                else if (Pos.Lat < -Coords.NinetyDegrees) //satellite flew over the South Pole
                {
                    Pos.Lat = -Coords.OneEightyDegrees - Pos.Lat;
                    Pos.Lon -= Coords.OneEightyDegrees;
                    Speed = -Speed;
                }
                else
                    break; //Lat is in legal range
            }

            AdjustLongitude(Pos);

            Range.DeltaLatMin -= RotSpeed*nbTurns;
            Range.DeltaLatMin = Math.Max(Range.DeltaLatMin, -MaxRot);
            Range.DeltaLatMax += RotSpeed*nbTurns;
            Range.DeltaLatMax = Math.Min(Range.DeltaLatMax, MaxRot);
            Range.DeltaLonMin -= RotSpeed*nbTurns;
            Range.DeltaLonMin = Math.Max(Range.DeltaLonMin, -MaxRot);
            Range.DeltaLonMax += RotSpeed*nbTurns;
            Range.DeltaLonMax = Math.Min(Range.DeltaLonMax, MaxRot);
        }

        public Satellite NextTurn()
        {
            var s = new Satellite(this);
            s.Move(1);
            return s;
        }

        private static void AdjustLongitude(Coords coord)
        {
            //adjust longitude
            coord.Lon += Coords.OneEightyDegrees; //put it in 0-360 range
            coord.Lon = coord.Lon % Coords.ThreeSixtyDegrees;
            if (coord.Lon < 0)
                coord.Lon += Coords.ThreeSixtyDegrees;
            coord.Lon -= Coords.OneEightyDegrees; //put it back in game coords (-180 - 180)
        }

        public bool CanTakePictureNextTurn(Coords pict)
        {
            var next = NextTurn();

            return pict.IsInRange(next.Range, next.Pos);
        }

        /// <summary>
        /// assumes satellite is already at the right position
        /// </summary>
        public Snapshot TakePicture(Coords pict)
        {
            CurrentRot.Lat = pict.Lat - Pos.Lat;
            CurrentRot.Lon = pict.Lon - Pos.Lon;
            Range = new Range{DeltaLatMin = CurrentRot.Lat, DeltaLatMax = CurrentRot.Lat, DeltaLonMin = CurrentRot.Lon, DeltaLonMax = CurrentRot.Lon}; //reset range to zero around current direction
            return new Snapshot(pict.Lat, pict.Lon, CurrentTurn, Id);
        }

    }

    public class TimeRange
    {
        public readonly int Start;
        public readonly int End;

        public TimeRange(int start, int end)
        {
            Start = start;
            End = end;
        }

        public bool IsInside(int turn)
        {
            return Start <= turn && turn <= End;
        }
    }

    public class PicCollection
    {
        public readonly int Value;
        public readonly List<Coords> Locations;
        public readonly List<TimeRange> TimeRanges;
        public readonly List<Coords> TakenPictures;

        public PicCollection(int value)
        {
            Value = value;
            Locations = new List<Coords>();
            TimeRanges = new List<TimeRange>();
            TakenPictures = new List<Coords>();
        }

        public void TakePicture(Coords coord)
        {
            TakenPictures.Add(coord);
            Locations.Remove(coord);
        }

        public bool PictureCanBeTaken(int turn)
        {
            return TimeRanges.Any(range => range.Start <= turn && turn <= range.End);
        }
    }

    /// <summary>
    /// contains an object representation of the input
    /// </summary>
    public class Input
    {
        public const int EarthRotationSpeed = -15;

        public int NbTurns;
        public readonly List<Satellite> Satellites = new List<Satellite>();
        public readonly List<PicCollection> Collections = new List<PicCollection>();
    }
}

