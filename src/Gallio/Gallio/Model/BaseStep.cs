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
using Gallio.Properties;

namespace Gallio.Model
{
    /// <summary>
    /// Base implementation of <see cref="IStep"/>.
    /// </summary>
    public class BaseStep : BaseModelComponent, IStep
    {
        private readonly string fullName;
        private readonly IStep parent;
        private readonly ITest test;

        /// <summary>
        /// Gets the localized name of the root step.
        /// </summary>
        public static string RootStepName
        {
            get { return Resources.BaseStep_RootStepName; }
        }

        /// <summary>
        /// Creates a step.
        /// </summary>
        /// <param name="name">The step name</param>
        /// <param name="codeReference">The code reference</param>
        /// <param name="test">The test to which the step belongs</param>
        /// <param name="parent">The parent step, or null if creating a root step</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>,
        /// <paramref name="codeReference"/> or <paramref name="test"/> is null</exception>
        public BaseStep(string name, CodeReference codeReference, ITest test, IStep parent)
            : base(name, codeReference)
        {
            if (name == null)
                throw new ArgumentNullException(@"name");

            this.test = test;
            this.parent = parent;
            fullName = GenerateFullName();
        }

        /// <inheritdoc />
        public string FullName
        {
            get { return fullName; }
        }

        /// <inheritdoc />
        public IStep Parent
        {
            get { return parent; }
        }

        /// <inheritdoc />
        public ITest Test
        {
            get { return test; }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return String.Format("[Step] {0}", fullName);
        }

        private string GenerateFullName()
        {
            if (parent == null)
                return test.Name;
            else if (parent.Parent == null)
                return parent.FullName + @":" + Name;
            else
                return parent.FullName + @"/" + Name;
        }
    }
}
