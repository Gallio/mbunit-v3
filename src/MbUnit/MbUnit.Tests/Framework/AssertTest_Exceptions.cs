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
using Gallio.Framework.Assertions;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(Assert))]
    public class AssertTest_Exceptions : BaseAssertTest
    {
        [Test]
        public void Throws_throws_if_arguments_invalid()
        {
            Assert.Throws<ArgumentNullException>(() => Assert.Throws<Exception>(null));
            Assert.Throws<ArgumentNullException>(() => Assert.Throws<Exception>(null, ""));
            Assert.Throws<ArgumentNullException>(() => Assert.Throws(null, () => { }));
            Assert.Throws<ArgumentNullException>(() => Assert.Throws(null, () => { }, ""));
            Assert.Throws<ArgumentNullException>(() => Assert.Throws(typeof(Exception), null));
            Assert.Throws<ArgumentNullException>(() => Assert.Throws(typeof(Exception), null, ""));
        }

        [Test]
        public void Throws_passes_and_returns_exception_when_expected_exception_occurs()
        {
            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => { throw new InvalidOperationException("Exception"); });
            Assert.IsNotNull(ex);
            Assert.AreEqual("Exception", ex.Message);
        }

        [Test]
        public void Throws_with_message_passes_and_returns_exception_when_expected_exception_occurs()
        {
            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => { throw new InvalidOperationException("Exception"); }, "Foo");
            Assert.IsNotNull(ex);
            Assert.AreEqual("Exception", ex.Message);
        }

        [Test]
        public void Throws_passes_and_returns_exception_when_subtype_of_expected_exception_occurs()
        {
            Exception ex = Assert.Throws<Exception>(() => { throw new InvalidOperationException("Exception"); });
            Assert.IsNotNull(ex);
            Assert.AreEqual("Exception", ex.Message);
        }

        [Test]
        public void Throws_with_message_passes_and_returns_exception_when_subtype_of_expected_exception_occurs()
        {
            Exception ex = Assert.Throws<Exception>(() => { throw new InvalidOperationException("Exception"); }, "Foo");
            Assert.IsNotNull(ex);
            Assert.AreEqual("Exception", ex.Message);
        }

        [Test]
        public void Throws_with_type_passes_and_returns_exception_when_expected_exception_occurs()
        {
            Exception ex = Assert.Throws(typeof(InvalidOperationException), () => { throw new InvalidOperationException("Exception"); });
            Assert.IsNotNull(ex);
            Assert.AreEqual("Exception", ex.Message);
        }

        [Test]
        public void Throws_with_type_with_message_passes_and_returns_exception_when_expected_exception_occurs()
        {
            Exception ex = Assert.Throws(typeof(InvalidOperationException), () => { throw new InvalidOperationException("Exception"); }, "Foo");
            Assert.IsNotNull(ex);
            Assert.AreEqual("Exception", ex.Message);
        }

        [Test]
        public void Throws_with_type_passes_and_returns_exception_when_subtype_of_expected_exception_occurs()
        {
            Exception ex = Assert.Throws(typeof(Exception), () => { throw new InvalidOperationException("Exception"); });
            Assert.IsNotNull(ex);
            Assert.AreEqual("Exception", ex.Message);
        }

        [Test]
        public void Throws_with_type_with_message_passes_and_returns_exception_when_subtype_of_expected_exception_occurs()
        {
            Exception ex = Assert.Throws(typeof(Exception), () => { throw new InvalidOperationException("Exception"); }, "Foo");
            Assert.IsNotNull(ex);
            Assert.AreEqual("Exception", ex.Message);
        }

        [Test]
        public void Throws_fails_if_no_exception_was_thrown()
        {
            AssertionFailure[] failures = Capture(()
                => Assert.Throws<InvalidOperationException>(() => { }));
            Assert.AreEqual(1, failures.Length);
            Assert.IsNull(failures[0].Message);
            Assert.AreEqual("Expected the block to throw an exception.", failures[0].Description);
            Assert.AreEqual(0, failures[0].Exceptions.Count);
        }

        [Test]
        public void Throws_with_message_fails_if_no_exception_was_thrown()
        {
            AssertionFailure[] failures = Capture(()
                => Assert.Throws<InvalidOperationException>(() => { }, "Hello {0}", "World"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Hello World", failures[0].Message);
            Assert.AreEqual("Expected the block to throw an exception.", failures[0].Description);
            Assert.AreEqual(0, failures[0].Exceptions.Count);
        }

        [Test]
        public void Throws_with_type_fails_if_no_exception_was_thrown()
        {
            AssertionFailure[] failures = Capture(()
                => Assert.Throws(typeof(InvalidOperationException), () => { }));
            Assert.AreEqual(1, failures.Length);
            Assert.IsNull(failures[0].Message);
            Assert.AreEqual("Expected the block to throw an exception.", failures[0].Description);
            Assert.AreEqual(0, failures[0].Exceptions.Count);
        }

        [Test]
        public void Throws_with_type_with_message_fails_if_no_exception_was_thrown()
        {
            AssertionFailure[] failures = Capture(()
                => Assert.Throws(typeof(InvalidOperationException), () => { }, "Hello {0}", "World"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Hello World", failures[0].Message);
            Assert.AreEqual("Expected the block to throw an exception.", failures[0].Description);
            Assert.AreEqual(0, failures[0].Exceptions.Count);
        }

        [Test]
        public void Throws_fails_if_supertype_of_expected_exception_was_thrown()
        {
            AssertionFailure[] failures = Capture(()
                => Assert.Throws<InvalidOperationException>(() => { throw new Exception("Wrong exception type."); }));
            Assert.AreEqual(1, failures.Length);
            Assert.IsNull(failures[0].Message);
            Assert.AreEqual("The block threw an exception of a different type than was expected.", failures[0].Description);
            Assert.AreEqual(1, failures[0].Exceptions.Count);
            Assert.AreEqual("Wrong exception type.", failures[0].Exceptions[0].Message);
        }

        [Test]
        public void Throws_fails_if_unrelated_expected_exception_was_thrown()
        {
            AssertionFailure[] failures = Capture(()
                => Assert.Throws<InvalidOperationException>(() => { throw new NotSupportedException("Wrong exception type."); }));
            Assert.AreEqual(1, failures.Length);
            Assert.IsNull(failures[0].Message);
            Assert.AreEqual("The block threw an exception of a different type than was expected.", failures[0].Description);
            Assert.AreEqual(1, failures[0].Exceptions.Count);
            Assert.AreEqual("Wrong exception type.", failures[0].Exceptions[0].Message);
        }

        [Test]
        public void DoesNotThrow_throws_if_arguments_invalid()
        {
            Assert.Throws<ArgumentNullException>(() => Assert.DoesNotThrow(null));
            Assert.Throws<ArgumentNullException>(() => Assert.DoesNotThrow(null, ""));
        }

        [Test]
        public void DoesNotThrow_passes_if_no_exception_thrown()
        {
            Assert.DoesNotThrow(() => { });
        }

        [Test]
        public void DoesNotThrow_with_message_passes_if_no_exception_thrown()
        {
            Assert.DoesNotThrow(() => { }, "Foo");
        }

        [Test]
        public void DoesNotThrow_fails_if_exception_thrown()
        {
            AssertionFailure[] failures = Capture(()
                => Assert.DoesNotThrow(() => { throw new NotSupportedException("Boom."); }));
            Assert.AreEqual(1, failures.Length);
            Assert.IsNull(failures[0].Message);
            Assert.AreEqual("The block threw an exception but none was expected.", failures[0].Description);
            Assert.AreEqual(1, failures[0].Exceptions.Count);
            Assert.AreEqual("Boom.", failures[0].Exceptions[0].Message);
        }

        [Test]
        public void DoesNotThrow_with_message_fails_if_exception_thrown()
        {
            AssertionFailure[] failures = Capture(()
                => Assert.DoesNotThrow(() => { throw new NotSupportedException("Boom."); }, "Hello {0}", "World"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Hello World", failures[0].Message);
            Assert.AreEqual("The block threw an exception but none was expected.", failures[0].Description);
            Assert.AreEqual("Boom.", failures[0].Exceptions[0].Message);
        }

        [Test]
        public void Throws_passes_and_returns_exception_when_expected_exception_and_inner_exception_occur()
        {
            InvalidOperationException ex = Assert.Throws<InvalidOperationException, ArgumentException>(() =>
                {
                    throw new InvalidOperationException("Exception", new ArgumentException());
                });
            Assert.IsNotNull(ex);
            Assert.AreEqual("Exception", ex.Message);
        }

        [Test]
        public void Throws_passes_and_returns_exception_when_expected_exception_and_derived_inner_exception_occur()
        {
            InvalidOperationException ex = Assert.Throws<InvalidOperationException, ArgumentException>(() =>
            {
                throw new InvalidOperationException("Exception", new ArgumentOutOfRangeException());
            });
            Assert.IsNotNull(ex);
            Assert.AreEqual("Exception", ex.Message);
        }

        [Test]
        public void Throws_fails_if_no_inner_exception()
        {
            AssertionFailure[] failures = Capture(() =>
                Assert.Throws<InvalidOperationException, ArgumentException>(() =>
                {
                    throw new InvalidOperationException("Exception");
                }));
            Assert.AreEqual(1, failures.Length);
            Assert.IsNull(failures[0].Message);
            Assert.AreEqual("The block threw an exception of the expected type, but having no inner expection.", failures[0].Description);
        }

        [Test]
        public void Throws_fails_if_inner_exception_does_not_match()
        {
            AssertionFailure[] failures = Capture(() => 
                Assert.Throws<InvalidOperationException, ArgumentException>(() =>
                {
                    throw new InvalidOperationException("Exception", new NotSupportedException());
                }));
            Assert.AreEqual(1, failures.Length);
            Assert.IsNull(failures[0].Message);
            Assert.AreEqual("The block threw an exception of the expected type, but having an unexpected inner expection.", failures[0].Description);
        }
    }
}
