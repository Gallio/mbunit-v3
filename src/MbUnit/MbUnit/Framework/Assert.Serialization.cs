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
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using Gallio;
using Gallio.Framework.Assertions;

namespace MbUnit.Framework
{
    public abstract partial class Assert
    {
        #region IsBinarySerializableType
        /// <summary>
        /// Verifies that a type supports the binary serialization protocol used by <see cref="BinaryFormatter" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Ensures that the type has the <see cref="SerializableAttribute" />.
        /// </para>
        /// </remarks>
        /// <param name="type">The type to verify</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null</exception>
        public static void IsBinarySerializableType(Type type)
        {
            IsBinarySerializableType(type, null);
        }

        /// <summary>
        /// Verifies that a type supports the binary serialization protocol used by <see cref="BinaryFormatter" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Ensures that the type has the <see cref="SerializableAttribute" />.
        /// </para>
        /// </remarks>
        /// <param name="type">The type to verify</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null</exception>
        public static void IsBinarySerializableType(Type type, string messageFormat, params object[] messageArgs)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            AssertionHelper.Verify(delegate
            {
                if (type.IsDefined(typeof(SerializableAttribute), false))
                    return null;

                return new AssertionFailureBuilder("Expected the type to support binary serialization but it lacks the [Serializable] attribute.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Type", type)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region IsBinarySerializable
        /// <summary>
        /// Verifies that an object can be serialized and deserialized using a
        /// <see cref="BinaryFormatter" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the value is null, then the assertion passes and returns null.
        /// </para>
        /// </remarks>
        /// <param name="value">The value</param>
        /// <returns>The deserialized object which may be inspected for additional verification</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static T IsBinarySerializable<T>(T value)
        {
            return IsBinarySerializable(value, null);
        }

        /// <summary>
        /// Verifies that an object can be serialized and deserialized using a
        /// <see cref="BinaryFormatter" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the value is null, then the assertion passes and returns null.
        /// </para>
        /// </remarks>
        /// <param name="value">The value</param>
        /// <returns>The deserialized object which may be inspected for additional verification</returns>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static T IsBinarySerializable<T>(T value, string messageFormat, params object[] messageArgs)
        {
            T deserializedValue = default(T);
            AssertionHelper.Verify(delegate
            {
                if (value == null)
                    return null;

                using (var serializationStream = new MemoryStream())
                {
                    // TODO: Allow the user to specify a particular formatter to use.
                    var formatter = new BinaryFormatter()
                    {
                        FilterLevel = TypeFilterLevel.Full
                    };

                    try
                    {
                        formatter.Serialize(serializationStream, value);
                    }
                    catch (Exception ex)
                    {
                        return new AssertionFailureBuilder("Expected the value to be binary serializable but an exception occurred during serialization.")
                            .SetMessage(messageFormat, messageArgs)
                            .AddException(ex)
                            .AddRawActualValue(value)
                            .ToAssertionFailure();
                    }

                    try
                    {
                        serializationStream.Position = 0;
                        deserializedValue = (T) formatter.Deserialize(serializationStream);
                    }
                    catch (Exception ex)
                    {
                        return new AssertionFailureBuilder("Expected the value to be binary serializable but an exception occurred during deserialization.")
                            .SetMessage(messageFormat, messageArgs)
                            .AddException(ex)
                            .AddRawActualValue(value)
                            .ToAssertionFailure();
                    }

                    return null;
                }
            });

            return deserializedValue;
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
                    return new AssertionFailureBuilder("Expected the type to support Xml serialization but an exception was thrown while constructing and XmlSerializer.")
                        .SetMessage(messageFormat, messageArgs)
                        .AddException(ex)
                        .AddRawLabeledValue("Type", type)
                        .ToAssertionFailure();
                }
            });
        }
        #endregion

        #region IsXmlSerializable
        /// <summary>
        /// Verifies that an object can be serialized and deserialized using an <see cref="XmlSerializer" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the value is null, then the assertion passes and returns null.
        /// </para>
        /// </remarks>
        /// <param name="value">The value</param>
        /// <returns>The deserialized object which may be inspected for additional verification</returns>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static T IsXmlSerializable<T>(T value)
        {
            return IsXmlSerializable(value, null);
        }

        /// <summary>
        /// Verifies that an object can be serialized and deserialized using an <see cref="XmlSerializer" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the value is null, then the assertion passes and returns null.
        /// </para>
        /// </remarks>
        /// <param name="value">The value</param>
        /// <returns>The deserialized object which may be inspected for additional verification</returns>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static T IsXmlSerializable<T>(T value, string messageFormat, params object[] messageArgs)
        {
            T deserializedValue = default(T);
            AssertionHelper.Verify(delegate
            {
                if (value == null)
                    return null;

                using (var serializationStream = new MemoryStream())
                {
                    // TODO: Allow the user to specify Xml serialization options.
                    var serializer = new XmlSerializer(typeof(T));

                    try
                    {
                        serializer.Serialize(serializationStream, value);
                    }
                    catch (Exception ex)
                    {
                        return new AssertionFailureBuilder("Expected the value to be Xml serializable but an exception occurred during serialization.")
                            .SetMessage(messageFormat, messageArgs)
                            .AddException(ex)
                            .AddRawActualValue(value)
                            .ToAssertionFailure();
                    }

                    try
                    {
                        serializationStream.Position = 0;
                        deserializedValue = (T)serializer.Deserialize(serializationStream);
                        return null;
                    }
                    catch (Exception ex)
                    {
                        return new AssertionFailureBuilder("Expected the value to be Xml serializable but an exception occurred during deserialization.")
                            .SetMessage(messageFormat, messageArgs)
                            .AddException(ex)
                            .AddRawActualValue(value)
                            .ToAssertionFailure();
                    }
                }
            });

            return deserializedValue;
        }
        #endregion
    }
}
