using System;
using Gallio.Model.Logging;
using Gallio.Model.Logging.Tags;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using MbUnit.Framework.Xml;

namespace Gallio.Tests.Model.Logging.Tags
{
    public abstract class BaseTagTest<T> : IEquivalenceClassProvider<T>
        where T : Tag, ICloneable<T>
    {
        public abstract EquivalenceClassCollection<T> GetEquivalenceClasses();

        [Test]
        public void CloneObject()
        {
            foreach (EquivalenceClass<T> @class in GetEquivalenceClasses())
            {
                foreach (ICloneable item in @class)
                {
                    NewAssert.AreEqual(item, item.Clone());
                }
            }
        }

        [Test]
        public void CloneGeneric()
        {
            foreach (EquivalenceClass<T> @class in GetEquivalenceClasses())
            {
                foreach (ICloneable<T> item in @class)
                {
                    NewAssert.AreEqual(item, item.Clone());
                }
            }
        }

        [Test]
        public void ToStringEqualsContentsReturnedByTagFormatter()
        {
            foreach (EquivalenceClass<T> @class in GetEquivalenceClasses())
            {
                foreach (Tag item in @class)
                {
                    TagFormatter formatter = new TagFormatter();
                    item.Accept(formatter);

                    NewAssert.AreEqual(formatter.ToString(), item.ToString());
                }
            }
        }

        [Test]
        public void WriteToReproducesTheContentExactly()
        {
            foreach (EquivalenceClass<T> @class in GetEquivalenceClasses())
            {
                foreach (Tag item in @class)
                {
                    StructuredTextWriter writer = new StructuredTextWriter();
                    PrepareLogWriterForWriteToTest(writer.Container);
                    item.WriteTo(writer);

                    NewAssert.AreEqual(writer.ToString(), item.ToString());
                }
            }
        }

        [Test]
        public void CanSerializeToXmlRoundTrip()
        {
            foreach (EquivalenceClass<T> @class in GetEquivalenceClasses())
            {
                foreach (Tag item in @class)
                {
                    XmlSerializationAssert.AreEqualAfterRoundTrip(item);
                }
            }
        }

        [Test, ExpectedArgumentNullException]
        public void AcceptThrowsIfVisitorIsNull()
        {
            new BodyTag().Accept(null);
        }

        [Test, ExpectedArgumentNullException]
        public void WriteToThrowsIfVisitorIsNull()
        {
            new BodyTag().WriteTo(null);
        }

        protected virtual void PrepareLogWriterForWriteToTest(TestLogWriter writer)
        {
        }
    }
}
