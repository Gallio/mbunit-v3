extern alias MbUnit2;
using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework.Kernel.Metadata;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Serialization;
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

        public static void AreEqual(TemplateInfo expected, TemplateInfo actual)
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

            MbUnit.Framework.InterimAssert.WithPairs(expected.Children, actual.Children, AreEqual);
            MbUnit.Framework.InterimAssert.WithPairs(expected.ParameterSets, actual.ParameterSets, AreEqual);
        }

        public static void AreEqual(TemplateParameterSetInfo expected, TemplateParameterSetInfo actual)
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

            MbUnit.Framework.InterimAssert.WithPairs(expected.Parameters, actual.Parameters, AreEqual);
        }

        public static void AreEqual(TemplateParameterInfo expected, TemplateParameterInfo actual)
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

        public static void AreEqual(TestInfo expected, TestInfo actual)
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
