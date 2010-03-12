// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Text;
using Gallio;
using Gallio.Framework.Assertions;
using Gallio.Common.Xml;
using System.Xml;
using System.IO;
using System.Data;
using Gallio.Common;

namespace MbUnit.Framework
{
    public abstract partial class Assert
    {
        /// <summary>
        /// Assertions for XML data.
        /// </summary>
        public abstract class Xml
        {
            #region Fragment Equality
            
            /// <summary>
            /// Asserts that two XML fragments have the same content.
            /// </summary>
            /// <param name="expectedXmlReader">A reader to get the expected XML fragment.</param>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedXmlReader"/> or <paramref name="actualXmlReader"/> is null.</exception>
            public static void AreEqual(TextReader expectedXmlReader, TextReader actualXmlReader)
            {
                AreEqual(expectedXmlReader, actualXmlReader, XmlOptions.Default, null, null);
            }

            /// <summary>
            /// Asserts that two XML fragments have the same content.
            /// </summary>
            /// <param name="expectedXmlReader">A reader to get the expected XML fragment.</param>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="options">Equality options.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedXmlReader"/>, <paramref name="actualXmlReader"/>, or <paramref name="options"/> is null.</exception>
            public static void AreEqual(TextReader expectedXmlReader, TextReader actualXmlReader, XmlOptions options)
            {
                AreEqual(expectedXmlReader, actualXmlReader, options, null, null);
            }

            /// <summary>
            /// Asserts that two XML fragments have the same content.
            /// </summary>
            /// <param name="expectedXmlReader">A reader to get the expected XML fragment.</param>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedXmlReader"/> or <paramref name="actualXmlReader"/> is null.</exception>
            public static void AreEqual(TextReader expectedXmlReader, TextReader actualXmlReader, string messageFormat, params object[] messageArgs)
            {
                AreEqual(expectedXmlReader, actualXmlReader, XmlOptions.Default, messageFormat, messageArgs);
            }

            /// <summary>
            /// Asserts that two XML fragments have the same content.
            /// </summary>
            /// <param name="expectedXmlReader">A reader to get the expected XML fragment.</param>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="options">Equality options.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedXmlReader"/>, <paramref name="actualXmlReader"/>, or <paramref name="options"/> is null.</exception>
            public static void AreEqual(TextReader expectedXmlReader, TextReader actualXmlReader, XmlOptions options, string messageFormat, params object[] messageArgs)
            {
                if (expectedXmlReader == null)
                    throw new ArgumentNullException("expectedXmlReader");
                if (actualXmlReader == null)
                    throw new ArgumentNullException("actualXmlReader");

                AreEqual(expectedXmlReader.ReadToEnd(), actualXmlReader.ReadToEnd(), options, messageFormat, messageArgs);
            }

            /// <summary>
            /// Asserts that two XML fragments have the same content.
            /// </summary>
            /// <param name="expectedXml">The expected XML fragment.</param>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedXml"/> or <paramref name="actualXml"/> is null.</exception>
            public static void AreEqual(string expectedXml, string actualXml)
            {
                AreEqual(expectedXml, actualXml, XmlOptions.Default, null, null);
            }

            /// <summary>
            /// Asserts that two XML fragments have the same content.
            /// </summary>
            /// <param name="expectedXml">The expected XML fragment.</param>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="options">Equality options.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedXml"/>, <paramref name="actualXml"/>, or <paramref name="options"/> is null.</exception>
            public static void AreEqual(string expectedXml, string actualXml, XmlOptions options)
            {
                AreEqual(expectedXml, actualXml, options, null, null);
            }

            /// <summary>
            /// Asserts that two XML fragments have the same content.
            /// </summary>
            /// <param name="expectedXml">The expected XML fragment.</param>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedXml"/> or <paramref name="actualXml"/> is null.</exception>
            public static void AreEqual(string expectedXml, string actualXml, string messageFormat, params object[] messageArgs)
            {
                AreEqual(expectedXml, actualXml, XmlOptions.Default, messageFormat, messageArgs);
            }

