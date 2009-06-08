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

namespace Gallio.Runner
{
    /// <summary>
    /// Provides constant names for the standard test runner factories.
    /// </summary>
    public static class StandardTestRunnerFactoryNames
    {
        /// <summary>
        /// Runs the test runner in the local AppDomain.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is is usually the default mode because tests typically
        /// run inside of isolated AppDomains of their own nested within
        /// the test runner.
        /// </para>
        /// </remarks>
        public const string Local = "Local";

        /// <summary>
        /// Runs the test runner in an isolated AppDomain of the current process.
        /// </summary>
        public const string IsolatedAppDomain = "IsolatedAppDomain";

        /// <summary>
        /// Runs the test runner in an isolated process.
        /// </summary>
        public const string IsolatedProcess = "IsolatedProcess";
    }
}
