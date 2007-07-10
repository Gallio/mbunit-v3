using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using TestDriven.UnitTesting.Exceptions;

namespace MbUnit.Framework
{
    public sealed class FileAssert
    {

        private FileAssert()
        { }

        public static void AreEqual(string expectedPath, string actualPath)
        {
            FileInfo expected = new FileInfo(expectedPath);
            AreEqual(expected, actualPath);
        }

        public static void AreEqual(FileInfo expected, string filePath)
        {
            FileInfo actual = new FileInfo(filePath);
            AreEqual(expected, actual);
        }

        public static void AreEqual(FileInfo expected, FileInfo actual)
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);

            Assert.AreEqual(expected.Length, actual.Length, "File length is not the same");
            using (Stream expectedStream = expected.OpenRead())
            {
                using (Stream actualStream = actual.OpenRead())
                {
                    AreStreamContentEqual(expectedStream, actualStream);
                }
            }
        }

        public static void AreStreamContentEqual(Stream expected, Stream actual)
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(actual);

            Assert.IsTrue(expected.CanRead);
            Assert.IsTrue(actual.CanRead);

            using (StreamReader esr = new StreamReader(expected))
            {
                using (StreamReader asr = new StreamReader(actual))
                {
                    string eline;
                    string aline;
                    do
                    {
                        eline = esr.ReadLine();
                        aline = asr.ReadLine();
                        Assert.AreEqual(eline, aline);
                    } while (eline != null);
                }
            }
        }

        public static void Exists(string fileName)
        {
            Assert.IsNotNull(fileName, "FileName is null");
            Assert.IsTrue(File.Exists(fileName), "file {0} does not exist", fileName);
        }

        public static void NotExists(string fileName)
        {
            Assert.IsNotNull(fileName, "FileName is null");
            Assert.IsFalse(File.Exists(fileName), "file {0} exists", fileName);
        }
    }
}
