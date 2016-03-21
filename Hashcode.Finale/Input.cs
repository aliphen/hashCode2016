using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using NUnit.Framework;

namespace Hashcode.Qualif
{
    public class Coords
    {
        public const int NinetyDegrees = 324000;
        public const int OneEightyDegrees = 2*NinetyDegrees;
        public const int ThreeSixtyDegrees = 4*NinetyDegrees;

        /// <summary> aka φ </summary>
        public int Lat;
        /// <summary> aka λ </summary>
        public int Lon;
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
            Pos = new Coords {Lat = lat, Lon = lon};
            Speed = speed;
            RotSpeed = rotSpeed;
            MaxRot = maxRot;
        }

        public void Move(int nbTurns)
        {
            Pos.Lat += Speed*nbTurns;
            Pos.Lon += Input.EarthRotationSpeed*nbTurns;

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

            //adjust longitude
            Pos.Lon += Coords.OneEightyDegrees; //put it in 0-360 range
            Pos.Lon = Pos.Lon%Coords.ThreeSixtyDegrees;
            if (Pos.Lon < 0)
                Pos.Lon += Coords.ThreeSixtyDegrees;
            Pos.Lon -= Coords.OneEightyDegrees; //put it back in game coords (-180 - 180)
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

