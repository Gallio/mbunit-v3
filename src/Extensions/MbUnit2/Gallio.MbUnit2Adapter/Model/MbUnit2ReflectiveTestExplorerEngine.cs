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

extern alias MbUnit2;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Gallio.Common.Collections;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Model.Tree;
using AssemblyDependsOnAttribute2 = MbUnit2::MbUnit.Framework.AssemblyDependsOnAttribute;
using TestFixturePatternAttribute2 = MbUnit2::MbUnit.Core.Framework.TestFixturePatternAttribute;
using TestFixtureAttribute2 = MbUnit2::MbUnit.Framework.TestFixtureAttribute;
using SetUpAttribute2 = MbUnit2::MbUnit.Framework.SetUpAttribute;
using TearDownAttribute2 = MbUnit2::MbUnit.Framework.TearDownAttribute;
using TestAttribute2 = MbUnit2::MbUnit.Framework.TestAttribute;
using RowTestAttribute2 = MbUnit2::MbUnit.Framework.RowTestAttribute;
using RowAttribute2 = MbUnit2::MbUnit.Framework.RowAttribute;
using CombinatorialTestAttribute2 = MbUnit2::MbUnit.Framework.CombinatorialTestAttribute;

namespace Gallio.MbUnit2Adapter.Model
{
    /// <summary>
    /// Builds a test tree using reflection-only techniques that are intended
    /// to produce an identical structure to that which the native MbUnit v2
    /// framework would have produced.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This mechanism is used when the test code is not loaded into the AppDomain.  
    /// Obviously the tests that are produced are not executable but because the generated test ids are
    /// the same as those produced by the native framework, they can be used
    /// to build a list of tests to be executed later.
    /// </para>
    /// </remarks>
    /// <todo>
    /// TODO: Support more standard MbUnit v2 features.
    /// Provide better diagnostic output in cases where unsupported extensions are used.
    /// </todo>
    /// <seealso cref="MbUnit2NativeTestExplorerEngine"/>
    internal class MbUnit2ReflectiveTestExplorerEngine : MbUnit2TestExplorerEngine
    {
        private readonly TestModel testModel;
        private readonly IAssemblyInfo assembly;

        private Test assemblyTest;
        private bool fullyPopulated;
        private HashSet<ITypeInfo> populatedTypes;

        public MbUnit2ReflectiveTestExplorerEngine(TestModel testModel, IAssemblyInfo assembly)
        {
            this.testModel = testModel;
            this.assembly = assembly;
        }

        public override Test GetAssemblyTest()
        {
            return assemblyTest;
        }

        public override void ExploreAssembly(bool skipChildren, ICollection<KeyValuePair<Test, string>> unresolvedDependencies)
        {
            if (assemblyTest == null)
            {
                assemblyTest = BuildAssemblyTest(testModel.RootTest, unresolvedDependencies);
            }

            if (!skipChildren && ! fullyPopulated)
            {
                foreach (ITypeInfo type in assembly.GetExportedTypes())
                    ExploreTypeIfNotAlreadyPopulated(type);

                fullyPopulated = true;
            }
        }

        public override void ExploreType(ITypeInfo type)
        {
            if (fullyPopulated)
                return;

            ExploreTypeIfNotAlreadyPopulated(type);
        }

        private void ExploreTypeIfNotAlreadyPopulated(ITypeInfo type)
        {
            if (populatedTypes == null)
            {
                populatedTypes = new HashSet<ITypeInfo>();
            }
            else if (populatedTypes.Contains(type))
            {
                return;
            }

            BuildFixturesFromType(assemblyTest, type);
            populatedTypes.Add(type);
        }

        private Test BuildAssemblyTest(Test parent, ICollection<KeyValuePair<Test, string>> unresolvedDependencies)
        {
            Test assemblyTest = new Test(assembly.Name, assembly);
            PopulateAssemblyTestMetadata(assemblyTest, assembly);

            foreach (AssemblyDependsOnAttribute2 attrib in AttributeUtils.GetAttributes<AssemblyDependsOnAttribute2>(assembly, false))
                unresolvedDependencies.Add(new KeyValuePair<Test, string>(assemblyTest, attrib.AssemblyName));

            parent.AddChild(assemblyTest);
            return assemblyTest;
        }

        private void BuildFixturesFromType(Test parent, ITypeInfo type)
        {
            try
            {
                foreach (TestFixturePatternAttribute2 attrib in AttributeUtils.GetAttributes<TestFixturePatternAttribute2>(type, true))
                    BuildTestFixtureFromPatternAttribute(parent, type, attrib);
            }
            catch (Exception ex)
            {
                testModel.AddAnnotation(new Annotation(AnnotationType.Error, type,
                    "An exception was thrown while exploring an MbUnit v2 test type.", ex));
            }
        }