            /// <summary>
            /// Asserts that two XML fragments have the same content.
            /// </summary>
            /// <param name="expectedXml">The expected XML fragment.</param>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="options">Equality options.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedXml"/>, <paramref name="actualXml"/>, or <paramref name="options"/> is null.</exception>
            public static void AreEqual(string expectedXml, string actualXml, XmlOptions options, string messageFormat, params object[] messageArgs)
            {
                if (expectedXml == null)
                    throw new ArgumentNullException("expectedXml");
                if (actualXml == null)
                    throw new ArgumentNullException("actualXml");

                AssertionHelper.Verify(() =>
                {
                    Fragment actual;
                    Fragment expected;

                    try
                    {
                        expected = Parser.Run(expectedXml, options.Value);
                    }
                    catch (XmlException exception)
                    {
                        return new AssertionFailureBuilder("Cannot parse the actual XML fragment.")
                            .SetMessage(messageFormat, messageArgs)
                            .AddException(exception)
                            .ToAssertionFailure();
                    }

                    try
                    {
                        actual = Parser.Run(actualXml, options.Value);
                    }
                    catch (XmlException exception)
                    {
                        return new AssertionFailureBuilder("Cannot parse the expected XML fragment.")
                            .SetMessage(messageFormat, messageArgs)
                            .AddException(exception)
                            .ToAssertionFailure();
                    }

                    DiffSet diffSet = actual.Diff(expected, options.Value);

                    if (diffSet.IsEmpty)
                        return null;

                    return new AssertionFailureBuilder("Expected XML fragments to be equal according to the specified options.")
                        .SetMessage(messageFormat, messageArgs)
                        .AddRawLabeledValue("Equality Options", options.Value)
                        .AddInnerFailures(diffSet.ToAssertionFailures(expected, actual))
                        .ToAssertionFailure();
                });
            }

            #endregion

