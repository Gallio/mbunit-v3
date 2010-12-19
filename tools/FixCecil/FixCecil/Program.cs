using System;
using Mono.Cecil;

namespace FixCecil
{
	class Program
	{
		static void Main(string[] args)
		{
			const string fileName = @"Mono.Cecil.Renamed.dll";

			var assembly = AssemblyDefinition.ReadAssembly(fileName);

			var isDirty = false;
			foreach (var type in assembly.MainModule.Types)
			{
				if (type.Name != "ExtensionAttribute")
					continue;

				Console.WriteLine("Found ExtensionAttribute");
				type.Name = "ExtensionAttribute2";
				isDirty = true;
				Console.WriteLine("Renamed ExtensionAttribute");
			}

			if (isDirty)
				assembly.Write(fileName);
		}
	}
}
