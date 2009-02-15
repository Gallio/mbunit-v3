// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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

using System;
using System.IO;

#pragma warning disable 1591
#pragma warning disable 3001

namespace MbUnit.Framework
{
    public static class OldFileAssert
    {
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
