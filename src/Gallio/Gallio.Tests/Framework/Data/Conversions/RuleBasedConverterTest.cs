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
using Gallio.Collections;
using Gallio.Framework.Data.Conversions;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Framework.Data.Conversions
{
    [TestFixture]
    [TestsOn(typeof(RuleBasedConverter))]
    [DependsOn(typeof(BaseConverterTest))]
    public class RuleBasedConverterTest : BaseUnitTest
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
            List<IConversionRule> rules = new List<IConversionRule>();
            RuleBasedConverter converter;

            using (Mocks.Record())
            {
                rules.Add(Mocks.CreateMock<IConversionRule>());
                rules.Add(Mocks.CreateMock<IConversionRule>());
                rules.Add(Mocks.CreateMock<IConversionRule>());

                converter = new RuleBasedConverter(rules);

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
            List<IConversionRule> rules = new List<IConversionRule>();
            RuleBasedConverter converter;

            using (Mocks.Record())
            {
                rules.Add(Mocks.CreateMock<IConversionRule>());
                rules.Add(Mocks.CreateMock<IConversionRule>());

                converter = new RuleBasedConverter(rules);

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
            List<IConversionRule> rules = new List<IConversionRule>();
            RuleBasedConverter converter;

            using (Mocks.Record())
            {
                rules.Add(Mocks.CreateMock<IConversionRule>());
                rules.Add(Mocks.CreateMock<IConversionRule>());

                converter = new RuleBasedConverter(rules);

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
            List<IConversionRule> rules = new List<IConversionRule>();
            RuleBasedConverter converter;

            using (Mocks.Record())
            {
                rules.Add(Mocks.CreateMock<IConversionRule>());

                converter = new RuleBasedConverter(rules);

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
            RuleBasedConverter converter = new RuleBasedConverter(EmptyArray<IConversionRule>.Instance);
            Assert.IsTrue(converter.CanConvert(typeof(int), typeof(int)));
            Assert.IsFalse(converter.CanConvert(typeof(int), typeof(string)));
        }

        [Test]
        public void GetConversionCostAlwaysReturnsZeroIfTypesAreSame()
        {
            RuleBasedConverter converter = new RuleBasedConverter(EmptyArray<IConversionRule>.Instance);
            Assert.AreEqual(ConversionCost.Zero, converter.GetConversionCost(typeof(int), typeof(int)));
            Assert.AreEqual(ConversionCost.Invalid, converter.GetConversionCost(typeof(int), typeof(string)));
        }

        [Test]
        public void ConvertReturnsSameValueIfTypesAreSame()
        {
            RuleBasedConverter converter = new RuleBasedConverter(EmptyArray<IConversionRule>.Instance);
            Assert.AreEqual(42, converter.Convert(42, typeof(int)));
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void ConvertThrowsIfConversionNotSupported()
        {
            RuleBasedConverter converter = new RuleBasedConverter(EmptyArray<IConversionRule>.Instance);
            converter.Convert("42", typeof(int));
        }

        [Test]
        public void NullsRemainNullDuringConversionsToReferenceTypes()
        {
            RuleBasedConverter converter = new RuleBasedConverter(EmptyArray<IConversionRule>.Instance);
            Assert.IsNull(converter.Convert(null, typeof(string)));
        }

        [Test]
        public void NullsRemainNullDuringConversionsToNullableValueTypes()
        {
            RuleBasedConverter converter = new RuleBasedConverter(EmptyArray<IConversionRule>.Instance);
            Assert.IsNull(converter.Convert(null, typeof(int?)));
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void NullsCannotBeConvertedToNonNullableValueTypes()
        {
            RuleBasedConverter converter = new RuleBasedConverter(EmptyArray<IConversionRule>.Instance);
            converter.Convert(null, typeof(int));
        }

        [Test]
        public void NullableTypesAreEquivalentToNonNullableTypesOfSameUnderlyingType()
        {
            RuleBasedConverter converter = new RuleBasedConverter(EmptyArray<IConversionRule>.Instance);
            Assert.AreEqual(ConversionCost.Zero, converter.GetConversionCost(typeof(int?), typeof(int)));
            Assert.AreEqual(ConversionCost.Zero, converter.GetConversionCost(typeof(int), typeof(int?)));
            Assert.AreEqual(ConversionCost.Zero, converter.GetConversionCost(typeof(int?), typeof(int?)));
        }

        [Test]
        public void NullableTargetTypeIsTranslatedBeforeBeingPassedToTheConversionRule()
        {
            List<IConversionRule> rules = new List<IConversionRule>();
            RuleBasedConverter converter;

            using (Mocks.Record())
            {
                rules.Add(Mocks.CreateMock<IConversionRule>());
                converter = new RuleBasedConverter(rules);

                Expect.Call(rules[0].GetConversionCost(typeof(int), typeof(double), converter)).Return(ConversionCost.Best);
            }

            using (Mocks.Playback())
            {
                Assert.AreEqual(ConversionCost.Best, converter.GetConversionCost(typeof(int?), typeof(double)));
                Assert.AreEqual(ConversionCost.Best, converter.GetConversionCost(typeof(int), typeof(double?)));
                Assert.AreEqual(ConversionCost.Best, converter.GetConversionCost(typeof(int?), typeof(double?)));
            }
        }
    }
}
