using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;

namespace Hashcode.Qualif
{
    public class Coords
    {
        public int Lat;
        public int Lon;
    }

    public class Satellite
    {
        public Coords CurrentRot;
        public readonly Coords Pos;
        public readonly int Speed;
        public readonly int RotSpeed;
        public readonly int MaxRot;

        public Satellite(int lat, int lon, int speed, int rotSpeed, int maxRot)
        {
            Pos = new Coords {Lat = lat, Lon = lon};
            Speed = speed;
            RotSpeed = rotSpeed;
            MaxRot = maxRot;
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
        public int NbTurns;
        public readonly List<Satellite> Satellites = new List<Satellite>();
        public readonly List<PicCollection> Collections = new List<PicCollection>();
    }
}

