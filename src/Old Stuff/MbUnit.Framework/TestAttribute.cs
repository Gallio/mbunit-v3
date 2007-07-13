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

namespace MbUnit.Framework
{
    using System;
    using System.Reflection;
    using System.Collections;
    using TestDriven.UnitTesting;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class TestAttribute : TestAttributeBase
    {
        public override ITestCase[] CreateTests(ITestFixture fixture, MethodInfo method)
        {
            if (!method.IsPublic)
            {
                return new ITestCase[] { new IgnoreTestCase(fixture.Name, method.Name, "Test methods must be public.") };
            }

            if (method.GetParameters().Length > 0)
            {
                return new ITestCase[] { new IgnoreTestCase(fixture.Name, method.Name, "Test methods mustn't take parameters.") };
            }

            if (method.ReturnType != typeof(void))
            {
                return new ITestCase[] { new IgnoreTestCase(fixture.Name, method.Name, "Test methods must return void.") };
            }

            return new ITestCase[] { new MethodTestCase(fixture.Name, method) };
        }
    }
}
