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

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Base implementation of <see cref="ITestParameter" />.
    /// </summary>
    public class BaseTestParameter : BaseTestComponent, ITestParameter
    {
        private ITestParameterSet parameterSet;
        private Type type;
        private int index;

        /// <summary>
        /// Initializes a test parameter.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeReference">The point of definition</param>
        /// <param name="parameterSet">The parameter set to which the parameter belongs</param>
        /// <param name="type">The type of the parameter</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>, <paramref name="codeReference"/>,
        /// <paramref name="parameterSet"/> or <paramref name="type"/> is null</exception>
        public BaseTestParameter(string name, CodeReference codeReference, ITestParameterSet parameterSet, Type type)
            : base(name, codeReference)
        {
            if (parameterSet == null)
                throw new ArgumentNullException("parameterSet");
            if (type == null)
                throw new ArgumentNullException("type");

            this.parameterSet = parameterSet;
            this.type = type;
        }

        /// <inheritdoc />
        public ITestParameterSet ParameterSet
        {
            get { return parameterSet; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                parameterSet = value;
            }
        }

        /// <inheritdoc />
        public Type Type
        {
            get { return type; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                type = value;
            }
        }

        /// <inheritdoc />
        public int Index
        {
            get { return index; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("index");

                index = value;
            }
        }
    }
}
