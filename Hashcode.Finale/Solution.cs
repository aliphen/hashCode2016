using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Hashcode.Qualif
{
    public class Solution
    {
        private readonly Input _input;

        public int Score { get; private set; }

        public Solution(Input input)
        {
            _input = input;
            Score = 1;
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
