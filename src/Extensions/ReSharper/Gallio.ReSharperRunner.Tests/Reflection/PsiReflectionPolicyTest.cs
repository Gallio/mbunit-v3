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

using Castle.Core.Logging;
using Gallio.Model;
using Gallio.Reflection;
using Gallio.ReSharperRunner.Reflection;
using Gallio.Tests.Reflection;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Impl;
using JetBrains.ReSharper.Psi;
using MbUnit2::MbUnit.Framework;
using System.Reflection;

namespace Gallio.ReSharperRunner.Tests.Reflection
{
    [TestFixture]
    [TestsOn(typeof(PsiReflectionPolicy))]
    public class PsiReflectionPolicyTest : BaseReflectionPolicyTest
    {
        private SolutionImpl solution;
        private PsiReflectionPolicy reflectionPolicy;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            ReSharperTestHarness.LoadTestSolutionIfNeeded();
        }

        public override void SetUp()
        {
            base.SetUp();
            WrapperAssert.SupportsSpecialFeatures = false;
            WrapperAssert.SupportsSpecialName = false;
            WrapperAssert.SupportsCallingConventions = false;
            WrapperAssert.SupportsReturnAttributes = false;
            WrapperAssert.SupportsEventFields = false;
            WrapperAssert.SupportsGenericParameterAttributes = false;
            WrapperAssert.SupportsFinalizers = false;

            PsiManager manager = PsiManager.GetInstance(SolutionManager.Instance.CurrentSolution);

            reflectionPolicy = new PsiReflectionPolicy(manager);
        }

        public override void TearDown()
        {
            if (solution != null)
            {
                solution.Dispose();
                solution = null;
            }

            base.TearDown();
        }

        protected override IReflectionPolicy ReflectionPolicy
        {
            get { return reflectionPolicy; }
        }

        [Test]
        public void WrapNullReturnsNull()
        {
            Assert.IsNull(reflectionPolicy.Wrap((IDeclaredElement)null));
            Assert.IsNull(reflectionPolicy.Wrap((IEvent)null));
            Assert.IsNull(reflectionPolicy.Wrap((IField)null));
            Assert.IsNull(reflectionPolicy.Wrap((IFunction)null));
            Assert.IsNull(reflectionPolicy.Wrap((IConstructor)null));
            Assert.IsNull(reflectionPolicy.Wrap((IMethod)null));
            Assert.IsNull(reflectionPolicy.Wrap((IParameter)null));
            Assert.IsNull(reflectionPolicy.Wrap((IProperty)null));
            Assert.IsNull(reflectionPolicy.Wrap((ITypeElement)null));
            Assert.IsNull(reflectionPolicy.Wrap((ITypeParameter)null));
        }

        [Test("Other tests exercise Psi project modules, this one checks Psi assembly modules.")]
        public void AssemblyWrapperForPsiAssemblyModules()
        {
            Assembly target = typeof(ILogger).Assembly;
            IAssemblyInfo info = GetAssembly(target);

            WrapperAssert.AreEquivalent(target, info, false);
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