        private void BuildTestFixtureFromPatternAttribute(Test parent, ITypeInfo type, TestFixturePatternAttribute2 attrib)
        {
            Test fixtureTest = new Test(type.Name, type);
            fixtureTest.Kind = TestKinds.Fixture;

            MbUnit2MetadataUtils.PopulateFixtureMetadata(fixtureTest, type);

            TestFixtureAttribute2 fixtureAttrib = attrib as TestFixtureAttribute2;
            if (fixtureAttrib != null)
            {
                PopulateTestFixture(fixtureTest, type, fixtureAttrib);
            }
            else
            {
                ThrowUnsupportedAttribute(attrib);
            }

            parent.AddChild(fixtureTest);
        }

        private void PopulateTestFixture(Test fixtureTest, ITypeInfo type, TestFixtureAttribute2 fixtureAttrib)
        {
            IMethodInfo setUpMethod = GetMethodWithAttribute<SetUpAttribute2>(type);
            IMethodInfo tearDownMethod = GetMethodWithAttribute<TearDownAttribute2>(type);

            string namePrefix = setUpMethod != null ? setUpMethod.Name + @"." : string.Empty;
            string nameSuffix = tearDownMethod != null ? @"." + tearDownMethod.Name : string.Empty;

            foreach (IMethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                BuildTestsFromMethod(fixtureTest, method, namePrefix, nameSuffix);
            }
        }

        private void BuildTestsFromMethod(Test parent, IMethodInfo method, string namePrefix, string nameSuffix)
        {
            try
            {
                foreach (TestAttribute2 attrib in AttributeUtils.GetAttributes<TestAttribute2>(method, true))
                    BuildTest(parent, method, namePrefix, nameSuffix, attrib);

                foreach (RowTestAttribute2 attrib in AttributeUtils.GetAttributes<RowTestAttribute2>(method, true))
                    BuildRowTest(parent, method, namePrefix, nameSuffix, attrib);

                foreach (CombinatorialTestAttribute2 attrib in AttributeUtils.GetAttributes<CombinatorialTestAttribute2>(method, true))
                    BuildCombinatorialTest(parent, method, namePrefix, nameSuffix, attrib);
            }
            catch (Exception ex)
            {
                testModel.AddAnnotation(new Annotation(AnnotationType.Error, method,
                    "An exception was thrown while exploring an MbUnit v2 test method.", ex));
            }
        }

        private static void BuildTest(Test parent, IMethodInfo method, string namePrefix, string nameSuffix,
            TestAttribute2 attrib)
        {
            AddChildTest(parent, method, namePrefix, nameSuffix);
        }

        private static void BuildRowTest(Test parent, IMethodInfo method, string namePrefix, string nameSuffix,
            RowTestAttribute2 attrib)
        {
            foreach (RowAttribute2 rowAttrib in AttributeUtils.GetAttributes<RowAttribute2>(method, true))
            {
                // Note: The way the name is formatted must be identical to that which MbUnit v2 natively produces.
                object[] row = rowAttrib.GetRow();
                StringWriter rowName = new StringWriter(CultureInfo.InvariantCulture);
                rowName.Write('(');
                for (int i = 0; i < row.Length; i++)
                {
                    if (i != 0)
                        rowName.Write(',');
                    rowName.Write(row[i]);
                }
                rowName.Write(')');

                AddChildTest(parent, method, namePrefix, rowName + nameSuffix);
            }
        }

        private static void BuildCombinatorialTest(Test parent, IMethodInfo method, string namePrefix, string nameSuffix,
            CombinatorialTestAttribute2 attrib)
        {
            ThrowUnsupportedAttribute(attrib);
        }

        private static void AddChildTest(Test parent, IMethodInfo method, string namePrefix, string nameSuffix)
        {
            Test test = new Test(namePrefix + method.Name + nameSuffix, method);
            test.IsTestCase = true;
            test.Kind = TestKinds.Test;

            MbUnit2MetadataUtils.PopulateTestMetadata(test, method);

            parent.AddChild(test);
        }

        private static IMethodInfo GetMethodWithAttribute<T>(ITypeInfo type)
        {
            foreach (IMethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                if (AttributeUtils.HasAttribute<T>(method, true))
                    return method;
            }

            return null;
        }

        private static void ThrowUnsupportedAttribute(Attribute attrib)
        {
            throw new NotSupportedException(attrib.GetType().FullName);
        }
    }
}
