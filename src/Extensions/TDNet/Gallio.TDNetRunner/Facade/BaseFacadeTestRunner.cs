// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

//#define RESIDENT

using System;
using System.Reflection;
using TestDriven.Framework;

#if RESIDENT
using TestDriven.Framework.Resident;
#endif

namespace Gallio.TDNetRunner.Facade
{
    /// <summary>
    /// An abstract implementation of the TestDriven.Net test runner interface.
    /// </summary>
    /// <remarks>
    /// This type is part of a facade that decouples the Gallio test runner from the TestDriven.Net interfaces.
    /// </remarks>
    public abstract class BaseFacadeTestRunner : ITestRunner, IDisposable
#if RESIDENT
        , IResidentTestRunner
#endif
    {
        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Aborts the test run.
        /// </summary>
        protected abstract void Abort();

        /// <summary>
        /// Runs tests.
        /// </summary>
        /// <param name="testListener">The test listener</param>
        /// <param name="assemblyPath">The test assembly path</param>
        /// <param name="cref">The cref expression that specifies the path of the test</param>
        /// <returns>The test run result</returns>
        protected abstract FacadeTestRunState Run(IFacadeTestListener testListener, string assemblyPath, string cref);

        /// <summary>
        /// Disposes the test runner.
        /// </summary>
        /// <param name="disposing">True if dispose was called</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        TestRunState ITestRunner.RunAssembly(ITestListener testListener, Assembly assembly)
        {
            if (testListener == null)
                throw new ArgumentNullException("testListener");
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            return FacadeUtils.ToTestRunState(Run(new AdapterFacadeTestListener(testListener), EnvironmentManager.GetAssemblyPath(assembly), null));
        }

        TestRunState ITestRunner.RunNamespace(ITestListener testListener, Assembly assembly, string ns)
        {
            if (testListener == null)
                throw new ArgumentNullException("testListener");
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            if (ns == null)
                throw new ArgumentNullException("ns");

            return FacadeUtils.ToTestRunState(Run(new AdapterFacadeTestListener(testListener), EnvironmentManager.GetAssemblyPath(assembly), @"N:" + ns));
        }

        TestRunState ITestRunner.RunMember(ITestListener testListener, Assembly assembly, MemberInfo member)
        {
            if (testListener == null)
                throw new ArgumentNullException("testListener");
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            if (member == null)
                throw new ArgumentNullException("member");

            return FacadeUtils.ToTestRunState(Run(new AdapterFacadeTestListener(testListener), EnvironmentManager.GetAssemblyPath(assembly),
                FacadeUtils.ToCref(member)));
        }

#if RESIDENT
        TestRunState IResidentTestRunner.Run(ITestListener testListener, string assemblyFile, string cref)
        {
            if (testListener == null)
                throw new ArgumentNullException("testListener");
            if (assemblyFile == null)
                throw new ArgumentNullException("assemblyFile");

            return FacadeUtils.ToTestRunState(Run(new AdapterFacadeTestListener(testListener), assemblyFile, cref));
        }

        void IResidentTestRunner.Abort()
        {
            Abort();
        }
#endif
    }
}
