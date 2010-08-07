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
using System.Runtime.Serialization;
using Gallio.Common.Diagnostics;
using Gallio.Model;

namespace MbUnit.Framework.ContractVerifiers.Core
{
    /// <summary>
    /// This exception type is used to signal that a <see cref="HashStore"/> was
    /// not initialized with a sufficient number of hash code values.
    /// </summary>
    [Serializable]
    internal class NotEnoughHashesException : Exception
    {
        /// <summary>
        /// Creates an exception specifying the expected minimum and the
        /// actual number of hash code values.
        /// </summary>
        public NotEnoughHashesException(int minimumExpected, int actual)
            : base(String.Format("Expected a minimum number of {0} hash codes, but found only {1}.", minimumExpected, actual))
        {
        }

        /// <summary>
        /// Creates a exception from serialization info.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected NotEnoughHashesException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
