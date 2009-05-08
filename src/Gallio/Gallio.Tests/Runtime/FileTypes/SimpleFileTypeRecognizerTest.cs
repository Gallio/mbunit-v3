using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Runtime.FileTypes;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.FileTypes
{
    [TestsOn(typeof(SimpleFileTypeRecognizer))]
    public class SimpleFileTypeRecognizerTest
    {
        [Test]
        public void IsRecognizedFile_Always_ReturnsTrue()
        {
            var recognizer = new SimpleFileTypeRecognizer();
            var inspector = MockRepository.GenerateStub<IFileInspector>();

            Assert.IsTrue(recognizer.IsRecognizedFile(inspector));
        }
    }
}