            #region Exists

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute.
            /// </summary>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            /// <remarks>
            /// <para>
            /// Options not related to the name case of elements and attributes are going to be ignored.
            /// </para>
            /// <include file='../../../Gallio/docs/XmlPathLooseSyntax.xml' path='doc/remarks/*' />
            /// </remarks>
            public static void Exists(TextReader actualXmlReader, string searchedPath, XmlOptions options)
            {
                Exists(actualXmlReader, searchedPath, options, null, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute.
            /// </summary>
            /// <remarks>
            /// <para>
            /// Options not related to the name case of elements and attributes are going to be ignored.
            /// </para>
            /// <include file='../../../Gallio/docs/XmlPathLooseSyntax.xml' path='doc/remarks/*' />
            /// </remarks>
             /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void Exists(TextReader actualXmlReader, string searchedPath, XmlOptions options, string messageFormat, params object[] messageArgs)
            {
                Exists(actualXmlReader, searchedPath, options, null, messageFormat, messageArgs);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute.
            /// </summary>
            /// <remarks>
            /// <include file='../../../Gallio/docs/XmlPathLooseSyntax.xml' path='doc/remarks/*' />
            /// </remarks>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void Exists(string actualXml, string searchedPath, XmlOptions options)
            {
                Exists(actualXml, searchedPath, options, null, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute.
            /// </summary>
            /// <remarks>
            /// <para>
            /// Options not related to the name case of elements and attributes are going to be ignored.
            /// </para>
            /// <include file='../../../Gallio/docs/XmlPathLooseSyntax.xml' path='doc/remarks/*' />
            /// </remarks>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void Exists(string actualXml, string searchedPath, XmlOptions options, string messageFormat, params object[] messageArgs)
            {
                Exists(actualXml, searchedPath, options, null, messageFormat, messageFormat);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute.
            /// </summary>
            /// <remarks>
            /// <para>
            /// Options not related to the name case of elements and attributes are going to be ignored.
            /// </para>
            /// <include file='../../../Gallio/docs/XmlPathLooseSyntax.xml' path='doc/remarks/*' />
            /// </remarks>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void Exists(string actualXml, string searchedPath, XmlOptions options, string expectedValue, string messageFormat, params object[] messageArgs)
            {
                Exists(actualXml, XmlPathRoot.Parse(searchedPath), options, expectedValue, messageFormat, messageFormat);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that it has the expected value.
            /// </summary>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(TextReader, IXmlPathLoose, XmlOptions)"/>.
            /// </para>
            /// <include file='../../../Gallio/docs/XmlPathLooseSyntax.xml' path='doc/remarks/*' />
            /// </remarks>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void Exists(TextReader actualXmlReader, string searchedPath, XmlOptions options, string expectedValue)
            {
                Exists(actualXmlReader, searchedPath, options, expectedValue, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that it has the expected value.
            /// </summary>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(TextReader, IXmlPathLoose, XmlOptions, string, object[])"/>.
            /// </para>
            /// <include file='../../../Gallio/docs/XmlPathLooseSyntax.xml' path='doc/remarks/*' />
            /// </remarks>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void Exists(TextReader actualXmlReader, string searchedPath, XmlOptions options, string expectedValue, string messageFormat, params object[] messageArgs)
            {
                Exists(actualXmlReader, searchedPath, options, expectedValue, messageFormat, messageArgs);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that it has the expected value.
            /// </summary>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(string, IXmlPathLoose, XmlOptions)"/>.
            /// </para>
            /// <include file='../../../Gallio/docs/XmlPathLooseSyntax.xml' path='doc/remarks/*' />
            /// </remarks>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <param name="options">Options for the search.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            /// <remarks>
            /// </remarks>
            public static void Exists(string actualXml, string searchedPath, XmlOptions options, string expectedValue)
            {
                Exists(actualXml, searchedPath, options, expectedValue, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute.
            /// </summary>
            /// <remarks>
            /// <para>
            /// Options not related to the name case of elements and attributes are going to be ignored.
            /// </para>
            /// </remarks>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void Exists(TextReader actualXmlReader, IXmlPathLoose searchedPath, XmlOptions options)
            {
                Exists(actualXmlReader, searchedPath, options, null, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute.
            /// </summary>
            /// <remarks>
            /// <para>
            /// Options not related to the name case of elements and attributes are going to be ignored.
            /// </para>
            /// </remarks>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void Exists(TextReader actualXmlReader, IXmlPathLoose searchedPath, XmlOptions options, string messageFormat, params object[] messageArgs)
            {
                Exists(actualXmlReader, searchedPath, options, null, messageFormat, messageArgs);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute.
            /// </summary>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            /// <remarks>
            /// <para>
            /// Options not related to the name case of elements and attributes are going to be ignored.
            /// </para>
            /// </remarks>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            public static void Exists(string actualXml, IXmlPathLoose searchedPath, XmlOptions options)
            {
                Exists(actualXml, searchedPath, options, null, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute.
            /// </summary>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            /// <remarks>
            /// <para>
            /// Options not related to the name case of elements and attributes are going to be ignored.
            /// </para>
            /// </remarks>
            public static void Exists(string actualXml, IXmlPathLoose searchedPath, XmlOptions options, string messageFormat, params object[] messageArgs)
            {
                Exists(actualXml, searchedPath, options, null, messageFormat, messageFormat);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that it has the expected value.
            /// </summary>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(TextReader, IXmlPathLoose, XmlOptions)"/>.
            /// </para>
            /// </remarks>
            public static void Exists(TextReader actualXmlReader, IXmlPathLoose searchedPath, XmlOptions options, string expectedValue)
            {
                Exists(actualXmlReader, searchedPath, options, expectedValue, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that it has the expected value.
            /// </summary>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(TextReader, IXmlPathLoose, XmlOptions, string, object[])"/>.
            /// </para>
            /// </remarks>
            public static void Exists(TextReader actualXmlReader, IXmlPathLoose searchedPath, XmlOptions options, string expectedValue, string messageFormat, params object[] messageArgs)
            {
                if (actualXmlReader == null)
                    throw new ArgumentNullException("actualXmlReader");

                Exists(actualXmlReader.ReadToEnd(), searchedPath, options, expectedValue, messageFormat, messageArgs);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that it has the expected value.
            /// </summary>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(string, IXmlPathLoose, XmlOptions)"/>.
            /// </para>
            /// </remarks>
            public static void Exists(string actualXml, IXmlPathLoose searchedPath, XmlOptions options, string expectedValue)
            {
                Exists(actualXml, searchedPath, options, expectedValue, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that it has the expected value.
            /// </summary>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(string, IXmlPathLoose, XmlOptions, string, object[])"/>.
            /// </para>
            /// </remarks>
            public static void Exists(string actualXml, IXmlPathLoose searchedPath, XmlOptions options, string expectedValue, string messageFormat, params object[] messageArgs)
            {
                if (actualXml == null)
                    throw new ArgumentNullException("actualXml");
                if (searchedPath == null)
                    throw new ArgumentNullException("searchedPath");
                if (options == null)
                    throw new ArgumentNullException("options");

                AssertionHelper.Verify(() =>
                {
                    Fragment actual;

                    try
                    {
                        actual = Parser.Run(actualXml, options.Value);
                    }
                    catch (XmlException exception)
                    {
                        return new AssertionFailureBuilder("Cannot parse the actual XML fragment.")
                            .SetMessage(messageFormat, messageArgs)
                            .AddException(exception)
                            .ToAssertionFailure();
                    }

                    if (actual.CountAt(searchedPath, expectedValue, options.Value) > 0)
                        return null;

                    var builder = new AssertionFailureBuilder("Expected the XML fragment to contain the searched XML element or attribute, but none was found.")
                        .SetMessage(messageFormat, messageArgs)
                        .AddLabeledValue("Item searched", searchedPath.ToString());

                    if (expectedValue != null)
                    {
                        builder.AddLabeledValue("Expected value", expectedValue);
                    }

                    return builder.ToAssertionFailure();
                });
            }

            #endregion

            #region IsUnique

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that this element or attribute is unique in the entire fragment.
            /// </summary>
            /// <remarks>
            /// <include file='../../../Gallio/docs/XmlPathLooseSyntax.xml' path='doc/remarks/*' />
            /// </remarks>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            /// <remarks>
            /// <para>
            /// Options not related to the name case of elements and attributes are going to be ignored.
            /// </para>
            /// </remarks>
            public static void IsUnique(TextReader actualXmlReader, string searchedPath, XmlOptions options)
            {
                IsUnique(actualXmlReader, searchedPath, options, null, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that this element or attribute is unique in the entire fragment.
            /// </summary>
            /// <para>
            /// Options not related to the name case of elements and attributes are going to be ignored.
            /// </para>
            /// <remarks>
            /// <include file='../../../Gallio/docs/XmlPathLooseSyntax.xml' path='doc/remarks/*' />
            /// </remarks>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void IsUnique(TextReader actualXmlReader, string searchedPath, XmlOptions options, string messageFormat, params object[] messageArgs)
            {
                IsUnique(actualXmlReader, searchedPath, options, null, messageFormat, messageArgs);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that this element or attribute is unique in the entire fragment.
            /// </summary>
            /// <remarks>
            /// <para>
            /// Options not related to the name case of elements and attributes are going to be ignored.
            /// </para>
            /// <include file='../../../Gallio/docs/XmlPathLooseSyntax.xml' path='doc/remarks/*' />
            /// </remarks>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void IsUnique(string actualXml, string searchedPath, XmlOptions options)
            {
                IsUnique(actualXml, searchedPath, options, null, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that this element or attribute is unique in the entire fragment.
            /// </summary>
            /// <remarks>
            /// <para>
            /// Options not related to the name case of elements and attributes are going to be ignored.
            /// </para>
            /// <include file='../../../Gallio/docs/XmlPathLooseSyntax.xml' path='doc/remarks/*' />
            /// </remarks>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void IsUnique(string actualXml, string searchedPath, XmlOptions options, string messageFormat, params object[] messageArgs)
            {
                IsUnique(actualXml, searchedPath, options, null, messageFormat, messageFormat);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, that it has the expected value, and that this element or attribute is unique in the entire fragment.
            /// </summary>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(TextReader, IXmlPathLoose, XmlOptions)"/>.
            /// </para>
            /// <include file='../../../Gallio/docs/XmlPathLooseSyntax.xml' path='doc/remarks/*' />
            /// </remarks>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void IsUnique(TextReader actualXmlReader, string searchedPath, string expectedValue, XmlOptions options)
            {
                IsUnique(actualXmlReader, searchedPath, options, expectedValue, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, that it has the expected value, and that this element or attribute is unique in the entire fragment.
            /// </summary>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(TextReader, IXmlPathLoose, XmlOptions, string, object[])"/>.
            /// </para>
            /// <include file='../../../Gallio/docs/XmlPathLooseSyntax.xml' path='doc/remarks/*' />
            /// </remarks>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void IsUnique(TextReader actualXmlReader, string searchedPath, XmlOptions options, string expectedValue, string messageFormat, params object[] messageArgs)
            {
                IsUnique(actualXmlReader, searchedPath, options, expectedValue, messageFormat, messageArgs);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, that it has the expected value, and that this element or attribute is unique in the entire fragment.
            /// </summary>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(string, IXmlPathLoose, XmlOptions)"/>.
            /// </para>
            /// <include file='../../../Gallio/docs/XmlPathLooseSyntax.xml' path='doc/remarks/*' />
            /// </remarks>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void IsUnique(string actualXml, string searchedPath, string expectedValue, XmlOptions options)
            {
                IsUnique(actualXml, searchedPath, options, expectedValue, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, that it has the expected value, and that this element or attribute is unique in the entire fragment.
            /// </summary>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(string, IXmlPathLoose, XmlOptions)"/>.
            /// </para>
            /// <include file='../../../Gallio/docs/XmlPathLooseSyntax.xml' path='doc/remarks/*' />
            /// </remarks>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void IsUnique(string actualXml, string searchedPath, XmlOptions options, string expectedValue, string messageFormat, params object[] messageArgs)
            {
                IsUnique(actualXml, XmlPathRoot.Parse(searchedPath), options, expectedValue, messageFormat, messageArgs);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that this element or attribute is unique in the entire fragment.
            /// </summary>
            /// <remarks>
            /// <para>
            /// Options not related to the name case of elements and attributes are going to be ignored.
            /// </para>
            /// </remarks>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void IsUnique(TextReader actualXmlReader, IXmlPathLoose searchedPath, XmlOptions options)
            {
                IsUnique(actualXmlReader, searchedPath, options, null, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that this element or attribute is unique in the entire fragment.
            /// </summary>
            /// <remarks>
            /// <para>
            /// Options not related to the name case of elements and attributes are going to be ignored.
            /// </para>
            /// </remarks>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void IsUnique(TextReader actualXmlReader, IXmlPathLoose searchedPath, XmlOptions options, string messageFormat, params object[] messageArgs)
            {
                IsUnique(actualXmlReader, searchedPath, options, null, messageFormat, messageArgs);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that this element or attribute is unique in the entire fragment.
            /// </summary>
            /// <remarks>
            /// <para>
            /// Options not related to the name case of elements and attributes are going to be ignored.
            /// </para>
            /// </remarks>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void IsUnique(string actualXml, IXmlPathLoose searchedPath, XmlOptions options)
            {
                IsUnique(actualXml, searchedPath, options, null, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that this element or attribute is unique in the entire fragment.
            /// </summary>
            /// <remarks>
            /// <para>
            /// Options not related to the name case of elements and attributes are going to be ignored.
            /// </para>
            /// </remarks>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void IsUnique(string actualXml, IXmlPathLoose searchedPath, XmlOptions options, string messageFormat, params object[] messageArgs)
            {
                IsUnique(actualXml, searchedPath, options, null, messageFormat, messageFormat);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, that it has the expected value, and that this element or attribute is unique in the entire fragment.
            /// </summary>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(TextReader, IXmlPathLoose, XmlOptions)"/>.
            /// </para>
            /// </remarks>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void IsUnique(TextReader actualXmlReader, IXmlPathLoose searchedPath, XmlOptions options, string expectedValue)
            {
                IsUnique(actualXmlReader, searchedPath, options, expectedValue, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, that it has the expected value, and that this element or attribute is unique in the entire fragment.
            /// </summary>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(TextReader, IXmlPathLoose, XmlOptions, string, object[])"/>.
            /// </para>
            /// </remarks>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void IsUnique(TextReader actualXmlReader, IXmlPathLoose searchedPath, XmlOptions options, string expectedValue, string messageFormat, params object[] messageArgs)
            {
                if (actualXmlReader == null)
                    throw new ArgumentNullException("actualXmlReader");

                IsUnique(actualXmlReader.ReadToEnd(), searchedPath, options, expectedValue, messageFormat, messageArgs);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, that it has the expected value, and that this element or attribute is unique in the entire fragment.
            /// </summary>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(string, IXmlPathLoose, XmlOptions)"/>.
            /// </para>
            /// </remarks>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <param name="options">Options for the search.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void IsUnique(string actualXml, IXmlPathLoose searchedPath, string expectedValue, XmlOptions options)
            {
                IsUnique(actualXml, searchedPath, options, expectedValue, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, that it has the expected value, and that this element or attribute is unique in the entire fragment.
            /// </summary>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(string, IXmlPathLoose, XmlOptions, string, object[])"/>.
            /// </para>
            /// </remarks>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedPath">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/>, <paramref name="searchedPath"/>, or <paramref name="options"/> is null.</exception>
            public static void IsUnique(string actualXml, IXmlPathLoose searchedPath, XmlOptions options, string expectedValue, string messageFormat, params object[] messageArgs)
            {
                if (actualXml == null)
                    throw new ArgumentNullException("actualXml");
                if (searchedPath == null)
                    throw new ArgumentNullException("searchedPath");
                if (options == null)
                    throw new ArgumentNullException("options");

                AssertionHelper.Verify(() =>
                {
                    Fragment actual;

                    try
                    {
                        actual = Parser.Run(actualXml, options.Value);
                    }
                    catch (XmlException exception)
                    {
                        return new AssertionFailureBuilder("Cannot parse the actual XML fragment.")
                            .SetMessage(messageFormat, messageArgs)
                            .AddException(exception)
                            .ToAssertionFailure();
                    }

                    int count = actual.CountAt(searchedPath, expectedValue, options.Value);

                    if (count == 1)
                        return null;

                    var builder = new AssertionFailureBuilder("Expected the XML fragment to contain only once the searched XML element or attribute, " +
                        (count == 0 ? "but none was found." : "But several were found."))
                        .SetMessage(messageFormat, messageArgs)
                        .AddLabeledValue("Item searched", searchedPath.ToString())
                        .AddRawLabeledValue("Number of items found", count);

                    if (expectedValue != null)
                    {
                        builder.AddLabeledValue("Expected value", expectedValue);
                    }

                    return builder.ToAssertionFailure();
                });
            }

            #endregion
        }
    }
}
