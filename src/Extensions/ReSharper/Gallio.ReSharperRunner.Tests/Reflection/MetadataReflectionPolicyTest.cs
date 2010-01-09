// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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


using System.Reflection;
using Gallio.ReSharperRunner.Reflection;
using Gallio.Tests.Common.Reflection;
using JetBrains.Metadata.Reader.API;
using MbUnit.Framework;
#if RESHARPER_50_OR_NEWER
using JetBrains.Metadata.Utils;
#endif

namespace Gallio.ReSharperRunner.Tests.Reflection
{
    [TestFixture]
    [TestsOn(typeof(MetadataReflectionPolicy))]
#if !RESHARPER_31
    [RunWithGuardedReadLock]
#endif
    [Category("Integration")]
    public class MetadataReflectionPolicyTest : BaseReflectionPolicyTest
    {
        private MetadataLoader loader;
        private MetadataReflectionPolicy reflectionPolicy;

        public override void SetUp()
        {
            base.SetUp();
            WrapperAssert.SupportsSpecialFeatures = false;
#if RESHARPER_31
            WrapperAssert.SupportsNestedGenericTypes = false;
#endif

            loader = new MetadataLoader(BuiltInMetadataAssemblyResolver.Instance);

            Assembly assembly = GetType().Assembly;
#if RESHARPER_50_OR_NEWER
            IMetadataAssembly metadataAssembly = loader.Load(new AssemblyNameInfo(assembly.GetName().FullName), delegate { return true; });
#else
            IMetadataAssembly metadataAssembly = loader.Load(assembly.GetName(), delegate { return true; });
#endif

            reflectionPolicy = new MetadataReflectionPolicy(metadataAssembly, null);
        }

        public override void TearDown()
        {
            if (loader != null)
            {
                loader.Dispose();
                loader = null;
            }

            base.TearDown();
        }

        protected override Common.Reflection.IReflectionPolicy ReflectionPolicy
        {
            get { return reflectionPolicy; }
        }

        [Test]
        public void WrapNullReturnsNull()
        {
            Assert.IsNull(reflectionPolicy.Wrap((IMetadataAssembly)null));
        }
    }
}
