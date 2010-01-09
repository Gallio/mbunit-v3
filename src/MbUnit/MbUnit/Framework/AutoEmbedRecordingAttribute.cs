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
using Gallio.Common;
using Gallio.Common.Media;
using Gallio.Framework;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Common.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Decorates a test method and causes a video of the test run to be automatically embeded when a trigger event occurs.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If screenshots cannot be captured, the attribute will embed a warning message to that effect.
    /// </para>
    /// </remarks>
    /// <seealso cref="AutoEmbedScreenshotAttribute"/>
    /// <seealso cref="Capture.AutoEmbedRecording(TriggerEvent, string)"/>
    /// <seealso cref="Capture.AutoEmbedRecording(TriggerEvent, string, CaptureParameters, double)"/>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = true, Inherited = true)]
    public class AutoEmbedRecordingAttribute : AutoEmbedScreenshotAttribute
    {
        /// <summary>
        /// Gets or sets the number of frames per second to capture (default is 5).
        /// </summary>
        public double FramesPerSecond
        {
            get; 
            set;
        }

        /// <summary>
        /// Automatically embeds a video of the test run from this point forward when a trigger event occurs.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Recording a screen capture video can be very CPU and space intensive particularly
        /// when running tests on a single-core CPU.  We recommend specifying a zoom with of 0.25 or 
        /// less and a frame rate  of no more than 5 to 10 frames per second.
        /// </para>
        /// <para>
        /// If screenshots cannot be captured, the method will embed a warning message to that effect.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// [Test]
        /// [AutoEmbedRecording(TriggerEvent.TestPassed, Zoom = 0.25, FramesPerSecond = 3)]
        /// public void Test()
        /// {
        ///     // Code logic here.
        /// }
        /// ]]></code>
        /// </example>
        /// <param name="triggerEvent">The trigger event.</param>
        public AutoEmbedRecordingAttribute(TriggerEvent triggerEvent)
            : base(triggerEvent)
        {
        }

        /// <inheritdoc />
        protected override void DecorateTest(IPatternScope scope, ICodeElementInfo codeElement)
        {
            scope.TestBuilder.TestInstanceActions.SetUpTestInstanceChain.Before(state =>
                Capture.AutoEmbedRecording(TriggerEvent, AttachmentName, Parameters, FramesPerSecond));
        }
    }
}
