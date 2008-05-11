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

extern alias MbUnit2;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Utilities;
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
    /// framework would have produced.  This mechanism is used when the test
    /// code is not loaded into the AppDomain.  Obviously the tests that are
    /// produced are not executable but because the generated test ids are
    /// the same as those produced by the native framework, they can be used
    /// to build a list of tests to be executed later.
    /// </summary>
    /// <todo author="jeff">
    /// Support more standard MbUnit v2 features.
    /// Provide better diagnostic output in cases where unsupported extensions
    /// are used.
    /// </todo>
    /// <seealso cref="MbUnit2NativeTestExplorer"/>
    internal static class MbUnit2ReflectiveTestExplorer
    {
        public static ITest BuildAssemblyTest(TestModel testModel, IAssemblyInfo assembly, ICollection<KeyValuePair<ITest, string>> unresolvedDependencies)
        {
            BaseTest assemblyTest = new BaseTest(assembly.Name, assembly);
            assemblyTest.Kind = TestKinds.Assembly;

            MbUnit2MetadataUtils.PopulateAssemblyMetadata(assemblyTest, assembly);

            foreach (AssemblyDependsOnAttribute2 attrib in AttributeUtils.GetAttributes<AssemblyDependsOnAttribute2>(assembly, false))
                unresolvedDependencies.Add(new KeyValuePair<ITest, string>(assemblyTest, attrib.AssemblyName));

            foreach (ITypeInfo type in assembly.GetExportedTypes())
                BuildFixturesFromType(testModel, assemblyTest, type);

            return assemblyTest;
        }

        private static void BuildFixturesFromType(TestModel testModel, ITest parent, ITypeInfo type)
        {
            try
            {
                foreach (TestFixturePatternAttribute2 attrib in AttributeUtils.GetAttributes<TestFixturePatternAttribute2>(type, true))
                    BuildTestFixtureFromPatternAttribute(testModel, parent, type, attrib);
            }
            catch (Exception ex)
            {
                testModel.AddAnnotation(new Annotation(AnnotationType.Error, type,
                    "An exception was thrown while exploring an MbUnit v2 test type.", ex));
            }
        }

        private static void BuildTestFixtureFromPatternAttribute(TestModel testModel, ITest parent, ITypeInfo type, TestFixturePatternAttribute2 attrib)
        {
            BaseTest fixtureTest = new BaseTest(type.Name, type);
            fixtureTest.Kind = TestKinds.Fixture;

            MbUnit2MetadataUtils.PopulateFixtureMetadata(fixtureTest, type);

            TestFixtureAttribute2 fixtureAttrib = attrib as TestFixtureAttribute2;
            if (fixtureAttrib != null)
            {
                PopulateTestFixture(testModel, fixtureTest, type, fixtureAttrib);
            }
            else
            {
                ThrowUnsupportedAttribute(attrib);
            }

            parent.AddChild(fixtureTest);
        }

        private static void PopulateTestFixture(TestModel testModel, ITest fixtureTest, ITypeInfo type, TestFixtureAttribute2 fixtureAttrib)
        {
            IMethodInfo setUpMethod = GetMethodWithAttribute<SetUpAttribute2>(type);
            IMethodInfo tearDownMethod = GetMethodWithAttribute<TearDownAttribute2>(type);

            string namePrefix = setUpMethod != null ? setUpMethod.Name + @"." : string.Empty;
            string nameSuffix = tearDownMethod != null ? @"." + tearDownMethod.Name : string.Empty;

            foreach (IMethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                BuildTestsFromMethod(testModel, fixtureTest, method, namePrefix, nameSuffix);
            }
        }

        private static void BuildTestsFromMethod(TestModel testModel, ITest parent, IMethodInfo method, string namePrefix, string nameSuffix)
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

        private static void BuildTest(ITest parent, IMethodInfo method, string namePrefix, string nameSuffix,
            TestAttribute2 attrib)
        {
            AddChildTest(parent, method, namePrefix, nameSuffix);
        }

        private static void BuildRowTest(ITest parent, IMethodInfo method, string namePrefix, string nameSuffix,
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

        private static void BuildCombinatorialTest(ITest parent, IMethodInfo method, string namePrefix, string nameSuffix,
            CombinatorialTestAttribute2 attrib)
        {
            ThrowUnsupportedAttribute(attrib);
        }

        private static void AddChildTest(ITest parent, IMethodInfo method, string namePrefix, string nameSuffix)
        {
            BaseTest test = new BaseTest(namePrefix + method.Name + nameSuffix, method);
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
