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

namespace Gallio.AutoCAD.Preferences
{
    /// <summary>
    /// Defines user configurable preferences for Gallio's AutoCAD integration components.
    /// </summary>
    public interface IAcadPreferenceManager
    {
        /// <summary>
        /// Command line arguments to be passed to new AutoCAD processes.
        /// </summary>
        string CommandLineArguments
        { get; set; }

        /// <summary>
        /// Specifies the <see cref="StartupAction"/>.
        /// </summary>
        StartupAction StartupAction
        { get; set; }

        /// <summary>
        /// The path to the AutoCAD executable to use for new processes.
        /// </summary>
        string UserSpecifiedExecutable
        { get; set; }

        /// <summary>
        /// The working directory to use for new AutoCAD processes.
        /// </summary>
        string WorkingDirectory
        { get; set; }
    }
}
