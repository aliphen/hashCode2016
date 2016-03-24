﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Hashcode.Qualif
{
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

        public void Increase(int nbTurns, int rotSpeed, int maxRot)
        {
            DeltaLatMin -= rotSpeed * nbTurns;
            DeltaLatMin = Math.Max(DeltaLatMin, -maxRot);
            DeltaLatMax += rotSpeed * nbTurns;
            DeltaLatMax = Math.Min(DeltaLatMax, maxRot);
            DeltaLonMin -= rotSpeed * nbTurns;
            DeltaLonMin = Math.Max(DeltaLonMin, -maxRot);
            DeltaLonMax += rotSpeed * nbTurns;
            DeltaLonMax = Math.Min(DeltaLonMax, maxRot);
        }

        public override string ToString()
        {
            return string.Format("φ {0},{1} λ {2},{3}", DeltaLatMin, DeltaLatMax, DeltaLonMin, DeltaLonMax);
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

