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

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Base implementation of <see cref="ITemplateParameterSet" />.
    /// </summary>
    public class BaseTemplateParameterSet : BaseTemplateComponent, ITemplateParameterSet
    {
        private List<ITemplateParameter> parameters;

        /// <summary>
        /// Initializes a test parameter set.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition of the parameter set</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or <paramref name="codeReference"/> is null</exception>
        public BaseTemplateParameterSet(string name, CodeReference codeReference)
            : base(name, codeReference)
        {
            parameters = new List<ITemplateParameter>();
        }

        /// <inheritdoc />
        public IList<ITemplateParameter> Parameters
        {
            get { return parameters; }
        }
    }
}
