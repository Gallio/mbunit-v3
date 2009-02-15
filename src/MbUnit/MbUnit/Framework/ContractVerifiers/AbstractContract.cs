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
using System.Reflection;
using Gallio.Framework.Assertions;
using Gallio.Model.Diagnostics;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// Abstract base class for contracts.
    /// </summary>
    [TestFrameworkInternal]
    public abstract class AbstractContract : IContract
    {
        /// <inheritdoc />
        public abstract IEnumerable<Test> GetContractVerificationTests();

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

        /// <summary>
        /// Verifies that the specified method info object is not null, indicating that the
        /// method exists, otherwise raises an assertion failure and describes the expected method signature.
        /// </summary>
        /// <param name="method">The method, or null if missing</param>
        /// <param name="methodSignature">The expected method signature for diagnostic output</param>
        protected static void AssertMethodExists(MethodInfo method, string methodSignature)
        {
            AssertionHelper.Verify(() =>
            {
                if (method != null)
                    return null;

                return new AssertionFailureBuilder("Expected method to exist.")
                    .AddLabeledValue("Expected Method", methodSignature)
                    .ToAssertionFailure();
            });
        }
    }
}
