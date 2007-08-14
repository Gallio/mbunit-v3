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
using MbUnit2::MbUnit.Framework;
using Assert = MbUnit2::MbUnit.Framework.Assert;

using System;
using System.Reflection;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit._Framework.Tests.Kernel.Model
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
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void CreateFromParameter_ThrowsIfNull()
        {
            CodeReference.CreateFromParameter(null);
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
