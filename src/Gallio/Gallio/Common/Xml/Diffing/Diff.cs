// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common.Xml.Paths;
using Gallio.Framework.Assertions;

namespace Gallio.Common.Xml.Diffing
{
    /// <summary>
    /// A diff item representing one single difference between two XML fragments.
    /// </summary>
    public sealed class Diff
    {
        private readonly string message;
        private readonly IXmlPathStrict path;
        private readonly DiffTargets targets;

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
        /// Gets the path of the difference.
        /// </summary>
        public IXmlPathStrict Path
        {
            get
            {
                return path;
            }
        }

        /// <summary>
        /// Indicates which XML fragment is targeted by the diff.
        /// </summary>
        public DiffTargets Targets
        {
            get
            {
                return targets;
            }
        }

        /// <summary>
        /// Constructs a diff item.
        /// </summary>
        /// <param name="message">The message explaining the difference.</param>
        /// <param name="path">The path of the difference.</param>
        /// <param name="targets">Indicates which XML fragment is targeted by the diff.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> or <paramref name="path"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="message"/> or <paramref name="path"/> is empty.</exception>
        public Diff(string message, IXmlPathStrict path, DiffTargets targets)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (path == null)
                throw new ArgumentNullException("path");
            if (message.Length == 0)
                throw new ArgumentException("Cannot be empty.", "message");

            this.message = message;
            this.path = path;
            this.targets = targets;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return String.Format("{0} at '{1}'.", message, path.ToString());
        }

        /// <summary>
        /// Returns the diff as an assertion failure.
        /// </summary>
        /// <param name="expected">The expected fragment used to format the diff.</param>
        /// <param name="actual">The actual fragment used to format the diff.</param>
        /// <returns>The resulting assertion failure.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expected"/> or <paramref name="actual"/> is null.</exception>
        public AssertionFailure ToAssertionFailure(NodeFragment expected, NodeFragment actual)
        {
            bool showActual = ((targets & DiffTargets.Actual) != 0);
            bool showExpected = ((targets & DiffTargets.Expected) != 0);
            var builder = new AssertionFailureBuilder(message);

            if (showActual && showExpected)
            {
                builder.AddRawExpectedAndActualValuesWithDiffs(path.Format(expected), path.Format(actual));
            }
            else if (showActual)
            {
                builder.AddRawActualValue(path.Format(actual));
            }
            else if (showExpected)
            {
                builder.AddRawExpectedValue(path.Format(expected));
            }

            return builder.ToAssertionFailure();
        }
    }
}
