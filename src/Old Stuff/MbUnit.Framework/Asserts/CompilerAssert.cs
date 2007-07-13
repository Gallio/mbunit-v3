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
using System.Collections;
using System.CodeDom.Compiler;
using System.IO;
using System.Collections.Specialized;
using TestDriven.UnitTesting.Exceptions;
using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace MbUnit.Framework
{
    public sealed class CompilerAssert
    {
        #region Private constructor and fields
        private static ICodeCompiler csharp = new CSharpCodeProvider().CreateCompiler();
        private static ICodeCompiler vb = new VBCodeProvider().CreateCompiler();
        private CompilerAssert()
        { }
        #endregion

        /// <summary>
        /// Gets the C# compiler from <see cref="CSharpCodeProvider"/>.
        /// </summary>
        /// <value>
        /// C# compiler.
        /// </value>
        public static ICodeCompiler CSharpCompiler
        {
            get
            {
                return csharp;
            }
        }

        /// <summary>
        /// Gets the VB.NET compiler from <see cref="VBCodeProvider"/>.
        /// </summary>
        /// <value>
        /// VB.NET compiler.
        /// </value>
        public static ICodeCompiler VBCompiler
        {
            get
            {
                return vb;
            }
        }

        /// <summary>
        /// Verifies that <paramref name="source"/> compiles using the provided compiler.
        /// </summary>
        /// <param name="compiler">Compiler instance</param>
        /// <param name="source">Source code to compile</param>
        public static void Compiles(ICodeCompiler compiler, string source)
        {
            Assert.IsNotNull(compiler);
            Assert.IsNotNull(source);
            CompilerParameters ps = new CompilerParameters();
            Compiles(compiler, ps, source);
        }

        /// <summary>
        /// Verifies that <paramref name="source"/> compiles using the provided compiler.
        /// </summary>
        /// <param name="compiler">Compiler instance</param>
        /// <param name="source">Source code to compile</param>
        public static void Compiles(ICodeCompiler compiler, Stream source)
        {
            Assert.IsNotNull(compiler);
            Assert.IsNotNull(source);
            CompilerParameters ps = new CompilerParameters();
            Compiles(compiler, ps, source);
        }

        /// <summary>
        /// Verifies that <paramref name="source"/> compiles using the provided compiler.
        /// </summary>
        /// <param name="compiler">Compiler instance</param>
        /// <param name="references">Referenced assemblies</param>
        /// <param name="source">Source code to compile</param>
        public static void Compiles(ICodeCompiler compiler, StringCollection references, string source)
        {
            Assert.IsNotNull(compiler);
            Assert.IsNotNull(references);
            Assert.IsNotNull(source);
            CompilerParameters ps = new CompilerParameters();
            foreach (string ra in references)
                ps.ReferencedAssemblies.Add(ra);

            Compiles(compiler, ps, source);
        }

        /// <summary>
        /// Verifies that <paramref name="source"/> compiles using the provided compiler.
        /// </summary>
        /// <param name="compiler">
        /// <see cref="ICodeCompiler"/> instance.</param>
        /// <param name="options">Compilation options</param>
        /// <param name="source">source to compile</param>
        public static void Compiles(ICodeCompiler compiler, CompilerParameters options, string source)
        {
            Assert.IsNotNull(compiler);
            Assert.IsNotNull(options);
            Assert.IsNotNull(source);
            Compiles(compiler, options, source, false);
        }

        /// <summary>
        /// Verifies that <paramref name="source"/> compiles using the provided compiler.
        /// </summary>
        /// <param name="compiler">
        /// <see cref="ICodeCompiler"/> instance.</param>
        /// <param name="options">Compilation options</param>
        /// <param name="source">Source to compile</param>
        /// <param name="throwOnWarning">
        /// true if assertion should throw if any warning.
        /// </param>
        public static void Compiles(ICodeCompiler compiler, CompilerParameters options, string source, bool throwOnWarning)
        {
            Assert.IsNotNull(compiler);
            Assert.IsNotNull(options);
            CompilerResults results = compiler.CompileAssemblyFromSource(options, source);
            if (results.Errors.HasErrors)
                throw new CompilationException(compiler, options, results, source);
            if (throwOnWarning && results.Errors.HasWarnings)
                throw new CompilationException(compiler, options, results, source);
        }

        /// <summary>
        /// Verifies that <paramref name="source"/> compiles using the provided compiler.
        /// </summary>
        /// <param name="compiler">
        /// <see cref="ICodeCompiler"/> instance.</param>
        /// <param name="options">Compilation options</param>
        /// <param name="source">Stream containing the source to compile</param>
        public static void Compiles(ICodeCompiler compiler, CompilerParameters options, Stream source)
        {
            Compiles(compiler, options, source, false);
        }

        /// <summary>
        /// Verifies that <paramref name="source"/> compiles using the provided compiler.
        /// </summary>
        /// <param name="compiler">
        /// <see cref="ICodeCompiler"/> instance.</param>
        /// <param name="options">Compilation options</param>
        /// <param name="source">Stream containing the source to compile</param>
        /// <param name="throwOnWarning">
        /// true if assertion should throw if any warning.
        /// </param>
        public static void Compiles(ICodeCompiler compiler, CompilerParameters options, Stream source, bool throwOnWarning)
        {
            using (StreamReader sr = new StreamReader(source))
            {
                Compiles(compiler, options, sr.ReadToEnd(), throwOnWarning);
            }
        }


        /// <summary>
        /// Verifies that <paramref name="source"/> does not compile using the provided compiler.
        /// </summary>
        /// <param name="compiler">
        /// <see cref="ICodeCompiler"/> instance.</param>
        /// <param name="source">Source to compile</param>
        public static void NotCompiles(
            ICodeCompiler compiler,
            string source)
        {
            CompilerParameters options = new CompilerParameters();
            NotCompiles(compiler, options, source);
        }

        /// <summary>
        /// Verifies that <paramref name="source"/> does not compile using the provided compiler.
        /// </summary>
        /// <param name="compiler">
        /// <see cref="ICodeCompiler"/> instance.</param>
        /// <param name="source">Source to compile</param>
        public static void NotCompiles(
            ICodeCompiler compiler,
            Stream source)
        {
            CompilerParameters options = new CompilerParameters();
            NotCompiles(compiler, options, source);
        }

        /// <summary>
        /// Verifies that <paramref name="source"/> does not compile using the provided compiler.
        /// </summary>
        /// <param name="compiler">
        /// <see cref="ICodeCompiler"/> instance.</param>
        /// <param name="referencedAssemblies">Collection of referenced assemblies</param>
        /// <param name="source">Source to compile</param>
        public static void NotCompiles(
            ICodeCompiler compiler,
            StringCollection referencedAssemblies,
            string source)
        {
            CompilerParameters options = new CompilerParameters();
            CompilerParameters ps = new CompilerParameters();
            foreach (string ra in referencedAssemblies)
                ps.ReferencedAssemblies.Add(ra);
            NotCompiles(compiler, options, source);
        }

        /// <summary>
        /// Verifies that <paramref name="source"/> does not compile using the provided compiler.
        /// </summary>
        /// <param name="compiler">
        /// <see cref="ICodeCompiler"/> instance.</param>
        /// <param name="options">Compilation options</param>
        /// <param name="source">Source to compile</param>
        public static void NotCompiles(
            ICodeCompiler compiler,
            CompilerParameters options,
            string source)
        {
            Assert.IncrementAssertCount();
            if (compiler == null)
                throw new ArgumentNullException("compiler");
            if (options == null)
                throw new ArgumentNullException("options");
            CompilerResults results = compiler.CompileAssemblyFromSource(options, source);
            if (!results.Errors.HasErrors)
                throw new CompilationException(compiler, options, results, source);
        }

        /// <summary>
        /// Verifies that <paramref name="source"/> does not compile using the provided compiler.
        /// </summary>
        /// <param name="compiler">
        /// <see cref="ICodeCompiler"/> instance.</param>
        /// <param name="options">Compilation options</param>
        /// <param name="source">Source to compile</param>
        public static void NotCompiles(
            ICodeCompiler compiler,
            CompilerParameters options,
            Stream source)
        {
            using (StreamReader sr = new StreamReader(source))
            {
                NotCompiles(compiler, options, sr.ReadToEnd());
            }
        }

        public static void DisplayErrors(CompilerResults results, TextWriter writer)
        {
            foreach (CompilerError error in results.Errors)
            {
                writer.Write(error);
            }
        }
    }
}
