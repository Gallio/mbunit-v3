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
using System.Text;
using MbUnit.Model.Execution;
using MbUnit.Model;

namespace MbUnit.Plugin.NUnitAdapter.Model
{
    /// <summary>
    /// Wraps an NUnit test.
    /// </summary>
    public class NUnitTest : BaseTest
    {
        private readonly NUnit.Core.ITest test;

        /// <summary>
        /// Initializes a test initially without a parent.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition</param>
        /// <param name="templateBinding">The template binding that produced this test</param>
        /// <param name="test">The NUnit test, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>,
        /// <paramref name="codeReference"/> or <paramref name="templateBinding"/> is null</exception>
        public NUnitTest(string name, CodeReference codeReference, NUnitFrameworkTemplateBinding templateBinding, NUnit.Core.ITest test)
            : base(name, codeReference, templateBinding)
        {
            this.test = test;
        }

        /// <summary>
        /// Gets the NUnit test.
        /// </summary>
        public NUnit.Core.ITest Test
        {
            get { return test; }
        }

        /// <summary>
        /// Gets the binding.
        /// </summary>
        new public NUnitFrameworkTemplateBinding TemplateBinding
        {
            get { return (NUnitFrameworkTemplateBinding)base.TemplateBinding; }
        }

        /// <inheritdoc />
        public override ITestController CreateTestController()
        {
            return new NUnitTestController(TemplateBinding.Runner);
        }
    }
}
