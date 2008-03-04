// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Reflection;
using Castle.Core.Logging;
using Gallio.Reflection.Impl;
using MbUnit.TestResources;
using MbUnit.Framework;

namespace Gallio.Tests.Reflection.Impl
{
    [TestFixture]
    [Author("Jeff")]
    [TestsOn(typeof(XmlDocumentationUtils))]
    public class XmlDocumentationUtilsTest : BaseUnitTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void FormatId_ThrowsIfNull()
        {
            XmlDocumentationUtils.FormatId(null);
        }

        [Test]
        public void FormatId_Type()
        {
            Type type = typeof(DocumentedClass);
            Assert.AreEqual("T:MbUnit.TestResources.DocumentedClass",
                XmlDocumentationUtils.FormatId(type));
        }

        [Test]
        public void FormatId_GenericNestedType()
        {
            Type type = typeof(DocumentedClass.GenericNestedClass<int>);
            Assert.AreEqual("T:MbUnit.TestResources.DocumentedClass.GenericNestedClass`1",
                XmlDocumentationUtils.FormatId(type));
        }

        [Test]
        public void FormatId_GenericNestedTypeDefinition()
        {
            Type type = typeof(DocumentedClass.GenericNestedClass<>);
            Assert.AreEqual("T:MbUnit.TestResources.DocumentedClass.GenericNestedClass`1",
                XmlDocumentationUtils.FormatId(type));
        }

        [Test]
        public void FormatId_Field()
        {
            FieldInfo field = typeof(DocumentedClass).GetField("DocumentedField");
            Assert.AreEqual("F:MbUnit.TestResources.DocumentedClass.DocumentedField",
                XmlDocumentationUtils.FormatId(field));
        }

        [Test]
        public void FormatId_Property()
        {
            PropertyInfo property = typeof(DocumentedClass).GetProperty("DocumentedProperty");
            Assert.AreEqual("P:MbUnit.TestResources.DocumentedClass.DocumentedProperty",
                XmlDocumentationUtils.FormatId(property));
        }

        [Test]
        public void FormatId_Event()
        {
            EventInfo @event = typeof(DocumentedClass).GetEvent("DocumentedEvent");
            Assert.AreEqual("E:MbUnit.TestResources.DocumentedClass.DocumentedEvent",
                XmlDocumentationUtils.FormatId(@event));
        }

        [Test]
        public void FormatId_Indexer()
        {
            PropertyInfo property = typeof(DocumentedClass).GetProperty("Item");
            Assert.AreEqual("P:MbUnit.TestResources.DocumentedClass.Item(System.Int32)",
                XmlDocumentationUtils.FormatId(property));
        }

        [Test]
        public void FormatId_Method()
        {
            MethodInfo method = typeof(DocumentedClass).GetMethod("DocumentedMethod", new Type[] { });
            Assert.AreEqual("M:MbUnit.TestResources.DocumentedClass.DocumentedMethod",
                XmlDocumentationUtils.FormatId(method));
        }

        [Test]
        public void FormatId_OverloadedMethodWithParameters()
        {
            MethodInfo method = typeof(DocumentedClass).GetMethod("DocumentedMethod",
                new Type[] { typeof(int), typeof(DocumentedClass), typeof(DocumentedClass.GenericNestedClass<int>) });
            Assert.AreEqual("M:MbUnit.TestResources.DocumentedClass.DocumentedMethod(System.Int32,MbUnit.TestResources.DocumentedClass,MbUnit.TestResources.DocumentedClass.GenericNestedClass{System.Int32})",
                XmlDocumentationUtils.FormatId(method));
        }

        [Test]
        public void FormatId_GenericMethodWithParameters()
        {
            MethodInfo method = typeof(DocumentedClass.GenericNestedClass<int>).
                GetMethod("DocumentedGenericMethodWithParameters");
            Assert.AreEqual("M:MbUnit.TestResources.DocumentedClass.GenericNestedClass`1.DocumentedGenericMethodWithParameters``1(``0,`0,System.Int32)",
                XmlDocumentationUtils.FormatId(method));
        }

