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

namespace MbUnit.Framework.ContractVerifiers.Patterns.StandardExceptionConstructor
{
    /// <summary>
    /// Builder for the test pattern <see cref="StandardExceptionConstructorPattern"/>
    /// </summary>
    internal class StandardExceptionConstructorPatternBuilder : ContractVerifierPatternBuilder
    {
        private Type targetExceptionType;
        private bool checkForSerializationSupport = true;
        private string friendlyName = String.Empty;
        private List<Type> parameterTypes = new List<Type>();
        private List<ExceptionConstructorSpec> constructorSpecifications = new List<ExceptionConstructorSpec>();

        /// <summary>
        /// Sets the target exception type.
        /// </summary>
        /// <param name="targetExceptionType">The target exception type.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal StandardExceptionConstructorPatternBuilder SetTargetExceptionType(Type targetExceptionType)
        {
            if (targetExceptionType == null)
            {
                throw new ArgumentNullException("targetExceptionType");
            }

            if (!typeof(Exception).IsAssignableFrom(targetExceptionType))
            {
                throw new ArgumentException("The specified type must derive from System.Exception.", "targetExceptionType");
            }

            this.targetExceptionType = targetExceptionType;
            return this;
        }
       
        /// <summary>
        /// Sets the value indicates whether the test pattern must include some
        /// verifications regarding serialization support as well.
        /// If not specified, the default value is true.
        /// </summary>
        /// <param name="enabled">True to activate serialization support verifications; false otherwise.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal StandardExceptionConstructorPatternBuilder SetCheckForSerializationSupport(bool enabled)
        {
            this.checkForSerializationSupport = enabled;
            return this;
        }

        /// <summary>
        /// Sets the friendly name used to build the full test pattern name.
        /// </summary>
        /// <param name="friendlyName">A friendly name.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal StandardExceptionConstructorPatternBuilder SetFriendlyName(string friendlyName)
        {
            if (friendlyName == null)
            {
                throw new ArgumentNullException("friendlyName");
            }

            this.friendlyName = friendlyName;
            return this;
        }

        /// <summary>
        /// Sets the types of the constructor parameters.
        /// </summary>
        /// <param name="parameterTypes">An array of types definining the types
        /// of the constructor parameters.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal StandardExceptionConstructorPatternBuilder SetParameterTypes(params Type[] parameterTypes)
        {
            if (parameterTypes == null)
            {
                throw new ArgumentNullException("parameterTypes");
            }

            this.parameterTypes = new List<Type>(parameterTypes);
            return this;
        }

        /// <summary>
        /// Sets the parameter specifications for the invocations of the constructor.
        /// </summary>
        /// <param name="constructorSpecifications">An array of parameter specifications.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal StandardExceptionConstructorPatternBuilder SetConstructorSpecifications(params ExceptionConstructorSpec[] constructorSpecifications)
        {
            if (constructorSpecifications == null)
            {
                throw new ArgumentNullException("constructorSpecifications");
            }

            this.constructorSpecifications = new List<ExceptionConstructorSpec>(constructorSpecifications);
            return this;
        }

        /// <inheritdoc />
        public override ContractVerifierPattern ToPattern()
        {
            if (targetExceptionType == null)
            {
                throw new InvalidOperationException("The exception target type must be specified.");
            }

            return new StandardExceptionConstructorPattern(
                new StandardExceptionConstructorPatternSettings(
                    targetExceptionType, checkForSerializationSupport, friendlyName,
                    parameterTypes, constructorSpecifications));
        }
    }
}