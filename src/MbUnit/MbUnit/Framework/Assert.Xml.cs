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
            #region Equality
            
            /// <summary>
            /// Asserts that two XML fragments have the same content.
            /// </summary>
            /// <param name="expectedXmlReader">A reader to get the expected XML fragment.</param>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            public static void AreEqual(TextReader expectedXmlReader, TextReader actualXmlReader)
            {
                AreEqual(expectedXmlReader, actualXmlReader, XmlOptions.Default, null, null);
            }

            /// <summary>
            /// Asserts that two XML fragments have the same content.
            /// </summary>
            /// <param name="expectedXmlReader">A reader to get the expected XML fragment.</param>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="settings">Equality options.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            public static void AreEqual(TextReader expectedXmlReader, TextReader actualXmlReader, XmlOptions settings)
            {
                AreEqual(expectedXmlReader, actualXmlReader, settings, null, null);
            }

            /// <summary>
            /// Asserts that two XML fragments have the same content.
            /// </summary>
            /// <param name="expectedXmlReader">A reader to get the expected XML fragment.</param>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            public static void AreEqual(TextReader expectedXmlReader, TextReader actualXmlReader, string messageFormat, params object[] messageArgs)
            {
                AreEqual(expectedXmlReader, actualXmlReader, XmlOptions.Default, messageFormat, messageArgs);
            }

            /// <summary>
            /// Asserts that two XML fragments have the same content.
            /// </summary>
            /// <param name="expectedXmlReader">A reader to get the expected XML fragment.</param>
            /// <param name="actualXmlReader">A reader to get the actual XML fragment.</param>
            /// <param name="settings">Equality options.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            public static void AreEqual(TextReader expectedXmlReader, TextReader actualXmlReader, XmlOptions settings, string messageFormat, params object[] messageArgs)
            {
                if (expectedXmlReader == null)
                    throw new ArgumentNullException("expectedXmlReader");
                if (actualXmlReader == null)
                    throw new ArgumentNullException("actualXmlReader");

                AreEqual(expectedXmlReader.ReadToEnd(), actualXmlReader.ReadToEnd(), settings, messageFormat, messageArgs);
            }

            /// <summary>
            /// Asserts that two XML fragments have the same content.
            /// </summary>
            /// <param name="expectedXml">The expected XML fragment.</param>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            public static void AreEqual(string expectedXml, string actualXml)
            {
                AreEqual(expectedXml, actualXml, XmlOptions.Default, null, null);
            }

            /// <summary>
            /// Asserts that two XML fragments have the same content.
            /// </summary>
            /// <param name="expectedXml">The expected XML fragment.</param>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="settings">Equality options.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            public static void AreEqual(string expectedXml, string actualXml, XmlOptions settings)
            {
                AreEqual(expectedXml, actualXml, settings, null, null);
            }

            /// <summary>
            /// Asserts that two XML fragments have the same content.
            /// </summary>
            /// <param name="expectedXml">The expected XML fragment.</param>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            public static void AreEqual(string expectedXml, string actualXml, string messageFormat, params object[] messageArgs)
            {
                AreEqual(expectedXml, actualXml, XmlOptions.Default, messageFormat, messageArgs);
            }

            /// <summary>
            /// Asserts that two XML fragments have the same content.
            /// </summary>
            /// <param name="expectedXml">The expected XML fragment.</param>
            /// <param name="actualXml">The actual XML fragment.</param>
            /// <param name="settings">Equality options.</param>
            /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
            /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
            /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
            public static void AreEqual(string expectedXml, string actualXml, XmlOptions settings, string messageFormat, params object[] messageArgs)
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
                        expectedDocument = new Parser(expectedXml).Run(settings.Value);
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
                        actualDocument = new Parser(actualXml).Run(settings.Value);
                    }
                    catch (XmlException exception)
                    {
                        return new AssertionFailureBuilder("Cannot parse the expected XML fragment.")
                            .SetMessage(messageFormat, messageArgs)
                            .AddException(exception)
                            .ToAssertionFailure();
                    }

                    DiffSet diffSet = actualDocument.Diff(expectedDocument, Gallio.Common.Xml.Path.Empty, settings.Value);

                    if (diffSet.IsEmpty)
                        return null;

                    return new AssertionFailureBuilder("Expected XML fragments to be equal according to the specified options.")
                        .SetMessage(messageFormat, messageArgs)
                        .AddRawLabeledValue("Equality Options", settings.Value)
                        .AddInnerFailures(diffSet.ToAssertionFailures())
                        .ToAssertionFailure();
                });
            }

            #endregion
        }
    }
}
