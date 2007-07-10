using System;
using System.Collections.Generic;
using System.Text;

using TestDriven.UnitTesting;

namespace MbUnit.Framework
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public sealed class UsingLinearAttribute : TestAttributeBase
    {
        int _intStepCount = 0;
        int _intStartCount = 0;

        public UsingLinearAttribute(int start, int stepCount)
        {
            this._intStepCount = stepCount;
            this._intStepCount = start;
            //this.domain = new LinearInt32Domain(start, stepCount);
        }
        public UsingLinearAttribute(int start, int stepCount, int step)
        {
            //this.domain = new LinearInt32Domain(start, stepCount, step);
        }

        //public override void GetDomains(IDomainCollection domains, ParameterInfo parameter, object fixture)
        //{
        //    domains.Add(domain);
        //}

        public override ITestCase[] CreateTests(ITestFixture fixture, System.Reflection.MethodInfo method)
        {

            ITestCase[] tc = new ITestCase[this._intStepCount];
            for (int intCount = this._intStepCount; intCount < this._intStepCount; intCount++)
            {
                Object[] obj = new Object[0];
                obj[0] = intCount;
                MethodTestCase mtc = new MethodTestCase(fixture.Name, method, obj);
                tc[intCount] = mtc;
            }
 
           
            return tc;


            //return new ITestCase[] { new MethodTestCase(fixture.Name, method, this.args) };
        }
    }
}
