using System;
using System.Reflection;
using System.Collections;
using TestFu.Operations;
//using MbUnit.Core;

namespace MbUnit.Framework
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public sealed class UsingLiteralsAttribute : UsingBaseAttribute
    {
        private string values;

        public UsingLiteralsAttribute(string values)
        {
            this.values = values;
        }

        /// <summary>
        /// Gets a list of values separated by ;
        /// </summary>
        /// <value></value>
        public string Values
        {
            get
            {
                return this.values;
            }
        }

        public override void GetDomains(IDomainCollection domains, ParameterInfo parameter, object fixture)
        {
            bool isString = parameter.ParameterType.IsAssignableFrom(typeof(string));
            ArrayList data = new ArrayList();
            foreach (string memberName in this.Values.Split(';'))
            {
                object cresult = null;
                if (isString)
                    cresult = memberName.ToString();
                else
                    cresult = Convert.ChangeType(memberName, parameter.ParameterType);
                data.Add(cresult);
            }
            if (data.Count == 0)
                return;

            CollectionDomain domain = new CollectionDomain(data);
            domains.Add(domain);
        }
    }
}
