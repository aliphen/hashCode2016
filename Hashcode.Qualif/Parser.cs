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
            using(var reader = new StreamReader(fileName))
            {
                var inputParams = reader.ReadLine().Split(' ').Select(Int32.Parse);
//                input.R = inputParams[0];
//                input.C = inputParams[1];

                for(int i = 0; i < /*nbLines*/0; i++)
                {
                    var line = reader.ReadLine().Split(' ').Select(Int32.Parse);
                    //fill input
                }
            }
            return input;
        }
    }
}

