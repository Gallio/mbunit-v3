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
using Gallio.Framework;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Mathematics
{
    [TestFixture]
    public class ChiSquareTestTest
    {
        [Test]
        public void ChiSquare()
        {
            var data = new double[]
            {
                432, 452, 465, 459, 426, 429, 460, 429, 424, 478, 419, 468, 448, 476, 444, 462, 438, 447,
                466, 449, 439, 441, 427, 440, 464, 444, 446, 436, 454, 462, 473, 429, 443, 424, 463, 428,
                429, 467, 454, 431, 458, 449, 423, 419, 458, 467, 490, 459, 420, 447, 427, 466, 433, 445,
                444, 457, 436, 476, 453, 450, 461, 459, 452, 448, 435, 479, 434, 457, 441, 441, 423, 416,
                432, 435, 471, 433, 473, 451, 451, 449, 452, 480, 454, 481, 465, 446, 437, 445, 475, 456,
                483, 431, 393, 448, 452, 439, 452, 460, 428, 475, 435, 443, 414, 486, 497, 434, 480, 459,
                472, 458, 420, 412, 433, 379, 431, 505, 418, 458, 509, 452, 461, 423, 477, 469, 471, 472,
                406, 427, 441, 402, 455, 438, 444, 458, 440, 446, 456, 458, 463, 458, 452, 405, 452, 461,
                488, 415, 444, 479, 478, 482, 460, 444, 483, 425, 457, 459, 408, 447, 460, 464, 452, 446,
                431, 435, 468, 436, 467, 430, 475, 425, 460, 453, 442, 457, 457, 427, 438, 463, 454, 443,
                452, 441, 444, 414, 436, 456, 484, 482, 427, 437, 459, 444, 445, 424, 450, 427, 451, 426,
                433, 437, 432, 443, 440, 438, 418, 474, 440, 452, 446, 464, 485, 435, 464, 460, 458, 450,
                464, 488, 425, 443, 437, 432, 426
            };

            Assert.AreEqual(1E5, data.Sum());
            Assert.Count(223, data);
            var test = new ChiSquareTest(1E5 / 223, data, 1);
            Assert.AreEqual(222, test.DegreesOfFreedom);
            Assert.AreApproximatelyEqual(207.677, test.ChiSquareValue, 0.001);
            Assert.AreApproximatelyEqual(0.746, test.TwoTailedPValue, 0.001);
        }
    }
}
