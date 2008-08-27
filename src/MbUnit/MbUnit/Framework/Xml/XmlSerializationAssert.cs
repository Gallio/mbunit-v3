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
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Gallio.Framework;

namespace MbUnit.Framework.Xml
{
    /// <summary>
    /// Assertions based on Xml serialization.
    /// </summary>
    /// <todo author="jeff">
    /// One-way and two-way serialization asserts need to be rewritten based on new
    /// constraints.  We can't assume that the plain object equality that Assert.AreEqual
    /// does now will be adequate for all cases.  In fact, I can't use these asserts for
    /// testing certain Xml-serializable framework bits right because the objects in question
    /// don't implement Equals (and really have no need to).
    /// </todo>
    public static class XmlSerializationAssert
    {
        /// <summary>
        /// Asserts that an <see cref="XmlSerializer" /> can be produced for a <see cref="Type" />.
        /// </summary>
        /// <param name="t">The type to check</param>
        public static void IsXmlSerializable(Type t)
        {
            Assert.DoesNotThrow(delegate
            {
                new XmlSerializer(t);
            }, "The type is not Xml serializable.");
        }

        /// <summary>
        /// Performs XML serialization then deserialization of the specified object.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize</typeparam>
        /// <param name="obj">The object</param>
        /// <returns>The deserialized object after serialization</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        public static T RoundTrip<T>(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, obj);

            stream.Position = 0;
            return (T) serializer.Deserialize(stream);
        }

        /// <summary>
        /// Performs XML serialization then deserialization of the specified object
        /// then compares the object to ensure that it equals the original.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize</typeparam>
        /// <param name="obj">The object</param>
        /// <returns>The deserialized object after serialization</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null</exception>
        public static void AreEqualAfterRoundTrip<T>(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            Assert.AreEqual(obj, RoundTrip(obj));
        }
    }
}
