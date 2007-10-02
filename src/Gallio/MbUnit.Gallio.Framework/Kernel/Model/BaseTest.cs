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
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Base implementation of <see cref="ITest" />.
    /// </summary>
    /// <remarks>
    /// The base test implementation acts as a simple container for tests.
    /// Accordingly its kind is set to <see cref="ComponentKind.Group" /> by default.
    /// </remarks>
    public class BaseTest : BaseTestComponent, ITest
    {
        private readonly List<ITest> children;
        private readonly List<ITest> dependencies;
        private readonly ITemplateBinding templateBinding;
        private ITest parent;
        private bool isTestCase;

        /// <summary>
        /// Initializes a test initially without a parent.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition</param>
        /// <param name="templateBinding">The template binding that produced this test</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>,
        /// <paramref name="codeReference"/> or <paramref name="templateBinding"/> is null</exception>
        public BaseTest(string name, CodeReference codeReference, ITemplateBinding templateBinding)
            : base(name, codeReference)
        {
            if (templateBinding == null)
                throw new ArgumentNullException(@"templateBinding");

            this.templateBinding = templateBinding;

            dependencies = new List<ITest>();
            children = new List<ITest>();

            Kind = ComponentKind.Group;
        }

        /// <inheritdoc />
        public ITest Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        /// <inheritdoc />
        public bool IsTestCase
        {
            get { return isTestCase; }
            set { isTestCase = value; }
        }

        /// <inheritdoc />
        public ITemplateBinding TemplateBinding
        {
            get { return templateBinding; }
        }

        /// <inheritdoc />
        public IList<ITest> Children
        {
            get { return children; }
        }

        /// <inheritdoc />
        public IList<ITest> Dependencies
        {
            get { return dependencies; }
        }

        /// <inheritdoc />
        public virtual void AddChild(ITest test)
        {
            ModelUtils.Link<ITest>(this, test);
        }
    }
}