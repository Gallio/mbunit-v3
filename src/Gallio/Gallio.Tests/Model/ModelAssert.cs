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
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Model.Serialization;
using MbUnit.Framework;

namespace Gallio.Tests.Model
{
    /// <summary>
    /// Provides assertions for common Gallio framework types.
    /// </summary>
    public static class ModelAssert
    {
        public static void AreEqual(TestModelData expected, TestModelData actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            AreEqual(expected.RootTest, actual.RootTest);
        }

        public static void AreEqual(TestComponentData expected, TestComponentData actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.CodeReference, actual.CodeReference);
            Assert.AreEqual(expected.SourceLocation, actual.SourceLocation);

            AreEqual(expected.Metadata, actual.Metadata);
        }

        public static void AreEqual(TestData expected, TestData actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            AreEqual((TestComponentData)expected, actual);

            Assert.AreEqual(expected.IsTestCase, actual.IsTestCase);
            MbUnit.Framework.InterimAssert.WithPairs(expected.Children, actual.Children, AreEqual);
            MbUnit.Framework.InterimAssert.WithPairs(expected.Parameters, actual.Parameters, AreEqual);
        }

        public static void AreEqual(TestParameterData expected, TestParameterData actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            AreEqual((TestComponentData)expected, actual);

            Assert.AreEqual(expected.Index, actual.Index);
            Assert.AreEqual(expected.TypeName, actual.TypeName);
        }

        public static void AreEqual(MetadataMap expected, MetadataMap actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            MbUnit.Framework.InterimAssert.WithKeyedPairs(expected, actual, CollectionAssert.AreElementsEqual);
        }

        public static void AreEqual(TestStepData expected, TestStepData actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            AreEqual((TestComponentData)expected, actual);

            Assert.AreEqual(expected.FullName, actual.FullName);
            Assert.AreEqual(expected.ParentId, actual.ParentId);
            Assert.AreEqual(expected.TestInstanceId, actual.TestInstanceId);
        }

        public static void AreEqual(TestInstanceData expected, TestInstanceData actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            AreEqual((TestComponentData)expected, actual);

            Assert.AreEqual(expected.TestId, actual.TestId);
            Assert.AreEqual(expected.IsDynamic, actual.IsDynamic);
        }

        public static void AreEqual(TestResult expected, TestResult actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.AreEqual(expected.AssertCount, actual.AssertCount);
            Assert.AreEqual(expected.Duration, actual.Duration);
            Assert.AreEqual(expected.Outcome, actual.Outcome);
            Assert.AreEqual(expected.Status, actual.Status);
        }
    }
}