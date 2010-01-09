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
using Gallio.Common.Markup;
using Gallio.Model.Schema;
using Gallio.Runner.Reports.Schema;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that a marker block has been started within a test step log stream.
    /// </summary>
    public sealed class TestStepLogStreamBeginMarkerBlockEventArgs : TestStepLogStreamEventArgs
    {
        private readonly Marker marker;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="test">The test data.</param>
        /// <param name="testStepRun">The test step run.</param>
        /// <param name="logStreamName">The log stream name.</param>
        /// <param name="marker">The marker.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report"/>, <paramref name="test"/>
        /// <paramref name="testStepRun"/> or <paramref name="logStreamName"/> is null.</exception>
        public TestStepLogStreamBeginMarkerBlockEventArgs(Report report, TestData test, TestStepRun testStepRun, string logStreamName, Marker marker)
            : base(report, test, testStepRun, logStreamName)
        {
            this.marker = marker;
        }

        /// <summary>
        /// Gets the marker.
        /// </summary>
        public Marker Marker
        {
            get { return marker; }
        }
    }
}
