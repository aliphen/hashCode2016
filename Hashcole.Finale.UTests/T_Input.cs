using Hashcode.Qualif;
using NUnit.Framework;

namespace Hashcole.Finale.UTests
{
    [TestFixture]
    public class T_Input
    {

        [Test]
        public void SatelliteMove()
        {
            var satellite = new Satellite(176400, 7200, 120,0,0);

            satellite.Move(1);
            Assert.AreEqual(176520, satellite.CurrentRot.Lat);
        }
    }
}
