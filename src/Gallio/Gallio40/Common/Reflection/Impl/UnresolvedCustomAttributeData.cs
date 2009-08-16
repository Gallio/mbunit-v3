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
using System.Linq;
using System.Text;
using System.Reflection;

namespace Gallio.Common.Reflection.Impl.DotNet40
{
    internal sealed class UnresolvedCustomAttributeData : CustomAttributeData
    {
        private readonly IAttributeInfo adapter;

        public UnresolvedCustomAttributeData(IAttributeInfo adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException("adapter");

            this.adapter = adapter;
        }

        public override ConstructorInfo Constructor
        {
            get
            {
                return adapter.Constructor.Resolve(false);
            }
        }

        public override IList<CustomAttributeTypedArgument> ConstructorArguments
        {
            get
            {
                return (from arg in adapter.InitializedArgumentValues
                       select new CustomAttributeTypedArgument(arg.Resolve(false)))
                       .ToList().AsReadOnly();
            }
        }

        public override IList<CustomAttributeNamedArgument> NamedArguments
        {
            get
            {
                return (from arg in adapter.InitializedFieldValues
                        select new CustomAttributeNamedArgument(arg.Key.Resolve(false), arg.Value.Resolve(false)))
                        .Concat(from arg in adapter.InitializedPropertyValues
                        select new CustomAttributeNamedArgument(arg.Key.Resolve(false), arg.Value.Resolve(false)))
                        .ToList().AsReadOnly();
            }
        }
    }
}
