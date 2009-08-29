// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using Gallio.Framework.Assertions;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// A diff item representing a difference between two XML fragments.
    /// </summary>
    public class Diff
    {
        private readonly string path;
        private readonly string message;
        private readonly string expected;
        private readonly string actual;

        /// <summary>
        /// Gets the path of the difference in the XML fragment.
        /// </summary>
        public string Path
        {
            get
            {
                return path;
            }
        }

        /// <summary>
        /// Gets a message explaining the difference.
        /// </summary>
        public string Message
        {
            get
            {
                return message;
            }
        }

        /// <summary>
        /// Gets the expected XML fragment.
        /// </summary>
        public string Expected
        {
            get
            {
                return expected;
            }
        }

        /// <summary>
        /// Gets the actual XML fragment.
        /// </summary>
        public string Actual
        {
            get
            {
                return actual;
            }
        }

        /// <summary>
        /// Constructs a diff item.
        /// </summary>
        /// <param name="path">The path of the difference in the XML fragment.</param>
        /// <param name="message">The message explaining the difference.</param>
        /// <param name="expected">The expected XML fragment.</param>
        /// <param name="actual">The actual XML fragment</param>
        public Diff(string path, string message, string expected, string actual)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (message == null)
                throw new ArgumentNullException("message");
            if (expected == null)
                throw new ArgumentNullException("expected");
            if (actual == null)
                throw new ArgumentNullException("actual");

            this.path = path;
            this.message = message;
            this.expected = expected;
            this.actual = actual;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var builder = new StringBuilder(message);

            if (expected.Length > 0)
            {
                builder.AppendFormat(" Expected = '{0}'.", expected);
            }

            if (actual.Length > 0)
            {
                builder.AppendFormat(" Found = '{0}'.", actual);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Returns the diff as an assertion failures.
        /// </summary>
        /// <returns>The resulting assertion failure.</returns>
        public AssertionFailure ToAssertionFailure()
        {
            var builder = new AssertionFailureBuilder(message);
            builder.AddLabeledValue("Path", path);

            if (expected.Length > 0)
            {
                builder.AddRawExpectedValue(expected);
            }

            if (actual.Length > 0)
            {
                builder.AddRawActualValue(actual);
            }

            return builder.ToAssertionFailure();
        }
    }
}
