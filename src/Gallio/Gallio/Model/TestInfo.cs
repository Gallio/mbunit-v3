// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Threading;
using Gallio.Common;
using Gallio.Model.Execution;

namespace Gallio.Model
{
    /// <summary>
    /// A read-only implementation of <see cref="ITest" /> for reflection.
    /// </summary>
    public sealed class TestInfo : TestComponentInfo, ITest
    {
        private TestParameterInfoList cachedParameters;
        private TestInfoList cachedDependencies;
        private TestInfo cachedParent;
        private TestInfoList cachedChildren;

        /// <summary>
        /// Creates a read-only wrapper of a test.
        /// </summary>
        /// <param name="source">The source test.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null.</exception>
        public TestInfo(ITest source)
            : base(source)
        {
        }

        /// <inheritdoc />
        public string LocalId
        {
            get { return Source.LocalId; }
        }

        /// <inheritdoc />
        public string FullName
        {
            get { return Source.FullName; }
        }

        /// <inheritdoc />
        public bool IsTestCase
        {
            get { return Source.IsTestCase; }
        }

        /// <inheritdoc cref="ITest.Parameters" />
        public TestParameterInfoList Parameters
        {
            get
            {
                if (cachedParameters == null)
                    Interlocked.CompareExchange(ref cachedParameters, new TestParameterInfoList(Source.Parameters), null);
                return cachedParameters;
            }
        }
        IList<ITestParameter> ITest.Parameters
        {
            get { return Parameters.AsModelList(); }
        }

        /// <inheritdoc cref="ITest.Dependencies" />
        public TestInfoList Dependencies
        {
            get
            {
                if (cachedDependencies == null)
                    Interlocked.CompareExchange(ref cachedDependencies, new TestInfoList(Source.Dependencies), null);
                return cachedDependencies;
            }
        }
        IList<ITest> ITest.Dependencies
        {
            get { return Dependencies.AsModelList(); }
        }

        /// <inheritdoc cref="ITest.Order" />
        public int Order
        {
            get { return Source.Order; }
        }
        int ITest.Order
        {
            get { return Order; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc cref="ITest.Parent" />
        public ITest Parent
        {
            get
            {
                if (cachedParent == null && Source.Parent != null)
                    Interlocked.CompareExchange(ref cachedParent, new TestInfo(Source.Parent), null);
                return cachedParent;
            }
        }
        ITest ITest.Parent
        {
            get { return Parent; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc cref="ITest.Children" />
        public TestInfoList Children
        {
            get
            {
                if (cachedChildren == null)
                    Interlocked.CompareExchange(ref cachedChildren, new TestInfoList(Source.Children), null);
                return cachedChildren;
            }
        }
        IList<ITest> ITest.Children
        {
            get { return Children.AsModelList(); }
        }

        void ITest.AddChild(ITest node)
        {
            throw new NotSupportedException();
        }

        void ITest.AddDependency(ITest test)
        {
            throw new NotSupportedException();
        }

        string ITest.GetUniqueLocalIdForChild(string localIdHint)
        {
            throw new NotSupportedException();
        }

        Func<ITestController> ITest.TestControllerFactory
        {
            get { throw new NotSupportedException(); }
        }

        void ITest.AddParameter(ITestParameter parameter)
        {
            throw new NotSupportedException();
        }

        new internal ITest Source
        {
            get { return (ITest)base.Source; }
        }
    }
}