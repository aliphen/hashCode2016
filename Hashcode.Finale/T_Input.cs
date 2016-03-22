using NUnit.Framework;

namespace Hashcode.Qualif
{
    class T_Input
    {

        [TestCase(1, 176520L, 7185L)]
        [TestCase(3600, 39600L, 601200L)]
        public void SatelliteMoveIsCorrect(int nbTurn, long expectedLat, long expectedLon)
        {
            var satellite = new Satellite(176400, 7200, 120, 0, 0, 0);

            satellite.Move(nbTurn);

            Assert.AreEqual(expectedLat, satellite.Pos.Lat);
            Assert.AreEqual(expectedLon, satellite.Pos.Lon);
        }


    }
}
