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
