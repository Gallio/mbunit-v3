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
using MbUnit.Framework;
using TestFu.Operations;
using System.Reflection;

namespace MbUnit.Framework
{
    [AttributeUsage(AttributeTargets.Parameter,AllowMultiple =true,Inherited =true)]
    public sealed class UsingEnumAttribute : UsingBaseAttribute
    {
        private Type enumType;

        public UsingEnumAttribute(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException("emumType");
            if (!enumType.IsEnum)
                throw new ArgumentException("Type "+enumType.FullName+" is not a enum");
            this.enumType = enumType;
        }

        public Type EnumType
        {
            get { return this.enumType; }
        }

        public override void GetDomains(
            IDomainCollection domains, 
            ParameterInfo parameter, 
            object fixture)
        {
            ArrayDomain domain = new ArrayDomain(Enum.GetValues(this.EnumType));
            domains.Add(domain);
        }
    }
}
