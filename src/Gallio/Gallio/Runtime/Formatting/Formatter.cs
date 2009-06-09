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
using System.Text;
using Gallio.Runtime;

namespace Gallio.Runtime.Formatting
{
    /// <summary>
    /// Service locator for <see cref="IFormatter" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Handles the case where the runtime is not initialized by returning a
    /// <see cref="StubFormatter" />.
    /// </para>
    /// </remarks>
    public static class Formatter
    {
        private static IFormatter cachedFormatter;

        static Formatter()
        {
            RuntimeAccessor.InstanceChanged += delegate { cachedFormatter = null; };
        }

        /// <summary>
        /// Gets the global formatter singleton.
        /// </summary>
        public static IFormatter Instance
        {
            get
            {
                if (cachedFormatter == null)
                {
                    if (RuntimeAccessor.IsInitialized)
                        cachedFormatter = RuntimeAccessor.ServiceLocator.Resolve<IFormatter>();
                    else
                        cachedFormatter = new StubFormatter();
                }

                return cachedFormatter;
            }
        }
    }
}
