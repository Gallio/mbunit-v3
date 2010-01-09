// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Reflection;
using Gallio.Common.Diagnostics;

namespace MbUnit.Framework.ContractVerifiers.Core
{
    /// <summary>
    /// Execution context of a contract verification test.
    /// </summary>
    public sealed class ContractVerificationContext
    {
        private readonly ICodeElementInfo codeElement;

        /// <summary>
        /// Gets the code element for the contract read-only field.
        /// </summary>
        public ICodeElementInfo CodeElement
        {
            get
            {
                return codeElement;
            }
        }

        /// <summary>
        /// Constructs an execution context for the verification tests of a contract verifier.
        /// </summary>
        /// <param name="codeElement">The code element for the contract read-only field.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null.</exception>
        public ContractVerificationContext(ICodeElementInfo codeElement)
        {
            if (codeElement == null)
                throw new ArgumentNullException("codeElement");

            this.codeElement = codeElement;
        }

        /// <summary>
        /// Gets an artificial stack trace data that points to 
        /// the contract read-only field.
        /// </summary>
        /// <returns>An artificial stack trace data that points to the contract read-only field.</returns>
        public StackTraceData GetStackTraceData()
        {
            return new StackTraceData(codeElement);
        }
    }
}
