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

namespace Gallio.UI.Common.Policies
{
    ///<summary>
    /// Wrapper for static UnhandledExceptionPolicy class (to improve testability).
    ///</summary>
    public interface IUnhandledExceptionPolicy
    {
        /// <summary>
        /// Reports an unhandled exception.
        /// </summary>
        /// <param name="message">A message to explain how the exception was intercepted.</param>
        /// <param name="unhandledException">The unhandled exception.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> or 
        /// <paramref name="unhandledException"/> is null.</exception>
        void Report(string message, Exception unhandledException);
    }
}
