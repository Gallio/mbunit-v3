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
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Gallio;
using Gallio.Framework.Assertions;

namespace MbUnit.Framework
{
    public abstract partial class Assert
    {
        #region IsSerializableType
        /// <summary>
        /// Verifies that a type supports the runtime serialization protocol.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Ensures that the type has the <see cref="SerializableAttribute" />.  Also ensures that if it
        /// implements <see cref="ISerializable" /> then it also has a deserialization constructor with
        /// signature .ctor(SerializationInfo info, StreamingContext context).
        /// </para>
        /// </remarks>
        /// <param name="type">The type to verify</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null</exception>
        public static void IsSerializableType(Type type)
        {
            IsSerializableType(type, null);
        }

        /// <summary>
        /// Verifies that a type supports the runtime serialization protocol.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Ensures that the type has the <see cref="SerializableAttribute" />.  Also ensures that if it
        /// implements <see cref="ISerializable" /> then it also has a deserialization constructor with
        /// signature .ctor(SerializationInfo, StreamingContext).
        /// </para>
        /// </remarks>
        /// <param name="type">The type to verify</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null</exception>
        public static void IsSerializableType(Type type, string messageFormat, params object[] messageArgs)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            AssertionHelper.Verify(delegate
            {
                if (!type.IsDefined(typeof(SerializableAttribute), false))
                {
                    return new AssertionFailureBuilder(
                        "Expected the type to support binary serialization but it lacks the [Serializable] attribute.")
                        .SetMessage(messageFormat, messageArgs)
                        .AddRawLabeledValue("Type", type)
                        .ToAssertionFailure();
                }

                if (typeof(ISerializable).IsAssignableFrom(type) && ! HasDeserializationConstructor(type))
                {
                    return new AssertionFailureBuilder(
                        "Expected the type to support binary serialization but it implements ISerializable and is missing a deserialization constructor with signature .ctor(SerializationInfo, StreamingContext).")
                        .SetMessage(messageFormat, messageArgs)
                        .AddRawLabeledValue("Type", type)
                        .ToAssertionFailure();
                }

                return null;
            });
        }

