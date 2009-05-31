using Gallio.Common.Collections;
using Gallio.Runtime.ConsoleSupport;

namespace Gallio.Copy
{
    internal class CopyArguments
    {
        [CommandLineArgument(CommandLineArgumentFlags.MultipleUnique, ShortName = "pd", 
            LongName = "plugin-directory", Description = "Additional plugin directories to search recursively", 
            ValueLabel = "dir")]
        public string[] PluginDirectories = EmptyArray<string>.Instance;

        [CommandLineArgument(CommandLineArgumentFlags.AtMostOnce, ShortName = "h", 
            LongName = "help", Description = "Display this help text.", 
            Synonyms = new[] { "?" })]
        public bool Help;
    }
}
