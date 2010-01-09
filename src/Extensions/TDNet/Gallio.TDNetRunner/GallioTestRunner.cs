// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Reflection;
using Gallio.TDNetRunner.Facade;
using TestDriven.Framework;

namespace Gallio.TDNetRunner
{
    /// <summary>
    /// Gallio test runner for TestDriven.NET.
    /// </summary>
    public class GallioTestRunner : BaseTestRunner, ITestRunner
    {
        public TestRunState RunAssembly(ITestListener testListener, Assembly assembly)
        {
            if (testListener == null)
                throw new ArgumentNullException("testListener");
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            FacadeOptions options = FacadeOptions.ReadFromRegistry();
            FacadeTestRunState result = TestRunner.Run(new AdapterFacadeTestListener(testListener), EnvironmentManager.GetAssemblyPath(assembly), null, options);
            return FacadeUtils.ToTestRunState(result);
        }

        public TestRunState RunNamespace(ITestListener testListener, Assembly assembly, string ns)
        {
            if (testListener == null)
                throw new ArgumentNullException("testListener");
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            if (ns == null)
                throw new ArgumentNullException("ns");

            FacadeOptions options = FacadeOptions.ReadFromRegistry();
            FacadeTestRunState result = TestRunner.Run(new AdapterFacadeTestListener(testListener), EnvironmentManager.GetAssemblyPath(assembly), @"N:" + ns, options);
            return FacadeUtils.ToTestRunState(result);
        }

        public TestRunState RunMember(ITestListener testListener, Assembly assembly, MemberInfo member)
        {
            if (testListener == null)
                throw new ArgumentNullException("testListener");
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            if (member == null)
                throw new ArgumentNullException("member");

            FacadeOptions options = FacadeOptions.ReadFromRegistry();
            FacadeTestRunState result = TestRunner.Run(new AdapterFacadeTestListener(testListener),
                EnvironmentManager.GetAssemblyPath(assembly), FacadeUtils.ToCref(member), options);
            return FacadeUtils.ToTestRunState(result);
        }
    }
}