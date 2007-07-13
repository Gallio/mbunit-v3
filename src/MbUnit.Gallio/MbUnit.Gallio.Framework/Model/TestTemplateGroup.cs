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
using MbUnit.Framework.Model.Metadata;

namespace MbUnit.Framework.Model
{
    /// <summary>
    /// A test template group is a test template that aggregates a collection
    /// of related templates under some common parent.  It supports the
    /// addition of arbitrary templates as children.
    /// </summary>
    public class TestTemplateGroup : BaseTestTemplate
    {
        private List<ITestTemplate> children;

        /// <summary>
        /// Initializes an empty test template group.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>
        /// or <paramref name="codeReference"/> is null</exception>
        public TestTemplateGroup(string name, CodeReference codeReference)
            : base(name, codeReference)
        {
            children = new List<ITestTemplate>();
            Kind = TemplateKind.Group;
        }

        /// <inheritdoc />
        public override IEnumerable<ITestTemplate> Children
        {
            get { return children; }
        }

        /// <summary>
        /// Gets the children of this test template as a list.
        /// </summary>
        public IList<ITestTemplate> ChildrenList
        {
            get { return children; }
        }

        /// <inheritdoc />
        public override void AddChild(ITestTemplate template)
        {
            ModelUtils.LinkTemplate(this, children, template);
        }
    }
}
