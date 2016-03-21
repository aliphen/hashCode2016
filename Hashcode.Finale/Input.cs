using System.Collections.Generic;

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

        public Coords(Coords c) : this(c.Lat, c.Lon)
        {
        }

    }

    public class Satellite
    {
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

        public Satellite(Satellite s) : this(s.Pos.Lat, s.Pos.Lon, s.Speed, s.RotSpeed, s.MaxRot)
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
    }

    public class PicCollection
    {
        public readonly int Value;
        public readonly List<Coords> Locations;
        public readonly List<TimeRange> TimeRanges;

        public PicCollection(int value)
        {
            Value = value;
            Locations = new List<Coords>();
            TimeRanges = new List<TimeRange>();
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

