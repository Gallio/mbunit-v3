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
using Gallio.Logging;
using Gallio.Model;
using Gallio.Reflection;

namespace Gallio.Contexts
{
    /// <summary>
    /// A stub implementation of <see cref="Context" /> that uses a <see cref="TextLogWriter" /> that
    /// logs output to <see cref="Console.Out" />.  Does not fully support nested test steps
    /// or other dynamic features.
    /// </summary>
    /// <seealso cref="StubContextManager" />
    public class StubContext : Context
    {
        /// <summary>
        /// Creates a stub context.
        /// </summary>
        public StubContext()
            : base(null,
                new TestStepInfo(new BaseTestStep(new BaseTestInstance(new RootTest(), null))),
                new TextLogWriter(Console.Out))
        {
        }

        /// <inheritdoc />
        protected override Context RunStepImpl(string name, ICodeElementInfo codeElement, Action action)
        {
            action();
            return this;
        }

        /// <inheritdoc />
        protected override void AddMetadataImpl(string metadataKey, string metadataValue)
        {
        }
    }
}
