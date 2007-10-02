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

extern alias MbUnit2;
using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Core.Model;
using MbUnit2::MbUnit.Framework;

namespace MbUnit._Framework.Tests
{
    /// <summary>
    /// Provides assertions for common Gallio framework types.
    /// </summary>
    public static class FrameworkAssert
    {
        public static void AreEqual(TemplateModel expected, TemplateModel actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            AreEqual(expected.RootTemplate, actual.RootTemplate);
        }

        public static void AreEqual(TemplateData expected, TemplateData actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            AreEqual(expected.CodeReference, actual.CodeReference);
            AreEqual(expected.Metadata, actual.Metadata);
            Assert.AreEqual(expected.IsGenerator, actual.IsGenerator);

            MbUnit.Framework.InterimAssert.WithPairs(expected.Children, actual.Children, AreEqual);
            MbUnit.Framework.InterimAssert.WithPairs(expected.Parameters, actual.Parameters, AreEqual);
        }

        public static void AreEqual(TemplateParameterData expected, TemplateParameterData actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Index, actual.Index);
            Assert.AreEqual(expected.TypeName, actual.TypeName);
            AreEqual(expected.CodeReference, actual.CodeReference);
            AreEqual(expected.Metadata, actual.Metadata);
        }

        public static void AreEqual(TestModel expected, TestModel actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            AreEqual(expected.RootTest, actual.RootTest);
        }

        public static void AreEqual(TestData expected, TestData actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.IsTestCase, actual.IsTestCase);
            AreEqual(expected.CodeReference, actual.CodeReference);
            AreEqual(expected.Metadata, actual.Metadata);

            MbUnit.Framework.InterimAssert.WithPairs(expected.Children, actual.Children, AreEqual);
        }

        public static void AreEqual(MetadataMap expected, MetadataMap actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            MbUnit.Framework.InterimAssert.WithKeyedPairs(expected.Entries, actual.Entries, CollectionAssert.AreElementsEqual);
        }

        public static void AreEqual(CodeReference expected, CodeReference actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.AreEqual(expected.AssemblyName, actual.AssemblyName);
            Assert.AreEqual(expected.NamespaceName, actual.NamespaceName);
            Assert.AreEqual(expected.TypeName, actual.TypeName);
            Assert.AreEqual(expected.MemberName, actual.MemberName);
            Assert.AreEqual(expected.ParameterName, actual.ParameterName);
        }
    }
}
