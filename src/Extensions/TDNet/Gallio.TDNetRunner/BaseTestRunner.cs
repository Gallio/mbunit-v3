// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Globalization;
using System.Reflection;
using Gallio.TDNetRunner.Core;
using System.IO;

namespace Gallio.TDNetRunner
{
    public abstract class BaseTestRunner : IDisposable
    {
        private static bool loaderAssemblyResolverInstalled;

        private IProxyTestRunner testRunner;

        protected BaseTestRunner()
        {
        }

        /// <inheritdoc />
        internal protected IProxyTestRunner TestRunner
        {
            get
            {
                if (testRunner == null)
                    testRunner = CreateLocalProxyTestRunner();
                return testRunner;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                testRunner = value;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (testRunner != null)
                testRunner.Dispose();
        }

        private static IProxyTestRunner CreateLocalProxyTestRunner()
        {
            InstallLoaderAssemblyResolverIfNeeded();
            return new LocalProxyTestRunner();
        }

        /// <summary>
        /// In the case where Gallio is being called from TDNet during a zero-registration
        /// test run (based on the contents of a *.tdnet file), we may 
        /// </summary>
        private static void InstallLoaderAssemblyResolverIfNeeded()
        {
            if (loaderAssemblyResolverInstalled)
                return;

            Assembly gallioTDNetRunnerAssembly = typeof(BaseTestRunner).Assembly;
            string gallioTDNetRunnerAssemblyPath = new Uri(gallioTDNetRunnerAssembly.CodeBase).LocalPath;
            string gallioTDNetRunnerAssemblyDir = Path.GetDirectoryName(gallioTDNetRunnerAssemblyPath);
            string gallioLoaderAssemblyPath = GetGallioLoaderAssemblyPath(gallioTDNetRunnerAssemblyDir);
            if (gallioLoaderAssemblyPath == null)
                return;

            AssemblyName gallioTDNetRunnerAssemblyName = gallioTDNetRunnerAssembly.GetName();
            var gallioLoaderAssemblyName = new AssemblyName("Gallio.Loader")
            {
                Version = gallioTDNetRunnerAssemblyName.Version,
                CultureInfo = CultureInfo.InvariantCulture
            };
            gallioLoaderAssemblyName.SetPublicKeyToken(gallioTDNetRunnerAssemblyName.GetPublicKeyToken());

            AppDomain.CurrentDomain.AssemblyResolve += (sender, e) =>
            {
                if (e.Name == gallioLoaderAssemblyName.Name
                    || e.Name == gallioLoaderAssemblyName.FullName)
                {
                    return Assembly.LoadFrom(gallioLoaderAssemblyPath);
                }

                return null;
            };

            loaderAssemblyResolverInstalled = true;
        }

        private static string GetGallioLoaderAssemblyPath(string gallioTDNetRunnerAssemblyDir)
        {
            // Case 1: Gallio.Loader.dll in same directory as Gallio.TDNetRunner.
            //         This is common during local development of Gallio.
            string gallioLoaderAssemblyPath = Path.Combine(gallioTDNetRunnerAssemblyDir, "Gallio.Loader.dll");
            if (File.Exists(gallioLoaderAssemblyPath))
                return gallioLoaderAssemblyPath;

            // Case 2: Typical Gallio installation where Gallio.TDNetRunner.dll is in the TDNet
            //         subdirectory and Gallio.Loader.dll is in the Loader subdirectory.
            string gallioDir = Path.GetDirectoryName(gallioTDNetRunnerAssemblyDir);
            if (gallioDir != null)
            {
                gallioLoaderAssemblyPath = Path.Combine(gallioDir, @"Loader\Gallio.Loader.dll");
                if (File.Exists(gallioLoaderAssemblyPath))
                    return gallioLoaderAssemblyPath;
            }

            return null;
        }
    }
}
