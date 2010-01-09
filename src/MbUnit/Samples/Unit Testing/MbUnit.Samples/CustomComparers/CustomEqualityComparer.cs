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
using System.Text;
using MbUnit.Framework;

namespace MbUnit.Samples.CustomEqualityComparers
{
    public class NonEquatable
    {
        private readonly string text;

        public string Text
        {
            get
            {
                return text;
            }
        }

        public NonEquatable(string text)
        {
            this.text = text;
        }
    }

    [TestFixture]
    public class CustomEqualityComparerTest
    {
        [EqualityComparer]
        public static bool Equals(NonEquatable x, NonEquatable y)
        {
            // The inner comparison engine of Gallio handles with the cases
            // where x or y is null. Therefore we can safely assume than x
            // and y are never null here.

            return x.Text.Equals(y.Text, StringComparison.OrdinalIgnoreCase); // Custom equality logic.
        }

        [Test]
        public void Test()
        {
            var foo1 = new NonEquatable("Hello World!");
            var foo2 = new NonEquatable("HELLO WORLD!");
            var foo3 = new NonEquatable("Goodbye World!");
            Assert.AreEqual(foo1, foo2); // The assertions will use the custom comparer defined above.
            Assert.AreNotEqual(foo1, foo3);
        }
    }
}