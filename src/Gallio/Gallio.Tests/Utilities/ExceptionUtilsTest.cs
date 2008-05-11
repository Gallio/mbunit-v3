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
using System.Reflection;
using Gallio.Collections;
using Gallio.Framework;
using Gallio.Utilities;
using MbUnit.Framework;

namespace Gallio.Tests.Utilities
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

        public static void ThrowBoom()
        {
            throw new InvalidOperationException("Boom!");
        }

        [Test]
        [ExpectedArgumentNullException]
        public void InvokeMethodWithoutTargetInvocationExceptionShouldThrowIfMethodIsNull()
        {
            ExceptionUtils.InvokeMethodWithoutTargetInvocationException(null, "abc", EmptyArray<object>.Instance);
        }

        [Test]
        public void InvokeMethodWithoutTargetInvocationExceptionShouldNotWrapTheException()
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
    }
}
