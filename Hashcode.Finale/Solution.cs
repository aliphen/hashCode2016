using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Hashcode.Qualif
{
    public class Solution
    {
        private readonly List<Snapshot> _snapshots;

        public int Score { get; private set; }

        public Solution(List<Snapshot> snapshots)
        {
            _snapshots = snapshots;
            Score = 1;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(_snapshots.Count.ToString());
            _snapshots.ForEach(s => sb.AppendLine(s.ToString()));
            return sb.ToString();
        }

        public class Tests
        {
            [Test]
            public void TestScore()
            {
            }
        }
    }
}
