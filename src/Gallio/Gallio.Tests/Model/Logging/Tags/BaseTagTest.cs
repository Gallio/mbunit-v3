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
using Gallio.Common;
using Gallio.Model.Logging;
using Gallio.Model.Logging.Tags;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Model.Logging.Tags
{
    public abstract class BaseTagTest<T>
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
                    Assert.AreEqual(item, item.Clone());
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
                    Assert.AreEqual(item, item.Clone());
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

                    Assert.AreEqual(formatter.ToString(), item.ToString());
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

                    Assert.AreEqual(writer.ToString(), item.ToString());
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
                    Assert.AreEqual(item, Assert.XmlSerializeThenDeserialize(item));
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
