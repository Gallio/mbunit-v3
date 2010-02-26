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
using Gallio.Common.Collections;
using Gallio.Runtime.Conversions;
using Gallio.Runtime.Extensibility;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.Conversions
{
    [TestFixture]
    [TestsOn(typeof(RuleBasedConverter))]
    [DependsOn(typeof(BaseConverterTest))]
    public class RuleBasedConverterTest
    {
        [Test, ExpectedArgumentNullException]
        public void Constructs_with_null_extensionPoints_should_throw_Exception()
        {
            new RuleBasedConverter(null, EmptyArray<IConversionRule>.Instance);
        }

        [Test, ExpectedArgumentNullException]
        public void Constructs_with_null_rules_should_throw_Exception()
        {
            var mockExtensionPoints = MockRepository.GenerateStub<IExtensionPoints>();
            new RuleBasedConverter(mockExtensionPoints, null);
        }

        [Test]
        public void GetConversionCost_chooses_least_costly_rule()
        {
            var mockRule0 = MockRepository.GenerateStub<IConversionRule>();
            var mockRule1 = MockRepository.GenerateStub<IConversionRule>();
            var mockRule2 = MockRepository.GenerateStub<IConversionRule>();
            var converter = new RuleBasedConverter(new DefaultExtensionPoints(), new[] { mockRule0, mockRule1, mockRule2 });;
            mockRule0.Stub(x => x.GetConversionCost(typeof(int), typeof(string), converter)).Return(ConversionCost.Best);
            mockRule1.Stub(x => x.GetConversionCost(typeof(int), typeof(string), converter)).Return(ConversionCost.Invalid);
            mockRule2.Stub(x => x.GetConversionCost(typeof(int), typeof(string), converter)).Return(ConversionCost.Default);
            Assert.AreEqual(ConversionCost.Best, converter.GetConversionCost(typeof(int), typeof(string)));
        }

        [Test]
        public void CanConvert_returns_true_if_and_only_if_a_conversion_rule_supports_the_conversion()
        {
            var mockRule0 = MockRepository.GenerateStub<IConversionRule>();
            var mockRule1 = MockRepository.GenerateStub<IConversionRule>();
            var converter = new RuleBasedConverter(new DefaultExtensionPoints(), new[] { mockRule0, mockRule1 });;
            mockRule0.Stub(x => x.GetConversionCost(typeof(int), typeof(string), converter)).Return(ConversionCost.Invalid);
            mockRule1.Stub(x => x.GetConversionCost(typeof(int), typeof(string), converter)).Return(ConversionCost.Invalid);
            mockRule0.Stub(x => x.GetConversionCost(typeof(double), typeof(string), converter)).Return(ConversionCost.Default);
            mockRule1.Stub(x => x.GetConversionCost(typeof(double), typeof(string), converter)).Return(ConversionCost.Invalid);
            Assert.IsFalse(converter.CanConvert(typeof(int), typeof(string)));
            Assert.IsTrue(converter.CanConvert(typeof(double), typeof(string)));
        }

        [Test]
        public void Convert_caches_the_conversion_so_GetConversionCost_is_only_called_once_for_a_pair_of_types()
        {
            var mockRule0 = MockRepository.GenerateMock<IConversionRule>();
            var mockRule1 = MockRepository.GenerateMock<IConversionRule>();
            var converter = new RuleBasedConverter(new DefaultExtensionPoints(), new[] { mockRule0, mockRule1 });
            mockRule0.Expect(x => x.GetConversionCost(typeof(int), typeof(string), converter)).Return(ConversionCost.Invalid);
            mockRule1.Expect(x => x.GetConversionCost(typeof(int), typeof(string), converter)).Return(ConversionCost.Default);
            mockRule1.Expect(x => x.Convert(42, typeof(string), converter)).Return("42");
            mockRule1.Expect(x => x.Convert(53, typeof(string), converter)).Return("53");
            Assert.AreEqual("42", converter.Convert(42, typeof(string)));
            Assert.AreEqual("53", converter.Convert(53, typeof(string)));
            mockRule0.VerifyAllExpectations();
            mockRule1.VerifyAllExpectations();
        }

        private delegate ConversionCost GetConversionCostDelegate(Type sourceType, Type targetType, IConverter converter);

        [Test]
        public void Recursive_conversions_attempts_are_denied()
        {
            var mockRule = MockRepository.GenerateMock<IConversionRule>();
            var converter = new RuleBasedConverter(new DefaultExtensionPoints(), new[] { mockRule });
            mockRule.Expect(x => x.GetConversionCost(typeof(int), typeof(string), converter)).Do((GetConversionCostDelegate)delegate
            {
                Assert.AreEqual(ConversionCost.Invalid, converter.GetConversionCost(typeof(int), typeof(string)));
                return ConversionCost.Best;
            });

            Assert.AreEqual(ConversionCost.Best, converter.GetConversionCost(typeof(int), typeof(string)));
            mockRule.VerifyAllExpectations();
        }

        [Test]
        public void CanConvert_always_returns_true_if_types_are_same()
        {
            var converter = new RuleBasedConverter(new DefaultExtensionPoints(), EmptyArray<IConversionRule>.Instance);
            Assert.IsTrue(converter.CanConvert(typeof(int), typeof(int)));
            Assert.IsFalse(converter.CanConvert(typeof(int), typeof(string)));
        }

        [Test]
        public void GetConversionCost_always_returns_zero_if_types_are_same()
        {
            var converter = new RuleBasedConverter(new DefaultExtensionPoints(), EmptyArray<IConversionRule>.Instance);
            Assert.AreEqual(ConversionCost.Zero, converter.GetConversionCost(typeof(int), typeof(int)));
            Assert.AreEqual(ConversionCost.Invalid, converter.GetConversionCost(typeof(int), typeof(string)));
        }

        [Test]
        public void Convert_returns_same_value_if_types_are_same()
        {
            var converter = new RuleBasedConverter(new DefaultExtensionPoints(), EmptyArray<IConversionRule>.Instance);
            Assert.AreEqual(42, converter.Convert(42, typeof(int)));
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void Convert_throws_if_conversion_not_supported()
        {
            var converter = new RuleBasedConverter(new DefaultExtensionPoints(), EmptyArray<IConversionRule>.Instance);
            converter.Convert("42", typeof(int));
        }

        [Test]
        public void Nulls_remain_null_during_conversions_to_reference_types()
        {
            var converter = new RuleBasedConverter(new DefaultExtensionPoints(), EmptyArray<IConversionRule>.Instance);
            Assert.IsNull(converter.Convert(null, typeof(string)));
        }

        [Test]
        public void Nulls_remain_rull_during_conversions_to_nullable_ValueTypes()
        {
            var converter = new RuleBasedConverter(new DefaultExtensionPoints(), EmptyArray<IConversionRule>.Instance);
            Assert.IsNull(converter.Convert(null, typeof(int?)));
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void Nulls_cannot_be_converted_to_non_nullable_ValueTypes()
        {
            var converter = new RuleBasedConverter(new DefaultExtensionPoints(), EmptyArray<IConversionRule>.Instance);
            converter.Convert(null, typeof(int));
        }

        [Test]
        public void Nullable_types_are_equivalent_to_non_nullable_types_of_same_underlying_type()
        {
            var converter = new RuleBasedConverter(new DefaultExtensionPoints(), EmptyArray<IConversionRule>.Instance);
            Assert.AreEqual(ConversionCost.Zero, converter.GetConversionCost(typeof(int?), typeof(int)));
            Assert.AreEqual(ConversionCost.Zero, converter.GetConversionCost(typeof(int), typeof(int?)));
            Assert.AreEqual(ConversionCost.Zero, converter.GetConversionCost(typeof(int?), typeof(int?)));
        }

        [Test]
        public void Nullable_target_type_is_translated_before_being_passed_to_the_conversion_rule()
        {
            var mockRule = MockRepository.GenerateMock<IConversionRule>();
            var converter = new RuleBasedConverter(new DefaultExtensionPoints(), new[] { mockRule });
            mockRule.Expect(x => x.GetConversionCost(typeof(int), typeof(double), converter)).Return(ConversionCost.Best);
            Assert.AreEqual(ConversionCost.Best, converter.GetConversionCost(typeof(int?), typeof(double)));
            Assert.AreEqual(ConversionCost.Best, converter.GetConversionCost(typeof(int), typeof(double?)));
            Assert.AreEqual(ConversionCost.Best, converter.GetConversionCost(typeof(int?), typeof(double?)));
            mockRule.VerifyAllExpectations();
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
            var extensionPoints = new DefaultExtensionPoints();
            extensionPoints.CustomConverters.Register<string, Foo>(x => new Foo(Int32.Parse(x)));
            var converter = new RuleBasedConverter(extensionPoints, EmptyArray<IConversionRule>.Instance);
            var actual = (Foo)converter.Convert("123", typeof(Foo));
            Assert.AreEqual(123, actual.Value);
        }
    }
}
