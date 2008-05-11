// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using Gallio.Framework.Data;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(Generators))]
    public class GeneratorsTest
    {
        [Test]
        public void LinearInt32()
        {
            CollectionAssert.AreElementsEqual(new int[] { 0, 2, 4, 6 }, Generators.Linear(0, 4, 2));
        }

        [Test, ExpectedArgumentOutOfRangeException]
        public void LinearInt32_ThrowsIfCountLessThanZero()
        {
            Generators.Linear(0, -1, 1);
        }

        [Test]
        public void LinearDouble()
        {
            CollectionAssert.AreElementsEqual(new double[] { -0.5, 2, 4.5, 7 }, Generators.Linear(-0.5, 4, 2.5));
        }

        [Test, ExpectedArgumentOutOfRangeException]
        public void LinearDouble_ThrowsIfCountLessThanZero()
        {
            Generators.Linear(0.0, -1, 1.0);
        }

        [Test]
        public void EnumValues()
        {
            CollectionAssert.AreElementsEqual(new Answer[] { Answer.Yes, Answer.No }, Generators.EnumValues(typeof(Answer)));
        }

        [Test, ExpectedArgumentNullException]
        public void EnumValues_ThrowsIfNull()
        {
            Generators.EnumValues(null);
        }

        [Test, ExpectedArgumentException]
        public void EnumValues_ThrowsIfNotAnEnum()
        {
            Generators.EnumValues(typeof(int));
        }

        private enum Answer
        {
            Yes, No
        }
    }
}