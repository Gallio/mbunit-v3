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

namespace MbUnit.Framework.ContractVerifiers.Patterns
{
    /// <summary>
    /// Represents the run-time state of a single instance of a 
    /// test pattern that is to be executed in the scope
    /// of contract verifier.
    /// </summary>
    public interface IContractVerifierPatternInstanceState
    {
        /// <summary>
        /// Gets othe test fixture type or null if none.
        /// </summary>
        Type FixtureType
        {
            get;
        }

        /// <summary>
        /// Gets the test fixture instance or null if none.
        /// </summary>
        object FixtureInstance
        {
            get;
        }
    }
}
