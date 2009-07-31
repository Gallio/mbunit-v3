using System;
using Gallio.Framework.Pattern;

namespace Gallio.Tests
{
    /// <summary>
    /// Used together with <see cref="BaseTestWithSampleRunner" /> to specify
    /// a sample file to run.
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.TestType, AllowMultiple = true, Inherited = true)]
    public class RunSampleFileAttribute : Attribute
    {
        public RunSampleFileAttribute(string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException("filePath");

            FilePath = filePath;
        }

        public string FilePath { get; private set; }
    }
}