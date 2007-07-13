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
using TestFu.Operations;
using System.Reflection;
using MbUnit.Core;
using MbUnit.Framework;
//using MbUnit.Core.Invokers;

namespace MbUnit.Framework
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public sealed class UsingFactoriesAttribute : UsingBaseAttribute
    {
        private Type factoryType = null;
        private string memberNames = null;

        #region Constructors
        public UsingFactoriesAttribute()
        {
            this.factoryType = null;
            this.memberNames = null;
        }
        public UsingFactoriesAttribute(string memberNames)
        {
            if (memberNames == null)
                throw new ArgumentNullException("memberNames");
            if (memberNames.Length == 0)
                throw new ArgumentException("Length is zero", "memberNames");
            this.factoryType = null;
            this.memberNames = memberNames;
        }
        public UsingFactoriesAttribute(Type factoryType)
        {
            if (factoryType == null)
                throw new ArgumentNullException("factoryType");
            this.factoryType = factoryType;
            this.memberNames = null;
        }
        public UsingFactoriesAttribute(Type factoryType, string memberNames)
        {
            if (factoryType == null)
                throw new ArgumentNullException("factoryType");
            if (memberNames == null)
                throw new ArgumentNullException("memberNames");
            if (memberNames.Length == 0)
                throw new ArgumentException("Length is zero", "memberNames");
            this.factoryType = factoryType;
            this.memberNames = memberNames;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a list of member names separated by ;
        /// </summary>
        /// <value></value>
        public string MemberNames
        {
            get
            {
                return this.memberNames;
            }
            set
            {
                this.memberNames = value;
            }
        }

        public Type FactoryType
        {
            get
            {
                return this.factoryType;
            }
            set
            {
                this.factoryType = value;
            }
        }
        #endregion

        public override void GetDomains(IDomainCollection domains, ParameterInfo parameter, object fixture)
        {
            Type t = null;
            if (this.factoryType != null)
                t = factoryType;
            else
                t = parameter.Member.DeclaringType;

            if (this.MemberNames == null)
            {
                GetAllDomains(domains, parameter, t);
            }
            else
            {
                GetNamedDomains(domains, parameter, t);
            }
        }

        private void GetNamedDomains(IDomainCollection domains, ParameterInfo parameter, Type t)
        {
            foreach (string memberName in this.MemberNames.Split(';'))
            {
                MethodInfo domainMethod = t.GetMethod(memberName, Type.EmptyTypes);
                if (domainMethod == null)
                    Assert.Fail("Could not find domain method {0} for parameter {1}",
                        memberName, parameter.Name);

                object result = this.InvokeMethod(t, domainMethod);

                IDomain domain = Domains.ToDomain(result);
                domain.Name = domainMethod.Name;
                domains.Add(domain);
            }
        }

        private void GetAllDomains(IDomainCollection domains, ParameterInfo parameter, Type t)
        {
            foreach (MethodInfo factoryMethod in TypeHelper.GetAttributedMethods(t, typeof(FactoryAttribute)))
            {
                if (factoryMethod.GetParameters().Length > 0)
                    continue;
                Type returnType = factoryMethod.ReturnType;

                // check single object return
                if (parameter.ParameterType.IsAssignableFrom(returnType))
                {
                    object result = this.InvokeMethod(t, factoryMethod);
                    IDomain domain = Domains.ToDomain(result);
                    domain.Name = factoryMethod.Name;
                    domains.Add(domain);
                    continue;
                }

                // check array
                if (returnType.HasElementType)
                {
                    Type elementType = returnType.GetElementType();
                    if (parameter.ParameterType == elementType)
                    {
                        object result = this.InvokeMethod(t, factoryMethod);
                        IDomain domain = Domains.ToDomain(result);
                        domain.Name = factoryMethod.Name;
                        domains.Add(domain);
                        continue;
                    }
                }

                // check factory type
                FactoryAttribute factoryAttribute = TypeHelper.TryGetFirstCustomAttribute(factoryMethod, typeof(FactoryAttribute)) as FactoryAttribute;
                if (factoryAttribute != null)
                {
                    Type factoredType = factoryAttribute.FactoredType;
                    if (parameter.ParameterType == factoredType)
                    {
                        object result = this.InvokeMethod(t, factoryMethod);
                        IDomain domain = Domains.ToDomain(result);
                        domain.Name = factoryMethod.Name;
                        domains.Add(domain);
                        continue;
                    }
                }
            }
        }
    }
}
