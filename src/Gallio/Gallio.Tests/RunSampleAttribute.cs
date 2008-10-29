using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework.Pattern;
using MbUnit.Framework;

namespace Gallio.Tests
{
    /// <summary>
    /// Used together with <see cref="BaseTestWithSampleRunner" /> to specify
    /// a sample type to run.
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.TestType, AllowMultiple = true, Inherited = true)]
    public class RunSampleAttribute : Attribute
    {
        public RunSampleAttribute(Type sampleType)
        {
            if (sampleType == null)
                throw new ArgumentNullException("sampleType");

            SampleType = sampleType;
        }

        public Type SampleType { get; private set; }
    }
}
