// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Loader
{
    /// <summary>
    /// Provides access to an instance of the Gallio runtime that is running in a foreign AppDomain.
    /// </summary>
    public interface IGallioRemoteEnvironment : IDisposable
    {
        /// <summary>
        /// Gets the remote AppDomain.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the remote environment has been disposed</exception>
        AppDomain AppDomain { get; }
        
        /// <summary>
        /// Gets the remote loader.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the remote environment has been disposed</exception>
        IGallioLoader Loader { get; }
    }
}
