using System;
using System.IO;
using System.Linq;

namespace Hashcode.Qualif
{
    public static class Parser
    {
        public static Input Parse(string fileName)
        {
            var input = new Input();
            using (var reader = new StreamReader(fileName))
            {
                input.NbTurns = ReadInt(reader);

                var nbSat = ReadInt(reader);
                for (int i = 0; i < nbSat; i++)
                {
                    var line = ReadMulti(reader);
                    var sat = new Satellite(
                        lat: line[0],
                        lon: line[1],
                        speed: line[2],
                        rotSpeed: line[3],
                        maxRot: line[4]);
                    input.Satellites.Add(sat);
                }

                var nbCollec = ReadInt(reader);
                for (int i = 0; i < nbCollec; i++)
                {
                    var line = ReadMulti(reader);
                    var collec = new PicCollection(value: line[0]);
                    var nbLoc = line[1];
                    for (int j = 0; j < nbLoc; j++)
                    {
                        var loc = ReadMulti(reader);
                        collec.Locations.Add(new Coords {Lat = loc[0], Lon = loc[1]});
                    }
                    var nbRanges = line[2];
                    for (int j = 0; j < nbRanges; j++)
                    {
                        var time = ReadMulti(reader);
                        collec.TimeRanges.Add(new TimeRange(time[0], time[1]));
                    }
                    input.Collections.Add(collec);
                }
            }
            return input;
        }

        #region helper methods

        private static int ReadInt(StreamReader reader)
        {
            return Int32.Parse(reader.ReadLine());
        }

        private static int[] ReadMulti(StreamReader reader)
        {
            return reader.ReadLine().Split(' ').Select(Int32.Parse).ToArray();
        }

        #endregion
    }
}

