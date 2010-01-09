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

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// The base class for plugin, service or component traits.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Subclasses should include properties with getters and setters for binding
    /// configuration values associated with services (aka. traits).  They may also
    /// include methods and other service-specific functionality based on the traits.
    /// </para>
    /// <para>
    /// Traits objects are instantiated in the same way as other components.  The container
    /// injects required dependencies (on services or configuration values) in the constructor
    /// and injects optional dependencies into settable properties.
    /// </para>
    /// </remarks>
    public class Traits
    {
    }
}
