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
using System.Reflection;
using System.Collections;
using TestFu.Operations;
//using MbUnit.Core;

namespace MbUnit.Framework
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public sealed class UsingLinearAttribute : UsingBaseAttribute
    {
        private IDomain domain;

        public UsingLinearAttribute(int start, int stepCount)
        {
            this.domain = new LinearInt32Domain(start, stepCount);
        }
        public UsingLinearAttribute(int start, int stepCount,int step)
        {
            this.domain = new LinearInt32Domain(start, stepCount,step);
        }

        public override void GetDomains(IDomainCollection domains, ParameterInfo parameter, object fixture)
        {
            domains.Add(domain);
        }
    }
}
