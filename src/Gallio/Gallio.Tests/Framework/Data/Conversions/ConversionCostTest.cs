// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using Gallio.Framework.Data.Conversions;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data.Conversions
{
    [TestFixture]
    [TestsOn(typeof(ConversionCost))]
    public class ConversionCostTest
    {
        [Test]
        public void StandardCosts()
        {
            Assert.AreEqual(0, ConversionCost.Zero.Value);
            Assert.IsFalse(ConversionCost.Zero.IsInvalid);

            Assert.AreEqual(1, ConversionCost.Best.Value);
            Assert.IsFalse(ConversionCost.Best.IsInvalid);

            Assert.AreEqual(1000, ConversionCost.Typical.Value);
            Assert.IsFalse(ConversionCost.Typical.IsInvalid);

            Assert.AreEqual(1000000, ConversionCost.Default.Value);
            Assert.IsFalse(ConversionCost.Default.IsInvalid);

            Assert.AreEqual(1000000000, ConversionCost.Maximum.Value);
            Assert.IsFalse(ConversionCost.Maximum.IsInvalid);

            Assert.AreEqual(int.MaxValue, ConversionCost.Invalid.Value);
            Assert.IsTrue(ConversionCost.Invalid.IsInvalid);
        }

        [Test, ExpectedArgumentOutOfRangeException]
        public void ConstructorThrowsIfValueIsNegative()
        {
            new ConversionCost(-1);
        }

        [Test]
        public void AddingInvalidToACostAlwaysReturnsInvalid()
        {
            Assert.AreEqual(ConversionCost.Invalid, ConversionCost.Invalid.Add(ConversionCost.Invalid));
            Assert.AreEqual(ConversionCost.Invalid, ConversionCost.Zero.Add(ConversionCost.Invalid));
            Assert.AreEqual(ConversionCost.Invalid, ConversionCost.Invalid.Add(ConversionCost.Zero));
            Assert.AreEqual(ConversionCost.Invalid, ConversionCost.Typical.Add(ConversionCost.Invalid));
            Assert.AreEqual(ConversionCost.Invalid, ConversionCost.Invalid.Add(ConversionCost.Typical));
        }

        [Test]
        public void AddingNonInvalidCostsSumsTheirValue()
        {
            Assert.AreEqual(0, ConversionCost.Zero.Add(ConversionCost.Zero).Value);
            Assert.AreEqual(1001, ConversionCost.Best.Add(ConversionCost.Typical).Value);
        }

        [Test]
        public void ToStringIncludesTheValueOfTheCostForDebugging()
        {
            Assert.Contains(ConversionCost.Typical.ToString(), "1000");
        }

        [Test]
        public void CompareToRanksConversionCostsNumerically()
        {
            Assert.LowerThan(0, ConversionCost.Typical.CompareTo(ConversionCost.Best));
            Assert.GreaterThan(0, ConversionCost.Best.CompareTo(ConversionCost.Typical));
            Assert.AreEqual(0, ConversionCost.Best.CompareTo(ConversionCost.Best));
        }
    }
}
