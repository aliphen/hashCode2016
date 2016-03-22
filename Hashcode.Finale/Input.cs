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
            && origin.Lon + range.DeltaLonMin <= Lon;
        }
    }

    public class Range
    {
        public int DeltaLatMin;
        public int DeltaLatMax;
        public int DeltaLonMin;
        public int DeltaLonMax;
    }

    public class Satellite
    {
        public Range Range;
        public Coords CurrentRot;
        public int CurrentTurn;
        public readonly Coords Pos;
        public int Speed;
        public readonly int RotSpeed;
        public readonly int MaxRot;

        public Satellite(int lat, int lon, int speed, int rotSpeed, int maxRot)
        {
            Pos = new Coords { Lat = lat, Lon = lon };
            Speed = speed;
            RotSpeed = rotSpeed;
            MaxRot = maxRot;
        }

        public Satellite(Satellite s)
            : this(s.Pos.Lat, s.Pos.Lon, s.Speed, s.RotSpeed, s.MaxRot)
        {
            CurrentRot = new Coords(s.CurrentRot);
            CurrentTurn = s.CurrentTurn;
        }

        public void Move(int nbTurns)
        {
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

            Range.DeltaLatMin = Math.Max(-MaxRot, CurrentRot.Lat - RotSpeed*nbTurns);
            Range.DeltaLatMax = Math.Min(MaxRot, CurrentRot.Lat + RotSpeed*nbTurns);
            Range.DeltaLonMin = Math.Max(-MaxRot, CurrentRot.Lon - RotSpeed*nbTurns);
            Range.DeltaLonMax = Math.Min(MaxRot, CurrentRot.Lon + RotSpeed*nbTurns);
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

        public void TakePicture(Coords pict)
        {
            // delta satellite and camera position
            var deltaLat = pict.Lat - (Pos.Lat + CurrentRot.Lat);
            var deltaLon = pict.Lon - (Pos.Lon + CurrentRot.Lon);

            CurrentRot.Lat += deltaLat;
            CurrentRot.Lon += deltaLon;
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

