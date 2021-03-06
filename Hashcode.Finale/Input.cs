﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Hashcode.Qualif
{
    public class Range
    {
        public bool OneSideMaxed { get; private set; }
        public int DeltaLatMin;
        public int DeltaLatMax;
        public int DeltaLonMin;
        public int DeltaLonMax;

        public Range()
        {
        }

        /// <summary>
        /// creates a range of size zero on the delta between coords and orig
        /// </summary>
        public Range(Coords origin, Coords coords)
        {
            var lat = coords.Lat - origin.Lat;
            var lon = coords.Lon - origin.Lon;
            DeltaLatMin = lat;
            DeltaLatMax = lat;
            DeltaLonMin = lon;
            DeltaLonMax = lon;
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
            if (DeltaLatMin <= -maxRot)
            {
                OneSideMaxed = true;
                DeltaLatMin = -maxRot;
            }
            DeltaLatMax += rotSpeed * nbTurns;
            if (DeltaLatMax >= maxRot)
            {
                OneSideMaxed = true;
                DeltaLatMax = maxRot;
            }
            DeltaLonMin -= rotSpeed * nbTurns;
            if (DeltaLonMin <= -maxRot)
            {
                OneSideMaxed = true;
                DeltaLonMin = -maxRot;
            }
            DeltaLonMax += rotSpeed * nbTurns;
            if (DeltaLonMax >= maxRot)
            {
                OneSideMaxed = true;
                DeltaLonMax = maxRot;
            }
        }

        public bool Contains(Range other)
        {
            return other.DeltaLatMin >= DeltaLatMin
                   && other.DeltaLatMax <= DeltaLatMax
                   && other.DeltaLonMin >= DeltaLonMin
                   && other.DeltaLonMax <= DeltaLonMax;
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
        public readonly int BasePicId;
        public readonly int Value;
        public readonly List<Coords> Locations;
        public readonly List<TimeRange> TimeRanges;
        public readonly List<Coords> TakenPictures;

        public PicCollection(int value, int basePicId)
        {
            BasePicId = basePicId;
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

