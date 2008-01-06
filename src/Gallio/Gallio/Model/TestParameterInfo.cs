// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Reflection;

namespace Gallio.Model
{
    /// <summary>
    /// A read-only implementation of <see cref="ITestParameter" /> for reflection.
    /// </summary>
    public sealed class TestParameterInfo : TestComponentInfo, ITestParameter
    {
        /// <summary>
        /// Creates a read-only wrapper of the specified test parameter.
        /// </summary>
        /// <param name="source">The source test parameter</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        public TestParameterInfo(ITestParameter source)
            : base(source)
        {
        }

        /// <inheritdoc />
        public TestInfo Owner
        {
            get { return Source.Owner != null ? new TestInfo(Source.Owner) : null; }
        }

        ITest ITestParameter.Owner
        {
            get { return Owner; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public ITypeInfo Type
        {
            get { return Source.Type; }
        }

        /// <inheritdoc />
        public int Index
        {
            get { return Source.Index; }
        }

        /// <inheritdoc />
        new internal ITestParameter Source
        {
            get { return (ITestParameter)base.Source; }
        }
    }
}