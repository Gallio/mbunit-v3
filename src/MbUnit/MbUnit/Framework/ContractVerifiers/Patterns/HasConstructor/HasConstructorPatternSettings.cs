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

namespace MbUnit.Framework.ContractVerifiers.Patterns.HasConstructor
{
    /// <summary>
    /// Data container which exposes necessary data required to
    /// run the test pattern <see cref="HasConstructorPattern"/>.
    /// </summary>
    internal class HasConstructorPatternSettings
    {
        /// <summary>
        /// Gets the target evaluated type.
        /// </summary>
        public Type TargetType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the required constructor accessibility.
        /// </summary>
        public HasConstructorAccessibility Accessibility
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a friendly name used to build the full pattern test name.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the parameter types for the searched constructor.
        /// </summary>
        public IEnumerable<Type> ParameterTypes
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructs the data container which exposes necessary data required to
        /// run the test pattern <see cref="HasConstructorPattern"/>.
        /// </summary>
        /// <param name="targetType">The target evaluated type.</param>
        /// <param name="accessibility">The required accessibility.</param>
        /// <param name="name">A friendly name for the pattern test.</param>
        /// <param name="parameterTypes">The parameter types.</param>
        public HasConstructorPatternSettings(Type targetType, HasConstructorAccessibility accessibility, 
            string name, IEnumerable<Type> parameterTypes)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }

            if (name == null)
            {
                throw new ArgumentNullException("friendlyName");
            }

            if (parameterTypes == null)
            {
                throw new ArgumentNullException("parameterTypes");
            }

            this.TargetType = targetType;
            this.Accessibility = accessibility;
            this.Name = String.Format("Has{0}Constructor", name);
            this.ParameterTypes = new ReadOnlyCollection<Type>(new List<Type>(parameterTypes));
        }
    }
}
