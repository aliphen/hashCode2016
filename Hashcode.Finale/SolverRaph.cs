using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KdTree;
using KdTree.Math;
using System.Collections;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework.Compatibility;

namespace Hashcode.Qualif
{
    public class PartialSolution
    {
        public readonly Range CurrentRange;
        public readonly List<Snapshot> Snapshots;
        public readonly float EstimatedValue;
        public readonly BitArray PicturesTaken;

        public PartialSolution(BitArray taken)
        {
            CurrentRange = new Range();
            Snapshots = new List<Snapshot>();
            PicturesTaken = taken;
        }

        public PartialSolution(float val, Range range, List<Snapshot> snaps, BitArray taken)
        {
            EstimatedValue = val;
            CurrentRange = range;
            Snapshots = snaps;
            PicturesTaken = taken;
        }
    }

    internal class SolverRaph
    {
        private static readonly float SQRT2 = (float)Math.Sqrt(2);

        public static Solution Solve(Input input)
        {
            var tree = BuildTree(input);
            var satellites = input.Satellites.OrderBy(s => s.MaxRot).ToArray();
            var totalNbPic = tree.Count;
            var solution = new List<Snapshot>();

            var picturesConfirmed = new BitArray(totalNbPic);
            for (int s = 0; s < satellites.Length; s++)
            {
                var satellite = satellites[s];
                Console.WriteLine($"satellite {s+1}/{satellites.Length} (#{satellite.Id})");

                var state = new List<PartialSolution> {new PartialSolution(picturesConfirmed)};

                for (int t = 1; t < input.NbTurns; t++)
                {
                    if (t % 20000 == 0) Console.WriteLine(t);

                    Step(state, satellite);
                    SimplifyRanges(state);

                    //look for pictures to take
                    var candidates = tree.RadialSearch(new float[] {satellite.Pos.Lat, satellite.Pos.Lon}, satellite.MaxRot*SQRT2, 150); //TOO SLOW !
                    Helper.Assert(() => candidates.Length < 145, candidates.Length);
                    candidates = candidates.Where(node => !picturesConfirmed.Get(node.Value.Item1 + node.Value.Item2.BasePicId) && node.Value.Item2.PictureCanBeTaken(t)).ToArray();


                    var stopIdx = state.Count; //we're gonna add more, but we don't want to browse'em
                    for (int st = 0; st < stopIdx; st++)
                    {
                        var sol = state[st];
                        var range = sol.CurrentRange;
                        foreach (var candidate in candidates)
                        {
                            var collec = candidate.Value.Item2;
                            var picIdx = candidate.Value.Item1;
                            if(sol.PicturesTaken.Get(collec.BasePicId + picIdx))
                                continue;

                            var picLoc = collec.Locations[picIdx];
                            if (picLoc.IsInRange(range, satellite.Pos))
                            {
                                var newScore = sol.EstimatedValue + Score(collec, sol);
                                var newRange = new Range(satellite.Pos, picLoc);
                                if (!WorthTaking(newScore, newRange, state))
                                    continue;

                                var newCommands = new List<Snapshot>(sol.Snapshots);
                                newCommands.Add(new Snapshot(picLoc.Lat, picLoc.Lon, t, satellite.Id));
                                var taken = new BitArray(sol.PicturesTaken);
                                taken.Set(collec.BasePicId + picIdx, true);
                                state.Add(new PartialSolution (newScore, newRange, newCommands, taken));
                            }
                        }
                    }
                }

                PartialSolution best = null;
                foreach (var partialSolution in state)
                {
                    if (best == null || partialSolution.EstimatedValue > best.EstimatedValue)
                        best = partialSolution;
                }
                solution.AddRange(best.Snapshots);
                picturesConfirmed.Or(best.PicturesTaken); //union
            }

            int score = 0;
            foreach (var picCollection in input.Collections)
            {
                bool complete = true;
                var stop = picCollection.BasePicId + picCollection.Locations.Count;
                for (int i = picCollection.BasePicId; i < stop; i++)
                {
                    if (!picturesConfirmed.Get(i))
                    {
                        complete = false;
                        break;
                    }
                }
                if (complete)
                    score += picCollection.Value;
            }
            return new Solution(solution, score);
        }

        private static float Score(PicCollection picCollection, PartialSolution sol)
        {
            var completion = 0;
            var stop = picCollection.BasePicId + picCollection.Locations.Count;
            for (int i = picCollection.BasePicId; i < stop; i++)
            {
                if (sol.PicturesTaken.Get(i))
                    completion++;
            }
            float x;
            if (completion == picCollection.Locations.Count - 1)
                x = 10f;
            else
                x = (float) (completion + 1)/(picCollection.Locations.Count + 1);
            return x*x*picCollection.Value/picCollection.Locations.Count;
        }

        private static void Step(List<PartialSolution> ranges, Satellite sat)
        {
            foreach (var range in ranges)
            {
                range.CurrentRange.Increase(1, sat.RotSpeed, sat.MaxRot);
            }
            sat.Move(1);
        }

        private static void SimplifyRanges(List<PartialSolution> state)
        {
            //TODO find better algo than n²
            for (int i = 0; i < state.Count; i++)
            {
                var sol = state[i];
                if(!sol.CurrentRange.OneSideMaxed)
                    continue;

                for (int j = 0; j < state.Count; j++)
                {
                    if(j == i) //do not check with self
                        continue;

                    var other = state[j];
                    if (other.EstimatedValue <= sol.EstimatedValue
                        && sol.CurrentRange.Contains(other.CurrentRange))
                    {
                        state.RemoveAt(j); //we have something better, no need to keep this solution
                        //adjust indexes
                        j--;
                        if (j < i) i--;
                    }
                }
            }
        }

        static bool WorthTaking(float newScore, Range newRange, List<PartialSolution> state)
        {
            bool take = true;
            Parallel.ForEach(state, (sol, loop) =>
            {
                if (newScore <= sol.EstimatedValue && sol.CurrentRange.Contains(newRange))
                {
                    take = false;
                    loop.Stop();
                }
            });
            return take;
        }

        private static KdTree<float, Tuple<int, PicCollection>> BuildTree(Input input)
        {
            var tree = new KdTree<float, Tuple<int, PicCollection>>(2, new FloatMath());

            foreach (var picCollection in input.Collections)
            {
                for (int p = 0; p < picCollection.Locations.Count; p++)
                {
                    var pict = picCollection.Locations[p];
                    tree.Add(new float[] {pict.Lat, pict.Lon}, Tuple.Create(p, picCollection));
                }
            }
            tree.Balance();
            return tree;
        }
    }
}