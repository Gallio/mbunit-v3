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

namespace Gallio.Model
{
    /// <summary>
    /// Defines the names of common test step lifecycle phases.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Custom phases may also be defined.
    /// </para>
    /// </remarks>
    public static class LifecyclePhases
    {
        /// <summary>
        /// The test step is starting.
        /// </summary>
        public const string Starting = "Starting";

        /// <summary>
        /// The test is being initialize. (Fixture construction, etc.)
        /// </summary>
        public const string Initialize = "Initialize";

        /// <summary>
        /// The test step is being set up.
        /// </summary>
        public const string SetUp = "SetUp";

        /// <summary>
        /// The test step is executing its main body.
        /// </summary>
        public const string Execute = "Execute";

        /// <summary>
        /// The test step is being torn down.
        /// </summary>
        public const string TearDown = "TearDown";

        /// <summary>
        /// The test step's context is being disposed.
        /// </summary>
        public const string Dispose = "Dispose";

        /// <summary>
        /// The test step is finishing.
        /// </summary>
        public const string Finishing = "Finishing";
    }
}