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

using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Framework.Data.Conversions;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data.Conversions
{
    [TestFixture]
    [TestsOn(typeof(NullConverter))]
    [DependsOn(typeof(BaseConverterTest))]
    public class NullConverterTest
    {
        [Test]
        public void CanConvertReturnsFalseIfConversionRequired()
        {
            Assert.IsFalse(new NullConverter().CanConvert(typeof(int), typeof(string)));
        }

        [Test]
        public void CanConvertReturnsTrueIfNoConversionRequired()
        {
            Assert.IsTrue(new NullConverter().CanConvert(typeof(int), typeof(int)));
        }

        [Test]
        public void GetConversionCostReturnsInvalidIfConversionRequired()
        {
            Assert.AreEqual(ConversionCost.Invalid, new NullConverter().GetConversionCost(typeof(int), typeof(string)));
        }

        [Test]
        public void GetConversionCostReturnsZeroIfNoConversionRequired()
        {
            Assert.AreEqual(ConversionCost.Zero, new NullConverter().GetConversionCost(typeof(int), typeof(int)));
        }

        [Test]
        public void ConvertReturnsSourceValueIsNoConversionRequired()
        {
            Assert.AreEqual(42, new NullConverter().Convert(42, typeof(int)));
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void ConvertThrowsOperationInvalidExceptionIfConversionRequired()
        {
            new NullConverter().Convert("42", typeof(int));
        }
    }
}
