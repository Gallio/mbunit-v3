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
    [TestsOn(typeof(SequentialNumbersGenerator))]
    public class SequentialNumbersGeneratorTest
    {
        private IEnumerable<object[]> GetSequenceValues()
        {
            yield return new object[] { 0, 0, 1, new decimal[] { 0 } };
            yield return new object[] { 1, 1, 1, new decimal[] { 1 } };
            yield return new object[] { 1, 1, 10, new decimal[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } };
            yield return new object[] { 10, -1, 10, new decimal[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 } };
            yield return new object[] { -3.5, 2.26, 6, new decimal[] { -3.5m, -1.24m, 1.02m, 3.28m, 5.54m, 7.8m } };
        }

        [Test]
        [Factory("GetSequenceValues")]
        public void Generate_sequence_ok(decimal start, decimal step, int count, decimal[] expectedOutput)
        {
            var generator = new SequentialNumbersGenerator(start, step, count);
            var actualOutput = generator.Run().Cast<decimal>();
            Assert.AreElementsEqual(expectedOutput, actualOutput, (x, y) => Math.Abs(x - y) < 0.0001m);
        }

        [Test]
        public void Generate_empty_sequence()
        {
            var generator = new SequentialNumbersGenerator(1, 5, 0);
            var actualOutput = generator.Run();
            Assert.IsEmpty(actualOutput);
        }

        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void Constructs_with_negative_count_argument_should_throw_exception()
        {
            new SequentialNumbersGenerator(0, 1, -1);
        }

        private IEnumerable<object[]> GetInvalidStartAndStepValues()
        {
            yield return new object[] { Decimal.MinValue, 10 };
            yield return new object[] { Decimal.MaxValue, 10 };
            yield return new object[] { 10, Decimal.MinValue };
            yield return new object[] { 10, Decimal.MaxValue };
        }

        [Test]
        [ExpectedArgumentException]
        [Factory("GetInvalidStartAndStepValues")]
        public void Constructs_with_invalid_start_or_step_arguments_should_throw_exception(decimal start, decimal step)
        {
            new SequentialNumbersGenerator(start, step, 1);
        }
    }
}
