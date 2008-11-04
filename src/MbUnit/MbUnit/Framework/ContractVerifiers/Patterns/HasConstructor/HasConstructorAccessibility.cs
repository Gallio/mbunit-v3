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
using System.Reflection;

namespace MbUnit.Framework.ContractVerifiers.Patterns.HasConstructor
{
    /// <summary>
    /// Required accessibility for the constructor searched
    /// by the <see cref="HasConstructorPattern"/>.
    /// </summary>
    internal class HasConstructorAccessibility
    {
        /// <summary>
        /// Gets the binding flags for reflection purpose.
        /// </summary>
        public BindingFlags BindingFlags
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the accessibility name.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        private HasConstructorAccessibility(BindingFlags bindingFlags, string name)
	    {
            this.BindingFlags = bindingFlags;
            this.Name = name;
	    }

        /// <summary>
        /// Public accessibility.
        /// </summary>
        public static HasConstructorAccessibility Public = 
            new HasConstructorAccessibility(BindingFlags.Public, "Public");
        
        /// <summary>
        /// Non-public accessibility (i.e. Protected or Internal)
        /// </summary>
        public static HasConstructorAccessibility NonPublic = 
            new HasConstructorAccessibility(BindingFlags.NonPublic, "Non-Public");
    }
}
