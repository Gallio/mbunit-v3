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

using Gallio.Hosting;

namespace Gallio.Framework.Data.Formatters
{
    /// <summary>
    /// A <see cref="RuleBasedFormatter" /> that uses all <see cref="IFormattingRule"/>s
    /// that are registered with the <see cref="IRuntime"/>.
    /// </summary>
    public class RuntimeRuleBasedFormatter : RuleBasedFormatter
    {
        /// <summary>
        /// Creates a runtime rule-based formatter.
        /// </summary>
        /// <param name="runtime">The runtime</param>
        public RuntimeRuleBasedFormatter(IRuntime runtime)
            : base(runtime.ResolveAll<IFormattingRule>())
        {
        }
    }
}
