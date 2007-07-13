extern alias MbUnit2;
using MbUnit.Framework.Core;
using MbUnit.Framework.Core.Model;
using MbUnit.Framework.Model;
using MbUnit.Framework.Model.Metadata;
using MbUnit.Framework.Services.Runtime;
using MbUnit.Framework.Utilities;
using MbUnit2::MbUnit.Framework;

using System;
using System.Reflection;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(MbUnitTestFramework))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class MbUnitTestFrameworkTest : BaseUnitTest
    {
        private MbUnitTestFramework framework;
        private TestTemplateTreeBuilder builder;
        private IAssemblyResolverManager resolverManager;

        public override void SetUp()
        {
            base.SetUp();

            resolverManager = Mocks.CreateMock<IAssemblyResolverManager>();
            framework = new MbUnitTestFramework(resolverManager);
        }

        [Test]
        public void NameIsMbUnitGallio()
        {
            Assert.AreEqual("MbUnit Gallio", framework.Name);
        }

        [Test]
        public void PopulateTree_WhenAssemblyDoesNotReferenceMbUnitGallio_IsEmpty()
        {
            PopulateTree(typeof(Int32).Assembly);

            Assert.AreEqual(0, builder.Root.ChildrenList.Count);
        }

        /// <summary>
        /// This is really just a quick sanity check to be sure that the framework
        /// seems to produce a sensible test tree.  More detailed checks on how particular
        /// attributes are handled belong elsewhere.
        /// </summary>
        [Test]
        public void PopulateTree_WhenAssemblyReferencesMbUnitGallio_ContainsSimpleTest()
        {
            Type fixtureType = typeof(MbUnit.TestResources.Gallio.SimpleTest);
            Assembly assembly = fixtureType.Assembly;
            Version expectedVersion = typeof(MbUnitTestFramework).Assembly.GetName().Version;

            PopulateTree(assembly);
            Assert.IsNull(builder.Root.Parent);
            Assert.AreEqual(TemplateKind.Root, builder.Root.Kind);
            Assert.AreEqual(CodeReference.Unknown, builder.Root.CodeReference);
            Assert.AreEqual(1, builder.Root.ChildrenList.Count);

            MbUnitTestFrameworkTemplate frameworkTemplate = (MbUnitTestFrameworkTemplate) builder.Root.ChildrenList[0];
            Assert.AreSame(builder.Root, frameworkTemplate.Parent);
            Assert.AreEqual(TemplateKind.Framework, frameworkTemplate.Kind);
            Assert.AreEqual(CodeReference.Unknown, frameworkTemplate.CodeReference);
            Assert.AreEqual(expectedVersion, frameworkTemplate.Version);
            Assert.AreEqual("MbUnit Gallio v" + expectedVersion, frameworkTemplate.Name);
            Assert.AreEqual(1, frameworkTemplate.AssemblyTemplates.Count);

            MbUnitTestAssemblyTemplate assemblyTemplate = frameworkTemplate.AssemblyTemplates[0];
            Assert.AreSame(frameworkTemplate, assemblyTemplate.Parent);
            Assert.AreEqual(TemplateKind.Assembly, assemblyTemplate.Kind);
            Assert.AreEqual(CodeReference.CreateFromAssembly(assembly), assemblyTemplate.CodeReference);
            Assert.AreEqual(assembly, assemblyTemplate.Assembly);
            Assert.GreaterEqualThan(1, assemblyTemplate.FixtureTemplates.Count);

            MbUnitTestFixtureTemplate fixtureTemplate = ListUtils.Find(assemblyTemplate.FixtureTemplates, delegate(MbUnitTestFixtureTemplate template)
            {
                return template.FixtureType == fixtureType;
            });
            Assert.IsNotNull(fixtureTemplate, "Could not find the SimpleTest fixture.");
            Assert.AreSame(assemblyTemplate, fixtureTemplate.Parent);
            Assert.AreEqual(TemplateKind.Fixture, fixtureTemplate.Kind);
            Assert.AreEqual(CodeReference.CreateFromType(fixtureType), fixtureTemplate.CodeReference);
            Assert.AreEqual(fixtureType, fixtureTemplate.FixtureType);
            Assert.AreEqual("SimpleTest", fixtureTemplate.Name);
            Assert.AreEqual(2, fixtureTemplate.MethodTemplates.Count);

            MbUnitTestMethodTemplate passTemplate = ListUtils.Find(fixtureTemplate.MethodTemplates, delegate(MbUnitTestMethodTemplate template)
            {
                return template.Name == "Pass";
            });
            Assert.IsNotNull(passTemplate, "Could not find the Pass test.");
            Assert.AreSame(fixtureTemplate, passTemplate.Parent);
            Assert.AreEqual(TemplateKind.Test, passTemplate.Kind);
            Assert.AreEqual(CodeReference.CreateFromMember(fixtureType.GetMethod("Pass")), passTemplate.CodeReference);
            Assert.AreEqual(fixtureType.GetMethod("Pass"), passTemplate.Method);
            Assert.AreEqual("Pass", passTemplate.Name);

            MbUnitTestMethodTemplate failTemplate = ListUtils.Find(fixtureTemplate.MethodTemplates, delegate(MbUnitTestMethodTemplate template)
            {
                return template.Name == "Fail";
            });
            Assert.IsNotNull(failTemplate, "Could not find the Fail test.");
            Assert.AreSame(fixtureTemplate, failTemplate.Parent);
            Assert.AreEqual(TemplateKind.Test, failTemplate.Kind);
            Assert.AreEqual(CodeReference.CreateFromMember(fixtureType.GetMethod("Fail")), failTemplate.CodeReference);
            Assert.AreEqual(fixtureType.GetMethod("Fail"), failTemplate.Method);
            Assert.AreEqual("Fail", failTemplate.Name);
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
