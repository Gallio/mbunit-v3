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
using System.Threading;
using Gallio.Model.Data;

namespace Gallio.Model
{
    /// <summary>
    /// A read-only implementation of <see cref="ITemplate" /> for reflection.
    /// </summary>
    public sealed class TemplateInfo : ModelComponentInfo, ITemplate
    {
        private TemplateInfo cachedParent;
        private TemplateParameterInfoList cachedParameters;
        private TemplateInfoList cachedChildren;

        /// <summary>
        /// Creates a read-only wrapper of the specified model object.
        /// </summary>
        /// <param name="source">The source model object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        public TemplateInfo(ITemplate source)
            : base(source)
        {
        }

        /// <inheritdoc />
        public bool IsGenerator
        {
            get { return Source.IsGenerator; }
        }
        bool ITemplate.IsGenerator
        {
            get { return IsGenerator; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public TemplateParameterInfoList Parameters
        {
            get
            {
                if (cachedParameters == null)
                    Interlocked.CompareExchange(ref cachedParameters, new TemplateParameterInfoList(Source.Parameters), null);
                return cachedParameters;
            }
        }
        IList<ITemplateParameter> ITemplate.Parameters
        {
            get { return Parameters.AsModelList(); }
        }

        /// <inheritdoc />
        public TemplateInfo Parent
        {
            get
            {
                if (cachedParent == null && Source.Parent != null)
                    Interlocked.CompareExchange(ref cachedParent, new TemplateInfo(Source.Parent), null);

                return cachedParent;
            }
        }
        ITemplate IModelTreeNode<ITemplate>.Parent
        {
            get { return Parent; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public TemplateInfoList Children
        {
            get
            {
                if (cachedChildren == null)
                    Interlocked.CompareExchange(ref cachedChildren, new TemplateInfoList(Source.Children), null);
                return cachedChildren;
            }
        }
        IList<ITemplate> IModelTreeNode<ITemplate>.Children
        {
            get { return Children.AsModelList(); }
        }

        void IModelTreeNode<ITemplate>.AddChild(ITemplate node)
        {
            throw new NotSupportedException();
        }

        ITemplateBinding ITemplate.Bind(TemplateBindingScope scope, IDictionary<ITemplateParameter, IDataFactory> arguments)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        new internal ITemplate Source
        {
            get { return (ITemplate)base.Source; }
        }
    }
}