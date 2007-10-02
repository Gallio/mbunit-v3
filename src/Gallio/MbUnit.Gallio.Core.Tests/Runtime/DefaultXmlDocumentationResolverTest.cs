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
using System.Reflection;
using System.Text;
using Castle.Core.Logging;
using MbUnit._Framework.Tests;
using MbUnit.Core.Runtime;
using MbUnit.TestResources.Gallio;
using MbUnit2::MbUnit.Framework;
using Rhino.Mocks;

namespace MbUnit.Core.Tests.Runtime
{
    [TestFixture]
    [Author("Jeff")]
    [TestsOn(typeof(DefaultXmlDocumentationResolver))]
    public class DefaultXmlDocumentationResolverTest : BaseUnitTest
    {
        private DefaultXmlDocumentationResolver resolver;

        public override void SetUp()
        {
            base.SetUp();

            resolver = new DefaultXmlDocumentationResolver();
        }

        [Test]
        [ExpectedArgumentNullException]
        public void GetXmlDocumentation_Type_ThrowsIfNull()
        {
            resolver.GetXmlDocumentation((Type)null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void GetXmlDocumentation_Field_ThrowsIfNull()
        {
            resolver.GetXmlDocumentation((FieldInfo)null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void GetXmlDocumentation_Property_ThrowsIfNull()
        {
            resolver.GetXmlDocumentation((PropertyInfo)null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void GetXmlDocumentation_Event_ThrowsIfNull()
        {
            resolver.GetXmlDocumentation((EventInfo)null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void GetXmlDocumentation_Method_ThrowsIfNull()
        {
            resolver.GetXmlDocumentation((MethodBase)null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void GetXmlDocumentation_Member_ThrowsIfNull()
        {
            resolver.GetXmlDocumentation((MemberInfo)null);
        }

        [Test]
        public void GetXmlDocumentation_Type_ReturnsNullIfUndocumented()
        {
            Type type = typeof(DocumentedClass.UndocumentedNestedClass);
            Assert.IsNull(resolver.GetXmlDocumentation(type));
            Assert.IsNull(resolver.GetXmlDocumentation((MemberInfo) type));
        }

        [Test]
        public void GetXmlDocumentation_Field_ReturnsNullIfUndocumented()
        {
            FieldInfo field = typeof(DocumentedClass.UndocumentedNestedClass).GetField("UndocumentedField");
            Assert.IsNull(resolver.GetXmlDocumentation(field));
            Assert.IsNull(resolver.GetXmlDocumentation((MemberInfo)field));
        }

        [Test]
        public void GetXmlDocumentation_Property_ReturnsNullIfUndocumented()
        {
            PropertyInfo property = typeof(DocumentedClass.UndocumentedNestedClass).GetProperty("UndocumentedProperty");
            Assert.IsNull(resolver.GetXmlDocumentation(property));
            Assert.IsNull(resolver.GetXmlDocumentation((MemberInfo)property));
        }

        [Test]
        public void GetXmlDocumentation_Event_ReturnsNullIfUndocumented()
        {
            EventInfo @event = typeof(DocumentedClass.UndocumentedNestedClass).GetEvent("UndocumentedEvent");
            Assert.IsNull(resolver.GetXmlDocumentation(@event));
            Assert.IsNull(resolver.GetXmlDocumentation((MemberInfo)@event));
        }

        [Test]
        public void GetXmlDocumentation_Method_ReturnsNullIfUndocumented()
        {
            MethodInfo method = typeof(DocumentedClass.UndocumentedNestedClass).GetMethod("UndocumentedMethod");
            Assert.IsNull(resolver.GetXmlDocumentation(method));
            Assert.IsNull(resolver.GetXmlDocumentation((MemberInfo)method));
        }

        [Test]
        public void GetXmlDocumentation_Type()
        {
            Type type = typeof(DocumentedClass);
            Assert.AreEqual("<summary>\nA documented class.\n</summary>\n<remarks>\nThe XML documentation of this test is significant.\n  Including the leading whitespace on this line.\n    And the extra 8 trailing spaces on this line!\n</remarks>",
                resolver.GetXmlDocumentation(type));
            Assert.AreEqual("<summary>\nA documented class.\n</summary>\n<remarks>\nThe XML documentation of this test is significant.\n  Including the leading whitespace on this line.\n    And the extra 8 trailing spaces on this line!\n</remarks>",
                resolver.GetXmlDocumentation((MemberInfo) type));
        }

        [Test]
        public void GetXmlDocumentation_GenericNestedType()
        {
            Type type = typeof(DocumentedClass.GenericNestedClass<int>);
            Assert.AreEqual("<summary>\nA documented generic nested class.\n</summary>",
                resolver.GetXmlDocumentation(type));
            Assert.AreEqual("<summary>\nA documented generic nested class.\n</summary>",
                resolver.GetXmlDocumentation((MemberInfo) type));
        }

        [Test]
        public void GetXmlDocumentation_GenericNestedTypeDefinition()
        {
            Type type = typeof(DocumentedClass.GenericNestedClass<>);
            Assert.AreEqual("<summary>\nA documented generic nested class.\n</summary>",
                resolver.GetXmlDocumentation(type));
            Assert.AreEqual("<summary>\nA documented generic nested class.\n</summary>",
                resolver.GetXmlDocumentation((MemberInfo)type));
        }

        [Test]
        public void GetXmlDocumentation_Field()
        {
            FieldInfo field = typeof(DocumentedClass).GetField("DocumentedField");
            Assert.AreEqual("<summary>\nA documented field.\n</summary>",
                resolver.GetXmlDocumentation(field));
            Assert.AreEqual("<summary>\nA documented field.\n</summary>",
                resolver.GetXmlDocumentation((MemberInfo)field));
        }

        [Test]
        public void GetXmlDocumentation_Property()
        {
            PropertyInfo property = typeof(DocumentedClass).GetProperty("DocumentedProperty");
            Assert.AreEqual("<summary>\nA documented property.\n</summary>",
                resolver.GetXmlDocumentation(property));
            Assert.AreEqual("<summary>\nA documented property.\n</summary>",
                resolver.GetXmlDocumentation((MemberInfo)property));
        }

        [Test]
        public void GetXmlDocumentation_Event()
        {
            EventInfo @event = typeof(DocumentedClass).GetEvent("DocumentedEvent");
            Assert.AreEqual("<summary>\nA documented event.\n</summary>",
                resolver.GetXmlDocumentation(@event));
            Assert.AreEqual("<summary>\nA documented event.\n</summary>",
                resolver.GetXmlDocumentation((MemberInfo)@event));
        }

        [Test]
        public void GetXmlDocumentation_Indexer()
        {
            PropertyInfo property = typeof(DocumentedClass).GetProperty("Item");
            Assert.AreEqual("<summary>\nA documented indexer.\n</summary>",
                resolver.GetXmlDocumentation(property));
            Assert.AreEqual("<summary>\nA documented indexer.\n</summary>",
                resolver.GetXmlDocumentation((MemberInfo)property));
        }

        [Test]
        public void GetXmlDocumentation_Method()
        {
            MethodInfo method = typeof(DocumentedClass).GetMethod("DocumentedMethod", new Type[] { });
            Assert.AreEqual("<summary>\nA documented method.\n</summary>",
                resolver.GetXmlDocumentation(method));
            Assert.AreEqual("<summary>\nA documented method.\n</summary>",
                resolver.GetXmlDocumentation((MemberInfo)method));
        }

        [Test]
        public void GetXmlDocumentation_OverloadedMethodWithParameters()
        {
            MethodInfo method = typeof(DocumentedClass).GetMethod("DocumentedMethod",
                new Type[] { typeof(int), typeof(DocumentedClass), typeof(DocumentedClass.GenericNestedClass<int>) });
            Assert.AreEqual("<summary>\nA documented overloaded method with parameters.\n</summary>",
                resolver.GetXmlDocumentation(method));
            Assert.AreEqual("<summary>\nA documented overloaded method with parameters.\n</summary>",
                resolver.GetXmlDocumentation((MemberInfo)method));
        }

        [Test]
        public void GetXmlDocumentation_GenericMethodWithParameters()
        {
            MethodInfo method = typeof(DocumentedClass.GenericNestedClass<int>).
                GetMethod("DocumentedGenericMethodWithParameters");
            Assert.AreEqual("<summary>\nA documented generic method with parameters.\n</summary>",
                resolver.GetXmlDocumentation(method));
            Assert.AreEqual("<summary>\nA documented generic method with parameters.\n</summary>",
                resolver.GetXmlDocumentation((MemberInfo)method));
        }

        [Test]
        public void GetXmlDocumentation_GenericMethodDefinitionWithParameters()
        {
            MethodInfo method = typeof(DocumentedClass.GenericNestedClass<int>).
                GetMethod("DocumentedGenericMethodWithParameters").GetGenericMethodDefinition();
            Assert.AreEqual("<summary>\nA documented generic method with parameters.\n</summary>",
                resolver.GetXmlDocumentation(method));
            Assert.AreEqual("<summary>\nA documented generic method with parameters.\n</summary>",
                resolver.GetXmlDocumentation((MemberInfo)method));
        }

        [Test]
        public void GetXmlDocumentation_GenericIndexer()
        {
            PropertyInfo method = typeof(DocumentedClass.GenericNestedClass<int>).GetProperty("Item");
            Assert.AreEqual("<summary>\nA documented generic indexer.\n</summary>",
                resolver.GetXmlDocumentation(method));
            Assert.AreEqual("<summary>\nA documented generic indexer.\n</summary>",
                resolver.GetXmlDocumentation((MemberInfo)method));
        }

        [Test]
        public void GetXmlDocumentation_OperatorMethod()
        {
            MethodInfo method = typeof(DocumentedClass).GetMethod("op_Addition");
            Assert.AreEqual("<summary>\nA documented operator.\n</summary>",
                resolver.GetXmlDocumentation(method));
            Assert.AreEqual("<summary>\nA documented operator.\n</summary>",
                resolver.GetXmlDocumentation((MemberInfo)method));
        }

        [Test]
        public void GetXmlDocumentation_ImplicitConversionOperatorMethod()
        {
            MethodInfo method = typeof(DocumentedClass).GetMethod("op_Implicit");
            Assert.AreEqual("<summary>\nA documented implicit conversion operator.\n</summary>",
                resolver.GetXmlDocumentation(method));
            Assert.AreEqual("<summary>\nA documented implicit conversion operator.\n</summary>",
                resolver.GetXmlDocumentation((MemberInfo)method));
        }

        [Test]
        public void GetXmlDocumentation_ExplicitConversionOperatorMethod()
        {
            MethodInfo method = typeof(DocumentedClass).GetMethod("op_Explicit");
            Assert.AreEqual("<summary>\nA documented explicit conversion operator.\n</summary>",
                resolver.GetXmlDocumentation(method));
            Assert.AreEqual("<summary>\nA documented explicit conversion operator.\n</summary>",
                resolver.GetXmlDocumentation((MemberInfo)method));
        }

        [Test]
        public void GetXmlDocumentation_ConstructorMethod()
        {
            ConstructorInfo method = typeof(DocumentedClass).GetConstructor(new Type[] { });
            Assert.AreEqual("<summary>\nA documented constructor.\n</summary>",
                resolver.GetXmlDocumentation(method));
            Assert.AreEqual("<summary>\nA documented constructor.\n</summary>",
                resolver.GetXmlDocumentation((MemberInfo)method));
        }

        [Test]
        public void GetXmlDocumentation_ConstructorMethodWithParameters()
        {
            ConstructorInfo method = typeof(DocumentedClass).GetConstructor(new Type[] { typeof(int) });
            Assert.AreEqual("<summary>\nA documented constructor with parameters.\n</summary>",
                resolver.GetXmlDocumentation(method));
            Assert.AreEqual("<summary>\nA documented constructor with parameters.\n</summary>",
                resolver.GetXmlDocumentation((MemberInfo)method));
        }

        [Test]
        public void GetXmlDocumentation_Finalizer()
        {
            MethodInfo method = typeof(DocumentedClass).GetMethod("Finalize", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.AreEqual("<summary>\nA documented finalizer.\n</summary>",
                resolver.GetXmlDocumentation(method));
            Assert.AreEqual("<summary>\nA documented finalizer.\n</summary>",
                resolver.GetXmlDocumentation((MemberInfo)method));
        }

        [Test]
        public void GetXmlDocumentation_LogsWarningIfXmlDocFileNotFound()
        {
            ILogger mockLogger = Mocks.CreateMock<ILogger>();
            mockLogger.WarnFormat((Exception)null, "", null);
            LastCall.IgnoreArguments();
            Mocks.ReplayAll();

            resolver.Logger = mockLogger;

            // The mock logger's type can't possibly have documentation because it is dynamically generated.
            Assert.IsNull(resolver.GetXmlDocumentation(mockLogger.GetType()));
        }
    }
}
