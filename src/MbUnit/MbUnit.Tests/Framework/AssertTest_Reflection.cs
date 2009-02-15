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
using System.Linq;
using System.Text;
using Gallio.Framework.Assertions;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(Assert))]
    public class AssertTest_Reflection : BaseAssertTest
    {
        #region IsAssignableFrom

        [Test]
        public void IsAssignableFrom_without_custom_message()
        {
            Assert.IsAssignableFrom(typeof(FormatException), new SystemException());
        }

        [Test]
        public void Generic_IsAssignableFrom_without_custom_message()
        {
            Assert.IsAssignableFrom<FormatException>(new SystemException());
        }

        [Test]
        [ExpectedArgumentNullException]
        public void IsAssignableFrom_with_null_expectedType()
        {
            Assert.IsAssignableFrom(null, new SystemException());
        }

        [Test]
        public void IsAssignableFrom_with_null_actualValue()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsAssignableFrom(typeof(int), null));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the actual type to be assignable to the expected type.", failures[0].Description);
            Assert.AreEqual("Expected Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("int", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Value", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("null", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void Generic_IsAssignableFrom_with_null_actualValue()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsAssignableFrom<int>(null));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the actual type to be assignable to the expected type.", failures[0].Description);
            Assert.AreEqual("Expected Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("int", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Value", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("null", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void IsAssignableFrom_fails_when_object_is_not_assignable_for_classes()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsAssignableFrom(typeof(string), new Int32()));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the actual type to be assignable to the expected type.", failures[0].Description);
            Assert.AreEqual("Expected Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("string", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Type", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("int", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void IsAssignableFrom_fails_when_object_is_not_assignable_for_arrays()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsAssignableFrom(typeof(int[,]), new int[2]));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the actual type to be assignable to the expected type.", failures[0].Description);
            Assert.AreEqual("Expected Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("int[,]", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Type", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("int[]", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void IsAssignableFrom_fails_when_object_is_not_assignable_for_generics()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsAssignableFrom(typeof(List<int>), new List<Type>()));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("System.Collections.Generic.List<int>", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("System.Collections.Generic.List<System.Type>", failures[0].LabeledValues[1].FormattedValue.ToString());
        }
        
        [Test]
        public void Generic_IsAssignableFrom_fails_when_object_is_not_assignable_for_classes()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsAssignableFrom<string>(new Int32()));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the actual type to be assignable to the expected type.", failures[0].Description);
            Assert.AreEqual("Expected Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("string", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Type", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("int", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void Generic_IsAssignableFrom_fails_when_object_is_not_assignable_for_arrays()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsAssignableFrom<int[,]>(new int[2]));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the actual type to be assignable to the expected type.", failures[0].Description);
            Assert.AreEqual("Expected Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("int[,]", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Type", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("int[]", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void Generic_IsAssignableFrom_fails_when_object_is_not_assignable_for_generics()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsAssignableFrom<List<int>>(new List<Type>()));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("System.Collections.Generic.List<int>", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("System.Collections.Generic.List<System.Type>", failures[0].LabeledValues[1].FormattedValue.ToString());
        }
        
        #endregion

        #region IsNotAssignableFrom

        [Test]
        public void IsNotAssignableFrom_without_custom_message()
        {
            Assert.IsNotAssignableFrom(typeof(string), new Int32());
        }

        [Test]
        public void Generic_IsNotAssignableFrom_without_custom_message()
        {
            Assert.IsNotAssignableFrom<string>(new Int32());
        }

        [Test]
        [ExpectedArgumentNullException]
        public void IsNotAssignableFrom_with_null_expectedType()
        {
            Assert.IsNotAssignableFrom(null, new SystemException());
        }

        [Test]
        public void IsNotAssignableFrom_with_null_actualValue()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsNotAssignableFrom(typeof(int), null));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the actual type not to be assignable to the expected type.", failures[0].Description);
            Assert.AreEqual("Unexpected Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("int", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Value", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("null", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void Generic_IsNotAssignableFrom_with_null_actualValue()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsNotAssignableFrom<int>(null));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the actual type not to be assignable to the expected type.", failures[0].Description);
            Assert.AreEqual("Unexpected Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("int", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Value", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("null", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void IsNotAssignableFrom_fails_when_object_is_not_assignable_for_classes()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsNotAssignableFrom(typeof(int), new Int32()));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the actual type not to be assignable to the expected type.", failures[0].Description);
            Assert.AreEqual("Unexpected Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("int", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Type", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("int", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void IsNotAssignableFrom_fails_when_object_is_not_assignable_for_arrays()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsNotAssignableFrom(typeof(int[,]), new int[2, 2]));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the actual type not to be assignable to the expected type.", failures[0].Description);
            Assert.AreEqual("Unexpected Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("int[,]", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Type", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("int[,]", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void Generic_IsNotAssignableFrom_fails_when_object_is_not_assignable_for_classes()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsNotAssignableFrom<int>(new Int32()));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the actual type not to be assignable to the expected type.", failures[0].Description);
            Assert.AreEqual("Unexpected Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("int", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Type", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("int", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void Generic_IsNotAssignableFrom_fails_when_object_is_not_assignable_for_arrays()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsNotAssignableFrom<int[,]>(new int[2, 2]));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the actual type not to be assignable to the expected type.", failures[0].Description);
            Assert.AreEqual("Unexpected Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("int[,]", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Type", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("int[,]", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        #endregion

        #region IsInstanceOfType

        [Test]
        [ExpectedArgumentNullException]
        public void IsInstanceOfType_with_null_expectedType()
        {
            Assert.IsInstanceOfType(null, new Object());
        }

        [Test]
        public void IsInstanceOfType_without_custom_message()
        {
            Assert.IsInstanceOfType(typeof(string), "Hello");
        }

        [Test]
        public void Generic_IsInstanceOfType_without_custom_message()
        {
            Assert.IsInstanceOfType<string>("Hello");
        }

        [Test]
        [ExpectedArgumentNullException]
        public void IsInstanceOfType_with_custom_message_and_null_expectedType()
        {
            Assert.IsInstanceOfType(null, new Object(), "Message With Some {0}", "Argument");
        }

        [Test]
        public void Generic_IsInstanceOfType_with_custom_message()
        {
            Assert.IsInstanceOfType<string>("Hello", "Message With Some {0}", "Argument");
        }

        [Test]
        public void Generic_IsInstanceOfType_with_custom_message_failed()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsInstanceOfType<string>(123));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected value to be an instance of a particular type.", failures[0].Description);
            Assert.AreEqual("Expected Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("string", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Type", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("int", failures[0].LabeledValues[1].FormattedValue.ToString());
            Assert.AreEqual("Actual Value", failures[0].LabeledValues[2].Label);
            Assert.AreEqual("123", failures[0].LabeledValues[2].FormattedValue.ToString());
        }

        [Test]
        public void IsInstanceOfType_with_custom_message_failed()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsInstanceOfType(typeof(string), 123));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected value to be an instance of a particular type.", failures[0].Description);
            Assert.AreEqual("Expected Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("string", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Type", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("int", failures[0].LabeledValues[1].FormattedValue.ToString());
            Assert.AreEqual("Actual Value", failures[0].LabeledValues[2].Label);
            Assert.AreEqual("123", failures[0].LabeledValues[2].FormattedValue.ToString());
        }

        #endregion
        
        #region IsNotInstanceOfType

        [Test]
        [ExpectedArgumentNullException]
        public void IsNotInstanceOfType_with_null_expectedType()
        {
            Assert.IsNotInstanceOfType(null, new Object());
        }

        [Test]
        public void IsNotInstanceOfType_without_custom_message()
        {
            Assert.IsNotInstanceOfType(typeof(int), "Hello");
        }

        [Test]
        public void Generic_IsNotInstanceOfType_without_custom_message()
        {
            Assert.IsNotInstanceOfType<int>("Hello");
        }

        [Test]
        [ExpectedArgumentNullException]
        public void IsNotInstanceOfType_with_custom_message_and_null_expectedType()
        {
            Assert.IsNotInstanceOfType(null, new Object(), "Message With Some {0}", "Argument");
        }

        [Test]
        public void Generic_IsNotInstanceOfType_with_custom_message()
        {
            Assert.IsNotInstanceOfType<int>("Hello", "Message With Some {0}", "Argument");
        }

        [Test]
        public void Generic_IsNotInstanceOfType_with_custom_message_failed()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsNotInstanceOfType<int>(123));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected value to not be an instance of a particular type.", failures[0].Description);
            Assert.AreEqual("Unexpected Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("int", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Type", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("int", failures[0].LabeledValues[1].FormattedValue.ToString());
            Assert.AreEqual("Actual Value", failures[0].LabeledValues[2].Label);
            Assert.AreEqual("123", failures[0].LabeledValues[2].FormattedValue.ToString());
        }

        [Test]
        public void IsNotInstanceOfType_with_custom_message_failed()
        {
            AssertionFailure[] failures = Capture(() => Assert.IsNotInstanceOfType(typeof(int), 123));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected value to not be an instance of a particular type.", failures[0].Description);
            Assert.AreEqual("Unexpected Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("int", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Type", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("int", failures[0].LabeledValues[1].FormattedValue.ToString());
            Assert.AreEqual("Actual Value", failures[0].LabeledValues[2].Label);
            Assert.AreEqual("123", failures[0].LabeledValues[2].FormattedValue.ToString());
        }

        #endregion
    }
}
