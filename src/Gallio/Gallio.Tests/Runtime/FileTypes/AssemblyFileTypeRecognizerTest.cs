using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Gallio.Common.Reflection;
using Gallio.Runtime.FileTypes;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.FileTypes
{
    [TestsOn(typeof(AssemblyFileTypeRecognizer))]
    public class AssemblyFileTypeRecognizerTest
    {
        [Test]
        public void IsRecognizedFile_WhenStreamIsACLRAssembly_ReturnsTrue()
        {
            var recognizer = new AssemblyFileTypeRecognizer();
            var path = Assembly.GetExecutingAssembly().Location;
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var inspector = MockRepository.GenerateStub<IFileInspector>();
                Stream dummy;
                inspector.Expect(x => x.TryGetStream(out dummy)).OutRef(stream).Return(true);

                Assert.IsTrue(recognizer.IsRecognizedFile(inspector));
            }
        }

        [Test]
        public void IsRecognizedFile_WhenStreamIsAPEFileButNotAnAssembly_ReturnsFalse()
        {
            var recognizer = new AssemblyFileTypeRecognizer();
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"kernel32.dll");
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var inspector = MockRepository.GenerateStub<IFileInspector>();
                Stream dummy;
                inspector.Expect(x => x.TryGetStream(out dummy)).OutRef(stream).Return(true);

                Assert.IsFalse(recognizer.IsRecognizedFile(inspector));
            }
        }

        [Test]
        public void IsRecognizedFile_WhenStreamIsNotAvailable_ReturnsFalse()
        {
            var recognizer = new AssemblyFileTypeRecognizer();
            var inspector = MockRepository.GenerateStub<IFileInspector>();
            Stream dummy;
            inspector.Expect(x => x.TryGetStream(out dummy)).OutRef(new object[] { null }).Return(false);

            Assert.IsFalse(recognizer.IsRecognizedFile(inspector));
        }
    }
}
