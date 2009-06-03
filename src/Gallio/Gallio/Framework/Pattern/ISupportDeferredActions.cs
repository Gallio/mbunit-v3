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
using Gallio.Common;
using Gallio.Common.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Provides support for performing deferred build actions.
    /// </summary>
    public interface ISupportDeferredActions
    {
        /// <summary>
        /// Registers a deferred action to be performed when <see cref="ApplyDeferredActions" /> is called.
        /// </summary>
        /// <remarks>
        /// Typically used to enable decorations to be applied in a particular order.
        /// </remarks>
        /// <param name="codeElement">The associated code element, used to report errors if the deferred action throws an exception.</param>
        /// <param name="order">The order in which the action should be applied, from least order to greatest.</param>
        /// <param name="deferredAction">The action to perform.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/>
        /// or <paramref name="deferredAction"/> is null.</exception>
        /// <seealso cref="ApplyDeferredActions"/>
        void AddDeferredAction(ICodeElementInfo codeElement, int order, Action deferredAction);

        /// <summary>
        /// Applies all pending deferred in order and clears the list.
        /// </summary>
        void ApplyDeferredActions();
    }
}
