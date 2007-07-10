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
