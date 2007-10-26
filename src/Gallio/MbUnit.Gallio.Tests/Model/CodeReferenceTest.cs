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
using System.Diagnostics;
using System.Reflection;
using System.Security.Permissions;
using MbUnit.Model;
using MbUnit.Framework;
using MbUnit.Utilities;

namespace MbUnit.Tests.Model
{
    [TestFixture]
    [TestsOn(typeof(CodeReference))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class CodeReferenceTest
    {
        private Assembly assembly;
        private string @namespace;
        private Type type;
        private MethodInfo member;
        private ParameterInfo parameter;

        [SetUp]
        public void SetUp()
        {
            type = typeof(CodeReferenceTest);
            assembly = type.Assembly;
            @namespace = type.Namespace;
            member = type.GetMethod("Dummy", BindingFlags.Instance | BindingFlags.NonPublic);
            parameter = member.GetParameters()[0];
        }

        [Test]
        public void Unknown_IsAllNulls()
        {
            Assert.IsNull(CodeReference.Unknown.AssemblyName);
            Assert.IsNull(CodeReference.Unknown.NamespaceName);
            Assert.IsNull(CodeReference.Unknown.TypeName);
            Assert.IsNull(CodeReference.Unknown.MemberName);
            Assert.IsNull(CodeReference.Unknown.ParameterName);

            Assert.AreEqual(CodeReferenceKind.Unknown, CodeReference.Unknown.Kind);
        }

        [Test]
        public void Copy()
        {
            CodeReference original = new CodeReference(assembly, @namespace, type, member, parameter);
            CodeReference copy = original.Copy();

            Assert.AreNotSame(original, copy);
            Assert.AreEqual(original, copy);
        }

        [Test]
        public void ReadOnlyCopy()
        {
            CodeReference original = new CodeReference(assembly, @namespace, type, member, parameter);
            CodeReference copy = original.ReadOnlyCopy();

            Assert.AreNotSame(original, copy);
            Assert.AreEqual(original, copy);

            InterimAssert.Throws<InvalidOperationException>(delegate { copy.AssemblyName = ""; });
            InterimAssert.Throws<InvalidOperationException>(delegate { copy.NamespaceName = ""; });
            InterimAssert.Throws<InvalidOperationException>(delegate { copy.TypeName = ""; });
            InterimAssert.Throws<InvalidOperationException>(delegate { copy.MemberName = ""; });
            InterimAssert.Throws<InvalidOperationException>(delegate { copy.ParameterName = ""; });
        }

        [Test]
        public void CreateByNameAndResolve()
        {
            CodeReference r = new CodeReference(assembly.FullName, type.Namespace, type.FullName, member.Name, parameter.Name);
            Assert.AreEqual(assembly.FullName, r.AssemblyName);
            Assert.AreEqual(type.Namespace, r.NamespaceName);
            Assert.AreEqual(type.FullName, r.TypeName);
            Assert.AreEqual(member.Name, r.MemberName);
            Assert.AreEqual(parameter.Name, r.ParameterName);

            Assert.AreEqual(CodeReferenceKind.Parameter, r.Kind);
            Assert.AreEqual(assembly, r.ResolveAssembly());
            Assert.AreEqual(type, r.ResolveType());
            Assert.AreEqual(member, r.ResolveMember());
            Assert.AreEqual(parameter, r.ResolveParameter());
        }

        [Test]
        public void CreateFromAssembly()
        {
            CodeReference r = CodeReference.CreateFromAssembly(assembly);
            Assert.AreEqual(assembly.FullName, r.AssemblyName);
            Assert.IsNull(r.NamespaceName);
            Assert.IsNull(r.TypeName);
            Assert.IsNull(r.MemberName);
            Assert.IsNull(r.ParameterName);

            Assert.AreEqual(CodeReferenceKind.Assembly, r.Kind);
            Assert.AreEqual(assembly, r.ResolveAssembly());
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void CreateFromAssembly_ThrowsIfNull()
        {
            CodeReference.CreateFromAssembly(null);
        }

        [Test]
        public void CreateFromType()
        {
            CodeReference r = CodeReference.CreateFromType(type);
            Assert.AreEqual(assembly.FullName, r.AssemblyName);
            Assert.AreEqual(@namespace, r.NamespaceName);
            Assert.AreEqual(type.FullName, r.TypeName);
            Assert.IsNull(r.MemberName);
            Assert.IsNull(r.ParameterName);

            Assert.AreEqual(CodeReferenceKind.Type, r.Kind);
            Assert.AreEqual(assembly, r.ResolveAssembly());
            Assert.AreEqual(type, r.ResolveType());
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void CreateFromType_ThrowsIfNull()
        {
            CodeReference.CreateFromType(null);
        }

        [Test]
        public void CreateFromMember()
        {
            CodeReference r = CodeReference.CreateFromMember(member);
            Assert.AreEqual(assembly.FullName, r.AssemblyName);
            Assert.AreEqual(@namespace, r.NamespaceName);
            Assert.AreEqual(type.FullName, r.TypeName);
            Assert.AreEqual(member.Name, r.MemberName);
            Assert.IsNull(r.ParameterName);

            Assert.AreEqual(CodeReferenceKind.Member, r.Kind);
            Assert.AreEqual(assembly, r.ResolveAssembly());
            Assert.AreEqual(type, r.ResolveType());
            Assert.AreEqual(member, r.ResolveMember());
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void CreateFromMember_ThrowsIfNull()
        {
            CodeReference.CreateFromMember(null);
        }

        [Test]
        public void CreateFromParameter()
        {
            CodeReference r = CodeReference.CreateFromParameter(parameter);
            Assert.AreEqual(assembly.FullName, r.AssemblyName);
            Assert.AreEqual(@namespace, r.NamespaceName);
            Assert.AreEqual(type.FullName, r.TypeName);
            Assert.AreEqual(member.Name, r.MemberName);
            Assert.AreEqual(parameter.Name, r.ParameterName);

            Assert.AreEqual(CodeReferenceKind.Parameter, r.Kind);
            Assert.AreEqual(assembly, r.ResolveAssembly());
            Assert.AreEqual(type, r.ResolveType());
            Assert.AreEqual(member, r.ResolveMember());
            Assert.AreEqual(parameter, r.ResolveParameter());
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void CreateFromParameter_ThrowsIfNull()
        {
            CodeReference.CreateFromParameter(null);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void CreateFromStackFrame_ThrowsIfNull()
        {
            CodeReference.CreateFromStackFrame(null);
        }

        [Test]
        public void CreateFromStackFrame()
        {
            CodeReference r = CodeReference.CreateFromStackFrame(new StackTrace(0).GetFrame(0));
            Assert.AreEqual("CreateFromStackFrame", r.MemberName);
        }

        [Test]
        public void CreateFromStackFrame_WithFrameCount()
        {
            CodeReference r = CreateFromStackFrame_WithFrameCount_Helper();
            Assert.AreEqual("CreateFromStackFrame_WithFrameCount", r.MemberName);
        }

        [NonInlined(SecurityAction.Demand)]
        private CodeReference CreateFromStackFrame_WithFrameCount_Helper()
        {
            return CodeReference.CreateFromStackFrame(1);
        }

        [Test]
        public void CreateFromCallingMethod()
        {
            CodeReference r = CreateFromCallingMethod_Helper();
            Assert.AreEqual("CreateFromCallingMethod", r.MemberName);
        }

        [NonInlined(SecurityAction.Demand)]
        private CodeReference CreateFromCallingMethod_Helper()
        {
            return CodeReference.CreateFromCallingMethod();
        }

        [Test]
        public void CreateFromExecutingMethod()
        {
            CodeReference r = CodeReference.CreateFromExecutingMethod();
            Assert.AreEqual("CreateFromExecutingMethod", r.MemberName);
        }

        [Test]
        public void Equals_SeemsSane()
        {
            Assert.IsFalse(CodeReference.Unknown.Equals(null));
            Assert.IsFalse(CodeReference.Unknown.Equals(CodeReference.CreateFromParameter(parameter)));
            Assert.IsTrue(CodeReference.CreateFromParameter(parameter).Equals(CodeReference.CreateFromParameter(parameter)));
        }

        [Test]
        public void GetHashCode_SeemsSane()
        {
            Assert.AreNotEqual(CodeReference.CreateFromParameter(parameter).GetHashCode(),
                CodeReference.Unknown.GetHashCode());
        }

        internal void Dummy(object dummy)
        {
        }
    }
}