        private static bool HasDeserializationConstructor(Type type)
        {
            return type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null, new[] { typeof(SerializationInfo), typeof(StreamingContext) }, null) != null;
        }
        #endregion

        #region Serialize
        /// <summary>
        /// Verifies that an object can be serialized to a stream using the specified <see cref="IFormatter" />
        /// and returns the resulting stream.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion fails if <paramref name="value"/> is null.
        /// </para>
        /// </remarks>
        /// <param name="value">The value</param>
        /// <param name="formatter">The object serialization formatter</param>
        /// <returns>The serialized stream</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatter"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static MemoryStream Serialize(object value, IFormatter formatter)
        {
            return Serialize(value, formatter, null);
        }

        /// <summary>
        /// Verifies that an object can be serialized to a stream using the specified <see cref="IFormatter" />
        /// and returns the resulting stream.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion fails if <paramref name="value"/> is null.
        /// </para>
        /// </remarks>
        /// <param name="value">The value</param>
        /// <param name="formatter">The object serialization formatter</param>
        /// <returns>The serialized stream</returns>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatter"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static MemoryStream Serialize(object value, IFormatter formatter, string messageFormat, params object[] messageArgs)
        {
            if (formatter == null)
                throw new ArgumentNullException("formatter");

            MemoryStream serializationStream = new MemoryStream();
            AssertionHelper.Verify(delegate
            {
                if (value == null)
                {
                    return new AssertionFailureBuilder("Could not serialize the value because it is null.")
                        .SetMessage(messageFormat, messageArgs)
                        .ToAssertionFailure();
                }

                try
                {
                    formatter.Serialize(serializationStream, value);
                    serializationStream.Position = 0;
                    return null;
                }
                catch (Exception ex)
                {
                    return new AssertionFailureBuilder("Could not serialize the value.")
                        .SetMessage(messageFormat, messageArgs)
                        .AddException(ex)
                        .AddRawLabeledValue("Value", value)
                        .ToAssertionFailure();
                }
            });

            return serializationStream;
        }
        #endregion

        #region Deserialize
        /// <summary>
        /// Verifies that an object can be deserialized from a stream using the
        /// specified <see cref="IFormatter" /> and returns the resulting deserialized object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion fails if <paramref name="stream"/> is null.
        /// </para>
        /// </remarks>
        /// <param name="stream">The stream to deserialize</param>
        /// <param name="formatter">The object serialization formatter</param>
        /// <returns>The deserialized object</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatter"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static T Deserialize<T>(Stream stream, IFormatter formatter)
        {
            return Deserialize<T>(stream, formatter, null);
        }

        /// <summary>
        /// Verifies that an object can be deserialized from a stream using the specified
        /// <see cref="IFormatter"/> and returns the resulting deserialized object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion fails if <paramref name="stream"/> is null.
        /// </para>
        /// </remarks>
        /// <param name="stream">The stream to deserialize</param>
        /// <param name="formatter">The object serialization formatter</param>
        /// <returns>The deserialized object</returns>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatter"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static T Deserialize<T>(Stream stream, IFormatter formatter, string messageFormat, params object[] messageArgs)
        {
            return Deserialize<T>(stream, formatter, messageFormat, messageArgs, null);
        }

        private static T Deserialize<T>(Stream stream, IFormatter formatter, string messageFormat, object[] messageArgs, Func<T> originalValueProvider)
        {
            if (formatter == null)
                throw new ArgumentNullException("formatter");

            T deserializedValue = default(T);
            AssertionHelper.Verify(delegate
            {
                if (stream == null)
                {
                    return new AssertionFailureBuilder("Could not deserialize the value because the stream is null.")
                        .SetMessage(messageFormat, messageArgs)
                        .ToAssertionFailure();
                }

                try
                {
                    deserializedValue = (T)formatter.Deserialize(stream);
                }
                catch (Exception ex)
                {
                    var failureBuilder = new AssertionFailureBuilder("Could not deserialize the value.")
                        .SetMessage(messageFormat, messageArgs)
                        .AddException(ex);
                    if (originalValueProvider != null)
                        failureBuilder.AddRawLabeledValue("Value", originalValueProvider());
                    return failureBuilder.ToAssertionFailure();
                }

                return null;
            });

            return deserializedValue;
        }
        #endregion

        #region SerializeThenDeserialize
        /// <summary>
        /// Verifies that an object can be serialized then deserialized to and from a stream using the
        /// specified <see cref="IFormatter" /> and returns the resulting deserialized object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion fails if <paramref name="value"/> is null.
        /// </para>
        /// </remarks>
        /// <param name="value">The value</param>
        /// <param name="formatter">The object serialization formatter</param>
        /// <returns>The serialized then deserialized object</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatter"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static T SerializeThenDeserialize<T>(T value, IFormatter formatter)
        {
            return SerializeThenDeserialize(value, formatter, null);
        }

        /// <summary>
        /// Verifies that an object can be serialized and deserialized to and from a stream using the
        /// specified <see cref="IFormatter" /> and returns the resulting deserialized object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion fails if <paramref name="value"/> is null.
        /// </para>
        /// </remarks>
        /// <param name="value">The value</param>
        /// <param name="formatter">The object serialization formatter</param>
        /// <returns>The serialized then deserialized object</returns>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatter"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static T SerializeThenDeserialize<T>(T value, IFormatter formatter, string messageFormat, params object[] messageArgs)
        {
            MemoryStream stream = Serialize(value, formatter, messageFormat, messageArgs);
            return Deserialize(stream, formatter, messageFormat, messageArgs, () => value);
        }
        #endregion

        #region BinarySerialize
        /// <summary>
        /// Verifies that an object can be serialized to a stream using a <see cref="BinaryFormatter" />
        /// and returns the resulting stream.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion fails if <paramref name="value"/> is null.
        /// </para>
        /// </remarks>
        /// <param name="value">The value</param>
        /// <returns>The serialized stream</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static MemoryStream BinarySerialize(object value)
        {
            return BinarySerialize(value, null);
        }

        /// <summary>
        /// Verifies that an object can be serialized to a stream using a <see cref="BinaryFormatter" />
        /// and returns the resulting stream.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion fails if <paramref name="value"/> is null.
        /// </para>
        /// </remarks>
        /// <param name="value">The value</param>
        /// <returns>The serialized stream</returns>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static MemoryStream BinarySerialize(object value, string messageFormat, params object[] messageArgs)
        {
            return Serialize(value, CreateBinaryFormatter(), messageFormat, messageArgs);
        }
        #endregion

        #region BinaryDeserialize
        /// <summary>
        /// Verifies that an object can be deserialized from a stream using a
        /// <see cref="BinaryFormatter" /> and returns the resulting deserialized object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion fails if <paramref name="stream"/> is null.
        /// </para>
        /// </remarks>
        /// <param name="stream">The stream to deserialize</param>
        /// <returns>The deserialized object</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static T BinaryDeserialize<T>(Stream stream)
        {
            return BinaryDeserialize<T>(stream, null);
        }

        /// <summary>
        /// Verifies that an object can be deserialized from a stream using a
        /// <see cref="BinaryFormatter" /> and returns the resulting deserialized object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion fails if <paramref name="stream"/> is null.
        /// </para>
        /// </remarks>
        /// <param name="stream">The stream to deserialize</param>
        /// <returns>The deserialized object</returns>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static T BinaryDeserialize<T>(Stream stream, string messageFormat, params object[] messageArgs)
        {
            return Deserialize<T>(stream, CreateBinaryFormatter(), messageFormat, messageArgs);
        }
        #endregion

        #region BinarySerializeThenDeserialize
        /// <summary>
        /// Verifies that an object can be serialized then deserialized to and from a stream using a
        /// <see cref="BinaryFormatter" /> and returns the resulting deserialized object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion fails if <paramref name="value"/> is null.
        /// </para>
        /// </remarks>
        /// <param name="value">The value</param>
        /// <returns>The serialized then deserialized object</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static T BinarySerializeThenDeserialize<T>(T value)
        {
            return BinarySerializeThenDeserialize(value, null);
        }

        /// <summary>
        /// Verifies that an object can be serialized and deserialized to and from a stream using a
        /// <see cref="BinaryFormatter" /> and returns the resulting deserialized object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion fails if <paramref name="value"/> is null.
        /// </para>
        /// </remarks>
        /// <param name="value">The value</param>
        /// <returns>The serialized then deserialized object</returns>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static T BinarySerializeThenDeserialize<T>(T value, string messageFormat, params object[] messageArgs)
        {
            return SerializeThenDeserialize<T>(value, CreateBinaryFormatter(), messageFormat, messageArgs);
        }
        #endregion

        #region IsXmlSerializableType
        /// <summary>
        /// Verifies that a type supports the Xml serialization protocol used by <see cref="XmlSerializer" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Ensures that an <see cref="XmlSerializer"/> for the type can be constructed without error.
        /// </para>
        /// </remarks>
        /// <param name="type">The type to verify</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null</exception>
        public static void IsXmlSerializableType(Type type)
        {
            IsXmlSerializableType(type, null);
        }

        /// <summary>
        /// Verifies that a type supports the Xml serialization protocol used by <see cref="XmlSerializer" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Ensures that an <see cref="XmlSerializer"/> for the type can be constructed without error.
        /// </para>
        /// </remarks>
        /// <param name="type">The type to verify</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null</exception>
        public static void IsXmlSerializableType(Type type, string messageFormat, params object[] messageArgs)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            AssertionHelper.Verify(delegate
            {
                try
                {
                    new XmlSerializer(type);
                    return null;
                }
                catch (Exception ex)
                {
                    return new AssertionFailureBuilder("Expected the type to support Xml serialization but an exception was thrown while constructing an XmlSerializer.")
                        .SetMessage(messageFormat, messageArgs)
                        .AddException(ex)
                        .AddRawLabeledValue("Type", type)
                        .ToAssertionFailure();
                }
            });
        }
        #endregion

        #region XmlSerialize
        /// <summary>
        /// Verifies that an object can be serialized to Xml using an
        /// <see cref="XmlSerializer" /> and returns the resulting Xml as a string.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion fails if <paramref name="value"/> is null.
        /// </para>
        /// </remarks>
        /// <param name="value">The value</param>
        /// <returns>The serialized Xml</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static string XmlSerialize(object value)
        {
            return XmlSerialize(value, null);
        }

        /// <summary>
        /// Verifies that an object can be serialized to Xml using
        /// an <see cref="XmlSerializer" /> and returns the resulting Xml as a string.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion fails if <paramref name="value"/> is null.
        /// </para>
        /// </remarks>
        /// <param name="value">The value</param>
        /// <returns>The serialized Xml</returns>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static string XmlSerialize(object value, string messageFormat, params object[] messageArgs)
        {
            StringBuilder xml = new StringBuilder();

            AssertionHelper.Verify(delegate
            {
                if (value == null)
                {
                    return new AssertionFailureBuilder("Could not serialize the value because it is null.")
                        .SetMessage(messageFormat, messageArgs)
                        .ToAssertionFailure();
                }

                // TODO: Allow the user to specify Xml serialization options.
                using (XmlWriter writer = XmlWriter.Create(xml, new XmlWriterSettings()
                {
                    CheckCharacters = false,
                    CloseOutput = false,
                    OmitXmlDeclaration = true,
                    Indent = false,
                    NewLineChars = "\n"
                }))
                {
                    try
                    {
                        var serializer = new XmlSerializer(value.GetType());
                        serializer.Serialize(writer, value);
                        return null;
                    }
                    catch (Exception ex)
                    {
                        return new AssertionFailureBuilder("Could not serialize the value.")
                            .SetMessage(messageFormat, messageArgs)
                            .AddException(ex)
                            .AddRawLabeledValue("Value", value)
                            .ToAssertionFailure();
                    }
                }
            });

            return xml.ToString();
        }
        #endregion

        #region XmlDeserialize
        /// <summary>
        /// Verifies that an object can be deserialized from Xml using
        /// an <see cref="XmlSerializer" /> and returns the resulting deserialized object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion fails if <paramref name="xml"/> is null.
        /// </para>
        /// </remarks>
        /// <param name="xml">The Xml</param>
        /// <returns>The deserialized object</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static T XmlDeserialize<T>(string xml)
        {
            return XmlDeserialize<T>(xml, null);
        }

        /// <summary>
        /// Verifies that an object can be deserialized from Xml using
        /// an <see cref="XmlSerializer" /> and returns the resulting deserialized object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion fails if <paramref name="xml"/> is null.
        /// </para>
        /// </remarks>
        /// <param name="xml">The Xml</param>
        /// <returns>The deserialized object</returns>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static T XmlDeserialize<T>(string xml, string messageFormat, params object[] messageArgs)
        {
            return XmlDeserialize<T>(typeof(T), xml, messageFormat, messageArgs, null);
        }

        private static T XmlDeserialize<T>(Type type, string xml, string messageFormat, object[] messageArgs, Func<T> originalValueProvider)
        {
            T deserializedValue = default(T);

            AssertionHelper.Verify(delegate
            {
                if (xml == null)
                {
                    return new AssertionFailureBuilder("Could not deserialize the value because the xml is null.")
                        .SetMessage(messageFormat, messageArgs)
                        .ToAssertionFailure();
                }

                // TODO: Allow the user to specify Xml serialization options.
                using (XmlReader reader = XmlReader.Create(new StringReader(xml), new XmlReaderSettings()
                {
                    CheckCharacters = false,
                    CloseInput = false,
                    ProhibitDtd = true
                }))
                {
                    try
                    {
                        var serializer = new XmlSerializer(type);
                        deserializedValue = (T) serializer.Deserialize(reader);
                        return null;
                    }
                    catch (Exception ex)
                    {
                        var failureBuilder = new AssertionFailureBuilder("Could not deserialize the value.")
                            .SetMessage(messageFormat, messageArgs)
                            .AddException(ex);
                        if (originalValueProvider != null)
                            failureBuilder.AddRawLabeledValue("Value", originalValueProvider());
                        return failureBuilder
                            .AddRawLabeledValue("Xml", xml)
                            .ToAssertionFailure();
                    }
                }
            });

            return deserializedValue;
        }
        #endregion

        #region XmlSerializeThenDeserialize
        /// <summary>
        /// Verifies that an object can be serialized then deserialized to and from Xml using
        /// an <see cref="XmlSerializer" /> and returns the resulting deserialized object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion fails if <paramref name="value"/> is null.
        /// </para>
        /// </remarks>
        /// <param name="value">The value</param>
        /// <returns>The serialized then deserialized object</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static T XmlSerializeThenDeserialize<T>(T value)
        {
            return XmlSerializeThenDeserialize(value, null);
        }

        /// <summary>
        /// Verifies that an object can be serialized then deserialized to and from Xml using
        /// an <see cref="XmlSerializer" /> and returns the resulting deserialized object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The assertion fails if <paramref name="value"/> is null.
        /// </para>
        /// </remarks>
        /// <param name="value">The value</param>
        /// <returns>The serialized then deserialized object</returns>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static T XmlSerializeThenDeserialize<T>(T value, string messageFormat, params object[] messageArgs)
        {
            string xml = XmlSerialize(value, messageFormat, messageArgs);
            return XmlDeserialize(value != null ? value.GetType() : typeof(T), xml, messageFormat, messageArgs, () => value);
        }
        #endregion

        private static BinaryFormatter CreateBinaryFormatter()
        {
            return new BinaryFormatter()
            {
                FilterLevel = TypeFilterLevel.Full
            };
        }
    }
}
