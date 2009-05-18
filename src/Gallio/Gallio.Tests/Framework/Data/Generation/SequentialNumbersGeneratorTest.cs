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
using Gallio.Framework;
using System.Collections;

namespace Gallio.Tests.Framework.Data.Generation
{
    [TestFixture]
    [TestsOn(typeof(SequentialNumbersGenerator))]
    public class SequentialNumbersGeneratorTest
    {
        [Test]
        [Row(0, 0, 0, new double[] { })]
        [Row(0, 0, 1, new double[] { 0 })]
        [Row(1, 1, 1, new double[] { 1 })]
        [Row(1, 1, 10, new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })]
        [Row(10, -1, 10, new double[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 })]
        [Row(-3.5, 2.26, 6, new double[] { -3.5, -1.24, 1.02, 3.28, 5.54, 7.8 })]
        public void Generate_start_step_count_sequence_ok(double start, double step, int count, double[] expectedOutput)
        {
            var generator = new SequentialNumbersGenerator
            {
                Start = start,
                Step = step,
                Count = count
            };

            var actualOutput = generator.Run().Cast<double>();
            Assert.AreElementsEqual(expectedOutput, actualOutput, (x, y) => Math.Abs(x - y) < 0.0001);
        }

        [Test]
        [Row(0, 0, 0, new double[] { })]
        [Row(0, 0, 1, new double[] { 0 })]
        [Row(1, 1, 1, new double[] { 1 })]
        [Row(1, 10, 10, new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })]
        [Row(10, 1, 10, new double[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 })]
        [Row(-3.5, 7.8, 6, new double[] { -3.5, -1.24, 1.02, 3.28, 5.54, 7.8 })]
        public void Generate_start_stop_count_sequence_ok(double start, double stop, int count, double[] expectedOutput)
        {
            var generator = new SequentialNumbersGenerator
            {
                Start = start,
                Stop = stop,
                Count = count
            };

            var actualOutput = generator.Run().Cast<double>();
            Assert.AreElementsEqual(expectedOutput, actualOutput, (x, y) => Math.Abs(x - y) < 0.0001);
        }

        [Test]
        [Row(0, 0, 1, new double[] { 0 })]
        [Row(1, 1, 1, new double[] { 1 })]
        [Row(1, 10, 1, new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })]
        [Row(10, 1, -1, new double[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 })]
        [Row(-3.5, 7.8, 2.26, new double[] { -3.5, -1.24, 1.02, 3.28, 5.54, 7.8 })]
        public void Generate_start_stop_step_sequence_ok(double start, double stop, double step, double[] expectedOutput)
        {
            var generator = new SequentialNumbersGenerator
            {
                Start = start,
                Stop = stop,
                Step = step
            };

            var actualOutput = generator.Run().Cast<double>();
            Assert.AreElementsEqual(expectedOutput, actualOutput, (x, y) => Math.Abs(x - y) < 0.0001);
        }

        [Test]
        [Row(null, null, null, null, Description = "No properties specified")]
        [Row(1, 10, 1, 10, Description = "Too many properties specified")]
        [Row(1, 10, -1, null, Description = "Inconsistent step sign")]
        [Row(1, 10, 0, null, Description = "Zero step")]
        [Row(1, 10, null, -1, Description = "Negative count")]
        [Row(Double.MaxValue, 10, null, 10, Description = "Invalid Value")]
        [Row(Double.MinValue, 10, null, 10, Description = "Invalid Value")]
        [Row(Double.NaN, 10, null, 10, Description = "Invalid Value")]
        [Row(Double.PositiveInfinity, 10, null, 10, Description = "Invalid Value")]
        [Row(Double.NegativeInfinity, 10, null, 10, Description = "Invalid Value")]
        [Row(1, Double.MaxValue, null, 10, Description = "Invalid Value")]
        [Row(1, Double.MinValue, null, 10, Description = "Invalid Value")]
        [Row(1, Double.NaN, null, 10, Description = "Invalid Value")]
        [Row(1, Double.PositiveInfinity, null, 10, Description = "Invalid Value")]
        [Row(1, Double.NegativeInfinity, null, 10, Description = "Invalid Value")]
        [Row(1, 10, Double.MaxValue, null, Description = "Invalid Value")]
        [Row(1, 10, Double.MinValue, null, Description = "Invalid Value")]
        [Row(1, 10, Double.NaN, null, Description = "Invalid Value")]
        [Row(1, 10, Double.PositiveInfinity, null, Description = "Invalid Value")]
        [Row(1, 10, Double.NegativeInfinity, null, Description = "Invalid Value")]
        [Row(1, 10, null, Int32.MaxValue, Description = "Invalid Value")]
        [Row(1, 10, null, Int32.MinValue, Description = "Invalid Value")]
        public void Generate_sequence_with_invalid_property_settings_should_throw_exception(double? start, double? stop, double? step, int? count)
        {
            var generator = new SequentialNumbersGenerator
            {
                Start = start,
                Stop = stop,
                Step = step,
                Count = count
            };

            Assert.Throws<GenerationException>(() => generator.Run().Cast<double>().ToArray());
        }
    }
}
