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


using System;
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Framework.Pattern;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Provides utility functions for implementors and clients of <see cref="IPattern" />.
    /// </summary>
    public static class PatternUtils
    {
        /// <summary>
        /// Gets the primary pattern associated with a code element, or null if none.
        /// </summary>
        /// <param name="patternResolver">The pattern resolver</param>
        /// <param name="codeElement">The code element</param>
        /// <returns>The primary pattern, or null if none</returns>
        /// <exception cref="ModelException">Thrown if there are multiple primary patterns associated with the code element</exception>
        public static IPattern GetPrimaryPattern(IPatternResolver patternResolver, ICodeElementInfo codeElement)
        {
            IPattern primaryPattern = null;
            foreach (IPattern pattern in patternResolver.GetPatterns(codeElement, true))
            {
                if (pattern.IsPrimary)
                {
                    if (primaryPattern != null)
                        throw new ModelException(String.Format("There are multiple primary patterns associated with code element '{0}'.  Perhaps it has inappropriate attributes.", codeElement));
                    primaryPattern = pattern;
                }
            }

            return primaryPattern;
        }
    }
}