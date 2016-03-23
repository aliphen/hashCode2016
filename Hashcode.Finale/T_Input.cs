using NUnit.Framework;

namespace Hashcode.Qualif
{
    class T_Input
    {

        [TestCase(1, 176520, 7185)]
        [TestCase(3600, 39600, 601200)]
        public void SatelliteMoveIsCorrect(int nbTurn, int expectedLat, int expectedLon)
        {
            var satellite = new Satellite(176400, 7200, 120, 0, 0, 0);

            satellite.Move(nbTurn);

            Assert.AreEqual(expectedLat, satellite.Pos.Lat);
            Assert.AreEqual(expectedLon, satellite.Pos.Lon);
        }

        [TestCase(2)]
        public void SatelliteMo(int nbTurn)
        {
            var satellite = new Satellite(176400, 7200, 120, 5, 50, 0);

            satellite.Move(nbTurn);

            Assert.AreEqual(0, satellite.CurrentRot.Lat);
            Assert.AreEqual(0, satellite.CurrentRot.Lon);

            Assert.AreEqual(10, satellite.Range.DeltaLatMax);
            Assert.AreEqual(-10, satellite.Range.DeltaLatMin);
            Assert.AreEqual(-10, satellite.Range.DeltaLonMin);
            Assert.AreEqual(10, satellite.Range.DeltaLonMax);
        }

    }
}
