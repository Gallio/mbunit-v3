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
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.CodeDom.Compiler;
using TestDriven.UnitTesting.Exceptions;

namespace MbUnit.Framework
{
    public class CompilationException : AssertExceptionBase
    {
        	private string message;

        public CompilationException(
			ICodeCompiler compiler,
			CompilerParameters parameters,
            CompilerResults results,
			params String[] sources
			)
		{
			StringWriter sw = new StringWriter();
			sw.WriteLine("Compilation:  {0} errors",results.Errors.Count);
			sw.WriteLine("Compiler: {0}",compiler.GetType().Name);
			sw.WriteLine("CompilerParameters: {0}",parameters.ToString());
			foreach(CompilerError error in results.Errors)
			{
				sw.WriteLine(error.ToString());
			}
			sw.WriteLine("Sources:");
            foreach(string source in sources)
    			sw.WriteLine(source);

			this.message =sw.ToString();
		}

		public override string Message
		{
			get
			{
				return this.message;
			}
		}
    }
}
