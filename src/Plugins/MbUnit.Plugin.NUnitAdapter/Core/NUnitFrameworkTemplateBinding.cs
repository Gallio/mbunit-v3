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
using MbUnit.Framework.Kernel.Metadata;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Utilities;
using NUnit.Core;

namespace MbUnit.Plugin.NUnitAdapter.Core
{
    /// <summary>
    /// The NUnit test framework template binding.
    /// This binding performs full exploration of all tests in MUnit test
    /// assemblies during test construction.
    /// </summary>
    public class NUnitFrameworkTemplateBinding : BaseTemplateBinding
    {
        private string TestTypeMetadataKey = "NUnit.TestType";

        private SimpleTestRunner runner;

        /// <summary>
        /// Creates a template binding.
        /// </summary>
        /// <param name="template">The template that was bound</param>
        /// <param name="scope">The scope in which the binding occurred</param>
        /// <param name="arguments">The template arguments</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="template"/>,
        /// <paramref name="scope"/> or <paramref name="arguments"/> is null</exception>
        public NUnitFrameworkTemplateBinding(NUnitFrameworkTemplate template, TestScope scope,
            IDictionary<ITemplateParameter, object> arguments)
            : base(template, scope, arguments)
        {
        }

        /// <summary>
        /// Gets the list of assemblies.
        /// </summary>
        public IList<Assembly> Assemblies
        {
            get { return ((NUnitFrameworkTemplate) Template).Assemblies; }
        }

        /// <inheritdoc />
        public override void BuildTests(TestTreeBuilder builder)
        {
            LoadTestPackageIfNeeded();

            NUnitTest frameworkTest = CreateFrameworkTest(Scope);
            frameworkTest.Batch = new TestBatch("NUnit", delegate
            {
                return new NUnitTestController(runner);
            });

            NUnit.Core.ITest rootTest = runner.Test;
            if (rootTest != null)
                BuildTests(frameworkTest, rootTest);
        }

        private void LoadTestPackageIfNeeded()
        {
            if (runner != null)
                return;

            try
            {
                TestPackage package = new TestPackage("Tests");
                package.AutoBinPath = true;
                package.Settings.Add("AutoNamespaceSuites", true);

                foreach (Assembly assembly in Assemblies)
                    package.Assemblies.Add(assembly.Location);

                runner = new SimpleTestRunner();
                if (!runner.Load(package))
                    throw new ModelException("Cannot load one or more NUnit test assemblies.");
            }
            catch (Exception)
            {
                runner = null;
                throw;
            }
        }

        private NUnitTest CreateFrameworkTest(TestScope parentScope)
        {
            NUnitTest test = new NUnitTest(Template.Name, CodeReference.Unknown, parentScope, null);
            test.Kind = ComponentKind.Framework;

            // TODO: Is there any useful framework metadata?

            parentScope.ContainingTest.AddChild(test);
            return test;
        }

        private void BuildTests(NUnitTest parentTest, NUnit.Core.ITest nunitTest)
        {
            // TODO: Get Type and Method information (how??)

            NUnitTest test = new NUnitTest(nunitTest.TestName.FullName, CodeReference.Unknown, parentTest.Scope, nunitTest);
            test.Kind = nunitTest.IsSuite ? ComponentKind.Suite : ComponentKind.Test;

            // Populate metadata
            if (!String.IsNullOrEmpty(nunitTest.Description))
                test.Metadata.Entries.Add(MetadataConstants.DescriptionKey, nunitTest.Description);

            if (!String.IsNullOrEmpty(nunitTest.IgnoreReason))
                test.Metadata.Entries.Add(MetadataConstants.IgnoreReasonKey, nunitTest.IgnoreReason);

            foreach (string category in nunitTest.Categories)
                test.Metadata.Entries.Add(MetadataConstants.CategoryNameKey, category);

            foreach (DictionaryEntry entry in nunitTest.Properties)
                test.Metadata.Entries.Add(entry.Key.ToString(), entry.Value != null ? entry.Value.ToString() : null);

            test.Metadata.Entries.Add(TestTypeMetadataKey, nunitTest.TestType);

            parentTest.AddChild(test);

            // Build children.
            foreach (NUnit.Core.ITest childNUnitTest in nunitTest.Tests)
                BuildTests(test, childNUnitTest);
        }
    }
}
