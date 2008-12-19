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
using Gallio.Framework.Pattern;
using MbUnit.Framework.ContractVerifiers.Patterns;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// Abstract base class for field-level contract verifier.
    /// </summary>
    public abstract class AbstractContractVerifier : IContractVerifier
    {
        /// <summary>
        /// Provides builders of pattern tests for the contract verifier.
        /// </summary>
        /// <returns>An enumeration of pattern test builders.</returns>
        public abstract IEnumerable<ContractVerifierPattern> GetContractPatterns();

        /// <summary>
        /// Gets the interface of a particular type if it is implemented by another type,
        /// otherwise returns null.
        /// </summary>
        /// <param name="implementationType">The implementation type</param>
        /// <param name="interfaceType">The interface type</param>
        /// <returns>The interface type or null if it is not implemented by the implementation type</returns>
        protected static Type GetInterface(Type implementationType, Type interfaceType)
        {
            return interfaceType.IsAssignableFrom(implementationType) ? interfaceType : null;
        }
    }
}
