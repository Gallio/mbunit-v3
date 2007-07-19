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

using System;
using System.Collections.Generic;
using MbUnit.Framework.Kernel.Harness;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// The builder for a template tree.
    /// </summary>
    public class TemplateTreeBuilder : ModelTreeBuilder<ITemplate>
    {
        /// <summary>
        /// Creates a template tree builder initially populated with
        /// a root template.
        /// </summary>
        /// <param name="harness">The test harness</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="harness"/> is null</exception>
        public TemplateTreeBuilder(ITestHarness harness)
            : base(harness, new RootTemplate())
        {
        }

        /// <summary>
        /// Gets the root template.
        /// </summary>
        new public RootTemplate Root
        {
            get { return (RootTemplate)base.Root; }
        }
    }
}
