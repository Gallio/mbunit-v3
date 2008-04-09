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

namespace Gallio.Runtime.ProgressMonitoring
{
    /// <summary>
    /// Creates instances of null progress monitors that do nothing.
    /// </summary>
    public static class NullProgressMonitor
    {
        /// <summary>
        /// Creates a null progress monitor.
        /// </summary>
        /// <returns>The null progress monitor</returns>
        public static IProgressMonitor CreateInstance()
        {
            return new ObservableProgressMonitor();
        }
    }
}
