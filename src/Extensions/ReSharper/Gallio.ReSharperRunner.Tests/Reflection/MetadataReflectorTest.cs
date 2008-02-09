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

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Gallio.Hosting;
using Gallio.ReSharperRunner.Reflection;
using Gallio.Tests.Reflection;
using JetBrains.Metadata.Reader.API;
using MbUnit2::MbUnit.Framework;

namespace Gallio.ReSharperRunner.Tests.Reflection
{
    [TestFixture]
    [TestsOn(typeof(MetadataReflector))]
    public class MetadataReflectorTest : BaseReflectionPolicyTest
    {
        private MetadataLoader loader;
        private MetadataReflector reflector;

        [SetUp]
        public void SetUp()
        {
            loader = new MetadataLoader(BuiltInMetadataAssemblyResolver.Instance);

            Assembly assembly = GetType().Assembly;
            IMetadataAssembly metadataAssembly = loader.Load(assembly.GetName(), delegate { return true; });

            reflector = new MetadataReflector(BuiltInAssemblyResolver.Instance, metadataAssembly, null);
        }

        [TearDown]
        public void TearDown()
        {
            if (loader != null)
            {
                loader.Dispose();
                loader = null;
            }
        }

        protected override Gallio.Reflection.IReflectionPolicy ReflectionPolicy
        {
            get { return reflector; }
        }

        [Test]
        public void Wrap_Null_ReturnsNull()
        {
            Assert.IsNull(reflector.Wrap((IMetadataAssembly)null));
            Assert.IsNull(reflector.Wrap((IMetadataCustomAttribute)null));
            Assert.IsNull(reflector.Wrap((IMetadataEvent)null));
            Assert.IsNull(reflector.Wrap((IMetadataField)null));
            Assert.IsNull(reflector.Wrap((IMetadataGenericArgument)null));
            Assert.IsNull(reflector.Wrap((IMetadataMethod)null));
            Assert.IsNull(reflector.Wrap((IMetadataParameter)null));
            Assert.IsNull(reflector.Wrap((IMetadataProperty)null));
            Assert.IsNull(reflector.Wrap((IMetadataReturnValue)null));
            Assert.IsNull(reflector.Wrap((IMetadataType)null));
            Assert.IsNull(reflector.Wrap((IMetadataCustomAttribute)null));

            Assert.IsNull(reflector.WrapConstructor(null));

            Assert.IsNull(reflector.WrapMethod(null));

            Assert.IsNull(reflector.WrapNamespace(null));

            Assert.IsNull(reflector.WrapOpenType(null));
        }
    }
}
