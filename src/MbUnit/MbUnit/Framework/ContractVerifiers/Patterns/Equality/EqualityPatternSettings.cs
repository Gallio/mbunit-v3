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

namespace MbUnit.Framework.ContractVerifiers.Patterns.Equality
{
    /// <summary>
    /// Data container which exposes necessary data required to
    /// run the test pattern <see cref="EqualityPattern{T}"/>.
    /// </summary>
    internal class EqualityPatternSettings
    {
        /// <summary>
        /// Information about a property of the contract verifier
        /// providing a collection of equivalence classes.
        /// </summary>
        public PropertyInfo EquivalentClassSource
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets information about the equality method.
        /// </summary>
        public MethodInfo EqualityMethodInfo
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a friendly description of the equality method signature.
        /// </summary>
        public string SignatureDescription
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Indicates whether the method represents an inequality operation.
        /// </summary>
        public bool Inequality
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a friendly name for the test pattern.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructs the data container which exposes necessary data required to
        /// run the test pattern <see cref="EqualityPattern{T}"/>.
        /// </summary>
        /// <param name="equalityMethodInfo">Information about the equality method.</param>
        /// <param name="signatureDescription">A friendly description of the equality method signature.</param>
        /// <param name="inequality">Indicates whether the method represents an inequality operation.</param>
        /// <param name="name">A friendly name for the test pattern.</param>
        /// <param name="equivalenceClassSource">Information about a property of the 
        /// contract verifier providing a collection of equivalence classes.</param>
        public EqualityPatternSettings(MethodInfo equalityMethodInfo, string signatureDescription, 
            bool inequality, string name, PropertyInfo equivalenceClassSource)
        {
            if (name == null)
            {
                throw new ArgumentNullException("friendlyName");
            }

            if (signatureDescription == null)
            {
                throw new ArgumentNullException("signatureDescription");
            }

            if (equivalenceClassSource == null)
            {
                throw new ArgumentNullException("equivalenceClassSource");
            }

            this.SignatureDescription = signatureDescription;
            this.EqualityMethodInfo = equalityMethodInfo;
            this.Inequality = inequality;
            this.Name = name;
            this.EquivalentClassSource = equivalenceClassSource;
        }
    }
}
