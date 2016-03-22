using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Hashcode.Qualif
{
    public class Coords : IEquatable<Coords>
    {
        public const int NinetyDegrees = 324000;
        public const int OneEightyDegrees = 2 * NinetyDegrees;
        public const int ThreeSixtyDegrees = 4 * NinetyDegrees;

        /// <summary> aka φ </summary>
        public int Lat;

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

    }

    public class Satellite
    {
        public readonly int RotSpeed;
        public readonly int MaxRot;

        public int Speed;
        public readonly Coords Pos;
        public int CurrentTurn;
        public Coords CurrentRot;

        public int Top = 0;
        public int Bottom = 0;
        public int Right = 0;
        public int Left = 0;

        public Satellite(int lat, int lon, int speed, int rotSpeed, int maxRot)
        {
            Pos = new Coords { Lat = lat, Lon = lon };
            Speed = speed;
            RotSpeed = rotSpeed;
            MaxRot = maxRot;
            CurrentRot = new Coords(0, 0);
        }

        public Satellite(Satellite s)
            : this(s.Pos.Lat, s.Pos.Lon, s.Speed, s.RotSpeed, s.MaxRot)
        {
            Speed = s.Speed;
            Pos = new Coords(s.Pos.Lat, s.Pos.Lon);
            CurrentRot = new Coords(s.CurrentRot);
            CurrentTurn = s.CurrentTurn;
            Top = s.Top;
            Bottom = s.Bottom;
            Right = s.Right;
            Left = s.Left;
        }

        public bool CanTakePicture(Coords pict)
        {
            return ((pict.Lat <= Pos.Lat + Top) && (pict.Lat >= Pos.Lat + Bottom)) && (pict.Lon >= Pos.Lon + Left) &&
                   (pict.Lon <= Pos.Lon + Right);
        }

        public bool CanTakePicture(int pictLat, int pictLon)
        {
            return ((pictLat <= Pos.Lat + Top) && (pictLat >= Pos.Lat + Bottom)) && (pictLon >= Pos.Lon + Left) &&
               (pictLon <= Pos.Lon + Right);
        }

        public Snapshot TakePicture(Coords pict, int satelliteId)
        {
            CurrentRot.Lat = pict.Lat - Pos.Lat;
            CurrentRot.Lon = pict.Lon - Pos.Lon;

            Top = CurrentRot.Lat;
            Left = CurrentRot.Lon;
            Bottom = CurrentRot.Lat;
            Right = CurrentRot.Lon;

            return new Snapshot(pict.Lat, pict.Lon, CurrentTurn, satelliteId);
        }

        public void NextTurn()
        {
            Pos.Lat += Speed;
            Pos.Lon += Input.EarthRotationSpeed;

            Top = Math.Min(Top + RotSpeed, MaxRot);
            Bottom = Math.Max(Bottom - RotSpeed, -MaxRot);
            Left = Math.Max(Left - RotSpeed, -MaxRot);
            Right = Math.Min(Right + RotSpeed, MaxRot);

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

            //adjust longitude
            Pos.Lon += Coords.OneEightyDegrees; //put it in 0-360 range
            Pos.Lon = Pos.Lon % Coords.ThreeSixtyDegrees;
            if (Pos.Lon < 0)
                Pos.Lon += Coords.ThreeSixtyDegrees;
            Pos.Lon -= Coords.OneEightyDegrees; //put it back in game coords (-180 - 180)

            ++CurrentTurn;
        }

        public void Move(int nbTurns)
        {
            for (var i = 0; i < nbTurns; ++i)
            {
                NextTurn();
            }
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

        public bool Contains(int turn)
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
            return TimeRanges.Any(r => r.Contains(turn));
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

