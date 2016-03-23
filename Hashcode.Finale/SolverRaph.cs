using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hashcode.Qualif
{
    internal class SolverRaph
    {
        private static object locker = new object();

        public static Solution Solve(Input input)
        {
            var snapShots = new List<Snapshot>();
            //var solution = new Solution(input);

            for (var id = 0; id < input.Satellites.Count; id++)
            {
                Console.WriteLine("Sat " + id);
                var satellite = input.Satellites[id];

                while (true)
                {
                    var closestTurn = 1000000; //longer than any simu
                    Coords closestPicture = null;
                    PicCollection closestPicCollection = null;
                    Action remover = null;
                    Parallel.ForEach(input.Collections, col =>
                    //foreach (var col in input.Collections)
                    {
                        for (int p = 0; p < col.Locations.Count; p++)
                        {
                            var pic = col.Locations[p];
                            foreach (var range in col.TimeRanges)
                            {
                                if (range.Start > satellite.CurrentTurn + closestTurn)
                                    continue;
                                int turn;
                                if (satellite.CanTakePicture(pic, range, out turn, closestTurn) && turn < closestTurn)
                                {
                                    lock(locker)
                                    {
                                        if (turn < closestTurn)
                                        {
                                            closestTurn = turn;
                                            closestPicture = pic;
                                            closestPicCollection = col;
                                            var idx = p;
                                            remover = () => col.Locations.RemoveAt(idx);
                                        }
                                    }
                                }
                            }
                        }
                    });
                    if (closestPicture == null)
                        break;
                    satellite.Move(closestTurn - satellite.CurrentTurn);
                    snapShots.Add(satellite.TakePicture(closestPicture));
                    closestPicCollection.TakePicture(closestPicture);
                    remover();
                    Console.WriteLine(closestTurn);
                }
            }

            var score = input.Collections.Where(c => (c.Locations.Count == 0) && (c.TakenPictures.Count > 0)).Sum(p => p.Value);
            return new Solution(snapShots, score);
        }
    }
}