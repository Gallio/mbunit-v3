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

extern alias MbUnit2;

using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Model.Execution;
using Gallio.Model;

using MbUnit2::MbUnit.Core;

namespace Gallio.Plugin.MbUnit2Adapter.Model
{
    /// <summary>
    /// Wraps an MbUnit v2 test.
    /// </summary>
    public class MbUnit2Test : BaseTest
    {
        private readonly Fixture fixture;
        private readonly RunPipe runPipe;

        /// <summary>
        /// Initializes a test initially without a parent.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition</param>
        /// <param name="templateBinding">The template binding that produced this test</param>
        /// <param name="fixture">The MbUnit v2 fixture, or null if none</param>
        /// <param name="runPipe">The MbUnit v2 run pipe, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>,
        /// <paramref name="codeReference"/> or <paramref name="templateBinding"/> is null</exception>
        public MbUnit2Test(string name, CodeReference codeReference, MbUnit2AssemblyTemplateBinding templateBinding, Fixture fixture, RunPipe runPipe)
            : base(name, codeReference, templateBinding)
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

        /// <summary>
        /// Gets the binding.
        /// </summary>
        new public MbUnit2AssemblyTemplateBinding TemplateBinding
        {
            get { return (MbUnit2AssemblyTemplateBinding)base.TemplateBinding; }
        }

        /// <inheritdoc />
        public override ITestController CreateTestController()
        {
            return new MbUnit2TestController(TemplateBinding.FixtureExplorer);
        }
    }
}
