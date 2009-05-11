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
    [TestsOn(typeof(SequenceInt32Generator))]
    public class SequenceInt32GeneratorTest
    {
        [Test]
        [Row(0, 0, 1, new int[] { 0 })]
        [Row(1, 1, 1, new int[] { 1 })]
        [Row(1, 1, 10, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 })]
        [Row(10, -1, 10, new int[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 })]
        public void Generate_sequence_ok(int from, int step, int count, int[] expectedOutput)
        {
            var generator = new SequenceInt32Generator(from, step, count);
            var actualOutput = generator.Run().Cast<int>();
            Assert.AreElementsEqual(expectedOutput, actualOutput);
        }

        [Test]
        public void Generate_empty_sequence()
        {
            var generator = new SequenceInt32Generator(1, 5, 0);
            var actualOutput = generator.Run();
            Assert.IsEmpty(actualOutput);
        }

        [Test]
        [ExpectedArgumentOutOfRangeException]
        public void Constructs_with_negative_count_argument_should_throw_exception()
        {
            new SequenceInt32Generator(0, 1, -1);
        }

        [Test]
        [ExpectedArgumentException]
        [Row(Int32.MinValue, 10)]
        [Row(Int32.MaxValue, 10)]
        [Row(10, Int32.MinValue)]
        [Row(10, Int32.MaxValue)]
        public void Constructs_with_invalid_from_or_step_arguments_should_throw_exception(int from, int step)
        {
            new SequenceInt32Generator(from, step, 1);
        }
    }
}
