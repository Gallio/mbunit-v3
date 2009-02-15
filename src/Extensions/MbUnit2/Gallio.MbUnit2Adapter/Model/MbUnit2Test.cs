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

extern alias MbUnit2;

using System;
using Gallio.Model.Execution;
using Gallio.Model;
using Gallio.Reflection;
using MbUnit2::MbUnit.Core;

namespace Gallio.MbUnit2Adapter.Model
{
    /// <summary>
    /// Wraps an MbUnit v2 test.
    /// </summary>
    internal class MbUnit2Test : BaseTest
    {
        private readonly Fixture fixture;
        private readonly RunPipe runPipe;

        /// <summary>
        /// Initializes a test initially without a parent.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeElement">The point of definition, or null if none</param>
        /// <param name="fixture">The MbUnit v2 fixture, or null if none</param>
        /// <param name="runPipe">The MbUnit v2 run pipe, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public MbUnit2Test(string name, ICodeElementInfo codeElement, Fixture fixture, RunPipe runPipe)
            : base(name, codeElement)
        {
            this.fixture = fixture;
            this.runPipe = runPipe;
        }

        /// <summary>
        /// Gets the MbUnit v2 fixture.
        /// </summary>
        public Fixture Fixture
        {
            get { return fixture; }
        }

        /// <summary>
        /// Gets the MbUnit v2 run pipe.
        /// </summary>
        public RunPipe RunPipe
        {
            get { return runPipe; }
        }
    }
}