        [Test]
        public void FormatId_GenericMethodDefinitionWithParameters()
        {
            MethodInfo method = typeof(DocumentedClass.GenericNestedClass<int>).
                GetMethod("DocumentedGenericMethodWithParameters").GetGenericMethodDefinition();
            Assert.AreEqual("M:MbUnit.TestResources.DocumentedClass.GenericNestedClass`1.DocumentedGenericMethodWithParameters``1(``0,`0,System.Int32)",
                XmlDocumentationUtils.FormatId(method));
        }

        [Test]
        public void FormatId_GenericIndexer()
        {
            PropertyInfo method = typeof(DocumentedClass.GenericNestedClass<int>).GetProperty("Item");
            Assert.AreEqual("P:MbUnit.TestResources.DocumentedClass.GenericNestedClass`1.Item(`0)",
                XmlDocumentationUtils.FormatId(method));
        }

        [Test]
        public void FormatId_OperatorMethod()
        {
            MethodInfo method = typeof(DocumentedClass).GetMethod("op_Addition");
            Assert.AreEqual("M:MbUnit.TestResources.DocumentedClass.op_Addition(MbUnit.TestResources.DocumentedClass,MbUnit.TestResources.DocumentedClass)",
                XmlDocumentationUtils.FormatId(method));
        }

        [Test]
        public void FormatId_ImplicitConversionOperatorMethod()
        {
            MethodInfo method = typeof(DocumentedClass).GetMethod("op_Implicit");
            Assert.AreEqual("M:MbUnit.TestResources.DocumentedClass.op_Implicit(MbUnit.TestResources.DocumentedClass)~System.Int32",
                XmlDocumentationUtils.FormatId(method));
        }

        [Test]
        public void FormatId_ExplicitConversionOperatorMethod()
        {
            MethodInfo method = typeof(DocumentedClass).GetMethod("op_Explicit");
            Assert.AreEqual("M:MbUnit.TestResources.DocumentedClass.op_Explicit(MbUnit.TestResources.DocumentedClass)~System.Double",
                XmlDocumentationUtils.FormatId(method));
        }

        [Test]
        public void FormatId_ConstructorMethod()
        {
            ConstructorInfo method = typeof(DocumentedClass).GetConstructor(new Type[] { });
            Assert.AreEqual("M:MbUnit.TestResources.DocumentedClass.#ctor",
                XmlDocumentationUtils.FormatId(method));
        }

        [Test]
        public void FormatId_ConstructorMethodWithParameters()
        {
            ConstructorInfo method = typeof(DocumentedClass).GetConstructor(new Type[] { typeof(int) });
            Assert.AreEqual("M:MbUnit.TestResources.DocumentedClass.#ctor(System.Int32)",
                XmlDocumentationUtils.FormatId(method));
        }

