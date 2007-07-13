// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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

using System;
using System.IO;
using System.Reflection;
using System.Collections;
using TestDriven.UnitTesting.Exceptions;

using MbUnit.Framework;

using System.CodeDom.Compiler;

namespace MbUnit.Framework.Tests
{
    [TestFixture]
    public class CompilerAssert_Test
    {
        protected Stream GetCSharpSample()
        {
                   
            Stream stream = File.Open("../../CsSample.cs", FileMode.Open);
            Assert.IsNotNull(stream);
            return stream;
        }

        #region CSharpCompiler, VBCompiler
        [Test]
        public void CSharpCompilerNotNull()
        {
            Assert.IsNotNull(CompilerAssert.CSharpCompiler);
        }
        [Test]
        public void VBCompilerNotNull()
        {
            Assert.IsNotNull(CompilerAssert.VBCompiler);
        }
        #endregion

        #region Null arguments
        //[Test]
        //[ExpectedException(typeof(AssertionException))]
        //public void CompilerNull()
        //{
        //    Stream stream = GetCSharpSample();
        //    CompilerAssert.Compiles(null, stream);
        //}

        //[Test]
        //[ExpectedException(typeof(AssertionException))]
        //public void StreamNull()
        //{
        //    Stream stream = null;
        //    CompilerAssert.Compiles(CompilerAssert.CSharpCompiler, stream);
        //}
        #endregion

        #region Compiles
        [Test]
        public void CSharpStreamCompiles()
        {
            Stream stream = GetCSharpSample();
            CompilerAssert.Compiles(CompilerAssert.CSharpCompiler, stream);
        }

        [Test]
        public void CSharpStringCompiles()
        {
            Stream stream = GetCSharpSample();
            using (StreamReader sr = new StreamReader(stream))
            {
                CompilerAssert.Compiles(CompilerAssert.CSharpCompiler, sr.ReadToEnd());
            }
        }

        //[Test]
        //[ExpectedException(typeof(AssertionException))]
        //public void CSharpStringCompilesFail()
        //{
        //    Stream stream = GetCSharpSample();
        //    using (StreamReader sr = new StreamReader(stream))
        //    {
        //        CompilerAssert.Compiles(CompilerAssert.CSharpCompiler, sr.ReadToEnd() + "Make me fail...");
        //    }
        //}

        #endregion

        #region Compiles
        //[Test]
        //[ExpectedException(typeof(AssertionException))]
        //public void CSharpStreamNotCompilesFail()
        //{
        //    Stream stream = GetCSharpSample();
        //    CompilerAssert.NotCompiles(CompilerAssert.CSharpCompiler, stream);
        //}
        //[Test]
        //[ExpectedException(typeof(AssertionException))]
        //public void CSharpStringNotCompilesFail()
        //{
        //    Stream stream = GetCSharpSample();
        //    using (StreamReader sr = new StreamReader(stream))
        //    {
        //        CompilerAssert.NotCompiles(CompilerAssert.CSharpCompiler, sr.ReadToEnd());
        //    }
        //}
        [Test]
        public void CSharpStringNotCompiles()
        {
            Stream stream = GetCSharpSample();
            using (StreamReader sr = new StreamReader(stream))
            {
                CompilerAssert.NotCompiles(CompilerAssert.CSharpCompiler, sr.ReadToEnd() + "Make me fail...");
            }
        }
        #endregion
    }
}
