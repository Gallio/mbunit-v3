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

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// A read-only implementation of <see cref="ITest" /> for reflection.
    /// </summary>
    public sealed class TestInfo : ModelComponentInfo, ITest
    {
        /// <summary>
        /// Creates a read-only wrapper of the specified model object.
        /// </summary>
        /// <param name="source">The source model object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        public TestInfo(ITest source)
            : base(source)
        {
        }

        /// <inheritdoc />
        public bool IsTestCase
        {
            get { return Source.IsTestCase; }
        }
        bool ITest.IsTestCase
        {
            get { return IsTestCase; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public TemplateBindingInfo TemplateBinding
        {
            get { return new TemplateBindingInfo(Source.TemplateBinding); }
        }
        ITemplateBinding ITest.TemplateBinding
        {
            get { return TemplateBinding; }
        }

        /// <inheritdoc />
        public TestInfoList Dependencies
        {
            get { return new TestInfoList(Source.Dependencies); }
        }
        IList<ITest> ITest.Dependencies
        {
            get { return Dependencies.AsModelList(); }
        }

        /// <inheritdoc />
        public ITest Parent
        {
            get { return Source.Parent != null ? new TestInfo(Source.Parent) : null; }
        }
        ITest IModelTreeNode<ITest>.Parent
        {
            get { return Parent; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public TestInfoList Children
        {
            get { return new TestInfoList(Source.Children); }
        }
        IList<ITest> IModelTreeNode<ITest>.Children
        {
            get { return Children.AsModelList(); }
        }

        void IModelTreeNode<ITest>.AddChild(ITest node)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        new internal ITest Source
        {
            get { return (ITest)base.Source; }
        }
    }
}