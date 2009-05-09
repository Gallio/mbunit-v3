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
using Gallio.Common.Markup;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Common.Markup
{
    public class MarkerTest
    {
        [VerifyContract]
        public readonly IContract EqualityTests = new EqualityContract<Marker>
        {
            EquivalenceClasses =
            {
                { Marker.AssertionFailure },
                { Marker.DiffAddition },
                { Marker.AssertionFailure.WithAttribute("a", "x") },
                { Marker.AssertionFailure.WithAttribute("a", "y") },
                { Marker.AssertionFailure.WithAttribute("a", "x").WithAttribute("b", "y") }
            }
        };

        [Test]
        [Row(null, ExpectedException=typeof(ArgumentNullException))]
        [Row("", ExpectedException = typeof(ArgumentException))]
        [Row("  ", ExpectedException = typeof(ArgumentException))]
        [Row(":$%", ExpectedException = typeof(ArgumentException))]
        [Row("abcd_1234")]
        public void ValidateIdentifier(string @class)
        {
            Marker.ValidateIdentifier(@class);
        }

        [Test]
        [Row(null, ExpectedException = typeof(ArgumentNullException))]
        [Row("", ExpectedException = typeof(ArgumentException))]
        [Row("  ", ExpectedException = typeof(ArgumentException))]
        [Row(":$%", ExpectedException = typeof(ArgumentException))]
        [Row("abcd_1234")]
        public void ConstructorPerformsValidationOfClass(string @class)
        {
            new Marker(@class);
        }

        [Test]
        [Row(null, "aa", ExpectedException = typeof(ArgumentNullException))]
        [Row("abc", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("", "aa", ExpectedException = typeof(ArgumentException))]
        [Row("  ", "aa", ExpectedException = typeof(ArgumentException))]
        [Row(":$%", "aa", ExpectedException = typeof(ArgumentException))]
        [Row("abcd_1234", "aa")]
        public void ConstructorPerformsValidationOfAttributeName(string name, string value)
        {
            new Marker(Marker.DiffAdditionClass,
                new Dictionary<string, string>() { { name, value }});
        }

        [Test]
        [Row(null, "aa", ExpectedException = typeof(ArgumentNullException))]
        [Row("abc", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("", "aa", ExpectedException = typeof(ArgumentException))]
        [Row("  ", "aa", ExpectedException = typeof(ArgumentException))]
        [Row(":$%", "aa", ExpectedException = typeof(ArgumentException))]
        [Row("abcd_1234", "aa")]
        public void WithAttributePerformsValidationOfAttributeNameAndValue(string name, string value)
        {
            new Marker(Marker.DiffAdditionClass).WithAttribute(name, value);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfAttributesDictionaryIsNull()
        {
            new Marker(Marker.AssertionFailureClass, null);
        }

        [Test]
        public void ConstructorInitializesProperties()
        {
            Assert.AreEqual("foo", new Marker("foo").Class);
        }

        [Test]
        public void ToStringReturnsTheMarkerClassAndSortedAttributeValues()
        {
            Assert.AreEqual("foo", new Marker("foo").ToString());
            Assert.AreEqual("foo: a = \"x\", b = \"z\", c = \"y\"", new Marker("foo")
                .WithAttribute("a", "x")
                .WithAttribute("c", "y")
                .WithAttribute("b", "z").ToString());
        }

        [Test]
        public void CanChainAttributesFluently()
        {
            Marker marker = Marker.Ellipsis
                .WithAttribute("a", "123")
                .WithAttribute("b", "456");
            Assert.AreEqual(2, marker.Attributes.Count);
            Assert.AreEqual("123", marker.Attributes["a"]);
            Assert.AreEqual("456", marker.Attributes["b"]);
        }
    }
}
