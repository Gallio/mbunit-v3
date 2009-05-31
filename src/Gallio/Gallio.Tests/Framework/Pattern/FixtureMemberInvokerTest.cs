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
using Gallio.Framework;
using Gallio.Framework.Pattern;
using MbUnit.Framework;
using Rhino.Mocks;
using Gallio.Common.Reflection;

#pragma warning disable 0414
namespace Gallio.Tests.Framework.Pattern
{
    [TestsOn(typeof(FixtureMemberInvoker<>))]
    public class FixtureMemberInvokerTest
    {
        private IPatternScope ArrangeMockScope()
        {
            var mockScope = MockRepository.GenerateStub<IPatternScope>();
            var mockTestBuilder = MockRepository.GenerateStub<ITestBuilder>();
            mockScope.Stub(x => x.TestBuilder).Return(mockTestBuilder);
            mockTestBuilder.Stub(x => x.CodeElement).Return(Reflector.Wrap(GetType()));
            return mockScope;
        }

        [Test]
        [Row(typeof(int), "PublicInstancePropertyInt32", true, 123)]
        [Row(typeof(int), "PrivateInstancePropertyInt32", true, 123)]
        [Row(typeof(int), "PublicInstanceMethodInt32", true, 123)]
        [Row(typeof(int), "PrivateInstanceMethodInt32", true, 123)]
        [Row(typeof(int), "PublicInstanceFieldInt32", true, 123)]
        [Row(typeof(int), "PrivateInstanceFieldInt32", true, 123)]
        [Row(typeof(int), "PublicStaticPropertyInt32", true, 123)]
        [Row(typeof(int), "PrivateStaticPropertyInt32", true, 123)]
        [Row(typeof(int), "PublicStaticMethodInt32", true, 123)]
        [Row(typeof(int), "PrivateStaticMethodInt32", true, 123)]
        [Row(typeof(int), "PublicStaticFieldInt32", true, 123)]
        [Row(typeof(int), "PrivateStaticFieldInt32", true, 123)]
        [Row(typeof(int), "PublicStaticPropertyInt32", false, 123)]
        [Row(typeof(int), "PrivateStaticPropertyInt32", false, 123)]
        [Row(typeof(int), "PublicStaticMethodInt32", false, 123)]
        [Row(typeof(int), "PrivateStaticMethodInt32", false, 123)]
        [Row(typeof(int), "PublicStaticFieldInt32", false, 123)]
        [Row(typeof(int), "PrivateStaticFieldInt32", false, 123)]
        [Row(typeof(string), "PublicInstancePropertyString", true, "Hello")]
        [Row(typeof(string), "PrivateInstancePropertyString", true, "Hello")]
        [Row(typeof(string), "PublicInstanceMethodString", true, "Hello")]
        [Row(typeof(string), "PrivateInstanceMethodString", true, "Hello")]
        [Row(typeof(string), "PublicInstanceFieldString", true, "Hello")]
        [Row(typeof(string), "PrivateInstanceFieldString", true, "Hello")]
        [Row(typeof(string), "PrivateStaticFieldString", true, "Hello")]
        [Row(typeof(string), "PublicStaticPropertyString", true, "Hello")]
        [Row(typeof(string), "PrivateStaticPropertyString", true, "Hello")]
        [Row(typeof(string), "PublicStaticMethodString", true, "Hello")]
        [Row(typeof(string), "PrivateStaticMethodString", true, "Hello")]
        [Row(typeof(string), "PublicStaticFieldString", true, "Hello")]
        [Row(typeof(string), "PrivateStaticFieldString", false, "Hello")]
        [Row(typeof(string), "PublicStaticPropertyString", false, "Hello")]
        [Row(typeof(string), "PrivateStaticPropertyString", false, "Hello")]
        [Row(typeof(string), "PublicStaticMethodString", false, "Hello")]
        [Row(typeof(string), "PrivateStaticMethodString", false, "Hello")]
        [Row(typeof(string), "PublicStaticFieldString", false, "Hello")]
        public void Invoke_argument_less_member<T>(string memberName, bool fromFixture, T expectedValue)
        {
            var mockScope = ArrangeMockScope();
            Type type = fromFixture ? null : typeof(Sample);
            var invoker = new FixtureMemberInvoker<T>(type, mockScope, memberName);
            T actualValue = invoker.Invoke();
            Assert.AreEqual<T>(expectedValue, actualValue);
        }

