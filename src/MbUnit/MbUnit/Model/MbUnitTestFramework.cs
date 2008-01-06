// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Reflection;
using MbUnit.Framework;
using Gallio.Hosting;
using Gallio.Model;
using MbUnit.Properties;

namespace MbUnit.Model
{
    /// <summary>
    /// Provides support for the MbUnit v3 test framework.
    /// </summary>
    public class MbUnitTestFramework : BaseTestFramework
    {
        /// <inheritdoc />
        public override string Name
        {
            get { return Resources.MbUnitTestFramework_FrameworkName; }
        }

        /// <inheritdoc />
        public override ITestExplorer CreateTestExplorer(TestModel testModel)
        {
            return new MbUnitTestExplorer(testModel);
        }

        /// <inheritdoc />
        public override void PrepareTestPackage(TestPackage package)
        {
            foreach (IAssemblyInfo assembly in package.Assemblies)
            {
                foreach (AssemblyResolverAttribute resolverAttribute in
                    AttributeUtils.GetAttributes<AssemblyResolverAttribute>(assembly, false))
                {
                    Type type = resolverAttribute.AssemblyResolverType;
                    try
                    {
                        IAssemblyResolver resolver = (IAssemblyResolver)Activator.CreateInstance(type);
                        Loader.AssemblyResolverManager.AddAssemblyResolver(resolver);
                    }
                    catch (Exception ex)
                    {
                        throw new ModelException(String.Format("Failed to create custom assembly resolver type '{0}'.", type), ex);
                    }
                }
            }
        }
    }
}