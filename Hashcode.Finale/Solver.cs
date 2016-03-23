using System;
using System.Collections.Generic;
using System.Linq;
using KdTree;
using KdTree.Math;

namespace Hashcode.Qualif
{
    internal class Solver
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

            for (int s = 0; s < input.Satellites.Count; ++s)
            {
                var satellite = input.Satellites[s];

                for (int turn = 0; turn < input.NbTurns; ++turn)
                {
                    var node = tree.RadialSearch(new float[] { satellite.Pos.Lat, satellite.Pos.Lon }, satellite.MaxRot, 150);

                    if (node.Length > 0)
                    {
                        var valid = node.Where(n => n.Value.PictureCanBeTaken(turn)).OrderByDescending(treeNode => treeNode.Value.TakenPictures.Count)
                            .FirstOrDefault(k => satellite.CanTakePicture((int)k.Point[0], (int)k.Point[1]));

                        if (valid != null)
                        {
                            var pict = valid;

                            var pictCoord = new Coords((int)pict.Point[0], (int)pict.Point[1]);

                            if (satellite.CanTakePicture(pictCoord))
                            {
                                var snap = satellite.TakePicture(pictCoord, s);
                                takenPictures.Add(snap);
                                pict.Value.TakePicture(pictCoord);
                                tree.RemoveAt(pict.Point);
                                Console.WriteLine("Satellite {1} Found {0} pict - Turn {2}", node.Length, s, turn);

                                Console.WriteLine("Satellite Lat {0} Lon {1} Pict {2} {3} Rot {4} {5}", satellite.Pos.Lat,
                                    satellite.Pos.Lon, pict.Point[0], pict.Point[1], satellite.CurrentRot.Lat, satellite.CurrentRot.Lon);

                                if ((Math.Abs(satellite.CurrentRot.Lat) > satellite.MaxRot) || (Math.Abs(satellite.CurrentRot.Lon) > satellite.MaxRot))
                                {
                                    throw new Exception("Illegal state");
                                }
                            }
                        }
                    }
                    satellite.NextTurn();
                } 
            }


            var score = input.Collections.Where(c => c.Locations.Count == 0 && c.TakenPictures.Count > 0).Sum(p => p.Value);

            var solution = new Solution(takenPictures, score);

            return solution;
        }
    }
}