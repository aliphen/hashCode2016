﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KdTree;
using KdTree.Math;

namespace Hashcode.Qualif
{
    public class PartialSolution
    {
        public readonly Range CurrentRange;
        public readonly List<Snapshot> Snapshots;
        public readonly int EstimatedValue;
        public readonly HashSet<Coords> PicturesTaken;

        public PartialSolution(HashSet<Coords> taken)
        {
            CurrentRange = new Range();
            Snapshots = new List<Snapshot>();
            PicturesTaken = taken;
        }

        public PartialSolution(int val, Range range, List<Snapshot> snaps, HashSet<Coords> taken)
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
            var solution = new List<Snapshot>();

            var picturesConfirmed = new HashSet<Coords>();
            for (int s = 0; s < input.Satellites.Count; s++)
            {
                Console.WriteLine($"satellite {s}");
                var satellite = input.Satellites[s];
                
                var state = new List<PartialSolution> {new PartialSolution(picturesConfirmed)};

                for (int t = 1; t < input.NbTurns; t++)
                {
                    if(t%50000 == 0) Console.WriteLine(t);

                    Step(state, satellite);
                    SimplifyRanges(state);

                    //look for pictures to take
                    var candidates = tree.RadialSearch(new float[] {satellite.Pos.Lat, satellite.Pos.Lon}, satellite.MaxRot*SQRT2, 150);
                    Helper.Assert(() => candidates.Length < 145, candidates.Length);
                    candidates = candidates.Where(node => node.Value.Item2.PictureCanBeTaken(t)).ToArray();

                    var stopIdx = state.Count; //we're gonna add more, but we don't want to browse'em
                    for (int st = 0; st < stopIdx; st++)
                    {
                        var sol = state[st];
                        var range = sol.CurrentRange;
                        foreach (var candidate in candidates)
                        {
                            var picLoc = candidate.Value.Item2.Locations[candidate.Value.Item1];
                            if(sol.PicturesTaken.Contains(picLoc))
                                continue;

                            if (picLoc.IsInRange(range, satellite.Pos))
                            {
                                var newScore = sol.EstimatedValue + Score(candidate.Value.Item2);
                                var newCommands = new List<Snapshot>(sol.Snapshots);
                                newCommands.Add(new Snapshot(picLoc.Lat, picLoc.Lon, t, satellite.Id));
                                var newRange = new Range(satellite.Pos, picLoc);
                                var taken = new HashSet<Coords>(sol.PicturesTaken);
                                taken.Add(picLoc);
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
                picturesConfirmed.UnionWith(best.PicturesTaken);
            }
            
            return new Solution(solution, 2);
        }

        private static int Score(PicCollection picCollection)
        {
            //TODO be smarter
            return picCollection.Value/picCollection.Locations.Count;
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
            for (int i = 0; i < state.Count; i++)
            {
                var sol = state[i];
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