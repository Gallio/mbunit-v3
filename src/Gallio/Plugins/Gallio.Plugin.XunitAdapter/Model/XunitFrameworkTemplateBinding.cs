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
using Gallio.Model.Data;
using Gallio.Model;
using Gallio.Model.Reflection;
using Xunit.Sdk;
using ITypeInfo=Gallio.Model.Reflection.ITypeInfo;
using XunitMethodUtility = Xunit.Sdk.MethodUtility;
using XunitTypeUtility = Xunit.Sdk.TypeUtility;

namespace Gallio.Plugin.XunitAdapter.Model
{
    /// <summary>
    /// The Xunit test framework template binding.
    /// This binding performs full exploration of all tests in MUnit test
    /// assemblies during test construction.
    /// </summary>
    public class XunitFrameworkTemplateBinding : BaseTemplateBinding
    {
        /// <summary>
        /// Creates a template binding.
        /// </summary>
        /// <param name="template">The template that was bound</param>
        /// <param name="scope">The scope in which the binding occurred</param>
        /// <param name="arguments">The template arguments</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="template"/>,
        /// <paramref name="scope"/> or <paramref name="arguments"/> is null</exception>
        public XunitFrameworkTemplateBinding(XunitFrameworkTemplate template, TemplateBindingScope scope,
            IDictionary<ITemplateParameter, IDataFactory> arguments)
            : base(template, scope, arguments)
        {
        }

        /// <summary>
        /// Gets the list of assemblies.
        /// </summary>
        public IList<IAssemblyInfo> Assemblies
        {
            get { return ((XunitFrameworkTemplate) Template).Assemblies; }
        }

        /// <inheritdoc />
        public override void BuildTests(TestTreeBuilder builder, ITest parent)
        {
            BaseTest frameworkTest = new BaseTest(Template.Name, null, this);
            frameworkTest.Kind = ComponentKind.Framework;
            parent.AddChild(frameworkTest);

            foreach (IAssemblyInfo assembly in Assemblies)
            {
                BuildAssemblyTest(frameworkTest, assembly);
            }
        }

        private void BuildAssemblyTest(ITest frameworkTest, IAssemblyInfo assembly)
        {
            BaseTest assemblyTest = new BaseTest(assembly.Name, assembly, this);
            assemblyTest.Kind = ComponentKind.Assembly;
            frameworkTest.AddChild(assemblyTest);

            foreach (ITypeInfo type in assembly.GetExportedTypes())
            {
                XunitTypeInfoAdapter xunitTypeInfo = new XunitTypeInfoAdapter(type);

                ITestClassCommand command = TestClassCommandFactory.Make(xunitTypeInfo);
                if (command != null)
                    BuildTypeTest(assemblyTest, xunitTypeInfo, command);
            }

            // Add assembly-level metadata.
            ModelUtils.PopulateMetadataFromAssembly(assembly, assemblyTest.Metadata);
        }

        private void BuildTypeTest(ITest assemblyTest, XunitTypeInfoAdapter typeInfo, ITestClassCommand testClassCommand)
        {
            XunitTest typeTest = new XunitTest(typeInfo.Target.Name, typeInfo.Target, this, typeInfo, null);
            typeTest.Kind = ComponentKind.Fixture;
            assemblyTest.AddChild(typeTest);
            
            foreach (XunitMethodInfoAdapter methodInfo in testClassCommand.EnumerateTestMethods())
            {
                BuildMethodTest(typeTest, typeInfo, methodInfo);
            }

            // Add XML documentation.
            string xmlDocumentation = typeInfo.Target.GetXmlDocumentation();
            if (xmlDocumentation != null)
                typeTest.Metadata.SetValue(MetadataKeys.XmlDocumentation, xmlDocumentation);
        }

        private void BuildMethodTest(XunitTest typeTest, XunitTypeInfoAdapter typeInfo, XunitMethodInfoAdapter methodInfo)
        {
            XunitTest methodTest = new XunitTest(methodInfo.Name, methodInfo.Target, this,
                typeInfo, methodInfo);
            methodTest.Kind = ComponentKind.Test;
            methodTest.IsTestCase = true;
            typeTest.AddChild(methodTest);

            // Add skip reason.
            if (XunitMethodUtility.IsSkip(methodInfo))
            {
                string skipReason = XunitMethodUtility.GetSkipReason(methodInfo);
                if (skipReason != null)
                    methodTest.Metadata.SetValue(MetadataKeys.IgnoreReason, skipReason);
            }

            // Add traits.
            if (XunitMethodUtility.HasTraits(methodInfo))
            {
                foreach (KeyValuePair<string, string> entry in XunitMethodUtility.GetTraits(methodInfo))
                {
                    methodTest.Metadata.Add(entry.Key ?? @"", entry.Value ?? @"");
                }
            }

            // Add XML documentation.
            string xmlDocumentation = methodInfo.Target.GetXmlDocumentation();
            if (xmlDocumentation != null)
                methodTest.Metadata.SetValue(MetadataKeys.XmlDocumentation, xmlDocumentation);
        }
    }
}
