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

namespace Gallio.Common.Xml
{
    /// <summary>
    /// Represents an immutable path to a markup in an XML fragment.
    /// </summary>
    public interface IXmlPath
    {
        /// <summary>
        /// Determines whether the current node represents the empty root path.
        /// </summary>
        bool IsEmpty
        { 
            get;
        }
        
        /// <summary>
        /// Formats the current path.
        /// </summary>
        /// <returns>The resulting formatted path.</returns>
        string ToString();
    }
}
