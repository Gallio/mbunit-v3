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
using System.Text;
using Gallio;
using Gallio.Framework.Assertions;
using Gallio.Common.Xml;
using System.Xml;
using System.IO;
using System.Data;

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
                    Document actualDocument;
                    Document expectedDocument;

                    try
                    {
                        expectedDocument = new Parser(expectedXml).Run(options.Value);
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
                        actualDocument = new Parser(actualXml).Run(options.Value);
                    }
                    catch (XmlException exception)
                    {
                        return new AssertionFailureBuilder("Cannot parse the expected XML fragment.")
                            .SetMessage(messageFormat, messageArgs)
                            .AddException(exception)
                            .ToAssertionFailure();
                    }

                    DiffSet diffSet = actualDocument.Diff(expectedDocument, XmlPathRoot.Empty, options.Value);

                    if (diffSet.IsEmpty)
                        return null;

                    return new AssertionFailureBuilder("Expected XML fragments to be equal according to the specified options.")
                        .SetMessage(messageFormat, messageArgs)
                        .AddRawLabeledValue("Equality Options", options.Value)
                        .AddInnerFailures(diffSet.ToAssertionFailures())
                        .ToAssertionFailure();
                });
            }

            #endregion

            #region Exists (without value)

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute.
            /// </summary>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedItem">The path of the searched element or attribute in the XML fragment.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/> or <paramref name="searchedItem"/> is null.</exception>
            public static void Exists(TextReader actualXmlReader, IXmlPath searchedItem)
            {
                Exists(actualXmlReader, searchedItem, XmlOptions.Default, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute.
            /// </summary>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedItem">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/>, <paramref name="searchedItem"/>, or <paramref name="options"/> is null.</exception>
            /// <remarks>
            /// <para>
            /// Options not related to the name case of elements and attributes are going to be ignored.
            /// </para>
            /// </remarks>
            public static void Exists(TextReader actualXmlReader, IXmlPath searchedItem, XmlOptions options)
            {
                Exists(actualXmlReader, searchedItem, options, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute.
            /// </summary>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedItem">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/> or <paramref name="searchedItem"/> is null.</exception>
            public static void Exists(TextReader actualXmlReader, IXmlPath searchedItem, string messageFormat, params object[] messageArgs)
            {
                Exists(actualXmlReader, searchedItem, XmlOptions.Default, messageFormat, messageArgs);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute.
            /// </summary>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedItem">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/>, <paramref name="searchedItem"/>, or <paramref name="options"/> is null.</exception>
            /// <remarks>
            /// <para>
            /// Options not related to the name case of elements and attributes are going to be ignored.
            /// </para>
            /// </remarks>
            public static void Exists(TextReader actualXmlReader, IXmlPath searchedItem, XmlOptions options, string messageFormat, params object[] messageArgs)
            {
                if (actualXmlReader == null)
                    throw new ArgumentNullException("actualXmlReader");

                Exists(actualXmlReader.ReadToEnd(), searchedItem, options, messageFormat, messageArgs);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute.
            /// </summary>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedItem">The path of the searched element or attribute in the XML fragment.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/> or <paramref name="searchedItem"/> is null.</exception>
            public static void Exists(string actualXml, IXmlPath searchedItem)
            {
                Exists(actualXml, searchedItem, XmlOptions.Default, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute.
            /// </summary>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedItem">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/>, <paramref name="searchedItem"/>, or <paramref name="options"/> is null.</exception>
            /// <remarks>
            /// <para>
            /// Options not related to the name case of elements and attributes are going to be ignored.
            /// </para>
            /// </remarks>
            public static void Exists(string actualXml, IXmlPath searchedItem, XmlOptions options)
            {
                Exists(actualXml, searchedItem, options, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute.
            /// </summary>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedItem">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/> or <paramref name="searchedItem"/> is null.</exception>
            public static void Exists(string actualXml, IXmlPath searchedItem, string messageFormat, params object[] messageArgs)
            {
                Exists(actualXml, searchedItem, XmlOptions.Default, messageFormat, messageArgs);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute.
            /// </summary>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedItem">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/>, <paramref name="searchedItem"/>, or <paramref name="options"/> is null.</exception>
            /// <remarks>
            /// <para>
            /// Options not related to the name case of elements and attributes are going to be ignored.
            /// </para>
            /// </remarks>
            public static void Exists(string actualXml, IXmlPath searchedItem, XmlOptions options, string messageFormat, params object[] messageArgs)
            {
                Exists(actualXml, searchedItem, null, options, messageFormat, messageFormat);
            }

            #endregion

            #region Exists (with value)

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that it has the expected value.
            /// </summary>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedItem">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/> or <paramref name="searchedItem"/> is null.</exception>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(TextReader, IXmlPath)"/>.
            /// </para>
            /// </remarks>
            public static void Exists(TextReader actualXmlReader, IXmlPath searchedItem, string expectedValue)
            {
                Exists(actualXmlReader, searchedItem, expectedValue, XmlOptions.Default, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that it has the expected value.
            /// </summary>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedItem">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <param name="options">Options for the search.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/>, <paramref name="searchedItem"/>, or <paramref name="options"/> is null.</exception>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(TextReader, IXmlPath, XmlOptions)"/>.
            /// </para>
            /// </remarks>
            public static void Exists(TextReader actualXmlReader, IXmlPath searchedItem, string expectedValue, XmlOptions options)
            {
                Exists(actualXmlReader, searchedItem, expectedValue, options, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that it has the expected value.
            /// </summary>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedItem">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/> or <paramref name="searchedItem"/> is null.</exception>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(TextReader, IXmlPath, string, object[])"/>.
            /// </para>
            /// </remarks>
            public static void Exists(TextReader actualXmlReader, IXmlPath searchedItem, string expectedValue, string messageFormat, params object[] messageArgs)
            {
                Exists(actualXmlReader, searchedItem, expectedValue, XmlOptions.Default, messageFormat, messageArgs);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that it has the expected value.
            /// </summary>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="searchedItem">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXmlReader"/>, <paramref name="searchedItem"/>, or <paramref name="options"/> is null.</exception>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(TextReader, IXmlPath, XmlOptions, string, object[])"/>.
            /// </para>
            /// </remarks>
            public static void Exists(TextReader actualXmlReader, IXmlPath searchedItem, string expectedValue, XmlOptions options, string messageFormat, params object[] messageArgs)
            {
                if (actualXmlReader == null)
                    throw new ArgumentNullException("actualXmlReader");

                Exists(actualXmlReader.ReadToEnd(), searchedItem, expectedValue, options, messageFormat, messageArgs);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that it has the expected value.
            /// </summary>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedItem">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/> or <paramref name="searchedItem"/> is null.</exception>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(string, IXmlPath)"/>.
            /// </para>
            /// </remarks>
            public static void Exists(string actualXml, IXmlPath searchedItem, string expectedValue)
            {
                Exists(actualXml, searchedItem, expectedValue, XmlOptions.Default, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that it has the expected value.
            /// </summary>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedItem">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <param name="options">Options for the search.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/>, <paramref name="searchedItem"/>, or <paramref name="options"/> is null.</exception>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(string, IXmlPath, XmlOptions)"/>.
            /// </para>
            /// </remarks>
            public static void Exists(string actualXml, IXmlPath searchedItem, string expectedValue, XmlOptions options)
            {
                Exists(actualXml, searchedItem, expectedValue, options, null, null);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that it has the expected value.
            /// </summary>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedItem">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/> or <paramref name="searchedItem"/> is null.</exception>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(string, IXmlPath, string, object[])"/>.
            /// </para>
            /// </remarks>
            public static void Exists(string actualXml, IXmlPath searchedItem, string expectedValue, string messageFormat, params object[] messageArgs)
            {
                Exists(actualXml, searchedItem, expectedValue, XmlOptions.Default, messageFormat, messageArgs);
            }

            /// <summary>
            /// Asserts that the XML fragment contains the searched element or attribute, and that it has the expected value.
            /// </summary>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="searchedItem">The path of the searched element or attribute in the XML fragment.</param>
            /// <param name="expectedValue">The expected value of the searched item (element or attribute).</param>
            /// <param name="options">Options for the search.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualXml"/>, <paramref name="searchedItem"/>, or <paramref name="options"/> is null.</exception>
            /// <remarks>
            /// <para>
            /// If <paramref name="expectedValue"/> is set to <c>null</c>, the assertion behaves like <see cref="Assert.Xml.Exists(string, IXmlPath, XmlOptions, string, object[])"/>.
            /// </para>
            /// </remarks>
            public static void Exists(string actualXml, IXmlPath searchedItem, string expectedValue, XmlOptions options, string messageFormat, params object[] messageArgs)
            {
                if (actualXml == null)
                    throw new ArgumentNullException("actualXml");
                if (searchedItem == null)
                    throw new ArgumentNullException("searchedItem");
                if (options == null)
                    throw new ArgumentNullException("options");

                AssertionHelper.Verify(() =>
                {
                    Document document;

                    try
                    {
                        document = new Parser(actualXml).Run(options.Value);
                    }
                    catch (XmlException exception)
                    {
                        return new AssertionFailureBuilder("Cannot parse the actual XML fragment.")
                            .SetMessage(messageFormat, messageArgs)
                            .AddException(exception)
                            .ToAssertionFailure();
                    }

                    if (document.Contains((XmlPathClosed)searchedItem, expectedValue, options.Value))
                        return null;

                    var builder = new AssertionFailureBuilder("Expected the XML fragment to contain the searched XML element or attribute, but none was found.")
                        .SetMessage(messageFormat, messageArgs)
                        .AddLabeledValue("Item searched", searchedItem.ToString());

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
