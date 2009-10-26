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
using Gallio.Framework.Assertions;

namespace MbUnit.Framework
{
    public abstract partial class Assert
    {
        #region IsAssignableFrom
        
        /// <summary>
        /// Verifies that a type may be assigned to a variable of the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if <paramref name="actualType"/> is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Type.IsAssignableFrom"/>
        /// <param name="expectedType">The Type to compare with the actual Type.</param>
        /// <param name="actualType">The Type under examination.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedType"/> or <paramref name="actualType"/> is null.</exception>
        public static void IsAssignableFrom(Type expectedType, Type actualType)
        {
            IsAssignableFrom(expectedType, actualType, null, null);
        }

        /// <summary>
        /// Verifies that a type may be assigned to a variable of the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if <paramref name="actualType"/> is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Type.IsAssignableFrom"/>
        /// <param name="expectedType">The Type to compare with the object's Type.</param>
        /// <param name="actualType">The Type under examination.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedType"/> or <paramref name="actualType"/> is null.</exception>
        public static void IsAssignableFrom(Type expectedType, Type actualType, string messageFormat, params object[] messageArgs)
        {
            if (expectedType == null)
                throw new ArgumentNullException("expectedType");

            AssertionHelper.Verify(() =>
            {
                if (actualType != null && actualType.IsAssignableFrom(expectedType))
                    return null;

                return new AssertionFailureBuilder("Expected the actual type to be assignable to the expected type.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Expected Type", expectedType)
                    .AddRawLabeledValue("Actual Type", actualType)
                    .ToAssertionFailure();
            });
        }

        /// <summary>
        /// Verifies that a type may be assigned to a variable of the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if <paramref name="actualType"/> is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Type.IsAssignableFrom"/>
        /// <typeparam name="TExpected">The Type to compare with the object's Type.</typeparam>
        /// <param name="actualType">The object under examination.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualType"/> is null.</exception>
        public static void IsAssignableFrom<TExpected>(Type actualType)
        {
            IsAssignableFrom(typeof(TExpected), actualType, null, null);
        }

        /// <summary>
        /// Verifies that a type may be assigned to a variable of the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if <paramref name="actualType"/> is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Type.IsAssignableFrom"/>
        /// <typeparam name="TExpected">The Type to compare with the object's Type.</typeparam>
        /// <param name="actualType">The Type under examination.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="actualType"/> is null.</exception>
        public static void IsAssignableFrom<TExpected>(Type actualType, string messageFormat, params object[] messageArgs)
        {
            IsAssignableFrom(typeof(TExpected), actualType, messageFormat, messageArgs);
        }

        #endregion

        #region IsNotAssignableFrom

        /// <summary>
        /// Verifies that a Type may not be assigned to a variable of the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if <paramref name="actualType"/> is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Type.IsAssignableFrom"/>
        /// <param name="unexpectedType">The Type to compare with the object's Type.</param>
        /// <param name="actualType">The Type under examination.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="unexpectedType"/> or <paramref name="actualType"/> is null.</exception>
        public static void IsNotAssignableFrom(Type unexpectedType, Type actualType)
        {
            IsNotAssignableFrom(unexpectedType, actualType, null, null);
        }

        /// <summary>
        /// Verifies that a Type may not be assigned to a variable of the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if <paramref name="actualType"/> is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Type.IsAssignableFrom"/>
        /// <param name="unexpectedType">The Type to compare with the object's Type.</param>
        /// <param name="actualType">The Type under examination.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="unexpectedType"/> or <paramref name="actualType"/> is null.</exception>
        public static void IsNotAssignableFrom(Type unexpectedType, Type actualType, string messageFormat, params object[] messageArgs)
        {
            if (unexpectedType == null)
                throw new ArgumentNullException("unexpectedType");

            AssertionHelper.Verify(delegate
            {
                if (actualType != null && !actualType.IsAssignableFrom(unexpectedType))
                    return null;

                return new AssertionFailureBuilder("Expected the actual type not to be assignable to the expected type.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Unexpected Type", unexpectedType)
                    .AddRawLabeledValue("Actual Type", actualType)
                    .ToAssertionFailure();
            });
        }

        /// <summary>
        /// Verifies that a Type may not be assigned to a variable of the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if <paramref name="actualType"/> is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Type.IsAssignableFrom"/>
        /// <typeparam name="TUnexpected">The Type to compare with the object's Type.</typeparam>
        /// <param name="actualType">The Type under examination.</param>
        public static void IsNotAssignableFrom<TUnexpected>(Type actualType)
        {
            IsNotAssignableFrom(typeof(TUnexpected), actualType, null, null);
        }

