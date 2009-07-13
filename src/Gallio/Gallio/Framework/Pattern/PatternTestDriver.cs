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
using System.Collections.Generic;
using System.Reflection;
using Gallio.Common.Policies;
using Gallio.Common.Reflection;
using Gallio.Model;
using Gallio.Model.Helpers;
using Gallio.Runtime;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Test driver for the pattern test framework.
    /// </summary>
    internal class PatternTestDriver : SimpleTestDriver
    {
        private readonly string[] frameworkIds;
        private readonly string frameworkName;

        public PatternTestDriver(string[] frameworkIds, string frameworkName)
        {
            this.frameworkIds = frameworkIds;
            this.frameworkName = frameworkName;
        }

        protected override object[] GetRemoteTestDriverArguments()
        {
            return new object[] { frameworkIds, frameworkName };
        }

        private IList<PatternTestFrameworkExtensionInfo> GetExtensions(IAssemblyInfo assembly)
        {
            ITestFrameworkManager frameworkManager = RuntimeAccessor.ServiceLocator.Resolve<ITestFrameworkManager>();

            var extensions = new List<PatternTestFrameworkExtensionInfo>();
            IList<AssemblyName> assemblyReferences = assembly.GetReferencedAssemblies();

            foreach (var frameworkHandle in frameworkManager.FrameworkHandles)
            {
                if (Array.IndexOf(frameworkIds, frameworkHandle.Id) < 0)
                    continue;

                if (frameworkHandle.GetTraits().IsFrameworkCompatibleWithAssemblyReferences(assemblyReferences))
                {
                    try
                    {
                        var framework = (PatternTestFramework) frameworkHandle.GetComponent();
                        extensions.AddRange(framework.GetExtensions(assembly));
                    }
                    catch (Exception ex)
                    {
                        UnhandledExceptionPolicy.Report(
                            "A pattern test framework extension threw an exception while enumerating referenced extensions for an assembly.",
                            ex);
                    }
                }
            }

            extensions.Sort((a, b) => a.Id.CompareTo(b.Id));

            return extensions;
        }

        protected override string FrameworkName
        {
            get { return frameworkName; }
        }

        protected override bool IsTestImpl(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
        {
            var evaluator = CreateReflectionOnlyPatternEvaluator(reflectionPolicy);
            return evaluator.IsTest(codeElement, GetAutomaticPattern(codeElement));
        }

        protected override bool IsTestPartImpl(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
        {
            var evaluator = CreateReflectionOnlyPatternEvaluator(reflectionPolicy);
            return evaluator.IsTestPart(codeElement, GetAutomaticPattern(codeElement));
        }

        private static IPatternEvaluator CreateReflectionOnlyPatternEvaluator(IReflectionPolicy reflectionPolicy)
        {
            var testModelBuilder = new ReflectionOnlyTestModelBuilder(reflectionPolicy);
            return new DefaultPatternEvaluator(testModelBuilder, DeclarativePatternResolver.Instance);
        }

        private static IPattern GetAutomaticPattern(ICodeElementInfo element)
        {
            switch (element.Kind)
            {
                case CodeElementKind.Type:
                    return TestTypePatternAttribute.AutomaticInstance;

                case CodeElementKind.Field:
                case CodeElementKind.Property:
                    return TestParameterPatternAttribute.AutomaticInstance;

                case CodeElementKind.Assembly:
                case CodeElementKind.Constructor:
                case CodeElementKind.Parameter:
                case CodeElementKind.GenericParameter:
                case CodeElementKind.Namespace:
                case CodeElementKind.Event:
                case CodeElementKind.Method:
                    return null;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override TestExplorer CreateTestExplorer()
        {
            return new PatternTestExplorer(GetExtensions);
        }

        protected override TestController CreateTestController()
        {
            return new DelegatingTestController(test =>
            {
                var topTest = test as PatternTest;
                return topTest != null ? RuntimeAccessor.ServiceLocator.Resolve<PatternTestController>() : null;
            });
        }
    }
}
