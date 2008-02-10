// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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

using System.IO;
using Castle.Core.Logging;
using Gallio.Hosting;
using Gallio.Model;
using Gallio.Reflection;
using Gallio.ReSharperRunner.Reflection;
using Gallio.Tests.Reflection;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Impl;
using JetBrains.ReSharper.Psi;
using JetBrains.Util;
using MbUnit2::MbUnit.Framework;
using System.Reflection;

namespace Gallio.ReSharperRunner.Tests.Reflection
{
    [TestFixture]
    [TestsOn(typeof(PsiReflector))]
    public class PsiReflectorTest : BaseReflectionPolicyTest
    {
        private SolutionImpl solution;
        private PsiReflector reflector;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            ReSharperTestHarness.LoadTestSolutionIfNeeded();
        }

        [SetUp]
        public void SetUp()
        {
            PsiManager manager = PsiManager.GetInstance(SolutionManager.Instance.CurrentSolution);

            reflector = new PsiReflector(BuiltInAssemblyResolver.Instance, manager);
        }

        [TearDown]
        public void TearDown()
        {
            if (solution != null)
            {
                solution.Dispose();
                solution = null;
            }
        }

        protected override IReflectionPolicy ReflectionPolicy
        {
            get { return reflector; }
        }

        [Test]
        public void Wrap_Null_ReturnsNull()
        {
            Assert.IsNull(reflector.Wrap((IAssembly) null));
            Assert.IsNull(reflector.Wrap((IAttributeInstance)null));
            Assert.IsNull(reflector.Wrap((IConstructor)null));
            Assert.IsNull(reflector.Wrap((IDeclaredElement)null));
            Assert.IsNull(reflector.Wrap((IEvent)null));
            Assert.IsNull(reflector.Wrap((IField)null));
            Assert.IsNull(reflector.Wrap((IFunction)null));
            Assert.IsNull(reflector.Wrap((IMethod)null));
            Assert.IsNull(reflector.Wrap((IModule)null));
            Assert.IsNull(reflector.Wrap((IOperator)null));
            Assert.IsNull(reflector.Wrap((IParameter)null));
            Assert.IsNull(reflector.Wrap((IProject)null));
            Assert.IsNull(reflector.Wrap((IProperty)null));
            Assert.IsNull(reflector.Wrap((IType)null));
            Assert.IsNull(reflector.Wrap((ITypeElement)null));
            Assert.IsNull(reflector.Wrap((ITypeParameter)null));

            Assert.IsNull(reflector.WrapNamespace(null));
        }

        [Test("Other tests exercise Psi project modules, this one checks Psi assembly modules.")]
        public void AssemblyWrapperForPsiAssemblyModules()
        {
            Assembly target = typeof(ILogger).Assembly;
            IAssemblyInfo info = GetAssembly(target);

            WrapperAssert.AreEquivalent(target, info);
        }

        [Test("Other tests exercise Psi project modules, this one checks Psi assembly modules.")]
        public void AssemblyWrapperForPsiAssemblyModules_EqualityAndHashcode()
        {
            VerifyEqualityAndHashcodeContracts<Assembly, IAssemblyInfo>(
                typeof(ILogger).Assembly,
                typeof(ITest).Assembly,
                GetAssembly);
        }
    }
}