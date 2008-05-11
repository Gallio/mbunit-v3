// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Reflection;

namespace Gallio.Model
{
    /// <summary>
    /// Base implementation of <see cref="ITestParameter" />.
    /// </summary>
    public class BaseTestParameter : BaseTestComponent, ITestParameter
    {
        private ITest owner;

        /// <summary>
        /// Initializes a test parameter.
        /// </summary>
        /// <param name="name">The name of the test parameter</param>
        /// <param name="codeElement">The point of definition of the parameter, or null if unknown</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public BaseTestParameter(string name, ICodeElementInfo codeElement)
            : base(name, codeElement)
        {
        }

        /// <inheritdoc />
        public override string Id
        {
            get
            {
                if (owner != null)
                    return string.Concat(owner.Id, @"@", Name);

                return Name;
            }
        }

        /// <inheritdoc />
        public ITest Owner
        {
            get { return owner; }
            set { owner = value; }
        }
    }
}
