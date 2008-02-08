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
            Assembly assembly = GetType().Assembly;
            loader = new MetadataLoader(new string[] { Path.GetDirectoryName(Loader.GetAssemblyLocalPath(assembly)) });
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
