// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

using System;
using Gallio.Common.Reflection;

namespace Gallio.Model.Tree
{
    /// <summary>
    /// A test parameter describes a formal parameter of a <see cref="TestComponent.Name" />
    /// to which a value may be bound and used during test execution.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="TestComponent" /> property of a test parameter should be
    /// unique among the set parameters belonging to its <see cref="Test"/> to ensure
    /// that it can be differentiated from others.  However, this constraint is not enforced.
    /// </para>
    /// </remarks>
    public class TestParameter : TestComponent
    {
        private Test owner;
        private string id;

        /// <summary>
        /// Initializes a test parameter.
        /// </summary>
        /// <param name="name">The name of the test parameter.</param>
        /// <param name="codeElement">The point of definition of the parameter, or null if unknown.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.</exception>
        public TestParameter(string name, ICodeElementInfo codeElement)
            : base(name, codeElement)
        {
        }

        /// <inheritdoc />
        public override string Id
        {
            get
            {
                if (id != null)
                    return id;

                if (owner != null)
                    return string.Concat(owner.Id, @"@", Name);

                return Name;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                id = value;
            }
        }

        /// <summary>
        /// Gets or sets the test that owns this parameter, or null if this parameter
        /// does not yet have an owner.
        /// </summary>
        public Test Owner
        {
            get { return owner; }
            set { owner = value; }
        }
    }
}