        [Test]
        [Row(123, false)]
        [Row(124, true)]
        public void Invoke_member_with_argument(int input, bool expectedResult)
        {
            var mockScope = ArrangeMockScope();
            var invoker = new FixtureMemberInvoker<bool>(null, mockScope, "IsOdd");
            bool actualResult = invoker.Invoke(input);
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        [Row(123.3d, false)]
        [Row(123.7d, true)]
        [Row(124.1d, true)]
        [Row(124.6d, false)]
        public void Invoke_member_with_converted_argument(double input, bool expectedResult)
        {
            var mockScope = ArrangeMockScope();
            var invoker = new FixtureMemberInvoker<bool>(null, mockScope, "IsOdd");
            bool actualResult = invoker.Invoke(input);
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void Invoke_member_not_found_should_throw_exception()
        {
            var mockScope = ArrangeMockScope();
            var invoker = new FixtureMemberInvoker<bool>(null, mockScope, "DoesNotExist");
            Assert.Throws<PatternUsageErrorException>(() => invoker.Invoke());
        }

        [Test]
        public void Invoke_member_with_invalid_number_of_arguments_should_throw_exception()
        {
            var mockScope = ArrangeMockScope();
            var invoker = new FixtureMemberInvoker<bool>(null, mockScope, "Add");
            Assert.Throws<PatternUsageErrorException>(() => invoker.Invoke(1, 2));
        }

        [Test]
        public void Invoke_member_with_incompatible_argument_should_throw_exception()
        {
            var mockScope = ArrangeMockScope();
            var invoker = new FixtureMemberInvoker<bool>(null, mockScope, "Add");
            Assert.Throws<PatternUsageErrorException>(() => invoker.Invoke(1, new object(), 3));
        }

        #region Invocation samples

        public int PublicInstanceFieldInt32 = 123;
        public string PublicInstanceFieldString = "Hello";
        #pragma warning disable 0414
        private int PrivateInstanceFieldInt32 = 123;
        private string PrivateInstanceFieldString = "Hello";
        public static int PublicStaticFieldInt32 = 123;
        public static string PublicStaticFieldString = "Hello";
        private static int PrivateStaticFieldInt32 = 123;
        private static string PrivateStaticFieldString = "Hello";

        public int PublicInstancePropertyInt32
        {
            get
            {
                return 123;
            }
        }

        public string PublicInstancePropertyString
        {
            get
            {
                return "Hello";
            }
        }

        private int PrivateInstancePropertyInt32
        {
            get
            {
                return 123;
            }
        }

        private string PrivateInstancePropertyString
        {
            get
            {
                return "Hello";
            }
        }

        public static int PublicStaticPropertyInt32
        {
            get
            {
                return 123;
            }
        }

        public static string PublicStaticPropertyString
        {
            get
            {
                return "Hello";
            }
        }

        private static int PrivateStaticPropertyInt32
        {
            get
            {
                return 123;
            }
        }

        private static string PrivateStaticPropertyString
        {
            get
            {
                return "Hello";
            }
        }

        public int PublicInstanceMethodInt32()
        {
            return 123;
        }

        public string PublicInstanceMethodString()
        {
            return "Hello";
        }

        private int PrivateInstanceMethodInt32()
        {
            return 123;
        }

        private string PrivateInstanceMethodString()
        {
            return "Hello";
        }

        public static int PublicStaticMethodInt32()
        {
            return 123;
        }

        public static string PublicStaticMethodString()
        {
            return "Hello";
        }

        private static int PrivateStaticMethodInt32()
        {
            return 123;
        }

        private static string PrivateStaticMethodString()
        {
            return "Hello";
        }

        public bool IsOdd(int value)
        {
            return value % 2 == 0;
        }

        public int Add(int value1, int value2, int value3)
        {
            return value1 + value2 + value3;
        }

        private class Sample
        {
            public static int PublicStaticFieldInt32 = 123;
            public static string PublicStaticFieldString = "Hello";
            private static int PrivateStaticFieldInt32 = 123;
            private static string PrivateStaticFieldString = "Hello";

            public static int PublicStaticPropertyInt32
            {
                get
                {
                    return 123;
                }
            }

            public static string PublicStaticPropertyString
            {
                get
                {
                    return "Hello";
                }
            }

            private static int PrivateStaticPropertyInt32
            {
                get
                {
                    return 123;
                }
            }

            private static string PrivateStaticPropertyString
            {
                get
                {
                    return "Hello";
                }
            }

            public static int PublicStaticMethodInt32()
            {
                return 123;
            }

            public static string PublicStaticMethodString()
            {
                return "Hello";
            }

            private static int PrivateStaticMethodInt32()
            {
                return 123;
            }

            private static string PrivateStaticMethodString()
            {
                return "Hello";
            }
        }

        #endregion
    }
}
