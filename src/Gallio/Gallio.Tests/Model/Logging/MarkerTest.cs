using System;
using Gallio.Model.Logging;
using Gallio.Model.Logging.Tags;
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
                Marker.DiffAddition
                );
        }

        [Test]
        [Row(null, ExpectedException=typeof(ArgumentNullException))]
        [Row("", ExpectedException = typeof(ArgumentException))]
        [Row("  ", ExpectedException = typeof(ArgumentException))]
        [Row(":$%", ExpectedException = typeof(ArgumentException))]
        [Row("abcd_1234")]
        public void ValidateClass(string @class)
        {
            Marker.ValidateClass(@class);
        }

        [Test]
        [Row(null, ExpectedException = typeof(ArgumentNullException))]
        [Row("", ExpectedException = typeof(ArgumentException))]
        [Row("  ", ExpectedException = typeof(ArgumentException))]
        [Row(":$%", ExpectedException = typeof(ArgumentException))]
        [Row("abcd_1234")]
        public void ConstructorPerformsValidation(string @class)
        {
            new Marker(@class);
        }

        [Test]
        public void ConstructorInitializesProperties()
        {
            NewAssert.AreEqual("foo", new Marker("foo").Class);
        }

        [Test]
        public void ToStringReturnsTheMarkerClass()
        {
            NewAssert.AreEqual("foo", new Marker("foo").ToString());
        }
    }
}