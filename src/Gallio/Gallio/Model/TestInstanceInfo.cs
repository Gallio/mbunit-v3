// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Threading;

namespace Gallio.Model
{
    /// <summary>
    /// A read-only implementation of <see cref="ITestInstance" /> for reflection.
    /// </summary>
    public sealed class TestInstanceInfo : TestComponentInfo, ITestInstance
    {
        private TestInfo cachedTest;

        /// <summary>
        /// Creates a read-only wrapper of the specified test instance.
        /// </summary>
        /// <param name="source">The source test instance</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        public TestInstanceInfo(ITestInstance source)
            : base(source)
        {
        }

        /// <inheritdoc />
        public string FullName
        {
            get { return Source.FullName; }
        }

        /// <inheritdoc />
        public bool IsDynamic
        {
            get { return Source.IsDynamic; }
        }

        /// <inheritdoc />
        public TestInfo Test
        {
            get
            {
                if (cachedTest == null)
                    Interlocked.CompareExchange(ref cachedTest, new TestInfo(Source.Test), null);
                return cachedTest;
            }
        }
        ITest ITestInstance.Test
        {
            get { return Test; }
        }

        /// <inheritdoc />
        public TestInstanceInfo Parent
        {
            get { return Source.Parent != null ? new TestInstanceInfo(Source.Parent) : null; }
        }

        ITestInstance ITestInstance.Parent
        {
            get { return Parent; }
        }

        /// <inheritdoc />
        new internal ITestInstance Source
        {
            get { return (ITestInstance)base.Source; }
        }
    }
}