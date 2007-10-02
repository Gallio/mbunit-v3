// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using MbUnit.Framework;
using MbUnit.Framework.Kernel.ExecutionLogs;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Runtime;

namespace MbUnit.Core.Runtime
{
    /// <summary>
    /// The core context manager extends <see cref="IContextManager" /> with methods
    /// to support the creation and disposal of contexts.
    /// </summary>
    public interface ICoreContextManager : IContextManager
    {
        /// <summary>
        /// Initializes the root context of the context manager with a new context
        /// created from the root step.
        /// </summary>
        /// <remarks>
        /// Subsequent attempts to access <see cref="IContextManager.RootContext" /> will
        /// the new root context as returned by this method.
        /// </remarks>
        /// <param name="serviceProvider">The context service provider</param>
        /// <returns>The new root context</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceProvider"/> is null</exception>
        Context InitializeRootContext(ICoreContextServiceProvider serviceProvider);

        /// <summary>
        /// Disposes the root context if there is one.
        /// </summary>
        /// <remarks>
        /// Subsequent attempts to access <see cref="IContextManager.RootContext" /> will return null.
        /// </remarks>
        void DisposeRootContext();

        /// <summary>
        /// Creates a new child context.
        /// </summary>
        /// <param name="parent">The parent context</param>
        /// <param name="serviceProvider">The context service provider</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parent"/>
        /// or <paramref name="serviceProvider"/> is null</exception>
        Context CreateChildContext(Context parent, ICoreContextServiceProvider serviceProvider);

        /// <summary>
        /// Disposes a context.
        /// </summary>
        /// <param name="context">The context to dispose</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is null</exception>
        void DisposeContext(Context context);
    }
}
