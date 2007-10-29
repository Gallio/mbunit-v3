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

namespace Gallio.Core.ProgressMonitoring
{
    /// <summary>
    /// A progress monitor that simply validates its parameters but does
    /// not perform any processing.
    /// </summary>
    /// <seealso cref="IProgressMonitor"/> for important thread-safety and usage remarks.
    public sealed class NullProgressMonitor : BaseProgressMonitor
    {
    }
}