using System;

namespace Hashcode.Qualif
{
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
}