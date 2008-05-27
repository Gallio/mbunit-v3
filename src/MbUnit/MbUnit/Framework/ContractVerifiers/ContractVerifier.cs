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

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// <para>
    /// An abstract base class for test fixtures that verify contracts.
    /// </para>
    /// <para>
    /// A contract verifier embodied tests designed to ensure that a particular type meets
    /// a particular contract.  To use a contract verifier, create a subclass of the
    /// appropriate subclass of <see cref="ContractVerifier{T}" /> and implement the required
    /// methods.  The subclass will be a test fixture that verifies the specified contract.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type that implements the contract that is to be verified</typeparam>
    [TestFixture]
    public abstract class ContractVerifier<T>
    {
    }
}
