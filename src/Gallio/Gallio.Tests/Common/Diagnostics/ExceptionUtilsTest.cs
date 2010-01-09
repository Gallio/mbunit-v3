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
using System.Reflection;
using System.Runtime.CompilerServices;
using Gallio.Common.Collections;
using Gallio.Common.Diagnostics;
using Gallio.Framework;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Diagnostics
{
    [TestFixture]
    [TestsOn(typeof(ExceptionUtils))]
    public class ExceptionUtilsTest
    {
        [Test, ExpectedArgumentNullException]
        public void SafeToStringThrowsIfExceptionIsNull()
        {
            ExceptionUtils.SafeToString(null);
        }

        [Test]
        public void SafeToStringReturnsToStringWhenExceptionCanBePrinted()
        {
            Exception ex = new Exception("Foo");
            Assert.AreEqual(ex.ToString(), ExceptionUtils.SafeToString(ex));
        }

        [Test]
        public void SafeToStringReturnsGenericTextWhenExceptionCannotBePrinted()
        {
            string text = ExceptionUtils.SafeToString(new HostileException());

            Assert.Contains(text, typeof(HostileException).FullName);
            Assert.Contains(text, typeof(RevengeOfTheHostileException).FullName);
        }

        [Test]
        public void RethrowWithNoStackTraceLoss()
        {
            try
            {
                try
                {
                    ThrowBoom();
                }
                catch (InvalidOperationException ex)
                {
                    ExceptionUtils.RethrowWithNoStackTraceLoss(ex);
                }
            }
            catch (InvalidOperationException ex)
            {
                Assert.Contains(ex.StackTrace, "ThrowBoom");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowBoom()
        {
            throw new InvalidOperationException("Boom!");
        }

        public static int Return42()
        {
            return 42;
        }

        [Test]
        [ExpectedArgumentNullException]
        public void InvokeMethodWithoutTargetInvocationException_WhenMethodIsNull_Throws()
        {
            ExceptionUtils.InvokeMethodWithoutTargetInvocationException(null, "abc", EmptyArray<object>.Instance);
        }

        [Test]
        public void InvokeMethodWithoutTargetInvocationException_WhenMethodSucceeds_ReturnsResult()
        {
            MethodInfo method = GetType().GetMethod("Return42");
            int result = (int)ExceptionUtils.InvokeMethodWithoutTargetInvocationException(method, null, null);
            Assert.AreEqual(42, result);
        }

        [Test]
        public void InvokeMethodWithoutTargetInvocationException_WhenMethodThrows_ShouldNotWrapTheException()
        {
            try
            {
                MethodInfo method = GetType().GetMethod("ThrowBoom");
                ExceptionUtils.InvokeMethodWithoutTargetInvocationException(method, null, null);
                Assert.Fail("Should have thrown an InvalidOperationException");
            }
            catch (InvalidOperationException ex)
            {
                Assert.Contains(ex.StackTrace, "ThrowBoom");
            }
        }

        [Test]
        [ExpectedArgumentNullException]
        public void InvokeConstructorWithoutTargetInvocationException_WhenConstructorIsNull_Throws()
        {
            ExceptionUtils.InvokeConstructorWithoutTargetInvocationException(null, EmptyArray<object>.Instance);
        }

        [Test]
        public void InvokeConstructorWithoutTargetInvocationException_WhenConstructorSucceeds_ShouldReturnInstance()
        {
            ConstructorInfo constructor = typeof(Object).GetConstructor(Type.EmptyTypes);
            object instance = ExceptionUtils.InvokeConstructorWithoutTargetInvocationException(constructor, null);
            Assert.IsNotNull(instance);
        }

        [Test]
        public void InvokeConstructorWithoutTargetInvocationException_WhenConstructorThrows_ShouldNotWrapTheException()
        {
            try
            {
                ConstructorInfo constructor = typeof(ThrowBoomWhenConstructed).GetConstructor(Type.EmptyTypes);
                ExceptionUtils.InvokeConstructorWithoutTargetInvocationException(constructor, null);
                Assert.Fail("Should have thrown an InvalidOperationException");
            }
            catch (InvalidOperationException ex)
            {
                Assert.Contains(ex.StackTrace, "ThrowBoom");
            }
        }

        [Test]
        [ExpectedArgumentNullException]
        public void CreateInstanceWithoutTargetInvocationException_WhenTypeIsNull_Throws()
        {
            ExceptionUtils.CreateInstanceWithoutTargetInvocationException(null, EmptyArray<object>.Instance);
        }

        [Test]
        [Row(true)]
        [Row(false)]
        public void CreateInstanceWithoutTargetInvocationException_WhenConstructorSucceeds_ReturnsInstance(bool includeArgs)
        {
            object result = ExceptionUtils.CreateInstanceWithoutTargetInvocationException(typeof(object), includeArgs ? Type.EmptyTypes : null);
            Assert.IsNotNull(result);
        }

        [Test]
        [Row(true)]
        [Row(false)]
        public void CreateInstanceWithoutTargetInvocationException_WhenConstructorThrows_ShouldNotWrapTheException(bool includeArgs)
        {
            try
            {
                ExceptionUtils.CreateInstanceWithoutTargetInvocationException(typeof(ThrowBoomWhenConstructed), includeArgs ? Type.EmptyTypes : null);
                Assert.Fail("Should have thrown an InvalidOperationException");
            }
            catch (InvalidOperationException ex)
            {
                Assert.Contains(ex.StackTrace, "ThrowBoom");
            }
        }

        private class HostileException : Exception
        {
            public override string ToString()
            {
                throw new RevengeOfTheHostileException();
            }
        }

        private class RevengeOfTheHostileException : HostileException
        {
        }

        private class ThrowBoomWhenConstructed
        {
            public ThrowBoomWhenConstructed()
            {
                ThrowBoom();
            }
        }
    }
}