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
