using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Gallio.Runtime.Extensibility
{
    internal static class AssemblyProbingRules
    {
        public static IEnumerable<string> GetProbingPathCombinations(string baseDirectory, IList<string> probingPaths)
        {
            yield return baseDirectory;
            yield return Path.Combine(baseDirectory, "bin");

            foreach (string probingPath in probingPaths)
            {
                yield return Path.Combine(baseDirectory, probingPath);
                yield return Path.Combine(baseDirectory, Path.Combine("bin", probingPath));
            }
        }

    }
}
