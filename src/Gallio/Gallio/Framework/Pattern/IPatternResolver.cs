// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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

using System;
using System.Collections.Generic;
using Gallio.Reflection;
using Gallio.Framework.Pattern;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// The pattern resolver provides a means for obtaining the <see cref="IPattern" />
    /// objects associated with an <see cref="ICodeElementInfo" />.
    /// </summary>
    public interface IPatternResolver
    {
        /// <summary>
        /// Gets the patterns associated with the specified code element.
        /// </summary>
        /// <param name="codeElement">The code element</param>
        /// <returns>The enumeration of patterns</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="codeElement"/> is null</exception>
        IEnumerable<IPattern> GetPatterns(ICodeElementInfo codeElement);
    }
}