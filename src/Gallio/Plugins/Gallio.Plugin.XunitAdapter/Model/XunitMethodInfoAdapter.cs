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
using System.Collections.Generic;
using System.Reflection;
using Gallio.Reflection;

using XunitMethodInfo = Xunit.Sdk.IMethodInfo;
using XunitAttributeInfo = Xunit.Sdk.IAttributeInfo;

namespace Gallio.Plugin.XunitAdapter.Model
{
    /// <summary>
    /// An adapter for converting <see cref="IMethodInfo" /> into <see cref="XunitMethodInfo" />.
    /// </summary>
    internal class XunitMethodInfoAdapter : XunitMethodInfo
    {
        private readonly IMethodInfo target;

        public XunitMethodInfoAdapter(IMethodInfo target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            this.target = target;
        }

        public IMethodInfo Target
        {
            get { return target; }
        }

        public IEnumerable<XunitAttributeInfo> GetCustomAttributes(Type attributeType)
        {
            foreach (IAttributeInfo attribute in target.GetAttributeInfos(attributeType, true))
                yield return new XunitAttributeInfoAdapter(attribute);
        }

        public bool HasAttribute(Type attributeType)
        {
            return target.HasAttribute(attributeType, true);
        }

        public string DeclaringTypeName
        {
            get { return target.DeclaringType.FullName; }
        }

        public bool IsAbstract
        {
            get { return target.IsAbstract; }
        }

        public bool IsStatic
        {
            get { return target.IsStatic; }
        }

        public MethodInfo MethodInfo
        {
            get { return target.Resolve(); }
        }

        public string Name
        {
            get { return target.Name; }
        }

        public string ReturnType
        {
            get { return target.ReturnType.FullName; }
        }

        public override string ToString()
        {
            return target.Name;
        }
    }
}
