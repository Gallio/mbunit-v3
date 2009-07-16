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
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Gallio.Common.Linq
{
    /// <summary>
    /// Extension methods for <see cref="Common.Action" /> delegates.
    /// </summary>
    public static class ActionExtensions
    {
        /// <summary>
        /// Wraps an action as a function that returns a dummy <see cref="Unit" /> value.
        /// </summary>
        /// <returns>The function.</returns>
        public static System.Func<Unit> AsUnitFunc(this System.Action action)
        {
            return new Shim(action).Invoke;
        }

        // We do this instead of using an anonymous delegate because we want to
        // mark the target method with the DebuggerHidden and DebuggerStepThrough
        // attributes to provide a smoother debugging experience.
        // For example, this ensures that the stack trace filter will ignore the
        // invoke function.
        private sealed class Shim
        {
            private readonly System.Action action;

            public Shim(System.Action action)
            {
                this.action = action;
            }

            [DebuggerHidden, DebuggerStepThrough]
            public Unit Invoke()
            {
                action();
                return Unit.Value;
            }
        }
    }
}
