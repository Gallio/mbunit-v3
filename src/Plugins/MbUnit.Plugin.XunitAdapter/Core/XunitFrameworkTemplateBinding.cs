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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MbUnit.Framework;
using MbUnit.Framework.Kernel.DataBinding;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Utilities;
using MbUnit.Plugin.XunitAdapter.Properties;
using Xunit.Sdk;

namespace MbUnit.Plugin.XunitAdapter.Core
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
        public IList<Assembly> Assemblies
        {
            get { return ((XunitFrameworkTemplate) Template).Assemblies; }
        }

        /// <inheritdoc />
        public override void BuildTests(TestTreeBuilder builder, ITest parent)
        {
            BaseTest frameworkTest = new BaseTest(Template.Name, CodeReference.Unknown, this);
            frameworkTest.Kind = ComponentKind.Framework;
            parent.AddChild(frameworkTest);

            foreach (Assembly assembly in Assemblies)
            {
                BuildAssemblyTest(frameworkTest, assembly);
            }
        }

        private void BuildAssemblyTest(ITest frameworkTest, Assembly assembly)
        {
            BaseTest assemblyTest = new BaseTest(assembly.GetName().Name, CodeReference.CreateFromAssembly(assembly), this);
            assemblyTest.Kind = ComponentKind.Assembly;
            frameworkTest.AddChild(assemblyTest);

            foreach (Type typeInfo in assembly.GetExportedTypes())
            {
                if (TypeReflectionUtilities.ContainsTestMethods(typeInfo))
                    BuildTypeTest(assemblyTest, typeInfo);
            }

            // Add assembly-level metadata.
            ReflectionUtils.PopulateMetadataFromAssembly(assembly, assemblyTest.Metadata);
        }

        private void BuildTypeTest(ITest assemblyTest, Type typeInfo)
        {
            XunitTest typeTest = new XunitTest(typeInfo.FullName, CodeReference.CreateFromType(typeInfo), this, typeInfo, null);
            typeTest.Kind = ComponentKind.Fixture;
            assemblyTest.AddChild(typeTest);

            foreach (MethodInfo methodInfo in TypeReflectionUtilities.GetTestMethods(typeInfo))
            {
                BuildMethodTest(typeTest, methodInfo);
            }

            // Add XML documentation.
            string xmlDocumentation = Runtime.XmlDocumentationResolver.GetXmlDocumentation(typeInfo);
            if (xmlDocumentation != null)
                typeTest.Metadata.SetValue(MetadataKeys.XmlDocumentation, xmlDocumentation);
        }

        private void BuildMethodTest(XunitTest typeTest, MethodInfo methodInfo)
        {
            XunitTest methodTest = new XunitTest(methodInfo.Name, CodeReference.CreateFromMember(methodInfo), this,
                methodInfo.ReflectedType, methodInfo);
            methodTest.Kind = ComponentKind.Test;
            methodTest.IsTestCase = true;
            typeTest.AddChild(methodTest);

            // Add skip reason.
            if (MethodReflectionUtilities.IsSkip(methodInfo))
            {
                string skipReason = MethodReflectionUtilities.GetSkipReason(methodInfo);
                if (skipReason != null)
                    methodTest.Metadata.SetValue(MetadataKeys.IgnoreReason, skipReason);
            }

            // Add properties.
            if (MethodReflectionUtilities.HasProperties(methodInfo))
            {
                foreach (KeyValuePair<string, string> entry in MethodReflectionUtilities.GetProperties(methodInfo))
                {
                    methodTest.Metadata.Add(entry.Key ?? @"", entry.Value ?? @"");
                }
            }

            // Add XML documentation.
            string xmlDocumentation = Runtime.XmlDocumentationResolver.GetXmlDocumentation(methodInfo);
            if (xmlDocumentation != null)
                methodTest.Metadata.SetValue(MetadataKeys.XmlDocumentation, xmlDocumentation);
        }
    }
}
