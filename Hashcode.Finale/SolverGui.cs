using System;
using System.Collections.Generic;
using System.Linq;
using KdTree;
using KdTree.Math;

namespace Hashcode.Qualif
{
    internal class SolverGui
    {
        public static Solution Solve(Input input)
        {
            var takenPictures = new List<Snapshot>();

            var tree = new KdTree<float, PicCollection>(2, new FloatMath());

            foreach (var picCollection in input.Collections)
            {
                foreach (var pict in picCollection.Locations)
                {
                    tree.Add(new float[] { pict.Lat, pict.Lon }, picCollection);
                }
            }
            tree.Balance();

            var orderedSatellelites = input.Satellites.OrderByDescending(sat => sat.MaxRot).ToList();

            for (int s = 0; s < orderedSatellelites.Count; ++s)
            {
                var satellite = orderedSatellelites[s];

                for (int turn = 0; turn < input.NbTurns; ++turn)
                {
                    var node = tree.RadialSearch(new float[] { satellite.Pos.Lat, satellite.Pos.Lon }, satellite.MaxRot, 150).ToArray();

                    if (node.Length > 0)
                    {
                        var valids = node.Where(n => n.Value.PictureCanBeTaken(turn))
                            .Where(k => satellite.CanTakePicture((int)k.Point[0], (int)k.Point[1]))
                            .OrderByDescending(k => Math.Pow(k.Point[0] - satellite.Pos.Lat, 2) + Math.Pow(k.Point[1] - satellite.Pos.Lon, 2))
                            //.OrderByDescending(k => k.Value.TakenPictures.Count)
                            .ToList();

                        if (valids.Count > 0)
                        {
                            Console.WriteLine("Found {0} valid new positions", valids.Count);

                            var pict = valids[0];
                            var pictCoord = new Coords((int)pict.Point[0], (int)pict.Point[1]);

                            var snap = satellite.TakePicture(pictCoord);
                            takenPictures.Add(snap);
                            pict.Value.TakePicture(pictCoord);
                            pict.Value.Locations.Remove(pictCoord);
                            tree.RemoveAt(pict.Point);
                            Console.WriteLine("Satellite {1} Found {0} pict - Turn {2}", node.Length, satellite.Id, turn);

                            ////Console.WriteLine("Satellite Lat {0} Lon {1} Pict {2} {3} Rot {4} {5}", satellite.Pos.Lat,
                            //satellite.Pos.Lon, pict.Point[0], pict.Point[1], satellite.CurrentRot.Lat, satellite.CurrentRot.Lon);
                        }

                    }
                    satellite.NextTurn();
                }
            }


            var score = input.Collections.Where(c => c.Locations.Count == 0 && c.TakenPictures.Count > 0).Sum(p => p.Value);

            var solution = new Solution(takenPictures, score);

            return solution;
        }

        public static int GetNumberPictInRangeInNextTurns(Satellite satellite, KdTreeNode<float, PicCollection> n, KdTree<float, PicCollection> tree)
        {
            var possiblePict = new Coords((int)n.Point[0], (int)n.Point[1]);
            var copySatellite = satellite.Clone();
            copySatellite.TakePicture(possiblePict);
            copySatellite.NextTurn();
            var inRange = tree.RadialSearch(new float[] { copySatellite.Pos.Lat, copySatellite.Pos.Lon }, copySatellite.MaxRot, 150)
                .Where(no => no.Value.PictureCanBeTaken(copySatellite.CurrentTurn))
                .Where(k => copySatellite.CanTakePicture((int)k.Point[0], (int)k.Point[1])).Count() + GetNumberPictInRangeFixTrajectory(copySatellite, tree, 500);

            //Console.WriteLine("Number in range for {0} {1}: {2}", possiblePict.Lat, possiblePict.Lon, inRange);
            return inRange;
        }

        public static int GetNumberPictInRangeFixTrajectory(Satellite satellite, KdTree<float, PicCollection> tree, int maxIter)
        {
            if (maxIter < 0)
            {
                return 0;
            }
            var copySatellite = satellite.Clone();
            copySatellite.NextTurn();
            return tree.RadialSearch(new float[] { copySatellite.Pos.Lat, copySatellite.Pos.Lon }, copySatellite.MaxRot, 150)
                .Where(no => no.Value.PictureCanBeTaken(copySatellite.CurrentTurn))
                .Where(k => copySatellite.CanTakePicture((int)k.Point[0], (int)k.Point[1])).Count() + GetNumberPictInRangeFixTrajectory(copySatellite, tree, --maxIter);
        }

    }
}