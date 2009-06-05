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

using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Common.Reflection;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Populates components lazily.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A populator function takes one parameter to specify a hint for the particular
    /// code element whose patterns should be processed to generate components.  If the hint
    /// is null or unrecognized then the populator should proceed to generate all remaining
    /// components.
    /// </para>
    /// </remarks>
    /// <param name="codeElementHint">The code element hint to identify the location of the
    /// particular components to populate, or null to populate them all.</param>
    public delegate void DeferredComponentPopulator(ICodeElementInfo codeElementHint);
}
