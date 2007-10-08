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
using MbUnit.Model.Data;

namespace MbUnit.Model
{
    /// <summary>
    /// A read-only implementation of <see cref="ITemplateBinding" /> for reflection.
    /// </summary>
    public sealed class TemplateBindingInfo : BaseInfo, ITemplateBinding
    {
        private TemplateInfo cachedTemplateInfo;

        /// <summary>
        /// Creates a read-only wrapper of the specified model object.
        /// </summary>
        /// <param name="source">The source model object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        public TemplateBindingInfo(ITemplateBinding source)
            : base(source)
        {
        }

        /// <inheritdoc />
        public TemplateInfo Template
        {
            get
            {
                if (cachedTemplateInfo == null)
                    Interlocked.CompareExchange(ref cachedTemplateInfo, new TemplateInfo(Source.Template), null);
                return cachedTemplateInfo;
            }
        }
        ITemplate ITemplateBinding.Template
        {
            get { throw new NotSupportedException(); }
        }

        TemplateBindingScope ITemplateBinding.Scope
        {
            get { throw new NotSupportedException(); }
        }

        IDictionary<ITemplateParameter, IDataFactory> ITemplateBinding.Arguments
        {
            get { throw new NotSupportedException(); }
        }

        void ITemplateBinding.BuildTests(TestTreeBuilder builder, ITest parent)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        new internal ITemplateBinding Source
        {
            get { return (ITemplateBinding)base.Source; }
        }
    }
}