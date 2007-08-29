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
    /// Base implementation of <see cref="IStep"/>.
    /// </summary>
    public class BaseStep : IStep
    {
        private string id;
        private string name;
        private IStep parent;
        private ITest test;

        /// <summary>
        /// Creates a step.
        /// </summary>
        /// <param name="name">The step name</param>
        /// <param name="test">The test to which the step belongs</param>
        /// <param name="parent">The parent step, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>,
        /// or <paramref name="test"/> is null</exception>
        public BaseStep(string name, ITest test, IStep parent)
        {
            if (name == null)
                throw new ArgumentNullException(@"name");
            if (test == null)
                throw new ArgumentNullException(@"test");

            this.name = name;
            this.test = test;
            this.parent = parent;

            id = Guid.NewGuid().ToString();
        }

        /// <inheritdoc />
        public string Id
        {
            get { return id; }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return name; }
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

        /// <summary>
        /// Creates a root step for a test.
        /// </summary>
        /// <param name="test">The test</param>
        /// <returns>The root step</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/> is null</exception>
        public static BaseStep CreateRootStep(ITest test)
        {
            if (test == null)
                throw new ArgumentNullException(@"test");

            return new BaseStep(Resources.BaseStep_RootStepName, test, null);
        }
    }
}
