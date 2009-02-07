// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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


using Gallio.Reflection;
using Gallio.Reflection.Impl;
using MbUnit.Framework;

namespace Gallio.Tests.Reflection.Impl
{
    [TestFixture]
    [TestsOn(typeof(CecilReflectionPolicy))]
    public class CecilReflectionPolicyTest : BaseReflectionPolicyTest
    {
        private CecilReflectionPolicy policy;

        public override void SetUp()
        {
            base.SetUp();
            WrapperAssert.SupportsSpecialFeatures = false;

            // FIXME: There is a Cecil bug dealing with recursively nested generic types.
            // It returns the incorrect generic type parameters.  In the when getting the
            // generic arguments of the base type of IndirectlyRecursiveNestedType we get
            // a generic parameter declared on the wrong type:
            // Expected: "MbUnit.TestResources.Reflection.ReflectionPolicySample+TortureTest`1+NestedType+MiddleType+IndirectlyRecursiveNestedType[T]"
            // Actual  : "MbUnit.TestResources.Reflection.ReflectionPolicySample+TortureTest`1+NestedType+DirectlyRecursiveNestedType[T]"
            //
            // Cecil bug: https://bugzilla.novell.com/show_bug.cgi?id=473186
            WrapperAssert.WorkaroundStrongTypeEquivalenceProblems = true;

            policy = new CecilReflectionPolicy();
        }

        protected override IReflectionPolicy ReflectionPolicy
        {
            get { return policy; }
        }
    }
}
