using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Model
{
    /// <summary>
    /// Describes the function of a code element in a test.
    /// </summary>
    public class TestPart
    {
        /// <summary>
        /// Gets whether the part represents a test case or test container.
        /// </summary>
        public bool IsTest
        {
            get { return IsTestCase || IsTestContainer; }
        }

        /// <summary>
        /// Gets or sets whether the part represents a test case.
        /// </summary>
        public bool IsTestCase { get; set; }

        /// <summary>
        /// Gets or sets whether the part represents a test container.
        /// </summary>
        public bool IsTestContainer { get; set; }

        /// <summary>
        /// Gets or sets whether the part represents some contribution to a test
        /// such as a setup or teardown method, a test parameter, or a factory.
        /// </summary>
        public bool IsTestContribution { get; set; }
    }
}
