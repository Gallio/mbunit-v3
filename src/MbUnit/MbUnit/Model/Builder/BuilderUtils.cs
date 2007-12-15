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


using Gallio.Model.Reflection;

namespace MbUnit.Model.Builder
{
    /// <summary>
    /// Provides utility functions for building MbUnit tests.
    /// </summary>
    public static class BuilderUtils
    {
        /// <summary>
        /// Delegate for consuming a code element.
        /// </summary>
        /// <typeparam name="T">The code element type</typeparam>
        /// <param name="containingTestBuilder">The containing test builder</param>
        /// <param name="codeElement">The code element</param>
        /// <returns>True if the element was consumed</returns>
        public delegate bool Consumer<T>(ITestBuilder containingTestBuilder, T codeElement)
            where T : ICodeElementInfo;

        /// <summary>
        /// Tries to consume a code element by calling the <see cref="IPattern.Consume" />
        /// method of all of its associated patterns.  If none of the patterns consumes
        /// the element, applies a fallback procedure by invoking <paramref name="fallback"/>.
        /// </summary>
        /// <typeparam name="T">The code element type</typeparam>
        /// <param name="containingTestBuilder">The containing test builder</param>
        /// <param name="codeElement">The code element</param>
        /// <param name="fallback">The fallback procedure</param>
        /// <returns>True if the element was consumed</returns>
        public static bool ConsumeWithFallback<T>(ITestBuilder containingTestBuilder, T codeElement,
            Consumer<T> fallback) where T : ICodeElementInfo
        {
            bool consumed = false;
            foreach (IPattern pattern in containingTestBuilder.TestModelBuilder.ReflectionPolicy.GetPatterns(codeElement))
                consumed |= pattern.Consume(containingTestBuilder, codeElement);

            if (!consumed)
                consumed = fallback(containingTestBuilder, codeElement);

            return consumed;
        }
    }
}
