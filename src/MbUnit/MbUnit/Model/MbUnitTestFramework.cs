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
using System.Collections.Generic;
using Gallio.Collections;
using Gallio.Model.Reflection;
using MbUnit.Framework;
using Gallio.Hosting;
using Gallio.Model;

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
            get { return "MbUnit v3"; }
        }

        /// <inheritdoc />
        public override void PrepareAssemblies(IList<IAssemblyInfo> assemblies)
        {
            foreach (IAssemblyInfo assembly in assemblies)
            {
                foreach (AssemblyResolverAttribute resolverAttribute in
                    assembly.GetAttributes<AssemblyResolverAttribute>(false))
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

        /// <inheritdoc />
        public override void BuildTemplates(TemplateTreeBuilder builder, IList<IAssemblyInfo> assemblies)
        {
            IMultiMap<Version, IAssemblyInfo> map = ReflectionUtils.MapByAssemblyReferenceVersion(assemblies, @"MbUnit");
            foreach (KeyValuePair<Version, IList<IAssemblyInfo>> entry in map)
            {
                // Build templates for the contents of the assemblies that reference MbUnit v3
                // via reflection.  The attributes exercise a great deal of control over this
                // process so that it can be easily extended by users.
                Version frameworkVersion = entry.Key;
                MbUnitFrameworkTemplate frameworkTemplate = new MbUnitFrameworkTemplate(frameworkVersion);
                builder.Root.AddChild(frameworkTemplate);

                MbUnitTestBuilder testBuilder = new MbUnitTestBuilder(builder);

                foreach (IAssemblyInfo assembly in entry.Value)
                {
                    testBuilder.ProcessAssembly(frameworkTemplate, assembly);
                }
            }
        }
    }
}