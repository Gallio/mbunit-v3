// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Runtime
{
    /// <summary>
    /// Provides functions for detecting CLR runtime parameters.
    /// </summary>
    public class RuntimeDetection
    {
        /// <summary>
        /// Returns true if the application is running within the Mono runtime.
        /// </summary>
        /// <remarks>
        /// It is occasionally necessary to tailor the execution of the test runner
        /// depending on whether Mono is running.  However, the number of such
        /// customizations should be very limited.
        /// </remarks>
        public static bool IsUsingMono
        {
            get { return Type.GetType(@"Mono.Runtime") != null; }
        }
    }
}
