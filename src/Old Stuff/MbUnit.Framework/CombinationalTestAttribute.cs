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
using TestDriven.UnitTesting;
using TestFu.Operations;

namespace MbUnit.Framework
{
    public enum CombinationType
    {
        AllPairs,
        Cartesian
    }
    
	/// <summary>
	/// Combinational Test
	/// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class CombinatorialTestAttribute : TestAttributeBase 
    {
        private CombinationType combinationType = CombinationType.AllPairs;

        private string tupleValidatorMethod = null;
        public CombinatorialTestAttribute()
        { }
        public CombinatorialTestAttribute(CombinationType combinationType)
        {
            this.combinationType = combinationType;
        }

        public CombinationType CombinationType
        {
            get { return this.combinationType; }
        }

        public string TupleValidatorMethod
        {
            get
            {
                return tupleValidatorMethod;
            }

            set
            {
                tupleValidatorMethod = value;
            }
        }
                
        public override ITestCase[] CreateTests(ITestFixture fixture, MethodInfo method)
        {
            ArrayList testList = new ArrayList();

            ParameterInfo[] parameters = method.GetParameters();

            // create the models
            DomainCollection domains = new DomainCollection();
            Type[] parameterTypes = new Type[parameters.Length];
            int index = 0;
            foreach (ParameterInfo parameter in parameters)
            {
                parameterTypes[index] = parameter.ParameterType;

                DomainCollection pdomains = new DomainCollection();
                foreach (UsingBaseAttribute usingAttribute in parameter.GetCustomAttributes(typeof(UsingBaseAttribute), true))
                {
                    try
                    {
                        usingAttribute.GetDomains(pdomains, parameter, fixture);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Failed while loading domains from parameter " + parameter.Name,
                            ex);                    
                    }
                }
                if (pdomains.Count == 0)
                {
                    throw new Exception("Could not find domain for argument " + parameter.Name);                
                }
                domains.Add(Domains.ToDomain(pdomains));

                index++;
            }

            // we make a cartesian product of all those
            foreach (ITuple tuple in Products.Cartesian(domains))
            {
                // create data domains
                DomainCollection tdomains = new DomainCollection();
                for (int i = 0; i < tuple.Count; ++i)
                {
                    IDomain dm = (IDomain)tuple[i];
                    tdomains.Add(dm);
                }

                // computing the pairwize product
                foreach (ITuple ptuple in GetProduct(tdomains))
                {

                    ITestCase test = new MethodTestCase(fixture.Name, method, ptuple.ToObjectArray());
                        testList.Add(test);        
                }
            }

            return (ITestCase[])testList.ToArray(typeof(ITestCase));
        }

        private ITupleEnumerable GetProduct(IDomainCollection domains)
        {
            switch (this.CombinationType)
            {
                case CombinationType.AllPairs:
                    return Products.PairWize(domains);
                case CombinationType.Cartesian:
                   return Products.Cartesian(domains);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