        [Test]
        public void FormatId_Finalizer()
        {
            MethodInfo method = typeof(DocumentedClass).GetMethod("Finalize", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.AreEqual("M:MbUnit.TestResources.DocumentedClass.Finalize",
                XmlDocumentationUtils.FormatId(method));
        }

        [Test]
        [ExpectedArgumentNullException]
        public void GetXmlDocumentation_ThrowsIfNull()
        {
            XmlDocumentationUtils.GetXmlDocumentation(null);
        }

        [Test]
        public void GetXmlDocumentation_Type_ReturnsNullIfUndocumented()
        {
            Type type = typeof(DocumentedClass.UndocumentedNestedClass);
            Assert.IsNull(XmlDocumentationUtils.GetXmlDocumentation(type));
        }

        [Test]
        public void GetXmlDocumentation_Field_ReturnsNullIfUndocumented()
        {
            FieldInfo field = typeof(DocumentedClass.UndocumentedNestedClass).GetField("UndocumentedField");
            Assert.IsNull(XmlDocumentationUtils.GetXmlDocumentation(field));
        }

        [Test]
        public void GetXmlDocumentation_Property_ReturnsNullIfUndocumented()
        {
            PropertyInfo property = typeof(DocumentedClass.UndocumentedNestedClass).GetProperty("UndocumentedProperty");
            Assert.IsNull(XmlDocumentationUtils.GetXmlDocumentation(property));
        }

        [Test]
        public void GetXmlDocumentation_Event_ReturnsNullIfUndocumented()
        {
            EventInfo @event = typeof(DocumentedClass.UndocumentedNestedClass).GetEvent("UndocumentedEvent");
            Assert.IsNull(XmlDocumentationUtils.GetXmlDocumentation(@event));
        }

        [Test]
        public void GetXmlDocumentation_Method_ReturnsNullIfUndocumented()
        {
            MethodInfo method = typeof(DocumentedClass.UndocumentedNestedClass).GetMethod("UndocumentedMethod");
            Assert.IsNull(XmlDocumentationUtils.GetXmlDocumentation(method));
        }

        [Test]
        public void GetXmlDocumentation_Type()
        {
            Type type = typeof(DocumentedClass);
            Assert.AreEqual("<summary>\nA documented class.\n</summary>\n<remarks>\nThe XML documentation of this test is significant.\n  Including the leading whitespace on this line.\n    And the extra 8 trailing spaces on this line!\n</remarks>",
                XmlDocumentationUtils.GetXmlDocumentation(type));
        }

        [Test]
        public void GetXmlDocumentation_GenericNestedType()
        {
            Type type = typeof(DocumentedClass.GenericNestedClass<int>);
            Assert.AreEqual("<summary>\nA documented generic nested class.\n</summary>",
                XmlDocumentationUtils.GetXmlDocumentation(type));
        }

        [Test]
        public void GetXmlDocumentation_GenericNestedTypeDefinition()
        {
            Type type = typeof(DocumentedClass.GenericNestedClass<>);
            Assert.AreEqual("<summary>\nA documented generic nested class.\n</summary>",
                XmlDocumentationUtils.GetXmlDocumentation(type));
        }

        [Test]
        public void GetXmlDocumentation_Field()
        {
            FieldInfo field = typeof(DocumentedClass).GetField("DocumentedField");
            Assert.AreEqual("<summary>\nA documented field.\n</summary>",
                XmlDocumentationUtils.GetXmlDocumentation(field));
        }

        [Test]
        public void GetXmlDocumentation_Property()
        {
            PropertyInfo property = typeof(DocumentedClass).GetProperty("DocumentedProperty");
            Assert.AreEqual("<summary>\nA documented property.\n</summary>",
                XmlDocumentationUtils.GetXmlDocumentation(property));
        }

        [Test]
        public void GetXmlDocumentation_Event()
        {
            EventInfo @event = typeof(DocumentedClass).GetEvent("DocumentedEvent");
            Assert.AreEqual("<summary>\nA documented event.\n</summary>",
                XmlDocumentationUtils.GetXmlDocumentation(@event));
        }

        [Test]
        public void GetXmlDocumentation_Indexer()
        {
            PropertyInfo property = typeof(DocumentedClass).GetProperty("Item");
            Assert.AreEqual("<summary>\nA documented indexer.\n</summary>",
                XmlDocumentationUtils.GetXmlDocumentation(property));
        }

        [Test]
        public void GetXmlDocumentation_Method()
        {
            MethodInfo method = typeof(DocumentedClass).GetMethod("DocumentedMethod", new Type[] { });
            Assert.AreEqual("<summary>\nA documented method.\n</summary>",
                XmlDocumentationUtils.GetXmlDocumentation(method));
        }

        [Test]
        public void GetXmlDocumentation_OverloadedMethodWithParameters()
        {
            MethodInfo method = typeof(DocumentedClass).GetMethod("DocumentedMethod",
                new Type[] { typeof(int), typeof(DocumentedClass), typeof(DocumentedClass.GenericNestedClass<int>) });
            Assert.AreEqual("<summary>\nA documented overloaded method with parameters.\n</summary>",
                XmlDocumentationUtils.GetXmlDocumentation(method));
        }

        [Test]
        public void GetXmlDocumentation_GenericMethodWithParameters()
        {
            MethodInfo method = typeof(DocumentedClass.GenericNestedClass<int>).
                GetMethod("DocumentedGenericMethodWithParameters");
            Assert.AreEqual("<summary>\nA documented generic method with parameters.\n</summary>",
                XmlDocumentationUtils.GetXmlDocumentation(method));
        }

        [Test]
        public void GetXmlDocumentation_GenericMethodDefinitionWithParameters()
        {
            MethodInfo method = typeof(DocumentedClass.GenericNestedClass<int>).
                GetMethod("DocumentedGenericMethodWithParameters").GetGenericMethodDefinition();
            Assert.AreEqual("<summary>\nA documented generic method with parameters.\n</summary>",
                XmlDocumentationUtils.GetXmlDocumentation(method));
        }

        [Test]
        public void GetXmlDocumentation_GenericIndexer()
        {
            PropertyInfo method = typeof(DocumentedClass.GenericNestedClass<int>).GetProperty("Item");
            Assert.AreEqual("<summary>\nA documented generic indexer.\n</summary>",
                XmlDocumentationUtils.GetXmlDocumentation(method));
        }

        [Test]
        public void GetXmlDocumentation_OperatorMethod()
        {
            MethodInfo method = typeof(DocumentedClass).GetMethod("op_Addition");
            Assert.AreEqual("<summary>\nA documented operator.\n</summary>",
                XmlDocumentationUtils.GetXmlDocumentation(method));
        }

        [Test]
        public void GetXmlDocumentation_ImplicitConversionOperatorMethod()
        {
            MethodInfo method = typeof(DocumentedClass).GetMethod("op_Implicit");
            Assert.AreEqual("<summary>\nA documented implicit conversion operator.\n</summary>",
                XmlDocumentationUtils.GetXmlDocumentation(method));
        }

        [Test]
        public void GetXmlDocumentation_ExplicitConversionOperatorMethod()
        {
            MethodInfo method = typeof(DocumentedClass).GetMethod("op_Explicit");
            Assert.AreEqual("<summary>\nA documented explicit conversion operator.\n</summary>",
                XmlDocumentationUtils.GetXmlDocumentation(method));
        }

        [Test]
        public void GetXmlDocumentation_ConstructorMethod()
        {
            ConstructorInfo method = typeof(DocumentedClass).GetConstructor(new Type[] { });
            Assert.AreEqual("<summary>\nA documented constructor.\n</summary>",
                XmlDocumentationUtils.GetXmlDocumentation(method));
        }

        [Test]
        public void GetXmlDocumentation_ConstructorMethodWithParameters()
        {
            ConstructorInfo method = typeof(DocumentedClass).GetConstructor(new Type[] { typeof(int) });
            Assert.AreEqual("<summary>\nA documented constructor with parameters.\n</summary>",
                XmlDocumentationUtils.GetXmlDocumentation(method));
        }

        [Test]
        public void GetXmlDocumentation_Finalizer()
        {
            MethodInfo method = typeof(DocumentedClass).GetMethod("Finalize", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.AreEqual("<summary>\nA documented finalizer.\n</summary>",
                XmlDocumentationUtils.GetXmlDocumentation(method));
        }

        [Test]
        public void GetXmlDocumentation_ReturnsNullIfAssemblyIsDynamic()
        {
            ILogger mockLogger = Mocks.CreateMock<ILogger>();
            Mocks.ReplayAll();

            // The mock logger's type can't possibly have documentation because it is dynamically generated.
            Assert.IsNull(XmlDocumentationUtils.GetXmlDocumentation(mockLogger.GetType()));
        }
    }
}