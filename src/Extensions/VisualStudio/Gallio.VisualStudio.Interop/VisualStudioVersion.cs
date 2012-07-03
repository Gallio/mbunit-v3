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

using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.VisualStudio.Interop
{
    /// <summary>
    /// Indentifies a particular version of Visual Studio.
    /// </summary>
    public enum VisualStudioVersion
    {
        /// <summary>
        /// Used to find an instance of any version of Visual Studio.
        /// </summary>
        Any = 0,

        /// <summary>
        /// Visual Studio 2005.
        /// </summary>
        VS2005 = 1,

        /// <summary>
        /// Visual Studio 2008.
        /// </summary>
        VS2008 = 2,

        /// <summary>
        /// Visual Studio 2010.
        /// </summary>
        VS2010 = 3,

        /// <summary>
        /// Visual Studio 2012.
        /// </summary>
        VS2012 = 4,
    }
}
