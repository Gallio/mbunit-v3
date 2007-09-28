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

namespace MbUnit.Framework.Kernel.Model.Reflection
{
    /// <summary>
    /// A read-only implementation of <see cref="IStep" /> for reflection.
    /// </summary>
    public sealed class StepInfo : BaseInfo, IStep
    {
        /// <summary>
        /// Creates a read-only wrapper of the specified model object.
        /// </summary>
        /// <param name="source">The source model object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        public StepInfo(IStep source)
            : base(source)
        {
        }

        /// <inheritdoc />
        public string Id
        {
            get { return Source.Id; }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return Source.Name; }
        }

        /// <inheritdoc />
        public string FullName
        {
            get { return Source.FullName; }
        }

        /// <inheritdoc />
        public StepInfo Parent
        {
            get { return Source.Parent != null ? new StepInfo(Source.Parent) : null; }
        }
        IStep IStep.Parent
        {
            get { return Parent; }
        }

        /// <inheritdoc />
        public TestInfo Test
        {
            get { return new TestInfo(Source.Test); }
        }
        ITest IStep.Test
        {
            get { return Test; }
        }

        /// <inheritdoc />
        new internal IStep Source
        {
            get { return (IStep)base.Source; }
        }
    }
}
