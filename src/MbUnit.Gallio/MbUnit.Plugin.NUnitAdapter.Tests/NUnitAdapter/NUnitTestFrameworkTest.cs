extern alias MbUnit2;
using MbUnit2::MbUnit.Framework;

using System.Reflection;
using MbUnit.Core.Metadata;
using MbUnit.Core.Model;
using MbUnit.Core.Utilities;
using MbUnit.Framework.Core.Model;
using MbUnit.Plugin.NUnitAdapter.Core;

using System;

namespace MbUnit.Plugin.NUnitAdapter.Tests.Core
{
    [TestFixture]
    [TestsOn(typeof(NUnitTestFramework))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class NUnitTestFrameworkTest
    {
        private NUnitTestFramework framework;
        private TestTemplateTreeBuilder builder;

        [SetUp]
        public void SetUp()
        {
            framework = new NUnitTestFramework();
        }

        [Test]
        public void NameIsNUnit()
        {
            Assert.AreEqual("NUnit", framework.Name);
        }

        [Test]
        public void PopulateTree_WhenAssemblyDoesNotReferenceMbUnit_IsEmpty()
        {
            PopulateTree(typeof(Int32).Assembly);

            Assert.AreEqual(0, builder.Root.ChildrenList.Count);
        }

        [Test]
        public void PopulateTree_WhenAssemblyReferencesNUnit_ContainsJustTheFrameworkTemplate()
        {
            Type fixtureType = typeof(MbUnit.Tests.Resources.NUnit.SimpleTest);
            Assembly assembly = fixtureType.Assembly;
            Version expectedVersion = typeof(NUnit.Framework.Assert).Assembly.GetName().Version;

            PopulateTree(assembly);
            Assert.IsNull(builder.Root.Parent);
            Assert.AreEqual(TemplateKind.Root, builder.Root.Kind);
            Assert.AreEqual(CodeReference.Unknown, builder.Root.CodeReference);
            Assert.AreEqual(1, builder.Root.ChildrenList.Count);

            TestTemplateGroup frameworkTemplate = (TestTemplateGroup)builder.Root.ChildrenList[0];
            Assert.AreSame(builder.Root, frameworkTemplate.Parent);
            Assert.AreEqual(TemplateKind.Framework, frameworkTemplate.Kind);
            Assert.AreEqual(CodeReference.Unknown, frameworkTemplate.CodeReference);
            Assert.AreEqual("NUnit v" + expectedVersion, frameworkTemplate.Name);
            Assert.AreEqual(1, frameworkTemplate.ChildrenList.Count);

            TestTemplateGroup assemblyTemplate = (TestTemplateGroup) frameworkTemplate.ChildrenList[0];
            Assert.AreSame(frameworkTemplate, assemblyTemplate.Parent);
            Assert.AreEqual(TemplateKind.Assembly, assemblyTemplate.Kind);
            Assert.AreEqual(CodeReference.CreateFromAssembly(assembly), assemblyTemplate.CodeReference);
            Assert.AreEqual(0, assemblyTemplate.ChildrenList.Count);
        }

        private void PopulateTree(Assembly assembly)
        {
            TestProject project = new TestProject();
            project.Assemblies.Add(assembly);

            builder = new TestTemplateTreeBuilder(project);
            framework.BuildTemplates(builder, builder.Root);
        }
    }
}
