using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Castle.Core.Logging;
using Gallio.Reflection;
using Gallio.Reflection.Impl;
using MbUnit.Framework;

namespace Gallio.Tests.Reflection.Impl
{
    [TestFixture]
    [TestsOn(typeof(ComDebugSymbolResolver))]
    public class ComDebugSymbolResolverTest
    {
        [Test, ExpectedArgumentNullException]
        public void GetSourceDocumentRangeForMethod_ThrowsIfAssemblyFileIsNull()
        {
            new ComDebugSymbolResolver().GetSourceLocationForMethod(null, 0);
        }

        [Test]
        public void GetSourceDocumentRangeForMethod_ReturnsValidRangeForConcreteMethod()
        {
            SourceLocation sourceLocation = GetSourceDocumentRangeForMethod("ConcreteMethod");

            StringAssert.EndsWith(sourceLocation.Filename, GetType().Name + ".cs");
            Assert.AreEqual(1000, sourceLocation.Line);
            Assert.GreaterEqualThan(sourceLocation.Column, 0);
        }

        [Test]
        public void GetSourceDocumentRangeForMethod_ReturnsNullIfMethodIsAbstract()
        {
            SourceLocation sourceLocation = GetSourceDocumentRangeForMethod("AbstractMethod");
            Assert.IsNull(sourceLocation);
        }

        [Test]
        public void GetSourceDocumentRangeForMethod_ThrowsIfAssemblyFileDoesNotExist()
        {
            new ComDebugSymbolResolver().GetSourceLocationForMethod("NoSuchAssembly", 0);
        }

        [Test]
        public void GetSourceDocumentRangeForMethod_ThrowsIfAssemblyExistsButThereIsNoPDB()
        {
            new ComDebugSymbolResolver().GetSourceLocationForMethod(typeof(ILogger).Assembly.Location, 0);
        }

        [Test]
        public void GetSourceDocumentRangeForMethod_ThrowsIfMethodTokenNotValid()
        {
            new ComDebugSymbolResolver().GetSourceLocationForMethod(GetType().Assembly.Location, 0);
        }

        private SourceLocation GetSourceDocumentRangeForMethod(string methodName)
        {
            ComDebugSymbolResolver resolver = new ComDebugSymbolResolver();

            MethodInfo method = typeof(Sample).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            return resolver.GetSourceLocationForMethod(method.DeclaringType.Assembly.Location, method.MetadataToken);
        }

        private abstract class Sample
        {
            private void ConcreteMethod()
#line 1000
            {
                ConcreteMethod();
            }

            protected abstract void AbstractMethod();
        }
    }
}
