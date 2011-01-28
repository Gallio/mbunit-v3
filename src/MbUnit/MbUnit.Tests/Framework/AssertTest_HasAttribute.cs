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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Gallio.Framework.Assertions;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(Assert))]
	public class AssertTest_HasAttribute : BaseAssertTest
    {
        #region Samples
        
        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        public class FooAttribute : Attribute
        {
            public string Text
            { 
                get;
                private set;
            }

            public FooAttribute(string text)
            {
                Text = text;
            }
        }

        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        public class BarAttribute : Attribute
        {
        }

        [Foo("Hello from type")]
        public class SampleType1
        {
            [Foo("Hello from field")]
            public int sampleField;

            [Foo("Hello from property")]
            public int SampleProperty
            {
                get; 
                set;
            }

            [Foo("Hello from method")]
            public void SampleMethod()
            {
            }
        }

        [Foo("Hello from type #1")]
        [Foo("Hello from type #2")]
        [Foo("Hello from type #3")]
        public class SampleType2
        {
            [Foo("Hello from field #1")]
            [Foo("Hello from field #2")]
            [Foo("Hello from field #3")]
            public int sampleField;

            [Foo("Hello from property #1")]
            [Foo("Hello from property #2")]
            [Foo("Hello from property #3")]
            public int SampleProperty
            {
                get;
                set;
            }

            [Foo("Hello from method #1")]
            [Foo("Hello from method #2")]
            [Foo("Hello from method #3")]
            public void SampleMethod()
            {
            }
        }

        #endregion

        [Test, ExpectedArgumentNullException]
        public void HasAttribute_with_null_expectedAttributeType_should_throw_exception()
        {
            Assert.HasAttribute(null, typeof(SampleType1));
        }

        [Test, ExpectedArgumentException]
        public void HasAttribute_with_invalid_expectedAttributeType_should_throw_exception()
        {
            Assert.HasAttribute(typeof(string), typeof(SampleType1)); // System.String is obviously not an attribute!
        }

        [Test, ExpectedArgumentNullException]
        public void HasAttribute_with_null_MemberInfo_target_should_throw_exception()
        {
            Assert.HasAttribute(typeof(FooAttribute), (MemberInfo)null);
        }

        [Test, ExpectedArgumentNullException]
        public void HasAttribute_with_null_MemberSet_target_should_throw_exception()
        {
            Assert.HasAttribute(typeof(FooAttribute), (Mirror.MemberSet)null);
        }

        private static void AssertCommonFailureLabeledValues(AssertionFailure failure, string target, string attribute)
        {
            Assert.AreEqual("Target Object", failure.LabeledValues[0].Label);
            Assert.AreEqual("MbUnit.Tests.Framework.AssertTest_HasAttribute." + target, failure.LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Attribute Type", failure.LabeledValues[1].Label);
            Assert.AreEqual("MbUnit.Tests.Framework.AssertTest_HasAttribute." + attribute, failure.LabeledValues[1].FormattedValue.ToString());
        }

        private static IEnumerable<object[]> GetMemberInfoFromSampleType(Type type)
        {
            yield return new object[] { type, "Hello from type", String.Empty };
            yield return new object[] { type.GetField("sampleField"), "Hello from field", ", Field:sampleField" };
            yield return new object[] { type.GetProperty("SampleProperty"), "Hello from property", ", Property:SampleProperty" };
            yield return new object[] { type.GetMethod("SampleMethod"), "Hello from method", ", Method:SampleMethod" };
        }

        private IEnumerable<object[]> GetMemberInfoFromSampleType1()
        {
            return GetMemberInfoFromSampleType(typeof(SampleType1));
        }

        private IEnumerable<object[]> GetMemberInfoFromSampleType2()
        {
            return GetMemberInfoFromSampleType(typeof(SampleType2));
        }

        [Test, Factory("GetMemberInfoFromSampleType1")]
        public void HasAttribute_on_reflected_object_should_pass(MemberInfo target, string expectedMessage)
        {
            Attribute attribute = Assert.HasAttribute(typeof(FooAttribute), target);
            Assert.IsInstanceOfType<FooAttribute>(attribute);
            Assert.AreEqual(expectedMessage, ((FooAttribute)attribute).Text);
        }

        [Test, Factory("GetMemberInfoFromSampleType1")]
        public void HasAttribute_on_reflected_object_should_fail(MemberInfo target, string expectedMessage, string expectedTargetFormat)
        {
            AssertionFailure[] failures = Capture(() => Assert.HasAttribute(typeof(BarAttribute), target));
            Assert.Count(1, failures);
            Assert.AreEqual("Expected the searched attribute to decorate the target object; but none was found.", failures[0].Description);
            AssertCommonFailureLabeledValues(failures[0], "SampleType1" + expectedTargetFormat, "BarAttribute");
        }

        [Test, Factory("GetMemberInfoFromSampleType2")]
        public void HasAttribute_on_reflected_object_with_multiple_instances_should_fail(MemberInfo target, string expectedMessage, string expectedTargetFormat)
        {
            AssertionFailure[] failures = Capture(() => Assert.HasAttribute(typeof(FooAttribute), target));
            Assert.Count(1, failures);
            Assert.AreEqual("Expected to find 1 attribute instance but found 3.", failures[0].Description);
            AssertCommonFailureLabeledValues(failures[0], "SampleType2" + expectedTargetFormat, "FooAttribute");
        }

        private static IEnumerable<object[]> GetMemberSetFromSampleType(Type type)
        {
            yield return new object[] { Mirror.ForType(type)["sampleField"], "Hello from field", ", Field:sampleField" };
            yield return new object[] { Mirror.ForType(type)["SampleProperty"], "Hello from property", ", Property:SampleProperty" };
            yield return new object[] { Mirror.ForType(type)["SampleMethod"], "Hello from method", ", Method:SampleMethod" };
        }

        private IEnumerable<object[]> GetMemberSetFromSampleType1()
        {
            return GetMemberSetFromSampleType(typeof(SampleType1));
        }

        private IEnumerable<object[]> GetMemberSetFromSampleType2()
        {
            return GetMemberSetFromSampleType(typeof(SampleType2));
        }

        [Test, Factory("GetMemberSetFromSampleType1")]
        public void HasAttribute_on_mirrored_object_should_pass(Mirror.MemberSet target, string expectedMessage)
        {
            Attribute attribute = Assert.HasAttribute(typeof(FooAttribute), target);
            Assert.IsInstanceOfType<FooAttribute>(attribute);
            Assert.AreEqual(expectedMessage, ((FooAttribute)attribute).Text);
        }

        [Test, Factory("GetMemberSetFromSampleType1")]
        public void HasAttribute_on_mirrored_object_should_fail(Mirror.MemberSet target, string expectedMessage, string expectedTargetFormat)
        {
            AssertionFailure[] failures = Capture(() => Assert.HasAttribute(typeof(BarAttribute), target));
            Assert.Count(1, failures);
            Assert.AreEqual("Expected the searched attribute to decorate the target object; but none was found.", failures[0].Description);
            AssertCommonFailureLabeledValues(failures[0], "SampleType1" + expectedTargetFormat, "BarAttribute");
        }

        [Test, Factory("GetMemberSetFromSampleType2")]
        public void HasAttribute_on_mirrored_object_with_multiple_instances_should_fail(Mirror.MemberSet target, string expectedMessage, string expectedTargetFormat)
        {
            AssertionFailure[] failures = Capture(() => Assert.HasAttribute(typeof(FooAttribute), target));
            Assert.Count(1, failures);
            Assert.AreEqual("Expected to find 1 attribute instance but found 3.", failures[0].Description);
            AssertCommonFailureLabeledValues(failures[0], "SampleType2" + expectedTargetFormat, "FooAttribute");
        }

        [Test, ExpectedArgumentOutOfRangeException]
        public void HasAttributes_with_invalid_expectedCount_should_throw_exception([Column(-1, 0)] int invalidExpectedCount)
        {
            Assert.HasAttributes<FooAttribute>(typeof(SampleType1).GetField("sampleField"), invalidExpectedCount);
        }

        [Test, Factory("GetMemberInfoFromSampleType2")]
        public void HasAttributes_on_reflected_object_should_pass(MemberInfo target, string expectedMessage)
        {
            FooAttribute[] attributes = Assert.HasAttributes<FooAttribute>(target);
            Assert.AreElementsEqualIgnoringOrder(attributes.Select(x => x.Text), Enumerable.Range(1, 3).Select(i => expectedMessage + " #" + i));
        }

        [Test, Factory("GetMemberSetFromSampleType2")]
        public void HasAttributes_on_mirroed_object_should_pass(Mirror.MemberSet target, string expectedMessage)
        {
            FooAttribute[] attributes = Assert.HasAttributes<FooAttribute>(target);
            Assert.AreElementsEqualIgnoringOrder(attributes.Select(x => x.Text), Enumerable.Range(1, 3).Select(i => expectedMessage + " #" + i));
        }

        [Test, Factory("GetMemberInfoFromSampleType2")]
        public void HasAttributes_on_reflected_object_with_exectedCount_should_pass(MemberInfo target, string expectedMessage)
        {
            FooAttribute[] attributes = Assert.HasAttributes<FooAttribute>(target, 3);
            Assert.AreElementsEqualIgnoringOrder(attributes.Select(x => x.Text), Enumerable.Range(1, 3).Select(i => expectedMessage + " #" + i));
        }

        [Test, Factory("GetMemberSetFromSampleType2")]
        public void HasAttributes_on_mirroed_object_with_exectedCount_should_pass(Mirror.MemberSet target, string expectedMessage)
        {
            FooAttribute[] attributes = Assert.HasAttributes<FooAttribute>(target, 3);
            Assert.AreElementsEqualIgnoringOrder(attributes.Select(x => x.Text), Enumerable.Range(1, 3).Select(i => expectedMessage + " #" + i));
        }

        [Test, Factory("GetMemberInfoFromSampleType2")]
        public void HasAttributes_on_reflected_object_with_exectedCount_should_fail(MemberInfo target, string expectedMessage, string expectedTargetFormat)
        {
            AssertionFailure[] failures = Capture(() => Assert.HasAttributes<FooAttribute>(target, 4));
            Assert.Count(1, failures);
            Assert.AreEqual("Expected to find 4 attribute instances but found 3.", failures[0].Description);
            AssertCommonFailureLabeledValues(failures[0], "SampleType2" + expectedTargetFormat, "FooAttribute");
        }

        [Test, Factory("GetMemberSetFromSampleType2")]
        public void HasAttributes_on_mirroed_object_with_exectedCount_should_fail(Mirror.MemberSet target, string expectedMessage, string expectedTargetFormat)
        {
            AssertionFailure[] failures = Capture(() => Assert.HasAttributes<FooAttribute>(target, 4));
            Assert.Count(1, failures);
            Assert.AreEqual("Expected to find 4 attribute instances but found 3.", failures[0].Description);
            AssertCommonFailureLabeledValues(failures[0], "SampleType2" + expectedTargetFormat, "FooAttribute");
        }
    }
}
