using System;
using System.Collections.Generic;
using Gallio.Model.Logging;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Model.Logging
{
    [VerifyEqualityContract(typeof(Marker))]
    public class MarkerTest : IEquivalenceClassProvider<Marker>
    {
        public EquivalenceClassCollection<Marker> GetEquivalenceClasses()
        {
            return EquivalenceClassCollection<Marker>.FromDistinctInstances(
                Marker.AssertionFailure,
                Marker.DiffAddition,
                Marker.AssertionFailure.WithAttribute("a", "x"),
                Marker.AssertionFailure.WithAttribute("a", "y"),
                Marker.AssertionFailure.WithAttribute("a", "x").WithAttribute("b", "y")
                );
        }

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
            NewAssert.AreEqual("foo", new Marker("foo").Class);
        }

        [Test]
        public void ToStringReturnsTheMarkerClassAndSortedAttributeValues()
        {
            NewAssert.AreEqual("foo", new Marker("foo").ToString());
            NewAssert.AreEqual("foo: a = \"x\", b = \"z\", c = \"y\"", new Marker("foo")
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