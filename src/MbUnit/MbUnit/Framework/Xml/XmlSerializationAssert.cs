// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Text;
using System.Xml.Serialization;

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
            InterimAssert.DoesNotThrow(delegate
            {
                new XmlSerializer(t);
            }, "The type is not Xml serializable.");
        }
    }
}
