extern alias MbUnit2;
using System;
using System.Reflection;
using MbUnit.Framework.Model;
using MbUnit2::MbUnit.Framework;

namespace MbUnit.Tests.Core.Model
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
            Assert.IsNull(CodeReference.Unknown.Assembly);
            Assert.IsNull(CodeReference.Unknown.Namespace);
            Assert.IsNull(CodeReference.Unknown.Type);
            Assert.IsNull(CodeReference.Unknown.Member);
            Assert.IsNull(CodeReference.Unknown.Parameter);
        }

        [Test]
        public void CreateFromAssembly()
        {
            CodeReference r = CodeReference.CreateFromAssembly(assembly);
            Assert.AreEqual(assembly, r.Assembly);
            Assert.IsNull(r.Namespace);
            Assert.IsNull(r.Type);
            Assert.IsNull(r.Member);
            Assert.IsNull(r.Parameter);
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
            Assert.AreEqual(assembly, r.Assembly);
            Assert.AreEqual(@namespace, r.Namespace);
            Assert.AreEqual(type, r.Type);
            Assert.IsNull(r.Member);
            Assert.IsNull(r.Parameter);
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
            Assert.AreEqual(assembly, r.Assembly);
            Assert.AreEqual(@namespace, r.Namespace);
            Assert.AreEqual(type, r.Type);
            Assert.AreEqual(member, r.Member);
            Assert.IsNull(r.Parameter);
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
            Assert.AreEqual(assembly, r.Assembly);
            Assert.AreEqual(@namespace, r.Namespace);
            Assert.AreEqual(type, r.Type);
            Assert.AreEqual(member, r.Member);
            Assert.AreEqual(parameter, r.Parameter);
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
