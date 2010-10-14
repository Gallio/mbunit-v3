// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
