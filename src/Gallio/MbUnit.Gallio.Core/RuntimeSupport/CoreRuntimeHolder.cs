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

using MbUnit.Framework;

namespace MbUnit.Core.RuntimeSupport
{
    /// <summary>
    /// Provides support for manipulating the core runtime.
    /// </summary>
    public static class CoreRuntimeHolder
    {
        /// <summary>
        /// Gets or sets the core runtime instance.  May be null if not initialized.
        /// </summary>
        public static ICoreRuntime Instance
        {
            get { return (ICoreRuntime) Runtime.Instance; }
            set { Runtime.SetInstance(value); }
        }
    }
}
