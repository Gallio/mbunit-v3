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
using Gallio.Common.Collections;
using Gallio.Runtime.Conversions;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.Conversions
{
    [TestFixture]
    [TestsOn(typeof(RuleBasedConverter))]
    [DependsOn(typeof(BaseConverterTest))]
    public class RuleBasedConverterTest : BaseTestWithMocks
    {
        private delegate ConversionCost GetConversionCostDelegate(Type sourceType, Type targetType, IConverter converter);

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsWhenRulesEnumerationIsNull()
        {
            new RuleBasedConverter(null);
        }

        [Test]
        public void GetConversionCostChoosesLeastCostlyRule()
        {
            var rules = new List<IConversionRule>();
            RuleBasedConverter converter;

            using (Mocks.Record())
            {
                rules.Add(Mocks.StrictMock<IConversionRule>());
                rules.Add(Mocks.StrictMock<IConversionRule>());
                rules.Add(Mocks.StrictMock<IConversionRule>());

                converter = new RuleBasedConverter(rules.ToArray());

                SetupResult.For(rules[0].GetConversionCost(typeof(int), typeof(string), converter)).Return(ConversionCost.Best);
                SetupResult.For(rules[1].GetConversionCost(typeof(int), typeof(string), converter)).Return(ConversionCost.Invalid);
                SetupResult.For(rules[2].GetConversionCost(typeof(int), typeof(string), converter)).Return(ConversionCost.Default);
            }

            using (Mocks.Playback())
            {
                Assert.AreEqual(ConversionCost.Best, converter.GetConversionCost(typeof(int), typeof(string)));
            }
        }

        [Test]
        public void CanConvertReturnsTrueIfAndOnlyIfAConversionRuleSupportsTheConversion()
        {
            var rules = new List<IConversionRule>();
            RuleBasedConverter converter;

            using (Mocks.Record())
            {
                rules.Add(Mocks.StrictMock<IConversionRule>());
                rules.Add(Mocks.StrictMock<IConversionRule>());

                converter = new RuleBasedConverter(rules.ToArray());

                SetupResult.For(rules[0].GetConversionCost(typeof(int), typeof(string), converter)).Return(ConversionCost.Invalid);
                SetupResult.For(rules[1].GetConversionCost(typeof(int), typeof(string), converter)).Return(ConversionCost.Invalid);
                SetupResult.For(rules[0].GetConversionCost(typeof(double), typeof(string), converter)).Return(ConversionCost.Default);
                SetupResult.For(rules[1].GetConversionCost(typeof(double), typeof(string), converter)).Return(ConversionCost.Invalid);
            }

            using (Mocks.Playback())
            {
                Assert.IsFalse(converter.CanConvert(typeof(int), typeof(string)));
                Assert.IsTrue(converter.CanConvert(typeof(double), typeof(string)));
            }
        }

        [Test]
        public void ConvertCachesTheConversionSoGetConversionCostIsOnlyCalledOnceForAPairOfTypes()
        {
            var rules = new List<IConversionRule>();
            RuleBasedConverter converter;

            using (Mocks.Record())
            {
                rules.Add(Mocks.StrictMock<IConversionRule>());
                rules.Add(Mocks.StrictMock<IConversionRule>());

                converter = new RuleBasedConverter(rules.ToArray());

                Expect.Call(rules[0].GetConversionCost(typeof(int), typeof(string), converter)).Return(ConversionCost.Invalid);
                Expect.Call(rules[1].GetConversionCost(typeof(int), typeof(string), converter)).Return(ConversionCost.Default);
                Expect.Call(rules[1].Convert(42, typeof(string), converter)).Return("42");
                Expect.Call(rules[1].Convert(53, typeof(string), converter)).Return("53");
            }

            using (Mocks.Playback())
            {
                Assert.AreEqual("42", converter.Convert(42, typeof(string)));
                Assert.AreEqual("53", converter.Convert(53, typeof(string)));
            }
        }

        [Test]
        public void RecursiveConversionsAttemptsAreDenied()
        {
            var rules = new List<IConversionRule>();
            RuleBasedConverter converter;

            using (Mocks.Record())
            {
                rules.Add(Mocks.StrictMock<IConversionRule>());

                converter = new RuleBasedConverter(rules.ToArray());

                Expect.Call(rules[0].GetConversionCost(typeof(int), typeof(string), converter)).Do(
                    (GetConversionCostDelegate)delegate
                    {
                        Assert.AreEqual(ConversionCost.Invalid, converter.GetConversionCost(typeof(int), typeof(string)));
                        return ConversionCost.Best;
                    });
            }

            using (Mocks.Playback())
            {
                Assert.AreEqual(ConversionCost.Best, converter.GetConversionCost(typeof(int), typeof(string)));
            }
        }

        [Test]
        public void CanConvertAlwaysReturnsTrueIfTypesAreSame()
        {
            var converter = new RuleBasedConverter(EmptyArray<IConversionRule>.Instance);
            Assert.IsTrue(converter.CanConvert(typeof(int), typeof(int)));
            Assert.IsFalse(converter.CanConvert(typeof(int), typeof(string)));
        }

        [Test]
        public void GetConversionCostAlwaysReturnsZeroIfTypesAreSame()
        {
            var converter = new RuleBasedConverter(EmptyArray<IConversionRule>.Instance);
            Assert.AreEqual(ConversionCost.Zero, converter.GetConversionCost(typeof(int), typeof(int)));
            Assert.AreEqual(ConversionCost.Invalid, converter.GetConversionCost(typeof(int), typeof(string)));
        }

        [Test]
        public void ConvertReturnsSameValueIfTypesAreSame()
        {
            var converter = new RuleBasedConverter(EmptyArray<IConversionRule>.Instance);
            Assert.AreEqual(42, converter.Convert(42, typeof(int)));
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void ConvertThrowsIfConversionNotSupported()
        {
            var converter = new RuleBasedConverter(EmptyArray<IConversionRule>.Instance);
            converter.Convert("42", typeof(int));
        }

        [Test]
        public void NullsRemainNullDuringConversionsToReferenceTypes()
        {
            var converter = new RuleBasedConverter(EmptyArray<IConversionRule>.Instance);
            Assert.IsNull(converter.Convert(null, typeof(string)));
        }

        [Test]
        public void NullsRemainNullDuringConversionsToNullableValueTypes()
        {
            var converter = new RuleBasedConverter(EmptyArray<IConversionRule>.Instance);
            Assert.IsNull(converter.Convert(null, typeof(int?)));
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void NullsCannotBeConvertedToNonNullableValueTypes()
        {
            var converter = new RuleBasedConverter(EmptyArray<IConversionRule>.Instance);
            converter.Convert(null, typeof(int));
        }

        [Test]
        public void NullableTypesAreEquivalentToNonNullableTypesOfSameUnderlyingType()
        {
            var converter = new RuleBasedConverter(EmptyArray<IConversionRule>.Instance);
            Assert.AreEqual(ConversionCost.Zero, converter.GetConversionCost(typeof(int?), typeof(int)));
            Assert.AreEqual(ConversionCost.Zero, converter.GetConversionCost(typeof(int), typeof(int?)));
            Assert.AreEqual(ConversionCost.Zero, converter.GetConversionCost(typeof(int?), typeof(int?)));
        }

        [Test]
        public void NullableTargetTypeIsTranslatedBeforeBeingPassedToTheConversionRule()
        {
            var rules = new List<IConversionRule>();
            RuleBasedConverter converter;

            using (Mocks.Record())
            {
                rules.Add(Mocks.StrictMock<IConversionRule>());
                converter = new RuleBasedConverter(rules.ToArray());

                Expect.Call(rules[0].GetConversionCost(typeof(int), typeof(double), converter)).Return(ConversionCost.Best);
            }

            using (Mocks.Playback())
            {
                Assert.AreEqual(ConversionCost.Best, converter.GetConversionCost(typeof(int?), typeof(double)));
                Assert.AreEqual(ConversionCost.Best, converter.GetConversionCost(typeof(int), typeof(double?)));
                Assert.AreEqual(ConversionCost.Best, converter.GetConversionCost(typeof(int?), typeof(double?)));
            }
        }

        internal class Foo
        {
            private readonly int value;

            public int Value
            {
                get
                {
                    return value;
                }
            }

            public Foo(int value)
            {
                this.value = value;
            }
        }

        [Test]
        public void Custom_conversion()
        {
            var converter = new RuleBasedConverter(EmptyArray<IConversionRule>.Instance);
            CustomConverters.Register<string, Foo>(x => new Foo(Int32.Parse(x)));

            try
            {
                var actual = (Foo)converter.Convert("123", typeof(Foo));
                Assert.AreEqual(123, actual.Value);
            }
            finally
            {
                CustomConverters.Unregister<string, Foo>();
            }
        }
    }
}
