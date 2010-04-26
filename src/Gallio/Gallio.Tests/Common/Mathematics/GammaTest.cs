using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Common.Mathematics;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Mathematics
{
    [TestFixture]
    public class GammaTest
    {
        [Row(8, 3, 0.046012)]
        [Row(13.6, 7, 0.058771)]
        [Row(9, 5, 0.109064)]
        [Row(237.2512, 222, 0.229857)]
        [Test]
        public void CalculatePValue(double chiSquareValue, int degreesOfFreedom, double expectedProbability)
        {
            double actual = Gamma.IncompleteGamma(degreesOfFreedom / 2d, chiSquareValue / 2d);
            Assert.AreApproximatelyEqual(expectedProbability, actual, 0.00001);
        }
    }
}
