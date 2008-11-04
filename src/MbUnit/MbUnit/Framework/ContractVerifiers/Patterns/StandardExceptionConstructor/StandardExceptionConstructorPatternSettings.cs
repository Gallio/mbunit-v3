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
using System.Collections.ObjectModel;

namespace MbUnit.Framework.ContractVerifiers.Patterns.StandardExceptionConstructor
{
    /// <summary>
    /// Data container which exposes necessary data required to run the test 
    /// pattern <see cref="StandardExceptionConstructorPattern"/>.
    /// </summary>
    internal class StandardExceptionConstructorPatternSettings
    {
        /// <summary>
        /// Gets the target exception type.
        /// </summary>
        public Type TargetExceptionType
        {
            get;
            private set;
        }

        /// <summary>
        /// Determines whether the test pattern must include
        /// verifications about serialization support too.
        /// </summary>
        public bool CheckForSerializationSupport
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a friendly name used to build the full pattern test name.
        /// </summary>
        public string FriendlyName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the parameter types of the constructor.
        /// </summary>
        public IEnumerable<Type> ParameterTypes
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the parameter specifications for constructor invocations.
        /// </summary>
        public IEnumerable<ExceptionConstructorSpec> ConstructorSpecifications
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructs the data container which exposes necessary data required to
        /// run the test pattern <see cref="StandardExceptionConstructorPattern"/>.
        /// </summary>
        /// <param name="targetExceptionType">The target exception type.</param>
        /// <param name="checkForSerializationSupport">Inclusion of verifications about serialization support.</param>
        /// <param name="friendlyName">A friendly name for the pattern test.</param>
        /// <param name="parameterTypes">The parameter types of the searched constructor.</param>
        /// <param name="constructorSpecifications">The parameter specifications for constructor invocations.</param>
        public StandardExceptionConstructorPatternSettings(
            Type targetExceptionType, bool checkForSerializationSupport, string friendlyName, 
            IEnumerable<Type> parameterTypes,
            IEnumerable<ExceptionConstructorSpec> constructorSpecifications)
        {
            if (targetExceptionType == null)
            {
                throw new ArgumentNullException("targetExceptionType");
            }

            if (friendlyName == null)
            {
                throw new ArgumentNullException("friendlyName");
            }

            if (parameterTypes == null)
            {
                throw new ArgumentNullException("parameterTypes");
            }

            if (constructorSpecifications == null)
            {
                throw new ArgumentNullException("constructorSpecifications");
            }

            this.TargetExceptionType = targetExceptionType;
            this.CheckForSerializationSupport = checkForSerializationSupport;
            this.FriendlyName = String.Format("Is{0}ConstructorWellDefined", friendlyName);
            this.ParameterTypes = new ReadOnlyCollection<Type>(new List<Type>(parameterTypes));
            this.ConstructorSpecifications = new ReadOnlyCollection<ExceptionConstructorSpec>(new List<ExceptionConstructorSpec>(constructorSpecifications));
        }
    }
}
