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
    /// Builder for the test pattern <see cref="StandardExceptionConstructorPattern{TException}"/>
    /// </summary>
    /// <typeparam name="TException">The target custom exception type.</typeparam>
    internal class StandardExceptionConstructorPatternBuilder<TException> : ContractVerifierPatternBuilder
        where TException : Exception
    {
        private bool checkForSerializationSupport = true;
        private string friendlyName = String.Empty;
        private List<Type> parameterTypes = new List<Type>();
        private List<ExceptionConstructorSpec> constructorSpecifications = new List<ExceptionConstructorSpec>();

        /// <summary>
        /// Sets the value indicates whether the test pattern must include some
        /// verifications regarding serialization support as well.
        /// If not specified, the default value is true.
        /// </summary>
        /// <param name="enabled">True to activate serialization support verifications; false otherwise.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal StandardExceptionConstructorPatternBuilder<TException> SetCheckForSerializationSupport(bool enabled)
        {
            this.checkForSerializationSupport = enabled;
            return this;
        }

        /// <summary>
        /// Sets the friendly name used to build the full test pattern name.
        /// </summary>
        /// <param name="friendlyName">A friendly name.</param>
        /// <returns>A reference to the builder itself.</returns>
        internal StandardExceptionConstructorPatternBuilder<TException> SetFriendlyName(string friendlyName)
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
        internal StandardExceptionConstructorPatternBuilder<TException> SetParameterTypes(params Type[] parameterTypes)
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
        internal StandardExceptionConstructorPatternBuilder<TException> SetConstructorSpecifications(params ExceptionConstructorSpec[] constructorSpecifications)
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
            return new StandardExceptionConstructorPattern<TException>(
                new StandardExceptionConstructorPatternSettings(
                    checkForSerializationSupport, friendlyName,
                    parameterTypes, constructorSpecifications));
        }
    }
}