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
using MbUnit.Framework.ContractVerifiers.Core;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// Describes a contract that can be verified.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contracts can be composed together into more complex forms by combining their
    /// lists of verification tests to create test suites.  This enables the implementation
    /// of one contract to leverage that of another.
    /// </para>
    /// </remarks>
    public interface IContract
    {
        /// <summary>
        /// Gets an enumeration of test cases whose purpose is to verify the contract.
        /// </summary>
        /// <param name="context">The context of execution.</param>
        /// <returns>An enumeration of tests</returns>
        IEnumerable<Test> GetContractVerificationTests(ContractVerificationContext context);
    }
}
