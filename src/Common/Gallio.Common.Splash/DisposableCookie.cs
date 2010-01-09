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

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Invokes a delegate when disposed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This type is useful for creating APIs that leverage the C# "using" syntax for scoping.
    /// </para>
    /// <example>
    /// <code>
    /// using (document.BeginStyle(Style.Default)) // returns a DisposableCookie
    /// {
    ///     document.AppendText("Hello world");
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    public struct DisposableCookie : IDisposable
    {
        private readonly Action action;

        /// <summary>
        /// Specifies the action to perform when <see cref="Dispose"/> is called.
        /// </summary>
        public delegate void Action();

        /// <summary>
        /// Creates a disposable cookie.
        /// </summary>
        /// <param name="action">The action to perform when <see cref="Dispose"/> is called.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
        public DisposableCookie(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            this.action = action;
        }

        /// <summary>
        /// Invokes the dispose action.
        /// </summary>
        public void Dispose()
        {
            action();
        }
    }
}
