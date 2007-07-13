extern alias MbUnit2;
using System;
using System.Reflection;
using MbUnit.Core.Serialization;
using MbUnit.Framework.Model;
using MbUnit2::MbUnit.Framework;

namespace MbUnit.Core.Tests.Serialization
{
    [TestFixture]
    [TestsOn(typeof(CodeReferenceInfo))]
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
        public void Constructor_FromModel_WithUnknown()
        {
            CodeReferenceInfo info = new CodeReferenceInfo(CodeReference.Unknown);
            Assert.IsNull(info.AssemblyName);
            Assert.IsNull(info.NamespaceName);
            Assert.IsNull(info.TypeName);
            Assert.IsNull(info.MemberName);
            Assert.IsNull(info.ParameterName);
        }

        [Test]
        public void Constructor_FromModel_WithPopulatedInstance()
        {
            CodeReferenceInfo info = new CodeReferenceInfo(CodeReference.CreateFromParameter(parameter));
            Assert.AreEqual(assembly.FullName, info.AssemblyName);
            Assert.AreEqual(@namespace, info.NamespaceName);
            Assert.AreEqual(type.FullName, info.TypeName);
            Assert.AreEqual(member.Name, info.MemberName);
            Assert.AreEqual(parameter.Name, info.ParameterName);
        }

        internal void Dummy(object dummy)
        {
        }
    }
}
