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
using System.Text;
using Gallio.Framework.Assertions;

namespace MbUnit.Framework
{
    public abstract partial class Assert
    {
        #region IsAssignableFrom
        /// <summary>
        /// Asserts that an object may be assigned a  value of a given Type.
        /// </summary>
        /// <param name="expectedType">The Type to compare with the object's Type</param>
        /// <param name="actualValue">The object under examination</param>
        public static void IsAssignableFrom(Type expectedType, object actualValue)
        {
            IsAssignableFrom(expectedType, actualValue, null, null);
        }

        /// <summary>
        /// Asserts that an object may be assigned a  value of a given Type.
        /// </summary>
        /// <param name="expectedType">The Type to compare with the object's Type</param>
        /// <param name="actualValue">The object under examination</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        public static void IsAssignableFrom(Type expectedType, object actualValue, string messageFormat, params object[] messageArgs)
        {
            if (expectedType == null)
                throw new ArgumentNullException("expectedType");
            if (actualValue == null)
                throw new ArgumentNullException("actualValue");

            AssertionHelper.Verify(delegate
            {
                if (actualValue.GetType().IsAssignableFrom(expectedType))
                    return null;

                return new AssertionFailureBuilder("Expected the actual type to be assignable to the expected type.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Actual Type", actualValue.GetType())
                    .AddRawLabeledValue("Expected Type", expectedType)
                    .ToAssertionFailure();
            });
        }

        #endregion

        #region IsNotAssignableFrom
        /// <summary>
        /// Asserts that an object may be assigned a  value of a given Type.
        /// </summary>
        /// <param name="expectedType">The Type to compare with the object's Type</param>
        /// <param name="actualValue">The object under examination</param>
        public static void IsNotAssignableFrom(Type expectedType, object actualValue)
        {
            IsNotAssignableFrom(expectedType, actualValue, null, null);
        }

        /// <summary>
        /// Asserts that an object may be assigned a  value of a given Type.
        /// </summary>
        /// <param name="expectedType">The Type to compare with the object's Type</param>
        /// <param name="actualValue">The object under examination</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        public static void IsNotAssignableFrom(Type expectedType, object actualValue, string messageFormat, params object[] messageArgs)
        {
            if (expectedType == null)
                throw new ArgumentNullException("expectedType");
            if (actualValue == null)
                throw new ArgumentNullException("actualValue");

            AssertionHelper.Verify(delegate
            {
                if (!actualValue.GetType().IsAssignableFrom(expectedType))
                    return null;

                return new AssertionFailureBuilder("Expected the actual type not to be assignable to the expected type.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Actual Type", actualValue.GetType())
                    .AddRawLabeledValue("Expected Type", expectedType)
                    .ToAssertionFailure();
            });
        }

        #endregion

        #region IsInstanceOfType
        /// <summary>
        /// Verifies that an actual value is an instance of some expected type.
        /// </summary>
        /// <param name="expectedType">The expected type</param>
        /// <param name="actualValue">The actual value</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedType"/> is null</exception>
        public static void IsInstanceOfType(Type expectedType, object actualValue)
        {
            IsInstanceOfType(expectedType, actualValue, null, null);
        }

        /// <summary>
        /// Verifies that an actual value is an instance of some expected type.
        /// </summary>
        /// <param name="expectedType">The expected type</param>
        /// <param name="actualValue">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expectedType"/> is null</exception>
        public static void IsInstanceOfType(Type expectedType, object actualValue, string messageFormat, params object[] messageArgs)
        {
            if (expectedType == null)
                throw new ArgumentNullException("expectedType");

            AssertionHelper.Verify(delegate
            {
                if (expectedType.IsInstanceOfType(actualValue))
                    return null;

                AssertionFailureBuilder builder = new AssertionFailureBuilder("Expected value to be an instance of a particular type.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Expected Type", expectedType);
                if (actualValue != null)
                    builder.AddRawLabeledValue("ActualType", actualValue.GetType());
                return builder
                    .AddRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion

        #region IsNotInstanceOfType
        /// <summary>
        /// Verifies that an actual value is not an instance of some unexpected type.
        /// </summary>
        /// <param name="unexpectedType">The unexpected type</param>
        /// <param name="actualValue">The actual value</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="unexpectedType"/> is null</exception>
        public static void IsNotInstanceOfType(Type unexpectedType, object actualValue)
        {
            IsNotInstanceOfType(unexpectedType, actualValue, null, null);
        }

        /// <summary>
        /// Verifies that an actual value is not an instance of some unexpected type.
        /// </summary>
        /// <param name="unexpectedType">The unexpected type</param>
        /// <param name="actualValue">The actual value</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="unexpectedType"/> is null</exception>
        public static void IsNotInstanceOfType(Type unexpectedType, object actualValue, string messageFormat, params object[] messageArgs)
        {
            if (unexpectedType == null)
                throw new ArgumentNullException("unexpectedType");

            AssertionHelper.Verify(delegate
            {
                if (!unexpectedType.IsInstanceOfType(actualValue))
                    return null;

                AssertionFailureBuilder builder = new AssertionFailureBuilder("Expected value to not be an instance of a particular type.")
                    .SetMessage(messageFormat, messageArgs)
                    .AddRawLabeledValue("Unexpected Type", unexpectedType);
                if (actualValue != null)
                    builder.AddRawLabeledValue("ActualType", actualValue.GetType());
                return builder
                    .AddRawActualValue(actualValue)
                    .ToAssertionFailure();
            });
        }
        #endregion
    }
}
