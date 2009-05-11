// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Framework.Data.Generation;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data.Generation
{
    [TestFixture]
    [TestsOn(typeof(SequenceDoubleGenerator))]
    public class SequenceDoubleGeneratorTest
    {
        [Test]
        [Row(0, 0, 1, new double[] { 0 })]
        [Row(1, 1, 1, new double[] { 1 })]
        [Row(1, 1, 10, new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })]
        [Row(10, -1, 10, new double[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 })]
        [Row(-3.5, 2.26, 6, new double[] { -3.5, -1.24, 1.02, 3.28, 5.54, 7.8  })]
        public void Generate_sequence_ok(double from, double step, int count, double[] expectedOutput)
        {
            var generator = new SequenceDoubleGenerator(from, step, count);
            var actualOutput = generator.Run().Cast<double>();
            Assert.AreElementsEqual(expectedOutput, actualOutput, (x, y) => Math.Abs(x - y) < 0.0001);
        }

        [Test]
        public void Generate_empty_sequence()
        {
            var generator = new SequenceDoubleGenerator(1, 5, 0);
            var actualOutput = generator.Run();
            Assert.IsEmpty(actualOutput);
        }

        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void Constructs_with_negative_count_argument_should_throw_exception()
        {
            new SequenceDoubleGenerator(0, 1, -1);
        }

        [Test]
        [ExpectedArgumentException]
        [Row(Double.MinValue, 10)]
        [Row(Double.MaxValue, 10)]
        [Row(Double.PositiveInfinity, 10)]
        [Row(Double.NegativeInfinity, 10)]
        [Row(Double.NaN, 10)]
        [Row(10, Double.MinValue)]
        [Row(10, Double.MaxValue)]
        [Row(10, Double.PositiveInfinity)]
        [Row(10, Double.NegativeInfinity)]
        [Row(10, Double.NaN)]
        public void Constructs_with_invalid_from_or_step_arguments_should_throw_exception(double from, double step)
        {
            new SequenceDoubleGenerator(from, step, 1);
        }
    }
}
