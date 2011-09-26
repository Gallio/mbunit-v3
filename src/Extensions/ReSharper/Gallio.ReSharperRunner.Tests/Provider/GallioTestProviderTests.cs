using System;
using System.Collections.Generic;
using Gallio.ReSharperRunner.Provider;
using Gallio.ReSharperRunner.Reflection;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;
using MbUnit.Framework;

namespace Gallio.ReSharperRunner.Tests.Provider
{
	public class GallioTestProviderTests
	{
		[Test]
		public void Test_name()
		{
			var provider = new GallioTestProvider();
			const string assemblyPath = "assemblyPath";
			var element = new TestUnitTestElement(null, null, "", "", "", false, null, null, assemblyPath, "", "");
			var explicitElements = new List<UnitTestElement>();

			var taskSequence = provider.GetTaskSequence(element, explicitElements);

			var unitTestTask = taskSequence[0];
			Assert.IsInstanceOfType(typeof(AssemblyLoadTask), unitTestTask.RemoteTask);
			Assert.AreEqual(assemblyPath, ((AssemblyLoadTask) unitTestTask.RemoteTask).AssemblyCodeBase);
		}

		private class TestUnitTestElement : GallioTestElement
		{
			public TestUnitTestElement(IUnitTestProvider provider, GallioTestElement parent, string testId, string testName, 
				string kind, bool isTestCase, IProject project, IDeclaredElementResolver declaredElementResolver, string assemblyPath, 
				string typeName, string namespaceName) 
				: base(provider, parent, testId, testName, kind, isTestCase, project, declaredElementResolver, assemblyPath, 
				typeName, namespaceName)
			{
			}

			public override IProject GetProject()
			{
				throw new NotImplementedException();
			}

			public override string GetTitle()
			{
				throw new NotImplementedException();
			}

			public override string GetTypeClrName()
			{
				throw new NotImplementedException();
			}

			public override UnitTestNamespace GetNamespace()
			{
				throw new NotImplementedException();
			}

			public override IList<IProjectFile> GetProjectFiles()
			{
				throw new NotImplementedException();
			}

			public override UnitTestElementDisposition GetDisposition()
			{
				throw new NotImplementedException();
			}

			public override IDeclaredElement GetDeclaredElement()
			{
				throw new NotImplementedException();
			}

			public override string GetKind()
			{
				throw new NotImplementedException();
			}

			public override string ShortName
			{
				get { throw new NotImplementedException(); }
			}
		}
	}
}
