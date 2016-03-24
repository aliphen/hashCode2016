using System;

namespace Hashcode.Qualif
{
    public class Satellite
    {
        private static readonly Random Rng = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));

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
            Pos = new Coords {Lat = lat, Lon = lon};
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

        public Satellite Clone()
        {
            return new Satellite(this);
        }

        public void Move(int nbTurns)
        {
            CurrentTurn += nbTurns;

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

            AdjustLongitude(Pos);

            Range.Increase(nbTurns, RotSpeed, MaxRot);
        }

        public bool CanTakePicture(Coords pict)
        {
            return ((pict.Lat <= Pos.Lat + Range.DeltaLatMax) && (pict.Lat >= Pos.Lat + Range.DeltaLatMin)) && (pict.Lon >= Pos.Lon + Range.DeltaLonMin) &&
                   (pict.Lon <= Pos.Lon + Range.DeltaLonMax);
        }

        public bool CanTakePicture(int pictLat, int pictLon)
        {
            return CanTakePicture(new Coords(pictLat, pictLon));
        }

        public void NextTurn()
        {
            Move(1);
        }

        private static void AdjustLongitude(Coords coord)
        {
            //adjust longitude
            coord.Lon += Coords.OneEightyDegrees; //put it in 0-360 range
            coord.Lon = coord.Lon%Coords.ThreeSixtyDegrees;
            if (coord.Lon < 0)
                coord.Lon += Coords.ThreeSixtyDegrees;
            coord.Lon -= Coords.OneEightyDegrees; //put it back in game coords (-180 - 180)
        }
        
        public Snapshot TakePicture(int pictLat, int pictLon)
        {
            return TakePicture(new Coords(pictLat, pictLon));
        }

        /// <summary>
        /// assumes satellite is already at the right position
        /// </summary>
        public Snapshot TakePicture(Coords pict)
        {
            CurrentRot.Lat = pict.Lat - Pos.Lat;
            CurrentRot.Lon = pict.Lon - Pos.Lon;
            Range = new Range {DeltaLatMin = CurrentRot.Lat, DeltaLatMax = CurrentRot.Lat, DeltaLonMin = CurrentRot.Lon, DeltaLonMax = CurrentRot.Lon}; //reset range to zero around current direction
            return new Snapshot(pict.Lat, pict.Lon, CurrentTurn, Id);
        }

        public void RandomCameraMove()
        {
            int maxTries = 10;
            while (maxTries > 0)
            {
                var moveLat = Rng.Next(-RotSpeed, RotSpeed);
                var moveLon = Rng.Next(-RotSpeed, RotSpeed);
                if (CanTakePicture(Pos.Lat + moveLat, Pos.Lon + moveLon))
                {
                    TakePicture(Pos.Lat + moveLat, Pos.Lon + moveLon);
                    return;
                }
                --maxTries;
            }
        }

        private bool _goRight = false;

        public void TryGoRightThenLeft()
        {
            if (_goRight)
            {
                if (CurrentRot.Lat + RotSpeed < MaxRot)
                {
                    CurrentRot.Lat += 1;
                    CurrentRot.Lon += 1;
                }
                else
                {
                    CurrentRot.Lat -= 1;
                    CurrentRot.Lon -= 1;
                    _goRight = false;
                }
            }
            else
            {
                if (CurrentRot.Lat - RotSpeed > -MaxRot)
                {
                    CurrentRot.Lat -= 1;
                    CurrentRot.Lon -= 1;
                }
                else
                {
                    CurrentRot.Lat += 1;
                    CurrentRot.Lon += 1;
                    _goRight = true;
                }

            }
        }
    }
}