        /// <summary>
        /// Verifies that a Type may not be assigned to a variable of the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if <paramref name="actualType"/> is null.
        /// </para>
        /// </remarks>
        /// <seealso cref="Type.IsAssignableFrom"/>
        /// <typeparam name="TUnexpected">The Type to compare with the object's Type.</typeparam>
        /// <param name="actualType">The Type under examination.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        public static void IsNotAssignableFrom<TUnexpected>(Type actualType, string messageFormat, params object[] messageArgs)
        {
            IsNotAssignableFrom(typeof(TUnexpected), actualType, messageFormat, messageArgs);
        }

        #endregion

        #region IsInstanceOfType

        /// <summary>
        /// Verifies that an actual value is an instance of some expected type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the object is null.
        /// </para>
        /// </remarks>
        /// <param name="expectedType">The expected type.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedType"/> is null.</exception>
        public static void IsInstanceOfType(Type expectedType, object actualValue)
        {
            IsInstanceOfType(expectedType, actualValue, null, null);
        }

        /// <summary>
        /// Verifies that an actual value is an instance of some expected type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the object is null.
        /// </para>
        /// </remarks>
        /// <param name="expectedType">The expected type.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedType"/> is null.</exception>
        public static void IsInstanceOfType(Type expectedType, object actualValue, string messageFormat, params object[] messageArgs)
        {
            if (expectedType == null)
                throw new ArgumentNullException("expectedType");

            AssertionHelper.Verify(delegate
            {
                if (actualValue != null && expectedType.IsInstanceOfType(actualValue))
                    return null;

                AssertionFailureBuilder builder = new AssertionFailureBuilder("Expected value to be an instance of a particular type.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Expected Type", expectedType);
                if (actualValue != null)
                    builder.AddRawLabeledValue("Actual Type", actualValue.GetType());
                return builder
                    .AddRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }

        /// <summary>
        /// Verifies that an actual value is an instance of some expected type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the object is null.
        /// </para>
        /// </remarks>
        /// <typeparam name="TExpected">The expected type.</typeparam>
        /// <param name="actualValue">The actual value.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void IsInstanceOfType<TExpected>(object actualValue)
        {
            IsInstanceOfType(typeof(TExpected), actualValue, null, null);
        }

        /// <summary>
        /// Verifies that an actual value is an instance of some expected type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the object is null.
        /// </para>
        /// </remarks>
        /// <typeparam name="TExpected">The expected type.</typeparam>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void IsInstanceOfType<TExpected>(object actualValue, string messageFormat, params object[] messageArgs)
        {
            IsInstanceOfType(typeof(TExpected), actualValue, messageFormat, messageArgs);
        }

        #endregion

        #region IsNotInstanceOfType
        
        /// <summary>
        /// Verifies that an actual value is not an instance of some unexpected type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the object is null.
        /// </para>
        /// </remarks>
        /// <param name="unexpectedType">The unexpected type.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="unexpectedType"/> is null.</exception>
        public static void IsNotInstanceOfType(Type unexpectedType, object actualValue)
        {
            IsNotInstanceOfType(unexpectedType, actualValue, null, null);
        }

        /// <summary>
        /// Verifies that an actual value is not an instance of some unexpected type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the object is null.
        /// </para>
        /// </remarks>
        /// <param name="unexpectedType">The unexpected type.</param>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="unexpectedType"/> is null.</exception>
        public static void IsNotInstanceOfType(Type unexpectedType, object actualValue, string messageFormat, params object[] messageArgs)
        {
            if (unexpectedType == null)
                throw new ArgumentNullException("unexpectedType");

            AssertionHelper.Verify(delegate
            {
                if (actualValue != null && !unexpectedType.IsInstanceOfType(actualValue))
                    return null;

                AssertionFailureBuilder builder = new AssertionFailureBuilder("Expected value to not be an instance of a particular type.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Unexpected Type", unexpectedType);
                if (actualValue != null)
                    builder.AddRawLabeledValue("Actual Type", actualValue.GetType());
                return builder
                    .AddRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }

        /// <summary>
        /// Verifies that an actual value is not an instance of some unexpected type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the object is null.
        /// </para>
        /// </remarks>
        /// <typeparam name="TUnexpected">The unexpected type.</typeparam>
        /// <param name="actualValue">The actual value.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void IsNotInstanceOfType<TUnexpected>(object actualValue)
        {
            IsNotInstanceOfType(typeof(TUnexpected), actualValue, null, null);
        }

        /// <summary>
        /// Verifies that an actual value is not an instance of some unexpected type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This assertion will fail if the object is null.
        /// </para>
        /// </remarks>
        /// <typeparam name="TUnexpected">The unexpected type.</typeparam>
        /// <param name="actualValue">The actual value.</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none.</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise.</exception>
        public static void IsNotInstanceOfType<TUnexpected>(object actualValue, string messageFormat, params object[] messageArgs)
        {
            IsNotInstanceOfType(typeof(TUnexpected), actualValue, messageFormat, messageArgs);
        }

        #endregion
    }
}
